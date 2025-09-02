using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces;

/// <summary>
/// 虛擬寵物服務介面
/// </summary>
public interface IPetService
{
    /// <summary>
    /// 獲取寵物資訊
    /// </summary>
    Task<ServiceResult<PetDto>> GetPetAsync(int userId);

    /// <summary>
    /// 更新寵物資料
    /// </summary>
    Task<ServiceResult<PetDto>> UpdatePetProfileAsync(int userId, UpdatePetProfileRequest request);

    /// <summary>
    /// 寵物互動（餵食、洗澡、玩耍、休息）
    /// </summary>
    Task<ServiceResult<PetActionResultDto>> PerformActionAsync(int userId, string action);

    /// <summary>
    /// 寵物換色
    /// </summary>
    Task<ServiceResult<PetRecolorResultDto>> RecolorPetAsync(int userId, PetRecolorRequest request);

    /// <summary>
    /// 獲取寵物歷史記錄
    /// </summary>
    Task<ServiceResult<PagedResult<PetHistoryDto>>> GetPetHistoryAsync(int userId, int page = 1, int pageSize = 20);
} 