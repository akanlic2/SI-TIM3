using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Session?> GetByIdAsync(Guid id) => await _context.Sessions.FindAsync(id);

    public async Task AddAsync(Session session) => await _context.Sessions.AddAsync(session);

    public async Task<bool> CheckOverlapAsync(Guid roomId, DateTime start, DateTime end, Guid? excludeSessionId = null)
    {
        return await _context.Sessions.AnyAsync(s =>
            s.RoomId == roomId &&
            s.SessionId != excludeSessionId &&
            start < s.EndTime &&
            end > s.StartTime);
    }

    public async Task UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Session session)
    {
        _context.Sessions.Remove(session);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Session>> GetSessionsByConferenceIdAsync(Guid conferenceId)
    {
        return await _context.Sessions
            .Where(s => s.ConferenceId == conferenceId)
            .Include(s => s.Room)
            .Include(s => s.SessionRegistrations)
                .ThenInclude(r => r.User)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task<Session?> GetByIdWithRegistrationsAsync(Guid id) =>
     await _context.Sessions
         .Include(s => s.Room)
         .Include(s => s.Conference)
         .Include(s => s.SessionRegistrations)
             .ThenInclude(r => r.User)
         .FirstOrDefaultAsync(s => s.SessionId == id);


    // --- IMPLEMENTACIJA ZA S43 PREDAVAC DASHBOARD ---
    public async Task<List<Session>> GetSessionsBySpeakerIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .Include(s => s.Conference)
            .Include(s => s.Room)
            .Where(s => s.SessionRegistrations
                .Any(sr => sr.UserId == userId && sr.IsSpeaker))
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }
    public async Task<List<Session>> GetSessionsByConferenceIdWithDetailsAsync(
    Guid conferenceId, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .Where(s => s.ConferenceId == conferenceId)
            .Include(s => s.Room)
            .Include(s => s.SessionRegistrations)
            .Include(s => s.Materials)
            .OrderBy(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }
}