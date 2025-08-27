using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class ForumService : IForumService
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<ForumService> _logger;

    public ForumService(GameCoreDbContext context, ILogger<ForumService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ForumPostDto>> GetPostsAsync(string? status = null)
    {
        try
        {
            var query = _context.ForumPosts
                .Include(p => p.User)
                .Where(p => string.IsNullOrEmpty(status) || p.Status == status);

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(p => new ForumPostDto
            {
                PostId = p.PostId,
                UserId = p.UserId,
                Username = p.User.Username,
                Title = p.Title,
                Content = p.Content,
                Status = p.Status,
                ViewCount = p.ViewCount,
                LikeCount = p.LikeCount,
                ReplyCount = p.ReplyCount,
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                IsLiked = false, // Will be populated based on current user
                IsBookmarked = false // Will be populated based on current user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forum posts");
            return Enumerable.Empty<ForumPostDto>();
        }
    }

    public async Task<ForumPostDto?> GetPostAsync(int postId)
    {
        try
        {
            var post = await _context.ForumPosts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null) return null;

            return new ForumPostDto
            {
                PostId = post.PostId,
                UserId = post.UserId,
                Username = post.User.Username,
                Title = post.Title,
                Content = post.Content,
                Status = post.Status,
                ViewCount = post.ViewCount,
                LikeCount = post.LikeCount,
                ReplyCount = post.ReplyCount,
                PublishedAt = post.PublishedAt,
                CreatedAt = post.CreatedAt,
                IsLiked = false,
                IsBookmarked = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forum post {PostId}", postId);
            return null;
        }
    }

    public async Task<ForumPostResult> CreatePostAsync(int userId, string title, string content)
    {
        try
        {
            var post = new ForumPost
            {
                UserId = userId,
                Title = title,
                Content = content,
                Status = "draft",
                ViewCount = 0,
                LikeCount = 0,
                ReplyCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ForumPosts.Add(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum post created: User {UserId}, Title {Title}", userId, title);

            return new ForumPostResult
            {
                Success = true,
                Message = "帖子創建成功",
                Post = await GetPostAsync(post.PostId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating forum post for user {UserId}", userId);
            return new ForumPostResult
            {
                Success = false,
                Message = "帖子創建失敗"
            };
        }
    }

    public async Task<ForumPostResult> UpdatePostAsync(int postId, int userId, string title, string content)
    {
        try
        {
            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);

            if (post == null)
            {
                return new ForumPostResult
                {
                    Success = false,
                    Message = "帖子不存在或無權限修改"
                };
            }

            post.Title = title;
            post.Content = content;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum post updated: Post {PostId}, User {UserId}", postId, userId);

            return new ForumPostResult
            {
                Success = true,
                Message = "帖子更新成功",
                Post = await GetPostAsync(postId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating forum post {PostId} for user {UserId}", postId, userId);
            return new ForumPostResult
            {
                Success = false,
                Message = "帖子更新失敗"
            };
        }
    }

    public async Task<ForumPostResult> UpdatePostStatusAsync(int postId, int userId, string status)
    {
        try
        {
            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);

            if (post == null)
            {
                return new ForumPostResult
                {
                    Success = false,
                    Message = "帖子不存在或無權限修改"
                };
            }

            post.Status = status;
            if (status == "published" && !post.PublishedAt.HasValue)
            {
                post.PublishedAt = DateTime.UtcNow;
            }
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum post status updated: Post {PostId}, Status {Status}", postId, status);

            return new ForumPostResult
            {
                Success = true,
                Message = "帖子狀態更新成功",
                Post = await GetPostAsync(postId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating forum post status {PostId} for user {UserId}", postId, userId);
            return new ForumPostResult
            {
                Success = false,
                Message = "帖子狀態更新失敗"
            };
        }
    }

    public async Task<IEnumerable<ForumThreadDto>> GetThreadsAsync(int postId)
    {
        try
        {
            var threads = await _context.ForumThreads
                .Include(t => t.User)
                .Where(t => t.PostId == postId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return threads.Select(t => new ForumThreadDto
            {
                ThreadId = t.ThreadId,
                PostId = t.PostId,
                UserId = t.UserId,
                Username = t.User.Username,
                Title = t.Title,
                Content = t.Content,
                Status = t.Status,
                ViewCount = t.ViewCount,
                LikeCount = t.LikeCount,
                ReplyCount = t.ReplyCount,
                CreatedAt = t.CreatedAt,
                IsLiked = false,
                IsBookmarked = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forum threads for post {PostId}", postId);
            return Enumerable.Empty<ForumThreadDto>();
        }
    }

    public async Task<ForumThreadDto?> GetThreadAsync(int threadId)
    {
        try
        {
            var thread = await _context.ForumThreads
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.ThreadId == threadId);

            if (thread == null) return null;

            return new ForumThreadDto
            {
                ThreadId = thread.ThreadId,
                PostId = thread.PostId,
                UserId = thread.UserId,
                Username = thread.User.Username,
                Title = thread.Title,
                Content = thread.Content,
                Status = thread.Status,
                ViewCount = thread.ViewCount,
                LikeCount = thread.LikeCount,
                ReplyCount = thread.ReplyCount,
                CreatedAt = thread.CreatedAt,
                IsLiked = false,
                IsBookmarked = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forum thread {ThreadId}", threadId);
            return null;
        }
    }

    public async Task<ForumThreadResult> CreateThreadAsync(int postId, int userId, string title, string content)
    {
        try
        {
            var thread = new ForumThread
            {
                PostId = postId,
                UserId = userId,
                Title = title,
                Content = content,
                Status = "normal",
                ViewCount = 0,
                LikeCount = 0,
                ReplyCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ForumThreads.Add(thread);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum thread created: Post {PostId}, User {UserId}", postId, userId);

            return new ForumThreadResult
            {
                Success = true,
                Message = "主題創建成功",
                Thread = await GetThreadAsync(thread.ThreadId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating forum thread for post {PostId}, user {UserId}", postId, userId);
            return new ForumThreadResult
            {
                Success = false,
                Message = "主題創建失敗"
            };
        }
    }

    public async Task<ForumThreadResult> UpdateThreadAsync(int threadId, int userId, string title, string content)
    {
        try
        {
            var thread = await _context.ForumThreads
                .FirstOrDefaultAsync(t => t.ThreadId == threadId && t.UserId == userId);

            if (thread == null)
            {
                return new ForumThreadResult
                {
                    Success = false,
                    Message = "主題不存在或無權限修改"
                };
            }

            thread.Title = title;
            thread.Content = content;
            thread.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum thread updated: Thread {ThreadId}, User {UserId}", threadId, userId);

            return new ForumThreadResult
            {
                Success = true,
                Message = "主題更新成功",
                Thread = await GetThreadAsync(threadId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating forum thread {ThreadId} for user {UserId}", threadId, userId);
            return new ForumThreadResult
            {
                Success = false,
                Message = "主題更新失敗"
            };
        }
    }

    public async Task<ForumThreadResult> UpdateThreadStatusAsync(int threadId, int userId, string status)
    {
        try
        {
            var thread = await _context.ForumThreads
                .FirstOrDefaultAsync(t => t.ThreadId == threadId && t.UserId == userId);

            if (thread == null)
            {
                return new ForumThreadResult
                {
                    Success = false,
                    Message = "主題不存在或無權限修改"
                };
            }

            thread.Status = status;
            thread.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum thread status updated: Thread {ThreadId}, Status {Status}", threadId, status);

            return new ForumThreadResult
            {
                Success = true,
                Message = "主題狀態更新成功",
                Thread = await GetThreadAsync(threadId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating forum thread status {ThreadId} for user {UserId}", threadId, userId);
            return new ForumThreadResult
            {
                Success = false,
                Message = "主題狀態更新失敗"
            };
        }
    }

    public async Task<IEnumerable<ForumThreadPostDto>> GetThreadPostsAsync(int threadId)
    {
        try
        {
            var posts = await _context.ForumThreadPosts
                .Include(p => p.User)
                .Where(p => p.ThreadId == threadId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(p => new ForumThreadPostDto
            {
                ThreadPostId = p.ThreadPostId,
                ThreadId = p.ThreadId,
                UserId = p.UserId,
                Username = p.User.Username,
                Content = p.Content,
                Status = p.Status,
                LikeCount = p.LikeCount,
                CreatedAt = p.CreatedAt,
                IsLiked = false,
                IsBookmarked = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forum thread posts for thread {ThreadId}", threadId);
            return Enumerable.Empty<ForumThreadPostDto>();
        }
    }

    public async Task<ForumThreadPostResult> CreateThreadPostAsync(int threadId, int userId, string content)
    {
        try
        {
            var post = new ForumThreadPost
            {
                ThreadId = threadId,
                UserId = userId,
                Content = content,
                Status = "normal",
                LikeCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ForumThreadPosts.Add(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum thread post created: Thread {ThreadId}, User {UserId}", threadId, userId);

            return new ForumThreadPostResult
            {
                Success = true,
                Message = "回覆創建成功",
                ThreadPost = new ForumThreadPostDto
                {
                    ThreadPostId = post.ThreadPostId,
                    ThreadId = post.ThreadId,
                    UserId = post.UserId,
                    Username = "當前用戶", // Will be populated when retrieved
                    Content = post.Content,
                    Status = post.Status,
                    LikeCount = post.LikeCount,
                    CreatedAt = post.CreatedAt,
                    IsLiked = false,
                    IsBookmarked = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating forum thread post for thread {ThreadId}, user {UserId}", threadId, userId);
            return new ForumThreadPostResult
            {
                Success = false,
                Message = "回覆創建失敗"
            };
        }
    }

    public async Task<ForumThreadPostResult> UpdateThreadPostAsync(int threadPostId, int userId, string content)
    {
        try
        {
            var post = await _context.ForumThreadPosts
                .FirstOrDefaultAsync(p => p.ThreadPostId == threadPostId && p.UserId == userId);

            if (post == null)
            {
                return new ForumThreadPostResult
                {
                    Success = false,
                    Message = "回覆不存在或無權限修改"
                };
            }

            post.Content = content;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum thread post updated: Post {PostId}, User {UserId}", threadPostId, userId);

            return new ForumThreadPostResult
            {
                Success = true,
                Message = "回覆更新成功",
                ThreadPost = new ForumThreadPostDto
                {
                    ThreadPostId = post.ThreadPostId,
                    ThreadId = post.ThreadId,
                    UserId = post.UserId,
                    Username = "當前用戶",
                    Content = post.Content,
                    Status = post.Status,
                    LikeCount = post.LikeCount,
                    CreatedAt = post.CreatedAt,
                    IsLiked = false,
                    IsBookmarked = false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating forum thread post {PostId} for user {UserId}", threadPostId, userId);
            return new ForumThreadPostResult
            {
                Success = false,
                Message = "回覆更新失敗"
            };
        }
    }

    public async Task<ForumReactionResult> AddReactionAsync(int userId, string targetType, int targetId, string reactionType)
    {
        try
        {
            // Check if reaction already exists
            var existingReaction = await _context.ForumReactions
                .FirstOrDefaultAsync(r => r.UserId == userId && r.TargetType == targetType && 
                                        r.TargetId == targetId && r.ReactionType == reactionType);

            if (existingReaction != null)
            {
                return new ForumReactionResult
                {
                    Success = false,
                    Message = "已經對此內容做出反應"
                };
            }

            var reaction = new ForumReaction
            {
                UserId = userId,
                TargetType = targetType,
                TargetId = targetId,
                ReactionType = reactionType,
                CreatedAt = DateTime.UtcNow
            };

            _context.ForumReactions.Add(reaction);

            // Update like count based on target type
            switch (targetType)
            {
                case "post":
                    var post = await _context.ForumPosts.FindAsync(targetId);
                    if (post != null) post.LikeCount++;
                    break;
                case "thread":
                    var thread = await _context.ForumThreads.FindAsync(targetId);
                    if (thread != null) thread.LikeCount++;
                    break;
                case "thread_post":
                    var threadPost = await _context.ForumThreadPosts.FindAsync(targetId);
                    if (threadPost != null) threadPost.LikeCount++;
                    break;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum reaction added: User {UserId}, Target {TargetType}:{TargetId}, Type {ReactionType}", 
                userId, targetType, targetId, reactionType);

            return new ForumReactionResult
            {
                Success = true,
                Message = "反應添加成功",
                LikeCount = await GetLikeCountAsync(targetType, targetId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding forum reaction for user {UserId}", userId);
            return new ForumReactionResult
            {
                Success = false,
                Message = "反應添加失敗"
            };
        }
    }

    public async Task<ForumReactionResult> RemoveReactionAsync(int userId, string targetType, int targetId, string reactionType)
    {
        try
        {
            var reaction = await _context.ForumReactions
                .FirstOrDefaultAsync(r => r.UserId == userId && r.TargetType == targetType && 
                                        r.TargetId == targetId && r.ReactionType == reactionType);

            if (reaction == null)
            {
                return new ForumReactionResult
                {
                    Success = false,
                    Message = "未找到此反應"
                };
            }

            _context.ForumReactions.Remove(reaction);

            // Update like count based on target type
            switch (targetType)
            {
                case "post":
                    var post = await _context.ForumPosts.FindAsync(targetId);
                    if (post != null && post.LikeCount > 0) post.LikeCount--;
                    break;
                case "thread":
                    var thread = await _context.ForumThreads.FindAsync(targetId);
                    if (thread != null && thread.LikeCount > 0) thread.LikeCount--;
                    break;
                case "thread_post":
                    var threadPost = await _context.ForumThreadPosts.FindAsync(targetId);
                    if (threadPost != null && threadPost.LikeCount > 0) threadPost.LikeCount--;
                    break;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum reaction removed: User {UserId}, Target {TargetType}:{TargetId}, Type {ReactionType}", 
                userId, targetType, targetId, reactionType);

            return new ForumReactionResult
            {
                Success = true,
                Message = "反應移除成功",
                LikeCount = await GetLikeCountAsync(targetType, targetId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing forum reaction for user {UserId}", userId);
            return new ForumReactionResult
            {
                Success = false,
                Message = "反應移除失敗"
            };
        }
    }

    public async Task<ForumBookmarkResult> AddBookmarkAsync(int userId, string targetType, int targetId)
    {
        try
        {
            // Check if bookmark already exists
            var existingBookmark = await _context.ForumBookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.TargetType == targetType && b.TargetId == targetId);

            if (existingBookmark != null)
            {
                return new ForumBookmarkResult
                {
                    Success = false,
                    Message = "已經收藏此內容"
                };
            }

            var bookmark = new ForumBookmark
            {
                UserId = userId,
                TargetType = targetType,
                TargetId = targetId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ForumBookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum bookmark added: User {UserId}, Target {TargetType}:{TargetId}", 
                userId, targetType, targetId);

            return new ForumBookmarkResult
            {
                Success = true,
                Message = "收藏成功",
                IsBookmarked = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding forum bookmark for user {UserId}", userId);
            return new ForumBookmarkResult
            {
                Success = false,
                Message = "收藏失敗"
            };
        }
    }

    public async Task<ForumBookmarkResult> RemoveBookmarkAsync(int userId, string targetType, int targetId)
    {
        try
        {
            var bookmark = await _context.ForumBookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.TargetType == targetType && b.TargetId == targetId);

            if (bookmark == null)
            {
                return new ForumBookmarkResult
                {
                    Success = false,
                    Message = "未找到此收藏"
                };
            }

            _context.ForumBookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Forum bookmark removed: User {UserId}, Target {TargetType}:{TargetId}", 
                userId, targetType, targetId);

            return new ForumBookmarkResult
            {
                Success = true,
                Message = "取消收藏成功",
                IsBookmarked = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing forum bookmark for user {UserId}", userId);
            return new ForumBookmarkResult
            {
                Success = false,
                Message = "取消收藏失敗"
            };
        }
    }

    public async Task<IEnumerable<ForumPostDto>> GetUserPostsAsync(int userId)
    {
        try
        {
            var posts = await _context.ForumPosts
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return posts.Select(p => new ForumPostDto
            {
                PostId = p.PostId,
                UserId = p.UserId,
                Username = p.User.Username,
                Title = p.Title,
                Content = p.Content,
                Status = p.Status,
                ViewCount = p.ViewCount,
                LikeCount = p.LikeCount,
                ReplyCount = p.ReplyCount,
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                IsLiked = false,
                IsBookmarked = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user posts for user {UserId}", userId);
            return Enumerable.Empty<ForumPostDto>();
        }
    }

    public async Task<IEnumerable<ForumBookmarkDto>> GetUserBookmarksAsync(int userId)
    {
        try
        {
            var bookmarks = await _context.ForumBookmarks
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var bookmarkDtos = new List<ForumBookmarkDto>();

            foreach (var bookmark in bookmarks)
            {
                string title = "";
                switch (bookmark.TargetType)
                {
                    case "post":
                        var post = await _context.ForumPosts.FindAsync(bookmark.TargetId);
                        title = post?.Title ?? "未知帖子";
                        break;
                    case "thread":
                        var thread = await _context.ForumThreads.FindAsync(bookmark.TargetId);
                        title = thread?.Title ?? "未知主題";
                        break;
                    case "thread_post":
                        title = "回覆內容";
                        break;
                }

                bookmarkDtos.Add(new ForumBookmarkDto
                {
                    BookmarkId = bookmark.BookmarkId,
                    TargetType = bookmark.TargetType,
                    TargetId = bookmark.TargetId,
                    Title = title,
                    CreatedAt = bookmark.CreatedAt
                });
            }

            return bookmarkDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user bookmarks for user {UserId}", userId);
            return Enumerable.Empty<ForumBookmarkDto>();
        }
    }

    private async Task<int> GetLikeCountAsync(string targetType, int targetId)
    {
        return targetType switch
        {
            "post" => await _context.ForumPosts.Where(p => p.PostId == targetId).Select(p => p.LikeCount).FirstOrDefaultAsync(),
            "thread" => await _context.ForumThreads.Where(t => t.ThreadId == targetId).Select(t => t.LikeCount).FirstOrDefaultAsync(),
            "thread_post" => await _context.ForumThreadPosts.Where(p => p.ThreadPostId == targetId).Select(p => p.LikeCount).FirstOrDefaultAsync(),
            _ => 0
        };
    }
}