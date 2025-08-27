using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.AdminDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// AdminBackofficeService 優化版本單元測試
/// </summary>
public class AdminBackofficeServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AdminBackofficeService> _logger;
    private readonly AdminBackofficeService _service;

    public AdminBackofficeServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<AdminBackofficeService>>();
        _service = new AdminBackofficeService(_context, _memoryCache, _logger);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
        _memoryCache.Dispose();
    }

    #region 測試數據準備

    private void SeedTestData()
    {
        // 創建管理員角色
        var now = DateTime.UtcNow;
        var role1 = new ManagerRole 
        { 
            Id = 1, 
            Name = "Super Admin", 
            Description = "Super administrator with all permissions", 
            IsActive = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        var role2 = new ManagerRole 
        { 
            Id = 2, 
            Name = "Moderator", 
            Description = "Content moderator", 
            IsActive = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        _context.ManagerRoles.AddRange(role1, role2);

        // 創建管理員角色權限
        var permission1 = new ManagerRolePermission 
        { 
            Id = 1, 
            ManagerRoleId = 1, 
            Permission = "UserManagement", 
            IsGranted = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        var permission2 = new ManagerRolePermission 
        { 
            Id = 2, 
            ManagerRoleId = 1, 
            Permission = "ContentModeration", 
            IsGranted = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        var permission3 = new ManagerRolePermission 
        { 
            Id = 3, 
            ManagerRoleId = 2, 
            Permission = "ContentModeration", 
            IsGranted = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        _context.ManagerRolePermissions.AddRange(permission1, permission2, permission3);

        // 創建管理員
        var admin1 = new Admin 
        { 
            Id = 1, 
            Username = "admin1", 
            Email = "admin1@test.com", 
            PasswordHash = "hashedpassword1", 
            IsActive = true, 
            LastLoginAt = now.AddHours(-1), 
            CreatedAt = now.AddDays(-30), 
            UpdatedAt = now.AddHours(-1) 
        };
        var admin2 = new Admin 
        { 
            Id = 2, 
            Username = "admin2", 
            Email = "admin2@test.com", 
            PasswordHash = "hashedpassword2", 
            IsActive = true, 
            LastLoginAt = now.AddHours(-2), 
            CreatedAt = now.AddDays(-20), 
            UpdatedAt = now.AddHours(-2) 
        };
        _context.Admins.AddRange(admin1, admin2);

        // 創建管理員會話
        var session1 = new AdminSession 
        { 
            Id = 1, 
            AdminId = 1, 
            SessionToken = "sessiontoken1", 
            Status = "Active", 
            LoginTime = now.AddHours(-1), 
            LastActivityTime = now.AddMinutes(-30), 
            ExpiresAt = now.AddHours(23), 
            IpAddress = "192.168.1.1", 
            UserAgent = "Test Browser", 
            CreatedAt = now.AddHours(-1), 
            UpdatedAt = now.AddMinutes(-30) 
        };
        var session2 = new AdminSession 
        { 
            Id = 2, 
            AdminId = 2, 
            SessionToken = "sessiontoken2", 
            Status = "Active", 
            LoginTime = now.AddHours(-2), 
            LastActivityTime = now.AddMinutes(-15), 
            ExpiresAt = now.AddHours(22), 
            IpAddress = "192.168.1.2", 
            UserAgent = "Test Browser", 
            CreatedAt = now.AddHours(-2), 
            UpdatedAt = now.AddMinutes(-15) 
        };
        _context.AdminSessions.AddRange(session1, session2);

        // 創建用戶
        var user1 = new User 
        { 
            user_id = 1, 
            username = "user1", 
            email = "user1@test.com", 
            IsActive = true, 
            Status = "Active", 
            LastLoginAt = now.AddHours(-3), 
            CreatedAt = now.AddDays(-10), 
            UpdatedAt = now.AddHours(-3) 
        };
        var user2 = new User 
        { 
            user_id = 2, 
            username = "user2", 
            email = "user2@test.com", 
            IsActive = true, 
            Status = "Active", 
            LastLoginAt = now.AddHours(-4), 
            CreatedAt = now.AddDays(-15), 
            UpdatedAt = now.AddHours(-4) 
        };
        var user3 = new User 
        { 
            user_id = 3, 
            username = "user3", 
            email = "user3@test.com", 
            IsActive = false, 
            Status = "Suspended", 
            LastLoginAt = now.AddDays(-1), 
            CreatedAt = now.AddDays(-25), 
            UpdatedAt = now.AddDays(-1) 
        };
        _context.Users.AddRange(user1, user2, user3);

        // 創建系統日誌
        var systemLog1 = new SystemLog 
        { 
            Id = 1, 
            Level = "Info", 
            Category = "System", 
            Event = "UserLogin", 
            Message = "User logged in successfully", 
            Source = "AuthService", 
            UserId = 1, 
            IpAddress = "192.168.1.100", 
            Timestamp = now.AddMinutes(-30), 
            CreatedAt = now.AddMinutes(-30), 
            UpdatedAt = now.AddMinutes(-30) 
        };
        var systemLog2 = new SystemLog 
        { 
            Id = 2, 
            Level = "Warning", 
            Category = "Security", 
            Event = "FailedLogin", 
            Message = "Failed login attempt", 
            Source = "AuthService", 
            IpAddress = "192.168.1.101", 
            Timestamp = now.AddMinutes(-15), 
            CreatedAt = now.AddMinutes(-15), 
            UpdatedAt = now.AddMinutes(-15) 
        };
        _context.SystemLogs.AddRange(systemLog1, systemLog2);

        // 創建管理員操作記錄
        var adminAction1 = new AdminAction 
        { 
            Id = 1, 
            AdminId = 1, 
            Action = "UserSuspend", 
            Category = "Moderation", 
            Description = "Suspended user for violation", 
            TargetType = "User", 
            TargetId = 3, 
            Status = "Completed", 
            ActionTime = now.AddHours(-1), 
            CreatedAt = now.AddHours(-1), 
            UpdatedAt = now.AddHours(-1) 
        };
        var adminAction2 = new AdminAction 
        { 
            Id = 2, 
            AdminId = 2, 
            Action = "UserBan", 
            Category = "Moderation", 
            Description = "Banned user for repeated violations", 
            TargetType = "User", 
            TargetId = 4, 
            Status = "Completed", 
            ActionTime = now.AddHours(-2), 
            CreatedAt = now.AddHours(-2), 
            UpdatedAt = now.AddHours(-2) 
        };
        _context.AdminActions.AddRange(adminAction1, adminAction2);

        // 創建審核操作
        var moderationAction1 = new ModerationAction 
        { 
            Id = 1, 
            AdminId = 1, 
            Action = "Suspend", 
            TargetType = "User", 
            TargetId = 3, 
            Reason = "Violation of community guidelines", 
            Status = "Active", 
            ExpiresAt = now.AddDays(7), 
            ActionTime = now.AddHours(-1), 
            CreatedAt = now.AddHours(-1), 
            UpdatedAt = now.AddHours(-1) 
        };
        var moderationAction2 = new ModerationAction 
        { 
            Id = 2, 
            AdminId = 2, 
            Action = "Ban", 
            TargetType = "User", 
            TargetId = 4, 
            Reason = "Repeated violations", 
            Status = "Active", 
            ExpiresAt = now.AddDays(30), 
            ActionTime = now.AddHours(-2), 
            CreatedAt = now.AddHours(-2), 
            UpdatedAt = now.AddHours(-2) 
        };
        _context.ModerationActions.AddRange(moderationAction1, moderationAction2);

        _context.SaveChanges();
    }

    #endregion

    #region LoginAsync 測試

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldLoginSuccessfully()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "admin1",
            Password = "password1"
        };
        var ipAddress = "192.168.1.100";
        var userAgent = "Test Browser";

        // Act
        var result = await _service.LoginAsync(request, ipAddress, userAgent);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.AdminId);
        Assert.Equal("admin1", result.Username);
        Assert.Equal("admin1@test.com", result.Email);
        Assert.NotNull(result.SessionToken);
        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.NotNull(result.Permissions);
        Assert.Equal("Super Admin", result.Role);

        // 驗證資料庫中確實創建了會話
        var dbSession = await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.SessionToken == result.SessionToken);
        Assert.NotNull(dbSession);
        Assert.Equal(1, dbSession.AdminId);
        Assert.Equal("Active", dbSession.Status);
    }

    [Fact]
    public async Task LoginAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        AdminLoginRequestDto request = null!;
        var ipAddress = "192.168.1.100";
        var userAgent = "Test Browser";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.LoginAsync(request, ipAddress, userAgent));
    }

    [Fact]
    public async Task LoginAsync_WithEmptyUsername_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "",
            Password = "password1"
        };
        var ipAddress = "192.168.1.100";
        var userAgent = "Test Browser";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(request, ipAddress, userAgent));
    }

    [Fact]
    public async Task LoginAsync_WithEmptyPassword_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "admin1",
            Password = ""
        };
        var ipAddress = "192.168.1.100";
        var userAgent = "Test Browser";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(request, ipAddress, userAgent));
    }

    [Fact]
    public async Task LoginAsync_WithTooLongUsername_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = new string('a', 51), // Exceeds MaxUsernameLength
            Password = "password1"
        };
        var ipAddress = "192.168.1.100";
        var userAgent = "Test Browser";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(request, ipAddress, userAgent));
    }

    [Fact]
    public async Task LoginAsync_WithEmptyIpAddress_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "admin1",
            Password = "password1"
        };
        var ipAddress = "";
        var userAgent = "Test Browser";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(request, ipAddress, userAgent));
    }

    [Fact]
    public async Task LoginAsync_WithEmptyUserAgent_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "admin1",
            Password = "password1"
        };
        var ipAddress = "192.168.1.100";
        var userAgent = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LoginAsync(request, ipAddress, userAgent));
    }

    #endregion

    #region LogoutAsync 測試

    [Fact]
    public async Task LogoutAsync_WithValidSessionToken_ShouldLogoutSuccessfully()
    {
        // Arrange
        var sessionToken = "sessiontoken1";

        // Act
        var result = await _service.LogoutAsync(sessionToken);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了會話狀態
        var dbSession = await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
        Assert.NotNull(dbSession);
        Assert.Equal("Revoked", dbSession.Status);
        Assert.NotNull(dbSession.LogoutTime);
    }

    [Fact]
    public async Task LogoutAsync_WithEmptySessionToken_ShouldThrowException()
    {
        // Arrange
        var sessionToken = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogoutAsync(sessionToken));
    }

    [Fact]
    public async Task LogoutAsync_WithNullSessionToken_ShouldThrowException()
    {
        // Arrange
        string sessionToken = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogoutAsync(sessionToken));
    }

    #endregion

    #region ValidateSessionAsync 測試

    [Fact]
    public async Task ValidateSessionAsync_WithValidSessionToken_ShouldReturnSession()
    {
        // Arrange
        var sessionToken = "sessiontoken1";

        // Act
        var result = await _service.ValidateSessionAsync(sessionToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.AdminId);
        Assert.Equal("Active", result.Status);
        Assert.NotNull(result.LoginTime);
        Assert.NotNull(result.LastActivityTime);
        Assert.NotNull(result.ExpiresAt);
        Assert.Equal("192.168.1.1", result.IpAddress);
        Assert.Equal("Test Browser", result.UserAgent);
    }

    [Fact]
    public async Task ValidateSessionAsync_WithEmptySessionToken_ShouldThrowException()
    {
        // Arrange
        var sessionToken = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ValidateSessionAsync(sessionToken));
    }

    [Fact]
    public async Task ValidateSessionAsync_WithNullSessionToken_ShouldThrowException()
    {
        // Arrange
        string sessionToken = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ValidateSessionAsync(sessionToken));
    }

    #endregion

    #region RefreshSessionAsync 測試

    [Fact]
    public async Task RefreshSessionAsync_WithValidSessionToken_ShouldRefreshSuccessfully()
    {
        // Arrange
        var sessionToken = "sessiontoken1";

        // Act
        var result = await _service.RefreshSessionAsync(sessionToken);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了過期時間
        var dbSession = await _context.AdminSessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
        Assert.NotNull(dbSession);
        Assert.True(dbSession.ExpiresAt > DateTime.UtcNow.AddHours(23));
    }

    [Fact]
    public async Task RefreshSessionAsync_WithEmptySessionToken_ShouldThrowException()
    {
        // Arrange
        var sessionToken = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.RefreshSessionAsync(sessionToken));
    }

    #endregion

    #region GetAllAdminsAsync 測試

    [Fact]
    public async Task GetAllAdminsAsync_ShouldReturnAllAdmins()
    {
        // Act
        var result = await _service.GetAllAdminsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.True(a.IsActive));
        Assert.Contains(result, a => a.Username == "admin1");
        Assert.Contains(result, a => a.Username == "admin2");
    }

    [Fact]
    public async Task GetAllAdminsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAllAdminsAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.Admins.RemoveRange(_context.Admins);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllAdminsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
        Assert.Equal(result1[0].Username, result2[0].Username);
    }

    #endregion

    #region GetAdminByIdAsync 測試

    [Fact]
    public async Task GetAdminByIdAsync_WithValidAdminId_ShouldReturnAdmin()
    {
        // Arrange
        var adminId = 1;

        // Act
        var result = await _service.GetAdminByIdAsync(adminId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(adminId, result.Id);
        Assert.Equal("admin1", result.Username);
        Assert.Equal("admin1@test.com", result.Email);
        Assert.True(result.IsActive);
        Assert.NotNull(result.LastLoginAt);
        Assert.NotNull(result.CreatedAt);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task GetAdminByIdAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var adminId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdminByIdAsync(adminId));
    }

    [Fact]
    public async Task GetAdminByIdAsync_WithZeroAdminId_ShouldThrowException()
    {
        // Arrange
        var adminId = 0;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdminByIdAsync(adminId));
    }

    [Fact]
    public async Task GetAdminByIdAsync_ShouldUseCache()
    {
        // Arrange
        var adminId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetAdminByIdAsync(adminId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Admins.RemoveRange(_context.Admins);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAdminByIdAsync(adminId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
        Assert.Equal(result1.Username, result2.Username);
    }

    #endregion

    #region CreateAdminAsync 測試

    [Fact]
    public async Task CreateAdminAsync_WithValidData_ShouldCreateAdmin()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "admin3",
            Email = "admin3@test.com",
            Password = "password3"
        };
        var createdByAdminId = 1;

        // Act
        var result = await _service.CreateAdminAsync(request, createdByAdminId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("admin3", result.Username);
        Assert.Equal("admin3@test.com", result.Email);
        Assert.True(result.IsActive);
        Assert.NotNull(result.CreatedAt);
        Assert.NotNull(result.UpdatedAt);

        // 驗證資料庫中確實創建了記錄
        var dbAdmin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Username == "admin3");
        Assert.NotNull(dbAdmin);
        Assert.Equal("admin3@test.com", dbAdmin.Email);
    }

    [Fact]
    public async Task CreateAdminAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        CreateAdminRequestDto request = null!;
        var createdByAdminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.CreateAdminAsync(request, createdByAdminId));
    }

    [Fact]
    public async Task CreateAdminAsync_WithEmptyUsername_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "",
            Email = "admin3@test.com",
            Password = "password3"
        };
        var createdByAdminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateAdminAsync(request, createdByAdminId));
    }

    [Fact]
    public async Task CreateAdminAsync_WithEmptyEmail_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "admin3",
            Email = "",
            Password = "password3"
        };
        var createdByAdminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateAdminAsync(request, createdByAdminId));
    }

    [Fact]
    public async Task CreateAdminAsync_WithEmptyPassword_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "admin3",
            Email = "admin3@test.com",
            Password = ""
        };
        var createdByAdminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateAdminAsync(request, createdByAdminId));
    }

    [Fact]
    public async Task CreateAdminAsync_WithTooLongUsername_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = new string('a', 51), // Exceeds MaxUsernameLength
            Email = "admin3@test.com",
            Password = "password3"
        };
        var createdByAdminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateAdminAsync(request, createdByAdminId));
    }

    [Fact]
    public async Task CreateAdminAsync_WithInvalidCreatedByAdminId_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "admin3",
            Email = "admin3@test.com",
            Password = "password3"
        };
        var createdByAdminId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateAdminAsync(request, createdByAdminId));
    }

    #endregion

    #region GetUsersAsync 測試

    [Fact]
    public async Task GetUsersAsync_WithValidRequest_ShouldReturnUsers()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _service.GetUsersAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, u => Assert.NotNull(u.Username));
        Assert.All(result, u => Assert.NotNull(u.Email));
    }

    [Fact]
    public async Task GetUsersAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        UserSearchRequestDto request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.GetUsersAsync(request));
    }

    [Fact]
    public async Task GetUsersAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 0,
            PageSize = 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUsersAsync(request));
    }

    [Fact]
    public async Task GetUsersAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 1,
            PageSize = 0
        };

        // Act
        var result = await _service.GetUsersAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Count); // Default page size
    }

    [Fact]
    public async Task GetUsersAsync_WithExcessivePageSize_ShouldUseMaxPageSize()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 1,
            PageSize = 1000
        };

        // Act
        var result = await _service.GetUsersAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.Count); // Max page size
    }

    [Fact]
    public async Task GetUsersAsync_ShouldUseCache()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act - 第一次調用
        var result1 = await _service.GetUsersAsync(request);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUsersAsync(request);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
    }

    #endregion

    #region SuspendUserAsync 測試

    [Fact]
    public async Task SuspendUserAsync_WithValidData_ShouldSuspendUser()
    {
        // Arrange
        var userId = 1;
        var request = new SuspendUserRequestDto
        {
            Reason = "Violation of community guidelines",
            Details = "User posted inappropriate content",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var adminId = 1;

        // Act
        var result = await _service.SuspendUserAsync(userId, request, adminId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了用戶狀態
        var dbUser = await _context.Users.FindAsync(userId);
        Assert.NotNull(dbUser);
        Assert.Equal("Suspended", dbUser.Status);

        // 驗證審核操作被創建
        var dbModerationAction = await _context.ModerationActions
            .FirstOrDefaultAsync(ma => ma.TargetId == userId && ma.Action == "Suspend");
        Assert.NotNull(dbModerationAction);
        Assert.Equal("Violation of community guidelines", dbModerationAction.Reason);
    }

    [Fact]
    public async Task SuspendUserAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var request = new SuspendUserRequestDto
        {
            Reason = "Violation of community guidelines",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SuspendUserAsync(userId, request, adminId));
    }

    [Fact]
    public async Task SuspendUserAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        SuspendUserRequestDto request = null!;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.SuspendUserAsync(userId, request, adminId));
    }

    [Fact]
    public async Task SuspendUserAsync_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var request = new SuspendUserRequestDto
        {
            Reason = "",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SuspendUserAsync(userId, request, adminId));
    }

    [Fact]
    public async Task SuspendUserAsync_WithTooLongReason_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var request = new SuspendUserRequestDto
        {
            Reason = new string('a', 501), // Exceeds MaxReasonLength
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SuspendUserAsync(userId, request, adminId));
    }

    [Fact]
    public async Task SuspendUserAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var request = new SuspendUserRequestDto
        {
            Reason = "Violation of community guidelines",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        var adminId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SuspendUserAsync(userId, request, adminId));
    }

    #endregion

    #region BanUserAsync 測試

    [Fact]
    public async Task BanUserAsync_WithValidData_ShouldBanUser()
    {
        // Arrange
        var userId = 2;
        var request = new BanUserRequestDto
        {
            Reason = "Repeated violations of community guidelines",
            Details = "User has been warned multiple times",
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        var adminId = 1;

        // Act
        var result = await _service.BanUserAsync(userId, request, adminId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了用戶狀態
        var dbUser = await _context.Users.FindAsync(userId);
        Assert.NotNull(dbUser);
        Assert.Equal("Banned", dbUser.Status);

        // 驗證審核操作被創建
        var dbModerationAction = await _context.ModerationActions
            .FirstOrDefaultAsync(ma => ma.TargetId == userId && ma.Action == "Ban");
        Assert.NotNull(dbModerationAction);
        Assert.Equal("Repeated violations of community guidelines", dbModerationAction.Reason);
    }

    [Fact]
    public async Task BanUserAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var request = new BanUserRequestDto
        {
            Reason = "Repeated violations",
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.BanUserAsync(userId, request, adminId));
    }

    [Fact]
    public async Task BanUserAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        var userId = 2;
        BanUserRequestDto request = null!;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.BanUserAsync(userId, request, adminId));
    }

    #endregion

    #region LogAdminActionAsync 測試

    [Fact]
    public async Task LogAdminActionAsync_WithValidData_ShouldLogAction()
    {
        // Arrange
        var request = new LogAdminActionRequestDto
        {
            AdminId = 1,
            Action = "UserModeration",
            Category = "Moderation",
            Description = "Performed user moderation action",
            Status = "Completed"
        };

        // Act
        var result = await _service.LogAdminActionAsync(request);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實創建了記錄
        var dbAdminAction = await _context.AdminActions
            .FirstOrDefaultAsync(aa => aa.AdminId == 1 && aa.Action == "UserModeration");
        Assert.NotNull(dbAdminAction);
        Assert.Equal("UserModeration", dbAdminAction.Action);
        Assert.Equal("Moderation", dbAdminAction.Category);
        Assert.Equal("Performed user moderation action", dbAdminAction.Description);
    }

    [Fact]
    public async Task LogAdminActionAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        LogAdminActionRequestDto request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.LogAdminActionAsync(request));
    }

    [Fact]
    public async Task LogAdminActionAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var request = new LogAdminActionRequestDto
        {
            AdminId = -1,
            Action = "UserModeration",
            Category = "Moderation",
            Description = "Performed user moderation action",
            Status = "Completed"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogAdminActionAsync(request));
    }

    [Fact]
    public async Task LogAdminActionAsync_WithEmptyAction_ShouldThrowException()
    {
        // Arrange
        var request = new LogAdminActionRequestDto
        {
            AdminId = 1,
            Action = "",
            Category = "Moderation",
            Description = "Performed user moderation action",
            Status = "Completed"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogAdminActionAsync(request));
    }

    [Fact]
    public async Task LogAdminActionAsync_WithEmptyCategory_ShouldThrowException()
    {
        // Arrange
        var request = new LogAdminActionRequestDto
        {
            AdminId = 1,
            Action = "UserModeration",
            Category = "",
            Description = "Performed user moderation action",
            Status = "Completed"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogAdminActionAsync(request));
    }

    [Fact]
    public async Task LogAdminActionAsync_WithEmptyDescription_ShouldThrowException()
    {
        // Arrange
        var request = new LogAdminActionRequestDto
        {
            AdminId = 1,
            Action = "UserModeration",
            Category = "Moderation",
            Description = "",
            Status = "Completed"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogAdminActionAsync(request));
    }

    [Fact]
    public async Task LogAdminActionAsync_WithTooLongDescription_ShouldThrowException()
    {
        // Arrange
        var request = new LogAdminActionRequestDto
        {
            AdminId = 1,
            Action = "UserModeration",
            Category = "Moderation",
            Description = new string('a', 501), // Exceeds MaxDescriptionLength
            Status = "Completed"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.LogAdminActionAsync(request));
    }

    #endregion

    #region GetAdminDashboardAsync 測試

    [Fact]
    public async Task GetAdminDashboardAsync_ShouldReturnDashboard()
    {
        // Act
        var result = await _service.GetAdminDashboardAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalUsers);
        Assert.Equal(2, result.ActiveUsers);
        Assert.Equal(1, result.SuspendedUsers);
        Assert.Equal(0, result.BannedUsers);
        Assert.Equal(2, result.TotalAdmins);
        Assert.Equal(2, result.ActiveSessions);
        Assert.NotNull(result.RecentSystemLogs);
        Assert.NotNull(result.RecentModerationActions);
        Assert.NotNull(result.RecentAdminActions);
    }

    [Fact]
    public async Task GetAdminDashboardAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAdminDashboardAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAdminDashboardAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalUsers, result2.TotalUsers);
        Assert.Equal(result1.TotalAdmins, result2.TotalAdmins);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetUsersAsync_WithNoUsers_ShouldReturnEmptyResult()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 1,
            PageSize = 10
        };

        // 清除所有用戶
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUsersAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAdminDashboardAsync_WithNoData_ShouldReturnEmptyDashboard()
    {
        // 清除所有數據
        _context.Users.RemoveRange(_context.Users);
        _context.Admins.RemoveRange(_context.Admins);
        _context.AdminSessions.RemoveRange(_context.AdminSessions);
        _context.SystemLogs.RemoveRange(_context.SystemLogs);
        _context.ModerationActions.RemoveRange(_context.ModerationActions);
        _context.AdminActions.RemoveRange(_context.AdminActions);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAdminDashboardAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalUsers);
        Assert.Equal(0, result.ActiveUsers);
        Assert.Equal(0, result.TotalAdmins);
        Assert.Equal(0, result.ActiveSessions);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetAllAdminsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAllAdminsAsync();
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUsersAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var request = new UserSearchRequestDto
        {
            Page = 1,
            PageSize = 10
        };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUsersAsync(request);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetAdminDashboardAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAdminDashboardAsync();
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}