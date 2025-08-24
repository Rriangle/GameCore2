namespace GameCore.Shared.DTOs;

/// <summary>
/// 寵物狀態 DTO
/// </summary>
public class PetStatusDto
{
    /// <summary>
    /// 寵物編號
    /// </summary>
    public int PetID { get; set; }

    /// <summary>
    /// 寵物主人編號
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 寵物主人名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 寵物名稱
    /// </summary>
    public string PetName { get; set; } = string.Empty;

    /// <summary>
    /// 寵物等級
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 當前經驗值
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// 升級所需經驗值
    /// </summary>
    public int ExperienceToNextLevel { get; set; }

    /// <summary>
    /// 升級進度百分比
    /// </summary>
    public int LevelProgress { get; set; }

    /// <summary>
    /// 飢餓值（0-100）
    /// </summary>
    public int Hunger { get; set; }

    /// <summary>
    /// 心情值（0-100）
    /// </summary>
    public int Mood { get; set; }

    /// <summary>
    /// 體力值（0-100）
    /// </summary>
    public int Stamina { get; set; }

    /// <summary>
    /// 清潔值（0-100）
    /// </summary>
    public int Cleanliness { get; set; }

    /// <summary>
    /// 健康度（0-100）
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// 膚色
    /// </summary>
    public string SkinColor { get; set; } = string.Empty;

    /// <summary>
    /// 背景色
    /// </summary>
    public string BackgroundColor { get; set; } = string.Empty;

    /// <summary>
    /// 寵物狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 是否可以進行冒險
    /// </summary>
    public bool CanAdventure { get; set; }

    /// <summary>
    /// 禁止冒險的原因
    /// </summary>
    public List<string> AdventureBlockReasons { get; set; } = new();

    /// <summary>
    /// 今日互動次數
    /// </summary>
    public int TodayInteractions { get; set; }

    /// <summary>
    /// 總互動次數
    /// </summary>
    public int TotalInteractions { get; set; }

    /// <summary>
    /// 最後互動時間
    /// </summary>
    public DateTime? LastInteractionAt { get; set; }

    /// <summary>
    /// 最後升級時間
    /// </summary>
    public DateTime LevelUpTime { get; set; }

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 寵物年齡 (天數)
    /// </summary>
    public int AgeDays { get; set; }

    /// <summary>
    /// 外觀配置
    /// </summary>
    public string? AppearanceConfig { get; set; }

    /// <summary>
    /// 各項數值的狀態評價
    /// </summary>
    public PetStatsEvaluationDto StatsEvaluation { get; set; } = new();
}

/// <summary>
/// 寵物數值評價 DTO
/// </summary>
public class PetStatsEvaluationDto
{
    /// <summary>
    /// 飢餓狀態 (excellent/good/normal/poor/critical)
    /// </summary>
    public string HungerLevel { get; set; } = string.Empty;

    /// <summary>
    /// 心情狀態
    /// </summary>
    public string MoodLevel { get; set; } = string.Empty;

    /// <summary>
    /// 體力狀態
    /// </summary>
    public string StaminaLevel { get; set; } = string.Empty;

    /// <summary>
    /// 清潔狀態
    /// </summary>
    public string CleanlinessLevel { get; set; } = string.Empty;

    /// <summary>
    /// 健康狀態
    /// </summary>
    public string HealthLevel { get; set; } = string.Empty;

    /// <summary>
    /// 整體狀態評分 (1-5 星)
    /// </summary>
    public int OverallRating { get; set; }

    /// <summary>
    /// 狀態建議
    /// </summary>
    public List<string> Suggestions { get; set; } = new();
}

/// <summary>
/// 寵物互動請求 DTO
/// </summary>
public class PetInteractionRequestDto
{
    /// <summary>
    /// 互動類型 (feed/bathe/play/rest)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 互動強度 (1-3，可選)
    /// </summary>
    public int Intensity { get; set; } = 1;

    /// <summary>
    /// 互動備註
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// 寵物互動結果 DTO
/// </summary>
public class PetInteractionResultDto
{
    /// <summary>
    /// 互動是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 互動類型
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 各項數值變化
    /// </summary>
    public PetStatsChangeDto StatsChange { get; set; } = new();

    /// <summary>
    /// 互動後的寵物狀態
    /// </summary>
    public PetStatusDto? UpdatedStatus { get; set; }

    /// <summary>
    /// 是否觸發健康度滿值
    /// </summary>
    public bool TriggeredHealthBonus { get; set; }

    /// <summary>
    /// 是否觸發升級
    /// </summary>
    public bool TriggeredLevelUp { get; set; }

