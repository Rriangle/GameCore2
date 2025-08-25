using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.DTOs;

/// <summary>
/// 用戶介紹更新 DTO
/// </summary>
public class UpdateUserIntroduceDto
{
    /// <summary>
    /// 使用者暱稱
    /// </summary>
    [Required(ErrorMessage = "使用者暱稱為必填項目")]
    [StringLength(100, ErrorMessage = "使用者暱稱長度不能超過 100 個字元")]
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別
    /// </summary>
    [Required(ErrorMessage = "性別為必填項目")]
    [StringLength(10, ErrorMessage = "性別長度不能超過 10 個字元")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號
    /// </summary>
    [Required(ErrorMessage = "身分證字號為必填項目")]
    [StringLength(20, ErrorMessage = "身分證字號長度不能超過 20 個字元")]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話
    /// </summary>
    [Required(ErrorMessage = "聯繫電話為必填項目")]
    [StringLength(20, ErrorMessage = "聯繫電話長度不能超過 20 個字元")]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required(ErrorMessage = "電子郵件為必填項目")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [StringLength(100, ErrorMessage = "電子郵件長度不能超過 100 個字元")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址為必填項目")]
    [StringLength(500, ErrorMessage = "地址長度不能超過 500 個字元")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日
    /// </summary>
    [Required(ErrorMessage = "出生年月日為必填項目")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 使用者自介
    /// </summary>
    [StringLength(200, ErrorMessage = "使用者自介長度不能超過 200 個字元")]
    public string? User_Introduce { get; set; }
}

/// <summary>
/// 用戶介紹回應 DTO
/// </summary>
public class UserIntroduceDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者暱稱
    /// </summary>
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別
    /// </summary>
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號
    /// </summary>
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話
    /// </summary>
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 創建帳號日期
    /// </summary>
    public DateTime Create_Account { get; set; }

    /// <summary>
    /// 使用者自介
    /// </summary>
    public string? User_Introduce { get; set; }
}

/// <summary>
/// 用戶權限 DTO
/// </summary>
public class UserRightsDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者狀態
    /// </summary>
    public bool User_Status { get; set; }

    /// <summary>
    /// 購物權限
    /// </summary>
    public bool ShoppingPermission { get; set; }

    /// <summary>
    /// 留言權限
    /// </summary>
    public bool MessagePermission { get; set; }

    /// <summary>
    /// 銷售權限
    /// </summary>
    public bool SalesAuthority { get; set; }
}

/// <summary>
/// 銷售權限申請 DTO
/// </summary>
public class SalesPermissionRequestDto
{
    /// <summary>
    /// 銀行代號
    /// </summary>
    [Required(ErrorMessage = "銀行代號為必填項目")]
    public int BankCode { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    [Required(ErrorMessage = "銀行帳號為必填項目")]
    [StringLength(50, ErrorMessage = "銀行帳號長度不能超過 50 個字元")]
    public string BankAccountNumber { get; set; } = string.Empty;
}

/// <summary>
/// 銷售權限申請回應 DTO
/// </summary>
public class SalesPermissionResponseDto
{
    /// <summary>
    /// 申請 ID
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 申請狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 申請時間
    /// </summary>
    public DateTime AppliedAt { get; set; }

    /// <summary>
    /// 審核時間
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// 審核備註
    /// </summary>
    public string? ReviewNotes { get; set; }
}

/// <summary>
/// 銷售錢包 DTO
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
    public decimal UserSales_Wallet { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 完整用戶資料 DTO
/// </summary>
public class CompleteUserProfileDto
{
    /// <summary>
    /// 基本用戶資料
    /// </summary>
    public UserProfileDto BasicInfo { get; set; } = null!;

    /// <summary>
    /// 用戶介紹
    /// </summary>
    public UserIntroduceDto? Introduce { get; set; }

    /// <summary>
    /// 用戶權限
    /// </summary>
    public UserRightsDto? Rights { get; set; }

    /// <summary>
    /// 錢包資訊
    /// </summary>
    public WalletBalanceDto? Wallet { get; set; }

    /// <summary>
    /// 銷售權限申請
    /// </summary>
    public SalesPermissionResponseDto? SalesPermission { get; set; }

    /// <summary>
    /// 銷售錢包
    /// </summary>
    public SalesWalletDto? SalesWallet { get; set; }
}