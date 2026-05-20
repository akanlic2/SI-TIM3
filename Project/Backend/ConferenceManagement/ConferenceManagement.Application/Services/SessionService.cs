using ConferenceManagement.Application.DTOs;
using ConferenceManagement.Application.DTOs.Session;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISessionRegistrationRepository _registrationRepository;
    private readonly IUserContextService _userContextService;

    public SessionService(
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        ISessionRegistrationRepository registrationRepository,
        IUserContextService userContextService)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _registrationRepository = registrationRepository;
        _userContextService = userContextService;
    }

    public async Task<Guid?> CreateSessionAsync(CreateSessionDto dto)
    {
        if (dto.EndTime <= dto.StartTime) return null;

        bool isOverlapping = await _sessionRepository.CheckOverlapAsync(dto.RoomId, dto.StartTime, dto.EndTime);
        if (isOverlapping) return null;

        var session = new Session
        {
            SessionId = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            ConferenceId = dto.ConferenceId,
            RoomId = dto.RoomId,
            SessionType = dto.SessionType
        };

        await _sessionRepository.AddAsync(session);
        await _sessionRepository.SaveChangesAsync();
        return session.SessionId;
    }

    public async Task<bool> UpdateSessionAsync(Guid id, UpdateSessionDto dto)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null) return false;

        if (dto.EndTime <= dto.StartTime) return false;

        bool isOverlapping = await _sessionRepository.CheckOverlapAsync(dto.RoomId, dto.StartTime, dto.EndTime, id);
        if (isOverlapping) return false;

        session.Title = dto.Title;
        session.Description = dto.Description;
        session.StartTime = dto.StartTime;
        session.EndTime = dto.EndTime;
        session.RoomId = dto.RoomId;
        session.SessionType = dto.SessionType;

        await _sessionRepository.UpdateAsync(session);
        await _sessionRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteSessionAsync(Guid id)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null) return false;

        await _sessionRepository.DeleteAsync(session);
        await _sessionRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignSpeakerAsync(Guid sessionId, Guid userId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null) return false;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Role != "predavac") return false;

        var existingReg = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);

        if (existingReg != null)
        {
            existingReg.IsSpeaker = true;
            await _registrationRepository.UpdateAsync(existingReg);
        }
        else
        {
            var newReg = new SessionRegistration
            {
                SessionRegistrationId = Guid.NewGuid(),
                SessionId = sessionId,
                UserId = userId,
                IsSpeaker = true,
                RegistrationDate = DateTime.UtcNow,
                RegistrationStatus = "Confirmed"
            };
            await _registrationRepository.AddAsync(newReg);
        }

        await _registrationRepository.SaveChangesAsync();
        return true;
    }

    public async Task RegisterAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
            throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");

        var userId = Guid.Parse(_userContextService.GetUserId());
        var existingRegistration = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);

        if (existingRegistration != null && existingRegistration.RegistrationStatus == "Confirmed")
            throw new InvalidOperationException("Korisnik je već prijavljen na sesiju.");

        if (existingRegistration != null && existingRegistration.RegistrationStatus == "Otkazano")
        {
            existingRegistration.RegistrationStatus = "Confirmed";
            existingRegistration.RegistrationDate = DateTime.UtcNow;
            await _registrationRepository.UpdateAsync(existingRegistration);
            await _registrationRepository.SaveChangesAsync();
            return;
        }

        var registration = new SessionRegistration
        {
            SessionRegistrationId = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = userId,
            RegistrationDate = DateTime.UtcNow,
            RegistrationStatus = "Confirmed",
            IsSpeaker = false
        };

        await _registrationRepository.AddAsync(registration);
        await _registrationRepository.SaveChangesAsync();
    }

    public async Task CancelRegistrationAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        var registration = await _registrationRepository.GetByIdAsync(registrationId);
        if (registration == null)
            throw new KeyNotFoundException($"Prijava sa ID-jem {registrationId} nije pronađena.");

        var userId = Guid.Parse(_userContextService.GetUserId());
        if (registration.UserId != userId)
            throw new UnauthorizedAccessException("Nemate pravo otkazati ovu prijavu.");

        registration.RegistrationStatus = "Cancelled";
        await _registrationRepository.UpdateAsync(registration);
        await _registrationRepository.SaveChangesAsync();
    }

    public async Task<List<SessionListDTO>> GetRegisteredForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var registrations = await _registrationRepository.GetConfirmedRegistrationsForUserAsync(userId, cancellationToken);

        return registrations.Select(r =>
        {
            var dto = MapToSessionListDto(r.Session);
            dto.SessionRegistrationId = r.SessionRegistrationId;
            return dto;
        }).ToList();
    }

    public async Task<List<SessionListDTO>> GetSessionsForConferenceAsync(Guid conferenceId)
    {
        var sessions = await _sessionRepository.GetSessionsByConferenceIdAsync(conferenceId);
        return sessions.Select(MapToSessionListDto).ToList();
    }

    // --- S43 PREDAVAC DASHBOARD DODACI ---

    public async Task<List<SpeakerSessionListDto>> GetSessionsForCurrentSpeakerAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var sessions = await _sessionRepository.GetSessionsBySpeakerIdAsync(userId, cancellationToken);

        return sessions.Select(s => new SpeakerSessionListDto
        {
            SessionId = s.SessionId,
            Title = s.Title,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            SessionType = s.SessionType,
            ConferenceId = s.ConferenceId,
            ConferenceTitle = s.Conference?.Title ?? "N/A",
            Location = s.Conference?.Location ?? "N/A"
        }).ToList();
    }

    public async Task<SpeakerSessionDetailsDto> GetSpeakerSessionDetailsAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());

        // Koristimo postojeću metodu repozitorija koja uključuje registracije i konferenciju
        var session = await _sessionRepository.GetByIdWithRegistrationsAsync(sessionId);

        if (session == null)
            throw new KeyNotFoundException("Sesija nije pronađena.");

        // GUARD (S43 - BE Role): Provjera da li je ulogovani korisnik zaista predavač na ovoj sesiji
        var isSpeaker = session.SessionRegistrations.Any(r => r.UserId == userId && r.IsSpeaker);
        if (!isSpeaker)
            throw new UnauthorizedAccessException("Nemate dozvolu za pristup detaljima ove sesije.");

        return new SpeakerSessionDetailsDto
        {
            SessionId = session.SessionId,
            Title = session.Title,
            Description = session.Description,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            SessionType = session.SessionType,
            ConferenceId = session.ConferenceId,
            ConferenceTitle = session.Conference?.Title ?? "N/A",
            Location = session.Conference?.Location ?? "N/A",
            RoomName = session.Room?.Name ?? "N/A",
            Attendees = session.SessionRegistrations
                .Where(r => !r.IsSpeaker && r.RegistrationStatus == "Confirmed")
                .Select(r => new SessionAttendeeDto
                {
                    UserId = r.UserId,
                    FirstName = r.User?.FirstName ?? "N/A",
                    LastName = r.User?.LastName ?? "N/A",
                    Email = r.User?.Email ?? "N/A",
                    RegistrationDate = r.RegistrationDate
                }).ToList()
        };
    }

    private static SessionListDTO MapToSessionListDto(Session session)
    {
        return new SessionListDTO
        {
            SessionId = session.SessionId,
            Title = session.Title,
            Description = session.Description,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            SessionType = session.SessionType,
            Status = session.Status,
            RoomId = session.RoomId,
            RoomName = session.Room?.Name,
            AssignedSpeakerId = session.SessionRegistrations
                .FirstOrDefault(r => r.IsSpeaker)?.UserId,
            SpeakerName = session.SessionRegistrations
                .FirstOrDefault(r => r.IsSpeaker)?.User != null
                ? $"{session.SessionRegistrations.First(r => r.IsSpeaker).User.FirstName} {session.SessionRegistrations.First(r => r.IsSpeaker).User.LastName}"
                : null
        };
    }
}