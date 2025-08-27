using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 自由市場商品 DTO - 商品資訊展示和操作
/// </summary>
public class PlayerMarketProductDto
{
    /// <summary>
    /// 自由市場商品編號
    /// </summary>
    public int PProductId { get; set; }

    /// <summary>
    /// 商品類型
    /// </summary>
    public string PProductType { get; set; } = string.Empty;

    /// <summary>
    /// 商品標題
    /// </summary>
    public string PProductTitle { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string PProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? PProductDescription { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    public decimal PProductPrice { get; set; }

    /// <summary>
    /// 商品數量
    /// </summary>
    public int PProductQuantity { get; set; }

    /// <summary>
    /// 商品狀態
    /// </summary>
    public string PStatus { get; set; } = string.Empty;

    /// <summary>
    /// 賣家編號
    /// </summary>
    public int SellerId { get; set; }

    /// <summary>
    /// 賣家名稱
    /// </summary>
    public string? SellerName { get; set; }

    /// <summary>
    /// 關聯官方商品編號
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 商品圖片列表
    /// </summary>
    public List<PlayerMarketProductImageDto> Images { get; set; } = new();

    /// <summary>
    /// 瀏覽次數
    /// </summary>
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 收藏次數
    /// </summary>
    public int FavoriteCount { get; set; } = 0;
}

/// <summary>
/// 自由市場商品圖片 DTO
/// </summary>
public class PlayerMarketProductImageDto
{
    /// <summary>
    /// 圖片編號
    /// </summary>
    public int PProductImgId { get; set; }

    /// <summary>
    /// 圖片說明
    /// </summary>
    public string? ImgDescription { get; set; }

    /// <summary>
    /// 圖片順序
    /// </summary>
    public int ImgOrder { get; set; }

    /// <summary>
    /// 是否為主圖片
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// 圖片 Base64 編碼 (用於傳輸)
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// 上傳時間
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// 建立自由市場商品請求 DTO
/// </summary>
public class CreatePlayerMarketProductDto
{
    /// <summary>
    /// 商品類型
    /// </summary>
    [Required]
    [StringLength(100)]
    public string PProductType { get; set; } = string.Empty;

    /// <summary>
    /// 商品標題
    /// </summary>
    [Required]
    [StringLength(200)]
    public string PProductTitle { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    [Required]
    [StringLength(200)]
    public string PProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    [StringLength(2000)]
    public string? PProductDescription { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "價格必須大於 0")]
    public decimal PProductPrice { get; set; }

    /// <summary>
    /// 商品數量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "數量必須大於 0")]
    public int PProductQuantity { get; set; }

    /// <summary>
    /// 關聯官方商品編號 (可選)
    /// </summary>
    public int? ProductId { get; set; }

    /// <summary>
    /// 商品圖片列表 (Base64 編碼)
    /// </summary>
    public List<CreatePlayerMarketProductImageDto> Images { get; set; } = new();
}

/// <summary>
/// 建立自由市場商品圖片 DTO
/// </summary>
public class CreatePlayerMarketProductImageDto
{
    /// <summary>
    /// 圖片說明
    /// </summary>
    [StringLength(200)]
    public string? ImgDescription { get; set; }

    /// <summary>
    /// 圖片順序
    /// </summary>
    public int ImgOrder { get; set; } = 0;

    /// <summary>
    /// 是否為主圖片
    /// </summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// 圖片 Base64 編碼
    /// </summary>
    [Required]
    public string ImageBase64 { get; set; } = string.Empty;
}

/// <summary>
/// 自由市場商品搜尋查詢 DTO
/// </summary>
public class PlayerMarketProductSearchDto
{
    /// <summary>
    /// 關鍵字搜尋 (標題/名稱)
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 商品類型篩選
    /// </summary>
    public string? ProductType { get; set; }

    /// <summary>
    /// 賣家編號篩選
    /// </summary>
    public int? SellerId { get; set; }

    /// <summary>
    /// 最低價格
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// 最高價格
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// 商品狀態篩選
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 排序方式 (price_asc/price_desc/created_desc/popular)
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
/// 分頁自由市場商品列表 DTO
/// </summary>
public class PagedPlayerMarketProductsDto
{
    /// <summary>
    /// 商品列表
    /// </summary>
    public List<PlayerMarketProductDto> Products { get; set; } = new();

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
/// 自由市場訂單 DTO
/// </summary>
public class PlayerMarketOrderDto
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public int POrderId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public int PProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 賣家編號
    /// </summary>
    public int SellerId { get; set; }

    /// <summary>
    /// 賣家名稱
    /// </summary>
    public string? SellerName { get; set; }

    /// <summary>
    /// 買家編號
    /// </summary>
    public int BuyerId { get; set; }

    /// <summary>
    /// 買家名稱
    /// </summary>
    public string? BuyerName { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public string POrderStatus { get; set; } = string.Empty;

    /// <summary>
    /// 付款狀態
    /// </summary>
    public string PPaymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// 訂單數量
    /// </summary>
    public int POrderQuantity { get; set; }

    /// <summary>
    /// 訂單單價
    /// </summary>
    public decimal POrderUnitPrice { get; set; }

    /// <summary>
    /// 訂單總額
    /// </summary>
    public decimal POrderTotalAmount { get; set; }

    /// <summary>
    /// 訂單創建時間
    /// </summary>
    public DateTime POrderCreatedAt { get; set; }

    /// <summary>
    /// 訂單更新時間
    /// </summary>
    public DateTime POrderUpdatedAt { get; set; }

    /// <summary>
    /// 付款完成時間
    /// </summary>
    public DateTime? PaymentCompletedAt { get; set; }

    /// <summary>
    /// 訂單完成時間
    /// </summary>
    public DateTime? OrderCompletedAt { get; set; }
}

/// <summary>
/// 建立自由市場訂單請求 DTO
/// </summary>
public class CreatePlayerMarketOrderDto
{
    /// <summary>
    /// 商品編號
    /// </summary>
    [Required]
    public int PProductId { get; set; }

    /// <summary>
    /// 訂購數量
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "數量必須大於 0")]
    public int POrderQuantity { get; set; }

    /// <summary>
    /// 買家備註
    /// </summary>
    [StringLength(500)]
    public string? BuyerNotes { get; set; }
}

/// <summary>
/// 自由市場交易頁面 DTO
/// </summary>
public class PlayerMarketTradepageDto
{
    /// <summary>
    /// 交易頁面編號
    /// </summary>
    public int POrderTradepageId { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    public int POrderId { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public int PProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 平台抽成
    /// </summary>
    public decimal POrderPlatformFee { get; set; }

    /// <summary>
    /// 賣家移交時間
    /// </summary>
    public DateTime? SellerTransferredAt { get; set; }

    /// <summary>
    /// 買家接收時間
    /// </summary>
    public DateTime? BuyerReceivedAt { get; set; }

    /// <summary>
    /// 交易完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 交易狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 賣家確認說明
    /// </summary>
    public string? SellerNotes { get; set; }

    /// <summary>
    /// 買家確認說明
    /// </summary>
    public string? BuyerNotes { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 訂單資訊
    /// </summary>
    public PlayerMarketOrderDto? Order { get; set; }

    /// <summary>
    /// 最近訊息列表
    /// </summary>
    public List<PlayerMarketTradeMsgDto> RecentMessages { get; set; } = new();
}

/// <summary>
/// 自由市場交易訊息 DTO
/// </summary>
public class PlayerMarketTradeMsgDto
{
    /// <summary>
    /// 訊息編號
    /// </summary>
    public int TradeMsgId { get; set; }

    /// <summary>
    /// 交易頁面編號
    /// </summary>
    public int POrderTradepageId { get; set; }

    /// <summary>
    /// 訊息發送者角色
    /// </summary>
    public string MsgFrom { get; set; } = string.Empty;

    /// <summary>
    /// 發送者用戶編號
    /// </summary>
    public int SenderUserId { get; set; }

    /// <summary>
    /// 發送者用戶名稱
    /// </summary>
    public string? SenderUserName { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// 訊息類型
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// 訊息狀態
    /// </summary>
    public string MessageStatus { get; set; } = string.Empty;

    /// <summary>
    /// 傳訊時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 讀取時間
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 附件檔名
    /// </summary>
    public string? AttachmentFilename { get; set; }

    /// <summary>
    /// 附件 Base64 編碼
    /// </summary>
    public string? AttachmentBase64 { get; set; }
}

/// <summary>
/// 發送交易訊息請求 DTO
/// </summary>
public class SendTradeMsgDto
{
    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// 訊息類型
    /// </summary>
    public string MessageType { get; set; } = "text";

    /// <summary>
    /// 附件 Base64 編碼 (可選)
    /// </summary>
    public string? AttachmentBase64 { get; set; }

    /// <summary>
    /// 附件檔名 (可選)
    /// </summary>
    [StringLength(255)]
    public string? AttachmentFilename { get; set; }
}

/// <summary>
/// 確認交易操作 DTO
/// </summary>
public class ConfirmTradeActionDto
{
    /// <summary>
    /// 確認說明
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 確認類型 (seller_transferred/buyer_received)
    /// </summary>
    [Required]
    public string ActionType { get; set; } = string.Empty;
}

/// <summary>
/// 自由市場排行榜 DTO
/// </summary>
public class PlayerMarketRankingDto
{
    /// <summary>
    /// 排行榜編號
    /// </summary>
    public int PRankingId { get; set; }

    /// <summary>
    /// 期間類型
    /// </summary>
    public string PPeriodType { get; set; } = string.Empty;

    /// <summary>
    /// 排行榜日期
    /// </summary>
    public DateOnly PRankingDate { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public int PProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? ProductName { get; set; }

    /// <summary>
    /// 排名指標
    /// </summary>
    public string PRankingMetric { get; set; } = string.Empty;

    /// <summary>
    /// 排名位置
    /// </summary>
    public byte PRankingPosition { get; set; }

    /// <summary>
    /// 交易額
    /// </summary>
    public decimal PTradingAmount { get; set; }

    /// <summary>
    /// 交易量
    /// </summary>
    public int PTradingVolume { get; set; }

    /// <summary>
    /// 瀏覽次數
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 收藏次數
    /// </summary>
    public int FavoriteCount { get; set; }

    /// <summary>
    /// 賣家評分
    /// </summary>
    public decimal? SellerRating { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 自由市場排行榜查詢 DTO
/// </summary>
public class PlayerMarketRankingQueryDto
{
    /// <summary>
    /// 期間類型 (daily/weekly/monthly)
    /// </summary>
    public string PeriodType { get; set; } = "daily";

    /// <summary>
    /// 查詢日期
    /// </summary>
    public DateOnly? Date { get; set; }

    /// <summary>
    /// 排名指標 (amount/volume/views)
    /// </summary>
    public string RankingMetric { get; set; } = "amount";

    /// <summary>
    /// 取得筆數
    /// </summary>
    public int Limit { get; set; } = 10;
}