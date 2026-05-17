using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Dal.Repositories;

public class AgendaItemRepository : IAgendaItemRepository
{
    private readonly ApplicationDbContext _context;

    public AgendaItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AgendaItem?> GetByIdAsync(Guid id)
        => await _context.AgendaItems
            .Include(a => a.Session)
                .ThenInclude(s => s!.SessionRegistrations)
                    .ThenInclude(r => r.User)
            .Include(a => a.Room)
            .FirstOrDefaultAsync(a => a.AgendaItemId == id);

    public async Task<IEnumerable<AgendaItem>> GetByConferenceIdAsync(Guid conferenceId)
        => await _context.AgendaItems
            .Where(a => a.ConferenceId == conferenceId)
            .Include(a => a.Session)
                .ThenInclude(s => s!.SessionRegistrations)
                    .ThenInclude(r => r.User)
            .Include(a => a.Room)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

    public async Task AddAsync(AgendaItem agendaItem)
        => await _context.AgendaItems.AddAsync(agendaItem);

    public Task UpdateAsync(AgendaItem agendaItem)
    {
        _context.AgendaItems.Update(agendaItem);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AgendaItem agendaItem)
    {
        _context.AgendaItems.Remove(agendaItem);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
