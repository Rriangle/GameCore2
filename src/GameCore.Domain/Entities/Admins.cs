using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 後台管理員表
/// </summary>
[Table("Admins")]
public class Admins
{
    /// <summary>
    /// 管理員ID (主鍵，外鍵到 ManagerRole)
    /// </summary>
    [Key]
    public int manager_id { get; set; }

    /// <summary>
    /// 上次登入時間，用於後台登入追蹤
    /// </summary>
    public DateTime? last_login { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    [ForeignKey("manager_id")]
    public virtual ManagerData Manager { get; set; } = null!;
}