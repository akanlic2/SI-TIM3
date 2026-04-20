using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class ConferenceService : IConferenceService
{
    private readonly IConferenceRepository _conferenceRepository;

    public ConferenceService(IConferenceRepository conferenceRepository)
    {
        _conferenceRepository = conferenceRepository;
    }

    public async Task<List<ConferenceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var conferences = await _conferenceRepository.GetAllAsync(cancellationToken);

        return conferences.Select(MapToDto).ToList();
    }

    public async Task<ConferenceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(id, cancellationToken);

        if (conference is null)
        {
            return null;
        }

        return MapToDto(conference);
    }

    public async Task<ConferenceDto> CreateAsync(CreateConferenceDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.StartDate > dto.EndDate)
        {
            throw new ArgumentException("Start date cannot be after end date.");
        }

        if (dto.MaxParticipants <= 0)
        {
            throw new ArgumentException("Max participants must be greater than 0.");
        }

        var conference = new Conference
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Location = dto.Location,
            Category = dto.Category,
            MaxParticipants = dto.MaxParticipants,
            Status = "Planned"
        };

        var createdConference = await _conferenceRepository.AddAsync(conference, cancellationToken);

        return MapToDto(createdConference);
    }

    private static ConferenceDto MapToDto(Conference conference)
    {
        return new ConferenceDto
        {
            ConferenceId = conference.ConferenceId,
            Title = conference.Title,
            Description = conference.Description,
            StartDate = conference.StartDate,
            EndDate = conference.EndDate,
            Location = conference.Location,
            Category = conference.Category,
            MaxParticipants = conference.MaxParticipants,
            Status = conference.Status
        };
    }
}