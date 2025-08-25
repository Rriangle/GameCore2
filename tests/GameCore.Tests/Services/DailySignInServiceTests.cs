using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.DailySignInDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

public class DailySignInServiceTests
{
    private readonly GameCoreDbContext _context;
    private readonly DailySignInService _service;
    private readonly Mock<ILogger<DailySignInService>> _loggerMock;

    public DailySignInServiceTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _loggerMock = new Mock<ILogger<DailySignInService>>();
        _service = new DailySignInService(_context, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test user
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(user);

        // Create user wallet
        var wallet = new UserWallet
        {
            WalletId = 1,
            UserId = 1,
            Balance = 100.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        };
        _context.UserWallets.Add(wallet);

        // Create sign-in rewards
        var rewards = new List<SignInReward>
        {
            new SignInReward
            {
                Id = 1,
                Name = "新手簽到",
                Description = "連續簽到3天",
                PointsReward = 50,
                StreakRequirement = 3,
                AttendanceRequirement = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new SignInReward
            {
                Id = 2,
                Name = "週末戰士",
                Description = "連續簽到7天",
                PointsReward = 100,
                StreakRequirement = 7,
                AttendanceRequirement = 7,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            }
        };
        _context.SignInRewards.AddRange(rewards);

        _context.SaveChanges();
    }

    [Fact]
    public async Task SignInAsync_FirstTimeSignIn_ShouldCreateNewRecord()
    {
        // Act
        var result = await _service.SignInAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(DateTime.Today, result.SignInDate);
        Assert.Equal(1, result.CurrentStreak);
        Assert.Equal(1, result.LongestStreak);
        Assert.True(result.PointsEarned > 0);
        Assert.Contains("Welcome back!", result.Message);

        // Verify database record
        var dbRecord = await _context.DailySignIns.FirstOrDefaultAsync(ds => ds.UserId == 1);
        Assert.NotNull(dbRecord);
        Assert.Equal(1, dbRecord.CurrentStreak);
    }

    [Fact]
    public async Task SignInAsync_AlreadySignedInToday_ShouldReturnExistingRecord()
    {
        // Arrange - Create existing sign-in for today
        var existingSignIn = new DailySignIn
        {
            UserId = 1,
            SignInDate = DateTime.Today,
            SignInTime = DateTime.UtcNow.AddHours(-2),
            CurrentStreak = 5,
            LongestStreak = 10,
            MonthlyPerfectAttendance = 0,
            PointsEarned = 100,
            IsBonusDay = false,
            BonusMultiplier = 1,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            UpdatedAt = DateTime.UtcNow.AddHours(-2)
        };
        _context.DailySignIns.Add(existingSignIn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SignInAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("You have already signed in today!", result.Message);
        Assert.Equal(5, result.CurrentStreak);
        Assert.Equal(100, result.PointsEarned);
    }

    [Fact]
    public async Task SignInAsync_ConsecutiveDays_ShouldIncreaseStreak()
    {
        // Arrange - Create sign-in for yesterday
        var yesterdaySignIn = new DailySignIn
        {
            UserId = 1,
            SignInDate = DateTime.Today.AddDays(-1),
            SignInTime = DateTime.UtcNow.AddDays(-1),
            CurrentStreak = 3,
            LongestStreak = 5,
            MonthlyPerfectAttendance = 0,
            PointsEarned = 80,
            IsBonusDay = false,
            BonusMultiplier = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.DailySignIns.Add(yesterdaySignIn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SignInAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.CurrentStreak);
        Assert.Equal(5, result.LongestStreak);
    }

    [Fact]
    public async Task SignInAsync_BreakInStreak_ShouldResetStreak()
    {
        // Arrange - Create sign-in for 3 days ago
        var oldSignIn = new DailySignIn
        {
            UserId = 1,
            SignInDate = DateTime.Today.AddDays(-3),
            SignInTime = DateTime.UtcNow.AddDays(-3),
            CurrentStreak = 5,
            LongestStreak = 10,
            MonthlyPerfectAttendance = 0,
            PointsEarned = 100,
            IsBonusDay = false,
            BonusMultiplier = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-3)
        };
        _context.DailySignIns.Add(oldSignIn);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.SignInAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CurrentStreak);
        Assert.Equal(10, result.LongestStreak); // Longest streak should remain
    }

    [Fact]
    public async Task SignInAsync_BonusDay_ShouldApplyMultiplier()
    {
        // Arrange - Set today as Saturday (bonus day)
        var today = DateTime.Today;
        if (today.DayOfWeek != DayOfWeek.Saturday)
        {
            // Skip test if not Saturday
            return;
        }

        // Act
        var result = await _service.SignInAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsBonusDay);
        Assert.Equal(2, result.BonusMultiplier);
        Assert.True(result.PointsEarned > 20); // Should be higher due to bonus
    }

