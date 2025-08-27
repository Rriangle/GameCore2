namespace GameCore.Domain.Interfaces;

public interface IPetService
{
    Task<PetDto?> GetPetAsync(int userId);
    Task<PetInteractionResult> InteractWithPetAsync(int userId, string interactionType);
    Task<PetColorChangeResult> ChangePetColorAsync(int userId, string newColor);
    Task<bool> CanPlayMiniGameAsync(int userId);
    Task ProcessDailyDecayAsync();
}

public class PetDto
{
    public int PetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Exp { get; set; }
    public int RequiredExp { get; set; }
    public int Hunger { get; set; }
    public int Mood { get; set; }
    public int Stamina { get; set; }
    public int Cleanliness { get; set; }
    public int Health { get; set; }
    public string SkinColor { get; set; } = string.Empty;
    public bool CanPlayMiniGame { get; set; }
}

public class PetInteractionResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public PetDto? Pet { get; set; }
    public bool LeveledUp { get; set; }
}

public class PetColorChangeResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public PetDto? Pet { get; set; }
    public decimal RemainingBalance { get; set; }
}