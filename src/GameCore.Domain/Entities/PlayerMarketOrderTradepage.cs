using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 交易中頁面表
/// 對應資料庫 PlayerMarketOrderTradepage 表
/// </summary>
[Table("PlayerMarketOrderTradepage")]
public class PlayerMarketOrderTradepage
{
    /// <summary>
    /// 流水號 交易頁面ID (主鍵)
    /// </summary>
    [Key]
    public int POrderTradepageID { get; set; }

    /// <summary>
    /// 自由市場訂單ID
    /// </summary>
    public int POrderID { get; set; }

    /// <summary>
    /// 指向自由市場商品
    /// </summary>
    public int PProductID { get; set; }

    /// <summary>
    /// 平台抽成
    /// </summary>
    public int POrderPlatformFee { get; set; }

    /// <summary>
    /// 賣家移交時間
    /// </summary>
    public DateTime? SellerTransferredAt { get; set; }

    /// <summary>
    /// 買家接收時間
    /// </summary>
    public DateTime? BuyerReceivedAt { get; set; }

    /// <summary>
    /// 交易完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬訂單
    /// </summary>
    public virtual PlayerMarketOrderInfo Order { get; set; } = null!;

    /// <summary>
    /// 商品
    /// </summary>
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;

    /// <summary>
    /// 交易訊息
    /// </summary>
    public virtual ICollection<PlayerMarketTradeMsg> TradeMessages { get; set; } = new List<PlayerMarketTradeMsg>();
} 