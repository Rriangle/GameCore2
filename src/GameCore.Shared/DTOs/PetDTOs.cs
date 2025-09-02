using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs;

/// <summary>
/// 寵物 DTO
/// </summary>
public class PetDto
{
    /// <summary>
    /// 寵物ID
    /// </summary>
    public int PetId { get; set; }

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
    /// 飢餓值 (0-100)
    /// </summary>
    public int Hunger { get; set; }

    /// <summary>
    /// 心情值 (0-100)
    /// </summary>
    public int Mood { get; set; }

    /// <summary>
    /// 體力值 (0-100)
    /// </summary>
    public int Stamina { get; set; }

    /// <summary>
    /// 清潔值 (0-100)
    /// </summary>
    public int Cleanliness { get; set; }

    /// <summary>
    /// 健康值 (0-100)
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
    /// 最後升級時間
    /// </summary>
    public DateTime LevelUpTime { get; set; }

    /// <summary>
    /// 最後膚色變更時間
    /// </summary>
    public DateTime ColorChangedTime { get; set; }

    /// <summary>
    /// 最後背景色變更時間
    /// </summary>
    public DateTime BackgroundColorChangedTime { get; set; }
}

/// <summary>
/// 更新寵物資料請求 DTO
/// </summary>
public class UpdatePetProfileRequest
{
    /// <summary>
    /// 寵物名稱
    /// </summary>
    [StringLength(50)]
    public string? PetName { get; set; }

    /// <summary>
    /// 膚色
    /// </summary>
    [StringLength(50)]
    public string? SkinColor { get; set; }

    /// <summary>
    /// 背景色
    /// </summary>
    [StringLength(50)]
    public string? BackgroundColor { get; set; }
}

/// <summary>
/// 寵物互動結果 DTO
/// </summary>
public class PetActionResultDto
{
    /// <summary>
    /// 互動成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 互動類型
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// 更新後的寵物資料
    /// </summary>
    public PetDto? Pet { get; set; }

    /// <summary>
    /// 互動訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 是否升級
    /// </summary>
    public bool LevelUp { get; set; }

    /// <summary>
    /// 升級獎勵點數
    /// </summary>
    public int LevelUpBonus { get; set; }
}

/// <summary>
/// 寵物換色請求 DTO
/// </summary>
public class PetRecolorRequest
{
    /// <summary>
    /// 膚色
    /// </summary>
    [Required]
    [StringLength(50)]
    public string SkinColor { get; set; } = string.Empty;

    /// <summary>
    /// 背景色
    /// </summary>
    [Required]
    [StringLength(50)]
    public string BackgroundColor { get; set; } = string.Empty;
}

/// <summary>
/// 寵物換色結果 DTO
/// </summary>
public class PetRecolorResultDto
{
    /// <summary>
    /// 換色成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 更新後的寵物資料
    /// </summary>
    public PetDto? Pet { get; set; }

    /// <summary>
    /// 花費點數
    /// </summary>
    public int PointsSpent { get; set; }

    /// <summary>
    /// 剩餘點數
    /// </summary>
    public int RemainingPoints { get; set; }

    /// <summary>
    /// 換色訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 寵物歷史記錄 DTO
/// </summary>
public class PetHistoryDto
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 記錄類型
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 記錄時間
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 記錄描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 相關數值
    /// </summary>
    public int? Value { get; set; }
} 