using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Users")]
public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(200)]
    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsEmailVerified { get; set; } = false;

    // Navigation properties
    public virtual UserWallet? Wallet { get; set; }
    public virtual ICollection<UserSignInStats> SignInStats { get; set; } = new List<UserSignInStats>();
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public virtual ICollection<OrderInfo> StoreOrders { get; set; } = new List<OrderInfo>();
    public virtual ICollection<PlayerOwned> MarketItems { get; set; } = new List<PlayerOwned>();
    public virtual ICollection<PlayerOwnedToPlayer> SellerTransactions { get; set; } = new List<PlayerOwnedToPlayer>();
    public virtual ICollection<PlayerOwnedToPlayer> BuyerTransactions { get; set; } = new List<PlayerOwnedToPlayer>();
    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
    public virtual ICollection<ForumThread> ForumThreads { get; set; } = new List<ForumThread>();
    public virtual ICollection<ForumThreadPost> ForumThreadPosts { get; set; } = new List<ForumThreadPost>();
    public virtual ICollection<ForumReaction> ForumReactions { get; set; } = new List<ForumReaction>();
    public virtual ICollection<ForumBookmark> ForumBookmarks { get; set; } = new List<ForumBookmark>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Group> OwnedGroups { get; set; } = new List<Group>();
    public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public virtual ICollection<GroupConversation> GroupConversations { get; set; } = new List<GroupConversation>();
    public virtual ICollection<BlockedUser> BlockedUsers { get; set; } = new List<BlockedUser>();
    public virtual ICollection<BlockedUser> BlockedByUsers { get; set; } = new List<BlockedUser>();
    public virtual ICollection<MiniGame> MiniGames { get; set; } = new List<MiniGame>();
}
