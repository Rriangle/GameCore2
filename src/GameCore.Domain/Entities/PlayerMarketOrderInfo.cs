using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場訂單表
/// 對應資料庫 PlayerMarketOrderInfo 表
/// </summary>
[Table("PlayerMarketOrderInfo")]
public class PlayerMarketOrderInfo
{
    /// <summary>
    /// 自由市場訂單ID (主鍵)
    /// </summary>
    [Key]
    public int POrderID { get; set; }

    /// <summary>
    /// 指向自由市場商品
    /// </summary>
    public int PProductID { get; set; }

    /// <summary>
    /// 賣家ID
    /// </summary>
    public int SellerID { get; set; }

    /// <summary>
    /// 買家ID
    /// </summary>
    public int BuyerID { get; set; }

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime POrderDate { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    [Required]
    [StringLength(50)]
    public string POrderStatus { get; set; } = "Created";

    /// <summary>
    /// 付款狀態
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PPaymentStatus { get; set; } = "Pending";

    /// <summary>
    /// 單價
    /// </summary>
    public int PUnitPrice { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int PQuantity { get; set; }

    /// <summary>
    /// 總額
    /// </summary>
    public int POrderTotal { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime POrderCreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime POrderUpdatedAt { get; set; }

    // 別名屬性，用於向後兼容
    /// <summary>
    /// 訂單ID (別名)
    /// </summary>
    [NotMapped]
    public int OrderID => POrderID;

    /// <summary>
    /// 商品ID (別名)
    /// </summary>
    [NotMapped]
    public int Product_ID => PProductID;

    /// <summary>
    /// 買家用戶ID (別名)
    /// </summary>
    [NotMapped]
    public int Buyer_User_ID => BuyerID;

    /// <summary>
    /// 賣家用戶ID (別名)
    /// </summary>
    [NotMapped]
    public int Seller_User_ID => SellerID;

    /// <summary>
    /// 數量 (別名)
    /// </summary>
    [NotMapped]
    public int Quantity => PQuantity;

    /// <summary>
    /// 單價 (別名)
    /// </summary>
    [NotMapped]
    public int Unit_Price => PUnitPrice;

    /// <summary>
    /// 總金額 (別名)
    /// </summary>
    [NotMapped]
    public int Total_Amount => POrderTotal;

    /// <summary>
    /// 平台費用
    /// </summary>
    public int Platform_Fee { get; set; } = 0;

    /// <summary>
    /// 狀態 (別名)
    /// </summary>
    [NotMapped]
    public string Status => POrderStatus;

    /// <summary>
    /// 創建時間 (別名)
    /// </summary>
    [NotMapped]
    public DateTime Created_At => POrderCreatedAt;

    /// <summary>
    /// 更新時間 (別名)
    /// </summary>
    [NotMapped]
    public DateTime Updated_At => POrderUpdatedAt;

    // 導航屬性
    /// <summary>
    /// 商品
    /// </summary>
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;

    /// <summary>
    /// 賣家
    /// </summary>
    public virtual User Seller { get; set; } = null!;

    /// <summary>
    /// 買家
    /// </summary>
    public virtual User Buyer { get; set; } = null!;

    /// <summary>
    /// 交易頁面
    /// </summary>
    public virtual ICollection<PlayerMarketOrderTradepage> TradePages { get; set; } = new List<PlayerMarketOrderTradepage>();
} 