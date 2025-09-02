namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 登入請求 DTO
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 使用者名稱或電子郵件
    /// </summary>
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 記住我
    /// </summary>
    public bool RememberMe { get; set; }
} 