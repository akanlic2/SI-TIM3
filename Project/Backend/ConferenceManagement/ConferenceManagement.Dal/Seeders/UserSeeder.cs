using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Seeders;

public static class UserSeeder
{
    public static async Task SeedUsersAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        var seedUsers = new List<User>
        {
            new User
            {
                UserId = Guid.NewGuid(),
                Username = "Administrator",
                Password = "Admin123", // TODO: In production, hash passwords
                FirstName = "Ajdin",
                LastName = "Kanlic",
                Email = "administrator@gmail.com",
                Role = "admin",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                UserId = Guid.NewGuid(),
                Username = "Organizator",
                Password = "Org123",
                FirstName = "Nejra",
                LastName = "Hodzic",
                Email = "organizator@gmail.com",
                Role = "organizator",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                UserId = Guid.NewGuid(),
                Username = "Predavac",
                Password = "Pred123",
                FirstName = "Hamza",
                LastName = "Kovac",
                Email = "predavac@gmail.com",
                Role = "predavac",
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                UserId = Guid.NewGuid(),
                Username = "Ucesnik",
                Password = "Uces123",
                FirstName = "Emira",
                LastName = "Kurtovic",
                Email = "ucesnik@gmail.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            }
        };

        var usersToAdd = new List<User>();

        foreach (var user in seedUsers)
        {
            var userExists = await context.Users
                .Where(u => u.Email == user.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (userExists == null)
            {
                usersToAdd.Add(user);
            }
        }

        if (usersToAdd.Count > 0)
        {
            await context.Users.AddRangeAsync(usersToAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
