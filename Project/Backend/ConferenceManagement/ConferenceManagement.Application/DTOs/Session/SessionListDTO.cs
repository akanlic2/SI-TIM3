namespace ConferenceManagement.Application.DTOs.Session;

public class SessionListDTO
{
    public Guid SessionId { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string SessionType { get; set; }
    public string Status { get; set; }
}