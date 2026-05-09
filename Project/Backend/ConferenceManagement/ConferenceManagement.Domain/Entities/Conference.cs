namespace ConferenceManagement.Domain.Entities;

public class Conference
{
    public Guid ConferenceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public string Status { get; set; } = "Planned";

    public ICollection<Session> Sessions { get; set; }
    public ICollection<ConferenceRegistration> ConferenceRegistrations { get; set; }
    public ICollection<Material> Materials { get; set; }
    public ICollection<LogisticsTask> LogisticsTasks { get; set; }
    public ICollection<AgendaItem> AgendaItems { get; set; }
    public ICollection<User> Organizers { get; set; }

}