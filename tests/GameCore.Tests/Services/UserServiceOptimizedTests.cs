using FluentAssertions;
using GameCore.Api.DTOs;
using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 優化版用戶服務測試 - 涵蓋新增的驗證、安全性和錯誤處理功能
/// </summary>
public class UserServiceOptimizedTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserIntroduceRepository> _mockUserIntroduceRepository;
    private readonly Mock<IUserRightsRepository> _mockUserRightsRepository;
    private readonly Mock<IMemberSalesProfileRepository> _mockMemberSalesProfileRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockUserSalesInformationRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceOptimizedTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserIntroduceRepository = new Mock<IUserIntroduceRepository>();
        _mockUserRightsRepository = new Mock<IUserRightsRepository>();
        _mockMemberSalesProfileRepository = new Mock<IMemberSalesProfileRepository>();
        _mockUserSalesInformationRepository = new Mock<IUserSalesInformationRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockUserIntroduceRepository.Object,
            _mockUserRightsRepository.Object,
            _mockMemberSalesProfileRepository.Object,
            _mockUserSalesInformationRepository.Object,
            _mockLogger.Object
        );
    }

    #region 用戶介紹更新測試 - 輸入驗證

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithEmptyNickname_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.User_NickName = "";

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithEmptyGender_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Gender = "";

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithEmptyIdNumber_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.IdNumber = "";

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithEmptyCellphone_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Cellphone = "";

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithEmptyEmail_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Email = "";

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithEmptyAddress_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Address = "";

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("a".PadRight(51, 'a'))] // 超過最大長度
    public async Task UpdateUserIntroduceAsync_WithNicknameTooLong_ShouldReturnFalse(string nickname)
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.User_NickName = nickname;

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("invalid-gender")]
    [InlineData("男")]
    [InlineData("女")]
    [InlineData("其他")]
    [InlineData("未指定")]
    public async Task UpdateUserIntroduceAsync_WithValidGender_ShouldPassValidation(string gender)
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Gender = gender;

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        if (gender == "男" || gender == "女" || gender == "其他" || gender == "未指定")
        {
            result.Should().BeTrue();
        }
        else
        {
            result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData("A123456789")] // 有效格式
    [InlineData("B987654321")] // 有效格式
    [InlineData("invalid")] // 無效格式
    [InlineData("123456789")] // 無效格式
    public async Task UpdateUserIntroduceAsync_WithIdNumber_ShouldValidateFormat(string idNumber)
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.IdNumber = idNumber;

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        if (idNumber.StartsWith("A") || idNumber.StartsWith("B"))
        {
            result.Should().BeTrue();
        }
        else
        {
            result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData("0912345678")] // 有效格式
    [InlineData("+886912345678")] // 有效格式
    [InlineData("886912345678")] // 有效格式
    [InlineData("invalid")] // 無效格式
    [InlineData("123456789")] // 無效格式
    public async Task UpdateUserIntroduceAsync_WithCellphone_ShouldValidateFormat(string cellphone)
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Cellphone = cellphone;

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        if (cellphone.StartsWith("09") || cellphone.StartsWith("+886") || cellphone.StartsWith("886"))
        {
            result.Should().BeTrue();
        }
        else
        {
            result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData("test@example.com")] // 有效格式
    [InlineData("user.name@domain.co.uk")] // 有效格式
    [InlineData("invalid-email")] // 無效格式
    [InlineData("test@")] // 無效格式
    [InlineData("@example.com")] // 無效格式
    public async Task UpdateUserIntroduceAsync_WithEmail_ShouldValidateFormat(string email)
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.Email = email;

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        if (email.Contains("@") && email.Contains(".") && !email.StartsWith("@") && !email.EndsWith("@"))
        {
            result.Should().BeTrue();
        }
        else
        {
            result.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(12)] // 太小
    [InlineData(121)] // 太大
    [InlineData(18)] // 有效
    [InlineData(25)] // 有效
    [InlineData(65)] // 有效
    public async Task UpdateUserIntroduceAsync_WithDateOfBirth_ShouldValidateAge(int age)
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.DateOfBirth = DateTime.Today.AddYears(-age);

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        if (age >= 13 && age <= 120)
        {
            result.Should().BeTrue();
        }
        else
        {
            result.Should().BeFalse();
        }
    }

    #endregion

    #region 用戶權限更新測試

    [Fact]
    public async Task UpdateUserRightsAsync_WithValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        var request = new UpdateUserRightsDto
        {
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        var existingRights = CreateTestUserRights(userId);

        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingRights);
        _mockUserRightsRepository.Setup(x => x.UpdateAsync(It.IsAny<UserRights>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserRightsAsync(userId, request);

        // Assert
        result.Should().BeTrue();
        _mockUserRightsRepository.Verify(x => x.UpdateAsync(It.IsAny<UserRights>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserRightsAsync_WithNullValues_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = new UpdateUserRightsDto
        {
            User_Status = null,
            ShoppingPermission = null,
            MessagePermission = null,
            SalesAuthority = null
        };

        // Act
        var result = await _userService.UpdateUserRightsAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region 用戶完整資料獲取測試

    [Fact]
    public async Task GetUserProfileAsync_WithValidUserId_ShouldReturnProfile()
    {
        // Arrange
        var userId = 1;
        var user = CreateTestUser(userId);
        var userIntroduce = CreateTestUserIntroduce(userId);
        var userRights = CreateTestUserRights(userId);
        var userSalesInfo = CreateTestUserSalesInformation(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userIntroduce);
        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userRights);
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userSalesInfo);

        // Act
        var result = await _userService.GetUserProfileAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.User_ID.Should().Be(userId);
        result.User_name.Should().Be(user.User_name);
        result.UserIntroduce.Should().NotBeNull();
        result.UserRights.Should().NotBeNull();
        result.UserSalesInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserProfileAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserProfileAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region 錯誤處理測試

    [Fact]
    public async Task UpdateUserIntroduceAsync_WhenUserRepositoryThrowsException_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenRepositoryThrowsException_ShouldReturnNull()
    {
        // Arrange
        var userId = 1;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _userService.GetUserProfileAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region 邊界值測試

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithMaximumLengthValues_ShouldSucceed()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.User_NickName = new string('A', 50); // 最大長度
        request.Address = new string('A', 200); // 最大長度
        request.User_Introduce = new string('A', 500); // 最大長度

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_WithMinimumAge_ShouldSucceed()
    {
        // Arrange
        var userId = 1;
        var request = CreateValidUpdateUserIntroduceRequest();
        request.DateOfBirth = DateTime.Today.AddYears(-13); // 最小年齡

        var user = CreateTestUser(userId);
        var existingUserIntroduce = CreateTestUserIntroduce(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetUserProfileAsync_WithValidUserId_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var userId = 1;
        var user = CreateTestUser(userId);
        var userIntroduce = CreateTestUserIntroduce(userId);
        var userRights = CreateTestUserRights(userId);
        var userSalesInfo = CreateTestUserSalesInformation(userId);

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userIntroduce);
        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userRights);
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userSalesInfo);

        // Act & Assert
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _userService.GetUserProfileAsync(userId);
        stopwatch.Stop();

        result.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 應該在1秒內完成
    }

    #endregion

    #region 輔助方法

    private UpdateUserIntroduceDto CreateValidUpdateUserIntroduceRequest()
    {
        return new UpdateUserIntroduceDto
        {
            User_NickName = "測試暱稱",
            Gender = "男",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市中正區測試路123號",
            DateOfBirth = DateTime.Today.AddYears(-25),
            User_Introduce = "這是一個測試用戶介紹"
        };
    }

    private User CreateTestUser(int userId)
    {
        return new User
        {
            User_ID = userId,
            User_name = "測試用戶",
            User_Account = "testuser",
            User_Password = "hashedpassword",
            Email = "test@example.com",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1),
            IsActive = true,
            IsEmailVerified = false
        };
    }

    private UserIntroduce CreateTestUserIntroduce(int userId)
    {
        return new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "舊暱稱",
            Gender = "女",
            IdNumber = "B987654321",
            Cellphone = "0987654321",
            Email = "old@example.com",
            Address = "舊地址",
            DateOfBirth = DateTime.Today.AddYears(-30),
            User_Picture = null,
            User_Introduce = "舊介紹",
            Create_Account = DateTime.UtcNow.AddDays(-10)
        };
    }

    private UserRights CreateTestUserRights(int userId)
    {
        return new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        };
    }

    private UserSalesInformation CreateTestUserSalesInformation(int userId)
    {
        return new UserSalesInformation
        {
            User_Id = userId,
            UserSales_Wallet = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        };
    }

    #endregion
}