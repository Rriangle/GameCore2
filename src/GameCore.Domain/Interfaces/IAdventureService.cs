using GameCore.Shared.DTOs.AdventureDtos;

namespace GameCore.Domain.Interfaces;

public interface IAdventureService
{
    // Adventure Management
    Task<AdventureResponseDto> StartAdventureAsync(int userId, int templateId);
    Task<AdventureResponseDto> GetAdventureAsync(int adventureId, int userId);
    Task<AdventureResponseDto> CompleteAdventureAsync(int adventureId, int userId);
    Task<AdventureResponseDto> FailAdventureAsync(int adventureId, int userId, string reason);
    Task<AdventureResponseDto> AbandonAdventureAsync(int adventureId, int userId);

    // Adventure Templates
    Task<List<AdventureTemplateDto>> GetAvailableTemplatesAsync(int userId);
    Task<AdventureTemplateDto> GetTemplateByIdAsync(int templateId);

    // Adventure Logs
    Task<AdventureLogResponseDto> GetUserAdventureLogsAsync(int userId, int page = 1, int pageSize = 20);
    Task<AdventureLogResponseDto> GetAdventureLogByIdAsync(int logId, int userId);
    Task<AdventureStatisticsDto> GetUserAdventureStatisticsAsync(int userId);

    // Monster Encounters
    Task<MonsterEncounterResponseDto> ProcessMonsterEncounterAsync(int adventureLogId, int userId, string monsterType);
    Task<List<MonsterEncounterDto>> GetAdventureMonsterEncountersAsync(int adventureLogId, int userId);

    // Adventure Progress
    Task<AdventureProgressDto> GetAdventureProgressAsync(int adventureId, int userId);
    Task<bool> CanStartAdventureAsync(int userId, int templateId);
    Task<AdventureRewardsDto> CalculateAdventureRewardsAsync(int templateId, int userId, bool isSuccess);
}