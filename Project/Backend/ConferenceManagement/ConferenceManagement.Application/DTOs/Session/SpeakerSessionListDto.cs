namespace ConferenceManagement.Application.DTOs.Session;

public class SpeakerSessionListDto
{
    public Guid SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string SessionType { get; set; } = string.Empty;

    // Podaci o konferenciji (Zahtjev S43-BE-01)
    public Guid ConferenceId { get; set; }
    public string ConferenceTitle { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}