namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 註冊請求 DTO
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 確認密碼
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// 暱稱
    /// </summary>
    public string Nickname { get; set; } = string.Empty;
} 