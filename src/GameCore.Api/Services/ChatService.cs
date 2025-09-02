using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Services;

/// <summary>
/// 聊天服務
/// 處理私訊、群組聊天等功能
/// </summary>
public class ChatService : IChatService
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        IChatMessageRepository chatMessageRepository,
        IUserRepository userRepository,
        ILogger<ChatService> logger)
    {
        _chatMessageRepository = chatMessageRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 取得聊天記錄
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>聊天記錄</returns>
    public async Task<IEnumerable<ChatMessageDTO>> GetChatHistoryAsync(int userId, int otherUserId, int page = 1, int pageSize = 50)
    {
        try
        {
            var messages = await _chatMessageRepository.GetChatHistoryAsync(userId, otherUserId, page, pageSize);
            var messageDtos = new List<ChatMessageDTO>();

            foreach (var message in messages)
            {
                var sender = await _userRepository.GetByIdAsync(message.SenderId);
                var receiver = message.ReceiverId.HasValue ? await _userRepository.GetByIdAsync(message.ReceiverId.Value) : null;

                var dto = new ChatMessageDTO
                {
                    MessageId = message.MessageId,
                    ManagerId = message.ManagerId,
                    SenderId = message.SenderId,
                    SenderName = sender?.User_Name ?? "未知用戶",
                    ReceiverId = message.ReceiverId,
                    ReceiverName = receiver?.User_Name,
                    Content = message.ChatContent,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead,
                    IsSent = message.IsSent
                };

                messageDtos.Add(dto);
            }

            return messageDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 與 {OtherUserId} 的聊天記錄時發生錯誤", userId, otherUserId);
            throw;
        }
    }

    /// <summary>
    /// 發送聊天訊息
    /// </summary>
    /// <param name="senderId">發送者ID</param>
    /// <param name="request">發送訊息請求</param>
    /// <returns>發送結果</returns>
    public async Task<bool> SendMessageAsync(int senderId, SendChatMessageRequest request)
    {
        try
        {
            // 驗證發送者是否存在
            var sender = await _userRepository.GetByIdAsync(senderId);
            if (sender == null)
            {
                _logger.LogWarning("發送者 {SenderId} 不存在", senderId);
                return false;
            }

            // 驗證接收者是否存在
            var receiver = await _userRepository.GetByIdAsync(request.ReceiverId);
            if (receiver == null)
            {
                _logger.LogWarning("接收者 {ReceiverId} 不存在", request.ReceiverId);
                return false;
            }

            // 建立聊天訊息
            var message = new ChatMessage
            {
                ManagerId = request.ManagerId,
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                ChatContent = request.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsSent = true
            };

            await _chatMessageRepository.AddAsync(message);

            _logger.LogInformation("用戶 {SenderId} 向用戶 {ReceiverId} 發送訊息", senderId, request.ReceiverId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送聊天訊息時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>標記結果</returns>
    public async Task<bool> MarkAsReadAsync(int messageId, int userId)
    {
        try
        {
            return await _chatMessageRepository.MarkAsReadAsync(messageId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記訊息 {MessageId} 為已讀時發生錯誤", messageId);
            throw;
        }
    }

    /// <summary>
    /// 標記與特定用戶的所有訊息為已讀
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>標記成功的數量</returns>
    public async Task<int> MarkAllAsReadAsync(int userId, int otherUserId)
    {
        try
        {
            return await _chatMessageRepository.MarkAllAsReadAsync(userId, otherUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記用戶 {UserId} 與 {OtherUserId} 的所有訊息為已讀時發生錯誤", userId, otherUserId);
            throw;
        }
    }

    /// <summary>
    /// 取得未讀訊息數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>未讀數量</returns>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        try
        {
            return await _chatMessageRepository.GetUnreadCountAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 未讀訊息數量時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得聊天對象列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>聊天對象列表</returns>
    public async Task<IEnumerable<ChatMessageDTO>> GetChatPartnersAsync(int userId)
    {
        try
        {
            var partners = await _chatMessageRepository.GetChatPartnersAsync(userId);
            var partnerDtos = new List<ChatMessageDTO>();

            foreach (var partner in partners)
            {
                var sender = await _userRepository.GetByIdAsync(partner.SenderId);
                var receiver = partner.ReceiverId.HasValue ? await _userRepository.GetByIdAsync(partner.ReceiverId.Value) : null;

                var dto = new ChatMessageDTO
                {
                    MessageId = partner.MessageId,
                    ManagerId = partner.ManagerId,
                    SenderId = partner.SenderId,
                    SenderName = sender?.User_Name ?? "未知用戶",
                    ReceiverId = partner.ReceiverId,
                    ReceiverName = receiver?.User_Name,
                    Content = partner.ChatContent,
                    SentAt = partner.SentAt,
                    IsRead = partner.IsRead,
                    IsSent = partner.IsSent
                };

                partnerDtos.Add(dto);
            }

            return partnerDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 聊天對象列表時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得最新訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>最新訊息</returns>
    public async Task<ChatMessageDTO?> GetLatestMessageAsync(int userId, int otherUserId)
    {
        try
        {
            var message = await _chatMessageRepository.GetLatestMessageAsync(userId, otherUserId);
            if (message == null) return null;

            var sender = await _userRepository.GetByIdAsync(message.SenderId);
            var receiver = message.ReceiverId.HasValue ? await _userRepository.GetByIdAsync(message.ReceiverId.Value) : null;

            return new ChatMessageDTO
            {
                MessageId = message.MessageId,
                ManagerId = message.ManagerId,
                SenderId = message.SenderId,
                SenderName = sender?.User_Name ?? "未知用戶",
                ReceiverId = message.ReceiverId,
                ReceiverName = receiver?.User_Name,
                Content = message.ChatContent,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                IsSent = message.IsSent
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 與 {OtherUserId} 的最新訊息時發生錯誤", userId, otherUserId);
            throw;
        }
    }

    /// <summary>
    /// 搜尋聊天訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>搜尋結果</returns>
    public async Task<IEnumerable<ChatMessageDTO>> SearchMessagesAsync(int userId, string keyword, int page = 1, int pageSize = 20)
    {
        try
        {
            var messages = await _chatMessageRepository.SearchMessagesAsync(userId, keyword, page, pageSize);
            var messageDtos = new List<ChatMessageDTO>();

            foreach (var message in messages)
            {
                var sender = await _userRepository.GetByIdAsync(message.SenderId);
                var receiver = message.ReceiverId.HasValue ? await _userRepository.GetByIdAsync(message.ReceiverId.Value) : null;

                var dto = new ChatMessageDTO
                {
                    MessageId = message.MessageId,
                    ManagerId = message.ManagerId,
                    SenderId = message.SenderId,
                    SenderName = sender?.User_Name ?? "未知用戶",
                    ReceiverId = message.ReceiverId,
                    ReceiverName = receiver?.User_Name,
                    Content = message.ChatContent,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead,
                    IsSent = message.IsSent
                };

                messageDtos.Add(dto);
            }

            return messageDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋用戶 {UserId} 聊天訊息時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得聊天統計
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>聊天統計</returns>
    public async Task<ChatStatsDTO> GetChatStatsAsync(int userId)
    {
        try
        {
            // 效能優化：使用資料庫層面計數方法，避免載入完整資料到記憶體
            var unreadCount = await _chatMessageRepository.GetUnreadCountAsync(userId);
            var chatPartnerCount = await _chatMessageRepository.GetChatPartnerCountAsync(userId);
            var todayMessageCount = await _chatMessageRepository.GetTodayMessageCountAsync(userId);

            return new ChatStatsDTO
            {
                UnreadCount = unreadCount,
                ChatPartnerCount = chatPartnerCount,
                TodayMessageCount = todayMessageCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 聊天統計時發生錯誤", userId);
            throw;
        }
    }
}

/// <summary>
/// 聊天服務介面
/// </summary>
public interface IChatService
{
    /// <summary>
    /// 取得聊天記錄
    /// </summary>
    Task<IEnumerable<ChatMessageDTO>> GetChatHistoryAsync(int userId, int otherUserId, int page = 1, int pageSize = 50);

    /// <summary>
    /// 發送聊天訊息
    /// </summary>
    Task<bool> SendMessageAsync(int senderId, SendChatMessageRequest request);

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    Task<bool> MarkAsReadAsync(int messageId, int userId);

    /// <summary>
    /// 標記與特定用戶的所有訊息為已讀
    /// </summary>
    Task<int> MarkAllAsReadAsync(int userId, int otherUserId);

    /// <summary>
    /// 取得未讀訊息數量
    /// </summary>
    Task<int> GetUnreadCountAsync(int userId);

    /// <summary>
    /// 取得聊天對象列表
    /// </summary>
    Task<IEnumerable<ChatMessageDTO>> GetChatPartnersAsync(int userId);

    /// <summary>
    /// 取得最新訊息
    /// </summary>
    Task<ChatMessageDTO?> GetLatestMessageAsync(int userId, int otherUserId);

    /// <summary>
    /// 搜尋聊天訊息
    /// </summary>
    Task<IEnumerable<ChatMessageDTO>> SearchMessagesAsync(int userId, string keyword, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得聊天統計
    /// </summary>
    Task<ChatStatsDTO> GetChatStatsAsync(int userId);
} 