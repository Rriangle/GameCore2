using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 排行榜服務測試
/// </summary>
public class LeaderboardServiceTests
{
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IMetricRepository> _mockMetricRepository;
    private readonly Mock<IGameMetricDailyRepository> _mockMetricDailyRepository;
    private readonly Mock<IPopularityIndexDailyRepository> _mockPopularityRepository;
    private readonly Mock<ILeaderboardSnapshotRepository> _mockLeaderboardRepository;
    private readonly LeaderboardService _leaderboardService;

    public LeaderboardServiceTests()
    {
        _mockGameRepository = new Mock<IGameRepository>();
        _mockMetricRepository = new Mock<IMetricRepository>();
        _mockMetricDailyRepository = new Mock<IGameMetricDailyRepository>();
        _mockPopularityRepository = new Mock<IPopularityIndexDailyRepository>();
        _mockLeaderboardRepository = new Mock<ILeaderboardSnapshotRepository>();
        _leaderboardService = new LeaderboardService(_mockGameRepository.Object, _mockMetricRepository.Object, _mockMetricDailyRepository.Object, _mockPopularityRepository.Object, _mockLeaderboardRepository.Object);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithValidParameters_ReturnsLeaderboardList()
    {
        // Arrange
        var searchParams = new LeaderboardSearchDto
        {
            Category = "動作遊戲",
            Period = "daily",
            Top = 10,
            SortBy = "popularity",
            SortOrder = "desc"
        };

        var popularityData = new List<PopularityIndexDaily>
        {
            new PopularityIndexDaily
            {
                Game_ID = 1,
                Period = "daily",
                Popularity_Index = 95.5m,
                Concurrent_Users = 1000,
                Forum_Posts = 150,
                Market_Listings = 50,
                Average_Rating = 4.8m,
                Snapshot_Date = DateTime.UtcNow.Date,
                Game = new Game { Game_ID = 1, Game_Name = "動作遊戲大作", Category = "動作遊戲" }
            },
            new PopularityIndexDaily
            {
                Game_ID = 2,
                Period = "daily",
                Popularity_Index = 85.2m,
                Concurrent_Users = 800,
                Forum_Posts = 120,
                Market_Listings = 40,
                Average_Rating = 4.5m,
                Snapshot_Date = DateTime.UtcNow.Date,
                Game = new Game { Game_ID = 2, Game_Name = "射擊遊戲", Category = "動作遊戲" }
            }
        };

        var mockQueryable = popularityData.AsQueryable();
        _mockPopularityRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _leaderboardService.GetLeaderboardAsync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Rank);
        Assert.Equal(1, result[0].GameID);
        Assert.Equal("動作遊戲大作", result[0].GameName);
        Assert.Equal(95.5m, result[0].PopularityScore);
        Assert.Equal(1000, result[0].ConcurrentUsers);
        Assert.Equal(150, result[0].TotalPosts);
        Assert.Equal(50, result[0].TotalViews);
        Assert.Equal(4.8m, result[0].AverageRating);
    }

