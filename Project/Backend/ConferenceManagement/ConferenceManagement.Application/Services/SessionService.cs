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

    public SessionService(
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        ISessionRegistrationRepository registrationRepository)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _registrationRepository = registrationRepository;
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

    public async Task<List<SessionListDTO>> GetSessionsForConferenceAsync(Guid conferenceId)
    {
        var sessions = await _sessionRepository.GetSessionsByConferenceIdAsync(conferenceId);

        // Ako nema sesija, vraćamo praznu listu [], što je "prazno stanje"
        return sessions.Select(s => new SessionListDTO
        {
            SessionId = s.SessionId,
            Title = s.Title,
            Description = s.Description,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            SessionType = s.SessionType,
            Status = s.Status,
            RoomId = s.RoomId,
            RoomName = s.Room?.Name,
            AssignedSpeakerId = s.SessionRegistrations
                .FirstOrDefault(r => r.IsSpeaker)?.UserId,
            SpeakerName = s.SessionRegistrations
                .FirstOrDefault(r => r.IsSpeaker)?.User != null
                ? $"{s.SessionRegistrations.First(r => r.IsSpeaker).User.FirstName} {s.SessionRegistrations.First(r => r.IsSpeaker).User.LastName}"
                : null
        }).ToList();
    }
}