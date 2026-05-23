using ConferenceManagement.Application.DTOs.Session;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOrSpeakerPolicy")]
public class SpeakersController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SpeakersController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// S43 — BE-01: Vraća listu sesija na kojima je trenutni korisnik predavač.
    /// </summary>
    [HttpGet("sessions")]
    public async Task<ActionResult<List<SpeakerSessionListDto>>> GetMySessions(CancellationToken cancellationToken)
    {
        var sessions = await _sessionService.GetSessionsForCurrentSpeakerAsync(cancellationToken);
        return Ok(sessions);
    }

    /// <summary>
    /// S43 — BE-02: Vraća detalje specifične sesije uključujući listu učesnika.
    /// </summary>
    [HttpGet("sessions/{sessionId:guid}")]
    public async Task<ActionResult<SpeakerSessionDetailsDto>> GetSessionDetails(Guid sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var details = await _sessionService.GetSpeakerSessionDetailsAsync(sessionId, cancellationToken);
            return Ok(details);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            // S43 — BE Role Guard: Ako predavač pokuša pristupiti sesiji koja nije njegova
            return Forbid();
        }
    }
}