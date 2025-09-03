using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;
using GameCore.Application.Interfaces;

namespace GameCore.Application.Services;

/// <summary>
/// 聊天服務存根實作
/// </summary>
public class ChatService : IChatService
{
    /// <summary>
    /// 發送私人訊息
    /// </summary>
    public Task<Result<SendMessageResponse>> SendPrivateMessageAsync(SendPrivateMessageRequest request)
    {
        // TODO: 實作真實的私人訊息發送邏輯
        throw new NotImplementedException("私人訊息發送功能尚未實作");
    }
    
    /// <summary>
    /// 發送群組訊息
    /// </summary>
    public Task<Result<SendMessageResponse>> SendGroupMessageAsync(SendGroupMessageRequest request)
    {
        // TODO: 實作真實的群組訊息發送邏輯
        throw new NotImplementedException("群組訊息發送功能尚未實作");
    }
    
    /// <summary>
    /// 取得私人聊天記錄
    /// </summary>
    public Task<Result<ChatHistoryResponse>> GetPrivateChatHistoryAsync(int userId, int targetUserId, int page = 1, int pageSize = 50)
    {
        // TODO: 實作真實的私人聊天記錄查詢邏輯
        throw new NotImplementedException("私人聊天記錄查詢功能尚未實作");
    }
    
    /// <summary>
    /// 取得群組聊天記錄
    /// </summary>
    public Task<Result<ChatHistoryResponse>> GetGroupChatHistoryAsync(int groupId, int page = 1, int pageSize = 50)
    {
        // TODO: 實作真實的群組聊天記錄查詢邏輯
        throw new NotImplementedException("群組聊天記錄查詢功能尚未實作");
    }
    
    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    public Task<Result<MarkAsReadResponse>> MarkMessageAsReadAsync(MarkAsReadRequest request)
    {
        // TODO: 實作真實的已讀標記邏輯
        throw new NotImplementedException("已讀標記功能尚未實作");
    }
    
    /// <summary>
    /// 取得未讀訊息數量
    /// </summary>
    public Task<Result<UnreadCountResponse>> GetUnreadCountAsync(int userId)
    {
        // TODO: 實作真實的未讀訊息數量查詢邏輯
        throw new NotImplementedException("未讀訊息數量查詢功能尚未實作");
    }
} 