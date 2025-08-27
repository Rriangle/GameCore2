using GameCore.Shared.DTOs.VirtualPetDtos;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// Service interface for virtual pet operations
/// </summary>
public interface IVirtualPetService
{
    /// <summary>
    /// Create a new virtual pet for a user
    /// </summary>
    Task<VirtualPetResponseDto> CreatePetAsync(int userId, CreatePetRequestDto request);
    
    /// <summary>
    /// Get user's virtual pet
    /// </summary>
    Task<VirtualPetResponseDto> GetUserPetAsync(int userId);
    
    /// <summary>
    /// Feed the pet
    /// </summary>
    Task<PetCareResponseDto> FeedPetAsync(int userId, int itemId);
    
    /// <summary>
    /// Play with the pet
    /// </summary>
    Task<PetCareResponseDto> PlayWithPetAsync(int userId, int itemId);
    
    /// <summary>
    /// Clean the pet
    /// </summary>
    Task<PetCareResponseDto> CleanPetAsync(int userId, int itemId);
    
    /// <summary>
    /// Let the pet rest
    /// </summary>
    Task<PetCareResponseDto> RestPetAsync(int userId);
    
    /// <summary>
    /// Change pet color
    /// </summary>
    Task<VirtualPetResponseDto> ChangePetColorAsync(int userId, string newColor);
    
    /// <summary>
    /// Get pet care history
    /// </summary>
    Task<PetCareHistoryResponseDto> GetPetCareHistoryAsync(int userId, int page = 1, int pageSize = 20);
    
    /// <summary>
    /// Get pet achievements
    /// </summary>
    Task<List<PetAchievementDto>> GetPetAchievementsAsync(int userId);
    
    /// <summary>
    /// Get available pet items
    /// </summary>
    Task<List<PetItemDto>> GetAvailablePetItemsAsync();
    
    /// <summary>
    /// Get pet statistics
    /// </summary>
    Task<PetStatisticsDto> GetPetStatisticsAsync(int userId);
    
    /// <summary>
    /// Process pet status decay (called periodically)
    /// </summary>
    Task ProcessPetStatusDecayAsync();
}