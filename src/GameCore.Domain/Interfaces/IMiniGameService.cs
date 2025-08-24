using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 小遊戲服務接口
/// </summary>
public interface IMiniGameService
{
    /// <summary>
    /// 檢查遊戲狀態
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>遊戲狀態</returns>
    Task<MiniGameStatusDto> CheckGameStatusAsync(int userId);

    /// <summary>
    /// 開始遊戲
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="request">開始請求</param>
    /// <returns>開始結果</returns>
    Task<MiniGameStartResultDto> StartGameAsync(int userId, MiniGameStartRequestDto request);

    /// <summary>
    /// 結束遊戲
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="request">結束請求</param>
    /// <returns>結束結果</returns>
    Task<MiniGameFinishResultDto> FinishGameAsync(int userId, MiniGameFinishRequestDto request);

    /// <summary>
    /// 中止遊戲
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="playId">遊戲記錄編號</param>
    /// <returns>是否成功</returns>
    Task<bool> AbortGameAsync(int userId, int playId);

    /// <summary>
    /// 獲取遊戲記錄
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="playId">遊戲記錄編號</param>
    /// <returns>遊戲記錄</returns>
    Task<MiniGameRecordDto?> GetGameRecordAsync(int userId, int playId);

    /// <summary>
    /// 分頁獲取遊戲記錄
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="query">查詢參數</param>
    /// <returns>分頁結果</returns>
    Task<PagedMiniGameRecordsDto> GetPagedRecordsAsync(int userId, MiniGameQueryDto query);

    /// <summary>
    /// 獲取遊戲統計
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>統計資訊</returns>
    Task<MiniGameStatisticsDto> GetStatisticsAsync(int userId);

    /// <summary>
    /// 獲取關卡配置
    /// </summary>
    /// <param name="level">關卡等級</param>
    /// <returns>關卡配置</returns>
    Task<MiniGameLevelConfigDto?> GetLevelConfigAsync(int level);

    /// <summary>
    /// 獲取所有關卡配置
    /// </summary>
    /// <returns>關卡配置列表</returns>
    Task<List<MiniGameLevelConfigDto>> GetAllLevelConfigsAsync();

    /// <summary>
    /// 獲取遊戲排行榜
    /// </summary>
    /// <param name="type">排行榜類型</param>
    /// <param name="limit">限制數量</param>
    /// <returns>排行榜列表</returns>
    Task<List<MiniGameRecordDto>> GetLeaderboardAsync(string type, int limit = 10);

    /// <summary>
    /// 計算關卡獎勵
    /// </summary>
    /// <param name="level">關卡等級</param>
    /// <param name="result">遊戲結果</param>
    /// <param name="monstersDefeated">擊敗怪物數</param>
    /// <param name="combo">連擊數</param>
    /// <returns>獎勵資訊</returns>
    Task<MiniGameRewardsDto> CalculateRewardsAsync(int level, string result, int monstersDefeated, int combo);

    /// <summary>
    /// 計算寵物數值變化
    /// </summary>
    /// <param name="petId">寵物編號</param>
    /// <param name="result">遊戲結果</param>
    /// <param name="level">關卡等級</param>
    /// <returns>數值變化</returns>
    Task<PetStatsChangeDto> CalculatePetStatsChangeAsync(int petId, string result, int level);

    /// <summary>
    /// 檢查寵物是否可以冒險
    /// </summary>
    /// <param name="petId">寵物編號</param>
    /// <returns>是否可以冒險</returns>
    Task<(bool CanAdventure, List<string> Reasons)> CheckPetAdventureEligibilityAsync(int petId);

    /// <summary>
    /// 獲取今日剩餘次數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>剩餘次數</returns>
    Task<int> GetRemainingAttemptsAsync(int userId);

    /// <summary>
    /// 獲取系統配置
    /// </summary>
    /// <returns>系統配置</returns>
    Task<MiniGameSystemConfigDto> GetSystemConfigAsync();

    /// <summary>
    /// 更新系統配置 (管理員用)
    /// </summary>
    /// <param name="config">系統配置</param>
    /// <param name="managerId">管理員編號</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateSystemConfigAsync(MiniGameSystemConfigDto config, int managerId);

    /// <summary>
    /// 獲取成就進度
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>成就進度</returns>
    Task<List<object>> GetAchievementProgressAsync(int userId);

    /// <summary>
    /// 重置每日次數 (系統用)
    /// </summary>
    /// <returns>重置的用戶數</returns>
    Task<int> ResetDailyAttemptsAsync();
}