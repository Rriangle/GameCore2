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
public class MiniGameController : ControllerBase
{
    private readonly IMiniGameService _miniGameService;
    private readonly ILogger<MiniGameController> _logger;

    public MiniGameController(IMiniGameService miniGameService, ILogger<MiniGameController> logger)
    {
        _miniGameService = miniGameService;
        _logger = logger;
    }

    /// <summary>
    /// 進行小遊戲
    /// </summary>
    /// <param name="request">遊戲請求</param>
    /// <returns>遊戲結果</returns>
    [HttpPost("play")]
    [ProducesResponseType(typeof(ApiResponse<MiniGameResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> PlayMiniGame([FromBody] PlayMiniGameRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到小遊戲請求: {CorrelationId}, GameType: {GameType}", correlationId, request.GameType);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("小遊戲請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _miniGameService.PlayMiniGameAsync(userId, request.GameType);

        if (result.Success)
        {
            _logger.LogInformation("小遊戲完成: {CorrelationId}, UserId: {UserId}, GameType: {GameType}, Win: {IsWin}, Score: {Score}, Points: {Points}, Exp: {Exp}", 
                correlationId, userId, request.GameType, result.IsWin, result.Score, result.PointsEarned, result.ExpEarned);
            return Ok(ApiResponse<MiniGameResult>.SuccessResult(result));
        }

        _logger.LogWarning("小遊戲失敗: {CorrelationId}, UserId: {UserId}, GameType: {GameType}, Message: {Message}", 
            correlationId, userId, request.GameType, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "遊戲失敗"));
    }

    /// <summary>
    /// 獲取用戶遊戲記錄
    /// </summary>
    /// <returns>遊戲記錄列表</returns>
    [HttpGet("records")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MiniGameRecordDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGameRecords()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取遊戲記錄請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var records = await _miniGameService.GetUserGameRecordsAsync(userId);

        _logger.LogInformation("成功獲取遊戲記錄: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, records.Count());
        return Ok(ApiResponse<IEnumerable<MiniGameRecordDto>>.SuccessResult(records));
    }

    /// <summary>
    /// 獲取用戶遊戲統計
    /// </summary>
    /// <returns>遊戲統計資訊</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<MiniGameStatsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGameStats()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取遊戲統計請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var stats = await _miniGameService.GetUserGameStatsAsync(userId);
        if (stats == null)
        {
            _logger.LogWarning("無法獲取遊戲統計: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return NotFound(ApiResponse<object>.ErrorResult("無法獲取遊戲統計"));
        }

        _logger.LogInformation("成功獲取遊戲統計: {CorrelationId}, UserId: {UserId}, TotalGames: {TotalGames}, WinRate: {WinRate}%", 
            correlationId, userId, stats.TotalGames, stats.WinRate);
        return Ok(ApiResponse<MiniGameStatsDto>.SuccessResult(stats));
    }
}

public class PlayMiniGameRequestDto
{
    [Required(ErrorMessage = "遊戲類型為必填項目")]
    [StringLength(50, ErrorMessage = "遊戲類型長度不能超過 50 個字元")]
    public string GameType { get; set; } = string.Empty; // puzzle, action, strategy, etc.
}