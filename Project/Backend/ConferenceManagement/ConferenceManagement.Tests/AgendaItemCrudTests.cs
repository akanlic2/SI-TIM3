using System.Security.Claims;
using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.DTOs.Agenda;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class AgendaItemCrudTests
{
    private const string AdminOrOrganizerPolicy = "AdminOrOrganizerPolicy";
    private const string ParticipantPolicy = "ParticipantPolicy";

    private readonly Mock<IAgendaItemRepository> _agendaRepositoryMock = new();
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();
    private readonly Mock<IConferenceRepository> _conferenceRepositoryMock = new();

    public static IEnumerable<object[]> SupportedNonSessionTypes =>
        new List<object[]>
        {
            new object[] { "Break" },
            new object[] { "Lunch" },
            new object[] { "Networking" },
            new object[] { "Opening" },
            new object[] { "Closing" }
        };

    private AgendaItemService CreateService() =>
        new(
            _agendaRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _conferenceRepositoryMock.Object);

    [Fact]
    public void GetByConference_UsesExpectedRoute()
    {
        var action = typeof(AgendaController).GetMethod(nameof(AgendaController.GetByConference));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpGetAttribute), inherit: false));
        var httpGetAttribute = Assert.IsType<HttpGetAttribute>(attribute);
        Assert.Equal("conferences/{conferenceId:guid}/agenda", httpGetAttribute.Template);
    }

    [Fact]
    public void Create_UsesExpectedRoute()
    {
        var action = typeof(AgendaController).GetMethod(nameof(AgendaController.Create));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpPostAttribute), inherit: false));
        var httpPostAttribute = Assert.IsType<HttpPostAttribute>(attribute);
        Assert.Equal("conferences/{conferenceId:guid}/agenda", httpPostAttribute.Template);
    }

    [Fact]
    public void Update_UsesExpectedRoute()
    {
        var action = typeof(AgendaController).GetMethod(nameof(AgendaController.Update));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpPutAttribute), inherit: false));
        var httpPutAttribute = Assert.IsType<HttpPutAttribute>(attribute);
        Assert.Equal("agenda/{id:guid}", httpPutAttribute.Template);
    }

    [Fact]
    public void Delete_UsesExpectedRoute()
    {
        var action = typeof(AgendaController).GetMethod(nameof(AgendaController.Delete));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpDeleteAttribute), inherit: false));
        var httpDeleteAttribute = Assert.IsType<HttpDeleteAttribute>(attribute);
        Assert.Equal("agenda/{id:guid}", httpDeleteAttribute.Template);
    }

    [Theory]
    [InlineData(nameof(AgendaController.Create))]
    [InlineData(nameof(AgendaController.Update))]
    [InlineData(nameof(AgendaController.Delete))]
    public void MutatingActions_RequireAdminOrOrganizerPolicy(string actionName)
    {
        var action = typeof(AgendaController).GetMethod(actionName);

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);
        Assert.Equal(AdminOrOrganizerPolicy, authorizeAttribute.Policy);
    }

    [Fact]
    public void GetByConference_RequiresParticipantPolicyInCurrentImplementation()
    {
        var action = typeof(AgendaController).GetMethod(nameof(AgendaController.GetByConference));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);
        Assert.Equal(ParticipantPolicy, authorizeAttribute.Policy);
    }

    [Theory]
    [InlineData("admin-sistema", true)]
    [InlineData("organizator", true)]
    [InlineData("predavac", false)]
    [InlineData("ucesnik", false)]
    public async Task AdminOrOrganizerPolicy_AllowsOnlyExpectedRolesForCrud(string role, bool shouldAuthorize)
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireRole("admin-sistema", "organizator")
            .Build();

        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, role) },
            authenticationType: "TestAuth"));

        var context = new AuthorizationHandlerContext(policy.Requirements, user, resource: null);
        var rolesRequirement = Assert.IsType<RolesAuthorizationRequirement>(Assert.Single(policy.Requirements));

        await rolesRequirement.HandleAsync(context);

        Assert.Equal(shouldAuthorize, context.HasSucceeded);
    }

    [Theory]
    [MemberData(nameof(SupportedNonSessionTypes))]
    public async Task CreateAsync_SupportedNonSessionTypes_CreateAgendaItem(string type)
    {
        var service = CreateService();
        var conference = CreateConference();
        var dto = new CreateAgendaItemDto
        {
            Type = type,
            Title = $"{type} title",
            Description = $"{type} description",
            StartTime = conference.StartDate.AddHours(1),
            EndTime = conference.StartDate.AddHours(2)
        };

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conference.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conference);

        var result = await service.CreateAsync(conference.ConferenceId, dto);

        Assert.Equal(type, result.Type);
        Assert.Equal(dto.Title, result.Title);
        Assert.Null(result.SessionId);
        _agendaRepositoryMock.Verify(r => r.AddAsync(It.Is<AgendaItem>(item =>
            item.ConferenceId == conference.ConferenceId &&
            item.Type == type &&
            item.Title == dto.Title &&
            item.SessionId == null)), Times.Once);
        _agendaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SessionTypeWithoutSessionId_ThrowsArgumentException()
    {
        var service = CreateService();
        var conference = CreateConference();
        var dto = new CreateAgendaItemDto
        {
            Type = "Session",
            StartTime = conference.StartDate.AddHours(1),
            EndTime = conference.StartDate.AddHours(2)
        };

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conference.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conference);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(conference.ConferenceId, dto));
        _agendaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AgendaItem>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_SessionTypeWithExistingSession_CreatesAgendaItem()
    {
        var service = CreateService();
        var conference = CreateConference();
        var session = CreateSession(conference.ConferenceId, conference.StartDate.AddHours(2), conference.StartDate.AddHours(3));
        var dto = new CreateAgendaItemDto
        {
            Type = "Session",
            SessionId = session.SessionId,
            StartTime = session.StartTime,
            EndTime = session.EndTime
        };

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conference.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conference);
        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(session.SessionId))
            .ReturnsAsync(session);

        var result = await service.CreateAsync(conference.ConferenceId, dto);

        Assert.Equal("Session", result.Type);
        Assert.Equal(session.SessionId, result.SessionId);
        _agendaRepositoryMock.Verify(r => r.AddAsync(It.Is<AgendaItem>(item =>
            item.SessionId == session.SessionId &&
            item.StartTime == session.StartTime.ToUniversalTime() &&
            item.EndTime == session.EndTime.ToUniversalTime())), Times.Once);
        _agendaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByConferenceIdAsync_ReturnsMappedAgendaItemsIncludingSessionData()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var speaker = new User { FirstName = "Ada", LastName = "Lovelace" };
        var session = new Session
        {
            SessionId = Guid.NewGuid(),
            Title = "Keynote",
            SessionType = "Lecture",
            SessionRegistrations = new List<SessionRegistration>
            {
                new() { IsSpeaker = true, User = speaker }
            }
        };
        var item = new AgendaItem
        {
            AgendaItemId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            SessionId = session.SessionId,
            Session = session,
            Title = "Keynote",
            Description = "Opening talk",
            Type = "Session",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Room = new Room { Name = "A1" }
        };

        _agendaRepositoryMock
            .Setup(r => r.GetByConferenceIdAsync(conferenceId))
            .ReturnsAsync(new[] { item });

        var result = await service.GetByConferenceIdAsync(conferenceId);

        var dto = Assert.Single(result);
        Assert.Equal(session.Title, dto.SessionTitle);
        Assert.Equal(session.SessionType, dto.SessionType);
        Assert.Equal("Ada Lovelace", dto.SpeakerName);
        Assert.Equal("A1", dto.RoomName);
    }

    [Fact]
    public async Task UpdateAsync_ExistingAgendaItem_ChangesTimeTitleDescriptionAndType()
    {
        var service = CreateService();
        var conference = CreateConference();
        var agendaItem = new AgendaItem
        {
            AgendaItemId = Guid.NewGuid(),
            ConferenceId = conference.ConferenceId,
            Type = "Break",
            Title = "Old",
            Description = "Old description",
            StartTime = conference.StartDate.AddHours(1),
            EndTime = conference.StartDate.AddHours(2)
        };
        var dto = new UpdateAgendaItemDto
        {
            Type = "Lunch",
            Title = "Lunch",
            Description = "Lunch break",
            StartTime = conference.StartDate.AddHours(3),
            EndTime = conference.StartDate.AddHours(4)
        };

        _agendaRepositoryMock
            .Setup(r => r.GetByIdAsync(agendaItem.AgendaItemId))
            .ReturnsAsync(agendaItem);
        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conference.ConferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conference);

        await service.UpdateAsync(agendaItem.AgendaItemId, dto);

        Assert.Equal(dto.Type, agendaItem.Type);
        Assert.Equal(dto.Title, agendaItem.Title);
        Assert.Equal(dto.Description, agendaItem.Description);
        Assert.Equal(dto.StartTime.ToUniversalTime(), agendaItem.StartTime);
        Assert.Equal(dto.EndTime.ToUniversalTime(), agendaItem.EndTime);
        _agendaRepositoryMock.Verify(r => r.UpdateAsync(agendaItem), Times.Once);
        _agendaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_AgendaItemDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        _agendaRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((AgendaItem?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateAsync(id, new UpdateAgendaItemDto()));
    }

    [Fact]
    public async Task DeleteAsync_ExistingAgendaItem_DeletesAgendaItem()
    {
        var service = CreateService();
        var agendaItem = new AgendaItem { AgendaItemId = Guid.NewGuid() };

        _agendaRepositoryMock
            .Setup(r => r.GetByIdAsync(agendaItem.AgendaItemId))
            .ReturnsAsync(agendaItem);

        await service.DeleteAsync(agendaItem.AgendaItemId);

        _agendaRepositoryMock.Verify(r => r.DeleteAsync(agendaItem), Times.Once);
        _agendaRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_AgendaItemDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        _agendaRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((AgendaItem?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteAsync(id));
    }

    private static Conference CreateConference()
    {
<<<<<<< HEAD
        var start = DateTime.UtcNow.Date.AddDays(10).AddHours(9);
=======
        var start = DateTime.SpecifyKind(DateTime.Now.Date.AddDays(10).AddHours(9), DateTimeKind.Local);
>>>>>>> 955ac0957ae0345a6d233a3f54cb5a8249a8d4f9
        return new Conference
        {
            ConferenceId = Guid.NewGuid(),
            Title = "Conference",
            StartDate = start,
            EndDate = start.AddDays(2)
        };
    }

    private static Session CreateSession(Guid conferenceId, DateTime start, DateTime end) =>
        new()
        {
            SessionId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            Title = "Session title",
            Description = "Session description",
            StartTime = start,
            EndTime = end,
            SessionType = "Lecture"
        };
}
