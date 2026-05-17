using ConferenceManagement.Application.DTOs.Agenda;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api")]
public class AgendaController : ControllerBase
{
    private readonly IAgendaItemService _agendaItemService;

    public AgendaController(IAgendaItemService agendaItemService)
    {
        _agendaItemService = agendaItemService;
    }

    // AGN-BE-02: GET /conferences/{id}/agenda — dostupno svim prijavljenim korisnicima
    [HttpGet("conferences/{conferenceId:guid}/agenda")]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<ActionResult<List<AgendaItemDto>>> GetByConference(Guid conferenceId)
    {
        var result = await _agendaItemService.GetByConferenceIdAsync(conferenceId);
        return Ok(result);
    }

    // AGN-BE-01: POST /conferences/{id}/agenda — admin ili organizator
    [HttpPost("conferences/{conferenceId:guid}/agenda")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<AgendaItemDto>> Create(
        Guid conferenceId,
        [FromBody] CreateAgendaItemDto dto)
    {
        try
        {
            var created = await _agendaItemService.CreateAsync(conferenceId, dto);
            return Ok(created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // AGN-BE-03: PUT /agenda/{id} — admin ili organizator
    [HttpPut("agenda/{id:guid}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAgendaItemDto dto)
    {
        try
        {
            await _agendaItemService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // AGN-BE-04: DELETE /agenda/{id} — admin ili organizator
    [HttpDelete("agenda/{id:guid}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _agendaItemService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
