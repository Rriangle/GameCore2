using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 會員銷售資料存取介面，定義銷售資料相關的資料庫操作
/// </summary>
public interface IMemberSalesProfileRepository
{
    /// <summary>
    /// 根據用戶編號取得銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售資料，不存在則返回 null</returns>
    Task<MemberSalesProfile?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 建立新銷售資料
    /// </summary>
    /// <param name="salesProfile">銷售資料</param>
    /// <returns>建立的銷售資料</returns>
    Task<MemberSalesProfile> CreateAsync(MemberSalesProfile salesProfile);

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="salesProfile">銷售資料</param>
    /// <returns>更新的銷售資料</returns>
    Task<MemberSalesProfile> UpdateAsync(MemberSalesProfile salesProfile);

    /// <summary>
    /// 刪除銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查銀行帳號是否已存在
    /// </summary>
    /// <param name="bankCode">銀行代號</param>
    /// <param name="bankAccountNumber">銀行帳號</param>
    /// <param name="excludeUserId">排除的用戶編號 (用於更新時檢查)</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsBankAccountExistsAsync(int bankCode, string bankAccountNumber, int? excludeUserId = null);

    /// <summary>
    /// 取得所有銷售資料列表 (管理者用)
    /// </summary>
    /// <returns>銷售資料列表</returns>
    Task<List<MemberSalesProfile>> GetAllAsync();

    /// <summary>
    /// 根據銀行代號查詢銷售資料
    /// </summary>
    /// <param name="bankCode">銀行代號</param>
    /// <returns>該銀行的銷售資料列表</returns>
    Task<List<MemberSalesProfile>> GetByBankCodeAsync(int bankCode);
}