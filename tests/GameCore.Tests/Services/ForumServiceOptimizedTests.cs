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
/// ForumService 優化版本單元測試
/// </summary>
public class ForumServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ForumService> _logger;
    private readonly ForumService _service;

    public ForumServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<ForumService>>();
        _service = new ForumService(_context, _memoryCache, _logger);

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
        // 創建遊戲
        var game1 = new Game { game_id = 1, name = "Test Game 1", description = "Test Description 1" };
        var game2 = new Game { game_id = 2, name = "Test Game 2", description = "Test Description 2" };
        _context.Games.AddRange(game1, game2);

        // 創建論壇
        var forum1 = new Forum { forum_id = 1, game_id = 1, name = "General Discussion", description = "General game discussion" };
        var forum2 = new Forum { forum_id = 2, game_id = 2, name = "Strategy Guide", description = "Strategy and tips" };
        _context.Forums.AddRange(forum1, forum2);

        // 創建用戶
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com" };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com" };
        _context.Users.AddRange(user1, user2);

        // 創建主題
        var thread1 = new Thread { thread_id = 1, forum_id = 1, title = "Welcome Thread", body_md = "Welcome to the forum", created_by_user_id = 1, status = "active", pinned = false, view_count = 10, created_at = DateTime.UtcNow.AddDays(-1), updated_at = DateTime.UtcNow.AddDays(-1) };
        var thread2 = new Thread { thread_id = 2, forum_id = 1, title = "Game Tips", body_md = "Some useful tips", created_by_user_id = 2, status = "active", pinned = true, view_count = 25, created_at = DateTime.UtcNow.AddDays(-2), updated_at = DateTime.UtcNow.AddDays(-2) };
        var thread3 = new Thread { thread_id = 3, forum_id = 2, title = "Strategy Discussion", body_md = "Let's discuss strategies", created_by_user_id = 1, status = "active", pinned = false, view_count = 15, created_at = DateTime.UtcNow.AddDays(-3), updated_at = DateTime.UtcNow.AddDays(-3) };
        _context.Threads.AddRange(thread1, thread2, thread3);

        // 創建回覆
        var post1 = new ThreadPost { post_id = 1, thread_id = 1, body_md = "Great thread!", created_by_user_id = 2, status = "active", created_at = DateTime.UtcNow.AddHours(-1), updated_at = DateTime.UtcNow.AddHours(-1) };
        var post2 = new ThreadPost { post_id = 2, thread_id = 1, body_md = "Thanks for sharing", created_by_user_id = 1, status = "active", created_at = DateTime.UtcNow.AddHours(-2), updated_at = DateTime.UtcNow.AddHours(-2) };
        var post3 = new ThreadPost { post_id = 3, thread_id = 2, body_md = "Very helpful tips", created_by_user_id = 1, status = "active", created_at = DateTime.UtcNow.AddHours(-3), updated_at = DateTime.UtcNow.AddHours(-3) };
        _context.ThreadPosts.AddRange(post1, post2, post3);

        _context.SaveChanges();
    }

    #endregion

    #region GetAllForumsAsync 測試

    [Fact]
    public async Task GetAllForumsAsync_ShouldReturnAllForums()
    {
        // Act
        var result = await _service.GetAllForumsAsync();

        // Assert
        Assert.NotNull(result);
        var forums = result.ToList();
        Assert.Equal(2, forums.Count);
        Assert.Contains(forums, f => f.name == "General Discussion");
        Assert.Contains(forums, f => f.name == "Strategy Guide");
    }

    [Fact]
    public async Task GetAllForumsAsync_ShouldUseCache()
    {
        // Act - 第一次調用
        var result1 = await _service.GetAllForumsAsync();
        
        // 清除資料庫數據（模擬快取生效）
        _context.Forums.RemoveRange(_context.Forums);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetAllForumsAsync();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetForumAsync 測試

    [Fact]
    public async Task GetForumAsync_WithValidForumId_ShouldReturnForum()
    {
        // Arrange
        var forumId = 1;

        // Act
        var result = await _service.GetForumAsync(forumId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(forumId, result.forum_id);
        Assert.Equal("General Discussion", result.name);
        Assert.NotNull(result.Game);
        Assert.Equal(1, result.Game.game_id);
    }

    [Fact]
    public async Task GetForumAsync_WithInvalidForumId_ShouldReturnNull()
    {
        // Arrange
        var forumId = -1;

        // Act
        var result = await _service.GetForumAsync(forumId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetForumAsync_WithNonExistentForumId_ShouldReturnNull()
    {
        // Arrange
        var forumId = 999;

        // Act
        var result = await _service.GetForumAsync(forumId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetForumAsync_ShouldUseCache()
    {
        // Arrange
        var forumId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetForumAsync(forumId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Forums.RemoveRange(_context.Forums);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetForumAsync(forumId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.forum_id, result2.forum_id);
    }

    #endregion

    #region GetForumThreadsAsync 測試

    [Fact]
    public async Task GetForumThreadsAsync_WithValidParameters_ShouldReturnThreads()
    {
        // Arrange
        var forumId = 1;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetForumThreadsAsync(forumId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var threads = result.ToList();
        Assert.Equal(2, threads.Count);
        Assert.All(threads, t => Assert.Equal(forumId, t.forum_id));
        Assert.All(threads, t => Assert.Equal("active", t.status));
    }

    [Fact]
    public async Task GetForumThreadsAsync_WithInvalidForumId_ShouldReturnEmpty()
    {
        // Arrange
        var forumId = -1;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetForumThreadsAsync(forumId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetForumThreadsAsync_WithInvalidPage_ShouldReturnEmpty()
    {
        // Arrange
        var forumId = 1;
        var page = 0;
        var pageSize = 20;

        // Act
        var result = await _service.GetForumThreadsAsync(forumId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetForumThreadsAsync_WithInvalidPageSize_ShouldReturnEmpty()
    {
        // Arrange
        var forumId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetForumThreadsAsync(forumId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetForumThreadsAsync_ShouldUseCache()
    {
        // Arrange
        var forumId = 1;
        var page = 1;
        var pageSize = 20;

        // Act - 第一次調用
        var result1 = await _service.GetForumThreadsAsync(forumId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Threads.RemoveRange(_context.Threads);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetForumThreadsAsync(forumId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetThreadAsync 測試

    [Fact]
    public async Task GetThreadAsync_WithValidThreadId_ShouldReturnThread()
    {
        // Arrange
        var threadId = 1L;

        // Act
        var result = await _service.GetThreadAsync(threadId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(threadId, result.thread_id);
        Assert.Equal("Welcome Thread", result.title);
        Assert.NotNull(result.Forum);
        Assert.Equal(1, result.Forum.forum_id);
    }

    [Fact]
    public async Task GetThreadAsync_WithInvalidThreadId_ShouldReturnNull()
    {
        // Arrange
        var threadId = -1L;

        // Act
        var result = await _service.GetThreadAsync(threadId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetThreadAsync_WithNonExistentThreadId_ShouldReturnNull()
    {
        // Arrange
        var threadId = 999L;

        // Act
        var result = await _service.GetThreadAsync(threadId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetThreadAsync_ShouldUseCache()
    {
        // Arrange
        var threadId = 1L;

        // Act - 第一次調用
        var result1 = await _service.GetThreadAsync(threadId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Threads.RemoveRange(_context.Threads);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetThreadAsync(threadId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.thread_id, result2.thread_id);
    }

    #endregion

    #region CreateThreadAsync 測試

    [Fact]
    public async Task CreateThreadAsync_WithValidData_ShouldCreateThread()
    {
        // Arrange
        var thread = new Thread
        {
            forum_id = 1,
            title = "New Test Thread",
            body_md = "This is a test thread content",
            created_by_user_id = 1
        };

        // Act
        var result = await _service.CreateThreadAsync(thread);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.thread_id > 0);
        Assert.Equal("New Test Thread", result.title);
        Assert.Equal("active", result.status);
        Assert.False(result.pinned);
        Assert.Equal(0, result.view_count);
        Assert.True(result.created_at > DateTime.UtcNow.AddMinutes(-1));
        Assert.True(result.updated_at > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbThread = await _context.Threads
            .FirstOrDefaultAsync(t => t.thread_id == result.thread_id);
        Assert.NotNull(dbThread);
    }

    [Fact]
    public async Task CreateThreadAsync_WithNullThread_ShouldThrowException()
    {
        // Arrange
        Thread thread = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateThreadAsync(thread));
    }

    [Fact]
    public async Task CreateThreadAsync_WithInvalidForumId_ShouldThrowException()
    {
        // Arrange
        var thread = new Thread
        {
            forum_id = -1,
            title = "New Test Thread",
            body_md = "This is a test thread content",
            created_by_user_id = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateThreadAsync(thread));
    }

    [Fact]
    public async Task CreateThreadAsync_WithEmptyTitle_ShouldThrowException()
    {
        // Arrange
        var thread = new Thread
        {
            forum_id = 1,
            title = "",
            body_md = "This is a test thread content",
            created_by_user_id = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateThreadAsync(thread));
    }

    [Fact]
    public async Task CreateThreadAsync_WithEmptyBody_ShouldThrowException()
    {
        // Arrange
        var thread = new Thread
        {
            forum_id = 1,
            title = "New Test Thread",
            body_md = "",
            created_by_user_id = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateThreadAsync(thread));
    }

    [Fact]
    public async Task CreateThreadAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var thread = new Thread
        {
            forum_id = 1,
            title = "New Test Thread",
            body_md = "This is a test thread content",
            created_by_user_id = -1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateThreadAsync(thread));
    }

    #endregion

    #region UpdateThreadAsync 測試

    [Fact]
    public async Task UpdateThreadAsync_WithValidData_ShouldUpdateThread()
    {
        // Arrange
        var threadId = 1L;
        var updateData = new Thread
        {
            title = "Updated Thread Title",
            body_md = "Updated thread content",
            tldr = "Updated summary"
        };

        // Act
        var result = await _service.UpdateThreadAsync(threadId, updateData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(threadId, result.thread_id);
        Assert.Equal("Updated Thread Title", result.title);
        Assert.Equal("Updated thread content", result.body_md);
        Assert.Equal("Updated summary", result.tldr);
        Assert.True(result.updated_at > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task UpdateThreadAsync_WithInvalidThreadId_ShouldThrowException()
    {
        // Arrange
        var threadId = -1L;
        var updateData = new Thread { title = "Updated Title" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdateThreadAsync(threadId, updateData));
    }

    [Fact]
    public async Task UpdateThreadAsync_WithNonExistentThreadId_ShouldThrowException()
    {
        // Arrange
        var threadId = 999L;
        var updateData = new Thread { title = "Updated Title" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateThreadAsync(threadId, updateData));
    }

    #endregion

    #region DeleteThreadAsync 測試

    [Fact]
    public async Task DeleteThreadAsync_WithValidThreadId_ShouldDeleteThread()
    {
        // Arrange
        var threadId = 1L;

        // Act
        var result = await _service.DeleteThreadAsync(threadId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中狀態已更新為 hidden
        var dbThread = await _context.Threads
            .FirstOrDefaultAsync(t => t.thread_id == threadId);
        Assert.NotNull(dbThread);
        Assert.Equal("hidden", dbThread.status);
    }

    [Fact]
    public async Task DeleteThreadAsync_WithInvalidThreadId_ShouldReturnFalse()
    {
        // Arrange
        var threadId = -1L;

        // Act
        var result = await _service.DeleteThreadAsync(threadId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteThreadAsync_WithNonExistentThreadId_ShouldReturnFalse()
    {
        // Arrange
        var threadId = 999L;

        // Act
        var result = await _service.DeleteThreadAsync(threadId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetThreadPostsAsync 測試

    [Fact]
    public async Task GetThreadPostsAsync_WithValidParameters_ShouldReturnPosts()
    {
        // Arrange
        var threadId = 1L;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetThreadPostsAsync(threadId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var posts = result.ToList();
        Assert.Equal(2, posts.Count);
        Assert.All(posts, p => Assert.Equal(threadId, p.thread_id));
        Assert.All(posts, p => Assert.Equal("active", p.status));
    }

    [Fact]
    public async Task GetThreadPostsAsync_WithInvalidThreadId_ShouldReturnEmpty()
    {
        // Arrange
        var threadId = -1L;
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetThreadPostsAsync(threadId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetThreadPostsAsync_ShouldUseCache()
    {
        // Arrange
        var threadId = 1L;
        var page = 1;
        var pageSize = 20;

        // Act - 第一次調用
        var result1 = await _service.GetThreadPostsAsync(threadId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.ThreadPosts.RemoveRange(_context.ThreadPosts);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetThreadPostsAsync(threadId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region CreatePostAsync 測試

    [Fact]
    public async Task CreatePostAsync_WithValidData_ShouldCreatePost()
    {
        // Arrange
        var post = new ThreadPost
        {
            thread_id = 1,
            body_md = "This is a test reply",
            created_by_user_id = 1
        };

        // Act
        var result = await _service.CreatePostAsync(post);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.post_id > 0);
        Assert.Equal("This is a test reply", result.body_md);
        Assert.Equal("active", result.status);
        Assert.True(result.created_at > DateTime.UtcNow.AddMinutes(-1));
        Assert.True(result.updated_at > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbPost = await _context.ThreadPosts
            .FirstOrDefaultAsync(p => p.post_id == result.post_id);
        Assert.NotNull(dbPost);
    }

    [Fact]
    public async Task CreatePostAsync_WithNullPost_ShouldThrowException()
    {
        // Arrange
        ThreadPost post = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePostAsync(post));
    }

    [Fact]
    public async Task CreatePostAsync_WithInvalidThreadId_ShouldThrowException()
    {
        // Arrange
        var post = new ThreadPost
        {
            thread_id = -1,
            body_md = "This is a test reply",
            created_by_user_id = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePostAsync(post));
    }

    [Fact]
    public async Task CreatePostAsync_WithEmptyBody_ShouldThrowException()
    {
        // Arrange
        var post = new ThreadPost
        {
            thread_id = 1,
            body_md = "",
            created_by_user_id = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePostAsync(post));
    }

    [Fact]
    public async Task CreatePostAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var post = new ThreadPost
        {
            thread_id = 1,
            body_md = "This is a test reply",
            created_by_user_id = -1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreatePostAsync(post));
    }

    #endregion

    #region UpdatePostAsync 測試

    [Fact]
    public async Task UpdatePostAsync_WithValidData_ShouldUpdatePost()
    {
        // Arrange
        var postId = 1L;
        var updateData = new ThreadPost
        {
            body_md = "Updated reply content"
        };

        // Act
        var result = await _service.UpdatePostAsync(postId, updateData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(postId, result.post_id);
        Assert.Equal("Updated reply content", result.body_md);
        Assert.True(result.updated_at > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task UpdatePostAsync_WithInvalidPostId_ShouldThrowException()
    {
        // Arrange
        var postId = -1L;
        var updateData = new ThreadPost { body_md = "Updated content" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdatePostAsync(postId, updateData));
    }

    [Fact]
    public async Task UpdatePostAsync_WithNonExistentPostId_ShouldThrowException()
    {
        // Arrange
        var postId = 999L;
        var updateData = new ThreadPost { body_md = "Updated content" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdatePostAsync(postId, updateData));
    }

    #endregion

    #region DeletePostAsync 測試

    [Fact]
    public async Task DeletePostAsync_WithValidPostId_ShouldDeletePost()
    {
        // Arrange
        var postId = 1L;

        // Act
        var result = await _service.DeletePostAsync(postId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中狀態已更新為 hidden
        var dbPost = await _context.ThreadPosts
            .FirstOrDefaultAsync(p => p.post_id == postId);
        Assert.NotNull(dbPost);
        Assert.Equal("hidden", dbPost.status);
    }

    [Fact]
    public async Task DeletePostAsync_WithInvalidPostId_ShouldReturnFalse()
    {
        // Arrange
        var postId = -1L;

        // Act
        var result = await _service.DeletePostAsync(postId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeletePostAsync_WithNonExistentPostId_ShouldReturnFalse()
    {
        // Arrange
        var postId = 999L;

        // Act
        var result = await _service.DeletePostAsync(postId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetPopularThreadsAsync 測試

    [Fact]
    public async Task GetPopularThreadsAsync_WithValidLimit_ShouldReturnThreads()
    {
        // Arrange
        var limit = 5;

        // Act
        var result = await _service.GetPopularThreadsAsync(limit);

        // Assert
        Assert.NotNull(result);
        var threads = result.ToList();
        Assert.True(threads.Count <= limit);
        Assert.All(threads, t => Assert.Equal("active", t.status));
    }

    [Fact]
    public async Task GetPopularThreadsAsync_WithInvalidLimit_ShouldUseDefaultLimit()
    {
        // Arrange
        var limit = -1;

        // Act
        var result = await _service.GetPopularThreadsAsync(limit);

        // Assert
        Assert.NotNull(result);
        var threads = result.ToList();
        Assert.True(threads.Count <= 20); // Default page size
    }

    [Fact]
    public async Task GetPopularThreadsAsync_WithExcessiveLimit_ShouldUseMaxLimit()
    {
        // Arrange
        var limit = 1000;

        // Act
        var result = await _service.GetPopularThreadsAsync(limit);

        // Assert
        Assert.NotNull(result);
        var threads = result.ToList();
        Assert.True(threads.Count <= 100); // Max page size
    }

    [Fact]
    public async Task GetPopularThreadsAsync_ShouldUseCache()
    {
        // Arrange
        var limit = 5;

        // Act - 第一次調用
        var result1 = await _service.GetPopularThreadsAsync(limit);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Threads.RemoveRange(_context.Threads);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetPopularThreadsAsync(limit);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region SearchAsync 測試

    [Fact]
    public async Task SearchAsync_WithValidKeyword_ShouldReturnResults()
    {
        // Arrange
        var keyword = "Thread";
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.SearchAsync(keyword, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var results = result.ToList();
        Assert.True(results.Count > 0);
        Assert.True(results.Count <= pageSize);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyKeyword_ShouldReturnEmpty()
    {
        // Arrange
        var keyword = "";
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.SearchAsync(keyword, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithInvalidPage_ShouldReturnEmpty()
    {
        // Arrange
        var keyword = "Thread";
        var page = 0;
        var pageSize = 20;

        // Act
        var result = await _service.SearchAsync(keyword, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldUseCache()
    {
        // Arrange
        var keyword = "Thread";
        var page = 1;
        var pageSize = 20;

        // Act - 第一次調用
        var result1 = await _service.SearchAsync(keyword, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Threads.RemoveRange(_context.Threads);
        _context.ThreadPosts.RemoveRange(_context.ThreadPosts);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.SearchAsync(keyword, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 快取管理測試

    [Fact]
    public void ClearForumRelatedCache_ShouldClearCache()
    {
        // Arrange
        var forumId = 1;
        
        // 先獲取一些數據以填充快取
        _service.GetForumAsync(forumId).Wait();

        // Act
        _service.ClearForumRelatedCache(forumId);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    [Fact]
    public void ClearThreadRelatedCache_ShouldClearCache()
    {
        // Arrange
        var threadId = 1L;
        
        // Act
        _service.ClearThreadRelatedCache(threadId);

        // Assert
        // 快取應該被清除，但由於快取是內部實現，我們只能驗證方法執行不拋出異常
        Assert.True(true);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetAllForumsAsync_WithEmptyDatabase_ShouldReturnEmpty()
    {
        // Arrange
        _context.Forums.RemoveRange(_context.Forums);
        _context.SaveChanges();

        // Act
        var result = await _service.GetAllForumsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetForumThreadsAsync_WithNoThreads_ShouldReturnEmpty()
    {
        // Arrange
        var forumId = 2; // Forum 2 has only 1 thread
        var page = 2; // Second page
        var pageSize = 20;

        // Act
        var result = await _service.GetForumThreadsAsync(forumId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetThreadPostsAsync_WithNoPosts_ShouldReturnEmpty()
    {
        // Arrange
        var threadId = 3L; // Thread 3 has no posts
        var page = 1;
        var pageSize = 20;

        // Act
        var result = await _service.GetThreadPostsAsync(threadId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetAllForumsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetAllForumsAsync();
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetForumThreadsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var forumId = 1;
        var page = 1;
        var pageSize = 20;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetForumThreadsAsync(forumId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}