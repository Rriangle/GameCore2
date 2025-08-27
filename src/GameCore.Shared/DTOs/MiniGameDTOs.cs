namespace GameCore.Shared.DTOs;

/// <summary>
/// 小遊戲開始請求 DTO
/// </summary>
public class MiniGameStartRequestDto
{
    /// <summary>
    /// 選擇的關卡等級
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// 遊戲難度 (easy/normal/hard)
    /// </summary>
    public string Difficulty { get; set; } = "normal";

    /// <summary>
    /// 遊戲模式 (adventure/survival/boss)
    /// </summary>
    public string GameMode { get; set; } = "adventure";

    /// <summary>
    /// 是否為練習模式
    /// </summary>
    public bool IsPracticeMode { get; set; } = false;

    /// <summary>
    /// 客戶端版本
    /// </summary>
    public string? ClientVersion { get; set; }
}

/// <summary>
/// 小遊戲開始結果 DTO
/// </summary>
public class MiniGameStartResultDto
{
    /// <summary>
    /// 是否成功開始
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 遊戲記錄編號
    /// </summary>
    public int PlayID { get; set; }

    /// <summary>
    /// 關卡配置
    /// </summary>
    public MiniGameLevelConfigDto? LevelConfig { get; set; }

    /// <summary>
    /// 寵物當前狀態
    /// </summary>
    public PetStatusDto? PetStatus { get; set; }

    /// <summary>
    /// 遊戲開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 今日剩餘次數
    /// </summary>
    public int RemainingAttemptsToday { get; set; }

    /// <summary>
    /// 預期獎勵
    /// </summary>
    public MiniGameRewardsDto? ExpectedRewards { get; set; }
}

/// <summary>
/// 小遊戲結束請求 DTO
/// </summary>
public class MiniGameFinishRequestDto
{
    /// <summary>
    /// 遊戲記錄編號
    /// </summary>
    public int PlayID { get; set; }

    /// <summary>
    /// 遊戲結果 (Win/Lose/Abort)
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// 擊敗的怪物數量
    /// </summary>
    public int MonstersDefeated { get; set; }

    /// <summary>
    /// 最高連擊數
    /// </summary>
    public int MaxCombo { get; set; }

    /// <summary>
    /// 遊戲總時長 (秒)
    /// </summary>
    public int DurationSeconds { get; set; }

    /// <summary>
    /// 遊戲評分 (1-5星)
    /// </summary>
    public int? Rating { get; set; }

    /// <summary>
    /// 獲得的成就 (JSON格式)
    /// </summary>
    public string? AchievementsEarned { get; set; }

    /// <summary>
    /// 遊戲備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 小遊戲結束結果 DTO
/// </summary>
public class MiniGameFinishResultDto
{
    /// <summary>
    /// 是否成功結束
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 遊戲記錄
    /// </summary>
    public MiniGameRecordDto? GameRecord { get; set; }

    /// <summary>
    /// 獲得的獎勵
    /// </summary>
    public MiniGameRewardsDto Rewards { get; set; } = new();

    /// <summary>
    /// 寵物數值變化
    /// </summary>
    public PetStatsChangeDto PetStatsChange { get; set; } = new();

    /// <summary>
    /// 更新後的寵物狀態
    /// </summary>
    public PetStatusDto? UpdatedPetStatus { get; set; }

    /// <summary>
    /// 是否觸發寵物升級
    /// </summary>
    public bool TriggeredPetLevelUp { get; set; }

    /// <summary>
    /// 升級獎勵 (如果有)
    /// </summary>
    public int PetLevelUpReward { get; set; }

    /// <summary>
    /// 更新後的會員點數餘額
    /// </summary>
    public int UpdatedPointsBalance { get; set; }

    /// <summary>
    /// 下次關卡建議
    /// </summary>
    public int SuggestedNextLevel { get; set; }

