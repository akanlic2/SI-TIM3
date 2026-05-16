using ConferenceManagement.Application.DTOs;
using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.DTOs.Room;
using ConferenceManagement.Application.DTOs.Session;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Dal; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IConferenceCapacityService _capacityService;
    private readonly ApplicationDbContext _context;

    public SessionsController(ISessionService sessionService, IConferenceCapacityService capacityService, ApplicationDbContext context)
    {
        _sessionService = sessionService;
        _capacityService = capacityService;
        _context = context;
    }

    [HttpGet("/api/conferences/{conferenceId}/sessions")]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<IActionResult> GetByConference(Guid conferenceId)
    {
        var result = await _sessionService.GetSessionsForConferenceAsync(conferenceId);
        return Ok(result);
    }

    [HttpGet("registered")]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<ActionResult<List<SessionListDTO>>> GetRegistered(CancellationToken cancellationToken)
    {
        var result = await _sessionService.GetRegisteredForCurrentUserAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateSessionDto dto)
    {
        var result = await _sessionService.CreateSessionAsync(dto);
        if (result == null)
            return BadRequest(new { error = "Termin je zauzet u ovoj dvorani. Odaberite drugi termin ili drugu salu." });

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionDto dto)
    {
        var success = await _sessionService.UpdateSessionAsync(id, dto);
        if (!success)
            return BadRequest(new { error = "Termin je zauzet u ovoj dvorani. Odaberite drugi termin ili drugu salu." });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _sessionService.DeleteSessionAsync(id);
        if (!success)
            return NotFound(new { error = $"Sesija sa ID {id} nije pronađena." });

        return NoContent();
    }

    [HttpPut("{id}/assign-speaker")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> AssignSpeaker(Guid id, [FromBody] AssignSpeakerDTO dto)
    {
        var result = await _sessionService.AssignSpeakerAsync(id, dto.UserId);
        if (!result)
        {
            return BadRequest(new { error = "Greška: Korisnik nema rolu 'predavac'." });
        }
        return Ok(new { message = "Predavač uspješno dodijeljen sesiji." });
    }

    [HttpPut("{id}/room")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> AssignRoomToSession(Guid id, [FromBody] AssignRoomDto dto)
    {
        try
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound(new { error = "Sesija nije pronađena." });
            }

            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == dto.RoomId);
            if (!roomExists)
            {
                return BadRequest(new { error = "Odabrana dvorana ne postoji." });
            }

            var isRoomOccupied = await _context.Sessions
                .AnyAsync(s => s.RoomId == dto.RoomId &&
                               s.SessionId != id &&
                               session.StartTime < s.EndTime &&
                               session.EndTime > s.StartTime);

            if (isRoomOccupied)
            {
                return BadRequest(new { error = "Dvorana je već zauzeta u ovom terminu drugom sesijom." });
            }

            session.RoomId = dto.RoomId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Dvorana uspješno dodijeljena sesiji.", sessionId = id, roomId = dto.RoomId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Greška pri dodjeli dvorane.", details = ex.Message });
        }
    }

    [HttpGet("{id:guid}/capacity")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<CapacityDto>> GetSessionCapacity(
        Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _capacityService.GetSessionCapacityAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}