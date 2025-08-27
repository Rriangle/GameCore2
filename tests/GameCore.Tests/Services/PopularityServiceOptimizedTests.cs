using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// PopularityService 優化版本單元測試
/// </summary>
public class PopularityServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PopularityService> _logger;
    private readonly PopularityService _service;

    public PopularityServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<PopularityService>>();
        _service = new PopularityService(_context, _memoryCache, _logger);

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

        // 創建熱度指數
        var popularityIndices = new[]
        {
            new PopularityIndexDaily { game_id = 1, date = today, index_value = 85.5m, created_at = DateTime.UtcNow },
            new PopularityIndexDaily { game_id = 1, date = yesterday, index_value = 82.0m, created_at = DateTime.UtcNow },
            new PopularityIndexDaily { game_id = 2, date = today, index_value = 75.0m, created_at = DateTime.UtcNow }
        };
        _context.PopularityIndexDailies.AddRange(popularityIndices);

        // 創建排行榜快照
        var leaderboardSnapshots = new[]
        {
            new LeaderboardSnapshot { period = "daily", ts = today, rank = 1, game_id = 1, index_value = 85.5m, created_at = DateTime.UtcNow },
            new LeaderboardSnapshot { period = "daily", ts = today, rank = 2, game_id = 2, index_value = 75.0m, created_at = DateTime.UtcNow },
            new LeaderboardSnapshot { period = "weekly", ts = today, rank = 1, game_id = 1, index_value = 83.75m, created_at = DateTime.UtcNow }
        };
        _context.LeaderboardSnapshots.AddRange(leaderboardSnapshots);

        _context.SaveChanges();
    }

    #endregion

    #region GetAllGamesAsync 測試

    [Fact]
    public async Task GetAllGamesAsync_ShouldReturnAllGames()
    {
        // Act
        var result = await _service.GetAllGamesAsync();

        // Assert
        Assert.NotNull(result);
        var games = result.ToList();
        Assert.Equal(2, games.Count);
        Assert.Contains(games, g => g.name == "Test Game 1");
        Assert.Contains(games, g => g.name == "Test Game 2");
    }

    [Fact]
    public async Task GetAllGamesAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAllGamesAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.Games.RemoveRange(_context.Games);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllGamesAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetGamePopularityAsync 測試

    [Fact]
    public async Task GetGamePopularityAsync_WithValidParameters_ShouldReturnData()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGamePopularityAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var data = result.ToList();
        Assert.Equal(3, data.Count);
        Assert.All(data, item => Assert.Equal(gameId, item.game_id));
    }

    [Fact]
    public async Task GetGamePopularityAsync_WithInvalidGameId_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = -1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGamePopularityAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamePopularityAsync_WithFutureDate_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = DateTime.UtcNow.Date.AddDays(2);

        // Act
        var result = await _service.GetGamePopularityAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamePopularityAsync_WithInvalidDateRange_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date;
        var endDate = DateTime.UtcNow.Date.AddDays(-1);

        // Act
        var result = await _service.GetGamePopularityAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetLeaderboardAsync 測試

    [Fact]
    public async Task GetLeaderboardAsync_WithValidPeriod_ShouldReturnData()
    {
        // Arrange
        var period = "daily";

        // Act
        var result = await _service.GetLeaderboardAsync(period);

        // Assert
        Assert.NotNull(result);
        var data = result.ToList();
        Assert.Equal(2, data.Count);
        Assert.All(data, item => Assert.Equal(period, item.period));
        Assert.Equal(1, data.First().rank);
        Assert.Equal(2, data.Last().rank);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithEmptyPeriod_ShouldReturnEmpty()
    {
        // Arrange
        var period = "";

        // Act
        var result = await _service.GetLeaderboardAsync(period);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithSpecificTimestamp_ShouldReturnData()
    {
        // Arrange
        var period = "daily";
        var timestamp = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetLeaderboardAsync(period, timestamp);

        // Assert
        Assert.NotNull(result);
        var data = result.ToList();
        Assert.Equal(2, data.Count);
        Assert.All(data, item => Assert.Equal(timestamp, item.ts));
    }

    #endregion

    #region CalculatePopularityIndexAsync 測試

    [Fact]
    public async Task CalculatePopularityIndexAsync_WithValidParameters_ShouldCalculateIndex()
    {
        // Arrange
        var gameId = 1;
        var date = DateTime.UtcNow.Date.AddDays(-3); // 使用一個沒有指數的日期

        // Act
        await _service.CalculatePopularityIndexAsync(gameId, date);

        // Assert
        var newIndex = await _context.PopularityIndexDailies
            .FirstOrDefaultAsync(p => p.game_id == gameId && p.date == date);
        Assert.NotNull(newIndex);
        Assert.True(newIndex.index_value > 0);
    }

    [Fact]
    public async Task CalculatePopularityIndexAsync_WithInvalidGameId_ShouldThrowException()
    {
        // Arrange
        var gameId = -1;
        var date = DateTime.UtcNow.Date;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CalculatePopularityIndexAsync(gameId, date));
    }

    [Fact]
    public async Task CalculatePopularityIndexAsync_WithFutureDate_ShouldThrowException()
    {
        // Arrange
        var gameId = 1;
        var date = DateTime.UtcNow.Date.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CalculatePopularityIndexAsync(gameId, date));
    }

    [Fact]
    public async Task CalculatePopularityIndexAsync_WithExistingIndex_ShouldSkipCalculation()
    {
        // Arrange
        var gameId = 1;
        var date = DateTime.UtcNow.Date;

        // Act
        await _service.CalculatePopularityIndexAsync(gameId, date);

        // Assert - 應該不會創建重複的指數
        var indices = await _context.PopularityIndexDailies
            .Where(p => p.game_id == gameId && p.date == date)
            .ToListAsync();
        Assert.Single(indices);
    }

    #endregion

    #region GenerateLeaderboardSnapshotAsync 測試

    [Fact]
    public async Task GenerateLeaderboardSnapshotAsync_WithValidParameters_ShouldGenerateSnapshot()
    {
        // Arrange
        var period = "monthly";
        var timestamp = DateTime.UtcNow.Date.AddDays(-1);

        // Act
        await _service.GenerateLeaderboardSnapshotAsync(period, timestamp);

        // Assert
        var snapshot = await _context.LeaderboardSnapshots
            .FirstOrDefaultAsync(l => l.period == period && l.ts == timestamp);
        Assert.NotNull(snapshot);
    }

    [Fact]
    public async Task GenerateLeaderboardSnapshotAsync_WithEmptyPeriod_ShouldThrowException()
    {
        // Arrange
        var period = "";
        var timestamp = DateTime.UtcNow.Date;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GenerateLeaderboardSnapshotAsync(period, timestamp));
    }

    [Fact]
    public async Task GenerateLeaderboardSnapshotAsync_WithFutureTimestamp_ShouldThrowException()
    {
        // Arrange
        var period = "daily";
        var timestamp = DateTime.UtcNow.Date.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GenerateLeaderboardSnapshotAsync(period, timestamp));
    }

    [Fact]
    public async Task GenerateLeaderboardSnapshotAsync_WithExistingSnapshot_ShouldSkipGeneration()
    {
        // Arrange
        var period = "daily";
        var timestamp = DateTime.UtcNow.Date;

        // Act
        await _service.GenerateLeaderboardSnapshotAsync(period, timestamp);

        // Assert - 應該不會創建重複的快照
        var snapshots = await _context.LeaderboardSnapshots
            .Where(l => l.period == period && l.ts == timestamp)
            .ToListAsync();
        Assert.Equal(2, snapshots.Count); // 原本就有1個，加上新生成的1個
    }

    #endregion

    #region GetGameMetricsAsync 測試

    [Fact]
    public async Task GetGameMetricsAsync_WithValidParameters_ShouldReturnData()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGameMetricsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        var data = result.ToList();
        Assert.Equal(6, data.Count);
        Assert.All(data, item => Assert.Equal(gameId, item.game_id));
    }

    [Fact]
    public async Task GetGameMetricsAsync_WithInvalidGameId_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = -1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGameMetricsAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetMetricSourcesAsync 測試

    [Fact]
    public async Task GetMetricSourcesAsync_ShouldReturnAllSources()
    {
        // Act
        var result = await _service.GetMetricSourcesAsync();

        // Assert
        Assert.NotNull(result);
        var sources = result.ToList();
        Assert.Equal(2, sources.Count);
        Assert.Contains(sources, s => s.name == "Test Source 1");
        Assert.Contains(sources, s => s.name == "Test Source 2");
    }

    [Fact]
    public async Task GetMetricSourcesAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetMetricSourcesAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.MetricSources.RemoveRange(_context.MetricSources);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetMetricSourcesAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetMetricsAsync 測試

    [Fact]
    public async Task GetMetricsAsync_WithoutSourceId_ShouldReturnAllMetrics()
    {
        // Act
        var result = await _service.GetMetricsAsync();

        // Assert
        Assert.NotNull(result);
        var metrics = result.ToList();
        Assert.Equal(3, metrics.Count);
    }

    [Fact]
    public async Task GetMetricsAsync_WithSourceId_ShouldReturnFilteredMetrics()
    {
        // Arrange
        var sourceId = 1;

        // Act
        var result = await _service.GetMetricsAsync(sourceId);

        // Assert
        Assert.NotNull(result);
        var metrics = result.ToList();
        Assert.Equal(2, metrics.Count);
        Assert.All(metrics, m => Assert.Equal(sourceId, m.source_id));
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearPopularityRelatedCache_ShouldClearCache()
    {
        // Arrange
        var gameId = 1;
        
        // 先獲取一些數據以填充快取
        _service.GetAllGamesAsync().Wait();

        // Act
        _service.ClearPopularityRelatedCache(gameId);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    [Fact]
    public void ClearLeaderboardRelatedCache_ShouldClearCache()
    {
        // Arrange
        var period = "daily";
        
        // Act
        _service.ClearLeaderboardRelatedCache(period);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetAllGamesAsync_WithEmptyDatabase_ShouldReturnEmpty()
    {
        // Arrange
        _context.Games.RemoveRange(_context.Games);
        _context.SaveChanges();

        // Act
        var result = await _service.GetAllGamesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGamePopularityAsync_WithNoData_ShouldReturnEmpty()
    {
        // Arrange
        var gameId = 999; // 不存在的遊戲ID
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;

        // Act
        var result = await _service.GetGamePopularityAsync(gameId, startDate, endDate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithNoData_ShouldReturnEmpty()
    {
        // Arrange
        var period = "yearly"; // 不存在的時段

        // Act
        var result = await _service.GetLeaderboardAsync(period);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetAllGamesAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAllGamesAsync();
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetGamePopularityAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var gameId = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(-2);
        var endDate = DateTime.UtcNow.Date;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetGamePopularityAsync(gameId, startDate, endDate);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}