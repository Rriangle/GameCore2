using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者基本資料存取介面
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據使用者ID取得使用者資料（包含所有相關資料）
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>使用者資料，如果不存在則返回null</returns>
    Task<User?> GetByIdAsync(int userId);

    /// <summary>
    /// 根據使用者姓名取得使用者資料
    /// </summary>
    /// <param name="userName">使用者姓名</param>
    /// <returns>使用者資料，如果不存在則返回null</returns>
    Task<User?> GetByUserNameAsync(string userName);

    /// <summary>
    /// 根據登入帳號取得使用者資料
    /// </summary>
    /// <param name="userAccount">登入帳號</param>
    /// <returns>使用者資料，如果不存在則返回null</returns>
    Task<User?> GetByUserAccountAsync(string userAccount);

    /// <summary>
    /// 創建使用者（包含預設的權限和錢包）
    /// </summary>
    /// <param name="user">使用者資料</param>
    /// <returns>創建的使用者資料</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// 更新使用者資料
    /// </summary>
    /// <param name="user">使用者資料</param>
    /// <returns>更新的使用者資料</returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// 刪除使用者（軟刪除或硬刪除）
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查使用者姓名是否已存在
    /// </summary>
    /// <param name="userName">使用者姓名</param>
    /// <param name="excludeUserId">排除的使用者ID（用於更新時檢查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByUserNameAsync(string userName, int? excludeUserId = null);

    /// <summary>
    /// 檢查登入帳號是否已存在
    /// </summary>
    /// <param name="userAccount">登入帳號</param>
    /// <param name="excludeUserId">排除的使用者ID（用於更新時檢查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByUserAccountAsync(string userAccount, int? excludeUserId = null);

    /// <summary>
    /// 取得使用者列表（分頁）
    /// </summary>
    /// <param name="skip">跳過筆數</param>
    /// <param name="take">取得筆數</param>
    /// <param name="searchKeyword">搜尋關鍵字（可選）</param>
    /// <returns>使用者列表</returns>
    Task<IEnumerable<User>> GetUsersAsync(int skip = 0, int take = 100, string? searchKeyword = null);

    /// <summary>
    /// 取得使用者總數
    /// </summary>
    /// <param name="searchKeyword">搜尋關鍵字（可選）</param>
    /// <returns>總數</returns>
    Task<int> GetUsersCountAsync(string? searchKeyword = null);

    /// <summary>
    /// 取得完整的使用者資料（包含所有關聯資料）
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>完整的使用者資料</returns>
    Task<User?> GetCompleteUserDataAsync(int userId);
}
