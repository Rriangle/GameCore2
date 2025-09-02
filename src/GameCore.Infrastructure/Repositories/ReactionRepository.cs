using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 反應資料庫操作實作
/// </summary>
public class ReactionRepository : IReactionRepository
{
    private readonly GameCoreDbContext _context;

    public ReactionRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據ID取得反應
    /// </summary>
    /// <param name="reactionId">反應ID</param>
    /// <returns>反應實體</returns>
    public async Task<Reaction?> GetByIdAsync(long reactionId)
    {
        return await _context.Reactions
            .FirstOrDefaultAsync(r => r.ID == reactionId);
    }

    /// <summary>
    /// 新增反應
    /// </summary>
    /// <param name="reaction">反應實體</param>
    /// <returns>新增的反應實體</returns>
    public async Task<Reaction> AddAsync(Reaction reaction)
    {
        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();
        return reaction;
    }

    /// <summary>
    /// 更新反應
    /// </summary>
    /// <param name="reaction">反應實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(Reaction reaction)
    {
        _context.Reactions.Update(reaction);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除反應
    /// </summary>
    /// <param name="reactionId">反應ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(long reactionId)
    {
        var reaction = await _context.Reactions.FindAsync(reactionId);
        if (reaction == null)
            return false;

        _context.Reactions.Remove(reaction);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    public IQueryable<Reaction> GetQueryable()
    {
        return _context.Reactions;
    }
} 