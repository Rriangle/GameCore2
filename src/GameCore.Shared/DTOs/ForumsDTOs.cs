using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 論壇 DTO
/// </summary>
public class ForumDto
{
    /// <summary>
    /// 論壇編號
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 論壇名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 論壇說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string? GameName { get; set; }

    /// <summary>
    /// 討論串數量
    /// </summary>
    public int ThreadCount { get; set; }

    /// <summary>
    /// 回覆數量
    /// </summary>
    public int PostCount { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// 論壇詳細資訊 DTO
/// </summary>
public class ForumDetailDto : ForumDto
{
    /// <summary>
    /// 最新討論串列表
    /// </summary>
    public List<ThreadDto> LatestThreads { get; set; } = new();

    /// <summary>
    /// 熱門討論串列表
    /// </summary>
    public List<ThreadDto> PopularThreads { get; set; } = new();

    /// <summary>
    /// 置頂討論串列表
    /// </summary>
    public List<ThreadDto> PinnedThreads { get; set; } = new();
}

/// <summary>
/// 討論串 DTO
/// </summary>
public class ThreadDto
{
    /// <summary>
    /// 討論串編號
    /// </summary>
    public long ThreadId { get; set; }

    /// <summary>
    /// 論壇編號
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    /// 作者會員編號
    /// </summary>
    public int AuthorUserId { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 作者使用者名稱
    /// </summary>
    public string? AuthorUserName { get; set; }

    /// <summary>
    /// 作者暱稱
    /// </summary>
    public string? AuthorNickName { get; set; }

    /// <summary>
    /// 論壇名稱
    /// </summary>
    public string? ForumName { get; set; }

    /// <summary>
    /// 回覆數量
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// 觀看次數
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 最後回覆時間
    /// </summary>
    public DateTime? LastReplyAt { get; set; }

    /// <summary>
    /// 最後回覆者
    /// </summary>
    public string? LastReplyBy { get; set; }

    /// <summary>
    /// 是否置頂
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// 是否鎖定
    /// </summary>
    public bool IsLocked { get; set; }
}

/// <summary>
/// 討論串詳細資訊 DTO
/// </summary>
public class ThreadDetailDto : ThreadDto
{
    /// <summary>
    /// 第一則回覆 (原始內容)
    /// </summary>
    public ThreadPostDto? FirstPost { get; set; }

    /// <summary>
    /// 反應統計
    /// </summary>
    public List<ReactionStatsDto> ReactionStats { get; set; } = new();

    /// <summary>
    /// 是否已收藏
    /// </summary>
    public bool IsBookmarked { get; set; }

    /// <summary>
    /// 用戶反應
    /// </summary>
    public string? UserReaction { get; set; }
}

/// <summary>
/// 討論串回覆 DTO
/// </summary>
public class ThreadPostDto
{
    /// <summary>
    /// 回覆編號
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 討論串編號
    /// </summary>
    public long ThreadId { get; set; }

    /// <summary>
    /// 回覆者會員編號
    /// </summary>
    public int AuthorUserId { get; set; }

    /// <summary>
    /// 內容 (Markdown)
    /// </summary>
    public string ContentMd { get; set; } = string.Empty;

    /// <summary>
    /// 父回覆編號 (支援二層)
    /// </summary>
    public long? ParentPostId { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 回覆者使用者名稱
    /// </summary>
    public string? AuthorUserName { get; set; }

    /// <summary>
    /// 回覆者暱稱
    /// </summary>
    public string? AuthorNickName { get; set; }

    /// <summary>
    /// 父回覆作者名稱 (如果是回覆)
    /// </summary>
    public string? ParentAuthorName { get; set; }

    /// <summary>
    /// 子回覆列表
    /// </summary>
    public List<ThreadPostDto> ChildPosts { get; set; } = new();

    /// <summary>
    /// 反應統計
    /// </summary>
    public List<ReactionStatsDto> ReactionStats { get; set; } = new();

    /// <summary>
    /// 是否已收藏
    /// </summary>
    public bool IsBookmarked { get; set; }

    /// <summary>
    /// 用戶反應
    /// </summary>
    public string? UserReaction { get; set; }

    /// <summary>
    /// 是否可編輯 (當前用戶)
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// 是否可刪除 (當前用戶)
    /// </summary>
    public bool CanDelete { get; set; }
}

/// <summary>
/// 反應統計 DTO
/// </summary>
public class ReactionStatsDto
{
    /// <summary>
    /// 反應類型
    /// </summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// 數量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 顯示名稱
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 圖示
    /// </summary>
    public string? Icon { get; set; }
}

/// <summary>
/// 反應結果 DTO
/// </summary>
public class ReactionResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 反應類型
    /// </summary>
    public string? Kind { get; set; }

    /// <summary>
    /// 更新後的統計
    /// </summary>
    public List<ReactionStatsDto> UpdatedStats { get; set; } = new();
}

/// <summary>
/// 收藏結果 DTO
/// </summary>
public class BookmarkResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 收藏編號
    /// </summary>
    public long? BookmarkId { get; set; }
}

/// <summary>
/// 收藏項目 DTO
/// </summary>
public class BookmarkItemDto
{
    /// <summary>
    /// 收藏編號
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 目標類型
    /// </summary>
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目標編號
    /// </summary>
    public long TargetId { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 目標標題
    /// </summary>
    public string? TargetTitle { get; set; }

    /// <summary>
    /// 目標作者
    /// </summary>
    public string? TargetAuthor { get; set; }

    /// <summary>
    /// 目標摘要
    /// </summary>
    public string? TargetSummary { get; set; }

    /// <summary>
    /// 論壇名稱 (如果是討論串或回覆)
    /// </summary>
    public string? ForumName { get; set; }
}

/// <summary>
/// 分頁討論串 DTO
/// </summary>
public class PagedThreadsDto
{
    /// <summary>
    /// 討論串列表
    /// </summary>
    public List<ThreadDto> Threads { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁回覆 DTO
/// </summary>
public class PagedThreadPostsDto
{
    /// <summary>
    /// 回覆列表
    /// </summary>
    public List<ThreadPostDto> Posts { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁收藏 DTO
/// </summary>
public class PagedBookmarksDto
{
    /// <summary>
    /// 收藏列表
    /// </summary>
    public List<BookmarkItemDto> Bookmarks { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 討論串查詢 DTO
/// </summary>
public class ThreadQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (latest/popular/oldest)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// 狀態篩選
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 是否包含置頂
    /// </summary>
    public bool IncludePinned { get; set; } = true;
}

/// <summary>
/// 回覆查詢 DTO
/// </summary>
public class ThreadPostQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 排序方式 (oldest/newest)
    /// </summary>
    public string? SortBy { get; set; } = "oldest";

    /// <summary>
    /// 父回覆編號 (篩選子回覆)
    /// </summary>
    public long? ParentPostId { get; set; }

    /// <summary>
    /// 是否包含子回覆
    /// </summary>
    public bool IncludeChildPosts { get; set; } = true;
}

/// <summary>
/// 討論串搜尋查詢 DTO
/// </summary>
public class ThreadSearchQueryDto
{
    /// <summary>
    /// 關鍵字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 論壇編號
    /// </summary>
    public int? ForumId { get; set; }

    /// <summary>
    /// 作者編號
    /// </summary>
    public int? AuthorUserId { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (relevance/latest/popular)
    /// </summary>
    public string? SortBy { get; set; } = "relevance";
}

/// <summary>
/// 收藏查詢 DTO
/// </summary>
public class BookmarkQueryDto
{
    /// <summary>
    /// 目標類型篩選
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (newest/oldest)
    /// </summary>
    public string? SortBy { get; set; } = "newest";
}

/// <summary>
/// 建立討論串請求 DTO
/// </summary>
public class CreateThreadRequestDto
{
    /// <summary>
    /// 標題
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 內容 (Markdown)
    /// </summary>
    [Required]
    public string ContentMd { get; set; } = string.Empty;

    /// <summary>
    /// 標籤
    /// </summary>
    public List<string>? Tags { get; set; }
}

/// <summary>
/// 建立回覆請求 DTO
/// </summary>
public class CreateThreadPostRequestDto
{
    /// <summary>
    /// 內容 (Markdown)
    /// </summary>
    [Required]
    public string ContentMd { get; set; } = string.Empty;

    /// <summary>
    /// 父回覆編號 (回覆特定回覆時使用)
    /// </summary>
    public long? ParentPostId { get; set; }
}

/// <summary>
/// 更新回覆請求 DTO
/// </summary>
public class UpdateThreadPostRequestDto
{
    /// <summary>
    /// 內容 (Markdown)
    /// </summary>
    [Required]
    public string ContentMd { get; set; } = string.Empty;
}

/// <summary>
/// 新增反應請求 DTO
/// </summary>
public class AddReactionRequestDto
{
    /// <summary>
    /// 目標類型
    /// </summary>
    [Required]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目標編號
    /// </summary>
    [Required]
    public long TargetId { get; set; }

    /// <summary>
    /// 反應類型
    /// </summary>
    [Required]
    public string Kind { get; set; } = string.Empty;
}

/// <summary>
/// 移除反應請求 DTO
/// </summary>
public class RemoveReactionRequestDto
{
    /// <summary>
    /// 目標類型
    /// </summary>
    [Required]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目標編號
    /// </summary>
    [Required]
    public long TargetId { get; set; }

    /// <summary>
    /// 反應類型
    /// </summary>
    [Required]
    public string Kind { get; set; } = string.Empty;
}

/// <summary>
/// 新增收藏請求 DTO
/// </summary>
public class AddBookmarkRequestDto
{
    /// <summary>
    /// 目標類型
    /// </summary>
    [Required]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目標編號
    /// </summary>
    [Required]
    public long TargetId { get; set; }
}

/// <summary>
/// 移除收藏請求 DTO
/// </summary>
public class RemoveBookmarkRequestDto
{
    /// <summary>
    /// 目標類型
    /// </summary>
    [Required]
    public string TargetType { get; set; } = string.Empty;

    /// <summary>
    /// 目標編號
    /// </summary>
    [Required]
    public long TargetId { get; set; }
}