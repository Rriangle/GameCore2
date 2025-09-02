using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;

namespace GameCore.Api.Services;

/// <summary>
/// 每日簽到服務
/// </summary>
public class SignInService : ISignInService
{
    private readonly IUserSignInStatsRepository _signInStatsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IPetRepository _petRepository;

    public SignInService(
        IUserSignInStatsRepository signInStatsRepository,
        IUserWalletRepository userWalletRepository,
        IPetRepository petRepository)
    {
        _signInStatsRepository = signInStatsRepository;
        _userWalletRepository = userWalletRepository;
        _petRepository = petRepository;
    }

    /// <summary>
    /// 獲取簽到狀態
    /// </summary>
    public async Task<ServiceResult<SignInStatusDto>> GetSignInStatusAsync(int userId)
    {
        try
        {
            // 獲取今日日期（台灣時間）
            var today = DateTime.UtcNow.AddHours(8).Date;
            
            // 檢查今日是否已簽到
            var todaySignIn = await _signInStatsRepository.GetByUserIdAndDateAsync(userId, today);
            var todaySigned = todaySignIn != null;

            // 計算連續天數
            var currentStreak = await CalculateCurrentStreakAsync(userId);

            // 計算本月簽到天數 - 優化：使用資料庫層面計數而非記憶體計數
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            // 效能優化：直接從資料庫取得計數，避免載入完整資料到記憶體
            var monthAttendance = await _signInStatsRepository.GetCountByUserIdAndDateRangeAsync(userId, monthStart, monthEnd);
            var monthTotalDays = DateTime.DaysInMonth(today.Year, today.Month);
            var monthAttendanceRate = monthTotalDays > 0 ? (decimal)monthAttendance / monthTotalDays : 0;

            // 獲取最後簽到日期
            var lastSignIn = await _signInStatsRepository.GetLastSignInAsync(userId);
            var lastSignInDate = lastSignIn?.SignTime.Date;

            var status = new SignInStatusDto
            {
                TodaySigned = todaySigned,
                CurrentStreak = currentStreak,
                MonthAttendance = monthAttendance,
                MonthTotalDays = monthTotalDays,
                MonthAttendanceRate = monthAttendanceRate,
                LastSignInDate = lastSignInDate
            };

            return ServiceResult<SignInStatusDto>.CreateSuccess(status);
        }
        catch (Exception ex)
        {
            return ServiceResult<SignInStatusDto>.CreateFailure($"獲取簽到狀態失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 執行每日簽到
    /// </summary>
    public async Task<ServiceResult<SignInResultDto>> SignInAsync(int userId)
    {
        try
        {
            // 獲取今日日期（台灣時間）
            var today = DateTime.UtcNow.AddHours(8).Date;
            
            // 檢查今日是否已簽到
            var todaySignIn = await _signInStatsRepository.GetByUserIdAndDateAsync(userId, today);
            if (todaySignIn != null)
            {
                return ServiceResult<SignInResultDto>.CreateFailure("今日已簽到，請明天再來");
            }

            // 計算獎勵
            var (pointsGained, expGained, gotStreakBonus, gotFullMonthBonus) = await CalculateRewardsAsync(userId, today);

            // 創建簽到記錄
            var signInRecord = new UserSignInStats
            {
                UserID = userId,
                SignTime = DateTime.UtcNow,
                PointsChanged = pointsGained,
                ExpGained = expGained,
                PointsChangedTime = DateTime.UtcNow,
                ExpGainedTime = DateTime.UtcNow
            };

            await _signInStatsRepository.AddAsync(signInRecord);

            // 更新用戶錢包
            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (userWallet != null)
            {
                userWallet.User_Point += pointsGained;
                userWallet.Last_Updated = DateTime.UtcNow;
                await _userWalletRepository.UpdateAsync(userWallet);
            }

            // 更新寵物經驗值
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet != null && expGained > 0)
            {
                pet.Experience += expGained;
                await _petRepository.UpdateAsync(pet);
            }

            // 計算簽到後連續天數
            var streakAfter = await CalculateCurrentStreakAsync(userId);

            var result = new SignInResultDto
            {
                Success = true,
                PointsGained = pointsGained,
                ExpGained = expGained,
                StreakAfter = streakAfter,
                GotStreakBonus = gotStreakBonus,
                GotFullMonthBonus = gotFullMonthBonus,
                SignInTime = DateTime.UtcNow
            };

            return ServiceResult<SignInResultDto>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<SignInResultDto>.CreateFailure($"簽到失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 獲取簽到歷史
    /// </summary>
    public async Task<ServiceResult<PagedResult<SignInHistoryDto>>> GetSignInHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var signIns = await _signInStatsRepository.GetByUserIdAsync(userId, page, pageSize);
            var totalCount = await _signInStatsRepository.GetCountByUserIdAsync(userId);

            var historyList = signIns.Select(s => new SignInHistoryDto
            {
                LogId = s.LogID,
                SignTime = s.SignTime,
                PointsChanged = s.PointsChanged,
                ExpGained = s.ExpGained,
                IsStreakBonus = s.PointsChanged > 50, // 簡單判斷是否為連續獎勵
                IsFullMonthBonus = s.PointsChanged > 200 // 簡單判斷是否為全勤獎勵
            }).ToList();

            var pagedResult = new PagedResult<SignInHistoryDto>
            {
                Data = historyList,
                Total = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return ServiceResult<PagedResult<SignInHistoryDto>>.CreateSuccess(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<SignInHistoryDto>>.CreateFailure($"獲取簽到歷史失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 計算當前連續天數
    /// </summary>
    private async Task<int> CalculateCurrentStreakAsync(int userId)
    {
        var today = DateTime.UtcNow.AddHours(8).Date;
        var streak = 0;
        var currentDate = today;

        while (true)
        {
            var signIn = await _signInStatsRepository.GetByUserIdAndDateAsync(userId, currentDate);
            if (signIn == null)
            {
                break;
            }
            streak++;
            currentDate = currentDate.AddDays(-1);
        }

        return streak;
    }

    /// <summary>
    /// 計算簽到獎勵
    /// </summary>
    private async Task<(int points, int exp, bool streakBonus, bool fullMonthBonus)> CalculateRewardsAsync(int userId, DateTime today)
    {
        var basePoints = 20; // 平日基礎點數
        var baseExp = 0; // 平日基礎經驗值

        // 判斷是否為假日（週六、週日）
        var isWeekend = today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday;
        if (isWeekend)
        {
            basePoints = 30; // 假日基礎點數
            baseExp = 200; // 假日基礎經驗值
        }

        var totalPoints = basePoints;
        var totalExp = baseExp;
        var gotStreakBonus = false;
        var gotFullMonthBonus = false;

        // 檢查連續7天獎勵
        var currentStreak = await CalculateCurrentStreakAsync(userId);
        if (currentStreak == 6) // 今天是第7天
        {
            totalPoints += 40;
            totalExp += 300;
            gotStreakBonus = true;
        }

        // 檢查當月全勤獎勵
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var monthSignIns = await _signInStatsRepository.GetByUserIdAndDateRangeAsync(userId, monthStart, monthEnd);
        var monthAttendance = monthSignIns.Count();

        // 如果今天是月底最後一天且之前每天都簽到
        if (today == monthEnd && monthAttendance == monthEnd.Day - 1)
        {
            totalPoints += 200;
            totalExp += 2000;
            gotFullMonthBonus = true;
        }

        return (totalPoints, totalExp, gotStreakBonus, gotFullMonthBonus);
    }
} 