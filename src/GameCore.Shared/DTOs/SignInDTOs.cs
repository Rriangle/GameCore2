using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 簽到狀態 DTO
/// </summary>
public class SignInStatusDto
{
    /// <summary>
    /// 今日是否已簽到
    /// </summary>
    public bool TodaySigned { get; set; }

    /// <summary>
    /// 當前連續天數
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
    /// 本月簽到率
    /// </summary>
    public decimal MonthAttendanceRate { get; set; }

    /// <summary>
    /// 最後簽到日期
    /// </summary>
    public DateTime? LastSignInDate { get; set; }
}

/// <summary>
/// 簽到結果 DTO
/// </summary>
public class SignInResultDto
{
    /// <summary>
    /// 簽到成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 獲得點數
    /// </summary>
    public int PointsGained { get; set; }

    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int ExpGained { get; set; }

    /// <summary>
    /// 簽到後連續天數
    /// </summary>
    public int StreakAfter { get; set; }

    /// <summary>
    /// 是否獲得連續獎勵
    /// </summary>
    public bool GotStreakBonus { get; set; }

    /// <summary>
    /// 是否獲得全勤獎勵
    /// </summary>
    public bool GotFullMonthBonus { get; set; }

    /// <summary>
    /// 簽到時間
    /// </summary>
    public DateTime SignInTime { get; set; }
}

/// <summary>
/// 簽到歷史 DTO
/// </summary>
public class SignInHistoryDto
{
    /// <summary>
    /// 簽到記錄ID
    /// </summary>
    public int LogId { get; set; }

    /// <summary>
    /// 簽到時間
    /// </summary>
    public DateTime SignTime { get; set; }

    /// <summary>
    /// 獲得點數
    /// </summary>
    public int PointsChanged { get; set; }

    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int ExpGained { get; set; }

    /// <summary>
    /// 是否為連續獎勵
    /// </summary>
    public bool IsStreakBonus { get; set; }

    /// <summary>
    /// 是否為全勤獎勵
    /// </summary>
    public bool IsFullMonthBonus { get; set; }
} 