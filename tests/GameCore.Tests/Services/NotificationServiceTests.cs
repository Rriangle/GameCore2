using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services
{
    /// <summary>
    /// 通知服務單元測試
    /// </summary>
    public class NotificationServiceTests
    {
        private readonly DbContextOptions<GameCoreDbContext> _options;
        private readonly Mock<ILogger<NotificationService>> _mockLogger;

        public NotificationServiceTests()
        {
            _options = new DbContextOptionsBuilder<GameCoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILogger<NotificationService>>();
        }

        [Fact]
        public async Task GetUserNotificationsAsync_ShouldReturnUserNotifications()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            // 創建測試數據
            var notificationSource = new NotificationSource { source_id = 1, source_name = "系統", created_at = DateTime.UtcNow };
            var notificationAction = new NotificationAction { action_id = 1, action_name = "通知", created_at = DateTime.UtcNow };
            var user = new User { UserId = 1, Username = "testuser", Email = "test@test.com", PasswordHash = "hash" };

            context.NotificationSources.Add(notificationSource);
            context.NotificationActions.Add(notificationAction);
            context.Users.Add(user);

            var notification = new Notification
            {
                notification_id = 1,
                source_id = 1,
                action_id = 1,
                sender_id = 1,
                notification_title = "測試通知",
                notification_message = "這是一個測試通知",
                created_at = DateTime.UtcNow
            };

            context.Notifications.Add(notification);

            var recipient = new NotificationRecipient
            {
                recipient_id = 1,
                notification_id = 1,
                user_id = 1,
                is_read = false
            };

            context.NotificationRecipients.Add(recipient);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUserNotificationsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstNotification = result.First();
            Assert.Equal("測試通知", firstNotification.notification_title);
        }

        [Fact]
        public async Task CreateNotificationAsync_ShouldCreateNotificationAndRecipients()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var notification = new Notification
            {
                source_id = 1,
                action_id = 1,
                sender_id = 1,
                notification_title = "新通知",
                notification_message = "這是一個新通知",
                created_at = DateTime.UtcNow
            };

            var recipientUserIds = new List<int> { 1, 2 };

            // Act
            var result = await service.CreateNotificationAsync(notification, recipientUserIds);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.notification_id > 0);

            var savedRecipients = await context.NotificationRecipients
                .Where(nr => nr.notification_id == result.notification_id)
                .ToListAsync();

            Assert.Equal(2, savedRecipients.Count);
        }

        [Fact]
        public async Task MarkNotificationAsReadAsync_ShouldMarkNotificationAsRead()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var recipient = new NotificationRecipient
            {
                recipient_id = 1,
                notification_id = 1,
                user_id = 1,
                is_read = false
            };

            context.NotificationRecipients.Add(recipient);
            await context.SaveChangesAsync();

            // Act
            var result = await service.MarkNotificationAsReadAsync(1, 1);

            // Assert
            Assert.True(result);

            var updatedRecipient = await context.NotificationRecipients.FindAsync(1);
            Assert.NotNull(updatedRecipient);
            Assert.True(updatedRecipient.is_read);
            Assert.NotNull(updatedRecipient.read_at);
        }

        [Fact]
        public async Task GetUnreadCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient { recipient_id = 1, notification_id = 1, user_id = 1, is_read = false },
                new NotificationRecipient { recipient_id = 2, notification_id = 2, user_id = 1, is_read = true },
                new NotificationRecipient { recipient_id = 3, notification_id = 3, user_id = 1, is_read = false }
            };

            context.NotificationRecipients.AddRange(recipients);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUnreadCountAsync(1);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetNotificationSourcesAsync_ShouldReturnAllSources()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var sources = new List<NotificationSource>
            {
                new NotificationSource { source_id = 1, source_name = "系統", created_at = DateTime.UtcNow },
                new NotificationSource { source_id = 2, source_name = "論壇", created_at = DateTime.UtcNow }
            };

            context.NotificationSources.AddRange(sources);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetNotificationSourcesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.source_name == "系統");
            Assert.Contains(result, s => s.source_name == "論壇");
        }

        [Fact]
        public async Task GetNotificationActionsAsync_ShouldReturnAllActions()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var actions = new List<NotificationAction>
            {
                new NotificationAction { action_id = 1, action_name = "通知", created_at = DateTime.UtcNow },
                new NotificationAction { action_id = 2, action_name = "提醒", created_at = DateTime.UtcNow }
            };

            context.NotificationActions.AddRange(actions);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetNotificationActionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.action_name == "通知");
            Assert.Contains(result, a => a.action_name == "提醒");
        }

        [Fact]
        public async Task CreateSystemNotificationAsync_ShouldCreateSystemNotification()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var title = "系統維護通知";
            var message = "系統將於今晚進行維護";
            var recipientUserIds = new List<int> { 1, 2, 3 };

            // Act
            var result = await service.CreateSystemNotificationAsync(title, message, recipientUserIds);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(title, result.notification_title);
            Assert.Equal(message, result.notification_message);
            Assert.Equal(1, result.source_id); // 假設 1 是系統來源
            Assert.Equal(1, result.action_id); // 假設 1 是系統行為

            var savedRecipients = await context.NotificationRecipients
                .Where(nr => nr.notification_id == result.notification_id)
                .ToListAsync();

            Assert.Equal(3, savedRecipients.Count);
        }

        [Fact]
        public async Task MarkAllNotificationsAsReadAsync_ShouldMarkAllNotificationsAsRead()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var recipients = new List<NotificationRecipient>
            {
                new NotificationRecipient { recipient_id = 1, notification_id = 1, user_id = 1, is_read = false },
                new NotificationRecipient { recipient_id = 2, notification_id = 2, user_id = 1, is_read = false },
                new NotificationRecipient { recipient_id = 3, notification_id = 3, user_id = 1, is_read = false }
            };

            context.NotificationRecipients.AddRange(recipients);
            await context.SaveChangesAsync();

            // Act
            var result = await service.MarkAllNotificationsAsReadAsync(1);

            // Assert
            Assert.True(result);

            var updatedRecipients = await context.NotificationRecipients
                .Where(nr => nr.user_id == 1)
                .ToListAsync();

            Assert.All(updatedRecipients, r => Assert.True(r.is_read));
            Assert.All(updatedRecipients, r => Assert.NotNull(r.read_at));
        }

        [Fact]
        public async Task DeleteNotificationAsync_ShouldDeleteRecipient()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            var recipient = new NotificationRecipient
            {
                recipient_id = 1,
                notification_id = 1,
                user_id = 1,
                is_read = false
            };

            context.NotificationRecipients.Add(recipient);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteNotificationAsync(1, 1);

            // Assert
            Assert.True(result);

            var deletedRecipient = await context.NotificationRecipients.FindAsync(1);
            Assert.Null(deletedRecipient);
        }

        [Fact]
        public async Task GetUserNotificationsAsync_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new NotificationService(context, _mockLogger.Object);

            // 創建多個通知
            for (int i = 1; i <= 5; i++)
            {
                var notification = new Notification
                {
                    notification_id = i,
                    source_id = 1,
                    action_id = 1,
                    sender_id = 1,
                    notification_title = $"通知 {i}",
                    notification_message = $"這是通知 {i}",
                    created_at = DateTime.UtcNow.AddDays(-i)
                };

                context.Notifications.Add(notification);

                var recipient = new NotificationRecipient
                {
                    recipient_id = i,
                    notification_id = i,
                    user_id = 1,
                    is_read = false
                };

                context.NotificationRecipients.Add(recipient);
            }

            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUserNotificationsAsync(1, page: 2, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            // 由於按時間倒序排列，第二頁應該包含通知 3 和 4
            Assert.Contains(result, n => n.notification_title == "通知 3");
            Assert.Contains(result, n => n.notification_title == "通知 4");
        }
    }
}