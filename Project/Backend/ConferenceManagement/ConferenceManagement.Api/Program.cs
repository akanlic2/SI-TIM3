using ConferenceManagement.Api.Extensions;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Dal;
using ConferenceManagement.Dal.Repositories;
using ConferenceManagement.Domain.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        }));

builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.AddScoped<IConferenceService, ConferenceService>();

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
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();