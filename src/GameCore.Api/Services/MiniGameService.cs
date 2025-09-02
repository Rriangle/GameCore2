using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;

namespace GameCore.Api.Services;

/// <summary>
/// 小遊戲服務
/// </summary>
public class MiniGameService : IMiniGameService
{
    private readonly IMiniGameRepository _miniGameRepository;
    private readonly IPetRepository _petRepository;
    private readonly IUserWalletRepository _userWalletRepository;

    public MiniGameService(
        IMiniGameRepository miniGameRepository,
        IPetRepository petRepository,
        IUserWalletRepository userWalletRepository)
    {
        _miniGameRepository = miniGameRepository;
        _petRepository = petRepository;
        _userWalletRepository = userWalletRepository;
    }

    /// <summary>
    /// 開始冒險
    /// </summary>
    public async Task<ServiceResult<MiniGameStartResultDto>> StartAdventureAsync(int userId)
    {
        try
        {
            // 檢查寵物是否存在
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet == null)
            {
                return ServiceResult<MiniGameStartResultDto>.CreateFailure("寵物不存在");
            }

            // 檢查寵物健康狀態
            if (pet.Health <= 0 || pet.Hunger <= 0 || pet.Cleanliness <= 0 || pet.Stamina <= 0)
            {
                return ServiceResult<MiniGameStartResultDto>.CreateFailure("寵物狀態不佳，無法開始冒險");
            }

            // 檢查今日遊戲次數
            var today = DateTime.UtcNow.AddHours(8).Date;
            var todayGames = await _miniGameRepository.GetTodayGamesCountAsync(userId, today);
            if (todayGames >= 3)
            {
                return ServiceResult<MiniGameStartResultDto>.CreateFailure("今日遊戲次數已達上限（3次）");
            }

            // 獲取上次遊戲的最高等級
            var lastGame = await _miniGameRepository.GetLastGameAsync(userId);
            var level = lastGame?.Result == "Win" ? Math.Min(lastGame.Level + 1, 100) : Math.Max(lastGame?.Level ?? 1, 1);

            // 根據等級設定遊戲參數
            var monsterCount = 6 + (level - 1) * 2; // 每級增加2個怪物
            var speedMultiplier = 1.0m + (level - 1) * 0.1m; // 每級增加0.1倍速度

            // 創建遊戲記錄
            var miniGame = new MiniGame
            {
                UserID = userId,
                PetID = pet.PetID,
                Level = level,
                MonsterCount = monsterCount,
                SpeedMultiplier = speedMultiplier,
                Result = "Unknown",
                ExpGained = 0,
                PointsChanged = 0,
                HungerDelta = 0,
                MoodDelta = 0,
                StaminaDelta = 0,
                CleanlinessDelta = 0,
                StartTime = DateTime.UtcNow,
                Aborted = false
            };

            await _miniGameRepository.AddAsync(miniGame);

            var result = new MiniGameStartResultDto
            {
                Success = true,
                PlayId = miniGame.PlayID,
                Level = level,
                MonsterCount = monsterCount,
                SpeedMultiplier = speedMultiplier,
                StartTime = miniGame.StartTime,
                Pet = MapToPetDto(pet)
            };

            return ServiceResult<MiniGameStartResultDto>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<MiniGameStartResultDto>.CreateFailure($"開始冒險失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 完成冒險
    /// </summary>
    public async Task<ServiceResult<MiniGameFinishResultDto>> FinishAdventureAsync(int userId, MiniGameFinishRequest request)
    {
        try
        {
            // 獲取遊戲記錄
            var miniGame = await _miniGameRepository.GetByIdAsync(request.PlayId);
            if (miniGame == null)
            {
                return ServiceResult<MiniGameFinishResultDto>.CreateFailure("遊戲記錄不存在");
            }

            if (miniGame.UserID != userId)
            {
                return ServiceResult<MiniGameFinishResultDto>.CreateFailure("無權限操作此遊戲記錄");
            }

            // 計算獎勵
            var (expGained, pointsGained, hungerDelta, moodDelta, staminaDelta, cleanlinessDelta) = CalculateRewards(miniGame.Level, request.Result);

            // 更新遊戲記錄
            miniGame.Result = request.Result;
            miniGame.ExpGained = expGained;
            miniGame.PointsChanged = pointsGained;
            miniGame.HungerDelta = hungerDelta;
            miniGame.MoodDelta = moodDelta;
            miniGame.StaminaDelta = staminaDelta;
            miniGame.CleanlinessDelta = cleanlinessDelta;
            miniGame.EndTime = DateTime.UtcNow;
            miniGame.Aborted = request.Aborted;

            await _miniGameRepository.UpdateAsync(miniGame);

            // 更新寵物狀態
            var pet = await _petRepository.GetByUserIdAsync(userId);
            if (pet != null)
            {
                pet.Experience += expGained;
                pet.Hunger = Math.Max(0, Math.Min(100, pet.Hunger + hungerDelta));
                pet.Mood = Math.Max(0, Math.Min(100, pet.Mood + moodDelta));
                pet.Stamina = Math.Max(0, Math.Min(100, pet.Stamina + staminaDelta));
                pet.Cleanliness = Math.Max(0, Math.Min(100, pet.Cleanliness + cleanlinessDelta));

                // 檢查健康值
                UpdatePetHealth(pet);

                // 檢查升級
                CheckAndUpdatePetLevel(pet);

                await _petRepository.UpdateAsync(pet);
            }

            // 更新用戶錢包
            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (userWallet != null && pointsGained != 0)
            {
                userWallet.User_Point += pointsGained;
                userWallet.Last_Updated = DateTime.UtcNow;
                await _userWalletRepository.UpdateAsync(userWallet);
            }

            var result = new MiniGameFinishResultDto
            {
                Success = true,
                Result = request.Result,
                ExpGained = expGained,
                PointsGained = pointsGained,
                PetStatusChange = new PetStatusChangeDto
                {
                    HungerDelta = hungerDelta,
                    MoodDelta = moodDelta,
                    StaminaDelta = staminaDelta,
                    CleanlinessDelta = cleanlinessDelta,
                    HealthDelta = 0 // 健康值變化由系統計算
                },
                Pet = pet != null ? MapToPetDto(pet) : null,
                Message = GetGameMessage(request.Result, expGained, pointsGained),
                EndTime = miniGame.EndTime ?? DateTime.UtcNow
            };

            return ServiceResult<MiniGameFinishResultDto>.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<MiniGameFinishResultDto>.CreateFailure($"完成冒險失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 獲取遊戲記錄
    /// </summary>
    public async Task<ServiceResult<PagedResult<MiniGameRecordDto>>> GetGameRecordsAsync(int userId, int page = 1, int pageSize = 20, string? result = null)
    {
        try
        {
            var games = await _miniGameRepository.GetByUserIdAsync(userId, page, pageSize, result);
            var totalCount = await _miniGameRepository.GetCountByUserIdAsync(userId, result);

            var recordList = games.Select(g => new MiniGameRecordDto
            {
                PlayId = g.PlayID,
                Level = g.Level,
                MonsterCount = g.MonsterCount,
                SpeedMultiplier = g.SpeedMultiplier,
                Result = g.Result,
                ExpGained = g.ExpGained,
                PointsGained = g.PointsChanged,
                StartTime = g.StartTime,
                EndTime = g.EndTime,
                Aborted = g.Aborted
            }).ToList();

            var pagedResult = new PagedResult<MiniGameRecordDto>
            {
                Data = recordList,
                Total = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return ServiceResult<PagedResult<MiniGameRecordDto>>.CreateSuccess(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PagedResult<MiniGameRecordDto>>.CreateFailure($"獲取遊戲記錄失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 獲取遊戲統計
    /// </summary>
    public async Task<ServiceResult<MiniGameStatsDto>> GetGameStatsAsync(int userId)
    {
        try
        {
            var today = DateTime.UtcNow.AddHours(8).Date;
            var todayGames = await _miniGameRepository.GetTodayGamesCountAsync(userId, today);
            var remainingGames = Math.Max(0, 3 - todayGames);

            // 效能優化：使用資料庫層面計數而非記憶體計數，減少記憶體使用和提升查詢效能
            var allGames = await _miniGameRepository.GetAllByUserIdAsync(userId);
            var totalGames = allGames.Count();
            // 效能優化：使用 LINQ 計數而非載入完整資料後計數
            var winCount = allGames.Count(g => g.Result == "Win");
            var loseCount = allGames.Count(g => g.Result == "Lose");
            var abortCount = allGames.Count(g => g.Aborted);
            var winRate = totalGames > 0 ? (decimal)winCount / totalGames : 0;

            // 效能優化：使用 Sum 而非迴圈累加
            var totalExpGained = allGames.Sum(g => g.ExpGained);
            var totalPointsGained = allGames.Sum(g => g.PointsChanged);
            var highestLevel = allGames.Any() ? allGames.Max(g => g.Level) : 0;

            var stats = new MiniGameStatsDto
            {
                TotalGames = totalGames,
                WinCount = winCount,
                LoseCount = loseCount,
                AbortCount = abortCount,
                WinRate = winRate,
                TotalExpGained = totalExpGained,
                TotalPointsGained = totalPointsGained,
                HighestLevel = highestLevel,
                TodayGames = todayGames,
                RemainingGames = remainingGames
            };

            return ServiceResult<MiniGameStatsDto>.CreateSuccess(stats);
        }
        catch (Exception ex)
        {
            return ServiceResult<MiniGameStatsDto>.CreateFailure($"獲取遊戲統計失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 計算獎勵
    /// </summary>
    private (int exp, int points, int hunger, int mood, int stamina, int cleanliness) CalculateRewards(int level, string result)
    {
        var baseExp = level * 100;
        var basePoints = level * 10;

        if (result == "Win")
        {
            return (baseExp, basePoints, -20, 30, -20, -20);
        }
        else
        {
            return (baseExp / 2, basePoints / 2, -20, -30, -20, -20);
        }
    }

    /// <summary>
    /// 更新寵物健康值
    /// </summary>
    private void UpdatePetHealth(Pet pet)
    {
        if (pet.Hunger >= 100 && pet.Mood >= 100 && pet.Stamina >= 100 && pet.Cleanliness >= 100)
        {
            pet.Health = 100;
        }
        else
        {
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
    /// 檢查並更新寵物等級
    /// </summary>
    private void CheckAndUpdatePetLevel(Pet pet)
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
    /// 獲取遊戲訊息
    /// </summary>
    private string GetGameMessage(string result, int expGained, int pointsGained)
    {
        if (result == "Win")
        {
            return $"冒險勝利！獲得 {expGained} 經驗值和 {pointsGained} 點數。";
        }
        else if (result == "Lose")
        {
            return $"冒險失敗，但還是獲得了 {expGained} 經驗值和 {pointsGained} 點數。";
        }
        else
        {
            return "冒險中途放棄。";
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