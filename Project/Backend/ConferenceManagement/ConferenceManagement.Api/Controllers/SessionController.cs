using ConferenceManagement.Application.DTOs;
using ConferenceManagement.Application.DTOs.Session;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    
    [HttpGet("/api/conferences/{conferenceId}/sessions")]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<IActionResult> GetByConference(Guid conferenceId)
    {
        var result = await _sessionService.GetSessionsForConferenceAsync(conferenceId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateSessionDto dto)
    {
        var result = await _sessionService.CreateSessionAsync(dto);
        if (result == null)
            return BadRequest("U ovom terminu već postoji sesija. Odaberite drugi termin.");

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionDto dto)
    {
        var success = await _sessionService.UpdateSessionAsync(id, dto);

        if (!success)
            return BadRequest("Nije moguće ažurirati sesiju.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _sessionService.DeleteSessionAsync(id);

        if (!success)
            return NotFound($"Sesija sa ID {id} nije pronađena.");

        return NoContent();
    }

    [HttpPut("{id}/assign-speaker")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> AssignSpeaker(Guid id, [FromBody] AssignSpeakerDTO dto)
    {
        var result = await _sessionService.AssignSpeakerAsync(id, dto.UserId);

        if (!result)
        {
            return BadRequest("Greška: Sesija ne postoji ili korisnik nema rolu 'predavac'.");
        }

        return Ok(new { Message = "Predavač uspješno dodijeljen sesiji." });
    }
}