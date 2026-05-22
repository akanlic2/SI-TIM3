using ConferenceManagement.Application.DTOs.Material;
using ConferenceManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Controllers;

[ApiController]
[Route("api/sessions/{sessionId:guid}/materials")]
[Authorize] // Osigurava da je korisnik ulogovan
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    /// <summary>
    /// S44-BE-01: Upload materijala. 
    /// Provjera permisija se vrši unutar servisa.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")] // Govori Swaggeru/Scalaru da očekuje fajl
    public async Task<ActionResult<Guid>> UploadMaterial(
        Guid sessionId,
        IFormFile file,
        [FromForm] string title,
        [FromForm] string description,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Fajl nije validan.");

        try
        {
            var materialId = await _materialService.UploadMaterialAsync(
                sessionId, file, title, description, cancellationToken);

            return Ok(new { id = materialId });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // Ako predavač pokuša na tuđu sesiju
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// S44-BE-02: Lista materijala za sesiju.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MaterialDto>>> GetMaterials(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var materials = await _materialService.GetMaterialsBySessionIdAsync(sessionId, cancellationToken);
            return Ok(materials);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("Morate biti prijavljeni na sesiju da biste vidjeli materijale.");
        }
    }
}