using System.ComponentModel.DataAnnotations;
using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.DTOs.Equipment;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests;

public class EquipmentTests
{
    private const string AdminOrOrganizerPolicy = "AdminOrOrganizerPolicy";

    private readonly Mock<IEquipmentRepository> _equipmentRepositoryMock = new();
    private readonly Mock<ISessionRepository> _sessionRepositoryMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly Mock<IEquipmentService> _equipmentServiceMock = new();

    private EquipmentService CreateService() =>
        new(
            _equipmentRepositoryMock.Object,
            _sessionRepositoryMock.Object,
            _userContextMock.Object);

    [Theory]
    [InlineData(nameof(EquipmentController.GetAllEquipment), typeof(HttpGetAttribute), "api/equipment")]
    [InlineData(nameof(EquipmentController.GetSessionEquipment), typeof(HttpGetAttribute), "api/sessions/{sessionId:guid}/equipment")]
    [InlineData(nameof(EquipmentController.CreateEquipment), typeof(HttpPostAttribute), "api/equipment")]
    [InlineData(nameof(EquipmentController.DeleteEquipment), typeof(HttpDeleteAttribute), "api/equipment/{id:guid}")]
    [InlineData(nameof(EquipmentController.AssignEquipment), typeof(HttpPostAttribute), "api/sessions/{sessionId:guid}/equipment")]
    [InlineData(nameof(EquipmentController.DecrementEquipmentQuantity), typeof(HttpPatchAttribute), "api/equipment/{id:guid}/decrement")]
    public void ControllerActions_UseExpectedRoutes(string actionName, Type httpAttributeType, string expectedTemplate)
    {
        var action = typeof(EquipmentController).GetMethod(actionName);

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(httpAttributeType, inherit: false));
        var template = attribute switch
        {
            HttpGetAttribute a => a.Template,
            HttpPostAttribute a => a.Template,
            HttpDeleteAttribute a => a.Template,
            HttpPatchAttribute a => a.Template,
            _ => null
        };

