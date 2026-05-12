using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConferenceRegistrationController : ControllerBase
{
    private readonly IConferenceRegistrationService _conferenceRegistrationService;

    public ConferenceRegistrationController(IConferenceRegistrationService conferenceRegistrationService)
    {
        _conferenceRegistrationService = conferenceRegistrationService;
    }

    [HttpPost("/api/conference/{id:guid}/register")]
    [Authorize(Policy = "AttendeePolicy")]
    public async Task<IActionResult> Register(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _conferenceRegistrationService.RegisterAsync(id, cancellationToken);
            return Ok(new { Message = "Uspješna prijava na konferenciju." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("/api/registration/{id:guid}/cancel")]
    [Authorize(Policy = "AttendeePolicy")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _conferenceRegistrationService.CancelAsync(id, cancellationToken);
            return Ok(new { Message = "Prijava je otkazana." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("/api/conferences/{conferenceId:guid}/registrations")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<List<global::ConferenceManagement.Application.DTOs.Conference.ConferenceRegistrationUserDto>>> GetRegistrationsByConference(
        Guid conferenceId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _conferenceRegistrationService
                .GetRegistrationsByConferenceAsync(conferenceId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
