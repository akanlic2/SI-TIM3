namespace ConferenceManagement.Domain.Entities;

public class Equipment
{
    public Guid EquipmentId { get; set; }

    /// <summary>
    /// Null = globalni inventar (nije dodijeljen nijednoj sesiji).
    /// Postavlja se kada se oprema dodijeli sesiji.
    /// </summary>
    public Guid? SessionId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string AvailabilityStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Session? Session { get; set; }
}