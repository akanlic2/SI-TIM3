using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.DTOs.User;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class ConferenceRegistrationService : IConferenceRegistrationService
{
    private const string ConfirmedStatus = "Confirmed";
    private const string CancelledStatus = "Cancelled";

    private readonly IConferenceRepository _conferenceRepository;
    private readonly IConferenceRegistrationRepository _conferenceRegistrationRepository;
    private readonly IUserContextService _userContextService;

    public ConferenceRegistrationService(
        IConferenceRepository conferenceRepository,
        IConferenceRegistrationRepository conferenceRegistrationRepository,
        IUserContextService userContextService)
    {
        _conferenceRepository = conferenceRepository;
        _conferenceRegistrationRepository = conferenceRegistrationRepository;
        _userContextService = userContextService;
    }

    public async Task RegisterAsync(Guid conferenceId, CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(conferenceId, cancellationToken);
        if (conference == null)
        {
            throw new KeyNotFoundException($"Konferencija sa ID-jem {conferenceId} nije pronađena.");
        }

        var userId = Guid.Parse(_userContextService.GetUserId());
        var existingRegistration = await _conferenceRegistrationRepository
            .GetByConferenceAndUserAsync(conferenceId, userId, cancellationToken);

        if (existingRegistration != null && existingRegistration.RegistrationStatus == ConfirmedStatus)
        {
            throw new InvalidOperationException("Korisnik je već prijavljen na konferenciju.");
        }

        if (existingRegistration != null && existingRegistration.RegistrationStatus == CancelledStatus)
        {
            var confirmedCount = await _conferenceRegistrationRepository
                .GetConfirmedCountForConferenceAsync(conferenceId, cancellationToken);

            if (confirmedCount >= conference.MaxParticipants)
            {
                throw new InvalidOperationException("Nema slobodnih mjesta za ovu konferenciju.");
            }

            existingRegistration.RegistrationStatus = ConfirmedStatus;
            existingRegistration.RegistrationDate = DateTime.UtcNow;
            await _conferenceRegistrationRepository.UpdateAsync(existingRegistration, cancellationToken);
            await _conferenceRegistrationRepository.SaveChangesAsync(cancellationToken);
            return;
        }

        var currentConfirmedCount = await _conferenceRegistrationRepository
            .GetConfirmedCountForConferenceAsync(conferenceId, cancellationToken);

        if (currentConfirmedCount >= conference.MaxParticipants)
        {
            throw new InvalidOperationException("Nema slobodnih mjesta za ovu konferenciju.");
        }

        var registration = new ConferenceRegistration
        {
            ConferenceRegistrationId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            UserId = userId,
            RegistrationDate = DateTime.UtcNow,
            RegistrationStatus = ConfirmedStatus
        };

        await _conferenceRegistrationRepository.AddAsync(registration, cancellationToken);
        await _conferenceRegistrationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(Guid registrationId, CancellationToken cancellationToken = default)
    {
        var registration = await _conferenceRegistrationRepository.GetByIdAsync(registrationId, cancellationToken);
        if (registration == null)
        {
            throw new KeyNotFoundException($"Prijava sa ID-jem {registrationId} nije pronađena.");
        }

        var userId = Guid.Parse(_userContextService.GetUserId());
        if (registration.UserId != userId)
        {
            throw new UnauthorizedAccessException("Nemate pravo otkazati ovu prijavu.");
        }

        if (registration.RegistrationStatus == CancelledStatus)
        {
            return;
        }

        registration.RegistrationStatus = CancelledStatus;
        await _conferenceRegistrationRepository.UpdateAsync(registration, cancellationToken);
        await _conferenceRegistrationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ConferenceRegistrationUserDto>> GetRegistrationsByConferenceAsync(
        Guid conferenceId,
        CancellationToken cancellationToken = default)
    {
        var conference = await _conferenceRepository.GetByIdAsync(conferenceId, cancellationToken);
        if (conference == null)
        {
            throw new KeyNotFoundException($"Konferencija sa ID-jem {conferenceId} nije pronađena.");
        }

        var registrations = await _conferenceRegistrationRepository
            .GetRegistrationsByConferenceAsync(conferenceId, cancellationToken);

        return registrations.Select(MapToRegistrationUserDto).ToList();
    }

    private static ConferenceRegistrationUserDto MapToRegistrationUserDto(ConferenceRegistration registration)
    {
        return new ConferenceRegistrationUserDto
        {
            ConferenceRegistrationId = registration.ConferenceRegistrationId,
            ConferenceId = registration.ConferenceId,
            UserId = registration.UserId,
            RegistrationDate = registration.RegistrationDate,
            RegistrationStatus = registration.RegistrationStatus,
            User = new UserDto
            {
                UserId = registration.User.UserId,
                Username = registration.User.Username,
                FirstName = registration.User.FirstName,
                LastName = registration.User.LastName,
                Email = registration.User.Email,
                Role = registration.User.Role,
                CreatedAt = registration.User.CreatedAt
            }
        };
    }
}
