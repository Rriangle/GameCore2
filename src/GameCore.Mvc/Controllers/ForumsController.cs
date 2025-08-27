using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 論壇控制器 - 處理論壇、討論串、回覆、反應與收藏功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ForumsController : ControllerBase
{
    private readonly IForumsService _forumsService;
    private readonly ILogger<ForumsController> _logger;

    public ForumsController(IForumsService forumsService, ILogger<ForumsController> logger)
    {
        _forumsService = forumsService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取所有論壇列表
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ForumDto>>> GetForums()
    {
        try
        {
            var result = await _forumsService.GetForumsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取論壇列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取論壇詳細資訊
    /// </summary>
    [HttpGet("{forumId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ForumDetailDto>> GetForumDetail(int forumId)
    {
        try
        {
            var result = await _forumsService.GetForumDetailAsync(forumId);
            if (result == null)
                return NotFound(new { message = "論壇不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取論壇詳細資訊時發生錯誤: {ForumId}", forumId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取論壇的討論串列表
    /// </summary>
    [HttpGet("{forumId}/threads")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedThreadsDto>> GetForumThreads(int forumId, [FromQuery] ThreadQueryDto query)
    {
        try
        {
            var result = await _forumsService.GetForumThreadsAsync(forumId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取論壇討論串列表時發生錯誤: {ForumId}", forumId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 建立新討論串
    /// </summary>
    [HttpPost("{forumId}/threads")]
    public async Task<ActionResult<ThreadDto>> CreateThread(int forumId, [FromBody] CreateThreadRequestDto request)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _forumsService.CreateThreadAsync(forumId, authorId, request);
            return CreatedAtAction(nameof(GetThreadDetail), new { threadId = result.ThreadId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立討論串時發生錯誤: {ForumId}", forumId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取討論串詳細資訊
    /// </summary>
    [HttpGet("threads/{threadId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ThreadDetailDto>> GetThreadDetail(long threadId)
    {
        try
        {
            var result = await _forumsService.GetThreadDetailAsync(threadId);
            if (result == null)
                return NotFound(new { message = "討論串不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取討論串詳細資訊時發生錯誤: {ThreadId}", threadId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取討論串的回覆列表
    /// </summary>
    [HttpGet("threads/{threadId}/posts")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedThreadPostsDto>> GetThreadPosts(long threadId, [FromQuery] ThreadPostQueryDto query)
    {
        try
        {
            var result = await _forumsService.GetThreadPostsAsync(threadId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取討論串回覆列表時發生錯誤: {ThreadId}", threadId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 建立回覆
    /// </summary>
    [HttpPost("threads/{threadId}/posts")]
    public async Task<ActionResult<ThreadPostDto>> CreateThreadPost(long threadId, [FromBody] CreateThreadPostRequestDto request)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _forumsService.CreateThreadPostAsync(threadId, authorId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立回覆時發生錯誤: {ThreadId}", threadId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 更新回覆內容
    /// </summary>
    [HttpPut("posts/{postId}")]
    public async Task<ActionResult<ThreadPostDto>> UpdateThreadPost(long postId, [FromBody] UpdateThreadPostRequestDto request)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _forumsService.UpdateThreadPostAsync(postId, authorId, request);
            if (result == null)
                return NotFound(new { message = "回覆不存在或無權限修改" });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新回覆時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 刪除回覆
    /// </summary>
    [HttpDelete("posts/{postId}")]
    public async Task<ActionResult> DeleteThreadPost(long postId)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _forumsService.DeleteThreadPostAsync(postId, authorId);
            if (!result)
                return NotFound(new { message = "回覆不存在或無權限刪除" });

            return Ok(new { message = "回覆已成功刪除" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除回覆時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 新增反應
    /// </summary>
    [HttpPost("reactions")]
    public async Task<ActionResult<ReactionResultDto>> AddReaction([FromBody] AddReactionRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _forumsService.AddReactionAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增反應時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 移除反應
    /// </summary>
    [HttpDelete("reactions")]
    public async Task<ActionResult> RemoveReaction([FromBody] RemoveReactionRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _forumsService.RemoveReactionAsync(userId, request);
            if (!result)
                return NotFound(new { message = "反應不存在" });

            return Ok(new { message = "反應已移除" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除反應時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取目標的反應統計
    /// </summary>
    [HttpGet("reactions")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReactionStatsDto>>> GetReactionStats([FromQuery] string targetType, [FromQuery] long targetId)
    {
        try
        {
            var result = await _forumsService.GetReactionStatsAsync(targetType, targetId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取反應統計時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 新增收藏
    /// </summary>
    [HttpPost("bookmarks")]
    public async Task<ActionResult<BookmarkResultDto>> AddBookmark([FromBody] AddBookmarkRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _forumsService.AddBookmarkAsync(userId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增收藏時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 移除收藏
    /// </summary>
    [HttpDelete("bookmarks")]
    public async Task<ActionResult> RemoveBookmark([FromBody] RemoveBookmarkRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _forumsService.RemoveBookmarkAsync(userId, request);
            if (!result)
                return NotFound(new { message = "收藏不存在" });

            return Ok(new { message = "收藏已移除" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除收藏時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取用戶的收藏列表
    /// </summary>
    [HttpGet("bookmarks")]
    public async Task<ActionResult<PagedBookmarksDto>> GetUserBookmarks([FromQuery] BookmarkQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _forumsService.GetUserBookmarksAsync(userId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶收藏列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 搜尋討論串
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedThreadsDto>> SearchThreads([FromQuery] ThreadSearchQueryDto query)
    {
        try
        {
            var result = await _forumsService.SearchThreadsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋討論串時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取熱門討論串
    /// </summary>
    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ThreadDto>>> GetPopularThreads([FromQuery] int? forumId = null, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _forumsService.GetPopularThreadsAsync(forumId, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取熱門討論串時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取最新討論串
    /// </summary>
    [HttpGet("latest")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ThreadDto>>> GetLatestThreads([FromQuery] int? forumId = null, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _forumsService.GetLatestThreadsAsync(forumId, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取最新討論串時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("無效的用戶身份");
        }
        return userId;
    }
}