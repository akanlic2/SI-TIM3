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
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockKeycloakService = new Mock<IKeycloakService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockKeycloakService.Object, _mockUserService.Object);

            // Mocking Request for the controller
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        #region Login Tests

        [Fact]
        public void Login_Returns_Ok()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        #endregion

        #region Logout Tests

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

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_Returns_Ok_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                UserId = userId,
                KeycloakUserId = "keycloak-123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = "Participant",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, returnedUser.UserId);
            Assert.Equal("John", returnedUser.FirstName);
            Assert.Equal("john.doe@example.com", returnedUser.Email);

            _mockUserService.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_User_Does_Not_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);

            _mockUserService.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetById_Returns_Different_User_Data_For_Different_Ids()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var userDto1 = new UserDto
            {
                UserId = userId1,
                FirstName = "Alice",
                Email = "alice@example.com"
            };

            var userDto2 = new UserDto
            {
                UserId = userId2,
                FirstName = "Bob",
                Email = "bob@example.com"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId1)).ReturnsAsync(userDto1);
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId2)).ReturnsAsync(userDto2);

            // Act & Assert for first user
            var result1 = await _controller.GetById(userId1);
            var okResult1 = Assert.IsType<OkObjectResult>(result1.Result);
            var returnedUser1 = Assert.IsType<UserDto>(okResult1.Value);
            Assert.Equal("Alice", returnedUser1.FirstName);

            // Act & Assert for second user
            var result2 = await _controller.GetById(userId2);
            var okResult2 = Assert.IsType<OkObjectResult>(result2.Result);
            var returnedUser2 = Assert.IsType<UserDto>(okResult2.Value);
            Assert.Equal("Bob", returnedUser2.FirstName);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_Returns_NoContent_When_Update_Succeeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_Returns_NotFound_When_User_Does_Not_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FirstName = "Jane"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);

            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_With_Only_FirstName_Returns_NoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FirstName = "UpdatedName",
                LastName = null,
                Email = null
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, It.Is<UpdateUserDto>(
                dto => dto.FirstName == "UpdatedName" && dto.LastName == null && dto.Email == null
            )), Times.Once);
        }

        [Fact]
        public async Task Update_With_Only_Email_Returns_NoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                Email = "newemail@example.com"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_With_All_Fields_Returns_NoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Role = "Organizer"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_With_Null_Dto_Returns_NoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto();

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(userId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId, updateDto), Times.Once);
        }

        [Fact]
        public async Task Update_Different_Users_With_Same_Data()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FirstName = "Same",
                Email = "same@example.com"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId1, updateDto)).ReturnsAsync(true);
            _mockUserService.Setup(s => s.UpdateUserAsync(userId2, updateDto)).ReturnsAsync(true);

            // Act
            var result1 = await _controller.Update(userId1, updateDto);
            var result2 = await _controller.Update(userId2, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result1);
            Assert.IsType<NoContentResult>(result2);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId1, updateDto), Times.Once);
            _mockUserService.Verify(s => s.UpdateUserAsync(userId2, updateDto), Times.Once);
        }

        #endregion
    }
}
