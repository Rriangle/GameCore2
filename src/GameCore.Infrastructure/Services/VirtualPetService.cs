using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs.VirtualPetDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 虛擬寵物服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class VirtualPetService : IVirtualPetService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<VirtualPetService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 30;
    private const int MaxPetNameLength = 50;
    private const int MaxPetColorLength = 30;
    private const int MaxPetPersonalityLength = 100;
    
    // 快取鍵定義
    private const string UserPetCacheKey = "UserPet_{0}";
    private const string PetCareHistoryCacheKey = "PetCareHistory_{0}_{1}_{2}";
    private const string PetAchievementsCacheKey = "PetAchievements_{0}";
    private const string AvailablePetItemsCacheKey = "AvailablePetItems";
    private const string PetStatisticsCacheKey = "PetStatistics_{0}";

    public VirtualPetService(
        GameCoreDbContext context, 
        IMemoryCache memoryCache, 
        ILogger<VirtualPetService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VirtualPetResponseDto> CreatePetAsync(int userId, CreatePetRequestDto request)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Pet name cannot be empty", nameof(request.Name));
        }
        if (request.Name.Length > MaxPetNameLength)
        {
            throw new ArgumentException($"Pet name cannot exceed {MaxPetNameLength} characters", nameof(request.Name));
        }
        if (string.IsNullOrWhiteSpace(request.Color))
        {
            throw new ArgumentException("Pet color cannot be empty", nameof(request.Color));
        }
        if (request.Color.Length > MaxPetColorLength)
        {
            throw new ArgumentException($"Pet color cannot exceed {MaxPetColorLength} characters", nameof(request.Color));
        }
        if (!string.IsNullOrWhiteSpace(request.Personality) && request.Personality.Length > MaxPetPersonalityLength)
        {
            throw new ArgumentException($"Pet personality cannot exceed {MaxPetPersonalityLength} characters", nameof(request.Personality));
        }

        // 檢查用戶是否已經有寵物
        var existingPet = await _context.VirtualPets
            .AsNoTracking()
            .FirstOrDefaultAsync(vp => vp.UserId == userId);

        if (existingPet != null)
        {
            throw new InvalidOperationException("User already has a virtual pet");
        }

        var now = DateTime.UtcNow;
        var pet = new VirtualPet
        {
            UserId = userId,
            Name = request.Name,
            Level = 1,
            Experience = 0,
            ExperienceToNextLevel = CalculateExperienceToNextLevel(1),
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
            Color = request.Color,
            Personality = request.Personality,
            LastFed = now,
            LastPlayed = now,
            LastCleaned = now,
            LastRested = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.VirtualPets.Add(pet);
        await _context.SaveChangesAsync();

        // 創建初始成就
        await CreateInitialAchievementsAsync(pet.Id);

        // 清除相關快取
        ClearUserPetRelatedCache(userId);

        _logger.LogInformation("Created virtual pet {PetName} for user {UserId}", pet.Name, userId);

        return await MapToVirtualPetResponseDto(pet);
    }

    public async Task<VirtualPetResponseDto> GetUserPetAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(UserPetCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out VirtualPetResponseDto cachedPet))
        {
            return cachedPet;
        }

        var pet = await _context.VirtualPets
            .AsNoTracking()
            .FirstOrDefaultAsync(vp => vp.UserId == userId);

        if (pet == null)
        {
            throw new InvalidOperationException("User does not have a virtual pet");
        }

        var petResponse = await MapToVirtualPetResponseDto(pet);

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, petResponse, cacheOptions);

        return petResponse;
    }

    public async Task<PetCareResponseDto> FeedPetAsync(int userId, int itemId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (itemId <= 0)
        {
            throw new ArgumentException("Item ID must be greater than 0", nameof(itemId));
        }

        var pet = await GetPetWithValidation(userId);
        var item = await GetPetItemWithValidation(itemId, "Food");

        if (DateTime.UtcNow.Subtract(pet.LastFed).TotalHours < 1)
        {
            throw new InvalidOperationException("Pet is not hungry yet. Wait at least 1 hour between feedings.");
        }

        var now = DateTime.UtcNow;
        var oldLevel = pet.Level;

        // Apply item effects
        pet.Health = Math.Min(pet.MaxHealth, pet.Health + item.HealthEffect);
        pet.Hunger = Math.Min(pet.MaxHunger, pet.Hunger + item.HungerEffect);
        pet.Energy = Math.Min(pet.MaxEnergy, pet.Energy + item.EnergyEffect);
        pet.Happiness = Math.Min(pet.MaxHappiness, pet.Happiness + item.HappinessEffect);
        pet.Cleanliness = Math.Min(pet.MaxCleanliness, pet.Cleanliness + item.CleanlinessEffect);
        pet.Experience += item.ExperienceEffect;
        pet.LastFed = now;
        pet.UpdatedAt = now;

        // Check for level up
        var levelUp = false;
        var newLevel = pet.Level;
        while (pet.Experience >= pet.ExperienceToNextLevel)
        {
            pet.Level++;
            pet.Experience -= pet.ExperienceToNextLevel;
            pet.ExperienceToNextLevel = CalculateExperienceToNextLevel(pet.Level);
            levelUp = true;
            newLevel = pet.Level;
        }

        // Create care log
        var careLog = new PetCareLog
        {
            PetId = pet.Id,
            UserId = userId,
            Action = "Feed",
            Description = $"Fed {pet.Name} with {item.Name}",
            HealthChange = item.HealthEffect,
            HungerChange = item.HungerEffect,
            EnergyChange = item.EnergyEffect,
            HappinessChange = item.HappinessEffect,
            CleanlinessChange = item.CleanlinessEffect,
            ExperienceGained = item.ExperienceEffect,
            PointsEarned = CalculatePointsEarned("Feed", item.ExperienceEffect),
            ActionTime = now,
            CreatedAt = now
        };

        _context.PetCareLogs.Add(careLog);

        // Update user wallet with points
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        if (userWallet != null)
        {
            userWallet.Balance += careLog.PointsEarned;
            userWallet.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // 檢查新成就
        var achievements = await CheckForNewAchievementsAsync(pet.Id);

        // 清除相關快取
        ClearUserPetRelatedCache(userId);

        _logger.LogInformation("User {UserId} fed pet {PetName} with {ItemName}", userId, pet.Name, item.Name);

        return new PetCareResponseDto
        {
            PetId = pet.Id,
            PetName = pet.Name,
            Action = "Feed",
            Description = careLog.Description,
            HealthChange = item.HealthEffect,
            HungerChange = item.HungerEffect,
            EnergyChange = item.EnergyEffect,
            HappinessChange = item.HappinessEffect,
            CleanlinessChange = item.CleanlinessEffect,
            ExperienceGained = item.ExperienceEffect,
            PointsEarned = careLog.PointsEarned,
            LevelUp = levelUp,
            NewLevel = newLevel,
            Achievements = achievements,
            Message = levelUp ? $"Congratulations! {pet.Name} reached level {newLevel}!" : $"{pet.Name} enjoyed the {item.Name}!"
        };
    }

    public async Task<PetCareResponseDto> PlayWithPetAsync(int userId, int itemId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (itemId <= 0)
        {
            throw new ArgumentException("Item ID must be greater than 0", nameof(itemId));
        }

        var pet = await GetPetWithValidation(userId);
        var item = await GetPetItemWithValidation(itemId, "Toy");

        if (DateTime.UtcNow.Subtract(pet.LastPlayed).TotalHours < 2)
        {
            throw new InvalidOperationException("Pet is not ready to play yet. Wait at least 2 hours between play sessions.");
        }

        var now = DateTime.UtcNow;
        var oldLevel = pet.Level;

        // Apply item effects
        pet.Health = Math.Min(pet.MaxHealth, pet.Health + item.HealthEffect);
        pet.Hunger = Math.Max(0, pet.Hunger + item.HungerEffect);
        pet.Energy = Math.Max(0, pet.Energy + item.EnergyEffect);
        pet.Happiness = Math.Min(pet.MaxHappiness, pet.Happiness + item.HappinessEffect);
        pet.Cleanliness = Math.Max(0, pet.Cleanliness + item.CleanlinessEffect);
        pet.Experience += item.ExperienceEffect;
        pet.LastPlayed = now;
        pet.UpdatedAt = now;

        // Check for level up
        var levelUp = false;
        var newLevel = pet.Level;
        while (pet.Experience >= pet.ExperienceToNextLevel)
        {
            pet.Level++;
            pet.Experience -= pet.ExperienceToNextLevel;
            pet.ExperienceToNextLevel = CalculateExperienceToNextLevel(pet.Level);
            levelUp = true;
            newLevel = pet.Level;
        }

        // Create care log
        var careLog = new PetCareLog
        {
            PetId = pet.Id,
            UserId = userId,
            Action = "Play",
            Description = $"Played with {pet.Name} using {item.Name}",
            HealthChange = item.HealthEffect,
            HungerChange = item.HungerEffect,
            EnergyChange = item.EnergyEffect,
            HappinessChange = item.HappinessEffect,
            CleanlinessChange = item.CleanlinessEffect,
            ExperienceGained = item.ExperienceEffect,
            PointsEarned = CalculatePointsEarned("Play", item.ExperienceEffect),
            ActionTime = now,
            CreatedAt = now
        };

        _context.PetCareLogs.Add(careLog);

        // Update user wallet with points
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        if (userWallet != null)
        {
            userWallet.Balance += careLog.PointsEarned;
            userWallet.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // 檢查新成就
        var achievements = await CheckForNewAchievementsAsync(pet.Id);

        // 清除相關快取
        ClearUserPetRelatedCache(userId);

        _logger.LogInformation("User {UserId} played with pet {PetName} using {ItemName}", userId, pet.Name, item.Name);

        return new PetCareResponseDto
        {
            PetId = pet.Id,
            PetName = pet.Name,
            Action = "Play",
            Description = careLog.Description,
            HealthChange = item.HealthEffect,
            HungerChange = item.HungerEffect,
            EnergyChange = item.EnergyEffect,
            HappinessChange = item.HappinessEffect,
            CleanlinessChange = item.CleanlinessEffect,
            ExperienceGained = item.ExperienceEffect,
            PointsEarned = careLog.PointsEarned,
            LevelUp = levelUp,
            NewLevel = newLevel,
            Achievements = achievements,
            Message = levelUp ? $"Congratulations! {pet.Name} reached level {newLevel}!" : $"{pet.Name} had fun playing with {item.Name}!"
        };
    }

    public async Task<PetCareResponseDto> CleanPetAsync(int userId, int itemId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (itemId <= 0)
        {
            throw new ArgumentException("Item ID must be greater than 0", nameof(itemId));
        }

        var pet = await GetPetWithValidation(userId);
        var item = await GetPetItemWithValidation(itemId, "Cleaning");

        if (DateTime.UtcNow.Subtract(pet.LastCleaned).TotalHours < 3)
        {
            throw new InvalidOperationException("Pet is not dirty yet. Wait at least 3 hours between cleanings.");
        }

        var now = DateTime.UtcNow;
        var oldLevel = pet.Level;

        // Apply item effects
        pet.Health = Math.Min(pet.MaxHealth, pet.Health + item.HealthEffect);
        pet.Hunger = Math.Min(pet.MaxHunger, pet.Hunger + item.HungerEffect);
        pet.Energy = Math.Min(pet.MaxEnergy, pet.Energy + item.EnergyEffect);
        pet.Happiness = Math.Min(pet.MaxHappiness, pet.Happiness + item.HappinessEffect);
        pet.Cleanliness = Math.Min(pet.MaxCleanliness, pet.Cleanliness + item.CleanlinessEffect);
        pet.Experience += item.ExperienceEffect;
        pet.LastCleaned = now;
        pet.UpdatedAt = now;

        // Check for level up
        var levelUp = false;
        var newLevel = pet.Level;
        while (pet.Experience >= pet.ExperienceToNextLevel)
        {
            pet.Level++;
            pet.Experience -= pet.ExperienceToNextLevel;
            pet.ExperienceToNextLevel = CalculateExperienceToNextLevel(pet.Level);
            levelUp = true;
            newLevel = pet.Level;
        }

        // Create care log
        var careLog = new PetCareLog
        {
            PetId = pet.Id,
            UserId = userId,
            Action = "Clean",
            Description = $"Cleaned {pet.Name} with {item.Name}",
            HealthChange = item.HealthEffect,
            HungerChange = item.HungerEffect,
            EnergyChange = item.EnergyEffect,
            HappinessChange = item.HappinessEffect,
            CleanlinessChange = item.CleanlinessEffect,
            ExperienceGained = item.ExperienceEffect,
            PointsEarned = CalculatePointsEarned("Clean", item.ExperienceEffect),
            ActionTime = now,
            CreatedAt = now
        };

        _context.PetCareLogs.Add(careLog);

        // Update user wallet with points
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        if (userWallet != null)
        {
            userWallet.Balance += careLog.PointsEarned;
            userWallet.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // 檢查新成就
        var achievements = await CheckForNewAchievementsAsync(pet.Id);

        // 清除相關快取
        ClearUserPetRelatedCache(userId);

        _logger.LogInformation("User {UserId} cleaned pet {PetName} with {ItemName}", userId, pet.Name, item.Name);

        return new PetCareResponseDto
        {
            PetId = pet.Id,
            PetName = pet.Name,
            Action = "Clean",
            Description = careLog.Description,
            HealthChange = item.HealthEffect,
            HungerChange = item.HungerEffect,
            EnergyChange = item.EnergyEffect,
            HappinessChange = item.HappinessEffect,
            CleanlinessChange = item.CleanlinessEffect,
            ExperienceGained = item.ExperienceEffect,
            PointsEarned = careLog.PointsEarned,
            LevelUp = levelUp,
            NewLevel = newLevel,
            Achievements = achievements,
            Message = levelUp ? $"Congratulations! {pet.Name} reached level {newLevel}!" : $"{pet.Name} is now clean and happy!"
        };
    }

    public async Task<PetCareResponseDto> RestPetAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        var pet = await GetPetWithValidation(userId);

        if (DateTime.UtcNow.Subtract(pet.LastRested).TotalHours < 4)
        {
            throw new InvalidOperationException("Pet is not tired yet. Wait at least 4 hours between rest sessions.");
        }

        var now = DateTime.UtcNow;
        var oldLevel = pet.Level;

        // Rest effects
        var energyGain = 50;
        var healthGain = 20;
        var experienceGain = 10;

        pet.Health = Math.Min(pet.MaxHealth, pet.Health + healthGain);
        pet.Energy = Math.Min(pet.MaxEnergy, pet.Energy + energyGain);
        pet.Experience += experienceGain;
        pet.LastRested = now;
        pet.UpdatedAt = now;

        // Check for level up
        var levelUp = false;
        var newLevel = pet.Level;
        while (pet.Experience >= pet.ExperienceToNextLevel)
        {
            pet.Level++;
            pet.Experience -= pet.ExperienceToNextLevel;
            pet.ExperienceToNextLevel = CalculateExperienceToNextLevel(pet.Level);
            levelUp = true;
            newLevel = pet.Level;
        }

        // Create care log
        var careLog = new PetCareLog
        {
            PetId = pet.Id,
            UserId = userId,
            Action = "Rest",
            Description = $"{pet.Name} took a rest",
            HealthChange = healthGain,
            HungerChange = 0,
            EnergyChange = energyGain,
            HappinessChange = 10,
            CleanlinessChange = 0,
            ExperienceGained = experienceGain,
            PointsEarned = CalculatePointsEarned("Rest", experienceGain),
            ActionTime = now,
            CreatedAt = now
        };

        _context.PetCareLogs.Add(careLog);

        // Update user wallet with points
        var userWallet = await _context.UserWallets
            .FirstOrDefaultAsync(uw => uw.UserId == userId);
        if (userWallet != null)
        {
            userWallet.Balance += careLog.PointsEarned;
            userWallet.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // 檢查新成就
        var achievements = await CheckForNewAchievementsAsync(pet.Id);

        // 清除相關快取
        ClearUserPetRelatedCache(userId);

        _logger.LogInformation("User {UserId} let pet {PetName} rest", userId, pet.Name);

        return new PetCareResponseDto
        {
            PetId = pet.Id,
            PetName = pet.Name,
            Action = "Rest",
            Description = careLog.Description,
            HealthChange = healthGain,
            HungerChange = 0,
            EnergyChange = energyGain,
            HappinessChange = 10,
            CleanlinessChange = 0,
            ExperienceGained = experienceGain,
            PointsEarned = careLog.PointsEarned,
            LevelUp = levelUp,
            NewLevel = newLevel,
            Achievements = achievements,
            Message = levelUp ? $"Congratulations! {pet.Name} reached level {newLevel}!" : $"{pet.Name} is well rested now!"
        };
    }

    public async Task<VirtualPetResponseDto> ChangePetColorAsync(int userId, string newColor)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (string.IsNullOrWhiteSpace(newColor))
        {
            throw new ArgumentException("New color cannot be empty", nameof(newColor));
        }
        if (newColor.Length > MaxPetColorLength)
        {
            throw new ArgumentException($"New color cannot exceed {MaxPetColorLength} characters", nameof(newColor));
        }

        var pet = await GetPetWithValidation(userId);

        var oldColor = pet.Color;
        pet.Color = newColor;
        pet.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 清除相關快取
        ClearUserPetRelatedCache(userId);

        _logger.LogInformation("User {UserId} changed pet {PetName} color from {OldColor} to {NewColor}", 
            userId, pet.Name, oldColor, newColor);

        return await MapToVirtualPetResponseDto(pet);
    }

    public async Task<PetCareHistoryResponseDto> GetPetCareHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }
        if (page < 1)
        {
            throw new ArgumentException("Page must be greater than 0", nameof(page));
        }
        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }
        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(PetCareHistoryCacheKey, userId, page, pageSize);
        if (_memoryCache.TryGetValue(cacheKey, out PetCareHistoryResponseDto cachedHistory))
        {
            return cachedHistory;
        }

        var pet = await _context.VirtualPets
            .AsNoTracking()
            .FirstOrDefaultAsync(vp => vp.UserId == userId);

        if (pet == null)
        {
            throw new InvalidOperationException("User does not have a virtual pet");
        }

        var query = _context.PetCareLogs
            .AsNoTracking()
            .Where(pcl => pcl.PetId == pet.Id)
            .OrderByDescending(pcl => pcl.ActionTime);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(pcl => new PetCareLogDto
            {
                Id = pcl.Id,
                Action = pcl.Action,
                Description = pcl.Description,
                HealthChange = pcl.HealthChange,
                HungerChange = pcl.HungerChange,
                EnergyChange = pcl.EnergyChange,
                HappinessChange = pcl.HappinessChange,
                CleanlinessChange = pcl.CleanlinessChange,
                ExperienceGained = pcl.ExperienceGained,
                PointsEarned = pcl.PointsEarned,
                ActionTime = pcl.ActionTime
            })
            .ToListAsync();

        var history = new PetCareHistoryResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, history, cacheOptions);

        return history;
    }

    public async Task<List<PetAchievementDto>> GetPetAchievementsAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(PetAchievementsCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out List<PetAchievementDto> cachedAchievements))
        {
            return cachedAchievements;
        }

        var pet = await _context.VirtualPets
            .AsNoTracking()
            .FirstOrDefaultAsync(vp => vp.UserId == userId);

        if (pet == null)
        {
            throw new InvalidOperationException("User does not have a virtual pet");
        }

        var achievements = await _context.PetAchievements
            .AsNoTracking()
            .Where(pa => pa.PetId == pet.Id)
            .OrderBy(pa => pa.Category)
            .ThenBy(pa => pa.Name)
            .Select(pa => new PetAchievementDto
            {
                Id = pa.Id,
                Name = pa.Name,
                Description = pa.Description,
                Category = pa.Category,
                PointsReward = pa.PointsReward,
                IsUnlocked = pa.IsUnlocked,
                UnlockedAt = pa.UnlockedAt
            })
            .ToListAsync();

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, achievements, cacheOptions);

        return achievements;
    }

    public async Task<List<PetItemDto>> GetAvailablePetItemsAsync()
    {
        // 嘗試從快取獲取
        if (_memoryCache.TryGetValue(AvailablePetItemsCacheKey, out List<PetItemDto> cachedItems))
        {
            return cachedItems;
        }

        var items = await _context.PetItems
            .AsNoTracking()
            .Where(pi => pi.IsActive)
            .OrderBy(pi => pi.Category)
            .ThenBy(pi => pi.Name)
            .Select(pi => new PetItemDto
            {
                Id = pi.Id,
                Name = pi.Name,
                Description = pi.Description,
                Type = pi.Type,
                Category = pi.Category,
                HealthEffect = pi.HealthEffect,
                HungerEffect = pi.HungerEffect,
                EnergyEffect = pi.EnergyEffect,
                HappinessEffect = pi.HappinessEffect,
                CleanlinessEffect = pi.CleanlinessEffect,
                ExperienceEffect = pi.ExperienceEffect,
                Price = pi.Price,
                IsActive = pi.IsActive
            })
            .ToListAsync();

        // 存入快取，物品列表變化較少，可以設定較長的過期時間
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };
        _memoryCache.Set(AvailablePetItemsCacheKey, items, cacheOptions);

        return items;
    }

    public async Task<PetStatisticsDto> GetPetStatisticsAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(userId));
        }

        // 嘗試從快取獲取
        var cacheKey = string.Format(PetStatisticsCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out PetStatisticsDto cachedStats))
        {
            return cachedStats;
        }

        var pet = await _context.VirtualPets
            .AsNoTracking()
            .FirstOrDefaultAsync(vp => vp.UserId == userId);

        if (pet == null)
        {
            throw new InvalidOperationException("User does not have a virtual pet");
        }

        var careLogs = await _context.PetCareLogs
            .AsNoTracking()
            .Where(pcl => pcl.PetId == pet.Id)
            .ToListAsync();

        var achievements = await _context.PetAchievements
            .AsNoTracking()
            .Where(pa => pa.PetId == pet.Id)
            .ToListAsync();

        var actionCounts = careLogs
            .GroupBy(pcl => pcl.Action)
            .ToDictionary(g => g.Key, g => g.Count());

        var categoryStats = careLogs
            .GroupBy(pcl => pcl.Action)
            .ToDictionary(g => g.Key, g => g.Sum(pcl => pcl.PointsEarned));

        var statistics = new PetStatisticsDto
        {
            TotalCareActions = careLogs.Count,
            TotalExperienceGained = careLogs.Sum(pcl => pcl.ExperienceGained),
            TotalPointsEarned = careLogs.Sum(pcl => pcl.PointsEarned),
            AchievementsUnlocked = achievements.Count(pa => pa.IsUnlocked),
            TotalAchievements = achievements.Count,
            FirstCareAction = careLogs.Any() ? careLogs.Min(pcl => pcl.ActionTime) : DateTime.MinValue,
            LastCareAction = careLogs.Any() ? careLogs.Max(pcl => pcl.ActionTime) : DateTime.MinValue,
            ActionCounts = actionCounts,
            CategoryStats = categoryStats
        };

        // 存入快取
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };
        _memoryCache.Set(cacheKey, statistics, cacheOptions);

        return statistics;
    }

    public async Task ProcessPetStatusDecayAsync()
    {
        var pets = await _context.VirtualPets.ToListAsync();
        var now = DateTime.UtcNow;

        foreach (var pet in pets)
        {
            var hoursSinceLastFed = now.Subtract(pet.LastFed).TotalHours;
            var hoursSinceLastPlayed = now.Subtract(pet.LastPlayed).TotalHours;
            var hoursSinceLastCleaned = now.Subtract(pet.LastCleaned).TotalHours;
            var hoursSinceLastRested = now.Subtract(pet.LastRested).TotalHours;

            // Decay stats over time
            if (hoursSinceLastFed > 6)
            {
                pet.Hunger = Math.Max(0, pet.Hunger - (int)(hoursSinceLastFed - 6) * 5);
            }

            if (hoursSinceLastPlayed > 8)
            {
                pet.Happiness = Math.Max(0, pet.Happiness - (int)(hoursSinceLastPlayed - 8) * 3);
            }

            if (hoursSinceLastCleaned > 12)
            {
                pet.Cleanliness = Math.Max(0, pet.Cleanliness - (int)(hoursSinceLastCleaned - 12) * 4);
            }

            if (hoursSinceLastRested > 10)
            {
                pet.Energy = Math.Max(0, pet.Energy - (int)(hoursSinceLastRested - 10) * 6);
            }

            // Health decay based on other stats
            var healthDecay = 0;
            if (pet.Hunger < 20) healthDecay += 2;
            if (pet.Happiness < 20) healthDecay += 1;
            if (pet.Cleanliness < 20) healthDecay += 1;
            if (pet.Energy < 20) healthDecay += 1;

            pet.Health = Math.Max(0, pet.Health - healthDecay);
            pet.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();

        // 清除所有寵物相關的快取
        ClearAllPetRelatedCache();

        _logger.LogInformation("Processed status decay for {PetCount} pets", pets.Count);
    }

    private async Task<VirtualPet> GetPetWithValidation(int userId)
    {
        var pet = await _context.VirtualPets
            .FirstOrDefaultAsync(vp => vp.UserId == userId);

        if (pet == null)
        {
            throw new InvalidOperationException("User does not have a virtual pet");
        }

        return pet;
    }

    private async Task<PetItem> GetPetItemWithValidation(int itemId, string expectedType)
    {
        var item = await _context.PetItems
            .FirstOrDefaultAsync(pi => pi.Id == itemId && pi.IsActive);

        if (item == null)
        {
            throw new InvalidOperationException("Pet item not found or not active");
        }

        if (item.Type != expectedType)
        {
            throw new InvalidOperationException($"Item type {item.Type} is not valid for this action. Expected {expectedType}.");
        }

        return item;
    }

    private int CalculateExperienceToNextLevel(int level)
    {
        return level * 100 + (level - 1) * 50;
    }

    private int CalculatePointsEarned(string action, int experienceGained)
    {
        var basePoints = action switch
        {
            "Feed" => 5,
            "Play" => 8,
            "Clean" => 6,
            "Rest" => 4,
            _ => 3
        };

        return basePoints + (experienceGained / 10);
    }

    private async Task CreateInitialAchievementsAsync(int petId)
    {
        var achievements = new List<PetAchievement>
        {
            new PetAchievement
            {
                PetId = petId,
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
                PetId = petId,
                Name = "Level 5",
                Description = "Reach level 5",
                Category = "Leveling",
                PointsReward = 100,
                IsUnlocked = false,
                UnlockedAt = null,
                CreatedAt = DateTime.UtcNow
            },
            new PetAchievement
            {
                PetId = petId,
                Name = "Level 10",
                Description = "Reach level 10",
                Category = "Leveling",
                PointsReward = 200,
                IsUnlocked = false,
                UnlockedAt = null,
                CreatedAt = DateTime.UtcNow
            },
            new PetAchievement
            {
                PetId = petId,
                Name = "Care Master",
                Description = "Perform 100 care actions",
                Category = "Care",
                PointsReward = 150,
                IsUnlocked = false,
                UnlockedAt = null,
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.PetAchievements.AddRange(achievements);
    }

    private async Task<List<string>> CheckForNewAchievementsAsync(int petId)
    {
        var pet = await _context.VirtualPets
            .Include(vp => vp.CareLogs)
            .Include(vp => vp.Achievements)
            .FirstOrDefaultAsync(vp => vp.Id == petId);

        if (pet == null) return new List<string>();

        var newAchievements = new List<string>();

        // Check level achievements
        var level5Achievement = pet.Achievements.FirstOrDefault(a => a.Name == "Level 5" && !a.IsUnlocked);
        if (level5Achievement != null && pet.Level >= 5)
        {
            level5Achievement.IsUnlocked = true;
            level5Achievement.UnlockedAt = DateTime.UtcNow;
            newAchievements.Add("Level 5");
        }

        var level10Achievement = pet.Achievements.FirstOrDefault(a => a.Name == "Level 10" && !a.IsUnlocked);
        if (level10Achievement != null && pet.Level >= 10)
        {
            level10Achievement.IsUnlocked = true;
            level10Achievement.UnlockedAt = DateTime.UtcNow;
            newAchievements.Add("Level 10");
        }

        // Check care achievements
        var careMasterAchievement = pet.Achievements.FirstOrDefault(a => a.Name == "Care Master" && !a.IsUnlocked);
        if (careMasterAchievement != null && pet.CareLogs.Count >= 100)
        {
            careMasterAchievement.IsUnlocked = true;
            careMasterAchievement.UnlockedAt = DateTime.UtcNow;
            newAchievements.Add("Care Master");
        }

        if (newAchievements.Any())
        {
            await _context.SaveChangesAsync();
        }

        return newAchievements;
    }

    private async Task<VirtualPetResponseDto> MapToVirtualPetResponseDto(VirtualPet pet)
    {
        var needs = new List<string>();
        if (pet.Hunger < 30) needs.Add("Hungry");
        if (pet.Energy < 30) needs.Add("Tired");
        if (pet.Happiness < 30) needs.Add("Unhappy");
        if (pet.Cleanliness < 30) needs.Add("Dirty");
        if (pet.Health < 50) needs.Add("Sick");

        var status = pet.Health switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Fair",
            >= 20 => "Poor",
            _ => "Critical"
        };

        return new VirtualPetResponseDto
        {
            Id = pet.Id,
            UserId = pet.UserId,
            Name = pet.Name,
            Level = pet.Level,
            Experience = pet.Experience,
            ExperienceToNextLevel = pet.ExperienceToNextLevel,
            Health = pet.Health,
            MaxHealth = pet.MaxHealth,
            Hunger = pet.Hunger,
            MaxHunger = pet.MaxHunger,
            Energy = pet.Energy,
            MaxEnergy = pet.MaxEnergy,
            Happiness = pet.Happiness,
            MaxHappiness = pet.MaxHappiness,
            Cleanliness = pet.Cleanliness,
            MaxCleanliness = pet.MaxCleanliness,
            Color = pet.Color,
            Personality = pet.Personality,
            LastFed = pet.LastFed,
            LastPlayed = pet.LastPlayed,
            LastCleaned = pet.LastCleaned,
            LastRested = pet.LastRested,
            CreatedAt = pet.CreatedAt,
            Status = status,
            Needs = needs
        };
    }

    #region 快取管理

    /// <summary>
    /// 清除用戶寵物相關的快取
    /// </summary>
    private void ClearUserPetRelatedCache(int userId)
    {
        var userPetKey = string.Format(UserPetCacheKey, userId);
        var petAchievementsKey = string.Format(PetAchievementsCacheKey, userId);
        var petStatisticsKey = string.Format(PetStatisticsCacheKey, userId);

        _memoryCache.Remove(userPetKey);
        _memoryCache.Remove(petAchievementsKey);
        _memoryCache.Remove(petStatisticsKey);

        // 清除分頁護理歷史記錄快取（需要遍歷所有可能的頁面）
        for (int page = 1; page <= 10; page++) // 假設最多10頁
        {
            for (int pageSize = 10; pageSize <= MaxPageSize; pageSize += 10)
            {
                var historyKey = string.Format(PetCareHistoryCacheKey, userId, page, pageSize);
                _memoryCache.Remove(historyKey);
            }
        }

        _logger.LogDebug("Cleared cache for user pet {UserId}", userId);
    }

    /// <summary>
    /// 清除所有寵物相關的快取
    /// </summary>
    private void ClearAllPetRelatedCache()
    {
        _memoryCache.Remove(AvailablePetItemsCacheKey);
        _logger.LogDebug("Cleared all pet-related cache");
    }

    #endregion
}