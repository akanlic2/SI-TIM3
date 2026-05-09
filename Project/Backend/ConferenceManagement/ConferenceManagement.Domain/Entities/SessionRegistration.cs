namespace ConferenceManagement.Domain.Entities;

public class SessionRegistration
{
    public Guid SessionRegistrationId { get; set; }
    public Guid UserId { get; set; }
    public Guid SessionId { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string RegistrationStatus { get; set; } = string.Empty;
    public bool IsSpeaker { get; set; } = false;

    public User User { get; set; }
    public Session Session { get; set; }
}