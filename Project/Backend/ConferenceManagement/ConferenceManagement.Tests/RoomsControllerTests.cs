using System.Reflection;
using System.Security.Claims;
using ConferenceManagement.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ConferenceManagement.Tests;

public class RoomsControllerTests
{
    private const string RequiredPolicy = "AdminOrOrganizerPolicy";

    private static MethodInfo GetAction(string name) =>
        typeof(RoomsController).GetMethod(name)
        ?? throw new InvalidOperationException($"Action {name} was not found.");

    public static IEnumerable<object[]> MutatingActions =>
        new List<object[]>
        {
            new object[] { nameof(RoomsController.CreateRoom) },
            new object[] { nameof(RoomsController.UpdateRoom) },
            new object[] { nameof(RoomsController.DeleteRoom) }
        };

    public static IEnumerable<object[]> AllActions =>
        new List<object[]>
        {
            new object[] { nameof(RoomsController.GetAllRooms) },
            new object[] { nameof(RoomsController.CreateRoom) },
            new object[] { nameof(RoomsController.UpdateRoom) },
            new object[] { nameof(RoomsController.DeleteRoom) }
        };

    [Theory]
    [MemberData(nameof(AllActions))]
    public void RoomsActions_RequireAdminOrOrganizerPolicy(string actionName)
    {
        var action = GetAction(actionName);

        var authorizeAttribute = action.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(RequiredPolicy, authorizeAttribute.Policy);
    }

    [Theory]
    [MemberData(nameof(MutatingActions))]
    public void PostPutDelete_RequireAdminOrOrganizerPolicy(string actionName)
    {
        var action = GetAction(actionName);

        var authorizeAttribute = action.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal(RequiredPolicy, authorizeAttribute.Policy);
    }

    [Fact]
    public void RoomsController_HasExpectedApiRoute()
    {
        var routeAttribute = typeof(RoomsController).GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(routeAttribute);
        Assert.Equal("api/rooms", routeAttribute.Template);
        Assert.NotNull(typeof(RoomsController).GetCustomAttribute<ApiControllerAttribute>());
    }

    [Fact]
    public void GetAllRooms_UsesHttpGet()
    {
        var action = GetAction(nameof(RoomsController.GetAllRooms));

        Assert.NotNull(action.GetCustomAttribute<HttpGetAttribute>());
    }

    [Fact]
    public void CreateRoom_UsesHttpPost()
    {
        var action = GetAction(nameof(RoomsController.CreateRoom));

        Assert.NotNull(action.GetCustomAttribute<HttpPostAttribute>());
    }

    [Fact]
    public void UpdateRoom_UsesHttpPutWithIdRoute()
    {
        var action = GetAction(nameof(RoomsController.UpdateRoom));
        var attribute = action.GetCustomAttribute<HttpPutAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("{id}", attribute.Template);
    }

    [Fact]
    public void DeleteRoom_UsesHttpDeleteWithIdRoute()
    {
        var action = GetAction(nameof(RoomsController.DeleteRoom));
        var attribute = action.GetCustomAttribute<HttpDeleteAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("{id}", attribute.Template);
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
}
