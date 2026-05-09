using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.DTOs.User;
using Microsoft.IdentityModel.Tokens;
using ConferenceManagement.Application.Services;

namespace ConferenceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserContextService _userContextService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IUserContextService userContextService, IConfiguration configuration)
        {
            _userService = userService;
            _userContextService = userContextService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Velika izmjena: prelazak sa eksternog IdP na lokalnu registraciju korisnika u bazi.
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { error = "Sva obavezna polja moraju biti ispunjena" });
            }

            if (await _userService.UsernameExistsAsync(request.Username))
            {
                return Conflict(new { error = "Korisničko ime već postoji." });
            }

            if (await _userService.EmailExistsAsync(request.Email))
            {
                return Conflict(new { error = "Email već postoji." });
            }

            var createdUser = await _userService.RegisterUserAsync(new RegisterUserDto
            {
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Role = request.Role
            });

            return Ok(createdUser);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByUsernameOrEmailAndPasswordAsync(request.UsernameOrEmail, request.Password);

            if (user is null)
            {
                return Unauthorized(new { error = "Nevalidni podaci" });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Uspješna odjava" });
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> Current()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { error = "Token nije validan" });
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new { Message = "Korisnik nije pronađen" });
            }

            return Ok(user);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("/api/users/all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { users, count = users.Count });
        }

        [Authorize(Policy = "ParticipantPolicy")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            if (!Guid.TryParse(_userContextService.GetUserId(), out var userId) ||
                (userId != id && !User.IsInRole("admin")))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user is null)
            {
                return NotFound(new { Message = $"Korisnik sa ID {id} nije pronađen." });
            }

            return Ok(user);
        }

        [Authorize(Policy = "ParticipantPolicy")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!Guid.TryParse(_userContextService.GetUserId(), out var userId) ||
                (userId != id && !User.IsInRole("admin")))
            {
                return Forbid();
            }

            var username = dto.Username ?? "";
            var email = dto.Email ?? "";

            if (await _userService.UsernameExistsAsync(username, id))
            {
                return Conflict(new { error = "Korisničko ime već postoji." });
            }

            if (await _userService.EmailExistsAsync(email, id))
            {
                return Conflict(new { error = "Email već postoji." });
            }

            var updated = await _userService.UpdateUserAsync(id, dto);

            if (!updated)
            {
                return NotFound(new { Message = $"Korisnik sa ID {id} nije pronađen." });
            }

            return NoContent();
        }

        private string GenerateJwtToken(UserDto user)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? "ConferenceManagement.Api";
            var audience = _configuration["Jwt:Audience"] ?? "ConferenceManagement.Client";
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var parsed) ? parsed : 120;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
                new("userId", user.UserId.ToString()),
                new("username", user.Username),
                new("email", user.Email),
                new("role", user.Role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Role { get; set; }
        }

        public class LoginRequest
        {
            public string UsernameOrEmail { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
