using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories
{
	public interface IUserRepository : IGenericRepository<User>
	{
		Task<int> GetUserCountAsync();
	}
}