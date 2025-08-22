using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.DTOs;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "用戶名為必填項目")]
    [UsernameValidation]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "電子郵件為必填項目")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [StringLength(100, ErrorMessage = "電子郵件長度不能超過 100 個字元")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼為必填項目")]
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "確認密碼為必填項目")]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不符")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    [Required(ErrorMessage = "用戶名為必填項目")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密碼為必填項目")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public UserProfileDto? User { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class UserProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class WalletBalanceDto
{
    public int UserId { get; set; }
    public decimal Balance { get; set; }
    public DateTime UpdatedAt { get; set; }
}
