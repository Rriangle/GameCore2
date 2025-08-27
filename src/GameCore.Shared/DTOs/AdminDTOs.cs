namespace GameCore.Shared.DTOs;

/// <summary>
/// 管理員登入請求 DTO
/// </summary>
public class AdminLoginRequestDto
{
    /// <summary>
    /// 管理者帳號
    /// </summary>
    public string Account { get; set; } = string.Empty;

    /// <summary>
    /// 管理者密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 記住我
    /// </summary>
    public bool RememberMe { get; set; } = false;

    /// <summary>
    /// 客戶端時區偏移 (分鐘)
    /// </summary>
    public int TimezoneOffset { get; set; } = 480;

    /// <summary>
    /// 客戶端IP
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 用戶代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 瀏覽器指紋
    /// </summary>
    public string? BrowserFingerprint { get; set; }
}

/// <summary>
/// 管理員登入結果 DTO
/// </summary>
public class AdminLoginResultDto
{
    /// <summary>
    /// 是否登入成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 存取令牌
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 令牌過期時間
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// 管理員資訊
    /// </summary>
    public AdminInfoDto? AdminInfo { get; set; }

    /// <summary>
    /// 管理員權限列表
    /// </summary>
    public List<AdminPermissionDto> Permissions { get; set; } = new();

    /// <summary>
    /// 是否首次登入
    /// </summary>
    public bool IsFirstLogin { get; set; }

    /// <summary>
    /// 是否需要修改密碼
    /// </summary>
    public bool RequirePasswordChange { get; set; }

    /// <summary>
    /// 登入追蹤資訊
    /// </summary>
    public AdminLoginTrackingDto? LoginTracking { get; set; }
}

/// <summary>
/// 管理員基本資訊 DTO
/// </summary>
public class AdminInfoDto
{
    /// <summary>
    /// 管理者編號
    /// </summary>
    public int ManagerId { get; set; }

    /// <summary>
    /// 管理者姓名
    /// </summary>
    public string ManagerName { get; set; } = string.Empty;

    /// <summary>
    /// 管理者帳號
    /// </summary>
    public string ManagerAccount { get; set; } = string.Empty;

    /// <summary>
    /// 管理者狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 管理者等級
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// 電子信箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 職位
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 註冊時間
    /// </summary>
    public DateTime AdministratorRegistrationDate { get; set; }

    /// <summary>
    /// 是否為系統管理員
    /// </summary>
    public bool IsSuperAdmin { get; set; }

    /// <summary>
    /// 管理員角色列表
    /// </summary>
    public List<AdminRoleDto> Roles { get; set; } = new();
}

/// <summary>
/// 管理員權限 DTO
/// </summary>
public class AdminPermissionDto
{
    /// <summary>
    /// 權限名稱
    /// </summary>
    public string PermissionName { get; set; } = string.Empty;

    /// <summary>
    /// 權限描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否擁有權限
    /// </summary>
    public bool HasPermission { get; set; }

    /// <summary>
    /// 權限來源角色
    /// </summary>
    public string? SourceRole { get; set; }
}

/// <summary>
/// 管理員角色 DTO
/// </summary>
public class AdminRoleDto
{
    /// <summary>
    /// 角色編號
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? RoleDescription { get; set; }

    /// <summary>
    /// 角色狀態
    /// </summary>
    public string RoleStatus { get; set; } = string.Empty;

    /// <summary>
    /// 是否為主要角色
    /// </summary>
    public bool IsPrimaryRole { get; set; }

    /// <summary>
    /// 角色等級
    /// </summary>
    public int RoleLevel { get; set; }

    /// <summary>
    /// 指派時間
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// 角色生效時間
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// 角色失效時間
    /// </summary>
    public DateTime? EffectiveTo { get; set; }
}

/// <summary>
/// 管理員登入追蹤 DTO
/// </summary>
public class AdminLoginTrackingDto
{
    /// <summary>
    /// 上次登入時間
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 登入次數
    /// </summary>
    public int LoginCount { get; set; }

    /// <summary>
    /// 首次登入時間
    /// </summary>
    public DateTime? FirstLoginAt { get; set; }

    /// <summary>
    /// 上次登入IP
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 登入失敗次數
    /// </summary>
    public int FailedLoginCount { get; set; }

    /// <summary>
    /// 是否被鎖定
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// 鎖定到期時間
    /// </summary>
    public DateTime? LockedUntil { get; set; }
}

