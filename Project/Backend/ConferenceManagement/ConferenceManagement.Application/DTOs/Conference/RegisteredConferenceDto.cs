namespace ConferenceManagement.Application.DTOs.Conference;

public class RegisteredConferenceDto
{
    public Guid ConferenceRegistrationId { get; set; }
    public Guid ConferenceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MaxParticipants { get; set; }
    public string Status { get; set; } = string.Empty;
}
