using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    // 用戶相關 DbSet
    public DbSet<User> Users { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<MemberSalesProfile> MemberSalesProfiles { get; set; }
    public DbSet<UserSalesInformation> UserSalesInformations { get; set; }

    // 商城相關 DbSet
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    // 玩家市場相關 DbSet
    public DbSet<PlayerMarketListing> PlayerMarketListings { get; set; }
    public DbSet<PlayerMarketOrder> PlayerMarketOrders { get; set; }

    // 熱度與洞察相關 DbSet
    public DbSet<Game> Games { get; set; }
    public DbSet<MetricSource> MetricSources { get; set; }
    public DbSet<Metric> Metrics { get; set; }
    public DbSet<GameSourceMap> GameSourceMaps { get; set; }
    public DbSet<GameMetricDaily> GameMetricDailies { get; set; }
    public DbSet<PopularityIndexDaily> PopularityIndexDailies { get; set; }
    public DbSet<LeaderboardSnapshot> LeaderboardSnapshots { get; set; }

    // 洞察貼文相關 DbSet
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostMetricSnapshot> PostMetricSnapshots { get; set; }
    public DbSet<PostSource> PostSources { get; set; }

    // 論壇相關 DbSet
    public DbSet<Forum> Forums { get; set; }
    public DbSet<Thread> Threads { get; set; }
    public DbSet<ThreadPost> ThreadPosts { get; set; }

    // 互動相關 DbSet
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }

    // Stage 4: Social/Notifications/DM/Groups/Blocks 相關 DbSet
    public DbSet<NotificationSource> NotificationSources { get; set; }
    public DbSet<NotificationAction> NotificationActions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupChat> GroupChats { get; set; }
    public DbSet<GroupBlock> GroupBlocks { get; set; }
    public DbSet<ManagerData> ManagerData { get; set; }
    public DbSet<ManagerRolePermission> ManagerRolePermissions { get; set; }
    public DbSet<ManagerRole> ManagerRoles { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Mute> Mutes { get; set; }
    public DbSet<Style> Styles { get; set; }

    // Stage 5: Daily Sign-In 相關 DbSet
    public DbSet<DailySignIn> DailySignIns { get; set; }
    public DbSet<SignInReward> SignInRewards { get; set; }
    public DbSet<UserSignInHistory> UserSignInHistories { get; set; }

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

        // 遊戲實體配置
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.game_id);
            entity.Property(e => e.name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.genre).HasMaxLength(100);
            entity.Property(e => e.created_at).IsRequired();
        });

        // 指標來源實體配置
        modelBuilder.Entity<MetricSource>(entity =>
        {
            entity.HasKey(e => e.source_id);
            entity.Property(e => e.name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.note).HasMaxLength(500);
            entity.Property(e => e.created_at).IsRequired();
        });

        // 指標實體配置
        modelBuilder.Entity<Metric>(entity =>
        {
            entity.HasKey(e => e.metric_id);
            entity.Property(e => e.source_id).IsRequired();
            entity.Property(e => e.code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.unit).HasMaxLength(50);
            entity.Property(e => e.description).HasMaxLength(200);
            entity.Property(e => e.is_active).IsRequired();
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Source)
                  .WithMany(e => e.Metrics)
                  .HasForeignKey(e => e.source_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合唯一索引
            entity.HasIndex(e => new { e.source_id, e.code }).IsUnique();
        });

        // 遊戲來源對應實體配置
        modelBuilder.Entity<GameSourceMap>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.game_id).IsRequired();
            entity.Property(e => e.source_id).IsRequired();
            entity.Property(e => e.external_key).IsRequired().HasMaxLength(200);

            // 外鍵關係
            entity.HasOne(e => e.Game)
                  .WithMany(e => e.GameSourceMaps)
                  .HasForeignKey(e => e.game_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Source)
                  .WithMany(e => e.GameSourceMaps)
                  .HasForeignKey(e => e.source_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合唯一索引
            entity.HasIndex(e => new { e.game_id, e.source_id }).IsUnique();
        });

        // 遊戲每日指標實體配置
        modelBuilder.Entity<GameMetricDaily>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.game_id).IsRequired();
            entity.Property(e => e.metric_id).IsRequired();
            entity.Property(e => e.date).IsRequired();
            entity.Property(e => e.value).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.agg_method).HasMaxLength(20);
            entity.Property(e => e.quality).HasMaxLength(20);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Game)
                  .WithMany(e => e.GameMetricDailies)
                  .HasForeignKey(e => e.game_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Metric)
                  .WithMany(e => e.GameMetricDailies)
                  .HasForeignKey(e => e.metric_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合唯一索引
            entity.HasIndex(e => new { e.game_id, e.metric_id, e.date }).IsUnique();
            entity.HasIndex(e => new { e.date, e.metric_id });
            entity.HasIndex(e => new { e.game_id, e.date });
        });

        // 每日熱度指數實體配置
        modelBuilder.Entity<PopularityIndexDaily>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.game_id).IsRequired();
            entity.Property(e => e.date).IsRequired();
            entity.Property(e => e.index_value).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Game)
                  .WithMany(e => e.PopularityIndexDailies)
                  .HasForeignKey(e => e.game_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合唯一索引
            entity.HasIndex(e => new { e.game_id, e.date }).IsUnique();
            entity.HasIndex(e => e.date);
        });

        // 排行榜快照實體配置
        modelBuilder.Entity<LeaderboardSnapshot>(entity =>
        {
            entity.HasKey(e => e.snapshot_id);
            entity.Property(e => e.period).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ts).IsRequired();
            entity.Property(e => e.rank).IsRequired();
            entity.Property(e => e.game_id).IsRequired();
            entity.Property(e => e.index_value).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Game)
                  .WithMany(e => e.LeaderboardSnapshots)
                  .HasForeignKey(e => e.game_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 索引
            entity.HasIndex(e => new { e.period, e.ts, e.rank });
            entity.HasIndex(e => new { e.period, e.ts, e.rank, e.game_id }).IsUnique();
            entity.HasIndex(e => new { e.period, e.ts, e.game_id });
        });

        // 洞察貼文實體配置
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.post_id);
            entity.Property(e => e.type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.tldr).HasMaxLength(300);
            entity.Property(e => e.body_md).HasMaxLength(10000);
            entity.Property(e => e.visibility).IsRequired();
            entity.Property(e => e.status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.pinned).IsRequired();
            entity.Property(e => e.created_by).IsRequired();
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Game)
                  .WithMany(e => e.Posts)
                  .HasForeignKey(e => e.game_id)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.created_by)
                  .OnDelete(DeleteBehavior.Cascade);

            // 索引
            entity.HasIndex(e => new { e.type, e.created_at });
            entity.HasIndex(e => new { e.game_id, e.created_at });
            entity.HasIndex(e => new { e.status, e.created_at });
        });

        // 貼文指標快照實體配置
        modelBuilder.Entity<PostMetricSnapshot>(entity =>
        {
            entity.HasKey(e => e.post_id);
            entity.Property(e => e.game_id).IsRequired();
            entity.Property(e => e.date).IsRequired();
            entity.Property(e => e.index_value).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.details_json).HasMaxLength(2000);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Post)
                  .WithOne(e => e.PostMetricSnapshot)
                  .HasForeignKey<PostMetricSnapshot>(e => e.post_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Game)
                  .WithMany()
                  .HasForeignKey(e => e.game_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 貼文引用來源實體配置
        modelBuilder.Entity<PostSource>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.post_id).IsRequired();
            entity.Property(e => e.source_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.url).HasMaxLength(500);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Post)
                  .WithMany(e => e.PostSources)
                  .HasForeignKey(e => e.post_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 索引
            entity.HasIndex(e => e.post_id);
        });

        // 論壇實體配置
        modelBuilder.Entity<Forum>(entity =>
        {
            entity.HasKey(e => e.forum_id);
            entity.Property(e => e.game_id).IsRequired();
            entity.Property(e => e.name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.description).HasMaxLength(1000);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Game)
                  .WithOne(e => e.Forum)
                  .HasForeignKey<Forum>(e => e.game_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 唯一索引
            entity.HasIndex(e => e.game_id).IsUnique();
        });

        // 主題實體配置
        modelBuilder.Entity<Thread>(entity =>
        {
            entity.HasKey(e => e.thread_id);
            entity.Property(e => e.forum_id).IsRequired();
            entity.Property(e => e.author_user_id).IsRequired();
            entity.Property(e => e.title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Forum)
                  .WithMany(e => e.Threads)
                  .HasForeignKey(e => e.forum_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AuthorUser)
                  .WithMany()
                  .HasForeignKey(e => e.author_user_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 索引
            entity.HasIndex(e => new { e.forum_id, e.updated_at });
        });

        // 主題回覆實體配置
        modelBuilder.Entity<ThreadPost>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.thread_id).IsRequired();
            entity.Property(e => e.author_user_id).IsRequired();
            entity.Property(e => e.content_md).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Thread)
                  .WithMany(e => e.ThreadPosts)
                  .HasForeignKey(e => e.thread_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AuthorUser)
                  .WithMany()
                  .HasForeignKey(e => e.author_user_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentPost)
                  .WithMany(e => e.ChildPosts)
                  .HasForeignKey(e => e.parent_post_id)
                  .OnDelete(DeleteBehavior.Restrict);

            // 索引
            entity.HasIndex(e => new { e.thread_id, e.created_at });
        });

        // 反應實體配置
        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.user_id).IsRequired();
            entity.Property(e => e.target_type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.target_id).IsRequired();
            entity.Property(e => e.kind).IsRequired().HasMaxLength(20);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合唯一索引
            entity.HasIndex(e => new { e.user_id, e.target_type, e.target_id, e.kind }).IsUnique();
            entity.HasIndex(e => new { e.target_type, e.target_id });
        });

        // 收藏實體配置
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.user_id).IsRequired();
            entity.Property(e => e.target_type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.target_id).IsRequired();
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合唯一索引
            entity.HasIndex(e => new { e.user_id, e.target_type, e.target_id }).IsUnique();
            entity.HasIndex(e => new { e.target_type, e.target_id });
        });

        // 商城相關實體配置（保留原有的）
        ConfigureProductEntities(modelBuilder);
        ConfigureOrderEntities(modelBuilder);
        ConfigurePlayerMarketEntities(modelBuilder);
        ConfigureStage4Entities(modelBuilder);
        ConfigureStage5Entities(modelBuilder);
    }

    /// <summary>
    /// 配置商城相關實體
    /// </summary>
    private void ConfigureProductEntities(ModelBuilder modelBuilder)
    {
        // 產品實體配置
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.StockQuantity).IsRequired();
            entity.Property(e => e.CategoryId).IsRequired();
            entity.Property(e => e.ImageUrl).HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.IsOfficialStore).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 產品分類實體配置
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IconUrl).HasMaxLength(100);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.SortOrder).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }

    /// <summary>
    /// 配置訂單相關實體
    /// </summary>
    private void ConfigureOrderEntities(ModelBuilder modelBuilder)
    {
        // 訂單實體配置
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PaymentStatus).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 訂單項目實體配置
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

            // 外鍵關係
            entity.HasOne(e => e.Order)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                  .WithMany(e => e.OrderItems)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 支付交易實體配置
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Order)
                  .WithMany()
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// 配置玩家市場相關實體
    /// </summary>
    private void ConfigurePlayerMarketEntities(ModelBuilder modelBuilder)
    {
        // 玩家市場掛單實體配置
        modelBuilder.Entity<PlayerMarketListing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SellerId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Seller)
                  .WithMany()
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 玩家市場訂單實體配置
        modelBuilder.Entity<PlayerMarketOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ListingId).IsRequired();
            entity.Property(e => e.BuyerId).IsRequired();
            entity.Property(e => e.SellerId).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.PlatformFee).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Listing)
                  .WithMany()
                  .HasForeignKey(e => e.ListingId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Buyer)
                  .WithMany()
                  .HasForeignKey(e => e.BuyerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Seller)
                  .WithMany()
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// 配置 Stage 4: Social/Notifications/DM/Groups/Blocks 相關實體
    /// </summary>
    private void ConfigureStage4Entities(ModelBuilder modelBuilder)
    {
        // 通知來源實體配置
        modelBuilder.Entity<NotificationSource>(entity =>
        {
            entity.HasKey(e => e.source_id);
            entity.Property(e => e.source_name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.created_at).IsRequired();
        });

        // 通知行為實體配置
        modelBuilder.Entity<NotificationAction>(entity =>
        {
            entity.HasKey(e => e.action_id);
            entity.Property(e => e.action_name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.created_at).IsRequired();
        });

        // 通知主表實體配置
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.notification_id);
            entity.Property(e => e.notification_title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.notification_message).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Source)
                  .WithMany(e => e.Notifications)
                  .HasForeignKey(e => e.source_id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Action)
                  .WithMany(e => e.Notifications)
                  .HasForeignKey(e => e.action_id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.sender_id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SenderManager)
                  .WithMany(e => e.Notifications)
                  .HasForeignKey(e => e.sender_manager_id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Group)
                  .WithMany()
                  .HasForeignKey(e => e.group_id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 通知接收者實體配置
        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.HasKey(e => e.recipient_id);
            entity.Property(e => e.is_read).IsRequired();
            entity.Property(e => e.read_at);

            // 外鍵關係
            entity.HasOne(e => e.Notification)
                  .WithMany(e => e.Recipients)
                  .HasForeignKey(e => e.notification_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 聊天訊息實體配置
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.message_id);
            entity.Property(e => e.chat_content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.sent_at).IsRequired();
            entity.Property(e => e.is_read).IsRequired();
            entity.Property(e => e.is_sent).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.ChatMessages)
                  .HasForeignKey(e => e.manager_id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.sender_id)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Receiver)
                  .WithMany()
                  .HasForeignKey(e => e.receiver_id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 群組實體配置
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.group_id);
            entity.Property(e => e.group_name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.created_by)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 群組成員實體配置
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.group_id, e.user_id });
            entity.Property(e => e.joined_at).IsRequired();
            entity.Property(e => e.is_admin).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Group)
                  .WithMany(e => e.Members)
                  .HasForeignKey(e => e.group_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 群組聊天訊息實體配置
        modelBuilder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.group_chat_id);
            entity.Property(e => e.group_chat_content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.sent_at).IsRequired();
            entity.Property(e => e.is_sent).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Group)
                  .WithMany(e => e.ChatMessages)
                  .HasForeignKey(e => e.group_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Sender)
                  .WithMany()
                  .HasForeignKey(e => e.sender_id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 群組封鎖實體配置
        modelBuilder.Entity<GroupBlock>(entity =>
        {
            entity.HasKey(e => e.block_id);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Group)
                  .WithMany()
                  .HasForeignKey(e => e.group_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.BlockedUser)
                  .WithMany()
                  .HasForeignKey(e => e.user_id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.BlockedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.blocked_by)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 管理者資料實體配置
        modelBuilder.Entity<ManagerData>(entity =>
        {
            entity.HasKey(e => e.Manager_Id);
            entity.Property(e => e.Manager_Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manager_Account).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manager_Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Administrator_registration_date).IsRequired();

            // 唯一索引
            entity.HasIndex(e => e.Manager_Account).IsUnique();
        });

        // 管理者角色權限實體配置
        modelBuilder.Entity<ManagerRolePermission>(entity =>
        {
            entity.HasKey(e => e.ManagerRole_Id);
            entity.Property(e => e.role_name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.created_at).IsRequired();
            entity.Property(e => e.updated_at);
        });

        // 管理者角色指派實體配置
        modelBuilder.Entity<ManagerRole>(entity =>
        {
            entity.HasKey(e => new { e.Manager_Id, e.ManagerRole_Id });
            entity.Property(e => e.ManagerRole).IsRequired().HasMaxLength(100);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.Roles)
                  .HasForeignKey(e => e.Manager_Id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.RolePermission)
                  .WithMany(e => e.ManagerRoles)
                  .HasForeignKey(e => e.ManagerRole_Id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 管理員登入追蹤實體配置
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.admin_id);
            entity.Property(e => e.manager_id).IsRequired();
            entity.Property(e => e.last_login);
            entity.Property(e => e.created_at).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.AdminLogins)
                  .HasForeignKey(e => e.manager_id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 禁言選項實體配置
        modelBuilder.Entity<Mute>(entity =>
        {
            entity.HasKey(e => e.mute_id);
            entity.Property(e => e.mute_name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.created_at).IsRequired();
            entity.Property(e => e.is_active).IsRequired();
            entity.Property(e => e.manager_id).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.Mutes)
                  .HasForeignKey(e => e.manager_id)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 樣式實體配置
        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.style_id);
            entity.Property(e => e.style_name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.effect_desc).HasMaxLength(500);
            entity.Property(e => e.created_at).IsRequired();
            entity.Property(e => e.manager_id).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.Manager)
                  .WithMany(e => e.Styles)
                  .HasForeignKey(e => e.manager_id)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// 配置 Stage 5: Daily Sign-In 相關實體
    /// </summary>
    private void ConfigureStage5Entities(ModelBuilder modelBuilder)
    {
        // 每日簽到實體配置
        modelBuilder.Entity<DailySignIn>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.SignInDate).IsRequired();
            entity.Property(e => e.SignInTime).IsRequired();
            entity.Property(e => e.CurrentStreak).IsRequired();
            entity.Property(e => e.LongestStreak).IsRequired();
            entity.Property(e => e.MonthlyPerfectAttendance).IsRequired();
            entity.Property(e => e.PointsEarned).IsRequired();
            entity.Property(e => e.IsBonusDay).IsRequired();
            entity.Property(e => e.BonusMultiplier).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // 複合索引：用戶ID + 簽到日期（確保每天只能簽到一次）
            entity.HasIndex(e => new { e.UserId, e.SignInDate }).IsUnique();
        });

        // 簽到獎勵實體配置
        modelBuilder.Entity<SignInReward>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.PointsReward).IsRequired();
            entity.Property(e => e.StreakRequirement).IsRequired();
            entity.Property(e => e.AttendanceRequirement).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // 用戶簽到歷史實體配置
        modelBuilder.Entity<UserSignInHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.SignInDate).IsRequired();
            entity.Property(e => e.SignInTime).IsRequired();
            entity.Property(e => e.DayOfWeek).IsRequired();
            entity.Property(e => e.DayOfMonth).IsRequired();
            entity.Property(e => e.Month).IsRequired();
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.WeekOfYear).IsRequired();
            entity.Property(e => e.PointsEarned).IsRequired();
            entity.Property(e => e.IsStreakContinued).IsRequired();
            entity.Property(e => e.IsBonusDay).IsRequired();
            entity.Property(e => e.BonusMultiplier).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // 外鍵關係
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // 索引：用戶ID + 簽到日期（優化查詢）
            entity.HasIndex(e => new { e.UserId, e.SignInDate });
            entity.HasIndex(e => new { e.UserId, e.Year, e.Month });
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