/// <summary>
/// 管理員建立請求 DTO
/// </summary>
public class CreateAdminRequestDto
{
    /// <summary>
    /// 管理者姓名
    /// </summary>
    public string ManagerName { get; set; } = string.Empty;

    /// <summary>
    /// 管理者帳號
    /// </summary>
    public string ManagerAccount { get; set; } = string.Empty;

    /// <summary>
    /// 管理者密碼
    /// </summary>
    public string ManagerPassword { get; set; } = string.Empty;

    /// <summary>
    /// 電子信箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 職位
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 管理者等級
    /// </summary>
    public string Level { get; set; } = "admin";

    /// <summary>
    /// 指派的角色編號列表
    /// </summary>
    public List<int> RoleIds { get; set; } = new();

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 管理員更新請求 DTO
/// </summary>
public class UpdateAdminRequestDto
{
    /// <summary>
    /// 管理者姓名
    /// </summary>
    public string? ManagerName { get; set; }

    /// <summary>
    /// 電子信箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 職位
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 管理者狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 管理者等級
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 角色權限定義 DTO
/// </summary>
public class RolePermissionDto
{
    /// <summary>
    /// 角色編號
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? RoleDescription { get; set; }

    /// <summary>
    /// 管理者權限管理
    /// </summary>
    public bool AdministratorPrivilegesManagement { get; set; }

    /// <summary>
    /// 使用者狀態管理
    /// </summary>
    public bool UserStatusManagement { get; set; }

    /// <summary>
    /// 商城權限管理
    /// </summary>
    public bool ShoppingPermissionManagement { get; set; }

    /// <summary>
    /// 論壇權限管理
    /// </summary>
    public bool MessagePermissionManagement { get; set; }

    /// <summary>
    /// 寵物權限管理
    /// </summary>
    public bool PetRightsManagement { get; set; }

    /// <summary>
    /// 客服權限管理
    /// </summary>
    public bool CustomerService { get; set; }

