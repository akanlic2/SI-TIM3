using ConferenceManagement.Domain.Entities;
using ConferenceManagement.Dal;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ConferenceManagement.Api.Middlewear
{
    public class KeycloakUserSyncMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<KeycloakUserSyncMiddleware> _logger;

        public KeycloakUserSyncMiddleware(RequestDelegate next, ILogger<KeycloakUserSyncMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        private string GetRelevantRole(ClaimsPrincipal principal)
        {
            var allRoles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var ignoredRoles = new[] { "offline_access", "uma_authorization" };
            
            var relevantRoles = allRoles
                .Where(r => !ignoredRoles.Contains(r) && !r.StartsWith("default-roles-"))
                .ToList();
                
            return relevantRoles.FirstOrDefault() ?? string.Empty;
        }

        private User MapToUserEntity(ClaimsPrincipal principal)
        {
            return new User
            {
                KeycloakUserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                FirstName = principal.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
                LastName = principal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty,
                Role = GetRelevantRole(principal),
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var keycloakId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (!string.IsNullOrEmpty(keycloakId))
                    {
                        var existingUser = await dbContext.Users
                            .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakId);

                        if (existingUser == null)
                        {
                            try
                            {
                                var newUser = MapToUserEntity(context.User);
                                dbContext.Users.Add(newUser);
                                await dbContext.SaveChangesAsync();
                                _logger.LogInformation($"User {keycloakId} synced to database");
                            }
                            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("duplicate") == true)
                            {
                                _logger.LogWarning($"User with KeycloakUserId {keycloakId} already exists (race condition handled)");
                            }
                        }
                        else
                        {
                            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                            var firstName = context.User.FindFirst(ClaimTypes.GivenName)?.Value;
                            var lastName = context.User.FindFirst(ClaimTypes.Surname)?.Value;
                            var role = GetRelevantRole(context.User);

                            if (existingUser.Email != email || existingUser.FirstName != firstName || 
                                existingUser.LastName != lastName || existingUser.Role != role)
                            {
                                existingUser.Email = email ?? existingUser.Email;
                                existingUser.FirstName = firstName ?? existingUser.FirstName;
                                existingUser.LastName = lastName ?? existingUser.LastName;
                                existingUser.Role = role ?? existingUser.Role;
                                existingUser.UpdatedAt = DateTime.UtcNow;

                                dbContext.Users.Update(existingUser);
                                await dbContext.SaveChangesAsync();
                                _logger.LogInformation($"User {keycloakId} updated");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "KeycloakUserSync failed");
                }
            }

            await _next(context);
        }
    }
}