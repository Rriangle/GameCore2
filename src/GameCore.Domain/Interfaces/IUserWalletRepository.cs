using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶錢包資料庫操作介面
/// </summary>
public interface IUserWalletRepository
{
    /// <summary>
    /// 根據用戶 ID 取得用戶錢包
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶錢包實體</returns>
    Task<UserWallet?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增用戶錢包
    /// </summary>
    /// <param name="wallet">用戶錢包實體</param>
    /// <returns>新增的用戶錢包</returns>
    Task<UserWallet> AddAsync(UserWallet wallet);

    /// <summary>
    /// 更新用戶錢包
    /// </summary>
    /// <param name="wallet">用戶錢包實體</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UserWallet wallet);

    /// <summary>
    /// 刪除用戶錢包
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 取得用戶錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>錢包餘額</returns>
    Task<int> GetBalanceAsync(int userId);

    /// <summary>
    /// 更新用戶錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="amount">金額變化</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateBalanceAsync(int userId, int amount);

    /// <summary>
    /// 取得所有用戶錢包 (分頁)
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>用戶錢包列表</returns>
    Task<IEnumerable<UserWallet>> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// 根據餘額範圍篩選用戶錢包
    /// </summary>
    /// <param name="minBalance">最小餘額</param>
    /// <param name="maxBalance">最大餘額</param>
    /// <returns>用戶錢包列表</returns>
    Task<IEnumerable<UserWallet>> GetByBalanceRangeAsync(int minBalance, int maxBalance);
}