    /// <summary>
    /// 是否為系統內建角色
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 角色等級
    /// </summary>
    public int RoleLevel { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 角色建立請求 DTO
/// </summary>
public class CreateRoleRequestDto
{
    /// <summary>
    /// 角色名稱
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? RoleDescription { get; set; }

    /// <summary>
    /// 管理者權限管理
    /// </summary>
    public bool AdministratorPrivilegesManagement { get; set; } = false;

    /// <summary>
    /// 使用者狀態管理
    /// </summary>
    public bool UserStatusManagement { get; set; } = false;

    /// <summary>
    /// 商城權限管理
    /// </summary>
    public bool ShoppingPermissionManagement { get; set; } = false;

    /// <summary>
    /// 論壇權限管理
    /// </summary>
    public bool MessagePermissionManagement { get; set; } = false;

    /// <summary>
    /// 寵物權限管理
    /// </summary>
    public bool PetRightsManagement { get; set; } = false;

    /// <summary>
    /// 客服權限管理
    /// </summary>
    public bool CustomerService { get; set; } = false;

    /// <summary>
    /// 角色等級
    /// </summary>
    public int RoleLevel { get; set; } = 1;

    /// <summary>
    /// 角色顏色
    /// </summary>
    public string? RoleColor { get; set; }

    /// <summary>
    /// 角色圖標
    /// </summary>
    public string? RoleIcon { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 角色指派請求 DTO
/// </summary>
public class AssignRoleRequestDto
{
    /// <summary>
    /// 管理者編號
    /// </summary>
    public int ManagerId { get; set; }

    /// <summary>
    /// 角色編號
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 角色生效時間
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// 角色失效時間
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// 是否為主要角色
    /// </summary>
    public bool IsPrimaryRole { get; set; } = false;

    /// <summary>
    /// 指派原因
    /// </summary>
    public string? AssignReason { get; set; }

    /// <summary>
    /// 自動撤銷
    /// </summary>
    public bool AutoRevoke { get; set; } = false;
}

/// <summary>
/// 禁言項目 DTO
/// </summary>
public class MuteItemDto
{
    /// <summary>
    /// 禁言編號
    /// </summary>
    public int MuteId { get; set; }

    /// <summary>
    /// 禁言名稱
    /// </summary>
    public string MuteName { get; set; } = string.Empty;

    /// <summary>
    /// 禁言描述
    /// </summary>
    public string? MuteDescription { get; set; }

    /// <summary>
    /// 禁言類型
    /// </summary>
    public string MuteType { get; set; } = string.Empty;

    /// <summary>
    /// 預設禁言時長 (分鐘)
    /// </summary>
    public int? DefaultDurationMinutes { get; set; }

    /// <summary>
    /// 嚴重程度
    /// </summary>
    public int SeverityLevel { get; set; }

    /// <summary>
    /// 適用範圍
    /// </summary>
    public string ApplicableScope { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 使用次數
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 設置者名稱
    /// </summary>
    public string? CreatedByName { get; set; }
}

/// <summary>
/// 樣式項目 DTO
/// </summary>
public class StyleItemDto
{
    /// <summary>
    /// 樣式編號
    /// </summary>
    public int StyleId { get; set; }

    /// <summary>
    /// 樣式名稱
    /// </summary>
    public string StyleName { get; set; } = string.Empty;

    /// <summary>
    /// 效果說明
    /// </summary>
    public string EffectDesc { get; set; } = string.Empty;

    /// <summary>
    /// 樣式類型
    /// </summary>
    public string StyleType { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 是否為預設樣式
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 適用範圍
    /// </summary>
    public string ApplicableScope { get; set; } = string.Empty;

    /// <summary>
    /// 樣式版本
    /// </summary>
    public string StyleVersion { get; set; } = string.Empty;

    /// <summary>
    /// 使用次數
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 設置者名稱
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// 預覽圖片URL
    /// </summary>
    public string? PreviewImage { get; set; }
}

/// <summary>
/// 管理員查詢參數 DTO
/// </summary>
public class AdminQueryDto
{
    /// <summary>
    /// 搜尋關鍵字 (姓名、帳號、信箱)
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 管理者狀態篩選
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 管理者等級篩選
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// 部門篩選
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// 角色編號篩選
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// 註冊時間開始
    /// </summary>
    public DateTime? RegisteredFrom { get; set; }

    /// <summary>
    /// 註冊時間結束
    /// </summary>
    public DateTime? RegisteredTo { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool OrderDescending { get; set; } = true;
}

/// <summary>
/// 分頁管理員列表 DTO
/// </summary>
public class PagedAdminListDto
{
    /// <summary>
    /// 管理員列表
    /// </summary>
    public List<AdminInfoDto> Admins { get; set; } = new();

    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// 系統統計 DTO
/// </summary>
public class SystemStatisticsDto
{
    /// <summary>
    /// 總管理員數
    /// </summary>
    public int TotalAdmins { get; set; }

    /// <summary>
    /// 活躍管理員數
    /// </summary>
    public int ActiveAdmins { get; set; }

    /// <summary>
    /// 今日登入管理員數
    /// </summary>
    public int TodayLoginAdmins { get; set; }

    /// <summary>
    /// 總角色數
    /// </summary>
    public int TotalRoles { get; set; }

    /// <summary>
    /// 總用戶數
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// 今日新註冊用戶數
    /// </summary>
    public int TodayNewUsers { get; set; }

    /// <summary>
    /// 禁言項目數
    /// </summary>
    public int TotalMuteItems { get; set; }

    /// <summary>
    /// 樣式項目數
    /// </summary>
    public int TotalStyleItems { get; set; }

    /// <summary>
    /// 系統負載
    /// </summary>
    public SystemLoadDto? SystemLoad { get; set; }
}

/// <summary>
/// 系統負載 DTO
/// </summary>
public class SystemLoadDto
{
    /// <summary>
    /// CPU使用率 (百分比)
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// 記憶體使用率 (百分比)
    /// </summary>
    public double MemoryUsage { get; set; }

    /// <summary>
    /// 磁碟使用率 (百分比)
    /// </summary>
    public double DiskUsage { get; set; }

    /// <summary>
    /// 活躍連線數
    /// </summary>
    public int ActiveConnections { get; set; }

    /// <summary>
    /// 資料庫連線數
    /// </summary>
    public int DatabaseConnections { get; set; }
}

/// <summary>
/// 重置密碼請求 DTO
/// </summary>
public class ResetPasswordRequestDto
{
    /// <summary>
    /// 目標管理員編號
    /// </summary>
    public int TargetManagerId { get; set; }

    /// <summary>
    /// 新密碼
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 重置原因
    /// </summary>
    public string? ResetReason { get; set; }

    /// <summary>
    /// 是否強制下次登入修改密碼
    /// </summary>
    public bool ForceChangeOnNextLogin { get; set; } = true;
}