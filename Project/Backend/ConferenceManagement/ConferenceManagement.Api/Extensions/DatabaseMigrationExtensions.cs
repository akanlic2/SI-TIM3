using ConferenceManagement.Dal;
using ConferenceManagement.Dal.Seeders;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Api.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task WaitForDatabaseAndApplyMigrationsAsync(
        this IServiceProvider services,
        ILogger logger,
        int maxRetries = 12,
        int delaySeconds = 5,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation(
                    "Database migration attempt {Attempt}/{MaxRetries}...",
                    attempt,
                    maxRetries);

                var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
                if (!canConnect)
                {
                    throw new InvalidOperationException("Database is not reachable yet.");
                }

                await dbContext.Database.MigrateAsync(cancellationToken);

                // Velika izmjena: zaštita za slučaj da je migraciona historija nekonzistentna
                // (npr. stara baza bez username/password kolona iako je migracija evidentirana).
                await dbContext.Database.ExecuteSqlRawAsync(
                    "DROP INDEX IF EXISTS ix_users_keycloak_user_id;",
                    cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE users DROP COLUMN IF EXISTS keycloak_user_id;",
                    cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE users ADD COLUMN IF NOT EXISTS username character varying(100);",
                    cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE users ADD COLUMN IF NOT EXISTS password character varying(255) NOT NULL DEFAULT '';",
                    cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE users SET username = split_part(email, '@', 1) || '_' || substring(user_id::text, 1, 8) WHERE username IS NULL OR username = '';",
                    cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync(
                    "ALTER TABLE users ALTER COLUMN username SET NOT NULL;",
                    cancellationToken);
                await dbContext.Database.ExecuteSqlRawAsync(
                    "CREATE UNIQUE INDEX IF NOT EXISTS ix_users_username ON users (username);",
                    cancellationToken);

                // Seed default users
                await UserSeeder.SeedUsersAsync(dbContext, cancellationToken);
                // Seed deafult rooms
                await RoomSeeder.SeedRoomsAsync(dbContext, cancellationToken);

                logger.LogInformation("Database is ready and migrations were applied.");
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Database is not ready on attempt {Attempt}/{MaxRetries}. Retrying in {DelaySeconds} seconds...",
                    attempt,
                    maxRetries,
                    delaySeconds);

                if (attempt == maxRetries)
                {
                    logger.LogError("Maximum retry count reached. Migration failed.");
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
            }
        }
    }
}