    [Fact]
    public async Task GetTodayStatusAsync_NoSignInToday_ShouldReturnFalse()
    {
        // Act
        var status = await _service.GetTodayStatusAsync(1);

        // Assert
        Assert.NotNull(status);
        Assert.False(status.HasSignedInToday);
        Assert.Equal(0, status.CurrentStreak);
        Assert.Equal(0, status.PointsEarnedToday);
    }

    [Fact]
    public async Task GetTodayStatusAsync_HasSignInToday_ShouldReturnTrue()
    {
        // Arrange - Create today's sign-in
        var todaySignIn = new DailySignIn
        {
            UserId = 1,
            SignInDate = DateTime.Today,
            SignInTime = DateTime.UtcNow.AddHours(-2),
            CurrentStreak = 3,
            LongestStreak = 5,
            MonthlyPerfectAttendance = 0,
            PointsEarned = 100,
            IsBonusDay = false,
            BonusMultiplier = 1,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            UpdatedAt = DateTime.UtcNow.AddHours(-2)
        };
        _context.DailySignIns.Add(todaySignIn);
        await _context.SaveChangesAsync();

        // Act
        var status = await _service.GetTodayStatusAsync(1);

        // Assert
        Assert.NotNull(status);
        Assert.True(status.HasSignedInToday);
        Assert.Equal(3, status.CurrentStreak);
        Assert.Equal(100, status.PointsEarnedToday);
    }

    [Fact]
    public async Task GetUserStatisticsAsync_NoSignIns_ShouldReturnEmptyStats()
    {
        // Act
        var stats = await _service.GetUserStatisticsAsync(1);

        // Assert
        Assert.NotNull(stats);
        Assert.Equal(0, stats.TotalSignIns);
        Assert.Equal(0, stats.CurrentStreak);
        Assert.Equal(0, stats.LongestStreak);
        Assert.Equal(0, stats.TotalPointsEarned);
    }

