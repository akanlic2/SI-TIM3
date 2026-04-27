using System.Threading.Tasks;

namespace ConferenceManagement.Application.Services
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
    }
}
