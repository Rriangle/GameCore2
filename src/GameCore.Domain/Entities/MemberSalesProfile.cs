using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 開通銷售功能表 - 銀行帳戶資料
/// </summary>
[Table("MemberSalesProfile")]
public class MemberSalesProfile
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int User_Id { get; set; }

    /// <summary>
    /// 銀行代號
    /// </summary>
    [Column("BankCode")]
    public int? BankCode { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    [Column("BankAccountNumber")]
    [StringLength(30)]
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// 帳戶封面照片
    /// </summary>
    [Column("AccountCoverPhoto")]
    public byte[]? AccountCoverPhoto { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬使用者
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}