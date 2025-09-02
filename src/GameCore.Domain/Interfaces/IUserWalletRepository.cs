using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶錢包資料庫操作介面
/// </summary>
public interface IUserWalletRepository
{
    /// <summary>
    /// 根據用戶ID取得用戶錢包
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶錢包實體</returns>
    Task<UserWallet?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增用戶錢包
    /// </summary>
    /// <param name="userWallet">用戶錢包實體</param>
    /// <returns>新增的用戶錢包實體</returns>
    Task<UserWallet> AddAsync(UserWallet userWallet);

    /// <summary>
    /// 更新用戶錢包
    /// </summary>
    /// <param name="userWallet">用戶錢包實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(UserWallet userWallet);

    /// <summary>
    /// 刪除用戶錢包
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 扣除用戶點數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="points">扣除點數</param>
    /// <param name="reason">扣除原因</param>
    /// <returns>是否扣除成功</returns>
    Task<bool> DeductPointsAsync(int userId, int points, string reason);

    /// <summary>
    /// 增加用戶點數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="points">增加點數</param>
    /// <param name="reason">增加原因</param>
    /// <returns>是否增加成功</returns>
    Task<bool> AddPointsAsync(int userId, int points, string reason);

    /// <summary>
    /// 設定用戶點數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="points">設定點數</param>
    /// <returns>是否設定成功</returns>
    Task<bool> SetPointsAsync(int userId, int points);

    /// <summary>
    /// 根據點數範圍取得用戶列表
    /// </summary>
    /// <param name="minPoints">最小點數</param>
    /// <param name="maxPoints">最大點數</param>
    /// <returns>用戶錢包列表</returns>
    Task<IEnumerable<UserWallet>> GetByPointsRangeAsync(int minPoints, int maxPoints);

    /// <summary>
    /// 取得所有用戶錢包
    /// </summary>
    /// <returns>用戶錢包列表</returns>
    Task<IEnumerable<UserWallet>> GetAllAsync();

    /// <summary>
    /// 分頁取得用戶錢包
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>用戶錢包列表</returns>
    Task<IEnumerable<UserWallet>> GetPagedAsync(int page, int pageSize);

    /// <summary>
    /// 取得用戶錢包總數
    /// </summary>
    /// <returns>用戶錢包總數</returns>
    Task<int> GetCountAsync();

    /// <summary>
    /// 取得用戶交易記錄
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>交易記錄列表</returns>
    Task<IEnumerable<UserWalletTransaction>> GetTransactionsByUserIdAsync(int userId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得用戶交易記錄總數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>交易記錄總數</returns>
    Task<int> GetTransactionCountByUserIdAsync(int userId);

    /// <summary>
    /// 新增交易記錄
    /// </summary>
    /// <param name="transaction">交易記錄實體</param>
    /// <returns>新增的交易記錄實體</returns>
    Task<UserWalletTransaction> AddTransactionAsync(UserWalletTransaction transaction);
}