        Assert.Equal(expectedTemplate, template);
    }

    [Fact]
    public void EquipmentController_RequiresAuthenticatedUserAtControllerLevel()
    {
        var attribute = Assert.Single(typeof(EquipmentController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);

        Assert.Null(authorizeAttribute.Policy);
    }

    [Theory]
    [InlineData(nameof(EquipmentController.CreateEquipment))]
    [InlineData(nameof(EquipmentController.DeleteEquipment))]
    [InlineData(nameof(EquipmentController.AssignEquipment))]
    [InlineData(nameof(EquipmentController.DecrementEquipmentQuantity))]
    public void MutatingActions_RequireAdminOrOrganizerPolicy(string actionName)
    {
        var action = typeof(EquipmentController).GetMethod(actionName);

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);
        Assert.Equal(AdminOrOrganizerPolicy, authorizeAttribute.Policy);
    }

    [Theory]
    [InlineData(nameof(EquipmentController.GetAllEquipment))]
    [InlineData(nameof(EquipmentController.GetSessionEquipment))]
    public void GetActions_DoNotRequireAdminOrOrganizerPolicyInCurrentImplementation(string actionName)
    {
        var action = typeof(EquipmentController).GetMethod(actionName);

        Assert.NotNull(action);
        Assert.Empty(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
    }

    [Fact]
    public async Task GetAllEquipmentAsync_ReturnsOnlyGlobalInventory()
    {
        var service = CreateService();
        var global = CreateEquipment(quantity: 4, availableQuantity: 4);
        var assigned = CreateEquipment(quantity: 1, availableQuantity: 0, sessionId: Guid.NewGuid(), status: "Assigned");

        _equipmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Equipment> { global, assigned });

        var result = await service.GetAllEquipmentAsync(CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal(global.EquipmentId, dto.EquipmentId);
        Assert.Null(dto.SessionId);
        Assert.Equal(global.Name, dto.Name);
    }

    [Fact]
    public async Task GetAllEquipmentAsync_ReturnsEmptyListWhenNoGlobalEquipmentExists()
    {
        var service = CreateService();

        _equipmentRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Equipment>());

        var result = await service.GetAllEquipmentAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEquipmentBySessionIdAsync_ReturnsAssignedEquipment()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var assigned = CreateEquipment(quantity: 2, availableQuantity: 0, sessionId: sessionId, status: "Assigned");

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Equipment> { assigned });

        var result = await service.GetEquipmentBySessionIdAsync(sessionId, CancellationToken.None);

        var dto = Assert.Single(result);
        Assert.Equal(assigned.EquipmentId, dto.EquipmentId);
        Assert.Equal(sessionId, dto.SessionId);
        Assert.Equal("Assigned", dto.AvailabilityStatus);
    }

    [Fact]
    public async Task GetEquipmentBySessionIdAsync_SessionDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync((Session?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetEquipmentBySessionIdAsync(sessionId, CancellationToken.None));
    }

    [Fact]
    public async Task CreateEquipmentAsync_AdminOrOrganizer_CreatesGlobalEquipmentAndDerivesAvailabilityStatus()
    {
        var service = CreateService();
        var dto = NewCreateEquipmentDto(quantity: 3);
        SetupHasAdminOrOrganizer(true);

        var result = await service.CreateEquipmentAsync(dto, CancellationToken.None);

        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Type, result.Type);
        Assert.Equal(3, result.Quantity);
        Assert.Equal(3, result.AvailableQuantity);
        Assert.True(result.IsAvailable);
        Assert.Equal("Available", result.AvailabilityStatus);
        Assert.Null(result.SessionId);
        _equipmentRepositoryMock.Verify(r => r.AddAsync(It.Is<Equipment>(equipment =>
            equipment.SessionId == null &&
            equipment.Name == dto.Name &&
            equipment.Type == dto.Type &&
            equipment.Quantity == dto.Quantity &&
            equipment.AvailableQuantity == dto.Quantity &&
            equipment.IsAvailable &&
            equipment.AvailabilityStatus == "Available"), It.IsAny<CancellationToken>()), Times.Once);
        _equipmentRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateEquipmentAsync_SpeakerOrAttendee_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        SetupHasAdminOrOrganizer(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateEquipmentAsync(NewCreateEquipmentDto(), CancellationToken.None));
        _equipmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void CreateEquipmentDto_RequiredFieldsAndPositiveQuantity_AreValidated()
    {
        var dto = new CreateEquipmentDto
        {
            Name = string.Empty,
            Type = string.Empty,
            Quantity = 0
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateEquipmentDto.Name)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateEquipmentDto.Type)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateEquipmentDto.Quantity)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateEquipmentDto_ZeroOrNegativeQuantity_IsInvalid(int quantity)
    {
        var dto = NewCreateEquipmentDto(quantity: quantity);

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateEquipmentDto.Quantity)));
    }

    [Fact]
    public async Task CreateEquipmentAsync_UnknownEquipmentType_CurrentlyCreatesEquipmentBecauseWhitelistValidationIsMissing()
    {
        var service = CreateService();
        var dto = NewCreateEquipmentDto(type: "Unknown-Type");
        SetupHasAdminOrOrganizer(true);

        var result = await service.CreateEquipmentAsync(dto, CancellationToken.None);

        Assert.Equal("Unknown-Type", result.Type);
        _equipmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEquipmentAsync_GlobalEquipmentWithNoAssignedQuantity_DeletesEquipment()
    {
        var service = CreateService();
        var equipment = CreateEquipment(quantity: 3, availableQuantity: 3);
        SetupHasAdminOrOrganizer(true);

        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        await service.DeleteEquipmentAsync(equipment.EquipmentId, CancellationToken.None);

        _equipmentRepositoryMock.Verify(r => r.DeleteAsync(equipment, It.IsAny<CancellationToken>()), Times.Once);
        _equipmentRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteEquipmentAsync_EquipmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var equipmentId = Guid.NewGuid();
        SetupHasAdminOrOrganizer(true);

        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteEquipmentAsync(equipmentId, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteEquipmentAsync_EquipmentPartiallyAssigned_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        var equipment = CreateEquipment(quantity: 5, availableQuantity: 3);
        SetupHasAdminOrOrganizer(true);

        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DeleteEquipmentAsync(equipment.EquipmentId, CancellationToken.None));
        _equipmentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEquipmentAsync_SpeakerOrAttendee_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        SetupHasAdminOrOrganizer(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.DeleteEquipmentAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task DecrementEquipmentQuantityAsync_DecrementsGlobalEquipmentAndKeepsAvailableStatus()
    {
        var service = CreateService();
        var equipment = CreateEquipment(quantity: 3, availableQuantity: 3);
        SetupHasAdminOrOrganizer(true);

        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        var result = await service.DecrementEquipmentQuantityAsync(equipment.EquipmentId, CancellationToken.None);

        Assert.Equal(2, result.Quantity);
        Assert.Equal(2, result.AvailableQuantity);
        Assert.True(result.IsAvailable);
        Assert.Equal("Available", result.AvailabilityStatus);
        _equipmentRepositoryMock.Verify(r => r.UpdateAsync(equipment, It.IsAny<CancellationToken>()), Times.Once);
        _equipmentRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DecrementEquipmentQuantityAsync_LastAvailableItem_MarksUnavailable()
    {
        var service = CreateService();
        var equipment = CreateEquipment(quantity: 1, availableQuantity: 1);
        SetupHasAdminOrOrganizer(true);

        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        var result = await service.DecrementEquipmentQuantityAsync(equipment.EquipmentId, CancellationToken.None);

        Assert.Equal(0, result.Quantity);
        Assert.Equal(0, result.AvailableQuantity);
        Assert.False(result.IsAvailable);
        Assert.Equal("Unavailable", result.AvailabilityStatus);
    }

    [Fact]
    public async Task DecrementEquipmentQuantityAsync_AssignedEquipment_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        var equipment = CreateEquipment(quantity: 1, availableQuantity: 0, sessionId: Guid.NewGuid(), status: "Assigned");
        SetupHasAdminOrOrganizer(true);

        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DecrementEquipmentQuantityAsync(equipment.EquipmentId, CancellationToken.None));
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_ValidRequest_AssignsEquipmentAndReducesGlobalQuantity()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var globalEquipment = CreateEquipment(quantity: 5, availableQuantity: 5);
        var dto = new AssignEquipmentDto { EquipmentId = globalEquipment.EquipmentId, Quantity = 2 };
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(globalEquipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(globalEquipment);

        await service.AssignEquipmentToSessionAsync(sessionId, dto, CancellationToken.None);

        Assert.Equal(3, globalEquipment.AvailableQuantity);
        Assert.True(globalEquipment.IsAvailable);
        Assert.Equal("Available", globalEquipment.AvailabilityStatus);
        _equipmentRepositoryMock.Verify(r => r.UpdateAsync(globalEquipment, It.IsAny<CancellationToken>()), Times.Once);
        _equipmentRepositoryMock.Verify(r => r.AddAsync(It.Is<Equipment>(equipment =>
            equipment.SessionId == sessionId &&
            equipment.Name == globalEquipment.Name &&
            equipment.Type == globalEquipment.Type &&
            equipment.Quantity == 2 &&
            equipment.AvailableQuantity == 0 &&
            !equipment.IsAvailable &&
            equipment.AvailabilityStatus == "Assigned"), It.IsAny<CancellationToken>()), Times.Once);
        _equipmentRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_AssigningAllAvailableQuantity_MarksGlobalEquipmentUnavailable()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var globalEquipment = CreateEquipment(quantity: 2, availableQuantity: 2);
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(globalEquipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(globalEquipment);

        await service.AssignEquipmentToSessionAsync(
            sessionId,
            new AssignEquipmentDto { EquipmentId = globalEquipment.EquipmentId, Quantity = 2 },
            CancellationToken.None);

        Assert.Equal(0, globalEquipment.AvailableQuantity);
        Assert.False(globalEquipment.IsAvailable);
        Assert.Equal("Unavailable", globalEquipment.AvailabilityStatus);
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_SessionDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Session?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AssignEquipmentToSessionAsync(Guid.NewGuid(), new AssignEquipmentDto(), CancellationToken.None));
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_GlobalEquipmentDoesNotExist_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(equipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AssignEquipmentToSessionAsync(
                sessionId,
                new AssignEquipmentDto { EquipmentId = equipmentId, Quantity = 1 },
                CancellationToken.None));
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_EquipmentAlreadyAssignedToSession_ThrowsKeyNotFoundException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var assignedEquipment = CreateEquipment(quantity: 1, availableQuantity: 0, sessionId: Guid.NewGuid(), status: "Assigned");
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(assignedEquipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignedEquipment);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.AssignEquipmentToSessionAsync(
                sessionId,
                new AssignEquipmentDto { EquipmentId = assignedEquipment.EquipmentId, Quantity = 1 },
                CancellationToken.None));
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_RequestedQuantityExceedsAvailableQuantity_ThrowsInvalidOperationException()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var globalEquipment = CreateEquipment(quantity: 2, availableQuantity: 1);
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(globalEquipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(globalEquipment);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AssignEquipmentToSessionAsync(
                sessionId,
                new AssignEquipmentDto { EquipmentId = globalEquipment.EquipmentId, Quantity = 2 },
                CancellationToken.None));
        _equipmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_SpeakerOrAttendee_ThrowsUnauthorizedAccessException()
    {
        var service = CreateService();
        SetupHasAdminOrOrganizer(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.AssignEquipmentToSessionAsync(Guid.NewGuid(), new AssignEquipmentDto(), CancellationToken.None));
    }

    [Fact]
    public async Task AssignEquipmentToSessionAsync_OrganizerRoleCurrentlyAssignsAnyExistingSessionBecauseOwnershipCheckIsMissing()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var globalEquipment = CreateEquipment(quantity: 2, availableQuantity: 2);
        SetupHasAdminOrOrganizer(true);

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId, ConferenceId = Guid.NewGuid() });
        _equipmentRepositoryMock
            .Setup(r => r.GetByIdAsync(globalEquipment.EquipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(globalEquipment);

        await service.AssignEquipmentToSessionAsync(
            sessionId,
            new AssignEquipmentDto { EquipmentId = globalEquipment.EquipmentId, Quantity = 1 },
            CancellationToken.None);

        _equipmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetEquipmentBySessionIdAsync_ReturnsAssignedEquipmentAfterRepositoryContainsAssignment()
    {
        var service = CreateService();
        var sessionId = Guid.NewGuid();
        var assigned = CreateEquipment(quantity: 1, availableQuantity: 0, sessionId: sessionId, status: "Assigned");

        _sessionRepositoryMock
            .Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(new Session { SessionId = sessionId });
        _equipmentRepositoryMock
            .Setup(r => r.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Equipment> { assigned });

        var result = await service.GetEquipmentBySessionIdAsync(sessionId, CancellationToken.None);

        Assert.Contains(result, item =>
            item.EquipmentId == assigned.EquipmentId &&
            item.SessionId == sessionId &&
            item.AvailabilityStatus == "Assigned");
    }

    [Fact]
    public async Task Controller_GetAllEquipment_ReturnsOkWithItems()
    {
        var controller = CreateController();
        var equipment = CreateEquipmentDtoResult();

        _equipmentServiceMock
            .Setup(s => s.GetAllEquipmentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EquipmentDto> { equipment });

        var result = await controller.GetAllEquipment(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<List<EquipmentDto>>(ok.Value);
        Assert.Single(items);
    }

    [Fact]
    public async Task Controller_CreateEquipment_ReturnsCreatedAtAction()
    {
        var controller = CreateController();
        var dto = NewCreateEquipmentDto();
        var created = CreateEquipmentDtoResult();

        _equipmentServiceMock
            .Setup(s => s.CreateEquipmentAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var result = await controller.CreateEquipment(dto, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(EquipmentController.GetAllEquipment), createdResult.ActionName);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public async Task Controller_DeleteEquipment_ReturnsNotFoundWhenServiceThrowsKeyNotFoundException()
    {
        var controller = CreateController();
        var equipmentId = Guid.NewGuid();

        _equipmentServiceMock
            .Setup(s => s.DeleteEquipmentAsync(equipmentId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("not found"));

        var result = await controller.DeleteEquipment(equipmentId, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Controller_AssignEquipment_ReturnsBadRequestWhenQuantityIsUnavailable()
    {
        var controller = CreateController();
        var sessionId = Guid.NewGuid();

        _equipmentServiceMock
            .Setup(s => s.AssignEquipmentToSessionAsync(sessionId, It.IsAny<AssignEquipmentDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("unavailable"));

        var result = await controller.AssignEquipment(
            sessionId,
            new AssignEquipmentDto { EquipmentId = Guid.NewGuid(), Quantity = 99 },
            CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Controller_DecrementEquipmentQuantity_ReturnsOkWithUpdatedEquipment()
    {
        var controller = CreateController();
        var equipmentId = Guid.NewGuid();
        var updated = CreateEquipmentDtoResult(quantity: 1, availableQuantity: 1);

        _equipmentServiceMock
            .Setup(s => s.DecrementEquipmentQuantityAsync(equipmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var result = await controller.DecrementEquipmentQuantity(equipmentId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(updated, ok.Value);
    }

    private EquipmentController CreateController() => new(_equipmentServiceMock.Object);

    private void SetupHasAdminOrOrganizer(bool value)
    {
        _userContextMock
            .Setup(s => s.HasAnyRole(It.IsAny<string[]>()))
            .Returns(value);
    }

    private static CreateEquipmentDto NewCreateEquipmentDto(
        string name = "Projector",
        string type = "Audio-Visual",
        int quantity = 2) =>
        new()
        {
            Name = name,
            Type = type,
            Quantity = quantity
        };

    private static Equipment CreateEquipment(
        int quantity,
        int availableQuantity,
        Guid? sessionId = null,
        string status = "Available") =>
        new()
        {
            EquipmentId = Guid.NewGuid(),
            SessionId = sessionId,
            Name = "Projector",
            Type = "Audio-Visual",
            Quantity = quantity,
            AvailableQuantity = availableQuantity,
            IsAvailable = status == "Available",
            AvailabilityStatus = status,
            CreatedAt = DateTime.UtcNow
        };

    private static EquipmentDto CreateEquipmentDtoResult(
        int quantity = 2,
        int availableQuantity = 2,
        Guid? sessionId = null,
        string status = "Available") =>
        new()
        {
            EquipmentId = Guid.NewGuid(),
            SessionId = sessionId,
            Name = "Projector",
            Type = "Audio-Visual",
            Quantity = quantity,
            AvailableQuantity = availableQuantity,
            IsAvailable = status == "Available",
            AvailabilityStatus = status,
            CreatedAt = DateTime.UtcNow
        };

    private static List<ValidationResult> Validate(object dto)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
        return results;
    }
}
