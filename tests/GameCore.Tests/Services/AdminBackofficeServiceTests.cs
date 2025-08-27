using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.AdminDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Tests.Services;

public class AdminBackofficeServiceTests
{
    private readonly GameCoreDbContext _context;
    private readonly AdminBackofficeService _service;
    private readonly Mock<ILogger<AdminBackofficeService>> _loggerMock;

    public AdminBackofficeServiceTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _loggerMock = new Mock<ILogger<AdminBackofficeService>>();
        _service = new AdminBackofficeService(_context, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test admin
        var admin = new Admin
        {
            Id = 1,
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = HashPassword("password123"),
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Admins.Add(admin);

        // Create test user
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            IsActive = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(user);

        // Create manager role
        var role = new ManagerRole
        {
            Id = 1,
            Name = "Admin",
            Description = "Administrator role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.ManagerRoles.Add(role);

        // Create role permissions
        var permissions = new List<ManagerRolePermission>
        {
            new ManagerRolePermission
            {
                Id = 1,
                RoleId = 1,
                Permission = "UserManagement",
                IsGranted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new ManagerRolePermission
            {
                Id = 2,
                RoleId = 1,
                Permission = "SystemAdmin",
                IsGranted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            }
        };
        _context.ManagerRolePermissions.AddRange(permissions);

        // Assign role to admin
        admin.ManagerRole = role;

        _context.SaveChanges();
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "admin",
            Password = "password123"
        };

        // Act
        var result = await _service.LoginAsync(request, "127.0.0.1", "TestAgent");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.AdminId);
        Assert.Equal("admin", result.Username);
        Assert.Equal("admin@example.com", result.Email);
        Assert.NotNull(result.SessionToken);
        Assert.Equal("Admin", result.Role);
        Assert.Contains("UserManagement", result.Permissions);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "admin",
            Password = "wrongpassword"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.LoginAsync(request, "127.0.0.1", "TestAgent"));
    }

    [Fact]
    public async Task LoginAsync_NonExistentUser_ShouldThrowException()
    {
        // Arrange
        var request = new AdminLoginRequestDto
        {
            Username = "nonexistent",
            Password = "password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.LoginAsync(request, "127.0.0.1", "TestAgent"));
    }

    [Fact]
    public async Task LogoutAsync_ValidSession_ShouldReturnTrue()
    {
        // Arrange
        var session = new AdminSession
        {
            AdminId = 1,
            SessionToken = "testtoken",
            Status = "Active",
            LoginTime = DateTime.UtcNow.AddHours(-1),
            LastActivityTime = DateTime.UtcNow.AddMinutes(-30),
            ExpiresAt = DateTime.UtcNow.AddHours(23),
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdminSessions.Add(session);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.LogoutAsync("testtoken");

        // Assert
        Assert.True(result);
        var updatedSession = await _context.AdminSessions.FindAsync(session.Id);
        Assert.Equal("Revoked", updatedSession.Status);
    }

    [Fact]
    public async Task LogoutAsync_InvalidSession_ShouldReturnFalse()
    {
        // Act
        var result = await _service.LogoutAsync("invalidtoken");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateSessionAsync_ValidSession_ShouldReturnSession()
    {
        // Arrange
        var session = new AdminSession
        {
            AdminId = 1,
            SessionToken = "validtoken",
            Status = "Active",
            LoginTime = DateTime.UtcNow.AddHours(-1),
            LastActivityTime = DateTime.UtcNow.AddMinutes(-30),
            ExpiresAt = DateTime.UtcNow.AddHours(23),
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdminSessions.Add(session);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ValidateSessionAsync("validtoken");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.AdminId);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task ValidateSessionAsync_ExpiredSession_ShouldThrowException()
    {
        // Arrange
        var session = new AdminSession
        {
            AdminId = 1,
            SessionToken = "expiredtoken",
            Status = "Active",
            LoginTime = DateTime.UtcNow.AddHours(-25),
            LastActivityTime = DateTime.UtcNow.AddHours(-24),
            ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expired
            CreatedAt = DateTime.UtcNow.AddHours(-25),
            UpdatedAt = DateTime.UtcNow.AddHours(-25)
        };
        _context.AdminSessions.Add(session);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.ValidateSessionAsync("expiredtoken"));
    }

    [Fact]
    public async Task GetAllAdminsAsync_ShouldReturnAdmins()
    {
        // Act
        var result = await _service.GetAllAdminsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var admin = result.First();
        Assert.Equal("admin", admin.Username);
        Assert.Equal("admin@example.com", admin.Email);
        Assert.Equal("Admin", admin.Role);
    }

    [Fact]
    public async Task GetAdminByIdAsync_ValidId_ShouldReturnAdmin()
    {
        // Act
        var result = await _service.GetAdminByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("admin", result.Username);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task GetAdminByIdAsync_InvalidId_ShouldThrowException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GetAdminByIdAsync(999));
    }

    [Fact]
    public async Task CreateAdminAsync_ValidRequest_ShouldCreateAdmin()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "newadmin",
            Email = "newadmin@example.com",
            Password = "password123"
        };

        // Act
        var result = await _service.CreateAdminAsync(request, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newadmin", result.Username);
        Assert.Equal("newadmin@example.com", result.Email);
        Assert.Equal("No Role", result.Role);

        // Verify admin was created in database
        var createdAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == "newadmin");
        Assert.NotNull(createdAdmin);
    }

