using Microsoft.EntityFrameworkCore;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 聊天訊息 Repository 實作
/// </summary>
public class ChatMessageRepository : IChatMessageRepository
{
    private readonly GameCoreDbContext _context;

    public ChatMessageRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 取得用戶的聊天歷史記錄
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>聊天訊息列表</returns>
    public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int userId, int otherUserId, int page = 1, int pageSize = 50)
    {
        // 效能優化：預載 Sender 和 Receiver 資料避免 N+1 查詢問題
        return await _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                       (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
            .ToListAsync();
    }

    /// <summary>
    /// 根據訊息ID取得訊息詳情
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <returns>聊天訊息實體</returns>
    public async Task<ChatMessage?> GetByIdAsync(int messageId)
    {
        return await _context.ChatMessages.FindAsync(messageId);
    }

    /// <summary>
    /// 新增聊天訊息
    /// </summary>
    /// <param name="message">聊天訊息實體</param>
    /// <returns>新增的聊天訊息實體</returns>
    public async Task<ChatMessage> AddAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    /// <summary>
    /// 更新聊天訊息
    /// </summary>
    /// <param name="message">聊天訊息實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(ChatMessage message)
    {
        _context.ChatMessages.Update(message);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除聊天訊息
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int messageId)
    {
        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null) return false;

        _context.ChatMessages.Remove(message);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否標記成功</returns>
    public async Task<bool> MarkAsReadAsync(int messageId, int userId)
    {
        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null || message.ReceiverId != userId) return false;

        message.IsRead = true;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 標記與特定用戶的所有訊息為已讀
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>標記成功的數量</returns>
    public async Task<int> MarkAllAsReadAsync(int userId, int otherUserId)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId && !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        var result = await _context.SaveChangesAsync();
        return result;
    }

    /// <summary>
    /// 取得用戶的未讀訊息數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>未讀訊息數量</returns>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.ChatMessages
            .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
    }

    /// <summary>
    /// 取得用戶的聊天列表（最近聯絡的用戶）
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>聊天列表</returns>
    public async Task<IEnumerable<ChatMessage>> GetChatListAsync(int userId, int page = 1, int pageSize = 20)
    {
        // 取得最近聯絡的用戶ID列表
        var recentChats = await _context.ChatMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => new
            {
                OtherUserId = g.Key,
                LastMessageTime = g.Max(m => m.SentAt)
            })
            .OrderByDescending(x => x.LastMessageTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 取得每個聯絡人的最新訊息
        var chatList = new List<ChatMessage>();
        foreach (var chat in recentChats)
        {
            var lastMessage = await _context.ChatMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == chat.OtherUserId) ||
                           (m.SenderId == chat.OtherUserId && m.ReceiverId == userId))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            if (lastMessage != null)
            {
                chatList.Add(lastMessage);
            }
        }

        return chatList;
    }

    /// <summary>
    /// 搜尋聊天訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="keyword">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>搜尋結果</returns>
    public async Task<IEnumerable<ChatMessage>> SearchMessagesAsync(int userId, string keyword, int page = 1, int pageSize = 20)
    {
        return await _context.ChatMessages
            .Where(m => (m.SenderId == userId || m.ReceiverId == userId) &&
                       m.ChatContent.Contains(keyword))
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 取得用戶的客服訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>客服訊息列表</returns>
    public async Task<IEnumerable<ChatMessage>> GetCustomerServiceMessagesAsync(int userId, int page = 1, int pageSize = 50)
    {
        return await _context.ChatMessages
            .Where(m => (m.SenderId == userId && m.ManagerId.HasValue) ||
                       (m.ReceiverId == userId && m.ManagerId.HasValue))
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 取得管理員的客服訊息列表
    /// </summary>
    /// <param name="managerId">管理員ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>客服訊息列表</returns>
    public async Task<IEnumerable<ChatMessage>> GetManagerServiceMessagesAsync(int managerId, int page = 1, int pageSize = 50)
    {
        return await _context.ChatMessages
            .Where(m => m.ManagerId == managerId)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 取得用戶的聊天對象列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>聊天對象列表</returns>
    public async Task<IEnumerable<ChatMessage>> GetChatPartnersAsync(int userId)
    {
        // 取得與該用戶有聊天記錄的所有用戶ID
        var partnerIds = await _context.ChatMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Where(id => id.HasValue)
            .Distinct()
            .ToListAsync();

        // 取得每個聊天對象的最新訊息
        var chatPartners = new List<ChatMessage>();
        foreach (var partnerId in partnerIds)
        {
            var latestMessage = await _context.ChatMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == partnerId) ||
                           (m.SenderId == partnerId && m.ReceiverId == userId))
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            if (latestMessage != null)
            {
                chatPartners.Add(latestMessage);
            }
        }

        return chatPartners.OrderByDescending(m => m.SentAt);
    }

    /// <summary>
    /// 取得最新的聊天訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>最新訊息</returns>
    public async Task<ChatMessage?> GetLatestMessageAsync(int userId, int otherUserId)
    {
        return await _context.ChatMessages
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                       (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();
    }

    // 效能優化：新增資料庫層面計數方法，避免記憶體載入
    /// <summary>
    /// 取得用戶聊天對象數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>聊天對象數量</returns>
    public async Task<int> GetChatPartnerCountAsync(int userId)
    {
        return await _context.ChatMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Where(id => id.HasValue)
            .Distinct()
            .CountAsync();
    }

    /// <summary>
    /// 取得用戶今日訊息數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>今日訊息數量</returns>
    public async Task<int> GetTodayMessageCountAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.ChatMessages
            .Where(m => (m.SenderId == userId || m.ReceiverId == userId) &&
                       m.SentAt.Date == today)
            .CountAsync();
    }
} 