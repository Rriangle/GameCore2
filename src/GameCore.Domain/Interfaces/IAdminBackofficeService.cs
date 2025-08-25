using GameCore.Shared.DTOs.AdminDtos;

namespace GameCore.Domain.Interfaces;

public interface IAdminBackofficeService
{
    // Admin Authentication & Session Management
    Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request, string ipAddress, string userAgent);
    Task<bool> LogoutAsync(string sessionToken);
    Task<AdminSessionDto> ValidateSessionAsync(string sessionToken);
    Task<bool> RefreshSessionAsync(string sessionToken);

    // Admin Management
    Task<List<AdminDto>> GetAllAdminsAsync();
    Task<AdminDto> GetAdminByIdAsync(int adminId);
    Task<AdminDto> CreateAdminAsync(CreateAdminRequestDto request, int createdByAdminId);
    Task<AdminDto> UpdateAdminAsync(int adminId, UpdateAdminRequestDto request, int updatedByAdminId);
    Task<bool> DeleteAdminAsync(int adminId, int deletedByAdminId);
    Task<bool> ChangeAdminPasswordAsync(int adminId, ChangePasswordRequestDto request);

    // Role & Permission Management
    Task<List<ManagerRoleDto>> GetAllRolesAsync();
    Task<ManagerRoleDto> GetRoleByIdAsync(int roleId);
    Task<ManagerRoleDto> CreateRoleAsync(CreateRoleRequestDto request, int createdByAdminId);
    Task<ManagerRoleDto> UpdateRoleAsync(int roleId, UpdateRoleRequestDto request, int updatedByAdminId);
    Task<bool> DeleteRoleAsync(int roleId, int deletedByAdminId);
    Task<List<ManagerRolePermissionDto>> GetRolePermissionsAsync(int roleId);
    Task<bool> UpdateRolePermissionsAsync(int roleId, UpdateRolePermissionsRequestDto request, int updatedByAdminId);

    // User Management & Moderation
    Task<List<UserDto>> GetUsersAsync(UserSearchRequestDto request);
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<bool> SuspendUserAsync(int userId, SuspendUserRequestDto request, int adminId);
    Task<bool> BanUserAsync(int userId, BanUserRequestDto request, int adminId);
    Task<bool> UnbanUserAsync(int userId, UnbanUserRequestDto request, int adminId);
    Task<bool> DeleteUserAsync(int userId, DeleteUserRequestDto request, int adminId);

    // Content Moderation
    Task<List<ModerationActionDto>> GetModerationActionsAsync(ModerationSearchRequestDto request);
    Task<ModerationActionDto> GetModerationActionByIdAsync(int actionId);
    Task<ModerationActionDto> CreateModerationActionAsync(CreateModerationActionRequestDto request, int adminId);
    Task<bool> ReverseModerationActionAsync(int actionId, ReverseModerationActionRequestDto request, int adminId);

    // System Monitoring & Logs
    Task<List<SystemLogDto>> GetSystemLogsAsync(SystemLogSearchRequestDto request);
    Task<SystemLogDto> GetSystemLogByIdAsync(int logId);
    Task<bool> CreateSystemLogAsync(CreateSystemLogRequestDto request);
    Task<SystemStatisticsDto> GetSystemStatisticsAsync();

    // Admin Activity Tracking
    Task<List<AdminActionDto>> GetAdminActionsAsync(AdminActionSearchRequestDto request);
    Task<AdminActionDto> GetAdminActionByIdAsync(int actionId);
    Task<bool> LogAdminActionAsync(LogAdminActionRequestDto request);
    Task<AdminStatisticsDto> GetAdminStatisticsAsync(int adminId);

    // Dashboard & Analytics
    Task<AdminDashboardDto> GetAdminDashboardAsync();
    Task<UserStatisticsDto> GetUserStatisticsAsync();
    Task<ContentStatisticsDto> GetContentStatisticsAsync();
    Task<SecurityStatisticsDto> GetSecurityStatisticsAsync();
}