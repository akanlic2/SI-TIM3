using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.DTOs.Logistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers
{
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    [ApiController]
    [Route("api")]
    public class LogisticsController : ControllerBase
    {
        private readonly ILogisticsService _logisticsService;

        public LogisticsController(ILogisticsService logisticsService)
        {
            _logisticsService = logisticsService;
        }

        // S46.1 — GET /conferences/:id/logistics
        [HttpGet("conferences/{id}/logistics")]
        public async Task<ActionResult<IEnumerable<LogisticsTaskDto>>> GetConferenceLogistics(Guid id, [FromQuery] string? taskType)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst(ClaimTypes.Name)?.Value;

                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized("Korisnički podaci nisu ispravno preneseni kroz autorizacijski token.");
                }

                Guid currentUserId = Guid.Parse(userIdClaim);

                var result = await _logisticsService.GetLogisticsForConferenceAsync(id, taskType, currentUserId, roleClaim);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // S46.2 — POST /conferences/:id/logistics
        [HttpPost("conferences/{id}/logistics")]
        public async Task<ActionResult<LogisticsTaskDto>> CreateLogisticsTask(Guid id, [FromBody] CreateLogisticsTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst(ClaimTypes.Name)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized("Korisnički podaci nisu ispravno preneseni kroz autorizacijski token.");
                }

                Guid currentUserId = Guid.Parse(userIdClaim);

                var createdTask = await _logisticsService.CreateLogisticsTaskAsync(id, dto, currentUserId, roleClaim);

                // POPRAVLJENO: Status201Created umjesto nepostojećeg Status211
                return StatusCode(StatusCodes.Status201Created, createdTask);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // S46.3 — PUT /logistics/{id}
        [HttpPut("logistics/{id}")]
        public async Task<ActionResult<LogisticsTaskDto>> UpdateLogisticsTask(Guid id, [FromBody] UpdateLogisticsTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst(ClaimTypes.Name)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized("Korisnički podaci nisu pronađeni u autorizacijskom tokenu.");
                }

                Guid currentUserId = Guid.Parse(userIdClaim);

                var updatedTask = await _logisticsService.UpdateLogisticsTaskAsync(id, dto, currentUserId, roleClaim);
                return Ok(updatedTask);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception)
            {
                throw;
            }
        }

        // S46.4 — DELETE /logistics/{id}
        [HttpDelete("logistics/{id}")]
        public async Task<IActionResult> DeleteLogisticsTask(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                return Unauthorized("Korisnički podaci nisu pronađeni u autorizacijskom tokenu.");

            Guid currentUserId = Guid.Parse(userIdClaim);

            await _logisticsService.DeleteLogisticsTaskAsync(id, currentUserId, roleClaim);

            return Ok(new { message = "Logistička aktivnost je uspješno obrisana." });
        }
    }
}