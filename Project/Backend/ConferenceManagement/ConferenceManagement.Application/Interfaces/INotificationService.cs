using ConferenceManagement.Application.DTOs.Notification;

namespace ConferenceManagement.Application.Interfaces;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
    Task<List<NotificationDto>> GetMyNotificationsAsync(CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(CancellationToken cancellationToken = default);
}
