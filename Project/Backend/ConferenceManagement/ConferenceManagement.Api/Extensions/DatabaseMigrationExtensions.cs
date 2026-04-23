using ConferenceManagement.Dal;
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