using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class ConferenceServiceTests
{
    private readonly Mock<IConferenceRepository> _repositoryMock;
    private readonly Mock<IConferenceRegistrationRepository> _conferenceRegistrationRepositoryMock;
    private readonly Mock<IUserContextService> _userContextMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ConferenceService _service;

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

    public ConferenceServiceTests()
    {
        _repositoryMock = new Mock<IConferenceRepository>();
        _conferenceRegistrationRepositoryMock = new Mock<IConferenceRegistrationRepository>();
        _userContextMock = new Mock<IUserContextService>();
        _userRepositoryMock = new Mock<IUserRepository>();  

        // Servis sada prima oba dependency-a kako zahtijeva tvoj kod
        _service = new ConferenceService(
            _repositoryMock.Object,
            _conferenceRegistrationRepositoryMock.Object,
            _userContextMock.Object,
            _userRepositoryMock.Object);
    }

    // ===================== GET & AUTHORIZATION (Tvoji testovi) =====================

    [Fact]
    public async Task GetPagedAsync_AdminSeesActiveDraftAndInactive()
    {
        _userContextMock
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { "admin-sistema" });

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
            .ReturnsAsync((new List<Conference>
            {
                ActiveConference,
                DraftConference
            }, 2));

        var result = await _service.GetPagedAsync(new ConferenceQueryDto
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
        var draft = DraftConference;

        _userContextMock
            .Setup(x => x.GetUserRoles())
            .Returns(new List<string> { "admin-sistema" });

        _repositoryMock
            .Setup(x => x.GetByIdAsync(draft.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(draft);

        var result = await _service.GetByIdAsync(draft.ConferenceId);

        Assert.NotNull(result);
        Assert.Equal("Draft", result!.Status);
    }

    // ===================== CREATE (Kombinovani testovi) =====================

    [Fact]
public async Task CreateAsync_ValidData_ReturnsConferenceDto()
{
    var fakeUser = new User { UserId = Guid.NewGuid() };

    _userContextMock
        .Setup(x => x.GetUserId())
        .Returns(Guid.NewGuid().ToString());

    _userContextMock
        .Setup(x => x.HasRole("organizator"))
        .Returns(false);

    _userRepositoryMock
        .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(fakeUser);

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

    var conference = new Conference
    {
        ConferenceId = Guid.NewGuid(),
        Title = dto.Title,
        Description = dto.Description,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Location = dto.Location,
        Category = dto.Category,
        MaxParticipants = dto.MaxParticipants,
        Status = "Planned"
    };

    _repositoryMock
        .Setup(r => r.AddAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(conference);

    var result = await _service.CreateAsync(dto);

    Assert.NotNull(result);
    Assert.Equal(dto.Title, result.Title);
    Assert.Equal(dto.Location, result.Location);
    Assert.Equal(dto.MaxParticipants, result.MaxParticipants);
}

    [Fact]
    public async Task CreateAsync_InvalidDates_ThrowsArgumentException()
    {
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

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_StartDateAfterEndDate_ThrowsArgumentException()
    {
        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Sarajevo",
            MaxParticipants = 50
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_StartDateEqualsEndDate_ThrowsArgumentException()
    {
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

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MaxParticipantsZero_ThrowsArgumentException()
    {
        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Sarajevo",
            MaxParticipants = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MaxParticipantsNegative_ThrowsArgumentException()
    {
        var dto = new CreateConferenceDto
        {
            Title = "Test",
            Description = "Opis",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Sarajevo",
            MaxParticipants = -10
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    // ===================== UPDATE =====================

    [Fact]
    public async Task UpdateAsync_ValidData_UpdatesSuccessfully()
    {
        var id = Guid.NewGuid();
        var existing = new Conference { ConferenceId = id, Title = "Stari naziv" };

        var dto = new UpdateConferenceDto
        {
            Title = "Novi naziv",
            Description = "Novi opis",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3),
            Location = "Mostar",
            MaxParticipants = 200
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        await _service.UpdateAsync(id, dto);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ConferenceNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conference?)null);

        var dto = new UpdateConferenceDto
        {
            Title = "Naziv",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            MaxParticipants = 50
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, dto));
    }

    // ===================== DELETE =====================

    [Fact]
    public async Task DeleteAsync_ExistingConference_DeletesSuccessfully()
    {
        var id = Guid.NewGuid();
        var conference = new Conference { ConferenceId = id, Title = "Test" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conference);

        _repositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Conference>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ConferenceNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conference?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(id));
    }
}
