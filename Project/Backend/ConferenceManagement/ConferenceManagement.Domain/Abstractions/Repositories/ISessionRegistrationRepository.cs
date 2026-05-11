using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface ISessionRegistrationRepository
{
    Task AddAsync(SessionRegistration registration);
    Task<SessionRegistration?> GetBySessionAndUserAsync(Guid sessionId, Guid userId);
    Task UpdateAsync(SessionRegistration registration);
    Task SaveChangesAsync();
}