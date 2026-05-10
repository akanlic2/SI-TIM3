namespace ConferenceManagement.Domain.Entities;

public class ConferenceRegistration
{
    public Guid ConferenceRegistrationId { get; set; }
    public Guid UserId { get; set; }
    public Guid ConferenceId { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public string RegistrationStatus { get; set; } = string.Empty;

    public User User { get; set; }
    public Conference Conference { get; set; }
    public Payment Payment { get; set; }
}