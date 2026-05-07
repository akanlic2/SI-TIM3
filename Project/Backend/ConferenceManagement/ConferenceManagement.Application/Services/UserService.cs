using ConferenceManagement.Dal;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConferenceManagement.Application.Services
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto dto);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _dbContext.Users.CountAsync();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .Select(MapToDto)
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext.Users
                .Where(u => u.UserId == userId)
                .Select(MapToDto)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user is null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
            {
                user.FirstName = dto.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(dto.LastName))
            {
                user.LastName = dto.LastName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user.Email = dto.Email;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static Expression<Func<User, UserDto>> MapToDto = user => new UserDto
        {
            UserId = user.UserId,
            KeycloakUserId = user.KeycloakUserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public class UserDto
    {
        public Guid UserId { get; set; }
        public string KeycloakUserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}

