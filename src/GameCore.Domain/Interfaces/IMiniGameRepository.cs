using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 小遊戲 Repository 介面
/// </summary>
public interface IMiniGameRepository
{
    /// <summary>
    /// 根據用戶ID獲取遊戲記錄（分頁）
    /// </summary>
    Task<IEnumerable<MiniGame>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20, string? result = null);

    /// <summary>
    /// 獲取用戶遊戲記錄總數
    /// </summary>
    Task<int> GetCountByUserIdAsync(int userId, string? result = null);

    /// <summary>
    /// 獲取用戶所有遊戲記錄
    /// </summary>
    Task<IEnumerable<MiniGame>> GetAllByUserIdAsync(int userId);

    /// <summary>
    /// 獲取用戶最後一次遊戲記錄
    /// </summary>
    Task<MiniGame?> GetLastGameAsync(int userId);

    /// <summary>
    /// 獲取用戶今日遊戲次數
    /// </summary>
    Task<int> GetTodayGamesCountAsync(int userId, DateTime today);

    /// <summary>
    /// 根據ID獲取遊戲記錄
    /// </summary>
    Task<MiniGame?> GetByIdAsync(int id);

    /// <summary>
    /// 新增遊戲記錄
    /// </summary>
    Task<MiniGame> AddAsync(MiniGame miniGame);

    /// <summary>
    /// 更新遊戲記錄
    /// </summary>
    Task<bool> UpdateAsync(MiniGame miniGame);

    /// <summary>
    /// 刪除遊戲記錄
    /// </summary>
    Task<bool> DeleteAsync(int id);

    // 效能優化：新增資料庫層面計數方法，避免記憶體載入
    /// <summary>
    /// 獲取用戶總遊戲次數
    /// </summary>
    Task<int> GetTotalGamesCountAsync(int userId);

    /// <summary>
    /// 獲取用戶勝利遊戲次數
    /// </summary>
    Task<int> GetWinGamesCountAsync(int userId);

    /// <summary>
    /// 獲取用戶失敗遊戲次數
    /// </summary>
    Task<int> GetLoseGamesCountAsync(int userId);

    /// <summary>
    /// 獲取用戶中止遊戲次數
    /// </summary>
    Task<int> GetAbortedGamesCountAsync(int userId);

    /// <summary>
    /// 獲取用戶總經驗值
    /// </summary>
    Task<int> GetTotalExpGainedAsync(int userId);

    /// <summary>
    /// 獲取用戶總點數
    /// </summary>
    Task<int> GetTotalPointsGainedAsync(int userId);

    /// <summary>
    /// 獲取用戶最高等級
    /// </summary>
    Task<int> GetHighestLevelAsync(int userId);
} 