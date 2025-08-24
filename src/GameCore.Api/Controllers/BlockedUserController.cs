using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlockedUserController : ControllerBase
{
    private readonly IBlockedUserService _blockedUserService;
    private readonly ILogger<BlockedUserController> _logger;

    public BlockedUserController(IBlockedUserService blockedUserService, ILogger<BlockedUserController> logger)
    {
        _blockedUserService = blockedUserService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取用戶封鎖的用戶列表
    /// </summary>
    /// <returns>封鎖用戶列表</returns>
    [HttpGet("blocked")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BlockedUserDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetBlockedUsers()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取封鎖用戶列表請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var blockedUsers = await _blockedUserService.GetBlockedUsersAsync(userId);

        _logger.LogInformation("成功獲取封鎖用戶列表: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, blockedUsers.Count());
        return Ok(ApiResponse<IEnumerable<BlockedUserDto>>.SuccessResult(blockedUsers));
    }

    /// <summary>
    /// 獲取封鎖當前用戶的用戶列表
    /// </summary>
    /// <returns>封鎖者列表</returns>
    [HttpGet("blocked-by")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BlockedUserDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetBlockedByUsers()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取封鎖者列表請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var blockedByUsers = await _blockedUserService.GetBlockedByUsersAsync(userId);

        _logger.LogInformation("成功獲取封鎖者列表: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, blockedByUsers.Count());
        return Ok(ApiResponse<IEnumerable<BlockedUserDto>>.SuccessResult(blockedByUsers));
    }

    /// <summary>
    /// 封鎖用戶
    /// </summary>
    /// <param name="request">封鎖請求</param>
    /// <returns>封鎖結果</returns>
    [HttpPost("block")]
    [ProducesResponseType(typeof(ApiResponse<BlockedUserResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到封鎖用戶請求: {CorrelationId}, BlockedId: {BlockedId}", 
            correlationId, request.BlockedId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("封鎖用戶請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _blockedUserService.BlockUserAsync(userId, request.BlockedId, request.Reason);

        if (result.Success)
        {
            _logger.LogInformation("用戶封鎖成功: {CorrelationId}, BlockerId: {BlockerId}, BlockedId: {BlockedId}", 
                correlationId, userId, request.BlockedId);
            return Ok(ApiResponse<BlockedUserResult>.SuccessResult(result));
        }

        _logger.LogWarning("用戶封鎖失敗: {CorrelationId}, BlockerId: {BlockerId}, BlockedId: {BlockedId}, Message: {Message}", 
            correlationId, userId, request.BlockedId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "封鎖失敗"));
    }

    /// <summary>
    /// 解除封鎖用戶
    /// </summary>
    /// <param name="blockedId">被封鎖用戶ID</param>
    /// <returns>解除封鎖結果</returns>
    [HttpDelete("unblock/{blockedId}")]
    [ProducesResponseType(typeof(ApiResponse<BlockedUserResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UnblockUser(int blockedId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到解除封鎖用戶請求: {CorrelationId}, BlockedId: {BlockedId}", 
            correlationId, blockedId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _blockedUserService.UnblockUserAsync(userId, blockedId);

        if (result.Success)
        {
            _logger.LogInformation("用戶解除封鎖成功: {CorrelationId}, BlockerId: {BlockerId}, BlockedId: {BlockedId}", 
                correlationId, userId, blockedId);
            return Ok(ApiResponse<BlockedUserResult>.SuccessResult(result));
        }

        _logger.LogWarning("用戶解除封鎖失敗: {CorrelationId}, BlockerId: {BlockerId}, BlockedId: {BlockedId}, Message: {Message}", 
            correlationId, userId, blockedId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "解除封鎖失敗"));
    }

    /// <summary>
    /// 檢查是否封鎖了指定用戶
    /// </summary>
    /// <param name="blockedId">被檢查用戶ID</param>
    /// <returns>是否封鎖</returns>
    [HttpGet("is-blocked/{blockedId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> IsUserBlocked(int blockedId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到檢查用戶封鎖狀態請求: {CorrelationId}, BlockedId: {BlockedId}", 
            correlationId, blockedId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var isBlocked = await _blockedUserService.IsUserBlockedAsync(userId, blockedId);

        _logger.LogInformation("成功檢查用戶封鎖狀態: {CorrelationId}, BlockerId: {BlockerId}, BlockedId: {BlockedId}, IsBlocked: {IsBlocked}", 
            correlationId, userId, blockedId, isBlocked);
        return Ok(ApiResponse<bool>.SuccessResult(isBlocked));
    }

    /// <summary>
    /// 檢查是否被指定用戶封鎖
    /// </summary>
    /// <param name="blockerId">封鎖者ID</param>
    /// <returns>是否被封鎖</returns>
    [HttpGet("is-blocked-by/{blockerId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> IsUserBlockedBy(int blockerId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到檢查是否被用戶封鎖請求: {CorrelationId}, BlockerId: {BlockerId}", 
            correlationId, blockerId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var isBlockedBy = await _blockedUserService.IsUserBlockedByAsync(userId, blockerId);

        _logger.LogInformation("成功檢查是否被用戶封鎖: {CorrelationId}, UserId: {UserId}, BlockerId: {BlockerId}, IsBlockedBy: {IsBlockedBy}", 
            correlationId, userId, blockerId, isBlockedBy);
        return Ok(ApiResponse<bool>.SuccessResult(isBlockedBy));
    }
}

public class BlockUserRequestDto
{
    [Required(ErrorMessage = "被封鎖用戶ID為必填項目")]
    public int BlockedId { get; set; }

    [Required(ErrorMessage = "封鎖原因為必填項目")]
    [StringLength(200, ErrorMessage = "封鎖原因長度不能超過 200 個字元")]
    public string Reason { get; set; } = string.Empty;
}