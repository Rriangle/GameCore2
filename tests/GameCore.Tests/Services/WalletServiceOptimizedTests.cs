using FluentAssertions;
using GameCore.Api.DTOs;
using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 優化版錢包服務測試 - 涵蓋新增的快取、驗證、分頁和性能功能
/// </summary>
public class WalletServiceOptimizedTests
{
    private readonly Mock<IUserWalletRepository> _mockUserWalletRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockUserSalesInformationRepository;
    private readonly Mock<IMemberSalesProfileRepository> _mockMemberSalesProfileRepository;
    private readonly Mock<ILogger<WalletService>> _mockLogger;
    private readonly IMemoryCache _memoryCache;
    private readonly WalletService _walletService;

    public WalletServiceOptimizedTests()
    {
        _mockUserWalletRepository = new Mock<IUserWalletRepository>();
        _mockUserSalesInformationRepository = new Mock<IUserSalesInformationRepository>();
        _mockMemberSalesProfileRepository = new Mock<IMemberSalesProfileRepository>();
        _mockLogger = new Mock<ILogger<WalletService>>();

        // 使用真實的記憶體快取進行測試
        var cacheOptions = new MemoryCacheOptions();
        _memoryCache = new MemoryCache(cacheOptions);

        _walletService = new WalletService(
            _mockUserWalletRepository.Object,
            _mockUserSalesInformationRepository.Object,
            _mockMemberSalesProfileRepository.Object,
            _mockLogger.Object,
            _memoryCache
        );
    }

    #region 用戶錢包餘額測試

