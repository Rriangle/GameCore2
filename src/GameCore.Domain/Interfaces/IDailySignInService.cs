using GameCore.Shared.DTOs.DailySignInDtos;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// Service interface for daily sign-in operations
/// </summary>
public interface IDailySignInService
{
    /// <summary>
    /// Perform daily sign-in for a user
    /// </summary>
    Task<DailySignInResponseDto> SignInAsync(int userId);
    
    /// <summary>
    /// Get user's daily sign-in status for today
    /// </summary>
    Task<DailySignInStatusDto> GetTodayStatusAsync(int userId);
    
    /// <summary>
    /// Get user's sign-in statistics
    /// </summary>
    Task<SignInStatisticsDto> GetUserStatisticsAsync(int userId);
    
    /// <summary>
    /// Get user's sign-in history with pagination
    /// </summary>
    Task<SignInHistoryResponseDto> GetUserHistoryAsync(int userId, int page = 1, int pageSize = 20);
    
    /// <summary>
    /// Get monthly attendance report for a user
    /// </summary>
    Task<MonthlyAttendanceDto> GetMonthlyAttendanceAsync(int userId, int year, int month);
    
    /// <summary>
    /// Get available sign-in rewards
    /// </summary>
    Task<List<SignInRewardDto>> GetAvailableRewardsAsync();
    
    /// <summary>
    /// Check if user can claim a specific reward
    /// </summary>
    Task<bool> CanClaimRewardAsync(int userId, int rewardId);
    
    /// <summary>
    /// Claim a sign-in reward
    /// </summary>
    Task<ClaimRewardResponseDto> ClaimRewardAsync(int userId, int rewardId);
}