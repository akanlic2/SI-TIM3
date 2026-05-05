using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface IConferenceRepository : IGenericRepository<Conference>
{
    
    new Task<Conference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<Conference>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
}