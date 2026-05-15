using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;

namespace ConferenceManagement.Application.Services;

public class ConferenceCapacityService : IConferenceCapacityService
{
    private readonly IConferenceRepository _conferenceRepository;
    private readonly IConferenceRegistrationRepository _conferenceRegistrationRepository;
    private readonly ISessionRepository _sessionRepository;

    public ConferenceCapacityService(
        IConferenceRepository conferenceRepository,
        IConferenceRegistrationRepository conferenceRegistrationRepository,
        ISessionRepository sessionRepository)
    {
        _conferenceRepository = conferenceRepository;
        _conferenceRegistrationRepository = conferenceRegistrationRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<CapacityDto> GetConferenceCapacityAsync(Guid conferenceId, CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(conferenceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Konferencija sa ID-jem {conferenceId} nije pronađena.");

        var registeredCount = await _conferenceRegistrationRepository
            .GetConfirmedCountForConferenceAsync(conferenceId, cancellationToken);

        return new CapacityDto
        {
            MaxParticipants = conference.MaxParticipants,
            RegisteredCount = registeredCount,
            AvailableSpots = conference.MaxParticipants - registeredCount,
            IsFull = registeredCount >= conference.MaxParticipants
        };
    }

    public async Task<CapacityDto> GetSessionCapacityAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdWithRegistrationsAsync(sessionId)
            ?? throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");

        var maxParticipants = session.Conference.MaxParticipants;
        var registeredCount = session.SessionRegistrations
            .Count(r => r.RegistrationStatus.ToLower() == "confirmed");

        return new CapacityDto
        {
            MaxParticipants = maxParticipants,
            RegisteredCount = registeredCount,
            AvailableSpots = maxParticipants - registeredCount,
            IsFull = registeredCount >= maxParticipants
        };
    }

    public async Task<List<ParticipantDto>> GetConferenceParticipantsAsync(
        Guid conferenceId, string? search, string? status,
        CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(conferenceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Konferencija sa ID-jem {conferenceId} nije pronađena.");

        var registrations = await _conferenceRegistrationRepository
            .GetRegistrationsByConferenceAsync(conferenceId, cancellationToken);

        var query = registrations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(r =>
                r.User.FirstName.ToLower().Contains(normalized) ||
                r.User.LastName.ToLower().Contains(normalized) ||
                r.User.Email.ToLower().Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = status.Trim().ToLower();
            query = query.Where(r => r.RegistrationStatus.ToLower() == normalized);
        }

        return query.Select(r => new ParticipantDto
        {
            UserId = r.UserId,
            FirstName = r.User.FirstName,
            LastName = r.User.LastName,
            Email = r.User.Email,
            RegistrationStatus = r.RegistrationStatus,
            RegistrationDate = r.RegistrationDate
        }).ToList();
    }
}