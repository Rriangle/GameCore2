using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.DTOs;

/// <summary>
/// 用戶註冊請求 DTO
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// 使用者姓名 (必填)
    /// </summary>
    [Required(ErrorMessage = "使用者姓名為必填項目")]
    [StringLength(100, ErrorMessage = "使用者姓名長度不能超過 100 個字元")]
    public string User_name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填項目")]
    [StringLength(100, ErrorMessage = "登入帳號長度不能超過 100 個字元")]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (必填)
    /// </summary>
    [Required(ErrorMessage = "密碼為必填項目")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "密碼長度必須在 8-255 個字元之間")]
    public string User_Password { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼 (必填)
    /// </summary>
    [Required(ErrorMessage = "確認密碼為必填項目")]
    [Compare("User_Password", ErrorMessage = "密碼與確認密碼不符")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "電子郵件為必填項目")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [StringLength(100, ErrorMessage = "電子郵件長度不能超過 100 個字元")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// 用戶登入請求 DTO
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// 登入帳號 (必填)
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填項目")]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (必填)
    /// </summary>
    [Required(ErrorMessage = "密碼為必填項目")]
    public string User_Password { get; set; } = string.Empty;
}

/// <summary>
/// 認證回應 DTO
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// JWT Token
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 回應訊息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 用戶資料
    /// </summary>
    public UserProfileDto? User { get; set; }

    /// <summary>
    /// 時間戳記
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 用戶資料 DTO
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
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 點數餘額
    /// </summary>
    public int User_Point { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 是否驗證 Email
    /// </summary>
    public bool IsEmailVerified { get; set; }
}

/// <summary>
/// 錢包餘額 DTO
/// </summary>
public class WalletBalanceDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int User_Id { get; set; }

    /// <summary>
    /// 點數餘額
    /// </summary>
    public int User_Point { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
