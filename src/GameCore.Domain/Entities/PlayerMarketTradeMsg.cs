using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場交易頁面對話表
/// 對應資料庫 PlayerMarketTradeMsg 表
/// </summary>
[Table("PlayerMarketTradeMsg")]
public class PlayerMarketTradeMsg
{
    /// <summary>
    /// 交易中雙方訊息ID (主鍵)
    /// </summary>
    [Key]
    public int TradeMsgID { get; set; }

    /// <summary>
    /// 交易頁面ID
    /// </summary>
    public int POrderTradepageID { get; set; }

    /// <summary>
    /// 誰傳的訊息 (seller/buyer)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string MsgFrom { get; set; } = string.Empty;

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// 傳訊時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬交易頁面
    /// </summary>
    public virtual PlayerMarketOrderTradepage TradePage { get; set; } = null!;
} 