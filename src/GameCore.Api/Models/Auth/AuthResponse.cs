namespace GameCore.Api.Models.Auth;

/// <summary>
/// 認證回應模型
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// JWT 存取權杖
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 重新整理權杖
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 權杖過期時間
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// 用戶資料
    /// </summary>
    public UserProfile? User { get; set; }
}

/// <summary>
/// 用戶資料模型
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 使用者姓名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號
    /// </summary>
    public string UserAccount { get; set; } = string.Empty;

    /// <summary>
    /// 使用者暱稱
    /// </summary>
    public string? UserNickName { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 頭像圖片 (Base64)
    /// </summary>
    public string? AvatarImage { get; set; }

    /// <summary>
    /// 使用者自介
    /// </summary>
    public string? UserIntroduce { get; set; }

    /// <summary>
    /// 點數餘額
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 權限資訊
    /// </summary>
    public UserPermissions? Permissions { get; set; }
}

/// <summary>
/// 用戶權限模型
/// </summary>
public class UserPermissions
{
    /// <summary>
    /// 使用者狀態
    /// </summary>
    public bool UserStatus { get; set; }

    /// <summary>
    /// 購物權限
    /// </summary>
    public bool ShoppingPermission { get; set; }

    /// <summary>
    /// 留言權限
    /// </summary>
    public bool MessagePermission { get; set; }

    /// <summary>
    /// 銷售權限
    /// </summary>
    public bool SalesAuthority { get; set; }
} 