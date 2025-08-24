using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者權限資料存取介面
/// </summary>
public interface IUserRightsRepository
{
    /// <summary>
    /// 根據使用者ID取得使用者權限
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>使用者權限，如果不存在則返回null</returns>
    Task<UserRights?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 創建使用者權限
    /// </summary>
    /// <param name="userRights">使用者權限</param>
    /// <returns>創建的使用者權限</returns>
    Task<UserRights> CreateAsync(UserRights userRights);

    /// <summary>
    /// 更新使用者權限
    /// </summary>
    /// <param name="userRights">使用者權限</param>
    /// <returns>更新的使用者權限</returns>
    Task<UserRights> UpdateAsync(UserRights userRights);

    /// <summary>
    /// 刪除使用者權限
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 批量取得多個使用者的權限
    /// </summary>
    /// <param name="userIds">使用者ID列表</param>
    /// <returns>使用者權限列表</returns>
    Task<IEnumerable<UserRights>> GetByUserIdsAsync(IEnumerable<int> userIds);

    /// <summary>
    /// 更新使用者狀態
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="isActive">是否啟用</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateUserStatusAsync(int userId, bool isActive);

    /// <summary>
    /// 更新購物權限
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="canShopping">是否可購物</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateShoppingPermissionAsync(int userId, bool canShopping);

    /// <summary>
    /// 更新留言權限
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="canMessage">是否可留言</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateMessagePermissionAsync(int userId, bool canMessage);

    /// <summary>
    /// 更新銷售權限
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="canSales">是否可銷售</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateSalesAuthorityAsync(int userId, bool canSales);
}