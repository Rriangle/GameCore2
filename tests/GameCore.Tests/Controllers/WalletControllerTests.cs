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
/// 錢包控制器測試
/// </summary>
public class WalletControllerTests
{
    private readonly Mock<IWalletService> _mockWalletService;
    private readonly Mock<ILogger<WalletController>> _mockLogger;
    private readonly WalletController _walletController;
    private readonly Mock<HttpContext> _mockHttpContext;

    public WalletControllerTests()
    {
        _mockWalletService = new Mock<IWalletService>();
        _mockLogger = new Mock<ILogger<WalletController>>();
        _walletController = new WalletController(_mockWalletService.Object, _mockLogger.Object);
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
        _walletController.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };
    }

    [Fact]
    public async Task GetBalance_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 5000;

        _mockWalletService.Setup(x => x.GetUserWalletBalanceAsync(userId))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _walletController.GetBalance();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<int>>(okResult.Value);
        Assert.Equal(expectedBalance, response.Data);
        Assert.Equal("獲取錢包餘額成功", response.Message);
        _mockWalletService.Verify(x => x.GetUserWalletBalanceAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetTransactions_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
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
            }
        };

        _mockWalletService.Setup(x => x.GetUserWalletTransactionsAsync(userId, page, pageSize))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletController.GetTransactions(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<WalletTransactionDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedTransactions.Count, response.Data.Count);
        Assert.Equal(expectedTransactions[0].TransactionId, response.Data[0].TransactionId);
        Assert.Equal(expectedTransactions[0].Amount, response.Data[0].Amount);
        Assert.Equal(expectedTransactions[0].Description, response.Data[0].Description);
        Assert.Equal("獲取錢包交易記錄成功", response.Message);
        _mockWalletService.Verify(x => x.GetUserWalletTransactionsAsync(userId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetTransactions_WithPagination_ReturnsCorrectPage()
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

        _mockWalletService.Setup(x => x.GetUserWalletTransactionsAsync(userId, page, pageSize))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletController.GetTransactions(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<WalletTransactionDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal(expectedTransactions[0].TransactionId, response.Data[0].TransactionId);
        _mockWalletService.Verify(x => x.GetUserWalletTransactionsAsync(userId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetSalesBalance_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var expectedBalance = 15000.00m;

        _mockWalletService.Setup(x => x.GetSalesWalletBalanceAsync(userId))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _walletController.GetSalesBalance();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<decimal>>(okResult.Value);
        Assert.Equal(expectedBalance, response.Data);
        Assert.Equal("獲取銷售錢包餘額成功", response.Message);
        _mockWalletService.Verify(x => x.GetSalesWalletBalanceAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetSalesTransactions_ValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
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
            }
        };

        _mockWalletService.Setup(x => x.GetSalesWalletTransactionsAsync(userId, page, pageSize))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletController.GetSalesTransactions(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<SalesWalletTransactionDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(expectedTransactions.Count, response.Data.Count);
        Assert.Equal(expectedTransactions[0].TransactionId, response.Data[0].TransactionId);
        Assert.Equal(expectedTransactions[0].Amount, response.Data[0].Amount);
        Assert.Equal(expectedTransactions[0].Description, response.Data[0].Description);
        Assert.Equal("獲取銷售錢包交易記錄成功", response.Message);
        _mockWalletService.Verify(x => x.GetSalesWalletTransactionsAsync(userId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetSalesTransactions_WithPagination_ReturnsCorrectPage()
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

        _mockWalletService.Setup(x => x.GetSalesWalletTransactionsAsync(userId, page, pageSize))
            .ReturnsAsync(expectedTransactions);

        // Act
        var result = await _walletController.GetSalesTransactions(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<SalesWalletTransactionDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Single(response.Data);
        Assert.Equal(expectedTransactions[0].TransactionId, response.Data[0].TransactionId);
        _mockWalletService.Verify(x => x.GetSalesWalletTransactionsAsync(userId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetTransactions_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        _mockWalletService.Setup(x => x.GetUserWalletTransactionsAsync(userId, page, pageSize))
            .ReturnsAsync(new List<WalletTransactionDto>());

        // Act
        var result = await _walletController.GetTransactions(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<WalletTransactionDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
        _mockWalletService.Verify(x => x.GetUserWalletTransactionsAsync(userId, page, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetSalesTransactions_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        _mockWalletService.Setup(x => x.GetSalesWalletTransactionsAsync(userId, page, pageSize))
            .ReturnsAsync(new List<SalesWalletTransactionDto>());

        // Act
        var result = await _walletController.GetSalesTransactions(page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<SalesWalletTransactionDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
        _mockWalletService.Verify(x => x.GetSalesWalletTransactionsAsync(userId, page, pageSize), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public async Task GetTransactions_InvalidPagination_ReturnsBadRequest(int page, int pageSize)
    {
        // Act
        var result = await _walletController.GetTransactions(page, pageSize);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<WalletTransactionDto>>>(badRequestResult.Value);
        Assert.Null(response.Data);
        Assert.Contains("分頁參數無效", response.Message);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public async Task GetSalesTransactions_InvalidPagination_ReturnsBadRequest(int page, int pageSize)
    {
        // Act
        var result = await _walletController.GetSalesTransactions(page, pageSize);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<SalesWalletTransactionDto>>>(badRequestResult.Value);
        Assert.Null(response.Data);
        Assert.Contains("分頁參數無效", response.Message);
    }

    [Fact]
    public void GetCurrentUserId_ValidClaims_ReturnsCorrectUserId()
    {
        // Arrange
        var expectedUserId = 1;

        // Act
        var result = _walletController.GetCurrentUserId();

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
        _walletController.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };

        // Act
        var result = _walletController.GetCurrentUserId();

        // Assert
        Assert.Equal(0, result);
    }
}