using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using GameCore.Api.Services;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers;

/// <summary>
/// 小遊戲控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MiniGameController : ControllerBase
{
    private readonly IMiniGameService _miniGameService;

    public MiniGameController(IMiniGameService miniGameService)
    {
        _miniGameService = miniGameService;
    }

    /// <summary>
    /// 開始冒險
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<ApiResponse<MiniGameStartResultDto>>> StartAdventure()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _miniGameService.StartAdventureAsync(userId);
            
            if (result.Success)
            {
                return Ok(ApiResponse<MiniGameStartResultDto>.SuccessResult(result.Data!, "開始冒險成功"));
            }
            
            return BadRequest(ApiResponse<MiniGameStartResultDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MiniGameStartResultDto>.FailureResult($"開始冒險失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 完成冒險
    /// </summary>
    [HttpPost("finish")]
    public async Task<ActionResult<ApiResponse<MiniGameFinishResultDto>>> FinishAdventure([FromBody] MiniGameFinishRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _miniGameService.FinishAdventureAsync(userId, request);
            
            if (result.Success)
            {
                return Ok(ApiResponse<MiniGameFinishResultDto>.SuccessResult(result.Data!, "完成冒險成功"));
            }
            
            return BadRequest(ApiResponse<MiniGameFinishResultDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MiniGameFinishResultDto>.FailureResult($"完成冒險失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 獲取遊戲記錄
    /// </summary>
    [HttpGet("records")]
    public async Task<ActionResult<ApiResponse<PagedResult<MiniGameRecordDto>>>> GetGameRecords(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? result = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result2 = await _miniGameService.GetGameRecordsAsync(userId, page, pageSize, result);
            
            if (result2.Success)
            {
                return Ok(ApiResponse<PagedResult<MiniGameRecordDto>>.SuccessResult(result2.Data!, "獲取遊戲記錄成功"));
            }
            
            return BadRequest(ApiResponse<PagedResult<MiniGameRecordDto>>.FailureResult(result2.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<MiniGameRecordDto>>.FailureResult($"獲取遊戲記錄失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 獲取遊戲統計
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<MiniGameStatsDto>>> GetGameStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _miniGameService.GetGameStatsAsync(userId);
            
            if (result.Success)
            {
                return Ok(ApiResponse<MiniGameStatsDto>.SuccessResult(result.Data!, "獲取遊戲統計成功"));
            }
            
            return BadRequest(ApiResponse<MiniGameStatsDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MiniGameStatsDto>.FailureResult($"獲取遊戲統計失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 獲取當前用戶ID
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("無法獲取用戶ID");
        }
        return userId;
    }
} 