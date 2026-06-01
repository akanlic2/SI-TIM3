namespace ConferenceManagement.Application.DTOs.Equipment;

public class EquipmentDto
{
    public Guid EquipmentId { get; set; }
    public Guid? SessionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public string AvailabilityStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
