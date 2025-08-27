using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

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
    /// 每日簽到
    /// </summary>
    /// <returns>簽到結果</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SignInResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> SignIn()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到簽到請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _signInService.SignInAsync(userId);

        if (result.Success)
        {
            _logger.LogInformation("簽到成功: {CorrelationId}, UserId: {UserId}, Points: {Points}, Exp: {Exp}, Streak: {Streak}", 
                correlationId, userId, result.PointsEarned, result.ExpEarned, result.StreakCount);
            return Ok(ApiResponse<SignInResult>.SuccessResult(result));
        }

        _logger.LogWarning("簽到失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "簽到失敗"));
    }

    /// <summary>
    /// 獲取簽到統計
    /// </summary>
    /// <returns>簽到統計資訊</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<SignInStatsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetSignInStats()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取簽到統計請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var stats = await _signInService.GetSignInStatsAsync(userId);
        if (stats == null)
        {
            _logger.LogWarning("無法獲取簽到統計: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return NotFound(ApiResponse<object>.ErrorResult("無法獲取簽到統計"));
        }

        _logger.LogInformation("成功獲取簽到統計: {CorrelationId}, UserId: {UserId}", correlationId, userId);
        return Ok(ApiResponse<SignInStatsDto>.SuccessResult(stats));
    }

    /// <summary>
    /// 檢查今日是否已簽到
    /// </summary>
    /// <returns>是否已簽到</returns>
    [HttpGet("today")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> HasSignedInToday()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到檢查今日簽到請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var hasSignedIn = await _signInService.HasSignedInTodayAsync(userId);

        _logger.LogInformation("檢查今日簽到完成: {CorrelationId}, UserId: {UserId}, HasSignedIn: {HasSignedIn}", 
            correlationId, userId, hasSignedIn);
        return Ok(ApiResponse<bool>.SuccessResult(hasSignedIn));
    }
}