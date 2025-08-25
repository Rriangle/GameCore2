namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 創建群組請求 DTO
    /// </summary>
    public class CreateGroupRequestDto
    {
        public string GroupName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
    }

    /// <summary>
    /// 群組響應 DTO
    /// </summary>
    public class GroupResponseDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public bool IsUserMember { get; set; }
        public bool IsUserAdmin { get; set; }
        public bool IsUserBlocked { get; set; }
    }

    /// <summary>
    /// 群組詳情響應 DTO
    /// </summary>
    public class GroupDetailResponseDto : GroupResponseDto
    {
        public List<GroupMemberResponseDto> Members { get; set; } = new();
        public List<GroupChatResponseDto> RecentMessages { get; set; } = new();
        public List<GroupBlockResponseDto> BlockedUsers { get; set; } = new();
    }

    /// <summary>
    /// 群組成員響應 DTO
    /// </summary>
    public class GroupMemberResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
    }

    /// <summary>
    /// 群組聊天訊息響應 DTO
    /// </summary>
    public class GroupChatResponseDto
    {
        public int GroupChatId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsSent { get; set; }
    }

    /// <summary>
    /// 群組封鎖響應 DTO
    /// </summary>
    public class GroupBlockResponseDto
    {
        public int BlockId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string BlockedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 加入群組請求 DTO
    /// </summary>
    public class JoinGroupRequestDto
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }

    /// <summary>
    /// 離開群組請求 DTO
    /// </summary>
    public class LeaveGroupRequestDto
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }

    /// <summary>
    /// 群組管理請求 DTO
    /// </summary>
    public class GroupManagementRequestDto
    {
        public int GroupId { get; set; }
        public int TargetUserId { get; set; }
        public int AdminUserId { get; set; }
    }

    /// <summary>
    /// 發送群組訊息請求 DTO
    /// </summary>
    public class SendGroupMessageRequestDto
    {
        public int GroupId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// 群組列表響應 DTO
    /// </summary>
    public class GroupListResponseDto
    {
        public List<GroupResponseDto> Groups { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 群組訊息列表響應 DTO
    /// </summary>
    public class GroupMessageListResponseDto
    {
        public List<GroupChatResponseDto> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}