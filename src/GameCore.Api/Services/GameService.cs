using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Models;

namespace GameCore.Api.Services;

/// <summary>
/// 遊戲服務
/// </summary>
public interface IGameService
{
    Task<GameDto?> GetGameByIdAsync(int gameId);
    Task<GameListResponseDto> GetGamesAsync(GameListQueryDto query);
    Task<GameDto> CreateGameAsync(CreateGameRequestDto request);
    Task<GameDto?> UpdateGameAsync(int gameId, UpdateGameRequestDto request);
    Task<bool> DeleteGameAsync(int gameId);
    Task<List<string>> GetCategoriesAsync();
}

/// <summary>
/// 遊戲服務實作
/// </summary>
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<GameService> _logger;

    public GameService(IGameRepository gameRepository, ILogger<GameService> logger)
    {
        _gameRepository = gameRepository;
        _logger = logger;
    }

    public async Task<GameDto?> GetGameByIdAsync(int gameId)
    {
        try
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            return game != null ? MapToDto(game) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲資訊時發生錯誤: GameId={GameId}", gameId);
            throw;
        }
    }

    public async Task<GameListResponseDto> GetGamesAsync(GameListQueryDto query)
    {
        try
        {
            var games = await _gameRepository.GetGamesAsync(
                query.SearchTerm,
                query.Category,
                query.MinPrice,
                query.MaxPrice,
                query.IsActive,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDescending
            );

            var totalCount = await _gameRepository.GetTotalCountAsync(
                query.SearchTerm,
                query.Category,
                query.MinPrice,
                query.MaxPrice,
                query.IsActive
            );

            return new GameListResponseDto
            {
                Games = games.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲列表時發生錯誤");
            throw;
        }
    }

    public async Task<GameDto> CreateGameAsync(CreateGameRequestDto request)
    {
        try
        {
            var game = new Game
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdGame = await _gameRepository.CreateAsync(game);
            _logger.LogInformation("遊戲創建成功: GameId={GameId}, Name={Name}", createdGame.GameId, createdGame.Name);

            return MapToDto(createdGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建遊戲時發生錯誤: Name={Name}", request.Name);
            throw;
        }
    }

    public async Task<GameDto?> UpdateGameAsync(int gameId, UpdateGameRequestDto request)
    {
        try
        {
            var existingGame = await _gameRepository.GetByIdAsync(gameId);
            if (existingGame == null)
            {
                _logger.LogWarning("遊戲不存在: GameId={GameId}", gameId);
                return null;
            }

            if (request.Name != null)
                existingGame.Name = request.Name;
            if (request.Description != null)
                existingGame.Description = request.Description;
            if (request.Category != null)
                existingGame.Category = request.Category;
            if (request.Price.HasValue)
                existingGame.Price = request.Price.Value;
            if (request.IsActive.HasValue)
                existingGame.IsActive = request.IsActive.Value;

            existingGame.UpdatedAt = DateTime.UtcNow;

            var updatedGame = await _gameRepository.UpdateAsync(existingGame);
            _logger.LogInformation("遊戲更新成功: GameId={GameId}", gameId);

            return MapToDto(updatedGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新遊戲時發生錯誤: GameId={GameId}", gameId);
            throw;
        }
    }

    public async Task<bool> DeleteGameAsync(int gameId)
    {
        try
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                _logger.LogWarning("遊戲不存在: GameId={GameId}", gameId);
                return false;
            }

            await _gameRepository.DeleteAsync(gameId);
            _logger.LogInformation("遊戲刪除成功: GameId={GameId}", gameId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除遊戲時發生錯誤: GameId={GameId}", gameId);
            throw;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            return await _gameRepository.GetCategoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲分類時發生錯誤");
            throw;
        }
    }

    private static GameDto MapToDto(Game game)
    {
        return new GameDto
        {
            GameId = game.GameId,
            Name = game.Name,
            Description = game.Description,
            Category = game.Category,
            Price = game.Price,
            IsActive = game.IsActive,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt
        };
    }
}