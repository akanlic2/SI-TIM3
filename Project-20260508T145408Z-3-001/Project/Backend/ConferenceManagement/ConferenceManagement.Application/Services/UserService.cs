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
        Task<UserDto?> GetUserByUsernameOrEmailAndPasswordAsync(string usernameOrEmail, string password);
        Task<UserDto> RegisterUserAsync(RegisterUserDto dto);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
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

        public async Task<UserDto?> GetUserByUsernameOrEmailAndPasswordAsync(string usernameOrEmail, string password)
        {
            var normalized = usernameOrEmail.Trim().ToLower();

            return await _dbContext.Users
                .Where(u =>
                    (u.Username.ToLower() == normalized || u.Email.ToLower() == normalized) &&
                    u.Password == password)
                .Select(MapToDto)
                .SingleOrDefaultAsync();
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
        {
            var role = string.IsNullOrWhiteSpace(dto.Role) ? "ucesnik" : dto.Role.Trim().ToLower();

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username.Trim(),
                Password = dto.Password,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public Task<bool> UsernameExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Task.FromResult(false);
            }

            var normalized = username.Trim().ToLower();
            return _dbContext.Users.AnyAsync(u => u.Username.ToLower() == normalized);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Task.FromResult(false);
            }

            var normalized = email.Trim().ToLower();
            return _dbContext.Users.AnyAsync(u => u.Email.ToLower() == normalized);
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

            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                user.Username = dto.Username;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.Password = dto.Password;
            }

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = dto.Role;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static Expression<Func<User, UserDto>> MapToDto = user => new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
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
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}

