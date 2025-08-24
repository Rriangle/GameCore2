using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 會員銷售資料存取介面
/// </summary>
public interface IMemberSalesProfileRepository
{
    /// <summary>
    /// 根據使用者ID取得會員銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>會員銷售資料，如果不存在則返回null</returns>
    Task<MemberSalesProfile?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 創建會員銷售資料
    /// </summary>
    /// <param name="salesProfile">會員銷售資料</param>
    /// <returns>創建的會員銷售資料</returns>
    Task<MemberSalesProfile> CreateAsync(MemberSalesProfile salesProfile);

    /// <summary>
    /// 更新會員銷售資料
    /// </summary>
    /// <param name="salesProfile">會員銷售資料</param>
    /// <returns>更新的會員銷售資料</returns>
    Task<MemberSalesProfile> UpdateAsync(MemberSalesProfile salesProfile);

    /// <summary>
    /// 刪除會員銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查使用者是否已有銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsAsync(int userId);

    /// <summary>
    /// 取得所有已申請銷售功能的會員列表
    /// </summary>
    /// <param name="skip">跳過筆數</param>
    /// <param name="take">取得筆數</param>
    /// <returns>會員銷售資料列表</returns>
    Task<IEnumerable<MemberSalesProfile>> GetAllAsync(int skip = 0, int take = 100);

    /// <summary>
    /// 取得已申請銷售功能的會員總數
    /// </summary>
    /// <returns>總數</returns>
    Task<int> GetCountAsync();
}