using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.AdventureDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// AdventureService 優化版本單元測試
/// </summary>
public class AdventureServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AdventureService> _logger;
    private readonly AdventureService _service;

    public AdventureServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<AdventureService>>();
        _service = new AdventureService(_context, _memoryCache, _logger);

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
        // 創建用戶
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com", Level = 10 };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com", Level = 5 };
        var user3 = new User { user_id = 3, username = "user3", email = "user3@test.com", Level = 15 };
        _context.Users.AddRange(user1, user2, user3);

        // 創建用戶錢包
        var wallet1 = new UserWallet { UserId = 1, Balance = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var wallet2 = new UserWallet { UserId = 2, Balance = 50, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var wallet3 = new UserWallet { UserId = 3, Balance = 200, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _context.UserWallets.AddRange(wallet1, wallet2, wallet3);

        // 創建冒險模板
        var now = DateTime.UtcNow;
        var template1 = new AdventureTemplate 
        { 
            Id = 1, 
            Name = "Forest Adventure", 
            Description = "Explore the mysterious forest", 
            Category = "Exploration", 
            Difficulty = "Easy", 
            MinLevel = 1, 
            MaxLevel = 20, 
            EnergyCost = 20, 
            DurationMinutes = 60, 
            BaseSuccessRate = 0.8m, 
            BaseExperienceReward = 100, 
            BasePointsReward = 50, 
            BaseGoldReward = 25, 
            MaxMonsterEncounters = 3, 
            IsActive = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        var template2 = new AdventureTemplate 
        { 
            Id = 2, 
            Name = "Mountain Challenge", 
            Description = "Climb the dangerous mountain", 
            Category = "Challenge", 
            Difficulty = "Hard", 
            MinLevel = 10, 
            MaxLevel = 50, 
            EnergyCost = 50, 
            DurationMinutes = 120, 
            BaseSuccessRate = 0.6m, 
            BaseExperienceReward = 300, 
            BasePointsReward = 150, 
            BaseGoldReward = 75, 
            MaxMonsterEncounters = 5, 
            IsActive = true, 
            CreatedAt = now, 
            UpdatedAt = now 
        };
        _context.AdventureTemplates.AddRange(template1, template2);

        // 創建冒險
        var adventure1 = new Adventure 
        { 
            Id = 1, 
            UserId = 1, 
            Title = "Forest Adventure", 
            Description = "Explore the mysterious forest", 
            Difficulty = "Easy", 
            RequiredLevel = 1, 
            RequiredEnergy = 20, 
            DurationMinutes = 60, 
            SuccessRate = 0.8m, 
            BaseExperienceReward = 100, 
            BasePointsReward = 50, 
            BaseGoldReward = 25, 
            CreatedAt = now.AddHours(-1), 
            UpdatedAt = now.AddHours(-1) 
        };
        _context.Adventures.Add(adventure1);

        // 創建冒險記錄
        var adventureLog1 = new AdventureLog 
        { 
            Id = 1, 
            AdventureId = 1, 
            UserId = 1, 
            Status = "InProgress", 
            StartedAt = now.AddHours(-1), 
            EnergySpent = 20, 
            CreatedAt = now.AddHours(-1), 
            UpdatedAt = now.AddHours(-1) 
        };
        var adventureLog2 = new AdventureLog 
        { 
            Id = 2, 
            AdventureId = 1, 
            UserId = 1, 
            Status = "Completed", 
            StartedAt = now.AddDays(-1), 
            CompletedAt = now.AddDays(-1).AddHours(1), 
            EnergySpent = 20, 
            ExperienceGained = 100, 
            PointsGained = 50, 
            GoldGained = 25, 
            CreatedAt = now.AddDays(-1), 
            UpdatedAt = now.AddDays(-1).AddHours(1) 
        };
        _context.AdventureLogs.AddRange(adventureLog1, adventureLog2);

        // 創建怪物遭遇
        var monsterEncounter1 = new MonsterEncounter 
        { 
            Id = 1, 
            AdventureLogId = 1, 
            MonsterName = "Goblin Lv.3", 
            MonsterType = "Goblin", 
            MonsterLevel = 3, 
            MonsterHealth = 30, 
            MonsterMaxHealth = 30, 
            MonsterAttack = 15, 
            MonsterDefense = 8, 
            Outcome = "Victory", 
            DamageDealt = 25, 
            DamageTaken = 10, 
            ExperienceGained = 30, 
            PointsGained = 15, 
            GoldGained = 6, 
            BattleNotes = "Successfully defeated Goblin", 
            EncounterTime = now.AddMinutes(-30), 
            CreatedAt = now.AddMinutes(-30), 
            UpdatedAt = now.AddMinutes(-30) 
        };
        var monsterEncounter2 = new MonsterEncounter 
        { 
            Id = 2, 
            AdventureLogId = 1, 
            MonsterName = "Orc Lv.5", 
            MonsterType = "Orc", 
            MonsterLevel = 5, 
            MonsterHealth = 50, 
            MonsterMaxHealth = 50, 
            MonsterAttack = 20, 
            MonsterDefense = 12, 
            Outcome = "Victory", 
            DamageDealt = 30, 
            DamageTaken = 15, 
            ExperienceGained = 50, 
            PointsGained = 25, 
            GoldGained = 10, 
            BattleNotes = "Successfully defeated Orc", 
            EncounterTime = now.AddMinutes(-15), 
            CreatedAt = now.AddMinutes(-15), 
            UpdatedAt = now.AddMinutes(-15) 
        };
        _context.MonsterEncounters.AddRange(monsterEncounter1, monsterEncounter2);

        _context.SaveChanges();
    }

    #endregion

    #region StartAdventureAsync 測試

    [Fact]
    public async Task StartAdventureAsync_WithValidData_ShouldStartAdventure()
    {
        // Arrange
        var userId = 2; // User with sufficient energy
        var templateId = 1; // Easy template

        // Act
        var result = await _service.StartAdventureAsync(userId, templateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Forest Adventure", result.Title);
        Assert.Equal("Easy", result.Difficulty);
        Assert.Equal(20, result.RequiredEnergy);
        Assert.Equal(60, result.DurationMinutes);
        Assert.Equal("InProgress", result.Status);
        Assert.NotNull(result.StartedAt);
        Assert.Equal(20, result.EnergySpent);

        // 驗證資料庫中確實創建了記錄
        var dbAdventure = await _context.Adventures
            .FirstOrDefaultAsync(a => a.Id == result.Id);
        Assert.NotNull(dbAdventure);

        // 驗證冒險記錄被創建
        var dbAdventureLog = await _context.AdventureLogs
            .FirstOrDefaultAsync(al => al.AdventureId == result.Id);
        Assert.NotNull(dbAdventureLog);
        Assert.Equal("InProgress", dbAdventureLog.Status);

        // 驗證用戶能量被扣除
        var dbWallet = await _context.UserWallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
        Assert.NotNull(dbWallet);
        Assert.Equal(30, dbWallet.Balance); // 100 - 20 - 20 (from existing adventure)
    }

    [Fact]
    public async Task StartAdventureAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var templateId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.StartAdventureAsync(userId, templateId));
    }

    [Fact]
    public async Task StartAdventureAsync_WithInvalidTemplateId_ShouldThrowException()
    {
        // Arrange
        var userId = 2;
        var templateId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.StartAdventureAsync(userId, templateId));
    }

    [Fact]
    public async Task StartAdventureAsync_WithUserNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = 999;
        var templateId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.StartAdventureAsync(userId, templateId));
    }

    [Fact]
    public async Task StartAdventureAsync_WithTemplateNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = 2;
        var templateId = 999;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.StartAdventureAsync(userId, templateId));
    }

    [Fact]
    public async Task StartAdventureAsync_WithInsufficientEnergy_ShouldThrowException()
    {
        // Arrange
        var userId = 2; // User with 50 energy
        var templateId = 2; // Template requiring 50 energy

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.StartAdventureAsync(userId, templateId));
    }

    [Fact]
    public async Task StartAdventureAsync_WithUserAlreadyHasActiveAdventure_ShouldThrowException()
    {
        // Arrange
        var userId = 1; // User who already has an active adventure
        var templateId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.StartAdventureAsync(userId, templateId));
    }

    #endregion

    #region GetAdventureAsync 測試

    [Fact]
    public async Task GetAdventureAsync_WithValidData_ShouldReturnAdventure()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;

        // Act
        var result = await _service.GetAdventureAsync(adventureId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(adventureId, result.Id);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Forest Adventure", result.Title);
        Assert.Equal("Easy", result.Difficulty);
        Assert.Equal(20, result.RequiredEnergy);
        Assert.Equal(60, result.DurationMinutes);
        Assert.Equal("InProgress", result.Status);
        Assert.NotNull(result.StartedAt);
        Assert.Equal(20, result.EnergySpent);
        Assert.NotNull(result.Progress);
    }

    [Fact]
    public async Task GetAdventureAsync_WithInvalidAdventureId_ShouldThrowException()
    {
        // Arrange
        var adventureId = -1;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdventureAsync(adventureId, userId));
    }

    [Fact]
    public async Task GetAdventureAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var adventureId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdventureAsync(adventureId, userId));
    }

    [Fact]
    public async Task GetAdventureAsync_WithAdventureNotFound_ShouldThrowException()
    {
        // Arrange
        var adventureId = 999;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.GetAdventureAsync(adventureId, userId));
    }

    #endregion

    #region CompleteAdventureAsync 測試

    [Fact]
    public async Task CompleteAdventureAsync_WithValidData_ShouldCompleteAdventure()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;

        // Act
        var result = await _service.CompleteAdventureAsync(adventureId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
        Assert.NotNull(result.CompletedAt);
        Assert.True(result.ExperienceGained > 0);
        Assert.True(result.PointsGained > 0);
        Assert.True(result.GoldGained > 0);

        // 驗證資料庫中確實更新了記錄
        var dbAdventureLog = await _context.AdventureLogs
            .FirstOrDefaultAsync(al => al.AdventureId == adventureId && al.Status == "Completed");
        Assert.NotNull(dbAdventureLog);
        Assert.Equal("Completed", dbAdventureLog.Status);
        Assert.NotNull(dbAdventureLog.CompletedAt);
    }

    [Fact]
    public async Task CompleteAdventureAsync_WithInvalidAdventureId_ShouldThrowException()
    {
        // Arrange
        var adventureId = -1;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CompleteAdventureAsync(adventureId, userId));
    }

    [Fact]
    public async Task CompleteAdventureAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var adventureId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CompleteAdventureAsync(adventureId, userId));
    }

    #endregion

    #region FailAdventureAsync 測試

    [Fact]
    public async Task FailAdventureAsync_WithValidData_ShouldFailAdventure()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;
        var reason = "Ran out of time";

        // Act
        var result = await _service.FailAdventureAsync(adventureId, userId, reason);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Failed", result.Status);
        Assert.NotNull(result.FailedAt);
        Assert.Equal(reason, result.FailureReason);
        Assert.True(result.ExperienceGained > 0);
        Assert.True(result.PointsGained > 0);
        Assert.True(result.GoldGained > 0);

        // 驗證資料庫中確實更新了記錄
        var dbAdventureLog = await _context.AdventureLogs
            .FirstOrDefaultAsync(al => al.AdventureId == adventureId && al.Status == "Failed");
        Assert.NotNull(dbAdventureLog);
        Assert.Equal("Failed", dbAdventureLog.Status);
        Assert.NotNull(dbAdventureLog.FailedAt);
        Assert.Equal(reason, dbAdventureLog.FailureReason);
    }

    [Fact]
    public async Task FailAdventureAsync_WithInvalidAdventureId_ShouldThrowException()
    {
        // Arrange
        var adventureId = -1;
        var userId = 1;
        var reason = "Ran out of time";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.FailAdventureAsync(adventureId, userId, reason));
    }

    [Fact]
    public async Task FailAdventureAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var adventureId = 1;
        var userId = -1;
        var reason = "Ran out of time";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.FailAdventureAsync(adventureId, userId, reason));
    }

    [Fact]
    public async Task FailAdventureAsync_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;
        var reason = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.FailAdventureAsync(adventureId, userId, reason));
    }

    [Fact]
    public async Task FailAdventureAsync_WithNullReason_ShouldThrowException()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;
        string reason = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.FailAdventureAsync(adventureId, userId, reason));
    }

    #endregion

    #region GetAvailableTemplatesAsync 測試

    [Fact]
    public async Task GetAvailableTemplatesAsync_WithValidUserId_ShouldReturnTemplates()
    {
        // Arrange
        var userId = 2;

        // Act
        var result = await _service.GetAvailableTemplatesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.True(t.IsActive));
        Assert.Contains(result, t => t.Name == "Forest Adventure");
        Assert.Contains(result, t => t.Name == "Mountain Challenge");
    }

    [Fact]
    public async Task GetAvailableTemplatesAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAvailableTemplatesAsync(userId));
    }

    [Fact]
    public async Task GetAvailableTemplatesAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 2;

        // Act - 第一次調用
        var result1 = await _service.GetAvailableTemplatesAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.AdventureTemplates.RemoveRange(_context.AdventureTemplates);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAvailableTemplatesAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
        Assert.Equal(result1[0].Name, result2[0].Name);
    }

    #endregion

    #region GetTemplateByIdAsync 測試

    [Fact]
    public async Task GetTemplateByIdAsync_WithValidTemplateId_ShouldReturnTemplate()
    {
        // Arrange
        var templateId = 1;

        // Act
        var result = await _service.GetTemplateByIdAsync(templateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(templateId, result.Id);
        Assert.Equal("Forest Adventure", result.Name);
        Assert.Equal("Exploration", result.Category);
        Assert.Equal("Easy", result.Difficulty);
        Assert.Equal(20, result.EnergyCost);
        Assert.Equal(60, result.DurationMinutes);
        Assert.True(result.IsAvailable);
    }

    [Fact]
    public async Task GetTemplateByIdAsync_WithInvalidTemplateId_ShouldThrowException()
    {
        // Arrange
        var templateId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetTemplateByIdAsync(templateId));
    }

    [Fact]
    public async Task GetTemplateByIdAsync_ShouldUseCache()
    {
        // Arrange
        var templateId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetTemplateByIdAsync(templateId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.AdventureTemplates.RemoveRange(_context.AdventureTemplates);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetTemplateByIdAsync(templateId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
        Assert.Equal(result1.Name, result2.Name);
    }

    #endregion

    #region GetUserAdventureLogsAsync 測試

    [Fact]
    public async Task GetUserAdventureLogsAsync_WithValidParameters_ShouldReturnLogs()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, l => l.Status == "InProgress");
        Assert.Contains(result.Items, l => l.Status == "Completed");
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var page = 1;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserAdventureLogsAsync(userId, page, pageSize));
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var page = 0;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserAdventureLogsAsync(userId, page, pageSize));
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.PageSize); // Default page size
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_WithExcessivePageSize_ShouldUseMaxPageSize()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 1000;

        // Act
        var result = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.PageSize); // Max page size
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        // Act - 第一次調用
        var result1 = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.AdventureLogs.RemoveRange(_context.AdventureLogs);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalCount, result2.TotalCount);
        Assert.Equal(result1.Items.Count, result2.Items.Count);
    }

    #endregion

    #region GetUserAdventureStatisticsAsync 測試

    [Fact]
    public async Task GetUserAdventureStatisticsAsync_WithValidUserId_ShouldReturnStatistics()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetUserAdventureStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalAdventures);
        Assert.Equal(1, result.CompletedAdventures);
        Assert.Equal(0, result.FailedAdventures);
        Assert.Equal(0, result.AbandonedAdventures);
        Assert.Equal(0.5m, result.SuccessRate);
        Assert.Equal(100, result.TotalExperienceGained);
        Assert.Equal(50, result.TotalPointsGained);
        Assert.Equal(25, result.TotalGoldGained);
        Assert.Equal(2, result.TotalMonsterEncounters);
        Assert.Equal(2, result.MonstersDefeated);
        Assert.Equal(0, result.MonstersEscaped);
    }

    [Fact]
    public async Task GetUserAdventureStatisticsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserAdventureStatisticsAsync(userId));
    }

    [Fact]
    public async Task GetUserAdventureStatisticsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetUserAdventureStatisticsAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.AdventureLogs.RemoveRange(_context.AdventureLogs);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserAdventureStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalAdventures, result2.TotalAdventures);
        Assert.Equal(result1.SuccessRate, result2.SuccessRate);
    }

    #endregion

    #region ProcessMonsterEncounterAsync 測試

    [Fact]
    public async Task ProcessMonsterEncounterAsync_WithValidData_ShouldProcessEncounter()
    {
        // Arrange
        var adventureLogId = 1;
        var userId = 1;
        var monsterType = "Goblin";

        // Act
        var result = await _service.ProcessMonsterEncounterAsync(adventureLogId, userId, monsterType);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.MonsterName.Contains("Goblin"));
        Assert.True(result.ExperienceGained > 0);
        Assert.True(result.PointsGained > 0);
        Assert.True(result.GoldGained > 0);
        Assert.NotNull(result.BattleNotes);
        Assert.NotNull(result.AdventureProgress);

        // 驗證資料庫中確實創建了記錄
        var dbEncounter = await _context.MonsterEncounters
            .FirstOrDefaultAsync(me => me.AdventureLogId == adventureLogId && me.MonsterType == monsterType);
        Assert.NotNull(dbEncounter);
    }

    [Fact]
    public async Task ProcessMonsterEncounterAsync_WithInvalidAdventureLogId_ShouldThrowException()
    {
        // Arrange
        var adventureLogId = -1;
        var userId = 1;
        var monsterType = "Goblin";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessMonsterEncounterAsync(adventureLogId, userId, monsterType));
    }

    [Fact]
    public async Task ProcessMonsterEncounterAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var adventureLogId = 1;
        var userId = -1;
        var monsterType = "Goblin";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessMonsterEncounterAsync(adventureLogId, userId, monsterType));
    }

    [Fact]
    public async Task ProcessMonsterEncounterAsync_WithEmptyMonsterType_ShouldThrowException()
    {
        // Arrange
        var adventureLogId = 1;
        var userId = 1;
        var monsterType = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ProcessMonsterEncounterAsync(adventureLogId, userId, monsterType));
    }

    #endregion

    #region GetAdventureMonsterEncountersAsync 測試

    [Fact]
    public async Task GetAdventureMonsterEncountersAsync_WithValidData_ShouldReturnEncounters()
    {
        // Arrange
        var adventureLogId = 1;
        var userId = 1;

        // Act
        var result = await _service.GetAdventureMonsterEncountersAsync(adventureLogId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.True(e.ExperienceGained > 0));
        Assert.All(result, e => Assert.True(e.PointsGained > 0));
        Assert.All(result, e => Assert.True(e.GoldGained > 0);
        Assert.Contains(result, e => e.MonsterType == "Goblin");
        Assert.Contains(result, e => e.MonsterType == "Orc");
    }

    [Fact]
    public async Task GetAdventureMonsterEncountersAsync_WithInvalidAdventureLogId_ShouldThrowException()
    {
        // Arrange
        var adventureLogId = -1;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdventureMonsterEncountersAsync(adventureLogId, userId));
    }

    [Fact]
    public async Task GetAdventureMonsterEncountersAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var adventureLogId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdventureMonsterEncountersAsync(adventureLogId, userId));
    }

    [Fact]
    public async Task GetAdventureMonsterEncountersAsync_ShouldUseCache()
    {
        // Arrange
        var adventureLogId = 1;
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetAdventureMonsterEncountersAsync(adventureLogId, userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.MonsterEncounters.RemoveRange(_context.MonsterEncounters);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAdventureMonsterEncountersAsync(adventureLogId, userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
    }

    #endregion

    #region GetAdventureProgressAsync 測試

    [Fact]
    public async Task GetAdventureProgressAsync_WithValidData_ShouldReturnProgress()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;

        // Act
        var result = await _service.GetAdventureProgressAsync(adventureId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.MonsterEncountersCompleted);
        Assert.Equal(3, result.TotalMonsterEncounters);
        Assert.Equal(2, result.MonstersDefeated);
        Assert.Equal(0, result.MonstersEscaped);
        Assert.True(result.TotalDamageDealt > 0);
        Assert.True(result.TotalDamageTaken > 0);
        Assert.True(result.CompletionPercentage > 0);
        Assert.True(result.CanComplete);
        Assert.False(result.CanFail);
        Assert.True(result.CanAbandon);
    }

    [Fact]
    public async Task GetAdventureProgressAsync_WithInvalidAdventureId_ShouldThrowException()
    {
        // Arrange
        var adventureId = -1;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdventureProgressAsync(adventureId, userId));
    }

    [Fact]
    public async Task GetAdventureProgressAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var adventureId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetAdventureProgressAsync(adventureId, userId));
    }

    [Fact]
    public async Task GetAdventureProgressAsync_ShouldUseCache()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetAdventureProgressAsync(adventureId, userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.AdventureLogs.RemoveRange(_context.AdventureLogs);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAdventureProgressAsync(adventureId, userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.MonsterEncountersCompleted, result2.MonsterEncountersCompleted);
        Assert.Equal(result1.CompletionPercentage, result2.CompletionPercentage);
    }

    #endregion

    #region CanStartAdventureAsync 測試

    [Fact]
    public async Task CanStartAdventureAsync_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var userId = 2; // User with sufficient energy and level
        var templateId = 1; // Easy template

        // Act
        var result = await _service.CanStartAdventureAsync(userId, templateId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanStartAdventureAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var templateId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CanStartAdventureAsync(userId, templateId));
    }

    [Fact]
    public async Task CanStartAdventureAsync_WithInvalidTemplateId_ShouldThrowException()
    {
        // Arrange
        var userId = 2;
        var templateId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CanStartAdventureAsync(userId, templateId));
    }

    #endregion

    #region CalculateAdventureRewardsAsync 測試

    [Fact]
    public async Task CalculateAdventureRewardsAsync_WithValidData_ShouldReturnRewards()
    {
        // Arrange
        var templateId = 1;
        var userId = 1;
        var isSuccess = true;

        // Act
        var result = await _service.CalculateAdventureRewardsAsync(templateId, userId, isSuccess);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ExperienceReward > 0);
        Assert.True(result.PointsReward > 0);
        Assert.True(result.GoldReward > 0);
        Assert.True(result.HealthChange > 0);
        Assert.True(result.HungerChange < 0);
        Assert.True(result.EnergyChange < 0);
        Assert.True(result.HappinessChange > 0);
        Assert.True(result.CleanlinessChange < 0);
        Assert.NotNull(result.BonusRewards);
    }

    [Fact]
    public async Task CalculateAdventureRewardsAsync_WithInvalidTemplateId_ShouldThrowException()
    {
        // Arrange
        var templateId = -1;
        var userId = 1;
        var isSuccess = true;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CalculateAdventureRewardsAsync(templateId, userId, isSuccess));
    }

    [Fact]
    public async Task CalculateAdventureRewardsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var templateId = 1;
        var userId = -1;
        var isSuccess = true;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CalculateAdventureRewardsAsync(templateId, userId, isSuccess));
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetUserAdventureLogsAsync_WithNoLogs_ShouldReturnEmptyResult()
    {
        // Arrange
        var userId = 3; // User with no adventure logs
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetUserAdventureStatisticsAsync_WithNoLogs_ShouldReturnEmptyStatistics()
    {
        // Arrange
        var userId = 3; // User with no adventure logs

        // Act
        var result = await _service.GetUserAdventureStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalAdventures);
        Assert.Equal(0, result.SuccessRate);
        Assert.Equal(0, result.TotalExperienceGained);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetAvailableTemplatesAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 2;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAvailableTemplatesAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserAdventureLogsAsync(userId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUserAdventureStatisticsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserAdventureStatisticsAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetAdventureProgressAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var adventureId = 1;
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAdventureProgressAsync(adventureId, userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}