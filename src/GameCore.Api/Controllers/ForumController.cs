using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumController : ControllerBase
{
    private readonly IForumService _forumService;
    private readonly ILogger<ForumController> _logger;

    public ForumController(IForumService forumService, ILogger<ForumController> logger)
    {
        _forumService = forumService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取論壇帖子列表
    /// </summary>
    /// <param name="status">帖子狀態</param>
    /// <returns>帖子列表</returns>
    [HttpGet("posts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ForumPostDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetPosts([FromQuery] string? status)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取論壇帖子列表請求: {CorrelationId}, Status: {Status}", correlationId, status);

        var posts = await _forumService.GetPostsAsync(status);

        _logger.LogInformation("成功獲取論壇帖子列表: {CorrelationId}, Count: {Count}", correlationId, posts.Count());
        return Ok(ApiResponse<IEnumerable<ForumPostDto>>.SuccessResult(posts));
    }

    /// <summary>
    /// 獲取論壇帖子詳情
    /// </summary>
    /// <param name="postId">帖子ID</param>
    /// <returns>帖子詳情</returns>
    [HttpGet("posts/{postId}")]
    [ProducesResponseType(typeof(ApiResponse<ForumPostDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetPost(int postId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取論壇帖子詳情請求: {CorrelationId}, PostId: {PostId}", correlationId, postId);

        var post = await _forumService.GetPostAsync(postId);
        if (post == null)
        {
            _logger.LogWarning("論壇帖子不存在: {CorrelationId}, PostId: {PostId}", correlationId, postId);
            return NotFound(ApiResponse<object>.ErrorResult("帖子不存在"));
        }

        _logger.LogInformation("成功獲取論壇帖子詳情: {CorrelationId}, PostId: {PostId}", correlationId, postId);
        return Ok(ApiResponse<ForumPostDto>.SuccessResult(post));
    }

    /// <summary>
    /// 創建論壇帖子
    /// </summary>
    /// <param name="request">帖子創建請求</param>
    /// <returns>創建結果</returns>
    [HttpPost("posts")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumPostResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建論壇帖子請求: {CorrelationId}, Title: {Title}", correlationId, request.Title);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建論壇帖子請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.CreatePostAsync(userId, request.Title, request.Content);

        if (result.Success)
        {
            _logger.LogInformation("論壇帖子創建成功: {CorrelationId}, UserId: {UserId}, PostId: {PostId}", 
                correlationId, userId, result.Post?.PostId);
            return Ok(ApiResponse<ForumPostResult>.SuccessResult(result));
        }

        _logger.LogWarning("論壇帖子創建失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "帖子創建失敗"));
    }

    /// <summary>
    /// 更新論壇帖子
    /// </summary>
    /// <param name="postId">帖子ID</param>
    /// <param name="request">更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("posts/{postId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumPostResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdatePost(int postId, [FromBody] UpdatePostRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新論壇帖子請求: {CorrelationId}, PostId: {PostId}", correlationId, postId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新論壇帖子請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.UpdatePostAsync(postId, userId, request.Title, request.Content);

        if (result.Success)
        {
            _logger.LogInformation("論壇帖子更新成功: {CorrelationId}, PostId: {PostId}, UserId: {UserId}", 
                correlationId, postId, userId);
            return Ok(ApiResponse<ForumPostResult>.SuccessResult(result));
        }

        _logger.LogWarning("論壇帖子更新失敗: {CorrelationId}, PostId: {PostId}, UserId: {UserId}, Message: {Message}", 
            correlationId, postId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "帖子更新失敗"));
    }

    /// <summary>
    /// 更新帖子狀態
    /// </summary>
    /// <param name="postId">帖子ID</param>
    /// <param name="request">狀態更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("posts/{postId}/status")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumPostResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdatePostStatus(int postId, [FromBody] UpdatePostStatusRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新帖子狀態請求: {CorrelationId}, PostId: {PostId}, Status: {Status}", 
            correlationId, postId, request.Status);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新帖子狀態請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.UpdatePostStatusAsync(postId, userId, request.Status);

        if (result.Success)
        {
            _logger.LogInformation("帖子狀態更新成功: {CorrelationId}, PostId: {PostId}, Status: {Status}", 
                correlationId, postId, request.Status);
            return Ok(ApiResponse<ForumPostResult>.SuccessResult(result));
        }

        _logger.LogWarning("帖子狀態更新失敗: {CorrelationId}, PostId: {PostId}, Status: {Status}, Message: {Message}", 
            correlationId, postId, request.Status, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "狀態更新失敗"));
    }

    /// <summary>
    /// 獲取帖子主題列表
    /// </summary>
    /// <param name="postId">帖子ID</param>
    /// <returns>主題列表</returns>
    [HttpGet("posts/{postId}/threads")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ForumThreadDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetThreads(int postId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取帖子主題列表請求: {CorrelationId}, PostId: {PostId}", correlationId, postId);

        var threads = await _forumService.GetThreadsAsync(postId);

        _logger.LogInformation("成功獲取帖子主題列表: {CorrelationId}, PostId: {PostId}, Count: {Count}", 
            correlationId, postId, threads.Count());
        return Ok(ApiResponse<IEnumerable<ForumThreadDto>>.SuccessResult(threads));
    }

    /// <summary>
    /// 創建主題
    /// </summary>
    /// <param name="postId">帖子ID</param>
    /// <param name="request">主題創建請求</param>
    /// <returns>創建結果</returns>
    [HttpPost("posts/{postId}/threads")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumThreadResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreateThread(int postId, [FromBody] CreateThreadRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建主題請求: {CorrelationId}, PostId: {PostId}, Title: {Title}", 
            correlationId, postId, request.Title);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建主題請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.CreateThreadAsync(postId, userId, request.Title, request.Content);

        if (result.Success)
        {
            _logger.LogInformation("主題創建成功: {CorrelationId}, PostId: {PostId}, UserId: {UserId}, ThreadId: {ThreadId}", 
                correlationId, postId, userId, result.Thread?.ThreadId);
            return Ok(ApiResponse<ForumThreadResult>.SuccessResult(result));
        }

        _logger.LogWarning("主題創建失敗: {CorrelationId}, PostId: {PostId}, UserId: {UserId}, Message: {Message}", 
            correlationId, postId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "主題創建失敗"));
    }

    /// <summary>
    /// 獲取主題回覆列表
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <returns>回覆列表</returns>
    [HttpGet("threads/{threadId}/posts")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ForumThreadPostDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetThreadPosts(int threadId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取主題回覆列表請求: {CorrelationId}, ThreadId: {ThreadId}", correlationId, threadId);

        var posts = await _forumService.GetThreadPostsAsync(threadId);

        _logger.LogInformation("成功獲取主題回覆列表: {CorrelationId}, ThreadId: {ThreadId}, Count: {Count}", 
            correlationId, threadId, posts.Count());
        return Ok(ApiResponse<IEnumerable<ForumThreadPostDto>>.SuccessResult(posts));
    }

    /// <summary>
    /// 創建主題回覆
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <param name="request">回覆創建請求</param>
    /// <returns>創建結果</returns>
    [HttpPost("threads/{threadId}/posts")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumThreadPostResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreateThreadPost(int threadId, [FromBody] CreateThreadPostRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建主題回覆請求: {CorrelationId}, ThreadId: {ThreadId}", correlationId, threadId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建主題回覆請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.CreateThreadPostAsync(threadId, userId, request.Content);

        if (result.Success)
        {
            _logger.LogInformation("主題回覆創建成功: {CorrelationId}, ThreadId: {ThreadId}, UserId: {UserId}, PostId: {PostId}", 
                correlationId, threadId, userId, result.ThreadPost?.ThreadPostId);
            return Ok(ApiResponse<ForumThreadPostResult>.SuccessResult(result));
        }

        _logger.LogWarning("主題回覆創建失敗: {CorrelationId}, ThreadId: {ThreadId}, UserId: {UserId}, Message: {Message}", 
            correlationId, threadId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "回覆創建失敗"));
    }

    /// <summary>
    /// 添加反應
    /// </summary>
    /// <param name="request">反應請求</param>
    /// <returns>添加結果</returns>
    [HttpPost("reactions")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumReactionResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> AddReaction([FromBody] AddReactionRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到添加反應請求: {CorrelationId}, TargetType: {TargetType}, TargetId: {TargetId}, ReactionType: {ReactionType}", 
            correlationId, request.TargetType, request.TargetId, request.ReactionType);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("添加反應請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.AddReactionAsync(userId, request.TargetType, request.TargetId, request.ReactionType);

        if (result.Success)
        {
            _logger.LogInformation("反應添加成功: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}", 
                correlationId, userId, request.TargetType, request.TargetId);
            return Ok(ApiResponse<ForumReactionResult>.SuccessResult(result));
        }

        _logger.LogWarning("反應添加失敗: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}, Message: {Message}", 
            correlationId, userId, request.TargetType, request.TargetId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "反應添加失敗"));
    }

    /// <summary>
    /// 移除反應
    /// </summary>
    /// <param name="request">移除反應請求</param>
    /// <returns>移除結果</returns>
    [HttpDelete("reactions")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumReactionResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RemoveReaction([FromBody] RemoveReactionRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到移除反應請求: {CorrelationId}, TargetType: {TargetType}, TargetId: {TargetId}, ReactionType: {ReactionType}", 
            correlationId, request.TargetType, request.TargetId, request.ReactionType);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("移除反應請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.RemoveReactionAsync(userId, request.TargetType, request.TargetId, request.ReactionType);

        if (result.Success)
        {
            _logger.LogInformation("反應移除成功: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}", 
                correlationId, userId, request.TargetType, request.TargetId);
            return Ok(ApiResponse<ForumReactionResult>.SuccessResult(result));
        }

        _logger.LogWarning("反應移除失敗: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}, Message: {Message}", 
            correlationId, userId, request.TargetType, request.TargetId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "反應移除失敗"));
    }

    /// <summary>
    /// 添加收藏
    /// </summary>
    /// <param name="request">收藏請求</param>
    /// <returns>添加結果</returns>
    [HttpPost("bookmarks")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumBookmarkResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> AddBookmark([FromBody] AddBookmarkRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到添加收藏請求: {CorrelationId}, TargetType: {TargetType}, TargetId: {TargetId}", 
            correlationId, request.TargetType, request.TargetId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("添加收藏請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.AddBookmarkAsync(userId, request.TargetType, request.TargetId);

        if (result.Success)
        {
            _logger.LogInformation("收藏添加成功: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}", 
                correlationId, userId, request.TargetType, request.TargetId);
            return Ok(ApiResponse<ForumBookmarkResult>.SuccessResult(result));
        }

        _logger.LogWarning("收藏添加失敗: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}, Message: {Message}", 
            correlationId, userId, request.TargetType, request.TargetId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "收藏添加失敗"));
    }

    /// <summary>
    /// 移除收藏
    /// </summary>
    /// <param name="request">移除收藏請求</param>
    /// <returns>移除結果</returns>
    [HttpDelete("bookmarks")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ForumBookmarkResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RemoveBookmark([FromBody] RemoveBookmarkRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到移除收藏請求: {CorrelationId}, TargetType: {TargetType}, TargetId: {TargetId}", 
            correlationId, request.TargetType, request.TargetId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("移除收藏請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _forumService.RemoveBookmarkAsync(userId, request.TargetType, request.TargetId);

        if (result.Success)
        {
            _logger.LogInformation("收藏移除成功: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}", 
                correlationId, userId, request.TargetType, request.TargetId);
            return Ok(ApiResponse<ForumBookmarkResult>.SuccessResult(result));
        }

        _logger.LogWarning("收藏移除失敗: {CorrelationId}, UserId: {UserId}, TargetType: {TargetType}, TargetId: {TargetId}, Message: {Message}", 
            correlationId, userId, request.TargetType, request.TargetId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "收藏移除失敗"));
    }

    /// <summary>
    /// 獲取用戶帖子列表
    /// </summary>
    /// <returns>用戶帖子列表</returns>
    [HttpGet("user/posts")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ForumPostDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUserPosts()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取用戶帖子列表請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var posts = await _forumService.GetUserPostsAsync(userId);

        _logger.LogInformation("成功獲取用戶帖子列表: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, posts.Count());
        return Ok(ApiResponse<IEnumerable<ForumPostDto>>.SuccessResult(posts));
    }

    /// <summary>
    /// 獲取用戶收藏列表
    /// </summary>
    /// <returns>用戶收藏列表</returns>
    [HttpGet("user/bookmarks")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ForumBookmarkDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUserBookmarks()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取用戶收藏列表請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var bookmarks = await _forumService.GetUserBookmarksAsync(userId);

        _logger.LogInformation("成功獲取用戶收藏列表: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, bookmarks.Count());
        return Ok(ApiResponse<IEnumerable<ForumBookmarkDto>>.SuccessResult(bookmarks));
    }
}

public class CreatePostRequestDto
{
    [Required(ErrorMessage = "帖子標題為必填項目")]
    [StringLength(200, ErrorMessage = "帖子標題長度不能超過 200 個字元")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "帖子內容為必填項目")]
    public string Content { get; set; } = string.Empty;
}

public class UpdatePostRequestDto
{
    [Required(ErrorMessage = "帖子標題為必填項目")]
    [StringLength(200, ErrorMessage = "帖子標題長度不能超過 200 個字元")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "帖子內容為必填項目")]
    public string Content { get; set; } = string.Empty;
}

public class UpdatePostStatusRequestDto
{
    [Required(ErrorMessage = "帖子狀態為必填項目")]
    [StringLength(20, ErrorMessage = "帖子狀態長度不能超過 20 個字元")]
    public string Status { get; set; } = string.Empty;
}

public class CreateThreadRequestDto
{
    [Required(ErrorMessage = "主題標題為必填項目")]
    [StringLength(200, ErrorMessage = "主題標題長度不能超過 200 個字元")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "主題內容為必填項目")]
    public string Content { get; set; } = string.Empty;
}

public class CreateThreadPostRequestDto
{
    [Required(ErrorMessage = "回覆內容為必填項目")]
    public string Content { get; set; } = string.Empty;
}

public class AddReactionRequestDto
{
    [Required(ErrorMessage = "目標類型為必填項目")]
    [StringLength(20, ErrorMessage = "目標類型長度不能超過 20 個字元")]
    public string TargetType { get; set; } = string.Empty;

    [Required(ErrorMessage = "目標ID為必填項目")]
    public int TargetId { get; set; }

    [Required(ErrorMessage = "反應類型為必填項目")]
    [StringLength(20, ErrorMessage = "反應類型長度不能超過 20 個字元")]
    public string ReactionType { get; set; } = string.Empty;
}

public class RemoveReactionRequestDto
{
    [Required(ErrorMessage = "目標類型為必填項目")]
    [StringLength(20, ErrorMessage = "目標類型長度不能超過 20 個字元")]
    public string TargetType { get; set; } = string.Empty;

    [Required(ErrorMessage = "目標ID為必填項目")]
    public int TargetId { get; set; }

    [Required(ErrorMessage = "反應類型為必填項目")]
    [StringLength(20, ErrorMessage = "反應類型長度不能超過 20 個字元")]
    public string ReactionType { get; set; } = string.Empty;
}

public class AddBookmarkRequestDto
{
    [Required(ErrorMessage = "目標類型為必填項目")]
    [StringLength(20, ErrorMessage = "目標類型長度不能超過 20 個字元")]
    public string TargetType { get; set; } = string.Empty;

    [Required(ErrorMessage = "目標ID為必填項目")]
    public int TargetId { get; set; }
}

public class RemoveBookmarkRequestDto
{
    [Required(ErrorMessage = "目標類型為必填項目")]
    [StringLength(20, ErrorMessage = "目標類型長度不能超過 20 個字元")]
    public string TargetType { get; set; } = string.Empty;

    [Required(ErrorMessage = "目標ID為必填項目")]
    public int TargetId { get; set; }
}