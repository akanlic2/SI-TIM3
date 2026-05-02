using Microsoft.AspNetCore.Mvc;
using ConferenceManagement.Application.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;

namespace ConferenceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IKeycloakService _keycloakService;

        public UserController(IKeycloakService keycloakService)
        {
            _keycloakService = keycloakService;
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            return Ok(new { message = "Login is handled via Keycloak authentication flow." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { error = "No token provided." });
                
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
