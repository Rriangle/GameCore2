using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 銷售錢包資料庫操作介面
/// </summary>
public interface IUserSalesInformationRepository
{
    /// <summary>
    /// 根據用戶 ID 取得銷售錢包
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>銷售錢包實體</returns>
    Task<UserSalesInformation?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增銷售錢包
    /// </summary>
    /// <param name="userSalesInformation">銷售錢包實體</param>
    /// <returns>新增的銷售錢包</returns>
    Task<UserSalesInformation> AddAsync(UserSalesInformation userSalesInformation);

    /// <summary>
    /// 更新銷售錢包
    /// </summary>
    /// <param name="userSalesInformation">銷售錢包實體</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UserSalesInformation userSalesInformation);

    /// <summary>
    /// 刪除銷售錢包
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 更新銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="amount">金額變化</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateBalanceAsync(int userId, decimal amount);

    /// <summary>
    /// 取得所有銷售錢包 (分頁)
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>銷售錢包列表</returns>
    Task<IEnumerable<UserSalesInformation>> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// 根據餘額範圍篩選銷售錢包
    /// </summary>
    /// <param name="minBalance">最小餘額</param>
    /// <param name="maxBalance">最大餘額</param>
    /// <returns>銷售錢包列表</returns>
    Task<IEnumerable<UserSalesInformation>> GetByBalanceRangeAsync(decimal minBalance, decimal maxBalance);
}