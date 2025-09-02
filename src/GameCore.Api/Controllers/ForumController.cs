using GameCore.Api.Services;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers;

/// <summary>
/// 論壇控制器
/// 處理論壇主題、回覆、反應和書籤功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ForumController : ControllerBase
{
    private readonly ForumService _forumService;

    public ForumController(ForumService forumService)
    {
        _forumService = forumService;
    }

    /// <summary>
    /// 取得論壇分類列表
    /// </summary>
    /// <returns>論壇分類列表</returns>
    [HttpGet("categories")]
    public async Task<ActionResult<List<ForumCategoryDto>>> GetCategories()
    {
        try
        {
            var result = await _forumService.GetForumCategoriesAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得論壇分類失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得論壇主題列表
    /// </summary>
    /// <param name="searchParams">搜尋參數</param>
    /// <returns>分頁的主題列表</returns>
    [HttpGet("threads")]
    public async Task<ActionResult<PagedResult<ForumThreadDto>>> GetThreads([FromQuery] ForumSearchDto searchParams)
    {
        try
        {
            var result = await _forumService.GetThreadsAsync(searchParams);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得主題列表失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 建立新主題
    /// </summary>
    /// <param name="threadDto">主題資訊</param>
    /// <returns>建立結果</returns>
    [HttpPost("threads")]
    [Authorize]
    public async Task<ActionResult<ForumThreadDto>> CreateThread([FromBody] CreateThreadDto threadDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            var result = await _forumService.CreateThreadAsync(userId.Value, threadDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetThreadDetail), new { threadId = result.Data!.ThreadID }, result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "建立主題失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得主題詳細資訊
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>主題和回覆資訊</returns>
    [HttpGet("threads/{threadId}")]
    public async Task<ActionResult<ThreadDetailDto>> GetThreadDetail(
        long threadId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "頁碼和每頁大小必須大於 0，每頁大小不能超過 100" });
            }

            var result = await _forumService.GetThreadDetailAsync(threadId, page, pageSize);
            if (!result.Success)
            {
                return NotFound(new { message = result.Message });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得主題詳細資訊失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 新增回覆
    /// </summary>
    /// <param name="postDto">回覆資訊</param>
    /// <returns>建立結果</returns>
    [HttpPost("posts")]
    [Authorize]
    public async Task<ActionResult<ForumPostDto>> CreatePost([FromBody] CreatePostDto postDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            var result = await _forumService.CreatePostAsync(userId.Value, postDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetPostDetail), new { postId = result.Data!.PostID }, result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "新增回覆失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得回覆詳細資訊
    /// </summary>
    /// <param name="postId">回覆ID</param>
    /// <returns>回覆詳細資訊</returns>
    [HttpGet("posts/{postId}")]
    public async Task<ActionResult<ForumPostDto>> GetPostDetail(long postId)
    {
        try
        {
            // 簡化版本：從主題中取得回覆
            var threads = await _forumService.GetThreadsAsync(new ForumSearchDto { Page = 1, PageSize = 100 });
            var posts = new List<ForumPostDto>();
            
            foreach (var thread in threads.Data)
            {
                var threadDetail = await _forumService.GetThreadDetailAsync(thread.ThreadID, 1, 100);
                if (threadDetail.Success)
                {
                    posts.AddRange(threadDetail.Data!.Posts);
                }
            }

            var post = posts.FirstOrDefault(p => p.PostID == postId);
            if (post == null)
            {
                return NotFound(new { message = "回覆不存在" });
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得回覆詳細資訊失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 新增反應
    /// </summary>
    /// <param name="reactionDto">反應資訊</param>
    /// <returns>新增結果</returns>
    [HttpPost("reactions")]
    [Authorize]
    public async Task<ActionResult<bool>> AddReaction([FromBody] AddReactionDto reactionDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            var result = await _forumService.AddReactionAsync(userId.Value, reactionDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "新增反應失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 新增書籤
    /// </summary>
    /// <param name="bookmarkDto">書籤資訊</param>
    /// <returns>新增結果</returns>
    [HttpPost("bookmarks")]
    [Authorize]
    public async Task<ActionResult<bool>> AddBookmark([FromBody] AddBookmarkDto bookmarkDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            var result = await _forumService.AddBookmarkAsync(userId.Value, bookmarkDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "新增書籤失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得使用者書籤列表
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>分頁的書籤列表</returns>
    [HttpGet("bookmarks")]
    [Authorize]
    public async Task<ActionResult<PagedResult<BookmarkDto>>> GetUserBookmarks(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "使用者未認證" });
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "頁碼和每頁大小必須大於 0，每頁大小不能超過 100" });
            }

            var result = await _forumService.GetUserBookmarksAsync(userId.Value, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得書籤列表失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得論壇統計資訊
    /// </summary>
    /// <param name="forumId">論壇ID</param>
    /// <returns>統計資訊</returns>
    [HttpGet("stats/{forumId}")]
    public async Task<ActionResult<ForumStatsDto>> GetForumStats(int forumId)
    {
        try
        {
            var result = await _forumService.GetForumStatsAsync(forumId);
            if (result == null)
            {
                return NotFound(new { message = "論壇不存在" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得論壇統計失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 搜尋論壇內容
    /// </summary>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="searchType">搜尋類型 (threads/posts/all)</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>搜尋結果</returns>
    [HttpGet("search")]
    public async Task<ActionResult<SearchResultDto>> SearchForum(
        [FromQuery] string searchTerm,
        [FromQuery] string searchType = "all",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "搜尋關鍵字不能為空" });
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "頁碼和每頁大小必須大於 0，每頁大小不能超過 100" });
            }

            // 簡化版本：返回模擬搜尋結果
            var result = new SearchResultDto
            {
                SearchTerm = searchTerm,
                SearchType = searchType,
                TotalResults = 0,
                Page = page,
                PageSize = pageSize,
                Threads = new List<ForumThreadDto>(),
                Posts = new List<ForumPostDto>()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "搜尋論壇失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得熱門主題
    /// </summary>
    /// <param name="top">前幾名 (1-50)</param>
    /// <param name="period">期間 (day/week/month)</param>
    /// <returns>熱門主題列表</returns>
    [HttpGet("hot-threads")]
    public async Task<ActionResult<List<ForumThreadDto>>> GetHotThreads(
        [FromQuery] int top = 10,
        [FromQuery] string period = "week")
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            if (top < 1 || top > 50)
            {
                return BadRequest(new { message = "排行榜數量必須在 1-50 之間" });
            }

            if (period != "day" && period != "week" && period != "month")
            {
                return BadRequest(new { message = "期間必須是 day、week 或 month" });
            }

            // 簡化版本：返回模擬熱門主題
            var hotThreads = new List<ForumThreadDto>();
            for (int i = 1; i <= top; i++)
            {
                hotThreads.Add(new ForumThreadDto
                {
                    ThreadID = i,
                    ForumID = 1,
                    ForumName = "一般討論",
                    AuthorUserID = i,
                    AuthorName = $"用戶 {i}",
                    Title = $"熱門主題 {i}",
                    Status = "normal",
                    IsPinned = i <= 3,
                    IsLocked = false,
                    ViewCount = 1000 - (i * 50),
                    ReplyCount = 100 - (i * 5),
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow.AddDays(-i),
                    LastReplyAt = DateTime.UtcNow.AddHours(-i),
                    LastReplyBy = $"用戶 {i + 1}"
                });
            }

            return Ok(hotThreads);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得熱門主題失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得當前使用者ID
    /// </summary>
    /// <returns>使用者ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
}

/// <summary>
/// 搜尋結果 DTO
/// </summary>
public class SearchResultDto
{
    public string SearchTerm { get; set; } = string.Empty;
    public string SearchType { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ForumThreadDto> Threads { get; set; } = new List<ForumThreadDto>();
    public List<ForumPostDto> Posts { get; set; } = new List<ForumPostDto>();
} 