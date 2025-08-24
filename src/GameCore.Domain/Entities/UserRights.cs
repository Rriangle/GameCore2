using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者權限資料表 - 控制用戶各項功能權限
/// 對應資料庫表格: User_Rights
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
    /// 使用者狀態 (是否啟用帳號)
    /// true = 啟用, false = 停權
    /// </summary>
    public bool User_Status { get; set; } = true;

    /// <summary>
    /// 購物權限 (是否可在官方商城/自由市場下單)
    /// true = 可購物, false = 禁止購物
    /// </summary>
    public bool ShoppingPermission { get; set; } = true;

    /// <summary>
    /// 留言權限 (是否可發表/回覆論壇、聊天室訊息)
    /// true = 可留言, false = 禁言
    /// </summary>
    public bool MessagePermission { get; set; } = true;

    /// <summary>
    /// 銷售權限 (是否可在自由市場上架商品)
    /// true = 可銷售, false = 禁止銷售
    /// </summary>
    public bool SalesAuthority { get; set; } = false; // 預設不開放銷售，需申請

    // 導航屬性
    /// <summary>
    /// 關聯到使用者主檔
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}