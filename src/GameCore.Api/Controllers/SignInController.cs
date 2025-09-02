using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using GameCore.Api.Services;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers;

/// <summary>
/// 每日簽到控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SignInController : ControllerBase
{
    private readonly ISignInService _signInService;
    private readonly ILogger<SignInController> _logger;

    public SignInController(ISignInService signInService, ILogger<SignInController> logger)
    {
        _signInService = signInService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取簽到狀態
    /// </summary>
    [HttpGet("status")]
    [ResponseCache(Duration = 300)]
    public async Task<ActionResult<ApiResponse<SignInStatusDto>>> GetSignInStatus()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogDebug("Getting sign-in status for user {UserId}", userId);
            
            var result = await _signInService.GetSignInStatusAsync(userId);
            
            if (result.Success)
            {
                return Ok(ApiResponse<SignInStatusDto>.SuccessResult(result.Data!, "獲取簽到狀態成功"));
            }
            
            _logger.LogWarning("Failed to get sign-in status for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(ApiResponse<SignInStatusDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sign-in status for user");
            return StatusCode(500, ApiResponse<SignInStatusDto>.FailureResult($"獲取簽到狀態失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 執行每日簽到
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<SignInResultDto>>> SignIn()
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Processing sign-in for user {UserId}", userId);
            
            var result = await _signInService.SignInAsync(userId);
            
            if (result.Success)
            {
                _logger.LogInformation("Sign-in successful for user {UserId}", userId);
                return Ok(ApiResponse<SignInResultDto>.SuccessResult(result.Data!, "簽到成功"));
            }
            
            _logger.LogWarning("Sign-in failed for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(ApiResponse<SignInResultDto>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign-in failed for user");
            return StatusCode(500, ApiResponse<SignInResultDto>.FailureResult($"簽到失敗：{ex.Message}"));
        }
    }

    /// <summary>
    /// 獲取簽到歷史
    /// </summary>
    [HttpGet("history")]
    [ResponseCache(Duration = 600, VaryByQueryKeys = new[] { "page", "pageSize" })]
    public async Task<ActionResult<ApiResponse<PagedResult<SignInHistoryDto>>>> GetSignInHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            _logger.LogDebug("Getting sign-in history for user {UserId}, page {Page}", userId, page);
            
            var result = await _signInService.GetSignInHistoryAsync(userId, page, pageSize);
            
            if (result.Success)
            {
                return Ok(ApiResponse<PagedResult<SignInHistoryDto>>.SuccessResult(result.Data!, "獲取簽到歷史成功"));
            }
            
            _logger.LogWarning("Failed to get sign-in history for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(ApiResponse<PagedResult<SignInHistoryDto>>.FailureResult(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sign-in history for user");
            return StatusCode(500, ApiResponse<PagedResult<SignInHistoryDto>>.FailureResult($"獲取簽到歷史失敗：{ex.Message}"));
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