using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 寵物狀態表 - 虛擬寵物(史萊姆)系統 (將在後續階段完整實現)
/// </summary>
[Table("Pet")]
public class Pet
{
    /// <summary>
    /// 寵物編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("PetID")]
    public int PetID { get; set; }

    /// <summary>
    /// 寵物主人會員編號 (外鍵參考 Users.User_ID)
    /// </summary>
    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    /// <summary>
    /// 寵物名稱
    /// </summary>
    [Required]
    [Column("PetName")]
    [StringLength(50)]
    public string PetName { get; set; } = "小可愛";

    // 導航屬性
    /// <summary>
    /// 寵物主人
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}