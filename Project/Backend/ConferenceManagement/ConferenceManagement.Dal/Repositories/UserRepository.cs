using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.CountAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameOrEmailAndPasswordAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        var normalized = usernameOrEmail.Trim().ToLower();

        return await _dbContext.Users
            .FirstOrDefaultAsync(u =>
                (u.Username.ToLower() == normalized || u.Email.ToLower() == normalized) &&
                u.Password == password, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AnyByUsernameAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        var normalized = username.Trim().ToLower();
        return await _dbContext.Users.AnyAsync(u => u.Username.ToLower() == normalized && u.UserId != excludeUserId, cancellationToken);
    }

    public async Task<bool> AnyByEmailAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var normalized = email.Trim().ToLower();
        return await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == normalized && u.UserId != excludeUserId, cancellationToken);
    }
}
