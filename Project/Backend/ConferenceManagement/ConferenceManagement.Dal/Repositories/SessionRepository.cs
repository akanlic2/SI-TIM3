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
            .OrderBy(s => s.StartTime) // Sortiramo po vremenu početka
            .ToListAsync();
    }
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}