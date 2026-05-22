namespace ConferenceManagement.Application.DTOs.Session;

public class SpeakerSessionDetailsDto : SpeakerSessionListDto
{
    public string Description { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;

    // Lista učesnika (Zahtjev S43-BE-02)
    public List<SessionAttendeeDto> Attendees { get; set; } = new();
}

public class SessionAttendeeDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}