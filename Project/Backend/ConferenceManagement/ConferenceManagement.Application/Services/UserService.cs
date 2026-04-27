using ConferenceManagement.Domain.Entities;
using ConferenceManagement.Dal;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ConferenceManagement.Application.Services
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
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
    }
}
