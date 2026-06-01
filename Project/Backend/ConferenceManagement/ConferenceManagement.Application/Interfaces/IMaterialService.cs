using ConferenceManagement.Application.DTOs.Material;
using Microsoft.AspNetCore.Http;

namespace ConferenceManagement.Application.Interfaces;

public interface IMaterialService
{
    // S44-BE-01: Upload materijala uz provjeru permisija
    Task<Guid> UploadMaterialAsync(Guid sessionId, IFormFile file, string title, string description, CancellationToken cancellationToken);

    // S44-BE-02: Lista materijala za korisnike prijavljene na sesiju
    Task<List<MaterialDto>> GetMaterialsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
}