    /// <summary>
    /// 成就解鎖訊息
    /// </summary>
    public List<string> AchievementMessages { get; set; } = new();
}

/// <summary>
/// 小遊戲記錄 DTO
/// </summary>
public class MiniGameRecordDto
{
    /// <summary>
    /// 遊戲記錄編號
    /// </summary>
    public int PlayID { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 寵物編號
    /// </summary>
    public int PetID { get; set; }

    /// <summary>
    /// 寵物名稱
    /// </summary>
    public string? PetName { get; set; }

    /// <summary>
    /// 關卡等級
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 怪物數量
    /// </summary>
    public int MonsterCount { get; set; }

    /// <summary>
    /// 速度倍率
    /// </summary>
    public decimal SpeedMultiplier { get; set; }

    /// <summary>
    /// 遊戲結果
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int ExpGained { get; set; }

    /// <summary>
    /// 點數變化
    /// </summary>
    public int PointsChanged { get; set; }

    /// <summary>
    /// 寵物數值變化
    /// </summary>
    public PetStatsChangeDto StatsChange { get; set; } = new();

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 是否中退
    /// </summary>
    public bool Aborted { get; set; }

    /// <summary>
    /// 遊戲時長 (秒)
    /// </summary>
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// 擊敗怪物數
    /// </summary>
    public int MonstersDefeated { get; set; }

    /// <summary>
    /// 遊戲難度
    /// </summary>
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// 遊戲模式
    /// </summary>
    public string GameMode { get; set; } = string.Empty;

    /// <summary>
    /// 最高連擊
    /// </summary>
    public int MaxCombo { get; set; }

    /// <summary>
    /// 遊戲評分
    /// </summary>
    public int? Rating { get; set; }

    /// <summary>
    /// 是否為練習模式
    /// </summary>
    public bool IsPracticeMode { get; set; }

    /// <summary>
    /// 是否觸發升級
    /// </summary>
    public bool TriggeredLevelUp { get; set; }

    /// <summary>
    /// 獲得的成就
    /// </summary>
    public string? AchievementsEarned { get; set; }
}

/// <summary>
/// 小遊戲關卡配置 DTO
/// </summary>
public class MiniGameLevelConfigDto
{
    /// <summary>
    /// 關卡等級
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 怪物數量
    /// </summary>
    public int MonsterCount { get; set; }

    /// <summary>
    /// 速度倍率
    /// </summary>
    public decimal SpeedMultiplier { get; set; }

    /// <summary>
    /// 勝利獎勵
    /// </summary>
    public MiniGameRewardsDto WinRewards { get; set; } = new();

    /// <summary>
    /// 失敗懲罰
    /// </summary>
    public MiniGamePenaltiesDto LosePenalties { get; set; } = new();

    /// <summary>
    /// 關卡名稱
    /// </summary>
    public string? LevelName { get; set; }

    /// <summary>
    /// 關卡描述
    /// </summary>
    public string? LevelDescription { get; set; }

    /// <summary>
    /// 推薦寵物等級
    /// </summary>
    public int RecommendedPetLevel { get; set; }

    /// <summary>
    /// 是否已解鎖
    /// </summary>
    public bool IsUnlocked { get; set; }

    /// <summary>
    /// 解鎖條件
    /// </summary>
    public string? UnlockCondition { get; set; }

    /// <summary>
    /// 關卡圖標
    /// </summary>
    public string? LevelIcon { get; set; }

    /// <summary>
    /// 背景音樂
    /// </summary>
    public string? BackgroundMusic { get; set; }
}

/// <summary>
/// 小遊戲獎勵 DTO
/// </summary>
public class MiniGameRewardsDto
{
    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int ExpGained { get; set; }

    /// <summary>
    /// 獲得點數
    /// </summary>
    public int PointsGained { get; set; }

    /// <summary>
    /// 獎勵描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 額外獎勵 (JSON格式)
    /// </summary>
    public string? ExtraRewards { get; set; }

    /// <summary>
    /// 獎勵倍率
    /// </summary>
    public decimal Multiplier { get; set; } = 1.0m;
}

/// <summary>
/// 小遊戲懲罰 DTO
/// </summary>
public class MiniGamePenaltiesDto
{
    /// <summary>
    /// 飢餓值減少
    /// </summary>
    public int HungerPenalty { get; set; } = -20;

    /// <summary>
    /// 心情值減少
    /// </summary>
    public int MoodPenalty { get; set; } = -30;

    /// <summary>
    /// 體力值減少
    /// </summary>
    public int StaminaPenalty { get; set; } = -20;

    /// <summary>
    /// 清潔值減少
    /// </summary>
    public int CleanlinessPenalty { get; set; } = -20;

    /// <summary>
    /// 懲罰描述
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 小遊戲統計 DTO
/// </summary>
public class MiniGameStatisticsDto
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 總遊戲次數
    /// </summary>
    public int TotalGames { get; set; }

    /// <summary>
    /// 勝利次數
    /// </summary>
    public int WinCount { get; set; }

    /// <summary>
    /// 失敗次數
    /// </summary>
    public int LoseCount { get; set; }

    /// <summary>
    /// 中退次數
    /// </summary>
    public int AbortCount { get; set; }

    /// <summary>
    /// 勝率
    /// </summary>
    public double WinRate { get; set; }

    /// <summary>
    /// 最高關卡
    /// </summary>
    public int MaxLevel { get; set; }

    /// <summary>
    /// 累計獲得經驗值
    /// </summary>
    public int TotalExpGained { get; set; }

    /// <summary>
    /// 累計獲得點數
    /// </summary>
    public int TotalPointsGained { get; set; }

    /// <summary>
    /// 累計擊敗怪物數
    /// </summary>
    public int TotalMonstersDefeated { get; set; }

