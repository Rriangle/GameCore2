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
/// 互動服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class InteractionService : IInteractionService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InteractionService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 1000;
    private const int DefaultPageSize = 100;
    private const int CacheExpirationMinutes = 10;
    private const string ReactionStatsCacheKey = "ReactionStats_{0}_{1}";
    private const string UserReactionCacheKey = "UserReaction_{0}_{1}_{2}_{3}";
    private const string UserBookmarkCacheKey = "UserBookmark_{0}_{1}_{2}";
    private const string BookmarkCountCacheKey = "BookmarkCount_{0}_{1}";
    private const string UserReactionsCacheKey = "UserReactions_{0}_{1}";
    private const string UserBookmarksCacheKey = "UserBookmarks_{0}_{1}";

    // 支援的反應類型
    private static readonly HashSet<string> SupportedReactionKinds = new()
    {
        "like", "love", "haha", "wow", "sad", "angry", "heart", "thumbs_up", "thumbs_down"
    };

    // 支援的目標類型
    private static readonly HashSet<string> SupportedTargetTypes = new()
    {
        "post", "thread", "game", "forum", "comment", "review"
    };

    public InteractionService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<InteractionService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 添加反應（讚、表情等） - 優化版本
    /// </summary>
    public async Task<Reaction> AddReactionAsync(int userId, string targetType, long targetId, string kind)
    {
        _logger.LogInformation("開始添加反應，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}, 反應類型: {Kind}", 
            userId, targetType, targetId, kind);

        try
        {
            // 輸入驗證
            var validationResult = ValidateReactionRequest(userId, targetType, targetId, kind);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("添加反應請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 檢查是否已存在相同反應
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.user_id == userId && 
                                        r.target_type == targetType && 
                                        r.target_id == targetId && 
                                        r.kind == kind);

            if (existingReaction != null)
            {
                _logger.LogInformation("用戶 {UserId} 已經對 {TargetType} {TargetId} 添加了 {Kind} 反應", 
                    userId, targetType, targetId, kind);
                return existingReaction;
            }

            // 創建新反應
            var reaction = new Reaction
            {
                user_id = userId,
                target_type = targetType,
                target_id = targetId,
                kind = kind,
                created_at = DateTime.UtcNow
            };

            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearReactionRelatedCache(userId, targetType, targetId);

            _logger.LogInformation("成功添加反應，反應ID: {ReactionId}", reaction.reaction_id);
            return reaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加反應時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 移除反應 - 優化版本
    /// </summary>
    public async Task<bool> RemoveReactionAsync(int userId, string targetType, long targetId, string kind)
    {
        _logger.LogInformation("開始移除反應，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}, 反應類型: {Kind}", 
            userId, targetType, targetId, kind);

        try
        {
            // 輸入驗證
            var validationResult = ValidateReactionRequest(userId, targetType, targetId, kind);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("移除反應請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return false;
            }

            var reaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.user_id == userId && 
                                        r.target_type == targetType && 
                                        r.target_id == targetId && 
                                        r.kind == kind);

            if (reaction == null)
            {
                _logger.LogWarning("反應不存在，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}, 反應類型: {Kind}", 
                    userId, targetType, targetId, kind);
                return false;
            }

            _context.Reactions.Remove(reaction);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearReactionRelatedCache(userId, targetType, targetId);

            _logger.LogInformation("成功移除反應，反應ID: {ReactionId}", reaction.reaction_id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除反應時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取目標的反應統計 - 優化版本（含快取）
    /// </summary>
    public async Task<object> GetReactionStatsAsync(string targetType, long targetId)
    {
        _logger.LogInformation("開始獲取反應統計，目標類型: {TargetType}, 目標ID: {TargetId}", targetType, targetId);

        try
        {
            // 輸入驗證
            if (string.IsNullOrWhiteSpace(targetType))
            {
                _logger.LogWarning("目標類型不能為空");
                return new { };
            }

            if (targetId <= 0)
            {
                _logger.LogWarning("無效的目標ID: {TargetId}", targetId);
                return new { };
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(ReactionStatsCacheKey, targetType, targetId);
            if (_memoryCache.TryGetValue(cacheKey, out object cachedStats))
            {
                _logger.LogDebug("從快取獲取反應統計，目標類型: {TargetType}, 目標ID: {TargetId}", targetType, targetId);
                return cachedStats;
            }

            // 從資料庫獲取
            var stats = await _context.Reactions
                .Where(r => r.target_type == targetType && r.target_id == targetId)
                .GroupBy(r => r.kind)
                .Select(g => new { Kind = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            var totalReactions = stats.Sum(x => x.Count);
            var result = new
            {
                TargetType = targetType,
                TargetId = targetId,
                TotalReactions = totalReactions,
                ReactionsByKind = stats,
                TopReaction = stats.FirstOrDefault()?.Kind,
                TopReactionCount = stats.FirstOrDefault()?.Count ?? 0
            };

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, result, cacheOptions);

            _logger.LogInformation("成功獲取反應統計，目標類型: {TargetType}, 目標ID: {TargetId}, 總反應數: {TotalReactions}", 
                targetType, targetId, totalReactions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取反應統計時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 檢查用戶是否已反應 - 優化版本（含快取）
    /// </summary>
    public async Task<bool> HasUserReactedAsync(int userId, string targetType, long targetId, string kind)
    {
        _logger.LogInformation("開始檢查用戶反應，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}, 反應類型: {Kind}", 
            userId, targetType, targetId, kind);

        try
        {
            // 輸入驗證
            var validationResult = ValidateReactionRequest(userId, targetType, targetId, kind);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("檢查用戶反應請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return false;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserReactionCacheKey, userId, targetType, targetId, kind);
            if (_memoryCache.TryGetValue(cacheKey, out bool cachedResult))
            {
                _logger.LogDebug("從快取獲取用戶反應檢查結果，用戶ID: {UserId}", userId);
                return cachedResult;
            }

            // 從資料庫檢查
            var hasReacted = await _context.Reactions
                .AnyAsync(r => r.user_id == userId && 
                              r.target_type == targetType && 
                              r.target_id == targetId && 
                              r.kind == kind);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, hasReacted, cacheOptions);

            _logger.LogDebug("用戶 {UserId} 對 {TargetType} {TargetId} 的 {Kind} 反應狀態: {HasReacted}", 
                userId, targetType, targetId, kind, hasReacted);
            return hasReacted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶反應時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 添加收藏 - 優化版本
    /// </summary>
    public async Task<Bookmark> AddBookmarkAsync(int userId, string targetType, long targetId)
    {
        _logger.LogInformation("開始添加收藏，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}", 
            userId, targetType, targetId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateBookmarkRequest(userId, targetType, targetId);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("添加收藏請求驗證失敗: {Error}", validationResult.ErrorMessage);
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 檢查是否已存在相同收藏
            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.user_id == userId && 
                                        b.target_type == targetType && 
                                        b.target_id == targetId);

            if (existingBookmark != null)
            {
                _logger.LogInformation("用戶 {UserId} 已經收藏了 {TargetType} {TargetId}", 
                    userId, targetType, targetId);
                return existingBookmark;
            }

            // 創建新收藏
            var bookmark = new Bookmark
            {
                user_id = userId,
                target_type = targetType,
                target_id = targetId,
                created_at = DateTime.UtcNow
            };

            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearBookmarkRelatedCache(userId, targetType, targetId);

            _logger.LogInformation("成功添加收藏，收藏ID: {BookmarkId}", bookmark.bookmark_id);
            return bookmark;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加收藏時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 移除收藏 - 優化版本
    /// </summary>
    public async Task<bool> RemoveBookmarkAsync(int userId, string targetType, long targetId)
    {
        _logger.LogInformation("開始移除收藏，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}", 
            userId, targetType, targetId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateBookmarkRequest(userId, targetType, targetId);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("移除收藏請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return false;
            }

            var bookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.user_id == userId && 
                                        b.target_type == targetType && 
                                        b.target_id == targetId);

            if (bookmark == null)
            {
                _logger.LogWarning("收藏不存在，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}", 
                    userId, targetType, targetId);
                return false;
            }

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearBookmarkRelatedCache(userId, targetType, targetId);

            _logger.LogInformation("成功移除收藏，收藏ID: {BookmarkId}", bookmark.bookmark_id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除收藏時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取用戶收藏列表 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Bookmark>> GetUserBookmarksAsync(int userId, string? targetType = null)
    {
        _logger.LogInformation("開始獲取用戶收藏列表，用戶ID: {UserId}, 目標類型: {TargetType}", userId, targetType);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return Enumerable.Empty<Bookmark>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserBookmarksCacheKey, userId, targetType ?? "all");
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Bookmark> cachedBookmarks))
            {
                _logger.LogDebug("從快取獲取用戶收藏列表，用戶ID: {UserId}", userId);
                return cachedBookmarks;
            }

            // 從資料庫獲取
            var query = _context.Bookmarks
                .Include(b => b.User)
                .Where(b => b.user_id == userId);

            if (!string.IsNullOrEmpty(targetType))
            {
                query = query.Where(b => b.target_type == targetType);
            }

            var bookmarks = await query
                .OrderByDescending(b => b.created_at)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, bookmarks, cacheOptions);

            _logger.LogInformation("成功獲取用戶收藏列表，用戶ID: {UserId}, 數量: {Count}", userId, bookmarks.Count);
            return bookmarks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶 {UserId} 收藏列表時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 檢查用戶是否已收藏 - 優化版本（含快取）
    /// </summary>
    public async Task<bool> HasUserBookmarkedAsync(int userId, string targetType, long targetId)
    {
        _logger.LogInformation("開始檢查用戶收藏，用戶ID: {UserId}, 目標類型: {TargetType}, 目標ID: {TargetId}", 
            userId, targetType, targetId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateBookmarkRequest(userId, targetType, targetId);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("檢查用戶收藏請求驗證失敗: {Error}", validationResult.ErrorMessage);
                return false;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserBookmarkCacheKey, userId, targetType, targetId);
            if (_memoryCache.TryGetValue(cacheKey, out bool cachedResult))
            {
                _logger.LogDebug("從快取獲取用戶收藏檢查結果，用戶ID: {UserId}", userId);
                return cachedResult;
            }

            // 從資料庫檢查
            var hasBookmarked = await _context.Bookmarks
                .AnyAsync(b => b.user_id == userId && 
                              b.target_type == targetType && 
                              b.target_id == targetId);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, hasBookmarked, cacheOptions);

            _logger.LogDebug("用戶 {UserId} 對 {TargetType} {TargetId} 的收藏狀態: {HasBookmarked}", 
                userId, targetType, targetId, hasBookmarked);
            return hasBookmarked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶收藏時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取收藏統計 - 優化版本（含快取）
    /// </summary>
    public async Task<int> GetBookmarkCountAsync(string targetType, long targetId)
    {
        _logger.LogInformation("開始獲取收藏統計，目標類型: {TargetType}, 目標ID: {TargetId}", targetType, targetId);

        try
        {
            // 輸入驗證
            if (string.IsNullOrWhiteSpace(targetType))
            {
                _logger.LogWarning("目標類型不能為空");
                return 0;
            }

            if (targetId <= 0)
            {
                _logger.LogWarning("無效的目標ID: {TargetId}", targetId);
                return 0;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(BookmarkCountCacheKey, targetType, targetId);
            if (_memoryCache.TryGetValue(cacheKey, out int cachedCount))
            {
                _logger.LogDebug("從快取獲取收藏統計，目標類型: {TargetType}, 目標ID: {TargetId}", targetType, targetId);
                return cachedCount;
            }

            // 從資料庫獲取
            var count = await _context.Bookmarks
                .CountAsync(b => b.target_type == targetType && b.target_id == targetId);

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, count, cacheOptions);

            _logger.LogInformation("成功獲取收藏統計，目標類型: {TargetType}, 目標ID: {TargetId}, 數量: {Count}", 
                targetType, targetId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取收藏統計時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取用戶反應列表 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Reaction>> GetUserReactionsAsync(int userId, string? targetType = null)
    {
        _logger.LogInformation("開始獲取用戶反應列表，用戶ID: {UserId}, 目標類型: {TargetType}", userId, targetType);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return Enumerable.Empty<Reaction>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(UserReactionsCacheKey, userId, targetType ?? "all");
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Reaction> cachedReactions))
            {
                _logger.LogDebug("從快取獲取用戶反應列表，用戶ID: {UserId}", userId);
                return cachedReactions;
            }

            // 從資料庫獲取
            var query = _context.Reactions
                .Include(r => r.User)
                .Where(r => r.user_id == userId);

            if (!string.IsNullOrEmpty(targetType))
            {
                query = query.Where(r => r.target_type == targetType);
            }

            var reactions = await query
                .OrderByDescending(r => r.created_at)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, reactions, cacheOptions);

            _logger.LogInformation("成功獲取用戶反應列表，用戶ID: {UserId}, 數量: {Count}", userId, reactions.Count);
            return reactions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶 {UserId} 反應列表時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 清除反應相關快取 - 新增方法
    /// </summary>
    public void ClearReactionRelatedCache(int userId, string targetType, long targetId)
    {
        try
        {
            // 清除特定目標的反應統計快取
            var statsCacheKey = string.Format(ReactionStatsCacheKey, targetType, targetId);
            _memoryCache.Remove(statsCacheKey);

            // 清除用戶反應檢查快取
            foreach (var kind in SupportedReactionKinds)
            {
                var userReactionCacheKey = string.Format(UserReactionCacheKey, userId, targetType, targetId, kind);
                _memoryCache.Remove(userReactionCacheKey);
            }

            // 清除用戶反應列表快取
            var userReactionsCacheKey = string.Format(UserReactionsCacheKey, userId, "all");
            _memoryCache.Remove(userReactionsCacheKey);

            var userReactionsByTypeCacheKey = string.Format(UserReactionsCacheKey, userId, targetType);
            _memoryCache.Remove(userReactionsByTypeCacheKey);

            _logger.LogDebug("已清除用戶 {UserId} 對 {TargetType} {TargetId} 的反應相關快取", userId, targetType, targetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除反應相關快取時發生錯誤");
        }
    }

    /// <summary>
    /// 清除收藏相關快取 - 新增方法
    /// </summary>
    public void ClearBookmarkRelatedCache(int userId, string targetType, long targetId)
    {
        try
        {
            // 清除特定目標的收藏統計快取
            var bookmarkCountCacheKey = string.Format(BookmarkCountCacheKey, targetType, targetId);
            _memoryCache.Remove(bookmarkCountCacheKey);

            // 清除用戶收藏檢查快取
            var userBookmarkCacheKey = string.Format(UserBookmarkCacheKey, userId, targetType, targetId);
            _memoryCache.Remove(userBookmarkCacheKey);

            // 清除用戶收藏列表快取
            var userBookmarksCacheKey = string.Format(UserBookmarksCacheKey, userId, "all");
            _memoryCache.Remove(userBookmarksCacheKey);

            var userBookmarksByTypeCacheKey = string.Format(UserBookmarksCacheKey, userId, targetType);
            _memoryCache.Remove(userBookmarksByTypeCacheKey);

            _logger.LogDebug("已清除用戶 {UserId} 對 {TargetType} {TargetId} 的收藏相關快取", userId, targetType, targetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除收藏相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證反應請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateReactionRequest(int userId, string targetType, long targetId, string kind)
    {
        var result = new ValidationResult();

        if (userId <= 0)
            result.AddError("無效的用戶ID");

        if (string.IsNullOrWhiteSpace(targetType))
            result.AddError("目標類型不能為空");

        if (!SupportedTargetTypes.Contains(targetType))
            result.AddError($"不支援的目標類型: {targetType}");

        if (targetId <= 0)
            result.AddError("無效的目標ID");

        if (string.IsNullOrWhiteSpace(kind))
            result.AddError("反應類型不能為空");

        if (!SupportedReactionKinds.Contains(kind))
            result.AddError($"不支援的反應類型: {kind}");

        return result;
    }

    /// <summary>
    /// 驗證收藏請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateBookmarkRequest(int userId, string targetType, long targetId)
    {
        var result = new ValidationResult();

        if (userId <= 0)
            result.AddError("無效的用戶ID");

        if (string.IsNullOrWhiteSpace(targetType))
            result.AddError("目標類型不能為空");

        if (!SupportedTargetTypes.Contains(targetType))
            result.AddError($"不支援的目標類型: {targetType}");

        if (targetId <= 0)
            result.AddError("無效的目標ID");

        return result;
    }

    #endregion
}