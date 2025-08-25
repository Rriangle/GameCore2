namespace GameCore.Api.DTOs;

/// <summary>
/// 假資料統計 DTO
/// </summary>
public class FakeDataStatsDto
{
    /// <summary>
    /// 總用戶數
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// 總用戶介紹數
    /// </summary>
    public int TotalUserIntroduces { get; set; }

    /// <summary>
    /// 總用戶權限數
    /// </summary>
    public int TotalUserRights { get; set; }

    /// <summary>
    /// 總用戶錢包數
    /// </summary>
    public int TotalUserWallets { get; set; }

    /// <summary>
    /// 總銷售權限申請數
    /// </summary>
    public int TotalMemberSalesProfiles { get; set; }

    /// <summary>
    /// 總銷售錢包數
    /// </summary>
    public int TotalUserSalesInformations { get; set; }
}