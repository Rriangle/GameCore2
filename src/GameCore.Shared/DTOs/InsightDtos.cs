namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 創建洞察貼文請求 DTO
    /// </summary>
    public class CreateInsightRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Tldr { get; set; } = string.Empty;
        public string BodyMd { get; set; } = string.Empty;
        public int? GameId { get; set; }
        public bool Visibility { get; set; } = true;
        public List<string> Sources { get; set; } = new(); // 引用來源名稱
        public List<string> SourceUrls { get; set; } = new(); // 引用來源URL
    }

    /// <summary>
    /// 更新洞察貼文請求 DTO
    /// </summary>
    public class UpdateInsightRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Tldr { get; set; } = string.Empty;
        public string BodyMd { get; set; } = string.Empty;
        public int? GameId { get; set; }
        public bool Visibility { get; set; } = true;
        public string Status { get; set; } = "draft";
        public List<string> Sources { get; set; } = new();
        public List<string> SourceUrls { get; set; } = new();
    }

    /// <summary>
    /// 洞察貼文響應 DTO
    /// </summary>
    public class InsightResponseDto
    {
        public int PostId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? GameId { get; set; }
        public string? GameName { get; set; }
        public string? GameGenre { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Tldr { get; set; } = string.Empty;
        public string BodyMd { get; set; } = string.Empty;
        public bool Visibility { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool Pinned { get; set; }
        public int CreatedBy { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PostMetricSnapshotDto? MetricSnapshot { get; set; }
        public List<PostSourceDto> Sources { get; set; } = new();
        public int ReactionCount { get; set; }
        public int BookmarkCount { get; set; }
        public bool IsUserReacted { get; set; }
        public bool IsUserBookmarked { get; set; }
    }

    /// <summary>
    /// 貼文指標快照 DTO
    /// </summary>
    public class PostMetricSnapshotDto
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal IndexValue { get; set; }
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// 貼文引用來源 DTO
    /// </summary>
    public class PostSourceDto
    {
        public string SourceName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    /// <summary>
    /// 洞察貼文列表響應 DTO
    /// </summary>
    public class InsightListResponseDto
    {
        public List<InsightResponseDto> Insights { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// 洞察貼文搜索請求 DTO
    /// </summary>
    public class InsightSearchRequestDto
    {
        public string? Keyword { get; set; }
        public int? GameId { get; set; }
        public string? Status { get; set; }
        public bool? Pinned { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } // created_at, published_at, title
        public string? SortOrder { get; set; } // asc, desc
    }

    /// <summary>
    /// 洞察貼文統計響應 DTO
    /// </summary>
    public class InsightStatsResponseDto
    {
        public int TotalInsights { get; set; }
        public int PublishedInsights { get; set; }
        public int DraftInsights { get; set; }
        public int PinnedInsights { get; set; }
        public int GameRelatedInsights { get; set; }
        public Dictionary<string, int> InsightsByGame { get; set; } = new();
        public Dictionary<string, int> InsightsByStatus { get; set; } = new();
        public Dictionary<string, int> InsightsByMonth { get; set; } = new();
    }
}