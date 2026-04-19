using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConferenceController : ControllerBase
{
    private readonly IConferenceService _conferenceService;

    public ConferenceController(IConferenceService conferenceService)
    {
        _conferenceService = conferenceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ConferenceDto>>> GetAll(CancellationToken cancellationToken)
    {
        var conferences = await _conferenceService.GetAllAsync(cancellationToken);
        return Ok(conferences);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConferenceDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var conference = await _conferenceService.GetByIdAsync(id, cancellationToken);

        if (conference is null)
        {
            return NotFound();
        }

        return Ok(conference);
    }

    [HttpPost]
    public async Task<ActionResult<ConferenceDto>> Create(
        [FromBody] CreateConferenceDto dto,
        CancellationToken cancellationToken)
    {
        var createdConference = await _conferenceService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdConference.ConferenceId },
            createdConference);
    }
}