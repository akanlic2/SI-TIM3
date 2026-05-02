using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConferenceManagement.Application.Services
{
    public class KeycloakService : IKeycloakService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public KeycloakService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        private async Task<string> GetAdminToken()
        {
            var baseUrl = _config["Keycloak:BaseUrl"];
            var realm = _config["Keycloak:Realm"];
            var clientId = _config["Keycloak:ClientId"];
            var clientSecret = _config["Keycloak:ClientSecret"];

            var body = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId!),
                new KeyValuePair<string, string>("client_secret", clientSecret!)
            });

            var response = await _httpClient.PostAsync(
                $"{baseUrl}/realms/{realm}/protocol/openid-connect/token", body);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get admin token: {response.StatusCode} - {json}");

            var tokenData = JsonSerializer.Deserialize<JsonElement>(json);
            return tokenData.GetProperty("access_token").GetString()!;
        }

        

        public async Task DeleteUser(string keycloakUserId)
        {
            var adminToken = await GetAdminToken();
            var baseUrl = _config["Keycloak:BaseUrl"];
            var realm = _config["Keycloak:Realm"];

            var request = new HttpRequestMessage(HttpMethod.Delete,
                $"{baseUrl}/admin/realms/{realm}/users/{keycloakUserId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete user from Keycloak: {response.StatusCode} - {error}");
            }
        }


        public async Task LogoutUser(string RawToken)
        {
            var token = RawToken?.Replace("Bearer ", "") ?? string.Empty;

            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is invalid or empty.");

            var baseUrl = _config["Keycloak:BaseUrl"];
            var realm = _config["Keycloak:Realm"];
            var clientId = _config["Keycloak:ClientId"];
            var clientSecret = _config["Keycloak:ClientSecret"];

            var body = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId!),
                new KeyValuePair<string, string>("client_secret", clientSecret!),
                new KeyValuePair<string, string>("token", token)
            });

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{baseUrl}/realms/{realm}/protocol/openid-connect/logout") { Content = body };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Keycloak logout failed: {response.StatusCode} - {error}");
            }
        }

        // Creates a user and assigns realm roles (e.g. ucesnik, organizator, predavac, admin)
        public async Task<string> CreateUserAsync(string username, string email, string password, IEnumerable<string>? roles = null)
        {
            var adminToken = await GetAdminToken();
            var baseUrl = _config["Keycloak:BaseUrl"];
            var realm = _config["Keycloak:Realm"];

            var createBody = new
            {
                username,
                email,
                enabled = true,
                credentials = new[] { new { type = "password", value = password, temporary = false } }
            };

            var content = new StringContent(JsonSerializer.Serialize(createBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/admin/realms/{realm}/users") { Content = content };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.Conflict)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create user in Keycloak: {response.StatusCode} - {error}");
            }

            // Try to get user id from Location header or by searching
            string? userId = null;
            if (response.Headers.Location != null)
            {
                var segments = response.Headers.Location.Segments;
                userId = segments.LastOrDefault()?.TrimEnd('/');
            }

            if (string.IsNullOrEmpty(userId))
            {
                // search by username
                var getReq = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/admin/realms/{realm}/users?username={Uri.EscapeDataString(username)}");
                getReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
                var getResp = await _httpClient.SendAsync(getReq);
                if (!getResp.IsSuccessStatusCode)
                {
                    var err = await getResp.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to find created user: {getResp.StatusCode} - {err}");
                }

                var json = await getResp.Content.ReadAsStringAsync();
                var arr = JsonSerializer.Deserialize<JsonElement>(json);
                if (arr.ValueKind == JsonValueKind.Array && arr.GetArrayLength() > 0)
                {
                    userId = arr.EnumerateArray().First().GetProperty("id").GetString();
                }
            }

            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unable to determine Keycloak user id after creation.");

            if (roles != null && roles.Any())
            {
                await AssignRealmRolesAsync(userId, roles, adminToken);
            }

            return userId;
        }

        public async Task AssignRealmRolesAsync(string userId, IEnumerable<string> roles, string? adminToken = null)
        {
            adminToken ??= await GetAdminToken();
            var baseUrl = _config["Keycloak:BaseUrl"];
            var realm = _config["Keycloak:Realm"];

            var roleReprs = new List<JsonElement>();

            foreach (var role in roles)
            {
                var getRoleReq = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/admin/realms/{realm}/roles/{Uri.EscapeDataString(role)}");
                getRoleReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
                var getRoleResp = await _httpClient.SendAsync(getRoleReq);
                if (!getRoleResp.IsSuccessStatusCode)
                {
                    // try to create role if it does not exist
                    var createRoleContent = new StringContent(JsonSerializer.Serialize(new { name = role }), Encoding.UTF8, "application/json");
                    var createRoleReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/admin/realms/{realm}/roles") { Content = createRoleContent };
                    createRoleReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
                    var createRoleResp = await _httpClient.SendAsync(createRoleReq);
                    if (!createRoleResp.IsSuccessStatusCode && createRoleResp.StatusCode != System.Net.HttpStatusCode.Conflict)
                    {
                        var err = await createRoleResp.Content.ReadAsStringAsync();
                        throw new Exception($"Failed to create role '{role}': {createRoleResp.StatusCode} - {err}");
                    }

                    // fetch again
                    getRoleResp = await _httpClient.SendAsync(getRoleReq);
                    if (!getRoleResp.IsSuccessStatusCode)
                    {
                        var err = await getRoleResp.Content.ReadAsStringAsync();
                        throw new Exception($"Failed to get role '{role}': {getRoleResp.StatusCode} - {err}");
                    }
                }

                var roleJson = await getRoleResp.Content.ReadAsStringAsync();
                var element = JsonSerializer.Deserialize<JsonElement>(roleJson);
                roleReprs.Add(element);
            }

            var mappingContent = new StringContent(JsonSerializer.Serialize(roleReprs), Encoding.UTF8, "application/json");
            var mapReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/admin/realms/{realm}/users/{userId}/role-mappings/realm") { Content = mappingContent };
            mapReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var mapResp = await _httpClient.SendAsync(mapReq);
            if (!mapResp.IsSuccessStatusCode)
            {
                var err = await mapResp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to assign roles to user: {mapResp.StatusCode} - {err}");
            }
        }
    }
}
