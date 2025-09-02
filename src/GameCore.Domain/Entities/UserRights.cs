using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者權限表
/// 對應資料庫 User_Rights 表
/// </summary>
[Table("User_Rights")]
public class UserRights
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者狀態 (是否啟用)
    /// </summary>
    public bool User_Status { get; set; } = true;

    /// <summary>
    /// 購物權限 (是否可在官方商城/自由市場下單)
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

    // 導航屬性
    /// <summary>
    /// 關聯的使用者資料
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}
