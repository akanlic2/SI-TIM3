using ConferenceManagement.Api.Extensions;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Dal;
using ConferenceManagement.Dal.Repositories;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost",
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Velika izmjena: lokalni JWT auth umjesto Keycloak validacije eksternog tokena.
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var signingKey = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"AUTH FAILED: {context.Exception.Message}");
                return Task.CompletedTask;
            }
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.AddScoped<IConferenceService, ConferenceService>();
builder.Services.AddScoped<IConferenceRegistrationRepository, ConferenceRegistrationRepository>();
builder.Services.AddScoped<IConferenceRegistrationService, ConferenceRegistrationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRegistrationRepository, SessionRegistrationRepository>();
builder.Services.AddScoped<IAgendaItemRepository, AgendaItemRepository>();
builder.Services.AddScoped<IAgendaItemService, AgendaItemService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("admin-sistema"))
    .AddPolicy("OrganizerPolicy", policy => policy.RequireRole("organizator"))
    .AddPolicy("AdminOrOrganizerPolicy", policy => policy.RequireRole("admin-sistema", "organizator"))
    .AddPolicy("SpeakerPolicy", policy => policy.RequireRole("predavac"))
    .AddPolicy("AttendeePolicy", policy => policy.RequireRole("ucesnik"))
    .AddPolicy("AdminOrSpeakerPolicy", policy => policy.RequireRole("admin-sistema", "predavac"))
    .AddPolicy("ParticipantPolicy", policy =>
        policy.RequireAuthenticatedUser());

builder.Services.AddScoped<IConferenceCapacityService, ConferenceCapacityService>();

var app = builder.Build();

var runMigrationsOnly = builder.Configuration.GetValue<bool>("RUN_MIGRATIONS_ONLY");

if (runMigrationsOnly)
{
    await app.Services.WaitForDatabaseAndApplyMigrationsAsync(app.Logger);
    app.Logger.LogInformation("Migrations finished successfully. Exiting migrator container.");
    return;
}

// Velika izmjena: osigurava da API pri standardnom startup-u automatski primijeni
// sve pending migracije (npr. dodavanje username/password kolona za lokalni auth).
await app.Services.WaitForDatabaseAndApplyMigrationsAsync(app.Logger);

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

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message ?? "Došlo je do greške."
        });
    });
});

app.UseCors("FrontendDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();