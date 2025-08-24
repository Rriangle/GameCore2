using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 管理者資料存儲接口
/// </summary>
public interface IManagerDataRepository
{
    /// <summary>
    /// 根據編號獲取管理者
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <returns>管理者實體</returns>
    Task<ManagerData?> GetByIdAsync(int managerId);

    /// <summary>
    /// 根據帳號獲取管理者
    /// </summary>
    /// <param name="account">管理者帳號</param>
    /// <returns>管理者實體</returns>
    Task<ManagerData?> GetByAccountAsync(string account);

    /// <summary>
    /// 獲取所有管理者
    /// </summary>
    /// <returns>管理者列表</returns>
    Task<IEnumerable<ManagerData>> GetAllAsync();

    /// <summary>
    /// 分頁獲取管理者
    /// </summary>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <param name="keyword">搜尋關鍵字</param>
    /// <param name="status">狀態篩選</param>
    /// <param name="level">等級篩選</param>
    /// <param name="department">部門篩選</param>
    /// <returns>分頁結果</returns>
    Task<(IEnumerable<ManagerData> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? keyword = null, 
        string? status = null, string? level = null, string? department = null);

    /// <summary>
    /// 創建管理者
    /// </summary>
    /// <param name="managerData">管理者實體</param>
    /// <returns>創建的管理者</returns>
    Task<ManagerData> CreateAsync(ManagerData managerData);

    /// <summary>
    /// 更新管理者
    /// </summary>
    /// <param name="managerData">管理者實體</param>
    /// <returns>更新的管理者</returns>
    Task<ManagerData> UpdateAsync(ManagerData managerData);

    /// <summary>
    /// 刪除管理者
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int managerId);

    /// <summary>
    /// 檢查帳號是否存在
    /// </summary>
    /// <param name="account">帳號</param>
    /// <param name="excludeId">排除的編號</param>
    /// <returns>是否存在</returns>
    Task<bool> AccountExistsAsync(string account, int? excludeId = null);

    /// <summary>
    /// 檢查信箱是否存在
    /// </summary>
    /// <param name="email">信箱</param>
    /// <param name="excludeId">排除的編號</param>
    /// <returns>是否存在</returns>
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);

    /// <summary>
    /// 獲取管理者統計
    /// </summary>
    /// <returns>統計資訊</returns>
    Task<(int Total, int Active, int TodayLogin)> GetStatisticsAsync();

    /// <summary>
    /// 根據角色獲取管理者
    /// </summary>
    /// <param name="roleId">角色編號</param>
    /// <returns>管理者列表</returns>
    Task<IEnumerable<ManagerData>> GetByRoleAsync(int roleId);

    /// <summary>
    /// 根據部門獲取管理者
    /// </summary>
    /// <param name="department">部門</param>
    /// <returns>管理者列表</returns>
    Task<IEnumerable<ManagerData>> GetByDepartmentAsync(string department);
}