    [Fact]
    public async Task GetUserWalletBalanceAsync_WithValidUserId_ShouldReturnBalance()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 1000;
        var userWallet = CreateTestUserWallet(userId, expectedBalance);

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userWallet);

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(expectedBalance);
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserWalletBalanceAsync_WithInvalidUserId_ShouldReturnZero()
    {
        // Arrange
        var userId = 0;

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0);
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetUserWalletBalanceAsync_WithNegativeUserId_ShouldReturnZero()
    {
        // Arrange
        var userId = -1;

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0);
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetUserWalletBalanceAsync_WithNonExistentWallet_ShouldReturnZero()
    {
        // Arrange
        var userId = 999;

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserWallet?)null);

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0);
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserWalletBalanceAsync_ShouldUseCache_OnSecondCall()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 1000;
        var userWallet = CreateTestUserWallet(userId, expectedBalance);

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userWallet);

        // Act - First call (should hit database)
        var result1 = await _walletService.GetUserWalletBalanceAsync(userId);
        
        // Act - Second call (should hit cache)
        var result2 = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        result1.Should().Be(expectedBalance);
        result2.Should().Be(expectedBalance);
        
        // Should only hit database once due to caching
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    #endregion

    #region 銷售錢包餘額測試

    [Fact]
    public async Task GetSalesWalletBalanceAsync_WithValidUserId_ShouldReturnBalance()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 500.50m;
        var salesInfo = CreateTestUserSalesInformation(userId, expectedBalance);

        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);

        // Act
        var result = await _walletService.GetSalesWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(expectedBalance);
        _mockUserSalesInformationRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetSalesWalletBalanceAsync_WithInvalidUserId_ShouldReturnZero()
    {
        // Arrange
        var userId = 0;

        // Act
        var result = await _walletService.GetSalesWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0m);
        _mockUserSalesInformationRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetSalesWalletBalanceAsync_WithNonExistentSalesInfo_ShouldReturnZero()
    {
        // Arrange
        var userId = 999;

        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserSalesInformation?)null);

        // Act
        var result = await _walletService.GetSalesWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0m);
        _mockUserSalesInformationRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    #endregion

    #region 錢包摘要測試

    [Fact]
    public async Task GetWalletSummaryAsync_WithValidUserId_ShouldReturnCompleteSummary()
    {
        // Arrange
        var userId = 1;
        var userWallet = CreateTestUserWallet(userId, 1000);
        var salesInfo = CreateTestUserSalesInformation(userId, 500.50m);
        var salesProfile = CreateTestMemberSalesProfile(userId);

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userWallet);
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);
        _mockMemberSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesProfile);

        // Act
        var result = await _walletService.GetWalletSummaryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.UserWalletBalance.Should().Be(1000);
        result.SalesWalletBalance.Should().Be(500.50m);
        result.SalesProfileStatus.Should().Be("已開通");
        result.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetWalletSummaryAsync_WithInvalidUserId_ShouldReturnEmptySummary()
    {
        // Arrange
        var userId = 0;

        // Act
        var result = await _walletService.GetWalletSummaryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.UserWalletBalance.Should().Be(0);
        result.SalesWalletBalance.Should().Be(0m);
        result.SalesProfileStatus.Should().Be("未知");
    }

    [Fact]
    public async Task GetWalletSummaryAsync_WithNonExistentSalesProfile_ShouldReturnNotOpenedStatus()
    {
        // Arrange
        var userId = 1;
        var userWallet = CreateTestUserWallet(userId, 1000);
        var salesInfo = CreateTestUserSalesInformation(userId, 500.50m);

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userWallet);
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);
        _mockMemberSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((MemberSalesProfile?)null);

        // Act
        var result = await _walletService.GetWalletSummaryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.SalesProfileStatus.Should().Be("未開通");
    }

    #endregion

    #region 分頁交易記錄測試

    [Fact]
    public async Task GetUserWalletTransactionsAsync_WithValidParameters_ShouldReturnPaginatedResults()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
        var transactions = CreateTestWalletTransactions(userId, 15);
        var totalCount = 15;

        _mockUserWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(transactions.Take(pageSize).ToList());
        _mockUserWalletRepository.Setup(x => x.GetTransactionCountByUserIdAsync(userId))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
        result.Transactions.Should().HaveCount(pageSize);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_WithInvalidPage_ShouldReturnEmptyResults()
    {
        // Arrange
        var userId = 1;
        var page = 0;
        var pageSize = 10;

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Transactions.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_WithInvalidPageSize_ShouldReturnEmptyResults()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Transactions.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_WithPageSizeExceedingMax_ShouldReturnEmptyResults()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 101; // Exceeds MaxPageSize (100)

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Transactions.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public async Task GetUserWalletTransactionsAsync_WithNoTransactions_ShouldReturnEmptyResults()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        _mockUserWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(new List<dynamic>());
        _mockUserWalletRepository.Setup(x => x.GetTransactionCountByUserIdAsync(userId))
            .ReturnsAsync(0);

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Transactions.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }

    #endregion

    #region 銷售錢包交易記錄測試

    [Fact]
    public async Task GetSalesWalletTransactionsAsync_WithValidParameters_ShouldReturnPaginatedResults()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
        var transactions = CreateTestSalesWalletTransactions(userId, 15);
        var totalCount = 15;

        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(transactions.Take(pageSize).ToList());
        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionCountByUserIdAsync(userId))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _walletService.GetSalesWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
        result.Transactions.Should().HaveCount(pageSize);
    }

    [Fact]
    public async Task GetSalesWalletTransactionsAsync_WithNoTransactions_ShouldReturnEmptyResults()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(new List<dynamic>());
        _mockUserSalesInformationRepository.Setup(x => x.GetTransactionCountByUserIdAsync(userId))
            .ReturnsAsync(0);

        // Act
        var result = await _walletService.GetSalesWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Transactions.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearWalletCache_WithValidUserId_ShouldRemoveCacheEntries()
    {
        // Arrange
        var userId = 1;
        var userWallet = CreateTestUserWallet(userId, 1000);
        var salesInfo = CreateTestUserSalesInformation(userId, 500.50m);

        // Populate cache first
        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userWallet);
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);

        // Act - Populate cache
        var _ = _walletService.GetUserWalletBalanceAsync(userId).Result;
        var __ = _walletService.GetSalesWalletBalanceAsync(userId).Result;

        // Act - Clear cache
        _walletService.ClearWalletCache(userId);

        // Act - Try to get from cache again (should hit database)
        var result1 = _walletService.GetUserWalletBalanceAsync(userId).Result;
        var result2 = _walletService.GetSalesWalletBalanceAsync(userId).Result;

        // Assert
        result1.Should().Be(1000);
        result2.Should().Be(500.50m);
        
        // Should hit database twice after cache clear
        _mockUserWalletRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Exactly(2));
        _mockUserSalesInformationRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Exactly(2));
    }

    #endregion

    #region 錯誤處理測試

    [Fact]
    public async Task GetUserWalletBalanceAsync_WhenRepositoryThrowsException_ShouldReturnZero()
    {
        // Arrange
        var userId = 1;

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _walletService.GetUserWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetSalesWalletBalanceAsync_WhenRepositoryThrowsException_ShouldReturnZero()
    {
        // Arrange
        var userId = 1;

        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _walletService.GetSalesWalletBalanceAsync(userId);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public async Task GetWalletSummaryAsync_WhenRepositoryThrowsException_ShouldReturnEmptySummary()
    {
        // Arrange
        var userId = 1;

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _walletService.GetWalletSummaryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserWalletBalance.Should().Be(0);
        result.SalesWalletBalance.Should().Be(0m);
        result.SalesProfileStatus.Should().Be("未知");
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetWalletSummaryAsync_WithValidUserId_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var userId = 1;
        var userWallet = CreateTestUserWallet(userId, 1000);
        var salesInfo = CreateTestUserSalesInformation(userId, 500.50m);
        var salesProfile = CreateTestMemberSalesProfile(userId);

        _mockUserWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(userWallet);
        _mockUserSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);
        _mockMemberSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesProfile);

        // Act & Assert
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _walletService.GetWalletSummaryAsync(userId);
        stopwatch.Stop();

        result.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 應該在1秒內完成
    }

    #endregion

    #region 邊界值測試

    [Theory]
    [InlineData(1, 1)] // 最小有效值
    [InlineData(1, 100)] // 最大有效頁面大小
    [InlineData(1000, 50)] // 大頁碼
    public async Task GetUserWalletTransactionsAsync_WithBoundaryValues_ShouldSucceed(int page, int pageSize)
    {
        // Arrange
        var userId = 1;
        var transactions = CreateTestWalletTransactions(userId, pageSize);
        var totalCount = pageSize * page;

        _mockUserWalletRepository.Setup(x => x.GetTransactionsByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(transactions);
        _mockUserWalletRepository.Setup(x => x.GetTransactionCountByUserIdAsync(userId))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _walletService.GetUserWalletTransactionsAsync(userId, page, pageSize);

        // Assert
        result.Should().NotBeNull();
        result!.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
    }

    #endregion

    #region 輔助方法

    private UserWallet CreateTestUserWallet(int userId, int balance)
    {
        return new UserWallet
        {
            User_Id = userId,
            User_Point = balance,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
    }

    private UserSalesInformation CreateTestUserSalesInformation(int userId, decimal balance)
    {
        return new UserSalesInformation
        {
            User_Id = userId,
            UserSales_Wallet = balance,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
    }

    private MemberSalesProfile CreateTestMemberSalesProfile(int userId)
    {
        return new MemberSalesProfile
        {
            User_Id = userId,
            Sales_Level = "Gold",
            Total_Sales = 10000,
            Commission_Rate = 0.05m,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
    }

    private List<dynamic> CreateTestWalletTransactions(int userId, int count)
    {
        var transactions = new List<dynamic>();
        for (int i = 1; i <= count; i++)
        {
            var transaction = new
            {
                TransactionId = i,
                UserId = userId,
                TransactionType = $"Type{i}",
                Amount = i * 100,
                Description = $"Transaction {i}",
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            };
            transactions.Add(transaction);
        }
        return transactions;
    }

    private List<dynamic> CreateTestSalesWalletTransactions(int userId, int count)
    {
        var transactions = new List<dynamic>();
        for (int i = 1; i <= count; i++)
        {
            var transaction = new
            {
                TransactionId = i,
                UserId = userId,
                TransactionType = $"SalesType{i}",
                Amount = i * 50.50m,
                Description = $"Sales Transaction {i}",
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            };
            transactions.Add(transaction);
        }
        return transactions;
    }

    #endregion
}