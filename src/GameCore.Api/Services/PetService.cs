using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class PetService : IPetService
{
    private readonly GameCoreDbContext _context;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ILogger<PetService> _logger;

    public PetService(GameCoreDbContext context, IUserWalletRepository walletRepository, ILogger<PetService> logger)
    {
        _context = context;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<PetDto?> GetPetAsync(int userId)
    {
        try
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                // Create default pet if none exists
                pet = new Pet
                {
                    UserId = userId,
                    Name = "小可愛",
                    Level = 1,
                    Exp = 0,
                    Hunger = 100,
                    Mood = 100,
                    Stamina = 100,
                    Cleanliness = 100,
                    Health = 100,
                    SkinColor = "default",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    LastDecayAt = DateTime.UtcNow
                };

                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();
            }

            return MapToPetDto(pet);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pet for user {UserId}", userId);
            return null;
        }
    }

    public async Task<PetInteractionResult> InteractWithPetAsync(int userId, string interactionType)
    {
        try
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return new PetInteractionResult
                {
                    Success = false,
                    Message = "寵物不存在"
                };
            }

            var leveledUp = false;

            // Apply interaction effects based on business rules
            switch (interactionType.ToLower())
            {
                case "feed":
                    pet.Hunger = Math.Min(100, pet.Hunger + 10);
                    break;
                case "wash":
                    pet.Cleanliness = Math.Min(100, pet.Cleanliness + 10);
                    break;
                case "play":
                    pet.Mood = Math.Min(100, pet.Mood + 10);
                    break;
                case "rest":
                    pet.Stamina = Math.Min(100, pet.Stamina + 10);
                    break;
                default:
                    return new PetInteractionResult
                    {
                        Success = false,
                        Message = "無效的互動類型"
                    };
            }

            // Update health based on business rules
            pet.UpdateHealth();

            // Try level up
            leveledUp = pet.TryLevelUp();

            pet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} interacted with pet: {InteractionType}, LeveledUp: {LeveledUp}", 
                userId, interactionType, leveledUp);

            return new PetInteractionResult
            {
                Success = true,
                Message = $"與寵物{interactionType}成功！",
                Pet = MapToPetDto(pet),
                LeveledUp = leveledUp
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during pet interaction for user {UserId}", userId);
            return new PetInteractionResult
            {
                Success = false,
                Message = "互動失敗，請稍後再試"
            };
        }
    }

    public async Task<PetColorChangeResult> ChangePetColorAsync(int userId, string newColor)
    {
        try
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (pet == null)
            {
                return new PetColorChangeResult
                {
                    Success = false,
                    Message = "寵物不存在"
                };
            }

            // Check if user has enough points (2000 points required)
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < 2000)
            {
                return new PetColorChangeResult
                {
                    Success = false,
                    Message = "點數不足，需要2000點"
                };
            }

            // Deduct points
            wallet.Balance -= 2000;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);

            // Update pet color
            pet.SkinColor = newColor;
            pet.UpdatedAt = DateTime.UtcNow;

            // Create notification
            var notification = new Notification
            {
                UserId = userId,
                Title = "寵物顏色變更",
                Content = $"您的寵物顏色已變更為 {newColor}，扣除2000點",
                Type = "pet_color",
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} changed pet color to {NewColor}", userId, newColor);

            return new PetColorChangeResult
            {
                Success = true,
                Message = "寵物顏色變更成功！",
                Pet = MapToPetDto(pet),
                RemainingBalance = wallet.Balance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing pet color for user {UserId}", userId);
            return new PetColorChangeResult
            {
                Success = false,
                Message = "顏色變更失敗，請稍後再試"
            };
        }
    }

    public async Task<bool> CanPlayMiniGameAsync(int userId)
    {
        try
        {
            var pet = await _context.Pets
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return pet?.CanPlayMiniGame() ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} can play mini game", userId);
            return false;
        }
    }

    public async Task ProcessDailyDecayAsync()
    {
        try
        {
            var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");
            var taipeiNow = TimeZoneInfo.ConvertTime(DateTime.UtcNow, taipeiTimeZone);
            var today = taipeiNow.Date;

            var pets = await _context.Pets
                .Where(p => p.LastDecayAt.Date < today)
                .ToListAsync();

            foreach (var pet in pets)
            {
                // Apply daily decay according to business rules
                pet.Hunger = Math.Max(0, pet.Hunger - 20);
                pet.Mood = Math.Max(0, pet.Mood - 30);
                pet.Stamina = Math.Max(0, pet.Stamina - 10);
                pet.Cleanliness = Math.Max(0, pet.Cleanliness - 20);
                pet.Health = Math.Max(0, pet.Health - 20);

                // Update health based on penalties
                pet.UpdateHealth();

                pet.LastDecayAt = DateTime.UtcNow;
                pet.UpdatedAt = DateTime.UtcNow;
            }

            if (pets.Any())
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Processed daily decay for {Count} pets", pets.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing daily decay");
        }
    }

    private PetDto MapToPetDto(Pet pet)
    {
        return new PetDto
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
        };
    }
}