using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface IMaterialRepository
{
    Task AddAsync(Material material, CancellationToken cancellationToken);
    Task<List<Material>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}