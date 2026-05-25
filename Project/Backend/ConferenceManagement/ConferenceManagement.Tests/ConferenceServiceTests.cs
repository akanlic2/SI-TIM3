using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class ConferenceServiceTests
{
    private readonly Mock<IConferenceRepository> _repositoryMock = new();
    private readonly Mock<IConferenceRegistrationRepository> _conferenceRegistrationRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();

    private ConferenceService CreateService() =>
        new(
            _repositoryMock.Object,
            _conferenceRegistrationRepositoryMock.Object,
            _userContextMock.Object,
            _userRepositoryMock.Object,
            _notificationServiceMock.Object
        );

    private static Conference ActiveConference => new()
    {
        ConferenceId = Guid.NewGuid(),
        Title = "AI Summit",
        Description = "Active conference",
        Location = "Sarajevo",
        Category = "IT",
        StartDate = DateTime.UtcNow.AddDays(10),
        EndDate = DateTime.UtcNow.AddDays(11),
        MaxParticipants = 100,
        Status = "Active",
        Organizers = new List<User>()
    };

    private static Conference DraftConference => new()
    {
        ConferenceId = Guid.NewGuid(),
        Title = "Draft Conference",
        Description = "Draft conference",
        Location = "Mostar",
        Category = "Business",
        StartDate = DateTime.UtcNow.AddDays(20),
        EndDate = DateTime.UtcNow.AddDays(21),
        MaxParticipants = 150,
        Status = "Draft",
        Organizers = new List<User>()
    };

    [Fact]
    public async Task GetPagedAsync_AdminSeesActiveDraftAndInactive()
    {
        var service = CreateService();

        _repositoryMock
            .Setup(x => x.GetPagedFilteredAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Conference> { ActiveConference, DraftConference }, 2));

        var result = await service.GetPagedAsync(new ConferenceQueryDto
        {
            Page = 1,
            PageSize = 6
        });

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetByIdAsync_AdminCanSeeDraftConference()
    {
        var service = CreateService();
        var draft = DraftConference;

        _repositoryMock
            .Setup(x => x.GetByIdWithOrganizersAsync(draft.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draft);

        var result = await service.GetByIdAsync(draft.ConferenceId);

        Assert.NotNull(result);
        Assert.Equal("Draft", result!.Status);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsConferenceDto()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { UserId = userId });

        var dto = new CreateConferenceDto
        {
            Title = "Test Konferencija",
            Description = "Opis konferencije",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Sarajevo",
            Category = "IT",
            MaxParticipants = 100
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Location, result.Location);
        Assert.Equal(dto.MaxParticipants, result.MaxParticipants);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidDates_ThrowsArgumentException()
    {
        var service = CreateService();

        var dto = new CreateConferenceDto
        {
            Title = "Test konferencija",
            Description = "Opis test konferencije",
            Location = "Sarajevo",
            Category = "IT",
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(4),
            MaxParticipants = 100
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_StartDateAfterEndDate_ThrowsArgumentException()
    {
        var service = CreateService();

        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Sarajevo",
            MaxParticipants = 50
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_StartDateEqualsEndDate_ThrowsArgumentException()
    {
        var service = CreateService();
        var date = DateTime.UtcNow.AddDays(2);

        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = date,
            EndDate = date,
            Location = "Sarajevo",
            MaxParticipants = 50
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MaxParticipantsZero_ThrowsArgumentException()
    {
        var service = CreateService();

        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Sarajevo",
            MaxParticipants = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MaxParticipantsNegative_ThrowsArgumentException()
    {
        var service = CreateService();

        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Sarajevo",
            MaxParticipants = -10
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_ValidData_UpdatesSuccessfully()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        var existing = new Conference
        {
            ConferenceId = id,
            Title = "Stari naziv",
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(6),
            Location = "Sarajevo"
        };

        var dto = new UpdateConferenceDto
        {
            Title = "Novi naziv",
            Description = "Novi opis",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(11),
            Location = "Mostar",
            MaxParticipants = 200
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _conferenceRegistrationRepositoryMock
            .Setup(r => r.GetRegistrationsByConferenceAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConferenceRegistration>());

        await service.UpdateAsync(id, dto);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ConferenceNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Conference?)null);

        var dto = new UpdateConferenceDto
        {
            Title = "Naziv",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            MaxParticipants = 50
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(id, dto));
    }

    [Fact]
    public async Task DeleteAsync_ExistingConference_DeletesSuccessfully()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        var conference = new Conference
        {
            ConferenceId = id,
            Title = "Test"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(conference);
        _conferenceRegistrationRepositoryMock
            .Setup(r => r.GetRegistrationsByConferenceAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConferenceRegistration>());

        await service.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ConferenceNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Conference?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(id));
    }
}