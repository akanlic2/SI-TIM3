using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ConferenceManagement.Tests;

public class MaterialUpdateDeleteTests
{
    [Fact]
    public void MaterialsController_UsesSessionMaterialsRouteAndRequiresAuthenticatedUser()
    {
        var routeAttribute = Assert.Single(typeof(MaterialsController)
            .GetCustomAttributes(typeof(RouteAttribute), inherit: false));
        var route = Assert.IsType<RouteAttribute>(routeAttribute);

        var authorizeAttribute = Assert.Single(typeof(MaterialsController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorize = Assert.IsType<AuthorizeAttribute>(authorizeAttribute);

        Assert.Equal("api/sessions/{sessionId:guid}/materials", route.Template);
        Assert.Null(authorize.Policy);
    }

    [Fact]
    public void MaterialsController_CurrentlyExposesOnlyUploadAndListActions()
    {
        var publicActions = typeof(MaterialsController)
            .GetMethods()
            .Where(m => m.DeclaringType == typeof(MaterialsController))
            .Select(m => m.Name)
            .OrderBy(name => name)
            .ToList();

        Assert.Equal(new[] { "GetMaterials", "UploadMaterial" }, publicActions);
    }

    [Fact]
    public void MaterialsController_DoesNotImplementUpdateEndpointForS45_3()
    {
        var putActions = typeof(MaterialsController)
            .GetMethods()
            .Where(m => m.DeclaringType == typeof(MaterialsController))
            .Where(m => m.GetCustomAttributes(typeof(HttpPutAttribute), inherit: false).Any())
            .ToList();

        Assert.Empty(putActions);
    }

    [Fact]
    public void MaterialsController_DoesNotImplementDeleteEndpointForS45_4()
    {
        var deleteActions = typeof(MaterialsController)
            .GetMethods()
            .Where(m => m.DeclaringType == typeof(MaterialsController))
            .Where(m => m.GetCustomAttributes(typeof(HttpDeleteAttribute), inherit: false).Any())
            .ToList();

        Assert.Empty(deleteActions);
    }

    [Fact]
    public void MaterialsController_HasNoS45UpdateDeleteActionLevelPoliciesBecauseActionsDoNotExist()
    {
        var updateDeleteActions = typeof(MaterialsController)
            .GetMethods()
            .Where(m => m.DeclaringType == typeof(MaterialsController))
            .Where(m =>
                m.GetCustomAttributes(typeof(HttpPutAttribute), inherit: false).Any() ||
                m.GetCustomAttributes(typeof(HttpDeleteAttribute), inherit: false).Any())
            .ToList();

        Assert.Empty(updateDeleteActions);
    }

    [Fact]
    public void MaterialsController_UploadAndListDoNotDeclareS45UpdateDeletePolicies()
    {
        var upload = typeof(MaterialsController).GetMethod(nameof(MaterialsController.UploadMaterial));
        var list = typeof(MaterialsController).GetMethod(nameof(MaterialsController.GetMaterials));

        Assert.NotNull(upload);
        Assert.NotNull(list);
        Assert.Empty(upload.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        Assert.Empty(list.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
    }

    [Theory]
    [InlineData("UpdateMaterial")]
    [InlineData("EditMaterial")]
    [InlineData("DeleteMaterial")]
    [InlineData("RemoveMaterial")]
    public void IMaterialService_DoesNotExposeUpdateOrDeleteContractForS45(string methodName)
    {
        var method = typeof(IMaterialService).GetMethod(methodName);

        Assert.Null(method);
    }

    [Fact]
    public void IMaterialService_CurrentlyExposesOnlyUploadAndListContracts()
    {
        var methodNames = typeof(IMaterialService)
            .GetMethods()
            .Select(m => m.Name)
            .OrderBy(name => name)
            .ToList();

        Assert.Equal(new[] { "GetMaterialsBySessionIdAsync", "UploadMaterialAsync" }, methodNames);
    }

    [Theory]
    [InlineData("UpdateMaterialAsync")]
    [InlineData("DeleteMaterialAsync")]
    [InlineData("RemoveMaterialAsync")]
    public void MaterialService_DoesNotImplementUpdateOrDeleteMethodsForS45(string methodName)
    {
        var method = typeof(MaterialService).GetMethod(methodName);

        Assert.Null(method);
    }

    [Fact]
    public void MaterialService_CurrentlyImplementsOnlyUploadAndListPublicMethods()
    {
        var methodNames = typeof(MaterialService)
            .GetMethods()
            .Where(m => m.DeclaringType == typeof(MaterialService))
            .Select(m => m.Name)
            .OrderBy(name => name)
            .ToList();

        Assert.Equal(new[] { "GetMaterialsBySessionIdAsync", "UploadMaterialAsync" }, methodNames);
    }

    [Theory]
    [InlineData("GetByIdAsync")]
    [InlineData("UpdateAsync")]
    [InlineData("DeleteAsync")]
    public void IMaterialRepository_DoesNotExposeRepositoryOperationsNeededForUpdateDelete(string methodName)
    {
        var method = typeof(IMaterialRepository).GetMethod(methodName);

        Assert.Null(method);
    }

    [Fact]
    public void IMaterialRepository_CurrentlySupportsOnlyAddListAndSave()
    {
        var methodNames = typeof(IMaterialRepository)
            .GetMethods()
            .Select(m => m.Name)
            .OrderBy(name => name)
            .ToList();

        Assert.Equal(new[] { "AddAsync", "GetBySessionIdAsync", "SaveChangesAsync" }, methodNames);
    }
}
