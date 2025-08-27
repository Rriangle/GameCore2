using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 管理者資料實體 - 後台管理員主表
/// 對應資料表: ManagerData
/// </summary>
[Table("ManagerData")]
public class ManagerData
{
    /// <summary>
    /// 管理者編號 (主鍵)
    /// </summary>
    [Key]
    [Column("Manager_Id")]
    public int ManagerId { get; set; }

    /// <summary>
    /// 管理者姓名
    /// </summary>
    [Required]
    [Column("Manager_Name")]
    [StringLength(100)]
    public string ManagerName { get; set; } = string.Empty;

    /// <summary>
    /// 管理者帳號（建議唯一）
    /// </summary>
    [Required]
    [Column("Manager_Account")]
    [StringLength(50)]
    public string ManagerAccount { get; set; } = string.Empty;

    /// <summary>
    /// 管理者密碼（實務請存雜湊）
    /// </summary>
    [Required]
    [Column("Manager_Password")]
    [StringLength(500)]
    public string ManagerPassword { get; set; } = string.Empty;

    /// <summary>
    /// 管理者註冊時間
    /// </summary>
    [Required]
    [Column("Administrator_registration_date")]
    public DateTime AdministratorRegistrationDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 管理者狀態 (active/inactive/suspended)
    /// </summary>
    [Column("Status")]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    /// <summary>
    /// 管理者等級 (super_admin/admin/moderator)
    /// </summary>
    [Column("Level")]
    [StringLength(20)]
    public string Level { get; set; } = "admin";

    /// <summary>
    /// 電子信箱
    /// </summary>
    [Column("Email")]
    [StringLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// 手機號碼
    /// </summary>
    [Column("Phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    [Column("Department")]
    [StringLength(50)]
    public string? Department { get; set; }

    /// <summary>
    /// 職位
    /// </summary>
    [Column("Position")]
    [StringLength(50)]
    public string? Position { get; set; }

    /// <summary>
    /// 建立者編號
    /// </summary>
    [Column("CreatedBy")]
    public int? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 是否為系統管理員
    /// </summary>
    [Column("IsSuperAdmin")]
    public bool IsSuperAdmin { get; set; } = false;

    /// <summary>
    /// 密碼最後更新時間
    /// </summary>
    [Column("PasswordLastChanged")]
    public DateTime? PasswordLastChanged { get; set; }

    /// <summary>
    /// 是否首次登入
    /// </summary>
    [Column("IsFirstLogin")]
    public bool IsFirstLogin { get; set; } = true;

    /// <summary>
    /// 導航屬性 - 管理者角色關聯
    /// </summary>
    public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();

    /// <summary>
    /// 導航屬性 - 登入追蹤記錄
    /// </summary>
    public virtual Admin? Admin { get; set; }

    /// <summary>
    /// 導航屬性 - 建立的禁言項目
    /// </summary>
    public virtual ICollection<Mute> CreatedMutes { get; set; } = new List<Mute>();

    /// <summary>
    /// 導航屬性 - 建立的樣式項目
    /// </summary>
    public virtual ICollection<Style> CreatedStyles { get; set; } = new List<Style>();
}