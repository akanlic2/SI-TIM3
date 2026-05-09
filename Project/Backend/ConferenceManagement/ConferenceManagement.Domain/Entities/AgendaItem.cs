namespace ConferenceManagement.Domain.Entities;

public class AgendaItem
{
    public Guid AgendaItemId { get; set; }
    public Guid ConferenceId { get; set; }
    public Guid? SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public Conference Conference { get; set; }
    public Session Session { get; set; }
}