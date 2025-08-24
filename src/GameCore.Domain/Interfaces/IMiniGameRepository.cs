using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 小遊戲存儲接口
/// </summary>
public interface IMiniGameRepository
{
    /// <summary>
    /// 根據編號獲取遊戲記錄
    /// </summary>
    /// <param name="playId">遊戲記錄編號</param>
    /// <returns>遊戲記錄實體</returns>
    Task<MiniGame?> GetByIdAsync(int playId);

    /// <summary>
    /// 獲取用戶遊戲記錄
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="limit">限制數量</param>
    /// <returns>遊戲記錄列表</returns>
    Task<IEnumerable<MiniGame>> GetByUserIdAsync(int userId, int? limit = null);

    /// <summary>
    /// 分頁獲取遊戲記錄
    /// </summary>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <param name="userId">用戶編號篩選</param>
    /// <param name="petId">寵物編號篩選</param>
    /// <param name="result">結果篩選</param>
    /// <param name="level">關卡篩選</param>
    /// <param name="startTime">開始時間篩選</param>
    /// <param name="endTime">結束時間篩選</param>
    /// <returns>分頁結果</returns>
    Task<(IEnumerable<MiniGame> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, int? userId = null, int? petId = null, 
        string? result = null, int? level = null, DateTime? startTime = null, DateTime? endTime = null);

    /// <summary>
    /// 創建遊戲記錄
    /// </summary>
    /// <param name="miniGame">遊戲記錄實體</param>
    /// <returns>創建的遊戲記錄</returns>
    Task<MiniGame> CreateAsync(MiniGame miniGame);

    /// <summary>
    /// 更新遊戲記錄
    /// </summary>
    /// <param name="miniGame">遊戲記錄實體</param>
    /// <returns>更新的遊戲記錄</returns>
    Task<MiniGame> UpdateAsync(MiniGame miniGame);

    /// <summary>
    /// 獲取用戶今日遊戲次數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="date">日期</param>
    /// <returns>遊戲次數</returns>
    Task<int> GetTodayGameCountAsync(int userId, DateOnly? date = null);

    /// <summary>
    /// 獲取用戶最高關卡
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>最高關卡</returns>
    Task<int> GetMaxLevelAsync(int userId);

    /// <summary>
    /// 獲取用戶遊戲統計
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>統計資訊</returns>
    Task<(int Total, int Win, int Lose, int Abort, int TotalExp, int TotalPoints, int MaxCombo)> GetStatisticsAsync(int userId);

    /// <summary>
    /// 獲取未完成的遊戲
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>未完成的遊戲記錄</returns>
    Task<MiniGame?> GetPendingGameAsync(int userId);

    /// <summary>
    /// 獲取寵物遊戲記錄
    /// </summary>
    /// <param name="petId">寵物編號</param>
    /// <param name="limit">限制數量</param>
    /// <returns>遊戲記錄列表</returns>
    Task<IEnumerable<MiniGame>> GetByPetIdAsync(int petId, int? limit = null);

    /// <summary>
    /// 獲取關卡統計
    /// </summary>
    /// <param name="level">關卡等級</param>
    /// <returns>統計資訊</returns>
    Task<(int Total, int Win, int Lose, double AvgDuration)> GetLevelStatisticsAsync(int level);

    /// <summary>
    /// 獲取遊戲排行榜
    /// </summary>
    /// <param name="type">排行榜類型 (level/exp/points/combo)</param>
    /// <param name="limit">限制數量</param>
    /// <returns>排行榜列表</returns>
    Task<IEnumerable<(int UserId, string UserName, int Value)>> GetLeaderboardAsync(string type, int limit = 10);

    /// <summary>
    /// 獲取系統遊戲統計
    /// </summary>
    /// <returns>統計資訊</returns>
    Task<(int TotalGames, int TodayGames, int TotalUsers, double AvgDuration)> GetSystemStatisticsAsync();

    /// <summary>
    /// 獲取遊戲模式統計
    /// </summary>
    /// <param name="gameMode">遊戲模式</param>
    /// <returns>統計資訊</returns>
    Task<(int Total, int Win, int Lose)> GetGameModeStatisticsAsync(string gameMode);

    /// <summary>
    /// 獲取難度統計
    /// </summary>
    /// <param name="difficulty">難度</param>
    /// <returns>統計資訊</returns>
    Task<(int Total, int Win, int Lose)> GetDifficultyStatisticsAsync(string difficulty);
}