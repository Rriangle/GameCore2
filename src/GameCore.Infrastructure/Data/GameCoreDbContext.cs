using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// GameCore 資料庫上下文 - 管理所有實體與資料庫的對應關係
/// 嚴格按照規格文件的資料表結構設計，不修改任何欄位名稱或資料型別
/// </summary>
public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    // DbSet 屬性 - 使用者相關實體
    /// <summary>
    /// 使用者基本資料表
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 使用者詳細介紹資料表
    /// </summary>
    public DbSet<UserIntroduce> UserIntroduces { get; set; }

    /// <summary>
    /// 使用者權限資料表
    /// </summary>
    public DbSet<UserRights> UserRights { get; set; }

    /// <summary>
    /// 使用者錢包資料表
    /// </summary>
    public DbSet<UserWallet> UserWallets { get; set; }

    /// <summary>
    /// 會員銷售資料表
    /// </summary>
    public DbSet<MemberSalesProfile> MemberSalesProfiles { get; set; }

    /// <summary>
    /// 使用者銷售資訊資料表
    /// </summary>
    public DbSet<UserSalesInformation> UserSalesInformations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 使用者基本資料表配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserAccount).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserPassword).IsRequired().HasMaxLength(255);
            
            // 唯一索引 - 按照規格要求
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.UserAccount).IsUnique();
        });

        // 使用者介紹資料表配置
        modelBuilder.Entity<UserIntroduce>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserNickName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Gender).IsRequired().HasColumnType("CHAR(1)");
            entity.Property(e => e.IdNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Cellphone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DateOfBirth).IsRequired().HasColumnType("DATE");
            entity.Property(e => e.CreateAccount).IsRequired().HasColumnType("DATETIME2");
            entity.Property(e => e.UserIntroduceText).HasMaxLength(200);
            
            // 唯一索引 - 按照規格要求
            entity.HasIndex(e => e.UserNickName).IsUnique();
            entity.HasIndex(e => e.IdNumber).IsUnique();
            entity.HasIndex(e => e.Cellphone).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            // 一對一關係：使用者 -> 使用者介紹
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserIntroduce)
                  .HasForeignKey<UserIntroduce>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 使用者權限資料表配置
        modelBuilder.Entity<UserRights>(entity =>
        {
            entity.HasKey(e => e.UserId);
            
            // 一對一關係：使用者 -> 使用者權限
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserRights)
                  .HasForeignKey<UserRights>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 使用者錢包資料表配置
        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.CouponNumber).HasMaxLength(50);
            
            // 一對一關係：使用者 -> 使用者錢包
            entity.HasOne(e => e.User)
                  .WithOne(e => e.Wallet)
                  .HasForeignKey<UserWallet>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 會員銷售資料表配置
        modelBuilder.Entity<MemberSalesProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.BankAccountNumber).HasMaxLength(50);
            
            // 一對一關係：使用者 -> 會員銷售資料
            entity.HasOne(e => e.User)
                  .WithOne(e => e.MemberSalesProfile)
                  .HasForeignKey<MemberSalesProfile>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 使用者銷售資訊資料表配置
        modelBuilder.Entity<UserSalesInformation>(entity =>
        {
            entity.HasKey(e => e.UserId);
            
            // 一對一關係：使用者 -> 使用者銷售資訊
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserSalesInformation)
                  .HasForeignKey<UserSalesInformation>(e => e.UserId)
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
