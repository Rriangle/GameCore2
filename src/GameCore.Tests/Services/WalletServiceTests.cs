using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 錢包服務測試
/// </summary>
public class WalletServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserWalletRepository> _mockWalletRepository;
    private readonly WalletService _walletService;

    public WalletServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockWalletRepository = new Mock<IUserWalletRepository>();
        _walletService = new WalletService(_mockUserRepository.Object, _mockWalletRepository.Object);
    }

    [Fact]
    public async Task GetWalletAsync_WithValidUserId_ReturnsWalletDto()
    {
        // Arrange
        var userId = 1;
        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 1000,
            User_Balance = 500.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);

        // Act
        var result = await _walletService.GetWalletAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(1000, result.Points);
        Assert.Equal(500.00m, result.Balance);
    }

    [Fact]
    public async Task GetWalletAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserWallet?)null);

        // Act
        var result = await _walletService.GetWalletAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddPointsAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var points = 100;
        var reason = "測試增加點數";
        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 1000,
            User_Balance = 500.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);
        _mockWalletRepository.Setup(x => x.UpdateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync(true);
        _mockWalletRepository.Setup(x => x.AddTransactionAsync(It.IsAny<UserWalletTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.AddPointsAsync(userId, points, reason);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("點數必須大於0", result.Message);
    }

    [Fact]
    public async Task AddPointsAsync_WithZeroPoints_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var points = 0;
        var reason = "測試增加點數";

        // Act
        var result = await _walletService.AddPointsAsync(userId, points, reason);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("點數必須大於0", result.Message);
    }

    [Fact]
    public async Task AddPointsAsync_WithNegativePoints_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var points = -100;
        var reason = "測試增加點數";

        // Act
        var result = await _walletService.AddPointsAsync(userId, points, reason);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("點數必須大於0", result.Message);
    }

    [Fact]
    public async Task DeductPointsAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var points = 100;
        var reason = "測試扣除點數";
        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 1000,
            User_Balance = 500.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);
        _mockWalletRepository.Setup(x => x.UpdateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync(true);
        _mockWalletRepository.Setup(x => x.AddTransactionAsync(It.IsAny<UserWalletTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.DeductPointsAsync(userId, points, reason);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("點數必須大於0", result.Message);
    }

    [Fact]
    public async Task DeductPointsAsync_WithInsufficientPoints_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var points = 1500;
        var reason = "測試扣除點數";
        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 1000,
            User_Balance = 500.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);

        // Act
        var result = await _walletService.DeductPointsAsync(userId, points, reason);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("點數不足", result.Message);
    }

    [Fact]
    public async Task TransferPointsAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var fromUserId = 1;
        var toUserId = 2;
        var points = 100;
        var reason = "測試轉帳";

        var fromWallet = new UserWallet
        {
            WalletID = 1,
            User_Id = fromUserId,
            User_Point = 1000,
            User_Balance = 500.00m,
            Last_Updated = DateTime.UtcNow
        };

        var toWallet = new UserWallet
        {
            WalletID = 2,
            User_Id = toUserId,
            User_Point = 500,
            User_Balance = 200.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(fromUserId))
            .ReturnsAsync(fromWallet);
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(toUserId))
            .ReturnsAsync(toWallet);
        _mockWalletRepository.Setup(x => x.UpdateAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync(true);
        _mockWalletRepository.Setup(x => x.AddTransactionAsync(It.IsAny<UserWalletTransaction>()))
            .ReturnsAsync(true);

        // Act
        var result = await _walletService.TransferPointsAsync(fromUserId, toUserId, points, reason);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("轉帳點數必須大於0", result.Message);
    }

    [Fact]
    public async Task TransferPointsAsync_WithSameUserId_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var points = 100;
        var reason = "測試轉帳";

        // Act
        var result = await _walletService.TransferPointsAsync(userId, userId, points, reason);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("不能轉帳給自己", result.Message);
    }

    [Fact]
    public async Task GetTransactionHistoryAsync_WithValidData_ReturnsPagedResult()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;
        var transactions = new List<UserWalletTransaction>
        {
            new UserWalletTransaction
            {
                TransactionID = 1,
                User_Id = userId,
                Amount = 100,
                Type = "add",
                Reason = "測試交易",
                Created_At = DateTime.UtcNow
            }
        };

        _mockWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(transactions);
        _mockWalletRepository.Setup(x => x.GetTransactionCountByUserIdAsync(userId))
            .ReturnsAsync(1);

        // Act
        var result = await _walletService.GetTransactionHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Single(result.Data);
    }
} 