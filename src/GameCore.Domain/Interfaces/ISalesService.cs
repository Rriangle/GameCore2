using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 銷售服務介面，定義銷售相關的業務邏輯操作
/// </summary>
public interface ISalesService
{
    /// <summary>
    /// 申請銷售功能
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="application">申請資料</param>
    /// <returns>是否申請成功</returns>
    Task<bool> ApplySalesProfileAsync(int userId, SalesProfileApplicationDto application);

    /// <summary>
    /// 取得銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售資料，無資料時返回 null</returns>
    Task<SalesProfileDto?> GetSalesProfileAsync(int userId);

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="application">更新資料</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateSalesProfileAsync(int userId, SalesProfileApplicationDto application);

    /// <summary>
    /// 取得銷售錢包資訊
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售錢包資訊</returns>
    Task<SalesWalletDto?> GetSalesWalletAsync(int userId);

    /// <summary>
    /// 銷售錢包交易 (增加收入)
    /// </summary>
    /// <param name="transaction">交易資料</param>
    /// <returns>是否交易成功</returns>
    Task<bool> AddSalesRevenueAsync(SalesWalletTransactionDto transaction);

    /// <summary>
    /// 銷售錢包交易 (扣除費用)
    /// </summary>
    /// <param name="transaction">交易資料</param>
    /// <returns>是否交易成功 (餘額不足會返回 false)</returns>
    Task<bool> DeductSalesAmountAsync(SalesWalletTransactionDto transaction);

    /// <summary>
    /// 管理者審核銷售申請
    /// </summary>
    /// <param name="adminUserId">管理者編號</param>
    /// <param name="userId">申請用戶編號</param>
    /// <param name="approved">是否核准</param>
    /// <param name="reason">審核理由</param>
    /// <returns>是否審核成功</returns>
    Task<bool> ReviewSalesApplicationAsync(int adminUserId, int userId, bool approved, string reason);

    /// <summary>
    /// 檢查用戶是否有銷售權限
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否有銷售權限</returns>
    Task<bool> HasSalesAuthorityAsync(int userId);

    /// <summary>
    /// 檢查銷售錢包是否有足夠餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="requiredAmount">需要的金額</param>
    /// <returns>是否有足夠餘額</returns>
    Task<bool> HasSufficientSalesBalanceAsync(int userId, int requiredAmount);

    /// <summary>
    /// 取得待審核的銷售申請列表 (管理者用)
    /// </summary>
    /// <returns>待審核申請列表</returns>
    Task<List<SalesProfileDto>> GetPendingApplicationsAsync();
}