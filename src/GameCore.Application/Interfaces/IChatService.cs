using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;

namespace GameCore.Application.Interfaces;

/// <summary>
/// 聊天服務介面
/// </summary>
public interface IChatService
{
    /// <summary>
    /// 發送私人訊息
    /// </summary>
    /// <param name="request">發送訊息請求</param>
    /// <returns>發送結果</returns>
    Task<Result<SendMessageResponse>> SendPrivateMessageAsync(SendPrivateMessageRequest request);
    
    /// <summary>
    /// 發送群組訊息
    /// </summary>
    /// <param name="request">發送群組訊息請求</param>
    /// <returns>發送結果</returns>
    Task<Result<SendMessageResponse>> SendGroupMessageAsync(SendGroupMessageRequest request);
    
    /// <summary>
    /// 取得私人聊天記錄
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <param name="targetUserId">目標使用者 ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>聊天記錄列表</returns>
    Task<Result<ChatHistoryResponse>> GetPrivateChatHistoryAsync(int userId, int targetUserId, int page = 1, int pageSize = 50);
    
    /// <summary>
    /// 取得群組聊天記錄
    /// </summary>
    /// <param name="groupId">群組 ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>群組聊天記錄列表</returns>
    Task<Result<ChatHistoryResponse>> GetGroupChatHistoryAsync(int groupId, int page = 1, int pageSize = 50);
    
    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    /// <param name="request">標記已讀請求</param>
    /// <returns>操作結果</returns>
    Task<Result<MarkAsReadResponse>> MarkMessageAsReadAsync(MarkAsReadRequest request);
    
    /// <summary>
    /// 取得未讀訊息數量
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>未讀訊息數量</returns>
    Task<Result<UnreadCountResponse>> GetUnreadCountAsync(int userId);
} 