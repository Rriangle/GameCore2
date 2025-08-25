namespace GameCore.Shared.DTOs.AdventureDtos;

public class AdventureTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }
    public int EnergyCost { get; set; }
    public int DurationMinutes { get; set; }
    public decimal BaseSuccessRate { get; set; }
    public int BaseExperienceReward { get; set; }
    public int BasePointsReward { get; set; }
    public decimal BaseGoldReward { get; set; }
    public int MaxMonsterEncounters { get; set; }
    public bool IsAvailable { get; set; }
    public string? UnavailableReason { get; set; }
}

public class StartAdventureRequestDto
{
    public int TemplateId { get; set; }
}

public class AdventureResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int RequiredLevel { get; set; }
    public int RequiredEnergy { get; set; }
    public int DurationMinutes { get; set; }
    public decimal SuccessRate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public int EnergySpent { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsGained { get; set; }
    public decimal GoldGained { get; set; }
    public AdventureProgressDto Progress { get; set; } = new();
}

public class AdventureProgressDto
{
    public int MonsterEncountersCompleted { get; set; }
    public int TotalMonsterEncounters { get; set; }
    public int MonstersDefeated { get; set; }
    public int MonstersEscaped { get; set; }
    public int TotalDamageDealt { get; set; }
    public int TotalDamageTaken { get; set; }
    public decimal CompletionPercentage { get; set; }
    public TimeSpan TimeRemaining { get; set; }
    public bool CanComplete { get; set; }
    public bool CanFail { get; set; }
    public bool CanAbandon { get; set; }
}

public class AdventureLogResponseDto
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<AdventureLogDto> Items { get; set; } = new();
}

public class AdventureLogDto
{
    public int Id { get; set; }
    public int AdventureId { get; set; }
    public string AdventureTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public int EnergySpent { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsGained { get; set; }
    public decimal GoldGained { get; set; }
    public int HealthChange { get; set; }
    public int HungerChange { get; set; }
    public int EnergyChange { get; set; }
    public int HappinessChange { get; set; }
    public int CleanlinessChange { get; set; }
    public string? AdventureNotes { get; set; }
    public string? FailureReason { get; set; }
    public int MonsterEncountersCount { get; set; }
}

public class MonsterEncounterDto
{
    public int Id { get; set; }
    public string MonsterName { get; set; } = string.Empty;
    public string MonsterType { get; set; } = string.Empty;
    public int MonsterLevel { get; set; }
    public int MonsterHealth { get; set; }
    public int MonsterMaxHealth { get; set; }
    public int MonsterAttack { get; set; }
    public int MonsterDefense { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public int DamageDealt { get; set; }
    public int DamageTaken { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsGained { get; set; }
    public decimal GoldGained { get; set; }
    public string? BattleNotes { get; set; }
    public DateTime EncounterTime { get; set; }
}

public class MonsterEncounterResponseDto
{
    public int Id { get; set; }
    public string MonsterName { get; set; } = string.Empty;
    public string MonsterType { get; set; } = string.Empty;
    public int MonsterLevel { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public int DamageDealt { get; set; }
    public int DamageTaken { get; set; }
    public int ExperienceGained { get; set; }
    public int PointsGained { get; set; }
    public decimal GoldGained { get; set; }
    public string? BattleNotes { get; set; }
    public bool IsVictory { get; set; }
    public bool IsDefeat { get; set; }
    public bool IsEscape { get; set; }
    public AdventureProgressDto AdventureProgress { get; set; } = new();
}

public class AdventureStatisticsDto
{
    public int TotalAdventures { get; set; }
    public int CompletedAdventures { get; set; }
    public int FailedAdventures { get; set; }
    public int AbandonedAdventures { get; set; }
    public decimal SuccessRate { get; set; }
    public int TotalExperienceGained { get; set; }
    public int TotalPointsGained { get; set; }
    public decimal TotalGoldGained { get; set; }
    public int TotalMonsterEncounters { get; set; }
    public int MonstersDefeated { get; set; }
    public int MonstersEscaped { get; set; }
    public int TotalDamageDealt { get; set; }
    public int TotalDamageTaken { get; set; }
    public TimeSpan TotalAdventureTime { get; set; }
    public Dictionary<string, int> AdventuresByDifficulty { get; set; } = new();
    public Dictionary<string, int> AdventuresByCategory { get; set; } = new();
    public List<AdventureLogDto> RecentAdventures { get; set; } = new();
}

public class AdventureRewardsDto
{
    public int ExperienceReward { get; set; }
    public int PointsReward { get; set; }
    public decimal GoldReward { get; set; }
    public int HealthChange { get; set; }
    public int HungerChange { get; set; }
    public int EnergyChange { get; set; }
    public int HappinessChange { get; set; }
    public int CleanlinessChange { get; set; }
    public List<string> BonusRewards { get; set; } = new();
    public string? SpecialReward { get; set; }
}

public class CompleteAdventureRequestDto
{
    public string? AdventureNotes { get; set; }
}

public class FailAdventureRequestDto
{
    public string FailureReason { get; set; } = string.Empty;
}