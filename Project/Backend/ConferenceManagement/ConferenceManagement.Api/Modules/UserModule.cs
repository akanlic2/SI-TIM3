using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceManagement.Api.Modules;

public static class UserModule
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users")
            .WithTags("Users");

        
        //Dohvata sve korisnike - Dostupno samo za admin-sistema
        
        group.MapGet("all", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(new { users, count = users.Count });
        })
        .RequireAuthorization("AdminSistemaPolicy")
        .WithSummary("Dohvata sve korisnike - Dostupno samo za admin-sistema");

        
        //Dohvata sve korisnike - Dostupno za organizatore i admin-sistema
        
        group.MapGet("organizer-view", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(new { users, count = users.Count });
        })
        .RequireAuthorization("OrganizerPolicy")
        .WithSummary("Dohvata sve korisnike - Dostupno za organizatore i admin-sistema");

        
        //Dohvata sve korisnike - Dostupno za sve autentificirane korisnike
        
        group.MapGet("public", async (IUserService userService) =>
        {
            var users = await userService.GetAllUsersAsync();
            return Results.Ok(new { users, count = users.Count });
        })
        .RequireAuthorization("ParticipantPolicy")
        .WithSummary("Dohvata sve korisnike - Dostupno za sve autentificirane korisnike");

        return app;
    }
}