using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.DailySignInDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// DailySignInService 優化版本單元測試
/// </summary>
public class DailySignInServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DailySignInService> _logger;
    private readonly DailySignInService _service;

    public DailySignInServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<DailySignInService>>();
        _service = new DailySignInService(_context, _memoryCache, _logger);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
        _memoryCache.Dispose();
    }

    #region 測試數據準備

    private void SeedTestData()
    {
        // 創建用戶
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com" };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com" };
        var user3 = new User { user_id = 3, username = "user3", email = "user3@test.com" };
        _context.Users.AddRange(user1, user2, user3);

        // 創建用戶錢包
        var wallet1 = new UserWallet { UserId = 1, Balance = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var wallet2 = new UserWallet { UserId = 2, Balance = 50, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var wallet3 = new UserWallet { UserId = 3, Balance = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _context.UserWallets.AddRange(wallet1, wallet2, wallet3);

        // 創建每日簽到記錄
        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);
        var twoDaysAgo = today.AddDays(-2);

        var signIn1 = new DailySignIn 
        { 
            Id = 1, 
            UserId = 1, 
            SignInDate = yesterday, 
            SignInTime = yesterday.AddHours(9), 
            CurrentStreak = 3, 
            LongestStreak = 5, 
            MonthlyPerfectAttendance = 0, 
            PointsEarned = 30, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        var signIn2 = new DailySignIn 
        { 
            Id = 2, 
            UserId = 2, 
            SignInDate = yesterday, 
            SignInTime = yesterday.AddHours(10), 
            CurrentStreak = 1, 
            LongestStreak = 1, 
            MonthlyPerfectAttendance = 0, 
            PointsEarned = 10, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        var signIn3 = new DailySignIn 
        { 
            Id = 3, 
            UserId = 1, 
            SignInDate = twoDaysAgo, 
            SignInTime = twoDaysAgo.AddHours(8), 
            CurrentStreak = 2, 
            LongestStreak = 5, 
            MonthlyPerfectAttendance = 0, 
            PointsEarned = 20, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        _context.DailySignIns.AddRange(signIn1, signIn2, signIn3);

        // 創建簽到歷史記錄
        var history1 = new UserSignInHistory 
        { 
            Id = 1, 
            UserId = 1, 
            SignInDate = yesterday, 
            SignInTime = yesterday.AddHours(9), 
            DayOfWeek = (int)yesterday.DayOfWeek, 
            DayOfMonth = yesterday.Day, 
            Month = yesterday.Month, 
            Year = yesterday.Year, 
            WeekOfYear = GetWeekOfYear(yesterday), 
            PointsEarned = 30, 
            IsStreakContinued = true, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        var history2 = new UserSignInHistory 
        { 
            Id = 2, 
            UserId = 2, 
            SignInDate = yesterday, 
            SignInTime = yesterday.AddHours(10), 
            DayOfWeek = (int)yesterday.DayOfWeek, 
            DayOfMonth = yesterday.Day, 
            Month = yesterday.Month, 
            Year = yesterday.Year, 
            WeekOfYear = GetWeekOfYear(yesterday), 
            PointsEarned = 10, 
            IsStreakContinued = false, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        _context.UserSignInHistories.AddRange(history1, history2);

        // 創建簽到獎勵
        var reward1 = new SignInReward 
        { 
            Id = 1, 
            Name = "Week Warrior", 
            Description = "7 day streak", 
            PointsReward = 100, 
            StreakRequirement = 7, 
            AttendanceRequirement = 7, 
            IsActive = true 
        };
        var reward2 = new SignInReward 
        { 
            Id = 2, 
            Name = "Monthly Master", 
            Description = "30 day streak", 
            PointsReward = 500, 
            StreakRequirement = 30, 
            AttendanceRequirement = 30, 
            IsActive = true 
        };
        var reward3 = new SignInReward 
        { 
            Id = 3, 
            Name = "Perfect Month", 
            Description = "Perfect attendance in a month", 
            PointsReward = 200, 
            StreakRequirement = 0, 
            AttendanceRequirement = 28, 
            IsActive = true 
        };
        _context.SignInRewards.AddRange(reward1, reward2, reward3);

        _context.SaveChanges();
    }

    private int GetWeekOfYear(DateTime date)
    {
        var calendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
        return calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    #endregion

    #region SignInAsync 測試

    [Fact]
    public async Task SignInAsync_WithValidUserId_ShouldCreateSignIn()
    {
        // Arrange
        var userId = 3; // User with no previous sign-ins

        // Act
        var result = await _service.SignInAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(DateTime.Today, result.SignInDate);
        Assert.Equal(1, result.CurrentStreak); // First sign-in
        Assert.Equal(1, result.LongestStreak);
        Assert.True(result.PointsEarned > 0);
        Assert.False(result.IsBonusDay);
        Assert.Equal(1, result.BonusMultiplier);
        Assert.Contains("Welcome back", result.Message);

        // 驗證資料庫中確實創建了記錄
        var dbSignIn = await _context.DailySignIns
            .FirstOrDefaultAsync(ds => ds.Id == result.Id);
        Assert.NotNull(dbSignIn);

        // 驗證歷史記錄
        var dbHistory = await _context.UserSignInHistories
            .FirstOrDefaultAsync(ush => ush.UserId == userId && ush.SignInDate == DateTime.Today);
        Assert.NotNull(dbHistory);

        // 驗證錢包更新
        var dbWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        Assert.NotNull(dbWallet);
        Assert.True(dbWallet.Balance > 0);
    }

    [Fact]
    public async Task SignInAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SignInAsync(userId));
    }

    [Fact]
    public async Task SignInAsync_WithZeroUserId_ShouldThrowException()
    {
        // Arrange
        var userId = 0;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SignInAsync(userId));
    }

    [Fact]
    public async Task SignInAsync_WithAlreadySignedInUser_ShouldReturnExistingSignIn()
    {
        // Arrange
        var userId = 1; // User who signed in yesterday

        // Act
        var result = await _service.SignInAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("You have already signed in today!", result.Message);
        Assert.Equal(0, result.PointsEarned); // No new points
    }

    [Fact]
    public async Task SignInAsync_WithStreakContinuation_ShouldIncreaseStreak()
    {
        // Arrange
        var userId = 1; // User with 3-day streak
        var today = DateTime.Today;
        
        // 創建今天的簽到記錄（模擬連續簽到）
        var todaySignIn = new DailySignIn 
        { 
            Id = 4, 
            UserId = userId, 
            SignInDate = today, 
            SignInTime = today.AddHours(9), 
            CurrentStreak = 4, 
            LongestStreak = 5, 
            MonthlyPerfectAttendance = 0, 
            PointsEarned = 40, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        _context.DailySignIns.Add(todaySignIn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SignInAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("You have already signed in today!", result.Message);
        Assert.Equal(4, result.CurrentStreak);
    }

    [Fact]
    public async Task SignInAsync_WithBonusDay_ShouldApplyBonusMultiplier()
    {
        // Arrange
        var userId = 3;
        var today = DateTime.Today;
        
        // 如果是週末或月初，測試獎勵日
        if (today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday || today.Day == 1)
        {
            // Act
            var result = await _service.SignInAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsBonusDay);
            Assert.Equal(2, result.BonusMultiplier);
            Assert.True(result.PointsEarned > 20); // Should be more than base points
        }
    }

    #endregion

    #region GetTodayStatusAsync 測試

    [Fact]
    public async Task GetTodayStatusAsync_WithValidUserId_ShouldReturnStatus()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetTodayStatusAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasSignedInToday); // User hasn't signed in today
        Assert.NotNull(result.LastSignInTime);
        Assert.Equal(3, result.CurrentStreak);
        Assert.Equal(5, result.LongestStreak);
        Assert.Equal(0, result.MonthlyPerfectAttendance);
        Assert.Equal(0, result.PointsEarnedToday);
    }

    [Fact]
    public async Task GetTodayStatusAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetTodayStatusAsync(userId));
    }

    [Fact]
    public async Task GetTodayStatusAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetTodayStatusAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.DailySignIns.RemoveRange(_context.DailySignIns);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetTodayStatusAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.HasSignedInToday, result2.HasSignedInToday);
        Assert.Equal(result1.CurrentStreak, result2.CurrentStreak);
    }

    [Fact]
    public async Task GetTodayStatusAsync_WithSignedInToday_ShouldReturnCorrectStatus()
    {
        // Arrange
        var userId = 1;
        var today = DateTime.Today;
        
        // 創建今天的簽到記錄
        var todaySignIn = new DailySignIn 
        { 
            Id = 4, 
            UserId = userId, 
            SignInDate = today, 
            SignInTime = today.AddHours(9), 
            CurrentStreak = 4, 
            LongestStreak = 5, 
            MonthlyPerfectAttendance = 0, 
            PointsEarned = 40, 
            IsBonusDay = false, 
            BonusMultiplier = 1 
        };
        _context.DailySignIns.Add(todaySignIn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTodayStatusAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasSignedInToday);
        Assert.Equal(40, result.PointsEarnedToday);
        Assert.True(result.TimeUntilNextSignIn > TimeSpan.Zero);
    }

    #endregion

    #region GetUserStatisticsAsync 測試

    [Fact]
    public async Task GetUserStatisticsAsync_WithValidUserId_ShouldReturnStatistics()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetUserStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalSignIns);
        Assert.Equal(3, result.CurrentStreak);
        Assert.Equal(5, result.LongestStreak);
        Assert.Equal(0, result.MonthlyPerfectAttendance);
        Assert.Equal(50, result.TotalPointsEarned); // 30 + 20
        Assert.Equal(25, result.AveragePointsPerSignIn);
        Assert.Equal(0, result.BonusDaysCount);
        Assert.True(result.FirstSignInDate > DateTime.MinValue);
        Assert.True(result.LastSignInDate > DateTime.MinValue);
    }

    [Fact]
    public async Task GetUserStatisticsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserStatisticsAsync(userId));
    }

    [Fact]
    public async Task GetUserStatisticsAsync_WithNoSignIns_ShouldReturnEmptyStatistics()
    {
        // Arrange
        var userId = 999; // User with no sign-ins

        // Act
        var result = await _service.GetUserStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalSignIns);
        Assert.Equal(0, result.CurrentStreak);
        Assert.Equal(0, result.LongestStreak);
        Assert.Equal(0, result.MonthlyPerfectAttendance);
        Assert.Equal(0, result.TotalPointsEarned);
        Assert.Equal(0, result.AveragePointsPerSignIn);
        Assert.Equal(0, result.BonusDaysCount);
        Assert.Equal(DateTime.MinValue, result.FirstSignInDate);
        Assert.Equal(DateTime.MinValue, result.LastSignInDate);
    }

    [Fact]
    public async Task GetUserStatisticsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetUserStatisticsAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.DailySignIns.RemoveRange(_context.DailySignIns);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalSignIns, result2.TotalSignIns);
        Assert.Equal(result1.TotalPointsEarned, result2.TotalPointsEarned);
    }

    #endregion

    #region GetUserHistoryAsync 測試

    [Fact]
    public async Task GetUserHistoryAsync_WithValidParameters_ShouldReturnHistory()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await _service.GetUserHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(30, result.Items[0].PointsEarned);
        Assert.True(result.Items[0].IsStreakContinued);
    }

    [Fact]
    public async Task GetUserHistoryAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var page = 1;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserHistoryAsync(userId, page, pageSize));
    }

    [Fact]
    public async Task GetUserHistoryAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var page = 0;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserHistoryAsync(userId, page, pageSize));
    }

    [Fact]
    public async Task GetUserHistoryAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetUserHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.PageSize); // Default page size
    }

    [Fact]
    public async Task GetUserHistoryAsync_WithExcessivePageSize_ShouldUseMaxPageSize()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 1000;

        // Act
        var result = await _service.GetUserHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.PageSize); // Max page size
    }

    [Fact]
    public async Task GetUserHistoryAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        // Act - 第一次調用
        var result1 = await _service.GetUserHistoryAsync(userId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.UserSignInHistories.RemoveRange(_context.UserSignInHistories);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalCount, result2.TotalCount);
        Assert.Equal(result1.Items.Count, result2.Items.Count);
    }

    #endregion

    #region GetMonthlyAttendanceAsync 測試

    [Fact]
    public async Task GetMonthlyAttendanceAsync_WithValidParameters_ShouldReturnAttendance()
    {
        // Arrange
        var userId = 1;
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month;

        // Act
        var result = await _service.GetMonthlyAttendanceAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(year, result.Year);
        Assert.Equal(month, result.Month);
        Assert.True(result.TotalDays > 0);
        Assert.True(result.SignInDays >= 0);
        Assert.True(result.MissedDays >= 0);
        Assert.True(result.AttendanceRate >= 0);
        Assert.NotNull(result.DailyRecords);
        Assert.Equal(result.TotalDays, result.DailyRecords.Count);
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var year = 2024;
        var month = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetMonthlyAttendanceAsync(userId, year, month));
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_WithInvalidYear_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var year = 2019; // Too old
        var month = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetMonthlyAttendanceAsync(userId, year, month));
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_WithInvalidMonth_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 13; // Invalid month

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetMonthlyAttendanceAsync(userId, year, month));
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 1;

        // Act - 第一次調用
        var result1 = await _service.GetMonthlyAttendanceAsync(userId, year, month);
        
        // 清除資料庫數據（模擬快取生效）
        _context.DailySignIns.RemoveRange(_context.DailySignIns);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetMonthlyAttendanceAsync(userId, year, month);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalDays, result2.TotalDays);
        Assert.Equal(result1.SignInDays, result2.SignInDays);
    }

    #endregion

    #region GetAvailableRewardsAsync 測試

    [Fact]
    public async Task GetAvailableRewardsAsync_ShouldReturnRewards()
    {
        // Act
        var result = await _service.GetAvailableRewardsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, r => Assert.True(r.IsActive));
        Assert.All(result, r => Assert.True(r.PointsReward > 0));
        Assert.All(result, r => Assert.False(r.CanClaim)); // Default value
    }

    [Fact]
    public async Task GetAvailableRewardsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAvailableRewardsAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.SignInRewards.RemoveRange(_context.SignInRewards);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAvailableRewardsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
    }

    #endregion

    #region CanClaimRewardAsync 測試

    [Fact]
    public async Task CanClaimRewardAsync_WithValidParameters_ShouldReturnResult()
    {
        // Arrange
        var userId = 1;
        var rewardId = 1; // Week Warrior (7 day streak required)

        // Act
        var result = await _service.CanClaimRewardAsync(userId, rewardId);

        // Assert
        Assert.False(result); // User only has 3-day streak, needs 7
    }

    [Fact]
    public async Task CanClaimRewardAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var rewardId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CanClaimRewardAsync(userId, rewardId));
    }

    [Fact]
    public async Task CanClaimRewardAsync_WithInvalidRewardId_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var rewardId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CanClaimRewardAsync(userId, rewardId));
    }

    [Fact]
    public async Task CanClaimRewardAsync_WithNonExistentReward_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var rewardId = 999;

        // Act
        var result = await _service.CanClaimRewardAsync(userId, rewardId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ClaimRewardAsync 測試

    [Fact]
    public async Task ClaimRewardAsync_WithValidParameters_ShouldClaimReward()
    {
        // Arrange
        var userId = 1;
        var rewardId = 3; // Perfect Month (0 streak, 28 attendance required)
        
        // 創建足夠的出勤記錄來滿足獎勵要求
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        for (int i = 0; i < 28; i++)
        {
            var signIn = new DailySignIn 
            { 
                Id = 100 + i, 
                UserId = userId, 
                SignInDate = startOfMonth.AddDays(i), 
                SignInTime = startOfMonth.AddDays(i).AddHours(9), 
                CurrentStreak = 1, 
                LongestStreak = 5, 
                MonthlyPerfectAttendance = 28, 
                PointsEarned = 10, 
                IsBonusDay = false, 
                BonusMultiplier = 1 
            };
            _context.DailySignIns.Add(signIn);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ClaimRewardAsync(userId, rewardId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(rewardId, result.RewardId);
        Assert.Equal("Perfect Month", result.RewardName);
        Assert.Equal(200, result.PointsEarned);
        Assert.Contains("Congratulations", result.Message);

        // 驗證錢包更新
        var dbWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        Assert.NotNull(dbWallet);
        Assert.True(dbWallet.Balance > 100); // Original balance was 100
    }

    [Fact]
    public async Task ClaimRewardAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var rewardId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ClaimRewardAsync(userId, rewardId));
    }

    [Fact]
    public async Task ClaimRewardAsync_WithInvalidRewardId_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var rewardId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ClaimRewardAsync(userId, rewardId));
    }

    [Fact]
    public async Task ClaimRewardAsync_WithUnmetRequirements_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1;
        var rewardId = 1; // Week Warrior (7 day streak required)

        // Act
        var result = await _service.ClaimRewardAsync(userId, rewardId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("You do not meet the requirements for this reward.", result.Message);
        Assert.Equal(0, result.PointsEarned);
    }

    [Fact]
    public async Task ClaimRewardAsync_WithNonExistentReward_ShouldReturnFailure()
    {
        // Arrange
        var userId = 1;
        var rewardId = 999;

        // Act
        var result = await _service.ClaimRewardAsync(userId, rewardId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("You do not meet the requirements for this reward.", result.Message);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task SignInAsync_WithUserHavingNoWallet_ShouldCreateWallet()
    {
        // Arrange
        var userId = 999; // User with no wallet
        var user = new User { user_id = userId, username = "user999", email = "user999@test.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SignInAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);

        // 驗證錢包被創建
        var dbWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        Assert.NotNull(dbWallet);
        Assert.True(dbWallet.Balance > 0);
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_WithNoSignInsInMonth_ShouldReturnZeroAttendance()
    {
        // Arrange
        var userId = 999; // User with no sign-ins
        var year = 2024;
        var month = 1;

        // Act
        var result = await _service.GetMonthlyAttendanceAsync(userId, year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.SignInDays);
        Assert.Equal(result.TotalDays, result.MissedDays);
        Assert.Equal(0, result.AttendanceRate);
        Assert.Equal(0, result.PerfectAttendanceDays);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetTodayStatusAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetTodayStatusAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUserStatisticsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserStatisticsAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUserHistoryAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserHistoryAsync(userId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var year = 2024;
        var month = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetMonthlyAttendanceAsync(userId, year, month);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}