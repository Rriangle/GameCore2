using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者權限表
/// </summary>
[Table("User_Rights")]
public class UserRights
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者狀態 (是否啟用/停權)
    /// </summary>
    public bool User_Status { get; set; } = true;

    /// <summary>
    /// 購物權限 (是否可於官方商城/自由市場下單)
    /// </summary>
    public bool ShoppingPermission { get; set; } = true;

    /// <summary>
    /// 留言權限 (是否可發表/回覆論壇、聊天室訊息)
    /// </summary>
    public bool MessagePermission { get; set; } = true;

    /// <summary>
    /// 銷售權限 (是否可在自由市場上架)
    /// </summary>
    public bool SalesAuthority { get; set; } = false;

    /// <summary>
    /// 建立時間
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