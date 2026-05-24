using ConferenceManagement.Application.DTOs.Question;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/sessions/{sessionId:guid}/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly IUserContextService _userContextService;
    private readonly IMemoryCache _cache;

    private const int RateLimitMax = 1;
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromSeconds(30);
    private const int MaxQuestionsPerUser = 10;

    public QuestionsController(
        IQuestionService questionService,
        IUserContextService userContextService,
        IMemoryCache cache)
    {
        _questionService = questionService;
        _userContextService = userContextService;
        _cache = cache;
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
            var userId = _userContextService.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return StatusCode(403, new { error = "Niste autentifikovani." });
            }

            var now = DateTime.UtcNow;
            var cacheKey = $"qa-rate:{sessionId}:{userId}";
            var totalKey = $"qa-total:{sessionId}:{userId}";
            var totalCount = _cache.Get<int?>(totalKey) ?? 0;

            if (totalCount >= MaxQuestionsPerUser)
            {
                return StatusCode(429, new
                {
                    error = "Dosegli ste maksimalan broj pitanja za ovu sesiju.",
                    maxQuestions = MaxQuestionsPerUser
                });
            }

            var timestamps = _cache.Get<List<DateTime>>(cacheKey) ?? new List<DateTime>();
            timestamps = timestamps.Where(t => now - t < RateLimitWindow).ToList();

            if (timestamps.Count >= RateLimitMax)
            {
                var oldest = timestamps.Min();
                var retryAfterSeconds = (int)Math.Ceiling((RateLimitWindow - (now - oldest)).TotalSeconds);
                return StatusCode(429, new
                {
                    error = $"Previše pitanja. Pokušajte ponovo za {retryAfterSeconds} sekundi.",
                    retryAfterSeconds
                });
            }

            timestamps.Add(now);
            _cache.Set(cacheKey, timestamps, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = RateLimitWindow
            });

            _cache.Set(totalKey, totalCount + 1);

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

    // S47-BE-03: PUT /api/sessions/{sessionId}/questions/{questionId}/answer
    [HttpPut("{questionId:guid}/answer")]
    [Authorize(Policy = "SpeakerPolicy")]
    public async Task<IActionResult> AnswerQuestion(
        Guid sessionId,
        Guid questionId,
        [FromBody] AnswerQuestionDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _questionService.AnswerQuestionAsync(sessionId, questionId, dto, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }
}