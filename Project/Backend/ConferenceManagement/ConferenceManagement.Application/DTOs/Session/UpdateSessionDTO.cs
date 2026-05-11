namespace ConferenceManagement.Application.DTOs;

public class UpdateSessionDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid RoomId { get; set; }
    public string SessionType { get; set; } = string.Empty;
}