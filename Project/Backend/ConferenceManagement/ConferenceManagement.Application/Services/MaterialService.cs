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
    // Ovdje bi išao tvoj MaterialRepository, koristim DbContext direktno ili repozitorij ako ga napraviš
    private readonly ApplicationDbContext _context;

    public MaterialService(
        ISessionRepository sessionRepository,
        ISessionRegistrationRepository registrationRepository,
        IUserContextService userContextService,
        ApplicationDbContext context)
    {
        _sessionRepository = sessionRepository;
        _registrationRepository = registrationRepository;
        _userContextService = userContextService;
        _context = context;
    }

    public async Task<Guid> UploadMaterialAsync(Guid sessionId, IFormFile file, string title, string description, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var session = await _sessionRepository.GetByIdAsync(sessionId);

        if (session == null) throw new KeyNotFoundException("Sesija nije pronađena.");

        // --- S44-BE Role Guard ---
        bool canUpload = false;

        if (_userContextService.HasAnyRole("admin-sistema", "organizator"))
        {
            canUpload = true; // Admin i Organizator mogu za bilo koju sesiju
        }
        else if (_userContextService.HasRole("predavac"))
        {
            // Provjera da li je to njegova sesija (isSpeaker flag)
            var registration = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);
            if (registration != null && registration.IsSpeaker)
            {
                canUpload = true;
            }
        }

        if (!canUpload)
            throw new UnauthorizedAccessException("Nemate dozvolu za upload materijala na ovu sesiju.");

        // Logika za čuvanje fajla (Simulacija putanje, ovdje bi išao stvarni upload na disk/cloud)
        var filePath = $"/uploads/materials/{Guid.NewGuid()}_{file.FileName}";

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

        _context.Materials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        return material.MaterialId;
    }

    public async Task<List<MaterialDto>> GetMaterialsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());

        // --- S44-BE Role Guard za pregled ---
        // Provjera da li je korisnik prijavljen na sesiju (bilo kao ucesnik ili predavac)
        var isRegistered = await _registrationRepository.GetBySessionAndUserAsync(sessionId, userId);

        if (isRegistered == null && !_userContextService.HasAnyRole("admin-sistema", "organizator"))
            throw new UnauthorizedAccessException("Morate biti prijavljeni na sesiju da biste vidjeli materijale.");

        var materials = await _context.Materials
            .Where(m => m.SessionId == sessionId)
            .Select(m => new MaterialDto
            {
                MaterialId = m.MaterialId,
                Title = m.Title,
                FileUrl = m.FileUrl,
                MaterialType = m.MaterialType,
                UploadDate = m.UploadDate
            })
            .ToListAsync(cancellationToken);

        return materials;
    }
}