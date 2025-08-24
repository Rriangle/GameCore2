using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者權限資料表 - 儲存使用者各項功能權限設定
/// 對應資料庫表：User_Rights
/// </summary>
[Table("User_Rights")]
public class UserRights
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int UserId { get; set; }

    /// <summary>
    /// 使用者狀態 (BIT 型別)
    /// true = 啟用, false = 停用
    /// </summary>
    [Column("User_Status")]
    public bool UserStatus { get; set; } = true;

    /// <summary>
    /// 購物權限 (BIT 型別)
    /// true = 可購物, false = 禁止購物
    /// </summary>
    public bool ShoppingPermission { get; set; } = true;

    /// <summary>
    /// 留言權限 (BIT 型別)
    /// true = 可留言, false = 禁止留言
    /// </summary>
    public bool MessagePermission { get; set; } = true;

    /// <summary>
    /// 銷售權限 (BIT 型別)
    /// true = 可銷售, false = 禁止銷售
    /// 需要通過銷售功能申請才能開啟
    /// </summary>
    public bool SalesAuthority { get; set; } = false;

    // 導航屬性
    /// <summary>
    /// 關聯的使用者基本資料
    /// </summary>
    public virtual User User { get; set; } = null!;
}