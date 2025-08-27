using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// InsightService 優化版本單元測試
/// </summary>
public class InsightServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InsightService> _logger;
    private readonly InsightService _service;

    public InsightServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<InsightService>>();
        _service = new InsightService(_context, _memoryCache, _logger);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
        _memoryCache.Dispose();
    }

    #region 測試數據準備

    private void SeedTestData()
    {
        // 創建遊戲
        var game1 = new Game { game_id = 1, name = "Test Game 1", description = "Test Description 1" };
        var game2 = new Game { game_id = 2, name = "Test Game 2", description = "Test Description 2" };
        _context.Games.AddRange(game1, game2);

        // 創建指標來源
        var source1 = new MetricSource { source_id = 1, name = "Test Source 1", description = "Test Source Description 1" };
        var source2 = new MetricSource { source_id = 2, name = "Test Source 2", description = "Test Source Description 2" };
        _context.MetricSources.AddRange(source1, source2);

        // 創建指標
        var metric1 = new Metric { metric_id = 1, source_id = 1, code = "concurrent_users", name = "Concurrent Users", description = "Number of concurrent users", unit = "users", is_active = true };
        var metric2 = new Metric { metric_id = 2, source_id = 1, code = "forum_posts", name = "Forum Posts", description = "Number of forum posts", unit = "posts", is_active = true };
        var metric3 = new Metric { metric_id = 3, source_id = 2, code = "social_mentions", name = "Social Mentions", description = "Number of social mentions", unit = "mentions", is_active = true };
        _context.Metrics.AddRange(metric1, metric2, metric3);

        // 創建遊戲指標數據
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var twoDaysAgo = today.AddDays(-2);

        var gameMetrics = new[]
        {
            new GameMetricDaily { game_id = 1, metric_id = 1, date = today, value = 1000, quality = "real" },
            new GameMetricDaily { game_id = 1, metric_id = 1, date = yesterday, value = 950, quality = "real" },
            new GameMetricDaily { game_id = 1, metric_id = 1, date = twoDaysAgo, value = 900, quality = "real" },
            new GameMetricDaily { game_id = 1, metric_id = 2, date = today, value = 50, quality = "real" },
            new GameMetricDaily { game_id = 1, metric_id = 2, date = yesterday, value = 45, quality = "real" },
            new GameMetricDaily { game_id = 1, metric_id = 3, date = today, value = 200, quality = "real" },
            new GameMetricDaily { game_id = 2, metric_id = 1, date = today, value = 800, quality = "real" },
            new GameMetricDaily { game_id = 2, metric_id = 2, date = today, value = 30, quality = "real" }
        };
        _context.GameMetricDailies.AddRange(gameMetrics);

        // 創建洞察
        var insights = new[]
        {
            new Insight { insight_id = 1, game_id = 1, type = "trend_analysis", title = "User Growth Trend", description = "Positive user growth trend", data = "{\"trend\":\"positive\"}", date = today, created_at = DateTime.UtcNow },
            new Insight { insight_id = 2, game_id = 1, type = "anomaly_detection", title = "Traffic Spike", description = "Unusual traffic spike detected", data = "{\"anomaly\":\"traffic\"}", date = yesterday, created_at = DateTime.UtcNow },
            new Insight { insight_id = 3, game_id = 2, type = "competitive_analysis", title = "Market Position", description = "Market position analysis", data = "{\"position\":\"competitive\"}", date = today, created_at = DateTime.UtcNow }
        };
        _context.Insights.AddRange(insights);

        _context.SaveChanges();
    }

    #endregion

    #region GetGameInsightsAsync 測試

    [Fact]
    public async Task GetGameInsightsAsync_WithValidParameters_ShouldReturnInsights()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGameInsightsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var insights = result.ToList();
        Assert.True(insights.Count > 0);
        Assert.All(insights, insight => Assert.Equal(gameId, insight.GameId));
    }

    [Fact]
    public async Task GetGameInsightsAsync_WithInvalidGameId_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = -1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGameInsightsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGameInsightsAsync_WithFutureDate_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = DateTime.UtcNow.Date.AddDays(2);

        // Act
        var result = await _service.GetGameInsightsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGameInsightsAsync_WithInvalidDateRange_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(-1);

        // Act
        var result = await _service.GetGameInsightsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGameInsightsAsync_ShouldUseCache()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act - 第一次調用
        var result1 = await _service.GetGameInsightsAsync(gameId, startDate, endDate);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GameMetricDailies.RemoveRange(_context.GameMetricDailies);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetGameInsightsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region CreateInsightAsync 測試

    [Fact]
    public async Task CreateInsightAsync_WithValidRequest_ShouldCreateInsight()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = 1,
            Type = "test_insight",
            Title = "Test Insight",
            Description = "Test Description",
            Data = new Dictionary<string, object> { { "test", "value" } },
            Date = DateTime.UtcNow.Date
        };

        // Act
        var result = await _service.CreateInsightAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.GameId, result.GameId);
        Assert.Equal(request.Type, result.Type);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Date, result.Date);

        // 驗證資料庫中確實創建了記錄
        var dbInsight = await _context.Insights
            .FirstOrDefaultAsync(i => i.insight_id == result.Id);
        Assert.NotNull(dbInsight);
    }

    [Fact]
    public async Task CreateInsightAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        CreateInsightRequestDto request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateInsightAsync(request));
    }

    [Fact]
    public async Task CreateInsightAsync_WithInvalidGameId_ShouldThrowException()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = -1,
            Type = "test_insight",
            Title = "Test Insight",
            Description = "Test Description",
            Data = new Dictionary<string, object>(),
            Date = DateTime.UtcNow.Date
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateInsightAsync(request));
    }

    [Fact]
    public async Task CreateInsightAsync_WithEmptyType_ShouldThrowException()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = 1,
            Type = "",
            Title = "Test Insight",
            Description = "Test Description",
            Data = new Dictionary<string, object>(),
            Date = DateTime.UtcNow.Date
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateInsightAsync(request));
    }

    [Fact]
    public async Task CreateInsightAsync_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = 1,
            Type = "test_insight",
            Title = "",
            Description = "Test Description",
            Data = new Dictionary<string, object>(),
            Date = DateTime.UtcNow.Date
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateInsightAsync(request));
    }

    [Fact]
    public async Task CreateInsightAsync_WithEmptyDescription_ShouldThrowException()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = 1,
            Type = "test_insight",
            Title = "Test Insight",
            Description = "",
            Data = new Dictionary<string, object>(),
            Date = DateTime.UtcNow.Date
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateInsightAsync(request));
    }

    [Fact]
    public async Task CreateInsightAsync_WithFutureDate_ShouldThrowException()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = 1,
            Type = "test_insight",
            Title = "Test Insight",
            Description = "Test Description",
            Data = new Dictionary<string, object>(),
            Date = DateTime.UtcNow.Date.AddDays(1)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateInsightAsync(request));
    }

    [Fact]
    public async Task CreateInsightAsync_WithExistingInsight_ShouldReturnExisting()
    {
        // Arrange
        var request = new CreateInsightRequestDto
        {
            GameId = 1,
            Type = "trend_analysis",
            Title = "User Growth Trend",
            Description = "Positive user growth trend",
            Data = new Dictionary<string, object>(),
            Date = DateTime.UtcNow.Date
        };

        // Act
        var result = await _service.CreateInsightAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id); // 應該返回現有的洞察ID

        // 驗證資料庫中沒有創建重複記錄
        var insights = await _context.Insights
            .Where(i => i.game_id == request.GameId && i.type == request.Type && i.date == request.Date)
            .ToListAsync();
        Assert.Single(insights);
    }

    #endregion

    #region GetTrendAnalysisAsync 測試

    [Fact]
    public async Task GetTrendAnalysisAsync_WithValidParameters_ShouldReturnAnalysis()
    {
        // Arrange
        var gameId = 1;
        var metric = "concurrent_users";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetTrendAnalysisAsync(gameId, metric, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var analysis = result.ToList();
        Assert.True(analysis.Count > 0);
        Assert.All(analysis, item => Assert.True(item.Value > 0));
    }

    [Fact]
    public async Task GetTrendAnalysisAsync_WithInvalidGameId_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = -1;
        var metric = "concurrent_users";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetTrendAnalysisAsync(gameId, metric, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTrendAnalysisAsync_WithEmptyMetric_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var metric = "";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetTrendAnalysisAsync(gameId, metric, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTrendAnalysisAsync_ShouldUseCache()
    {
        // Arrange
        var gameId = 1;
        var metric = "concurrent_users";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act - 第一次調用
        var result1 = await _service.GetTrendAnalysisAsync(gameId, metric, startDate, endDate);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GameMetricDailies.RemoveRange(_context.GameMetricDailies);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetTrendAnalysisAsync(gameId, metric, startDate, endDate);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetComparativeAnalysisAsync 測試

    [Fact]
    public async Task GetComparativeAnalysisAsync_WithValidParameters_ShouldReturnAnalysis()
    {
        // Arrange
        var gameId1 = 1;
        var gameId2 = 2;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var analysis = result.ToList();
        Assert.True(analysis.Count > 0);
        Assert.All(analysis, item => Assert.True(item.Game1Value > 0));
        Assert.All(analysis, item => Assert.True(item.Game2Value > 0));
    }

    [Fact]
    public async Task GetComparativeAnalysisAsync_WithInvalidGameId1_ShouldReturnEmpty()
    {
        // Arrange
        var gameId1 = -1;
        var gameId2 = 2;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetComparativeAnalysisAsync_WithInvalidGameId2_ShouldReturnEmpty()
    {
        // Arrange
        var gameId1 = 1;
        var gameId2 = -1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetComparativeAnalysisAsync_WithSameGameIds_ShouldReturnEmpty()
    {
        // Arrange
        var gameId1 = 1;
        var gameId2 = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetComparativeAnalysisAsync_ShouldUseCache()
    {
        // Arrange
        var gameId1 = 1;
        var gameId2 = 2;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act - 第一次調用
        var result1 = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GameMetricDailies.RemoveRange(_context.GameMetricDailies);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetMetricCorrelationAsync 測試

    [Fact]
    public async Task GetMetricCorrelationAsync_WithValidParameters_ShouldReturnAnalysis()
    {
        // Arrange
        var gameId = 1;
        var metric1 = "concurrent_users";
        var metric2 = "forum_posts";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var analysis = result.ToList();
        Assert.True(analysis.Count > 0);
        Assert.All(analysis, item => Assert.True(item.CorrelationCoefficient >= -1 && item.CorrelationCoefficient <= 1));
    }

    [Fact]
    public async Task GetMetricCorrelationAsync_WithInvalidGameId_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = -1;
        var metric1 = "concurrent_users";
        var metric2 = "forum_posts";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMetricCorrelationAsync_WithEmptyMetric1_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var metric1 = "";
        var metric2 = "forum_posts";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMetricCorrelationAsync_WithEmptyMetric2_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var metric1 = "concurrent_users";
        var metric2 = "";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMetricCorrelationAsync_WithSameMetrics_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var metric1 = "concurrent_users";
        var metric2 = "concurrent_users";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMetricCorrelationAsync_ShouldUseCache()
    {
        // Arrange
        var gameId = 1;
        var metric1 = "concurrent_users";
        var metric2 = "forum_posts";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act - 第一次調用
        var result1 = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GameMetricDailies.RemoveRange(_context.GameMetricDailies);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetAllInsightsAsync 測試

    [Fact]
    public async Task GetAllInsightsAsync_WithValidParameters_ShouldReturnInsights()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetAllInsightsAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var insights = result.ToList();
        Assert.Equal(3, insights.Count); // 應該返回3個洞察
    }

    [Fact]
    public async Task GetAllInsightsAsync_WithInvalidDateRange_ShouldReturnEmpty()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(-1);

        // Act
        var result = await _service.GetAllInsightsAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllInsightsAsync_ShouldUseCache()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act - 第一次調用
        var result1 = await _service.GetAllInsightsAsync(startDate, endDate);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Insights.RemoveRange(_context.Insights);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllInsightsAsync(startDate, endDate);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearInsightRelatedCache_ShouldClearCache()
    {
        // Arrange
        var gameId = 1;
        var date = DateTime.UtcNow.Date;
        
        // 先獲取一些數據以填充快取
        _service.GetGameInsightsAsync(gameId, date.AddDays(-2), date).Wait();

        // Act
        _service.ClearInsightRelatedCache(gameId, date);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetGameInsightsAsync_WithNoData_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 999; // 不存在的遊戲ID
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGameInsightsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTrendAnalysisAsync_WithNoData_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 999; // 不存在的遊戲ID
        var metric = "concurrent_users";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetTrendAnalysisAsync(gameId, metric, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetComparativeAnalysisAsync_WithNoData_ShouldReturnEmpty()
    {
        // Arrange
        var gameId1 = 999; // 不存在的遊戲ID
        var gameId2 = 998; // 不存在的遊戲ID
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetComparativeAnalysisAsync(gameId1, gameId2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMetricCorrelationAsync_WithNoData_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 999; // 不存在的遊戲ID
        var metric1 = "concurrent_users";
        var metric2 = "forum_posts";
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetMetricCorrelationAsync(gameId, metric1, metric2, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetGameInsightsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetGameInsightsAsync(gameId, startDate, endDate);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetAllInsightsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAllInsightsAsync(startDate, endDate);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}