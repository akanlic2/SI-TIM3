using ConferenceManagement.Api.Extensions;
using ConferenceManagement.Api.Middlewear;
using ConferenceManagement.Api.Modules;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Dal;
using ConferenceManagement.Dal.Repositories;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

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
        var keycloakSection = builder.Configuration.GetSection("Keycloak");
        var authority = keycloakSection["Authority"];

        options.Authority = authority;
        options.MetadataAddress = "http://keycloak:8080/keycloak/realms/conference-app/.well-known/openid-configuration";
        options.Audience = keycloakSection["Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuers = new[] {
                authority,
                authority.Replace("keycloak", "localhost"),
                "http://localhost:8080/keycloak/realms/conference-app"
            },
            NameClaimType = "sub",
            RoleClaimType = "role"

           
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
builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.AddScoped<IConferenceService, ConferenceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKeycloakService, KeycloakService>();
builder.Services.AddHttpClient<IKeycloakService, KeycloakService>();
builder.Services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, ConferenceManagement.Api.Middlewear.KeycloakRolesTransformer>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminSistemaPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(System.Security.Claims.ClaimTypes.Role, "admin-sistema")))
    .AddPolicy("OrganizerPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(System.Security.Claims.ClaimTypes.Role, "organizator")))
    .AddPolicy("AdminOrOrganizerPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(System.Security.Claims.ClaimTypes.Role, "admin-sistema") ||
            context.User.HasClaim(System.Security.Claims.ClaimTypes.Role, "organizator")))
    .AddPolicy("SpeakerPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(System.Security.Claims.ClaimTypes.Role, "predavac")))
    .AddPolicy("ParticipantPolicy", policy =>
        policy.RequireAuthenticatedUser());

var app = builder.Build();

var runMigrationsOnly = builder.Configuration.GetValue<bool>("RUN_MIGRATIONS_ONLY");

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

app.UseCors("FrontendDev");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<KeycloakUserSyncMiddleware>();

app.MapControllers();
app.MapUserEndpoints();


app.Run();