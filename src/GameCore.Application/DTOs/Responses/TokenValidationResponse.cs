namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// Token 驗證回應 DTO
/// </summary>
public class TokenValidationResponse
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }
    
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// 過期時間
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }
} 