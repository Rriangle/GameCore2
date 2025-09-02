namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 登出回應 DTO
/// </summary>
public class LogoutResponse
{
    /// <summary>
    /// 登出成功訊息
    /// </summary>
    public string Message { get; set; } = "登出成功";
    
    /// <summary>
    /// 登出時間
    /// </summary>
    public DateTime LogoutTime { get; set; } = DateTime.UtcNow;
} 