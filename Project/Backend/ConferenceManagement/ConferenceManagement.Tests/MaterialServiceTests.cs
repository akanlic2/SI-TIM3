using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class MaterialServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();
    private readonly Mock<ISessionRegistrationRepository> _registrationRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly Mock<IMaterialRepository> _materialRepositoryMock = new();

    private MaterialService CreateService() =>
        new(
            _sessionRepositoryMock.Object,
            _registrationRepositoryMock.Object,
            _userContextMock.Object,
            _materialRepositoryMock.Object
        );

    private static IFormFile CreateFakeFile(
        string fileName = "test.pdf",
        string contentType = "application/pdf")
    {
        var content = "fake file content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public async Task UploadMaterialAsync_SessionNotFound_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        var file = CreateFakeFile();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UploadMaterialAsync(sessionId, file, "Materijal", "Opis", CancellationToken.None));
    }

    [Fact]
    public async Task UploadMaterialAsync_AttendeeWithoutPermission_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
        _userContextMock.Setup(x => x.HasAnyRole("admin-sistema", "organizator")).Returns(false);
        _userContextMock.Setup(x => x.HasRole("predavac")).Returns(false);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                Title = "Test sesija"
            });

        var file = CreateFakeFile();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UploadMaterialAsync(sessionId, file, "Materijal", "Opis", CancellationToken.None));
    }

    [Fact]
    public async Task UploadMaterialAsync_SpeakerNotAssignedToSession_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
        _userContextMock.Setup(x => x.HasAnyRole("admin-sistema", "organizator")).Returns(false);
        _userContextMock.Setup(x => x.HasRole("predavac")).Returns(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                Title = "Test sesija"
            });

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync((SessionRegistration?)null);

        var file = CreateFakeFile();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UploadMaterialAsync(sessionId, file, "Materijal", "Opis", CancellationToken.None));
    }

    [Fact]
    public async Task UploadMaterialAsync_AssignedSpeaker_UploadsMaterial()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
        _userContextMock.Setup(x => x.HasAnyRole("admin-sistema", "organizator")).Returns(false);
        _userContextMock.Setup(x => x.HasRole("predavac")).Returns(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                Title = "Test sesija"
            });

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync(new SessionRegistration
            {
                SessionId = sessionId,
                UserId = userId,
                IsSpeaker = true
            });

        var file = CreateFakeFile();

        var result = await service.UploadMaterialAsync(
            sessionId,
            file,
            "Materijal za sesiju",
            "Opis materijala",
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);

        _materialRepositoryMock.Verify(r => r.AddAsync(It.Is<Material>(
            m => m.SessionId == sessionId &&
                 m.Title == "Materijal za sesiju" &&
                 m.Description == "Opis materijala" &&
                 m.MaterialType == "application/pdf"
        ), It.IsAny<CancellationToken>()), Times.Once);

        _materialRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadMaterialAsync_AdminOrOrganizer_UploadsMaterial()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
        _userContextMock.Setup(x => x.HasAnyRole("admin-sistema", "organizator")).Returns(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session
            {
                SessionId = sessionId,
                Title = "Test sesija"
            });

        var file = CreateFakeFile("prezentacija.pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");

        var result = await service.UploadMaterialAsync(
            sessionId,
            file,
            "Prezentacija",
            "Opis prezentacije",
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);

        _materialRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Material>(), It.IsAny<CancellationToken>()), Times.Once);
        _materialRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMaterialsBySessionIdAsync_UserNotRegisteredAndNotAdmin_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
        _userContextMock.Setup(x => x.HasAnyRole("admin-sistema", "organizator")).Returns(false);

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync((SessionRegistration?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetMaterialsBySessionIdAsync(sessionId, CancellationToken.None));
    }

    [Fact]
    public async Task GetMaterialsBySessionIdAsync_RegisteredUser_ReturnsMaterials()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync(new SessionRegistration
            {
                SessionId = sessionId,
                UserId = userId,
                RegistrationStatus = "Confirmed"
            });

        _materialRepositoryMock
            .Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Material>
            {
                new Material
                {
                    MaterialId = Guid.NewGuid(),
                    SessionId = sessionId,
                    Title = "PDF materijal",
                    FileUrl = "/uploads/materials/test.pdf",
                    MaterialType = "application/pdf",
                    UploadDate = DateTime.UtcNow
                }
            });

        var result = await service.GetMaterialsBySessionIdAsync(sessionId, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("PDF materijal", result[0].Title);
        Assert.Equal("application/pdf", result[0].MaterialType);
    }

    [Fact]
    public async Task GetMaterialsBySessionIdAsync_AdminOrOrganizer_ReturnsMaterials()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContextMock.Setup(x => x.GetUserId()).Returns(userId.ToString());
        _userContextMock.Setup(x => x.HasAnyRole("admin-sistema", "organizator")).Returns(true);

        _registrationRepositoryMock
            .Setup(r => r.GetBySessionAndUserAsync(sessionId, userId))
            .ReturnsAsync((SessionRegistration?)null);

        _materialRepositoryMock
            .Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Material>());

        var result = await service.GetMaterialsBySessionIdAsync(sessionId, CancellationToken.None);

        Assert.Empty(result);
    }
}