using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/conferences")]
public class ConferenceCapacityController : ControllerBase
{
    private readonly IConferenceCapacityService _capacityService;

    public ConferenceCapacityController(IConferenceCapacityService capacityService)
    {
        _capacityService = capacityService;
    }

    [HttpGet("{id:guid}/capacity")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<CapacityDto>> GetConferenceCapacity(
        Guid id, CancellationToken cancellationToken)
    {
        try
        {
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
            var result = await _capacityService.GetConferenceParticipantsAsync(id, search, status, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}