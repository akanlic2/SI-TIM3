namespace ConferenceManagement.Domain.Entities;

public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public User User { get; set; }
}