namespace GameCore.Domain.Interfaces;

public interface IGroupService
{
    Task<IEnumerable<GroupDto>> GetGroupsAsync();
    Task<GroupDto?> GetGroupAsync(int groupId);
    Task<GroupResult> CreateGroupAsync(int userId, string name, string description, string? avatarUrl);
    Task<GroupResult> UpdateGroupAsync(int groupId, int userId, string name, string description, string? avatarUrl);
    Task<GroupResult> DeleteGroupAsync(int groupId, int userId);
    Task<GroupMemberResult> AddMemberAsync(int groupId, int userId, int memberId, string role);
    Task<GroupMemberResult> RemoveMemberAsync(int groupId, int userId, int memberId);
    Task<GroupMemberResult> UpdateMemberRoleAsync(int groupId, int userId, int memberId, string role);
    Task<IEnumerable<GroupMemberDto>> GetGroupMembersAsync(int groupId);
    Task<GroupConversationResult> SendMessageAsync(int groupId, int userId, string message, string messageType);
    Task<IEnumerable<GroupConversationDto>> GetGroupMessagesAsync(int groupId, int? limit = null);
    Task<IEnumerable<GroupDto>> GetUserGroupsAsync(int userId);
    Task<bool> IsUserMemberAsync(int groupId, int userId);
    Task<bool> IsUserOwnerAsync(int groupId, int userId);
}

public class GroupDto
{
    public int GroupId { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class GroupMemberDto
{
    public int MemberId { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
}

public class GroupConversationDto
{
    public int ConversationId { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class GroupResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public GroupDto? Group { get; set; }
}

public class GroupMemberResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public GroupMemberDto? Member { get; set; }
}

public class GroupConversationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public GroupConversationDto? Message { get; set; }
}