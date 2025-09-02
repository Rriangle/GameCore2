using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 論壇主題 DTO
/// </summary>
public class ForumThreadDto
{
    public long ThreadID { get; set; }
    public int ForumID { get; set; }
    public string ForumName { get; set; } = string.Empty;
    public int AuthorUserID { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public int ViewCount { get; set; }
    public int ReplyCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastReplyAt { get; set; }
    public string LastReplyBy { get; set; } = string.Empty;
}

/// <summary>
/// 建立論壇主題 DTO
/// </summary>
public class CreateThreadDto
{
    [Required]
    public int ForumID { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10000)]
    public string Content { get; set; } = string.Empty;
    
    public List<string> Tags { get; set; } = new List<string>();
}

/// <summary>
/// 論壇回覆 DTO
/// </summary>
public class ForumPostDto
{
    public long PostID { get; set; }
    public long ThreadID { get; set; }
    public string ThreadTitle { get; set; } = string.Empty;
    public int AuthorUserID { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text"; // "text", "markdown", "html"
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public string? EditReason { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PostReactionDto> Reactions { get; set; } = new List<PostReactionDto>();
}

/// <summary>
/// 建立論壇回覆 DTO
/// </summary>
public class CreatePostDto
{
    [Required]
    public long ThreadID { get; set; }
    
    [Required]
    [StringLength(10000)]
    public string Content { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = "text";
    
    public long? ReplyToPostID { get; set; }
}

/// <summary>
/// 回覆反應 DTO
/// </summary>
public class PostReactionDto
{
    public long ReactionID { get; set; }
    public long PostID { get; set; }
    public int UserID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ReactionType { get; set; } = string.Empty; // "like", "dislike", "love", "laugh", "wow", "sad", "angry"
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 新增反應 DTO
/// </summary>
public class AddReactionDto
{
    [Required]
    public long PostID { get; set; }
    
    [Required]
    [StringLength(20)]
    public string ReactionType { get; set; } = string.Empty;
}

/// <summary>
/// 書籤 DTO
/// </summary>
public class BookmarkDto
{
    public long BookmarkID { get; set; }
    public int UserID { get; set; }
    public string ContentType { get; set; } = string.Empty; // "thread", "post", "user"
    public long ContentID { get; set; }
    public string ContentTitle { get; set; } = string.Empty;
    public string ContentPreview { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 新增書籤 DTO
/// </summary>
public class AddBookmarkDto
{
    [Required]
    public string ContentType { get; set; } = string.Empty;
    
    [Required]
    public long ContentID { get; set; }
}

/// <summary>
/// 論壇搜尋參數 DTO
/// </summary>
public class ForumSearchDto
{
    public string? SearchTerm { get; set; }
    public int? ForumID { get; set; }
    public int? AuthorUserID { get; set; }
    public string? Status { get; set; }
    public bool? IsPinned { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "created"; // "created", "updated", "replies", "views"
    public string SortOrder { get; set; } = "desc"; // "asc", "desc"
}

/// <summary>
/// 論壇統計 DTO
/// </summary>
public class ForumStatsDto
{
    public int ForumID { get; set; }
    public string ForumName { get; set; } = string.Empty;
    public int TotalThreads { get; set; }
    public int TotalPosts { get; set; }
    public int TotalUsers { get; set; }
    public DateTime LastActivity { get; set; }
    public List<ForumThreadDto> RecentThreads { get; set; } = new List<ForumThreadDto>();
    public List<ForumPostDto> RecentPosts { get; set; } = new List<ForumPostDto>();
}

/// <summary>
/// 論壇分類 DTO
/// </summary>
public class ForumCategoryDto
{
    public int ForumID { get; set; }
    public string ForumName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ParentForumID { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int ThreadCount { get; set; }
    public int PostCount { get; set; }
    public DateTime LastActivity { get; set; }
    public List<ForumCategoryDto> SubForums { get; set; } = new List<ForumCategoryDto>();
} 