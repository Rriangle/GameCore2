using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 群組服務接口
    /// </summary>
    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetUserGroupsAsync(int userId);
        Task<Group?> GetGroupAsync(int groupId);
        Task<Group> CreateGroupAsync(string groupName, int createdBy);
        Task<bool> JoinGroupAsync(int groupId, int userId);
        Task<bool> LeaveGroupAsync(int groupId, int userId);
        Task<bool> RemoveMemberAsync(int groupId, int userId, int adminUserId);
        Task<bool> SetMemberAsAdminAsync(int groupId, int userId, int adminUserId);
        Task<bool> RemoveAdminAsync(int groupId, int userId, int adminUserId);
        Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId);
        Task<IEnumerable<GroupChat>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 50);
        Task<GroupChat> SendGroupMessageAsync(int groupId, int senderId, string content);
        Task<bool> BlockUserAsync(int groupId, int userId, int blockedBy);
        Task<bool> UnblockUserAsync(int groupId, int userId, int adminUserId);
        Task<IEnumerable<GroupBlock>> GetGroupBlocksAsync(int groupId);
        Task<bool> IsUserBlockedAsync(int groupId, int userId);
        Task<bool> IsUserAdminAsync(int groupId, int userId);
        Task<bool> DeleteGroupAsync(int groupId, int adminUserId);
    }
}