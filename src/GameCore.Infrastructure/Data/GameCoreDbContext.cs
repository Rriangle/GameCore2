using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

public class GameCoreDbContext : DbContext
{
    public GameCoreDbContext(DbContextOptions<GameCoreDbContext> options) : base(options)
    {
    }

    // DbSet properties
    public DbSet<User> Users { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<UserSignInStats> UserSignInStats { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<StoreProduct> StoreProducts { get; set; }
    public DbSet<OrderInfo> OrderInfos { get; set; }
    public DbSet<PlayerOwned> PlayerOwneds { get; set; }
    public DbSet<PlayerOwnedToPlayer> PlayerOwnedToPlayers { get; set; }
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<ForumThread> ForumThreads { get; set; }
    public DbSet<ForumThreadPost> ForumThreadPosts { get; set; }
    public DbSet<ForumReaction> ForumReactions { get; set; }
    public DbSet<ForumBookmark> ForumBookmarks { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupConversation> GroupConversations { get; set; }
    public DbSet<BlockedUser> BlockedUsers { get; set; }
    public DbSet<MiniGame> MiniGames { get; set; }

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

        // UserSignInStats entity configuration
        modelBuilder.Entity<UserSignInStats>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SignInDate).HasColumnType("date");
            
            // Unique constraint: one sign-in per user per day
            entity.HasIndex(e => new { e.UserId, e.SignInDate }).IsUnique();
        });

        // Pet entity configuration
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(e => e.PetId);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SkinColor).HasMaxLength(20);
        });

        // Supplier entity configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        // StoreProduct entity configuration
        modelBuilder.Entity<StoreProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).HasMaxLength(20);
            entity.Property(e => e.ImageUrl).HasMaxLength(200);
        });

        // OrderInfo entity configuration
        modelBuilder.Entity<OrderInfo>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OrderStatus).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.ShippingAddress).HasMaxLength(200);
            entity.Property(e => e.TrackingNumber).HasMaxLength(50);
        });

        // PlayerOwned entity configuration
        modelBuilder.Entity<PlayerOwned>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.ImageUrl).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(20);
        });

        // PlayerOwnedToPlayer entity configuration
        modelBuilder.Entity<PlayerOwnedToPlayer>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PlatformFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OrderStatus).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
        });

        // ForumPost entity configuration
        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.HasKey(e => e.PostId);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        // ForumThread entity configuration
        modelBuilder.Entity<ForumThread>(entity =>
        {
            entity.HasKey(e => e.ThreadId);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        // ForumThreadPost entity configuration
        modelBuilder.Entity<ForumThreadPost>(entity =>
        {
            entity.HasKey(e => e.ThreadPostId);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        // ForumReaction entity configuration
        modelBuilder.Entity<ForumReaction>(entity =>
        {
            entity.HasKey(e => e.ReactionId);
            entity.Property(e => e.TargetType).HasMaxLength(20);
            entity.Property(e => e.ReactionType).HasMaxLength(20);
            
            // Unique constraint: one reaction per user per target
            entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId, e.ReactionType }).IsUnique();
        });

        // ForumBookmark entity configuration
        modelBuilder.Entity<ForumBookmark>(entity =>
        {
            entity.HasKey(e => e.BookmarkId);
            entity.Property(e => e.TargetType).HasMaxLength(20);
            
            // Unique constraint: one bookmark per user per target
            entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId }).IsUnique();
        });

        // Notification entity configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(20);
        });

        // Group entity configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.AvatarUrl).HasMaxLength(200);
        });

        // GroupMember entity configuration
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.MemberId);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        // GroupConversation entity configuration
        modelBuilder.Entity<GroupConversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId);
            entity.Property(e => e.MessageType).HasMaxLength(20);
        });

        // BlockedUser entity configuration
        modelBuilder.Entity<BlockedUser>(entity =>
        {
            entity.HasKey(e => e.BlockId);
            entity.Property(e => e.Reason).HasMaxLength(200);
            
            // Unique constraint: one block per blocker-blocked pair
            entity.HasIndex(e => new { e.BlockerId, e.BlockedId }).IsUnique();
        });

        // MiniGame entity configuration
        modelBuilder.Entity<MiniGame>(entity =>
        {
            entity.HasKey(e => e.GameId);
            entity.Property(e => e.GameType).HasMaxLength(50);
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
