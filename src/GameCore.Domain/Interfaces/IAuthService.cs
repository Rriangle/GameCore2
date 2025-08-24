namespace GameCore.Domain.Interfaces;

/// <summary>
/// 認證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用戶註冊 (簡化版)
    /// </summary>
    Task<AuthResult> RegisterAsync(string username, string email, string password);
    
    /// <summary>
    /// 用戶註冊 (完整版)
    /// </summary>
    Task<AuthResult> RegisterAsync(string username, string email, string password, string fullName, string nickname, string gender, string idNumber, string cellphone, string address, DateTime dateOfBirth);
    
    /// <summary>
    /// 用戶登入
    /// </summary>
    Task<AuthResult> LoginAsync(string username, string password);
    
    /// <summary>
    /// 取得用戶個人資訊
    /// </summary>
    Task<UserInfo?> GetUserInfoAsync(int userId);
}

/// <summary>
/// 認證結果
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public UserInfo? User { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 用戶資訊 DTO
/// </summary>
public class UserInfo
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool CanShop { get; set; }
    public bool CanMessage { get; set; }
    public bool CanSell { get; set; }
    public bool IsActive { get; set; }
}
