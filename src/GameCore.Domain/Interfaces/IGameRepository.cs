using GameCore.Domain.Models;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 遊戲資料存取介面
/// </summary>
public interface IGameRepository
{
    Task<Game?> GetByIdAsync(int gameId);
    Task<List<Game>> GetGamesAsync(
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 10,
        string? sortBy = null,
        bool sortDescending = false);
    Task<int> GetTotalCountAsync(
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null);
    Task<Game> CreateAsync(Game game);
    Task<Game> UpdateAsync(Game game);
    Task DeleteAsync(int gameId);
    Task<List<string>> GetCategoriesAsync();
}