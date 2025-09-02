using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 用戶註冊請求 DTO
/// </summary>
public class UserRegisterDto
{
    [Required(ErrorMessage = "用戶名為必填項")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "用戶名長度必須在 2-50 個字符之間")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "帳號為必填項")]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "帳號長度必須在 4-50 個字符之間")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "帳號只能包含字母、數字和下劃線")]
    public string UserAccount { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼為必填項")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度必須在 6-100 個字符之間")]
    public string UserPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "確認密碼為必填項")]
    [Compare("UserPassword", ErrorMessage = "兩次輸入的密碼不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "電子郵件為必填項")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "手機號碼為必填項")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "請輸入有效的手機號碼格式")]
    public string Cellphone { get; set; } = string.Empty;
}

/// <summary>
/// 用戶登入請求 DTO
/// </summary>
public class UserLoginDto
{
    [Required(ErrorMessage = "帳號為必填項")]
    public string UserAccount { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼為必填項")]
    public string UserPassword { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// OAuth 登入請求 DTO
/// </summary>
public class OAuthLoginDto
{
    [Required(ErrorMessage = "OAuth 提供者為必填項")]
    public string Provider { get; set; } = string.Empty; // "google", "facebook", "discord"

    [Required(ErrorMessage = "OAuth 令牌為必填項")]
    public string AccessToken { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
}

/// <summary>
/// 登入回應 DTO
/// </summary>
public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserInfoDto? User { get; set; }
}

/// <summary>
/// 用戶資訊 DTO
/// </summary>
public class UserInfoDto
{
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAccount { get; set; } = string.Empty;
    public string? NickName { get; set; }
    public string? Email { get; set; }
    public string? Cellphone { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRightsDto Rights { get; set; } = new();
    public UserWalletDto Wallet { get; set; } = new();
}

/// <summary>
/// 用戶權限 DTO
/// </summary>
public class UserRightsDto
{
    public bool UserStatus { get; set; } = true;
    public bool ShoppingPermission { get; set; } = true;
    public bool MessagePermission { get; set; } = true;
    public bool SalesAuthority { get; set; } = false;
}

/// <summary>
/// 用戶錢包 DTO
/// </summary>
public class UserWalletDto
{
    public int UserPoint { get; set; } = 0;
    public string? CouponNumber { get; set; }
}

/// <summary>
/// 用戶個資更新 DTO
/// </summary>
public class UpdateUserProfileDto
{
    [StringLength(50, MinimumLength = 2, ErrorMessage = "暱稱長度必須在 2-50 個字符之間")]
    public string? NickName { get; set; }

    [RegularExpression(@"^[MF]$", ErrorMessage = "性別必須是 M 或 F")]
    public string? Gender { get; set; }

    [RegularExpression(@"^[A-Z][12]\d{8}$", ErrorMessage = "請輸入有效的身分證字號格式")]
    public string? IdNumber { get; set; }

    [RegularExpression(@"^09\d{8}$", ErrorMessage = "請輸入有效的手機號碼格式")]
    public string? Cellphone { get; set; }

    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = "地址長度不能超過 200 個字符")]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(200, ErrorMessage = "自我介紹長度不能超過 200 個字符")]
    public string? UserIntroduce { get; set; }
}

/// <summary>
/// 密碼變更 DTO
/// </summary>
public class ChangePasswordDto
{
    [Required(ErrorMessage = "當前密碼為必填項")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密碼為必填項")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "新密碼長度必須在 6-100 個字符之間")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "確認新密碼為必填項")]
    [Compare("NewPassword", ErrorMessage = "兩次輸入的新密碼不一致")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 忘記密碼 DTO
/// </summary>
public class ForgotPasswordDto
{
    [Required(ErrorMessage = "電子郵件為必填項")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// 重設密碼 DTO
/// </summary>
public class ResetPasswordDto
{
    [Required(ErrorMessage = "重設令牌為必填項")]
    public string ResetToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密碼為必填項")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "新密碼長度必須在 6-100 個字符之間")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "確認新密碼為必填項")]
    [Compare("NewPassword", ErrorMessage = "兩次輸入的新密碼不一致")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

/// <summary>
/// 刷新令牌 DTO
/// </summary>
public class RefreshTokenDto
{
    [Required(ErrorMessage = "刷新令牌為必填項")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// 通用回應 DTO
/// </summary>
public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

/// <summary>
/// 分頁請求 DTO
/// </summary>
public class PaginationDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "asc"; // "asc" or "desc"
}

/// <summary>
/// 分頁回應 DTO
/// </summary>
public class PaginatedResponseDto<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
