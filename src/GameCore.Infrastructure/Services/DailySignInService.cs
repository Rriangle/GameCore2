using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs.DailySignInDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 每日簽到服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class DailySignInService : IDailySignInService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DailySignInService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 30;
    private const int MaxStreakBonus = 50;
    private const int BasePoints = 10;
    private const int StreakMultiplier = 2;
    
    // 快取鍵定義
    private const string TodayStatusCacheKey = "TodayStatus_{0}";
    private const string UserStatisticsCacheKey = "UserStatistics_{0}";
    private const string UserHistoryCacheKey = "UserHistory_{0}_{1}_{2}";
    private const string MonthlyAttendanceCacheKey = "MonthlyAttendance_{0}_{1}_{2}";
    private const string AvailableRewardsCacheKey = "AvailableRewards";
    private const string UserStreakCacheKey = "UserStreak_{0}";

    public DailySignInService(
        GameCoreDbContext context, 
        IMemoryCache memoryCache, 
        ILogger<DailySignInService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DailySignInResponseDto> SignInAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        var today = DateTime.Today;
        var now = DateTime.UtcNow;

        // 檢查用戶是否已經簽到
        var existingSignIn = await _context.DailySignIns
            .AsNoTracking()
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

        // 獲取用戶最後簽到記錄來計算連續簽到
        var lastSignIn = await _context.DailySignIns
            .AsNoTracking()
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
                currentStreak = 1; // 重置連續簽到
            }
        }
        else
        {
            currentStreak = 1; // 首次簽到
        }

        // 計算積分和獎勵
        var streakBonus = Math.Min(currentStreak * StreakMultiplier, MaxStreakBonus);
        var isBonusDay = IsBonusDay(today);
        var bonusMultiplier = isBonusDay ? 2 : 1;
        var totalPoints = (BasePoints + streakBonus) * bonusMultiplier;

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

        // 生成成就
        var achievements = await GenerateAchievements(userId, currentStreak, dailySignIn.MonthlyPerfectAttendance);

        // 清除相關快取
        ClearUserRelatedCache(userId);

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
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(TodayStatusCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out DailySignInStatusDto cachedStatus))
        {
            return cachedStatus;
        }

        var today = DateTime.Today;
        var now = DateTime.UtcNow;

        var todaySignIn = await _context.DailySignIns
            .AsNoTracking()
            .FirstOrDefaultAsync(ds => ds.UserId == userId && ds.SignInDate == today);

        var lastSignIn = await _context.DailySignIns
            .AsNoTracking()
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

        var status = new DailySignInStatusDto
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

        // 存入快取，設定較短的過期時間（因為狀態會頻繁變化）
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        _memoryCache.Set(cacheKey, status, cacheOptions);

        return status;
    }

    public async Task<SignInStatisticsDto> GetUserStatisticsAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(UserStatisticsCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out SignInStatisticsDto cachedStats))
        {
            return cachedStats;
        }

        var signIns = await _context.DailySignIns
            .AsNoTracking()
            .Where(ds => ds.UserId == userId)
            .OrderBy(ds => ds.SignInDate)
            .ToListAsync();

        SignInStatisticsDto statistics;
        if (!signIns.Any())
        {
            statistics = new SignInStatisticsDto
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
        else
        {
            var totalSignIns = signIns.Count;
            var currentStreak = signIns.Last().CurrentStreak;
            var longestStreak = signIns.Max(ds => ds.LongestStreak);
            var monthlyPerfectAttendance = signIns.Last().MonthlyPerfectAttendance;
            var totalPointsEarned = signIns.Sum(ds => ds.PointsEarned);
            var averagePointsPerSignIn = totalPointsEarned / totalSignIns;
            var bonusDaysCount = signIns.Count(ds => ds.IsBonusDay);
            var firstSignInDate = signIns.First().SignInDate;
            var lastSignInDate = signIns.Last().SignInDate;

            statistics = new SignInStatisticsDto
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

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, statistics, cacheOptions);

        return statistics;
    }

    public async Task<SignInHistoryResponseDto> GetUserHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (page < 1)
        {
            throw new ArgumentException("Page must be greater than 0", nameof(page));
        }
        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }
        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(UserHistoryCacheKey, userId, page, pageSize);
        if (_memoryCache.TryGetValue(cacheKey, out SignInHistoryResponseDto cachedHistory))
        {
            return cachedHistory;
        }

        var query = _context.UserSignInHistories
            .AsNoTracking()
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

        var history = new SignInHistoryResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, history, cacheOptions);

        return history;
    }

    public async Task<MonthlyAttendanceDto> GetMonthlyAttendanceAsync(int userId, int year, int month)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (year < 2020 || year > 2030)
        {
            throw new ArgumentException("Year must be between 2020 and 2030", nameof(year));
        }
        if (month < 1 || month > 12)
        {
            throw new ArgumentException("Month must be between 1 and 12", nameof(month));
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(MonthlyAttendanceCacheKey, userId, year, month);
        if (_memoryCache.TryGetValue(cacheKey, out MonthlyAttendanceDto cachedAttendance))
        {
            return cachedAttendance;
        }

        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var totalDays = endDate.Day;

        var signIns = await _context.DailySignIns
            .AsNoTracking()
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

        var attendance = new MonthlyAttendanceDto
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

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, attendance, cacheOptions);

        return attendance;
    }

    public async Task<List<SignInRewardDto>> GetAvailableRewardsAsync()
    {
        // 嘗試從快取獲取
        if (_memoryCache.TryGetValue(AvailableRewardsCacheKey, out List<SignInRewardDto> cachedRewards))
        {
            return cachedRewards;
        }

        var rewards = await _context.SignInRewards
            .AsNoTracking()
            .Where(sr => sr.IsActive)
            .OrderBy(sr => sr.StreakRequirement)
            .ThenBy(sr => sr.AttendanceRequirement)
            .ToListAsync();

        var rewardDtos = rewards.Select(sr => new SignInRewardDto
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

        // 存入快取，獎勵列表變化較少，可以設定較長的過期時間
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };
        _memoryCache.Set(AvailableRewardsCacheKey, rewardDtos, cacheOptions);

        return rewardDtos;
    }

    public async Task<bool> CanClaimRewardAsync(int userId, int rewardId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (rewardId <= 0)
        {
            throw new ArgumentException("Reward ID must be greater than 0", nameof(rewardId));
        }

        var reward = await _context.SignInRewards
            .AsNoTracking()
            .FirstOrDefaultAsync(sr => sr.Id == rewardId && sr.IsActive);

        if (reward == null) return false;

        var userStats = await GetUserStatisticsAsync(userId);
        var monthlyAttendance = await GetMonthlyAttendanceAsync(userId, DateTime.UtcNow.Year, DateTime.UtcNow.Month);

        return userStats.CurrentStreak >= reward.StreakRequirement &&
               monthlyAttendance.SignInDays >= reward.AttendanceRequirement;
    }

    public async Task<ClaimRewardResponseDto> ClaimRewardAsync(int userId, int rewardId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (rewardId <= 0)
        {
            throw new ArgumentException("Reward ID must be greater than 0", nameof(rewardId));
        }

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

        // 更新用戶錢包
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);

        if (userWallet != null)
        {
            userWallet.Balance += reward.PointsReward;
            userWallet.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // 清除相關快取
        ClearUserRelatedCache(userId);

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

    #region 快取管理

    /// <summary>
    /// 清除用戶相關的快取
    /// </summary>
    private void ClearUserRelatedCache(int userId)
    {
        var todayStatusKey = string.Format(TodayStatusCacheKey, userId);
        var userStatisticsKey = string.Format(UserStatisticsCacheKey, userId);
        var userStreakKey = string.Format(UserStreakCacheKey, userId);

        _memoryCache.Remove(todayStatusKey);
        _memoryCache.Remove(userStatisticsKey);
        _memoryCache.Remove(userStreakKey);

        // 清除分頁歷史記錄快取（需要遍歷所有可能的頁面）
        for (int page = 1; page <= 10; page++) // 假設最多10頁
        {
            for (int pageSize = 10; pageSize <= MaxPageSize; pageSize += 10)
            {
                var historyKey = string.Format(UserHistoryCacheKey, userId, page, pageSize);
                _memoryCache.Remove(historyKey);
            }
        }

        // 清除月度出勤快取（當前年份的所有月份）
        var currentYear = DateTime.UtcNow.Year;
        for (int month = 1; month <= 12; month++)
        {
            var attendanceKey = string.Format(MonthlyAttendanceCacheKey, userId, currentYear, month);
            _memoryCache.Remove(attendanceKey);
        }

        _logger.LogDebug("Cleared cache for user {UserId}", userId);
    }

    /// <summary>
    /// 清除獎勵相關的快取
    /// </summary>
    private void ClearRewardRelatedCache()
    {
        _memoryCache.Remove(AvailableRewardsCacheKey);
        _logger.LogDebug("Cleared reward-related cache");
    }

    #endregion
}