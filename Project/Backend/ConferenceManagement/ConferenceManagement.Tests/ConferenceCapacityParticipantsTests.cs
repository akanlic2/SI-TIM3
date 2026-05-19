using System.Security.Claims;
using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.DTOs.Conference;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class ConferenceCapacityParticipantsTests
{
    private const string AdminOrOrganizerPolicy = "AdminOrOrganizerPolicy";

    private readonly Mock<IConferenceRepository> _conferenceRepositoryMock = new();
    private readonly Mock<IConferenceRegistrationRepository> _registrationRepositoryMock = new();
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();

    private ConferenceCapacityService CreateService() =>
        new(
            _conferenceRepositoryMock.Object,
            _registrationRepositoryMock.Object,
            _sessionRepositoryMock.Object);

    [Fact]
    public void GetConferenceCapacity_UsesExpectedRoute()
    {
        var action = typeof(ConferenceCapacityController)
            .GetMethod(nameof(ConferenceCapacityController.GetConferenceCapacity));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpGetAttribute), inherit: false));
        var httpGetAttribute = Assert.IsType<HttpGetAttribute>(attribute);
        Assert.Equal("{id:guid}/capacity", httpGetAttribute.Template);
    }

    [Fact]
    public void GetSessionCapacity_UsesExpectedRoute()
    {
        var action = typeof(SessionsController)
            .GetMethod(nameof(SessionsController.GetSessionCapacity));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpGetAttribute), inherit: false));
        var httpGetAttribute = Assert.IsType<HttpGetAttribute>(attribute);
        Assert.Equal("{id:guid}/capacity", httpGetAttribute.Template);
    }

    [Fact]
    public void GetParticipants_UsesExpectedRoute()
    {
        var action = typeof(ConferenceCapacityController)
            .GetMethod(nameof(ConferenceCapacityController.GetParticipants));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpGetAttribute), inherit: false));
        var httpGetAttribute = Assert.IsType<HttpGetAttribute>(attribute);
        Assert.Equal("{id:guid}/participants", httpGetAttribute.Template);
    }

    [Theory]
    [InlineData(nameof(ConferenceCapacityController.GetConferenceCapacity))]
    [InlineData(nameof(ConferenceCapacityController.GetParticipants))]
    public void ConferenceCapacityControllerEndpoints_RequireAdminOrOrganizerPolicy(string actionName)
    {
        var action = typeof(ConferenceCapacityController).GetMethod(actionName);

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);
        Assert.Equal(AdminOrOrganizerPolicy, authorizeAttribute.Policy);
    }

    [Fact]
    public void GetSessionCapacity_RequiresAdminOrOrganizerPolicy()
    {
        var action = typeof(SessionsController)
            .GetMethod(nameof(SessionsController.GetSessionCapacity));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);
        Assert.Equal(AdminOrOrganizerPolicy, authorizeAttribute.Policy);
    }

    [Theory]
    [InlineData("admin-sistema", true)]
    [InlineData("organizator", true)]
    [InlineData("predavac", false)]
    [InlineData("ucesnik", false)]
    public async Task AdminOrOrganizerPolicy_AllowsOnlyExpectedRoles(string role, bool shouldAuthorize)
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

    [Fact]
    public async Task GetConferenceCapacityAsync_ReturnsRegisteredMaxAvailableAndFullStatus()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var conference = new Conference
        {
            ConferenceId = conferenceId,
            MaxParticipants = 10
        };

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conference);
        _registrationRepositoryMock
            .Setup(r => r.GetConfirmedCountForConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);

        var result = await service.GetConferenceCapacityAsync(conferenceId);

        Assert.Equal(10, result.MaxParticipants);
        Assert.Equal(7, result.RegisteredCount);
        Assert.Equal(3, result.AvailableSpots);
        Assert.False(result.IsFull);
    }

    [Fact]
    public async Task GetConferenceCapacityAsync_ReturnsIsFullWhenRegisteredCountReachesCapacity()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference { ConferenceId = conferenceId, MaxParticipants = 2 });
        _registrationRepositoryMock
            .Setup(r => r.GetConfirmedCountForConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var result = await service.GetConferenceCapacityAsync(conferenceId);

        Assert.True(result.IsFull);
        Assert.Equal(0, result.AvailableSpots);
    }

    [Fact]
    public async Task GetConferenceCapacityAsync_ConferenceDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conference?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetConferenceCapacityAsync(conferenceId));
    }

    [Fact]
    public async Task GetSessionCapacityAsync_ReturnsRegisteredMaxAvailableAndFullStatus()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            SessionId = sessionId,
            Conference = new Conference { MaxParticipants = 3 },
            SessionRegistrations = new List<SessionRegistration>
            {
                new() { RegistrationStatus = "Confirmed" },
                new() { RegistrationStatus = "confirmed" },
                new() { RegistrationStatus = "Cancelled" }
            }
        };

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync(session);

        var result = await service.GetSessionCapacityAsync(sessionId);

        Assert.Equal(3, result.MaxParticipants);
        Assert.Equal(2, result.RegisteredCount);
        Assert.Equal(1, result.AvailableSpots);
        Assert.False(result.IsFull);
    }

    [Fact]
    public async Task GetSessionCapacityAsync_ReturnsIsFullWhenSessionRegistrationCountReachesCapacity()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            SessionId = sessionId,
            Conference = new Conference { MaxParticipants = 1 },
            SessionRegistrations = new List<SessionRegistration>
            {
                new() { RegistrationStatus = "Confirmed" }
            }
        };

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync(session);

        var result = await service.GetSessionCapacityAsync(sessionId);

        Assert.True(result.IsFull);
        Assert.Equal(0, result.AvailableSpots);
    }

    [Fact]
    public async Task GetSessionCapacityAsync_SessionDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdWithRegistrationsAsync(sessionId))
            .ReturnsAsync((Session?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetSessionCapacityAsync(sessionId));
    }

    [Fact]
    public async Task GetConferenceParticipantsAsync_ReturnsRegisteredParticipantsWithNameEmailAndStatus()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var registrationDate = DateTime.UtcNow.AddDays(-1);

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference { ConferenceId = conferenceId });
        _registrationRepositoryMock
            .Setup(r => r.GetRegistrationsByConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConferenceRegistration>
            {
                CreateRegistration(conferenceId, "Ada", "Lovelace", "ada@example.com", "Confirmed", registrationDate)
            });

        var result = await service.GetConferenceParticipantsAsync(conferenceId, null, null);

        var participant = Assert.Single(result);
        Assert.Equal("Ada", participant.FirstName);
        Assert.Equal("Lovelace", participant.LastName);
        Assert.Equal("ada@example.com", participant.Email);
        Assert.Equal("Confirmed", participant.RegistrationStatus);
        Assert.Equal(registrationDate, participant.RegistrationDate);
    }

    [Fact]
    public async Task GetConferenceParticipantsAsync_SupportsSearchByNameAndEmail()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        SetupParticipants(conferenceId);

        var byName = await service.GetConferenceParticipantsAsync(conferenceId, "ada", null);
        var byEmail = await service.GetConferenceParticipantsAsync(conferenceId, "grace@example.com", null);

        Assert.Single(byName);
        Assert.Equal("Ada", byName[0].FirstName);
        Assert.Single(byEmail);
        Assert.Equal("Grace", byEmail[0].FirstName);
    }

    [Fact]
    public async Task GetConferenceParticipantsAsync_SupportsStatusFilter()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        SetupParticipants(conferenceId);

        var result = await service.GetConferenceParticipantsAsync(conferenceId, null, "confirmed");

        Assert.Single(result);
        Assert.Equal("Confirmed", result[0].RegistrationStatus);
    }

    [Fact]
    public async Task GetConferenceParticipantsAsync_ReturnsEmptyListWhenNoParticipants()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference { ConferenceId = conferenceId });
        _registrationRepositoryMock
            .Setup(r => r.GetRegistrationsByConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConferenceRegistration>());

        var result = await service.GetConferenceParticipantsAsync(conferenceId, null, null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetConferenceParticipantsAsync_ConferenceDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conference?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetConferenceParticipantsAsync(conferenceId, null, null));
    }

    [Fact]
    public async Task GetConferenceCapacity_AdminCanSeeAnyConferenceCapacity()
    {
        var conferenceId = Guid.NewGuid();
        var capacity = new CapacityDto { MaxParticipants = 20, RegisteredCount = 4, AvailableSpots = 16 };
        var capacityServiceMock = new Mock<IConferenceCapacityService>();
        var userContextMock = CreateUserContextMock(Guid.NewGuid(), isAdmin: true, isOrganizer: false);
        var conferenceRepositoryMock = new Mock<IConferenceRepository>();
        var controller = new ConferenceCapacityController(
            capacityServiceMock.Object,
            userContextMock.Object,
            conferenceRepositoryMock.Object);

        capacityServiceMock
            .Setup(s => s.GetConferenceCapacityAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(capacity);

        var response = await controller.GetConferenceCapacity(conferenceId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(capacity, okResult.Value);
        conferenceRepositoryMock.Verify(
            r => r.GetByIdWithOrganizersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetConferenceCapacity_OrganizerCanSeeOwnConferenceCapacity()
    {
        var organizerId = Guid.NewGuid();
        var conferenceId = Guid.NewGuid();
        var capacity = new CapacityDto { MaxParticipants = 20, RegisteredCount = 4, AvailableSpots = 16 };
        var capacityServiceMock = new Mock<IConferenceCapacityService>();
        var userContextMock = CreateUserContextMock(organizerId, isAdmin: false, isOrganizer: true);
        var conferenceRepositoryMock = new Mock<IConferenceRepository>();
        var controller = new ConferenceCapacityController(
            capacityServiceMock.Object,
            userContextMock.Object,
            conferenceRepositoryMock.Object);

        conferenceRepositoryMock
            .Setup(r => r.GetByIdWithOrganizersAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                Organizers = new List<User> { new() { UserId = organizerId } }
            });
        capacityServiceMock
            .Setup(s => s.GetConferenceCapacityAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(capacity);

        var response = await controller.GetConferenceCapacity(conferenceId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(capacity, okResult.Value);
    }

    [Fact]
    public async Task GetConferenceCapacity_OrganizerCannotSeeOtherConferenceCapacity()
    {
        var organizerId = Guid.NewGuid();
        var conferenceId = Guid.NewGuid();
        var capacityServiceMock = new Mock<IConferenceCapacityService>();
        var userContextMock = CreateUserContextMock(organizerId, isAdmin: false, isOrganizer: true);
        var conferenceRepositoryMock = new Mock<IConferenceRepository>();
        var controller = new ConferenceCapacityController(
            capacityServiceMock.Object,
            userContextMock.Object,
            conferenceRepositoryMock.Object);

        conferenceRepositoryMock
            .Setup(r => r.GetByIdWithOrganizersAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                Organizers = new List<User> { new() { UserId = Guid.NewGuid() } }
            });

        var response = await controller.GetConferenceCapacity(conferenceId, CancellationToken.None);

        Assert.IsType<ForbidResult>(response.Result);
        capacityServiceMock.Verify(
            s => s.GetConferenceCapacityAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetParticipants_AdminCanSeeParticipantsForAnyConference()
    {
        var conferenceId = Guid.NewGuid();
        var participants = new List<ParticipantDto>
        {
            new() { FirstName = "Ada", Email = "ada@example.com", RegistrationStatus = "Confirmed" }
        };
        var capacityServiceMock = new Mock<IConferenceCapacityService>();
        var userContextMock = CreateUserContextMock(Guid.NewGuid(), isAdmin: true, isOrganizer: false);
        var conferenceRepositoryMock = new Mock<IConferenceRepository>();
        var controller = new ConferenceCapacityController(
            capacityServiceMock.Object,
            userContextMock.Object,
            conferenceRepositoryMock.Object);

        capacityServiceMock
            .Setup(s => s.GetConferenceParticipantsAsync(conferenceId, "ada", "confirmed", It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);

        var response = await controller.GetParticipants(conferenceId, "ada", "confirmed", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(participants, okResult.Value);
    }

    [Fact]
    public async Task GetParticipants_OrganizerCanSeeOwnConferenceParticipants()
    {
        var organizerId = Guid.NewGuid();
        var conferenceId = Guid.NewGuid();
        var participants = new List<ParticipantDto>();
        var capacityServiceMock = new Mock<IConferenceCapacityService>();
        var userContextMock = CreateUserContextMock(organizerId, isAdmin: false, isOrganizer: true);
        var conferenceRepositoryMock = new Mock<IConferenceRepository>();
        var controller = new ConferenceCapacityController(
            capacityServiceMock.Object,
            userContextMock.Object,
            conferenceRepositoryMock.Object);

        conferenceRepositoryMock
            .Setup(r => r.GetByIdWithOrganizersAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                Organizers = new List<User> { new() { UserId = organizerId } }
            });
        capacityServiceMock
            .Setup(s => s.GetConferenceParticipantsAsync(conferenceId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);

        var response = await controller.GetParticipants(conferenceId, null, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(participants, okResult.Value);
    }

    [Fact]
    public async Task GetParticipants_OrganizerCannotSeeOtherConferenceParticipants()
    {
        var organizerId = Guid.NewGuid();
        var conferenceId = Guid.NewGuid();
        var capacityServiceMock = new Mock<IConferenceCapacityService>();
        var userContextMock = CreateUserContextMock(organizerId, isAdmin: false, isOrganizer: true);
        var conferenceRepositoryMock = new Mock<IConferenceRepository>();
        var controller = new ConferenceCapacityController(
            capacityServiceMock.Object,
            userContextMock.Object,
            conferenceRepositoryMock.Object);

        conferenceRepositoryMock
            .Setup(r => r.GetByIdWithOrganizersAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference
            {
                ConferenceId = conferenceId,
                Organizers = new List<User> { new() { UserId = Guid.NewGuid() } }
            });

        var response = await controller.GetParticipants(conferenceId, null, null, CancellationToken.None);

        Assert.IsType<ForbidResult>(response.Result);
        capacityServiceMock.Verify(
            s => s.GetConferenceParticipantsAsync(
                It.IsAny<Guid>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void SetupParticipants(Guid conferenceId)
    {
        _conferenceRepositoryMock
            .Setup(r => r.GetByIdAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conference { ConferenceId = conferenceId });
        _registrationRepositoryMock
            .Setup(r => r.GetRegistrationsByConferenceAsync(conferenceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConferenceRegistration>
            {
                CreateRegistration(conferenceId, "Ada", "Lovelace", "ada@example.com", "Confirmed", DateTime.UtcNow),
                CreateRegistration(conferenceId, "Grace", "Hopper", "grace@example.com", "Cancelled", DateTime.UtcNow)
            });
    }

    private static ConferenceRegistration CreateRegistration(
        Guid conferenceId,
        string firstName,
        string lastName,
        string email,
        string status,
        DateTime registrationDate)
    {
        var userId = Guid.NewGuid();
        return new ConferenceRegistration
        {
            ConferenceRegistrationId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            UserId = userId,
            User = new User
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email
            },
            RegistrationStatus = status,
            RegistrationDate = registrationDate
        };
    }

    private static Mock<IUserContextService> CreateUserContextMock(
        Guid userId,
        bool isAdmin,
        bool isOrganizer)
    {
        var roles = new List<string>();
        if (isAdmin)
        {
            roles.Add("admin-sistema");
        }

        if (isOrganizer)
        {
            roles.Add("organizator");
        }

        var userContextMock = new Mock<IUserContextService>();
        userContextMock.Setup(c => c.GetUserId()).Returns(userId.ToString());
        userContextMock.Setup(c => c.GetUserRoles()).Returns(roles);
        userContextMock.Setup(c => c.HasRole("admin-sistema")).Returns(isAdmin);
        userContextMock.Setup(c => c.HasRole("organizator")).Returns(isOrganizer);

        return userContextMock;
    }
}
