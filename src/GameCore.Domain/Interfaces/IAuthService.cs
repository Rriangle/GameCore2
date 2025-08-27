using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 認證服務介面 - 處理使用者註冊、登入和認證相關功能
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="userName">使用者姓名</param>
    /// <param name="userAccount">登入帳號</param>
    /// <param name="password">密碼</param>
    /// <returns>註冊結果</returns>
    Task<AuthResult> RegisterAsync(string userName, string userAccount, string password);

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="userAccount">登入帳號</param>
    /// <param name="password">密碼</param>
    /// <returns>登入結果</returns>
    Task<AuthResult> LoginAsync(string userAccount, string password);

    /// <summary>
    /// 取得使用者基本資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>使用者基本資料</returns>
    Task<User?> GetUserProfileAsync(int userId);

    /// <summary>
    /// 驗證使用者權限
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="permission">權限類型</param>
    /// <returns>是否有權限</returns>
    Task<bool> ValidateUserPermissionAsync(int userId, UserPermissionType permission);
}

/// <summary>
/// 認證結果
/// </summary>
public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
    public User? User { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// 使用者權限類型
/// </summary>
public enum UserPermissionType
{
    /// <summary>
    /// 使用者狀態（是否啟用）
    /// </summary>
    UserStatus,
    
    /// <summary>
    /// 購物權限
    /// </summary>
    Shopping,
    
    /// <summary>
    /// 留言權限
    /// </summary>
    Message,
    
    /// <summary>
    /// 銷售權限
    /// </summary>
    Sales
}
