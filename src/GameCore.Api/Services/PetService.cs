using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;

namespace GameCore.Api.Services;

/// <summary>
/// 虛擬寵物服務
/// </summary>
public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly INotificationService _notificationService;

    public PetService(
        IPetRepository petRepository,
        IUserWalletRepository userWalletRepository,
        INotificationService notificationService)
    {
        _petRepository = petRepository;
        _userWalletRepository = userWalletRepository;
        _notificationService = notificationService;
    }

    /// <summary>
    /// 獲取寵物資訊
    /// </summary>
    public async Task<ServiceResult<PetDto>> GetPetAsync(int userId)
    {
        try
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null)
            {
                return ServiceResult<PetDto>.CreateFailure("寵物不存在");
            }

            var petDto = MapToPetDto(pet);
            return ServiceResult<PetDto>.CreateSuccess(petDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<PetDto>.CreateFailure($"獲取寵物資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新寵物資料
    /// </summary>
    public async Task<ServiceResult<PetDto>> UpdatePetProfileAsync(int userId, UpdatePetProfileRequest request)
    {
        try
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null)
            {
                return ServiceResult<PetDto>.CreateFailure("寵物不存在");
            }

            // 更新寵物資料
            if (!string.IsNullOrEmpty(request.PetName))
            {
                pet.PetName = request.PetName;
            }

            if (!string.IsNullOrEmpty(request.SkinColor))
            {
                pet.SkinColor = request.SkinColor;
                pet.ColorChangedTime = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(request.BackgroundColor))
            {
                pet.BackgroundColor = request.BackgroundColor;
                pet.BackgroundColorChangedTime = DateTime.UtcNow;
            }

            await _petRepository.UpdateAsync(pet);

            var petDto = MapToPetDto(pet);
            return ServiceResult<PetDto>.CreateSuccess(petDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<PetDto>.CreateFailure($"更新寵物資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 寵物互動（餵食、洗澡、玩耍、休息）
    /// </summary>
    public async Task<ServiceResult<PetActionResultDto>> PerformActionAsync(int userId, string action)
    {
        try
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null)
            {
                return ServiceResult<PetActionResultDto>.CreateFailure("寵物不存在");
            }

            var levelUp = false;
            var levelUpBonus = 0;
            var message = "";

            // 根據動作類型更新寵物狀態
            switch (action.ToLower())
            {
                case "feed":
                    pet.Hunger = Math.Min(100, pet.Hunger + 10);
                    message = "餵食成功！寵物飢餓值增加了。";
                    break;
                case "bath":
                    pet.Cleanliness = Math.Min(100, pet.Cleanliness + 10);
                    message = "洗澡成功！寵物清潔值增加了。";
                    break;
                case "play":
                    pet.Mood = Math.Min(100, pet.Mood + 10);
                    message = "玩耍成功！寵物心情值增加了。";
                    break;
                case "rest":
                    pet.Stamina = Math.Min(100, pet.Stamina + 10);
                    message = "休息成功！寵物體力值增加了。";
                    break;
                default:
                    return ServiceResult<PetActionResultDto>.CreateFailure("無效的動作類型");
            }

            // 檢查健康值
            UpdateHealth(pet);

            // 檢查升級
            var oldLevel = pet.Level;
            CheckAndUpdateLevel(pet);
            if (pet.Level > oldLevel)
            {
                levelUp = true;
                levelUpBonus = pet.Level * 10; // 升級獎勵點數
                message += $" 恭喜！寵物升級到 {pet.Level} 級！";
            }

            await _petRepository.UpdateAsync(pet);

            // 如果有升級獎勵，更新用戶錢包
            if (levelUpBonus > 0)
            {
                var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
                if (userWallet != null)
                {
                    userWallet.User_Point += levelUpBonus;
                    userWallet.Last_Updated = DateTime.UtcNow;
                    await _userWalletRepository.UpdateAsync(userWallet);
                }
            }

            var result = new PetActionResultDto
            {
                Success = true,
                Action = action,
                Pet = MapToPetDto(pet),
                Message = message,
                LevelUp = levelUp,
                LevelUpBonus = levelUpBonus
            };

            return ServiceResult<PetActionResultDto>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<PetActionResultDto>.CreateFailure($"寵物互動失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 寵物換色
    /// </summary>
    public async Task<ServiceResult<PetRecolorResultDto>> RecolorPetAsync(int userId, PetRecolorRequest request)
    {
        try
        {
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null)
            {
                return ServiceResult<PetRecolorResultDto>.CreateFailure("寵物不存在");
            }

            // 檢查用戶點數
            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (userWallet == null)
            {
                return ServiceResult<PetRecolorResultDto>.CreateFailure("用戶錢包不存在");
            }

            const int recolorCost = 2000; // 換色費用
            if (userWallet.User_Point < recolorCost)
            {
                return ServiceResult<PetRecolorResultDto>.CreateFailure("點數不足，需要 2000 點");
            }

            // 扣除點數
            userWallet.User_Point -= recolorCost;
            userWallet.Last_Updated = DateTime.UtcNow;
            await _userWalletRepository.UpdateAsync(userWallet);

            // 更新寵物顏色
            pet.SkinColor = request.SkinColor;
            pet.BackgroundColor = request.BackgroundColor;
            pet.ColorChangedTime = DateTime.UtcNow;
            pet.BackgroundColorChangedTime = DateTime.UtcNow;
            pet.PointsChanged = recolorCost;
            pet.PointsChangedTime = DateTime.UtcNow;

            await _petRepository.UpdateAsync(pet);

            // 發送通知
            await _notificationService.CreateNotificationAsync(new CreateNotificationRequest
            {
                SourceId = 1, // 系統通知
                ActionId = 1, // 寵物相關
                SenderId = 0, // 系統
                RecipientIds = new List<int> { userId },
                Title = "寵物換色成功",
                Message = $"寵物換色成功，花費 {recolorCost} 點"
            });

            var result = new PetRecolorResultDto
            {
                Success = true,
                Pet = MapToPetDto(pet),
                PointsSpent = recolorCost,
                RemainingPoints = userWallet.User_Point,
                Message = "寵物換色成功！"
            };

            return ServiceResult<PetRecolorResultDto>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<PetRecolorResultDto>.CreateFailure($"寵物換色失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 獲取寵物歷史記錄
    /// </summary>
    public async Task<ServiceResult<PagedResult<PetHistoryDto>>> GetPetHistoryAsync(int userId, int page = 1, int pageSize = 20)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            // 這裡應該從寵物歷史記錄表中獲取資料
            // 由於目前沒有專門的寵物歷史記錄表，暫時返回空結果
            var historyList = new List<PetHistoryDto>();

            var pagedResult = new PagedResult<PetHistoryDto>
            {
                Data = historyList,
                Total = 0,
                Page = page,
                PageSize = pageSize,
                TotalPages = 0
            };

            return ServiceResult<PagedResult<PetHistoryDto>>.CreateSuccess(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<PetHistoryDto>>.CreateFailure($"獲取寵物歷史記錄失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新健康值
    /// </summary>
    private void UpdateHealth(Pet pet)
    {
        // 如果所有屬性都滿了，健康值設為100
        if (pet.Hunger >= 100 && pet.Mood >= 100 && pet.Stamina >= 100 && pet.Cleanliness >= 100)
        {
            pet.Health = 100;
        }
        else
        {
            // 根據各屬性值調整健康值
            var healthFactors = new List<int>();
            
            if (pet.Hunger < 30) healthFactors.Add(-20);
            if (pet.Cleanliness < 30) healthFactors.Add(-20);
            if (pet.Stamina < 30) healthFactors.Add(-20);

            foreach (var factor in healthFactors)
            {
                pet.Health = Math.Max(0, pet.Health + factor);
            }
        }
    }

    /// <summary>
    /// 檢查並更新等級
    /// </summary>
    private void CheckAndUpdateLevel(Pet pet)
    {
        var requiredExp = CalculateRequiredExp(pet.Level);
        while (pet.Experience >= requiredExp && pet.Level < 250)
        {
            pet.Experience -= requiredExp;
            pet.Level++;
            pet.LevelUpTime = DateTime.UtcNow;
            requiredExp = CalculateRequiredExp(pet.Level);
        }
    }

    /// <summary>
    /// 計算升級所需經驗值
    /// </summary>
    private int CalculateRequiredExp(int level)
    {
        if (level <= 10)
        {
            return 40 * level + 60;
        }
        else if (level <= 100)
        {
            return (int)(0.8 * level * level + 380);
        }
        else
        {
            return (int)(285.69 * Math.Pow(1.06, level));
        }
    }

    /// <summary>
    /// 映射到 PetDto
    /// </summary>
    private PetDto MapToPetDto(Pet pet)
    {
        var requiredExp = CalculateRequiredExp(pet.Level);
        return new PetDto
        {
            PetId = pet.PetID,
            PetName = pet.PetName,
            Level = pet.Level,
            Experience = pet.Experience,
            ExperienceToNextLevel = requiredExp,
            Hunger = pet.Hunger,
            Mood = pet.Mood,
            Stamina = pet.Stamina,
            Cleanliness = pet.Cleanliness,
            Health = pet.Health,
            SkinColor = pet.SkinColor,
            BackgroundColor = pet.BackgroundColor,
            LevelUpTime = pet.LevelUpTime,
            ColorChangedTime = pet.ColorChangedTime,
            BackgroundColorChangedTime = pet.BackgroundColorChangedTime
        };
    }
} 