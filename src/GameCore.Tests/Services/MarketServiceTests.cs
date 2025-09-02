using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 玩家市場服務測試
/// </summary>
public class MarketServiceTests
{
    private readonly Mock<IPlayerMarketProductInfoRepository> _mockProductRepository;
    private readonly Mock<IPlayerMarketOrderInfoRepository> _mockOrderRepository;
    private readonly Mock<IUserWalletRepository> _mockWalletRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly MarketService _marketService;

    public MarketServiceTests()
    {
        _mockProductRepository = new Mock<IPlayerMarketProductInfoRepository>();
        _mockOrderRepository = new Mock<IPlayerMarketOrderInfoRepository>();
        _mockWalletRepository = new Mock<IUserWalletRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _marketService = new MarketService(_mockProductRepository.Object, _mockOrderRepository.Object, _mockWalletRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task GetProductsAsync_WithValidParameters_ReturnsPagedResult()
    {
        // Arrange
        var searchParams = new MarketSearchDto
        {
            Category = "遊戲",
            SearchTerm = "RPG",
            Page = 1,
            PageSize = 20
        };

        var products = new List<PlayerMarketProductInfo>
        {
            new PlayerMarketProductInfo
            {
                ProductID = 1,
                Seller_User_ID = 1,
                Product_Name = "RPG遊戲大作",
                Description = "經典RPG遊戲",
                Price = 299.00m,
                Quantity = 1,
                Category = "遊戲",
                Condition = "九成新",
                Is_Active = true,
                Created_At = DateTime.UtcNow,
                Updated_At = DateTime.UtcNow,
                Seller = new UserSalesInformation { User_Name = "賣家1", Seller_Rating = 4.5m },
                Images = new List<PlayerMarketProductImg>
                {
                    new PlayerMarketProductImg { Image_URL = "image1.jpg" }
                }
            }
        };

        var mockQueryable = products.AsQueryable();
        _mockProductRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _marketService.GetProductsAsync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Single(result.Data);
        Assert.Equal("RPG遊戲大作", result.Data[0].ProductName);
        Assert.Equal("遊戲", result.Data[0].Category);
    }

    [Fact]
    public async Task GetProductDetailAsync_WithValidProductId_ReturnsProductDetail()
    {
        // Arrange
        var productId = 1;
        var product = new PlayerMarketProductInfo
        {
            ProductID = productId,
            Seller_User_ID = 1,
            Product_Name = "RPG遊戲大作",
            Description = "經典RPG遊戲",
            Price = 299.00m,
            Quantity = 1,
            Category = "遊戲",
            Condition = "九成新",
            Is_Active = true,
            Created_At = DateTime.UtcNow,
            Updated_At = DateTime.UtcNow,
            Seller = new UserSalesInformation { User_Name = "賣家1", Seller_Rating = 4.5m },
            Images = new List<PlayerMarketProductImg>
            {
                new PlayerMarketProductImg { Image_URL = "image1.jpg" }
            }
        };

        var mockQueryable = new List<PlayerMarketProductInfo> { product }.AsQueryable();
        _mockProductRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _marketService.GetProductDetailAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductID);
        Assert.Equal("RPG遊戲大作", result.ProductName);
        Assert.Equal("賣家1", result.SellerName);
        Assert.Equal(4.5m, result.SellerRating);
    }

    [Fact]
    public async Task GetProductDetailAsync_WithInactiveProduct_ReturnsNull()
    {
        // Arrange
        var productId = 1;
        var product = new PlayerMarketProductInfo
        {
            ProductID = productId,
            Is_Active = false
        };

        var mockQueryable = new List<PlayerMarketProductInfo> { product }.AsQueryable();
        _mockProductRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _marketService.GetProductDetailAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidOrder_CreatesOrderSuccessfully()
    {
        // Arrange
        var buyerUserId = 1;
        var orderDto = new CreateMarketOrderDto
        {
            ProductID = 1,
            Quantity = 1
        };

        var product = new PlayerMarketProductInfo
        {
            ProductID = 1,
            Seller_User_ID = 2,
            Product_Name = "RPG遊戲大作",
            Price = 299.00m,
            Quantity = 2,
            Is_Active = true,
            Seller = new UserSalesInformation { User_Name = "賣家1", Seller_Rating = 4.5m }
        };

        var buyerWallet = new UserWallet
        {
            User_ID = buyerUserId,
            User_Point = 1000
        };

        var buyer = new User { User_ID = buyerUserId, User_Name = "買家1" };
        var seller = new User { User_ID = 2, User_Name = "賣家1" };

        _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(buyerUserId)).ReturnsAsync(buyerWallet);
        _mockUserRepository.Setup(x => x.GetByIdAsync(buyerUserId)).ReturnsAsync(buyer);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(seller);

        var createdOrder = new PlayerMarketOrderInfo
        {
            OrderID = 1,
            Buyer_User_ID = buyerUserId,
            Seller_User_ID = 2,
            Product_ID = 1,
            Quantity = 1,
            Unit_Price = 299.00m,
            Total_Amount = 299.00m,
            Platform_Fee = 15.00m,
            Status = "pending",
            Created_At = DateTime.UtcNow
        };

        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<PlayerMarketOrderInfo>())).ReturnsAsync(createdOrder);
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<PlayerMarketProductInfo>())).ReturnsAsync(true);
        _mockWalletRepository.Setup(x => x.DeductPointsAsync(buyerUserId, It.IsAny<decimal>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _marketService.CreateOrderAsync(buyerUserId, orderDto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.OrderID);
        Assert.Equal(buyerUserId, result.Data.BuyerUserID);
        Assert.Equal(2, result.Data.SellerUserID);
        Assert.Equal(1, result.Data.ProductID);
        Assert.Equal(1, result.Data.Quantity);
        Assert.Equal(299.00m, result.Data.UnitPrice);
        Assert.Equal(299.00m, result.Data.TotalAmount);
        Assert.Equal(15.00m, result.Data.PlatformFee);
        Assert.Equal("pending", result.Data.Status);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientPoints_ReturnsFailure()
    {
        // Arrange
        var buyerUserId = 1;
        var orderDto = new CreateMarketOrderDto
        {
            ProductID = 1,
            Quantity = 1
        };

        var product = new PlayerMarketProductInfo
        {
            ProductID = 1,
            Price = 299.00m,
            Quantity = 1,
            Is_Active = true
        };

        var buyerWallet = new UserWallet
        {
            User_ID = buyerUserId,
            User_Point = 100 // 不足的點數
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(buyerUserId)).ReturnsAsync(buyerWallet);

        // Act
        var result = await _marketService.CreateOrderAsync(buyerUserId, orderDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("點數不足", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientStock_ReturnsFailure()
    {
        // Arrange
        var buyerUserId = 1;
        var orderDto = new CreateMarketOrderDto
        {
            ProductID = 1,
            Quantity = 2
        };

        var product = new PlayerMarketProductInfo
        {
            ProductID = 1,
            Price = 299.00m,
            Quantity = 1, // 庫存不足
            Is_Active = true
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _marketService.CreateOrderAsync(buyerUserId, orderDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("商品庫存不足", result.Message);
    }

    [Fact]
    public async Task GetUserOrdersAsync_WithValidParameters_ReturnsPagedResult()
    {
        // Arrange
        var userId = 1;
        var role = "buyer";
        var page = 1;
        var pageSize = 20;

        var orders = new List<PlayerMarketOrderInfo>
        {
            new PlayerMarketOrderInfo
            {
                OrderID = 1,
                Buyer_User_ID = userId,
                Seller_User_ID = 2,
                Product_ID = 1,
                Quantity = 1,
                Unit_Price = 299.00m,
                Total_Amount = 299.00m,
                Platform_Fee = 15.00m,
                Status = "pending",
                Created_At = DateTime.UtcNow,
                Product = new PlayerMarketProductInfo { Product_Name = "RPG遊戲大作" },
                Buyer = new User { User_Name = "買家1" },
                Seller = new User { User_Name = "賣家1" }
            }
        };

        var mockQueryable = orders.AsQueryable();
        _mockOrderRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _marketService.GetUserOrdersAsync(userId, role, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Data[0].OrderID);
        Assert.Equal("RPG遊戲大作", result.Data[0].ProductName);
    }

    [Fact]
    public async Task GetMarketRankingAsync_WithValidTop_ReturnsRankingList()
    {
        // Arrange
        var top = 5;
        var orders = new List<PlayerMarketOrderInfo>
        {
            new PlayerMarketOrderInfo
            {
                Seller_User_ID = 1,
                Total_Amount = 1000.00m,
                Seller_Rating = 4.5m
            },
            new PlayerMarketOrderInfo
            {
                Seller_User_ID = 2,
                Total_Amount = 800.00m,
                Seller_Rating = 4.2m
            }
        };

        var mockQueryable = orders.AsQueryable();
        _mockOrderRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        var user1 = new User { User_ID = 1, User_Name = "賣家1" };
        var user2 = new User { User_ID = 2, User_Name = "賣家2" };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user1);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(user2);

        // Act
        var result = await _marketService.GetMarketRankingAsync(top);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Rank);
        Assert.Equal(1, result[0].UserID);
        Assert.Equal("賣家1", result[0].UserName);
        Assert.Equal(1000.00m, result[0].TotalRevenue);
        Assert.Equal(2, result[1].Rank);
        Assert.Equal(2, result[1].UserID);
        Assert.Equal("賣家2", result[1].UserName);
        Assert.Equal(800.00m, result[1].TotalRevenue);
    }

    [Fact]
    public async Task GetMarketRankingAsync_WithEmptyOrders_ReturnsEmptyList()
    {
        // Arrange
        var top = 5;
        var mockQueryable = new List<PlayerMarketOrderInfo>().AsQueryable();
        _mockOrderRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _marketService.GetMarketRankingAsync(top);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
} 