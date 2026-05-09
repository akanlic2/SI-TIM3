namespace ConferenceManagement.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<ConferenceRegistration> ConferenceRegistrations { get; set; } = new List<ConferenceRegistration>();
    public ICollection<SessionRegistration> SessionRegistrations { get; set; } = new List<SessionRegistration>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<Conference> OrganizedConferences { get; set; } = new List<Conference>();
}