namespace GameCore.Domain.Interfaces;

public interface ISignInService
{
    Task<SignInResult> SignInAsync(int userId);
    Task<SignInStatsDto?> GetSignInStatsAsync(int userId);
    Task<bool> HasSignedInTodayAsync(int userId);
}

public class SignInResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int PointsEarned { get; set; }
    public int ExpEarned { get; set; }
    public int StreakCount { get; set; }
    public bool IsStreakBonus { get; set; }
    public bool IsMonthBonus { get; set; }
}

public class SignInStatsDto
{
    public int TotalSignIns { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastSignInDate { get; set; }
    public bool HasSignedInToday { get; set; }
}