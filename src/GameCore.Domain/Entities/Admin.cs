using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 後台管理員表
/// 對應資料庫 Admins 表
/// </summary>
[Table("Admins")]
public class Admin
{
    /// <summary>
    /// 管理員ID (主鍵)
    /// </summary>
    [Key]
    public int ManagerID { get; set; }

    /// <summary>
    /// 上次登入時間，用於後台登入追蹤
    /// </summary>
    public DateTime? LastLogin { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的管理者資料
    /// </summary>
    public virtual ManagerData Manager { get; set; } = null!;
} 