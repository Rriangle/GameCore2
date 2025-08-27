namespace GameCore.Domain.Interfaces;

public interface IMarketService
{
    Task<IEnumerable<MarketItemDto>> GetMarketItemsAsync(string? category = null);
    Task<MarketItemDto?> GetMarketItemAsync(int itemId);
    Task<MarketItemResult> CreateMarketItemAsync(int userId, string name, string description, decimal price, string? imageUrl, string? category);
    Task<MarketItemResult> UpdateMarketItemAsync(int itemId, int userId, string name, string description, decimal price, string? imageUrl, string? category);
    Task<MarketItemResult> CancelMarketItemAsync(int itemId, int userId);
    Task<MarketTransactionResult> PurchaseItemAsync(int itemId, int buyerId);
    Task<MarketTransactionResult> ConfirmTransactionAsync(int transactionId, int userId, bool isSeller);
    Task<IEnumerable<MarketTransactionDto>> GetUserTransactionsAsync(int userId);
    Task<MarketTransactionDto?> GetTransactionAsync(int transactionId);
}

public class MarketItemDto
{
    public int ItemId { get; set; }
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MarketTransactionDto
{
    public int TransactionId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int BuyerId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal PlatformFee { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime? SellerConfirmedAt { get; set; }
    public DateTime? BuyerConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MarketItemResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public MarketItemDto? Item { get; set; }
}

public class MarketTransactionResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public MarketTransactionDto? Transaction { get; set; }
}