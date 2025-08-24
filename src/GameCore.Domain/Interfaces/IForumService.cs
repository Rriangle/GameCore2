namespace GameCore.Domain.Interfaces;

public interface IForumService
{
    Task<IEnumerable<ForumPostDto>> GetPostsAsync(string? status = null);
    Task<ForumPostDto?> GetPostAsync(int postId);
    Task<ForumPostResult> CreatePostAsync(int userId, string title, string content);
    Task<ForumPostResult> UpdatePostAsync(int postId, int userId, string title, string content);
    Task<ForumPostResult> UpdatePostStatusAsync(int postId, int userId, string status);
    Task<IEnumerable<ForumThreadDto>> GetThreadsAsync(int postId);
    Task<ForumThreadDto?> GetThreadAsync(int threadId);
    Task<ForumThreadResult> CreateThreadAsync(int postId, int userId, string title, string content);
    Task<ForumThreadResult> UpdateThreadAsync(int threadId, int userId, string title, string content);
    Task<ForumThreadResult> UpdateThreadStatusAsync(int threadId, int userId, string status);
    Task<IEnumerable<ForumThreadPostDto>> GetThreadPostsAsync(int threadId);
    Task<ForumThreadPostResult> CreateThreadPostAsync(int threadId, int userId, string content);
    Task<ForumThreadPostResult> UpdateThreadPostAsync(int threadPostId, int userId, string content);
    Task<ForumReactionResult> AddReactionAsync(int userId, string targetType, int targetId, string reactionType);
    Task<ForumReactionResult> RemoveReactionAsync(int userId, string targetType, int targetId, string reactionType);
    Task<ForumBookmarkResult> AddBookmarkAsync(int userId, string targetType, int targetId);
    Task<ForumBookmarkResult> RemoveBookmarkAsync(int userId, string targetType, int targetId);
    Task<IEnumerable<ForumPostDto>> GetUserPostsAsync(int userId);
    Task<IEnumerable<ForumBookmarkDto>> GetUserBookmarksAsync(int userId);
}

public class ForumPostDto
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ReplyCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public class ForumThreadDto
{
    public int ThreadId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ReplyCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public class ForumThreadPostDto
{
    public int ThreadPostId { get; set; }
    public int ThreadId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public class ForumBookmarkDto
{
    public int BookmarkId { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public int TargetId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ForumPostResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ForumPostDto? Post { get; set; }
}

public class ForumThreadResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ForumThreadDto? Thread { get; set; }
}

public class ForumThreadPostResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public ForumThreadPostDto? ThreadPost { get; set; }
}

public class ForumReactionResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int LikeCount { get; set; }
}

public class ForumBookmarkResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool IsBookmarked { get; set; }
}