using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 銷售服務測試
/// </summary>
public class SalesServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMemberSalesProfileRepository> _mockSalesProfileRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockSalesInfoRepository;
    private readonly SalesService _salesService;

    public SalesServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockSalesProfileRepository = new Mock<IMemberSalesProfileRepository>();
        _mockSalesInfoRepository = new Mock<IUserSalesInformationRepository>();
        _salesService = new SalesService(_mockUserRepository.Object, _mockSalesProfileRepository.Object, _mockSalesInfoRepository.Object);
    }

    [Fact]
    public async Task GetSalesProfileAsync_WithValidUserId_ReturnsSalesProfileDto()
    {
        // Arrange
        var userId = 1;
        var profile = new MemberSalesProfile
        {
            ProfileID = 1,
            User_Id = userId,
            Sales_Level = 2,
            Total_Sales = 5000.00m,
            Commission_Rate = 15.0m,
            Is_Active = true,
            Last_Updated = DateTime.UtcNow
        };

        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(profile);

        // Act
        var result = await _salesService.GetSalesProfileAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2, result.SalesLevel);
        Assert.Equal(5000.00m, result.TotalSales);
        Assert.Equal(15.0m, result.CommissionRate);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetSalesProfileAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((MemberSalesProfile?)null);

        // Act
        var result = await _salesService.GetSalesProfileAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateSalesProfileAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var request = new UpdateSalesProfileRequest
        {
            SalesLevel = 3,
            CommissionRate = 20.0m,
            IsActive = true
        };

        var profile = new MemberSalesProfile
        {
            ProfileID = 1,
            User_Id = userId,
            Sales_Level = 2,
            Total_Sales = 5000.00m,
            Commission_Rate = 15.0m,
            Is_Active = true,
            Last_Updated = DateTime.UtcNow
        };

        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(profile);
        _mockSalesProfileRepository.Setup(x => x.UpdateAsync(It.IsAny<MemberSalesProfile>()))
            .ReturnsAsync(true);

        // Act
        var result = await _salesService.UpdateSalesProfileAsync(userId, request);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task UpdateSalesProfileAsync_WithInvalidCommissionRate_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var request = new UpdateSalesProfileRequest
        {
            CommissionRate = 150.0m // 超過100%
        };

        var profile = new MemberSalesProfile
        {
            ProfileID = 1,
            User_Id = userId,
            Sales_Level = 2,
            Total_Sales = 5000.00m,
            Commission_Rate = 15.0m,
            Is_Active = true,
            Last_Updated = DateTime.UtcNow
        };

        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(profile);

        // Act
        var result = await _salesService.UpdateSalesProfileAsync(userId, request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("佣金率必須在 0-100 之間", result.Message);
    }

    [Fact]
    public async Task GetSalesStatisticsAsync_WithValidUserId_ReturnsSalesStatisticsDto()
    {
        // Arrange
        var userId = 1;
        var salesInfo = new UserSalesInformation
        {
            InformationID = 1,
            User_Id = userId,
            Total_Orders = 25,
            Total_Revenue = 7500.00m,
            Last_Order_Date = DateTime.UtcNow.AddDays(-1),
            Customer_Count = 15,
            Created_At = DateTime.UtcNow.AddDays(-30),
            Updated_At = DateTime.UtcNow
        };

        _mockSalesInfoRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);

        // Act
        var result = await _salesService.GetSalesStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(25, result.TotalOrders);
        Assert.Equal(7500.00m, result.TotalRevenue);
        Assert.Equal(300.00m, result.AverageOrderValue); // 7500 / 25
        Assert.Equal(15, result.CustomerCount);
    }

    [Fact]
    public async Task GetSalesStatisticsAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        _mockSalesInfoRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((UserSalesInformation?)null);

        // Act
        var result = await _salesService.GetSalesStatisticsAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RecordSalesTransactionAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var orderAmount = 500.00m;
        var customerId = 100;

        var salesInfo = new UserSalesInformation
        {
            InformationID = 1,
            User_Id = userId,
            Total_Orders = 25,
            Total_Revenue = 7500.00m,
            Last_Order_Date = DateTime.UtcNow.AddDays(-1),
            Customer_Count = 15,
            Created_At = DateTime.UtcNow.AddDays(-30),
            Updated_At = DateTime.UtcNow
        };

        _mockSalesInfoRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);
        _mockSalesInfoRepository.Setup(x => x.UpdateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync(true);

        // Act
        var result = await _salesService.RecordSalesTransactionAsync(userId, orderAmount, customerId);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task RecordSalesTransactionAsync_WithZeroOrderAmount_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var orderAmount = 0.00m;
        var customerId = 100;

        // Act
        var result = await _salesService.RecordSalesTransactionAsync(userId, orderAmount, customerId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("訂單金額必須大於0", result.Message);
    }

    [Fact]
    public async Task RecordSalesTransactionAsync_WithNegativeOrderAmount_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var orderAmount = -100.00m;
        var customerId = 100;

        // Act
        var result = await _salesService.RecordSalesTransactionAsync(userId, orderAmount, customerId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("訂單金額必須大於0", result.Message);
    }

    [Fact]
    public async Task GetSalesRankingAsync_WithValidTop_ReturnsRankingList()
    {
        // Arrange
        var top = 5;
        var rankings = new List<UserSalesInformation>
        {
            new UserSalesInformation
            {
                InformationID = 1,
                User_Id = 1,
                Total_Orders = 50,
                Total_Revenue = 15000.00m,
                Customer_Count = 30
            },
            new UserSalesInformation
            {
                InformationID = 2,
                User_Id = 2,
                Total_Orders = 30,
                Total_Revenue = 9000.00m,
                Customer_Count = 20
            }
        };

        _mockSalesInfoRepository.Setup(x => x.GetTopSalesUsersAsync(top))
            .ReturnsAsync(rankings);

        // Act
        var result = await _salesService.GetSalesRankingAsync(top);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Rank);
        Assert.Equal(2, result[1].Rank);
        Assert.Equal(15000.00m, result[0].TotalSales);
        Assert.Equal(9000.00m, result[1].TotalSales);
    }

    [Fact]
    public async Task CalculateCommissionAsync_WithValidData_ReturnsCorrectCommission()
    {
        // Arrange
        var userId = 1;
        var orderAmount = 1000.00m;
        var profile = new MemberSalesProfile
        {
            ProfileID = 1,
            User_Id = userId,
            Sales_Level = 2,
            Total_Sales = 5000.00m,
            Commission_Rate = 15.0m,
            Is_Active = true,
            Last_Updated = DateTime.UtcNow
        };

        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(profile);

        // Act
        var result = await _salesService.CalculateCommissionAsync(userId, orderAmount);

        // Assert
        Assert.Equal(150.00m, result); // 1000 * 15% = 150
    }

    [Fact]
    public async Task CalculateCommissionAsync_WithInactiveProfile_ReturnsZero()
    {
        // Arrange
        var userId = 1;
        var orderAmount = 1000.00m;
        var profile = new MemberSalesProfile
        {
            ProfileID = 1,
            User_Id = userId,
            Sales_Level = 2,
            Total_Sales = 5000.00m,
            Commission_Rate = 15.0m,
            Is_Active = false,
            Last_Updated = DateTime.UtcNow
        };

        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(profile);

        // Act
        var result = await _salesService.CalculateCommissionAsync(userId, orderAmount);

        // Assert
        Assert.Equal(0.00m, result);
    }

    [Fact]
    public async Task CalculateCommissionAsync_WithNullProfile_ReturnsZero()
    {
        // Arrange
        var userId = 999;
        var orderAmount = 1000.00m;

        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((MemberSalesProfile?)null);

        // Act
        var result = await _salesService.CalculateCommissionAsync(userId, orderAmount);

        // Assert
        Assert.Equal(0.00m, result);
    }
} 