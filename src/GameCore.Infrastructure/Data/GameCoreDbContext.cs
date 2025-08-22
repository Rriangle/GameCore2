using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    // DbSet 屬性
    public DbSet<User> Users { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 用戶實體配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.AvatarUrl).HasMaxLength(200);
            
            // 唯一索引
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // 用戶錢包實體配置
        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.WalletId);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(100.00m);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.Wallet)
                  .HasForeignKey<UserWallet>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=GameCore;Trusted_Connection=true;MultipleActiveResultSets=true");
        }
    }
}
