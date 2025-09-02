namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 重新整理 Token 回應 DTO
/// </summary>
public class RefreshTokenResponse
{
    /// <summary>
    /// 新的存取 Token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 新的重新整理 Token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Token 過期時間
    /// </summary>
    public DateTime ExpiresAt { get; set; }
} 