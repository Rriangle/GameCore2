using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 論壇主題資料庫操作實作
/// </summary>
public class ForumThreadRepository : IForumThreadRepository
{
    private readonly GameCoreDbContext _context;

    public ForumThreadRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據ID取得主題
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <returns>主題實體</returns>
    public async Task<ForumThread?> GetByIdAsync(long threadId)
    {
        return await _context.Threads
            .FirstOrDefaultAsync(t => t.ThreadID == threadId);
    }

    /// <summary>
    /// 新增主題
    /// </summary>
    /// <param name="thread">主題實體</param>
    /// <returns>新增的主題實體</returns>
    public async Task<ForumThread> AddAsync(ForumThread thread)
    {
        _context.Threads.Add(thread);
        await _context.SaveChangesAsync();
        return thread;
    }

    /// <summary>
    /// 更新主題
    /// </summary>
    /// <param name="thread">主題實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(ForumThread thread)
    {
        _context.Threads.Update(thread);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除主題
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(long threadId)
    {
        var thread = await _context.Threads.FindAsync(threadId);
        if (thread == null)
            return false;

        _context.Threads.Remove(thread);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    public IQueryable<ForumThread> GetQueryable()
    {
        return _context.Threads;
    }
} 