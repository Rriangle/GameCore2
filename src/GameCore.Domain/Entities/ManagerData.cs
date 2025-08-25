using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 管理者資料表（主表）
/// </summary>
[Table("ManagerData")]
public class ManagerData
{
    /// <summary>
    /// 管理者編號 (主鍵)
    /// </summary>
    [Key]
    public int Manager_Id { get; set; }

    /// <summary>
    /// 管理者姓名
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Manager_Name { get; set; } = string.Empty;

    /// <summary>
    /// 管理者帳號 (建議唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Manager_Account { get; set; } = string.Empty;

    /// <summary>
    /// 管理者密碼 (實務請存雜湊)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Manager_Password { get; set; } = string.Empty;

    /// <summary>
    /// 管理者註冊時間
    /// </summary>
    public DateTime Administrator_registration_date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // 導航屬性
    public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
}