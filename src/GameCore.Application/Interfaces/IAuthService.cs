using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;

namespace GameCore.Application.Interfaces;

/// <summary>
/// 認證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="request">登入請求</param>
    /// <returns>登入結果</returns>
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    
    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="request">註冊請求</param>
    /// <returns>註冊結果</returns>
    Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request);
    
    /// <summary>
    /// 使用者登出
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>登出結果</returns>
    Task<Result<LogoutResponse>> LogoutAsync(int userId);
    
    /// <summary>
    /// 重新整理 Token
    /// </summary>
    /// <param name="request">重新整理請求</param>
    /// <returns>重新整理結果</returns>
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    
    /// <summary>
    /// 驗證 Token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>驗證結果</returns>
    Task<Result<TokenValidationResponse>> ValidateTokenAsync(string token);
} 