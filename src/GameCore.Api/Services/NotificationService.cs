using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class NotificationService : INotificationService
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(GameCoreDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, bool? isRead = null)
    {
        try
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);
            
            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                Title = n.Title,
                Content = n.Content,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
            return Enumerable.Empty<NotificationDto>();
        }
    }

    public async Task<NotificationDto?> GetNotificationAsync(int notificationId, int userId)
    {
        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null) return null;

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Title = notification.Title,
                Content = notification.Content,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification {NotificationId} for user {UserId}", notificationId, userId);
            return null;
        }
    }

    public async Task<NotificationResult> CreateNotificationAsync(int userId, string title, string content, string type)
    {
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification created: User {UserId}, Type {Type}", userId, type);

            return new NotificationResult
            {
                Success = true,
                Message = "通知創建成功",
                Notification = new NotificationDto
                {
                    NotificationId = notification.NotificationId,
                    UserId = notification.UserId,
                    Title = notification.Title,
                    Content = notification.Content,
                    Type = notification.Type,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt,
                    ReadAt = notification.ReadAt
                },
                UnreadCount = await GetUnreadCountAsync(userId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
            return new NotificationResult
            {
                Success = false,
                Message = "通知創建失敗"
            };
        }
    }

    public async Task<NotificationResult> MarkAsReadAsync(int notificationId, int userId)
    {
        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return new NotificationResult
                {
                    Success = false,
                    Message = "通知不存在"
                };
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification marked as read: {NotificationId}, User {UserId}", notificationId, userId);

            return new NotificationResult
            {
                Success = true,
                Message = "通知已標記為已讀",
                UnreadCount = await GetUnreadCountAsync(userId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read: {NotificationId}", notificationId);
            return new NotificationResult
            {
                Success = false,
                Message = "標記已讀失敗"
            };
        }
    }

    public async Task<NotificationResult> MarkAllAsReadAsync(int userId)
    {
        try
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("All notifications marked as read: User {UserId}", userId);

            return new NotificationResult
            {
                Success = true,
                Message = "所有通知已標記為已讀",
                UnreadCount = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read: User {UserId}", userId);
            return new NotificationResult
            {
                Success = false,
                Message = "標記已讀失敗"
            };
        }
    }

    public async Task<NotificationResult> DeleteNotificationAsync(int notificationId, int userId)
    {
        try
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification == null)
            {
                return new NotificationResult
                {
                    Success = false,
                    Message = "通知不存在"
                };
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification deleted: {NotificationId}, User {UserId}", notificationId, userId);

            return new NotificationResult
            {
                Success = true,
                Message = "通知刪除成功",
                UnreadCount = await GetUnreadCountAsync(userId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification: {NotificationId}", notificationId);
            return new NotificationResult
            {
                Success = false,
                Message = "通知刪除失敗"
            };
        }
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        try
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<NotificationResult> CreateSystemNotificationAsync(string title, string content)
    {
        try
        {
            // Get all active users
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();

            var notifications = new List<Notification>();
            foreach (var user in users)
            {
                notifications.Add(new Notification
                {
                    UserId = user.UserId,
                    Title = title,
                    Content = content,
                    Type = "system",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            _logger.LogInformation("System notification created: {Title}, Sent to {Count} users", title, users.Count);

            return new NotificationResult
            {
                Success = true,
                Message = $"系統通知已發送給 {users.Count} 個用戶"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating system notification");
            return new NotificationResult
            {
                Success = false,
                Message = "系統通知創建失敗"
            };
        }
    }

    public async Task<NotificationResult> CreatePointsAdjustmentNotificationAsync(int userId, decimal amount, string reason)
    {
        var title = "點數調整通知";
        var content = $"您的點數已調整 {amount:F2} 點。原因：{reason}";
        
        return await CreateNotificationAsync(userId, title, content, "points_adjustment");
    }

    public async Task<NotificationResult> CreatePetColorChangeNotificationAsync(int userId, string oldColor, string newColor)
    {
        var title = "寵物顏色變更通知";
        var content = $"您的寵物顏色已從 {oldColor} 變更為 {newColor}，已扣除 2000 點。";
        
        return await CreateNotificationAsync(userId, title, content, "pet_color_change");
    }
}