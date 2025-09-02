using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.Models.Auth;

/// <summary>
/// 用戶登入請求模型
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 登入帳號 (必填)
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填項目")]
    public string UserAccount { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (必填)
    /// </summary>
    [Required(ErrorMessage = "密碼為必填項目")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 記住我 (可選)
    /// </summary>
    public bool RememberMe { get; set; } = false;
} 