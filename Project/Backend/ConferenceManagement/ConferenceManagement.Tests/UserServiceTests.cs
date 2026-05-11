using ConferenceManagement.Application.DTOs.User;
using ConferenceManagement.Application.Services;
using ConferenceManagement.Domain.Abstractions.Repositories;
using ConferenceManagement.Domain.Entities;
using Moq;
using Xunit;

namespace ConferenceManagement.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        #region GetUserCountAsync Tests

        [Fact]
        public async Task GetUserCountAsync_Returns_Count_From_Repository()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            // Act
            var result = await _userService.GetUserCountAsync();

            // Assert
            Assert.Equal(5, result);
            _mockUserRepository.Verify(r => r.GetCountAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserCountAsync_Returns_Zero_When_No_Users_Exist()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _userService.GetUserCountAsync();

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region GetAllUsersAsync Tests

        [Fact]
        public async Task GetAllUsersAsync_Returns_All_Users_Mapped_To_Dto()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "user1",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Role = "ucesnik",
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "user2",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    Role = "organizer",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(users[0].UserId, result[0].UserId);
            Assert.Equal(users[0].Username, result[0].Username);
            Assert.Equal(users[1].Email, result[1].Email);
            _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_Empty_List_When_No_Users_Exist()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_Maps_All_User_Fields_Correctly()
        {
            // Arrange
            var createdAt = DateTime.UtcNow;
            var users = new List<User>
            {
                new User
                {
                    UserId = Guid.NewGuid(),
                    Username = "testuser",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    Role = "admin",
                    CreatedAt = createdAt
                }
            };

            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            var userDto = result.First();
            Assert.Equal(users[0].UserId, userDto.UserId);
            Assert.Equal(users[0].Username, userDto.Username);
            Assert.Equal(users[0].FirstName, userDto.FirstName);
            Assert.Equal(users[0].LastName, userDto.LastName);
            Assert.Equal(users[0].Email, userDto.Email);
            Assert.Equal(users[0].Role, userDto.Role);
            Assert.Equal(createdAt, userDto.CreatedAt);
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Fact]
        public async Task GetUserByIdAsync_Returns_User_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            _mockUserRepository.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_Returns_Null_When_User_Does_Not_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserByUsernameOrEmailAndPasswordAsync Tests

        [Fact]
        public async Task GetUserByUsernameOrEmailAndPasswordAsync_Returns_User_When_User_Exists()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.GetByUsernameOrEmailAndPasswordAsync("testuser", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByUsernameOrEmailAndPasswordAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameOrEmailAndPasswordAsync_Returns_User_When_Email_And_Password_Match()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.GetByUsernameOrEmailAndPasswordAsync("test@example.com", "password", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByUsernameOrEmailAndPasswordAsync("test@example.com", "password");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByUsernameOrEmailAndPasswordAsync_Returns_Null_When_Credentials_Invalid()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByUsernameOrEmailAndPasswordAsync("invalid", "wrong", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUserByUsernameOrEmailAndPasswordAsync("invalid", "wrong");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region RegisterUserAsync Tests

        [Fact]
        public async Task RegisterUserAsync_Creates_User_With_Default_Role_When_Role_Not_Provided()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = null
            };

            var createdUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ucesnik", result.Role);
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_Creates_User_With_Provided_Role_In_Lowercase()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "ORGANIZER"
            };

            var createdUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "organizer",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("organizer", result.Role);
        }

        [Fact]
        public async Task RegisterUserAsync_Trims_Whitespace_From_User_Fields()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "  newuser  ",
                Password = "password123",
                FirstName = "  New  ",
                LastName = "  User  ",
                Email = "  new@example.com  ",
                Role = "  UCESNIK  "
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .ReturnsAsync((User user, CancellationToken ct) => user);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.Equal("newuser", capturedUser.Username);
            Assert.Equal("New", capturedUser.FirstName);
            Assert.Equal("User", capturedUser.LastName);
            Assert.Equal("new@example.com", capturedUser.Email);
            Assert.Equal("ucesnik", capturedUser.Role);
        }

        [Fact]
        public async Task RegisterUserAsync_Creates_User_With_New_UserId()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "ucesnik"
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .ReturnsAsync((User user, CancellationToken ct) => user);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.NotEqual(Guid.Empty, capturedUser.UserId);
            Assert.NotEqual(Guid.Empty, result.UserId);
        }

        [Fact]
        public async Task RegisterUserAsync_Sets_CreatedAt_To_Current_Time()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com"
            };

            var beforeTime = DateTime.UtcNow;
            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .ReturnsAsync((User user, CancellationToken ct) => user);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);
            var afterTime = DateTime.UtcNow;

            // Assert
            Assert.True(beforeTime <= capturedUser.CreatedAt && capturedUser.CreatedAt <= afterTime);
        }

        [Fact]
        public async Task RegisterUserAsync_Returns_UserDto_With_All_Fields()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "admin"
            };

            var createdUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "admin",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.Equal(createdUser.UserId, result.UserId);
            Assert.Equal(createdUser.Username, result.Username);
            Assert.Equal(createdUser.FirstName, result.FirstName);
            Assert.Equal(createdUser.LastName, result.LastName);
            Assert.Equal(createdUser.Email, result.Email);
            Assert.Equal(createdUser.Role, result.Role);
            Assert.Equal(createdUser.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task RegisterUserAsync_With_Empty_Role_String_Uses_Default_Role()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = ""
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .ReturnsAsync((User user, CancellationToken ct) => user);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.Equal("ucesnik", capturedUser.Role);
        }

        [Fact]
        public async Task RegisterUserAsync_With_Whitespace_Role_String_Uses_Default_Role()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "   "
            };

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .ReturnsAsync((User user, CancellationToken ct) => user);

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.Equal("ucesnik", capturedUser.Role);
        }

        #endregion

        #region UsernameExistsAsync Tests

        [Fact]
        public async Task UsernameExistsAsync_Returns_True_When_Username_Exists()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.AnyByUsernameAsync("existinguser", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.UsernameExistsAsync("existinguser");

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.AnyByUsernameAsync("existinguser", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UsernameExistsAsync_Returns_False_When_Username_Does_Not_Exist()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.AnyByUsernameAsync("nonexistent", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.UsernameExistsAsync("nonexistent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UsernameExistsAsync_Passes_UserId_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(r => r.AnyByUsernameAsync("testuser", userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.UsernameExistsAsync("testuser", userId);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.AnyByUsernameAsync("testuser", userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region EmailExistsAsync Tests

        [Fact]
        public async Task EmailExistsAsync_Returns_True_When_Email_Exists()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.AnyByEmailAsync("test@example.com", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.EmailExistsAsync("test@example.com");

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.AnyByEmailAsync("test@example.com", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EmailExistsAsync_Returns_False_When_Email_Does_Not_Exist()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.AnyByEmailAsync("nonexistent@example.com", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.EmailExistsAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmailExistsAsync_Passes_UserId_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(r => r.AnyByEmailAsync("test@example.com", userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.EmailExistsAsync("test@example.com", userId);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.AnyByEmailAsync("test@example.com", userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateUserAsync Tests

        [Fact]
        public async Task UpdateUserAsync_Returns_False_When_User_Does_Not_Exist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateDto = new UpdateUserDto
            {
                FirstName = "Updated",
                LastName = "User"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_FirstName_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Old",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                FirstName = "Updated"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("Updated", capturedUser.FirstName);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_LastName_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "Old",
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                LastName = "Updated"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("Updated", capturedUser.LastName);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_Email_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "old@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                Email = "new@example.com"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("new@example.com", capturedUser.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_Username_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "oldusername",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                Username = "newusername"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("newusername", capturedUser.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_Password_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "oldpassword",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                Password = "newpassword"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("newpassword", capturedUser.Password);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_Role_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                Role = "organizer"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("organizer", capturedUser.Role);
        }

        [Fact]
        public async Task UpdateUserAsync_Does_Not_Update_Fields_When_Not_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalFirstName = "Original";
            var originalLastName = "Name";
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = originalFirstName,
                LastName = originalLastName,
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                // All fields are null/empty
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal(originalFirstName, capturedUser.FirstName);
            Assert.Equal(originalLastName, capturedUser.LastName);
        }

        [Fact]
        public async Task UpdateUserAsync_Does_Not_Update_Field_When_Empty_String_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalFirstName = "Original";
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = originalFirstName,
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                FirstName = ""
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal(originalFirstName, capturedUser.FirstName);
        }

        [Fact]
        public async Task UpdateUserAsync_Does_Not_Update_Field_When_Whitespace_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalLastName = "Original";
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = originalLastName,
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                LastName = "   "
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal(originalLastName, capturedUser.LastName);
        }

        [Fact]
        public async Task UpdateUserAsync_Updates_Multiple_Fields_When_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "olduser",
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                FirstName = "New",
                LastName = "Updated",
                Email = "new@example.com",
                Username = "newuser"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal("New", capturedUser.FirstName);
            Assert.Equal("Updated", capturedUser.LastName);
            Assert.Equal("new@example.com", capturedUser.Email);
            Assert.Equal("newuser", capturedUser.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_Sets_UpdatedAt_Timestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik",
                UpdatedAt = null
            };

            var updateDto = new UpdateUserDto
            {
                FirstName = "Updated"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var beforeTime = DateTime.UtcNow;
            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);
            var afterTime = DateTime.UtcNow;

            // Assert
            Assert.True(result);
            Assert.NotNull(capturedUser.UpdatedAt);
            Assert.True(beforeTime <= capturedUser.UpdatedAt && capturedUser.UpdatedAt <= afterTime);
        }

        [Fact]
        public async Task UpdateUserAsync_Calls_Repository_Update_Method()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Role = "ucesnik"
            };

            var updateDto = new UpdateUserDto
            {
                FirstName = "Updated"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public async Task Multiple_Operations_Work_Sequentially()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var registerDto = new RegisterUserDto
            {
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com"
            };

            var newUser = new User
            {
                UserId = userId,
                Username = "newuser",
                Password = "password123",
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                Role = "ucesnik",
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newUser);

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newUser);

            _mockUserRepository.Setup(r => r.AnyByEmailAsync("new@example.com", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var registeredUser = await _userService.RegisterUserAsync(registerDto);
            var retrievedUser = await _userService.GetUserByIdAsync(userId);
            var emailExists = await _userService.EmailExistsAsync("new@example.com");

            // Assert
            Assert.NotNull(registeredUser);
            Assert.NotNull(retrievedUser);
            Assert.True(emailExists);
            Assert.Equal(registeredUser.UserId, retrievedUser.UserId);
        }

        [Fact]
        public async Task UpdateUserAsync_Preserves_Unmodified_Fields()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalPassword = "originalpassword";
            var originalRole = "ucesnik";
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = originalPassword,
                Role = originalRole
            };

            var updateDto = new UpdateUserDto
            {
                FirstName = "Updated"
                // Other fields are not provided
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            User capturedUser = null!;
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);
            Assert.Equal(originalPassword, capturedUser.Password);
            Assert.Equal(originalRole, capturedUser.Role);
        }

        #endregion
    }
}
