namespace GameCore.Shared.DTOs;

/// <summary>
/// 銷售資料 DTO
/// </summary>
public class SalesProfileDto
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 銷售等級
    /// </summary>
    public int SalesLevel { get; set; }

    /// <summary>
    /// 總銷售額
    /// </summary>
    public decimal TotalSales { get; set; }

    /// <summary>
    /// 佣金率
    /// </summary>
    public decimal CommissionRate { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// 更新銷售資料請求 DTO
/// </summary>
public class UpdateSalesProfileRequest
{
    /// <summary>
    /// 銷售等級
    /// </summary>
    public int? SalesLevel { get; set; }

    /// <summary>
    /// 佣金率
    /// </summary>
    public decimal? CommissionRate { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// 銷售統計 DTO
/// </summary>
public class SalesStatisticsDto
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 總訂單數
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// 總營收
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// 平均訂單金額
    /// </summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>
    /// 最後訂單日期
    /// </summary>
    public DateTime? LastOrderDate { get; set; }

    /// <summary>
    /// 客戶數量
    /// </summary>
    public int CustomerCount { get; set; }
}

/// <summary>
/// 銷售資訊 DTO
/// </summary>
public class SalesInformationDto
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 總訂單數
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// 總營收
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// 平均訂單金額
    /// </summary>
    public decimal AverageOrderValue { get; set; }

    /// <summary>
    /// 最後訂單日期
    /// </summary>
    public DateTime? LastOrderDate { get; set; }

    /// <summary>
    /// 客戶數量
    /// </summary>
    public int CustomerCount { get; set; }
}

/// <summary>
/// 銷售排行榜 DTO
/// </summary>
public class SalesRankingDto
{
    /// <summary>
    /// 排名
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 總銷售額
    /// </summary>
    public decimal TotalSales { get; set; }

    /// <summary>
    /// 總訂單數
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// 平均訂單金額
    /// </summary>
    public decimal AverageOrderValue { get; set; }
} 