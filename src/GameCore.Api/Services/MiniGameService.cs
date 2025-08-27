using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class MiniGameService : IMiniGameService
{
    private readonly GameCoreDbContext _context;
    private readonly IPetService _petService;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ILogger<MiniGameService> _logger;
    private readonly Random _random;

    public MiniGameService(GameCoreDbContext context, IPetService petService, IUserWalletRepository walletRepository, ILogger<MiniGameService> logger)
    {
        _context = context;
        _petService = petService;
        _walletRepository = walletRepository;
        _logger = logger;
        _random = new Random();
    }

    public async Task<MiniGameResult> PlayMiniGameAsync(int userId, string gameType)
    {
        try
        {
            // Check if user can play mini game
            var canPlay = await _petService.CanPlayMiniGameAsync(userId);
            if (!canPlay)
            {
                return new MiniGameResult
                {
                    Success = false,
                    Message = "寵物狀態不佳，無法進行小遊戲"
                };
            }

            // Simulate game result (random win/lose)
            var isWin = _random.Next(100) < 60; // 60% win rate
            var score = _random.Next(50, 100);
            
            // Calculate rewards based on game type and result
            var (pointsEarned, expEarned) = CalculateRewards(gameType, isWin, score);

            // Create game record
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserId == userId);
            if (pet == null)
            {
                return new MiniGameResult
                {
                    Success = false,
                    Message = "寵物不存在"
                };
            }

            var gameRecord = new MiniGame
            {
                UserId = userId,
                PetId = pet.PetId,
                GameType = gameType,
                IsWin = isWin,
                Score = score,
                PointsEarned = pointsEarned,
                ExpEarned = expEarned,
                CreatedAt = DateTime.UtcNow
            };

            _context.MiniGames.Add(gameRecord);

            // Update pet experience
            pet.Exp += expEarned;
            var leveledUp = pet.TryLevelUp();
            pet.UpdatedAt = DateTime.UtcNow;

            // Update user wallet
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet != null)
            {
                wallet.Balance += pointsEarned;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Mini game played: User {UserId}, Game {GameType}, Win {IsWin}, Score {Score}, Points {Points}, Exp {Exp}", 
                userId, gameType, isWin, score, pointsEarned, expEarned);

            return new MiniGameResult
            {
                Success = true,
                Message = isWin ? "恭喜獲勝！" : "再接再厲！",
                IsWin = isWin,
                Score = score,
                PointsEarned = pointsEarned,
                ExpEarned = expEarned,
                UpdatedPet = new PetDto
                {
                    PetId = pet.PetId,
                    Name = pet.Name,
                    Level = pet.Level,
                    Exp = pet.Exp,
                    RequiredExp = pet.GetRequiredExpForNextLevel(),
                    Hunger = pet.Hunger,
                    Mood = pet.Mood,
                    Stamina = pet.Stamina,
                    Cleanliness = pet.Cleanliness,
                    Health = pet.Health,
                    SkinColor = pet.SkinColor,
                    CanPlayMiniGame = pet.CanPlayMiniGame()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing mini game for user {UserId}", userId);
            return new MiniGameResult
            {
                Success = false,
                Message = "遊戲失敗，請稍後再試"
            };
        }
    }

    public async Task<IEnumerable<MiniGameRecordDto>> GetUserGameRecordsAsync(int userId)
    {
        try
        {
            var records = await _context.MiniGames
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return records.Select(g => new MiniGameRecordDto
            {
                GameId = g.GameId,
                GameType = g.GameType,
                IsWin = g.IsWin,
                Score = g.Score,
                PointsEarned = g.PointsEarned,
                ExpEarned = g.ExpEarned,
                CreatedAt = g.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game records for user {UserId}", userId);
            return Enumerable.Empty<MiniGameRecordDto>();
        }
    }

    public async Task<MiniGameStatsDto?> GetUserGameStatsAsync(int userId)
    {
        try
        {
            var records = await _context.MiniGames
                .Where(g => g.UserId == userId)
                .ToListAsync();

            if (!records.Any())
            {
                return new MiniGameStatsDto
                {
                    TotalGames = 0,
                    Wins = 0,
                    Losses = 0,
                    WinRate = 0,
                    TotalPointsEarned = 0,
                    TotalExpEarned = 0,
                    AverageScore = 0,
                    HighestScore = 0
                };
            }

            var totalGames = records.Count;
            var wins = records.Count(g => g.IsWin);
            var losses = totalGames - wins;
            var winRate = totalGames > 0 ? (double)wins / totalGames * 100 : 0;
            var totalPointsEarned = records.Sum(g => g.PointsEarned);
            var totalExpEarned = records.Sum(g => g.ExpEarned);
            var averageScore = (int)records.Average(g => g.Score);
            var highestScore = records.Max(g => g.Score);

            return new MiniGameStatsDto
            {
                TotalGames = totalGames,
                Wins = wins,
                Losses = losses,
                WinRate = winRate,
                TotalPointsEarned = totalPointsEarned,
                TotalExpEarned = totalExpEarned,
                AverageScore = averageScore,
                HighestScore = highestScore
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game stats for user {UserId}", userId);
            return null;
        }
    }

    private (int points, int exp) CalculateRewards(string gameType, bool isWin, int score)
    {
        var basePoints = gameType.ToLower() switch
        {
            "puzzle" => 50,
            "action" => 40,
            "strategy" => 60,
            _ => 30
        };

        var baseExp = gameType.ToLower() switch
        {
            "puzzle" => 100,
            "action" => 80,
            "strategy" => 120,
            _ => 60
        };

        // Apply win/lose multiplier
        var pointsMultiplier = isWin ? 1.5 : 0.5;
        var expMultiplier = isWin ? 1.2 : 0.3;

        // Apply score bonus (0-20% bonus)
        var scoreBonus = score / 100.0;

        var finalPoints = (int)(basePoints * pointsMultiplier * (1 + scoreBonus));
        var finalExp = (int)(baseExp * expMultiplier * (1 + scoreBonus));

        return (finalPoints, finalExp);
    }
}