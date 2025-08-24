using FluentAssertions;
using GameCore.Api.DTOs;
using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserWalletRepository> _walletRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly JwtService _jwtService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _walletRepositoryMock = new Mock<IUserWalletRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // 設定 JWT 配置
        _configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("your-super-secret-key-with-at-least-32-characters");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("GameCore");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("GameCoreUsers");
        _configurationMock.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");

        _jwtService = new JwtService(_configurationMock.Object);
        _authService = new AuthService(_userRepositoryMock.Object, _walletRepositoryMock.Object, _jwtService, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(request.Username))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);

        // Act
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.ErrorMessage.Should().BeNull();
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be(request.Username);
        result.User.Email.Should().Be(request.Email);
        result.User.Balance.Should().Be(100.00m);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ShouldReturnFailure()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(request.Username))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("用戶名已存在");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(request.Username))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("郵箱已被註冊");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithBoundaryValues_ShouldHandleCorrectly()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = TestDataFactory.BoundaryValues.MinUsername,
            Email = TestDataFactory.BoundaryValues.MaxEmail,
            Password = TestDataFactory.BoundaryValues.MinPassword,
            ConfirmPassword = TestDataFactory.BoundaryValues.MinPassword
        };

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(request.Username))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);

        // Act
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.User!.Username.Should().Be(request.Username);
        result.User.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var request = TestDataFactory.CreateValidLoginRequest();
        var user = TestDataFactory.CreateTestUser();
        user.Username = request.Username;

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);
        _walletRepositoryMock.Setup(x => x.GetBalanceAsync(user.UserId))
            .ReturnsAsync(100.00m);

        // Act
        var result = await _authService.LoginAsync(request.Username, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.ErrorMessage.Should().BeNull();
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be(user.Username);
        result.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var request = TestDataFactory.CreateValidLoginRequest();
        var user = TestDataFactory.CreateTestUser();
        user.Username = request.Username;

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request.Username, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("用戶名或密碼錯誤");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnFailure()
    {
        // Arrange
        var request = TestDataFactory.CreateValidLoginRequest();

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request.Username, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("用戶名或密碼錯誤");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldReturnFailure()
    {
        // Arrange
        var request = TestDataFactory.CreateValidLoginRequest();
        var user = TestDataFactory.CreateTestUser();
        user.Username = request.Username;
        user.IsActive = false;

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request.Username, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("帳戶已被停用");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task GetUserProfileAsync_WithValidUserId_ShouldReturnProfile()
    {
        // Arrange
        var userId = 1;
        var user = TestDataFactory.CreateTestUser(userId);
        var expectedBalance = 150.00m;

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _walletRepositoryMock.Setup(x => x.GetBalanceAsync(userId))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _authService.GetUserProfileAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        result.Balance.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task GetUserProfileAsync_WithInvalidUserId_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.GetUserProfileAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByUsernameAsync(request.Username))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("註冊失敗，請稍後再試");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidLoginRequest();

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _authService.LoginAsync(request.Username, request.Password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("登入失敗，請稍後再試");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }
}