    /// <summary>
    /// 升級獎勵 (如果有)
    /// </summary>
    public int LevelUpReward { get; set; }

    /// <summary>
    /// 互動描述
    /// </summary>
    public string? InteractionDescription { get; set; }

    /// <summary>
    /// 獲得經驗值 (如果有)
    /// </summary>
    public int ExpGained { get; set; }
}

/// <summary>
/// 寵物數值變化 DTO
/// </summary>
public class PetStatsChangeDto
{
    /// <summary>
    /// 飢餓值變化
    /// </summary>
    public int HungerDelta { get; set; }

    /// <summary>
    /// 心情值變化
    /// </summary>
    public int MoodDelta { get; set; }

    /// <summary>
    /// 體力值變化
    /// </summary>
    public int StaminaDelta { get; set; }

    /// <summary>
    /// 清潔值變化
    /// </summary>
    public int CleanlinessDelta { get; set; }

    /// <summary>
    /// 健康度變化
    /// </summary>
    public int HealthDelta { get; set; }

    /// <summary>
    /// 經驗值變化
    /// </summary>
    public int ExperienceDelta { get; set; }

    /// <summary>
    /// 等級變化
    /// </summary>
    public int LevelDelta { get; set; }
}

/// <summary>
/// 寵物換色請求 DTO
/// </summary>
public class PetRecolorRequestDto
{
    /// <summary>
    /// 新膚色
    /// </summary>
    public string? SkinColor { get; set; }

    /// <summary>
    /// 新背景色
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// 確認扣款 (必須為true)
    /// </summary>
    public bool ConfirmPayment { get; set; }
}

/// <summary>
/// 寵物換色結果 DTO
/// </summary>
public class PetRecolorResultDto
{
    /// <summary>
    /// 換色是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 扣除的點數
    /// </summary>
    public int PointsDeducted { get; set; }

    /// <summary>
    /// 更新後的點數餘額
    /// </summary>
    public int UpdatedBalance { get; set; }

    /// <summary>
    /// 新膚色
    /// </summary>
    public string? NewSkinColor { get; set; }

    /// <summary>
    /// 新背景色
    /// </summary>
    public string? NewBackgroundColor { get; set; }

    /// <summary>
    /// 換色時間
    /// </summary>
    public DateTime ColorChangedTime { get; set; }
}

/// <summary>
/// 寵物資料更新 DTO
/// </summary>
public class PetProfileUpdateDto
{
    /// <summary>
    /// 寵物名稱 (可選)
    /// </summary>
    public string? PetName { get; set; }

    /// <summary>
    /// 備註 (可選)
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 外觀配置 (可選)
    /// </summary>
    public string? AppearanceConfig { get; set; }
}

/// <summary>
/// 寵物等級配置 DTO
/// </summary>
public class PetLevelConfigDto
{
    /// <summary>
    /// 等級
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 該等級所需經驗值
    /// </summary>
    public int ExperienceRequired { get; set; }

    /// <summary>
    /// 累計經驗值
    /// </summary>
    public int CumulativeExperience { get; set; }

    /// <summary>
    /// 升級獎勵點數
    /// </summary>
    public int LevelUpReward { get; set; }

    /// <summary>
    /// 等級標題
    /// </summary>
    public string? LevelTitle { get; set; }

    /// <summary>
    /// 等級描述
    /// </summary>
    public string? LevelDescription { get; set; }
}

/// <summary>
/// 寵物統計 DTO
/// </summary>
public class PetStatisticsDto
{
    /// <summary>
    /// 寵物編號
    /// </summary>
    public int PetID { get; set; }

    /// <summary>
    /// 總互動次數
    /// </summary>
    public int TotalInteractions { get; set; }

    /// <summary>
    /// 餵食次數
    /// </summary>
    public int FeedCount { get; set; }

    /// <summary>
    /// 洗澡次數
    /// </summary>
    public int BatheCount { get; set; }

    /// <summary>
    /// 玩耍次數
    /// </summary>
    public int PlayCount { get; set; }

    /// <summary>
    /// 休息次數
    /// </summary>
    public int RestCount { get; set; }

    /// <summary>
    /// 換色次數
    /// </summary>
    public int RecolorCount { get; set; }

    /// <summary>
    /// 升級次數
    /// </summary>
    public int LevelUpCount { get; set; }

    /// <summary>
    /// 冒險次數
    /// </summary>
    public int AdventureCount { get; set; }

    /// <summary>
    /// 平均健康度
    /// </summary>
    public double AverageHealth { get; set; }

    /// <summary>
    /// 最高等級
    /// </summary>
    public int MaxLevel { get; set; }

