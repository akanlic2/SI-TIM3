using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id);
    Task AddAsync(Session session);
    Task UpdateAsync(Session session);
    Task DeleteAsync(Session session);
    Task SaveChangesAsync();
    Task<IEnumerable<Session>> GetSessionsByConferenceIdAsync(Guid conferenceId);
    Task<bool> CheckOverlapAsync(Guid roomId, DateTime start, DateTime end, Guid? excludeSessionId = null);
    Task<Session?> GetByIdWithRegistrationsAsync(Guid id);

    /// <summary>
    /// Dobavlja sve sesije za određenog predavača uključujući podatke o konferenciji.
    /// </summary>
    Task<List<Session>> GetSessionsBySpeakerIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Session>> GetSessionsByConferenceIdWithDetailsAsync(
    Guid conferenceId, CancellationToken cancellationToken = default);
}