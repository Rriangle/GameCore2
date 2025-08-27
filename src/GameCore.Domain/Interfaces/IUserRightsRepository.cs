using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶權限資料庫操作介面
/// </summary>
public interface IUserRightsRepository
{
    /// <summary>
    /// 根據用戶 ID 取得用戶權限
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶權限實體</returns>
    Task<UserRights?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增用戶權限
    /// </summary>
    /// <param name="userRights">用戶權限實體</param>
    /// <returns>新增的用戶權限</returns>
    Task<UserRights> AddAsync(UserRights userRights);

    /// <summary>
    /// 更新用戶權限
    /// </summary>
    /// <param name="userRights">用戶權限實體</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UserRights userRights);

    /// <summary>
    /// 刪除用戶權限
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查用戶是否有特定權限
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="permission">權限類型</param>
    /// <returns>是否有權限</returns>
    Task<bool> HasPermissionAsync(int userId, string permission);

    /// <summary>
    /// 取得所有用戶權限 (分頁)
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// 根據權限篩選用戶
    /// </summary>
    /// <param name="permission">權限類型</param>
    /// <param name="hasPermission">是否有權限</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetByPermissionAsync(string permission, bool hasPermission = true);
}