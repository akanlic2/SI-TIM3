using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace ConferenceManagement.Api.Middlewear
{
    public class KeycloakRolesTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal Principal)
        {
            if (Principal.HasClaim(C => C.Type == ClaimTypes.Role))
            {
                return Task.FromResult(Principal);
            }

            var Clone = Principal.Clone();
            var NewIdentity = Clone.Identity as ClaimsIdentity;

            if (NewIdentity == null)
                return Task.FromResult(Principal);

            var RealmAccessClaim = Principal.FindFirst("realm_access")?.Value;
            if (!string.IsNullOrWhiteSpace(RealmAccessClaim))
            {
                using var RealmDoc = JsonDocument.Parse(RealmAccessClaim);
                if (RealmDoc.RootElement.TryGetProperty("roles", out var Roles))
                {
                    foreach (var Role in Roles.EnumerateArray())
                    {
                        NewIdentity.AddClaim(new Claim(ClaimTypes.Role, Role.GetString()!.ToLower()));
                    }
                }
            }

            var ResourceAccessClaim = Principal.FindFirst("resource_access")?.Value;
            if (!string.IsNullOrWhiteSpace(ResourceAccessClaim))
            {
                using var ResourceDoc = JsonDocument.Parse(ResourceAccessClaim);

                if (ResourceDoc.RootElement.TryGetProperty("conference-backend", out var ClientAccess) &&
                    ClientAccess.TryGetProperty("roles", out var ClientRoles))
                {
                    foreach (var Role in ClientRoles.EnumerateArray())
                    {
                        var RoleName = Role.GetString()!.ToLower();

                        if (!NewIdentity.HasClaim(ClaimTypes.Role, RoleName))
                        {
                            NewIdentity.AddClaim(new Claim(ClaimTypes.Role, RoleName));
                        }
                    }
                }
            }

            return Task.FromResult(Clone);
        }
    }
}