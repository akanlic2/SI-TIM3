using ConferenceManagement.Application.DTOs.Material;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace ConferenceManagement.Application.Services;

public class MaterialService : IMaterialService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ISessionRegistrationRepository _registrationRepository;
    private readonly IUserContextService _userContextService;
    private readonly IMaterialRepository _materialRepository; // Koristimo repozitorij

    public MaterialService(
        ISessionRepository sessionRepository,
        ISessionRegistrationRepository registrationRepository,
        IUserContextService userContextService,
        IMaterialRepository materialRepository) // Inject repozitorij
    {
        _sessionRepository = sessionRepository;
        _registrationRepository = registrationRepository;
        _userContextService = userContextService;
        _materialRepository = materialRepository;
    }

    public async Task<Guid> UploadMaterialAsync(Guid sessionId, IFormFile file, string title, string description, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var session = await _sessionRepository.GetByIdAsync(sessionId);

        if (session == null) throw new KeyNotFoundException("Sesija nije pronađena.");

        bool canUpload = false;
        if (_userContextService.HasAnyRole("admin-sistema", "organizator"))
        {
            canUpload = true;
        }
        else if (_userContextService.HasRole("predavac"))
        {
            var registration = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);
            if (registration != null && registration.IsSpeaker) canUpload = true;
        }

        if (!canUpload) throw new UnauthorizedAccessException("Nemate dozvolu za upload.");

        // ← DODAJ OVO: fizičko čuvanje fajla na disk
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "materials");
        Directory.CreateDirectory(uploadsFolder); // kreira folder ako ne postoji

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var fullPath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var filePath = $"/uploads/materials/{fileName}"; // relativna putanja za bazu

        var material = new Material
        {
            MaterialId = Guid.NewGuid(),
            SessionId = sessionId,
            Title = title,
            Description = description,
            FileUrl = filePath,
            MaterialType = file.ContentType,
            UploadDate = DateTime.UtcNow
        };

        await _materialRepository.AddAsync(material, cancellationToken);
        await _materialRepository.SaveChangesAsync(cancellationToken);

        return material.MaterialId;
    }

    public async Task<List<MaterialDto>> GetMaterialsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var isRegistered = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);

        if (isRegistered == null && !_userContextService.HasAnyRole("admin-sistema", "organizator"))
            throw new UnauthorizedAccessException("Niste prijavljeni na sesiju.");

        var materials = await _materialRepository.GetBySessionIdAsync(sessionId, cancellationToken);

        return materials.Select(m => new MaterialDto
        {
            MaterialId = m.MaterialId,
            Title = m.Title,
            FileUrl = m.FileUrl,
            MaterialType = m.MaterialType,
            UploadDate = m.UploadDate
        }).ToList();
    }
}