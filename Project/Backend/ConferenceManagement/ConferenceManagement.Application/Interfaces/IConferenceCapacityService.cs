using ConferenceManagement.Application.DTOs.Conference;

namespace ConferenceManagement.Application.Interfaces;

public interface IConferenceCapacityService
{
    Task<CapacityDto> GetConferenceCapacityAsync(Guid conferenceId, CancellationToken cancellationToken = default);
    Task<CapacityDto> GetSessionCapacityAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<ParticipantDto>> GetConferenceParticipantsAsync(Guid conferenceId, string? search, string? status, CancellationToken cancellationToken = default);
}