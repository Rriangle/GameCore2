using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class SignInService : ISignInService
{
    private readonly GameCoreDbContext _context;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ILogger<SignInService> _logger;

    public SignInService(GameCoreDbContext context, IUserWalletRepository walletRepository, ILogger<SignInService> logger)
    {
        _context = context;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<SignInResult> SignInAsync(int userId)
    {
        try
        {
            // Check if user has already signed in today (Asia/Taipei timezone)
            var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");
            var taipeiNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, taipeiTimeZone);
            var today = taipeiNow.Date;

            var existingSignIn = await _context.UserSignInStats
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SignInDate == today);

            if (existingSignIn != null)
            {
                return new SignInResult
                {
                    Success = false,
                    Message = "您今天已經簽到了"
                };
            }

            // Get user's sign-in history to calculate streak
            var recentSignIns = await _context.UserSignInStats
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SignInDate)
                .Take(30)
                .ToListAsync();

            var currentStreak = CalculateCurrentStreak(recentSignIns, today);
            var newStreak = currentStreak + 1;

            // Calculate rewards based on business rules
            var (pointsEarned, expEarned) = CalculateRewards(today, newStreak);

            // Create new sign-in record
            var signInRecord = new UserSignInStats
            {
                UserId = userId,
                SignInDate = today,
                PointsEarned = pointsEarned,
                ExpEarned = expEarned,
                StreakCount = newStreak,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserSignInStats.Add(signInRecord);

            // Update user wallet
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet != null)
            {
                wallet.Balance += pointsEarned;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);
            }

            await _context.SaveChangesAsync();

            var isStreakBonus = newStreak == 7;
            var isMonthBonus = IsLastDayOfMonth(today) && newStreak >= 28;

            _logger.LogInformation("User {UserId} signed in successfully. Points: {Points}, Exp: {Exp}, Streak: {Streak}", 
                userId, pointsEarned, expEarned, newStreak);

            return new SignInResult
            {
                Success = true,
                Message = "簽到成功！",
                PointsEarned = pointsEarned,
                ExpEarned = expEarned,
                StreakCount = newStreak,
                IsStreakBonus = isStreakBonus,
                IsMonthBonus = isMonthBonus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign-in for user {UserId}", userId);
            return new SignInResult
            {
                Success = false,
                Message = "簽到失敗，請稍後再試"
            };
        }
    }

    public async Task<SignInStatsDto?> GetSignInStatsAsync(int userId)
    {
        try
        {
            var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");
            var taipeiNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, taipeiTimeZone);
            var today = taipeiNow.Date;

            var signIns = await _context.UserSignInStats
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SignInDate)
                .ToListAsync();

            if (!signIns.Any())
            {
                return new SignInStatsDto
                {
                    TotalSignIns = 0,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    LastSignInDate = null,
                    HasSignedInToday = false
                };
            }

            var currentStreak = CalculateCurrentStreak(signIns, today);
            var longestStreak = signIns.Max(s => s.StreakCount);
            var lastSignInDate = signIns.First().SignInDate;
            var hasSignedInToday = signIns.Any(s => s.SignInDate == today);

            return new SignInStatsDto
            {
                TotalSignIns = signIns.Count,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                LastSignInDate = lastSignInDate,
                HasSignedInToday = hasSignedInToday
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sign-in stats for user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> HasSignedInTodayAsync(int userId)
    {
        try
        {
            var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");
            var taipeiNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, taipeiTimeZone);
            var today = taipeiNow.Date;

            return await _context.UserSignInStats
                .AnyAsync(s => s.UserId == userId && s.SignInDate == today);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has signed in today", userId);
            return false;
        }
    }

    private int CalculateCurrentStreak(List<UserSignInStats> signIns, DateTime today)
    {
        if (!signIns.Any()) return 0;

        var currentStreak = 0;
        var currentDate = today;

        foreach (var signIn in signIns.OrderByDescending(s => s.SignInDate))
        {
            if (signIn.SignInDate == currentDate)
            {
                currentStreak++;
                currentDate = currentDate.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        return currentStreak;
    }

    private (int points, int exp) CalculateRewards(DateTime date, int streak)
    {
        var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        var isStreakBonus = streak == 7;
        var isMonthBonus = IsLastDayOfMonth(date) && streak >= 28;

        var basePoints = isWeekend ? 30 : 20;
        var baseExp = isWeekend ? 200 : 0;

        var streakBonus = isStreakBonus ? 40 : 0;
        var monthBonus = isMonthBonus ? 200 : 0;
        var monthExpBonus = isMonthBonus ? 2000 : 0;

        var totalPoints = basePoints + streakBonus + monthBonus;
        var totalExp = baseExp + monthExpBonus;

        return (totalPoints, totalExp);
    }

    private bool IsLastDayOfMonth(DateTime date)
    {
        return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
    }
}