using GameCore.Api.Controllers;
using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace GameCore.Tests.Controllers;

/// <summary>
/// 假資料生成控制器測試
/// </summary>
public class FakeDataControllerTests
{
    private readonly Mock<IFakeDataService> _mockFakeDataService;
    private readonly Mock<ILogger<FakeDataController>> _mockLogger;
    private readonly FakeDataController _fakeDataController;
    private readonly Mock<HttpContext> _mockHttpContext;

    public FakeDataControllerTests()
    {
        _mockFakeDataService = new Mock<IFakeDataService>();
        _mockLogger = new Mock<ILogger<FakeDataController>>();
        _fakeDataController = new FakeDataController(_mockFakeDataService.Object, _mockLogger.Object);
        _mockHttpContext = new Mock<HttpContext>();

        // 設置 HTTP 上下文 - 模擬管理員用戶
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Email, "admin@example.com"),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _mockHttpContext.Setup(x => x.User).Returns(principal);
        _fakeDataController.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };
    }

    [Fact]
    public async Task GenerateFakeUsers_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var count = 100;
        var expectedGeneratedCount = 100;

        _mockFakeDataService.Setup(x => x.GenerateFakeUsersAsync(count))
            .ReturnsAsync(expectedGeneratedCount);

        // Act
        var result = await _fakeDataController.GenerateFakeUsers(count);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(okResult.Value);
        Assert.Equal(expectedGeneratedCount, response.Data);
        Assert.Equal($"成功生成 {expectedGeneratedCount} 筆假資料", response.Message);
        _mockFakeDataService.Verify(x => x.GenerateFakeUsersAsync(count), Times.Once);
    }

    [Fact]
    public async Task GenerateFakeUsers_LargeCount_ReturnsOkResult()
    {
        // Arrange
        var count = 1000;
        var expectedGeneratedCount = 1000;

        _mockFakeDataService.Setup(x => x.GenerateFakeUsersAsync(count))
            .ReturnsAsync(expectedGeneratedCount);

        // Act
        var result = await _fakeDataController.GenerateFakeUsers(count);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(okResult.Value);
        Assert.Equal(expectedGeneratedCount, response.Data);
        Assert.Equal($"成功生成 {expectedGeneratedCount} 筆假資料", response.Message);
        _mockFakeDataService.Verify(x => x.GenerateFakeUsersAsync(count), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GenerateFakeUsers_InvalidCount_ReturnsBadRequest(int invalidCount)
    {
        // Act
        var result = await _fakeDataController.GenerateFakeUsers(invalidCount);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(badRequestResult.Value);
        Assert.Equal(0, response.Data);
        Assert.Equal("生成數量必須大於 0", response.Message);
        _mockFakeDataService.Verify(x => x.GenerateFakeUsersAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GenerateFakeUsers_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var count = 100;
        var exceptionMessage = "資料庫連接失敗";

        _mockFakeDataService.Setup(x => x.GenerateFakeUsersAsync(count))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fakeDataController.GenerateFakeUsers(count);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ApiResponse<int>>(statusCodeResult.Value);
        Assert.Equal(0, response.Data);
        Assert.Contains("生成假資料時發生錯誤", response.Message);
        _mockFakeDataService.Verify(x => x.GenerateFakeUsersAsync(count), Times.Once);
    }

    [Fact]
    public async Task GetFakeDataStats_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var expectedStats = new FakeDataStatsDto
        {
            TotalUsers = 1000,
            TotalUserIntroduces = 1000,
            TotalUserRights = 1000,
            TotalUserWallets = 1000,
            TotalMemberSalesProfiles = 333,
            TotalUserSalesInformations = 333
        };

        _mockFakeDataService.Setup(x => x.GetFakeDataStatsAsync())
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _fakeDataController.GetFakeDataStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<FakeDataStatsDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedStats.TotalUsers, response.Data.TotalUsers);
        Assert.Equal(expectedStats.TotalUserIntroduces, response.Data.TotalUserIntroduces);
        Assert.Equal(expectedStats.TotalUserRights, response.Data.TotalUserRights);
        Assert.Equal(expectedStats.TotalUserWallets, response.Data.TotalUserWallets);
        Assert.Equal(expectedStats.TotalMemberSalesProfiles, response.Data.TotalMemberSalesProfiles);
        Assert.Equal(expectedStats.TotalUserSalesInformations, response.Data.TotalUserSalesInformations);
        Assert.Equal("獲取假資料統計成功", response.Message);
        _mockFakeDataService.Verify(x => x.GetFakeDataStatsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetFakeDataStats_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "資料庫查詢失敗";

        _mockFakeDataService.Setup(x => x.GetFakeDataStatsAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fakeDataController.GetFakeDataStats();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ApiResponse<FakeDataStatsDto>>(statusCodeResult.Value);
        Assert.Null(response.Data);
        Assert.Contains("獲取假資料統計時發生錯誤", response.Message);
        _mockFakeDataService.Verify(x => x.GetFakeDataStatsAsync(), Times.Once);
    }

    [Fact]
    public async Task CleanupFakeData_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var expectedDeletedCount = 600; // 6 個表各 100 筆

        _mockFakeDataService.Setup(x => x.CleanupFakeDataAsync())
            .ReturnsAsync(expectedDeletedCount);

        // Act
        var result = await _fakeDataController.CleanupFakeData();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(okResult.Value);
        Assert.Equal(expectedDeletedCount, response.Data);
        Assert.Equal($"成功清理 {expectedDeletedCount} 筆假資料", response.Message);
        _mockFakeDataService.Verify(x => x.CleanupFakeDataAsync(), Times.Once);
    }

    [Fact]
    public async Task CleanupFakeData_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var exceptionMessage = "資料庫刪除失敗";

        _mockFakeDataService.Setup(x => x.CleanupFakeDataAsync())
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _fakeDataController.CleanupFakeData();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ApiResponse<int>>(statusCodeResult.Value);
        Assert.Equal(0, response.Data);
        Assert.Contains("清理假資料時發生錯誤", response.Message);
        _mockFakeDataService.Verify(x => x.CleanupFakeDataAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateFakeUsers_ZeroCount_ReturnsBadRequest()
    {
        // Act
        var result = await _fakeDataController.GenerateFakeUsers(0);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(badRequestResult.Value);
        Assert.Equal(0, response.Data);
        Assert.Equal("生成數量必須大於 0", response.Message);
    }

    [Fact]
    public async Task GenerateFakeUsers_NegativeCount_ReturnsBadRequest()
    {
        // Act
        var result = await _fakeDataController.GenerateFakeUsers(-50);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(badRequestResult.Value);
        Assert.Equal(0, response.Data);
        Assert.Equal("生成數量必須大於 0", response.Message);
    }

    [Fact]
    public async Task GenerateFakeUsers_ValidCount_LogsInformation()
    {
        // Arrange
        var count = 100;
        var expectedGeneratedCount = 100;

        _mockFakeDataService.Setup(x => x.GenerateFakeUsersAsync(count))
            .ReturnsAsync(expectedGeneratedCount);

        // Act
        var result = await _fakeDataController.GenerateFakeUsers(count);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mockFakeDataService.Verify(x => x.GenerateFakeUsersAsync(count), Times.Once);
    }

    [Fact]
    public async Task GetFakeDataStats_EmptyDatabase_ReturnsZeroCounts()
    {
        // Arrange
        var expectedStats = new FakeDataStatsDto
        {
            TotalUsers = 0,
            TotalUserIntroduces = 0,
            TotalUserRights = 0,
            TotalUserWallets = 0,
            TotalMemberSalesProfiles = 0,
            TotalUserSalesInformations = 0
        };

        _mockFakeDataService.Setup(x => x.GetFakeDataStatsAsync())
            .ReturnsAsync(expectedStats);

        // Act
        var result = await _fakeDataController.GetFakeDataStats();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<FakeDataStatsDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(0, response.Data.TotalUsers);
        Assert.Equal(0, response.Data.TotalUserIntroduces);
        Assert.Equal(0, response.Data.TotalUserRights);
        Assert.Equal(0, response.Data.TotalUserWallets);
        Assert.Equal(0, response.Data.TotalMemberSalesProfiles);
        Assert.Equal(0, response.Data.TotalUserSalesInformations);
    }

    [Fact]
    public async Task CleanupFakeData_EmptyDatabase_ReturnsZeroDeleted()
    {
        // Arrange
        var expectedDeletedCount = 0;

        _mockFakeDataService.Setup(x => x.CleanupFakeDataAsync())
            .ReturnsAsync(expectedDeletedCount);

        // Act
        var result = await _fakeDataController.CleanupFakeData();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(okResult.Value);
        Assert.Equal(expectedDeletedCount, response.Data);
        Assert.Equal($"成功清理 {expectedDeletedCount} 筆假資料", response.Message);
    }
}