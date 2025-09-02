using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 主題回覆資料庫操作實作
/// </summary>
public class ThreadPostRepository : IThreadPostRepository
{
    private readonly GameCoreDbContext _context;

    public ThreadPostRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據ID取得回覆
    /// </summary>
    /// <param name="postId">回覆ID</param>
    /// <returns>回覆實體</returns>
    public async Task<ThreadPost?> GetByIdAsync(long postId)
    {
        return await _context.ThreadPosts
            .FirstOrDefaultAsync(p => p.ID == postId);
    }

    /// <summary>
    /// 新增回覆
    /// </summary>
    /// <param name="post">回覆實體</param>
    /// <returns>新增的回覆實體</returns>
    public async Task<ThreadPost> AddAsync(ThreadPost post)
    {
        _context.ThreadPosts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    /// <summary>
    /// 更新回覆
    /// </summary>
    /// <param name="post">回覆實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(ThreadPost post)
    {
        _context.ThreadPosts.Update(post);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除回覆
    /// </summary>
    /// <param name="postId">回覆ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(long postId)
    {
        var post = await _context.ThreadPosts.FindAsync(postId);
        if (post == null)
            return false;

        _context.ThreadPosts.Remove(post);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    public IQueryable<ThreadPost> GetQueryable()
    {
        return _context.ThreadPosts;
    }
} 