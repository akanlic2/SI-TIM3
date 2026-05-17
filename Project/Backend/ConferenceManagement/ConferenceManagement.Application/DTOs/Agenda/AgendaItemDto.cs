namespace ConferenceManagement.Application.DTOs.Agenda;

public class AgendaItemDto
{
    public Guid AgendaItemId { get; set; }
    public Guid ConferenceId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid? RoomId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Session podaci (samo ako je Type == "Session")
    public string? SessionTitle { get; set; }
    public string? SessionType { get; set; }
    public string? SpeakerName { get; set; }

    // Room podaci
    public string? RoomName { get; set; }
}
