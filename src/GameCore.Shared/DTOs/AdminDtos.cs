namespace GameCore.Shared.DTOs.AdminDtos;

public class AdminLoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AdminLoginResponseDto
{
    public int AdminId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SessionToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public List<string> Permissions { get; set; } = new();
    public string Role { get; set; } = string.Empty;
}

public class AdminSessionDto
{
    public int Id { get; set; }
    public int AdminId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public DateTime LastActivityTime { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
}

public class AdminDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAdminRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UpdateAdminRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ManagerRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}

public class UpdateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class ManagerRolePermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}

public class UpdateRolePermissionsRequestDto
{
    public List<string> Permissions { get; set; } = new();
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PostCount { get; set; }
    public int ThreadCount { get; set; }
}

public class UserSearchRequestDto
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SuspendUserRequestDto
{
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class BanUserRequestDto
{
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UnbanUserRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

public class DeleteUserRequestDto
{
    public string Reason { get; set; } = string.Empty;
    public bool Permanent { get; set; }
}

public class ModerationActionDto
{
    public int Id { get; set; }
    public int AdminId { get; set; }
    public string AdminUsername { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public int TargetId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public DateTime ActionTime { get; set; }
    public DateTime? ReversedAt { get; set; }
    public string? ReversalReason { get; set; }
}

public class ModerationSearchRequestDto
{
    public string? Action { get; set; }
    public string? TargetType { get; set; }
    public string? Status { get; set; }
    public int? AdminId { get; set; }
    public DateTime? ActionAfter { get; set; }
    public DateTime? ActionBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CreateModerationActionRequestDto
{
    public string Action { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public int TargetId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class ReverseModerationActionRequestDto
{
    public string Reason { get; set; } = string.Empty;
}

public class SystemLogDto
{
    public int Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? Source { get; set; }
    public int? UserId { get; set; }
    public int? AdminId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SystemLogSearchRequestDto
{
    public string? Level { get; set; }
    public string? Category { get; set; }
    public string? Event { get; set; }
    public int? UserId { get; set; }
    public int? AdminId { get; set; }
    public DateTime? After { get; set; }
    public DateTime? Before { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CreateSystemLogRequestDto
{
    public string Level { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? Source { get; set; }
    public int? UserId { get; set; }
    public int? AdminId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class AdminActionDto
{
    public int Id { get; set; }
    public int AdminId { get; set; }
    public string AdminUsername { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? TargetType { get; set; }
    public int? TargetId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime ActionTime { get; set; }
}

public class AdminActionSearchRequestDto
{
    public int? AdminId { get; set; }
    public string? Action { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public DateTime? After { get; set; }
    public DateTime? Before { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class LogAdminActionRequestDto
{
    public int AdminId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? TargetType { get; set; }
    public int? TargetId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public int BannedUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalThreads { get; set; }
    public int TotalAdmins { get; set; }
    public int ActiveSessions { get; set; }
    public List<SystemLogDto> RecentSystemLogs { get; set; } = new();
    public List<ModerationActionDto> RecentModerationActions { get; set; } = new();
    public List<AdminActionDto> RecentAdminActions { get; set; } = new();
}

public class UserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int ActiveUsersToday { get; set; }
    public int ActiveUsersThisWeek { get; set; }
    public Dictionary<string, int> UsersByStatus { get; set; } = new();
    public Dictionary<string, int> UserRegistrationsByDay { get; set; } = new();
}

public class ContentStatisticsDto
{
    public int TotalPosts { get; set; }
    public int TotalThreads { get; set; }
    public int PostsThisWeek { get; set; }
    public int ThreadsThisWeek { get; set; }
    public int ModeratedContentThisWeek { get; set; }
    public Dictionary<string, int> ContentByStatus { get; set; } = new();
    public Dictionary<string, int> ContentByCategory { get; set; } = new();
}

public class SecurityStatisticsDto
{
    public int FailedLoginAttempts { get; set; }
    public int SuspiciousActivities { get; set; }
    public int SecurityIncidents { get; set; }
    public int ActiveThreats { get; set; }
    public Dictionary<string, int> SecurityEventsByLevel { get; set; } = new();
    public Dictionary<string, int> SecurityEventsByCategory { get; set; } = new();
}

public class SystemStatisticsDto
{
    public int TotalSystemLogs { get; set; }
    public int LogsToday { get; set; }
    public int LogsThisWeek { get; set; }
    public Dictionary<string, int> LogsByLevel { get; set; } = new();
    public Dictionary<string, int> LogsByCategory { get; set; } = new();
    public List<SystemLogDto> CriticalLogs { get; set; } = new();
}

public class AdminStatisticsDto
{
    public int TotalActions { get; set; }
    public int ActionsToday { get; set; }
    public int ActionsThisWeek { get; set; }
    public Dictionary<string, int> ActionsByCategory { get; set; } = new();
    public Dictionary<string, int> ActionsByStatus { get; set; } = new();
    public List<AdminActionDto> RecentActions { get; set; } = new();
}