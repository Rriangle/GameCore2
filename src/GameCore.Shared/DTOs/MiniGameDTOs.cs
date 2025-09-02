using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 小遊戲開始結果 DTO
/// </summary>
public class MiniGameStartResultDto
{
    /// <summary>
    /// 開始成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 遊戲ID
    /// </summary>
    public int PlayId { get; set; }

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
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 寵物狀態
    /// </summary>
    public PetDto? Pet { get; set; }
}

/// <summary>
/// 小遊戲完成請求 DTO
/// </summary>
public class MiniGameFinishRequest
{
    /// <summary>
    /// 遊戲ID
    /// </summary>
    [Required]
    public int PlayId { get; set; }

    /// <summary>
    /// 遊戲結果
    /// </summary>
    [Required]
    public string Result { get; set; } = string.Empty; // Win, Lose, Abort

    /// <summary>
    /// 是否中途放棄
    /// </summary>
    public bool Aborted { get; set; } = false;
}

/// <summary>
/// 小遊戲完成結果 DTO
/// </summary>
public class MiniGameFinishResultDto
{
    /// <summary>
    /// 完成成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 遊戲結果
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// 獲得經驗值
    /// </summary>
    public int ExpGained { get; set; }

    /// <summary>
    /// 獲得點數
    /// </summary>
    public int PointsGained { get; set; }

    /// <summary>
    /// 寵物狀態變化
    /// </summary>
    public PetStatusChangeDto? PetStatusChange { get; set; }

    /// <summary>
    /// 更新後的寵物資料
    /// </summary>
    public PetDto? Pet { get; set; }

    /// <summary>
    /// 遊戲訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime EndTime { get; set; }
}

/// <summary>
/// 寵物狀態變化 DTO
/// </summary>
public class PetStatusChangeDto
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
    /// 健康值變化
    /// </summary>
    public int HealthDelta { get; set; }
}

/// <summary>
/// 小遊戲記錄 DTO
/// </summary>
public class MiniGameRecordDto
{
    /// <summary>
    /// 遊戲記錄ID
    /// </summary>
    public int PlayId { get; set; }

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
    /// 獲得點數
    /// </summary>
    public int PointsGained { get; set; }

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 是否中途放棄
    /// </summary>
    public bool Aborted { get; set; }
}

/// <summary>
/// 小遊戲統計 DTO
/// </summary>
public class MiniGameStatsDto
{
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
    /// 中途放棄次數
    /// </summary>
    public int AbortCount { get; set; }

    /// <summary>
    /// 勝率
    /// </summary>
    public decimal WinRate { get; set; }

    /// <summary>
    /// 總獲得經驗值
    /// </summary>
    public int TotalExpGained { get; set; }

    /// <summary>
    /// 總獲得點數
    /// </summary>
    public int TotalPointsGained { get; set; }

    /// <summary>
    /// 最高關卡
    /// </summary>
    public int HighestLevel { get; set; }

    /// <summary>
    /// 今日遊戲次數
    /// </summary>
    public int TodayGames { get; set; }

    /// <summary>
    /// 今日剩餘次數
    /// </summary>
    public int RemainingGames { get; set; }
} 