namespace ConferenceManagement.Domain.Entities;

public class Equipment
{
    public Guid EquipmentId { get; set; }
    public Guid SessionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string AvailabilityStatus { get; set; } = string.Empty;

    public Session Session { get; set; }
}