using ConferenceManagement.Application.DTOs.Equipment;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    /// <summary>
    /// S47.1: Pretraga i filtriranje opreme (vraća globalnu opremu).
    /// </summary>
    [HttpGet("api/equipment")]
    public async Task<ActionResult<List<EquipmentDto>>> GetAllEquipment(CancellationToken cancellationToken)
    {
        var items = await _equipmentService.GetAllEquipmentAsync(cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// S47.2: Kreiranje zapisa o opremi u globalnom inventaru.
    /// </summary>
    [HttpPost("api/equipment")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<EquipmentDto>> CreateEquipment(
        [FromBody] CreateEquipmentDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _equipmentService.CreateEquipmentAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAllEquipment), null, created);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// S47.2: Brisanje zapisa o opremi u globalnom inventaru.
    /// </summary>
    [HttpDelete("api/equipment/{id:guid}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> DeleteEquipment(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _equipmentService.DeleteEquipmentAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Smanjenje ukupne kolicine opreme u globalnom inventaru.
    /// </summary>
    [HttpPatch("api/equipment/{id:guid}/decrement")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<ActionResult<EquipmentDto>> DecrementEquipmentQuantity(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _equipmentService.DecrementEquipmentQuantityAsync(id, cancellationToken);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// S47.3: Lista opreme dodijeljene određenoj sesiji.
    /// </summary>
    [HttpGet("api/sessions/{sessionId:guid}/equipment")]
    public async Task<ActionResult<List<EquipmentDto>>> GetSessionEquipment(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await _equipmentService.GetEquipmentBySessionIdAsync(sessionId, cancellationToken);
            return Ok(items);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// S47.3 & S47.4: Dodjela opreme određenoj sesiji.
    /// </summary>
    [HttpPost("api/sessions/{sessionId:guid}/equipment")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> AssignEquipment(
        Guid sessionId,
        [FromBody] AssignEquipmentDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _equipmentService.AssignEquipmentToSessionAsync(sessionId, dto, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Oslobađanje/uklanjanje opreme iz sesije.
    /// </summary>
    [HttpDelete("api/sessions/{sessionId:guid}/equipment/{equipmentId:guid}")]
    [Authorize(Policy = "AdminOrOrganizerPolicy")]
    public async Task<IActionResult> UnassignEquipment(
        Guid sessionId,
        Guid equipmentId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _equipmentService.UnassignEquipmentFromSessionAsync(sessionId, equipmentId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
