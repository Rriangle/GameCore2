using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs.AdventureDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 冒險遊戲服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class AdventureService : IAdventureService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<AdventureService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 30;
    private const int MaxMonsterEncounters = 10;
    private const int MinAdventureDuration = 5;
    private const int MaxAdventureDuration = 480; // 8 hours
    private const int MinEnergyCost = 1;
    private const int MaxEnergyCost = 100;
    private const int MinLevel = 1;
    private const int MaxLevel = 100;
    
    // 快取鍵定義
    private const string AvailableTemplatesCacheKey = "AvailableTemplates_{0}";
    private const string TemplateCacheKey = "Template_{0}";
    private const string UserAdventureLogsCacheKey = "UserAdventureLogs_{0}_{1}_{2}";
    private const string AdventureLogCacheKey = "AdventureLog_{0}";
    private const string UserAdventureStatisticsCacheKey = "UserAdventureStatistics_{0}";
    private const string AdventureProgressCacheKey = "AdventureProgress_{0}_{1}";
    private const string MonsterEncountersCacheKey = "MonsterEncounters_{0}";

    public AdventureService(
        GameCoreDbContext context, 
        IMemoryCache memoryCache, 
        ILogger<AdventureService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AdventureResponseDto> StartAdventureAsync(int userId, int templateId)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (templateId <= 0)
            throw new ArgumentException("Template ID must be a positive integer", nameof(templateId));

        var user = await _context.Users
            .Include(u => u.UserWallet)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new InvalidOperationException("User not found");

        var template = await _context.AdventureTemplates
            .FirstOrDefaultAsync(u => u.Id == templateId && u.IsActive);

        if (template == null)
            throw new InvalidOperationException("Adventure template not found");

        // Check if user can start adventure
        if (!await CanStartAdventureAsync(userId, templateId))
            throw new InvalidOperationException("Cannot start adventure. Check level and energy requirements.");

        // Check if user already has an active adventure
        var activeAdventure = await _context.Adventures
            .FirstOrDefaultAsync(a => a.UserId == userId && a.AdventureLogs.Any(al => al.Status == "InProgress"));

        if (activeAdventure != null)
            throw new InvalidOperationException("User already has an active adventure");

        // Deduct energy from user wallet
        var wallet = user.UserWallet;
        if (wallet.Balance < template.EnergyCost)
            throw new InvalidOperationException("Insufficient energy to start adventure");

        wallet.Balance -= template.EnergyCost;
        wallet.UpdatedAt = DateTime.UtcNow;

        // Create adventure
        var adventure = new Adventure
        {
            UserId = userId,
            Title = template.Name,
            Description = template.Description,
            Difficulty = template.Difficulty,
            RequiredLevel = template.MinLevel,
            RequiredEnergy = template.EnergyCost,
            DurationMinutes = template.DurationMinutes,
            SuccessRate = template.BaseSuccessRate,
            BaseExperienceReward = template.BaseExperienceReward,
            BasePointsReward = template.BasePointsReward,
            BaseGoldReward = template.BaseGoldReward,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Adventures.Add(adventure);
        await _context.SaveChangesAsync();

        // Create adventure log
        var adventureLog = new AdventureLog
        {
            AdventureId = adventure.Id,
            UserId = userId,
            Status = "InProgress",
            StartedAt = DateTime.UtcNow,
            EnergySpent = template.EnergyCost,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AdventureLogs.Add(adventureLog);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} started adventure {AdventureId} with template {TemplateId}", 
            userId, adventure.Id, templateId);

        // 清除相關快取
        ClearUserAdventureRelatedCache(userId);
        ClearAdventureRelatedCache(adventure.Id, userId);

        return await GetAdventureAsync(adventure.Id, userId);
    }

    public async Task<AdventureResponseDto> GetAdventureAsync(int adventureId, int userId)
    {
        // 輸入驗證
        if (adventureId <= 0)
            throw new ArgumentException("Adventure ID must be a positive integer", nameof(adventureId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        var adventure = await _context.Adventures
            .Include(a => a.AdventureLogs)
            .ThenInclude(al => al.MonsterEncounters)
            .FirstOrDefaultAsync(a => a.Id == adventureId && a.UserId == userId);

        if (adventure == null)
            throw new InvalidOperationException("Adventure not found");

        var activeLog = adventure.AdventureLogs.FirstOrDefault(al => al.Status == "InProgress");
        if (activeLog == null)
            throw new InvalidOperationException("No active adventure log found");

        var progress = await GetAdventureProgressAsync(adventureId, userId);

        return new AdventureResponseDto
        {
            Id = adventure.Id,
            UserId = adventure.UserId,
            Title = adventure.Title,
            Description = adventure.Description,
            Difficulty = adventure.Difficulty,
            RequiredLevel = adventure.RequiredLevel,
            RequiredEnergy = adventure.RequiredEnergy,
            DurationMinutes = adventure.DurationMinutes,
            SuccessRate = adventure.SuccessRate,
            Status = activeLog.Status,
            StartedAt = activeLog.StartedAt,
            CompletedAt = activeLog.CompletedAt,
            FailedAt = activeLog.FailedAt,
            EnergySpent = activeLog.EnergySpent,
            ExperienceGained = activeLog.ExperienceGained,
            PointsGained = activeLog.PointsGained,
            GoldGained = activeLog.GoldGained,
            Progress = progress
        };
    }

    public async Task<AdventureResponseDto> CompleteAdventureAsync(int adventureId, int userId)
    {
        // 輸入驗證
        if (adventureId <= 0)
            throw new ArgumentException("Adventure ID must be a positive integer", nameof(adventureId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        var adventure = await _context.Adventures
            .Include(a => a.AdventureLogs)
            .FirstOrDefaultAsync(a => a.Id == adventureId && a.UserId == userId);

        if (adventure == null)
            throw new InvalidOperationException("Adventure not found");

        var activeLog = adventure.AdventureLogs.FirstOrDefault(al => al.Status == "InProgress");
        if (activeLog == null)
            throw new InvalidOperationException("No active adventure log found");

        // Calculate rewards
        var rewards = await CalculateAdventureRewardsAsync(adventure.Id, userId, true);

        // Update adventure log
        activeLog.Status = "Completed";
        activeLog.CompletedAt = DateTime.UtcNow;
        activeLog.ExperienceGained = rewards.ExperienceReward;
        activeLog.PointsGained = rewards.PointsReward;
        activeLog.GoldGained = rewards.GoldReward;
        activeLog.HealthChange = rewards.HealthChange;
        activeLog.HungerChange = rewards.HungerChange;
        activeLog.EnergyChange = rewards.EnergyChange;
        activeLog.HappinessChange = rewards.HappinessChange;
        activeLog.CleanlinessChange = rewards.CleanlinessChange;
        activeLog.UpdatedAt = DateTime.UtcNow;

        // Update user wallet
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet != null)
        {
            wallet.Balance += rewards.GoldReward;
            wallet.UpdatedAt = DateTime.UtcNow;
        }

        // Update virtual pet stats if user has one
        var virtualPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.UserId == userId);
        if (virtualPet != null)
        {
            virtualPet.Health = Math.Min(virtualPet.MaxHealth, virtualPet.Health + rewards.HealthChange);
            virtualPet.Hunger = Math.Min(virtualPet.MaxHunger, virtualPet.Hunger + rewards.HungerChange);
            virtualPet.Energy = Math.Min(virtualPet.MaxEnergy, virtualPet.Energy + rewards.EnergyChange);
            virtualPet.Happiness = Math.Min(virtualPet.MaxHappiness, virtualPet.Happiness + rewards.HappinessChange);
            virtualPet.Cleanliness = Math.Min(virtualPet.MaxCleanliness, virtualPet.Cleanliness + rewards.CleanlinessChange);
            virtualPet.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} completed adventure {AdventureId}", userId, adventureId);

        // 清除相關快取
        ClearUserAdventureRelatedCache(userId);
        ClearAdventureRelatedCache(adventureId, userId);

        return await GetAdventureAsync(adventureId, userId);
    }

    public async Task<AdventureResponseDto> FailAdventureAsync(int adventureId, int userId, string reason)
    {
        // 輸入驗證
        if (adventureId <= 0)
            throw new ArgumentException("Adventure ID must be a positive integer", nameof(adventureId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be null or empty", nameof(reason));

        var adventure = await _context.Adventures
            .Include(a => a.AdventureLogs)
            .FirstOrDefaultAsync(a => a.Id == adventureId && a.UserId == userId);

        if (adventure == null)
            throw new InvalidOperationException("Adventure not found");

        var activeLog = adventure.AdventureLogs.FirstOrDefault(al => al.Status == "InProgress");
        if (activeLog == null)
            throw new InvalidOperationException("No active adventure log found");

        // Calculate partial rewards
        var rewards = await CalculateAdventureRewardsAsync(adventure.Id, userId, false);

        // Update adventure log
        activeLog.Status = "Failed";
        activeLog.FailedAt = DateTime.UtcNow;
        activeLog.ExperienceGained = rewards.ExperienceReward;
        activeLog.PointsGained = rewards.PointsReward;
        activeLog.GoldGained = rewards.GoldReward;
        activeLog.HealthChange = rewards.HealthChange;
        activeLog.HungerChange = rewards.HungerChange;
        activeLog.EnergyChange = rewards.EnergyChange;
        activeLog.HappinessChange = rewards.HappinessChange;
        activeLog.CleanlinessChange = rewards.CleanlinessChange;
        activeLog.FailureReason = reason;
        activeLog.UpdatedAt = DateTime.UtcNow;

        // Update user wallet (partial rewards)
        var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet != null)
        {
            wallet.Balance += rewards.GoldReward;
            wallet.UpdatedAt = DateTime.UtcNow;
        }

        // Update virtual pet stats (partial effects)
        var virtualPet = await _context.VirtualPets.FirstOrDefaultAsync(vp => vp.UserId == userId);
        if (virtualPet != null)
        {
            virtualPet.Health = Math.Min(virtualPet.MaxHealth, virtualPet.Health + rewards.HealthChange);
            virtualPet.Hunger = Math.Min(virtualPet.MaxHunger, virtualPet.Hunger + rewards.HungerChange);
            virtualPet.Energy = Math.Min(virtualPet.MaxEnergy, virtualPet.Energy + rewards.EnergyChange);
            virtualPet.Happiness = Math.Min(virtualPet.MaxHappiness, virtualPet.Happiness + rewards.HappinessChange);
            virtualPet.Cleanliness = Math.Min(virtualPet.MaxCleanliness, virtualPet.Cleanliness + rewards.CleanlinessChange);
            virtualPet.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} failed adventure {AdventureId}: {Reason}", userId, adventureId, reason);

        // 清除相關快取
        ClearUserAdventureRelatedCache(userId);
        ClearAdventureRelatedCache(adventureId, userId);

        return await GetAdventureAsync(adventureId, userId);
    }

    public async Task<AdventureResponseDto> AbandonAdventureAsync(int adventureId, int userId)
    {
        // 輸入驗證
        if (adventureId <= 0)
            throw new ArgumentException("Adventure ID must be a positive integer", nameof(adventureId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        var adventure = await _context.Adventures
            .Include(a => a.AdventureLogs)
            .FirstOrDefaultAsync(a => a.Id == adventureId && a.UserId == userId);

        if (adventure == null)
            throw new InvalidOperationException("Adventure not found");

        var activeLog = adventure.AdventureLogs.FirstOrDefault(al => al.Status == "InProgress");
        if (activeLog == null)
            throw new InvalidOperationException("No active adventure log found");

        // Update adventure log
        activeLog.Status = "Abandoned";
        activeLog.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} abandoned adventure {AdventureId}", userId, adventureId);

        // 清除相關快取
        ClearUserAdventureRelatedCache(userId);
        ClearAdventureRelatedCache(adventureId, userId);

        return await GetAdventureAsync(adventureId, userId);
    }

    public async Task<List<AdventureTemplateDto>> GetAvailableTemplatesAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        // 檢查快取
        var cacheKey = string.Format(AvailableTemplatesCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out List<AdventureTemplateDto> cachedTemplates))
        {
            _logger.LogDebug("Retrieved available templates from cache for user {UserId}", userId);
            return cachedTemplates;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var templates = await _context.AdventureTemplates
            .Where(t => t.IsActive)
            .AsNoTracking()
            .ToListAsync();

        var result = new List<AdventureTemplateDto>();

        foreach (var template in templates)
        {
            var isAvailable = await CanStartAdventureAsync(userId, template.Id);
            var unavailableReason = "";

            if (!isAvailable)
            {
                if (user.Level < template.MinLevel)
                    unavailableReason = $"Requires level {template.MinLevel}";
                else if (user.Level > template.MaxLevel)
                    unavailableReason = $"Maximum level {template.MaxLevel}";
                else
                    unavailableReason = "Insufficient energy";
            }

            result.Add(new AdventureTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                Difficulty = template.Difficulty,
                MinLevel = template.MinLevel,
                MaxLevel = template.MaxLevel,
                EnergyCost = template.EnergyCost,
                DurationMinutes = template.DurationMinutes,
                BaseSuccessRate = template.BaseSuccessRate,
                BaseExperienceReward = template.BaseExperienceReward,
                BasePointsReward = template.BasePointsReward,
                BaseGoldReward = template.BaseGoldReward,
                MaxMonsterEncounters = template.MaxMonsterEncounters,
                IsAvailable = isAvailable,
                UnavailableReason = unavailableReason
            });
        }

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, result, cacheOptions);

        _logger.LogDebug("Cached available templates for user {UserId}", userId);
        return result;
    }

    public async Task<AdventureTemplateDto> GetTemplateByIdAsync(int templateId)
    {
        // 輸入驗證
        if (templateId <= 0)
            throw new ArgumentException("Template ID must be a positive integer", nameof(templateId));

        // 檢查快取
        var cacheKey = string.Format(TemplateCacheKey, templateId);
        if (_memoryCache.TryGetValue(cacheKey, out AdventureTemplateDto cachedTemplate))
        {
            _logger.LogDebug("Retrieved template {TemplateId} from cache", templateId);
            return cachedTemplate;
        }

        var template = await _context.AdventureTemplates
            .FirstOrDefaultAsync(t => t.Id == templateId && t.IsActive);

        if (template == null)
            throw new InvalidOperationException("Adventure template not found");

        var result = new AdventureTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            Category = template.Category,
            Difficulty = template.Difficulty,
            MinLevel = template.MinLevel,
            MaxLevel = template.MaxLevel,
            EnergyCost = template.EnergyCost,
            DurationMinutes = template.DurationMinutes,
            BaseSuccessRate = template.BaseSuccessRate,
            BaseExperienceReward = template.BaseExperienceReward,
            BasePointsReward = template.BasePointsReward,
            BaseGoldReward = template.BaseGoldReward,
            MaxMonsterEncounters = template.MaxMonsterEncounters,
            IsAvailable = true,
            UnavailableReason = null
        };

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, result, cacheOptions);

        _logger.LogDebug("Cached template {TemplateId}", templateId);
        return result;
    }

    public async Task<AdventureLogResponseDto> GetUserAdventureLogsAsync(int userId, int page = 1, int pageSize = 20)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (page <= 0)
            throw new ArgumentException("Page must be a positive integer", nameof(page));
        
        if (pageSize <= 0)
            pageSize = DefaultPageSize;
        else if (pageSize > MaxPageSize)
            pageSize = MaxPageSize;

        // 檢查快取
        var cacheKey = string.Format(UserAdventureLogsCacheKey, userId, page, pageSize);
        if (_memoryCache.TryGetValue(cacheKey, out AdventureLogResponseDto cachedLogs))
        {
            _logger.LogDebug("Retrieved adventure logs from cache for user {UserId}, page {Page}", userId, page);
            return cachedLogs;
        }

        var query = _context.AdventureLogs
            .Include(al => al.Adventure)
            .Include(al => al.MonsterEncounters)
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.StartedAt);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var logs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(al => new AdventureLogDto
            {
                Id = al.Id,
                AdventureId = al.AdventureId,
                AdventureTitle = al.Adventure.Title,
                Status = al.Status,
                StartedAt = al.StartedAt,
                CompletedAt = al.CompletedAt,
                FailedAt = al.FailedAt,
                EnergySpent = al.EnergySpent,
                ExperienceGained = al.ExperienceGained,
                PointsGained = al.PointsGained,
                GoldGained = al.GoldGained,
                HealthChange = al.HealthChange,
                HungerChange = al.HungerChange,
                EnergyChange = al.EnergyChange,
                HappinessChange = al.HappinessChange,
                CleanlinessChange = al.CleanlinessChange,
                AdventureNotes = al.AdventureNotes,
                FailureReason = al.FailureReason,
                MonsterEncountersCount = al.MonsterEncounters.Count
            })
            .ToListAsync();

        return new AdventureLogResponseDto
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Items = logs
        };

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, result, cacheOptions);

        _logger.LogDebug("Cached adventure logs for user {UserId}, page {Page}", userId, page);
        return result;
    }

    public async Task<AdventureLogResponseDto> GetAdventureLogByIdAsync(int logId, int userId)
    {
        // 輸入驗證
        if (logId <= 0)
            throw new ArgumentException("Log ID must be a positive integer", nameof(logId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        var log = await _context.AdventureLogs
            .Include(al => al.Adventure)
            .Include(al => al.MonsterEncounters)
            .FirstOrDefaultAsync(al => al.Id == logId && al.UserId == userId);

        if (log == null)
            throw new InvalidOperationException("Adventure log not found");

        var logDto = new AdventureLogDto
        {
            Id = log.Id,
            AdventureId = log.AdventureId,
            AdventureTitle = log.Adventure.Title,
            Status = log.Status,
            StartedAt = log.StartedAt,
            CompletedAt = log.CompletedAt,
            FailedAt = log.FailedAt,
            EnergySpent = log.EnergySpent,
            ExperienceGained = log.ExperienceGained,
            PointsGained = log.PointsGained,
            GoldGained = log.GoldGained,
            HealthChange = log.HealthChange,
            HungerChange = log.HungerChange,
            EnergyChange = log.EnergyChange,
            HappinessChange = log.HappinessChange,
            CleanlinessChange = log.CleanlinessChange,
            AdventureNotes = log.AdventureNotes,
            FailureReason = log.FailureReason,
            MonsterEncountersCount = log.MonsterEncounters.Count
        };

        return new AdventureLogResponseDto
        {
            TotalCount = 1,
            Page = 1,
            PageSize = 1,
            TotalPages = 1,
            Items = new List<AdventureLogDto> { logDto }
        };
    }

    public async Task<AdventureStatisticsDto> GetUserAdventureStatisticsAsync(int userId)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        // 檢查快取
        var cacheKey = string.Format(UserAdventureStatisticsCacheKey, userId);
        if (_memoryCache.TryGetValue(cacheKey, out AdventureStatisticsDto cachedStats))
        {
            _logger.LogDebug("Retrieved adventure statistics from cache for user {UserId}", userId);
            return cachedStats;
        }

        var logs = await _context.AdventureLogs
            .Include(al => al.Adventure)
            .Include(al => al.MonsterEncounters)
            .Where(al => al.UserId == userId)
            .AsNoTracking()
            .ToListAsync();

        var totalAdventures = logs.Count;
        var completedAdventures = logs.Count(al => al.Status == "Completed");
        var failedAdventures = logs.Count(al => al.Status == "Failed");
        var abandonedAdventures = logs.Count(al => al.Status == "Abandoned");

        var successRate = totalAdventures > 0 ? (decimal)completedAdventures / totalAdventures : 0;

        var totalExperienceGained = logs.Sum(al => al.ExperienceGained);
        var totalPointsGained = logs.Sum(al => al.PointsGained);
        var totalGoldGained = logs.Sum(al => al.GoldGained);

        var totalMonsterEncounters = logs.Sum(al => al.MonsterEncounters.Count);
        var monstersDefeated = logs.Sum(al => al.MonsterEncounters.Count(me => me.Outcome == "Victory"));
        var monstersEscaped = logs.Sum(al => al.MonsterEncounters.Count(me => me.Outcome == "Escaped"));

        var totalDamageDealt = logs.Sum(al => al.MonsterEncounters.Sum(me => me.DamageDealt));
        var totalDamageTaken = logs.Sum(al => al.MonsterEncounters.Sum(me => me.DamageTaken));

        var totalAdventureTime = logs
            .Where(al => al.CompletedAt.HasValue || al.FailedAt.HasValue)
            .Sum(al => (al.CompletedAt ?? al.FailedAt ?? al.StartedAt) - al.StartedAt);

        var adventuresByDifficulty = logs
            .GroupBy(al => al.Adventure.Difficulty)
            .ToDictionary(g => g.Key, g => g.Count());

        var adventuresByCategory = logs
            .GroupBy(al => al.Adventure.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        var recentAdventures = logs
            .OrderByDescending(al => al.StartedAt)
            .Take(5)
            .Select(al => new AdventureLogDto
            {
                Id = al.Id,
                AdventureId = al.AdventureId,
                AdventureTitle = al.Adventure.Title,
                Status = al.Status,
                StartedAt = al.StartedAt,
                CompletedAt = al.CompletedAt,
                FailedAt = al.FailedAt,
                EnergySpent = al.EnergySpent,
                ExperienceGained = al.ExperienceGained,
                PointsGained = al.PointsGained,
                GoldGained = al.GoldGained,
                HealthChange = al.HealthChange,
                HungerChange = al.HungerChange,
                EnergyChange = al.EnergyChange,
                HappinessChange = al.HappinessChange,
                CleanlinessChange = al.CleanlinessChange,
                AdventureNotes = al.AdventureNotes,
                FailureReason = al.FailureReason,
                MonsterEncountersCount = al.MonsterEncounters.Count
            })
            .ToList();

        return new AdventureStatisticsDto
        {
            TotalAdventures = totalAdventures,
            CompletedAdventures = completedAdventures,
            FailedAdventures = failedAdventures,
            AbandonedAdventures = abandonedAdventures,
            SuccessRate = successRate,
            TotalExperienceGained = totalExperienceGained,
            TotalPointsGained = totalPointsGained,
            TotalGoldGained = totalGoldGained,
            TotalMonsterEncounters = totalMonsterEncounters,
            MonstersDefeated = monstersDefeated,
            MonstersEscaped = monstersEscaped,
            TotalDamageDealt = totalDamageDealt,
            TotalDamageTaken = totalDamageTaken,
            TotalAdventureTime = totalAdventureTime,
            AdventuresByDifficulty = adventuresByDifficulty,
            AdventuresByCategory = adventuresByCategory,
            RecentAdventures = recentAdventures
        };

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, result, cacheOptions);

        _logger.LogDebug("Cached adventure statistics for user {UserId}", userId);
        return result;
    }

    public async Task<MonsterEncounterResponseDto> ProcessMonsterEncounterAsync(int adventureLogId, int userId, string monsterType)
    {
        // 輸入驗證
        if (adventureLogId <= 0)
            throw new ArgumentException("Adventure log ID must be a positive integer", nameof(adventureLogId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(monsterType))
            throw new ArgumentException("Monster type cannot be null or empty", nameof(monsterType));

        var adventureLog = await _context.AdventureLogs
            .Include(al => al.Adventure)
            .Include(al => al.MonsterEncounters)
            .FirstOrDefaultAsync(al => al.Id == adventureLogId && al.UserId == userId);

        if (adventureLog == null)
            throw new InvalidOperationException("Adventure log not found");

        if (adventureLog.Status != "InProgress")
            throw new InvalidOperationException("Adventure is not in progress");

        // Generate monster based on type and difficulty
        var monster = GenerateMonster(adventureLog.Adventure.Difficulty, monsterType);

        // Simulate battle
        var battleResult = SimulateBattle(monster, adventureLog.Adventure.Difficulty);

        // Create monster encounter
        var encounter = new MonsterEncounter
        {
            AdventureLogId = adventureLogId,
            MonsterName = monster.Name,
            MonsterType = monster.Type,
            MonsterLevel = monster.Level,
            MonsterHealth = monster.Health,
            MonsterMaxHealth = monster.MaxHealth,
            MonsterAttack = monster.Attack,
            MonsterDefense = monster.Defense,
            Outcome = battleResult.Outcome,
            DamageDealt = battleResult.DamageDealt,
            DamageTaken = battleResult.DamageTaken,
            ExperienceGained = battleResult.ExperienceGained,
            PointsGained = battleResult.PointsGained,
            GoldGained = battleResult.GoldGained,
            BattleNotes = battleResult.BattleNotes,
            EncounterTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.MonsterEncounters.Add(encounter);
        await _context.SaveChangesAsync();

        // 清除相關快取
        ClearAdventureLogRelatedCache(adventureLogId);
        ClearAdventureRelatedCache(adventureLog.AdventureId, userId);

        // Update adventure progress
        var progress = await GetAdventureProgressAsync(adventureLog.AdventureId, userId);

        return new MonsterEncounterResponseDto
        {
            Id = encounter.Id,
            MonsterName = encounter.MonsterName,
            MonsterType = encounter.MonsterType,
            MonsterLevel = encounter.MonsterLevel,
            Outcome = encounter.Outcome,
            DamageDealt = encounter.DamageDealt,
            DamageTaken = encounter.DamageTaken,
            ExperienceGained = encounter.ExperienceGained,
            PointsGained = encounter.PointsGained,
            GoldGained = encounter.GoldGained,
            BattleNotes = encounter.BattleNotes,
            IsVictory = encounter.Outcome == "Victory",
            IsDefeat = encounter.Outcome == "Defeat",
            IsEscape = encounter.Outcome == "Escaped",
            AdventureProgress = progress
        };
    }

    public async Task<List<MonsterEncounterDto>> GetAdventureMonsterEncountersAsync(int adventureLogId, int userId)
    {
        // 輸入驗證
        if (adventureLogId <= 0)
            throw new ArgumentException("Adventure log ID must be a positive integer", nameof(adventureLogId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        // 檢查快取
        var cacheKey = string.Format(MonsterEncountersCacheKey, adventureLogId);
        if (_memoryCache.TryGetValue(cacheKey, out List<MonsterEncounterDto> cachedEncounters))
        {
            _logger.LogDebug("Retrieved monster encounters from cache for adventure log {AdventureLogId}", adventureLogId);
            return cachedEncounters;
        }

        var encounters = await _context.MonsterEncounters
            .Where(me => me.AdventureLog.UserId == userId && me.AdventureLogId == adventureLogId)
            .OrderBy(me => me.EncounterTime)
            .AsNoTracking()
            .Select(me => new MonsterEncounterDto
            {
                Id = me.Id,
                MonsterName = me.MonsterName,
                MonsterType = me.MonsterType,
                MonsterLevel = me.MonsterLevel,
                MonsterHealth = me.MonsterHealth,
                MonsterMaxHealth = me.MonsterMaxHealth,
                MonsterAttack = me.MonsterAttack,
                MonsterDefense = me.MonsterDefense,
                Outcome = me.Outcome,
                DamageDealt = me.DamageDealt,
                DamageTaken = me.DamageTaken,
                ExperienceGained = me.ExperienceGained,
                PointsGained = me.PointsGained,
                GoldGained = me.GoldGained,
                BattleNotes = me.BattleNotes,
                EncounterTime = me.EncounterTime
            })
            .ToListAsync();

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, encounters, cacheOptions);

        _logger.LogDebug("Cached monster encounters for adventure log {AdventureLogId}", adventureLogId);
        return encounters;
    }

    public async Task<AdventureProgressDto> GetAdventureProgressAsync(int adventureId, int userId)
    {
        // 輸入驗證
        if (adventureId <= 0)
            throw new ArgumentException("Adventure ID must be a positive integer", nameof(adventureId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        // 檢查快取
        var cacheKey = string.Format(AdventureProgressCacheKey, adventureId, userId);
        if (_memoryCache.TryGetValue(cacheKey, out AdventureProgressDto cachedProgress))
        {
            _logger.LogDebug("Retrieved adventure progress from cache for adventure {AdventureId}, user {UserId}", adventureId, userId);
            return cachedProgress;
        }

        var adventure = await _context.Adventures
            .Include(a => a.AdventureLogs)
            .ThenInclude(al => al.MonsterEncounters)
            .FirstOrDefaultAsync(a => a.Id == adventureId && a.UserId == userId);

        if (adventure == null)
            throw new InvalidOperationException("Adventure not found");

        var activeLog = adventure.AdventureLogs.FirstOrDefault(al => al.Status == "InProgress");
        if (activeLog == null)
            return new AdventureProgressDto();

        var monsterEncountersCompleted = activeLog.MonsterEncounters.Count;
        var totalMonsterEncounters = 3; // Default value, could be from template
        var monstersDefeated = activeLog.MonsterEncounters.Count(me => me.Outcome == "Victory");
        var monstersEscaped = activeLog.MonsterEncounters.Count(me => me.Outcome == "Escaped");

        var totalDamageDealt = activeLog.MonsterEncounters.Sum(me => me.DamageDealt);
        var totalDamageTaken = activeLog.MonsterEncounters.Sum(me => me.DamageTaken);

        var completionPercentage = totalMonsterEncounters > 0 
            ? (decimal)monsterEncountersCompleted / totalMonsterEncounters * 100 
            : 0;

        var timeRemaining = adventure.DurationMinutes - (DateTime.UtcNow - activeLog.StartedAt).TotalMinutes;
        var canComplete = monsterEncountersCompleted >= totalMonsterEncounters && timeRemaining > 0;
        var canFail = timeRemaining <= 0;
        var canAbandon = true;

        var result = new AdventureProgressDto
        {
            MonsterEncountersCompleted = monsterEncountersCompleted,
            TotalMonsterEncounters = totalMonsterEncounters,
            MonstersDefeated = monstersDefeated,
            MonstersEscaped = monstersEscaped,
            TotalDamageDealt = totalDamageDealt,
            TotalDamageTaken = totalDamageTaken,
            CompletionPercentage = completionPercentage,
            TimeRemaining = TimeSpan.FromMinutes(Math.Max(0, timeRemaining)),
            CanComplete = canComplete,
            CanFail = canFail,
            CanAbandon = canAbandon
        };

        // 儲存到快取
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
        _memoryCache.Set(cacheKey, result, cacheOptions);

        _logger.LogDebug("Cached adventure progress for adventure {AdventureId}, user {UserId}", adventureId, userId);
        return result;
    }

    public async Task<bool> CanStartAdventureAsync(int userId, int templateId)
    {
        // 輸入驗證
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));
        
        if (templateId <= 0)
            throw new ArgumentException("Template ID must be a positive integer", nameof(templateId));

        var user = await _context.Users
            .Include(u => u.UserWallet)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return false;

        var template = await _context.AdventureTemplates
            .FirstOrDefaultAsync(t => t.Id == templateId && t.IsActive);

        if (template == null) return false;

        // Check level requirements
        if (user.Level < template.MinLevel || user.Level > template.MaxLevel)
            return false;

        // Check energy requirements
        if (user.UserWallet?.Balance < template.EnergyCost)
            return false;

        // Check if user already has an active adventure
        var activeAdventure = await _context.Adventures
            .AnyAsync(a => a.UserId == userId && a.AdventureLogs.Any(al => al.Status == "InProgress"));

        return !activeAdventure;
    }

    public async Task<AdventureRewardsDto> CalculateAdventureRewardsAsync(int templateId, int userId, bool isSuccess)
    {
        // 輸入驗證
        if (templateId <= 0)
            throw new ArgumentException("Template ID must be a positive integer", nameof(templateId));
        
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive integer", nameof(userId));

        var template = await _context.AdventureTemplates
            .FirstOrDefaultAsync(t => t.Id == templateId);

        if (template == null)
            throw new InvalidOperationException("Adventure template not found");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new ArgumentException("User not found", nameof(userId));

        // Base rewards
        var experienceReward = template.BaseExperienceReward;
        var pointsReward = template.BasePointsReward;
        var goldReward = template.BaseGoldReward;

        // Apply difficulty multiplier
        var difficultyMultiplier = GetDifficultyMultiplier(template.Difficulty);
        experienceReward = (int)(experienceReward * difficultyMultiplier);
        pointsReward = (int)(pointsReward * difficultyMultiplier);
        goldReward = goldReward * difficultyMultiplier;

        // Apply level bonus
        var levelBonus = Math.Max(1.0, user.Level * 0.1);
        experienceReward = (int)(experienceReward * levelBonus);
        pointsReward = (int)(pointsReward * levelBonus);
        goldReward = goldReward * (decimal)levelBonus;

        // Apply success/failure modifier
        if (!isSuccess)
        {
            experienceReward = (int)(experienceReward * 0.5);
            pointsReward = (int)(pointsReward * 0.5);
            goldReward = goldReward * 0.5m;
        }

        // Calculate stat changes
        var healthChange = isSuccess ? 5 : -10;
        var hungerChange = isSuccess ? -15 : -5;
        var energyChange = isSuccess ? -20 : -10;
        var happinessChange = isSuccess ? 15 : -5;
        var cleanlinessChange = isSuccess ? -5 : -10;

        var bonusRewards = new List<string>();
        if (isSuccess && user.Level >= 10)
            bonusRewards.Add("Level Bonus");
        if (isSuccess && template.Difficulty == "Hard")
            bonusRewards.Add("Difficulty Bonus");
        if (isSuccess && template.Difficulty == "Extreme")
            bonusRewards.Add("Extreme Challenge Bonus");

        var specialReward = isSuccess && template.Difficulty == "Extreme" ? "Rare Item" : null;

        return new AdventureRewardsDto
        {
            ExperienceReward = experienceReward,
            PointsReward = pointsReward,
            GoldReward = goldReward,
            HealthChange = healthChange,
            HungerChange = hungerChange,
            EnergyChange = energyChange,
            HappinessChange = happinessChange,
            CleanlinessChange = cleanlinessChange,
            BonusRewards = bonusRewards,
            SpecialReward = specialReward
        };
    }

    private Monster GenerateMonster(string difficulty, string monsterType)
    {
        var random = new Random();
        var baseLevel = difficulty switch
        {
            "Easy" => 1,
            "Normal" => 5,
            "Hard" => 10,
            "Extreme" => 20,
            _ => 5
        };

        var levelVariation = random.Next(-2, 3);
        var level = Math.Max(1, baseLevel + levelVariation);

        var healthMultiplier = difficulty switch
        {
            "Easy" => 1.0,
            "Normal" => 1.5,
            "Hard" => 2.0,
            "Extreme" => 3.0,
            _ => 1.5
        };

        var baseHealth = 50 + (level * 10);
        var health = (int)(baseHealth * healthMultiplier);

        var attack = 10 + (level * 2);
        var defense = 5 + level;

        var names = new[] { "Goblin", "Orc", "Troll", "Dragon", "Demon", "Undead", "Beast", "Elemental" };
        var name = names[random.Next(names.Length)];

        return new Monster
        {
            Name = $"{name} Lv.{level}",
            Type = monsterType,
            Level = level,
            Health = health,
            MaxHealth = health,
            Attack = attack,
            Defense = defense
        };
    }

    private BattleResult SimulateBattle(Monster monster, string difficulty)
    {
        var random = new Random();
        var playerLevel = 10; // This would come from user data
        var playerAttack = 15 + (playerLevel * 2);
        var playerDefense = 8 + playerLevel;

        // Calculate damage
        var damageDealt = Math.Max(1, playerAttack - monster.Defense);
        var damageTaken = Math.Max(1, monster.Attack - playerDefense);

        // Apply difficulty modifiers
        var difficultyMultiplier = difficulty switch
        {
            "Easy" => 0.8,
            "Normal" => 1.0,
            "Hard" => 1.3,
            "Extreme" => 1.8,
            _ => 1.0
        };

        damageTaken = (int)(damageTaken * difficultyMultiplier);

        // Determine outcome
        var successRate = difficulty switch
        {
            "Easy" => 0.9,
            "Normal" => 0.7,
            "Hard" => 0.5,
            "Extreme" => 0.3,
            _ => 0.7
        };

        var isVictory = random.NextDouble() < successRate;
        var outcome = isVictory ? "Victory" : "Defeat";

        // Calculate rewards
        var experienceGained = isVictory ? monster.Level * 10 : monster.Level * 3;
        var pointsGained = isVictory ? monster.Level * 5 : monster.Level * 1;
        var goldGained = isVictory ? monster.Level * 2.0m : monster.Level * 0.5m;

        var battleNotes = isVictory 
            ? $"Successfully defeated {monster.Name}"
            : $"Lost battle against {monster.Name}";

        return new BattleResult
        {
            Outcome = outcome,
            DamageDealt = damageDealt,
            DamageTaken = damageTaken,
            ExperienceGained = experienceGained,
            PointsGained = pointsGained,
            GoldGained = goldGained,
            BattleNotes = battleNotes
        };
    }

    private double GetDifficultyMultiplier(string difficulty)
    {
        return difficulty switch
        {
            "Easy" => 0.8,
            "Normal" => 1.0,
            "Hard" => 1.5,
            "Extreme" => 2.0,
            _ => 1.0
        };
    }

    #region 快取管理

    /// <summary>
    /// 清除用戶冒險相關的快取
    /// </summary>
    private void ClearUserAdventureRelatedCache(int userId)
    {
        var availableTemplatesKey = string.Format(AvailableTemplatesCacheKey, userId);
        var userAdventureStatisticsKey = string.Format(UserAdventureStatisticsCacheKey, userId);

        _memoryCache.Remove(availableTemplatesKey);
        _memoryCache.Remove(userAdventureStatisticsKey);

        // 清除分頁冒險記錄快取（需要遍歷所有可能的頁面）
        for (int page = 1; page <= 10; page++) // 假設最多10頁
        {
            for (int pageSize = 10; pageSize <= MaxPageSize; pageSize += 10)
            {
                var logsKey = string.Format(UserAdventureLogsCacheKey, userId, page, pageSize);
                _memoryCache.Remove(logsKey);
            }
        }

        _logger.LogDebug("Cleared cache for user adventure {UserId}", userId);
    }

    /// <summary>
    /// 清除冒險相關的快取
    /// </summary>
    private void ClearAdventureRelatedCache(int adventureId, int userId)
    {
        var adventureProgressKey = string.Format(AdventureProgressCacheKey, adventureId, userId);
        var templateKey = string.Format(TemplateCacheKey, adventureId);

        _memoryCache.Remove(adventureProgressKey);
        _memoryCache.Remove(templateKey);

        _logger.LogDebug("Cleared cache for adventure {AdventureId}", adventureId);
    }

    /// <summary>
    /// 清除冒險記錄相關的快取
    /// </summary>
    private void ClearAdventureLogRelatedCache(int adventureLogId)
    {
        var monsterEncountersKey = string.Format(MonsterEncountersCacheKey, adventureLogId);

        _memoryCache.Remove(monsterEncountersKey);

        _logger.LogDebug("Cleared cache for adventure log {AdventureLogId}", adventureLogId);
    }

    #endregion

    private class Monster
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
    }

    private class BattleResult
    {
        public string Outcome { get; set; } = string.Empty;
        public int DamageDealt { get; set; }
        public int DamageTaken { get; set; }
        public int ExperienceGained { get; set; }
        public int PointsGained { get; set; }
        public decimal GoldGained { get; set; }
        public string BattleNotes { get; set; } = string.Empty;
    }
}