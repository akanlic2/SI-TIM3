using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.Services;
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
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("ConferenceManagement_LocalAuth_SigningKey_2026_StrongKey");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("ConferenceManagement.Api");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("ConferenceManagement.Client");
            _mockConfiguration.Setup(c => c["Jwt:ExpiresMinutes"]).Returns("120");

            _controller = new UserController(_mockUserService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Register_Returns_Conflict_When_Username_Exists()
        {
            _mockUserService.Setup(s => s.UsernameExistsAsync("existing")).ReturnsAsync(true);

            var result = await _controller.Register(new UserController.RegisterRequest
            {
                Username = "existing",
                Email = "test@test.com",
                Password = "123",
                FirstName = "A",
                LastName = "B"
            });

            Assert.IsType<ConflictObjectResult>(result);
        }

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
            _mockUserService
                .Setup(s => s.GetUserByUsernameOrEmailAndPasswordAsync("user", "good"))
                .ReturnsAsync(new UserDto
                {
                    UserId = Guid.NewGuid(),
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
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_User_Exists()
        {
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(new UserDto
            {
                UserId = userId,
                Username = "john",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            });

            var result = await _controller.GetById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_Returns_NotFound_When_User_Does_Not_Exist()
        {
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto { FirstName = "Jane" };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(false);

            var result = await _controller.Update(userId, updateDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
