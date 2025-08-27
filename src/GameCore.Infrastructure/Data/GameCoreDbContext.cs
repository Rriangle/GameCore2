using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// GameCore 資料庫內容類別
/// 負責與 SQL Server 資料庫互動，定義所有實體和關聯關係
/// </summary>
public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    // DbSet 屬性 - 對應資料庫表格
    /// <summary>
    /// 使用者基本資料表
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 使用者詳細介紹表
    /// </summary>
    public DbSet<UserIntroduce> UserIntroduces { get; set; }

    /// <summary>
    /// 使用者權限表
    /// </summary>
    public DbSet<UserRights> UserRights { get; set; }

    /// <summary>
    /// 使用者錢包表
    /// </summary>
    public DbSet<UserWallet> UserWallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Users 實體配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.User_ID);
            entity.Property(e => e.User_name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.User_Account).IsRequired().HasMaxLength(50);
            entity.Property(e => e.User_Password).IsRequired().HasMaxLength(255);
            
            // 唯一索引
            entity.HasIndex(e => e.User_name).IsUnique();
            entity.HasIndex(e => e.User_Account).IsUnique();
        });

        // UserIntroduce 實體配置
        modelBuilder.Entity<UserIntroduce>(entity =>
        {
            entity.HasKey(e => e.User_ID);
            entity.Property(e => e.User_NickName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Gender).IsRequired().HasMaxLength(1);
            entity.Property(e => e.IdNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Cellphone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.Create_Account).HasColumnType("datetime2");
            entity.Property(e => e.User_Picture).HasColumnType("varbinary(max)");
            entity.Property(e => e.User_Introduce).HasMaxLength(200);
            
            // 唯一索引
            entity.HasIndex(e => e.User_NickName).IsUnique();
            entity.HasIndex(e => e.IdNumber).IsUnique();
            entity.HasIndex(e => e.Cellphone).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            // 一對一關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserIntroduce)
                  .HasForeignKey<UserIntroduce>(e => e.User_ID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserRights 實體配置  
        modelBuilder.Entity<UserRights>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.User_Status).HasDefaultValue(true);
            entity.Property(e => e.ShoppingPermission).HasDefaultValue(true);
            entity.Property(e => e.MessagePermission).HasDefaultValue(true);
            entity.Property(e => e.SalesAuthority).HasDefaultValue(false);
            
            // 一對一關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserRights)
                  .HasForeignKey<UserRights>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserWallet 實體配置
        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.User_Point).HasDefaultValue(0);
            entity.Property(e => e.Coupon_Number).HasMaxLength(50);
            
            // 一對一關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.Wallet)
                  .HasForeignKey<UserWallet>(e => e.User_Id)
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
