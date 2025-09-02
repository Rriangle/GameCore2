using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 遊戲排行榜項目 DTO
/// </summary>
public class LeaderboardItemDto
{
    public int Rank { get; set; }
    public int GameID { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal PopularityScore { get; set; }
    public int ConcurrentUsers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalViews { get; set; }
    public decimal AverageRating { get; set; }
    public string Change { get; set; } = string.Empty; // "up", "down", "flat"
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// 熱門度指標 DTO
/// </summary>
public class PopularityMetricDto
{
    public int GameID { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string MetricCode { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Trend { get; set; } = string.Empty; // "increasing", "decreasing", "stable"
    public decimal ChangePercent { get; set; }
}

/// <summary>
/// 遊戲熱門度快照 DTO
/// </summary>
public class GamePopularitySnapshotDto
{
    public int SnapshotID { get; set; }
    public int GameID { get; set; }
    public string GameName { get; set; } = string.Empty;
    public decimal PopularityIndex { get; set; }
    public int Rank { get; set; }
    public int ConcurrentUsers { get; set; }
    public int ForumPosts { get; set; }
    public int MarketListings { get; set; }
    public decimal AverageRating { get; set; }
    public DateTime SnapshotDate { get; set; }
    public string Period { get; set; } = string.Empty; // "daily", "weekly", "monthly"
}

/// <summary>
/// 排行榜搜尋參數 DTO
/// </summary>
public class LeaderboardSearchDto
{
    public string? Category { get; set; }
    public string? Period { get; set; } = "daily";
    public DateTime? Date { get; set; } = DateTime.UtcNow.Date; // 新增 Date 屬性
    public int Top { get; set; } = 100;
    public string SortBy { get; set; } = "popularity"; // "popularity", "users", "posts", "rating"
    public string SortOrder { get; set; } = "desc"; // "asc", "desc"
}

/// <summary>
/// 遊戲詳細資訊 DTO
/// </summary>
public class GameDetailDto
{
    public int GameID { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<PopularityMetricDto> Metrics { get; set; } = new List<PopularityMetricDto>();
}

/// <summary>
/// 熱門度趨勢 DTO
/// </summary>
public class PopularityTrendDto
{
    public int GameID { get; set; }
    public string GameName { get; set; } = string.Empty;
    public List<PopularityDataPoint> DataPoints { get; set; } = new List<PopularityDataPoint>();
}

/// <summary>
/// 熱門度資料點
/// </summary>
public class PopularityDataPoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Metric { get; set; } = string.Empty;
}

/// <summary>
/// 遊戲分類排行榜 DTO
/// </summary>
public class CategoryLeaderboardDto
{
    public string Category { get; set; } = string.Empty;
    public List<LeaderboardItemDto> TopGames { get; set; } = new List<LeaderboardItemDto>();
    public DateTime LastUpdated { get; set; }
    public int TotalGames { get; set; }
} 