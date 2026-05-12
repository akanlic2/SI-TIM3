using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface ISessionRegistrationRepository
{
    Task AddAsync(SessionRegistration registration);
    Task<SessionRegistration?> GetBySessionAndUserAsync(Guid sessionId, Guid userId);
    Task<SessionRegistration?> GetByIdAsync(Guid registrationId);
    Task<List<SessionRegistration>> GetConfirmedRegistrationsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(SessionRegistration registration);
    Task UpdateAsync(SessionRegistration registration);
    Task SaveChangesAsync();
}