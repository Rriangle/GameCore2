using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;

namespace GameCore.Api.Services;

/// <summary>
/// 銷售服務
/// 處理銷售相關業務邏輯
/// </summary>
public class SalesService : ISalesService
{
    private readonly IUserRepository _userRepository;
    private readonly IMemberSalesProfileRepository _salesProfileRepository;
    private readonly IUserSalesInformationRepository _salesInfoRepository;

    public SalesService(
        IUserRepository userRepository,
        IMemberSalesProfileRepository salesProfileRepository,
        IUserSalesInformationRepository salesInfoRepository)
    {
        _userRepository = userRepository;
        _salesProfileRepository = salesProfileRepository;
        _salesInfoRepository = salesInfoRepository;
    }

    /// <summary>
    /// 取得使用者銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>銷售資料</returns>
    public async Task<SalesProfileDto?> GetSalesProfileAsync(int userId)
    {
        var profile = await _salesProfileRepository.GetByUserIdAsync(userId);
        if (profile == null) return null;

        var salesProfile = new SalesProfileDto
        {
            UserId = profile.User_Id,
            SalesLevel = profile.Sales_Level,
            TotalSales = profile.Total_Sales,
            CommissionRate = profile.Commission_Rate,
            IsActive = profile.Is_Active,
            LastUpdated = profile.Last_Updated
        };

        return salesProfile;
    }

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="profile">銷售資料</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> UpdateSalesProfileAsync(int userId, UpdateSalesProfileRequest request)
    {
        var existingProfile = await _salesProfileRepository.GetByUserIdAsync(userId);
        if (existingProfile == null)
            return ServiceResult<bool>.CreateFailure("銷售資料不存在");

        // 更新銷售等級
        if (request.SalesLevel.HasValue)
            existingProfile.Sales_Level = request.SalesLevel.Value;

        // 更新佣金率
        if (request.CommissionRate.HasValue)
        {
            if (request.CommissionRate.Value < 0 || request.CommissionRate.Value > 100)
                return ServiceResult<bool>.CreateFailure("佣金率必須在 0-100 之間");

            existingProfile.Commission_Rate = request.CommissionRate.Value;
        }

        // 更新啟用狀態
        if (request.IsActive.HasValue)
            existingProfile.Is_Active = request.IsActive.Value;

        existingProfile.Last_Updated = DateTime.UtcNow;

        await _salesProfileRepository.UpdateAsync(existingProfile);

        return ServiceResult<bool>.CreateSuccess(true);
    }

    /// <summary>
    /// 取得銷售統計資訊
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>銷售統計</returns>
    public async Task<SalesStatisticsDto?> GetSalesStatisticsAsync(int userId)
    {
        var salesInfo = await _salesInfoRepository.GetByUserIdAsync(userId);
        if (salesInfo == null) return null;

        var salesStatistics = new SalesStatisticsDto
        {
            UserId = salesInfo.User_Id,
            TotalOrders = salesInfo.Total_Orders,
            TotalRevenue = salesInfo.Total_Revenue,
            AverageOrderValue = salesInfo.Total_Orders > 0 ? salesInfo.Total_Revenue / salesInfo.Total_Orders : 0,
            LastOrderDate = salesInfo.Last_Order_Date,
            CustomerCount = salesInfo.Customer_Count
        };

        return salesStatistics;
    }

    /// <summary>
    /// 記錄銷售交易
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="orderAmount">訂單金額</param>
    /// <param name="customerId">客戶ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> RecordSalesTransactionAsync(int userId, decimal orderAmount, int customerId)
    {
        try
        {
            var salesInfo = await _salesInfoRepository.GetByUserIdAsync(userId);
            if (salesInfo == null)
            {
                return ServiceResult<bool>.CreateFailure("銷售資料不存在");
            }

            // 更新銷售統計
            salesInfo.Total_Orders++;
            salesInfo.Total_Revenue += orderAmount;
            salesInfo.Last_Order_Date = DateTime.UtcNow;
            salesInfo.Updated_At = DateTime.UtcNow;

            await _salesInfoRepository.UpdateAsync(salesInfo);

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception)
        {
            return ServiceResult<bool>.CreateFailure("記錄銷售交易失敗");
        }
    }

    /// <summary>
    /// 取得銷售排行榜
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>銷售排行榜</returns>
    public async Task<List<SalesRankingDto>> GetSalesRankingAsync(int top = 10)
    {
        try
        {
            var topSalesUsers = await _salesInfoRepository.GetTopSalesUsersAsync(top);
            var rankings = new List<SalesRankingDto>();

            int rank = 1;
            foreach (var salesInfo in topSalesUsers)
            {
                var ranking = new SalesRankingDto
                {
                    Rank = rank++,
                    UserId = salesInfo.User_Id,
                    TotalSales = salesInfo.Total_Revenue,
                    TotalOrders = salesInfo.Total_Orders,
                    AverageOrderValue = salesInfo.Total_Orders > 0 ? salesInfo.Total_Revenue / salesInfo.Total_Orders : 0
                };

                rankings.Add(ranking);
            }

            return rankings;
        }
        catch (Exception)
        {
            return new List<SalesRankingDto>();
        }
    }

    /// <summary>
    /// 計算佣金
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="orderAmount">訂單金額</param>
    /// <returns>佣金金額</returns>
    public async Task<decimal> CalculateCommissionAsync(int userId, decimal orderAmount)
    {
        var profile = await _salesProfileRepository.GetByUserIdAsync(userId);
        if (profile == null || !profile.Is_Active)
        {
            return 0; // 沒有銷售資料或未啟用，佣金為0
        }

        // 使用使用者的佣金率計算
        return orderAmount * (profile.Commission_Rate / 100);
    }
} 