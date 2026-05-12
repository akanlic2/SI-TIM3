using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface IConferenceRegistrationRepository
{
    Task AddAsync(ConferenceRegistration registration, CancellationToken cancellationToken = default);
    Task<ConferenceRegistration?> GetByConferenceAndUserAsync(Guid conferenceId, Guid userId, CancellationToken cancellationToken = default);
    Task<ConferenceRegistration?> GetByIdAsync(Guid registrationId, CancellationToken cancellationToken = default);
    Task<List<ConferenceRegistration>> GetConfirmedRegistrationsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<ConferenceRegistration>> GetRegistrationsByConferenceAsync(Guid conferenceId, CancellationToken cancellationToken = default);
    Task<int> GetConfirmedCountForConferenceAsync(Guid conferenceId, CancellationToken cancellationToken = default);
    Task UpdateAsync(ConferenceRegistration registration, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
