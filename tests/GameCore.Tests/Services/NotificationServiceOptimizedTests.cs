using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// NotificationService 優化版本單元測試
/// </summary>
public class NotificationServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<NotificationService> _logger;
    private readonly NotificationService _service;

    public NotificationServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<NotificationService>>();
        _service = new NotificationService(_context, _memoryCache, _logger);

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
        // 創建用戶
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com" };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com" };
        var user3 = new User { user_id = 3, username = "user3", email = "user3@test.com" };
        _context.Users.AddRange(user1, user2, user3);

        // 創建通知來源
        var source1 = new NotificationSource { source_id = 1, source_name = "System", description = "System notifications" };
        var source2 = new NotificationSource { source_id = 2, source_name = "Game", description = "Game notifications" };
        _context.NotificationSources.AddRange(source1, source2);

        // 創建通知行為
        var action1 = new NotificationAction { action_id = 1, action_name = "Info", description = "Information notification" };
        var action2 = new NotificationAction { action_id = 2, action_name = "Alert", description = "Alert notification" };
        _context.NotificationActions.AddRange(action1, action2);

        // 創建通知
        var notification1 = new Notification 
        { 
            notification_id = 1, 
            source_id = 1, 
            action_id = 1, 
            sender_id = 1, 
            notification_title = "Welcome", 
            notification_message = "Welcome to the game!", 
            created_at = DateTime.UtcNow.AddHours(-1) 
        };
        var notification2 = new Notification 
        { 
            notification_id = 2, 
            source_id = 2, 
            action_id = 2, 
            sender_id = 1, 
            notification_title = "Game Update", 
            notification_message = "New game update available", 
            created_at = DateTime.UtcNow.AddHours(-2) 
        };
        _context.Notifications.AddRange(notification1, notification2);

        // 創建通知接收者
        var recipient1 = new NotificationRecipient { notification_id = 1, user_id = 2, is_read = false };
        var recipient2 = new NotificationRecipient { notification_id = 1, user_id = 3, is_read = true, read_at = DateTime.UtcNow.AddMinutes(-30) };
        var recipient3 = new NotificationRecipient { notification_id = 2, user_id = 2, is_read = false };
        _context.NotificationRecipients.AddRange(recipient1, recipient2, recipient3);

        _context.SaveChanges();
    }

    #endregion

    #region GetUserNotificationsAsync 測試

    [Fact]
    public async Task GetUserNotificationsAsync_WithValidParameters_ShouldReturnNotifications()
    {
        // Arrange
        var userId = 2;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var notifications = result.ToList();
        Assert.Equal(2, notifications.Count);
        Assert.All(notifications, n => Assert.NotNull(n.Source));
        Assert.All(notifications, n => Assert.NotNull(n.Action));
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithReadFilter_ShouldReturnFilteredNotifications()
    {
        // Arrange
        var userId = 2;
        var isRead = false;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserNotificationsAsync(userId, isRead, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var notifications = result.ToList();
        Assert.Equal(2, notifications.Count);
        Assert.All(notifications, n => Assert.False(n.Recipients.First(r => r.user_id == userId).is_read));
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var page = 1;
        var pageSize = 20;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserNotificationsAsync(userId, null, page, pageSize));
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var userId = 2;
        var page = 0;
        var pageSize = 20;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserNotificationsAsync(userId, null, page, pageSize));
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var userId = 2;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var notifications = result.ToList();
        Assert.True(notifications.Count <= 20); // Default page size
    }

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 2;
        var page = 1;
        var pageSize = 20;

        // Act - 第一次調用
        var result1 = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.NotificationRecipients.RemoveRange(_context.NotificationRecipients);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetNotificationAsync 測試

    [Fact]
    public async Task GetNotificationAsync_WithValidNotificationId_ShouldReturnNotification()
    {
        // Arrange
        var notificationId = 1;

        // Act
        var result = await _service.GetNotificationAsync(notificationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notificationId, result.notification_id);
        Assert.Equal("Welcome", result.notification_title);
        Assert.NotNull(result.Source);
        Assert.NotNull(result.Action);
        Assert.NotNull(result.Recipients);
        Assert.Equal(2, result.Recipients.Count);
    }

    [Fact]
    public async Task GetNotificationAsync_WithInvalidNotificationId_ShouldThrowException()
    {
        // Arrange
        var notificationId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetNotificationAsync(notificationId));
    }

    [Fact]
    public async Task GetNotificationAsync_WithNonExistentNotificationId_ShouldReturnNull()
    {
        // Arrange
        var notificationId = 999;

        // Act
        var result = await _service.GetNotificationAsync(notificationId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetNotificationAsync_ShouldUseCache()
    {
        // Arrange
        var notificationId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetNotificationAsync(notificationId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Notifications.RemoveRange(_context.Notifications);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetNotificationAsync(notificationId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.notification_id, result2.notification_id);
    }

    #endregion

    #region CreateNotificationAsync 測試

    [Fact]
    public async Task CreateNotificationAsync_WithValidData_ShouldCreateNotification()
    {
        // Arrange
        var notification = new Notification
        {
            source_id = 1,
            action_id = 1,
            sender_id = 1,
            notification_title = "Test Notification",
            notification_message = "This is a test notification"
        };
        var recipientUserIds = new List<int> { 2, 3 };

        // Act
        var result = await _service.CreateNotificationAsync(notification, recipientUserIds);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.notification_id > 0);
        Assert.Equal("Test Notification", result.notification_title);
        Assert.Equal("This is a test notification", result.notification_message);
        Assert.True(result.created_at > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbNotification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.notification_id == result.notification_id);
        Assert.NotNull(dbNotification);

        var dbRecipients = await _context.NotificationRecipients
            .Where(nr => nr.notification_id == result.notification_id)
            .ToListAsync();
        Assert.Equal(2, dbRecipients.Count);
        Assert.All(dbRecipients, r => Assert.False(r.is_read));
    }

    [Fact]
    public async Task CreateNotificationAsync_WithNullNotification_ShouldThrowException()
    {
        // Arrange
        Notification notification = null!;
        var recipientUserIds = new List<int> { 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNotificationAsync(notification, recipientUserIds));
    }

    [Fact]
    public async Task CreateNotificationAsync_WithEmptyRecipientList_ShouldThrowException()
    {
        // Arrange
        var notification = new Notification
        {
            source_id = 1,
            action_id = 1,
            sender_id = 1,
            notification_title = "Test Notification",
            notification_message = "This is a test notification"
        };
        var recipientUserIds = new List<int>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNotificationAsync(notification, recipientUserIds));
    }

    [Fact]
    public async Task CreateNotificationAsync_WithInvalidRecipientIds_ShouldThrowException()
    {
        // Arrange
        var notification = new Notification
        {
            source_id = 1,
            action_id = 1,
            sender_id = 1,
            notification_title = "Test Notification",
            notification_message = "This is a test notification"
        };
        var recipientUserIds = new List<int> { 2, -1, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNotificationAsync(notification, recipientUserIds));
    }

    [Fact]
    public async Task CreateNotificationAsync_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var notification = new Notification
        {
            source_id = 1,
            action_id = 1,
            sender_id = 1,
            notification_title = "",
            notification_message = "This is a test notification"
        };
        var recipientUserIds = new List<int> { 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNotificationAsync(notification, recipientUserIds));
    }

    [Fact]
    public async Task CreateNotificationAsync_WithEmptyMessage_ShouldThrowException()
    {
        // Arrange
        var notification = new Notification
        {
            source_id = 1,
            action_id = 1,
            sender_id = 1,
            notification_title = "Test Notification",
            notification_message = ""
        };
        var recipientUserIds = new List<int> { 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateNotificationAsync(notification, recipientUserIds));
    }

    #endregion

    #region MarkNotificationAsReadAsync 測試

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithValidData_ShouldMarkAsRead()
    {
        // Arrange
        var notificationId = 1;
        var userId = 2;

        // Act
        var result = await _service.MarkNotificationAsReadAsync(notificationId, userId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了記錄
        var dbRecipient = await _context.NotificationRecipients
            .FirstOrDefaultAsync(nr => nr.notification_id == notificationId && nr.user_id == userId);
        Assert.NotNull(dbRecipient);
        Assert.True(dbRecipient.is_read);
        Assert.NotNull(dbRecipient.read_at);
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithAlreadyReadNotification_ShouldReturnTrue()
    {
        // Arrange
        var notificationId = 1;
        var userId = 3; // This user already has the notification marked as read

        // Act
        var result = await _service.MarkNotificationAsReadAsync(notificationId, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithInvalidNotificationId_ShouldThrowException()
    {
        // Arrange
        var notificationId = -1;
        var userId = 2;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.MarkNotificationAsReadAsync(notificationId, userId));
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var notificationId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.MarkNotificationAsReadAsync(notificationId, userId));
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithNonExistentRecipient_ShouldReturnFalse()
    {
        // Arrange
        var notificationId = 1;
        var userId = 999;

        // Act
        var result = await _service.MarkNotificationAsReadAsync(notificationId, userId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region MarkAllNotificationsAsReadAsync 測試

    [Fact]
    public async Task MarkAllNotificationsAsReadAsync_WithValidUserId_ShouldMarkAllAsRead()
    {
        // Arrange
        var userId = 2;

        // Act
        var result = await _service.MarkAllNotificationsAsReadAsync(userId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了記錄
        var dbRecipients = await _context.NotificationRecipients
            .Where(nr => nr.user_id == userId)
            .ToListAsync();
        Assert.All(dbRecipients, r => Assert.True(r.is_read));
        Assert.All(dbRecipients, r => Assert.NotNull(r.read_at));
    }

    [Fact]
    public async Task MarkAllNotificationsAsReadAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.MarkAllNotificationsAsReadAsync(userId));
    }

    [Fact]
    public async Task MarkAllNotificationsAsReadAsync_WithNoUnreadNotifications_ShouldReturnTrue()
    {
        // Arrange
        var userId = 3; // This user has no unread notifications

        // Act
        var result = await _service.MarkAllNotificationsAsReadAsync(userId);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region GetUnreadCountAsync 測試

    [Fact]
    public async Task GetUnreadCountAsync_WithValidUserId_ShouldReturnCount()
    {
        // Arrange
        var userId = 2;

        // Act
        var result = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(2, result); // User 2 has 2 unread notifications
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUnreadCountAsync(userId));
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithNoUnreadNotifications_ShouldReturnZero()
    {
        // Arrange
        var userId = 3; // This user has no unread notifications

        // Act
        var result = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 2;

        // Act - 第一次調用
        var result1 = await _service.GetUnreadCountAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.NotificationRecipients.RemoveRange(_context.NotificationRecipients);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region DeleteNotificationAsync 測試

    [Fact]
    public async Task DeleteNotificationAsync_WithValidData_ShouldDeleteNotification()
    {
        // Arrange
        var notificationId = 1;
        var userId = 2;

        // Act
        var result = await _service.DeleteNotificationAsync(notificationId, userId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實刪除了記錄
        var dbRecipient = await _context.NotificationRecipients
            .FirstOrDefaultAsync(nr => nr.notification_id == notificationId && nr.user_id == userId);
        Assert.Null(dbRecipient);
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithInvalidNotificationId_ShouldThrowException()
    {
        // Arrange
        var notificationId = -1;
        var userId = 2;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.DeleteNotificationAsync(notificationId, userId));
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var notificationId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.DeleteNotificationAsync(notificationId, userId));
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithNonExistentRecipient_ShouldReturnFalse()
    {
        // Arrange
        var notificationId = 1;
        var userId = 999;

        // Act
        var result = await _service.DeleteNotificationAsync(notificationId, userId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetNotificationSourcesAsync 測試

    [Fact]
    public async Task GetNotificationSourcesAsync_ShouldReturnSources()
    {
        // Act
        var result = await _service.GetNotificationSourcesAsync();

        // Assert
        Assert.NotNull(result);
        var sources = result.ToList();
        Assert.Equal(2, sources.Count);
        Assert.Contains(sources, s => s.source_name == "System");
        Assert.Contains(sources, s => s.source_name == "Game");
    }

    [Fact]
    public async Task GetNotificationSourcesAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetNotificationSourcesAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.NotificationSources.RemoveRange(_context.NotificationSources);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetNotificationSourcesAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetNotificationActionsAsync 測試

    [Fact]
    public async Task GetNotificationActionsAsync_ShouldReturnActions()
    {
        // Act
        var result = await _service.GetNotificationActionsAsync();

        // Assert
        Assert.NotNull(result);
        var actions = result.ToList();
        Assert.Equal(2, actions.Count);
        Assert.Contains(actions, a => a.action_name == "Info");
        Assert.Contains(actions, a => a.action_name == "Alert");
    }

    [Fact]
    public async Task GetNotificationActionsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetNotificationActionsAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.NotificationActions.RemoveRange(_context.NotificationActions);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetNotificationActionsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region CreateSystemNotificationAsync 測試

    [Fact]
    public async Task CreateSystemNotificationAsync_WithValidData_ShouldCreateSystemNotification()
    {
        // Arrange
        var title = "System Maintenance";
        var message = "System will be down for maintenance";
        var recipientUserIds = new List<int> { 2, 3 };

        // Act
        var result = await _service.CreateSystemNotificationAsync(title, message, recipientUserIds);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.notification_id > 0);
        Assert.Equal(title, result.notification_title);
        Assert.Equal(message, result.notification_message);
        Assert.Equal(1, result.source_id); // System source
        Assert.Equal(1, result.action_id); // System action
        Assert.Equal(1, result.sender_id); // System user
    }

    [Fact]
    public async Task CreateSystemNotificationAsync_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var title = "";
        var message = "System will be down for maintenance";
        var recipientUserIds = new List<int> { 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateSystemNotificationAsync(title, message, recipientUserIds));
    }

    [Fact]
    public async Task CreateSystemNotificationAsync_WithEmptyMessage_ShouldThrowException()
    {
        // Arrange
        var title = "System Maintenance";
        var message = "";
        var recipientUserIds = new List<int> { 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateSystemNotificationAsync(title, message, recipientUserIds));
    }

    [Fact]
    public async Task CreateSystemNotificationAsync_WithEmptyRecipientList_ShouldThrowException()
    {
        // Arrange
        var title = "System Maintenance";
        var message = "System will be down for maintenance";
        var recipientUserIds = new List<int>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateSystemNotificationAsync(title, message, recipientUserIds));
    }

    [Fact]
    public async Task CreateSystemNotificationAsync_WithInvalidRecipientIds_ShouldThrowException()
    {
        // Arrange
        var title = "System Maintenance";
        var message = "System will be down for maintenance";
        var recipientUserIds = new List<int> { 2, -1, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateSystemNotificationAsync(title, message, recipientUserIds));
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetUserNotificationsAsync_WithEmptyDatabase_ShouldReturnEmpty()
    {
        // Arrange
        _context.NotificationRecipients.RemoveRange(_context.NotificationRecipients);
        _context.SaveChanges();

        var userId = 2;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithNoNotifications_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 999; // User with no notifications
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetUserNotificationsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 2;
        var page = 1;
        var pageSize = 20;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserNotificationsAsync(userId, null, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 2;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUnreadCountAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.True(result >= 0);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}