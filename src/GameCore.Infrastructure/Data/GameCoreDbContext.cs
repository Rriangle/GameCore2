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

    #region DbSet 屬性 - 官方商城相關表

    /// <summary>
    /// 供應商表
    /// </summary>
    public DbSet<Supplier> Suppliers { get; set; }

    /// <summary>
    /// 商品資訊表
    /// </summary>
    public DbSet<ProductInfo> ProductInfos { get; set; }

    /// <summary>
    /// 遊戲商品詳細資訊表
    /// </summary>
    public DbSet<GameProductDetails> GameProductDetails { get; set; }

    /// <summary>
    /// 其他商品詳細資訊表
    /// </summary>
    public DbSet<OtherProductDetails> OtherProductDetails { get; set; }

    /// <summary>
    /// 訂單資訊表
    /// </summary>
    public DbSet<OrderInfo> OrderInfos { get; set; }

    /// <summary>
    /// 訂單明細表
    /// </summary>
    public DbSet<OrderItems> OrderItems { get; set; }

    /// <summary>
    /// 官方商城排行榜表
    /// </summary>
    public DbSet<OfficialStoreRanking> OfficialStoreRankings { get; set; }

    /// <summary>
    /// 商品資訊異動記錄表
    /// </summary>
    public DbSet<ProductInfoAuditLog> ProductInfoAuditLogs { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureUserEntities(modelBuilder);
        ConfigureStoreEntities(modelBuilder);
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

    /// <summary>
    /// 配置官方商城相關實體
    /// </summary>
    private static void ConfigureStoreEntities(ModelBuilder modelBuilder)
    {
        // 配置 Supplier 實體
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.SupplierId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.SupplierName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(e => e.SupplierName).IsUnique();
        });

        // 配置 ProductInfo 實體
        modelBuilder.Entity<ProductInfo>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.ProductType)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");
            
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10);
            
            entity.Property(e => e.ProductCreatedAt)
                .HasDefaultValueSql("GETDATE()");
            
            entity.Property(e => e.ProductUpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 關聯到 User (建立者)
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 配置 GameProductDetails 實體
        modelBuilder.Entity<GameProductDetails>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(2000);
            
            entity.Property(e => e.GameName)
                .HasMaxLength(200);
            
            entity.Property(e => e.DownloadLink)
                .HasMaxLength(500);

            // 一對一關聯到 ProductInfo
            entity.HasOne(e => e.ProductInfo)
                .WithOne(p => p.GameProductDetails)
                .HasForeignKey<GameProductDetails>(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 關聯到 Supplier
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.GameProducts)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 配置 OtherProductDetails 實體
        modelBuilder.Entity<OtherProductDetails>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(2000);
            
            entity.Property(e => e.DigitalCode)
                .HasMaxLength(100);
            
            entity.Property(e => e.Size)
                .HasMaxLength(50);
            
            entity.Property(e => e.Color)
                .HasMaxLength(50);
            
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(10,2)");
            
            entity.Property(e => e.Dimensions)
                .HasMaxLength(100);
            
            entity.Property(e => e.Material)
                .HasMaxLength(100);

            // 一對一關聯到 ProductInfo
            entity.HasOne(e => e.ProductInfo)
                .WithOne(p => p.OtherProductDetails)
                .HasForeignKey<OtherProductDetails>(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 關聯到 Supplier
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.OtherProducts)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 配置 OrderInfo 實體
        modelBuilder.Entity<OrderInfo>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("GETDATE()");
            
            entity.Property(e => e.OrderStatus)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.PaymentStatus)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.OrderTotal)
                .HasColumnType("decimal(18,2)");

            // 關聯到 User
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 配置 OrderItems 實體
        modelBuilder.Entity<OrderItems>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.ItemId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18,2)");
            
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18,2)");

            // 關聯到 OrderInfo
            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 關聯到 ProductInfo
            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 配置 OfficialStoreRanking 實體
        modelBuilder.Entity<OfficialStoreRanking>(entity =>
        {
            entity.HasKey(e => e.RankingId);
            entity.Property(e => e.RankingId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.PeriodType)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.RankingMetric)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.TradingAmount)
                .HasColumnType("decimal(18,2)");
            
            entity.Property(e => e.RankingUpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // 關聯到 ProductInfo
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Rankings)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // 複合索引
            entity.HasIndex(e => new { e.PeriodType, e.RankingDate, e.ProductId })
                .IsUnique();
        });

        // 配置 ProductInfoAuditLog 實體
        modelBuilder.Entity<ProductInfoAuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId);
            entity.Property(e => e.LogId).ValueGeneratedOnAdd();
            
            entity.Property(e => e.ActionType)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.FieldName)
                .HasMaxLength(100);
            
            entity.Property(e => e.OldValue)
                .HasMaxLength(1000);
            
            entity.Property(e => e.NewValue)
                .HasMaxLength(1000);
            
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("GETDATE()");

            // 關聯到 ProductInfo
            entity.HasOne(e => e.Product)
                .WithMany(p => p.AuditLogs)
                .HasForeignKey(e => e.ProductId)
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
