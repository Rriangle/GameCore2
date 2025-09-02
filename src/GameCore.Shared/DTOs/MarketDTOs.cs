using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 玩家市場商品資訊 DTO
/// </summary>
public class MarketProductDto
{
    public int ProductId { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Images { get; set; } = new List<string>();
}

/// <summary>
/// 建立市場商品 DTO
/// </summary>
public class CreateMarketProductDto
{
    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(1, 999)]
    public int Quantity { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Condition { get; set; } = string.Empty;
    
    public List<string> ImageUrls { get; set; } = new List<string>();
}

/// <summary>
/// 市場訂單 DTO
/// </summary>
public class MarketOrderDto
{
    public int OrderID { get; set; }
    public int BuyerUserID { get; set; }
    public int SellerUserID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PlatformFee { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
}

/// <summary>
/// 建立市場訂單 DTO
/// </summary>
public class CreateMarketOrderDto
{
    [Required]
    public int ProductID { get; set; }
    
    [Required]
    [Range(1, 999)]
    public int Quantity { get; set; }
}

/// <summary>
/// 市場排行榜項目 DTO
/// </summary>
public class MarketRankingDto
{
    public int Rank { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageRating { get; set; }
    public string Change { get; set; } = string.Empty; // "up", "down", "flat"
}

/// <summary>
/// 市場搜尋參數 DTO
/// </summary>
public class MarketSearchDto
{
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Condition { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "created"; // "created", "price", "rating"
    public string SortOrder { get; set; } = "desc"; // "asc", "desc"
}

/// <summary>
/// 市場搜尋請求 DTO
/// </summary>
public class MarketSearchRequest
{
    /// <summary>
    /// 搜尋關鍵字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// 商品類型
    /// </summary>
    public string? ProductType { get; set; }

    /// <summary>
    /// 最低價格
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高價格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 賣家ID
    /// </summary>
    public int? SellerId { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁大小
    /// </summary>
    public int PageSize { get; set; } = 20;
}
