using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶權限資料庫操作介面
/// </summary>
public interface IUserRightsRepository
{
    /// <summary>
    /// 根據用戶ID取得用戶權限
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶權限實體</returns>
    Task<UserRights?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增用戶權限
    /// </summary>
    /// <param name="userRights">用戶權限實體</param>
    /// <returns>新增的用戶權限實體</returns>
    Task<UserRights> AddAsync(UserRights userRights);

    /// <summary>
    /// 更新用戶權限
    /// </summary>
    /// <param name="userRights">用戶權限實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(UserRights userRights);

    /// <summary>
    /// 刪除用戶權限
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 根據權限狀態取得用戶列表
    /// </summary>
    /// <param name="userStatus">用戶狀態</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetByStatusAsync(bool userStatus);

    /// <summary>
    /// 根據購物權限取得用戶列表
    /// </summary>
    /// <param name="shoppingPermission">購物權限</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetByShoppingPermissionAsync(bool shoppingPermission);

    /// <summary>
    /// 根據留言權限取得用戶列表
    /// </summary>
    /// <param name="messagePermission">留言權限</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetByMessagePermissionAsync(bool messagePermission);

    /// <summary>
    /// 根據銷售權限取得用戶列表
    /// </summary>
    /// <param name="salesAuthority">銷售權限</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetBySalesAuthorityAsync(bool salesAuthority);

    /// <summary>
    /// 取得所有用戶權限
    /// </summary>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetAllAsync();

    /// <summary>
    /// 分頁取得用戶權限
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>用戶權限列表</returns>
    Task<IEnumerable<UserRights>> GetPagedAsync(int page, int pageSize);

    /// <summary>
    /// 取得用戶權限總數
    /// </summary>
    /// <returns>用戶權限總數</returns>
    Task<int> GetCountAsync();
} 