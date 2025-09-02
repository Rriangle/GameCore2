using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 認證服務介面
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用戶註冊
    /// </summary>
    /// <param name="registerDto">註冊請求</param>
    /// <returns>註冊結果</returns>
    Task<ApiResponseDto<UserInfoDto>> RegisterAsync(UserRegisterDto registerDto);

    /// <summary>
    /// 用戶登入
    /// </summary>
    /// <param name="loginDto">登入請求</param>
    /// <returns>登入結果</returns>
    Task<ApiResponseDto<LoginResponseDto>> LoginAsync(UserLoginDto loginDto);

    /// <summary>
    /// OAuth 登入
    /// </summary>
    /// <param name="oauthDto">OAuth 登入請求</param>
    /// <returns>登入結果</returns>
    Task<ApiResponseDto<LoginResponseDto>> OAuthLoginAsync(OAuthLoginDto oauthDto);

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的登入結果</returns>
    Task<ApiResponseDto<LoginResponseDto>> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 變更密碼
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="changePasswordDto">密碼變更請求</param>
    /// <returns>變更結果</returns>
    Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

    /// <summary>
    /// 忘記密碼
    /// </summary>
    /// <param name="forgotPasswordDto">忘記密碼請求</param>
    /// <returns>處理結果</returns>
    Task<ApiResponseDto<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

    /// <summary>
    /// 重設密碼
    /// </summary>
    /// <param name="resetPasswordDto">重設密碼請求</param>
    /// <returns>重設結果</returns>
    Task<ApiResponseDto<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
} 