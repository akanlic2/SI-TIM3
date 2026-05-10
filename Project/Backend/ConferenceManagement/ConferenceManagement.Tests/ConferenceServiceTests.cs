using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class ConferenceServiceTests
{
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
        Status = "Active"
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
        Status = "Draft"
    };

    [Fact]
    public async Task GetPagedAsync_AdminSeesActiveDraftAndInactive()
    {
        var repositoryMock = new Mock<IConferenceRepository>();
        var userContextMock = new Mock<IUserContextService>();

        userContextMock
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { "admin-sistema" });

        repositoryMock
            .Setup(x => x.GetPagedFilteredAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Conference>
            {
                ActiveConference,
                DraftConference
            }, 2));

        var service = new ConferenceService(repositoryMock.Object, userContextMock.Object);

        var result = await service.GetPagedAsync(new ConferenceQueryDto
        {
            Page = 1,
            PageSize = 6
        });

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetPagedAsync_NonAdminSeesOnlyActive()
    {
        var repositoryMock = new Mock<IConferenceRepository>();
        var userContextMock = new Mock<IUserContextService>();

        userContextMock
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { "ucesnik" });

        repositoryMock
            .Setup(x => x.GetPagedFilteredAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Conference>
            {
                ActiveConference
            }, 1));

        var service = new ConferenceService(repositoryMock.Object, userContextMock.Object);

        var result = await service.GetPagedAsync(new ConferenceQueryDto
        {
            Page = 1,
            PageSize = 6
        });

        Assert.Single(result.Items);
        Assert.Equal("Active", result.Items[0].Status);
    }

    [Fact]
    public async Task GetByIdAsync_NonAdminCannotSeeDraftConference()
    {
        var draft = DraftConference;

        var repositoryMock = new Mock<IConferenceRepository>();
        var userContextMock = new Mock<IUserContextService>();

        userContextMock
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { "ucesnik" });

        repositoryMock
            .Setup(x => x.GetByIdAsync(draft.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draft);

        var service = new ConferenceService(repositoryMock.Object, userContextMock.Object);

        var result = await service.GetByIdAsync(draft.ConferenceId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_AdminCanSeeDraftConference()
    {
        var draft = DraftConference;

        var repositoryMock = new Mock<IConferenceRepository>();
        var userContextMock = new Mock<IUserContextService>();

        userContextMock
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { "admin-sistema" });

        repositoryMock
            .Setup(x => x.GetByIdAsync(draft.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draft);

        var service = new ConferenceService(repositoryMock.Object, userContextMock.Object);

        var result = await service.GetByIdAsync(draft.ConferenceId);

        Assert.NotNull(result);
        Assert.Equal("Draft", result!.Status);
    }

    [Fact]
    public async Task CreateAsync_InvalidDates_ThrowsArgumentException()
    {
        var repositoryMock = new Mock<IConferenceRepository>();
        var userContextMock = new Mock<IUserContextService>();

        var service = new ConferenceService(repositoryMock.Object, userContextMock.Object);

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
}
