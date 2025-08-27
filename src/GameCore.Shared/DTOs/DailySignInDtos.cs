using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs.DailySignInDtos;

/// <summary>
/// Response DTO for daily sign-in operation
/// </summary>
public class DailySignInResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime SignInDate { get; set; }
    public DateTime SignInTime { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int MonthlyPerfectAttendance { get; set; }
    public int PointsEarned { get; set; }
    public bool IsBonusDay { get; set; }
    public int BonusMultiplier { get; set; }
    public string Message { get; set; }
    public List<string> Achievements { get; set; } = new();
}

/// <summary>
/// DTO for daily sign-in status
/// </summary>
public class DailySignInStatusDto
{
    public bool HasSignedInToday { get; set; }
    public DateTime? LastSignInTime { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int MonthlyPerfectAttendance { get; set; }
    public int PointsEarnedToday { get; set; }
    public bool IsBonusDay { get; set; }
    public int BonusMultiplier { get; set; }
    public TimeSpan TimeUntilNextSignIn { get; set; }
}

/// <summary>
/// DTO for sign-in statistics
/// </summary>
public class SignInStatisticsDto
{
    public int TotalSignIns { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int MonthlyPerfectAttendance { get; set; }
    public int TotalPointsEarned { get; set; }
    public int AveragePointsPerSignIn { get; set; }
    public int BonusDaysCount { get; set; }
    public DateTime FirstSignInDate { get; set; }
    public DateTime LastSignInDate { get; set; }
}

/// <summary>
/// DTO for sign-in history response
/// </summary>
public class SignInHistoryResponseDto
{
    public List<SignInHistoryItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// DTO for individual sign-in history item
/// </summary>
public class SignInHistoryItemDto
{
    public int Id { get; set; }
    public DateTime SignInDate { get; set; }
    public DateTime SignInTime { get; set; }
    public int PointsEarned { get; set; }
    public bool IsStreakContinued { get; set; }
    public bool IsBonusDay { get; set; }
    public int BonusMultiplier { get; set; }
}

/// <summary>
/// DTO for monthly attendance
/// </summary>
public class MonthlyAttendanceDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalDays { get; set; }
    public int SignInDays { get; set; }
    public int MissedDays { get; set; }
    public double AttendanceRate { get; set; }
    public int PerfectAttendanceDays { get; set; }
    public List<DailyAttendanceDto> DailyRecords { get; set; } = new();
}

/// <summary>
/// DTO for daily attendance record
/// </summary>
public class DailyAttendanceDto
{
    public int Day { get; set; }
    public bool HasSignedIn { get; set; }
    public DateTime? SignInTime { get; set; }
    public int PointsEarned { get; set; }
    public bool IsStreakContinued { get; set; }
}

/// <summary>
/// DTO for sign-in reward
/// </summary>
public class SignInRewardDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int PointsReward { get; set; }
    public int StreakRequirement { get; set; }
    public int AttendanceRequirement { get; set; }
    public bool IsActive { get; set; }
    public bool CanClaim { get; set; }
}

/// <summary>
/// DTO for claiming reward response
/// </summary>
public class ClaimRewardResponseDto
{
    public int RewardId { get; set; }
    public string RewardName { get; set; }
    public int PointsEarned { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

/// <summary>
/// DTO for sign-in streak milestone
/// </summary>
public class StreakMilestoneDto
{
    public int StreakDays { get; set; }
    public int PointsReward { get; set; }
    public string AchievementName { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}