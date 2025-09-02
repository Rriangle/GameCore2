using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 聊天訊息資料庫操作介面
/// </summary>
public interface IChatMessageRepository
{
    /// <summary>
    /// 根據用戶ID取得聊天記錄
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>聊天訊息列表</returns>
    Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int userId, int otherUserId, int page = 1, int pageSize = 50);

    /// <summary>
    /// 根據訊息ID取得訊息詳情
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <returns>聊天訊息實體</returns>
    Task<ChatMessage?> GetByIdAsync(int messageId);

    /// <summary>
    /// 新增聊天訊息
    /// </summary>
    /// <param name="message">聊天訊息實體</param>
    /// <returns>新增的聊天訊息實體</returns>
    Task<ChatMessage> AddAsync(ChatMessage message);

    /// <summary>
    /// 更新聊天訊息
    /// </summary>
    /// <param name="message">聊天訊息實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(ChatMessage message);

    /// <summary>
    /// 刪除聊天訊息
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int messageId);

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否標記成功</returns>
    Task<bool> MarkAsReadAsync(int messageId, int userId);

    /// <summary>
    /// 標記與特定用戶的所有訊息為已讀
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>標記成功的數量</returns>
    Task<int> MarkAllAsReadAsync(int userId, int otherUserId);

    /// <summary>
    /// 取得用戶未讀訊息數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>未讀數量</returns>
    Task<int> GetUnreadCountAsync(int userId);

    /// <summary>
    /// 取得用戶的聊天對象列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>聊天對象列表</returns>
    Task<IEnumerable<ChatMessage>> GetChatPartnersAsync(int userId);

    /// <summary>
    /// 取得最新的聊天訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>最新訊息</returns>
    Task<ChatMessage?> GetLatestMessageAsync(int userId, int otherUserId);

    /// <summary>
    /// 搜尋聊天訊息
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>搜尋結果</returns>
    Task<IEnumerable<ChatMessage>> SearchMessagesAsync(int userId, string keyword, int page = 1, int pageSize = 20);

    // 效能優化：新增資料庫層面計數方法，避免記憶體載入
    /// <summary>
    /// 取得用戶聊天對象數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>聊天對象數量</returns>
    Task<int> GetChatPartnerCountAsync(int userId);

    /// <summary>
    /// 取得用戶今日訊息數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>今日訊息數量</returns>
    Task<int> GetTodayMessageCountAsync(int userId);
} 