using GameCore.Api.Controllers;
using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace GameCore.Tests.Controllers;

/// <summary>
/// 用戶控制器測試
/// </summary>
public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private readonly UserController _userController;
    private readonly Mock<HttpContext> _mockHttpContext;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _userController = new UserController(_mockUserService.Object, _mockLogger.Object);
        _mockHttpContext = new Mock<HttpContext>();

        // 設置 HTTP 上下文
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _mockHttpContext.Setup(x => x.User).Returns(principal);
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };
    }

    [Fact]
    public async Task UpdateIntroduce_ValidRequest_ReturnsOkResult()
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

        _mockUserService.Setup(x => x.UpdateUserIntroduceAsync(userId, request))
            .ReturnsAsync(true);

        // Act
        var result = await _userController.UpdateIntroduce(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Data);
        Assert.Equal("用戶介紹更新成功", response.Message);
        _mockUserService.Verify(x => x.UpdateUserIntroduceAsync(userId, request), Times.Once);
    }

    [Fact]
    public async Task UpdateIntroduce_UpdateFails_ReturnsBadRequest()
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

        _mockUserService.Setup(x => x.UpdateUserIntroduceAsync(userId, request))
            .ReturnsAsync(false);

        // Act
        var result = await _userController.UpdateIntroduce(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Data);
        Assert.Equal("用戶介紹更新失敗", response.Message);
        _mockUserService.Verify(x => x.UpdateUserIntroduceAsync(userId, request), Times.Once);
    }

    [Fact]
    public async Task GetIntroduce_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedUserIntroduce = new UserIntroduceDto
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

        _mockUserService.Setup(x => x.GetUserIntroduceAsync(userId))
            .ReturnsAsync(expectedUserIntroduce);

        // Act
        var result = await _userController.GetIntroduce();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<UserIntroduceDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedUserIntroduce.User_ID, response.Data.User_ID);
        Assert.Equal(expectedUserIntroduce.User_NickName, response.Data.User_NickName);
        Assert.Equal(expectedUserIntroduce.Gender, response.Data.Gender);
        Assert.Equal(expectedUserIntroduce.IdNumber, response.Data.IdNumber);
        Assert.Equal(expectedUserIntroduce.Cellphone, response.Data.Cellphone);
        Assert.Equal(expectedUserIntroduce.Email, response.Data.Email);
        Assert.Equal(expectedUserIntroduce.Address, response.Data.Address);
        Assert.Equal(expectedUserIntroduce.DateOfBirth, response.Data.DateOfBirth);
        Assert.Equal(expectedUserIntroduce.User_Introduce, response.Data.User_Introduce);
        _mockUserService.Verify(x => x.GetUserIntroduceAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetRights_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedUserRights = new UserRightsDto
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        _mockUserService.Setup(x => x.GetUserRightsAsync(userId))
            .ReturnsAsync(expectedUserRights);

        // Act
        var result = await _userController.GetRights();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<UserRightsDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedUserRights.User_Id, response.Data.User_Id);
        Assert.Equal(expectedUserRights.User_Status, response.Data.User_Status);
        Assert.Equal(expectedUserRights.ShoppingPermission, response.Data.ShoppingPermission);
        Assert.Equal(expectedUserRights.MessagePermission, response.Data.MessagePermission);
        Assert.Equal(expectedUserRights.SalesAuthority, response.Data.SalesAuthority);
        _mockUserService.Verify(x => x.GetUserRightsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ApplySalesPermission_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var request = new SalesPermissionRequestDto
        {
            BankCode = 1234,
            BankAccountNumber = "1234567890123456"
        };

        var expectedResponse = new SalesPermissionResponseDto
        {
            User_Id = userId,
            BankCode = request.BankCode,
            BankAccountNumber = request.BankAccountNumber,
            Status = "Pending",
            AppliedAt = DateTime.UtcNow
        };

        _mockUserService.Setup(x => x.ApplySalesPermissionAsync(userId, request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _userController.ApplySalesPermission(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<SalesPermissionResponseDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedResponse.User_Id, response.Data.User_Id);
        Assert.Equal(expectedResponse.BankCode, response.Data.BankCode);
        Assert.Equal(expectedResponse.BankAccountNumber, response.Data.BankAccountNumber);
        Assert.Equal(expectedResponse.Status, response.Data.Status);
        Assert.Equal(expectedResponse.AppliedAt, response.Data.AppliedAt);
        Assert.Equal("銷售權限申請提交成功", response.Message);
        _mockUserService.Verify(x => x.ApplySalesPermissionAsync(userId, request), Times.Once);
    }

    [Fact]
    public async Task GetSalesPermissionStatus_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedResponse = new SalesPermissionResponseDto
        {
            User_Id = userId,
            BankCode = 1234,
            BankAccountNumber = "1234567890123456",
            Status = "Approved",
            AppliedAt = DateTime.UtcNow.AddDays(-7),
            ReviewedAt = DateTime.UtcNow.AddDays(-1),
            ReviewNotes = "資料齊全，審核通過"
        };

        _mockUserService.Setup(x => x.GetSalesPermissionStatusAsync(userId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _userController.GetSalesPermissionStatus();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<SalesPermissionResponseDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedResponse.User_Id, response.Data.User_Id);
        Assert.Equal(expectedResponse.Status, response.Data.Status);
        Assert.Equal(expectedResponse.AppliedAt, response.Data.AppliedAt);
        Assert.Equal(expectedResponse.ReviewedAt, response.Data.ReviewedAt);
        Assert.Equal(expectedResponse.ReviewNotes, response.Data.ReviewNotes);
        _mockUserService.Verify(x => x.GetSalesPermissionStatusAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetSalesWallet_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedSalesWallet = new SalesWalletDto
        {
            User_Id = userId,
            UserSales_Wallet = 15000.00m,
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserService.Setup(x => x.GetSalesWalletAsync(userId))
            .ReturnsAsync(expectedSalesWallet);

        // Act
        var result = await _userController.GetSalesWallet();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<SalesWalletDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedSalesWallet.User_Id, response.Data.User_Id);
        Assert.Equal(expectedSalesWallet.UserSales_Wallet, response.Data.UserSales_Wallet);
        Assert.Equal(expectedSalesWallet.UpdatedAt, response.Data.UpdatedAt);
        _mockUserService.Verify(x => x.GetSalesWalletAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UpdateAvatar_ValidImageData_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var imageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header

        _mockUserService.Setup(x => x.UpdateUserAvatarAsync(userId, imageData))
            .ReturnsAsync(true);

        // Act
        var result = await _userController.UpdateAvatar(imageData);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Data);
        Assert.Equal("用戶頭像更新成功", response.Message);
        _mockUserService.Verify(x => x.UpdateUserAvatarAsync(userId, imageData), Times.Once);
    }

    [Fact]
    public async Task UpdateAvatar_UpdateFails_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var imageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header

        _mockUserService.Setup(x => x.UpdateUserAvatarAsync(userId, imageData))
            .ReturnsAsync(false);

        // Act
        var result = await _userController.UpdateAvatar(imageData);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Data);
        Assert.Equal("用戶頭像更新失敗", response.Message);
        _mockUserService.Verify(x => x.UpdateUserAvatarAsync(userId, imageData), Times.Once);
    }

    [Fact]
    public async Task GetAvatar_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedImageData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header

        _mockUserService.Setup(x => x.GetUserAvatarAsync(userId))
            .ReturnsAsync(expectedImageData);

        // Act
        var result = await _userController.GetAvatar();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<byte[]>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedImageData, response.Data);
        _mockUserService.Verify(x => x.GetUserAvatarAsync(userId), Times.Once);
    }

    [Fact]
    public void GetCurrentUserId_ValidClaims_ReturnsCorrectUserId()
    {
        // Arrange
        var expectedUserId = 1;

        // Act
        var result = _userController.GetCurrentUserId();

        // Assert
        Assert.Equal(expectedUserId, result);
    }

    [Fact]
    public void GetCurrentUserId_InvalidClaims_ReturnsZero()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _mockHttpContext.Setup(x => x.User).Returns(principal);
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };

        // Act
        var result = _userController.GetCurrentUserId();

        // Assert
        Assert.Equal(0, result);
    }
}