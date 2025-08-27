using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶銷售資訊存取介面，定義銷售錢包相關的資料庫操作
/// </summary>
public interface IUserSalesInformationRepository
{
    /// <summary>
    /// 根據用戶編號取得銷售資訊
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售資訊，不存在則返回 null</returns>
    Task<UserSalesInformation?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 建立新銷售資訊
    /// </summary>
    /// <param name="salesInfo">銷售資訊</param>
    /// <returns>建立的銷售資訊</returns>
    Task<UserSalesInformation> CreateAsync(UserSalesInformation salesInfo);

    /// <summary>
    /// 更新銷售資訊
    /// </summary>
    /// <param name="salesInfo">銷售資訊</param>
    /// <returns>更新的銷售資訊</returns>
    Task<UserSalesInformation> UpdateAsync(UserSalesInformation salesInfo);

    /// <summary>
    /// 取得銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售錢包餘額</returns>
    Task<int> GetSalesWalletBalanceAsync(int userId);

    /// <summary>
    /// 更新銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="balance">新餘額</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateSalesWalletBalanceAsync(int userId, int balance);

    /// <summary>
    /// 增加銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="amount">增加的金額</param>
    /// <returns>是否增加成功</returns>
    Task<bool> AddToSalesWalletAsync(int userId, int amount);

    /// <summary>
    /// 扣除銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="amount">扣除的金額</param>
    /// <returns>是否扣除成功 (餘額不足會返回 false)</returns>
    Task<bool> DeductFromSalesWalletAsync(int userId, int amount);

    /// <summary>
    /// 刪除銷售資訊
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 取得所有銷售資訊列表 (管理者用)
    /// </summary>
    /// <returns>銷售資訊列表</returns>
    Task<List<UserSalesInformation>> GetAllAsync();

    /// <summary>
    /// 取得銷售收入統計 (按用戶)
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="fromDate">開始日期</param>
    /// <param name="toDate">結束日期</param>
    /// <returns>收入統計</returns>
    Task<decimal> GetSalesRevenueStatisticsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
}