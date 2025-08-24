namespace GameCore.Api.DTOs;

// Sign-in DTOs
public class SignInResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int PointsEarned { get; set; }
    public int ExpEarned { get; set; }
    public int StreakCount { get; set; }
    public bool IsHoliday { get; set; }
    public bool IsStreakBonus { get; set; }
    public bool IsMonthBonus { get; set; }
}

public class SignInStatsDto
{
    public int TotalSignIns { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalPointsEarned { get; set; }
    public int TotalExpEarned { get; set; }
    public DateTime? LastSignInDate { get; set; }
    public bool HasSignedInToday { get; set; }
}

// Pet DTOs
public class PetDto
{
    public int PetId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Hunger { get; set; }
    public int Mood { get; set; }
    public int Stamina { get; set; }
    public int Cleanliness { get; set; }
    public int Health { get; set; }
    public string SkinColor { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastDecayAt { get; set; }
    public bool CanPlayMiniGame { get; set; }
    public int RequiredExpForNextLevel { get; set; }
}

public class PetInteractionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PetDto? Pet { get; set; }
    public string InteractionType { get; set; } = string.Empty;
    public int StatChange { get; set; }
}

public class PetColorChangeResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PetDto? Pet { get; set; }
    public string OldColor { get; set; } = string.Empty;
    public string NewColor { get; set; } = string.Empty;
    public int PointsDeducted { get; set; }
}

// Store DTOs
public class StoreProductDto
{
    public int ProductId { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? PaymentAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public OrderDto? Order { get; set; }
    public decimal PointsDeducted { get; set; }
    public decimal RemainingBalance { get; set; }
}

// Market DTOs
public class MarketItemDto
{
    public int ItemId { get; set; }
    public int UserId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SoldAt { get; set; }
}

public class MarketTransactionDto
{
    public int TransactionId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public int BuyerId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal PlatformFee { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime? SellerConfirmedAt { get; set; }
    public DateTime? BuyerConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MarketItemResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public MarketItemDto? Item { get; set; }
}

public class MarketTransactionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public MarketTransactionDto? Transaction { get; set; }
    public decimal PointsDeducted { get; set; }
    public decimal SellerCredited { get; set; }
    public decimal PlatformFeeAmount { get; set; }
}

// Forum DTOs
public class ForumPostDto
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ReplyCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public class ForumThreadDto
{
    public int ThreadId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ReplyCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public class ForumThreadPostDto
{
    public int ThreadPostId { get; set; }
    public int ThreadId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsLiked { get; set; }
    public bool IsBookmarked { get; set; }
}

public class ForumBookmarkDto
{
    public int BookmarkId { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public int TargetId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ForumPostResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ForumPostDto? Post { get; set; }
}

public class ForumThreadResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ForumThreadDto? Thread { get; set; }
}

public class ForumThreadPostResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ForumThreadPostDto? ThreadPost { get; set; }
}

public class ForumReactionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int LikeCount { get; set; }
}

public class ForumBookmarkResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsBookmarked { get; set; }
}

// MiniGame DTOs
public class MiniGameResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsWin { get; set; }
    public int Score { get; set; }
    public int PointsEarned { get; set; }
    public int ExpEarned { get; set; }
    public string GameType { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public int PetExpGained { get; set; }
    public int PetNewLevel { get; set; }
    public bool PetLeveledUp { get; set; }
}

public class MiniGameRecordDto
{
    public int GameId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;
    public bool IsWin { get; set; }
    public int Score { get; set; }
    public int PointsEarned { get; set; }
    public int ExpEarned { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MiniGameStatsDto
{
    public int TotalGames { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public double WinRate { get; set; }
    public int TotalScore { get; set; }
    public int AverageScore { get; set; }
    public int TotalPointsEarned { get; set; }
    public int TotalExpEarned { get; set; }
    public int HighestScore { get; set; }
    public DateTime? LastPlayedAt { get; set; }
}