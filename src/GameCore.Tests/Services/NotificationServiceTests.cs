using Xunit;
using Moq;
using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Tests.Services;

/// <summary>
/// 通知服務測試
/// </summary>
public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _mockNotificationRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly NotificationService _notificationService;

    public NotificationServiceTests()
    {
        _mockNotificationRepository = new Mock<INotificationRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _notificationService = new NotificationService(
            _mockNotificationRepository.Object,
            _mockUserRepository.Object,
            Mock.Of<ILogger<NotificationService>>());
    }

    /// <summary>
    /// 測試取得用戶通知列表
    /// </summary>
    [Fact]
    public async Task GetUserNotificationsAsync_ShouldReturnNotifications()
    {
        // Arrange
        var userId = 1;
        var notifications = new List<Notification>
        {
            new Notification
            {
                NotificationId = 1,
                SourceId = 1,
                ActionId = 1,
                SenderId = 2,
                NotificationTitle = "測試通知",
                NotificationMessage = "這是一個測試通知",
                CreatedAt = DateTime.UtcNow
            }
        };

        var sources = new List<NotificationSource>
        {
            new NotificationSource { SourceId = 1, SourceName = "系統" }
        };

        var actions = new List<NotificationAction>
        {
            new NotificationAction { ActionId = 1, ActionName = "系統通知" }
        };

        var sender = new User { UserId = 2, UserName = "測試用戶" };

        _mockNotificationRepository.Setup(x => x.GetByUserIdAsync(userId, 1, 20, null))
            .ReturnsAsync(notifications);
        _mockNotificationRepository.Setup(x => x.GetSourcesAsync())
            .ReturnsAsync(sources);
        _mockNotificationRepository.Setup(x => x.GetActionsAsync())
            .ReturnsAsync(actions);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(sender);

        // Act
        var result = await _notificationService.GetUserNotificationsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var notification = result.First();
        Assert.Equal(1, notification.NotificationId);
        Assert.Equal("測試通知", notification.Title);
        Assert.Equal("這是一個測試通知", notification.Message);
    }

    /// <summary>
    /// 測試建立通知
    /// </summary>
    [Fact]
    public async Task CreateNotificationAsync_ShouldCreateNotification()
    {
        // Arrange
        var request = new CreateNotificationRequest
        {
            SourceId = 1,
            ActionId = 1,
            SenderId = 1,
            Title = "測試通知",
            Message = "這是一個測試通知",
            RecipientIds = new List<int> { 2, 3 }
        };

        var sender = new User { UserId = 1, UserName = "發送者" };
        var createdNotification = new Notification
        {
            NotificationId = 1,
            SourceId = 1,
            ActionId = 1,
            SenderId = 1,
            NotificationTitle = "測試通知",
            NotificationMessage = "這是一個測試通知",
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(sender);
        _mockNotificationRepository.Setup(x => x.AddAsync(It.IsAny<Notification>()))
            .ReturnsAsync(createdNotification);

        // Act
        var result = await _notificationService.CreateNotificationAsync(request);

        // Assert
        Assert.True(result);
        _mockNotificationRepository.Verify(x => x.AddAsync(It.IsAny<Notification>()), Times.Once);
    }

    /// <summary>
    /// 測試建立通知 - 發送者不存在
    /// </summary>
    [Fact]
    public async Task CreateNotificationAsync_WithInvalidSender_ShouldReturnFalse()
    {
        // Arrange
        var request = new CreateNotificationRequest
        {
            SourceId = 1,
            ActionId = 1,
            SenderId = 999,
            Title = "測試通知",
            Message = "這是一個測試通知",
            RecipientIds = new List<int> { 2 }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _notificationService.CreateNotificationAsync(request);

        // Assert
        Assert.False(result);
        _mockNotificationRepository.Verify(x => x.AddAsync(It.IsAny<Notification>()), Times.Never);
    }

    /// <summary>
    /// 測試標記通知為已讀
    /// </summary>
    [Fact]
    public async Task MarkAsReadAsync_ShouldMarkAsRead()
    {
        // Arrange
        var notificationId = 1;
        var userId = 1;

        _mockNotificationRepository.Setup(x => x.MarkAsReadAsync(notificationId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _notificationService.MarkAsReadAsync(notificationId, userId);

        // Assert
        Assert.True(result);
        _mockNotificationRepository.Verify(x => x.MarkAsReadAsync(notificationId, userId), Times.Once);
    }

    /// <summary>
    /// 測試標記所有通知為已讀
    /// </summary>
    [Fact]
    public async Task MarkAllAsReadAsync_ShouldMarkAllAsRead()
    {
        // Arrange
        var userId = 1;
        var expectedCount = 5;

        _mockNotificationRepository.Setup(x => x.MarkAllAsReadAsync(userId))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _notificationService.MarkAllAsReadAsync(userId);

        // Assert
        Assert.Equal(expectedCount, result);
        _mockNotificationRepository.Verify(x => x.MarkAllAsReadAsync(userId), Times.Once);
    }

    /// <summary>
    /// 測試取得通知統計
    /// </summary>
    [Fact]
    public async Task GetNotificationStatsAsync_ShouldReturnStats()
    {
        // Arrange
        var userId = 1;
        var unreadCount = 3;

        _mockNotificationRepository.Setup(x => x.GetUnreadCountAsync(userId))
            .ReturnsAsync(unreadCount);
        _mockNotificationRepository.Setup(x => x.GetByUserIdAsync(userId, 1, int.MaxValue, null))
            .ReturnsAsync(new List<Notification>());

        // Act
        var result = await _notificationService.GetNotificationStatsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(unreadCount, result.UnreadCount);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TodayCount);
    }

    /// <summary>
    /// 測試刪除通知
    /// </summary>
    [Fact]
    public async Task DeleteNotificationAsync_ShouldDeleteNotification()
    {
        // Arrange
        var notificationId = 1;
        var userId = 1;
        var notification = new Notification
        {
            NotificationId = notificationId,
            SenderId = userId
        };

        _mockNotificationRepository.Setup(x => x.GetByIdAsync(notificationId))
            .ReturnsAsync(notification);
        _mockNotificationRepository.Setup(x => x.DeleteAsync(notificationId))
            .ReturnsAsync(true);

        // Act
        var result = await _notificationService.DeleteNotificationAsync(notificationId, userId);

        // Assert
        Assert.True(result);
        _mockNotificationRepository.Verify(x => x.DeleteAsync(notificationId), Times.Once);
    }

    /// <summary>
    /// 測試刪除通知 - 通知不存在
    /// </summary>
    [Fact]
    public async Task DeleteNotificationAsync_WithInvalidNotification_ShouldReturnFalse()
    {
        // Arrange
        var notificationId = 999;
        var userId = 1;

        _mockNotificationRepository.Setup(x => x.GetByIdAsync(notificationId))
            .ReturnsAsync((Notification?)null);

        // Act
        var result = await _notificationService.DeleteNotificationAsync(notificationId, userId);

        // Assert
        Assert.False(result);
        _mockNotificationRepository.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試建立系統通知
    /// </summary>
    [Fact]
    public async Task CreateSystemNotificationAsync_ShouldCreateSystemNotification()
    {
        // Arrange
        var title = "系統通知";
        var message = "這是一個系統通知";
        var recipientIds = new List<int> { 1, 2, 3 };

        var sender = new User { UserId = 1, UserName = "系統" };
        var createdNotification = new Notification
        {
            NotificationId = 1,
            SourceId = 1,
            ActionId = 1,
            SenderId = 1,
            NotificationTitle = title,
            NotificationMessage = message,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(sender);
        _mockNotificationRepository.Setup(x => x.AddAsync(It.IsAny<Notification>()))
            .ReturnsAsync(createdNotification);

        // Act
        var result = await _notificationService.CreateSystemNotificationAsync(title, message, recipientIds);

        // Assert
        Assert.True(result);
        _mockNotificationRepository.Verify(x => x.AddAsync(It.IsAny<Notification>()), Times.Once);
    }
} 