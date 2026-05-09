using System.Security.Claims;
using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Application.Interfaces;
using ConferenceManagement.Application.DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserContextService> _mockUserContextService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockUserContextService = new Mock<IUserContextService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ConferenceManagement_LocalAuth_SigningKey_2026_StrongKey");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("ConferenceManagement.Api");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("ConferenceManagement.Client");
            _mockConfiguration.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("120");

            _controller = new UserController(_mockUserService.Object, _mockUserContextService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        #region Register Tests

        [Fact]
        public async Task Register_Returns_BadRequest_When_Required_Fields_Missing()
        {
            var result = await _controller.Register(new UserController.RegisterRequest
            {
                Username = "",
                Email = "test@test.com",
                Password = "password",
                FirstName = "A",
                LastName = "B"
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_Returns_Conflict_When_Username_Exists()
        {
            _mockUserService.Setup(s => s.UsernameExistsAsync("existing")).ReturnsAsync(true);

            var result = await _controller.Register(new UserController.RegisterRequest
            {
                Username = "existing",
                Email = "test@test.com",
                Password = "password123",
                FirstName = "A",
                LastName = "B"
            });

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Register_Returns_Conflict_When_Email_Exists()
        {
            _mockUserService.Setup(s => s.UsernameExistsAsync("newuser")).ReturnsAsync(false);
            _mockUserService.Setup(s => s.EmailExistsAsync("existing@test.com")).ReturnsAsync(true);

            var result = await _controller.Register(new UserController.RegisterRequest
            {
                Username = "newuser",
                Email = "existing@test.com",
                Password = "password123",
                FirstName = "A",
                LastName = "B"
            });

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Register_Returns_Ok_With_Valid_Data()
        {
            var userId = Guid.NewGuid();
            var registerRequest = new UserController.RegisterRequest
            {
                Username = "newuser",
                Email = "newuser@test.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe",
                Role = "ucesnik"
            };

            _mockUserService.Setup(s => s.UsernameExistsAsync("newuser")).ReturnsAsync(false);
            _mockUserService.Setup(s => s.EmailExistsAsync("newuser@test.com")).ReturnsAsync(false);
            _mockUserService.Setup(s => s.RegisterUserAsync(It.IsAny<RegisterUserDto>()))
                .ReturnsAsync(new UserDto
                {
                    UserId = userId,
                    Username = "newuser",
                    Email = "newuser@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Role = "ucesnik",
                    CreatedAt = DateTime.UtcNow
                });

            var result = await _controller.Register(registerRequest);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_Returns_Unauthorized_For_Invalid_Credentials()
        {
            _mockUserService
                .Setup(s => s.GetUserByUsernameOrEmailAndPasswordAsync("user", "bad"))
                .ReturnsAsync((UserDto?)null);

            var result = await _controller.Login(new UserController.LoginRequest
            {
                UsernameOrEmail = "user",
                Password = "bad"
            });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_Returns_Token_For_Valid_Credentials()
        {
            var userId = Guid.NewGuid();
            _mockUserService
                .Setup(s => s.GetUserByUsernameOrEmailAndPasswordAsync("user", "good"))
                .ReturnsAsync(new UserDto
                {
                    UserId = userId,
                    Username = "user",
                    Email = "user@test.com",
                    Role = "ucesnik",
                    FirstName = "A",
                    LastName = "B",
                    CreatedAt = DateTime.UtcNow
                });

            var result = await _controller.Login(new UserController.LoginRequest
            {
                UsernameOrEmail = "user",
                Password = "good"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            var responseValue = ok.Value;
            Assert.NotNull(responseValue);
        }

        [Fact]
        public async Task Login_Returns_Ok_With_Token_And_User_Data()
        {
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@test.com",
                Role = "ucesnik",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserService
                .Setup(s => s.GetUserByUsernameOrEmailAndPasswordAsync("testuser", "password"))
                .ReturnsAsync(userDto);

            var result = await _controller.Login(new UserController.LoginRequest
            {
                UsernameOrEmail = "testuser",
                Password = "password"
            });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public void Logout_Returns_Ok()
        {
            var result = _controller.Logout();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        #endregion

        #region Current Tests

        [Fact]
        public async Task Current_Returns_Unauthorized_When_Invalid_Token()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            var result = await _controller.Current();

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Current_Returns_NotFound_When_User_Does_Not_Exist()
        {
            var userId = Guid.NewGuid();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDto?)null);

            var result = await _controller.Current();

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Current_Returns_Ok_When_User_Exists()
        {
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                UserId = userId,
                Username = "john",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userDto);

            var result = await _controller.Current();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }

        #endregion

        #region GetAllUsers Tests

        [Fact]
        public async Task GetAllUsers_Returns_Ok_With_Users()
        {
            var users = new List<UserDto>
            {
                new UserDto
                {
                    UserId = Guid.NewGuid(),
                    Username = "user1",
                    Email = "user1@test.com",
                    FirstName = "User",
                    LastName = "One",
                    Role = "ucesnik",
                    CreatedAt = DateTime.UtcNow
                },
                new UserDto
                {
                    UserId = Guid.NewGuid(),
                    Username = "user2",
                    Email = "user2@test.com",
                    FirstName = "User",
                    LastName = "Two",
                    Role = "ucesnik",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_Returns_Forbidden_When_Unauthorized()
        {
            var userId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());

            var result = await _controller.GetById(targetUserId);

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_User_Does_Not_Exist()
        {
            var userId = Guid.NewGuid();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDto?)null);

            var result = await _controller.GetById(userId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_User_Exists_And_Authorized()
        {
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                UserId = userId,
                Username = "john",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userDto);

            var result = await _controller.GetById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Admin_Requests_Different_User()
        {
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var userDto = new UserDto
            {
                UserId = targetUserId,
                Username = "john",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminUserId.ToString()),
                new Claim(ClaimTypes.Role, "admin-sistema")
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(adminUserId.ToString());
            _mockUserService.Setup(s => s.GetUserByIdAsync(targetUserId)).ReturnsAsync(userDto);

            var result = await _controller.GetById(targetUserId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_Returns_Forbidden_When_Unauthorized()
        {
            var userId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { FirstName = "Jane" };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());

            var result = await _controller.Update(targetUserId, updateDto);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Update_Returns_Conflict_When_Username_Exists()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { Username = "existinguser", FirstName = "Jane" };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());
            _mockUserService.Setup(s => s.UsernameExistsAsync("existinguser", userId)).ReturnsAsync(true);

            var result = await _controller.Update(userId, updateDto);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Update_Returns_Conflict_When_Email_Exists()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { Email = "existing@test.com", FirstName = "Jane" };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());
            _mockUserService.Setup(s => s.UsernameExistsAsync("", userId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.EmailExistsAsync("existing@test.com", userId)).ReturnsAsync(true);

            var result = await _controller.Update(userId, updateDto);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Update_Returns_NotFound_When_User_Does_Not_Exist()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { FirstName = "Jane" };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());
            _mockUserService.Setup(s => s.UsernameExistsAsync("", userId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.EmailExistsAsync("", userId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(false);

            var result = await _controller.Update(userId, updateDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_User_Updated_Successfully()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { FirstName = "Jane", LastName = "Doe" };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(userId.ToString());
            _mockUserService.Setup(s => s.UsernameExistsAsync("", userId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.EmailExistsAsync("", userId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            var result = await _controller.Update(userId, updateDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Admin_Updates_Different_User()
        {
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { FirstName = "Jane" };

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminUserId.ToString()),
                new Claim(ClaimTypes.Role, "admin-sistema") 
            });
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = principal;
            _mockUserContextService.Setup(s => s.GetUserId()).Returns(adminUserId.ToString());
            _mockUserService.Setup(s => s.UsernameExistsAsync("", targetUserId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.EmailExistsAsync("", targetUserId)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.UpdateUserAsync(targetUserId, updateDto)).ReturnsAsync(true);

            var result = await _controller.Update(targetUserId, updateDto);

            Assert.IsType<NoContentResult>(result);
        }

        #endregion
    }
}
