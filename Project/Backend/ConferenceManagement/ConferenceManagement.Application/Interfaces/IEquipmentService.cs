using ConferenceManagement.Application.DTOs.Equipment;

namespace ConferenceManagement.Application.Interfaces;

public interface IEquipmentService
{
    Task<List<EquipmentDto>> GetAllEquipmentAsync(CancellationToken cancellationToken);
    Task<EquipmentDto> CreateEquipmentAsync(CreateEquipmentDto dto, CancellationToken cancellationToken);
    Task DeleteEquipmentAsync(Guid equipmentId, CancellationToken cancellationToken);
    Task<EquipmentDto> DecrementEquipmentQuantityAsync(Guid equipmentId, CancellationToken cancellationToken);
    Task<List<EquipmentDto>> GetEquipmentBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
    Task AssignEquipmentToSessionAsync(Guid sessionId, AssignEquipmentDto dto, CancellationToken cancellationToken);
    Task UnassignEquipmentFromSessionAsync(Guid sessionId, Guid equipmentId, CancellationToken cancellationToken);
}
