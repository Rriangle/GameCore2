using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 聊天服務實現
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<ChatService> _logger;

        public ChatService(GameCoreDbContext context, ILogger<ChatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取私訊對話
        /// </summary>
        public async Task<IEnumerable<ChatMessage>> GetDirectMessagesAsync(int userId, int peerId, int page = 1, int pageSize = 50)
        {
            try
            {
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
                    .ToListAsync();

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
                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(cm => cm.message_id == messageId && cm.receiver_id == userId);

                if (message == null)
                    return false;

                message.is_read = true;
                await _context.SaveChangesAsync();

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
                return await _context.ChatMessages
                    .CountAsync(cm => cm.receiver_id == userId && !cm.is_read);
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
                var recentMessages = await _context.ChatMessages
                    .Include(cm => cm.Sender)
                    .Include(cm => cm.Manager)
                    .Include(cm => cm.Receiver)
                    .Where(cm => cm.receiver_id == userId || cm.sender_id == userId)
                    .GroupBy(cm => cm.sender_id == userId ? cm.receiver_id : cm.sender_id)
                    .Select(g => g.OrderByDescending(cm => cm.sent_at).First())
                    .OrderByDescending(cm => cm.sent_at)
                    .Take(limit)
                    .ToListAsync();

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
                var message = await _context.ChatMessages
                    .FirstOrDefaultAsync(cm => cm.message_id == messageId && cm.sender_id == userId);

                if (message == null)
                    return false;

                _context.ChatMessages.Remove(message);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除訊息 {MessageId} 時發生錯誤", messageId);
                throw;
            }
        }
    }
}