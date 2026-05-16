namespace ConferenceManagement.Application.DTOs.Conference;

public class CapacityDto
{
    public int MaxParticipants { get; set; }
    public int RegisteredCount { get; set; }
    public int AvailableSpots { get; set; }
    public bool IsFull { get; set; }
}
