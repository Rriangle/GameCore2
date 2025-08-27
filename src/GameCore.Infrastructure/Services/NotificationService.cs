using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 通知服務實現 - 優化版本
    /// 增強性能、快取、輸入驗證、錯誤處理和可維護性
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly GameCoreDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<NotificationService> _logger;

        // 常數定義，提高可維護性
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private const int CacheExpirationMinutes = 15;
        private const string UserNotificationsCacheKey = "Notifications_User_{0}_{1}_{2}_{3}";
        private const string NotificationCacheKey = "Notification_{0}";
        private const string UnreadCountCacheKey = "UnreadCount_{0}";
        private const string SourcesCacheKey = "NotificationSources";
        private const string ActionsCacheKey = "NotificationActions";

        public NotificationService(
            GameCoreDbContext context,
            IMemoryCache memoryCache,
            ILogger<NotificationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 獲取用戶通知列表
        /// </summary>
        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool? isRead = null, int page = 1, int pageSize = 20)
        {
            try
            {
                // 輸入驗證
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (page <= 0)
                    throw new ArgumentException("頁碼必須為正整數", nameof(page));
                
                if (pageSize <= 0 || pageSize > MaxPageSize)
                    pageSize = DefaultPageSize;

                // 快取鍵
                var cacheKey = string.Format(UserNotificationsCacheKey, userId, isRead?.ToString() ?? "all", page, pageSize);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Notification> cachedNotifications))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 的通知列表", userId);
                    return cachedNotifications;
                }

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
                    .Where(nr => nr.user_id == userId)
                    .AsNoTracking();

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

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, notifications, cacheOptions);

                _logger.LogInformation("獲取用戶 {UserId} 的通知列表，共 {Count} 條", userId, notifications.Count());
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
                // 輸入驗證
                if (notificationId <= 0)
                    throw new ArgumentException("通知ID必須為正整數", nameof(notificationId));

                // 快取鍵
                var cacheKey = string.Format(NotificationCacheKey, notificationId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out Notification cachedNotification))
                {
                    _logger.LogDebug("從快取獲取通知 {NotificationId}", notificationId);
                    return cachedNotification;
                }

                var notification = await _context.Notifications
                    .Include(n => n.Source)
                    .Include(n => n.Action)
                    .Include(n => n.Sender)
                    .Include(n => n.SenderManager)
                    .Include(n => n.Group)
                    .Include(n => n.Recipients)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.notification_id == notificationId);

                if (notification != null)
                {
                    // 設置快取
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                    _memoryCache.Set(cacheKey, notification, cacheOptions);
                }

                _logger.LogDebug("獲取通知 {NotificationId}，結果: {Result}", notificationId, notification != null ? "找到" : "未找到");
                return notification;
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
                // 輸入驗證
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification));
                
                if (recipientUserIds == null || !recipientUserIds.Any())
                    throw new ArgumentException("接收者用戶ID列表不能為空", nameof(recipientUserIds));
                
                if (recipientUserIds.Any(id => id <= 0))
                    throw new ArgumentException("所有接收者用戶ID必須為正整數", nameof(recipientUserIds));

                // 驗證通知內容
                if (string.IsNullOrWhiteSpace(notification.notification_title))
                    throw new ArgumentException("通知標題不能為空", nameof(notification.notification_title));
                
                if (string.IsNullOrWhiteSpace(notification.notification_message))
                    throw new ArgumentException("通知內容不能為空", nameof(notification.notification_message));

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

                    // 清除相關快取
                    ClearNotificationRelatedCache(notification.notification_id);
                    foreach (var userId in recipientUserIds)
                    {
                        ClearUserNotificationCache(userId);
                    }

                    _logger.LogInformation("成功創建通知 {NotificationId}，發送給 {Count} 個用戶", 
                        notification.notification_id, recipientUserIds.Count);
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
                // 輸入驗證
                if (notificationId <= 0)
                    throw new ArgumentException("通知ID必須為正整數", nameof(notificationId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                var recipient = await _context.NotificationRecipients
                    .FirstOrDefaultAsync(nr => nr.notification_id == notificationId && nr.user_id == userId);

                if (recipient == null)
                {
                    _logger.LogWarning("用戶 {UserId} 嘗試標記不存在的通知 {NotificationId} 為已讀", userId, notificationId);
                    return false;
                }

                if (recipient.is_read)
                {
                    _logger.LogDebug("通知 {NotificationId} 已經被用戶 {UserId} 標記為已讀", notificationId, userId);
                    return true;
                }

                recipient.is_read = true;
                recipient.read_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearNotificationRelatedCache(notificationId);
                ClearUserNotificationCache(userId);

                _logger.LogInformation("用戶 {UserId} 成功標記通知 {NotificationId} 為已讀", userId, notificationId);
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
                // 輸入驗證
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                var recipients = await _context.NotificationRecipients
                    .Where(nr => nr.user_id == userId && !nr.is_read)
                    .ToListAsync();

                if (!recipients.Any())
                {
                    _logger.LogDebug("用戶 {UserId} 沒有未讀通知", userId);
                    return true;
                }

                foreach (var recipient in recipients)
                {
                    recipient.is_read = true;
                    recipient.read_at = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearUserNotificationCache(userId);

                _logger.LogInformation("用戶 {UserId} 成功標記 {Count} 條通知為已讀", userId, recipients.Count);
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
                // 輸入驗證
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                // 快取鍵
                var cacheKey = string.Format(UnreadCountCacheKey, userId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out int cachedCount))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 的未讀通知數量: {Count}", userId, cachedCount);
                    return cachedCount;
                }

                var count = await _context.NotificationRecipients
                    .AsNoTracking()
                    .CountAsync(nr => nr.user_id == userId && !nr.is_read);

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, count, cacheOptions);

                _logger.LogDebug("用戶 {UserId} 的未讀通知數量: {Count}", userId, count);
                return count;
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
                // 輸入驗證
                if (notificationId <= 0)
                    throw new ArgumentException("通知ID必須為正整數", nameof(notificationId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                var recipient = await _context.NotificationRecipients
                    .FirstOrDefaultAsync(nr => nr.notification_id == notificationId && nr.user_id == userId);

                if (recipient == null)
                {
                    _logger.LogWarning("用戶 {UserId} 嘗試刪除不存在的通知 {NotificationId}", userId, notificationId);
                    return false;
                }

                _context.NotificationRecipients.Remove(recipient);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearNotificationRelatedCache(notificationId);
                ClearUserNotificationCache(userId);

                _logger.LogInformation("用戶 {UserId} 成功刪除通知 {NotificationId}", userId, notificationId);
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
                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(SourcesCacheKey, out IEnumerable<NotificationSource> cachedSources))
                {
                    _logger.LogDebug("從快取獲取通知來源列表");
                    return cachedSources;
                }

                var sources = await _context.NotificationSources
                    .AsNoTracking()
                    .OrderBy(ns => ns.source_name)
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes * 2)); // 來源數據變化較少，快取時間更長
                _memoryCache.Set(SourcesCacheKey, sources, cacheOptions);

                _logger.LogDebug("獲取通知來源列表，共 {Count} 個", sources.Count);
                return sources;
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
                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(ActionsCacheKey, out IEnumerable<NotificationAction> cachedActions))
                {
                    _logger.LogDebug("從快取獲取通知行為列表");
                    return cachedActions;
                }

                var actions = await _context.NotificationActions
                    .AsNoTracking()
                    .OrderBy(na => na.action_name)
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes * 2)); // 行為數據變化較少，快取時間更長
                _memoryCache.Set(ActionsCacheKey, actions, cacheOptions);

                _logger.LogDebug("獲取通知行為列表，共 {Count} 個", actions.Count);
                return actions;
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
                // 輸入驗證
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("通知標題不能為空", nameof(title));
                
                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentException("通知內容不能為空", nameof(message));
                
                if (recipientUserIds == null || !recipientUserIds.Any())
                    throw new ArgumentException("接收者用戶ID列表不能為空", nameof(recipientUserIds));
                
                if (recipientUserIds.Any(id => id <= 0))
                    throw new ArgumentException("所有接收者用戶ID必須為正整數", nameof(recipientUserIds));

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

                var result = await CreateNotificationAsync(notification, recipientUserIds);

                _logger.LogInformation("成功創建系統通知: {Title}，發送給 {Count} 個用戶", title, recipientUserIds.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建系統通知時發生錯誤");
                throw;
            }
        }

        #region 快取管理

        /// <summary>
        /// 清除通知相關快取
        /// </summary>
        private void ClearNotificationRelatedCache(int notificationId)
        {
            var cacheKey = string.Format(NotificationCacheKey, notificationId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除通知 {NotificationId} 的快取", notificationId);
        }

        /// <summary>
        /// 清除用戶通知相關快取
        /// </summary>
        private void ClearUserNotificationCache(int userId)
        {
            // 清除用戶通知列表快取（需要清除所有頁面的快取）
            for (int page = 1; page <= 5; page++) // 清除前5頁的快取
            {
                for (int pageSize = 10; pageSize <= MaxPageSize; pageSize += 10)
                {
                    var cacheKey = string.Format(UserNotificationsCacheKey, userId, "all", page, pageSize);
                    _memoryCache.Remove(cacheKey);
                    
                    cacheKey = string.Format(UserNotificationsCacheKey, userId, "true", page, pageSize);
                    _memoryCache.Remove(cacheKey);
                    
                    cacheKey = string.Format(UserNotificationsCacheKey, userId, "false", page, pageSize);
                    _memoryCache.Remove(cacheKey);
                }
            }

            // 清除未讀數量快取
            var unreadCountKey = string.Format(UnreadCountCacheKey, userId);
            _memoryCache.Remove(unreadCountKey);

            _logger.LogDebug("清除用戶 {UserId} 的通知相關快取", userId);
        }

        #endregion
    }
}