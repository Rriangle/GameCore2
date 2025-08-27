using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 聊天服務實現 - 優化版本
    /// 增強性能、快取、輸入驗證、錯誤處理和可維護性
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly GameCoreDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ChatService> _logger;

        // 常數定義，提高可維護性
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 50;
        private const int MaxMessageLength = 1000;
        private const int CacheExpirationMinutes = 10;
        private const string DirectMessagesCacheKey = "DirectMessages_{0}_{1}_{2}_{3}";
        private const string UnreadCountCacheKey = "ChatUnreadCount_{0}";
        private const string RecentConversationsCacheKey = "RecentConversations_{0}_{1}";

        public ChatService(
            GameCoreDbContext context,
            IMemoryCache memoryCache,
            ILogger<ChatService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 獲取私訊對話
        /// </summary>
        public async Task<IEnumerable<ChatMessage>> GetDirectMessagesAsync(int userId, int peerId, int page = 1, int pageSize = 50)
        {
            try
            {
                // 輸入驗證
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (peerId <= 0)
                    throw new ArgumentException("對等用戶ID必須為正整數", nameof(peerId));
                
                if (page <= 0)
                    throw new ArgumentException("頁碼必須為正整數", nameof(page));
                
                if (pageSize <= 0 || pageSize > MaxPageSize)
                    pageSize = DefaultPageSize;

                // 快取鍵
                var cacheKey = string.Format(DirectMessagesCacheKey, userId, peerId, page, pageSize);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<ChatMessage> cachedMessages))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 與 {PeerId} 的私訊對話，頁面 {Page}", userId, peerId, page);
                    return cachedMessages;
                }

                var messages = await _context.ChatMessages
                    .Include(cm => cm.Sender)
                    .Include(cm => cm.Manager)
                    .Include(cm => cm.Receiver)
                    .Where(cm => (cm.sender_id == userId && cm.receiver_id == peerId) ||
                                (cm.sender_id == peerId && cm.receiver_id == userId))
                    .OrderByDescending(cm => cm.sent_at)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(cm => cm.sent_at)
                    .AsNoTracking()
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, messages, cacheOptions);

                _logger.LogDebug("獲取用戶 {UserId} 與 {PeerId} 的私訊對話，頁面 {Page}，共 {Count} 條", 
                    userId, peerId, page, messages.Count);
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 與 {PeerId} 的私訊時發生錯誤", userId, peerId);
                throw;
            }
        }

        /// <summary>
        /// 發送私訊
        /// </summary>
        public async Task<ChatMessage> SendDirectMessageAsync(int senderId, int receiverId, string content)
        {
            try
            {
                // 輸入驗證
                if (senderId <= 0)
                    throw new ArgumentException("發送者ID必須為正整數", nameof(senderId));
                
                if (receiverId <= 0)
                    throw new ArgumentException("接收者ID必須為正整數", nameof(receiverId));
                
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("訊息內容不能為空", nameof(content));
                
                if (content.Length > MaxMessageLength)
                    throw new ArgumentException($"訊息內容長度不能超過 {MaxMessageLength} 個字符", nameof(content));

                // 檢查是否發送給自己
                if (senderId == receiverId)
                    throw new ArgumentException("不能發送訊息給自己", nameof(receiverId));

                var message = new ChatMessage
                {
                    sender_id = senderId,
                    receiver_id = receiverId,
                    chat_content = content,
                    sent_at = DateTime.UtcNow,
                    is_read = false,
                    is_sent = true
                };

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearDirectMessageCache(senderId, receiverId);
                ClearUnreadCountCache(receiverId);
                ClearRecentConversationsCache(senderId);
                ClearRecentConversationsCache(receiverId);

                _logger.LogInformation("用戶 {SenderId} 成功發送私訊給用戶 {ReceiverId}，訊息ID: {MessageId}", 
                    senderId, receiverId, message.message_id);
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送私訊時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 發送系統訊息
        /// </summary>
        public async Task<ChatMessage> SendSystemMessageAsync(int receiverId, string content, int? managerId = null)
        {
            try
            {
                // 輸入驗證
                if (receiverId <= 0)
                    throw new ArgumentException("接收者ID必須為正整數", nameof(receiverId));
                
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("訊息內容不能為空", nameof(content));
                
                if (content.Length > MaxMessageLength)
                    throw new ArgumentException($"訊息內容長度不能超過 {MaxMessageLength} 個字符", nameof(content));

                var message = new ChatMessage
                {
                    manager_id = managerId,
                    sender_id = 1, // 假設 1 是系統用戶
                    receiver_id = receiverId,
                    chat_content = content,
                    sent_at = DateTime.UtcNow,
                    is_read = false,
                    is_sent = true
                };

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearUnreadCountCache(receiverId);
                ClearRecentConversationsCache(receiverId);

                _logger.LogInformation("系統成功發送訊息給用戶 {ReceiverId}，訊息ID: {MessageId}", 
                    receiverId, message.message_id);
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送系統訊息時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 標記訊息為已讀
        /// </summary>
        public async Task<bool> MarkMessageAsReadAsync(int messageId, int userId)
        {
            try
            {
                // 輸入驗證
                if (messageId <= 0)
                    throw new ArgumentException("訊息ID必須為正整數", nameof(messageId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(cm => cm.message_id == messageId && cm.receiver_id == userId);

                if (message == null)
                {
                    _logger.LogWarning("用戶 {UserId} 嘗試標記不存在的訊息 {MessageId} 為已讀", userId, messageId);
                    return false;
                }

                if (message.is_read)
                {
                    _logger.LogDebug("訊息 {MessageId} 已經被用戶 {UserId} 標記為已讀", messageId, userId);
                    return true;
                }

                message.is_read = true;
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearUnreadCountCache(userId);
                ClearRecentConversationsCache(userId);

                _logger.LogInformation("用戶 {UserId} 成功標記訊息 {MessageId} 為已讀", userId, messageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記訊息 {MessageId} 為已讀時發生錯誤", messageId);
                throw;
            }
        }

        /// <summary>
        /// 獲取未讀訊息數量
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
                    _logger.LogDebug("從快取獲取用戶 {UserId} 的未讀訊息數量: {Count}", userId, cachedCount);
                    return cachedCount;
                }

                var count = await _context.ChatMessages
                    .AsNoTracking()
                    .CountAsync(cm => cm.receiver_id == userId && !cm.is_read);

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, count, cacheOptions);

                _logger.LogDebug("用戶 {UserId} 的未讀訊息數量: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 未讀訊息數量時發生錯誤", userId);
                throw;
            }
        }

        /// <summary>
        /// 獲取最近對話
        /// </summary>
        public async Task<IEnumerable<ChatMessage>> GetRecentConversationsAsync(int userId, int limit = 10)
        {
            try
            {
                // 輸入驗證
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (limit <= 0 || limit > MaxPageSize)
                    limit = 10;

                // 快取鍵
                var cacheKey = string.Format(RecentConversationsCacheKey, userId, limit);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<ChatMessage> cachedConversations))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 的最近對話，限制 {Limit}", userId, limit);
                    return cachedConversations;
                }

                var recentMessages = await _context.ChatMessages
                    .Include(cm => cm.Sender)
                    .Include(cm => cm.Manager)
                    .Include(cm => cm.Receiver)
                    .Where(cm => cm.receiver_id == userId || cm.sender_id == userId)
                    .GroupBy(cm => cm.sender_id == userId ? cm.receiver_id : cm.sender_id)
                    .Select(g => g.OrderByDescending(cm => cm.sent_at).First())
                    .OrderByDescending(cm => cm.sent_at)
                    .Take(limit)
                    .AsNoTracking()
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, recentMessages, cacheOptions);

                _logger.LogDebug("獲取用戶 {UserId} 的最近對話，限制 {Limit}，共 {Count} 個", 
                    userId, limit, recentMessages.Count);
                return recentMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 最近對話時發生錯誤", userId);
                throw;
            }
        }

        /// <summary>
        /// 刪除訊息
        /// </summary>
        public async Task<bool> DeleteMessageAsync(int messageId, int userId)
        {
            try
            {
                // 輸入驗證
                if (messageId <= 0)
                    throw new ArgumentException("訊息ID必須為正整數", nameof(messageId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(cm => cm.message_id == messageId && cm.sender_id == userId);

                if (message == null)
                {
                    _logger.LogWarning("用戶 {UserId} 嘗試刪除不存在的訊息 {MessageId}", userId, messageId);
                    return false;
                }

                _context.ChatMessages.Remove(message);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearDirectMessageCache(message.sender_id, message.receiver_id);
                ClearRecentConversationsCache(message.sender_id);
                ClearRecentConversationsCache(message.receiver_id);

                _logger.LogInformation("用戶 {UserId} 成功刪除訊息 {MessageId}", userId, messageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除訊息 {MessageId} 時發生錯誤", messageId);
                throw;
            }
        }

        #region 快取管理

        /// <summary>
        /// 清除私訊對話快取
        /// </summary>
        private void ClearDirectMessageCache(int userId1, int userId2)
        {
            // 清除所有頁面的快取
            for (int page = 1; page <= 5; page++)
            {
                for (int pageSize = 10; pageSize <= MaxPageSize; pageSize += 10)
                {
                    var cacheKey = string.Format(DirectMessagesCacheKey, userId1, userId2, page, pageSize);
                    _memoryCache.Remove(cacheKey);
                    
                    cacheKey = string.Format(DirectMessagesCacheKey, userId2, userId1, page, pageSize);
                    _memoryCache.Remove(cacheKey);
                }
            }
            _logger.LogDebug("清除用戶 {UserId1} 與 {UserId2} 的私訊對話快取", userId1, userId2);
        }

        /// <summary>
        /// 清除未讀數量快取
        /// </summary>
        private void ClearUnreadCountCache(int userId)
        {
            var cacheKey = string.Format(UnreadCountCacheKey, userId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除用戶 {UserId} 的未讀訊息數量快取", userId);
        }

        /// <summary>
        /// 清除最近對話快取
        /// </summary>
        private void ClearRecentConversationsCache(int userId)
        {
            // 清除不同限制數量的快取
            for (int limit = 5; limit <= MaxPageSize; limit += 5)
            {
                var cacheKey = string.Format(RecentConversationsCacheKey, userId, limit);
                _memoryCache.Remove(cacheKey);
            }
            _logger.LogDebug("清除用戶 {UserId} 的最近對話快取", userId);
        }

        #endregion
    }
}