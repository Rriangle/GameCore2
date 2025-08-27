using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.VirtualPetDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// VirtualPetService 優化版本單元測試
/// </summary>
public class VirtualPetServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<VirtualPetService> _logger;
    private readonly VirtualPetService _service;

    public VirtualPetServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<VirtualPetService>>();
        _service = new VirtualPetService(_context, _memoryCache, _logger);

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
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com" };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com" };
        var user3 = new User { user_id = 3, username = "user3", email = "user3@test.com" };
        _context.Users.AddRange(user1, user2, user3);

        // 創建用戶錢包
        var wallet1 = new UserWallet { UserId = 1, Balance = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var wallet2 = new UserWallet { UserId = 2, Balance = 50, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var wallet3 = new UserWallet { UserId = 3, Balance = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        _context.UserWallets.AddRange(wallet1, wallet2, wallet3);

        // 創建虛擬寵物
        var now = DateTime.UtcNow;
        var pet1 = new VirtualPet 
        { 
            Id = 1, 
            UserId = 1, 
            Name = "Fluffy", 
            Level = 5, 
            Experience = 250, 
            ExperienceToNextLevel = 500, 
            Health = 80, 
            MaxHealth = 100, 
            Hunger = 70, 
            MaxHunger = 100, 
            Energy = 60, 
            MaxEnergy = 100, 
            Happiness = 90, 
            MaxHappiness = 100, 
            Cleanliness = 85, 
            MaxCleanliness = 100, 
            Color = "Blue", 
            Personality = "Friendly", 
            LastFed = now.AddHours(-2), 
            LastPlayed = now.AddHours(-3), 
            LastCleaned = now.AddHours(-4), 
            LastRested = now.AddHours(-5), 
            CreatedAt = now.AddDays(-30), 
            UpdatedAt = now 
        };
        var pet2 = new VirtualPet 
        { 
            Id = 2, 
            UserId = 2, 
            Name = "Spike", 
            Level = 3, 
            Experience = 150, 
            ExperienceToNextLevel = 300, 
            Health = 90, 
            MaxHealth = 100, 
            Hunger = 80, 
            MaxHunger = 100, 
            Energy = 75, 
            MaxEnergy = 100, 
            Happiness = 85, 
            MaxHappiness = 100, 
            Cleanliness = 90, 
            MaxCleanliness = 100, 
            Color = "Red", 
            Personality = "Playful", 
            LastFed = now.AddHours(-1), 
            LastPlayed = now.AddHours(-2), 
            LastCleaned = now.AddHours(-3), 
            LastRested = now.AddHours(-4), 
            CreatedAt = now.AddDays(-20), 
            UpdatedAt = now 
        };
        _context.VirtualPets.AddRange(pet1, pet2);

        // 創建寵物物品
        var food1 = new PetItem 
        { 
            Id = 1, 
            Name = "Premium Food", 
            Description = "High-quality pet food", 
            Type = "Food", 
            Category = "Nutrition", 
            HealthEffect = 10, 
            HungerEffect = 30, 
            EnergyEffect = 5, 
            HappinessEffect = 5, 
            CleanlinessEffect = -2, 
            ExperienceEffect = 15, 
            Price = 50, 
            IsActive = true 
        };
        var toy1 = new PetItem 
        { 
            Id = 2, 
            Name = "Ball", 
            Description = "Bouncy ball for play", 
            Type = "Toy", 
            Category = "Entertainment", 
            HealthEffect = 5, 
            HungerEffect = -5, 
            EnergyEffect = -10, 
            HappinessEffect = 20, 
            CleanlinessEffect = -3, 
            ExperienceEffect = 10, 
            Price = 30, 
            IsActive = true 
        };
        var cleaning1 = new PetItem 
        { 
            Id = 3, 
            Name = "Shampoo", 
            Description = "Gentle cleaning shampoo", 
            Type = "Cleaning", 
            Category = "Hygiene", 
            HealthEffect = 5, 
            HungerEffect = 0, 
            EnergyEffect = 0, 
            HappinessEffect = 10, 
            CleanlinessEffect = 25, 
            ExperienceEffect = 8, 
            Price = 40, 
            IsActive = true 
        };
        _context.PetItems.AddRange(food1, toy1, cleaning1);

        // 創建寵物護理記錄
        var careLog1 = new PetCareLog 
        { 
            Id = 1, 
            PetId = 1, 
            UserId = 1, 
            Action = "Feed", 
            Description = "Fed Fluffy with Premium Food", 
            HealthChange = 10, 
            HungerChange = 30, 
            EnergyChange = 5, 
            HappinessChange = 5, 
            CleanlinessChange = -2, 
            ExperienceGained = 15, 
            PointsEarned = 20, 
            ActionTime = now.AddHours(-2), 
            CreatedAt = now.AddHours(-2) 
        };
        var careLog2 = new PetCareLog 
        { 
            Id = 2, 
            PetId = 1, 
            UserId = 1, 
            Action = "Play", 
            Description = "Played with Fluffy using Ball", 
            HealthChange = 5, 
            HungerChange = -5, 
            EnergyChange = -10, 
            HappinessChange = 20, 
            CleanlinessChange = -3, 
            ExperienceGained = 10, 
            PointsEarned = 15, 
            ActionTime = now.AddHours(-3), 
            CreatedAt = now.AddHours(-3) 
        };
        _context.PetCareLogs.AddRange(careLog1, careLog2);

        // 創建寵物成就
        var achievement1 = new PetAchievement 
        { 
            Id = 1, 
            PetId = 1, 
            Name = "First Steps", 
            Description = "Created your first virtual pet", 
            Category = "Creation", 
            PointsReward = 50, 
            IsUnlocked = true, 
            UnlockedAt = now.AddDays(-30), 
            CreatedAt = now.AddDays(-30) 
        };
        var achievement2 = new PetAchievement 
        { 
            Id = 2, 
            PetId = 1, 
            Name = "Level 5", 
            Description = "Reach level 5", 
            Category = "Leveling", 
            PointsReward = 100, 
            IsUnlocked = true, 
            UnlockedAt = now.AddDays(-25), 
            CreatedAt = now.AddDays(-30) 
        };
        var achievement3 = new PetAchievement 
        { 
            Id = 3, 
            PetId = 1, 
            Name = "Level 10", 
            Description = "Reach level 10", 
            Category = "Leveling", 
            PointsReward = 200, 
            IsUnlocked = false, 
            UnlockedAt = null, 
            CreatedAt = now.AddDays(-30) 
        };
        _context.PetAchievements.AddRange(achievement1, achievement2, achievement3);

        _context.SaveChanges();
    }

    #endregion

    #region CreatePetAsync 測試

    [Fact]
    public async Task CreatePetAsync_WithValidData_ShouldCreatePet()
    {
        // Arrange
        var userId = 3; // User with no pet
        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = "Green",
            Personality = "Loyal"
        };

        // Act
        var result = await _service.CreatePetAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Buddy", result.Name);
        Assert.Equal("Green", result.Color);
        Assert.Equal("Loyal", result.Personality);
        Assert.Equal(1, result.Level);
        Assert.Equal(0, result.Experience);
        Assert.Equal(100, result.Health);
        Assert.Equal(100, result.Hunger);
        Assert.Equal(100, result.Energy);
        Assert.Equal(100, result.Happiness);
        Assert.Equal(100, result.Cleanliness);

        // 驗證資料庫中確實創建了記錄
        var dbPet = await _context.VirtualPets
            .FirstOrDefaultAsync(vp => vp.Id == result.Id);
        Assert.NotNull(dbPet);

        // 驗證初始成就被創建
        var dbAchievements = await _context.PetAchievements
            .Where(pa => pa.PetId == result.Id)
            .ToListAsync();
        Assert.True(dbAchievements.Count >= 1);
    }

    [Fact]
    public async Task CreatePetAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = "Green",
            Personality = "Loyal"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        var userId = 3;
        CreatePetRequestDto request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var userId = 3;
        var request = new CreatePetRequestDto
        {
            Name = "",
            Color = "Green",
            Personality = "Loyal"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithEmptyColor_ShouldThrowException()
    {
        // Arrange
        var userId = 3;
        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = "",
            Personality = "Loyal"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithTooLongName_ShouldThrowException()
    {
        // Arrange
        var userId = 3;
        var request = new CreatePetRequestDto
        {
            Name = new string('a', 51), // Exceeds MaxPetNameLength
            Color = "Green",
            Personality = "Loyal"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithTooLongColor_ShouldThrowException()
    {
        // Arrange
        var userId = 3;
        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = new string('a', 31), // Exceeds MaxPetColorLength
            Personality = "Loyal"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithTooLongPersonality_ShouldThrowException()
    {
        // Arrange
        var userId = 3;
        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = "Green",
            Personality = new string('a', 101) // Exceeds MaxPetPersonalityLength
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    [Fact]
    public async Task CreatePetAsync_WithUserAlreadyHavingPet_ShouldThrowException()
    {
        // Arrange
        var userId = 1; // User who already has a pet
        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = "Green",
            Personality = "Loyal"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.CreatePetAsync(userId, request));
    }

    #endregion

    #region GetUserPetAsync 測試

    [Fact]
    public async Task GetUserPetAsync_WithValidUserId_ShouldReturnPet()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetUserPetAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Fluffy", result.Name);
        Assert.Equal(5, result.Level);
        Assert.Equal(250, result.Experience);
        Assert.Equal(80, result.Health);
        Assert.Equal(70, result.Hunger);
        Assert.Equal(60, result.Energy);
        Assert.Equal(90, result.Happiness);
        Assert.Equal(85, result.Cleanliness);
        Assert.Equal("Blue", result.Color);
        Assert.Equal("Friendly", result.Personality);
        Assert.Equal("Good", result.Status);
        Assert.Contains("Tired", result.Needs);
    }

    [Fact]
    public async Task GetUserPetAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserPetAsync(userId));
    }

    [Fact]
    public async Task GetUserPetAsync_WithUserHavingNoPet_ShouldThrowException()
    {
        // Arrange
        var userId = 3; // User with no pet

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.GetUserPetAsync(userId));
    }

    [Fact]
    public async Task GetUserPetAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetUserPetAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.VirtualPets.RemoveRange(_context.VirtualPets);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserPetAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id);
        Assert.Equal(result1.Name, result2.Name);
    }

    #endregion

    #region FeedPetAsync 測試

    [Fact]
    public async Task FeedPetAsync_WithValidData_ShouldFeedPet()
    {
        // Arrange
        var userId = 1;
        var itemId = 1; // Premium Food

        // Act
        var result = await _service.FeedPetAsync(userId, itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Fluffy", result.PetName);
        Assert.Equal("Feed", result.Action);
        Assert.Equal(10, result.HealthChange);
        Assert.Equal(30, result.HungerChange);
        Assert.Equal(5, result.EnergyChange);
        Assert.Equal(5, result.HappinessChange);
        Assert.Equal(-2, result.CleanlinessChange);
        Assert.Equal(15, result.ExperienceGained);
        Assert.True(result.PointsEarned > 0);
        Assert.False(result.LevelUp);
        Assert.Contains("enjoyed", result.Message);

        // 驗證資料庫中確實更新了記錄
        var dbPet = await _context.VirtualPets
            .FirstOrDefaultAsync(vp => vp.Id == 1);
        Assert.NotNull(dbPet);
        Assert.True(dbPet.Health > 80);
        Assert.True(dbPet.Hunger > 70);

        // 驗證護理記錄被創建
        var dbCareLog = await _context.PetCareLogs
            .FirstOrDefaultAsync(pcl => pcl.PetId == 1 && pcl.Action == "Feed");
        Assert.NotNull(dbCareLog);
    }

    [Fact]
    public async Task FeedPetAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var itemId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.FeedPetAsync(userId, itemId));
    }

    [Fact]
    public async Task FeedPetAsync_WithInvalidItemId_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var itemId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.FeedPetAsync(userId, itemId));
    }

    [Fact]
    public async Task FeedPetAsync_WithWrongItemType_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var itemId = 2; // Ball (Toy), not Food

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.FeedPetAsync(userId, itemId));
    }

    [Fact]
    public async Task FeedPetAsync_WithPetNotHungry_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var itemId = 1;
        
        // 更新寵物的最後餵食時間為1小時前（不滿足1小時間隔要求）
        var pet = await _context.VirtualPets.FindAsync(1);
        pet!.LastFed = DateTime.UtcNow.AddHours(-0.5);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.FeedPetAsync(userId, itemId));
    }

    #endregion

    #region PlayWithPetAsync 測試

    [Fact]
    public async Task PlayWithPetAsync_WithValidData_ShouldPlayWithPet()
    {
        // Arrange
        var userId = 1;
        var itemId = 2; // Ball

        // Act
        var result = await _service.PlayWithPetAsync(userId, itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Fluffy", result.PetName);
        Assert.Equal("Play", result.Action);
        Assert.Equal(5, result.HealthChange);
        Assert.Equal(-5, result.HungerChange);
        Assert.Equal(-10, result.EnergyChange);
        Assert.Equal(20, result.HappinessChange);
        Assert.Equal(-3, result.CleanlinessChange);
        Assert.Equal(10, result.ExperienceGained);
        Assert.True(result.PointsEarned > 0);
        Assert.False(result.LevelUp);
        Assert.Contains("had fun", result.Message);
    }

    [Fact]
    public async Task PlayWithPetAsync_WithPetNotReadyToPlay_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var itemId = 2;
        
        // 更新寵物的最後遊戲時間為1小時前（不滿足2小時間隔要求）
        var pet = await _context.VirtualPets.FindAsync(1);
        pet!.LastPlayed = DateTime.UtcNow.AddHours(-1.5);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.PlayWithPetAsync(userId, itemId));
    }

    #endregion

    #region CleanPetAsync 測試

    [Fact]
    public async Task CleanPetAsync_WithValidData_ShouldCleanPet()
    {
        // Arrange
        var userId = 1;
        var itemId = 3; // Shampoo

        // Act
        var result = await _service.CleanPetAsync(userId, itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Fluffy", result.PetName);
        Assert.Equal("Clean", result.Action);
        Assert.Equal(5, result.HealthChange);
        Assert.Equal(0, result.HungerChange);
        Assert.Equal(0, result.EnergyChange);
        Assert.Equal(10, result.HappinessChange);
        Assert.Equal(25, result.CleanlinessChange);
        Assert.Equal(8, result.ExperienceGained);
        Assert.True(result.PointsEarned > 0);
        Assert.False(result.LevelUp);
        Assert.Contains("clean and happy", result.Message);
    }

    [Fact]
    public async Task CleanPetAsync_WithPetNotDirty_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var itemId = 3;
        
        // 更新寵物的最後清潔時間為2小時前（不滿足3小時間隔要求）
        var pet = await _context.VirtualPets.FindAsync(1);
        pet!.LastCleaned = DateTime.UtcNow.AddHours(-2.5);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.CleanPetAsync(userId, itemId));
    }

    #endregion

    #region RestPetAsync 測試

    [Fact]
    public async Task RestPetAsync_WithValidData_ShouldRestPet()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.RestPetAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PetId);
        Assert.Equal("Fluffy", result.PetName);
        Assert.Equal("Rest", result.Action);
        Assert.Equal(20, result.HealthChange);
        Assert.Equal(0, result.HungerChange);
        Assert.Equal(50, result.EnergyChange);
        Assert.Equal(10, result.HappinessChange);
        Assert.Equal(0, result.CleanlinessChange);
        Assert.Equal(10, result.ExperienceGained);
        Assert.True(result.PointsEarned > 0);
        Assert.False(result.LevelUp);
        Assert.Contains("well rested", result.Message);
    }

    [Fact]
    public async Task RestPetAsync_WithPetNotTired_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        
        // 更新寵物的最後休息時間為3小時前（不滿足4小時間隔要求）
        var pet = await _context.VirtualPets.FindAsync(1);
        pet!.LastRested = DateTime.UtcNow.AddHours(-3.5);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RestPetAsync(userId));
    }

    #endregion

    #region ChangePetColorAsync 測試

    [Fact]
    public async Task ChangePetColorAsync_WithValidData_ShouldChangeColor()
    {
        // Arrange
        var userId = 1;
        var newColor = "Purple";

        // Act
        var result = await _service.ChangePetColorAsync(userId, newColor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Purple", result.Color);

        // 驗證資料庫中確實更新了記錄
        var dbPet = await _context.VirtualPets
            .FirstOrDefaultAsync(vp => vp.Id == 1);
        Assert.NotNull(dbPet);
        Assert.Equal("Purple", dbPet.Color);
    }

    [Fact]
    public async Task ChangePetColorAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var newColor = "Purple";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ChangePetColorAsync(userId, newColor));
    }

    [Fact]
    public async Task ChangePetColorAsync_WithEmptyColor_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var newColor = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ChangePetColorAsync(userId, newColor));
    }

    [Fact]
    public async Task ChangePetColorAsync_WithTooLongColor_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var newColor = new string('a', 31); // Exceeds MaxPetColorLength

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.ChangePetColorAsync(userId, newColor));
    }

    #endregion

    #region GetPetCareHistoryAsync 測試

    [Fact]
    public async Task GetPetCareHistoryAsync_WithValidParameters_ShouldReturnHistory()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await _service.GetPetCareHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Feed", result.Items[0].Action);
        Assert.Equal("Play", result.Items[1].Action);
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var page = 1;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetPetCareHistoryAsync(userId, page, pageSize));
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var page = 0;
        var pageSize = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetPetCareHistoryAsync(userId, page, pageSize));
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetPetCareHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.PageSize); // Default page size
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_WithExcessivePageSize_ShouldUseMaxPageSize()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 1000;

        // Act
        var result = await _service.GetPetCareHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.PageSize); // Max page size
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;

        // Act - 第一次調用
        var result1 = await _service.GetPetCareHistoryAsync(userId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.PetCareLogs.RemoveRange(_context.PetCareLogs);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetPetCareHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalCount, result2.TotalCount);
        Assert.Equal(result1.Items.Count, result2.Items.Count);
    }

    #endregion

    #region GetPetAchievementsAsync 測試

    [Fact]
    public async Task GetPetAchievementsAsync_WithValidUserId_ShouldReturnAchievements()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetPetAchievementsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, a => Assert.True(a.PointsReward > 0));
        Assert.Equal(2, result.Count(a => a.IsUnlocked));
        Assert.Equal(1, result.Count(a => !a.IsUnlocked));
    }

    [Fact]
    public async Task GetPetAchievementsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetPetAchievementsAsync(userId));
    }

    [Fact]
    public async Task GetPetAchievementsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetPetAchievementsAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.PetAchievements.RemoveRange(_context.PetAchievements);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetPetAchievementsAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
    }

    #endregion

    #region GetAvailablePetItemsAsync 測試

    [Fact]
    public async Task GetAvailablePetItemsAsync_ShouldReturnItems()
    {
        // Act
        var result = await _service.GetAvailablePetItemsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, i => Assert.True(i.IsActive));
        Assert.All(result, i => Assert.True(i.Price > 0));
        Assert.Contains(result, i => i.Type == "Food");
        Assert.Contains(result, i => i.Type == "Toy");
        Assert.Contains(result, i => i.Type == "Cleaning");
    }

    [Fact]
    public async Task GetAvailablePetItemsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAvailablePetItemsAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.PetItems.RemoveRange(_context.PetItems);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAvailablePetItemsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count, result2.Count);
    }

    #endregion

    #region GetPetStatisticsAsync 測試

    [Fact]
    public async Task GetPetStatisticsAsync_WithValidUserId_ShouldReturnStatistics()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetPetStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCareActions);
        Assert.Equal(25, result.TotalExperienceGained);
        Assert.Equal(35, result.TotalPointsEarned);
        Assert.Equal(2, result.AchievementsUnlocked);
        Assert.Equal(3, result.TotalAchievements);
        Assert.True(result.FirstCareAction > DateTime.MinValue);
        Assert.True(result.LastCareAction > DateTime.MinValue);
        Assert.Equal(2, result.ActionCounts.Count);
        Assert.Equal(2, result.CategoryStats.Count);
    }

    [Fact]
    public async Task GetPetStatisticsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetPetStatisticsAsync(userId));
    }

    [Fact]
    public async Task GetPetStatisticsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetPetStatisticsAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.PetCareLogs.RemoveRange(_context.PetCareLogs);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetPetStatisticsAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalCareActions, result2.TotalCareActions);
        Assert.Equal(result1.TotalPointsEarned, result2.TotalPointsEarned);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task CreatePetAsync_WithUserHavingNoWallet_ShouldCreateWallet()
    {
        // Arrange
        var userId = 999; // User with no wallet
        var user = new User { user_id = userId, username = "user999", email = "user999@test.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new CreatePetRequestDto
        {
            Name = "Buddy",
            Color = "Green",
            Personality = "Loyal"
        };

        // Act
        var result = await _service.CreatePetAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);

        // 驗證錢包被創建
        var dbWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        Assert.NotNull(dbWallet);
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_WithNoCareActions_ShouldReturnEmptyHistory()
    {
        // Arrange
        var userId = 2; // User with pet but no care actions
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await _service.GetPetCareHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetUserPetAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserPetAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 10;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetPetCareHistoryAsync(userId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetPetAchievementsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetPetAchievementsAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetPetStatisticsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetPetStatisticsAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}