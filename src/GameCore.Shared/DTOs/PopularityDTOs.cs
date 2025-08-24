using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

// ==================== 遊戲相關 DTOs ====================

/// <summary>
/// 遊戲 DTO
/// </summary>
public class GameDto
{
    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 遊戲描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 發行商
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// 開發商
    /// </summary>
    public string? Developer { get; set; }

    /// <summary>
    /// 發行日期
    /// </summary>
    public DateOnly? ReleaseDate { get; set; }

    /// <summary>
    /// 遊戲類型
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// 平台
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// 遊戲圖片
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 當前熱度指數
    /// </summary>
    public decimal? CurrentPopularityIndex { get; set; }

    /// <summary>
    /// 當前排名
    /// </summary>
    public int? CurrentRank { get; set; }

    /// <summary>
    /// 排名變化
    /// </summary>
    public int? RankChange { get; set; }
}

/// <summary>
/// 遊戲詳情 DTO
/// </summary>
public class GameDetailDto : GameDto
{
    /// <summary>
    /// 遊戲標籤
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// 遊戲來源對應
    /// </summary>
    public List<GameSourceMapDto> SourceMaps { get; set; } = new();

    /// <summary>
    /// 最近7天熱度趨勢
    /// </summary>
    public List<PopularityIndexDto> RecentTrend { get; set; } = new();

    /// <summary>
    /// 統計摘要
    /// </summary>
    public GameStatsDto? Stats { get; set; }
}

/// <summary>
/// 遊戲統計 DTO
/// </summary>
public class GameStatsDto
{
    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 總論壇主題數
    /// </summary>
    public int TotalThreads { get; set; }

    /// <summary>
    /// 總論壇回覆數
    /// </summary>
    public int TotalPosts { get; set; }

    /// <summary>
    /// 總洞察貼文數
    /// </summary>
    public int TotalInsightPosts { get; set; }

    /// <summary>
    /// 平均熱度指數 (30天)
    /// </summary>
    public decimal AveragePopularityIndex { get; set; }

    /// <summary>
    /// 最高熱度指數
    /// </summary>
    public decimal MaxPopularityIndex { get; set; }

    /// <summary>
    /// 最高排名
    /// </summary>
    public int BestRank { get; set; }