    /// <summary>
    /// 最高連擊記錄
    /// </summary>
    public int MaxComboRecord { get; set; }

    /// <summary>
    /// 平均遊戲時長 (秒)
    /// </summary>
    public double AverageGameDuration { get; set; }

    /// <summary>
    /// 今日遊戲次數
    /// </summary>
    public int TodayGames { get; set; }

    /// <summary>
    /// 今日剩餘次數
    /// </summary>
    public int TodayRemainingAttempts { get; set; }

    /// <summary>
    /// 首次遊戲時間
    /// </summary>
    public DateTime? FirstGameTime { get; set; }

    /// <summary>
    /// 最後遊戲時間
    /// </summary>
    public DateTime? LastGameTime { get; set; }

    /// <summary>
    /// 偏好遊戲模式
    /// </summary>
    public string? PreferredGameMode { get; set; }

    /// <summary>
    /// 偏好難度
    /// </summary>
    public string? PreferredDifficulty { get; set; }
}

/// <summary>
/// 小遊戲查詢參數 DTO
/// </summary>
public class MiniGameQueryDto
{
    /// <summary>
    /// 使用者編號 (管理員查詢用)
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 寵物編號
    /// </summary>
    public int? PetId { get; set; }

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 遊戲結果篩選
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// 關卡等級篩選
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// 遊戲模式篩選
    /// </summary>
    public string? GameMode { get; set; }

    /// <summary>
    /// 難度篩選
    /// </summary>
    public string? Difficulty { get; set; }

    /// <summary>
    /// 是否只顯示練習模式
    /// </summary>
    public bool? OnlyPracticeMode { get; set; }

    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool OrderDescending { get; set; } = true;
}

/// <summary>
/// 分頁小遊戲記錄 DTO
/// </summary>
public class PagedMiniGameRecordsDto
{
    /// <summary>
    /// 記錄列表
    /// </summary>
    public List<MiniGameRecordDto> Records { get; set; } = new();

    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// 統計資訊
    /// </summary>
    public MiniGameStatisticsDto? Statistics { get; set; }
}

/// <summary>
/// 小遊戲系統配置 DTO (管理員用)
/// </summary>
public class MiniGameSystemConfigDto
{
    /// <summary>
    /// 每日遊戲次數限制
    /// </summary>
    public int DailyAttemptsLimit { get; set; } = 3;

    /// <summary>
    /// 是否啟用練習模式
    /// </summary>
    public bool EnablePracticeMode { get; set; } = true;

    /// <summary>
    /// 關卡配置列表
    /// </summary>
    public List<MiniGameLevelConfigDto> LevelConfigs { get; set; } = new();

    /// <summary>
    /// 成就配置
    /// </summary>
    public string? AchievementConfigs { get; set; }

    /// <summary>
    /// 是否啟用系統
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 最低寵物健康度要求
    /// </summary>
    public int MinPetHealthRequired { get; set; } = 1;

    /// <summary>
    /// 經驗值倍率
    /// </summary>
    public decimal ExpMultiplier { get; set; } = 1.0m;

    /// <summary>
    /// 點數倍率
    /// </summary>
    public decimal PointsMultiplier { get; set; } = 1.0m;

    /// <summary>
    /// 是否啟用關卡進度限制
    /// </summary>
    public bool EnableLevelProgression { get; set; } = true;

    /// <summary>
    /// 最大關卡等級
    /// </summary>
    public int MaxLevel { get; set; } = 50;
}

/// <summary>
/// 小遊戲狀態檢查 DTO
/// </summary>
public class MiniGameStatusDto
{
    /// <summary>
    /// 是否可以開始遊戲
    /// </summary>
    public bool CanStartGame { get; set; }

    /// <summary>
    /// 無法開始的原因
    /// </summary>
    public List<string> BlockReasons { get; set; } = new();

    /// <summary>
    /// 今日剩餘次數
    /// </summary>
    public int RemainingAttemptsToday { get; set; }

    /// <summary>
    /// 下次重置時間
    /// </summary>
    public DateTime NextResetTime { get; set; }

    /// <summary>
    /// 當前可挑戰的最高關卡
    /// </summary>
    public int MaxAvailableLevel { get; set; }

    /// <summary>
    /// 寵物當前狀態
    /// </summary>
    public PetStatusDto? PetStatus { get; set; }

    /// <summary>
    /// 推薦關卡
    /// </summary>
    public int RecommendedLevel { get; set; }

    /// <summary>
    /// 系統是否啟用
    /// </summary>
    public bool SystemEnabled { get; set; }

    /// <summary>
    /// 未完成的遊戲記錄 (如果有)
    /// </summary>
    public MiniGameRecordDto? PendingGame { get; set; }
}