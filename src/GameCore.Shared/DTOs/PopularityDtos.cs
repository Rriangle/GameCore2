namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 遊戲熱度指數響應 DTO
    /// </summary>
    public class PopularityIndexResponseDto
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal IndexValue { get; set; }
        public string Trend { get; set; } = string.Empty; // up, down, stable
        public decimal ChangePercent { get; set; }
    }

    /// <summary>
    /// 排行榜響應 DTO
    /// </summary>
    public class LeaderboardResponseDto
    {
        public int Rank { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public decimal IndexValue { get; set; }
        public string Trend { get; set; } = string.Empty;
        public decimal ChangePercent { get; set; }
        public string Period { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// 遊戲指標響應 DTO
    /// </summary>
    public class GameMetricResponseDto
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public int MetricId { get; set; }
        public string MetricCode { get; set; } = string.Empty;
        public string MetricDescription { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Quality { get; set; } = string.Empty;
    }

    /// <summary>
    /// 遊戲響應 DTO
    /// </summary>
    public class GameResponseDto
    {
        public int GameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal? CurrentIndexValue { get; set; }
        public string? Trend { get; set; }
        public int ThreadCount { get; set; }
        public int PostCount { get; set; }
    }

    /// <summary>
    /// 指標來源響應 DTO
    /// </summary>
    public class MetricSourceResponseDto
    {
        public int SourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int MetricCount { get; set; }
    }

    /// <summary>
    /// 指標響應 DTO
    /// </summary>
    public class MetricResponseDto
    {
        public int MetricId { get; set; }
        public int SourceId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 熱度分析請求 DTO
    /// </summary>
    public class PopularityAnalysisRequestDto
    {
        public int? GameId { get; set; }
        public string? Period { get; set; } // daily, weekly, monthly
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Genre { get; set; }
        public int? Limit { get; set; }
    }

    /// <summary>
    /// 熱度分析響應 DTO
    /// </summary>
    public class PopularityAnalysisResponseDto
    {
        public string Period { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PopularityIndexResponseDto> TopGames { get; set; } = new();
        public List<PopularityIndexResponseDto> TrendingGames { get; set; } = new();
        public List<PopularityIndexResponseDto> DecliningGames { get; set; } = new();
        public Dictionary<string, int> GenreDistribution { get; set; } = new();
        public decimal AverageIndexValue { get; set; }
        public decimal MaxIndexValue { get; set; }
        public decimal MinIndexValue { get; set; }
    }
}