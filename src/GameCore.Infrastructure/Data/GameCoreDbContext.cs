using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// GameCore 資料庫上下文 - 管理所有實體與資料庫連線
/// </summary>
public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    #region DbSet 屬性 - 使用者相關表

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

    /// <summary>
    /// 開通銷售功能表
    /// </summary>
    public DbSet<MemberSalesProfile> MemberSalesProfiles { get; set; }

    /// <summary>
    /// 使用者銷售資訊表
    /// </summary>
    public DbSet<UserSalesInformation> UserSalesInformation { get; set; }

    /// <summary>
    /// 寵物表
    /// </summary>
    public DbSet<Pet> Pets { get; set; }

    /// <summary>
    /// 簽到記錄表
    /// </summary>
    public DbSet<UserSignInStats> UserSignInStats { get; set; }

    /// <summary>
    /// 小遊戲記錄表
    /// </summary>
    public DbSet<MiniGame> MiniGames { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureUserEntities(modelBuilder);
    }

    /// <summary>
    /// 配置使用者相關實體
    /// </summary>
    private static void ConfigureUserEntities(ModelBuilder modelBuilder)
    {
        // 配置 Users 實體
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.User_ID);
            entity.Property(e => e.User_ID).ValueGeneratedOnAdd();
            
            entity.Property(e => e.User_name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.User_Account)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.User_Password)
                .IsRequired()
                .HasMaxLength(255);
            
            // 唯一索引
            entity.HasIndex(e => e.User_name).IsUnique();
            entity.HasIndex(e => e.User_Account).IsUnique();
        });

        // 配置 UserIntroduce 實體
        modelBuilder.Entity<UserIntroduce>(entity =>
        {
            entity.HasKey(e => e.User_ID);
            
            entity.Property(e => e.User_NickName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(1);
            
            entity.Property(e => e.IdNumber)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.Cellphone)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");
            
            entity.Property(e => e.Create_Account)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            
            entity.Property(e => e.User_Picture)
                .HasColumnType("varbinary(max)");
            
            entity.Property(e => e.User_Introduce)
                .HasMaxLength(200);
            
            // 唯一索引
            entity.HasIndex(e => e.User_NickName).IsUnique();
            entity.HasIndex(e => e.IdNumber).IsUnique();
            entity.HasIndex(e => e.Cellphone).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserIntroduce)
                  .HasForeignKey<UserIntroduce>(e => e.User_ID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 UserRights 實體
        modelBuilder.Entity<UserRights>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            
            entity.Property(e => e.User_Status)
                .HasDefaultValue(true);
            
            entity.Property(e => e.ShoppingPermission)
                .HasDefaultValue(true);
            
            entity.Property(e => e.MessagePermission)
                .HasDefaultValue(true);
            
            entity.Property(e => e.SalesAuthority)
                .HasDefaultValue(false);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserRights)
                  .HasForeignKey<UserRights>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 UserWallet 實體
        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            
            entity.Property(e => e.User_Point)
                .HasDefaultValue(0);
            
            entity.Property(e => e.Coupon_Number)
                .HasMaxLength(50);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserWallet)
                  .HasForeignKey<UserWallet>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 MemberSalesProfile 實體
        modelBuilder.Entity<MemberSalesProfile>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            
            entity.Property(e => e.BankAccountNumber)
                .HasMaxLength(30);
            
            entity.Property(e => e.AccountCoverPhoto)
                .HasColumnType("varbinary(max)");
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.MemberSalesProfile)
                  .HasForeignKey<MemberSalesProfile>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 UserSalesInformation 實體
        modelBuilder.Entity<UserSalesInformation>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            
            entity.Property(e => e.UserSales_Wallet)
                .HasDefaultValue(0);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserSalesInformation)
                  .HasForeignKey<UserSalesInformation>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 Pet 實體 (基本配置，將在後續階段完整實現)
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetID);
            entity.Property(e => e.PetID).ValueGeneratedOnAdd();
            
            entity.Property(e => e.PetName)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("小可愛");
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.Pet)
                  .HasForeignKey<Pet>(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 UserSignInStats 實體 (基本配置，將在後續階段完整實現)
        modelBuilder.Entity<UserSignInStats>(entity =>
        {
            entity.HasKey(e => e.LogID);
            entity.Property(e => e.LogID).ValueGeneratedOnAdd();
            
            entity.Property(e => e.SignTime)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany(e => e.SignInStats)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置 MiniGame 實體 (基本配置，將在後續階段完整實現)
        modelBuilder.Entity<MiniGame>(entity =>
        {
            entity.HasKey(e => e.PlayID);
            entity.Property(e => e.PlayID).ValueGeneratedOnAdd();
            
            entity.Property(e => e.StartTime)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany(e => e.MiniGames)
                  .HasForeignKey(e => e.UserID)
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
