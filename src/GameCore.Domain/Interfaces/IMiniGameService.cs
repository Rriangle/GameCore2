namespace GameCore.Domain.Interfaces;

public interface IMiniGameService
{
    Task<MiniGameResult> PlayMiniGameAsync(int userId, string gameType);
    Task<IEnumerable<MiniGameRecordDto>> GetUserGameRecordsAsync(int userId);
    Task<MiniGameStatsDto?> GetUserGameStatsAsync(int userId);
}

public class MiniGameResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool IsWin { get; set; }
    public int Score { get; set; }
    public int PointsEarned { get; set; }
    public int ExpEarned { get; set; }
    public PetDto? UpdatedPet { get; set; }
}

public class MiniGameRecordDto
{
    public int GameId { get; set; }
    public string GameType { get; set; } = string.Empty;
    public bool IsWin { get; set; }
    public int Score { get; set; }
    public int PointsEarned { get; set; }
    public int ExpEarned { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MiniGameStatsDto
{
    public int TotalGames { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public double WinRate { get; set; }
    public int TotalPointsEarned { get; set; }
    public int TotalExpEarned { get; set; }
    public int AverageScore { get; set; }
    public int HighestScore { get; set; }
}