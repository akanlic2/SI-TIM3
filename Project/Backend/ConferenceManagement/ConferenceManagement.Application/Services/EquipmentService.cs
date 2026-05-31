using ConferenceManagement.Application.DTOs.Equipment;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserContextService _userContextService;

    public EquipmentService(
        IEquipmentRepository equipmentRepository,
        ISessionRepository sessionRepository,
        IUserContextService userContextService)
    {
        _equipmentRepository = equipmentRepository;
        _sessionRepository = sessionRepository;
        _userContextService = userContextService;
    }

    public async Task<List<EquipmentDto>> GetAllEquipmentAsync(CancellationToken cancellationToken)
    {
        // Svi korisnici mogu pregledati opremu
        var items = await _equipmentRepository.GetAllAsync(cancellationToken);
        
        // Vraćamo samo globalnu opremu (gdje je SessionId == null) da se ne miješa dodijeljena oprema sa globalnom
        return items
            .Where(e => e.SessionId == null)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<EquipmentDto> CreateEquipmentAsync(CreateEquipmentDto dto, CancellationToken cancellationToken)
    {
        // Samo administratori i organizatori mogu kreirati opremu
        if (!_userContextService.HasAnyRole("admin-sistema", "organizator"))
        {
            throw new UnauthorizedAccessException("Nemate dozvolu za kreiranje opreme.");
        }

        var equipment = new Equipment
        {
            EquipmentId = Guid.NewGuid(),
            SessionId = null, // Globalna oprema
            Name = dto.Name,
            Type = dto.Type,
            Quantity = dto.Quantity,
            AvailableQuantity = dto.Quantity,
            IsAvailable = true,
            AvailabilityStatus = "Available",
            CreatedAt = DateTime.UtcNow
        };

        await _equipmentRepository.AddAsync(equipment, cancellationToken);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(equipment);
    }

    public async Task DeleteEquipmentAsync(Guid equipmentId, CancellationToken cancellationToken)
    {
        // Samo administratori i organizatori mogu brisati opremu
        if (!_userContextService.HasAnyRole("admin-sistema", "organizator"))
        {
            throw new UnauthorizedAccessException("Nemate dozvolu za brisanje opreme.");
        }

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId, cancellationToken);
        if (equipment == null)
        {
            throw new KeyNotFoundException("Oprema nije pronađena.");
        }

        // Ako ima aktivne dodjele, ne dozvoliti brisanje ili prvo osloboditi?
        // Sigurnije je provjeriti da li je neka količina zauzeta (tj. AvailableQuantity != Quantity)
        if (equipment.AvailableQuantity < equipment.Quantity)
        {
            throw new InvalidOperationException("Nije moguće obrisati opremu jer je dio opreme dodijeljen sesijama.");
        }

        await _equipmentRepository.DeleteAsync(equipment, cancellationToken);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<EquipmentDto>> GetEquipmentBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new KeyNotFoundException("Sesija nije pronađena.");
        }

        var items = await _equipmentRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task AssignEquipmentToSessionAsync(Guid sessionId, AssignEquipmentDto dto, CancellationToken cancellationToken)
    {
        if (!_userContextService.HasAnyRole("admin-sistema", "organizator"))
        {
            throw new UnauthorizedAccessException("Nemate dozvolu za dodjelu opreme.");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new KeyNotFoundException("Sesija nije pronađena.");
        }

        var globalEquipment = await _equipmentRepository.GetByIdAsync(dto.EquipmentId, cancellationToken);
        if (globalEquipment == null || globalEquipment.SessionId != null)
        {
            throw new KeyNotFoundException("Oprema nije pronađena u globalnom inventaru.");
        }

        if (globalEquipment.AvailableQuantity < dto.Quantity)
        {
            throw new InvalidOperationException($"Nedovoljna količina opreme na stanju. Dostupno: {globalEquipment.AvailableQuantity}.");
        }

        // Oduzmi iz globalnog inventara
        globalEquipment.AvailableQuantity -= dto.Quantity;
        if (globalEquipment.AvailableQuantity == 0)
        {
            globalEquipment.IsAvailable = false;
            globalEquipment.AvailabilityStatus = "Unavailable";
        }
        await _equipmentRepository.UpdateAsync(globalEquipment, cancellationToken);

        // Kreiraj zapis dodijeljene opreme za sesiju
        var assignedEquipment = new Equipment
        {
            EquipmentId = Guid.NewGuid(),
            SessionId = sessionId,
            Name = globalEquipment.Name,
            Type = globalEquipment.Type,
            Quantity = dto.Quantity,
            AvailableQuantity = 0, // Za dodijeljenu opremu nema "slobodne" količine
            IsAvailable = false,
            AvailabilityStatus = "Assigned",
            CreatedAt = DateTime.UtcNow
        };

        await _equipmentRepository.AddAsync(assignedEquipment, cancellationToken);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UnassignEquipmentFromSessionAsync(Guid sessionId, Guid equipmentId, CancellationToken cancellationToken)
    {
        if (!_userContextService.HasAnyRole("admin-sistema", "organizator"))
        {
            throw new UnauthorizedAccessException("Nemate dozvolu za uklanjanje opreme sa sesije.");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new KeyNotFoundException("Sesija nije pronađena.");
        }

        var assignedEquipment = await _equipmentRepository.GetByIdAsync(equipmentId, cancellationToken);
        if (assignedEquipment == null || assignedEquipment.SessionId != sessionId)
        {
            throw new KeyNotFoundException("Oprema nije pronađena na ovoj sesiji.");
        }

        // Pronađi originalnu globalnu opremu po nazivu i tipu u globalnom inventaru
        var globalItems = await _equipmentRepository.GetAllAsync(cancellationToken);
        var globalEquipment = globalItems.FirstOrDefault(e => e.SessionId == null && e.Name == assignedEquipment.Name && e.Type == assignedEquipment.Type);

        if (globalEquipment != null)
        {
            // Vrati količinu u globalni inventar
            globalEquipment.AvailableQuantity += assignedEquipment.Quantity;
            globalEquipment.IsAvailable = true;
            globalEquipment.AvailabilityStatus = "Available";
            await _equipmentRepository.UpdateAsync(globalEquipment, cancellationToken);
        }

        // Briši dodjelu
        await _equipmentRepository.DeleteAsync(assignedEquipment, cancellationToken);
        await _equipmentRepository.SaveChangesAsync(cancellationToken);
    }

    private static EquipmentDto MapToDto(Equipment equipment)
    {
        return new EquipmentDto
        {
            EquipmentId = equipment.EquipmentId,
            SessionId = equipment.SessionId,
            Name = equipment.Name,
            Type = equipment.Type,
            Quantity = equipment.Quantity,
            AvailableQuantity = equipment.AvailableQuantity,
            IsAvailable = equipment.IsAvailable,
            AvailabilityStatus = equipment.AvailabilityStatus,
            CreatedAt = equipment.CreatedAt
        };
    }
}
