using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 用戶服務測試
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserIntroduceRepository> _mockUserIntroduceRepository;
    private readonly Mock<IUserRightsRepository> _mockUserRightsRepository;
    private readonly Mock<IMemberSalesProfileRepository> _mockMemberSalesProfileRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockUserSalesInformationRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
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

    [Fact]
    public async Task UpdateUserIntroduceAsync_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var request = new UpdateUserIntroduceDto
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

        var existingUserIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "舊暱稱",
            Gender = "女",
            IdNumber = "B987654321",
            Cellphone = "0987654321",
            Email = "old@example.com",
            Address = "舊地址",
            DateOfBirth = DateTime.Today.AddYears(-30),
            User_Introduce = "舊介紹"
        };

        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        Assert.True(result);
        _mockUserIntroduceRepository.Verify(x => x.UpdateAsync(It.Is<UserIntroduce>(u => 
            u.User_NickName == request.User_NickName &&
            u.Gender == request.Gender &&
            u.IdNumber == request.IdNumber &&
            u.Cellphone == request.Cellphone &&
            u.Email == request.Email &&
            u.Address == request.Address &&
            u.DateOfBirth == request.DateOfBirth &&
            u.User_Introduce == request.User_Introduce)), Times.Once);
    }

    [Fact]
    public async Task UpdateUserIntroduceAsync_UserIntroduceNotFound_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var request = new UpdateUserIntroduceDto
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

        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserIntroduce?)null);

        // Act
        var result = await _userService.UpdateUserIntroduceAsync(userId, request);

        // Assert
        Assert.False(result);
        _mockUserIntroduceRepository.Verify(x => x.UpdateAsync(It.IsAny<UserIntroduce>()), Times.Never);
    }

    [Fact]
    public async Task GetUserIntroduceAsync_ValidUserId_ReturnsUserIntroduce()
    {
        // Arrange
        var userId = 1;
        var expectedUserIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "測試暱稱",
            Gender = "男",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市中正區測試路123號",
            DateOfBirth = DateTime.Today.AddYears(-25),
            User_Introduce = "這是一個測試用戶介紹"
        };

        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedUserIntroduce);

        // Act
        var result = await _userService.GetUserIntroduceAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUserIntroduce.User_ID, result.User_ID);
        Assert.Equal(expectedUserIntroduce.User_NickName, result.User_NickName);
        Assert.Equal(expectedUserIntroduce.Gender, result.Gender);
        Assert.Equal(expectedUserIntroduce.IdNumber, result.IdNumber);
        Assert.Equal(expectedUserIntroduce.Cellphone, result.Cellphone);
        Assert.Equal(expectedUserIntroduce.Email, result.Email);
        Assert.Equal(expectedUserIntroduce.Address, result.Address);
        Assert.Equal(expectedUserIntroduce.DateOfBirth, result.DateOfBirth);
        Assert.Equal(expectedUserIntroduce.User_Introduce, result.User_Introduce);
    }

    [Fact]
    public async Task GetUserRightsAsync_ValidUserId_ReturnsUserRights()
    {
        // Arrange
        var userId = 1;
        var expectedUserRights = new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserRightsRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedUserRights);

        // Act
        var result = await _userService.GetUserRightsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUserRights.User_Id, result.User_Id);
        Assert.Equal(expectedUserRights.User_Status, result.User_Status);
        Assert.Equal(expectedUserRights.ShoppingPermission, result.ShoppingPermission);
        Assert.Equal(expectedUserRights.MessagePermission, result.MessagePermission);
        Assert.Equal(expectedUserRights.SalesAuthority, result.SalesAuthority);
    }

    [Fact]
    public async Task ApplySalesPermissionAsync_ValidRequest_ReturnsSalesPermissionResponse()
    {
        // Arrange
        var userId = 1;
        var request = new SalesPermissionRequestDto
        {
            BankCode = 1234,
            BankAccountNumber = "1234567890123456"
        };

        var expectedProfile = new MemberSalesProfile
        {
            User_Id = userId,
            BankCode = request.BankCode,
            BankAccountNumber = request.BankAccountNumber,
            Status = "Pending",
            AppliedAt = DateTime.UtcNow
        };

        _mockMemberSalesProfileRepository.Setup(x => x.AddAsync(It.IsAny<MemberSalesProfile>()))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _userService.ApplySalesPermissionAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProfile.User_Id, result.User_Id);
        Assert.Equal(expectedProfile.Status, result.Status);
        Assert.Equal(expectedProfile.AppliedAt, result.AppliedAt);
        _mockMemberSalesProfileRepository.Verify(x => x.AddAsync(It.Is<MemberSalesProfile>(p => 
            p.User_Id == userId &&
            p.BankCode == request.BankCode &&
            p.BankAccountNumber == request.BankAccountNumber &&
            p.Status == "Pending")), Times.Once);
    }

    [Fact]
    public async Task GetSalesPermissionStatusAsync_ValidUserId_ReturnsSalesPermissionResponse()
    {
        // Arrange
        var userId = 1;
        var expectedProfile = new MemberSalesProfile
        {
            User_Id = userId,
            BankCode = 1234,
            BankAccountNumber = "1234567890123456",
            Status = "Approved",
            AppliedAt = DateTime.UtcNow.AddDays(-7),
            ReviewedAt = DateTime.UtcNow.AddDays(-1),
            ReviewNotes = "資料齊全，審核通過"
        };

        _mockMemberSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _userService.GetSalesPermissionStatusAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProfile.User_Id, result.User_Id);
        Assert.Equal(expectedProfile.Status, result.Status);
        Assert.Equal(expectedProfile.AppliedAt, result.AppliedAt);
        Assert.Equal(expectedProfile.ReviewedAt, result.ReviewedAt);
        Assert.Equal(expectedProfile.ReviewNotes, result.ReviewNotes);
    }

    [Fact]
    public async Task GetSalesWalletAsync_ValidUserId_ReturnsSalesWallet()
    {
        // Arrange
        var userId = 1;
        var expectedSalesWallet = new UserSalesInformation
        {
            User_Id = userId,
            UserSales_Wallet = 15000.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedSalesWallet);

        // Act
        var result = await _userService.GetSalesWalletAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSalesWallet.User_Id, result.User_Id);
        Assert.Equal(expectedSalesWallet.UserSales_Wallet, result.UserSales_Wallet);
        Assert.Equal(expectedSalesWallet.UpdatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateUserAvatarAsync_ValidImageData_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var imageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header

        var existingUserIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "測試暱稱",
            Gender = "男",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市中正區測試路123號",
            DateOfBirth = DateTime.Today.AddYears(-25),
            User_Introduce = "這是一個測試用戶介紹"
        };

        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);
        _mockUserIntroduceRepository.Setup(x => x.UpdateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserAvatarAsync(userId, imageData);

        // Assert
        Assert.True(result);
        _mockUserIntroduceRepository.Verify(x => x.UpdateAsync(It.Is<UserIntroduce>(u => 
            u.User_ID == userId &&
            u.User_Picture == imageData)), Times.Once);
    }

    [Fact]
    public async Task GetUserAvatarAsync_ValidUserId_ReturnsImageData()
    {
        // Arrange
        var userId = 1;
        var expectedImageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header

        var existingUserIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = "測試暱稱",
            Gender = "男",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市中正區測試路123號",
            DateOfBirth = DateTime.Today.AddYears(-25),
            User_Introduce = "這是一個測試用戶介紹",
            User_Picture = expectedImageData
        };

        _mockUserIntroduceRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingUserIntroduce);

        // Act
        var result = await _userService.GetUserAvatarAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedImageData, result);
    }
}