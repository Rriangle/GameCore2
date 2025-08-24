using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 社交服務介面，定義私訊、群組、封鎖相關的業務邏輯
/// </summary>
public interface ISocialService
{
    // ==================== 私訊 (Direct Messages) ====================
    
    /// <summary>
    /// 取得與特定用戶的聊天記錄 (分頁查詢)
    /// </summary>
    /// <param name="userId">當前用戶編號</param>
    /// <param name="peerId">聊天對象編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁聊天記錄</returns>
    Task<PagedChatMessagesDto> GetChatWithUserAsync(int userId, int peerId, ChatQueryDto query);

    /// <summary>
    /// 發送私訊
    /// </summary>
    /// <param name="senderId">發送者編號</param>
    /// <param name="request">發送私訊請求</param>
    /// <returns>發送的訊息資料</returns>
    Task<ChatMessageDto> SendDirectMessageAsync(int senderId, SendDirectMessageRequestDto request);

    /// <summary>
    /// 標記私訊為已讀
    /// </summary>
    /// <param name="userId">接收者編號</param>
    /// <param name="messageId">訊息編號</param>
    /// <returns>是否標記成功</returns>
    Task<bool> MarkDirectMessageAsReadAsync(int userId, int messageId);

    /// <summary>
    /// 取得用戶的聊天列表 (最近聯絡人)
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>聊天列表</returns>
    Task<List<ChatContactDto>> GetChatContactsAsync(int userId);

    /// <summary>
    /// 取得未讀私訊數量
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>未讀私訊數量</returns>
    Task<int> GetUnreadDirectMessageCountAsync(int userId);

    // ==================== 群組管理 ====================

    /// <summary>
    /// 建立群組
    /// </summary>
    /// <param name="creatorId">建立者編號</param>
    /// <param name="request">建立群組請求</param>
    /// <returns>建立的群組資料</returns>
    Task<GroupDto> CreateGroupAsync(int creatorId, CreateGroupRequestDto request);

    /// <summary>
    /// 取得群組詳情
    /// </summary>
    /// <param name="userId">用戶編號 (用於檢查權限)</param>
    /// <param name="groupId">群組編號</param>
    /// <returns>群組詳情，不存在或無權限則返回 null</returns>
    Task<GroupDetailDto?> GetGroupDetailAsync(int userId, int groupId);

    /// <summary>
    /// 取得用戶加入的群組列表
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁群組列表</returns>
    Task<PagedGroupsDto> GetUserGroupsAsync(int userId, GroupQueryDto query);

    /// <summary>
    /// 加入群組
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="groupId">群組編號</param>
    /// <returns>是否加入成功</returns>
    Task<bool> JoinGroupAsync(int userId, int groupId);

    /// <summary>
    /// 離開群組
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="groupId">群組編號</param>
    /// <returns>是否離開成功</returns>
    Task<bool> LeaveGroupAsync(int userId, int groupId);

    /// <summary>
    /// 取得群組成員列表
    /// </summary>
    /// <param name="userId">用戶編號 (用於檢查權限)</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁成員列表</returns>
    Task<PagedGroupMembersDto> GetGroupMembersAsync(int userId, int groupId, GroupMemberQueryDto query);

    // ==================== 群組聊天 ====================

    /// <summary>
    /// 取得群組聊天記錄 (分頁查詢)
    /// </summary>
    /// <param name="userId">用戶編號 (用於檢查權限)</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁群組聊天記錄</returns>
    Task<PagedGroupChatMessagesDto> GetGroupChatMessagesAsync(int userId, int groupId, GroupChatQueryDto query);

    /// <summary>
    /// 發送群組訊息
    /// </summary>
    /// <param name="senderId">發送者編號</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="request">發送群組訊息請求</param>
    /// <returns>發送的訊息資料</returns>
    Task<GroupChatMessageDto> SendGroupMessageAsync(int senderId, int groupId, SendGroupMessageRequestDto request);

    // ==================== 群組封鎖 ====================

    /// <summary>
    /// 在群組中封鎖用戶
    /// </summary>
    /// <param name="blockerId">封鎖者編號</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="blockedUserId">被封鎖用戶編號</param>
    /// <param name="reason">封鎖原因</param>
    /// <returns>是否封鎖成功</returns>
    Task<bool> BlockUserInGroupAsync(int blockerId, int groupId, int blockedUserId, string? reason = null);

    /// <summary>
    /// 在群組中解除封鎖用戶
    /// </summary>
    /// <param name="unblockerId">解除封鎖者編號</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="blockedUserId">被封鎖用戶編號</param>
    /// <returns>是否解除封鎖成功</returns>
    Task<bool> UnblockUserInGroupAsync(int unblockerId, int groupId, int blockedUserId);

    /// <summary>
    /// 取得群組的封鎖用戶列表
    /// </summary>
    /// <param name="userId">用戶編號 (用於檢查權限)</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁封鎖用戶列表</returns>
    Task<PagedGroupBlocksDto> GetGroupBlocksAsync(int userId, int groupId, GroupBlockQueryDto query);

    /// <summary>
    /// 檢查用戶是否在群組中被封鎖
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="groupId">群組編號</param>
    /// <returns>是否被封鎖</returns>
    Task<bool> IsUserBlockedInGroupAsync(int userId, int groupId);

    // ==================== 搜尋與發現 ====================

    /// <summary>
    /// 搜尋群組
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁搜尋結果</returns>
    Task<PagedGroupsDto> SearchGroupsAsync(GroupSearchQueryDto query);

    /// <summary>
    /// 取得推薦群組
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="limit">取得數量</param>
    /// <returns>推薦群組列表</returns>
    Task<List<GroupDto>> GetRecommendedGroupsAsync(int userId, int limit = 10);

    /// <summary>
    /// 取得熱門群組
    /// </summary>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門群組列表</returns>
    Task<List<GroupDto>> GetPopularGroupsAsync(int limit = 10);
}