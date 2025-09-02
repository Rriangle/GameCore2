using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces;

/// <summary>
/// 用戶管理服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 取得用戶完整資訊
    /// </summary>
    Task<ApiResponseDto<UserInfoDto>> GetUserInfoAsync(int userId);

    /// <summary>
    /// 更新用戶個資
    /// </summary>
    Task<ApiResponseDto<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);

    /// <summary>
    /// 變更密碼
    /// </summary>
    Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

    /// <summary>
    /// 取得用戶權限
    /// </summary>
    Task<ApiResponseDto<UserRightsDto>> GetUserRightsAsync(int userId);

    /// <summary>
    /// 檢查用戶權限
    /// </summary>
    Task<bool> CheckUserPermissionAsync(int userId, string permission);
} 