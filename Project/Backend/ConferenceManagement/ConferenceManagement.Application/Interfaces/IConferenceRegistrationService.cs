using ConferenceManagement.Application.DTOs.Conference;

namespace ConferenceManagement.Application.Interfaces;

public interface IConferenceRegistrationService
{
    Task RegisterAsync(Guid conferenceId, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid registrationId, CancellationToken cancellationToken = default);
    Task<List<ConferenceRegistrationUserDto>> GetRegistrationsByConferenceAsync(Guid conferenceId, CancellationToken cancellationToken = default);
}
