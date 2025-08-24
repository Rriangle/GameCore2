using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 身份驗證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="request">註冊請求資料</param>
    /// <returns>註冊結果</returns>
    Task<(bool Success, string Message, LoginResponseDto? Response)> RegisterAsync(RegisterRequestDto request);

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="request">登入請求資料</param>
    /// <returns>登入結果</returns>
    Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// OAuth 登入
    /// </summary>
    /// <param name="request">OAuth 登入請求資料</param>
    /// <returns>登入結果</returns>
    Task<(bool Success, string Message, LoginResponseDto? Response)> OAuthLoginAsync(OAuthLoginRequestDto request);

    /// <summary>
    /// 重新整理存取權杖
    /// </summary>
    /// <param name="refreshToken">重新整理權杖</param>
    /// <returns>新的存取權杖</returns>
    Task<(bool Success, string Message, LoginResponseDto? Response)> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <returns>登出結果</returns>
    Task<bool> LogoutAsync(int userId);

    /// <summary>
    /// 驗證密碼
    /// </summary>
    /// <param name="password">明文密碼</param>
    /// <param name="hashedPassword">雜湊密碼</param>
    /// <returns>是否符合</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// 產生密碼雜湊
    /// </summary>
    /// <param name="password">明文密碼</param>
    /// <returns>雜湊密碼</returns>
    string HashPassword(string password);
}
