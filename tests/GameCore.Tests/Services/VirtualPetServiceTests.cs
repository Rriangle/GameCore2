using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs.VirtualPetDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

public class VirtualPetServiceTests
{
    private readonly GameCoreDbContext _context;
    private readonly VirtualPetService _service;
    private readonly Mock<ILogger<VirtualPetService>> _loggerMock;

    public VirtualPetServiceTests()
    {
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _loggerMock = new Mock<ILogger<VirtualPetService>>();
        _service = new VirtualPetService(_context, _loggerMock.Object);

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

        // Create pet items
        var items = new List<PetItem>
        {
            new PetItem
            {
                Id = 1,
                Name = "基礎飼料",
                Description = "營養均衡的基礎飼料",
                Type = "Food",
                Category = "Basic",
                HealthEffect = 5,
                HungerEffect = 30,
                EnergyEffect = 10,
                HappinessEffect = 5,
                CleanlinessEffect = 0,
                ExperienceEffect = 5,
                Price = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new PetItem
            {
                Id = 2,
                Name = "基礎球",
                Description = "簡單的球類玩具",
                Type = "Toy",
                Category = "Basic",
                HealthEffect = 5,
                HungerEffect = -10,
                EnergyEffect = -15,
                HappinessEffect = 20,
                CleanlinessEffect = -5,
                ExperienceEffect = 8,
                Price = 15,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new PetItem
            {
                Id = 3,
                Name = "基礎清潔劑",
                Description = "溫和的清潔劑",
                Type = "Cleaning",
                Category = "Basic",
                HealthEffect = 5,
                HungerEffect = 0,
                EnergyEffect = 0,
                HappinessEffect = 10,
                CleanlinessEffect = 40,
                ExperienceEffect = 5,
                Price = 20,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            }
        };
        _context.PetItems.AddRange(items);

        _context.SaveChanges();
    }

    [Fact]
    public async Task CreatePetAsync_FirstTime_ShouldCreateNewPet()
    {
        // Arrange
        var request = new CreatePetRequestDto
        {
            Name = "TestSlime",
            Color = "Blue",
            Personality = "Friendly"
        };

        // Act
        var result = await _service.CreatePetAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("TestSlime", result.Name);
        Assert.Equal("Blue", result.Color);
        Assert.Equal("Friendly", result.Personality);
        Assert.Equal(1, result.Level);
        Assert.Equal(0, result.Experience);
        Assert.Equal(100, result.Health);
        Assert.Equal(100, result.Hunger);
        Assert.Equal(100, result.Energy);
        Assert.Equal(100, result.Happiness);
        Assert.Equal(100, result.Cleanliness);

        // Verify database record
        var dbPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.UserId == 1);
        Assert.NotNull(dbPet);
        Assert.Equal("TestSlime", dbPet.Name);

        // Verify achievements were created
        var achievements = await _context.PetAchievements.Where(pa => pa.PetId == dbPet.Id).ToListAsync();
        Assert.Equal(4, achievements.Count);
        Assert.Contains(achievements, a => a.Name == "First Steps" && a.IsUnlocked);
    }

    [Fact]
    public async Task CreatePetAsync_UserAlreadyHasPet_ShouldThrowException()
    {
        // Arrange - Create existing pet
        var existingPet = new VirtualPet
        {
            UserId = 1,
            Name = "ExistingSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Red",
            Personality = "Shy",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.VirtualPets.Add(existingPet);
        await _context.SaveChangesAsync();

        var request = new CreatePetRequestDto
        {
            Name = "NewSlime",
            Color = "Green",
            Personality = "Energetic"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreatePetAsync(1, request));
        Assert.Equal("User already has a virtual pet", exception.Message);
    }

    [Fact]
    public async Task GetUserPetAsync_ValidUser_ShouldReturnPet()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 3,
            Experience = 150,
            ExperienceToNextLevel = 300,
            Health = 85,
            MaxHealth = 100,
            Hunger = 70,
            MaxHunger = 100,
            Energy = 90,
            MaxEnergy = 100,
            Happiness = 95,
            MaxHappiness = 100,
            Cleanliness = 80,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow.AddHours(-2),
            LastPlayed = DateTime.UtcNow.AddHours(-4),
            LastCleaned = DateTime.UtcNow.AddHours(-6),
            LastRested = DateTime.UtcNow.AddHours(-8),
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserPetAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestSlime", result.Name);
        Assert.Equal(3, result.Level);
        Assert.Equal(150, result.Experience);
        Assert.Equal(300, result.ExperienceToNextLevel);
        Assert.Equal(85, result.Health);
        Assert.Equal(70, result.Hunger);
        Assert.Equal(90, result.Energy);
        Assert.Equal(95, result.Happiness);
        Assert.Equal(80, result.Cleanliness);
        Assert.Equal("Good", result.Status);
        Assert.Contains("Hungry", result.Needs);
    }

    [Fact]
    public async Task GetUserPetAsync_UserWithoutPet_ShouldThrowException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.GetUserPetAsync(1));
        Assert.Equal("User does not have a virtual pet", exception.Message);
    }

    [Fact]
    public async Task FeedPetAsync_ValidFeeding_ShouldSucceed()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 50,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow.AddHours(-2),
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.FeedPetAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Feed", result.Action);
        Assert.Equal("TestSlime", result.PetName);
        Assert.Equal(5, result.HealthChange);
        Assert.Equal(30, result.HungerChange);
        Assert.Equal(10, result.EnergyChange);
        Assert.Equal(5, result.HappinessChange);
        Assert.Equal(0, result.CleanlinessChange);
        Assert.Equal(5, result.ExperienceGained);
        Assert.True(result.PointsEarned > 0);
        Assert.False(result.LevelUp);

        // Verify pet stats were updated
        var updatedPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.Id == pet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal(105, updatedPet.Health);
        Assert.Equal(80, updatedPet.Hunger);
        Assert.Equal(110, updatedPet.Energy);
        Assert.Equal(105, updatedPet.Happiness);
        Assert.Equal(5, updatedPet.Experience);

        // Verify care log was created
        var careLog = await _context.PetCareLogs.FirstOrDefaultAsync(pcl => pcl.PetId == pet.Id);
        Assert.NotNull(careLog);
        Assert.Equal("Feed", careLog.Action);

        // Verify wallet was updated
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(uw => uw.UserId == 1);
        Assert.NotNull(wallet);
        Assert.True(wallet.Balance > 100.00m);
    }

    [Fact]
    public async Task FeedPetAsync_TooSoon_ShouldThrowException()
    {
        // Arrange - Create pet with recent feeding
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 50,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow.AddMinutes(-30), // Too recent
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.FeedPetAsync(1, 1));
        Assert.Contains("not hungry yet", exception.Message);
    }

    [Fact]
    public async Task FeedPetAsync_InvalidItemType_ShouldThrowException()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 50,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow.AddHours(-2),
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act & Assert - Try to use a toy for feeding
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.FeedPetAsync(1, 2)); // Item ID 2 is a toy
        Assert.Contains("not valid for this action", exception.Message);
    }

    [Fact]
    public async Task PlayWithPetAsync_ValidPlay_ShouldSucceed()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 80,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow.AddHours(-3), // Ready to play
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.PlayWithPetAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Play", result.Action);
        Assert.Equal("TestSlime", result.PetName);
        Assert.Equal(5, result.HealthChange);
        Assert.Equal(-10, result.HungerChange);
        Assert.Equal(-15, result.EnergyChange);
        Assert.Equal(20, result.HappinessChange);
        Assert.Equal(-5, result.CleanlinessChange);
        Assert.Equal(8, result.ExperienceGained);

        // Verify pet stats were updated
        var updatedPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.Id == pet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal(105, updatedPet.Health);
        Assert.Equal(90, updatedPet.Hunger);
        Assert.Equal(85, updatedPet.Energy);
        Assert.Equal(100, updatedPet.Happiness);
        Assert.Equal(95, updatedPet.Cleanliness);
        Assert.Equal(8, updatedPet.Experience);
    }

    [Fact]
    public async Task CleanPetAsync_ValidCleaning_ShouldSucceed()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 60,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow.AddHours(-4), // Ready to clean
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CleanPetAsync(1, 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Clean", result.Action);
        Assert.Equal("TestSlime", result.PetName);
        Assert.Equal(5, result.HealthChange);
        Assert.Equal(0, result.HungerChange);
        Assert.Equal(0, result.EnergyChange);
        Assert.Equal(10, result.HappinessChange);
        Assert.Equal(40, result.CleanlinessChange);
        Assert.Equal(5, result.ExperienceGained);

        // Verify pet stats were updated
        var updatedPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.Id == pet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal(105, updatedPet.Health);
        Assert.Equal(100, updatedPet.Cleanliness);
        Assert.Equal(110, updatedPet.Happiness);
        Assert.Equal(5, updatedPet.Experience);
    }

    [Fact]
    public async Task RestPetAsync_ValidRest_ShouldSucceed()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 80,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 40,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow.AddHours(-5), // Ready to rest
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.RestPetAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rest", result.Action);
        Assert.Equal("TestSlime", result.PetName);
        Assert.Equal(20, result.HealthChange);
        Assert.Equal(0, result.HungerChange);
        Assert.Equal(50, result.EnergyChange);
        Assert.Equal(10, result.HappinessChange);
        Assert.Equal(0, result.CleanlinessChange);
        Assert.Equal(10, result.ExperienceGained);

        // Verify pet stats were updated
        var updatedPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.Id == pet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal(100, updatedPet.Health);
        Assert.Equal(90, updatedPet.Energy);
        Assert.Equal(110, updatedPet.Happiness);
        Assert.Equal(10, updatedPet.Experience);
    }

    [Fact]
    public async Task ChangePetColorAsync_ValidColor_ShouldSucceed()
    {
        // Arrange - Create pet
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ChangePetColorAsync(1, "Green");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Green", result.Color);

        // Verify database was updated
        var updatedPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.Id == pet.Id);
        Assert.NotNull(updatedPet);
        Assert.Equal("Green", updatedPet.Color);
    }

    [Fact]
    public async Task GetPetCareHistoryAsync_ShouldReturnPaginatedResults()
    {
        // Arrange - Create pet and care logs
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // Create care logs
        var careLogs = new List<PetCareLog>();
        for (int i = 0; i < 25; i++)
        {
            careLogs.Add(new PetCareLog
            {
                PetId = pet.Id,
                UserId = 1,
                Action = "Feed",
                Description = $"Fed {pet.Name}",
                HealthChange = 5,
                HungerChange = 30,
                EnergyChange = 10,
                HappinessChange = 5,
                CleanlinessChange = 0,
                ExperienceGained = 5,
                PointsEarned = 10,
                ActionTime = DateTime.UtcNow.AddDays(-i),
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        _context.PetCareLogs.AddRange(careLogs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPetCareHistoryAsync(1, page: 2, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task GetPetAchievementsAsync_ShouldReturnAchievements()
    {
        // Arrange - Create pet and achievements
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        var achievements = new List<PetAchievement>
        {
            new PetAchievement
            {
                PetId = pet.Id,
                Name = "First Steps",
                Description = "Created your first virtual pet",
                Category = "Creation",
                PointsReward = 50,
                IsUnlocked = true,
                UnlockedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            },
            new PetAchievement
            {
                PetId = pet.Id,
                Name = "Level 5",
                Description = "Reach level 5",
                Category = "Leveling",
                PointsReward = 100,
                IsUnlocked = false,
                UnlockedAt = null,
                CreatedAt = DateTime.UtcNow
            }
        };
        _context.PetAchievements.AddRange(achievements);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPetAchievementsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.Name == "First Steps" && a.IsUnlocked);
        Assert.Contains(result, a => a.Name == "Level 5" && !a.IsUnlocked);
    }

    [Fact]
    public async Task GetAvailablePetItemsAsync_ShouldReturnActiveItems()
    {
        // Act
        var items = await _service.GetAvailablePetItemsAsync();

        // Assert
        Assert.NotNull(items);
        Assert.Equal(3, items.Count);
        Assert.All(items, i => Assert.True(i.IsActive));
        Assert.Contains(items, i => i.Type == "Food");
        Assert.Contains(items, i => i.Type == "Toy");
        Assert.Contains(items, i => i.Type == "Cleaning");
    }

    [Fact]
    public async Task GetPetStatisticsAsync_ShouldReturnCorrectStats()
    {
        // Arrange - Create pet and care logs
        var pet = new VirtualPet
        {
            UserId = 1,
            Name = "TestSlime",
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = 100,
            Health = 100,
            MaxHealth = 100,
            Hunger = 100,
            MaxHunger = 100,
            Energy = 100,
            MaxEnergy = 100,
            Happiness = 100,
            MaxHappiness = 100,
            Cleanliness = 100,
            MaxCleanliness = 100,
            Color = "Blue",
            Personality = "Friendly",
            LastFed = DateTime.UtcNow,
            LastPlayed = DateTime.UtcNow,
            LastCleaned = DateTime.UtcNow,
            LastRested = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        var careLogs = new List<PetCareLog>
        {
            new PetCareLog
            {
                PetId = pet.Id,
                UserId = 1,
                Action = "Feed",
                Description = "Fed pet",
                HealthChange = 5,
                HungerChange = 30,
                EnergyChange = 10,
                HappinessChange = 5,
                CleanlinessChange = 0,
                ExperienceGained = 5,
                PointsEarned = 10,
                ActionTime = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new PetCareLog
            {
                PetId = pet.Id,
                UserId = 1,
                Action = "Play",
                Description = "Played with pet",
                HealthChange = 5,
                HungerChange = -10,
                EnergyChange = -15,
                HappinessChange = 20,
                CleanlinessChange = -5,
                ExperienceGained = 8,
                PointsEarned = 15,
                ActionTime = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };
        _context.PetCareLogs.AddRange(careLogs);
        await _context.SaveChangesAsync();

        // Act
        var stats = await _service.GetPetStatisticsAsync(1);

        // Assert
        Assert.NotNull(stats);
        Assert.Equal(2, stats.TotalCareActions);
        Assert.Equal(13, stats.TotalExperienceGained);
        Assert.Equal(25, stats.TotalPointsEarned);
        Assert.Equal(1, stats.AchievementsUnlocked);
        Assert.Equal(4, stats.TotalAchievements);
        Assert.Equal(2, stats.ActionCounts.Count);
        Assert.Equal(1, stats.ActionCounts["Feed"]);
        Assert.Equal(1, stats.ActionCounts["Play"]);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}