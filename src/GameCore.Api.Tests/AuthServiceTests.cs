using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GameCore.Api.Tests;

/// <summary>
/// 認證服務測試
/// 測試 Stage 1 的基本認證功能
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserWalletRepository> _mockWalletRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockWalletRepository = new Mock<IUserWalletRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // 設定 JWT 配置
        _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("test-secret-key-for-testing-purposes-only");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("GameCore");
        _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("GameCoreUsers");
        
        _authService = new AuthService(_mockUserRepository.Object, _mockWalletRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "測試使用者",
            Account = "testuser",
            Password = "password123",
            ConfirmPassword = "password123",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市測試區測試路123號",
            DateOfBirth = new DateTime(1990, 1, 1),
            UserIntroduce = "這是一個測試使用者"
        };

        _mockUserRepository.Setup(x => x.AccountExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.UserNameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<GameCore.Domain.Entities.User>())).ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("註冊成功", result.Message);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingAccount_ShouldReturnFailure()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "測試使用者",
            Account = "existinguser",
            Password = "password123",
            ConfirmPassword = "password123",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市測試區測試路123號",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        _mockUserRepository.Setup(x => x.AccountExistsAsync("existinguser")).ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("帳號已存在", result.Message);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var request = new LoginRequest
        {
            AccountOrEmail = "testuser",
            Password = "password123"
        };

        var user = new GameCore.Domain.Entities.User
        {
            User_ID = 1,
            User_name = "測試使用者",
            User_Account = "testuser",
            User_Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=" // SHA256 hash of "password123"
        };

        var userRights = new GameCore.Domain.Entities.UserRights
        {
            User_Id = 1,
            User_Status = true
        };

        _mockUserRepository.Setup(x => x.GetByAccountAsync("testuser")).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((GameCore.Domain.Entities.User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("登入成功", result.Message);
        Assert.NotNull(result.Token);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            AccountOrEmail = "testuser",
            Password = "wrongpassword"
        };

        var user = new GameCore.Domain.Entities.User
        {
            User_ID = 1,
            User_name = "測試使用者",
            User_Account = "testuser",
            User_Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=" // SHA256 hash of "password123"
        };

        _mockUserRepository.Setup(x => x.GetByAccountAsync("testuser")).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((GameCore.Domain.Entities.User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("帳號或密碼錯誤", result.Message);
    }

    [Fact]
    public async Task GetUserProfileAsync_WithValidUserId_ShouldReturnProfile()
    {
        // Arrange
        var userId = 1;
        var user = new GameCore.Domain.Entities.User
        {
            User_ID = 1,
            User_name = "測試使用者",
            User_Account = "testuser"
        };

        var userIntroduce = new GameCore.Domain.Entities.UserIntroduce
        {
            User_ID = 1,
            User_NickName = "測試暱稱",
            Email = "test@example.com",
            Gender = "M",
            Cellphone = "0912345678",
            Address = "台北市測試區測試路123號",
            DateOfBirth = new DateTime(1990, 1, 1),
            User_Introduce = "這是一個測試使用者"
        };

        var userRights = new GameCore.Domain.Entities.UserRights
        {
            User_Id = 1,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        var userWallet = new GameCore.Domain.Entities.UserWallet
        {
            User_Id = 1,
            User_Point = 100
        };

        user.UserIntroduce = userIntroduce;
        user.UserRights = userRights;
        user.UserWallet = userWallet;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _authService.GetUserProfileAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("測試使用者", result.UserName);
        Assert.Equal("testuser", result.UserAccount);
        Assert.Equal("測試暱稱", result.NickName);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("M", result.Gender);
        Assert.Equal("0912345678", result.Cellphone);
        Assert.Equal("台北市測試區測試路123號", result.Address);
        Assert.Equal(new DateTime(1990, 1, 1), result.DateOfBirth);
        Assert.Equal("這是一個測試使用者", result.UserIntroduce);
        Assert.True(result.UserStatus);
        Assert.True(result.ShoppingPermission);
        Assert.True(result.MessagePermission);
        Assert.False(result.SalesAuthority);
        Assert.Equal(100, result.UserPoints);
    }

    [Fact]
    public async Task GetUserProfileAsync_WithInvalidUserId_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((GameCore.Domain.Entities.User?)null);

        // Act
        var result = await _authService.GetUserProfileAsync(userId);

        // Assert
        Assert.Null(result);
    }
} 