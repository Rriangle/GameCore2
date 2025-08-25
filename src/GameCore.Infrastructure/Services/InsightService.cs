using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 遊戲洞察服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class InsightService : IInsightService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InsightService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 1000;
    private const int DefaultPageSize = 100;
    private const int CacheExpirationMinutes = 20;
    private const string GameInsightsCacheKey = "Insights_Game_{0}_{1:yyyyMMdd}_{2:yyyyMMdd}";
    private const string TrendAnalysisCacheKey = "Insights_Trend_{0}_{1:yyyyMMdd}_{2:yyyyMMdd}";
    private const string ComparativeAnalysisCacheKey = "Insights_Comparative_{0}_{1}_{2:yyyyMMdd}_{3:yyyyMMdd}";
    private const string MetricCorrelationCacheKey = "Insights_Correlation_{0}_{1}_{2:yyyyMMdd}_{3:yyyyMMdd}";
    private const string AllInsightsCacheKey = "Insights_All_{0:yyyyMMdd}_{1:yyyyMMdd}";

    // 趨勢分析配置
    private static readonly Dictionary<string, int> TrendPeriods = new()
    {
        { "daily", 7 },
        { "weekly", 4 },
        { "monthly", 12 },
        { "quarterly", 4 },
        { "yearly", 5 }
    };

    public InsightService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<InsightService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 獲取遊戲洞察 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<InsightResponseDto>> GetGameInsightsAsync(int gameId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取遊戲洞察，遊戲ID: {GameId}, 開始日期: {StartDate}, 結束日期: {EndDate}", 
            gameId, startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateInsightRequest(gameId, startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("洞察請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<InsightResponseDto>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(GameInsightsCacheKey, gameId, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<InsightResponseDto> cachedInsights))
            {
                _logger.LogDebug("從快取獲取遊戲洞察，遊戲ID: {GameId}", gameId);
                return cachedInsights;
            }

            // 從資料庫獲取
            var insights = await GenerateGameInsightsAsync(gameId, startDate, endDate);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, insights, cacheOptions);

            _logger.LogInformation("成功獲取遊戲洞察，遊戲ID: {GameId}, 洞察數量: {Count}", gameId, insights.Count());
            return insights;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲 {GameId} 洞察時發生錯誤", gameId);
            throw;
        }
    }

    /// <summary>
    /// 創建洞察 - 優化版本
    /// </summary>
    public async Task<InsightResponseDto> CreateInsightAsync(CreateInsightRequestDto request)
    {
        _logger.LogInformation("開始創建洞察，遊戲ID: {GameId}, 類型: {Type}", request.GameId, request.Type);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateInsightRequest(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("創建洞察請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 檢查是否已存在相同類型的洞察
            var existingInsight = await _context.Insights
                .FirstOrDefaultAsync(i => i.game_id == request.GameId && 
                                        i.type == request.Type && 
                                        i.date == request.Date);

            if (existingInsight != null)
            {
                _logger.LogInformation("遊戲 {GameId} 在 {Date} 的 {Type} 洞察已存在，跳過創建", 
                    request.GameId, request.Date, request.Type);
                return MapToInsightResponseDto(existingInsight);
            }

            // 創建新洞察
            var insight = new Insight
            {
                game_id = request.GameId,
                type = request.Type,
                title = request.Title,
                description = request.Description,
                data = JsonSerializer.Serialize(request.Data),
                date = request.Date,
                created_at = DateTime.UtcNow
            };

            _context.Insights.Add(insight);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearInsightRelatedCache(request.GameId, request.Date);

            var response = MapToInsightResponseDto(insight);
            _logger.LogInformation("成功創建洞察，ID: {InsightId}", response.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建洞察時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取趨勢分析 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<TrendAnalysisDto>> GetTrendAnalysisAsync(int gameId, string metric, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取趨勢分析，遊戲ID: {GameId}, 指標: {Metric}, 開始日期: {StartDate}, 結束日期: {EndDate}", 
            gameId, metric, startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateTrendAnalysisRequest(gameId, metric, startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("趨勢分析請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<TrendAnalysisDto>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(TrendAnalysisCacheKey, gameId, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<TrendAnalysisDto> cachedAnalysis))
            {
                _logger.LogDebug("從快取獲取趨勢分析，遊戲ID: {GameId}, 指標: {Metric}", gameId, metric);
                return cachedAnalysis;
            }

            // 從資料庫獲取並分析
            var analysis = await GenerateTrendAnalysisAsync(gameId, metric, startDate, endDate);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, analysis, cacheOptions);

            _logger.LogInformation("成功獲取趨勢分析，遊戲ID: {GameId}, 指標: {Metric}, 分析點數: {Count}", 
                gameId, metric, analysis.Count());
            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲 {GameId} 指標 {Metric} 趨勢分析時發生錯誤", gameId, metric);
            throw;
        }
    }

    /// <summary>
    /// 獲取比較分析 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<ComparativeAnalysisDto>> GetComparativeAnalysisAsync(int gameId1, int gameId2, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取比較分析，遊戲1: {GameId1}, 遊戲2: {GameId2}, 開始日期: {StartDate}, 結束日期: {EndDate}", 
            gameId1, gameId2, startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateComparativeAnalysisRequest(gameId1, gameId2, startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("比較分析請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<ComparativeAnalysisDto>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ComparativeAnalysisCacheKey, gameId1, gameId2, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<ComparativeAnalysisDto> cachedAnalysis))
            {
                _logger.LogDebug("從快取獲取比較分析，遊戲1: {GameId1}, 遊戲2: {GameId2}", gameId1, gameId2);
                return cachedAnalysis;
            }

            // 從資料庫獲取並分析
            var analysis = await GenerateComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, analysis, cacheOptions);

            _logger.LogInformation("成功獲取比較分析，遊戲1: {GameId1}, 遊戲2: {GameId2}, 分析點數: {Count}", 
                gameId1, gameId2, analysis.Count());
            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲 {GameId1} 和 {GameId2} 比較分析時發生錯誤", gameId1, gameId2);
            throw;
        }
    }

    /// <summary>
    /// 獲取指標相關性分析 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<MetricCorrelationDto>> GetMetricCorrelationAsync(int gameId, string metric1, string metric2, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取指標相關性分析，遊戲ID: {GameId}, 指標1: {Metric1}, 指標2: {Metric2}, 開始日期: {StartDate}, 結束日期: {EndDate}", 
            gameId, metric1, metric2, startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateMetricCorrelationRequest(gameId, metric1, metric2, startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("指標相關性分析請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<MetricCorrelationDto>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(MetricCorrelationCacheKey, gameId, metric1, metric2, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<MetricCorrelationDto> cachedAnalysis))
            {
                _logger.LogDebug("從快取獲取指標相關性分析，遊戲ID: {GameId}, 指標1: {Metric1}, 指標2: {Metric2}", gameId, metric1, metric2);
                return cachedAnalysis;
            }

            // 從資料庫獲取並分析
            var analysis = await GenerateMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, analysis, cacheOptions);

            _logger.LogInformation("成功獲取指標相關性分析，遊戲ID: {GameId}, 指標1: {Metric1}, 指標2: {Metric2}, 分析點數: {Count}", 
                gameId, metric1, metric2, analysis.Count());
            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲 {GameId} 指標 {Metric1} 和 {Metric2} 相關性分析時發生錯誤", gameId, metric1, metric2);
            throw;
        }
    }

    /// <summary>
    /// 獲取所有洞察 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<InsightResponseDto>> GetAllInsightsAsync(DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取所有洞察，開始日期: {StartDate}, 結束日期: {EndDate}", startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateDateRange(startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("日期範圍驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<InsightResponseDto>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(AllInsightsCacheKey, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<InsightResponseDto> cachedInsights))
            {
                _logger.LogDebug("從快取獲取所有洞察");
                return cachedInsights;
            }

            // 從資料庫獲取
            var insights = await _context.Insights
                .Include(i => i.Game)
                .Where(i => i.date >= startDate && i.date <= endDate)
                .OrderByDescending(i => i.date)
                .ThenBy(i => i.game_id)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            var response = insights.Select(MapToInsightResponseDto);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, response, cacheOptions);

            _logger.LogInformation("成功獲取所有洞察，數量: {Count}", insights.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取所有洞察時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 清除洞察相關快取 - 新增方法
    /// </summary>
    public void ClearInsightRelatedCache(int gameId, DateTime date)
    {
        try
        {
            // 清除特定遊戲的洞察快取
            var keysToRemove = new List<string>();
            foreach (var key in _memoryCache.GetKeys())
            {
                if (key is string stringKey && 
                    (stringKey.Contains($"Insights_Game_{gameId}_") || 
                     stringKey.Contains($"Insights_All_")))
                {
                    keysToRemove.Add(stringKey);
                }
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            _logger.LogDebug("已清除遊戲 {GameId} 在 {Date} 的洞察相關快取", gameId, date);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除洞察相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證洞察請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateInsightRequest(int gameId, DateTime startDate, DateTime endDate)
    {
        var result = new ValidationResult();

        if (gameId <= 0)
            result.AddError("無效的遊戲ID");

        var dateValidation = ValidateDateRange(startDate, endDate);
        if (!dateValidation.IsValid)
            result.AddError(dateValidation.ErrorMessage);

        return result;
    }

    /// <summary>
    /// 驗證創建洞察請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateInsightRequest(CreateInsightRequestDto request)
    {
        var result = new ValidationResult();

        if (request == null)
            result.AddError("請求不能為空");

        if (request.GameId <= 0)
            result.AddError("無效的遊戲ID");

        if (string.IsNullOrWhiteSpace(request.Type))
            result.AddError("洞察類型不能為空");

        if (string.IsNullOrWhiteSpace(request.Title))
            result.AddError("洞察標題不能為空");

        if (string.IsNullOrWhiteSpace(request.Description))
            result.AddError("洞察描述不能為空");

        if (request.Date > DateTime.UtcNow.Date)
            result.AddError("洞察日期不能是未來日期");

        return result;
    }

    /// <summary>
    /// 驗證趨勢分析請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateTrendAnalysisRequest(int gameId, string metric, DateTime startDate, DateTime endDate)
    {
        var result = new ValidationResult();

        if (gameId <= 0)
            result.AddError("無效的遊戲ID");

        if (string.IsNullOrWhiteSpace(metric))
            result.AddError("指標不能為空");

        var dateValidation = ValidateDateRange(startDate, endDate);
        if (!dateValidation.IsValid)
            result.AddError(dateValidation.ErrorMessage);

        return result;
    }

    /// <summary>
    /// 驗證比較分析請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateComparativeAnalysisRequest(int gameId1, int gameId2, DateTime startDate, DateTime endDate)
    {
        var result = new ValidationResult();

        if (gameId1 <= 0)
            result.AddError("無效的遊戲1 ID");

        if (gameId2 <= 0)
            result.AddError("無效的遊戲2 ID");

        if (gameId1 == gameId2)
            result.AddError("兩個遊戲ID不能相同");

        var dateValidation = ValidateDateRange(startDate, endDate);
        if (!dateValidation.IsValid)
            result.AddError(dateValidation.ErrorMessage);

        return result;
    }

    /// <summary>
    /// 驗證指標相關性分析請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateMetricCorrelationRequest(int gameId, string metric1, string metric2, DateTime startDate, DateTime endDate)
    {
        var result = new ValidationResult();

        if (gameId <= 0)
            result.AddError("無效的遊戲ID");

        if (string.IsNullOrWhiteSpace(metric1))
            result.AddError("指標1不能為空");

        if (string.IsNullOrWhiteSpace(metric2))
            result.AddError("指標2不能為空");

        if (metric1 == metric2)
            result.AddError("兩個指標不能相同");

        var dateValidation = ValidateDateRange(startDate, endDate);
        if (!dateValidation.IsValid)
            result.AddError(dateValidation.ErrorMessage);

        return result;
    }

    /// <summary>
    /// 驗證日期範圍 - 新增方法
    /// </summary>
    private ValidationResult ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        var result = new ValidationResult();

        if (startDate > endDate)
            result.AddError("開始日期不能晚於結束日期");

        if (startDate > DateTime.UtcNow.Date)
            result.AddError("開始日期不能是未來日期");

        if (endDate > DateTime.UtcNow.Date)
            result.AddError("結束日期不能是未來日期");

        var dateRange = endDate - startDate;
        if (dateRange.TotalDays > 365)
            result.AddError("日期範圍不能超過一年");

        return result;
    }

    /// <summary>
    /// 生成遊戲洞察 - 優化版本
    /// </summary>
    private async Task<List<InsightResponseDto>> GenerateGameInsightsAsync(int gameId, DateTime startDate, DateTime endDate)
    {
        var insights = new List<InsightResponseDto>();

        try
        {
            // 獲取遊戲指標數據
            var metrics = await _context.GameMetricDailies
                .Include(m => m.Metric)
                .Where(m => m.game_id == gameId && m.date >= startDate && m.date <= endDate)
                .OrderBy(m => m.date)
                .AsNoTracking()
                .ToListAsync();

            if (!metrics.Any())
            {
                _logger.LogWarning("遊戲 {GameId} 在指定日期範圍內沒有指標數據", gameId);
                return insights;
            }

            // 生成趨勢洞察
            var trendInsights = GenerateTrendInsights(metrics, gameId);
            insights.AddRange(trendInsights);

            // 生成異常洞察
            var anomalyInsights = GenerateAnomalyInsights(metrics, gameId);
            insights.AddRange(anomalyInsights);

            // 生成比較洞察
            var comparisonInsights = await GenerateComparisonInsights(gameId, startDate, endDate);
            insights.AddRange(comparisonInsights);

            _logger.LogDebug("成功生成遊戲 {GameId} 的洞察，數量: {Count}", gameId, insights.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成遊戲 {GameId} 洞察時發生錯誤", gameId);
        }

        return insights;
    }

    /// <summary>
    /// 生成趨勢洞察 - 新增方法
    /// </summary>
    private List<InsightResponseDto> GenerateTrendInsights(List<GameMetricDaily> metrics, int gameId)
    {
        var insights = new List<InsightResponseDto>();

        try
        {
            var metricGroups = metrics.GroupBy(m => m.metric_id).ToList();

            foreach (var group in metricGroups)
            {
                var metric = group.First().Metric;
                var values = group.OrderBy(m => m.date).Select(m => m.value).ToList();

                if (values.Count < 3) continue; // 至少需要3個數據點

                // 計算趨勢
                var trend = CalculateTrend(values);
                var trendType = DetermineTrendType(trend);

                if (trendType != "stable")
                {
                    var insight = new InsightResponseDto
                    {
                        Id = 0, // 臨時ID
                        GameId = gameId,
                        Type = "trend_analysis",
                        Title = $"{metric.name} 趨勢分析",
                        Description = $"指標 {metric.name} 顯示{trendType}趨勢，變化率為 {trend:F2}%",
                        Data = new Dictionary<string, object>
                        {
                            { "metric_code", metric.code },
                            { "trend_type", trendType },
                            { "trend_percentage", trend },
                            { "data_points", values.Count }
                        },
                        Date = group.Max(m => m.date),
                        CreatedAt = DateTime.UtcNow
                    };

                    insights.Add(insight);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成趨勢洞察時發生錯誤");
        }

        return insights;
    }

    /// <summary>
    /// 生成異常洞察 - 新增方法
    /// </summary>
    private List<InsightResponseDto> GenerateAnomalyInsights(List<GameMetricDaily> metrics, int gameId)
    {
        var insights = new List<InsightResponseDto>();

        try
        {
            var metricGroups = metrics.GroupBy(m => m.metric_id).ToList();

            foreach (var group in metricGroups)
            {
                var metric = group.First().Metric;
                var values = group.Select(m => m.value).ToList();

                if (values.Count < 5) continue; // 至少需要5個數據點

                // 計算統計數據
                var mean = values.Average();
                var stdDev = CalculateStandardDeviation(values, mean);

                // 檢測異常值（超過2個標準差）
                var anomalies = values.Where(v => Math.Abs(v - mean) > 2 * stdDev).ToList();

                if (anomalies.Any())
                {
                    var insight = new InsightResponseDto
                    {
                        Id = 0, // 臨時ID
                        GameId = gameId,
                        Type = "anomaly_detection",
                        Title = $"{metric.name} 異常檢測",
                        Description = $"檢測到 {anomalies.Count} 個異常值，平均偏差為 {Math.Abs(anomalies.Average() - mean):F2}",
                        Data = new Dictionary<string, object>
                        {
                            { "metric_code", metric.code },
                            { "anomaly_count", anomalies.Count },
                            { "mean_value", mean },
                            { "std_deviation", stdDev },
                            { "anomaly_values", anomalies }
                        },
                        Date = group.Max(m => m.date),
                        CreatedAt = DateTime.UtcNow
                    };

                    insights.Add(insight);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成異常洞察時發生錯誤");
        }

        return insights;
    }

    /// <summary>
    /// 生成比較洞察 - 新增方法
    /// </summary>
    private async Task<List<InsightResponseDto>> GenerateComparisonInsights(int gameId, DateTime startDate, DateTime endDate)
    {
        var insights = new List<InsightResponseDto>();

        try
        {
            // 獲取同期其他遊戲的數據進行比較
            var otherGamesMetrics = await _context.GameMetricDailies
                .Include(m => m.Game)
                .Include(m => m.Metric)
                .Where(m => m.game_id != gameId && m.date >= startDate && m.date <= endDate)
                .AsNoTracking()
                .ToListAsync();

            if (!otherGamesMetrics.Any()) return insights;

            // 按指標分組比較
            var metricGroups = otherGamesMetrics.GroupBy(m => m.metric_id).ToList();

            foreach (var group in metricGroups)
            {
                var metric = group.First().Metric;
                var currentGameMetrics = await _context.GameMetricDailies
                    .Where(m => m.game_id == gameId && m.metric_id == metric.metric_id && m.date >= startDate && m.date <= endDate)
                    .AsNoTracking()
                    .ToListAsync();

                if (!currentGameMetrics.Any()) continue;

                var currentGameAvg = currentGameMetrics.Average(m => m.value);
                var otherGamesAvg = group.Average(m => m.value);

                if (Math.Abs(currentGameAvg - otherGamesAvg) > otherGamesAvg * 0.2m) // 20%差異閾值
                {
                    var insight = new InsightResponseDto
                    {
                        Id = 0, // 臨時ID
                        GameId = gameId,
                        Type = "competitive_analysis",
                        Title = $"{metric.name} 競爭分析",
                        Description = $"與其他遊戲相比，{metric.name} 指標差異為 {((currentGameAvg - otherGamesAvg) / otherGamesAvg * 100):F1}%",
                        Data = new Dictionary<string, object>
                        {
                            { "metric_code", metric.code },
                            { "current_game_avg", currentGameAvg },
                            { "other_games_avg", otherGamesAvg },
                            { "difference_percentage", (currentGameAvg - otherGamesAvg) / otherGamesAvg * 100 }
                        },
                        Date = endDate,
                        CreatedAt = DateTime.UtcNow
                    };

                    insights.Add(insight);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成比較洞察時發生錯誤");
        }

        return insights;
    }

    /// <summary>
    /// 生成趨勢分析 - 新增方法
    /// </summary>
    private async Task<List<TrendAnalysisDto>> GenerateTrendAnalysisAsync(int gameId, string metric, DateTime startDate, DateTime endDate)
    {
        var analysis = new List<TrendAnalysisDto>();

        try
        {
            var metrics = await _context.GameMetricDailies
                .Include(m => m.Metric)
                .Where(m => m.game_id == gameId && m.Metric.code == metric && m.date >= startDate && m.date <= endDate)
                .OrderBy(m => m.date)
                .AsNoTracking()
                .ToListAsync();

            if (!metrics.Any()) return analysis;

            // 按日期分組計算平均值
            var dailyAverages = metrics.GroupBy(m => m.date)
                .Select(g => new { Date = g.Key, Value = g.Average(m => m.value) })
                .OrderBy(x => x.Date)
                .ToList();

            // 計算移動平均
            var movingAverage = CalculateMovingAverage(dailyAverages.Select(x => x.Value).ToList(), 3);

            for (int i = 0; i < dailyAverages.Count; i++)
            {
                var trendPoint = new TrendAnalysisDto
                {
                    Date = dailyAverages[i].Date,
                    Value = dailyAverages[i].Value,
                    MovingAverage = i < movingAverage.Count ? movingAverage[i] : 0,
                    Trend = i > 0 ? CalculateTrendPercentage(dailyAverages[i - 1].Value, dailyAverages[i].Value) : 0
                };

                analysis.Add(trendPoint);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成趨勢分析時發生錯誤");
        }

        return analysis;
    }

    /// <summary>
    /// 生成比較分析 - 新增方法
    /// </summary>
    private async Task<List<ComparativeAnalysisDto>> GenerateComparativeAnalysisAsync(int gameId1, int gameId2, DateTime startDate, DateTime endDate)
    {
        var analysis = new List<ComparativeAnalysisDto>();

        try
        {
            // 並行獲取兩個遊戲的指標數據
            var tasks = new[]
            {
                _context.GameMetricDailies
                    .Include(m => m.Metric)
                    .Where(m => m.game_id == gameId1 && m.date >= startDate && m.date <= endDate)
                    .AsNoTracking()
                    .ToListAsync(),
                _context.GameMetricDailies
                    .Include(m => m.Metric)
                    .Where(m => m.game_id == gameId2 && m.date >= startDate && m.date <= endDate)
                    .AsNoTracking()
                    .ToListAsync()
            };

            var results = await Task.WhenAll(tasks);
            var game1Metrics = results[0];
            var game2Metrics = results[1];

            if (!game1Metrics.Any() || !game2Metrics.Any()) return analysis;

            // 按指標和日期分組比較
            var metricGroups = game1Metrics.GroupBy(m => m.metric_id).ToList();

            foreach (var group in metricGroups)
            {
                var metric = group.First().Metric;
                var game1Data = group.ToList();
                var game2Data = game2Metrics.Where(m => m.metric_id == metric.metric_id).ToList();

                if (!game2Data.Any()) continue;

                // 按日期分組計算平均值
                var dates = game1Data.Select(m => m.date).Union(game2Data.Select(m => m.date)).OrderBy(d => d).ToList();

                foreach (var date in dates)
                {
                    var game1Value = game1Data.Where(m => m.date == date).Average(m => m.value);
                    var game2Value = game2Data.Where(m => m.date == date).Average(m => m.value);

                    var comparison = new ComparativeAnalysisDto
                    {
                        Date = date,
                        MetricCode = metric.code,
                        MetricName = metric.name,
                        Game1Value = game1Value,
                        Game2Value = game2Value,
                        Difference = game1Value - game2Value,
                        DifferencePercentage = game2Value != 0 ? (game1Value - game2Value) / game2Value * 100 : 0
                    };

                    analysis.Add(comparison);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成比較分析時發生錯誤");
        }

        return analysis;
    }

    /// <summary>
    /// 生成指標相關性分析 - 新增方法
    /// </summary>
    private async Task<List<MetricCorrelationDto>> GenerateMetricCorrelationAsync(int gameId, string metric1, string metric2, DateTime startDate, DateTime endDate)
    {
        var analysis = new List<MetricCorrelationDto>();

        try
        {
            var metrics = await _context.GameMetricDailies
                .Include(m => m.Metric)
                .Where(m => m.game_id == gameId && 
                           (m.Metric.code == metric1 || m.Metric.code == metric2) && 
                           m.date >= startDate && m.date <= endDate)
                .OrderBy(m => m.date)
                .AsNoTracking()
                .ToListAsync();

            if (!metrics.Any()) return analysis;

            // 按日期分組，獲取兩個指標的值
            var dailyData = metrics.GroupBy(m => m.date)
                .Select(g => new
                {
                    Date = g.Key,
                    Metric1Value = g.Where(m => m.Metric.code == metric1).Average(m => m.value),
                    Metric2Value = g.Where(m => m.Metric.code == metric2).Average(m => m.value)
                })
                .Where(x => x.Metric1Value > 0 && x.Metric2Value > 0) // 過濾無效數據
                .OrderBy(x => x.Date)
                .ToList();

            if (dailyData.Count < 3) return analysis; // 至少需要3個數據點

            // 計算相關性係數
            var correlation = CalculateCorrelationCoefficient(
                dailyData.Select(x => x.Metric1Value).ToList(),
                dailyData.Select(x => x.Metric2Value).ToList()
            );

            // 創建相關性分析結果
            var correlationAnalysis = new MetricCorrelationDto
            {
                Date = endDate,
                Metric1Code = metric1,
                Metric2Code = metric2,
                CorrelationCoefficient = correlation,
                CorrelationStrength = DetermineCorrelationStrength(correlation),
                DataPoints = dailyData.Count,
                Trend = correlation > 0 ? "positive" : correlation < 0 ? "negative" : "none"
            };

            analysis.Add(correlationAnalysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成指標相關性分析時發生錯誤");
        }

        return analysis;
    }

    /// <summary>
    /// 計算趨勢百分比 - 新增方法
    /// </summary>
    private decimal CalculateTrendPercentage(decimal previousValue, decimal currentValue)
    {
        if (previousValue == 0) return 0;
        return (currentValue - previousValue) / previousValue * 100;
    }

    /// <summary>
    /// 計算趨勢 - 新增方法
    /// </summary>
    private decimal CalculateTrend(List<decimal> values)
    {
        if (values.Count < 2) return 0;

        var firstValue = values.First();
        var lastValue = values.Last();

        if (firstValue == 0) return 0;
        return (lastValue - firstValue) / firstValue * 100;
    }

    /// <summary>
    /// 確定趨勢類型 - 新增方法
    /// </summary>
    private string DetermineTrendType(decimal trend)
    {
        if (trend > 5) return "上升";
        if (trend < -5) return "下降";
        return "穩定";
    }

    /// <summary>
    /// 計算標準差 - 新增方法
    /// </summary>
    private decimal CalculateStandardDeviation(List<decimal> values, decimal mean)
    {
        if (values.Count < 2) return 0;

        var sumSquaredDifferences = values.Sum(v => (v - mean) * (v - mean));
        var variance = sumSquaredDifferences / (values.Count - 1);
        return (decimal)Math.Sqrt((double)variance);
    }

    /// <summary>
    /// 計算移動平均 - 新增方法
    /// </summary>
    private List<decimal> CalculateMovingAverage(List<decimal> values, int windowSize)
    {
        var result = new List<decimal>();

        for (int i = 0; i < values.Count; i++)
        {
            var start = Math.Max(0, i - windowSize + 1);
            var count = Math.Min(windowSize, i + 1);
            var average = values.Skip(start).Take(count).Average();
            result.Add(average);
        }

        return result;
    }

    /// <summary>
    /// 計算相關性係數 - 新增方法
    /// </summary>
    private decimal CalculateCorrelationCoefficient(List<decimal> values1, List<decimal> values2)
    {
        if (values1.Count != values2.Count || values1.Count < 2) return 0;

        var n = values1.Count;
        var sum1 = values1.Sum();
        var sum2 = values2.Sum();
        var sum1Sq = values1.Sum(x => x * x);
        var sum2Sq = values2.Sum(x => x * x);
        var sumProduct = values1.Zip(values2, (a, b) => a * b).Sum();

        var numerator = n * sumProduct - sum1 * sum2;
        var denominator = (decimal)Math.Sqrt((double)((n * sum1Sq - sum1 * sum1) * (n * sum2Sq - sum2 * sum2)));

        return denominator != 0 ? numerator / denominator : 0;
    }

    /// <summary>
    /// 確定相關性強度 - 新增方法
    /// </summary>
    private string DetermineCorrelationStrength(decimal correlation)
    {
        var absCorrelation = Math.Abs(correlation);
        if (absCorrelation >= 0.8m) return "強";
        if (absCorrelation >= 0.5m) return "中等";
        if (absCorrelation >= 0.3m) return "弱";
        return "無";
    }

    /// <summary>
    /// 映射到洞察響應DTO - 新增方法
    /// </summary>
    private InsightResponseDto MapToInsightResponseDto(Insight insight)
    {
        return new InsightResponseDto
        {
            Id = insight.insight_id,
            GameId = insight.game_id,
            Type = insight.type,
            Title = insight.title,
            Description = insight.description,
            Data = !string.IsNullOrEmpty(insight.data) 
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(insight.data) ?? new Dictionary<string, object>()
                : new Dictionary<string, object>(),
            Date = insight.date,
            CreatedAt = insight.created_at
        };
    }

    #endregion
}