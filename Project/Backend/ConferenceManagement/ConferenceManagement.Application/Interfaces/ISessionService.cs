using ConferenceManagement.Application.DTOs;
using ConferenceManagement.Application.DTOs.Session;

namespace ConferenceManagement.Application.Interfaces;

public interface ISessionService
{
    /// <summary>
    /// Kreira novu sesiju uz provjeru validnosti podataka i preklapanja termina.
    /// </summary>
    /// <param name="dto">Podaci za kreiranje sesije.</param>
    /// <returns>Guid kreirane sesije ili null ako kreiranje nije uspjelo (npr. termin zauzet).</returns>
    Task<Guid?> CreateSessionAsync(CreateSessionDto dto);
    Task<bool> UpdateSessionAsync(Guid id, UpdateSessionDto dto);
    Task<bool> DeleteSessionAsync(Guid id);
    Task<bool> AssignSpeakerAsync(Guid sessionId, Guid userId);
    Task RegisterAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task CancelRegistrationAsync(Guid registrationId, CancellationToken cancellationToken = default);
    Task<List<SessionListDTO>> GetRegisteredForCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<List<SessionListDTO>> GetSessionsForConferenceAsync(Guid conferenceId);
}