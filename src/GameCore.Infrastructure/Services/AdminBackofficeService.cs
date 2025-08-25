using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs.AdminDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 管理員後台服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class AdminBackofficeService : IAdminBackofficeService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AdminBackofficeService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 30;
    private const int MaxUsernameLength = 50;
    private const int MaxEmailLength = 100;
    private const int MaxPasswordLength = 128;
    private const int MaxReasonLength = 500;
    private const int MaxDetailsLength = 1000;
    private const int MaxDescriptionLength = 500;
    private const int MaxErrorMessageLength = 1000;
    private const int SessionTokenLength = 32;
    private const int SessionExpirationHours = 24;
    
    // 快取鍵定義
    private const string AdminCacheKey = "Admin_{0}";
    private const string AllAdminsCacheKey = "AllAdmins";
    private const string AdminDashboardCacheKey = "AdminDashboard";
    private const string UserSearchCacheKey = "UserSearch_{0}_{1}_{2}_{3}_{4}_{5}";
    private const string RecentSystemLogsCacheKey = "RecentSystemLogs";
    private const string RecentModerationActionsCacheKey = "RecentModerationActions";
    private const string RecentAdminActionsCacheKey = "RecentAdminActions";

    public AdminBackofficeService(
        GameCoreDbContext context, 
        IMemoryCache memoryCache, 
        ILogger<AdminBackofficeService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request, string ipAddress, string userAgent)
    {
        // 輸入驗證
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Username cannot be null or empty", nameof(request.Username));
        
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password cannot be null or empty", nameof(request.Password));
        
        if (request.Username.Length > MaxUsernameLength)
            throw new ArgumentException($"Username cannot exceed {MaxUsernameLength} characters", nameof(request.Username));
        
        if (request.Password.Length > MaxPasswordLength)
            throw new ArgumentException($"Password cannot exceed {MaxPasswordLength} characters", nameof(request.Password));
        
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be null or empty", nameof(ipAddress));
        
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("User agent cannot be null or empty", nameof(userAgent));

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
        // 輸入驗證
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new ArgumentException("Session token cannot be null or empty", nameof(sessionToken));

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
        // 輸入驗證
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new ArgumentException("Session token cannot be null or empty", nameof(sessionToken));

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
        // 輸入驗證
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new ArgumentException("Session token cannot be null or empty", nameof(sessionToken));

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
        // 檢查快取
        if (_memoryCache.TryGetValue(AllAdminsCacheKey, out List<AdminDto> cachedAdmins))
        {
            _logger.LogDebug("Retrieved all admins from cache");
            return cachedAdmins;
        }

        var admins = await _context.Admins
            .Include(a => a.ManagerRole)
            .AsNoTracking()
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

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(AllAdminsCacheKey, admins, cacheOptions);

        _logger.LogDebug("Cached all admins");
        return admins;
    }

    public async Task<AdminDto> GetAdminByIdAsync(int adminId)
    {
        // 輸入驗證
        if (adminId <= 0)
            throw new ArgumentException("Admin ID must be a positive integer", nameof(adminId));

        // 檢查快取
        var cacheKey = string.Format(AdminCacheKey, adminId);
        if (_memoryCache.TryGetValue(cacheKey, out AdminDto cachedAdmin))
        {
            _logger.LogDebug("Retrieved admin {AdminId} from cache", adminId);
            return cachedAdmin;
        }

        var admin = await _context.Admins
            .Include(a => a.ManagerRole)
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin == null)
            throw new InvalidOperationException("Admin not found");

        var result = new AdminDto
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

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, result, cacheOptions);

        _logger.LogDebug("Cached admin {AdminId}", adminId);
        return result;
    }

    public async Task<AdminDto> CreateAdminAsync(CreateAdminRequestDto request, int createdByAdminId)
    {
        // 輸入驗證
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Username cannot be null or empty", nameof(request.Username));
        
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email cannot be null or empty", nameof(request.Email));
        
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password cannot be null or empty", nameof(request.Password));
        
        if (request.Username.Length > MaxUsernameLength)
            throw new ArgumentException($"Username cannot exceed {MaxUsernameLength} characters", nameof(request.Username));
        
        if (request.Email.Length > MaxEmailLength)
            throw new ArgumentException($"Email cannot exceed {MaxEmailLength} characters", nameof(request.Email));
        
        if (request.Password.Length > MaxPasswordLength)
            throw new ArgumentException($"Password cannot exceed {MaxPasswordLength} characters", nameof(request.Password));
        
        if (createdByAdminId <= 0)
            throw new ArgumentException("Created by admin ID must be a positive integer", nameof(createdByAdminId));

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

        // 清除相關快取
        ClearAllAdminBackofficeCache();

        return await GetAdminByIdAsync(admin.Id);
    }

    public async Task<List<UserDto>> GetUsersAsync(UserSearchRequestDto request)
    {
        // 輸入驗證
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (request.Page <= 0)
            throw new ArgumentException("Page must be a positive integer", nameof(request.Page));
        
        if (request.PageSize <= 0)
            request.PageSize = DefaultPageSize;
        else if (request.PageSize > MaxPageSize)
            request.PageSize = MaxPageSize;

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

        // 檢查快取
        var cacheKey = string.Format(UserSearchCacheKey, 
            request.SearchTerm ?? "", 
            request.Status ?? "", 
            request.IsActive?.ToString() ?? "", 
            request.CreatedAfter?.ToString("yyyy-MM-dd") ?? "", 
            request.CreatedBefore?.ToString("yyyy-MM-dd") ?? "", 
            $"{request.Page}_{request.PageSize}");
        
        if (_memoryCache.TryGetValue(cacheKey, out List<UserDto> cachedUsers))
        {
            _logger.LogDebug("Retrieved user search results from cache for page {Page}", request.Page);
            return cachedUsers;
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
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

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, users, cacheOptions);

        _logger.LogDebug("Cached user search results for page {Page}", request.Page);
        return users;
    }

    public async Task<bool> SuspendUserAsync(int userId, SuspendUserRequestDto request, int adminId)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ArgumentException("Reason cannot be null or empty", nameof(request.Reason));
        
        if (request.Reason.Length > MaxReasonLength)
            throw new ArgumentException($"Reason cannot exceed {MaxReasonLength} characters", nameof(request.Reason));
        
        if (!string.IsNullOrEmpty(request.Details) && request.Details.Length > MaxDetailsLength)
            throw new ArgumentException($"Details cannot exceed {MaxDetailsLength} characters", nameof(request.Details));
        
        if (adminId <= 0)
            throw new ArgumentException("Admin ID must be a positive integer", nameof(adminId));

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

        // 清除相關快取
        ClearUserRelatedCache(userId);

        return true;
    }

    public async Task<bool> BanUserAsync(int userId, BanUserRequestDto request, int adminId)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new ArgumentException("Reason cannot be null or empty", nameof(request.Reason));
        
        if (request.Reason.Length > MaxReasonLength)
            throw new ArgumentException($"Reason cannot exceed {MaxReasonLength} characters", nameof(request.Reason));
        
        if (!string.IsNullOrEmpty(request.Details) && request.Details.Length > MaxDetailsLength)
            throw new ArgumentException($"Details cannot exceed {MaxDetailsLength} characters", nameof(request.Details));
        
        if (adminId <= 0)
            throw new ArgumentException("Admin ID must be a positive integer", nameof(adminId));

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

        // 清除相關快取
        ClearUserRelatedCache(userId);

        return true;
    }

    public async Task<bool> LogAdminActionAsync(LogAdminActionRequestDto request)
    {
        // 輸入驗證
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (request.AdminId <= 0)
            throw new ArgumentException("Admin ID must be a positive integer", nameof(request.AdminId));
        
        if (string.IsNullOrWhiteSpace(request.Action))
            throw new ArgumentException("Action cannot be null or empty", nameof(request.Action));
        
        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ArgumentException("Category cannot be null or empty", nameof(request.Category));
        
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Description cannot be null or empty", nameof(request.Description));
        
        if (request.Description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description cannot exceed {MaxDescriptionLength} characters", nameof(request.Description));
        
        if (!string.IsNullOrEmpty(request.Details) && request.Details.Length > MaxDetailsLength)
            throw new ArgumentException($"Details cannot exceed {MaxDetailsLength} characters", nameof(request.Details));
        
        if (!string.IsNullOrEmpty(request.ErrorMessage) && request.ErrorMessage.Length > MaxErrorMessageLength)
            throw new ArgumentException($"Error message cannot exceed {MaxErrorMessageLength} characters", nameof(request.ErrorMessage));

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

        // 清除相關快取
        _memoryCache.Remove(RecentAdminActionsCacheKey);
        _memoryCache.Remove(AdminDashboardCacheKey);

        return true;
    }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        // 檢查快取
        if (_memoryCache.TryGetValue(AdminDashboardCacheKey, out AdminDashboardDto cachedDashboard))
        {
            _logger.LogDebug("Retrieved admin dashboard from cache");
            return cachedDashboard;
        }

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

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(AdminDashboardCacheKey, result, cacheOptions);

        _logger.LogDebug("Cached admin dashboard");
        return result;
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

    #region 快取管理

    /// <summary>
    /// 清除管理員相關的快取
    /// </summary>
    private void ClearAdminRelatedCache(int adminId)
    {
        var adminKey = string.Format(AdminCacheKey, adminId);
        _memoryCache.Remove(adminKey);
        _memoryCache.Remove(AllAdminsCacheKey);
        _memoryCache.Remove(AdminDashboardCacheKey);

        _logger.LogDebug("Cleared cache for admin {AdminId}", adminId);
    }

    /// <summary>
    /// 清除用戶相關的快取
    /// </summary>
    private void ClearUserRelatedCache(int userId)
    {
        _memoryCache.Remove(AdminDashboardCacheKey);
        // 清除用戶搜尋快取（需要遍歷所有可能的搜尋條件）
        // 這裡簡化處理，直接清除所有相關快取
        _memoryCache.Remove(RecentSystemLogsCacheKey);
        _memoryCache.Remove(RecentModerationActionsCacheKey);
        _memoryCache.Remove(RecentAdminActionsCacheKey);

        _logger.LogDebug("Cleared cache for user {UserId}", userId);
    }

    /// <summary>
    /// 清除所有管理後台快取
    /// </summary>
    private void ClearAllAdminBackofficeCache()
    {
        _memoryCache.Remove(AllAdminsCacheKey);
        _memoryCache.Remove(AdminDashboardCacheKey);
        _memoryCache.Remove(RecentSystemLogsCacheKey);
        _memoryCache.Remove(RecentModerationActionsCacheKey);
        _memoryCache.Remove(RecentAdminActionsCacheKey);

        _logger.LogDebug("Cleared all admin backoffice cache");
    }

    #endregion

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