namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 重新整理 Token 請求 DTO
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// 重新整理 Token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
} 