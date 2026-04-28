using ConferenceManagement.Api.Controllers;
using ConferenceManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IKeycloakService> _mockKeycloakService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockKeycloakService = new Mock<IKeycloakService>();
            _controller = new UserController(_mockKeycloakService.Object);
            
            // Mocking Request for the controller
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Logout_Returns_Ok_When_Token_Is_Present()
        {
            // Arrange
            _controller.Request.Headers["Authorization"] = "Bearer mock-token";
            _mockKeycloakService.Setup(s => s.LogoutUser(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Logout_Returns_BadRequest_When_Token_Is_Missing()
        {
            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public void Login_Returns_Ok()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
