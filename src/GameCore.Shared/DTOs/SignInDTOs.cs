namespace GameCore.Shared.DTOs;

/// <summary>
/// 簽到狀態查詢 DTO
/// </summary>
public class SignInStatusDto
{
    /// <summary>
    /// 今日是否已簽到
    /// </summary>
    public bool TodaySigned { get; set; }

    /// <summary>
    /// 當前連續簽到天數
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// 本月簽到天數
    /// </summary>
    public int MonthAttendance { get; set; }

    /// <summary>
    /// 本月總天數
    /// </summary>
    public int MonthTotalDays { get; set; }

    /// <summary>
    /// 是否達成本月全勤
    /// </summary>
    public bool IsMonthlyPerfect { get; set; }

    /// <summary>
    /// 下次簽到可獲得的點數
    /// </summary>
    public int NextSignInPoints { get; set; }

    /// <summary>
    /// 下次簽到可獲得的經驗值
    /// </summary>
    public int NextSignInExp { get; set; }

    /// <summary>
    /// 是否週末 (可獲得額外獎勵)
    /// </summary>
    public bool IsWeekend { get; set; }

    /// <summary>
    /// 明日是否為連續第7天
    /// </summary>
    public bool NextIs7thDay { get; set; }

    /// <summary>
    /// 今日簽到記錄
    /// </summary>
    public SignInRecordDto? TodayRecord { get; set; }

    /// <summary>
    /// 本月簽到日曆
    /// </summary>
    public List<SignInCalendarDayDto> MonthCalendar { get; set; } = new();

    /// <summary>
    /// 最近簽到記錄
    /// </summary>
    public List<SignInRecordDto> RecentRecords { get; set; } = new();
}

/// <summary>
/// 簽到請求 DTO
/// </summary>
public class SignInRequestDto
{
    /// <summary>
    /// 簽到來源 (web/mobile)
    /// </summary>
    public string Source { get; set; } = "web";

    /// <summary>
    /// 時區偏移 (分鐘，用於計算當地時間)
    /// </summary>
    public int TimezoneOffset { get; set; } = 480; // 台北時間 UTC+8

    /// <summary>
    /// 客戶端IP (可選)
    /// </summary>
    public string? ClientIp { get; set; }

    /// <summary>
    /// 用戶代理 (可選)
    /// </summary>
    public string? UserAgent { get; set; }
}

/// <summary>
/// 簽到結果 DTO
/// </summary>
public class SignInResultDto
{
    /// <summary>
    /// 是否簽到成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息 (如已簽到)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 獲得的點數
    /// </summary>
    public int PointsDelta { get; set; }

    /// <summary>
    /// 獲得的經驗值
    /// </summary>
    public int ExpDelta { get; set; }

    /// <summary>
    /// 簽到後的連續天數
    /// </summary>
    public int StreakAfter { get; set; }

    /// <summary>
    /// 是否獲得連續7天獎勵
    /// </summary>
    public bool Got7DayBonus { get; set; }

    /// <summary>
    /// 是否獲得全月獎勵
    /// </summary>
    public bool GotMonthlyBonus { get; set; }

    /// <summary>
    /// 獎勵說明
    /// </summary>
    public List<string> RewardDescriptions { get; set; } = new();

    /// <summary>
    /// 簽到記錄
    /// </summary>
    public SignInRecordDto? Record { get; set; }

    /// <summary>
    /// 更新後的會員點數餘額
    /// </summary>
    public int UpdatedBalance { get; set; }
}

/// <summary>
/// 簽到記錄 DTO
/// </summary>
public class SignInRecordDto
{
    /// <summary>
    /// 記錄編號
    /// </summary>
    public int LogID { get; set; }

    /// <summary>
    /// 簽到時間
    /// </summary>
    public DateTime SignTime { get; set; }

    /// <summary>
    /// 簽到日期 (台北時間)
    /// </summary>
    public DateOnly SignInDate { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 獲得點數
    /// </summary>
    public int PointsChanged { get; set; }

    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int ExpGained { get; set; }

    /// <summary>
    /// 簽到類型
    /// </summary>
    public string SignInType { get; set; } = string.Empty;

    /// <summary>
    /// 連續天數
    /// </summary>
    public int StreakDays { get; set; }

    /// <summary>
    /// 是否為額外獎勵
    /// </summary>
    public bool IsBonusReward { get; set; }

    /// <summary>
    /// 額外獎勵類型
    /// </summary>
    public string? BonusType { get; set; }

    /// <summary>
    /// 獎勵說明
    /// </summary>
    public string? RewardDescription { get; set; }

    /// <summary>
    /// 簽到來源
    /// </summary>
    public string SignInSource { get; set; } = string.Empty;
}

/// <summary>
/// 簽到日曆日期 DTO
/// </summary>
public class SignInCalendarDayDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 日期號碼
    /// </summary>
    public int Day { get; set; }

