using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Pet")]
public class Pet
{
    [Key]
    public int PetId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = "小可愛";

    [Required]
    public int Level { get; set; } = 1;

    [Required]
    public int Exp { get; set; } = 0;

    [Required]
    public int Hunger { get; set; } = 100;

    [Required]
    public int Mood { get; set; } = 100;

    [Required]
    public int Stamina { get; set; } = 100;

    [Required]
    public int Cleanliness { get; set; } = 100;

    [Required]
    public int Health { get; set; } = 100;

    [Required]
    [StringLength(20)]
    public string SkinColor { get; set; } = "default";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastDecayAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    // Business logic methods
    public void ClampAttributes()
    {
        Hunger = Math.Max(0, Math.Min(100, Hunger));
        Mood = Math.Max(0, Math.Min(100, Mood));
        Stamina = Math.Max(0, Math.Min(100, Stamina));
        Cleanliness = Math.Max(0, Math.Min(100, Cleanliness));
        Health = Math.Max(0, Math.Min(100, Health));
    }

    public void UpdateHealth()
    {
        if (Hunger == 100 && Mood == 100 && Stamina == 100 && Cleanliness == 100)
        {
            Health = 100;
        }
        else
        {
            if (Hunger < 30) Health = Math.Max(0, Health - 20);
            if (Cleanliness < 30) Health = Math.Max(0, Health - 20);
            if (Stamina < 30) Health = Math.Max(0, Health - 20);
        }
    }

    public bool CanPlayMiniGame()
    {
        return Health > 0 && Hunger > 0 && Mood > 0 && Stamina > 0 && Cleanliness > 0;
    }

    public int GetRequiredExpForNextLevel()
    {
        if (Level <= 10)
        {
            return 40 * Level + 60;
        }
        else if (Level <= 100)
        {
            return (int)(0.8 * Math.Pow(Level, 2) + 380);
        }
        else
        {
            return (int)(285.69 * Math.Pow(1.06, Level));
        }
    }

    public bool TryLevelUp()
    {
        if (Level >= 250) return false;
        
        var requiredExp = GetRequiredExpForNextLevel();
        if (Exp >= requiredExp)
        {
            Exp -= requiredExp;
            Level++;
            return true;
        }
        return false;
    }
}