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
    private readonly ConferenceService _service;

    public ConferenceServiceTests()
    {
        _repositoryMock = new Mock<IConferenceRepository>();
        _service = new ConferenceService(_repositoryMock.Object);
    }

    // ===================== CREATE =====================

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsConferenceDto()
    {
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