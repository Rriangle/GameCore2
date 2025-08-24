using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests;

/// <summary>
/// AuthService 單元測試
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserWalletRepository> _mockWalletRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockWalletRepository = new Mock<IUserWalletRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        
        _authService = new AuthService(
            _mockUserRepository.Object,
            _mockWalletRepository.Object,
            _mockJwtService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            User_name = "測試用戶",
            User_Account = "test@example.com",
            User_Password = "password123",
            User_NickName = "TestUser",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市",
            DateOfBirth = DateTime.UtcNow.AddYears(-25)
        };

        var createdUser = new User
        {
            User_ID = 1,
            User_name = request.User_name,
            User_Account = request.User_Account,
            User_Password = "hashed_password"
        };

        var wallet = new UserWallet
        {
            User_Id = 1,
            User_Point = 0
        };

        // Setup mocks
        _mockUserRepository.Setup(x => x.IsUsernameUniqueAsync(request.User_name, null))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(x => x.IsAccountUniqueAsync(request.User_Account, null))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);
        _mockWalletRepository.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync(wallet);
        _mockJwtService.Setup(x => x.GenerateAccessToken(createdUser.User_ID, createdUser.User_Account, null))
            .Returns("mock_access_token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("mock_refresh_token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("註冊成功", result.Message);
        Assert.NotNull(result.Response);
        Assert.Equal(createdUser.User_ID, result.Response.User_ID);
        Assert.Equal(createdUser.User_name, result.Response.User_name);
        Assert.Equal(createdUser.User_Account, result.Response.User_Account);
        Assert.Equal("mock_access_token", result.Response.AccessToken);
        Assert.Equal("mock_refresh_token", result.Response.RefreshToken);
        
        // Verify repository calls
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _mockWalletRepository.Verify(x => x.CreateAsync(It.IsAny<UserWallet>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUsername_ShouldReturnFailure()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            User_name = "重複用戶",
            User_Account = "test@example.com",
            User_Password = "password123"
        };

        _mockUserRepository.Setup(x => x.IsUsernameUniqueAsync(request.User_name, null))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("用戶姓名已存在", result.Message);
        Assert.Null(result.Response);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "test@example.com",
            User_Password = "password123"
        };

        var user = new User
        {
            User_ID = 1,
            User_name = "測試用戶",
            User_Account = request.User_Account,
            User_Password = "hashed_password"
        };

        _mockUserRepository.Setup(x => x.ValidateCredentialsAsync(request.User_Account, It.IsAny<string>()))
            .ReturnsAsync(user);
        _mockJwtService.Setup(x => x.GenerateAccessToken(user.User_ID, user.User_Account, null))
            .Returns("mock_access_token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("mock_refresh_token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("登入成功", result.Message);
        Assert.NotNull(result.Response);
        Assert.Equal(user.User_ID, result.Response.User_ID);
        Assert.Equal("mock_access_token", result.Response.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "test@example.com",
            User_Password = "wrong_password"
        };

        _mockUserRepository.Setup(x => x.ValidateCredentialsAsync(request.User_Account, It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("帳號或密碼錯誤", result.Message);
        Assert.Null(result.Response);
    }

    [Fact]
    public void HashPassword_ShouldReturnConsistentHash()
    {
        // Arrange
        var password = "test_password";

        // Act
        var hash1 = _authService.HashPassword(password);
        var hash2 = _authService.HashPassword(password);

        // Assert
        Assert.Equal(hash1, hash2);
        Assert.NotEqual(password, hash1); // 確保不是明文
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "test_password";
        var hash = _authService.HashPassword(password);

        // Act
        var result = _authService.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "test_password";
        var wrongPassword = "wrong_password";
        var hash = _authService.HashPassword(password);

        // Act
        var result = _authService.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }
}