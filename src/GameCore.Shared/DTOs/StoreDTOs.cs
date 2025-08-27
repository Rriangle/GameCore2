using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 商品資訊 DTO - 商品基本資訊
/// </summary>
public class ProductDto
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品類型
    /// </summary>
    public string ProductType { get; set; } = string.Empty;

    /// <summary>
    /// 售價
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 幣別
    /// </summary>
    public string CurrencyCode { get; set; } = "TWD";

    /// <summary>
    /// 庫存數量
    /// </summary>
    public int ShipmentQuantity { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// 創建者
    /// </summary>
    public string? ProductCreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? ProductCreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? ProductUpdatedAt { get; set; }

    /// <summary>
    /// 商品詳細資訊 (遊戲或其他商品)
    /// </summary>
    public ProductDetailsDto? Details { get; set; }
}

/// <summary>
/// 商品詳細資訊 DTO - 抽象基底類別
/// </summary>
public abstract class ProductDetailsDto
{
    /// <summary>
    /// 商品描述
    /// </summary>
    public string? ProductDescription { get; set; }

    /// <summary>
    /// 供應商編號
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// 平台編號
    /// </summary>
    public int? PlatformId { get; set; }
}

/// <summary>
/// 遊戲商品詳細資訊 DTO
/// </summary>
public class GameProductDetailsDto : ProductDetailsDto
{
    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string? GameName { get; set; }

    /// <summary>
    /// 下載連結
    /// </summary>
    public string? DownloadLink { get; set; }
}

/// <summary>
/// 其他商品詳細資訊 DTO
/// </summary>
public class OtherProductDetailsDto : ProductDetailsDto
{
    /// <summary>
    /// 數位序號
    /// </summary>
    public string? DigitalCode { get; set; }

    /// <summary>
    /// 尺寸
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// 顏色
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    public string? Weight { get; set; }

    /// <summary>
    /// 尺寸規格
    /// </summary>
    public string? Dimensions { get; set; }

    /// <summary>
    /// 材質
    /// </summary>
    public string? Material { get; set; }

    /// <summary>
    /// 庫存數量 (字串格式)
    /// </summary>
    public string? StockQuantity { get; set; }
}

/// <summary>
/// 商品搜尋查詢 DTO
/// </summary>
public class ProductSearchQueryDto
{
    /// <summary>
    /// 關鍵字搜尋
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 商品類型篩選
    /// </summary>
    public string? ProductType { get; set; }

    /// <summary>
    /// 供應商篩選
    /// </summary>
    public int? SupplierId { get; set; }

    /// <summary>
    /// 最低價格
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高價格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 是否有庫存
    /// </summary>
    public bool? InStock { get; set; }

    /// <summary>
    /// 排序方式 (price_asc/price_desc/name_asc/name_desc/created_desc)
    /// </summary>
    public string SortBy { get; set; } = "created_desc";

    /// <summary>
    /// 頁碼
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 分頁商品列表 DTO
/// </summary>
public class PagedProductsDto
{
    /// <summary>
    /// 商品列表
    /// </summary>
    public List<ProductDto> Products { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// 購物車商品項目 DTO
/// </summary>
public class CartItemDto
{
    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "數量必須大於 0")]
    public int Quantity { get; set; }

    /// <summary>
    /// 商品資訊 (用於顯示)
    /// </summary>
    public ProductDto? Product { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    public decimal Subtotal => Product?.Price * Quantity ?? 0;
}

/// <summary>
/// 購物車 DTO
/// </summary>
public class ShoppingCartDto
{
    /// <summary>
    /// 購物車項目
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount => Items.Sum(item => item.Subtotal);

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalQuantity => Items.Sum(item => item.Quantity);
}

/// <summary>
/// 下單請求 DTO
/// </summary>
public class CreateOrderRequestDto
{
    /// <summary>
    /// 購物車項目
    /// </summary>
    [Required]
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 訂單資訊 DTO
/// </summary>
public class OrderDto
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 下訂會員編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 下訂會員名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 下單日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public string OrderStatus { get; set; } = string.Empty;

    /// <summary>
    /// 付款狀態
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// 訂單總額
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// 付款時間
    /// </summary>
    public DateTime? PaymentAt { get; set; }

    /// <summary>
    /// 出貨時間
    /// </summary>
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// 完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 訂單明細
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// 訂單明細 DTO
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// 明細編號
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    public decimal Subtotal { get; set; }
}

/// <summary>
/// 訂單查詢 DTO
/// </summary>
public class OrderSearchQueryDto
{
    /// <summary>
    /// 會員編號篩選
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 訂單狀態篩選
    /// </summary>
    public string? OrderStatus { get; set; }

    /// <summary>
    /// 付款狀態篩選
    /// </summary>
    public string? PaymentStatus { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 排序方式
    /// </summary>
    public string SortBy { get; set; } = "order_date_desc";

    /// <summary>
    /// 頁碼
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 分頁訂單列表 DTO
/// </summary>
public class PagedOrdersDto
{
    /// <summary>
    /// 訂單列表
    /// </summary>
    public List<OrderDto> Orders { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// 付款回調 DTO
/// </summary>
public class PaymentCallbackDto
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    [Required]
    public int OrderId { get; set; }

    /// <summary>
    /// 付款結果 (success/failed)
    /// </summary>
    [Required]
    public string PaymentResult { get; set; } = string.Empty;

    /// <summary>
    /// 交易編號
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal? PaidAmount { get; set; }

    /// <summary>
    /// 付款時間
    /// </summary>
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// 商城排行榜 DTO
/// </summary>
public class StoreRankingDto
{
    /// <summary>
    /// 排行榜編號
    /// </summary>
    public int RankingId { get; set; }

    /// <summary>
    /// 期間類型
    /// </summary>
    public string PeriodType { get; set; } = string.Empty;

    /// <summary>
    /// 排行榜日期
    /// </summary>
    public DateOnly RankingDate { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 排名指標
    /// </summary>
    public string RankingMetric { get; set; } = string.Empty;

    /// <summary>
    /// 名次
    /// </summary>
    public byte RankingPosition { get; set; }

    /// <summary>
    /// 交易額
    /// </summary>
    public decimal TradingAmount { get; set; }

    /// <summary>
    /// 交易量
    /// </summary>
    public int TradingVolume { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime RankingUpdatedAt { get; set; }
}

/// <summary>
/// 排行榜查詢 DTO
/// </summary>
public class RankingQueryDto
{
    /// <summary>
    /// 期間類型 (daily/monthly/quarterly/yearly)
    /// </summary>
    public string PeriodType { get; set; } = "daily";

    /// <summary>
    /// 查詢日期
    /// </summary>
    public DateOnly? Date { get; set; }

    /// <summary>
    /// 排名指標 (amount/volume)
    /// </summary>
    public string RankingMetric { get; set; } = "amount";

    /// <summary>
    /// 取得筆數
    /// </summary>
    public int Limit { get; set; } = 10;
}

/// <summary>
/// 供應商 DTO
/// </summary>
public class SupplierDto
{
    /// <summary>
    /// 供應商編號
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 商品數量
    /// </summary>
    public int ProductCount { get; set; }
}