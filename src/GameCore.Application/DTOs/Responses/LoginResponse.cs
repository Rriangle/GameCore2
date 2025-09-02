namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 登入回應 DTO
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 暱稱
    /// </summary>
    public string Nickname { get; set; } = string.Empty;
    
    /// <summary>
    /// 存取 Token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 重新整理 Token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Token 過期時間
    /// </summary>
    public DateTime ExpiresAt { get; set; }
} 