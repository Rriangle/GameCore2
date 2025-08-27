using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 錢包服務介面，定義錢包相關的業務邏輯操作
/// </summary>
public interface IWalletService
{
    /// <summary>
    /// 取得用戶錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>錢包餘額資訊</returns>
    Task<WalletBalanceDto?> GetWalletBalanceAsync(int userId);

    /// <summary>
    /// 取得點數交易明細 (分頁查詢)
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁交易明細</returns>
    Task<PagedPointTransactionsDto> GetPointTransactionsAsync(int userId, PointTransactionQueryDto query);

    /// <summary>
    /// 增加用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="points">增加的點數</param>
    /// <param name="source">來源 (signin, minigame, pet_color, adjustment)</param>
    /// <param name="description">交易描述</param>
    /// <param name="sourceId">來源記錄編號</param>
    /// <returns>是否成功</returns>
    Task<bool> AddPointsAsync(int userId, int points, string source, string description, string? sourceId = null);

    /// <summary>
    /// 扣除用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="points">扣除的點數</param>
    /// <param name="source">來源 (pet_color, store_purchase, market_purchase)</param>
    /// <param name="description">交易描述</param>
    /// <param name="sourceId">來源記錄編號</param>
    /// <returns>是否成功 (餘額不足會返回 false)</returns>
    Task<bool> DeductPointsAsync(int userId, int points, string source, string description, string? sourceId = null);

    /// <summary>
    /// 管理者調整用戶點數
    /// </summary>
    /// <param name="adminUserId">管理者編號</param>
    /// <param name="request">調整請求</param>
    /// <returns>是否成功</returns>
    Task<bool> AdminAdjustPointsAsync(int adminUserId, AdminAdjustPointsDto request);

    /// <summary>
    /// 檢查用戶是否有足夠點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="requiredPoints">需要的點數</param>
    /// <returns>是否有足夠點數</returns>
    Task<bool> HasSufficientPointsAsync(int userId, int requiredPoints);

    /// <summary>
    /// 取得銀行代號清單
    /// </summary>
    /// <returns>銀行代號清單</returns>
    Task<List<BankCodeDto>> GetBankCodesAsync();
}