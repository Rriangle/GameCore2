using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 管理員服務接口
/// </summary>
public interface IAdminService
{
    /// <summary>
    /// 管理員登入
    /// </summary>
    /// <param name="request">登入請求</param>
    /// <returns>登入結果</returns>
    Task<AdminLoginResultDto> LoginAsync(AdminLoginRequestDto request);

    /// <summary>
    /// 管理員登出
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <returns>是否成功</returns>
    Task<bool> LogoutAsync(int managerId);

    /// <summary>
    /// 獲取管理員資訊
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <returns>管理員資訊</returns>
    Task<AdminInfoDto?> GetAdminInfoAsync(int managerId);

    /// <summary>
    /// 創建管理員
    /// </summary>
    /// <param name="request">創建請求</param>
    /// <param name="createdBy">創建者編號</param>
    /// <returns>創建結果</returns>
    Task<AdminInfoDto> CreateAdminAsync(CreateAdminRequestDto request, int createdBy);

    /// <summary>
    /// 更新管理員
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <param name="request">更新請求</param>
    /// <param name="updatedBy">更新者編號</param>
    /// <returns>更新結果</returns>
    Task<AdminInfoDto> UpdateAdminAsync(int managerId, UpdateAdminRequestDto request, int updatedBy);

    /// <summary>
    /// 刪除管理員
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <param name="deletedBy">刪除者編號</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAdminAsync(int managerId, int deletedBy);

    /// <summary>
    /// 分頁獲取管理員列表
    /// </summary>
    /// <param name="query">查詢參數</param>
    /// <returns>分頁結果</returns>
    Task<PagedAdminListDto> GetPagedAdminsAsync(AdminQueryDto query);

    /// <summary>
    /// 修改密碼
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <param name="request">修改請求</param>
    /// <returns>是否成功</returns>
    Task<bool> ChangePasswordAsync(int managerId, ChangePasswordRequestDto request);

    /// <summary>
    /// 重置密碼
    /// </summary>
    /// <param name="request">重置請求</param>
    /// <param name="resetBy">重置者編號</param>
    /// <returns>是否成功</returns>
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request, int resetBy);

    /// <summary>
    /// 獲取管理員權限
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <returns>權限列表</returns>
    Task<List<AdminPermissionDto>> GetAdminPermissionsAsync(int managerId);

    /// <summary>
    /// 獲取系統統計
    /// </summary>
    /// <returns>統計資訊</returns>
    Task<SystemStatisticsDto> GetSystemStatisticsAsync();

    /// <summary>
    /// 更新登入追蹤
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <param name="ip">IP位址</param>
    /// <param name="userAgent">用戶代理</param>
    /// <param name="isSuccess">是否成功</param>
    /// <returns>追蹤資訊</returns>
    Task<AdminLoginTrackingDto> UpdateLoginTrackingAsync(int managerId, string? ip, string? userAgent, bool isSuccess);

    /// <summary>
    /// 檢查權限
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <param name="permission">權限名稱</param>
    /// <returns>是否有權限</returns>
    Task<bool> HasPermissionAsync(int managerId, string permission);

    /// <summary>
    /// 鎖定/解鎖管理員帳號
    /// </summary>
    /// <param name="managerId">管理者編號</param>
    /// <param name="isLocked">是否鎖定</param>
    /// <param name="reason">原因</param>
    /// <param name="operatedBy">操作者編號</param>
    /// <returns>是否成功</returns>
    Task<bool> SetAccountLockAsync(int managerId, bool isLocked, string? reason, int operatedBy);
}