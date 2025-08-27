using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 開通銷售功能表
/// </summary>
[Table("MemberSalesProfile")]
public class MemberSalesProfile
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

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
    [Column(TypeName = "varbinary(max)")]
    public byte[]? AccountCoverPhoto { get; set; }

    /// <summary>
    /// 申請狀態 (Pending/Approved/Rejected)
    /// </summary>
    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 申請時間
    /// </summary>
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 審核時間
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// 審核者 ID (管理員)
    /// </summary>
    public int? ReviewedBy { get; set; }

    /// <summary>
    /// 審核備註
    /// </summary>
    [StringLength(500)]
    public string? ReviewNotes { get; set; }

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}