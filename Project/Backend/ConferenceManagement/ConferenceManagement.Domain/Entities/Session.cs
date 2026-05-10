namespace ConferenceManagement.Domain.Entities;

public class Session
{
    public Guid SessionId { get; set; }
    public Guid ConferenceId { get; set; }
    public Guid RoomId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string SessionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public Conference Conference { get; set; }
    public Room Room { get; set; }
    public ICollection<SessionRegistration> SessionRegistrations { get; set; } = new List<SessionRegistration>();
    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public AgendaItem AgendaItem { get; set; }
}