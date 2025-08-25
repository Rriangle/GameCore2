using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 錢包服務測試
/// </summary>
public class WalletServiceTests
{
    private readonly Mock<IUserWalletRepository> _mockUserWalletRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockUserSalesInformationRepository;
    private readonly Mock<ILogger<WalletService>> _mockLogger;
    private readonly WalletService _walletService;

    public WalletServiceTests()
    {
        _mockUserWalletRepository = new Mock<IUserWalletRepository>();
        _mockUserSalesInformationRepository = new Mock<IUserSalesInformationRepository>();
        _mockLogger = new Mock<ILogger<WalletService>>();

        _walletService = new WalletService(
            _mockUserWalletRepository.Object,
            _mockUserSalesInformationRepository.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetUserWalletBalanceAsync_ValidUserId_ReturnsBalance()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 5000;
        var expectedWallet = new UserWallet
        {
            WalletId = 1,
            User_Id = userId,
            User_Point = expectedBalance,
            Coupon_Number = "COUPON123",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedWallet);

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        Assert.Equal(expectedBalance, result);
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserWalletBalanceAsync_UserNotFound_ReturnsZero()
    {
        // Arrange
        var userId = 999;
        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserWallet?)null);

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        Assert.Equal(0, result);
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetSalesWalletBalanceAsync_ValidUserId_ReturnsBalance()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 15000.00m;
        var expectedSalesWallet = new UserSalesInformation
        {
            User_Id = userId,
            UserSales_Wallet = expectedBalance,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };

        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(expectedSalesWallet);

        // Act
        var result = await _walletService.GetSalesWalletBalanceAsync(userId);

        // Assert
        Assert.Equal(expectedBalance, result);
        _mockUserSalesInformationRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetSalesWalletBalanceAsync_UserNotFound_ReturnsZero()
    {
        // Arrange
        var userId = 999;
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserSalesInformation?)null);

        // Act
        var result = await _walletService.GetSalesWalletBalanceAsync(userId);

        // Assert
        Assert.Equal(0m, result);
        _mockUserSalesInformationRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_ValidUserId_ReturnsTransactions()
    {
        // Arrange
        var userId = 1;
        var expectedTransactions = new List<WalletTransactionDto>
        {
            new WalletTransactionDto
            {
                TransactionId = 1,
                UserId = userId,
                TransactionType = "SignIn",
                Amount = 100,
                Description = "每日簽到獎勵",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new WalletTransactionDto
            {
                TransactionId = 2,
                UserId = userId,
                TransactionType = "MiniGame",
                Amount = 50,
                Description = "小遊戲獎勵",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new WalletTransactionDto
            {
                TransactionId = 3,
                UserId = userId,
                TransactionType = "PetRecolor",
                Amount = -200,
                Description = "寵物重新上色",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _mockUserWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTransactions.Count, result.Count);
        Assert.Equal(expectedTransactions[0].TransactionId, result[0].TransactionId);
        Assert.Equal(expectedTransactions[0].Amount, result[0].Amount);
        Assert.Equal(expectedTransactions[0].Description, result[0].Description);
        _mockUserWalletRepository.Verify(x => x.GetTransactionsByUserIdAsync(userId, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetSalesWalletTransactionsAsync_ValidUserId_ReturnsTransactions()
    {
        // Arrange
        var userId = 1;
        var expectedTransactions = new List<SalesWalletTransactionDto>
        {
            new SalesWalletTransactionDto
            {
                TransactionId = 1,
                UserId = userId,
                TransactionType = "Sale",
                Amount = 500.00m,
                Description = "商品銷售收入",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new SalesWalletTransactionDto
            {
                TransactionId = 2,
                UserId = userId,
                TransactionType = "Withdrawal",
                Amount = -1000.00m,
                Description = "提現到銀行帳戶",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new SalesWalletTransactionDto
            {
                TransactionId = 3,
                UserId = userId,
                TransactionType = "Refund",
                Amount = -150.00m,
                Description = "商品退貨退款",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletService.GetSalesWalletTransactionsAsync(userId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTransactions.Count, result.Count);
        Assert.Equal(expectedTransactions[0].TransactionId, result[0].TransactionId);
        Assert.Equal(expectedTransactions[0].Amount, result[0].Amount);
        Assert.Equal(expectedTransactions[0].Description, result[0].Description);
        _mockUserSalesInformationRepository.Verify(x => x.GetTransactionsByUserIdAsync(userId, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        _mockUserWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<WalletTransactionDto>());

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockUserWalletRepository.Verify(x => x.GetTransactionsByUserIdAsync(userId, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetSalesWalletTransactionsAsync_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<SalesWalletTransactionDto>());

        // Act
        var result = await _walletService.GetSalesWalletTransactionsAsync(userId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockUserSalesInformationRepository.Verify(x => x.GetTransactionsByUserIdAsync(userId, 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var userId = 1;
        var page = 2;
        var pageSize = 5;
        var expectedTransactions = new List<WalletTransactionDto>
        {
            new WalletTransactionDto
            {
                TransactionId = 6,
                UserId = userId,
                TransactionType = "Bonus",
                Amount = 75,
                Description = "活動獎勵",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            }
        };

        _mockUserWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedTransactions[0].TransactionId, result[0].TransactionId);
        _mockUserWalletRepository.Verify(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetSalesWalletTransactionsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var userId = 1;
        var page = 2;
        var pageSize = 5;
        var expectedTransactions = new List<SalesWalletTransactionDto>
        {
            new SalesWalletTransactionDto
            {
                TransactionId = 6,
                UserId = userId,
                TransactionType = "Commission",
                Amount = 25.00m,
                Description = "平台佣金",
                CreatedAt = DateTime.UtcNow.AddDays(-6)
            }
        };

        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletService.GetSalesWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedTransactions[0].TransactionId, result[0].TransactionId);
        _mockUserSalesInformationRepository.Verify(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize), Times.Once);
    }
}