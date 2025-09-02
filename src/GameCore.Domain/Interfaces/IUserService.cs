using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶管理服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 取得用戶完整資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶資訊</returns>
    Task<ApiResponseDto<UserInfoDto>> GetUserInfoAsync(int userId);

    /// <summary>
    /// 更新用戶個資
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="updateDto">更新資料</param>
    /// <returns>更新結果</returns>
    Task<ApiResponseDto<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);

    /// <summary>
    /// 取得用戶權限
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶權限</returns>
    Task<ApiResponseDto<UserRightsDto>> GetUserRightsAsync(int userId);

    /// <summary>
    /// 取得用戶錢包
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶錢包</returns>
    Task<ApiResponseDto<UserWalletDto>> GetUserWalletAsync(int userId);

    /// <summary>
    /// 檢查用戶權限
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="permission">權限類型</param>
    /// <returns>是否有權限</returns>
    Task<bool> CheckUserPermissionAsync(int userId, string permission);

    /// <summary>
    /// 搜尋用戶
    /// </summary>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>搜尋結果</returns>
    Task<ApiResponseDto<List<UserInfoDto>>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20);
} 