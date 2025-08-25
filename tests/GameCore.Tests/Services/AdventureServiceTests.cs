using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.AdventureDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

public class AdventureServiceTests
{
    private readonly GameCoreDbContext _context;
    private readonly AdventureService _service;
    private readonly Mock<ILogger<AdventureService>> _loggerMock;

    public AdventureServiceTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _loggerMock = new Mock<ILogger<AdventureService>>();
        _service = new AdventureService(_context, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test user
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Level = 10,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastLoginAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Users.Add(user);

        // Create user wallet
        var wallet = new UserWallet
        {
            WalletId = 1,
            UserId = 1,
            Balance = 100.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-10)
        };
        _context.UserWallets.Add(wallet);

        // Create adventure template
        var template = new AdventureTemplate
        {
            Id = 1,
            Name = "Test Adventure",
            Description = "A test adventure for testing purposes",
            Category = "Exploration",
            Difficulty = "Normal",
            MinLevel = 1,
            MaxLevel = 20,
            EnergyCost = 20,
            DurationMinutes = 60,
            BaseSuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            MaxMonsterEncounters = 3,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.AdventureTemplates.Add(template);

        _context.SaveChanges();
    }

    [Fact]
    public async Task StartAdventureAsync_ValidRequest_ShouldStartAdventure()
    {
        // Arrange
        var request = new StartAdventureRequestDto { TemplateId = 1 };

        // Act
        var result = await _service.StartAdventureAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("Test Adventure", result.Title);
        Assert.Equal("Normal", result.Difficulty);
        Assert.Equal("InProgress", result.Status);
        Assert.Equal(20, result.EnergySpent);

        // Verify database records were created
        var adventure = await _context.Adventures.FirstOrDefaultAsync(a => a.UserId == 1);
        Assert.NotNull(adventure);
        Assert.Equal("Test Adventure", adventure.Title);

        var adventureLog = await _context.AdventureLogs.FirstOrDefaultAsync(al => al.AdventureId == adventure.Id);
        Assert.NotNull(adventureLog);
        Assert.Equal("InProgress", adventureLog.Status);

        // Verify energy was deducted from wallet
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == 1);
        Assert.NotNull(wallet);
        Assert.Equal(80.00m, wallet.Balance);
    }

    [Fact]
    public async Task StartAdventureAsync_UserAlreadyHasActiveAdventure_ShouldThrowException()
    {
        // Arrange - Create existing active adventure
        var existingAdventure = new Adventure
        {
            UserId = 1,
            Title = "Existing Adventure",
            Difficulty = "Easy",
            RequiredLevel = 1,
            RequiredEnergy = 10,
            DurationMinutes = 30,
            SuccessRate = 0.8m,
            BaseExperienceReward = 50,
            BasePointsReward = 25,
            BaseGoldReward = 10.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(existingAdventure);
        await _context.SaveChangesAsync();

        var existingLog = new AdventureLog
        {
            AdventureId = existingAdventure.Id,
            UserId = 1,
            Status = "InProgress",
            StartedAt = DateTime.UtcNow.AddHours(-1),
            EnergySpent = 10,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdventureLogs.Add(existingLog);
        await _context.SaveChangesAsync();

        var request = new StartAdventureRequestDto { TemplateId = 1 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.StartAdventureAsync(1, 1));
        Assert.Equal("User already has an active adventure", exception.Message);
    }

    [Fact]
    public async Task StartAdventureAsync_InsufficientEnergy_ShouldThrowException()
    {
        // Arrange - Update wallet to have insufficient energy
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == 1);
        wallet.Balance = 10.00m; // Less than required 20
        await _context.SaveChangesAsync();

        var request = new StartAdventureRequestDto { TemplateId = 1 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.StartAdventureAsync(1, 1));
        Assert.Equal("Insufficient energy to start adventure", exception.Message);
    }

    [Fact]
    public async Task StartAdventureAsync_UserLevelTooLow_ShouldThrowException()
    {
        // Arrange - Update user to have insufficient level
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);
        user.Level = 5; // Template requires level 1-20, but let's test with a template that requires higher
        await _context.SaveChangesAsync();

        // Create a template with higher level requirement
        var highLevelTemplate = new AdventureTemplate
        {
            Id = 2,
            Name = "High Level Adventure",
            Description = "Requires high level",
            Category = "Combat",
            Difficulty = "Hard",
            MinLevel = 25, // Higher than user's level
            MaxLevel = 50,
            EnergyCost = 30,
            DurationMinutes = 90,
            BaseSuccessRate = 0.6m,
            BaseExperienceReward = 200,
            BasePointsReward = 100,
            BaseGoldReward = 50.00m,
            MaxMonsterEncounters = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.AdventureTemplates.Add(highLevelTemplate);
        await _context.SaveChangesAsync();

        var request = new StartAdventureRequestDto { TemplateId = 2 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.StartAdventureAsync(1, 2));
        Assert.Contains("Cannot start adventure", exception.Message);
    }

    [Fact]
    public async Task GetAdventureAsync_ValidAdventure_ShouldReturnAdventure()
    {
        // Arrange - Create adventure and log
        var adventure = new Adventure
        {
            UserId = 1,
            Title = "Test Adventure",
            Description = "Test description",
            Difficulty = "Normal",
            RequiredLevel = 1,
            RequiredEnergy = 20,
            DurationMinutes = 60,
            SuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        var adventureLog = new AdventureLog
        {
            AdventureId = adventure.Id,
            UserId = 1,
            Status = "InProgress",
            StartedAt = DateTime.UtcNow.AddHours(-1),
            EnergySpent = 20,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdventureLogs.Add(adventureLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAdventureAsync(adventure.Id, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(adventure.Id, result.Id);
        Assert.Equal("Test Adventure", result.Title);
        Assert.Equal("Normal", result.Difficulty);
        Assert.Equal("InProgress", result.Status);
        Assert.Equal(20, result.EnergySpent);
        Assert.NotNull(result.Progress);
    }

    [Fact]
    public async Task CompleteAdventureAsync_ValidAdventure_ShouldCompleteAdventure()
    {
        // Arrange - Create adventure and log
        var adventure = new Adventure
        {
            UserId = 1,
            Title = "Test Adventure",
            Description = "Test description",
            Difficulty = "Normal",
            RequiredLevel = 1,
            RequiredEnergy = 20,
            DurationMinutes = 60,
            SuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        var adventureLog = new AdventureLog
        {
            AdventureId = adventure.Id,
            UserId = 1,
            Status = "InProgress",
            StartedAt = DateTime.UtcNow.AddHours(-1),
            EnergySpent = 20,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdventureLogs.Add(adventureLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CompleteAdventureAsync(adventure.Id, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
        Assert.NotNull(result.CompletedAt);
        Assert.True(result.ExperienceGained > 0);
        Assert.True(result.PointsGained > 0);
        Assert.True(result.GoldGained > 0);

        // Verify database was updated
        var updatedLog = await _context.AdventureLogs.FirstOrDefaultAsync(al => al.Id == adventureLog.Id);
        Assert.NotNull(updatedLog);
        Assert.Equal("Completed", updatedLog.Status);
        Assert.NotNull(updatedLog.CompletedAt);

        // Verify wallet was updated
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == 1);
        Assert.NotNull(wallet);
        Assert.True(wallet.Balance > 80.00m); // Should have received gold reward
    }

    [Fact]
    public async Task FailAdventureAsync_ValidAdventure_ShouldFailAdventure()
    {
        // Arrange - Create adventure and log
        var adventure = new Adventure
        {
            UserId = 1,
            Title = "Test Adventure",
            Description = "Test description",
            Difficulty = "Normal",
            RequiredLevel = 1,
            RequiredEnergy = 20,
            DurationMinutes = 60,
            SuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        var adventureLog = new AdventureLog
        {
            AdventureId = adventure.Id,
            UserId = 1,
            Status = "InProgress",
            StartedAt = DateTime.UtcNow.AddHours(-1),
            EnergySpent = 20,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdventureLogs.Add(adventureLog);
        await _context.SaveChangesAsync();

        var request = new FailAdventureRequestDto { FailureReason = "Ran out of time" };

        // Act
        var result = await _service.FailAdventureAsync(adventure.Id, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Failed", result.Status);
        Assert.NotNull(result.FailedAt);
        Assert.True(result.ExperienceGained > 0); // Should get partial rewards
        Assert.True(result.PointsGained > 0);
        Assert.True(result.GoldGained > 0);

        // Verify database was updated
        var updatedLog = await _context.AdventureLogs.FirstOrDefaultAsync(al => al.Id == adventureLog.Id);
        Assert.NotNull(updatedLog);
        Assert.Equal("Failed", updatedLog.Status);
        Assert.NotNull(updatedLog.FailedAt);
        Assert.Equal("Ran out of time", updatedLog.FailureReason);
    }

    [Fact]
    public async Task GetAvailableTemplatesAsync_ShouldReturnTemplatesWithAvailability()
    {
        // Act
        var templates = await _service.GetAvailableTemplatesAsync(1);

        // Assert
        Assert.NotNull(templates);
        Assert.NotEmpty(templates);
        
        var template = templates.First(t => t.Id == 1);
        Assert.Equal("Test Adventure", template.Name);
        Assert.Equal("Exploration", template.Category);
        Assert.Equal("Normal", template.Difficulty);
        Assert.True(template.IsAvailable);
        Assert.Null(template.UnavailableReason);
    }

    [Fact]
    public async Task GetUserAdventureLogsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange - Create multiple adventure logs
        var adventure = new Adventure
        {
            UserId = 1,
            Title = "Test Adventure",
            Description = "Test description",
            Difficulty = "Normal",
            RequiredLevel = 1,
            RequiredEnergy = 20,
            DurationMinutes = 60,
            SuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        var logs = new List<AdventureLog>();
        for (int i = 0; i < 25; i++)
        {
            logs.Add(new AdventureLog
            {
                AdventureId = adventure.Id,
                UserId = 1,
                Status = "Completed",
                StartedAt = DateTime.UtcNow.AddDays(-i),
                CompletedAt = DateTime.UtcNow.AddDays(-i).AddHours(1),
                EnergySpent = 20,
                ExperienceGained = 100,
                PointsGained = 50,
                GoldGained = 25.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        _context.AdventureLogs.AddRange(logs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserAdventureLogsAsync(1, page: 2, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task ProcessMonsterEncounterAsync_ValidEncounter_ShouldProcessEncounter()
    {
        // Arrange - Create adventure and log
        var adventure = new Adventure
        {
            UserId = 1,
            Title = "Test Adventure",
            Description = "Test description",
            Difficulty = "Normal",
            RequiredLevel = 1,
            RequiredEnergy = 20,
            DurationMinutes = 60,
            SuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        var adventureLog = new AdventureLog
        {
            AdventureId = adventure.Id,
            UserId = 1,
            Status = "InProgress",
            StartedAt = DateTime.UtcNow.AddHours(-1),
            EnergySpent = 20,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };
        _context.AdventureLogs.Add(adventureLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ProcessMonsterEncounterAsync(adventureLog.Id, 1, "Normal");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.MonsterName);
        Assert.Equal("Normal", result.MonsterType);
        Assert.True(result.MonsterLevel > 0);
        Assert.True(result.DamageDealt > 0);
        Assert.True(result.ExperienceGained > 0);
        Assert.True(result.PointsGained > 0);
        Assert.True(result.GoldGained > 0);
        Assert.NotNull(result.AdventureProgress);

        // Verify database record was created
        var encounter = await _context.MonsterEncounters.FirstOrDefaultAsync(me => me.AdventureLogId == adventureLog.Id);
        Assert.NotNull(encounter);
        Assert.Equal(result.MonsterName, encounter.MonsterName);
        Assert.Equal(result.Outcome, encounter.Outcome);
    }

    [Fact]
    public async Task GetUserAdventureStatisticsAsync_ShouldReturnStatistics()
    {
        // Arrange - Create completed adventures
        var adventure = new Adventure
        {
            UserId = 1,
            Title = "Test Adventure",
            Description = "Test description",
            Difficulty = "Normal",
            RequiredLevel = 1,
            RequiredEnergy = 20,
            DurationMinutes = 60,
            SuccessRate = 0.7m,
            BaseExperienceReward = 100,
            BasePointsReward = 50,
            BaseGoldReward = 25.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        var logs = new List<AdventureLog>
        {
            new AdventureLog
            {
                AdventureId = adventure.Id,
                UserId = 1,
                Status = "Completed",
                StartedAt = DateTime.UtcNow.AddDays(-1),
                CompletedAt = DateTime.UtcNow.AddDays(-1).AddHours(1),
                EnergySpent = 20,
                ExperienceGained = 100,
                PointsGained = 50,
                GoldGained = 25.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new AdventureLog
            {
                AdventureId = adventure.Id,
                UserId = 1,
                Status = "Failed",
                StartedAt = DateTime.UtcNow.AddDays(-2),
                FailedAt = DateTime.UtcNow.AddDays(-2).AddMinutes(30),
                EnergySpent = 20,
                ExperienceGained = 50,
                PointsGained = 25,
                GoldGained = 12.50m,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };
        _context.AdventureLogs.AddRange(logs);
        await _context.SaveChangesAsync();

        // Act
        var statistics = await _service.GetUserAdventureStatisticsAsync(1);

        // Assert
        Assert.NotNull(statistics);
        Assert.Equal(2, statistics.TotalAdventures);
        Assert.Equal(1, statistics.CompletedAdventures);
        Assert.Equal(1, statistics.FailedAdventures);
        Assert.Equal(0, statistics.AbandonedAdventures);
        Assert.Equal(0.5m, statistics.SuccessRate);
        Assert.Equal(150, statistics.TotalExperienceGained);
        Assert.Equal(75, statistics.TotalPointsGained);
        Assert.Equal(37.50m, statistics.TotalGoldGained);
        Assert.Equal(0, statistics.TotalMonsterEncounters);
    }

    [Fact]
    public async Task CanStartAdventureAsync_ValidUser_ShouldReturnTrue()
    {
        // Act
        var canStart = await _service.CanStartAdventureAsync(1, 1);

        // Assert
        Assert.True(canStart);
    }

    [Fact]
    public async Task CanStartAdventureAsync_UserLevelTooLow_ShouldReturnFalse()
    {
        // Arrange - Create high level template
        var highLevelTemplate = new AdventureTemplate
        {
            Id = 3,
            Name = "High Level Adventure",
            Description = "Requires high level",
            Category = "Combat",
            Difficulty = "Hard",
            MinLevel = 25, // Higher than user's level (10)
            MaxLevel = 50,
            EnergyCost = 30,
            DurationMinutes = 90,
            BaseSuccessRate = 0.6m,
            BaseExperienceReward = 200,
            BasePointsReward = 100,
            BaseGoldReward = 50.00m,
            MaxMonsterEncounters = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };
        _context.AdventureTemplates.Add(highLevelTemplate);
        await _context.SaveChangesAsync();

        // Act
        var canStart = await _service.CanStartAdventureAsync(1, 3);

        // Assert
        Assert.False(canStart);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}