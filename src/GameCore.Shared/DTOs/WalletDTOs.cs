using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 錢包餘額回應 DTO
/// </summary>
public class WalletBalanceDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 目前點數餘額
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// 優惠券編號
    /// </summary>
    public string? Coupon_Number { get; set; }
}

/// <summary>
/// 點數交易明細 DTO
/// </summary>
public class PointTransactionDto
{
    /// <summary>
    /// 交易編號 (來源表 + ID)
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 交易時間
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 交易類型 (signin, minigame, pet_color, adjustment)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 點數變化量 (正數為增加，負數為扣除)
    /// </summary>
    public int Delta { get; set; }

    /// <summary>
    /// 交易後餘額 (可選)
    /// </summary>
    public int? BalanceAfter { get; set; }

    /// <summary>
    /// 交易詳細資訊 (JSON 格式)
    /// </summary>
    public string? Meta { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// 點數交易明細查詢請求 DTO
/// </summary>
public class PointTransactionQueryDto
{
    /// <summary>
    /// 查詢開始日期
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// 查詢結束日期
    /// </summary>
    public DateTime? To { get; set; }

    /// <summary>
    /// 交易類型篩選
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 頁碼 (從 1 開始)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 分頁交易明細回應 DTO
/// </summary>
public class PagedPointTransactionsDto
{
    /// <summary>
    /// 交易明細列表
    /// </summary>
    public List<PointTransactionDto> Transactions { get; set; } = new();

    /// <summary>
    /// 總筆數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 目前頁碼
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;
}

/// <summary>
/// 管理者調整點數請求 DTO
/// </summary>
public class AdminAdjustPointsDto
{
    /// <summary>
    /// 目標使用者編號
    /// </summary>
    [Required(ErrorMessage = "使用者編號為必填欄位")]
    public int UserId { get; set; }

    /// <summary>
    /// 點數變化量 (正數為增加，負數為扣除)
    /// </summary>
    [Required(ErrorMessage = "點數變化量為必填欄位")]
    public int Delta { get; set; }

    /// <summary>
    /// 調整原因
    /// </summary>
    [Required(ErrorMessage = "調整原因為必填欄位")]
    [StringLength(500, ErrorMessage = "調整原因不能超過500個字元")]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 銷售資料申請請求 DTO
/// </summary>
public class SalesProfileApplicationDto
{
    /// <summary>
    /// 銀行代號
    /// </summary>
    [Required(ErrorMessage = "銀行代號為必填欄位")]
    [Range(1, 999, ErrorMessage = "銀行代號必須為1-999之間的數字")]
    public int BankCode { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    [Required(ErrorMessage = "銀行帳號為必填欄位")]
    [StringLength(30, ErrorMessage = "銀行帳號不能超過30個字元")]
    [RegularExpression(@"^[0-9]{10,30}$", ErrorMessage = "銀行帳號必須為10-30位數字")]
    public string BankAccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶封面照片 (Base64)
    /// </summary>
    public string? AccountCoverPhotoBase64 { get; set; }
}

/// <summary>
/// 銷售資料回應 DTO
/// </summary>
public class SalesProfileDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 銀行代號
    /// </summary>
    public int? BankCode { get; set; }

    /// <summary>
    /// 銀行帳號 (遮蔽顯示)
    /// </summary>
    public string? BankAccountNumberMasked { get; set; }

    /// <summary>
    /// 帳戶封面照片 (Base64)
    /// </summary>
    public string? AccountCoverPhotoBase64 { get; set; }

    /// <summary>
    /// 銷售權限狀態
    /// </summary>
    public bool HasSalesAuthority { get; set; }

    /// <summary>
    /// 申請狀態 (pending, approved, rejected)
    /// </summary>
    public string ApplicationStatus { get; set; } = "none";

    /// <summary>
    /// 申請時間
    /// </summary>
    public DateTime? ApplicationTime { get; set; }
}

/// <summary>
/// 銷售錢包回應 DTO
/// </summary>
public class SalesWalletDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 銷售錢包餘額
    /// </summary>
    public int UserSales_Wallet { get; set; }

    /// <summary>
    /// 是否有銷售權限
    /// </summary>
    public bool HasSalesAuthority { get; set; }
}

/// <summary>
/// 銷售錢包交易請求 DTO
/// </summary>
public class SalesWalletTransactionDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    [Required(ErrorMessage = "使用者編號為必填欄位")]
    public int UserId { get; set; }

    /// <summary>
    /// 交易金額 (正數為收入，負數為支出)
    /// </summary>
    [Required(ErrorMessage = "交易金額為必填欄位")]
    public int Amount { get; set; }

    /// <summary>
    /// 交易類型 (market_sale, platform_fee, withdrawal, adjustment)
    /// </summary>
    [Required(ErrorMessage = "交易類型為必填欄位")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 交易備註
    /// </summary>
    [StringLength(200, ErrorMessage = "交易備註不能超過200個字元")]
    public string? Note { get; set; }

    /// <summary>
    /// 關聯訂單編號 (可選)
    /// </summary>
    public string? RelatedOrderId { get; set; }
}

/// <summary>
/// 銀行代號字典 DTO
/// </summary>
public class BankCodeDto
{
    /// <summary>
    /// 銀行代號
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 銀行名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 銀行英文簡稱
    /// </summary>
    public string ShortName { get; set; } = string.Empty;
}