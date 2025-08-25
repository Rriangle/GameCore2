using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 通知服務實現
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(GameCoreDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取用戶通知列表
        /// </summary>
        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool? isRead = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.NotificationRecipients
                    .Include(nr => nr.Notification)
                        .ThenInclude(n => n.Source)
                    .Include(nr => nr.Notification)
                        .ThenInclude(n => n.Action)
                    .Include(nr => nr.Notification)
                        .ThenInclude(n => n.Sender)
                    .Include(nr => nr.Notification)
                        .ThenInclude(n => n.SenderManager)
                    .Include(nr => nr.Notification)
                        .ThenInclude(n => n.Group)
                    .Where(nr => nr.user_id == userId);

                if (isRead.HasValue)
                {
                    query = query.Where(nr => nr.is_read == isRead.Value);
                }

                var notifications = await query
                    .OrderByDescending(nr => nr.Notification.created_at)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(nr => nr.Notification)
                    .ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 通知列表時發生錯誤", userId);
                throw;
            }
        }

        /// <summary>
        /// 獲取單個通知
        /// </summary>
        public async Task<Notification?> GetNotificationAsync(int notificationId)
        {
            try
            {
                return await _context.Notifications
                    .Include(n => n.Source)
                    .Include(n => n.Action)
                    .Include(n => n.Sender)
                    .Include(n => n.SenderManager)
                    .Include(n => n.Group)
                    .Include(n => n.Recipients)
                    .FirstOrDefaultAsync(n => n.notification_id == notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知 {NotificationId} 時發生錯誤", notificationId);
                throw;
            }
        }

        /// <summary>
        /// 創建通知
        /// </summary>
        public async Task<Notification> CreateNotificationAsync(Notification notification, List<int> recipientUserIds)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 創建通知
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    // 創建接收者記錄
                    var recipients = recipientUserIds.Select(userId => new NotificationRecipient
                    {
                        notification_id = notification.notification_id,
                        user_id = userId,
                        is_read = false
                    }).ToList();

                    _context.NotificationRecipients.AddRange(recipients);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return notification;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建通知時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 標記通知為已讀
        /// </summary>
        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            try
            {
                var recipient = await _context.NotificationRecipients
                    .FirstOrDefaultAsync(nr => nr.notification_id == notificationId && nr.user_id == userId);

                if (recipient == null)
                    return false;

                recipient.is_read = true;
                recipient.read_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記通知 {NotificationId} 為已讀時發生錯誤", notificationId);
                throw;
            }
        }

        /// <summary>
        /// 標記所有通知為已讀
        /// </summary>
        public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
        {
            try
            {
                var recipients = await _context.NotificationRecipients
                    .Where(nr => nr.user_id == userId && !nr.is_read)
                    .ToListAsync();

                foreach (var recipient in recipients)
                {
                    recipient.is_read = true;
                    recipient.read_at = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記用戶 {UserId} 所有通知為已讀時發生錯誤", userId);
                throw;
            }
        }

        /// <summary>
        /// 獲取未讀通知數量
        /// </summary>
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                return await _context.NotificationRecipients
                    .CountAsync(nr => nr.user_id == userId && !nr.is_read);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 未讀通知數量時發生錯誤", userId);
                throw;
            }
        }

        /// <summary>
        /// 刪除通知
        /// </summary>
        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            try
            {
                var recipient = await _context.NotificationRecipients
                    .FirstOrDefaultAsync(nr => nr.notification_id == notificationId && nr.user_id == userId);

                if (recipient == null)
                    return false;

                _context.NotificationRecipients.Remove(recipient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除通知 {NotificationId} 時發生錯誤", notificationId);
                throw;
            }
        }

        /// <summary>
        /// 獲取通知來源列表
        /// </summary>
        public async Task<IEnumerable<NotificationSource>> GetNotificationSourcesAsync()
        {
            try
            {
                return await _context.NotificationSources
                    .OrderBy(ns => ns.source_name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知來源列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 獲取通知行為列表
        /// </summary>
        public async Task<IEnumerable<NotificationAction>> GetNotificationActionsAsync()
        {
            try
            {
                return await _context.NotificationActions
                    .OrderBy(na => na.action_name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知行為列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 創建系統通知
        /// </summary>
        public async Task<Notification> CreateSystemNotificationAsync(string title, string message, List<int> recipientUserIds, int? senderManagerId = null)
        {
            try
            {
                var notification = new Notification
                {
                    source_id = 1, // 假設 1 是系統來源
                    action_id = 1, // 假設 1 是系統行為
                    sender_id = 1, // 假設 1 是系統用戶
                    sender_manager_id = senderManagerId,
                    notification_title = title,
                    notification_message = message,
                    created_at = DateTime.UtcNow
                };

                return await CreateNotificationAsync(notification, recipientUserIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建系統通知時發生錯誤");
                throw;
            }
        }
    }
}