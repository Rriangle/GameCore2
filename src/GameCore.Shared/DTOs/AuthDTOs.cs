using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 使用者註冊請求 DTO
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// 使用者姓名
    /// </summary>
    [Required(ErrorMessage = "使用者姓名為必填欄位")]
    [StringLength(100, ErrorMessage = "使用者姓名不能超過100個字元")]
    public string User_name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填欄位")]
    [StringLength(100, ErrorMessage = "登入帳號不能超過100個字元")]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "密碼為必填欄位")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度必須在6-100個字元之間")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼
    /// </summary>
    [Required(ErrorMessage = "確認密碼為必填欄位")]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不符")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 使用者暱稱
    /// </summary>
    [Required(ErrorMessage = "使用者暱稱為必填欄位")]
    [StringLength(100, ErrorMessage = "使用者暱稱不能超過100個字元")]
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (M/F)
    /// </summary>
    [Required(ErrorMessage = "性別為必填欄位")]
    [RegularExpression("^[MF]$", ErrorMessage = "性別必須為 M 或 F")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號
    /// </summary>
    [Required(ErrorMessage = "身分證字號為必填欄位")]
    [StringLength(20, ErrorMessage = "身分證字號不能超過20個字元")]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話
    /// </summary>
    [Required(ErrorMessage = "聯繫電話為必填欄位")]
    [StringLength(20, ErrorMessage = "聯繫電話不能超過20個字元")]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required(ErrorMessage = "電子郵件為必填欄位")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件格式")]
    [StringLength(100, ErrorMessage = "電子郵件不能超過100個字元")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址為必填欄位")]
    [StringLength(200, ErrorMessage = "地址不能超過200個字元")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日
    /// </summary>
    [Required(ErrorMessage = "出生年月日為必填欄位")]
    public DateTime DateOfBirth { get; set; }
}

/// <summary>
/// 使用者登入請求 DTO
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// 登入帳號
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填欄位")]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "密碼為必填欄位")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 記住我
    /// </summary>
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// OAuth 登入請求 DTO
/// </summary>
public class OAuthLoginRequestDto
{
    /// <summary>
    /// OAuth 提供商 (google, facebook, discord)
    /// </summary>
    [Required(ErrorMessage = "OAuth 提供商為必填欄位")]
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// OAuth 授權碼
    /// </summary>
    [Required(ErrorMessage = "授權碼為必填欄位")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 回調 URL
    /// </summary>
    public string? RedirectUri { get; set; }
}

/// <summary>
/// 登入回應 DTO
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// JWT 存取權杖
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 重新整理權杖
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// 權杖過期時間 (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 使用者基本資訊
    /// </summary>
    public UserProfileDto User { get; set; } = new();
}

/// <summary>
/// 使用者個人資料 DTO
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者姓名
    /// </summary>
    public string User_name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號
    /// </summary>
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者暱稱
    /// </summary>
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別
    /// </summary>
    public string Gender { get; set; } = string.Empty;

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

    /// <summary>
    /// 頭像圖片 Base64
    /// </summary>
    public string? User_Picture_Base64 { get; set; }

    /// <summary>
    /// 使用者權限
    /// </summary>
    public UserRightsDto UserRights { get; set; } = new();

    /// <summary>
    /// 錢包資訊
    /// </summary>
    public UserWalletDto UserWallet { get; set; } = new();
}

/// <summary>
/// 使用者權限 DTO
/// </summary>
public class UserRightsDto
{
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
/// 使用者錢包 DTO
/// </summary>
public class UserWalletDto
{
    /// <summary>
    /// 使用者點數
    /// </summary>
    public int User_Point { get; set; }

    /// <summary>
    /// 優惠券編號
    /// </summary>
    public string? Coupon_Number { get; set; }
}

/// <summary>
/// 更新個人資料請求 DTO
/// </summary>
public class UpdateProfileRequestDto
{
    /// <summary>
    /// 使用者暱稱
    /// </summary>
    [Required(ErrorMessage = "使用者暱稱為必填欄位")]
    [StringLength(100, ErrorMessage = "使用者暱稱不能超過100個字元")]
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (M/F)
    /// </summary>
    [Required(ErrorMessage = "性別為必填欄位")]
    [RegularExpression("^[MF]$", ErrorMessage = "性別必須為 M 或 F")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話
    /// </summary>
    [Required(ErrorMessage = "聯繫電話為必填欄位")]
    [StringLength(20, ErrorMessage = "聯繫電話不能超過20個字元")]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required(ErrorMessage = "電子郵件為必填欄位")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件格式")]
    [StringLength(100, ErrorMessage = "電子郵件不能超過100個字元")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址為必填欄位")]
    [StringLength(200, ErrorMessage = "地址不能超過200個字元")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日
    /// </summary>
    [Required(ErrorMessage = "出生年月日為必填欄位")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 使用者自介
    /// </summary>
    [StringLength(200, ErrorMessage = "使用者自介不能超過200個字元")]
    public string? User_Introduce { get; set; }
}

/// <summary>
/// 修改密碼請求 DTO
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// 目前密碼
    /// </summary>
    [Required(ErrorMessage = "目前密碼為必填欄位")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密碼
    /// </summary>
    [Required(ErrorMessage = "新密碼為必填欄位")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "新密碼長度必須在6-100個字元之間")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 確認新密碼
    /// </summary>
    [Required(ErrorMessage = "確認新密碼為必填欄位")]
    [Compare("NewPassword", ErrorMessage = "新密碼與確認新密碼不符")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}