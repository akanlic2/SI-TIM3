namespace ConferenceManagement.Domain.Entities;

public class Room
{
    public Guid RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}