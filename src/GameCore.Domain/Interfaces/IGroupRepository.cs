using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 群組資料庫操作介面
/// </summary>
public interface IGroupRepository
{
    /// <summary>
    /// 根據群組ID取得群組詳情
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組實體</returns>
    Task<Group?> GetByIdAsync(int groupId);

    /// <summary>
    /// 根據用戶ID取得用戶加入的群組
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組列表</returns>
    Task<IEnumerable<Group>> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增群組
    /// </summary>
    /// <param name="group">群組實體</param>
    /// <returns>新增的群組實體</returns>
    Task<Group> AddAsync(Group group);

    /// <summary>
    /// 更新群組
    /// </summary>
    /// <param name="group">群組實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(Group group);

    /// <summary>
    /// 刪除群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int groupId);

    /// <summary>
    /// 取得群組成員列表
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組成員列表</returns>
    Task<IEnumerable<GroupMember>> GetMembersAsync(int groupId);

    /// <summary>
    /// 新增群組成員
    /// </summary>
    /// <param name="member">群組成員實體</param>
    /// <returns>新增的群組成員實體</returns>
    Task<GroupMember> AddMemberAsync(GroupMember member);

    /// <summary>
    /// 移除群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemoveMemberAsync(int groupId, int userId);

    /// <summary>
    /// 檢查用戶是否為群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否為成員</returns>
    Task<bool> IsMemberAsync(int groupId, int userId);

    /// <summary>
    /// 檢查用戶是否為群組管理員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否為管理員</returns>
    Task<bool> IsAdminAsync(int groupId, int userId);

    /// <summary>
    /// 取得群組聊天記錄
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>群組聊天記錄</returns>
    Task<IEnumerable<GroupChat>> GetChatHistoryAsync(int groupId, int page = 1, int pageSize = 50);

    /// <summary>
    /// 新增群組聊天訊息
    /// </summary>
    /// <param name="chat">群組聊天實體</param>
    /// <returns>新增的群組聊天實體</returns>
    Task<GroupChat> AddChatMessageAsync(GroupChat chat);

    /// <summary>
    /// 取得群組封鎖列表
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組封鎖列表</returns>
    Task<IEnumerable<GroupBlock>> GetBlocksAsync(int groupId);

    /// <summary>
    /// 新增群組封鎖
    /// </summary>
    /// <param name="block">群組封鎖實體</param>
    /// <returns>新增的群組封鎖實體</returns>
    Task<GroupBlock> AddBlockAsync(GroupBlock block);

    /// <summary>
    /// 移除群組封鎖
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemoveBlockAsync(int groupId, int userId);

    /// <summary>
    /// 檢查用戶是否被群組封鎖
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否被封鎖</returns>
    Task<bool> IsBlockedAsync(int groupId, int userId);

    /// <summary>
    /// 搜尋群組
    /// </summary>
    /// <param name="keyword">關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>搜尋結果</returns>
    Task<IEnumerable<Group>> SearchGroupsAsync(string keyword, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得特定群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組成員</returns>
    Task<GroupMember?> GetMemberAsync(int groupId, int userId);

    /// <summary>
    /// 檢查用戶是否被群組封鎖 (別名方法)
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否被封鎖</returns>
    Task<bool> IsUserBlockedAsync(int groupId, int userId);

    /// <summary>
    /// 新增群組訊息 (別名方法)
    /// </summary>
    /// <param name="chat">群組聊天實體</param>
    /// <returns>新增的群組聊天實體</returns>
    Task<GroupChat> AddMessageAsync(GroupChat chat);

    /// <summary>
    /// 取得群組成員數量
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>成員數量</returns>
    Task<int> GetMemberCountAsync(int groupId);

    /// <summary>
    /// 取得用戶加入的群組
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組列表</returns>
    Task<IEnumerable<Group>> GetUserGroupsAsync(int userId);

    /// <summary>
    /// 取得群組成員列表 (別名方法)
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組成員列表</returns>
    Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId);

    /// <summary>
    /// 取得群組聊天歷史 (別名方法)
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>群組聊天記錄</returns>
    Task<IEnumerable<GroupChat>> GetGroupChatHistoryAsync(int groupId, int page = 1, int pageSize = 50);

    /// <summary>
    /// 取得群組訊息數量
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>訊息數量</returns>
    Task<int> GetMessageCountAsync(int groupId);

    /// <summary>
    /// 取得群組聊天訊息
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>群組聊天訊息</returns>
    Task<IEnumerable<GroupChat>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 取得用戶加入的群組
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組列表</returns>
    Task<IEnumerable<Group>> GetUserJoinedGroupsAsync(int userId);

    /// <summary>
    /// 取得用戶管理的群組
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組列表</returns>
    Task<IEnumerable<Group>> GetUserAdminGroupsAsync(int userId);

    /// <summary>
    /// 取得總群組數量
    /// </summary>
    /// <returns>群組數量</returns>
    Task<int> GetTotalGroupCountAsync();

    // 效能優化：新增資料庫層面計數方法，避免記憶體載入
    /// <summary>
    /// 取得用戶加入的群組數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>加入的群組數量</returns>
    Task<int> GetUserJoinedGroupCountAsync(int userId);

    /// <summary>
    /// 取得用戶管理的群組數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>管理的群組數量</returns>
    Task<int> GetUserAdminGroupCountAsync(int userId);
} 