using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces;

/// <summary>
/// 每日簽到服務介面
/// </summary>
public interface ISignInService
{
    /// <summary>
    /// 獲取簽到狀態
    /// </summary>
    Task<ServiceResult<SignInStatusDto>> GetSignInStatusAsync(int userId);

    /// <summary>
    /// 執行每日簽到
    /// </summary>
    Task<ServiceResult<SignInResultDto>> SignInAsync(int userId);

    /// <summary>
    /// 獲取簽到歷史
    /// </summary>
    Task<ServiceResult<PagedResult<SignInHistoryDto>>> GetSignInHistoryAsync(int userId, int page = 1, int pageSize = 20);
} 