    /// <summary>
    /// 是否已簽到
    /// </summary>
    public bool HasSignedIn { get; set; }

    /// <summary>
    /// 是否為今天
    /// </summary>
    public bool IsToday { get; set; }

    /// <summary>
    /// 是否為週末
    /// </summary>
    public bool IsWeekend { get; set; }

    /// <summary>
    /// 是否為未來日期
    /// </summary>
    public bool IsFuture { get; set; }

    /// <summary>
    /// 簽到記錄 (如果有)
    /// </summary>
    public SignInRecordDto? Record { get; set; }

    /// <summary>
    /// 獲得的點數 (如果已簽到)
    /// </summary>
    public int? PointsEarned { get; set; }

    /// <summary>
    /// 獲得的經驗值 (如果已簽到)
    /// </summary>
    public int? ExpEarned { get; set; }

    /// <summary>
    /// 是否有額外獎勵
    /// </summary>
    public bool HasBonus { get; set; }

    /// <summary>
    /// 額外獎勵類型
    /// </summary>
    public string? BonusType { get; set; }
}

/// <summary>
/// 簽到統計 DTO
/// </summary>
public class SignInStatisticsDto
{
    /// <summary>
    /// 總簽到天數
    /// </summary>
    public int TotalSignInDays { get; set; }

    /// <summary>
    /// 最長連續簽到天數
    /// </summary>
    public int MaxStreak { get; set; }

    /// <summary>
    /// 當前連續簽到天數
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// 累計獲得點數
    /// </summary>
    public int TotalPointsEarned { get; set; }

    /// <summary>
    /// 累計獲得經驗值
    /// </summary>
    public int TotalExpEarned { get; set; }

    /// <summary>
    /// 連續7天獎勵次數
    /// </summary>
    public int Streak7Count { get; set; }

    /// <summary>
    /// 全月獎勵次數
    /// </summary>
    public int MonthlyBonusCount { get; set; }

    /// <summary>
    /// 首次簽到時間
    /// </summary>
    public DateTime? FirstSignInTime { get; set; }

    /// <summary>
    /// 最後簽到時間
    /// </summary>
    public DateTime? LastSignInTime { get; set; }

    /// <summary>
    /// 本月簽到天數
    /// </summary>
    public int ThisMonthDays { get; set; }

    /// <summary>
    /// 本年簽到天數
    /// </summary>
    public int ThisYearDays { get; set; }
}

/// <summary>
/// 簽到查詢參數 DTO
/// </summary>
public class SignInQueryDto
{
    /// <summary>
    /// 使用者編號 (管理員查詢用)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// 簽到類型篩選
    /// </summary>
    public string? SignInType { get; set; }

    /// <summary>
    /// 是否只顯示額外獎勵
    /// </summary>
    public bool? OnlyBonusRewards { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 分頁簽到記錄 DTO
/// </summary>
public class PagedSignInRecordsDto
{
    /// <summary>
    /// 記錄列表
    /// </summary>
    public List<SignInRecordDto> Records { get; set; } = new();

    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;
}

/// <summary>
/// 簽到獎勵配置 DTO (管理員用)
/// </summary>
public class SignInRewardConfigDto
{
    /// <summary>
    /// 平日獎勵點數
    /// </summary>
    public int WeekdayPoints { get; set; } = 20;

    /// <summary>
    /// 平日獎勵經驗值
    /// </summary>
    public int WeekdayExp { get; set; } = 0;

    /// <summary>
    /// 週末獎勵點數
    /// </summary>
    public int WeekendPoints { get; set; } = 30;

    /// <summary>
    /// 週末獎勵經驗值
    /// </summary>
    public int WeekendExp { get; set; } = 200;

    /// <summary>
    /// 連續7天額外點數
    /// </summary>
    public int Streak7Points { get; set; } = 40;

    /// <summary>
    /// 連續7天額外經驗值
    /// </summary>
    public int Streak7Exp { get; set; } = 300;

    /// <summary>
    /// 全月獎勵點數
    /// </summary>
    public int MonthlyPoints { get; set; } = 200;

    /// <summary>
    /// 全月獎勵經驗值
    /// </summary>
    public int MonthlyExp { get; set; } = 2000;

    /// <summary>
    /// 是否啟用系統
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 是否允許補簽
    /// </summary>
    public bool AllowMakeUp { get; set; } = false;

    /// <summary>
    /// 每日簽到次數限制
    /// </summary>
    public int DailyLimit { get; set; } = 1;

    /// <summary>
    /// 時區設定 (分鐘偏移)
    /// </summary>
    public int TimezoneOffset { get; set; } = 480; // UTC+8
}