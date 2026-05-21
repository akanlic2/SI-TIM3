using System.Security.Claims;
using ConferenceManagement.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ConferenceManagement.Tests;

public class SessionRoomAssignmentTests
{
    private const string RequiredPolicy = "AdminOrOrganizerPolicy";

    [Fact]
    public void AssignRoomToSession_UsesHttpPutWithExpectedRoute()
    {
        var action = typeof(SessionsController).GetMethod(nameof(SessionsController.AssignRoomToSession));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(HttpPutAttribute), inherit: false));
        var httpPutAttribute = Assert.IsType<HttpPutAttribute>(attribute);
        Assert.Equal("{id}/room", httpPutAttribute.Template);
    }

    [Fact]
    public void AssignRoomToSession_RequiresAdminOrOrganizerPolicy()
    {
        var action = typeof(SessionsController).GetMethod(nameof(SessionsController.AssignRoomToSession));

        Assert.NotNull(action);
        var attribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var authorizeAttribute = Assert.IsType<AuthorizeAttribute>(attribute);
        Assert.Equal(RequiredPolicy, authorizeAttribute.Policy);
    }

    [Fact]
    public void SessionsController_UsesExpectedBaseRoute()
    {
        var attribute = Assert.Single(
            typeof(SessionsController).GetCustomAttributes(typeof(RouteAttribute), inherit: false));
        var routeAttribute = Assert.IsType<RouteAttribute>(attribute);

        Assert.Equal("api/[controller]", routeAttribute.Template);
        Assert.NotNull(typeof(SessionsController).GetCustomAttributes(typeof(ApiControllerAttribute), inherit: false).SingleOrDefault());
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
