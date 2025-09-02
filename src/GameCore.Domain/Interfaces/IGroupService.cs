using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 群組服務介面
/// </summary>
public interface IGroupService
{
    /// <summary>
    /// 建立群組
    /// </summary>
    /// <param name="request">建立群組請求</param>
    /// <returns>群組資訊</returns>
    Task<ServiceResult<GroupDTO>> CreateGroupAsync(CreateGroupRequest request);

    /// <summary>
    /// 加入群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> JoinGroupAsync(int groupId, int userId);

    /// <summary>
    /// 退出群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> LeaveGroupAsync(int groupId, int userId);

    /// <summary>
    /// 發送群組訊息
    /// </summary>
    /// <param name="request">發送訊息請求</param>
    /// <returns>訊息資訊</returns>
    Task<ServiceResult<GroupChatDTO>> SendGroupMessageAsync(SendGroupMessageRequest request);

    /// <summary>
    /// 封鎖群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被封鎖用戶ID</param>
    /// <param name="blockedBy">封鎖者ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> BlockGroupMemberAsync(int groupId, int userId, int blockedBy);

    /// <summary>
    /// 解除封鎖群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被解除封鎖用戶ID</param>
    /// <param name="unblockedBy">解除封鎖者ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> UnblockGroupMemberAsync(int groupId, int userId, int unblockedBy);

    /// <summary>
    /// 取得群組資訊
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組資訊</returns>
    Task<ServiceResult<GroupDTO>> GetGroupAsync(int groupId);

    /// <summary>
    /// 取得用戶的群組列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組列表</returns>
    Task<ServiceResult<List<GroupDTO>>> GetUserGroupsAsync(int userId);

    /// <summary>
    /// 取得群組成員列表
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>成員列表</returns>
    Task<ServiceResult<List<GroupMemberDTO>>> GetGroupMembersAsync(int groupId);

    /// <summary>
    /// 取得群組聊天記錄
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>聊天記錄</returns>
    Task<ServiceResult<List<GroupChatDTO>>> GetGroupChatHistoryAsync(int groupId, int page = 1, int pageSize = 50);

    /// <summary>
    /// 搜尋群組
    /// </summary>
    /// <param name="keyword">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>群組列表</returns>
    Task<ServiceResult<List<GroupDTO>>> SearchGroupsAsync(string keyword, int page = 1, int pageSize = 20);

    /// <summary>
    /// 封鎖群組用戶
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被封鎖用戶ID</param>
    /// <param name="blockedBy">封鎖者ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> BlockUserAsync(int groupId, int userId, int blockedBy);

    /// <summary>
    /// 解除群組用戶封鎖
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被解除封鎖用戶ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> UnblockUserAsync(int groupId, int userId);

    /// <summary>
    /// 取得群組統計資訊
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>統計資訊</returns>
    Task<ServiceResult<GroupStatsDTO>> GetGroupStatsAsync(int groupId);
} 