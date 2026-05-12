using ConferenceManagement.Application.DTOs;
using ConferenceManagement.Application.DTOs.Session;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
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
        // 1. Provjera sesije
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null) return false;

        // 2. Provjera korisnika i role 
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || user.Role != "predavac") return false;

        // 3. Provjera da li već postoji zapis u veznoj tabeli (session_registrations)
        var existingReg = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);

        if (existingReg != null)
        {
            // Ako je već bio registrovan, samo ga postavi za predavača
            existingReg.IsSpeaker = true;
            await _registrationRepository.UpdateAsync(existingReg);
        }
        else
        {
            // Ako ne postoji, kreiraj novi zapis prema tvom ER dijagramu
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
        {
            throw new KeyNotFoundException($"Sesija sa ID-jem {sessionId} nije pronađena.");
        }

        var userId = Guid.Parse(_userContextService.GetUserId());
        var existingRegistration = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);

        if (existingRegistration != null && existingRegistration.RegistrationStatus == "Confirmed")
        {
            throw new InvalidOperationException("Korisnik je već prijavljen na sesiju.");
        }

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
        {
            throw new KeyNotFoundException($"Prijava sa ID-jem {registrationId} nije pronađena.");
        }

        var userId = Guid.Parse(_userContextService.GetUserId());
        if (registration.UserId != userId)
        {
            throw new UnauthorizedAccessException("Nemate pravo otkazati ovu prijavu.");
        }

        registration.RegistrationStatus = "Cancelled";
        await _registrationRepository.UpdateAsync(registration);
        await _registrationRepository.SaveChangesAsync();
    }

    public async Task<List<SessionListDTO>> GetRegisteredForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
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

        // Ako nema sesija, vraćamo praznu listu [], što je "prazno stanje"
        return sessions.Select(MapToSessionListDto).ToList();
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