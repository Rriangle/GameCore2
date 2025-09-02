using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 銷售服務介面
/// </summary>
public interface ISalesService
{
    /// <summary>
    /// 取得使用者銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>銷售資料</returns>
    Task<SalesProfileDto?> GetSalesProfileAsync(int userId);

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="request">更新請求</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> UpdateSalesProfileAsync(int userId, UpdateSalesProfileRequest request);

    /// <summary>
    /// 取得銷售統計
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>銷售統計</returns>
    Task<SalesStatisticsDto?> GetSalesStatisticsAsync(int userId);

    /// <summary>
    /// 記錄銷售交易
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="orderAmount">訂單金額</param>
    /// <param name="customerId">客戶ID</param>
    /// <returns>操作結果</returns>
    Task<ServiceResult<bool>> RecordSalesTransactionAsync(int userId, decimal orderAmount, int customerId);

    /// <summary>
    /// 取得銷售排行榜
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>銷售排行榜</returns>
    Task<List<SalesRankingDto>> GetSalesRankingAsync(int top = 10);

    /// <summary>
    /// 計算佣金
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="orderAmount">訂單金額</param>
    /// <returns>佣金金額</returns>
    Task<decimal> CalculateCommissionAsync(int userId, decimal orderAmount);
} 