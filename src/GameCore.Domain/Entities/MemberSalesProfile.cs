using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 開通銷售功能資料表 - 儲存會員銷售相關的銀行帳戶資訊
/// 對應資料庫表：MemberSalesProfile
/// </summary>
[Table("MemberSalesProfile")]
public class MemberSalesProfile
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int UserId { get; set; }

    /// <summary>
    /// 銀行代號 (INT 型別)
    /// 儲存銀行的數字代碼
    /// </summary>
    public int? BankCode { get; set; }

    /// <summary>
    /// 銀行帳號 (VARCHAR 型別)
    /// 儲存銀行帳戶號碼
    /// </summary>
    [StringLength(50)]
    public string? BankAccountNumber { get; set; }

    /// <summary>
    /// 帳戶封面照片 (VARBINARY(MAX) 型別)
    /// 儲存帳戶相關的證明文件圖片
    /// </summary>
    public byte[]? AccountCoverPhoto { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的使用者基本資料
    /// </summary>
    public virtual User User { get; set; } = null!;
}