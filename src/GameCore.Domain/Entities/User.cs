using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者基本資料表
/// 對應資料庫 Users 表
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 使用者編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者姓名 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_Name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (必填，雜湊儲存)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string User_Password { get; set; } = string.Empty;

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    // 導航屬性 - 一對一關係
    /// <summary>
    /// 使用者介紹資料
    /// </summary>
    public virtual UserIntroduce? UserIntroduce { get; set; }

    /// <summary>
    /// 使用者權限
    /// </summary>
    public virtual UserRights? UserRights { get; set; }

    /// <summary>
    /// 使用者錢包
    /// </summary>
    public virtual UserWallet? UserWallet { get; set; }

    /// <summary>
    /// 會員銷售資料
    /// </summary>
    public virtual MemberSalesProfile? MemberSalesProfile { get; set; }

    /// <summary>
    /// 使用者銷售資訊
    /// </summary>
    public virtual UserSalesInformation? UserSalesInformation { get; set; }

    /// <summary>
    /// 簽到統計
    /// </summary>
    public virtual ICollection<UserSignInStats> UserSignInStats { get; set; } = new List<UserSignInStats>();

    /// <summary>
    /// 寵物資料
    /// </summary>
    public virtual Pet? Pet { get; set; }

    /// <summary>
    /// 小遊戲記錄
    /// </summary>
    public virtual ICollection<MiniGame> MiniGames { get; set; } = new List<MiniGame>();

    /// <summary>
    /// 論壇主題
    /// </summary>
            public virtual ICollection<ForumThread> Threads { get; set; } = new List<ForumThread>();

    /// <summary>
    /// 論壇回覆
    /// </summary>
    public virtual ICollection<ThreadPost> ThreadPosts { get; set; } = new List<ThreadPost>();

    /// <summary>
    /// 反應記錄
    /// </summary>
    public virtual ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();

    /// <summary>
    /// 收藏記錄
    /// </summary>
    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    /// <summary>
    /// 通知
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    /// <summary>
    /// 通知接收者
    /// </summary>
    public virtual ICollection<NotificationRecipient> NotificationRecipients { get; set; } = new List<NotificationRecipient>();

    /// <summary>
    /// 聊天訊息 (發送者)
    /// </summary>
    public virtual ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();

    /// <summary>
    /// 聊天訊息 (接收者)
    /// </summary>
    public virtual ICollection<ChatMessage> ReceivedMessages { get; set; } = new List<ChatMessage>();

    /// <summary>
    /// 群組成員
    /// </summary>
    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    /// <summary>
    /// 群組聊天 (發送者)
    /// </summary>
    public virtual ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();

    /// <summary>
    /// 群組封鎖 (被封鎖者)
    /// </summary>
    public virtual ICollection<GroupBlock> BlockedInGroups { get; set; } = new List<GroupBlock>();

    /// <summary>
    /// 群組封鎖 (封鎖者)
    /// </summary>
    public virtual ICollection<GroupBlock> BlockedUsers { get; set; } = new List<GroupBlock>();

    /// <summary>
    /// 官方商城訂單
    /// </summary>
    public virtual ICollection<OrderInfo> Orders { get; set; } = new List<OrderInfo>();

    /// <summary>
    /// 玩家市場商品 (賣家)
    /// </summary>
    public virtual ICollection<PlayerMarketProductInfo> MarketProducts { get; set; } = new List<PlayerMarketProductInfo>();

    /// <summary>
    /// 玩家市場訂單 (買家)
    /// </summary>
    public virtual ICollection<PlayerMarketOrderInfo> MarketOrdersAsBuyer { get; set; } = new List<PlayerMarketOrderInfo>();

    /// <summary>
    /// 玩家市場訂單 (賣家)
    /// </summary>
    public virtual ICollection<PlayerMarketOrderInfo> MarketOrdersAsSeller { get; set; } = new List<PlayerMarketOrderInfo>();

    /// <summary>
    /// 洞察貼文
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    /// <summary>
    /// 錢包交易記錄
    /// </summary>
    public virtual ICollection<UserWalletTransaction> WalletTransactions { get; set; } = new List<UserWalletTransaction>();
}
