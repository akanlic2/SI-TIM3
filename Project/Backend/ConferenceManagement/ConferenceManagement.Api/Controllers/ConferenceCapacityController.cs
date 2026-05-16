using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/conferences")]
public class ConferenceCapacityController : ControllerBase
{
    private readonly IConferenceCapacityService _capacityService;
    private readonly IUserContextService _userContextService;
    private readonly IConferenceRepository _conferenceRepository;

    public ConferenceCapacityController(
        IConferenceCapacityService capacityService,
        IUserContextService userContextService,
        IConferenceRepository conferenceRepository)
    {
        _capacityService = capacityService;
        _userContextService = userContextService;
        _conferenceRepository = conferenceRepository;
    }

    [HttpGet("{id:guid}/capacity")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<CapacityDto>> GetConferenceCapacity(
        Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"=== DEBUG CAPACITY ===");
            Console.WriteLine($"Role korisnika: {string.Join(", ", _userContextService.GetUserRoles())}");
            Console.WriteLine($"HasRole organizator: {_userContextService.HasRole("organizator")}");
            Console.WriteLine($"HasRole admin-sistema: {_userContextService.HasRole("admin-sistema")}");

            if (_userContextService.HasRole("organizator") && !_userContextService.HasRole("admin-sistema"))
            {
                var conference = await _conferenceRepository.GetByIdWithOrganizersAsync(id, cancellationToken);
                if (conference == null) return NotFound(new { Message = "Konferencija nije pronađena." });

                var userId = Guid.Parse(_userContextService.GetUserId());
                var isOrganizer = conference.Organizers.Any(o => o.UserId == userId);

                Console.WriteLine($"UserId iz tokena: {userId}");
                Console.WriteLine($"Organizers u konferenciji: {string.Join(", ", conference.Organizers.Select(o => o.UserId))}");
                Console.WriteLine($"isOrganizer: {isOrganizer}");

                if (!isOrganizer) return Forbid();
            }

            var result = await _capacityService.GetConferenceCapacityAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/participants")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<List<ParticipantDto>>> GetParticipants(
        Guid id,
        [FromQuery] string? search,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"=== DEBUG PARTICIPANTS ===");
            Console.WriteLine($"Role korisnika: {string.Join(", ", _userContextService.GetUserRoles())}");
            Console.WriteLine($"HasRole organizator: {_userContextService.HasRole("organizator")}");
            Console.WriteLine($"HasRole admin-sistema: {_userContextService.HasRole("admin-sistema")}");

            if (_userContextService.HasRole("organizator") && !_userContextService.HasRole("admin-sistema"))
            {
                var conference = await _conferenceRepository.GetByIdWithOrganizersAsync(id, cancellationToken);
                if (conference == null) return NotFound(new { Message = "Konferencija nije pronađena." });

                var userId = Guid.Parse(_userContextService.GetUserId());
                var isOrganizer = conference.Organizers.Any(o => o.UserId == userId);

                Console.WriteLine($"UserId iz tokena: {userId}");
                Console.WriteLine($"Organizers u konferenciji: {string.Join(", ", conference.Organizers.Select(o => o.UserId))}");
                Console.WriteLine($"isOrganizer: {isOrganizer}");

                if (!isOrganizer) return Forbid();
            }

            var result = await _capacityService.GetConferenceParticipantsAsync(id, search, status, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}