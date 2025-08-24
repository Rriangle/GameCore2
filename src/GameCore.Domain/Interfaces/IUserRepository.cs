using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶資料存取介面，定義用戶相關的資料庫操作
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據用戶編號取得用戶資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>用戶資料，不存在則返回 null</returns>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// 根據用戶姓名取得用戶資料
    /// </summary>
    /// <param name="username">用戶姓名</param>
    /// <returns>用戶資料，不存在則返回 null</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// 根據登入帳號取得用戶資料
    /// </summary>
    /// <param name="account">登入帳號</param>
    /// <returns>用戶資料，不存在則返回 null</returns>
    Task<User?> GetByAccountAsync(string account);

    /// <summary>
    /// 檢查用戶姓名是否唯一
    /// </summary>
    /// <param name="username">用戶姓名</param>
    /// <param name="excludeUserId">排除的用戶編號（用於更新時排除自己）</param>
    /// <returns>是否唯一</returns>
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null);

    /// <summary>
    /// 檢查登入帳號是否唯一
    /// </summary>
    /// <param name="account">登入帳號</param>
    /// <param name="excludeUserId">排除的用戶編號（用於更新時排除自己）</param>
    /// <returns>是否唯一</returns>
    Task<bool> IsAccountUniqueAsync(string account, int? excludeUserId = null);

    /// <summary>
    /// 建立新用戶
    /// </summary>
    /// <param name="user">用戶資料</param>
    /// <returns>建立的用戶資料</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// 更新用戶資料
    /// </summary>
    /// <param name="user">用戶資料</param>
    /// <returns>更新的用戶資料</returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// 刪除用戶
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 驗證用戶憑證
    /// </summary>
    /// <param name="account">登入帳號</param>
    /// <param name="passwordHash">密碼雜湊</param>
    /// <returns>驗證成功的用戶資料，失敗則返回 null</returns>
    Task<User?> ValidateCredentialsAsync(string account, string passwordHash);
}
