using ConferenceManagement.Domain.Entities;
using ConferenceManagement.Dal;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ConferenceManagement.Application.Services
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
        Task<List<UserDto>> GetAllUsersAsync();
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
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    KeycloakUserId = u.KeycloakUserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }
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
}

