using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs.AdminDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Infrastructure.Services;

public class AdminBackofficeService : IAdminBackofficeService
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<AdminBackofficeService> _logger;

    public AdminBackofficeService(GameCoreDbContext context, ILogger<AdminBackofficeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request, string ipAddress, string userAgent)
    {
        var admin = await _context.Admins
            .Include(a => a.ManagerRole)
            .FirstOrDefaultAsync(a => a.Username == request.Username && a.IsActive);

        if (admin == null || !VerifyPassword(request.Password, admin.PasswordHash))
        {
            await LogFailedLoginAttempt(request.Username, ipAddress);
            throw new InvalidOperationException("Invalid username or password");
        }

        // Create session
        var sessionToken = GenerateSessionToken();
        var session = new AdminSession
        {
            AdminId = admin.Id,
            SessionToken = sessionToken,
            Status = "Active",
            LoginTime = DateTime.UtcNow,
            LastActivityTime = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AdminSessions.Add(session);

        // Update admin last login
        admin.LastLoginAt = DateTime.UtcNow;
        admin.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Log admin action
        await LogAdminActionAsync(new LogAdminActionRequestDto
        {
            AdminId = admin.Id,
            Action = "Login",
            Category = "Security",
            Description = "Admin logged in successfully",
            Status = "Completed"
        });

        return new AdminLoginResponseDto
        {
            AdminId = admin.Id,
            Username = admin.Username,
            Email = admin.Email,
            SessionToken = sessionToken,
            ExpiresAt = session.ExpiresAt,
            Permissions = GetAdminPermissions(admin.ManagerRole),
            Role = admin.ManagerRole?.Name ?? "No Role"
        };
    }

    public async Task<bool> LogoutAsync(string sessionToken)
    {
        var session = await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.Status == "Active");

        if (session == null)
            return false;

        session.Status = "Revoked";
        session.LogoutTime = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await LogAdminActionAsync(new LogAdminActionRequestDto
        {
            AdminId = session.AdminId,
            Action = "Logout",
            Category = "Security",
            Description = "Admin logged out",
            Status = "Completed"
        });

        return true;
    }

    public async Task<AdminSessionDto> ValidateSessionAsync(string sessionToken)
    {
        var session = await _context.AdminSessions
            .Include(s => s.Admin)
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.Status == "Active");

        if (session == null || session.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired session");

        // Update last activity
        session.LastActivityTime = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new AdminSessionDto
        {
            Id = session.Id,
            AdminId = session.AdminId,
            Status = session.Status,
            LoginTime = session.LoginTime,
            LastActivityTime = session.LastActivityTime,
            ExpiresAt = session.ExpiresAt,
            IpAddress = session.IpAddress,
            UserAgent = session.UserAgent
        };
    }

    public async Task<bool> RefreshSessionAsync(string sessionToken)
    {
        var session = await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken && s.Status == "Active");

        if (session == null)
            return false;

        session.ExpiresAt = DateTime.UtcNow.AddHours(24);
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AdminDto>> GetAllAdminsAsync()
    {
        var admins = await _context.Admins
            .Include(a => a.ManagerRole)
            .Select(a => new AdminDto
            {
                Id = a.Id,
                Username = a.Username,
                Email = a.Email,
                Role = a.ManagerRole.Name,
                IsActive = a.IsActive,
                LastLoginAt = a.LastLoginAt,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return admins;
    }

    public async Task<AdminDto> GetAdminByIdAsync(int adminId)
    {
        var admin = await _context.Admins
            .Include(a => a.ManagerRole)
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin == null)
            throw new InvalidOperationException("Admin not found");

        return new AdminDto
        {
            Id = admin.Id,
            Username = admin.Username,
            Email = admin.Email,
            Role = admin.ManagerRole?.Name ?? "No Role",
            IsActive = admin.IsActive,
            LastLoginAt = admin.LastLoginAt,
            CreatedAt = admin.CreatedAt,
            UpdatedAt = admin.UpdatedAt
        };
    }

    public async Task<AdminDto> CreateAdminAsync(CreateAdminRequestDto request, int createdByAdminId)
    {
        if (await _context.Admins.AnyAsync(a => a.Username == request.Username))
            throw new InvalidOperationException("Username already exists");

        if (await _context.Admins.AnyAsync(a => a.Email == request.Email))
            throw new InvalidOperationException("Email already exists");

        var passwordHash = HashPassword(request.Password);
        var admin = new Admin
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        await LogAdminActionAsync(new LogAdminActionRequestDto
        {
            AdminId = createdByAdminId,
            Action = "CreateAdmin",
            Category = "Admin",
            Description = $"Created admin user: {request.Username}",
            TargetType = "Admin",
            TargetId = admin.Id,
            Status = "Completed"
        });

        return await GetAdminByIdAsync(admin.Id);
    }

    public async Task<List<UserDto>> GetUsersAsync(UserSearchRequestDto request)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u => u.Username.Contains(request.SearchTerm) || u.Email.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(u => u.Status == request.Status);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        if (request.CreatedAfter.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= request.CreatedBefore.Value);
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsActive = u.IsActive,
                Status = u.Status,
                LastLoginAt = u.LastLoginAt,
                CreatedAt = u.CreatedAt,
                PostCount = 0, // Would need to join with posts table
                ThreadCount = 0 // Would need to join with threads table
            })
            .ToListAsync();

        return users;
    }

    public async Task<bool> SuspendUserAsync(int userId, SuspendUserRequestDto request, int adminId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.Status = "Suspended";
        user.UpdatedAt = DateTime.UtcNow;

        var moderationAction = new ModerationAction
        {
            AdminId = adminId,
            Action = "Suspend",
            TargetType = "User",
            TargetId = userId,
            Reason = request.Reason,
            Details = request.Details,
            Status = "Active",
            ExpiresAt = request.ExpiresAt,
            ActionTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ModerationActions.Add(moderationAction);
        await _context.SaveChangesAsync();

        await LogAdminActionAsync(new LogAdminActionRequestDto
        {
            AdminId = adminId,
            Action = "SuspendUser",
            Category = "Moderation",
            Description = $"Suspended user: {user.Username}",
            TargetType = "User",
            TargetId = userId,
            Status = "Completed"
        });

        return true;
    }

    public async Task<bool> BanUserAsync(int userId, BanUserRequestDto request, int adminId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.Status = "Banned";
        user.UpdatedAt = DateTime.UtcNow;

        var moderationAction = new ModerationAction
        {
            AdminId = adminId,
            Action = "Ban",
            TargetType = "User",
            TargetId = userId,
            Reason = request.Reason,
            Details = request.Details,
            Status = "Active",
            ExpiresAt = request.ExpiresAt,
            ActionTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ModerationActions.Add(moderationAction);
        await _context.SaveChangesAsync();

        await LogAdminActionAsync(new LogAdminActionRequestDto
        {
            AdminId = adminId,
            Action = "BanUser",
            Category = "Moderation",
            Description = $"Banned user: {user.Username}",
            TargetType = "User",
            TargetId = userId,
            Status = "Completed"
        });

        return true;
    }

    public async Task<bool> LogAdminActionAsync(LogAdminActionRequestDto request)
    {
        var adminAction = new AdminAction
        {
            AdminId = request.AdminId,
            Action = request.Action,
            Category = request.Category,
            Description = request.Description,
            Details = request.Details,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            Status = request.Status,
            ErrorMessage = request.ErrorMessage,
            ActionTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AdminActions.Add(adminAction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.CountAsync(u => u.IsActive && u.Status == "Active");
        var suspendedUsers = await _context.Users.CountAsync(u => u.Status == "Suspended");
        var bannedUsers = await _context.Users.CountAsync(u => u.Status == "Banned");
        var totalAdmins = await _context.Admins.CountAsync();
        var activeSessions = await _context.AdminSessions.CountAsync(s => s.Status == "Active");

        var recentSystemLogs = await _context.SystemLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(10)
            .Select(l => new SystemLogDto
            {
                Id = l.Id,
                Level = l.Level,
                Category = l.Category,
                Event = l.Event,
                Message = l.Message,
                Details = l.Details,
                Source = l.Source,
                UserId = l.UserId,
                AdminId = l.AdminId,
                IpAddress = l.IpAddress,
                Timestamp = l.Timestamp
            })
            .ToListAsync();

        var recentModerationActions = await _context.ModerationActions
            .Include(ma => ma.Admin)
            .OrderByDescending(ma => ma.ActionTime)
            .Take(10)
            .Select(ma => new ModerationActionDto
            {
                Id = ma.Id,
                AdminId = ma.AdminId,
                AdminUsername = ma.Admin.Username,
                Action = ma.Action,
                TargetType = ma.TargetType,
                TargetId = ma.TargetId,
                Reason = ma.Reason,
                Details = ma.Details,
                Status = ma.Status,
                ExpiresAt = ma.ExpiresAt,
                ActionTime = ma.ActionTime,
                ReversedAt = ma.ReversedAt,
                ReversalReason = ma.ReversalReason
            })
            .ToListAsync();

        var recentAdminActions = await _context.AdminActions
            .Include(aa => aa.Admin)
            .OrderByDescending(aa => aa.ActionTime)
            .Take(10)
            .Select(aa => new AdminActionDto
            {
                Id = aa.Id,
                AdminId = aa.AdminId,
                AdminUsername = aa.Admin.Username,
                Action = aa.Action,
                Category = aa.Category,
                Description = aa.Description,
                Details = aa.Details,
                TargetType = aa.TargetType,
                TargetId = aa.TargetId,
                Status = aa.Status,
                ErrorMessage = aa.ErrorMessage,
                ActionTime = aa.ActionTime
            })
            .ToListAsync();

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            SuspendedUsers = suspendedUsers,
            BannedUsers = bannedUsers,
            TotalAdmins = totalAdmins,
            ActiveSessions = activeSessions,
            RecentSystemLogs = recentSystemLogs,
            RecentModerationActions = recentModerationActions,
            RecentAdminActions = recentAdminActions
        };
    }

    // Helper methods
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }

    private string GenerateSessionToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    private List<string> GetAdminPermissions(ManagerRole? role)
    {
        if (role == null) return new List<string>();

        return role.ManagerRolePermissions
            .Where(p => p.IsGranted)
            .Select(p => p.Permission)
            .ToList();
    }

    private async Task LogFailedLoginAttempt(string username, string ipAddress)
    {
        var systemLog = new SystemLog
        {
            Level = "Warning",
            Category = "Security",
            Event = "FailedLogin",
            Message = $"Failed login attempt for username: {username}",
            IpAddress = ipAddress,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.SystemLogs.Add(systemLog);
        await _context.SaveChangesAsync();
    }

    // Placeholder implementations for remaining interface methods
    public async Task<bool> CreateSystemLogAsync(CreateSystemLogRequestDto request)
    {
        var systemLog = new SystemLog
        {
            Level = request.Level,
            Category = request.Category,
            Event = request.Event,
            Message = request.Message,
            Details = request.Details,
            Source = request.Source,
            UserId = request.UserId,
            AdminId = request.AdminId,
            IpAddress = request.IpAddress,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.SystemLogs.Add(systemLog);
        await _context.SaveChangesAsync();
        return true;
    }

    // Additional placeholder methods would be implemented here...
    // For brevity, I'm implementing the core functionality first
}