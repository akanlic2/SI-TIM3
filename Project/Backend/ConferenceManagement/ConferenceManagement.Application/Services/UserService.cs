using ConferenceManagement.Application.DTOs.User;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _userRepository.GetCountAsync();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByUsernameOrEmailAndPasswordAsync(string usernameOrEmail, string password)
        {
            var user = await _userRepository.GetByUsernameOrEmailAndPasswordAsync(usernameOrEmail, password);
            return user != null ? MapToDto(user) : null;
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

            var createdUser = await _userRepository.AddAsync(user);

            return MapToDto(createdUser);
        }

        public async Task<bool> UsernameExistsAsync(string username, Guid? userId = null)
        {
            return await _userRepository.AnyByUsernameAsync(username, userId);
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? userId = null)
        {
            return await _userRepository.AnyByEmailAsync(email, userId);
        }

        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

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

            await _userRepository.UpdateAsync(user);
            return true;
        }

        private static UserDto MapToDto(User user)
        {
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
    }
}
