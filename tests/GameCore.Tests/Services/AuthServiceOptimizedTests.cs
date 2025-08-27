using FluentAssertions;
using GameCore.Api.DTOs;
using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.Models;
using GameCore.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 優化版認證服務測試 - 涵蓋新增的驗證、安全性和錯誤處理功能
/// </summary>
public class AuthServiceOptimizedTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserWalletRepository> _walletRepositoryMock;
    private readonly Mock<IUserIntroduceRepository> _userIntroduceRepositoryMock;
    private readonly Mock<IUserRightsRepository> _userRightsRepositoryMock;
    private readonly Mock<IMemberSalesProfileRepository> _memberSalesProfileRepositoryMock;
    private readonly Mock<IUserSalesInformationRepository> _userSalesInformationRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly JwtService _jwtService;
    private readonly AuthService _authService;

    public AuthServiceOptimizedTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _walletRepositoryMock = new Mock<IUserWalletRepository>();
        _userIntroduceRepositoryMock = new Mock<IUserIntroduceRepository>();
        _userRightsRepositoryMock = new Mock<IUserRightsRepository>();
        _memberSalesProfileRepositoryMock = new Mock<IMemberSalesProfileRepository>();
        _userSalesInformationRepositoryMock = new Mock<IUserSalesInformationRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // 設定 JWT 配置
        _configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("your-super-secret-key-with-at-least-32-characters");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("GameCore");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("GameCoreUsers");
        _configurationMock.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");

        _jwtService = new JwtService(_configurationMock.Object);
        _authService = new AuthService(
            _userRepositoryMock.Object, 
            _walletRepositoryMock.Object,
            _userIntroduceRepositoryMock.Object,
            _userRightsRepositoryMock.Object,
            _memberSalesProfileRepositoryMock.Object,
            _userSalesInformationRepositoryMock.Object,
            _jwtService, 
            _loggerMock.Object);
    }

    #region 註冊測試 - 輸入驗證

    [Fact]
    public async Task RegisterAsync_WithEmptyUsername_ShouldReturnValidationError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_name = "";

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("使用者姓名不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithEmptyAccount_ShouldReturnValidationError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Account = "";

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("登入帳號不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithEmptyPassword_ShouldReturnValidationError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Password = "";

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("密碼不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithEmptyEmail_ShouldReturnValidationError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.Email = "";

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("電子郵件不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Theory]
    [InlineData("ab")] // 太短
    [InlineData("a".PadRight(51, 'a'))] // 太長
    public async Task RegisterAsync_WithInvalidAccountLength_ShouldReturnValidationError(string account)
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Account = account;

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("登入帳號長度必須在3-50個字符之間");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Theory]
    [InlineData("weak")] // 太短
    [InlineData("weakpassword")] // 缺少大寫、數字、特殊字符
    [InlineData("WEAKPASSWORD")] // 缺少小寫、數字、特殊字符
    [InlineData("WeakPassword")] // 缺少數字、特殊字符
    [InlineData("WeakPassword123")] // 缺少特殊字符
    public async Task RegisterAsync_WithWeakPassword_ShouldReturnValidationError(string password)
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Password = password;

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("密碼必須包含大小寫字母、數字和特殊字符");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test.example.com")]
    [InlineData("test@example")]
    public async Task RegisterAsync_WithInvalidEmail_ShouldReturnValidationError(string email)
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.Email = email;

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("電子郵件格式不正確");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Theory]
    [InlineData("user@name")] // 包含特殊字符
    [InlineData("user name")] // 包含空格
    [InlineData("user-name")] // 包含連字符
    public async Task RegisterAsync_WithInvalidAccountFormat_ShouldReturnValidationError(string account)
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Account = account;

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("登入帳號只能包含字母、數字、下劃線和中文字符");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Theory]
    [InlineData("ValidUser123")]
    [InlineData("user_name")]
    [InlineData("用戶名")]
    [InlineData("User123")]
    [InlineData("測試用戶")]
    public async Task RegisterAsync_WithValidAccountFormat_ShouldPassValidation(string account)
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Account = account;

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(account))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);
        _userRightsRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync((UserRights rights) => rights);
        _userIntroduceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce intro) => intro);
        _userSalesInformationRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("註冊成功");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
    }

    #endregion

    #region 登入測試 - 輸入驗證

    [Fact]
    public async Task LoginAsync_WithEmptyAccount_ShouldReturnValidationError()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "",
            User_Password = "ValidPass123!"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("帳號和密碼不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPassword_ShouldReturnValidationError()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "testuser",
            User_Password = ""
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("帳號和密碼不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithWhitespaceOnly_ShouldReturnValidationError()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "   ",
            User_Password = "   "
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("帳號和密碼不能為空");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    #endregion

    #region 登入測試 - 用戶狀態檢查

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "inactiveuser",
            User_Password = "ValidPass123!"
        };

        var inactiveUser = new User
        {
            User_ID = 1,
            User_Account = "inactiveuser",
            User_Password = BCrypt.Net.BCrypt.HashPassword("ValidPass123!"),
            IsActive = false
        };

        _userRepositoryMock.Setup(x => x.GetByAccountAsync("inactiveuser"))
            .ReturnsAsync(inactiveUser);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("帳號已被停用，請聯繫客服");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    #endregion

    #region 安全性測試

    [Fact]
    public async Task RegisterAsync_WithStrongPassword_ShouldSucceed()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_Password = "StrongPass123!@#";

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(request.User_Account))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);
        _userRightsRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync((UserRights rights) => rights);
        _userIntroduceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce intro) => intro);
        _userSalesInformationRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("註冊成功");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithChineseCharacters_ShouldSucceed()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_name = "張三";
        request.User_Account = "zhangsan123";
        request.User_Password = "StrongPass123!@#";

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(request.User_Account))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);
        _userRightsRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync((UserRights rights) => rights);
        _userIntroduceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce intro) => intro);
        _userSalesInformationRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("註冊成功");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User!.User_name.Should().Be("張三");
    }

    #endregion

    #region 錯誤處理測試

    [Fact]
    public async Task RegisterAsync_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(request.User_Account))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("註冊過程中發生錯誤，請稍後再試");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WhenRepositoryThrowsException_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            User_Account = "testuser",
            User_Password = "ValidPass123!"
        };

        _userRepositoryMock.Setup(x => x.GetByAccountAsync("testuser"))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("登入過程中發生錯誤，請稍後再試");
        result.Token.Should().BeNull();
        result.User.Should().BeNull();
    }

    #endregion

    #region 邊界值測試

    [Fact]
    public async Task RegisterAsync_WithMaximumLengthValues_ShouldSucceed()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_name = new string('A', 100); // 最大長度
        request.User_Account = new string('a', 50); // 最大長度
        request.Email = new string('a', 90) + "@example.com"; // 接近最大長度

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(request.User_Account))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);
        _userRightsRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync((UserRights rights) => rights);
        _userIntroduceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce intro) => intro);
        _userSalesInformationRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("註冊成功");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithMinimumLengthValues_ShouldSucceed()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();
        request.User_name = "張"; // 最小長度
        request.User_Account = "abc"; // 最小長度
        request.User_Password = "ValidPass123!@#"; // 強密碼

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(request.User_Account))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);
        _userRightsRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync((UserRights rights) => rights);
        _userIntroduceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce intro) => intro);
        _userSalesInformationRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("註冊成功");
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var request = TestDataFactory.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByAccountAsync(request.User_Account))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _walletRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync((UserWallet wallet) => wallet);
        _userRightsRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync((UserRights rights) => rights);
        _userIntroduceRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce intro) => intro);
        _userSalesInformationRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act & Assert
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _authService.RegisterAsync(request);
        stopwatch.Stop();

        result.Success.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 應該在1秒內完成
    }

    #endregion
}