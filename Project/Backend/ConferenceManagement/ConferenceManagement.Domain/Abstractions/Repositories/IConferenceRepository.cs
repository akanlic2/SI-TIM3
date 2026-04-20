using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface IConferenceRepository: IGenericRepository<Conference>
{
    Task<List<Conference>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
}