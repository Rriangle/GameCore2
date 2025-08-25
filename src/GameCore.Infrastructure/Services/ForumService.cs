using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 論壇服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class ForumService : IForumService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ForumService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 15;
    private const string AllForumsCacheKey = "Forums_All";
    private const string ForumCacheKey = "Forum_{0}";
    private const string ForumThreadsCacheKey = "ForumThreads_{0}_{1}_{2}";
    private const string ThreadCacheKey = "Thread_{0}";
    private const string ThreadPostsCacheKey = "ThreadPosts_{0}_{1}_{2}";
    private const string PopularThreadsCacheKey = "PopularThreads_{0}";
    private const string SearchCacheKey = "Search_{0}_{1}_{2}";

    public ForumService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<ForumService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 獲取所有論壇 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Forum>> GetAllForumsAsync()
    {
        _logger.LogInformation("開始獲取所有論壇");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(AllForumsCacheKey, out IEnumerable<Forum> cachedForums))
            {
                _logger.LogDebug("從快取獲取所有論壇，數量: {Count}", cachedForums.Count());
                return cachedForums;
            }

            // 從資料庫獲取
            var forums = await _context.Forums
                .Include(f => f.Game)
                .OrderBy(f => f.name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(AllForumsCacheKey, forums, cacheOptions);

            _logger.LogInformation("成功獲取所有論壇，數量: {Count}", forums.Count);
            return forums;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取論壇列表時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取論壇詳情 - 優化版本（含快取）
    /// </summary>
    public async Task<Forum?> GetForumAsync(int forumId)
    {
        _logger.LogInformation("開始獲取論壇詳情，論壇ID: {ForumId}", forumId);

        try
        {
            // 輸入驗證
            if (forumId <= 0)
            {
                _logger.LogWarning("無效的論壇ID: {ForumId}", forumId);
                return null;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ForumCacheKey, forumId);
            if (_memoryCache.TryGetValue(cacheKey, out Forum cachedForum))
            {
                _logger.LogDebug("從快取獲取論壇詳情，論壇ID: {ForumId}", forumId);
                return cachedForum;
            }

            // 從資料庫獲取
            var forum = await _context.Forums
                .Include(f => f.Game)
                .Include(f => f.Threads.OrderByDescending(t => t.updated_at).Take(5))
                .FirstOrDefaultAsync(f => f.forum_id == forumId);

            if (forum != null)
            {
                // 存入快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, forum, cacheOptions);

                _logger.LogInformation("成功獲取論壇詳情，論壇ID: {ForumId}, 名稱: {Name}", forumId, forum.name);
            }
            else
            {
                _logger.LogWarning("論壇不存在，論壇ID: {ForumId}", forumId);
            }

            return forum;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取論壇 {ForumId} 詳情時發生錯誤", forumId);
            throw;
        }
    }

    /// <summary>
    /// 獲取論壇主題列表 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Thread>> GetForumThreadsAsync(int forumId, int page = 1, int pageSize = 20)
    {
        _logger.LogInformation("開始獲取論壇主題列表，論壇ID: {ForumId}, 頁碼: {Page}, 頁面大小: {PageSize}", 
            forumId, page, pageSize);

        try
        {
            // 輸入驗證
            var validationResult = ValidatePaginationParameters(page, pageSize);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("分頁參數驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<Thread>();
            }

            if (forumId <= 0)
            {
                _logger.LogWarning("無效的論壇ID: {ForumId}", forumId);
                return Enumerable.Empty<Thread>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ForumThreadsCacheKey, forumId, page, pageSize);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Thread> cachedThreads))
            {
                _logger.LogDebug("從快取獲取論壇主題列表，論壇ID: {ForumId}", forumId);
                return cachedThreads;
            }

            // 從資料庫獲取
            var threads = await _context.Threads
                .Include(t => t.CreatedByUser)
                .Include(t => t.Forum)
                .Where(t => t.forum_id == forumId && t.status == "active")
                .OrderByDescending(t => t.pinned)
                .ThenByDescending(t => t.updated_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, threads, cacheOptions);

            _logger.LogInformation("成功獲取論壇主題列表，論壇ID: {ForumId}, 數量: {Count}", forumId, threads.Count);
            return threads;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取論壇 {ForumId} 主題列表時發生錯誤", forumId);
            throw;
        }
    }

    /// <summary>
    /// 獲取主題詳情 - 優化版本（含快取）
    /// </summary>
    public async Task<Thread?> GetThreadAsync(long threadId)
    {
        _logger.LogInformation("開始獲取主題詳情，主題ID: {ThreadId}", threadId);

        try
        {
            // 輸入驗證
            if (threadId <= 0)
            {
                _logger.LogWarning("無效的主題ID: {ThreadId}", threadId);
                return null;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ThreadCacheKey, threadId);
            if (_memoryCache.TryGetValue(cacheKey, out Thread cachedThread))
            {
                _logger.LogDebug("從快取獲取主題詳情，主題ID: {ThreadId}", threadId);
                return cachedThread;
            }

            // 從資料庫獲取
            var thread = await _context.Threads
                .Include(t => t.CreatedByUser)
                .Include(t => t.Forum)
                .Include(t => t.ThreadPosts.OrderBy(tp => tp.created_at))
                .FirstOrDefaultAsync(t => t.thread_id == threadId && t.status == "active");

            if (thread != null)
            {
                // 存入快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, thread, cacheOptions);

                _logger.LogInformation("成功獲取主題詳情，主題ID: {ThreadId}, 標題: {Title}", threadId, thread.title);
            }
            else
            {
                _logger.LogWarning("主題不存在，主題ID: {ThreadId}", threadId);
            }

            return thread;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取主題 {ThreadId} 詳情時發生錯誤", threadId);
            throw;
        }
    }

    /// <summary>
    /// 創建主題 - 優化版本
    /// </summary>
    public async Task<Thread> CreateThreadAsync(Thread thread)
    {
        _logger.LogInformation("開始創建主題，論壇ID: {ForumId}, 標題: {Title}", thread.forum_id, thread.title);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreateThreadRequest(thread);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("創建主題請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 設置默認值
            thread.created_at = DateTime.UtcNow;
            thread.updated_at = DateTime.UtcNow;
            thread.status = "active";
            thread.pinned = false;
            thread.view_count = 0;

            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearForumRelatedCache(thread.forum_id);

            _logger.LogInformation("成功創建主題，主題ID: {ThreadId}, 標題: {Title}", thread.thread_id, thread.title);
            return thread;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建主題時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 更新主題 - 優化版本
    /// </summary>
    public async Task<Thread> UpdateThreadAsync(long threadId, Thread updateData)
    {
        _logger.LogInformation("開始更新主題，主題ID: {ThreadId}", threadId);

        try
        {
            // 輸入驗證
            if (threadId <= 0)
            {
                _logger.LogWarning("無效的主題ID: {ThreadId}", threadId);
                throw new ArgumentException("無效的主題ID");
            }

            var existingThread = await _context.Threads
                .FirstOrDefaultAsync(t => t.thread_id == threadId && t.status == "active");

            if (existingThread == null)
            {
                _logger.LogWarning("主題不存在，主題ID: {ThreadId}", threadId);
                throw new InvalidOperationException($"主題 {threadId} 不存在");
            }

            // 更新允許的欄位
            UpdateThreadFields(existingThread, updateData);
            existingThread.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearThreadRelatedCache(threadId);
            ClearForumRelatedCache(existingThread.forum_id);

            _logger.LogInformation("成功更新主題，主題ID: {ThreadId}", threadId);
            return existingThread;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新主題 {ThreadId} 時發生錯誤", threadId);
            throw;
        }
    }

    /// <summary>
    /// 刪除主題 - 優化版本
    /// </summary>
    public async Task<bool> DeleteThreadAsync(long threadId)
    {
        _logger.LogInformation("開始刪除主題，主題ID: {ThreadId}", threadId);

        try
        {
            // 輸入驗證
            if (threadId <= 0)
            {
                _logger.LogWarning("無效的主題ID: {ThreadId}", threadId);
                return false;
            }

            var thread = await _context.Threads
                .FirstOrDefaultAsync(t => t.thread_id == threadId && t.status == "active");

            if (thread == null)
            {
                _logger.LogWarning("主題不存在，主題ID: {ThreadId}", threadId);
                return false;
            }

            // 軟刪除：將狀態設為 hidden
            thread.status = "hidden";
            thread.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearThreadRelatedCache(threadId);
            ClearForumRelatedCache(thread.forum_id);

            _logger.LogInformation("成功刪除主題，主題ID: {ThreadId}", threadId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除主題 {ThreadId} 時發生錯誤", threadId);
            throw;
        }
    }

    /// <summary>
    /// 獲取主題回覆 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<ThreadPost>> GetThreadPostsAsync(long threadId, int page = 1, int pageSize = 20)
    {
        _logger.LogInformation("開始獲取主題回覆，主題ID: {ThreadId}, 頁碼: {Page}, 頁面大小: {PageSize}", 
            threadId, page, pageSize);

        try
        {
            // 輸入驗證
            var validationResult = ValidatePaginationParameters(page, pageSize);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("分頁參數驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<ThreadPost>();
            }

            if (threadId <= 0)
            {
                _logger.LogWarning("無效的主題ID: {ThreadId}", threadId);
                return Enumerable.Empty<ThreadPost>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ThreadPostsCacheKey, threadId, page, pageSize);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<ThreadPost> cachedPosts))
            {
                _logger.LogDebug("從快取獲取主題回覆，主題ID: {ThreadId}", threadId);
                return cachedPosts;
            }

            // 從資料庫獲取
            var posts = await _context.ThreadPosts
                .Include(tp => tp.CreatedByUser)
                .Where(tp => tp.thread_id == threadId && tp.status == "active")
                .OrderBy(tp => tp.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, posts, cacheOptions);

            _logger.LogInformation("成功獲取主題回覆，主題ID: {ThreadId}, 數量: {Count}", threadId, posts.Count);
            return posts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取主題 {ThreadId} 回覆時發生錯誤", threadId);
            throw;
        }
    }

    /// <summary>
    /// 創建回覆 - 優化版本
    /// </summary>
    public async Task<ThreadPost> CreatePostAsync(ThreadPost post)
    {
        _logger.LogInformation("開始創建回覆，主題ID: {ThreadId}", post.thread_id);

        try
        {
            // 輸入驗證
            var validationResult = ValidateCreatePostRequest(post);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("創建回覆請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 設置默認值
            post.created_at = DateTime.UtcNow;
            post.updated_at = DateTime.UtcNow;
            post.status = "active";

            _context.ThreadPosts.Add(post);
            await _context.SaveChangesAsync();

            // 更新主題的更新時間
            var thread = await _context.Threads.FindAsync(post.thread_id);
            if (thread != null)
            {
                thread.updated_at = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            // 清除相關快取
            ClearThreadRelatedCache(post.thread_id);
            ClearForumRelatedCache(thread?.forum_id ?? 0);

            _logger.LogInformation("成功創建回覆，回覆ID: {PostId}, 主題ID: {ThreadId}", post.post_id, post.thread_id);
            return post;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建回覆時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 更新回覆 - 優化版本
    /// </summary>
    public async Task<ThreadPost> UpdatePostAsync(long postId, ThreadPost updateData)
    {
        _logger.LogInformation("開始更新回覆，回覆ID: {PostId}", postId);

        try
        {
            // 輸入驗證
            if (postId <= 0)
            {
                _logger.LogWarning("無效的回覆ID: {PostId}", postId);
                throw new ArgumentException("無效的回覆ID");
            }

            var existingPost = await _context.ThreadPosts
                .FirstOrDefaultAsync(tp => tp.post_id == postId && tp.status == "active");

            if (existingPost == null)
            {
                _logger.LogWarning("回覆不存在，回覆ID: {PostId}", postId);
                throw new InvalidOperationException($"回覆 {postId} 不存在");
            }

            // 更新允許的欄位
            UpdatePostFields(existingPost, updateData);
            existingPost.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearThreadRelatedCache(existingPost.thread_id);

            _logger.LogInformation("成功更新回覆，回覆ID: {PostId}", postId);
            return existingPost;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新回覆 {PostId} 時發生錯誤", postId);
            throw;
        }
    }

    /// <summary>
    /// 刪除回覆 - 優化版本
    /// </summary>
    public async Task<bool> DeletePostAsync(long postId)
    {
        _logger.LogInformation("開始刪除回覆，回覆ID: {PostId}", postId);

        try
        {
            // 輸入驗證
            if (postId <= 0)
            {
                _logger.LogWarning("無效的回覆ID: {PostId}", postId);
                return false;
            }

            var post = await _context.ThreadPosts
                .FirstOrDefaultAsync(tp => tp.post_id == postId && tp.status == "active");

            if (post == null)
            {
                _logger.LogWarning("回覆不存在，回覆ID: {PostId}", postId);
                return false;
            }

            // 軟刪除：將狀態設為 hidden
            post.status = "hidden";
            post.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearThreadRelatedCache(post.thread_id);

            _logger.LogInformation("成功刪除回覆，回覆ID: {PostId}", postId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除回覆 {PostId} 時發生錯誤", postId);
            throw;
        }
    }

    /// <summary>
    /// 獲取熱門主題 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Thread>> GetPopularThreadsAsync(int limit = 10)
    {
        _logger.LogInformation("開始獲取熱門主題，限制數量: {Limit}", limit);

        try
        {
            // 輸入驗證
            if (limit <= 0 || limit > MaxPageSize)
            {
                _logger.LogWarning("無效的限制數量: {Limit}", limit);
                limit = DefaultPageSize;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(PopularThreadsCacheKey, limit);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Thread> cachedThreads))
            {
                _logger.LogDebug("從快取獲取熱門主題，限制數量: {Limit}", limit);
                return cachedThreads;
            }

            // 從資料庫獲取（基於回覆數和瀏覽數排序）
            var popularThreads = await _context.Threads
                .Include(t => t.CreatedByUser)
                .Include(t => t.Forum)
                .Where(t => t.status == "active")
                .OrderByDescending(t => t.pinned)
                .ThenByDescending(t => t.ThreadPosts.Count(tp => tp.status == "active"))
                .ThenByDescending(t => t.view_count)
                .ThenByDescending(t => t.updated_at)
                .Take(limit)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, popularThreads, cacheOptions);

            _logger.LogInformation("成功獲取熱門主題，數量: {Count}", popularThreads.Count);
            return popularThreads;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取熱門主題時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 搜索主題和回覆 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<object>> SearchAsync(string keyword, int page = 1, int pageSize = 20)
    {
        _logger.LogInformation("開始搜索，關鍵字: {Keyword}, 頁碼: {Page}, 頁面大小: {PageSize}", 
            keyword, page, pageSize);

        try
        {
            // 輸入驗證
            if (string.IsNullOrWhiteSpace(keyword))
            {
                _logger.LogWarning("搜索關鍵字不能為空");
                return Enumerable.Empty<object>();
            }

            var validationResult = ValidatePaginationParameters(page, pageSize);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("分頁參數驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<object>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(SearchCacheKey, keyword, page, pageSize);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<object> cachedResults))
            {
                _logger.LogDebug("從快取獲取搜索結果，關鍵字: {Keyword}", keyword);
                return cachedResults;
            }

            // 從資料庫搜索
            var searchResults = new List<object>();

            // 搜索主題
            var threads = await _context.Threads
                .Include(t => t.CreatedByUser)
                .Include(t => t.Forum)
                .Where(t => t.status == "active" && 
                           (t.title.Contains(keyword) || t.body_md.Contains(keyword)))
                .OrderByDescending(t => t.updated_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize / 2)
                .AsNoTracking()
                .ToListAsync();

            searchResults.AddRange(threads);

            // 搜索回覆
            var posts = await _context.ThreadPosts
                .Include(tp => tp.CreatedByUser)
                .Include(tp => tp.Thread)
                .Where(tp => tp.status == "active" && 
                            (tp.body_md.Contains(keyword)))
                .OrderByDescending(tp => tp.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize / 2)
                .AsNoTracking()
                .ToListAsync();

            searchResults.AddRange(posts);

            // 按相關性排序
            var sortedResults = searchResults
                .OrderByDescending(r => r is Thread ? ((Thread)r).updated_at : ((ThreadPost)r).created_at)
                .Take(pageSize)
                .ToList();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, sortedResults, cacheOptions);

            _logger.LogInformation("成功搜索，關鍵字: {Keyword}, 結果數量: {Count}", keyword, sortedResults.Count);
            return sortedResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 清除論壇相關快取 - 新增方法
    /// </summary>
    public void ClearForumRelatedCache(int forumId)
    {
        try
        {
            _memoryCache.Remove(AllForumsCacheKey);
            
            // 清除特定論壇的快取
            var keysToRemove = new List<string>();
            foreach (var key in _memoryCache.GetKeys())
            {
                if (key is string stringKey && stringKey.Contains($"Forum_{forumId}_"))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            _logger.LogDebug("已清除論壇 {ForumId} 的相關快取", forumId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除論壇相關快取時發生錯誤");
        }
    }

    /// <summary>
    /// 清除主題相關快取 - 新增方法
    /// </summary>
    public void ClearThreadRelatedCache(long threadId)
    {
        try
        {
            // 清除特定主題的快取
            var keysToRemove = new List<string>();
            foreach (var key in _memoryCache.GetKeys())
            {
                if (key is string stringKey && stringKey.Contains($"Thread_{threadId}_"))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            _logger.LogDebug("已清除主題 {ThreadId} 的相關快取", threadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除主題相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證分頁參數 - 新增方法
    /// </summary>
    private ValidationResult ValidatePaginationParameters(int page, int pageSize)
    {
        var result = new ValidationResult();

        if (page <= 0)
            result.AddError("頁碼必須大於0");

        if (pageSize <= 0)
            result.AddError("頁面大小必須大於0");

        if (pageSize > MaxPageSize)
            result.AddError($"頁面大小不能超過 {MaxPageSize}");

        return result;
    }

    /// <summary>
    /// 驗證創建主題請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreateThreadRequest(Thread thread)
    {
        var result = new ValidationResult();

        if (thread == null)
            result.AddError("主題數據不能為空");

        if (thread.forum_id <= 0)
            result.AddError("無效的論壇ID");

        if (string.IsNullOrWhiteSpace(thread.title))
            result.AddError("主題標題不能為空");

        if (string.IsNullOrWhiteSpace(thread.body_md))
            result.AddError("主題內容不能為空");

        if (thread.created_by_user_id <= 0)
            result.AddError("無效的創建者用戶ID");

        return result;
    }

    /// <summary>
    /// 驗證創建回覆請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateCreatePostRequest(ThreadPost post)
    {
        var result = new ValidationResult();

        if (post == null)
            result.AddError("回覆數據不能為空");

        if (post.thread_id <= 0)
            result.AddError("無效的主題ID");

        if (string.IsNullOrWhiteSpace(post.body_md))
            result.AddError("回覆內容不能為空");

        if (post.created_by_user_id <= 0)
            result.AddError("無效的創建者用戶ID");

        return result;
    }

    /// <summary>
    /// 更新主題欄位 - 新增方法
    /// </summary>
    private void UpdateThreadFields(Thread existingThread, Thread updateData)
    {
        if (!string.IsNullOrWhiteSpace(updateData.title))
            existingThread.title = updateData.title;

        if (!string.IsNullOrWhiteSpace(updateData.body_md))
            existingThread.body_md = updateData.body_md;

        if (!string.IsNullOrWhiteSpace(updateData.tldr))
            existingThread.tldr = updateData.tldr;

        if (updateData.game_id.HasValue)
            existingThread.game_id = updateData.game_id;

        if (updateData.visibility.HasValue)
            existingThread.visibility = updateData.visibility.Value;

        if (updateData.pinned.HasValue)
            existingThread.pinned = updateData.pinned.Value;
    }

    /// <summary>
    /// 更新回覆欄位 - 新增方法
    /// </summary>
    private void UpdatePostFields(ThreadPost existingPost, ThreadPost updateData)
    {
        if (!string.IsNullOrWhiteSpace(updateData.body_md))
            existingPost.body_md = updateData.body_md;

        if (updateData.visibility.HasValue)
            existingPost.visibility = updateData.visibility.Value;
    }

    #endregion
}