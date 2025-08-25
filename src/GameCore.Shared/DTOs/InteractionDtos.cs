namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 添加反應請求 DTO
    /// </summary>
    public class AddReactionRequestDto
    {
        public string TargetType { get; set; } = string.Empty; // post, thread, thread_post
        public long TargetId { get; set; }
        public string Kind { get; set; } = "like"; // like, love, laugh, wow, sad, angry
    }

    /// <summary>
    /// 移除反應請求 DTO
    /// </summary>
    public class RemoveReactionRequestDto
    {
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
        public string Kind { get; set; } = string.Empty;
    }

    /// <summary>
    /// 反應響應 DTO
    /// </summary>
    public class ReactionResponseDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
        public string Kind { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 反應統計響應 DTO
    /// </summary>
    public class ReactionStatsResponseDto
    {
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
        public Dictionary<string, int> ReactionCounts { get; set; } = new();
        public int TotalReactions { get; set; }
        public List<ReactionResponseDto> RecentReactions { get; set; } = new();
    }

    /// <summary>
    /// 添加收藏請求 DTO
    /// </summary>
    public class AddBookmarkRequestDto
    {
        public string TargetType { get; set; } = string.Empty; // post, thread, game, forum
        public long TargetId { get; set; }
    }

    /// <summary>
    /// 移除收藏請求 DTO
    /// </summary>
    public class RemoveBookmarkRequestDto
    {
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
    }

    /// <summary>
    /// 收藏響應 DTO
    /// </summary>
    public class BookmarkResponseDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public long TargetId { get; set; }
        public DateTime CreatedAt { get; set; }
        public object? Target { get; set; } // 收藏的目標對象
    }

    /// <summary>
    /// 用戶互動統計響應 DTO
    /// </summary>
    public class UserInteractionStatsResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalReactions { get; set; }
        public int TotalBookmarks { get; set; }
        public Dictionary<string, int> ReactionsByType { get; set; } = new();
        public Dictionary<string, int> BookmarksByType { get; set; } = new();
        public List<ReactionResponseDto> RecentReactions { get; set; } = new();
        public List<BookmarkResponseDto> RecentBookmarks { get; set; } = new();
    }

    /// <summary>
    /// 互動搜索請求 DTO
    /// </summary>
    public class InteractionSearchRequestDto
    {
        public string? TargetType { get; set; }
        public long? TargetId { get; set; }
        public int? UserId { get; set; }
        public string? Kind { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } // created_at, user_id, target_id
        public string? SortOrder { get; set; } // asc, desc
    }

    /// <summary>
    /// 互動統計響應 DTO
    /// </summary>
    public class InteractionStatsResponseDto
    {
        public int TotalReactions { get; set; }
        public int TotalBookmarks { get; set; }
        public Dictionary<string, int> ReactionsByType { get; set; } = new();
        public Dictionary<string, int> ReactionsByKind { get; set; } = new();
        public Dictionary<string, int> BookmarksByType { get; set; } = new();
        public Dictionary<string, int> ActivityByMonth { get; set; } = new();
        public List<string> TopReactionKinds { get; set; } = new();
        public List<string> TopBookmarkTypes { get; set; } = new();
    }
}