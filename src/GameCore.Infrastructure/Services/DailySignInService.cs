using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs.DailySignInDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// Service implementation for daily sign-in operations
/// </summary>
public class DailySignInService : IDailySignInService
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<DailySignInService> _logger;

    public DailySignInService(GameCoreDbContext context, ILogger<DailySignInService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DailySignInResponseDto> SignInAsync(int userId)
    {
        var today = DateTime.Today;
        var now = DateTime.UtcNow;

        // Check if user has already signed in today
        var existingSignIn = await _context.DailySignIns
            .FirstOrDefaultAsync(ds => ds.UserId == userId && ds.SignInDate == today);

        if (existingSignIn != null)
        {
            return new DailySignInResponseDto
            {
                Id = existingSignIn.Id,
                UserId = existingSignIn.UserId,
                SignInDate = existingSignIn.SignInDate,
                SignInTime = existingSignIn.SignInTime,
                CurrentStreak = existingSignIn.CurrentStreak,
                LongestStreak = existingSignIn.LongestStreak,
                MonthlyPerfectAttendance = existingSignIn.MonthlyPerfectAttendance,
                PointsEarned = existingSignIn.PointsEarned,
                IsBonusDay = existingSignIn.IsBonusDay,
                BonusMultiplier = existingSignIn.BonusMultiplier,
                Message = "You have already signed in today!",
                Achievements = new List<string>()
            };
        }

        // Get user's last sign-in to calculate streak
        var lastSignIn = await _context.DailySignIns
            .Where(ds => ds.UserId == userId)
            .OrderByDescending(ds => ds.SignInDate)
            .FirstOrDefaultAsync();

        var currentStreak = 0;
        var isStreakContinued = false;

        if (lastSignIn != null)
        {
            var daysSinceLastSignIn = (today - lastSignIn.SignInDate).Days;
            if (daysSinceLastSignIn == 1)
            {
                currentStreak = lastSignIn.CurrentStreak + 1;
                isStreakContinued = true;
            }
            else if (daysSinceLastSignIn > 1)
            {
                currentStreak = 1; // Reset streak
            }
        }
        else
        {
            currentStreak = 1; // First sign-in
        }

        // Calculate points and bonus
        var basePoints = 10;
        var streakBonus = Math.Min(currentStreak * 2, 50); // Max 50 points for streak
        var isBonusDay = IsBonusDay(today);
        var bonusMultiplier = isBonusDay ? 2 : 1;
        var totalPoints = (basePoints + streakBonus) * bonusMultiplier;

        // Create new daily sign-in record
        var dailySignIn = new DailySignIn
        {
            UserId = userId,
            SignInDate = today,
            SignInTime = now,
            CurrentStreak = currentStreak,
            LongestStreak = Math.Max(currentStreak, lastSignIn?.LongestStreak ?? 0),
            MonthlyPerfectAttendance = await CalculateMonthlyPerfectAttendance(userId, today),
            PointsEarned = totalPoints,
            IsBonusDay = isBonusDay,
            BonusMultiplier = bonusMultiplier,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.DailySignIns.Add(dailySignIn);

        // Create history record
        var historyRecord = new UserSignInHistory
        {
            UserId = userId,
            SignInDate = today,
            SignInTime = now,
            DayOfWeek = (int)today.DayOfWeek,
            DayOfMonth = today.Day,
            Month = today.Month,
            Year = today.Year,
            WeekOfYear = GetWeekOfYear(today),
            PointsEarned = totalPoints,
            IsStreakContinued = isStreakContinued,
            IsBonusDay = isBonusDay,
            BonusMultiplier = bonusMultiplier,
            CreatedAt = now
        };

        _context.UserSignInHistories.Add(historyRecord);

        // Update user wallet with points
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);

        if (userWallet != null)
        {
            userWallet.Balance += totalPoints;
            userWallet.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // Generate achievements
        var achievements = await GenerateAchievements(userId, currentStreak, dailySignIn.MonthlyPerfectAttendance);

        _logger.LogInformation("User {UserId} signed in successfully. Points earned: {Points}, Streak: {Streak}", 
            userId, totalPoints, currentStreak);

        return new DailySignInResponseDto
        {
            Id = dailySignIn.Id,
            UserId = dailySignIn.UserId,
            SignInDate = dailySignIn.SignInDate,
            SignInTime = dailySignIn.SignInTime,
            CurrentStreak = dailySignIn.CurrentStreak,
            LongestStreak = dailySignIn.LongestStreak,
            MonthlyPerfectAttendance = dailySignIn.MonthlyPerfectAttendance,
            PointsEarned = dailySignIn.PointsEarned,
            IsBonusDay = dailySignIn.IsBonusDay,
            BonusMultiplier = dailySignIn.BonusMultiplier,
            Message = $"Welcome back! You've earned {totalPoints} points. Current streak: {currentStreak} days.",
            Achievements = achievements
        };
    }

    public async Task<DailySignInStatusDto> GetTodayStatusAsync(int userId)
    {
        var today = DateTime.Today;
        var now = DateTime.UtcNow;

        var todaySignIn = await _context.DailySignIns
            .FirstOrDefaultAsync(ds => ds.UserId == userId && ds.SignInDate == today);

        var lastSignIn = await _context.DailySignIns
            .Where(ds => ds.UserId == userId)
            .OrderByDescending(ds => ds.SignInDate)
            .FirstOrDefaultAsync();

        var hasSignedInToday = todaySignIn != null;
        var currentStreak = todaySignIn?.CurrentStreak ?? (lastSignIn?.CurrentStreak ?? 0);
        var longestStreak = lastSignIn?.LongestStreak ?? 0;
        var monthlyPerfectAttendance = todaySignIn?.MonthlyPerfectAttendance ?? 0;
        var pointsEarnedToday = todaySignIn?.PointsEarned ?? 0;

        var isBonusDay = IsBonusDay(today);
        var bonusMultiplier = isBonusDay ? 2 : 1;

        var timeUntilNextSignIn = TimeSpan.Zero;
        if (hasSignedInToday)
        {
            var nextSignIn = today.AddDays(1);
            timeUntilNextSignIn = nextSignIn - now;
        }

        return new DailySignInStatusDto
        {
            HasSignedInToday = hasSignedInToday,
            LastSignInTime = lastSignIn?.SignInTime,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            MonthlyPerfectAttendance = monthlyPerfectAttendance,
            PointsEarnedToday = pointsEarnedToday,
            IsBonusDay = isBonusDay,
            BonusMultiplier = bonusMultiplier,
            TimeUntilNextSignIn = timeUntilNextSignIn
        };
    }

    public async Task<SignInStatisticsDto> GetUserStatisticsAsync(int userId)
    {
        var signIns = await _context.DailySignIns
            .Where(ds => ds.UserId == userId)
            .OrderBy(ds => ds.SignInDate)
            .ToListAsync();

        if (!signIns.Any())
        {
            return new SignInStatisticsDto
            {
                TotalSignIns = 0,
                CurrentStreak = 0,
                LongestStreak = 0,
                MonthlyPerfectAttendance = 0,
                TotalPointsEarned = 0,
                AveragePointsPerSignIn = 0,
                BonusDaysCount = 0,
                FirstSignInDate = DateTime.MinValue,
                LastSignInDate = DateTime.MinValue
            };
        }

        var totalSignIns = signIns.Count;
        var currentStreak = signIns.Last().CurrentStreak;
        var longestStreak = signIns.Max(ds => ds.LongestStreak);
        var monthlyPerfectAttendance = signIns.Last().MonthlyPerfectAttendance;
        var totalPointsEarned = signIns.Sum(ds => ds.PointsEarned);
        var averagePointsPerSignIn = totalPointsEarned / totalSignIns;
        var bonusDaysCount = signIns.Count(ds => ds.IsBonusDay);
        var firstSignInDate = signIns.First().SignInDate;
        var lastSignInDate = signIns.Last().SignInDate;

        return new SignInStatisticsDto
        {
            TotalSignIns = totalSignIns,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            MonthlyPerfectAttendance = monthlyPerfectAttendance,
            TotalPointsEarned = totalPointsEarned,
            AveragePointsPerSignIn = averagePointsPerSignIn,
            BonusDaysCount = bonusDaysCount,
            FirstSignInDate = firstSignInDate,
            LastSignInDate = lastSignInDate
        };
    }

    public async Task<SignInHistoryResponseDto> GetUserHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        var query = _context.UserSignInHistories
            .Where(ush => ush.UserId == userId)
            .OrderByDescending(ush => ush.SignInDate);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ush => new SignInHistoryItemDto
            {
                Id = ush.Id,
                SignInDate = ush.SignInDate,
                SignInTime = ush.SignInTime,
                PointsEarned = ush.PointsEarned,
                IsStreakContinued = ush.IsStreakContinued,
                IsBonusDay = ush.IsBonusDay,
                BonusMultiplier = ush.BonusMultiplier
            })
            .ToListAsync();

        return new SignInHistoryResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<MonthlyAttendanceDto> GetMonthlyAttendanceAsync(int userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var totalDays = endDate.Day;

        var signIns = await _context.DailySignIns
            .Where(ds => ds.UserId == userId && 
                        ds.SignInDate >= startDate && 
                        ds.SignInDate <= endDate)
            .ToListAsync();

        var signInDays = signIns.Count;
        var missedDays = totalDays - signInDays;
        var attendanceRate = totalDays > 0 ? (double)signInDays / totalDays * 100 : 0;

        var dailyRecords = new List<DailyAttendanceDto>();
        for (int day = 1; day <= totalDays; day++)
        {
            var date = new DateTime(year, month, day);
            var signIn = signIns.FirstOrDefault(ds => ds.SignInDate == date);

            dailyRecords.Add(new DailyAttendanceDto
            {
                Day = day,
                HasSignedIn = signIn != null,
                SignInTime = signIn?.SignInTime,
                PointsEarned = signIn?.PointsEarned ?? 0,
                IsStreakContinued = signIn != null && signIn.CurrentStreak > 1
            });
        }

        var perfectAttendanceDays = signIns.Count(ds => ds.MonthlyPerfectAttendance > 0);

        return new MonthlyAttendanceDto
        {
            Year = year,
            Month = month,
            TotalDays = totalDays,
            SignInDays = signInDays,
            MissedDays = missedDays,
            AttendanceRate = attendanceRate,
            PerfectAttendanceDays = perfectAttendanceDays,
            DailyRecords = dailyRecords
        };
    }

    public async Task<List<SignInRewardDto>> GetAvailableRewardsAsync()
    {
        var rewards = await _context.SignInRewards
            .Where(sr => sr.IsActive)
            .OrderBy(sr => sr.StreakRequirement)
            .ThenBy(sr => sr.AttendanceRequirement)
            .ToListAsync();

        return rewards.Select(sr => new SignInRewardDto
        {
            Id = sr.Id,
            Name = sr.Name,
            Description = sr.Description,
            PointsReward = sr.PointsReward,
            StreakRequirement = sr.StreakRequirement,
            AttendanceRequirement = sr.AttendanceRequirement,
            IsActive = sr.IsActive,
            CanClaim = false // Will be set by caller if needed
        }).ToList();
    }

    public async Task<bool> CanClaimRewardAsync(int userId, int rewardId)
    {
        var reward = await _context.SignInRewards
            .FirstOrDefaultAsync(sr => sr.Id == rewardId && sr.IsActive);

        if (reward == null) return false;

        var userStats = await GetUserStatisticsAsync(userId);
        var monthlyAttendance = await GetMonthlyAttendanceAsync(userId, DateTime.UtcNow.Year, DateTime.UtcNow.Month);

        return userStats.CurrentStreak >= reward.StreakRequirement &&
               monthlyAttendance.SignInDays >= reward.AttendanceRequirement;
    }

    public async Task<ClaimRewardResponseDto> ClaimRewardAsync(int userId, int rewardId)
    {
        if (!await CanClaimRewardAsync(userId, rewardId))
        {
            return new ClaimRewardResponseDto
            {
                RewardId = rewardId,
                RewardName = "",
                PointsEarned = 0,
                Success = false,
                Message = "You do not meet the requirements for this reward."
            };
        }

        var reward = await _context.SignInRewards
            .FirstOrDefaultAsync(sr => sr.Id == rewardId);

        if (reward == null)
        {
            return new ClaimRewardResponseDto
            {
                RewardId = rewardId,
                RewardName = "",
                PointsEarned = 0,
                Success = false,
                Message = "Reward not found."
            };
        }

        // Update user wallet
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);

        if (userWallet != null)
        {
            userWallet.Balance += reward.PointsReward;
            userWallet.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} claimed reward {RewardId} and earned {Points} points", 
            userId, rewardId, reward.PointsReward);

        return new ClaimRewardResponseDto
        {
            RewardId = rewardId,
            RewardName = reward.Name,
            PointsEarned = reward.PointsReward,
            Success = true,
            Message = $"Congratulations! You've earned {reward.PointsReward} points for {reward.Name}."
        };
    }

    private bool IsBonusDay(DateTime date)
    {
        // Bonus days: weekends (Saturday = 6, Sunday = 0) and first day of month
        return date.DayOfWeek == DayOfWeek.Saturday || 
               date.DayOfWeek == DayOfWeek.Sunday || 
               date.Day == 1;
    }

    private async Task<int> CalculateMonthlyPerfectAttendance(int userId, DateTime date)
    {
        var startOfMonth = new DateTime(date.Year, date.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        var signInDays = await _context.DailySignIns
            .CountAsync(ds => ds.UserId == userId && 
                             ds.SignInDate >= startOfMonth && 
                             ds.SignInDate <= startOfMonth.AddDays(daysInMonth - 1));

        return signInDays == daysInMonth ? daysInMonth : 0;
    }

    private int GetWeekOfYear(DateTime date)
    {
        var calendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
        return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    private async Task<List<string>> GenerateAchievements(int userId, int currentStreak, int monthlyPerfectAttendance)
    {
        var achievements = new List<string>();

        // Streak achievements
        if (currentStreak == 7)
            achievements.Add("Week Warrior - 7 day streak!");
        else if (currentStreak == 30)
            achievements.Add("Monthly Master - 30 day streak!");
        else if (currentStreak == 100)
            achievements.Add("Century Club - 100 day streak!");

        // Perfect attendance achievements
        if (monthlyPerfectAttendance >= 28)
            achievements.Add("Perfect Month - Excellent attendance!");

        // First sign-in achievement
        var totalSignIns = await _context.DailySignIns.CountAsync(ds => ds.UserId == userId);
        if (totalSignIns == 1)
            achievements.Add("First Steps - Welcome to daily sign-ins!");

        return achievements;
    }
}