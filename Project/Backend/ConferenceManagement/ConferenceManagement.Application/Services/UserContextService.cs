using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ConferenceManagement.Application.Services
{
    /// <inheritdoc />
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal GetCurrentUser()
        {
            return _httpContextAccessor?.HttpContext?.User 
                ?? throw new InvalidOperationException("Nema dostupnog HTTP konteksta. UserContextService se može koristiti samo u kontekstu HTTP zahtjeva.");
        }

        /// <inheritdoc />
        public IEnumerable<string> GetUserRoles()
        {
            var user = GetCurrentUser();
            return user.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }

        /// <inheritdoc />
        public bool HasRole(string role)
        {
            var user = GetCurrentUser();
            return user.HasClaim(ClaimTypes.Role, role?.ToLower() ?? "");
        }

        /// <inheritdoc />
        public bool HasAnyRole(params string[] roles)
        {
            var userRoles = GetUserRoles();
            return roles.Any(r => userRoles.Contains(r.ToLower()));
        }

        /// <inheritdoc />
        public bool HasAllRoles(params string[] roles)
        {
            var userRoles = GetUserRoles();
            return roles.All(r => userRoles.Contains(r.ToLower()));
        }

        /// <inheritdoc />
        public string GetUserId()
        {
            var user = GetCurrentUser();
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new InvalidOperationException("Korisnik nema dostupnog ID-a (NameIdentifier claim).");
        }

        /// <inheritdoc />
        public string GetUsername()
        {
            var user = GetCurrentUser();
            return user.FindFirst(ClaimTypes.Name)?.Value 
                ?? user.FindFirst("preferred_username")?.Value 
                ?? throw new InvalidOperationException("Korisnik nema dostupnog korisničkog imena.");
        }

        /// <inheritdoc />
        public bool IsAuthenticated()
        {
            var user = GetCurrentUser();
            return user?.Identity?.IsAuthenticated ?? false;
        }
    }
}
