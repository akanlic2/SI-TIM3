using ConferenceManagement.Application.DTOs.Common;
using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class ConferenceService : IConferenceService
{
    private readonly IConferenceRepository _conferenceRepository;
    private readonly IConferenceRegistrationRepository _conferenceRegistrationRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository; //novo
    private readonly INotificationService _notificationService;

    public ConferenceService(
        IConferenceRepository conferenceRepository,
        IConferenceRegistrationRepository conferenceRegistrationRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        INotificationService notificationService) // novo
    {
        _conferenceRepository = conferenceRepository;
        _conferenceRegistrationRepository = conferenceRegistrationRepository;
        _userContextService = userContextService;
        _userRepository = userRepository; // novo
        _notificationService = notificationService;
    }

    public async Task<List<ConferenceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var conferences = await _conferenceRepository.GetAllAsync(cancellationToken);

        return conferences.Select(MapToDto).ToList();
    }

    public async Task<PagedResultDto<ConferenceDto>> GetPagedAsync(
        ConferenceQueryDto query,
        CancellationToken cancellationToken = default)
    {
        
        var (items, totalCount) =
            await _conferenceRepository.GetPagedFilteredAsync(
                query.Page,
                query.PageSize,
                query.Search,
                query.Location,
                query.Category,
                query.Status,
                includeInactiveAndDraft: true,
                cancellationToken);

        return new PagedResultDto<ConferenceDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ConferenceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(id, cancellationToken);

        if (conference is null)
        {
            return null;
        }

        return MapToDto(conference);
    }

    public async Task<List<RegisteredConferenceDto>> GetConfirmedForCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var registrations = await _conferenceRegistrationRepository
            .GetConfirmedRegistrationsForUserAsync(userId, cancellationToken);

        return registrations.Select(registration => MapToRegisteredDto(registration.Conference, registration.ConferenceRegistrationId)).ToList();
    }

    public async Task<ConferenceDto> CreateAsync(CreateConferenceDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.StartDate >= dto.EndDate)
        {
            throw new ArgumentException("Datum početka mora biti prije datuma završetka.");
        }

        if (dto.MaxParticipants <= 0)
        {
            throw new ArgumentException("Maksimalan broj učesnika mora biti veći od 0.");
        }

        var organizerId = Guid.Parse(_userContextService.GetUserId());
        var organizer = await _userRepository.GetByIdAsync(organizerId, cancellationToken);

        if (organizer is null)
        {
            throw new KeyNotFoundException($"Korisnik sa ID-jem {organizerId} nije pronađen.");
        }

        var conference = new Conference
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate.ToUniversalTime(),
            EndDate = dto.EndDate.ToUniversalTime(),
            Location = dto.Location,
            Category = dto.Category,
            MaxParticipants = dto.MaxParticipants,
            Status = "Planned",
            Organizers = new List<User> { organizer }
        };

        // novo - automatski dodavanje organizatora koji kreira konferenciju
        if (_userContextService.HasRole("organizator"))
        {
            var userId = Guid.Parse(_userContextService.GetUserId());
            Console.WriteLine($"=== DODAJEM ORGANIZATORA: {userId} ==="); // DODAJ OVO
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                conference.Organizers = new List<User> { user };
            }
        }

        var createdConference =
            await _conferenceRepository.AddAsync(conference, cancellationToken);

        return MapToDto(createdConference);
    }

    public async Task UpdateAsync(Guid id, UpdateConferenceDto dto, CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(id, cancellationToken);

        if (conference == null)
        {
            throw new KeyNotFoundException($"Konferencija sa ID-jem {id} nije pronađena.");
        }

        if (dto.StartDate > dto.EndDate)
        {
            throw new ArgumentException("Datum početka mora biti prije datuma završetka.");
        }

        if (dto.StartDate <= DateTime.UtcNow)
        {
            throw new ArgumentException("Datum početka mora biti u budućnosti.");
        }

        if (dto.MaxParticipants <= 0)
        {
            throw new ArgumentException("Maksimalan broj učesnika mora biti veći od 0.");
        }

        bool isScheduleChanged = conference.StartDate != dto.StartDate.ToUniversalTime() || conference.EndDate != dto.EndDate.ToUniversalTime() || conference.Location != dto.Location;

        conference.Title = dto.Title;
        conference.Description = dto.Description;
        conference.StartDate = dto.StartDate.ToUniversalTime();
        conference.EndDate = dto.EndDate.ToUniversalTime();
        conference.Location = dto.Location;
        conference.Category = dto.Category;
        conference.MaxParticipants = dto.MaxParticipants;

        await _conferenceRepository.UpdateAsync(conference, cancellationToken);

        if (isScheduleChanged)
        {
            var registrations = await _conferenceRegistrationRepository.GetRegistrationsByConferenceAsync(id, cancellationToken);
            foreach (var registration in registrations.Where(r => r.RegistrationStatus == "Confirmed"))
            {
                await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationDto
                {
                    UserId = registration.UserId,
                    Title = "Promjena termina konferencije",
                    Content = $"Konferencija {conference.Title} na koju ste prijavljeni je promijenila termin ili lokaciju.",
                    NotificationType = "ConferenceUpdated"
                }, cancellationToken);
            }
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(id, cancellationToken);

        if (conference == null)
        {
            throw new KeyNotFoundException($"Conference with ID {id} not found.");
        }

        var registrations = await _conferenceRegistrationRepository.GetRegistrationsByConferenceAsync(id, cancellationToken);
        foreach (var registration in registrations.Where(r => r.RegistrationStatus == "Confirmed"))
        {
            await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationDto
            {
                UserId = registration.UserId,
                Title = "Konferencija otkazana",
                Content = $"Konferencija {conference.Title} na koju ste prijavljeni je otkazana.",
                NotificationType = "ConferenceCancelled"
            }, cancellationToken);
        }

        await _conferenceRepository.DeleteAsync(conference, cancellationToken);
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

    private static RegisteredConferenceDto MapToRegisteredDto(Conference conference, Guid registrationId)
    {
        return new RegisteredConferenceDto
        {
            ConferenceRegistrationId = registrationId,
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