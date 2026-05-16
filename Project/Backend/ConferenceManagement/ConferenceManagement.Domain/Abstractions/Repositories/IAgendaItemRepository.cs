using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Domain.Abstractions.Repositories;

public interface IAgendaItemRepository
{
    Task<AgendaItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<AgendaItem>> GetByConferenceIdAsync(Guid conferenceId);
    Task AddAsync(AgendaItem agendaItem);
    Task UpdateAsync(AgendaItem agendaItem);
    Task DeleteAsync(AgendaItem agendaItem);
    Task SaveChangesAsync();
}
