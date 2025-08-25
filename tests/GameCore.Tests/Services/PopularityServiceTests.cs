using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services
{
    /// <summary>
    /// 熱度服務單元測試
    /// </summary>
    public class PopularityServiceTests
    {
        private readonly DbContextOptions<GameCoreDbContext> _options;
        private readonly Mock<ILogger<PopularityService>> _mockLogger;

        public PopularityServiceTests()
        {
            _options = new DbContextOptionsBuilder<GameCoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockLogger = new Mock<ILogger<PopularityService>>();
        }

        /// <summary>
        /// 測試獲取所有遊戲
        /// </summary>
        [Fact]
        public async Task GetAllGamesAsync_ShouldReturnAllGames()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var games = new List<Game>
            {
                new Game { game_id = 1, name = "英雄聯盟", genre = "MOBA", created_at = DateTime.UtcNow },
                new Game { game_id = 2, name = "原神", genre = "RPG", created_at = DateTime.UtcNow },
                new Game { game_id = 3, name = "Steam", genre = "Platform", created_at = DateTime.UtcNow }
            };

            context.Games.AddRange(games);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllGamesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, g => g.name == "英雄聯盟");
            Assert.Contains(result, g => g.name == "原神");
            Assert.Contains(result, g => g.name == "Steam");
        }

        /// <summary>
        /// 測試獲取遊戲熱度指數
        /// </summary>
        [Fact]
        public async Task GetGamePopularityAsync_ShouldReturnGamePopularity()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var game = new Game { game_id = 1, name = "英雄聯盟", genre = "MOBA", created_at = DateTime.UtcNow };
            context.Games.Add(game);

            var popularityData = new List<PopularityIndexDaily>
            {
                new PopularityIndexDaily
                {
                    id = 1,
                    game_id = 1,
                    date = DateTime.Today.AddDays(-2),
                    index_value = 85.5m,
                    created_at = DateTime.UtcNow
                },
                new PopularityIndexDaily
                {
                    id = 2,
                    game_id = 1,
                    date = DateTime.Today.AddDays(-1),
                    index_value = 87.2m,
                    created_at = DateTime.UtcNow
                },
                new PopularityIndexDaily
                {
                    id = 3,
                    game_id = 1,
                    date = DateTime.Today,
                    index_value = 89.1m,
                    created_at = DateTime.UtcNow
                }
            };

            context.PopularityIndexDailies.AddRange(popularityData);
            await context.SaveChangesAsync();

            // Act
            var startDate = DateTime.Today.AddDays(-2);
            var endDate = DateTime.Today;
            var result = await service.GetGamePopularityAsync(1, startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.All(result, p => Assert.Equal(1, p.game_id));
            Assert.Equal(85.5m, result.First().index_value);
            Assert.Equal(89.1m, result.Last().index_value);
        }

        /// <summary>
        /// 測試獲取排行榜快照
        /// </summary>
        [Fact]
        public async Task GetLeaderboardAsync_ShouldReturnLeaderboard()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var games = new List<Game>
            {
                new Game { game_id = 1, name = "英雄聯盟", genre = "MOBA", created_at = DateTime.UtcNow },
                new Game { game_id = 2, name = "原神", genre = "RPG", created_at = DateTime.UtcNow },
                new Game { game_id = 3, name = "Steam", genre = "Platform", created_at = DateTime.UtcNow }
            };
            context.Games.AddRange(games);

            var timestamp = DateTime.UtcNow;
            var leaderboardData = new List<LeaderboardSnapshot>
            {
                new LeaderboardSnapshot
                {
                    snapshot_id = 1,
                    period = "weekly",
                    ts = timestamp,
                    rank = 1,
                    game_id = 1,
                    index_value = 95.5m,
                    created_at = DateTime.UtcNow
                },
                new LeaderboardSnapshot
                {
                    snapshot_id = 2,
                    period = "weekly",
                    ts = timestamp,
                    rank = 2,
                    game_id = 2,
                    index_value = 88.2m,
                    created_at = DateTime.UtcNow
                },
                new LeaderboardSnapshot
                {
                    snapshot_id = 3,
                    period = "weekly",
                    ts = timestamp,
                    rank = 3,
                    game_id = 3,
                    index_value = 82.1m,
                    created_at = DateTime.UtcNow
                }
            };

            context.LeaderboardSnapshots.AddRange(leaderboardData);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetLeaderboardAsync("weekly");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Equal(1, result.First().rank);
            Assert.Equal(3, result.Last().rank);
            Assert.Equal("英雄聯盟", result.First().Game?.name);
            Assert.Equal(95.5m, result.First().index_value);
        }

        /// <summary>
        /// 測試計算遊戲熱度指數
        /// </summary>
        [Fact]
        public async Task CalculatePopularityIndexAsync_ShouldCalculateAndSaveIndex()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var game = new Game { game_id = 1, name = "英雄聯盟", genre = "MOBA", created_at = DateTime.UtcNow };
            context.Games.Add(game);

            var metricSource = new MetricSource { source_id = 1, name = "Steam", note = "Steam平台", created_at = DateTime.UtcNow };
            context.MetricSources.Add(metricSource);

            var metric = new Metric
            {
                metric_id = 1,
                source_id = 1,
                code = "concurrent_users",
                unit = "users",
                description = "同時在線用戶數",
                is_active = true,
                created_at = DateTime.UtcNow
            };
            context.Metrics.Add(metric);

            var gameMetric = new GameMetricDaily
            {
                id = 1,
                game_id = 1,
                metric_id = 1,
                date = DateTime.Today,
                value = 1000000m,
                agg_method = "max",
                quality = "real",
                created_at = DateTime.UtcNow
            };
            context.GameMetricDailies.Add(gameMetric);

            await context.SaveChangesAsync();

            // Act
            await service.CalculatePopularityIndexAsync(1, DateTime.Today);

            // Assert
            var savedIndex = await context.PopularityIndexDailies
                .FirstOrDefaultAsync(p => p.game_id == 1 && p.date == DateTime.Today);

            Assert.NotNull(savedIndex);
            Assert.Equal(1, savedIndex.game_id);
            Assert.Equal(DateTime.Today, savedIndex.date);
            Assert.True(savedIndex.index_value > 0);
        }

        /// <summary>
        /// 測試生成排行榜快照
        /// </summary>
        [Fact]
        public async Task GenerateLeaderboardSnapshotAsync_ShouldGenerateSnapshot()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var games = new List<Game>
            {
                new Game { game_id = 1, name = "英雄聯盟", genre = "MOBA", created_at = DateTime.UtcNow },
                new Game { game_id = 2, name = "原神", genre = "RPG", created_at = DateTime.UtcNow }
            };
            context.Games.AddRange(games);

            var popularityData = new List<PopularityIndexDaily>
            {
                new PopularityIndexDaily
                {
                    id = 1,
                    game_id = 1,
                    date = DateTime.Today,
                    index_value = 95.5m,
                    created_at = DateTime.UtcNow
                },
                new PopularityIndexDaily
                {
                    id = 2,
                    game_id = 2,
                    date = DateTime.Today,
                    index_value = 88.2m,
                    created_at = DateTime.UtcNow
                }
            };
            context.PopularityIndexDailies.AddRange(popularityData);
            await context.SaveChangesAsync();

            // Act
            var timestamp = DateTime.UtcNow;
            await service.GenerateLeaderboardSnapshotAsync("weekly", timestamp);

            // Assert
            var snapshots = await context.LeaderboardSnapshots
                .Where(l => l.period == "weekly" && l.ts == timestamp)
                .ToListAsync();

            Assert.Equal(2, snapshots.Count);
            Assert.Equal(1, snapshots.First().rank);
            Assert.Equal(2, snapshots.Last().rank);
            Assert.Equal(1, snapshots.First().game_id);
            Assert.Equal(2, snapshots.Last().game_id);
        }

        /// <summary>
        /// 測試獲取指標來源
        /// </summary>
        [Fact]
        public async Task GetMetricSourcesAsync_ShouldReturnMetricSources()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var sources = new List<MetricSource>
            {
                new MetricSource { source_id = 1, name = "Steam", note = "Steam平台", created_at = DateTime.UtcNow },
                new MetricSource { source_id = 2, name = "YouTube", note = "YouTube平台", created_at = DateTime.UtcNow }
            };
            context.MetricSources.AddRange(sources);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetMetricSourcesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.name == "Steam");
            Assert.Contains(result, s => s.name == "YouTube");
        }

        /// <summary>
        /// 測試獲取指標定義
        /// </summary>
        [Fact]
        public async Task GetMetricsAsync_ShouldReturnMetrics()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var source = new MetricSource { source_id = 1, name = "Steam", note = "Steam平台", created_at = DateTime.UtcNow };
            context.MetricSources.Add(source);

            var metrics = new List<Metric>
            {
                new Metric
                {
                    metric_id = 1,
                    source_id = 1,
                    code = "concurrent_users",
                    unit = "users",
                    description = "同時在線用戶數",
                    is_active = true,
                    created_at = DateTime.UtcNow
                },
                new Metric
                {
                    metric_id = 2,
                    source_id = 1,
                    code = "forum_posts",
                    unit = "posts",
                    description = "論壇發文數",
                    is_active = true,
                    created_at = DateTime.UtcNow
                }
            };
            context.Metrics.AddRange(metrics);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetMetricsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.code == "concurrent_users");
            Assert.Contains(result, m => m.code == "forum_posts");
        }

        /// <summary>
        /// 測試獲取指標定義（按來源篩選）
        /// </summary>
        [Fact]
        public async Task GetMetricsAsync_WithSourceId_ShouldReturnFilteredMetrics()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // 添加測試數據
            var sources = new List<MetricSource>
            {
                new MetricSource { source_id = 1, name = "Steam", note = "Steam平台", created_at = DateTime.UtcNow },
                new MetricSource { source_id = 2, name = "YouTube", note = "YouTube平台", created_at = DateTime.UtcNow }
            };
            context.MetricSources.AddRange(sources);

            var metrics = new List<Metric>
            {
                new Metric
                {
                    metric_id = 1,
                    source_id = 1,
                    code = "concurrent_users",
                    unit = "users",
                    description = "同時在線用戶數",
                    is_active = true,
                    created_at = DateTime.UtcNow
                },
                new Metric
                {
                    metric_id = 2,
                    source_id = 2,
                    code = "view_count",
                    unit = "views",
                    description = "觀看次數",
                    is_active = true,
                    created_at = DateTime.UtcNow
                }
            };
            context.Metrics.AddRange(metrics);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetMetricsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().source_id);
            Assert.Equal("concurrent_users", result.First().code);
        }

        /// <summary>
        /// 測試空數據情況
        /// </summary>
        [Fact]
        public async Task GetAllGamesAsync_WithEmptyData_ShouldReturnEmptyList()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // Act
            var result = await service.GetAllGamesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// 測試無效遊戲ID的情況
        /// </summary>
        [Fact]
        public async Task GetGamePopularityAsync_WithInvalidGameId_ShouldReturnEmptyList()
        {
            // Arrange
            using var context = new GameCoreDbContext(_options);
            var service = new PopularityService(context, _mockLogger.Object);

            // Act
            var result = await service.GetGamePopularityAsync(999, DateTime.Today.AddDays(-7), DateTime.Today);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}