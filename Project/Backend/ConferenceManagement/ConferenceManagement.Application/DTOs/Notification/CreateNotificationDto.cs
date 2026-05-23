using System.ComponentModel.DataAnnotations;

namespace ConferenceManagement.Application.DTOs.Notification;

public class CreateNotificationDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NotificationType { get; set; } = string.Empty;
}
