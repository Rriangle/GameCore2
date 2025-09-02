using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 用戶管理服務測試
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserIntroduceRepository> _mockUserIntroduceRepository;
    private readonly Mock<IUserRightsRepository> _mockUserRightsRepository;
    private readonly Mock<IUserWalletRepository> _mockUserWalletRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserIntroduceRepository = new Mock<IUserIntroduceRepository>();
        _mockUserRightsRepository = new Mock<IUserRightsRepository>();
        _mockUserWalletRepository = new Mock<IUserWalletRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockUserIntroduceRepository.Object,
            _mockUserRightsRepository.Object,
            _mockUserWalletRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetUserInfoAsync_WithValidUserId_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            User_ID = userId,
            User_Name = "測試用戶",
            User_Account = "testuser",
            Created_At = DateTime.UtcNow
        };

        var userIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "測試用戶",
            Email = "test@example.com",
            Cellphone = "0912345678"
        };

        var userRights = new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        var userWallet = new UserWallet
        {
            User_Id = userId,
            User_Point = 100,
            Coupon_Number = null
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userIntroduce);
        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userRights);
        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userWallet);

        // Act
        var result = await _userService.GetUserInfoAsync(userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("取得用戶資訊成功", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.UserID);
        Assert.Equal("測試用戶", result.Data.UserName);
        Assert.Equal("testuser", result.Data.UserAccount);
        Assert.Equal("test@example.com", result.Data.Email);
        Assert.Equal("0912345678", result.Data.Cellphone);
        Assert.True(result.Data.Rights.UserStatus);
        Assert.True(result.Data.Rights.ShoppingPermission);
        Assert.True(result.Data.Rights.MessagePermission);
        Assert.False(result.Data.Rights.SalesAuthority);
        Assert.Equal(100, result.Data.Wallet.UserPoint);
    }

    [Fact]
    public async Task GetUserInfoAsync_WithNonExistentUser_ReturnsFailure()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserInfoAsync(userId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("用戶不存在", result.Message);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var updateDto = new UpdateUserProfileDto
        {
            NickName = "新暱稱",
            Gender = "F",
            Email = "newemail@example.com",
            Cellphone = "0987654321"
        };

        var user = new User
        {
            User_ID = userId,
            User_Name = "測試用戶",
            User_Account = "testuser"
        };

        var userIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "舊暱稱",
            Email = "oldemail@example.com",
            Cellphone = "0912345678"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userIntroduce);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(updateDto.Email)).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.GetByPhoneAsync(updateDto.Cellphone)).ReturnsAsync((User?)null);
        _mockUserIntroduceRepository.Setup(x => x.GetByIdNumberAsync(It.IsAny<string>())).ReturnsAsync((UserIntroduce?)null);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>())).ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserProfileAsync(userId, updateDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("用戶個資更新成功", result.Message);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var updateDto = new UpdateUserProfileDto
        {
            Email = "existing@example.com"
        };

        var user = new User
        {
            User_ID = userId,
            User_Name = "測試用戶",
            User_Account = "testuser"
        };

        var existingUser = new User
        {
            User_ID = 2,
            User_Name = "其他用戶",
            User_Account = "otheruser"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(updateDto.Email)).ReturnsAsync(existingUser);

        // Act
        var result = await _userService.UpdateUserProfileAsync(userId, updateDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("電子郵件已被其他用戶使用", result.Message);
    }

    [Fact]
    public async Task GetUserRightsAsync_WithValidUserId_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var userRights = new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userRights);

        // Act
        var result = await _userService.GetUserRightsAsync(userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("取得用戶權限成功", result.Message);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.UserStatus);
        Assert.True(result.Data.ShoppingPermission);
        Assert.True(result.Data.MessagePermission);
        Assert.False(result.Data.SalesAuthority);
    }

    [Fact]
    public async Task GetUserRightsAsync_WithNonExistentRights_ReturnsFailure()
    {
        // Arrange
        var userId = 999;
        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync((UserRights?)null);

        // Act
        var result = await _userService.GetUserRightsAsync(userId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("用戶權限不存在", result.Message);
    }

    [Fact]
    public async Task GetUserWalletAsync_WithValidUserId_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var userWallet = new UserWallet
        {
            User_Id = userId,
            User_Point = 500,
            Coupon_Number = "COUPON123"
        };

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userWallet);

        // Act
        var result = await _userService.GetUserWalletAsync(userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("取得用戶錢包成功", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(500, result.Data.UserPoint);
        Assert.Equal("COUPON123", result.Data.CouponNumber);
    }

    [Fact]
    public async Task GetUserWalletAsync_WithNonExistentWallet_ReturnsFailure()
    {
        // Arrange
        var userId = 999;
        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync((UserWallet?)null);

        // Act
        var result = await _userService.GetUserWalletAsync(userId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("用戶錢包不存在", result.Message);
    }

    [Fact]
    public async Task CheckUserPermissionAsync_WithValidPermission_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var userRights = new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userRights);

        // Act
        var result = await _userService.CheckUserPermissionAsync(userId, "shopping");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckUserPermissionAsync_WithInvalidPermission_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var userRights = new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userRights);

        // Act
        var result = await _userService.CheckUserPermissionAsync(userId, "sales");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CheckUserPermissionAsync_WithDisabledUser_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var userRights = new UserRights
        {
            User_Id = userId,
            User_Status = false, // 停權
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(userRights);

        // Act
        var result = await _userService.CheckUserPermissionAsync(userId, "shopping");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchUsersAsync_WithValidSearchTerm_ReturnsSuccess()
    {
        // Arrange
        var searchTerm = "測試";
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm, page, pageSize);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("搜尋完成", result.Message);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data); // 目前返回空結果，因為搜尋功能尚未實現
    }

    [Fact]
    public async Task SearchUsersAsync_WithEmptySearchTerm_ReturnsFailure()
    {
        // Arrange
        var searchTerm = "";
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm, page, pageSize);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("搜尋關鍵字不能為空", result.Message);
    }
} 