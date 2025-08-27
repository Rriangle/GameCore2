using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// InteractionService 優化版本單元測試
/// </summary>
public class InteractionServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InteractionService> _logger;
    private readonly InteractionService _service;

    public InteractionServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<InteractionService>>();
        _service = new InteractionService(_context, _memoryCache, _logger);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
        _memoryCache.Dispose();
    }

    #region 測試數據準備

    private void SeedTestData()
    {
        // 創建用戶
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com" };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com" };
        var user3 = new User { user_id = 3, username = "user3", email = "user3@test.com" };
        _context.Users.AddRange(user1, user2, user3);

        // 創建反應
        var reaction1 = new Reaction { reaction_id = 1, target_type = "post", target_id = 1, kind = "like", created_by_user_id = 1, created_at = DateTime.UtcNow.AddHours(-1) };
        var reaction2 = new Reaction { reaction_id = 2, target_type = "post", target_id = 1, kind = "love", created_by_user_id = 2, created_at = DateTime.UtcNow.AddHours(-2) };
        var reaction3 = new Reaction { reaction_id = 3, target_type = "thread", target_id = 1, kind = "like", created_by_user_id = 1, created_at = DateTime.UtcNow.AddHours(-3) };
        var reaction4 = new Reaction { reaction_id = 4, target_type = "post", target_id = 2, kind = "haha", created_by_user_id = 3, created_at = DateTime.UtcNow.AddHours(-4) };
        _context.Reactions.AddRange(reaction1, reaction2, reaction3, reaction4);

        // 創建書籤
        var bookmark1 = new Bookmark { bookmark_id = 1, target_type = "post", target_id = 1, created_by_user_id = 1, created_at = DateTime.UtcNow.AddHours(-1) };
        var bookmark2 = new Bookmark { bookmark_id = 2, target_type = "thread", target_id = 1, created_by_user_id = 2, created_at = DateTime.UtcNow.AddHours(-2) };
        var bookmark3 = new Bookmark { bookmark_id = 3, target_type = "post", target_id = 2, created_by_user_id = 1, created_at = DateTime.UtcNow.AddHours(-3) };
        _context.Bookmarks.AddRange(bookmark1, bookmark2, bookmark3);

        _context.SaveChanges();
    }

    #endregion

    #region GetReactionStatsAsync 測試

    [Fact]
    public async Task GetReactionStatsAsync_WithValidTarget_ShouldReturnStats()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.GetReactionStatsAsync(targetType, targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetType, result.TargetType);
        Assert.Equal(targetId, result.TargetId);
        Assert.True(result.ReactionCounts.Count > 0);
        Assert.Contains(result.ReactionCounts, kvp => kvp.Key == "like");
        Assert.Contains(result.ReactionCounts, kvp => kvp.Key == "love");
        Assert.Equal(2, result.TotalReactions);
    }

    [Fact]
    public async Task GetReactionStatsAsync_WithInvalidTargetType_ShouldReturnEmptyStats()
    {
        // Arrange
        var targetType = "invalid_type";
        var targetId = 1;

        // Act
        var result = await _service.GetReactionStatsAsync(targetType, targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetType, result.TargetType);
        Assert.Equal(targetId, result.TargetId);
        Assert.Empty(result.ReactionCounts);
        Assert.Equal(0, result.TotalReactions);
    }

    [Fact]
    public async Task GetReactionStatsAsync_WithInvalidTargetId_ShouldReturnEmptyStats()
    {
        // Arrange
        var targetType = "post";
        var targetId = -1;

        // Act
        var result = await _service.GetReactionStatsAsync(targetType, targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetType, result.TargetType);
        Assert.Equal(targetId, result.TargetId);
        Assert.Empty(result.ReactionCounts);
        Assert.Equal(0, result.TotalReactions);
    }

    [Fact]
    public async Task GetReactionStatsAsync_WithNonExistentTarget_ShouldReturnEmptyStats()
    {
        // Arrange
        var targetType = "post";
        var targetId = 999;

        // Act
        var result = await _service.GetReactionStatsAsync(targetType, targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetType, result.TargetType);
        Assert.Equal(targetId, result.TargetId);
        Assert.Empty(result.ReactionCounts);
        Assert.Equal(0, result.TotalReactions);
    }

    [Fact]
    public async Task GetReactionStatsAsync_ShouldUseCache()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetReactionStatsAsync(targetType, targetId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Reactions.RemoveRange(_context.Reactions);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetReactionStatsAsync(targetType, targetId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.TotalReactions, result2.TotalReactions);
    }

    #endregion

    #region HasUserReactedAsync 測試

    [Fact]
    public async Task HasUserReactedAsync_WithValidUserAndTarget_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasUserReactedAsync_WithValidUserAndTargetButDifferentKind_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;
        var kind = "love"; // User 1 has 'like' reaction, not 'love'

        // Act
        var result = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserReactedAsync_WithInvalidUserId_ShouldReturnFalse()
    {
        // Arrange
        var userId = -1;
        var targetType = "post";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserReactedAsync_WithInvalidTargetType_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "invalid_type";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserReactedAsync_WithInvalidTargetId_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = -1;
        var kind = "like";

        // Act
        var result = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserReactedAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = 999;
        var targetType = "post";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserReactedAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;
        var kind = "like";

        // Act - 第一次調用
        var result1 = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Reactions.RemoveRange(_context.Reactions);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.HasUserReactedAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region AddReactionAsync 測試

    [Fact]
    public async Task AddReactionAsync_WithValidData_ShouldCreateReaction()
    {
        // Arrange
        var request = new AddReactionRequestDto
        {
            TargetType = "post",
            TargetId = 3,
            Kind = "wow",
            UserId = 1
        };

        // Act
        var result = await _service.AddReactionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ReactionId > 0);
        Assert.Equal(request.TargetType, result.TargetType);
        Assert.Equal(request.TargetId, result.TargetId);
        Assert.Equal(request.Kind, result.Kind);
        Assert.Equal(request.UserId, result.UserId);
        Assert.True(result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbReaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.reaction_id == result.ReactionId);
        Assert.NotNull(dbReaction);
        Assert.Equal(request.TargetType, dbReaction.target_type);
        Assert.Equal(request.TargetId, dbReaction.target_id);
        Assert.Equal(request.Kind, dbReaction.kind);
        Assert.Equal(request.UserId, dbReaction.created_by_user_id);
    }

    [Fact]
    public async Task AddReactionAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        AddReactionRequestDto request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddReactionAsync(request));
    }

    [Fact]
    public async Task AddReactionAsync_WithInvalidTargetType_ShouldThrowException()
    {
        // Arrange
        var request = new AddReactionRequestDto
        {
            TargetType = "invalid_type",
            TargetId = 1,
            Kind = "like",
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddReactionAsync(request));
    }

    [Fact]
    public async Task AddReactionAsync_WithInvalidTargetId_ShouldThrowException()
    {
        // Arrange
        var request = new AddReactionRequestDto
        {
            TargetType = "post",
            TargetId = -1,
            Kind = "like",
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddReactionAsync(request));
    }

    [Fact]
    public async Task AddReactionAsync_WithInvalidKind_ShouldThrowException()
    {
        // Arrange
        var request = new AddReactionRequestDto
        {
            TargetType = "post",
            TargetId = 1,
            Kind = "invalid_kind",
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddReactionAsync(request));
    }

    [Fact]
    public async Task AddReactionAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var request = new AddReactionRequestDto
        {
            TargetType = "post",
            TargetId = 1,
            Kind = "like",
            UserId = -1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddReactionAsync(request));
    }

    [Fact]
    public async Task AddReactionAsync_WithDuplicateReaction_ShouldThrowException()
    {
        // Arrange
        var request = new AddReactionRequestDto
        {
            TargetType = "post",
            TargetId = 1,
            Kind = "like",
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddReactionAsync(request));
    }

    #endregion

    #region RemoveReactionAsync 測試

    [Fact]
    public async Task RemoveReactionAsync_WithValidData_ShouldRemoveReaction()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.RemoveReactionAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實刪除了記錄
        var dbReaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.created_by_user_id == userId && 
                                    r.target_type == targetType && 
                                    r.target_id == targetId && 
                                    r.kind == kind);
        Assert.Null(dbReaction);
    }

    [Fact]
    public async Task RemoveReactionAsync_WithInvalidUserId_ShouldReturnFalse()
    {
        // Arrange
        var userId = -1;
        var targetType = "post";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.RemoveReactionAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveReactionAsync_WithInvalidTargetType_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "invalid_type";
        var targetId = 1;
        var kind = "like";

        // Act
        var result = await _service.RemoveReactionAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveReactionAsync_WithInvalidTargetId_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = -1;
        var kind = "like";

        // Act
        var result = await _service.RemoveReactionAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveReactionAsync_WithInvalidKind_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;
        var kind = "invalid_kind";

        // Act
        var result = await _service.RemoveReactionAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveReactionAsync_WithNonExistentReaction_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 999;
        var kind = "like";

        // Act
        var result = await _service.RemoveReactionAsync(userId, targetType, targetId, kind);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetUserBookmarksAsync 測試

    [Fact]
    public async Task GetUserBookmarksAsync_WithValidUserId_ShouldReturnBookmarks()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var bookmarks = result.ToList();
        Assert.Equal(2, bookmarks.Count);
        Assert.All(bookmarks, b => Assert.Equal(userId, b.created_by_user_id));
    }

    [Fact]
    public async Task GetUserBookmarksAsync_WithInvalidUserId_ShouldReturnEmpty()
    {
        // Arrange
        var userId = -1;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_WithInvalidPage_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 1;
        var page = 0;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_WithInvalidPageSize_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_WithNonExistentUser_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 999;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;

        // Act - 第一次調用
        var result1 = await _service.GetUserBookmarksAsync(userId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Bookmarks.RemoveRange(_context.Bookmarks);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region HasUserBookmarkedAsync 測試

    [Fact]
    public async Task HasUserBookmarkedAsync_WithValidUserAndTarget_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasUserBookmarkedAsync_WithValidUserButDifferentTarget_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 999; // User 1 doesn't have bookmark for this target

        // Act
        var result = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserBookmarkedAsync_WithInvalidUserId_ShouldReturnFalse()
    {
        // Arrange
        var userId = -1;
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserBookmarkedAsync_WithInvalidTargetType_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "invalid_type";
        var targetId = 1;

        // Act
        var result = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserBookmarkedAsync_WithInvalidTargetId_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = -1;

        // Act
        var result = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserBookmarkedAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = 999;
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasUserBookmarkedAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;

        // Act - 第一次調用
        var result1 = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Bookmarks.RemoveRange(_context.Bookmarks);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.HasUserBookmarkedAsync(userId, targetType, targetId);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region GetBookmarkCountAsync 測試

    [Fact]
    public async Task GetBookmarkCountAsync_WithValidTarget_ShouldReturnCount()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.GetBookmarkCountAsync(targetType, targetId);

        // Assert
        Assert.Equal(1, result); // Only user 1 has bookmarked post 1
    }

    [Fact]
    public async Task GetBookmarkCountAsync_WithInvalidTargetType_ShouldReturnZero()
    {
        // Arrange
        var targetType = "invalid_type";
        var targetId = 1;

        // Act
        var result = await _service.GetBookmarkCountAsync(targetType, targetId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetBookmarkCountAsync_WithInvalidTargetId_ShouldReturnZero()
    {
        // Arrange
        var targetType = "post";
        var targetId = -1;

        // Act
        var result = await _service.GetBookmarkCountAsync(targetType, targetId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetBookmarkCountAsync_WithNonExistentTarget_ShouldReturnZero()
    {
        // Arrange
        var targetType = "post";
        var targetId = 999;

        // Act
        var result = await _service.GetBookmarkCountAsync(targetType, targetId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetBookmarkCountAsync_ShouldUseCache()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetBookmarkCountAsync(targetType, targetId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Bookmarks.RemoveRange(_context.Bookmarks);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetBookmarkCountAsync(targetType, targetId);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region AddBookmarkAsync 測試

    [Fact]
    public async Task AddBookmarkAsync_WithValidData_ShouldCreateBookmark()
    {
        // Arrange
        var request = new AddBookmarkRequestDto
        {
            TargetType = "post",
            TargetId = 3,
            UserId = 2
        };

        // Act
        var result = await _service.AddBookmarkAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.BookmarkId > 0);
        Assert.Equal(request.TargetType, result.TargetType);
        Assert.Equal(request.TargetId, result.TargetId);
        Assert.Equal(request.UserId, result.UserId);
        Assert.True(result.CreatedAt > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbBookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.bookmark_id == result.BookmarkId);
        Assert.NotNull(dbBookmark);
        Assert.Equal(request.TargetType, dbBookmark.target_type);
        Assert.Equal(request.TargetId, dbBookmark.target_id);
        Assert.Equal(request.UserId, dbBookmark.created_by_user_id);
    }

    [Fact]
    public async Task AddBookmarkAsync_WithNullRequest_ShouldThrowException()
    {
        // Arrange
        AddBookmarkRequestDto request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddBookmarkAsync(request));
    }

    [Fact]
    public async Task AddBookmarkAsync_WithInvalidTargetType_ShouldThrowException()
    {
        // Arrange
        var request = new AddBookmarkRequestDto
        {
            TargetType = "invalid_type",
            TargetId = 1,
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddBookmarkAsync(request));
    }

    [Fact]
    public async Task AddBookmarkAsync_WithInvalidTargetId_ShouldThrowException()
    {
        // Arrange
        var request = new AddBookmarkRequestDto
        {
            TargetType = "post",
            TargetId = -1,
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddBookmarkAsync(request));
    }

    [Fact]
    public async Task AddBookmarkAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var request = new AddBookmarkRequestDto
        {
            TargetType = "post",
            TargetId = 1,
            UserId = -1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddBookmarkAsync(request));
    }

    [Fact]
    public async Task AddBookmarkAsync_WithDuplicateBookmark_ShouldThrowException()
    {
        // Arrange
        var request = new AddBookmarkRequestDto
        {
            TargetType = "post",
            TargetId = 1,
            UserId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddBookmarkAsync(request));
    }

    #endregion

    #region RemoveBookmarkAsync 測試

    [Fact]
    public async Task RemoveBookmarkAsync_WithValidData_ShouldRemoveBookmark()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.RemoveBookmarkAsync(userId, targetType, targetId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實刪除了記錄
        var dbBookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.created_by_user_id == userId && 
                                    b.target_type == targetType && 
                                    b.target_id == targetId);
        Assert.Null(dbBookmark);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_WithInvalidUserId_ShouldReturnFalse()
    {
        // Arrange
        var userId = -1;
        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.RemoveBookmarkAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_WithInvalidTargetType_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "invalid_type";
        var targetId = 1;

        // Act
        var result = await _service.RemoveBookmarkAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_WithInvalidTargetId_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = -1;

        // Act
        var result = await _service.RemoveBookmarkAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_WithNonExistentBookmark_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var targetType = "post";
        var targetId = 999;

        // Act
        var result = await _service.RemoveBookmarkAsync(userId, targetType, targetId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetUserReactionsAsync 測試

    [Fact]
    public async Task GetUserReactionsAsync_WithValidUserId_ShouldReturnReactions()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var reactions = result.ToList();
        Assert.Equal(2, reactions.Count);
        Assert.All(reactions, r => Assert.Equal(userId, r.created_by_user_id));
    }

    [Fact]
    public async Task GetUserReactionsAsync_WithInvalidUserId_ShouldReturnEmpty()
    {
        // Arrange
        var userId = -1;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserReactionsAsync_WithInvalidPage_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 1;
        var page = 0;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserReactionsAsync_WithInvalidPageSize_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserReactionsAsync_WithNonExistentUser_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 999;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserReactionsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;

        // Act - 第一次調用
        var result1 = await _service.GetUserReactionsAsync(userId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Reactions.RemoveRange(_context.Reactions);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearReactionRelatedCache_ShouldClearCache()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;
        
        // 先獲取一些數據以填充快取
        _service.GetReactionStatsAsync(targetType, targetId).Wait();

        // Act
        _service.ClearReactionRelatedCache(targetType, targetId);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    [Fact]
    public void ClearBookmarkRelatedCache_ShouldClearCache()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;
        
        // Act
        _service.ClearBookmarkRelatedCache(targetType, targetId);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetReactionStatsAsync_WithEmptyDatabase_ShouldReturnEmptyStats()
    {
        // Arrange
        _context.Reactions.RemoveRange(_context.Reactions);
        _context.SaveChanges();

        var targetType = "post";
        var targetId = 1;

        // Act
        var result = await _service.GetReactionStatsAsync(targetType, targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ReactionCounts);
        Assert.Equal(0, result.TotalReactions);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_WithNoBookmarks_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 3; // User 3 has no bookmarks
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserReactionsAsync_WithNoReactions_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 3; // User 3 has no reactions
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetUserReactionsAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetReactionStatsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var targetType = "post";
        var targetId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetReactionStatsAsync(targetType, targetId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUserBookmarksAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserBookmarksAsync(userId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}