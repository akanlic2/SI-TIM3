using ConferenceManagement.Application.DTOs.Notification;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    private NotificationService CreateService() =>
        new(
            _notificationRepositoryMock.Object,
            _userContextMock.Object,
            _userRepositoryMock.Object
        );

    [Fact]
    public async Task CreateNotificationAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var dto = new CreateNotificationDto
        {
            UserId = userId,
            Title = "Test",
            Content = "Test content",
            NotificationType = "Test"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CreateNotificationAsync(dto));
    }

    [Fact]
    public async Task CreateNotificationAsync_ValidData_CreatesUnreadNotification()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "User"
            });

        _notificationRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification n, CancellationToken _) => n);

        var dto = new CreateNotificationDto
        {
            UserId = userId,
            Title = "Nova notifikacija",
            Content = "Sadržaj notifikacije",
            NotificationType = "Info"
        };

        var result = await service.CreateNotificationAsync(dto);

        Assert.Equal(userId, result.UserId);
        Assert.Equal("Nova notifikacija", result.Title);
        Assert.Equal("Sadržaj notifikacije", result.Content);
        Assert.Equal("Info", result.NotificationType);
        Assert.False(result.IsRead);

        _notificationRepositoryMock.Verify(r => r.AddAsync(It.Is<Notification>(
            n => n.UserId == userId &&
                 n.Title == "Nova notifikacija" &&
                 n.Content == "Sadržaj notifikacije" &&
                 n.NotificationType == "Info" &&
                 n.IsRead == false
        ), It.IsAny<CancellationToken>()), Times.Once);

        _notificationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMyNotificationsAsync_ReturnsCurrentUserNotifications()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(userId.ToString());

        _notificationRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>
            {
                new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Prva",
                    Content = "Prva notifikacija",
                    NotificationType = "Info",
                    IsRead = false,
                    SentDate = DateTime.UtcNow
                },
                new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Druga",
                    Content = "Druga notifikacija",
                    NotificationType = "Warning",
                    IsRead = true,
                    SentDate = DateTime.UtcNow
                }
            });

        var result = await service.GetMyNotificationsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, n => n.Title == "Prva" && n.IsRead == false);
        Assert.Contains(result, n => n.Title == "Druga" && n.IsRead == true);
    }

    [Fact]
    public async Task MarkAsReadAsync_NotificationNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(userId.ToString());

        _notificationRepositoryMock
            .Setup(r => r.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.MarkAsReadAsync(notificationId));
    }

    [Fact]
    public async Task MarkAsReadAsync_NotificationBelongsToOtherUser_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(currentUserId.ToString());

        _notificationRepositoryMock
            .Setup(r => r.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Notification
            {
                NotificationId = notificationId,
                UserId = otherUserId,
                IsRead = false
            });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.MarkAsReadAsync(notificationId));
    }

    [Fact]
    public async Task MarkAsReadAsync_UnreadNotification_MarksAsRead()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        var notification = new Notification
        {
            NotificationId = notificationId,
            UserId = userId,
            Title = "Test",
            Content = "Test",
            NotificationType = "Info",
            IsRead = false
        };

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(userId.ToString());

        _notificationRepositoryMock
            .Setup(r => r.GetByIdAsync(notificationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        await service.MarkAsReadAsync(notificationId);

        Assert.True(notification.IsRead);
        _notificationRepositoryMock.Verify(r => r.UpdateAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
        _notificationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_NoUnreadNotifications_DoesNotSave()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(userId.ToString());

        _notificationRepositoryMock
            .Setup(r => r.GetUnreadByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>());

        await service.MarkAllAsReadAsync();

        _notificationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MarkAllAsReadAsync_UnreadNotifications_MarksAllAsRead()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var notifications = new List<Notification>
        {
            new Notification { NotificationId = Guid.NewGuid(), UserId = userId, IsRead = false },
            new Notification { NotificationId = Guid.NewGuid(), UserId = userId, IsRead = false }
        };

        _userContextMock
            .Setup(x => x.GetUserId())
            .Returns(userId.ToString());

        _notificationRepositoryMock
            .Setup(r => r.GetUnreadByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications);

        await service.MarkAllAsReadAsync();

        Assert.All(notifications, n => Assert.True(n.IsRead));
        _notificationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _notificationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}