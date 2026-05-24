using ConferenceManagement.Application.DTOs.Notification;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;

namespace ConferenceManagement.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        
        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"Korisnik sa ID-jem {dto.UserId} nije pronađen.");
        }

        var notification = new Notification
        {
            NotificationId = Guid.NewGuid(),
            UserId = dto.UserId,
            Title = dto.Title,
            Content = dto.Content,
            NotificationType = dto.NotificationType,
            SentDate = DateTime.UtcNow,
            IsRead = false
        };

        var createdNotification = await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(createdNotification);
    }

    public async Task<List<NotificationDto>> GetMyNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var notifications = await _notificationRepository.GetByUserIdAsync(userId, cancellationToken);
        return notifications.Select(MapToDto).ToList();
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);

        if (notification == null)
        {
            throw new KeyNotFoundException($"Notifikacija sa ID-jem {notificationId} nije pronađena.");
        }

        if (notification.UserId != userId)
        {
            throw new UnauthorizedAccessException("Nemate pravo pristupiti ovoj notifikaciji.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAllAsReadAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(_userContextService.GetUserId());
        var unreadNotifications = await _notificationRepository.GetUnreadByUserIdAsync(userId, cancellationToken);

        if (!unreadNotifications.Any())
        {
            return;
        }

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
        }

        await _notificationRepository.SaveChangesAsync(cancellationToken);
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Title = notification.Title,
            Content = notification.Content,
            NotificationType = notification.NotificationType,
            SentDate = notification.SentDate,
            IsRead = notification.IsRead
        };
    }
}
