using GameCore.Domain.Interfaces;
using GameCore.Domain.Models;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 遊戲資料存取層
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<GameRepository> _logger;

    public GameRepository(GameCoreDbContext context, ILogger<GameRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Game?> GetByIdAsync(int gameId)
    {
        _logger.LogDebug("查詢遊戲: GameId = {GameId}", gameId);
        
        return await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GameId == gameId);
    }

    public async Task<List<Game>> GetGamesAsync(
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 10,
        string? sortBy = null,
        bool sortDescending = false)
    {
        _logger.LogDebug("查詢遊戲列表: SearchTerm={SearchTerm}, Category={Category}, Page={Page}", 
            searchTerm, category, page);

        var query = _context.Games.AsNoTracking();

        // 應用篩選條件
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g => g.Name.Contains(searchTerm) || g.Description.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(g => g.Category == category);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(g => g.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(g => g.Price <= maxPrice.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(g => g.IsActive == isActive.Value);
        }

        // 應用排序
        query = sortBy?.ToLower() switch
        {
            "name" => sortDescending ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
            "price" => sortDescending ? query.OrderByDescending(g => g.Price) : query.OrderBy(g => g.Price),
            "createdat" => sortDescending ? query.OrderByDescending(g => g.CreatedAt) : query.OrderBy(g => g.CreatedAt),
            _ => query.OrderByDescending(g => g.CreatedAt)
        };

        // 應用分頁
        var games = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _logger.LogInformation("查詢遊戲列表完成: 返回 {Count} 個遊戲", games.Count);
        return games;
    }

    public async Task<int> GetTotalCountAsync(
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null)
    {
        var query = _context.Games.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(g => g.Name.Contains(searchTerm) || g.Description.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(g => g.Category == category);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(g => g.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(g => g.Price <= maxPrice.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(g => g.IsActive == isActive.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Game> CreateAsync(Game game)
    {
        _logger.LogInformation("建立新遊戲: Name = {Name}", game.Name);
        
        game.CreatedAt = DateTime.UtcNow;
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("遊戲建立成功: GameId = {GameId}", game.GameId);
        return game;
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        _logger.LogInformation("更新遊戲: GameId = {GameId}", game.GameId);
        
        game.UpdatedAt = DateTime.UtcNow;
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("遊戲更新成功: GameId = {GameId}", game.GameId);
        return game;
    }

    public async Task DeleteAsync(int gameId)
    {
        _logger.LogInformation("刪除遊戲: GameId = {GameId}", gameId);
        
        var game = await _context.Games.FindAsync(gameId);
        if (game != null)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            _logger.LogInformation("遊戲刪除成功: GameId = {GameId}", gameId);
        }
        else
        {
            _logger.LogWarning("遊戲不存在，無法刪除: GameId = {GameId}", gameId);
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        _logger.LogDebug("查詢遊戲分類列表");
        
        var categories = await _context.Games
            .Where(g => g.IsActive)
            .Select(g => g.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
        
        _logger.LogInformation("查詢遊戲分類完成: 返回 {Count} 個分類", categories.Count);
        return categories;
    }
}