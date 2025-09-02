using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// GameCore 資料庫上下文
/// 負責管理所有實體與資料庫的對應關係
/// </summary>
public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    // 使用者相關
    public DbSet<User> Users { get; set; }
    public DbSet<UserIntroduce> UserIntroduces { get; set; }
    public DbSet<UserRights> UserRights { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<UserWalletTransaction> UserWalletTransactions { get; set; }
    public DbSet<MemberSalesProfile> MemberSalesProfiles { get; set; }
    public DbSet<UserSalesInformation> UserSalesInformations { get; set; }

    // 簽到與寵物系統
    public DbSet<UserSignInStats> UserSignInStats { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<MiniGame> MiniGames { get; set; }

    // 論壇與互動
    public DbSet<Forum> Forums { get; set; }
    public DbSet<ForumThread> Threads { get; set; }
    public DbSet<ThreadPost> ThreadPosts { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }

    // 洞察與貼文
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostMetricSnapshot> PostMetricSnapshots { get; set; }
    public DbSet<PostSource> PostSources { get; set; }

    // 社交與通知
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSource> NotificationSources { get; set; }
    public DbSet<NotificationAction> NotificationActions { get; set; }
    public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupChat> GroupChats { get; set; }
    public DbSet<GroupBlock> GroupBlocks { get; set; }

    // 官方商城
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<ProductInfo> ProductInfos { get; set; }
    public DbSet<GameProductDetails> GameProductDetails { get; set; }
    public DbSet<OtherProductDetails> OtherProductDetails { get; set; }
    public DbSet<OrderInfo> OrderInfos { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OfficialStoreRanking> OfficialStoreRankings { get; set; }
    public DbSet<ProductInfoAuditLog> ProductInfoAuditLogs { get; set; }

    // 玩家市場
    public DbSet<PlayerMarketProductInfo> PlayerMarketProductInfos { get; set; }
    public DbSet<PlayerMarketProductImg> PlayerMarketProductImgs { get; set; }
    public DbSet<PlayerMarketOrderInfo> PlayerMarketOrderInfos { get; set; }
    public DbSet<PlayerMarketOrderTradepage> PlayerMarketOrderTradepages { get; set; }
    public DbSet<PlayerMarketTradeMsg> PlayerMarketTradeMsgs { get; set; }
    public DbSet<PlayerMarketRanking> PlayerMarketRankings { get; set; }

    // 熱度與排行榜
    public DbSet<Game> Games { get; set; }
    public DbSet<MetricSource> MetricSources { get; set; }
    public DbSet<Metric> Metrics { get; set; }
    public DbSet<GameSourceMap> GameSourceMaps { get; set; }
    public DbSet<GameMetricDaily> GameMetricDailies { get; set; }
    public DbSet<PopularityIndexDaily> PopularityIndexDailies { get; set; }
    public DbSet<LeaderboardSnapshot> LeaderboardSnapshots { get; set; }

    // 管理員系統
    public DbSet<ManagerData> ManagerDatas { get; set; }
    public DbSet<ManagerRolePermission> ManagerRolePermissions { get; set; }
    public DbSet<ManagerRole> ManagerRoles { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Mute> Mutes { get; set; }
    public DbSet<Style> Styles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 使用者實體配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.User_ID);
            entity.Property(e => e.User_Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.User_Account).IsRequired().HasMaxLength(100);
            entity.Property(e => e.User_Password).IsRequired().HasMaxLength(255);
            
            // 唯一索引
            entity.HasIndex(e => e.User_Name).IsUnique();
            entity.HasIndex(e => e.User_Account).IsUnique();
        });

        // 使用者介紹實體配置
        modelBuilder.Entity<UserIntroduce>(entity =>
        {
            entity.HasKey(e => e.User_ID);
            entity.Property(e => e.User_NickName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Gender).IsRequired().HasMaxLength(1);
            entity.Property(e => e.IdNumber).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Cellphone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.Create_Account).IsRequired();
            entity.Property(e => e.User_Introduce).HasMaxLength(200);
            
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

        // 使用者權限實體配置
        modelBuilder.Entity<UserRights>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.User_Status).HasDefaultValue(true);
            entity.Property(e => e.ShoppingPermission).HasDefaultValue(true);
            entity.Property(e => e.MessagePermission).HasDefaultValue(true);
            entity.Property(e => e.SalesAuthority).HasDefaultValue(false);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserRights)
                  .HasForeignKey<UserRights>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 使用者錢包實體配置
        modelBuilder.Entity<UserWallet>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.User_Point).HasDefaultValue(0);
            entity.Property(e => e.Coupon_Number).HasMaxLength(50);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserWallet)
                  .HasForeignKey<UserWallet>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 會員銷售資料實體配置
        modelBuilder.Entity<MemberSalesProfile>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.BankAccountNumber).HasMaxLength(50);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.MemberSalesProfile)
                  .HasForeignKey<MemberSalesProfile>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 使用者銷售資訊實體配置
        modelBuilder.Entity<UserSalesInformation>(entity =>
        {
            entity.HasKey(e => e.User_Id);
            entity.Property(e => e.UserSales_Wallet).HasDefaultValue(0);
            
            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithOne(e => e.UserSalesInformation)
                  .HasForeignKey<UserSalesInformation>(e => e.User_Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 簽到統計實體配置
        modelBuilder.Entity<UserSignInStats>(entity =>
        {
            entity.HasKey(e => e.LogID);
            entity.Property(e => e.SignTime).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.PointsChanged).HasDefaultValue(0);
            entity.Property(e => e.ExpGained).HasDefaultValue(0);
            entity.Property(e => e.PointsChangedTime).IsRequired();
            entity.Property(e => e.ExpGainedTime).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.UserSignInStats)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 寵物實體配置
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetID);
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.PetName).IsRequired().HasMaxLength(50).HasDefaultValue("小可愛");
            entity.Property(e => e.Level).HasDefaultValue(0);
            entity.Property(e => e.LevelUpTime).IsRequired();
            entity.Property(e => e.Experience).HasDefaultValue(0);
            entity.Property(e => e.Hunger).HasDefaultValue(0);
            entity.Property(e => e.Mood).HasDefaultValue(0);
            entity.Property(e => e.Stamina).HasDefaultValue(0);
            entity.Property(e => e.Cleanliness).HasDefaultValue(0);
            entity.Property(e => e.Health).HasDefaultValue(0);
            entity.Property(e => e.SkinColor).HasMaxLength(50).HasDefaultValue("#ADD8E6");
            entity.Property(e => e.ColorChangedTime).IsRequired();
            entity.Property(e => e.BackgroundColor).HasMaxLength(50).HasDefaultValue("粉藍");
            entity.Property(e => e.BackgroundColorChangedTime).IsRequired();
            entity.Property(e => e.PointsChanged).HasDefaultValue(0);
            entity.Property(e => e.PointsChangedTime).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithOne(e => e.Pet)
                  .HasForeignKey<Pet>(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 小遊戲實體配置
        modelBuilder.Entity<MiniGame>(entity =>
        {
            entity.HasKey(e => e.PlayID);
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.PetID).IsRequired();
            entity.Property(e => e.Level).HasDefaultValue(0);
            entity.Property(e => e.MonsterCount).HasDefaultValue(0);
            entity.Property(e => e.SpeedMultiplier).HasColumnType("decimal(5,2)").HasDefaultValue(1.00m);
            entity.Property(e => e.Result).IsRequired().HasMaxLength(10).HasDefaultValue("Unknown");
            entity.Property(e => e.ExpGained).HasDefaultValue(0);
            entity.Property(e => e.PointsChanged).HasDefaultValue(0);
            entity.Property(e => e.HungerDelta).HasDefaultValue(0);
            entity.Property(e => e.MoodDelta).HasDefaultValue(0);
            entity.Property(e => e.StaminaDelta).HasDefaultValue(0);
            entity.Property(e => e.CleanlinessDelta).HasDefaultValue(0);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.Aborted).HasDefaultValue(false);
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.MiniGames)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 論壇實體配置
        modelBuilder.Entity<Forum>(entity =>
        {
            entity.HasKey(e => e.ForumID);
            entity.Property(e => e.GameID).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 唯一索引
            entity.HasIndex(e => e.GameID).IsUnique();
        });

        // 主題實體配置
        modelBuilder.Entity<ForumThread>(entity =>
        {
            entity.HasKey(e => e.ThreadID);
            entity.Property(e => e.ForumID).IsRequired();
            entity.Property(e => e.AuthorUserID).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("normal");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.Threads)
                  .HasForeignKey(e => e.AuthorUserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 主題回覆實體配置
        modelBuilder.Entity<ThreadPost>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ThreadID).IsRequired();
            entity.Property(e => e.AuthorUserID).IsRequired();
            entity.Property(e => e.ContentMD).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("normal");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.ThreadPosts)
                  .HasForeignKey(e => e.AuthorUserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 反應實體配置
        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.TargetType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TargetID).IsRequired();
            entity.Property(e => e.Kind).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 唯一索引
            entity.HasIndex(e => new { e.UserID, e.TargetType, e.TargetID, e.Kind }).IsUnique();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.Reactions)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 收藏實體配置
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.TargetType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TargetID).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 唯一索引
            entity.HasIndex(e => new { e.UserID, e.TargetType, e.TargetID }).IsUnique();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.Bookmarks)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 洞察貼文實體配置
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostID);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TLDR).HasMaxLength(500);
            entity.Property(e => e.BodyMD).IsRequired();
            entity.Property(e => e.Visibility).HasDefaultValue(true);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("draft");
            entity.Property(e => e.Pinned).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 通知實體配置
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.SourceId).IsRequired();
            entity.Property(e => e.ActionId).IsRequired();
            entity.Property(e => e.SenderId).IsRequired();
            entity.Property(e => e.NotificationTitle).HasMaxLength(200);
            entity.Property(e => e.NotificationMessage).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.Notifications)
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 通知接收者實體配置
        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.HasKey(e => e.RecipientID);
            entity.Property(e => e.NotificationID).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            
            // 唯一索引
            entity.HasIndex(e => new { e.NotificationID, e.UserID }).IsUnique();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.NotificationRecipients)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 聊天訊息實體配置
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            entity.Property(e => e.SenderId).IsRequired();
            entity.Property(e => e.ChatContent).IsRequired();
            entity.Property(e => e.SentAt).IsRequired();
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.IsSent).HasDefaultValue(true);
            
            // 外鍵關係 - 發送者
            entity.HasOne<User>(e => e.Sender)
                  .WithMany(e => e.SentMessages)
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // 外鍵關係 - 接收者
            entity.HasOne<User>(e => e.Receiver)
                  .WithMany(e => e.ReceivedMessages)
                  .HasForeignKey(e => e.ReceiverId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 群組實體配置
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupID);
            entity.Property(e => e.GroupName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 群組成員實體配置
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupID, e.UserID });
            entity.Property(e => e.JoinedAt).IsRequired();
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.GroupMembers)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 群組聊天實體配置
        modelBuilder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.GroupChatID);
            entity.Property(e => e.GroupID).IsRequired();
            entity.Property(e => e.SenderID).IsRequired();
            entity.Property(e => e.GroupChatContent).IsRequired();
            entity.Property(e => e.SentAt).IsRequired();
            entity.Property(e => e.IsSent).HasDefaultValue(true);
            
            // 外鍵關係
            entity.HasOne<User>()
                  .WithMany(e => e.GroupChats)
                  .HasForeignKey(e => e.SenderID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 群組封鎖實體配置
        modelBuilder.Entity<GroupBlock>(entity =>
        {
            entity.HasKey(e => e.BlockID);
            entity.Property(e => e.GroupID).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
            entity.Property(e => e.BlockedBy).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 唯一索引
            entity.HasIndex(e => new { e.GroupID, e.UserID }).IsUnique();
            
            // 外鍵關係 - 被封鎖者
            entity.HasOne<User>()
                  .WithMany(e => e.BlockedInGroups)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // 外鍵關係 - 封鎖者
            entity.HasOne<User>()
                  .WithMany(e => e.BlockedUsers)
                  .HasForeignKey(e => e.BlockedBy)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 官方商城相關實體配置
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierID);
            entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ProductInfo>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrencyCode).HasMaxLength(10);
            entity.Property(e => e.ShipmentQuantity).HasDefaultValue(0);
            entity.Property(e => e.ProductCreatedBy).HasMaxLength(100);
            entity.Property(e => e.ProductCreatedAt).IsRequired();
            entity.Property(e => e.ProductUpdatedBy).HasMaxLength(100);
            entity.Property(e => e.ProductUpdatedAt).IsRequired();
            entity.Property(e => e.UserID).IsRequired();
        });

        // 玩家市場相關實體配置
        modelBuilder.Entity<PlayerMarketProductInfo>(entity =>
        {
            entity.HasKey(e => e.PProductID);
            entity.Property(e => e.PProductType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PProductTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PProductDescription).IsRequired();
            entity.Property(e => e.PStatus).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PProductImgID).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // 熱度相關實體配置
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<MetricSource>(entity =>
        {
            entity.HasKey(e => e.SourceID);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Metric>(entity =>
        {
            entity.HasKey(e => e.MetricID);
            entity.Property(e => e.SourceID).IsRequired();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // 唯一索引
            entity.HasIndex(e => new { e.SourceID, e.Code }).IsUnique();
        });

        // 管理員相關實體配置
        modelBuilder.Entity<ManagerData>(entity =>
        {
            entity.HasKey(e => e.ManagerID);
            entity.Property(e => e.ManagerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ManagerAccount).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ManagerPassword).IsRequired().HasMaxLength(255);
            entity.Property(e => e.AdministratorRegistrationDate).IsRequired();
            
            // 唯一索引
            entity.HasIndex(e => e.ManagerAccount).IsUnique();
        });

        modelBuilder.Entity<ManagerRolePermission>(entity =>
        {
            entity.HasKey(e => e.ManagerRoleID);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ManagerRole>(entity =>
        {
            entity.HasKey(e => new { e.ManagerID, e.ManagerRoleID });
            entity.Property(e => e.ManagerRoleName).IsRequired().HasMaxLength(100);
        });

        // ChatMessage 關係配置
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);
            
            // 發送者關係
            entity.HasOne(e => e.Sender)
                  .WithMany(u => u.SentMessages)
                  .HasForeignKey(e => e.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            // 接收者關係
            entity.HasOne(e => e.Receiver)
                  .WithMany(u => u.ReceivedMessages)
                  .HasForeignKey(e => e.ReceiverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // GroupBlock 關係配置
        modelBuilder.Entity<GroupBlock>(entity =>
        {
            entity.HasKey(e => e.BlockID);
            
            // 被封鎖者關係
            entity.HasOne<User>()
                  .WithMany(u => u.BlockedInGroups)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.Restrict);

            // 封鎖者關係
            entity.HasOne<User>()
                  .WithMany(u => u.BlockedUsers)
                  .HasForeignKey(e => e.BlockedBy)
                  .OnDelete(DeleteBehavior.Restrict);
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
