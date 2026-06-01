using ConferenceManagement.Application.DTOs.Report;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/conferences")]
public class ConferenceReportController : ControllerBase
{
    private readonly IConferenceReportService _reportService;
    private readonly IUserContextService _userContextService;
    private readonly IConferenceRepository _conferenceRepository;

    public ConferenceReportController(
        IConferenceReportService reportService,
        IUserContextService userContextService,
        IConferenceRepository conferenceRepository)
    {
        _reportService = reportService;
        _userContextService = userContextService;
        _conferenceRepository = conferenceRepository;
    }

    [HttpGet("{id:guid}/report")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<ConferenceReportDto>> GetReport(
        Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (_userContextService.HasRole("organizator") &&
                !_userContextService.HasRole("admin-sistema"))
            {
                var conference = await _conferenceRepository
                    .GetByIdWithOrganizersAsync(id, cancellationToken);
                if (conference == null)
                    return NotFound(new { Message = "Konferencija nije pronađena." });

                var userId = Guid.Parse(_userContextService.GetUserId());
                if (!conference.Organizers.Any(o => o.UserId == userId))
                    return Forbid();
            }

            var result = await _reportService.GetReportAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/report/download")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> DownloadReport(
        Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (_userContextService.HasRole("organizator") &&
                !_userContextService.HasRole("admin-sistema"))
            {
                var conference = await _conferenceRepository
                    .GetByIdWithOrganizersAsync(id, cancellationToken);
                if (conference == null)
                    return NotFound(new { Message = "Konferencija nije pronađena." });

                var userId = Guid.Parse(_userContextService.GetUserId());
                if (!conference.Organizers.Any(o => o.UserId == userId))
                    return Forbid();
            }

            var pdfBytes = await _reportService.GenerateReportPdfAsync(id, cancellationToken);
            return File(pdfBytes, "application/pdf", $"izvjestaj-{id}.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}