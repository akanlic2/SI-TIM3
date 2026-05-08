using ConferenceManagement.Application.DTOs.User;

namespace ConferenceManagement.Application.Interfaces;

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
