using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs.VirtualPetDtos;

/// <summary>
/// Request DTO for creating a new virtual pet
/// </summary>
public class CreatePetRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Color { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Personality { get; set; }
}

/// <summary>
/// Response DTO for virtual pet information
/// </summary>
public class VirtualPetResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Hunger { get; set; }
    public int MaxHunger { get; set; }
    public int Energy { get; set; }
    public int MaxEnergy { get; set; }
    public int Happiness { get; set; }
    public int MaxHappiness { get; set; }
    public int Cleanliness { get; set; }
    public int MaxCleanliness { get; set; }
    public string Color { get; set; }
    public string Personality { get; set; }
    public DateTime LastFed { get; set; }
    public DateTime LastPlayed { get; set; }
    public DateTime LastCleaned { get; set; }
    public DateTime LastRested { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public List<string> Needs { get; set; } = new();
}

/// <summary>
/// Response DTO for pet care actions
/// </summary>
public class PetCareResponseDto
{
    public int PetId { get; set; }
    public string PetName { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public int HealthChange { get; set; }
    public int HungerChange { get; set; }
    public int EnergyChange { get; set; }
    public int HappinessChange { get; set; }
    public int CleanlinessChange { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsEarned { get; set; }
    public bool LevelUp { get; set; }
    public int NewLevel { get; set; }
    public List<string> Achievements { get; set; } = new();
    public string Message { get; set; }
}

/// <summary>
/// Response DTO for pet care history
/// </summary>
public class PetCareHistoryResponseDto
{
    public List<PetCareLogDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// DTO for individual pet care log item
/// </summary>
public class PetCareLogDto
{
    public int Id { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
    public int HealthChange { get; set; }
    public int HungerChange { get; set; }
    public int EnergyChange { get; set; }
    public int HappinessChange { get; set; }
    public int CleanlinessChange { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsEarned { get; set; }
    public DateTime ActionTime { get; set; }
}

/// <summary>
/// DTO for pet achievements
/// </summary>
public class PetAchievementDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public int PointsReward { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}

/// <summary>
/// DTO for pet items
/// </summary>
public class PetItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
    public int HealthEffect { get; set; }
    public int HungerEffect { get; set; }
    public int EnergyEffect { get; set; }
    public int HappinessEffect { get; set; }
    public int CleanlinessEffect { get; set; }
    public int ExperienceEffect { get; set; }
    public int Price { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for pet statistics
/// </summary>
public class PetStatisticsDto
{
    public int TotalCareActions { get; set; }
    public int TotalExperienceGained { get; set; }
    public int TotalPointsEarned { get; set; }
    public int AchievementsUnlocked { get; set; }
    public int TotalAchievements { get; set; }
    public DateTime FirstCareAction { get; set; }
    public DateTime LastCareAction { get; set; }
    public Dictionary<string, int> ActionCounts { get; set; } = new();
    public Dictionary<string, int> CategoryStats { get; set; } = new();
}

/// <summary>
/// DTO for pet status summary
/// </summary>
public class PetStatusSummaryDto
{
    public int PetId { get; set; }
    public string PetName { get; set; }
    public int Level { get; set; }
    public string Status { get; set; }
    public List<string> Needs { get; set; } = new();
    public bool IsHealthy { get; set; }
    public bool IsHungry { get; set; }
    public bool IsTired { get; set; }
    public bool IsUnhappy { get; set; }
    public bool IsDirty { get; set; }
    public DateTime LastActivity { get; set; }
}

/// <summary>
/// DTO for pet level up event
/// </summary>
public class PetLevelUpDto
{
    public int PetId { get; set; }
    public string PetName { get; set; }
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsEarned { get; set; }
    public List<string> NewAbilities { get; set; } = new();
    public string Message { get; set; }
}