    [Fact]
    public async Task GetGameMetricsAsync_WithValidGameId_ReturnsMetricsList()
    {
        // Arrange
        var gameId = 1;
        var date = DateTime.UtcNow.Date;

        var metrics = new List<GameMetricDaily>
        {
            new GameMetricDaily
            {
                Game_ID = gameId,
                Metric_ID = 1,
                Value = 1000,
                Date = date,
                Metric = new Metric { Code = "concurrent_users", Description = "同時在線人數", Unit = "人" },
                Game = new Game { Game_ID = gameId, Game_Name = "動作遊戲大作" }
            },
            new GameMetricDaily
            {
                Game_ID = gameId,
                Metric_ID = 2,
                Value = 150,
                Date = date,
                Metric = new Metric { Code = "forum_posts", Description = "論壇發文數", Unit = "篇" },
                Game = new Game { Game_ID = gameId, Game_Name = "動作遊戲大作" }
            }
        };

        var mockQueryable = metrics.AsQueryable();
        _mockMetricDailyRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _leaderboardService.GetGameMetricsAsync(gameId, date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(gameId, result[0].GameID);
        Assert.Equal("動作遊戲大作", result[0].GameName);
        Assert.Equal("concurrent_users", result[0].MetricCode);
        Assert.Equal("同時在線人數", result[0].MetricName);
        Assert.Equal(1000, result[0].Value);
        Assert.Equal("人", result[0].Unit);
        Assert.Equal(date, result[0].Date);
    }

    [Fact]
    public async Task GetGameTrendAsync_WithValidParameters_ReturnsTrendData()
    {
        // Arrange
        var gameId = 1;
        var days = 7;
        var metricCode = "concurrent_users";

        var game = new Game { Game_ID = gameId, Game_Name = "動作遊戲大作" };
        _mockGameRepository.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync(game);

        var metric = new Metric { MetricID = 1, Code = metricCode };
        _mockMetricRepository.Setup(x => x.GetQueryable()).Returns(new List<Metric> { metric }.AsQueryable());

        var dataPoints = new List<GameMetricDaily>();
        for (int i = 0; i < days; i++)
        {
            dataPoints.Add(new GameMetricDaily
            {
                Game_ID = gameId,
                Metric_ID = 1,
                Value = 1000 - (i * 50),
                Date = DateTime.UtcNow.Date.AddDays(-i)
            });
        }

        var mockQueryable = dataPoints.AsQueryable();
        _mockMetricDailyRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _leaderboardService.GetGameTrendAsync(gameId, days, metricCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameId, result.GameID);
        Assert.Equal("動作遊戲大作", result.GameName);
        Assert.Equal(days, result.DataPoints.Count);
        Assert.Equal(1000, result.DataPoints[0].Value);
        Assert.Equal(950, result.DataPoints[1].Value);
    }

    [Fact]
    public async Task GetGameTrendAsync_WithInvalidGameId_ReturnsNull()
    {
        // Arrange
        var gameId = 999;
        var days = 7;
        var metricCode = "concurrent_users";

        _mockGameRepository.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

        // Act
        var result = await _leaderboardService.GetGameTrendAsync(gameId, days, metricCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCategoryLeaderboardAsync_WithValidCategory_ReturnsCategoryLeaderboard()
    {
        // Arrange
        var category = "動作遊戲";
        var top = 5;

        var popularityData = new List<PopularityIndexDaily>
        {
            new PopularityIndexDaily
            {
                Game_ID = 1,
                Period = "daily",
                Popularity_Index = 95.5m,
                Concurrent_Users = 1000,
                Forum_Posts = 150,
                Market_Listings = 50,
                Average_Rating = 4.8m,
                Snapshot_Date = DateTime.UtcNow.Date,
                Game = new Game { Game_ID = 1, Game_Name = "動作遊戲大作", Category = category }
            }
        };

        var mockQueryable = popularityData.AsQueryable();
        _mockPopularityRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        var gamesQueryable = new List<Game> { new Game { Game_ID = 1, Category = category } }.AsQueryable();
        _mockGameRepository.Setup(x => x.GetQueryable()).Returns(gamesQueryable);

        // Act
        var result = await _leaderboardService.GetCategoryLeaderboardAsync(category, top);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category, result.Category);
        Assert.Equal(1, result.TotalGames);
        Assert.Single(result.TopGames);
        Assert.Equal(1, result.TopGames[0].GameID);
        Assert.Equal("動作遊戲大作", result.TopGames[0].GameName);
        Assert.Equal(95.5m, result.TopGames[0].PopularityScore);
    }

    [Fact]
    public async Task GetCategoryLeaderboardAsync_WithEmptyCategory_ReturnsNull()
    {
        // Arrange
        var category = "不存在的分類";
        var top = 5;

        var mockQueryable = new List<PopularityIndexDaily>().AsQueryable();
        _mockPopularityRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _leaderboardService.GetCategoryLeaderboardAsync(category, top);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePopularityIndexAsync_WithValidGameId_ReturnsTrue()
    {
        // Arrange
        var gameId = 1;
        var game = new Game { Game_ID = gameId, Game_Name = "動作遊戲大作", Category = "動作遊戲" };
        _mockGameRepository.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync(game);

        var mockQueryable = new List<GameMetricDaily>().AsQueryable();
        _mockMetricDailyRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        var mockPopularityQueryable = new List<PopularityIndexDaily>().AsQueryable();
        _mockPopularityRepository.Setup(x => x.GetQueryable()).Returns(mockPopularityQueryable);

        _mockPopularityRepository.Setup(x => x.AddAsync(It.IsAny<PopularityIndexDaily>())).ReturnsAsync(new PopularityIndexDaily());

        // Act
        var result = await _leaderboardService.UpdatePopularityIndexAsync(gameId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdatePopularityIndexAsync_WithInvalidGameId_ReturnsFalse()
    {
        // Arrange
        var gameId = 999;
        _mockGameRepository.Setup(x => x.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

        // Act
        var result = await _leaderboardService.UpdatePopularityIndexAsync(gameId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetLeaderboardAsync_WithSortByUsers_OrdersByConcurrentUsers()
    {
        // Arrange
        var searchParams = new LeaderboardSearchDto
        {
            SortBy = "users",
            SortOrder = "desc",
            Top = 10
        };

        var popularityData = new List<PopularityIndexDaily>
        {
            new PopularityIndexDaily
            {
                Game_ID = 1,
                Period = "daily",
                Popularity_Index = 80.0m,
                Concurrent_Users = 1200,
                Forum_Posts = 100,
                Market_Listings = 30,
                Average_Rating = 4.2m,
                Snapshot_Date = DateTime.UtcNow.Date,
                Game = new Game { Game_ID = 1, Game_Name = "遊戲1", Category = "動作遊戲" }
            },
            new PopularityIndexDaily
            {
                Game_ID = 2,
                Period = "daily",
                Popularity_Index = 90.0m,
                Concurrent_Users = 800,
                Forum_Posts = 150,
                Market_Listings = 50,
                Average_Rating = 4.8m,
                Snapshot_Date = DateTime.UtcNow.Date,
                Game = new Game { Game_ID = 2, Game_Name = "遊戲2", Category = "動作遊戲" }
            }
        };

        var mockQueryable = popularityData.AsQueryable();
        _mockPopularityRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _leaderboardService.GetLeaderboardAsync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].GameID); // 應該按照同時在線人數排序，1200 > 800
        Assert.Equal(1200, result[0].ConcurrentUsers);
        Assert.Equal(2, result[1].GameID);
        Assert.Equal(800, result[1].ConcurrentUsers);
    }
} 