using GameCore.Api.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 更新用戶介紹
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="request">更新請求</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateUserIntroduceAsync(int userId, UpdateUserIntroduceDto request);

    /// <summary>
    /// 取得用戶介紹
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶介紹</returns>
    Task<UserIntroduceDto?> GetUserIntroduceAsync(int userId);

    /// <summary>
    /// 取得用戶權限
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶權限</returns>
    Task<UserRightsDto?> GetUserRightsAsync(int userId);

    /// <summary>
    /// 更新用戶權限
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="rights">權限資料</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateUserRightsAsync(int userId, UserRightsDto rights);

    /// <summary>
    /// 申請銷售權限
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="request">申請請求</param>
    /// <returns>申請結果</returns>
    Task<SalesPermissionResponseDto> ApplySalesPermissionAsync(int userId, SalesPermissionRequestDto request);

    /// <summary>
    /// 取得銷售權限申請狀態
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>申請狀態</returns>
    Task<SalesPermissionResponseDto?> GetSalesPermissionStatusAsync(int userId);

    /// <summary>
    /// 取得銷售錢包
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>銷售錢包</returns>
    Task<SalesWalletDto?> GetSalesWalletAsync(int userId);

    /// <summary>
    /// 更新用戶頭像
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="imageData">圖片資料</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateUserAvatarAsync(int userId, byte[] imageData);

    /// <summary>
    /// 取得用戶頭像
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>頭像資料</returns>
    Task<byte[]?> GetUserAvatarAsync(int userId);
}