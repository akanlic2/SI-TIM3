using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionRegistrationController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionRegistrationController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("/api/session/{id:guid}/register")]
    [Authorize(Policy = "AttendeePolicy")]
    public async Task<IActionResult> Register(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _sessionService.RegisterAsync(id, cancellationToken);
            return Ok(new { Message = "Uspješna prijava na sesiju." });
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

    [HttpPut("/api/session/{id:guid}/cancel")]
    [Authorize(Policy = "AttendeePolicy")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _sessionService.CancelRegistrationAsync(id, cancellationToken);
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
}
