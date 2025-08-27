using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶資料庫操作介面
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據 ID 取得用戶
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// 根據帳號取得用戶
    /// </summary>
    /// <param name="userAccount">用戶帳號</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByAccountAsync(string userAccount);

    /// <summary>
    /// 根據 Email 取得用戶
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// 新增用戶
    /// </summary>
    /// <param name="user">用戶實體</param>
    /// <returns>新增的用戶</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// 更新用戶
    /// </summary>
    /// <param name="user">用戶實體</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// 刪除用戶
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查帳號是否已存在
    /// </summary>
    /// <param name="userAccount">用戶帳號</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByAccountAsync(string userAccount);

    /// <summary>
    /// 檢查 Email 是否已存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// 取得所有用戶 (分頁)
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>用戶列表</returns>
    Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得用戶總數
    /// </summary>
    /// <returns>用戶總數</returns>
    Task<int> GetCountAsync();
}
