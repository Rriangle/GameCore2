using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using GameCore.Api.Services;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers;

/// <summary>
/// 虛擬寵物控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetController : ControllerBase
{
    private readonly IPetService _petService;
    private readonly ILogger<PetController> _logger;

    public PetController(IPetService petService, ILogger<PetController> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取寵物資訊
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 120)]
    public async Task<ActionResult<ApiResponse<PetDto>>> GetPet()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogDebug("Getting pet for user {UserId}", userId);
            
            var result = await _petService.GetPetAsync(userId);
            
            if (result.Success)
            {
                return Ok(ApiResponse<PetDto>.SuccessResult(result.Data!, "獲取寵物資訊成功"));
            }
            
            _logger.LogWarning("Failed to get pet for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(ApiResponse<PetDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pet for user");
            return StatusCode(500, ApiResponse<PetDto>.FailureResult($"獲取寵物資訊失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 更新寵物資料
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<PetDto>>> UpdatePetProfile([FromBody] UpdatePetProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Updating pet profile for user {UserId}", userId);
            
            var result = await _petService.UpdatePetProfileAsync(userId, request);
            
            if (result.Success)
            {
                _logger.LogInformation("Pet profile updated successfully for user {UserId}", userId);
                return Ok(ApiResponse<PetDto>.SuccessResult(result.Data!, "更新寵物資料成功"));
            }
            
            _logger.LogWarning("Failed to update pet profile for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(ApiResponse<PetDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update pet profile for user");
            return StatusCode(500, ApiResponse<PetDto>.FailureResult($"更新寵物資料失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 寵物互動（餵食、洗澡、玩耍、休息）
    /// </summary>
    [HttpPost("actions/{action}")]
    public async Task<ActionResult<ApiResponse<PetActionResultDto>>> PetAction(string action)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Pet action {Action} for user {UserId}", action, userId);
            
            var result = await _petService.PerformActionAsync(userId, action);
            
            if (result.Success)
            {
                return Ok(ApiResponse<PetActionResultDto>.SuccessResult(result.Data!, "寵物互動成功"));
            }
            
            _logger.LogWarning("Pet action {Action} failed for user {UserId}: {Message}", action, userId, result.Message);
            return BadRequest(ApiResponse<PetActionResultDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pet action {Action} failed for user", action);
            return StatusCode(500, ApiResponse<PetActionResultDto>.FailureResult($"寵物互動失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 寵物換色
    /// </summary>
    [HttpPost("recolor")]
    public async Task<ActionResult<ApiResponse<PetRecolorResultDto>>> RecolorPet([FromBody] PetRecolorRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _petService.RecolorPetAsync(userId, request);
            
            if (result.Success)
            {
                return Ok(ApiResponse<PetRecolorResultDto>.SuccessResult(result.Data!, "寵物換色成功"));
            }
            
            return BadRequest(ApiResponse<PetRecolorResultDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PetRecolorResultDto>.FailureResult($"寵物換色失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 獲取寵物歷史記錄
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<PagedResult<PetHistoryDto>>>> GetPetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _petService.GetPetHistoryAsync(userId, page, pageSize);
            
            if (result.Success)
            {
                return Ok(ApiResponse<PagedResult<PetHistoryDto>>.SuccessResult(result.Data!, "獲取寵物歷史記錄成功"));
            }
            
            return BadRequest(ApiResponse<PagedResult<PetHistoryDto>>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<PetHistoryDto>>.FailureResult($"獲取寵物歷史記錄失敗：{ex.Message}"));
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