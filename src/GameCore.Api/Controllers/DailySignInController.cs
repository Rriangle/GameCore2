using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs.DailySignInDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// Controller for daily sign-in operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailySignInController : ControllerBase
{
    private readonly IDailySignInService _dailySignInService;
    private readonly ILogger<DailySignInController> _logger;

    public DailySignInController(IDailySignInService dailySignInService, ILogger<DailySignInController> logger)
    {
        _dailySignInService = dailySignInService;
        _logger = logger;
    }

    /// <summary>
    /// Perform daily sign-in for the authenticated user
    /// </summary>
    /// <returns>Sign-in result with points and achievements</returns>
    [HttpPost("signin")]
    public async Task<ActionResult<DailySignInResponseDto>> SignIn()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _dailySignInService.SignInAsync(userId.Value);
            
            _logger.LogInformation("User {UserId} performed daily sign-in", userId);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during daily sign-in");
            return StatusCode(500, "An error occurred during sign-in");
        }
    }

    /// <summary>
    /// Get today's sign-in status for the authenticated user
    /// </summary>
    /// <returns>Today's sign-in status</returns>
    [HttpGet("today")]
    public async Task<ActionResult<DailySignInStatusDto>> GetTodayStatus()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var status = await _dailySignInService.GetTodayStatusAsync(userId.Value);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's sign-in status");
            return StatusCode(500, "An error occurred while getting status");
        }
    }

    /// <summary>
    /// Get sign-in statistics for the authenticated user
    /// </summary>
    /// <returns>User's sign-in statistics</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<SignInStatisticsDto>> GetUserStatistics()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var statistics = await _dailySignInService.GetUserStatisticsAsync(userId.Value);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user sign-in statistics");
            return StatusCode(500, "An error occurred while getting statistics");
        }
    }

    /// <summary>
    /// Get sign-in history for the authenticated user
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Paginated sign-in history</returns>
    [HttpGet("history")]
    public async Task<ActionResult<SignInHistoryResponseDto>> GetUserHistory(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid page or page size parameters");
            }

            var history = await _dailySignInService.GetUserHistoryAsync(userId.Value, page, pageSize);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user sign-in history");
            return StatusCode(500, "An error occurred while getting history");
        }
    }

    /// <summary>
    /// Get monthly attendance for the authenticated user
    /// </summary>
    /// <param name="year">Year (default: current year)</param>
    /// <param name="month">Month (default: current month)</param>
    /// <returns>Monthly attendance report</returns>
    [HttpGet("attendance/{year:int}/{month:int}")]
    public async Task<ActionResult<MonthlyAttendanceDto>> GetMonthlyAttendance(
        int year, 
        int month)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            if (year < 2020 || year > 2030 || month < 1 || month > 12)
            {
                return BadRequest("Invalid year or month parameters");
            }

            var attendance = await _dailySignInService.GetMonthlyAttendanceAsync(userId.Value, year, month);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monthly attendance");
            return StatusCode(500, "An error occurred while getting attendance");
        }
    }

    /// <summary>
    /// Get available sign-in rewards
    /// </summary>
    /// <returns>List of available rewards</returns>
    [HttpGet("rewards")]
    public async Task<ActionResult<List<SignInRewardDto>>> GetAvailableRewards()
    {
        try
        {
            var rewards = await _dailySignInService.GetAvailableRewardsAsync();
            return Ok(rewards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available rewards");
            return StatusCode(500, "An error occurred while getting rewards");
        }
    }

    /// <summary>
    /// Check if user can claim a specific reward
    /// </summary>
    /// <param name="rewardId">Reward ID to check</param>
    /// <returns>Whether the reward can be claimed</returns>
    [HttpGet("rewards/{rewardId:int}/can-claim")]
    public async Task<ActionResult<bool>> CanClaimReward(int rewardId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var canClaim = await _dailySignInService.CanClaimRewardAsync(userId.Value, rewardId);
            return Ok(canClaim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user can claim reward {RewardId}", rewardId);
            return StatusCode(500, "An error occurred while checking reward eligibility");
        }
    }

    /// <summary>
    /// Claim a sign-in reward
    /// </summary>
    /// <param name="rewardId">Reward ID to claim</param>
    /// <returns>Reward claim result</returns>
    [HttpPost("rewards/{rewardId:int}/claim")]
    public async Task<ActionResult<ClaimRewardResponseDto>> ClaimReward(int rewardId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _dailySignInService.ClaimRewardAsync(userId.Value, rewardId);
            
            if (result.Success)
            {
                _logger.LogInformation("User {UserId} claimed reward {RewardId}", userId, rewardId);
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error claiming reward {RewardId}", rewardId);
            return StatusCode(500, "An error occurred while claiming reward");
        }
    }

    /// <summary>
    /// Get current month attendance for the authenticated user
    /// </summary>
    /// <returns>Current month attendance report</returns>
    [HttpGet("attendance/current")]
    public async Task<ActionResult<MonthlyAttendanceDto>> GetCurrentMonthAttendance()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var now = DateTime.UtcNow;
            var attendance = await _dailySignInService.GetMonthlyAttendanceAsync(userId.Value, now.Year, now.Month);
            return Ok(attendance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current month attendance");
            return StatusCode(500, "An error occurred while getting attendance");
        }
    }

    private int? GetUserIdFromToken()
    {
        // In a real implementation, this would extract the user ID from the JWT token
        // For now, we'll use a placeholder that would be replaced with actual token parsing
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}