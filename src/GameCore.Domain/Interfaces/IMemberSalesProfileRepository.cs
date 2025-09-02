using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 會員銷售資料存取介面
/// </summary>
public interface IMemberSalesProfileRepository
{
    /// <summary>
    /// 根據用戶ID取得銷售資料
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>銷售資料</returns>
    Task<MemberSalesProfile?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增銷售資料
    /// </summary>
    /// <param name="profile">銷售資料</param>
    /// <returns>是否成功</returns>
    Task<bool> AddAsync(MemberSalesProfile profile);

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="profile">銷售資料</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(MemberSalesProfile profile);

    /// <summary>
    /// 刪除銷售資料
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 取得所有銷售資料
    /// </summary>
    /// <returns>銷售資料列表</returns>
    Task<List<MemberSalesProfile>> GetAllAsync();

    /// <summary>
    /// 根據銀行代碼取得銷售資料
    /// </summary>
    /// <param name="bankCode">銀行代碼</param>
    /// <returns>銷售資料列表</returns>
    Task<List<MemberSalesProfile>> GetByBankCodeAsync(int bankCode);

    /// <summary>
    /// 檢查用戶是否已有銷售資料
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(int userId);
} 