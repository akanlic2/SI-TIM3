using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class ConferenceRegistrationServiceTests
{
    private readonly Mock<IConferenceRepository> _conferenceRepositoryMock = new();
    private readonly Mock<IConferenceRegistrationRepository> _registrationRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();

    private ConferenceRegistrationService CreateService() =>
        new(
            _conferenceRepositoryMock.Object,
            _registrationRepositoryMock.Object,
            _userContextMock.Object
        );

    [Fact]
    public async Task RegisterAsync_ConferenceNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conference?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.RegisterAsync(conferenceId));
    }

    [Fact]
    public async Task RegisterAsync_UserAlreadyConfirmed_ThrowsInvalidOperationException()
    {
        var service = CreateService();

        var conferenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                MaxParticipants = 100
            });

        _registrationRepositoryMock
            .Setup(r => r.GetByConferenceAndUserAsync(conferenceId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConferenceRegistration
            {
                ConferenceId = conferenceId,
                UserId = userId,
                RegistrationStatus = "Confirmed"
            });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(conferenceId));
    }

    [Fact]
    public async Task RegisterAsync_NoFreePlaces_ThrowsInvalidOperationException()
    {
        var service = CreateService();

        var conferenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                MaxParticipants = 1
            });

        _registrationRepositoryMock
            .Setup(r => r.GetByConferenceAndUserAsync(conferenceId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConferenceRegistration?)null);

        _registrationRepositoryMock
            .Setup(r => r.GetConfirmedCountForConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(conferenceId));
    }

    [Fact]
    public async Task RegisterAsync_ValidRegistration_AddsRegistration()
    {
        var service = CreateService();

        var conferenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                MaxParticipants = 100
            });

        _registrationRepositoryMock
            .Setup(r => r.GetByConferenceAndUserAsync(conferenceId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConferenceRegistration?)null);

        _registrationRepositoryMock
            .Setup(r => r.GetConfirmedCountForConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await service.RegisterAsync(conferenceId);

        _registrationRepositoryMock.Verify(r => r.AddAsync(It.Is<ConferenceRegistration>(
            cr => cr.ConferenceId == conferenceId &&
                  cr.UserId == userId &&
                  cr.RegistrationStatus == "Confirmed"
        ), It.IsAny<CancellationToken>()), Times.Once);

        _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_CancelledRegistration_ReactivatesRegistration()
    {
        var service = CreateService();

        var conferenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var registration = new ConferenceRegistration
        {
            ConferenceRegistrationId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            UserId = userId,
            RegistrationStatus = "Cancelled"
        };

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                MaxParticipants = 100
            });

        _registrationRepositoryMock
            .Setup(r => r.GetByConferenceAndUserAsync(conferenceId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registration);

        _registrationRepositoryMock
            .Setup(r => r.GetConfirmedCountForConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        await service.RegisterAsync(conferenceId);

        Assert.Equal("Confirmed", registration.RegistrationStatus);
        _registrationRepositoryMock.Verify(r => r.UpdateAsync(registration, It.IsAny<CancellationToken>()), Times.Once);
        _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_RegistrationNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var registrationId = Guid.NewGuid();

        _registrationRepositoryMock
            .Setup(r => r.GetByIdAsync(registrationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConferenceRegistration?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CancelAsync(registrationId));
    }

    [Fact]
    public async Task CancelAsync_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();

        var currentUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var registrationId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(currentUserId.ToString());

        _registrationRepositoryMock
            .Setup(r => r.GetByIdAsync(registrationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConferenceRegistration
            {
                ConferenceRegistrationId = registrationId,
                UserId = otherUserId,
                RegistrationStatus = "Confirmed"
            });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CancelAsync(registrationId));
    }

    [Fact]
    public async Task CancelAsync_ValidRegistration_SetsCancelledStatus()
    {
        var service = CreateService();

        var userId = Guid.NewGuid();
        var registrationId = Guid.NewGuid();

        var registration = new ConferenceRegistration
        {
            ConferenceRegistrationId = registrationId,
            UserId = userId,
            RegistrationStatus = "Confirmed"
        };

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _registrationRepositoryMock
            .Setup(r => r.GetByIdAsync(registrationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(registration);

        await service.CancelAsync(registrationId);

        Assert.Equal("Cancelled", registration.RegistrationStatus);
        _registrationRepositoryMock.Verify(r => r.UpdateAsync(registration, It.IsAny<CancellationToken>()), Times.Once);
        _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