    [Fact]
    public async Task GetUserStatisticsAsync_WithSignIns_ShouldReturnCorrectStats()
    {
        // Arrange - Create multiple sign-ins
        var signIns = new List<DailySignIn>
        {
            new DailySignIn
            {
                UserId = 1,
                SignInDate = DateTime.Today.AddDays(-2),
                SignInTime = DateTime.UtcNow.AddDays(-2),
                CurrentStreak = 1,
                LongestStreak = 1,
                MonthlyPerfectAttendance = 0,
                PointsEarned = 50,
                IsBonusDay = false,
                BonusMultiplier = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new DailySignIn
            {
                UserId = 1,
                SignInDate = DateTime.Today.AddDays(-1),
                SignInTime = DateTime.UtcNow.AddDays(-1),
                CurrentStreak = 2,
                LongestStreak = 2,
                MonthlyPerfectAttendance = 0,
                PointsEarned = 60,
                IsBonusDay = false,
                BonusMultiplier = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };
        _context.DailySignIns.AddRange(signIns);
        await _context.SaveChangesAsync();

        // Act
        var stats = await _service.GetUserStatisticsAsync(1);

        // Assert
        Assert.NotNull(stats);
        Assert.Equal(2, stats.TotalSignIns);
        Assert.Equal(2, stats.CurrentStreak);
        Assert.Equal(2, stats.LongestStreak);
        Assert.Equal(110, stats.TotalPointsEarned);
        Assert.Equal(55, stats.AveragePointsPerSignIn);
    }

    [Fact]
    public async Task GetUserHistoryAsync_ShouldReturnPaginatedResults()
    {
        // Arrange - Create multiple history records
        var histories = new List<UserSignInHistory>();
        for (int i = 0; i < 25; i++)
        {
            histories.Add(new UserSignInHistory
            {
                UserId = 1,
                SignInDate = DateTime.Today.AddDays(-i),
                SignInTime = DateTime.UtcNow.AddDays(-i),
                DayOfWeek = (int)DateTime.Today.AddDays(-i).DayOfWeek,
                DayOfMonth = DateTime.Today.AddDays(-i).Day,
                Month = DateTime.Today.AddDays(-i).Month,
                Year = DateTime.Today.AddDays(-i).Year,
                WeekOfYear = 1,
                PointsEarned = 50 + i,
                IsStreakContinued = i > 0,
                IsBonusDay = false,
                BonusMultiplier = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        _context.UserSignInHistories.AddRange(histories);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserHistoryAsync(1, page: 2, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task GetMonthlyAttendanceAsync_ShouldReturnCorrectAttendance()
    {
        // Arrange - Create sign-ins for current month
        var currentMonth = DateTime.UtcNow;
        var signIns = new List<DailySignIn>();
        for (int day = 1; day <= 15; day++)
        {
            signIns.Add(new DailySignIn
            {
                UserId = 1,
                SignInDate = new DateTime(currentMonth.Year, currentMonth.Month, day),
                SignInTime = DateTime.UtcNow,
                CurrentStreak = day,
                LongestStreak = day,
                MonthlyPerfectAttendance = 0,
                PointsEarned = 50,
                IsBonusDay = false,
                BonusMultiplier = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        _context.DailySignIns.AddRange(signIns);
        await _context.SaveChangesAsync();

        // Act
        var attendance = await _service.GetMonthlyAttendanceAsync(1, currentMonth.Year, currentMonth.Month);

        // Assert
        Assert.NotNull(attendance);
        Assert.Equal(currentMonth.Year, attendance.Year);
        Assert.Equal(currentMonth.Month, attendance.Month);
        Assert.Equal(15, attendance.SignInDays);
        Assert.True(attendance.AttendanceRate > 0);
        Assert.Equal(15, attendance.DailyRecords.Count);
    }

    [Fact]
    public async Task GetAvailableRewardsAsync_ShouldReturnActiveRewards()
    {
        // Act
        var rewards = await _service.GetAvailableRewardsAsync();

        // Assert
        Assert.NotNull(rewards);
        Assert.Equal(2, rewards.Count);
        Assert.All(rewards, r => Assert.True(r.IsActive));
        Assert.Contains(rewards, r => r.Name == "新手簽到");
        Assert.Contains(rewards, r => r.Name == "週末戰士");
    }

    [Fact]
    public async Task CanClaimRewardAsync_MeetsRequirements_ShouldReturnTrue()
    {
        // Arrange - Create sign-ins to meet requirements
        var signIns = new List<DailySignIn>();
        for (int i = 0; i < 7; i++)
        {
            signIns.Add(new DailySignIn
            {
                UserId = 1,
                SignInDate = DateTime.Today.AddDays(-i),
                SignInTime = DateTime.UtcNow.AddDays(-i),
                CurrentStreak = i + 1,
                LongestStreak = i + 1,
                MonthlyPerfectAttendance = 0,
                PointsEarned = 50,
                IsBonusDay = false,
                BonusMultiplier = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        _context.DailySignIns.AddRange(signIns);
        await _context.SaveChangesAsync();

        // Act
        var canClaim = await _service.CanClaimRewardAsync(1, 2); // 週末戰士 reward

        // Assert
        Assert.True(canClaim);
    }

    [Fact]
    public async Task CanClaimRewardAsync_DoesNotMeetRequirements_ShouldReturnFalse()
    {
        // Act
        var canClaim = await _service.CanClaimRewardAsync(1, 2); // 週末戰士 reward

        // Assert
        Assert.False(canClaim);
    }

    [Fact]
    public async Task ClaimRewardAsync_ValidClaim_ShouldSucceed()
    {
        // Arrange - Create sign-ins to meet requirements
        var signIns = new List<DailySignIn>();
        for (int i = 0; i < 3; i++)
        {
            signIns.Add(new DailySignIn
            {
                UserId = 1,
                SignInDate = DateTime.Today.AddDays(-i),
                SignInTime = DateTime.UtcNow.AddDays(-i),
                CurrentStreak = i + 1,
                LongestStreak = i + 1,
                MonthlyPerfectAttendance = 0,
                PointsEarned = 50,
                IsBonusDay = false,
                BonusMultiplier = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        _context.DailySignIns.AddRange(signIns);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ClaimRewardAsync(1, 1); // 新手簽到 reward

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(1, result.RewardId);
        Assert.Equal("新手簽到", result.RewardName);
        Assert.Equal(50, result.PointsEarned);

        // Verify wallet was updated
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(uw => uw.UserId == 1);
        Assert.NotNull(wallet);
        Assert.Equal(150.00m, wallet.Balance); // 100 + 50
    }

    [Fact]
    public async Task ClaimRewardAsync_InvalidClaim_ShouldFail()
    {
        // Act
        var result = await _service.ClaimRewardAsync(1, 2); // 週末戰士 reward

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Contains("do not meet the requirements", result.Message);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}