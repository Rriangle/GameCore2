using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶資料庫操作介面
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據ID取得用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// 根據帳號取得用戶
    /// </summary>
    /// <param name="account">用戶帳號</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByAccountAsync(string account);

    /// <summary>
    /// 根據電子郵件取得用戶
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// 根據手機號碼取得用戶
    /// </summary>
    /// <param name="phone">手機號碼</param>
    /// <returns>用戶實體</returns>
    Task<User?> GetByPhoneAsync(string phone);

    /// <summary>
    /// 新增用戶
    /// </summary>
    /// <param name="user">用戶實體</param>
    /// <returns>新增的用戶實體</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// 更新用戶
    /// </summary>
    /// <param name="user">用戶實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// 刪除用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 檢查帳號是否存在
    /// </summary>
    /// <param name="account">用戶帳號</param>
    /// <returns>是否存在</returns>
    Task<bool> AccountExistsAsync(string account);

    /// <summary>
    /// 檢查電子郵件是否存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否存在</returns>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// 檢查手機號碼是否存在
    /// </summary>
    /// <param name="phone">手機號碼</param>
    /// <returns>是否存在</returns>
    Task<bool> PhoneExistsAsync(string phone);

    /// <summary>
    /// 取得所有用戶
    /// </summary>
    /// <returns>用戶列表</returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// 分頁取得用戶
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>用戶列表</returns>
    Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize);

    /// <summary>
    /// 取得用戶總數
    /// </summary>
    /// <returns>用戶總數</returns>
    Task<int> GetCountAsync();
}
