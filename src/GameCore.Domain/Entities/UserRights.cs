using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者權限表 - 管理各種功能權限
/// </summary>
[Table("User_Rights")]
public class UserRights
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者狀態 (是否啟用/停權)
    /// </summary>
    [Column("User_Status")]
    public bool User_Status { get; set; } = true;

    /// <summary>
    /// 購物權限 (是否可於官方商城/自由市場下單)
    /// </summary>
    [Column("ShoppingPermission")]
    public bool ShoppingPermission { get; set; } = true;

    /// <summary>
    /// 留言權限 (是否可發表/回覆論壇、聊天室訊息)
    /// </summary>
    [Column("MessagePermission")]
    public bool MessagePermission { get; set; } = true;

    /// <summary>
    /// 銷售權限 (是否可在自由市場上架)
    /// </summary>
    [Column("SalesAuthority")]
    public bool SalesAuthority { get; set; } = false;

    // 導航屬性
    /// <summary>
    /// 所屬使用者
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}