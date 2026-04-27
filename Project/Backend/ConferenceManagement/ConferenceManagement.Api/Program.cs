using ConferenceManagement.Api.Extensions;
using ConferenceManagement.Api.Middlewear;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Dal;
using ConferenceManagement.Dal.Repositories;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// builder.Services.AddSwaggerGen();

// CORS – dozvoli frontendu na localhost:5173 da poziva API
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost",
                "http://localhost:5173",
                "http://localhost:5174"   // fallback ako Vite promijeni port
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keycloakSection = builder.Configuration.GetSection("Keycloak");
        var authority = keycloakSection["Authority"];
        
        options.Authority = authority;
        // Zato što je API u Dockeru, localhost:8080 gađao bi sam sebe umesto Keycloak-a.
        // host.docker.internal rešava taj problem komunikacije.
        options.MetadataAddress = authority.Replace("localhost", "host.docker.internal") + "/.well-known/openid-configuration";
        options.Audience = keycloakSection["Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = authority,
            NameClaimType = "sub", // Mapira Keycloak 'sub' na ClaimTypes.NameIdentifier
            RoleClaimType = "role" // Mapira role koje doda nas Transformer
        };
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            })
        .UseSnakeCaseNamingConvention()
        .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.AddScoped<IConferenceService, ConferenceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKeycloakService, KeycloakService>();
builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();
builder.Services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, ConferenceManagement.Api.Middlewear.KeycloakRolesTransformer>();

var app = builder.Build();

var runMigrationsOnly =
    builder.Configuration.GetValue<bool>("RUN_MIGRATIONS_ONLY");

if (runMigrationsOnly)
{
    await app.Services.WaitForDatabaseAndApplyMigrationsAsync(app.Logger);
    app.Logger.LogInformation("Migrations finished successfully. Exiting migrator container.");
    return;
}



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Conference Management API")
               .WithTheme(ScalarTheme.BluePlanet)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.UseCors("FrontendDev");  // CORS mora biti prije Auth
app.UseAuthentication();
app.UseAuthorization();

// Add KeycloakUserSyncMiddleware
app.UseMiddleware<KeycloakUserSyncMiddleware>();
app.MapControllers();

app.Run();
