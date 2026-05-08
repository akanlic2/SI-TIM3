using System.Threading.Tasks;

namespace ConferenceManagement.Application.Services
{
    public interface IKeycloakService
    {
        Task DeleteUser(string keycloakUserId);
        Task LogoutUser(string token);
        Task<string> CreateUserAsync(string username, string email, string password, IEnumerable<string>? roles = null);
        Task AssignRealmRolesAsync(string userId, IEnumerable<string> roles, string? adminToken = null);
    }
}
