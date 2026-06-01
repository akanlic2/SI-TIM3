using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.DTOs.Logistics;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class LogisticsTaskTests
{
    private const string AdminRole = "admin-sistema";
    private const string OrganizerRole = "organizator";
    private const string SpeakerRole = "predavac";
    private const string AttendeeRole = "ucesnik";
    private const string AdminOrOrganizerPolicy = "AdminOrOrganizerPolicy";

    private readonly Mock<ILogisticsRepository> _repositoryMock = new();
    private readonly Mock<ILogisticsService> _serviceMock = new();

    private LogisticsService CreateService() => new(_repositoryMock.Object);

    [Fact]
    public void Controller_UsesApiRoutePrefix()
    {
        var attribute = Assert.Single(typeof(LogisticsController)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false));
        var route = Assert.IsType<RouteAttribute>(attribute);

        Assert.Equal("api", route.Template);
    }

    [Theory]
    [InlineData(nameof(LogisticsController.GetConferenceLogistics), typeof(HttpGetAttribute), "conferences/{id}/logistics")]
    [InlineData(nameof(LogisticsController.CreateLogisticsTask), typeof(HttpPostAttribute), "conferences/{id}/logistics")]
    [InlineData(nameof(LogisticsController.UpdateLogisticsTask), typeof(HttpPutAttribute), "logistics/{id}")]
    [InlineData(nameof(LogisticsController.DeleteLogisticsTask), typeof(HttpDeleteAttribute), "logistics/{id}")]
    public void ControllerActions_UseExpectedRoutes(string actionName, Type httpAttributeType, string expectedTemplate)
    {
        var action = typeof(LogisticsController).GetMethod(actionName);

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(httpAttributeType, inherit: false));
        var template = attribute switch
        {
            HttpGetAttribute a => a.Template,
            HttpPostAttribute a => a.Template,
            HttpPutAttribute a => a.Template,
            HttpDeleteAttribute a => a.Template,
            _ => null
        };

        Assert.Equal(expectedTemplate, template);
    }

    [Fact]
    public void LogisticsController_RequiresAdminOrOrganizerPolicy()
    {
        var attribute = Assert.Single(typeof(LogisticsController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);

        Assert.Equal(AdminOrOrganizerPolicy, authorizeAttribute.Policy);
    }

    [Theory]
    [InlineData(AdminRole, true)]
    [InlineData(OrganizerRole, true)]
    [InlineData(SpeakerRole, false)]
    [InlineData(AttendeeRole, false)]
    public async Task AdminOrOrganizerPolicy_AllowsOnlyAdminAndOrganizer(string role, bool shouldAuthorize)
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireRole(AdminRole, OrganizerRole)
            .Build();
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, role) },
            authenticationType: "TestAuth"));
        var context = new AuthorizationHandlerContext(policy.Requirements, user, resource: null);
        var requirement = Assert.IsType<RolesAuthorizationRequirement>(Assert.Single(policy.Requirements));

        await requirement.HandleAsync(context);

        Assert.Equal(shouldAuthorize, context.HasSucceeded);
    }

    [Fact]
    public async Task GetLogisticsForConferenceAsync_Admin_ReturnsMappedTasks()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var task = CreateTask(conferenceId, "Catering");

        _repositoryMock
            .Setup(r => r.GetByConferenceIdAsync(conferenceId, null))
            .ReturnsAsync(new[] { task });

        var result = await service.GetLogisticsForConferenceAsync(
            conferenceId, null, Guid.NewGuid(), AdminRole);

        var dto = Assert.Single(result);
        Assert.Equal(task.LogisticsTaskId, dto.LogisticsTaskId);
        Assert.Equal(task.ConferenceId, dto.ConferenceId);
        Assert.Equal(task.Title, dto.Title);
        Assert.Equal(task.Description, dto.Description);
        Assert.Equal(task.TaskType, dto.TaskType);
        Assert.Equal(task.DueDate, dto.DueDate);
        Assert.Equal(task.Status, dto.Status);
        _repositoryMock.Verify(r => r.IsUserOrganizerOfConferenceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetLogisticsForConferenceAsync_PassesTaskTypeFilterToRepository()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        const string taskType = "Catering";

        _repositoryMock
            .Setup(r => r.GetByConferenceIdAsync(conferenceId, taskType))
            .ReturnsAsync(new[] { CreateTask(conferenceId, taskType) });

        var result = await service.GetLogisticsForConferenceAsync(
            conferenceId, taskType, Guid.NewGuid(), AdminRole);

        Assert.Single(result);
        _repositoryMock.Verify(r => r.GetByConferenceIdAsync(conferenceId, taskType), Times.Once);
    }

    [Fact]
    public async Task GetLogisticsForConferenceAsync_ReturnsEmptyListWhenNoTasksExist()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByConferenceIdAsync(conferenceId, null))
            .ReturnsAsync(Array.Empty<LogisticsTask>());

        var result = await service.GetLogisticsForConferenceAsync(
            conferenceId, null, Guid.NewGuid(), AdminRole);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateLogisticsTaskAsync_Admin_CreatesTaskWithValidData()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var dto = CreateDto();

        var result = await service.CreateLogisticsTaskAsync(
            conferenceId, dto, Guid.NewGuid(), AdminRole);

        Assert.NotEqual(Guid.Empty, result.LogisticsTaskId);
        Assert.Equal(conferenceId, result.ConferenceId);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.TaskType, result.TaskType);
        Assert.Equal(dto.DueDate, result.DueDate);
        Assert.Equal(dto.Status, result.Status);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<LogisticsTask>(task =>
            task.ConferenceId == conferenceId &&
            task.Title == dto.Title &&
            task.Description == dto.Description &&
            task.TaskType == dto.TaskType &&
            task.DueDate == dto.DueDate &&
            task.Status == dto.Status)), Times.Once);
    }

    [Fact]
    public async Task CreateLogisticsTaskAsync_OrganizerOfOwnConference_CreatesTask()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.IsUserOrganizerOfConferenceAsync(conferenceId, organizerId))
            .ReturnsAsync(true);

        var result = await service.CreateLogisticsTaskAsync(
            conferenceId, CreateDto(), organizerId, OrganizerRole);

        Assert.Equal(conferenceId, result.ConferenceId);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<LogisticsTask>()), Times.Once);
    }

    [Fact]
    public async Task CreateLogisticsTaskAsync_OrganizerOfOtherConference_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.IsUserOrganizerOfConferenceAsync(conferenceId, organizerId))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateLogisticsTaskAsync(conferenceId, CreateDto(), organizerId, OrganizerRole));
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<LogisticsTask>()), Times.Never);
    }

    [Theory]
    [InlineData(SpeakerRole)]
    [InlineData(AttendeeRole)]
    public async Task CreateLogisticsTaskAsync_NonAdminNonOrganizerRole_ThrowsWhenNotConferenceOrganizer(string role)
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.IsUserOrganizerOfConferenceAsync(conferenceId, userId))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateLogisticsTaskAsync(conferenceId, CreateDto(), userId, role));
    }

    [Fact]
    public void CreateLogisticsTaskDto_RequiredFields_AreValidated()
    {
        var dto = new CreateLogisticsTaskDto
        {
            Title = string.Empty,
            Description = string.Empty,
            TaskType = string.Empty,
            Status = string.Empty
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateLogisticsTaskDto.Title)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateLogisticsTaskDto.Description)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateLogisticsTaskDto.TaskType)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateLogisticsTaskDto.Status)));
    }

    [Fact]
    public async Task CreateLogisticsTaskAsync_InvalidTaskType_CurrentlyCreatesTaskBecauseWhitelistValidationIsMissing()
    {
        var service = CreateService();
        var dto = CreateDto(taskType: "Not-A-Predefined-Type");

        var result = await service.CreateLogisticsTaskAsync(
            Guid.NewGuid(), dto, Guid.NewGuid(), AdminRole);

        Assert.Equal("Not-A-Predefined-Type", result.TaskType);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<LogisticsTask>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLogisticsTaskAsync_ExistingTask_UpdatesTask()
    {
        var service = CreateService();
        var conferenceId = Guid.NewGuid();
        var existingTask = CreateTask(conferenceId, "Catering");
        var dto = new UpdateLogisticsTaskDto
        {
            Title = "Updated",
            Description = "Updated description",
            TaskType = "Transport",
            DueDate = DateTime.UtcNow.AddDays(3),
            Status = "Completed"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(existingTask.LogisticsTaskId))
            .ReturnsAsync(existingTask);

        var result = await service.UpdateLogisticsTaskAsync(
            existingTask.LogisticsTaskId, dto, Guid.NewGuid(), AdminRole);

        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.TaskType, result.TaskType);
        Assert.Equal(dto.DueDate, result.DueDate);
        Assert.Equal(dto.Status, result.Status);
        _repositoryMock.Verify(r => r.UpdateAsync(existingTask), Times.Once);
    }

    [Fact]
    public async Task UpdateLogisticsTaskAsync_TaskDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((LogisticsTask?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateLogisticsTaskAsync(id, new UpdateLogisticsTaskDto(), Guid.NewGuid(), AdminRole));
    }

    [Fact]
    public async Task UpdateLogisticsTaskAsync_OrganizerOfOtherConference_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var existingTask = CreateTask(Guid.NewGuid(), "Catering");
        var organizerId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(existingTask.LogisticsTaskId))
            .ReturnsAsync(existingTask);
        _repositoryMock
            .Setup(r => r.IsUserOrganizerOfConferenceAsync(existingTask.ConferenceId, organizerId))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpdateLogisticsTaskAsync(existingTask.LogisticsTaskId, new UpdateLogisticsTaskDto(), organizerId, OrganizerRole));
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<LogisticsTask>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLogisticsTask_ControllerReturnsConflictWhenServiceReportsConcurrencyConflict()
    {
        var id = Guid.NewGuid();
        var controller = CreateController(AdminRole);

        _serviceMock
            .Setup(s => s.UpdateLogisticsTaskAsync(id, It.IsAny<UpdateLogisticsTaskDto>(), It.IsAny<Guid>(), AdminRole))
            .ThrowsAsync(new InvalidOperationException("Konflikt pri istovremenom uređivanju!"));

        var result = await controller.UpdateLogisticsTask(id, new UpdateLogisticsTaskDto());

        var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
    }

    [Fact]
    public void LogisticsTask_DoesNotDeclareRowVersionConcurrencyToken_CurrentGap()
    {
        var rowVersionProperty = typeof(LogisticsTask).GetProperty("RowVersion");

        Assert.Null(rowVersionProperty);
    }

    [Fact]
    public async Task DeleteLogisticsTaskAsync_ExistingTask_DeletesTask()
    {
        var service = CreateService();
        var existingTask = CreateTask(Guid.NewGuid(), "Catering");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(existingTask.LogisticsTaskId))
            .ReturnsAsync(existingTask);

        await service.DeleteLogisticsTaskAsync(existingTask.LogisticsTaskId, Guid.NewGuid(), AdminRole);

        _repositoryMock.Verify(r => r.DeleteAsync(existingTask), Times.Once);
    }

    [Fact]
    public async Task DeleteLogisticsTaskAsync_TaskDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((LogisticsTask?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteLogisticsTaskAsync(id, Guid.NewGuid(), AdminRole));
    }

    [Fact]
    public async Task DeleteLogisticsTaskAsync_OrganizerOfOtherConference_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        var existingTask = CreateTask(Guid.NewGuid(), "Catering");
        var organizerId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(existingTask.LogisticsTaskId))
            .ReturnsAsync(existingTask);
        _repositoryMock
            .Setup(r => r.IsUserOrganizerOfConferenceAsync(existingTask.ConferenceId, organizerId))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.DeleteLogisticsTaskAsync(existingTask.LogisticsTaskId, organizerId, OrganizerRole));
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<LogisticsTask>()), Times.Never);
    }

    [Fact]
    public async Task Controller_GetConferenceLogistics_ReturnsOkWithTasks()
    {
        var conferenceId = Guid.NewGuid();
        var controller = CreateController(AdminRole);
        var task = CreateTaskDto(conferenceId);

        _serviceMock
            .Setup(s => s.GetLogisticsForConferenceAsync(conferenceId, "Catering", It.IsAny<Guid>(), AdminRole))
            .ReturnsAsync(new[] { task });

        var result = await controller.GetConferenceLogistics(conferenceId, "Catering");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<LogisticsTaskDto>>(ok.Value);
        Assert.Single(items);
    }

    [Fact]
    public async Task Controller_CreateLogisticsTask_ReturnsCreatedStatusCode()
    {
        var conferenceId = Guid.NewGuid();
        var controller = CreateController(AdminRole);
        var dto = CreateDto();
        var created = CreateTaskDto(conferenceId);

        _serviceMock
            .Setup(s => s.CreateLogisticsTaskAsync(conferenceId, dto, It.IsAny<Guid>(), AdminRole))
            .ReturnsAsync(created);

        var result = await controller.CreateLogisticsTask(conferenceId, dto);

        var createdResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public async Task Controller_DeleteLogisticsTask_ReturnsOkMessage()
    {
        var id = Guid.NewGuid();
        var controller = CreateController(AdminRole);

        var result = await controller.DeleteLogisticsTask(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
        _serviceMock.Verify(s => s.DeleteLogisticsTaskAsync(id, It.IsAny<Guid>(), AdminRole), Times.Once);
    }

    private LogisticsController CreateController(string role, Guid? userId = null)
    {
        var id = userId ?? Guid.NewGuid();
        var controller = new LogisticsController(_serviceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                            new Claim(ClaimTypes.Role, role)
                        },
                        authenticationType: "TestAuth"))
                }
            }
        };

        return controller;
    }

    private static CreateLogisticsTaskDto CreateDto(string taskType = "Catering") =>
        new()
        {
            Title = "Logistics task",
            Description = "Task description",
            TaskType = taskType,
            DueDate = DateTime.UtcNow.AddDays(2),
            Status = "Pending"
        };

    private static LogisticsTask CreateTask(Guid conferenceId, string taskType) =>
        new()
        {
            LogisticsTaskId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            Title = "Logistics task",
            Description = "Task description",
            TaskType = taskType,
            DueDate = DateTime.UtcNow.AddDays(2),
            Status = "Pending"
        };

    private static LogisticsTaskDto CreateTaskDto(Guid conferenceId) =>
        new()
        {
            LogisticsTaskId = Guid.NewGuid(),
            ConferenceId = conferenceId,
            Title = "Logistics task",
            Description = "Task description",
            TaskType = "Catering",
            DueDate = DateTime.UtcNow.AddDays(2),
            Status = "Pending"
        };

    private static List<ValidationResult> Validate(object dto)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
        return results;
    }
}
