using ConferenceManagement.Application.DTOs.Conference;

namespace ConferenceManagement.Application.Interfaces;

public interface IConferenceService
{
    Task<List<ConferenceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ConferenceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ConferenceDto> CreateAsync(CreateConferenceDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateConferenceDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}