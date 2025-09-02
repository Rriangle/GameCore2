namespace GameCore.Application.DTOs.Responses;

/// <summary>
/// 註冊回應 DTO
/// </summary>
public class RegisterResponse
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
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 註冊成功訊息
    /// </summary>
    public string Message { get; set; } = "註冊成功";
} 