using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 洞察服務實現
    /// </summary>
    public class InsightService : IInsightService
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<InsightService> _logger;

        public InsightService(GameCoreDbContext context, ILogger<InsightService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取洞察貼文列表
        /// </summary>
        public async Task<IEnumerable<Post>> GetInsightsAsync(int? gameId = null, string? status = null, bool? pinned = null)
        {
            try
            {
                var query = _context.Posts
                    .Include(p => p.Game)
                    .Include(p => p.CreatedByUser)
                    .Include(p => p.PostMetricSnapshot)
                    .Include(p => p.PostSources)
                    .Where(p => p.type == "insight");

                if (gameId.HasValue)
                {
                    query = query.Where(p => p.game_id == gameId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(p => p.status == status);
                }

                if (pinned.HasValue)
                {
                    query = query.Where(p => p.pinned == pinned.Value);
                }

                return await query
                    .OrderByDescending(p => p.pinned)
                    .ThenByDescending(p => p.published_at ?? p.created_at)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取洞察貼文列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 獲取洞察貼文詳情
        /// </summary>
        public async Task<Post?> GetInsightAsync(int postId)
        {
            try
            {
                return await _context.Posts
                    .Include(p => p.Game)
                    .Include(p => p.CreatedByUser)
                    .Include(p => p.PostMetricSnapshot)
                    .Include(p => p.PostSources)
                    .FirstOrDefaultAsync(p => p.post_id == postId && p.type == "insight");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取洞察貼文 {PostId} 詳情時發生錯誤", postId);
                throw;
            }
        }

        /// <summary>
        /// 創建洞察貼文
        /// </summary>
        public async Task<Post> CreateInsightAsync(Post post)
        {
            try
            {
                // 設置默認值
                post.type = "insight";
                post.status = "draft";
                post.created_at = DateTime.UtcNow;
                post.visibility = true;
                post.pinned = false;

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功創建洞察貼文 {PostId}", post.post_id);
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建洞察貼文時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 更新洞察貼文
        /// </summary>
        public async Task<Post> UpdateInsightAsync(int postId, Post updateData)
        {
            try
            {
                var existingPost = await _context.Posts
                    .FirstOrDefaultAsync(p => p.post_id == postId && p.type == "insight");

                if (existingPost == null)
                {
                    throw new InvalidOperationException($"洞察貼文 {postId} 不存在");
                }

                // 更新允許的欄位
                existingPost.title = updateData.title;
                existingPost.tldr = updateData.tldr;
                existingPost.body_md = updateData.body_md;
                existingPost.game_id = updateData.game_id;
                existingPost.visibility = updateData.visibility;
                existingPost.status = updateData.status;
                existingPost.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("成功更新洞察貼文 {PostId}", postId);
                return existingPost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新洞察貼文 {PostId} 時發生錯誤", postId);
                throw;
            }
        }

        /// <summary>
        /// 刪除洞察貼文
        /// </summary>
        public async Task<bool> DeleteInsightAsync(int postId)
        {
            try
            {
                var post = await _context.Posts
                    .FirstOrDefaultAsync(p => p.post_id == postId && p.type == "insight");

                if (post == null)
                {
                    return false;
                }

                // 軟刪除：將狀態設為 hidden
                post.status = "hidden";
                post.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("成功刪除洞察貼文 {PostId}", postId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除洞察貼文 {PostId} 時發生錯誤", postId);
                throw;
            }
        }

        /// <summary>
        /// 發布洞察貼文
        /// </summary>
        public async Task<Post> PublishInsightAsync(int postId)
        {
            try
            {
                var post = await _context.Posts
                    .FirstOrDefaultAsync(p => p.post_id == postId && p.type == "insight");

                if (post == null)
                {
                    throw new InvalidOperationException($"洞察貼文 {postId} 不存在");
                }

                if (post.status == "published")
                {
                    _logger.LogWarning("洞察貼文 {PostId} 已經是發布狀態", postId);
                    return post;
                }

                // 檢查是否關聯遊戲，如果是則創建指標快照
                if (post.game_id.HasValue)
                {
                    await CreateMetricSnapshotAsync(post);
                }

                // 更新狀態
                post.status = "published";
                post.published_at = DateTime.UtcNow;
                post.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("成功發布洞察貼文 {PostId}", postId);
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發布洞察貼文 {PostId} 時發生錯誤", postId);
                throw;
            }
        }

        /// <summary>
        /// 設置/取消置頂
        /// </summary>
        public async Task<Post> TogglePinAsync(int postId)
        {
            try
            {
                var post = await _context.Posts
                    .FirstOrDefaultAsync(p => p.post_id == postId && p.type == "insight");

                if (post == null)
                {
                    throw new InvalidOperationException($"洞察貼文 {PostId} 不存在");
                }

                post.pinned = !post.pinned;
                post.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("洞察貼文 {PostId} 置頂狀態已切換為 {Pinned}", postId, post.pinned);
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切換洞察貼文 {PostId} 置頂狀態時發生錯誤", postId);
                throw;
            }
        }

        /// <summary>
        /// 獲取熱門洞察貼文
        /// </summary>
        public async Task<IEnumerable<Post>> GetPopularInsightsAsync(int limit = 10)
        {
            try
            {
                // 這裡可以根據實際需求實現熱門算法
                // 目前簡單地根據發布時間和置頂狀態排序
                return await _context.Posts
                    .Include(p => p.Game)
                    .Include(p => p.CreatedByUser)
                    .Where(p => p.type == "insight" && p.status == "published")
                    .OrderByDescending(p => p.pinned)
                    .ThenByDescending(p => p.published_at)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取熱門洞察貼文時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 獲取遊戲相關洞察貼文
        /// </summary>
        public async Task<IEnumerable<Post>> GetGameInsightsAsync(int gameId, int limit = 20)
        {
            try
            {
                return await _context.Posts
                    .Include(p => p.Game)
                    .Include(p => p.CreatedByUser)
                    .Include(p => p.PostMetricSnapshot)
                    .Where(p => p.type == "insight" && p.game_id == gameId && p.status == "published")
                    .OrderByDescending(p => p.pinned)
                    .ThenByDescending(p => p.published_at)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲 {GameId} 相關洞察貼文時發生錯誤", gameId);
                throw;
            }
        }

        /// <summary>
        /// 創建指標快照
        /// </summary>
        private async Task CreateMetricSnapshotAsync(Post post)
        {
            try
            {
                if (!post.game_id.HasValue)
                {
                    return;
                }

                // 檢查是否已存在快照
                var existingSnapshot = await _context.PostMetricSnapshots
                    .FirstOrDefaultAsync(pms => pms.post_id == post.post_id);

                if (existingSnapshot != null)
                {
                    return;
                }

                // 獲取最新的熱度指數
                var latestIndex = await _context.PopularityIndexDailies
                    .Where(pid => pid.game_id == post.game_id.Value)
                    .OrderByDescending(pid => pid.date)
                    .FirstOrDefaultAsync();

                if (latestIndex == null)
                {
                    _logger.LogWarning("遊戲 {GameId} 沒有熱度指數數據，無法創建快照", post.game_id.Value);
                    return;
                }

                // 獲取當日的指標數據
                var metrics = await _context.GameMetricDailies
                    .Include(m => m.Metric)
                    .Where(m => m.game_id == post.game_id.Value && m.date == latestIndex.date)
                    .ToListAsync();

                // 創建快照
                var snapshot = new PostMetricSnapshot
                {
                    post_id = post.post_id,
                    game_id = post.game_id.Value,
                    date = latestIndex.date,
                    index_value = latestIndex.index_value,
                    details_json = JsonSerializer.Serialize(new
                    {
                        metrics = metrics.Select(m => new
                        {
                            code = m.Metric.code,
                            description = m.Metric.description,
                            value = m.value,
                            unit = m.Metric.unit,
                            quality = m.quality
                        }).ToList(),
                        calculated_at = DateTime.UtcNow
                    }),
                    created_at = DateTime.UtcNow
                };

                _context.PostMetricSnapshots.Add(snapshot);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功為洞察貼文 {PostId} 創建指標快照", post.post_id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "為洞察貼文 {PostId} 創建指標快照時發生錯誤", post.post_id);
                // 不拋出異常，避免影響貼文發布流程
            }
        }
    }
}