    /// <summary>
    /// 在榜天數
    /// </summary>
    public int DaysOnLeaderboard { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

// ==================== 指標相關 DTOs ====================

/// <summary>
/// 指標來源 DTO
/// </summary>
public class MetricSourceDto
{
    /// <summary>
    /// 來源編號
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 來源名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 指標數量
    /// </summary>
    public int MetricCount { get; set; }
}

/// <summary>
/// 指標 DTO
/// </summary>
public class MetricDto
{
    /// <summary>
    /// 指標編號
    /// </summary>
    public int MetricId { get; set; }

    /// <summary>
    /// 來源編號
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 指標代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 來源名稱
    /// </summary>
    public string? SourceName { get; set; }
}

/// <summary>
/// 遊戲來源對應 DTO
/// </summary>
public class GameSourceMapDto
{
    /// <summary>
    /// 對應編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 來源編號
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 外部鍵
    /// </summary>
    public string ExternalKey { get; set; } = string.Empty;

    /// <summary>
    /// 來源名稱
    /// </summary>
    public string? SourceName { get; set; }

    /// <summary>
    /// 外部連結
    /// </summary>
    public string? ExternalUrl { get; set; }
}

/// <summary>
/// 遊戲指標數據 DTO
/// </summary>
public class GameMetricDto
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 指標編號
    /// </summary>
    public int MetricId { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 數值
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 聚合方法
    /// </summary>
    public string? AggMethod { get; set; }

    /// <summary>
    /// 資料品質
    /// </summary>
    public string? Quality { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 指標資訊
    /// </summary>
    public MetricDto? Metric { get; set; }

    /// <summary>
    /// 與前一天的變化
    /// </summary>
    public decimal? DayChange { get; set; }

    /// <summary>
    /// 變化百分比
    /// </summary>
    public decimal? ChangePercentage { get; set; }
}

/// <summary>
/// 遊戲指標比較 DTO
/// </summary>
public class GameMetricComparisonDto
{
    /// <summary>
    /// 指標資訊
    /// </summary>
    public MetricDto Metric { get; set; } = new();

    /// <summary>
    /// 比較期間
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// 比較期間
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// 遊戲數據對比
    /// </summary>
    public List<GameMetricComparisonItemDto> Games { get; set; } = new();
}

/// <summary>
/// 遊戲指標比較項目 DTO
/// </summary>
public class GameMetricComparisonItemDto
{
    /// <summary>
    /// 遊戲資訊
    /// </summary>
    public GameDto Game { get; set; } = new();

    /// <summary>
    /// 時間序列數據
    /// </summary>
    public List<GameMetricDto> Data { get; set; } = new();

    /// <summary>
    /// 平均值
    /// </summary>
    public decimal Average { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public decimal Maximum { get; set; }

    /// <summary>
    /// 最小值
    /// </summary>
    public decimal Minimum { get; set; }

    /// <summary>
    /// 總變化
    /// </summary>
    public decimal TotalChange { get; set; }
}

// ==================== 熱度指數相關 DTOs ====================

/// <summary>
/// 熱度指數 DTO
/// </summary>
public class PopularityIndexDto
{
    /// <summary>
    /// 編號
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 熱度指數
    /// </summary>
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string? GameName { get; set; }

    /// <summary>
    /// 與前一天的變化
    /// </summary>
    public decimal? DayChange { get; set; }

    /// <summary>
    /// 變化百分比
    /// </summary>
    public decimal? ChangePercentage { get; set; }

    /// <summary>
    /// 當日排名
    /// </summary>
    public int? Rank { get; set; }

    /// <summary>
    /// 排名變化
    /// </summary>
    public int? RankChange { get; set; }
}

// ==================== 排行榜相關 DTOs ====================

/// <summary>
/// 排行榜項目 DTO
/// </summary>
public class LeaderboardItemDto
{
    /// <summary>
    /// 排名
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string GameName { get; set; } = string.Empty;

    /// <summary>
    /// 熱度指數
    /// </summary>
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 排名變化
    /// </summary>
    public int? RankChange { get; set; }

    /// <summary>
    /// 指數變化
    /// </summary>
    public decimal? IndexChange { get; set; }

    /// <summary>
    /// 遊戲圖片
    /// </summary>
    public string? GameImageUrl { get; set; }

    /// <summary>
    /// 遊戲類型
    /// </summary>
    public string? GameGenre { get; set; }

    /// <summary>
    /// 是否新上榜
    /// </summary>
    public bool IsNew { get; set; }
}

/// <summary>
/// 排行榜快照 DTO
/// </summary>
public class LeaderboardSnapshotDto
{
    /// <summary>
    /// 期間類型
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// 快照時間
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 排行榜項目
    /// </summary>
    public List<LeaderboardItemDto> Items { get; set; } = new();

    /// <summary>
    /// 總項目數
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// 快照建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 遊戲排名歷史 DTO
/// </summary>
public class GameRankHistoryDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 排名
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 熱度指數
    /// </summary>
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 排名變化
    /// </summary>
    public int? RankChange { get; set; }

    /// <summary>
    /// 指數變化
    /// </summary>
    public decimal? IndexChange { get; set; }
}

// ==================== 洞察貼文相關 DTOs ====================

/// <summary>
/// 貼文 DTO
/// </summary>
public class PostDto
{
    /// <summary>
    /// 貼文編號
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// 貼文類型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 三行摘要
    /// </summary>
    public string? Tldr { get; set; }

    /// <summary>
    /// 內文 (Markdown)
    /// </summary>
    public string? BodyMd { get; set; }

    /// <summary>
    /// 可見性
    /// </summary>
    public bool Visibility { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 是否置頂
    /// </summary>
    public bool Pinned { get; set; }

    /// <summary>
    /// 作者編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 發佈時間
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 作者名稱
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string? GameName { get; set; }

    /// <summary>
    /// 觀看次數
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 讚數
    /// </summary>
    public int LikeCount { get; set; }

    /// <summary>
    /// 收藏數
    /// </summary>
    public int BookmarkCount { get; set; }

    /// <summary>
    /// 是否已按讚
    /// </summary>
    public bool IsLiked { get; set; }

    /// <summary>
    /// 是否已收藏
    /// </summary>
    public bool IsBookmarked { get; set; }
}

/// <summary>
/// 貼文詳情 DTO
/// </summary>
public class PostDetailDto : PostDto
{
    /// <summary>
    /// 指標快照
    /// </summary>
    public PostMetricSnapshotDto? MetricSnapshot { get; set; }

    /// <summary>
    /// 引用來源
    /// </summary>
    public List<PostSourceDto> Sources { get; set; } = new();

    /// <summary>
    /// 標籤
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// 相關貼文
    /// </summary>
    public List<PostDto> RelatedPosts { get; set; } = new();
}

/// <summary>
/// 貼文指標快照 DTO
/// </summary>
public class PostMetricSnapshotDto
{
    /// <summary>
    /// 貼文編號
    /// </summary>
    public long PostId { get; set; }

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 快照日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 熱度指數
    /// </summary>
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 詳細資料 (JSON)
    /// </summary>
    public string? DetailsJson { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string? GameName { get; set; }

    /// <summary>
    /// 解析後的詳細資料
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }
}

/// <summary>
/// 貼文來源 DTO
/// </summary>
public class PostSourceDto
{
    /// <summary>
    /// 來源編號
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 貼文編號
    /// </summary>
    public long PostId { get; set; }

    /// <summary>
    /// 來源名稱
    /// </summary>
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 連結
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 遊戲洞察統計 DTO
/// </summary>
public class GameInsightStatsDto
{
    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    public string GameName { get; set; } = string.Empty;

    /// <summary>
    /// 總洞察貼文數
    /// </summary>
    public int TotalPosts { get; set; }

    /// <summary>
    /// 已發佈貼文數
    /// </summary>
    public int PublishedPosts { get; set; }

    /// <summary>
    /// 草稿貼文數
    /// </summary>
    public int DraftPosts { get; set; }

    /// <summary>
    /// 置頂貼文數
    /// </summary>
    public int PinnedPosts { get; set; }

    /// <summary>
    /// 總觀看次數
    /// </summary>
    public int TotalViews { get; set; }

    /// <summary>
    /// 總讚數
    /// </summary>
    public int TotalLikes { get; set; }

    /// <summary>
    /// 總收藏數
    /// </summary>
    public int TotalBookmarks { get; set; }

    /// <summary>
    /// 平均觀看次數
    /// </summary>
    public decimal AverageViews { get; set; }

    /// <summary>
    /// 最後發佈時間
    /// </summary>
    public DateTime? LastPublishedAt { get; set; }

    /// <summary>
    /// 最受歡迎的貼文
    /// </summary>
    public PostDto? MostPopularPost { get; set; }
}

/// <summary>
/// 貼文趨勢分析 DTO
/// </summary>
public class PostTrendAnalysisDto
{
    /// <summary>
    /// 分析期間開始
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// 分析期間結束
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// 每日貼文數量趨勢
    /// </summary>
    public List<PostCountTrendDto> DailyPostCount { get; set; } = new();

    /// <summary>
    /// 熱門遊戲排行
    /// </summary>
    public List<GamePostStatsDto> PopularGames { get; set; } = new();

    /// <summary>
    /// 熱門作者排行
    /// </summary>
    public List<AuthorPostStatsDto> PopularAuthors { get; set; } = new();

    /// <summary>
    /// 總統計
    /// </summary>
    public PostTrendSummaryDto Summary { get; set; } = new();
}

/// <summary>
/// 貼文數量趨勢 DTO
/// </summary>
public class PostCountTrendDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 貼文數量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 發佈數量
    /// </summary>
    public int PublishedCount { get; set; }

    /// <summary>
    /// 觀看次數
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// 互動次數
    /// </summary>
    public int Interactions { get; set; }
}

/// <summary>
/// 遊戲貼文統計 DTO
/// </summary>
public class GamePostStatsDto
{
    /// <summary>
    /// 遊戲資訊
    /// </summary>
    public GameDto Game { get; set; } = new();

    /// <summary>
    /// 貼文數量
    /// </summary>
    public int PostCount { get; set; }

    /// <summary>
    /// 觀看次數
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 互動次數
    /// </summary>
    public int InteractionCount { get; set; }
}

/// <summary>
/// 作者貼文統計 DTO
/// </summary>
public class AuthorPostStatsDto
{
    /// <summary>
    /// 作者編號
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// 作者名稱
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// 貼文數量
    /// </summary>
    public int PostCount { get; set; }

    /// <summary>
    /// 觀看次數
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 互動次數
    /// </summary>
    public int InteractionCount { get; set; }
}

/// <summary>
/// 貼文趨勢摘要 DTO
/// </summary>
public class PostTrendSummaryDto
{
    /// <summary>
    /// 總貼文數
    /// </summary>
    public int TotalPosts { get; set; }

    /// <summary>
    /// 已發佈貼文數
    /// </summary>
    public int PublishedPosts { get; set; }

    /// <summary>
    /// 總觀看次數
    /// </summary>
    public int TotalViews { get; set; }

    /// <summary>
    /// 總互動次數
    /// </summary>
    public int TotalInteractions { get; set; }

    /// <summary>
    /// 平均每日貼文數
    /// </summary>
    public decimal AverageDailyPosts { get; set; }

    /// <summary>
    /// 最活躍的日期
    /// </summary>
    public DateOnly? MostActiveDate { get; set; }

    /// <summary>
    /// 成長率 (與上期比較)
    /// </summary>
    public decimal? GrowthRate { get; set; }
}

// ==================== 分頁 DTOs ====================

/// <summary>
/// 分頁遊戲 DTO
/// </summary>
public class PagedGamesDto
{
    /// <summary>
    /// 遊戲列表
    /// </summary>
    public List<GameDto> Games { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁遊戲指標 DTO
/// </summary>
public class PagedGameMetricsDto
{
    /// <summary>
    /// 指標數據列表
    /// </summary>
    public List<GameMetricDto> Metrics { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁熱度指數 DTO
/// </summary>
public class PagedPopularityIndexDto
{
    /// <summary>
    /// 熱度指數列表
    /// </summary>
    public List<PopularityIndexDto> PopularityIndexes { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁遊戲排名歷史 DTO
/// </summary>
public class PagedGameRankHistoryDto
{
    /// <summary>
    /// 排名歷史列表
    /// </summary>
    public List<GameRankHistoryDto> RankHistory { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// 分頁貼文 DTO
/// </summary>
public class PagedPostsDto
{
    /// <summary>
    /// 貼文列表
    /// </summary>
    public List<PostDto> Posts { get; set; } = new();

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}

// ==================== 查詢 DTOs ====================

/// <summary>
/// 遊戲查詢 DTO
/// </summary>
public class GameQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (name/popularity/release_date)
    /// </summary>
    public string? SortBy { get; set; } = "popularity";

    /// <summary>
    /// 是否只顯示啟用的遊戲
    /// </summary>
    public bool? ActiveOnly { get; set; } = true;

    /// <summary>
    /// 遊戲類型篩選
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// 平台篩選
    /// </summary>
    public string? Platform { get; set; }
}

/// <summary>
/// 遊戲搜尋查詢 DTO
/// </summary>
public class GameSearchQueryDto
{
    /// <summary>
    /// 關鍵字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序方式 (relevance/popularity/name)
    /// </summary>
    public string? SortBy { get; set; } = "relevance";

    /// <summary>
    /// 遊戲類型篩選
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// 平台篩選
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// 發行商篩選
    /// </summary>
    public string? Publisher { get; set; }
}

/// <summary>
/// 遊戲指標查詢 DTO
/// </summary>
public class GameMetricQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// 指標編號篩選
    /// </summary>
    public int? MetricId { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// 排序方式 (date/value)
    /// </summary>
    public string? SortBy { get; set; } = "date";

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool Descending { get; set; } = true;
}

/// <summary>
/// 熱度指數查詢 DTO
/// </summary>
public class PopularityIndexQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// 排序方式 (date/index_value)
    /// </summary>
    public string? SortBy { get; set; } = "date";

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool Descending { get; set; } = true;
}

/// <summary>
/// 排行榜查詢 DTO
/// </summary>
public class LeaderboardQueryDto
{
    /// <summary>
    /// 期間類型
    /// </summary>
    [Required]
    public string Period { get; set; } = "daily";

    /// <summary>
    /// 查詢日期
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// 取得數量
    /// </summary>
    public int Limit { get; set; } = 10;
}

/// <summary>
/// 遊戲排名查詢 DTO
/// </summary>
public class GameRankQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateOnly? EndDate { get; set; }
}

/// <summary>
/// 貼文查詢 DTO
/// </summary>
public class PostQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 貼文類型篩選
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 遊戲編號篩選
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 狀態篩選
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 是否只顯示置頂
    /// </summary>
    public bool? PinnedOnly { get; set; }

    /// <summary>
    /// 作者編號篩選
    /// </summary>
    public int? AuthorId { get; set; }

    /// <summary>
    /// 排序方式 (latest/popular/published_at)
    /// </summary>
    public string? SortBy { get; set; } = "latest";

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 貼文搜尋查詢 DTO
/// </summary>
public class PostSearchQueryDto
{
    /// <summary>
    /// 關鍵字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁數量
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 貼文類型篩選
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 遊戲編號篩選
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 作者編號篩選
    /// </summary>
    public int? AuthorId { get; set; }

    /// <summary>
    /// 排序方式 (relevance/latest/popular)
    /// </summary>
    public string? SortBy { get; set; } = "relevance";

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 貼文趨勢查詢 DTO
/// </summary>
public class PostTrendQueryDto
{
    /// <summary>
    /// 開始日期
    /// </summary>
    [Required]
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    [Required]
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// 遊戲編號篩選
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 貼文類型篩選
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 分組方式 (daily/weekly/monthly)
    /// </summary>
    public string? GroupBy { get; set; } = "daily";
}

// ==================== 請求 DTOs ====================

/// <summary>
/// 建立貼文請求 DTO
/// </summary>
public class CreatePostRequestDto
{
    /// <summary>
    /// 貼文類型
    /// </summary>
    [Required]
    public string Type { get; set; } = "insight";

    /// <summary>
    /// 遊戲編號
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// 標題
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 三行摘要
    /// </summary>
    [StringLength(300)]
    public string? Tldr { get; set; }

    /// <summary>
    /// 內文 (Markdown)
    /// </summary>
    [Required]
    public string BodyMd { get; set; } = string.Empty;

    /// <summary>
    /// 可見性
    /// </summary>
    public bool Visibility { get; set; } = true;

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "draft";

    /// <summary>
    /// 標籤
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 引用來源
    /// </summary>
    public List<CreatePostSourceDto>? Sources { get; set; }
}

/// <summary>
/// 更新貼文請求 DTO
/// </summary>
public class UpdatePostRequestDto
{
    /// <summary>
    /// 標題
    /// </summary>
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// 三行摘要
    /// </summary>
    [StringLength(300)]
    public string? Tldr { get; set; }

    /// <summary>
    /// 內文 (Markdown)
    /// </summary>
    public string? BodyMd { get; set; }

    /// <summary>
    /// 可見性
    /// </summary>
    public bool? Visibility { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 標籤
    /// </summary>
    public List<string>? Tags { get; set; }
}

/// <summary>
/// 建立貼文來源 DTO
/// </summary>
public class CreatePostSourceDto
{
    /// <summary>
    /// 來源名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 連結
    /// </summary>
    [Url]
    public string? Url { get; set; }
}

/// <summary>
/// 新增貼文來源請求 DTO
/// </summary>
public class AddPostSourceRequestDto
{
    /// <summary>
    /// 來源名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string SourceName { get; set; } = string.Empty;

    /// <summary>
    /// 連結
    /// </summary>
    [Url]
    public string? Url { get; set; }
}