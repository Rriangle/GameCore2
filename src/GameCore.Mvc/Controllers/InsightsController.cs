using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 洞察控制器 - 處理洞察貼文、數據快照、分析相關功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InsightsController : ControllerBase
{
    private readonly IInsightsService _insightsService;
    private readonly ILogger<InsightsController> _logger;

    public InsightsController(IInsightsService insightsService, ILogger<InsightsController> logger)
    {
        _insightsService = insightsService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取洞察貼文列表
    /// </summary>
    [HttpGet("posts")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedPostsDto>> GetInsightPosts([FromQuery] PostQueryDto query)
    {
        try
        {
            var result = await _insightsService.GetInsightPostsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取洞察貼文列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取洞察貼文詳情
    /// </summary>
    [HttpGet("posts/{postId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDetailDto>> GetInsightPostDetail(int postId)
    {
        try
        {
            var result = await _insightsService.GetInsightPostDetailAsync(postId);
            if (result == null)
                return NotFound(new { message = "洞察貼文不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取洞察貼文詳情時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 建立洞察貼文
    /// </summary>
    [HttpPost("posts")]
    [Authorize]
    public async Task<ActionResult<PostDto>> CreateInsightPost([FromBody] CreatePostRequestDto request)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _insightsService.CreateInsightPostAsync(authorId, request);
            return CreatedAtAction(nameof(GetInsightPostDetail), new { postId = result.PostId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立洞察貼文時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 更新洞察貼文
    /// </summary>
    [HttpPut("posts/{postId}")]
    [Authorize]
    public async Task<ActionResult<PostDto>> UpdateInsightPost(int postId, [FromBody] UpdatePostRequestDto request)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _insightsService.UpdateInsightPostAsync(postId, authorId, request);
            if (result == null)
                return NotFound(new { message = "洞察貼文不存在或無權限修改" });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新洞察貼文時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 刪除洞察貼文
    /// </summary>
    [HttpDelete("posts/{postId}")]
    [Authorize]
    public async Task<ActionResult> DeleteInsightPost(int postId)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _insightsService.DeleteInsightPostAsync(postId, authorId);
            if (!result)
                return NotFound(new { message = "洞察貼文不存在或無權限刪除" });

            return Ok(new { message = "洞察貼文已刪除" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除洞察貼文時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 發佈洞察貼文
    /// </summary>
    [HttpPost("posts/{postId}/publish")]
    [Authorize]
    public async Task<ActionResult<PostDto>> PublishInsightPost(int postId)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _insightsService.PublishInsightPostAsync(postId, authorId);
            if (result == null)
                return NotFound(new { message = "洞察貼文不存在或無權限發佈" });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發佈洞察貼文時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 搜尋洞察貼文
    /// </summary>
    [HttpGet("posts/search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedPostsDto>> SearchInsightPosts([FromQuery] PostSearchQueryDto query)
    {
        try
        {
            var result = await _insightsService.SearchInsightPostsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋洞察貼文時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取熱門洞察貼文
    /// </summary>
    [HttpGet("posts/popular")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PostDto>>> GetPopularInsightPosts([FromQuery] int? gameId = null, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _insightsService.GetPopularInsightPostsAsync(gameId, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取熱門洞察貼文時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取最新洞察貼文
    /// </summary>
    [HttpGet("posts/latest")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PostDto>>> GetLatestInsightPosts([FromQuery] int? gameId = null, [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _insightsService.GetLatestInsightPostsAsync(gameId, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取最新洞察貼文時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取置頂洞察貼文
    /// </summary>
    [HttpGet("posts/pinned")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PostDto>>> GetPinnedInsightPosts([FromQuery] int? gameId = null)
    {
        try
        {
            var result = await _insightsService.GetPinnedInsightPostsAsync(gameId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取置頂洞察貼文時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取作者的洞察貼文
    /// </summary>
    [HttpGet("authors/{authorId}/posts")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedPostsDto>> GetAuthorInsightPosts(int authorId, [FromQuery] PostQueryDto query)
    {
        try
        {
            var result = await _insightsService.GetAuthorInsightPostsAsync(authorId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取作者洞察貼文時發生錯誤: {AuthorId}", authorId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取我的洞察貼文
    /// </summary>
    [HttpGet("my-posts")]
    [Authorize]
    public async Task<ActionResult<PagedPostsDto>> GetMyInsightPosts([FromQuery] PostQueryDto query)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _insightsService.GetAuthorInsightPostsAsync(authorId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取我的洞察貼文時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取貼文指標快照
    /// </summary>
    [HttpGet("posts/{postId}/metric-snapshot")]
    [AllowAnonymous]
    public async Task<ActionResult<PostMetricSnapshotDto>> GetPostMetricSnapshot(int postId)
    {
        try
        {
            var result = await _insightsService.GetPostMetricSnapshotAsync(postId);
            if (result == null)
                return NotFound(new { message = "指標快照不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取貼文指標快照時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 建立貼文指標快照
    /// </summary>
    [HttpPost("posts/{postId}/metric-snapshot")]
    [Authorize]
    public async Task<ActionResult<PostMetricSnapshotDto>> CreatePostMetricSnapshot(int postId, [FromBody] CreateMetricSnapshotRequestDto request)
    {
        try
        {
            var result = await _insightsService.CreatePostMetricSnapshotAsync(postId, request.GameId, request.Date);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立貼文指標快照時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取貼文引用來源
    /// </summary>
    [HttpGet("posts/{postId}/sources")]
    [AllowAnonymous]
    public async Task<ActionResult<List<PostSourceDto>>> GetPostSources(int postId)
    {
        try
        {
            var result = await _insightsService.GetPostSourcesAsync(postId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取貼文引用來源時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 新增貼文引用來源
    /// </summary>
    [HttpPost("posts/{postId}/sources")]
    [Authorize]
    public async Task<ActionResult<PostSourceDto>> AddPostSource(int postId, [FromBody] AddPostSourceRequestDto request)
    {
        try
        {
            var result = await _insightsService.AddPostSourceAsync(postId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增貼文引用來源時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 移除貼文引用來源
    /// </summary>
    [HttpDelete("posts/sources/{sourceId}")]
    [Authorize]
    public async Task<ActionResult> RemovePostSource(long sourceId)
    {
        try
        {
            var authorId = GetCurrentUserId();
            var result = await _insightsService.RemovePostSourceAsync(sourceId, authorId);
            if (!result)
                return NotFound(new { message = "引用來源不存在或無權限刪除" });

            return Ok(new { message = "引用來源已移除" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除貼文引用來源時發生錯誤: {SourceId}", sourceId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 設定貼文置頂狀態 (管理員功能)
    /// </summary>
    [HttpPost("posts/{postId}/pin")]
    [Authorize] // 需要管理員權限
    public async Task<ActionResult> SetPostPinnedStatus(int postId, [FromBody] SetPinnedStatusRequestDto request)
    {
        try
        {
            var adminId = GetCurrentUserId();
            var result = await _insightsService.SetPostPinnedStatusAsync(postId, request.Pinned, adminId);
            if (!result)
                return BadRequest(new { message = "設定置頂狀態失敗" });

            return Ok(new { message = request.Pinned ? "貼文已置頂" : "已取消置頂" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定貼文置頂狀態時發生錯誤: {PostId}", postId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲洞察統計
    /// </summary>
    [HttpGet("games/{gameId}/stats")]
    [AllowAnonymous]
    public async Task<ActionResult<GameInsightStatsDto>> GetGameInsightStats(int gameId)
    {
        try
        {
            var result = await _insightsService.GetGameInsightStatsAsync(gameId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲洞察統計時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取洞察貼文趨勢分析
    /// </summary>
    [HttpGet("trends")]
    [AllowAnonymous]
    public async Task<ActionResult<PostTrendAnalysisDto>> GetPostTrendAnalysis([FromQuery] PostTrendQueryDto query)
    {
        try
        {
            var result = await _insightsService.GetPostTrendAnalysisAsync(query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取洞察貼文趨勢分析時發生錯誤");
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

/// <summary>
/// 建立指標快照請求 DTO
/// </summary>
public class CreateMetricSnapshotRequestDto
{
    public int GameId { get; set; }
    public DateOnly Date { get; set; }
}

/// <summary>
/// 設定置頂狀態請求 DTO
/// </summary>
public class SetPinnedStatusRequestDto
{
    public bool Pinned { get; set; }
}