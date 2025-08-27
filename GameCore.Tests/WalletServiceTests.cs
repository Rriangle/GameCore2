using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests;

/// <summary>
/// 錢包服務測試類別
/// </summary>
public class WalletServiceTests
{
    private readonly Mock<IUserWalletRepository> _mockWalletRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<WalletService>> _mockLogger;
    private readonly WalletService _walletService;

    /// <summary>
    /// 測試建構函式，設定模擬物件
    /// </summary>
    public WalletServiceTests()
    {
        _mockWalletRepository = new Mock<IUserWalletRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<WalletService>>();
        
        _walletService = new WalletService(
            _mockWalletRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);
    }

    /// <summary>
    /// 測試取得錢包餘額 - 成功情境
    /// </summary>
    [Fact]
    public async Task GetWalletBalanceAsync_ShouldReturnBalance_WhenWalletExists()
    {
        // Arrange
        var userId = 1;
        var mockWallet = new Domain.Entities.UserWallet
        {
            User_Id = userId,
            User_Point = 1000,
            Coupon_Number = "TEST2024"
        };
        
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(mockWallet);

        // Act
        var result = await _walletService.GetWalletBalanceAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.User_Id);
        Assert.Equal(1000, result.Balance);
        Assert.Equal("TEST2024", result.Coupon_Number);
    }

    /// <summary>
    /// 測試取得錢包餘額 - 錢包不存在
    /// </summary>
    [Fact]
    public async Task GetWalletBalanceAsync_ShouldReturnNull_WhenWalletNotExists()
    {
        // Arrange
        var userId = 999;
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((Domain.Entities.UserWallet)null);

        // Act
        var result = await _walletService.GetWalletBalanceAsync(userId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// 測試增加用戶點數 - 成功情境
    /// </summary>
    [Fact]
    public async Task AddPointsAsync_ShouldReturnTrue_WhenValidPoints()
    {
        // Arrange
        var userId = 1;
        var points = 100;
        var source = "signin";
        var description = "每日簽到獎勵";

        _mockWalletRepository.Setup(x => x.AddPointsAsync(userId, points))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.AddPointsAsync(userId, points, source, description);

        // Assert
        Assert.True(result);
        _mockWalletRepository.Verify(x => x.AddPointsAsync(userId, points), Times.Once);
    }

    /// <summary>
    /// 測試增加用戶點數 - 點數為零或負數
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task AddPointsAsync_ShouldReturnFalse_WhenInvalidPoints(int points)
    {
        // Arrange
        var userId = 1;
        var source = "test";
        var description = "測試";

        // Act
        var result = await _walletService.AddPointsAsync(userId, points, source, description);

        // Assert
        Assert.False(result);
        _mockWalletRepository.Verify(x => x.AddPointsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試扣除用戶點數 - 成功情境
    /// </summary>
    [Fact]
    public async Task DeductPointsAsync_ShouldReturnTrue_WhenSufficientBalance()
    {
        // Arrange
        var userId = 1;
        var points = 100;
        var source = "pet_color";
        var description = "寵物換色";

        _mockWalletRepository.Setup(x => x.GetPointsAsync(userId))
            .ReturnsAsync(500); // 足夠的餘額
        _mockWalletRepository.Setup(x => x.DeductPointsAsync(userId, points))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.DeductPointsAsync(userId, points, source, description);

        // Assert
        Assert.True(result);
        _mockWalletRepository.Verify(x => x.DeductPointsAsync(userId, points), Times.Once);
    }

    /// <summary>
    /// 測試扣除用戶點數 - 餘額不足
    /// </summary>
    [Fact]
    public async Task DeductPointsAsync_ShouldReturnFalse_WhenInsufficientBalance()
    {
        // Arrange
        var userId = 1;
        var points = 1000;
        var source = "pet_color";
        var description = "寵物換色";

        _mockWalletRepository.Setup(x => x.GetPointsAsync(userId))
            .ReturnsAsync(500); // 不足的餘額

        // Act
        var result = await _walletService.DeductPointsAsync(userId, points, source, description);

        // Assert
        Assert.False(result);
        _mockWalletRepository.Verify(x => x.DeductPointsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試檢查用戶是否有足夠點數 - 足夠
    /// </summary>
    [Fact]
    public async Task HasSufficientPointsAsync_ShouldReturnTrue_WhenSufficientPoints()
    {
        // Arrange
        var userId = 1;
        var requiredPoints = 100;
        
        _mockWalletRepository.Setup(x => x.GetPointsAsync(userId))
            .ReturnsAsync(500);

        // Act
        var result = await _walletService.HasSufficientPointsAsync(userId, requiredPoints);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 測試檢查用戶是否有足夠點數 - 不足
    /// </summary>
    [Fact]
    public async Task HasSufficientPointsAsync_ShouldReturnFalse_WhenInsufficientPoints()
    {
        // Arrange
        var userId = 1;
        var requiredPoints = 1000;
        
        _mockWalletRepository.Setup(x => x.GetPointsAsync(userId))
            .ReturnsAsync(500);

        // Act
        var result = await _walletService.HasSufficientPointsAsync(userId, requiredPoints);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 測試管理者調整用戶點數 - 增加點數成功
    /// </summary>
    [Fact]
    public async Task AdminAdjustPointsAsync_ShouldReturnTrue_WhenAddingPoints()
    {
        // Arrange
        var adminUserId = 1;
        var request = new AdminAdjustPointsDto
        {
            UserId = 2,
            Delta = 500,
            Reason = "活動獎勵"
        };

        var mockUser = new Domain.Entities.User
        {
            User_ID = request.UserId,
            User_name = "TestUser",
            User_Account = "test@example.com",
            User_Password = "hashedpassword"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(request.UserId))
            .ReturnsAsync(mockUser);
        _mockWalletRepository.Setup(x => x.AddPointsAsync(request.UserId, request.Delta))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.AdminAdjustPointsAsync(adminUserId, request);

        // Assert
        Assert.True(result);
        _mockWalletRepository.Verify(x => x.AddPointsAsync(request.UserId, request.Delta), Times.Once);
    }

    /// <summary>
    /// 測試管理者調整用戶點數 - 扣除點數成功
    /// </summary>
    [Fact]
    public async Task AdminAdjustPointsAsync_ShouldReturnTrue_WhenDeductingPoints()
    {
        // Arrange
        var adminUserId = 1;
        var request = new AdminAdjustPointsDto
        {
            UserId = 2,
            Delta = -200,
            Reason = "違規懲罰"
        };

        var mockUser = new Domain.Entities.User
        {
            User_ID = request.UserId,
            User_name = "TestUser",
            User_Account = "test@example.com",
            User_Password = "hashedpassword"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(request.UserId))
            .ReturnsAsync(mockUser);
        _mockWalletRepository.Setup(x => x.GetPointsAsync(request.UserId))
            .ReturnsAsync(500); // 足夠的餘額
        _mockWalletRepository.Setup(x => x.DeductPointsAsync(request.UserId, 200))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.AdminAdjustPointsAsync(adminUserId, request);

        // Assert
        Assert.True(result);
        _mockWalletRepository.Verify(x => x.DeductPointsAsync(request.UserId, 200), Times.Once);
    }

    /// <summary>
    /// 測試管理者調整用戶點數 - 用戶不存在
    /// </summary>
    [Fact]
    public async Task AdminAdjustPointsAsync_ShouldReturnFalse_WhenUserNotExists()
    {
        // Arrange
        var adminUserId = 1;
        var request = new AdminAdjustPointsDto
        {
            UserId = 999,
            Delta = 100,
            Reason = "測試"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(request.UserId))
            .ReturnsAsync((Domain.Entities.User)null);

        // Act
        var result = await _walletService.AdminAdjustPointsAsync(adminUserId, request);

        // Assert
        Assert.False(result);
        _mockWalletRepository.Verify(x => x.AddPointsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockWalletRepository.Verify(x => x.DeductPointsAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試取得銀行代號清單
    /// </summary>
    [Fact]
    public async Task GetBankCodesAsync_ShouldReturnBankCodes()
    {
        // Act
        var result = await _walletService.GetBankCodesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Code == 822 && x.Name == "中國信託");
        Assert.Contains(result, x => x.Code == 700 && x.Name == "中華郵政");
        Assert.Contains(result, x => x.Code == 808 && x.Name == "玉山銀行");
    }

    /// <summary>
    /// 測試取得點數交易明細 - 分頁查詢
    /// </summary>
    [Fact]
    public async Task GetPointTransactionsAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var userId = 1;
        var query = new PointTransactionQueryDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _walletService.GetPointTransactionsAsync(userId, query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount); // 目前實作返回空結果
        Assert.NotNull(result.Transactions);
    }
}