    [Fact]
    public async Task CreateAdminAsync_DuplicateUsername_ShouldThrowException()
    {
        // Arrange
        var request = new CreateAdminRequestDto
        {
            Username = "admin", // Already exists
            Email = "another@example.com",
            Password = "password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateAdminAsync(request, 1));
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsers()
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
        Assert.Single(result);
        var user = result.First();
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Active", user.Status);
    }

    [Fact]
    public async Task SuspendUserAsync_ValidUser_ShouldSuspendUser()
    {
        // Arrange
        var request = new SuspendUserRequestDto
        {
            Reason = "Violation of community guidelines",
            Details = "User posted inappropriate content",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = await _service.SuspendUserAsync(1, request, 1);

        // Assert
        Assert.True(result);

        // Verify user was suspended
        var user = await _context.Users.FindAsync(1);
        Assert.Equal("Suspended", user.Status);

        // Verify moderation action was created
        var moderationAction = await _context.ModerationActions
            .FirstOrDefaultAsync(ma => ma.TargetId == 1 && ma.Action == "Suspend");
        Assert.NotNull(moderationAction);
        Assert.Equal("Active", moderationAction.Status);
    }

    [Fact]
    public async Task BanUserAsync_ValidUser_ShouldBanUser()
    {
        // Arrange
        var request = new BanUserRequestDto
        {
            Reason = "Repeated violations",
            Details = "Multiple community guideline violations",
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _service.BanUserAsync(1, request, 1);

        // Assert
        Assert.True(result);

        // Verify user was banned
        var user = await _context.Users.FindAsync(1);
        Assert.Equal("Banned", user.Status);

        // Verify moderation action was created
        var moderationAction = await _context.ModerationActions
            .FirstOrDefaultAsync(ma => ma.TargetId == 1 && ma.Action == "Ban");
        Assert.NotNull(moderationAction);
        Assert.Equal("Active", moderationAction.Status);
    }

    [Fact]
    public async Task GetAdminDashboardAsync_ShouldReturnDashboard()
    {
        // Act
        var result = await _service.GetAdminDashboardAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalUsers);
        Assert.Equal(1, result.TotalAdmins);
        Assert.Equal(0, result.ActiveSessions);
        Assert.NotNull(result.RecentSystemLogs);
        Assert.NotNull(result.RecentModerationActions);
        Assert.NotNull(result.RecentAdminActions);
    }

    [Fact]
    public async Task CreateSystemLogAsync_ValidRequest_ShouldCreateLog()
    {
        // Arrange
        var request = new CreateSystemLogRequestDto
        {
            Level = "Info",
            Category = "System",
            Event = "TestEvent",
            Message = "Test system log message",
            Source = "TestService"
        };

        // Act
        var result = await _service.CreateSystemLogAsync(request);

        // Assert
        Assert.True(result);

        // Verify log was created in database
        var log = await _context.SystemLogs
            .FirstOrDefaultAsync(l => l.Event == "TestEvent");
        Assert.NotNull(log);
        Assert.Equal("Info", log.Level);
        Assert.Equal("System", log.Category);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}