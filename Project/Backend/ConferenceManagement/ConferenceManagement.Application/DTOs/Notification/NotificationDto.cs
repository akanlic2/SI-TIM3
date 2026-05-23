namespace ConferenceManagement.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public bool IsRead { get; set; }
}
