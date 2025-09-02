using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 開通銷售功能表
/// 對應資料庫 MemberSalesProfile 表
/// </summary>
[Table("MemberSalesProfile")]
public class MemberSalesProfile
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 銷售等級
    /// </summary>
    public int Sales_Level { get; set; } = 1;

    /// <summary>
    /// 總銷售額
    /// </summary>
    public decimal Total_Sales { get; set; } = 0;

    /// <summary>
    /// 佣金率
    /// </summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal Commission_Rate { get; set; } = 0.05m;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool Is_Active { get; set; } = true;

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime Last_Updated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 銀行代號
    /// </summary>
    public int BankCode { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    [StringLength(50)]
    public string BankAccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶封面照片 (二進位資料)
    /// </summary>
    public byte[]? AccountCoverPhoto { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的使用者資料
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
} 