using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 銷售權限申請資料庫操作介面
/// </summary>
public interface IMemberSalesProfileRepository
{
    /// <summary>
    /// 根據用戶 ID 取得銷售權限申請
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>銷售權限申請實體</returns>
    Task<MemberSalesProfile?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增銷售權限申請
    /// </summary>
    /// <param name="memberSalesProfile">銷售權限申請實體</param>
    /// <returns>新增的銷售權限申請</returns>
    Task<MemberSalesProfile> AddAsync(MemberSalesProfile memberSalesProfile);

    /// <summary>
    /// 更新銷售權限申請
    /// </summary>
    /// <param name="memberSalesProfile">銷售權限申請實體</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(MemberSalesProfile memberSalesProfile);

    /// <summary>
    /// 刪除銷售權限申請
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 取得所有待審核的申請
    /// </summary>
    /// <returns>待審核申請列表</returns>
    Task<IEnumerable<MemberSalesProfile>> GetPendingApplicationsAsync();

    /// <summary>
    /// 取得所有已審核的申請
    /// </summary>
    /// <returns>已審核申請列表</returns>
    Task<IEnumerable<MemberSalesProfile>> GetReviewedApplicationsAsync();

    /// <summary>
    /// 根據狀態取得申請
    /// </summary>
    /// <param name="status">申請狀態</param>
    /// <returns>申請列表</returns>
    Task<IEnumerable<MemberSalesProfile>> GetByStatusAsync(string status);

    /// <summary>
    /// 更新申請狀態
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="status">新狀態</param>
    /// <param name="reviewedBy">審核者 ID</param>
    /// <param name="reviewNotes">審核備註</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(int userId, string status, int reviewedBy, string? reviewNotes = null);
}