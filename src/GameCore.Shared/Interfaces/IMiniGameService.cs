using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces;

/// <summary>
/// 小遊戲服務介面
/// </summary>
public interface IMiniGameService
{
    /// <summary>
    /// 開始冒險
    /// </summary>
    Task<ServiceResult<MiniGameStartResultDto>> StartAdventureAsync(int userId);

    /// <summary>
    /// 完成冒險
    /// </summary>
    Task<ServiceResult<MiniGameFinishResultDto>> FinishAdventureAsync(int userId, MiniGameFinishRequest request);

    /// <summary>
    /// 獲取遊戲記錄
    /// </summary>
    Task<ServiceResult<PagedResult<MiniGameRecordDto>>> GetGameRecordsAsync(int userId, int page = 1, int pageSize = 20, string? result = null);

    /// <summary>
    /// 獲取遊戲統計
    /// </summary>
    Task<ServiceResult<MiniGameStatsDto>> GetGameStatsAsync(int userId);
} 