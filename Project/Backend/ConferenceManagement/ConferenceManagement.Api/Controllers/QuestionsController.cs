using ConferenceManagement.Application.DTOs.Question;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/sessions/{sessionId:guid}/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    // S47-BE-01: POST /api/sessions/{sessionId}/questions
    // Role guard: ti dodjeljuješ ko smije postavljati pitanja (npr. "ParticipantPolicy")
    [HttpPost]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<IActionResult> CreateQuestion(
        Guid sessionId,
        [FromBody] CreateQuestionDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _questionService.CreateQuestionAsync(sessionId, dto, cancellationToken);
            return CreatedAtAction(
                nameof(GetQuestions),
                new { sessionId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Sesija još nije počela
            return StatusCode(403, new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // S47-BE-02: GET /api/sessions/{sessionId}/questions
    [HttpGet]
    [Authorize(Policy = "ParticipantPolicy")]
    public async Task<IActionResult> GetQuestions(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _questionService.GetQuestionsBySessionAsync(sessionId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}