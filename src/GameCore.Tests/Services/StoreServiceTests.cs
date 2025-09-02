using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 商店服務測試
/// </summary>
public class StoreServiceTests
{
    private readonly Mock<IProductInfoRepository> _mockProductRepository;
    private readonly Mock<IOrderInfoRepository> _mockOrderRepository;
    private readonly Mock<IUserWalletRepository> _mockWalletRepository;
    private readonly Mock<ISupplierRepository> _mockSupplierRepository;
    private readonly StoreService _storeService;

    public StoreServiceTests()
    {
        _mockProductRepository = new Mock<IProductInfoRepository>();
        _mockOrderRepository = new Mock<IOrderInfoRepository>();
        _mockWalletRepository = new Mock<IUserWalletRepository>();
        _mockSupplierRepository = new Mock<ISupplierRepository>();
        _storeService = new StoreService(_mockProductRepository.Object, _mockOrderRepository.Object, _mockWalletRepository.Object, _mockSupplierRepository.Object);
    }

    [Fact]
    public async Task GetProductsAsync_WithValidParameters_ReturnsPagedResult()
    {
        // Arrange
        var category = "遊戲";
        var searchTerm = "RPG";
        var page = 1;
        var pageSize = 20;
        var products = new List<ProductInfo>
        {
            new ProductInfo
            {
                ProductID = 1,
                Product_Name = "RPG遊戲大作",
                Category = category,
                Price = 299.00m,
                Description = "經典RPG遊戲",
                Stock_Quantity = 100,
                SupplierID = 1,
                Is_Active = true,
                Created_At = DateTime.UtcNow
            }
        };

        _mockProductRepository.Setup(x => x.GetProductsAsync(category, searchTerm, page, pageSize))
            .ReturnsAsync(products);
        _mockProductRepository.Setup(x => x.GetProductCountAsync(category, searchTerm))
            .ReturnsAsync(1);

        // Act
        var result = await _storeService.GetProductsAsync(category, searchTerm, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Single(result.Data);
        Assert.Equal("RPG遊戲大作", result.Data[0].ProductName);
        Assert.Equal(category, result.Data[0].Category);
    }

    [Fact]
    public async Task GetProductDetailAsync_WithValidProductId_ReturnsProductDetailDto()
    {
        // Arrange
        var productId = 1;
        var product = new ProductInfo
        {
            ProductID = productId,
            Product_Name = "RPG遊戲大作",
            Category = "遊戲",
            Price = 299.00m,
            Description = "經典RPG遊戲",
            Stock_Quantity = 100,
            SupplierID = 1,
            Is_Active = true,
            Created_At = DateTime.UtcNow,
            Updated_At = DateTime.UtcNow
        };

        var supplier = new Supplier
        {
            SupplierID = 1,
            SupplierName = "遊戲供應商"
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mockSupplierRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(supplier);

        // Act
        var result = await _storeService.GetProductDetailAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal("RPG遊戲大作", result.ProductName);
        Assert.Equal("遊戲供應商", result.SupplierName);
    }

    [Fact]
    public async Task GetProductDetailAsync_WithInvalidProductId_ReturnsNull()
    {
        // Arrange
        var productId = 999;
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((ProductInfo?)null);

        // Act
        var result = await _storeService.GetProductDetailAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        var product = new ProductInfo
        {
            ProductID = 1,
            Product_Name = "RPG遊戲大作",
            Category = "遊戲",
            Price = 299.00m,
            Description = "經典RPG遊戲",
            Stock_Quantity = 100,
            SupplierID = 1,
            Is_Active = true,
            Created_At = DateTime.UtcNow
        };

        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 1000,
            User_Balance = 500.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);
        _mockWalletRepository.Setup(x => x.DeductPointsAsync(userId, 598, It.IsAny<string>()))
            .ReturnsAsync(true);
        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<OrderInfo>()))
            .ReturnsAsync(1);
        _mockOrderRepository.Setup(x => x.AddOrderItemAsync(It.IsAny<OrderItem>()))
            .ReturnsAsync(true);
        _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<ProductInfo>()))
            .ReturnsAsync(true);

        // Act
        var result = await _storeService.CreateOrderAsync(userId, request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.OrderId);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal(598.00m, result.Data.TotalAmount);
        Assert.Equal(2, result.Data.Items.Count);
    }

    [Fact]
    public async Task CreateOrderAsync_WithEmptyItems_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>()
        };

        // Act
        var result = await _storeService.CreateOrderAsync(userId, request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("訂單項目不能為空", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientPoints_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        var product = new ProductInfo
        {
            ProductID = 1,
            Product_Name = "RPG遊戲大作",
            Category = "遊戲",
            Price = 299.00m,
            Description = "經典RPG遊戲",
            Stock_Quantity = 100,
            SupplierID = 1,
            Is_Active = true,
            Created_At = DateTime.UtcNow
        };

        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 100, // 點數不足
            User_Balance = 50.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);

        // Act
        var result = await _storeService.CreateOrderAsync(userId, request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("點數不足", result.Message);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientStock_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 200 // 超過庫存
                }
            }
        };

        var product = new ProductInfo
        {
            ProductID = 1,
            Product_Name = "RPG遊戲大作",
            Category = "遊戲",
            Price = 299.00m,
            Description = "經典RPG遊戲",
            Stock_Quantity = 100, // 庫存只有100
            SupplierID = 1,
            Is_Active = true,
            Created_At = DateTime.UtcNow
        };

        var wallet = new UserWallet
        {
            WalletID = 1,
            User_Id = userId,
            User_Point = 10000,
            User_Balance = 5000.00m,
            Last_Updated = DateTime.UtcNow
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockWalletRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);

        // Act
        var result = await _storeService.CreateOrderAsync(userId, request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("商品 RPG遊戲大作 庫存不足", result.Message);
    }

    [Fact]
    public async Task GetUserOrdersAsync_WithValidData_ReturnsPagedResult()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;
        var orders = new List<OrderInfo>
        {
            new OrderInfo
            {
                OrderID = 1,
                UserID = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 598.00m,
                Status = "completed",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var orderItems = new List<OrderItem>
        {
            new OrderItem
            {
                OrderID = 1,
                ProductID = 1,
                Quantity = 2,
                UnitPrice = 299.00m,
                TotalPrice = 598.00m
            }
        };

        _mockOrderRepository.Setup(x => x.GetByUserIdAsync(userId, page, pageSize))
            .ReturnsAsync(orders);
        _mockOrderRepository.Setup(x => x.GetCountByUserIdAsync(userId))
            .ReturnsAsync(1);
        _mockOrderRepository.Setup(x => x.GetOrderItemsAsync(1))
            .ReturnsAsync(orderItems);

        // Act
        var result = await _storeService.GetUserOrdersAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Data[0].OrderId);
        Assert.Equal(598.00m, result.Data[0].TotalAmount);
    }

    [Fact]
    public async Task GetProductCategoriesAsync_ReturnsCategoryList()
    {
        // Arrange
        var categories = new List<string> { "遊戲", "硬體", "周邊" };
        _mockProductRepository.Setup(x => x.GetCategoriesAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _storeService.GetProductCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains("遊戲", result);
        Assert.Contains("硬體", result);
        Assert.Contains("周邊", result);
    }

    [Fact]
    public async Task GetPopularProductsAsync_WithValidTop_ReturnsProductList()
    {
        // Arrange
        var top = 5;
        var products = new List<ProductInfo>
        {
            new ProductInfo
            {
                ProductID = 1,
                Product_Name = "熱門遊戲1",
                Category = "遊戲",
                Price = 299.00m,
                Description = "熱門遊戲",
                Stock_Quantity = 100,
                SupplierID = 1,
                Is_Active = true,
                Created_At = DateTime.UtcNow
            },
            new ProductInfo
            {
                ProductID = 2,
                Product_Name = "熱門遊戲2",
                Category = "遊戲",
                Price = 399.00m,
                Description = "熱門遊戲",
                Stock_Quantity = 80,
                SupplierID = 1,
                Is_Active = true,
                Created_At = DateTime.UtcNow
            }
        };

        _mockProductRepository.Setup(x => x.GetPopularProductsAsync(top))
            .ReturnsAsync(products);

        // Act
        var result = await _storeService.GetPopularProductsAsync(top);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("熱門遊戲1", result[0].ProductName);
        Assert.Equal("熱門遊戲2", result[1].ProductName);
    }
} 