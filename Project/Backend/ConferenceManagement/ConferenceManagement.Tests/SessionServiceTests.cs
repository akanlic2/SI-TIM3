using ConferenceManagement.Application.DTOs;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class SessionServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ISessionRegistrationRepository> _registrationRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();

    private SessionService CreateService() =>
        new(
            _sessionRepositoryMock.Object,
            _userRepositoryMock.Object,
            _registrationRepositoryMock.Object,
            _userContextMock.Object
        );

    [Fact]
    public async Task CreateSessionAsync_ValidData_CreatesSession()
    {
        var service = CreateService();

        var dto = new CreateSessionDto
        {
            Title = "AI Session",
            Description = "Session description",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3),
            ConferenceId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            SessionType = "Lecture"
        };

        _sessionRepositoryMock
            .Setup(r => r.CheckOverlapAsync(dto.RoomId, dto.StartTime, dto.EndTime, null))
            .ReturnsAsync(false);

        var result = await service.CreateSessionAsync(dto);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Value);

        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Once);
        _sessionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateSessionAsync_EndBeforeStart_ReturnsNull()
    {
        var service = CreateService();

        var dto = new CreateSessionDto
        {
            Title = "Invalid Session",
            Description = "Invalid time",
            StartTime = DateTime.UtcNow.AddHours(4),
            EndTime = DateTime.UtcNow.AddHours(3),
            ConferenceId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            SessionType = "Workshop"
        };

        var result = await service.CreateSessionAsync(dto);

        Assert.Null(result);
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task CreateSessionAsync_OverlappingSession_ReturnsNull()
    {
        var service = CreateService();

        var dto = new CreateSessionDto
        {
            Title = "Overlapping Session",
            Description = "Overlap test",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3),
            ConferenceId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            SessionType = "Lecture"
        };

        _sessionRepositoryMock
            .Setup(r => r.CheckOverlapAsync(dto.RoomId, dto.StartTime, dto.EndTime, null))
            .ReturnsAsync(true);

        var result = await service.CreateSessionAsync(dto);

        Assert.Null(result);
        _sessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task UpdateSessionAsync_ExistingSession_UpdatesSuccessfully()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        var existingSession = new Session
        {
            SessionId = sessionId,
            Title = "Old title",
            RoomId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3)
        };

        var dto = new UpdateSessionDto
        {
            Title = "New title",
            Description = "New description",
            StartTime = DateTime.UtcNow.AddHours(4),
            EndTime = DateTime.UtcNow.AddHours(5),
            RoomId = Guid.NewGuid(),
            SessionType = "Workshop"
        };

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(existingSession);

        _sessionRepositoryMock
            .Setup(r => r.CheckOverlapAsync(dto.RoomId, dto.StartTime, dto.EndTime, sessionId))
            .ReturnsAsync(false);

        var result = await service.UpdateSessionAsync(sessionId, dto);

        Assert.True(result);
        Assert.Equal("New title", existingSession.Title);
        _sessionRepositoryMock.Verify(r => r.UpdateAsync(existingSession), Times.Once);
        _sessionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateSessionAsync_SessionNotFound_ReturnsFalse()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var dto = new UpdateSessionDto
        {
            Title = "New title",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3),
            RoomId = Guid.NewGuid(),
            SessionType = "Lecture"
        };

        var result = await service.UpdateSessionAsync(sessionId, dto);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteSessionAsync_ExistingSession_DeletesSuccessfully()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        var session = new Session
        {
            SessionId = sessionId,
            Title = "Session"
        };

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        var result = await service.DeleteSessionAsync(sessionId);

        Assert.True(result);
        _sessionRepositoryMock.Verify(r => r.DeleteAsync(session), Times.Once);
        _sessionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteSessionAsync_NotFound_ReturnsFalse()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var result = await service.DeleteSessionAsync(sessionId);

        Assert.False(result);
    }

    [Fact]
    public async Task AssignSpeakerAsync_ValidSpeaker_AssignsSpeaker()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                UserId = userId,
                Role = "predavac",
                FirstName = "Test",
                LastName = "Predavac"
            });

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync((SessionRegistration?)null);

        var result = await service.AssignSpeakerAsync(sessionId, userId);

        Assert.True(result);
        _registrationRepositoryMock.Verify(r => r.AddAsync(It.Is<SessionRegistration>(
            sr => sr.SessionId == sessionId &&
                  sr.UserId == userId &&
                  sr.IsSpeaker &&
                  sr.RegistrationStatus == "Confirmed"
        )), Times.Once);
        _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AssignSpeakerAsync_UserIsNotSpeaker_ReturnsFalse()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                UserId = userId,
                Role = "ucesnik"
            });

        var result = await service.AssignSpeakerAsync(sessionId, userId);

        Assert.False(result);
    }

    [Fact]
    public async Task RegisterAsync_AlreadyConfirmed_ThrowsInvalidOperationException()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync(new SessionRegistration
            {
                SessionId = sessionId,
                UserId = userId,
                RegistrationStatus = "Confirmed"
            });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(sessionId));
    }

    [Fact]
    public async Task RegisterAsync_CancelledRegistration_ReactivatesRegistration()
    {
        var service = CreateService();

        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var registration = new SessionRegistration
        {
            SessionRegistrationId = Guid.NewGuid(),
            SessionId = sessionId,
            UserId = userId,
            RegistrationStatus = "Otkazano"
        };

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync(registration);

        await service.RegisterAsync(sessionId);

        Assert.Equal("Confirmed", registration.RegistrationStatus);
        _registrationRepositoryMock.Verify(r => r.UpdateAsync(registration), Times.Once);
        _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelRegistrationAsync_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();

        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var registrationId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(currentUserId.ToString());

        _registrationRepositoryMock
            .Setup(r => r.GetByIdAsync(registrationId))
            .ReturnsAsync(new SessionRegistration
            {
                SessionRegistrationId = registrationId,
                UserId = otherUserId,
                RegistrationStatus = "Confirmed"
            });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CancelRegistrationAsync(registrationId));
    }
}
