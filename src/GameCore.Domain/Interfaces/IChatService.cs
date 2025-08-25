using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 聊天服務接口
    /// </summary>
    public interface IChatService
    {
        Task<IEnumerable<ChatMessage>> GetDirectMessagesAsync(int userId, int peerId, int page = 1, int pageSize = 50);
        Task<ChatMessage> SendDirectMessageAsync(int senderId, int receiverId, string content);
        Task<ChatMessage> SendSystemMessageAsync(int receiverId, string content, int? managerId = null);
        Task<bool> MarkMessageAsReadAsync(int messageId, int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<IEnumerable<ChatMessage>> GetRecentConversationsAsync(int userId, int limit = 10);
        Task<bool> DeleteMessageAsync(int messageId, int userId);
    }
}