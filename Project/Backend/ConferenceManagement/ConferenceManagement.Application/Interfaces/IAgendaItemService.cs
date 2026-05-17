using ConferenceManagement.Application.DTOs.Agenda;

namespace ConferenceManagement.Application.Interfaces;

public interface IAgendaItemService
{
    /// <summary>
    /// Vraća sve agenda stavke konferencije sortirane po StartTime.
    /// </summary>
    Task<List<AgendaItemDto>> GetByConferenceIdAsync(Guid conferenceId);

    /// <summary>
    /// Kreira novu agenda stavku uz validaciju tipa i vremena.
    /// </summary>
    Task<AgendaItemDto> CreateAsync(Guid conferenceId, CreateAgendaItemDto dto);

    /// <summary>
    /// Ažurira postojeću agenda stavku.
    /// </summary>
    Task UpdateAsync(Guid agendaItemId, UpdateAgendaItemDto dto);

    /// <summary>
    /// Briše agenda stavku po ID-u.
    /// </summary>
    Task DeleteAsync(Guid agendaItemId);
}
