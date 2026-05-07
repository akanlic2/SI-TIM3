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
        private readonly IUserService _userService;

        public UserController(IKeycloakService keycloakService, IUserService userService)
        {
            _keycloakService = keycloakService;
            _userService = userService;
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

        [Authorize(Policy = "ParticipantPolicy")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user is null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            return Ok(user);
        }

        [Authorize(Policy = "ParticipantPolicy")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            var updated = await _userService.UpdateUserAsync(id, dto);

            if (!updated)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            return NoContent();
        }
    }
}
