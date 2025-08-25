namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 創建主題請求 DTO
    /// </summary>
    public class CreateThreadRequestDto
    {
        public int ForumId { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新主題請求 DTO
    /// </summary>
    public class UpdateThreadRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "normal";
    }

    /// <summary>
    /// 主題響應 DTO
    /// </summary>
    public class ThreadResponseDto
    {
        public long ThreadId { get; set; }
        public int ForumId { get; set; }
        public string ForumName { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public int AuthorUserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int PostCount { get; set; }
        public int ReactionCount { get; set; }
        public int BookmarkCount { get; set; }
        public bool IsUserReacted { get; set; }
        public bool IsUserBookmarked { get; set; }
        public DateTime? LastPostAt { get; set; }
        public string? LastPostAuthor { get; set; }
    }

    /// <summary>
    /// 創建回覆請求 DTO
    /// </summary>
    public class CreatePostRequestDto
    {
        public long ThreadId { get; set; }
        public string ContentMd { get; set; } = string.Empty;
        public long? ParentPostId { get; set; }
    }

    /// <summary>
    /// 更新回覆請求 DTO
    /// </summary>
    public class UpdatePostRequestDto
    {
        public string ContentMd { get; set; } = string.Empty;
        public string Status { get; set; } = "normal";
    }

    /// <summary>
    /// 回覆響應 DTO
    /// </summary>
    public class PostResponseDto
    {
        public long Id { get; set; }
        public long ThreadId { get; set; }
        public int AuthorUserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string ContentMd { get; set; } = string.Empty;
        public long? ParentPostId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ReactionCount { get; set; }
        public bool IsUserReacted { get; set; }
        public List<PostResponseDto> ChildPosts { get; set; } = new();
    }

    /// <summary>
    /// 論壇響應 DTO
    /// </summary>
    public class ForumResponseDto
    {
        public int ForumId { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string GameGenre { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ThreadCount { get; set; }
        public int PostCount { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }

    /// <summary>
    /// 主題列表響應 DTO
    /// </summary>
    public class ThreadListResponseDto
    {
        public List<ThreadResponseDto> Threads { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// 回覆列表響應 DTO
    /// </summary>
    public class PostListResponseDto
    {
        public List<PostResponseDto> Posts { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// 論壇搜索請求 DTO
    /// </summary>
    public class ForumSearchRequestDto
    {
        public string? Keyword { get; set; }
        public int? ForumId { get; set; }
        public int? GameId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } // created_at, updated_at, title, post_count
        public string? SortOrder { get; set; } // asc, desc
    }

    /// <summary>
    /// 論壇統計響應 DTO
    /// </summary>
    public class ForumStatsResponseDto
    {
        public int TotalForums { get; set; }
        public int TotalThreads { get; set; }
        public int TotalPosts { get; set; }
        public Dictionary<string, int> ThreadsByGame { get; set; } = new();
        public Dictionary<string, int> PostsByGame { get; set; } = new();
        public Dictionary<string, int> ActivityByMonth { get; set; } = new();
    }
}