    /// <summary>
    /// 寵物存活天數
    /// </summary>
    public int AliveDays { get; set; }

    /// <summary>
    /// 首次創建時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// 寵物日常衰減結果 DTO
/// </summary>
public class PetDecayResultDto
{
    /// <summary>
    /// 是否執行了衰減
    /// </summary>
    public bool DecayApplied { get; set; }

    /// <summary>
    /// 衰減時間
    /// </summary>
    public DateTime DecayTime { get; set; }

    /// <summary>
    /// 數值變化
    /// </summary>
    public PetStatsChangeDto StatsChange { get; set; } = new();

    /// <summary>
    /// 衰減後的寵物狀態
    /// </summary>
    public PetStatusDto? UpdatedStatus { get; set; }

    /// <summary>
    /// 是否觸發了健康警告
    /// </summary>
    public bool HealthWarningTriggered { get; set; }

    /// <summary>
    /// 警告訊息
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// 寵物顏色選項 DTO
/// </summary>
public class PetColorOptionDto
{
    /// <summary>
    /// 顏色名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 顏色值 (hex)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 顏色類型 (skin/background)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 是否為預設色
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 是否需要特殊解鎖
    /// </summary>
    public bool RequiresUnlock { get; set; }

    /// <summary>
    /// 解鎖條件描述
    /// </summary>
    public string? UnlockCondition { get; set; }

    /// <summary>
    /// 顏色預覽圖
    /// </summary>
    public string? PreviewImage { get; set; }
}

/// <summary>
/// 寵物系統配置 DTO (管理員用)
/// </summary>
public class PetSystemConfigDto
{
    /// <summary>
    /// 換色費用 (點數)
    /// </summary>
    public int RecolorCost { get; set; } = 2000;

    /// <summary>
    /// 每日衰減配置
    /// </summary>
    public PetDecayConfigDto DailyDecay { get; set; } = new();

    /// <summary>
    /// 互動效果配置
    /// </summary>
    public PetInteractionConfigDto InteractionEffects { get; set; } = new();

    /// <summary>
    /// 健康檢查規則
    /// </summary>
    public PetHealthRulesDto HealthRules { get; set; } = new();

    /// <summary>
    /// 升級獎勵配置
    /// </summary>
    public List<PetLevelConfigDto> LevelRewards { get; set; } = new();

    /// <summary>
    /// 是否啟用互動冷卻
    /// </summary>
    public bool EnableInteractionCooldown { get; set; } = false;

    /// <summary>
    /// 互動冷卻時間 (分鐘)
    /// </summary>
    public int InteractionCooldownMinutes { get; set; } = 0;
}

/// <summary>
/// 寵物衰減配置 DTO
/// </summary>
public class PetDecayConfigDto
{
    /// <summary>
    /// 飢餓值衰減
    /// </summary>
    public int HungerDecay { get; set; } = -20;

    /// <summary>
    /// 心情值衰減
    /// </summary>
    public int MoodDecay { get; set; } = -30;

    /// <summary>
    /// 體力值衰減
    /// </summary>
    public int StaminaDecay { get; set; } = -10;

    /// <summary>
    /// 清潔值衰減
    /// </summary>
    public int CleanlinessDecay { get; set; } = -20;

    /// <summary>
    /// 健康度衰減
    /// </summary>
    public int HealthDecay { get; set; } = -20;
}

/// <summary>
/// 寵物互動配置 DTO
/// </summary>
public class PetInteractionConfigDto
{
    /// <summary>
    /// 餵食效果
    /// </summary>
    public int FeedEffect { get; set; } = 10;

    /// <summary>
    /// 洗澡效果
    /// </summary>
    public int BatheEffect { get; set; } = 10;

    /// <summary>
    /// 玩耍效果
    /// </summary>
    public int PlayEffect { get; set; } = 10;

    /// <summary>
    /// 休息效果
    /// </summary>
    public int RestEffect { get; set; } = 10;

    /// <summary>
    /// 互動給予經驗值
    /// </summary>
    public int InteractionExp { get; set; } = 5;
}

/// <summary>
/// 寵物健康規則 DTO
/// </summary>
public class PetHealthRulesDto
{
    /// <summary>
    /// 健康警告閾值
    /// </summary>
    public int HealthWarningThreshold { get; set; } = 30;

    /// <summary>
    /// 禁止冒險閾值
    /// </summary>
    public int AdventureBanThreshold { get; set; } = 30;

    /// <summary>
    /// 健康懲罰值
    /// </summary>
    public int HealthPenalty { get; set; } = -20;

    /// <summary>
    /// 滿值健康獎勵
    /// </summary>
    public bool EnableFullHealthBonus { get; set; } = true;
}