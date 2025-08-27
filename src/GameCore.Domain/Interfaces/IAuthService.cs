using GameCore.Api.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 認證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用戶註冊
    /// </summary>
    /// <param name="request">註冊請求</param>
    /// <returns>認證結果</returns>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// 用戶登入
    /// </summary>
    /// <param name="request">登入請求</param>
    /// <returns>認證結果</returns>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// 取得用戶資料
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶資料</returns>
    Task<UserProfileDto?> GetUserProfileAsync(int userId);

    /// <summary>
    /// 取得完整用戶資料
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>完整用戶資料</returns>
    Task<CompleteUserProfileDto?> GetCompleteUserProfileAsync(int userId);

    /// <summary>
    /// 驗證 JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>是否有效</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// 從 Token 取得用戶 ID
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>用戶 ID</returns>
    Task<int?> GetUserIdFromTokenAsync(string token);
}
