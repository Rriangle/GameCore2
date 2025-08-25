using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Services;

/// <summary>
/// 假資料生成服務實現
/// </summary>
public class FakeDataService : IFakeDataService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IMemberSalesProfileRepository _memberSalesProfileRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly ILogger<FakeDataService> _logger;

    public FakeDataService(
        IUserRepository userRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IUserWalletRepository userWalletRepository,
        IMemberSalesProfileRepository memberSalesProfileRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        ILogger<FakeDataService> logger)
    {
        _userRepository = userRepository;
        _userIntroduceRepository = userIntroduceRepository;
        _userRightsRepository = userRightsRepository;
        _userWalletRepository = userWalletRepository;
        _memberSalesProfileRepository = memberSalesProfileRepository;
        _userSalesInformationRepository = userSalesInformationRepository;
        _logger = logger;
    }

    /// <summary>
    /// 生成假用戶資料
    /// </summary>
    public async Task<int> GenerateFakeUsersAsync(int count)
    {
        try
        {
            _logger.LogInformation("開始生成 {Count} 筆假用戶資料", count);

            var generatedCount = 0;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    // 生成用戶基本資料
                    var user = new User
                    {
                        User_name = GenerateFakeName(),
                        User_Account = GenerateFakeAccount(),
                        User_Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        Email = GenerateFakeEmail(),
                        CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365)),
                        LastLoginAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 30)),
                        IsActive = true,
                        IsEmailVerified = true
                    };

                    var createdUser = await _userRepository.AddAsync(user);
                    if (createdUser != null)
                    {
                        // 生成用戶介紹
                        var userIntroduce = new UserIntroduce
                        {
                            User_ID = createdUser.User_ID,
                            User_NickName = GenerateFakeNickname(),
                            Gender = Random.Shared.Next(2) == 0 ? "男" : "女",
                            IdNumber = GenerateFakeIdNumber(),
                            Cellphone = GenerateFakeCellphone(),
                            Email = GenerateFakeEmail(),
                            Address = GenerateFakeAddress(),
                            DateOfBirth = DateTime.Today.AddYears(-Random.Shared.Next(18, 65)),
                            User_Introduce = GenerateFakeUserIntroduce(),
                            CreatedAt = createdUser.CreatedAt,
                            UpdatedAt = createdUser.CreatedAt
                        };

                        await _userIntroduceRepository.AddAsync(userIntroduce);

                        // 生成用戶權限
                        var userRights = new UserRights
                        {
                            User_Id = createdUser.User_ID,
                            User_Status = true,
                            ShoppingPermission = true,
                            MessagePermission = true,
                            SalesAuthority = Random.Shared.Next(10) == 0, // 10% 機率有銷售權限
                            CreatedAt = createdUser.CreatedAt,
                            UpdatedAt = createdUser.CreatedAt
                        };

                        await _userRightsRepository.AddAsync(userRights);

                        // 生成用戶錢包
                        var userWallet = new UserWallet
                        {
                            User_Id = createdUser.User_ID,
                            User_Point = Random.Shared.Next(0, 10000),
                            Coupon_Number = GenerateFakeCouponNumber(),
                            CreatedAt = createdUser.CreatedAt,
                            UpdatedAt = createdUser.CreatedAt
                        };

                        await _userWalletRepository.AddAsync(userWallet);

                        // 如果用戶有銷售權限，生成銷售相關資料
                        if (userRights.SalesAuthority)
                        {
                            var memberSalesProfile = new MemberSalesProfile
                            {
                                User_Id = createdUser.User_ID,
                                BankCode = Random.Shared.Next(1000, 9999),
                                BankAccountNumber = GenerateFakeBankAccountNumber(),
                                Status = "Approved",
                                AppliedAt = createdUser.CreatedAt.AddDays(Random.Shared.Next(1, 30)),
                                ReviewedAt = createdUser.CreatedAt.AddDays(Random.Shared.Next(31, 60)),
                                ReviewNotes = GenerateFakeReviewNotes(),
                                CreatedAt = createdUser.CreatedAt,
                                UpdatedAt = createdUser.CreatedAt
                            };

                            await _memberSalesProfileRepository.AddAsync(memberSalesProfile);

                            var userSalesInformation = new UserSalesInformation
                            {
                                User_Id = createdUser.User_ID,
                                UserSales_Wallet = Random.Shared.Next(1000, 50000),
                                CreatedAt = createdUser.CreatedAt,
                                UpdatedAt = createdUser.CreatedAt
                            };

                            await _userSalesInformationRepository.AddAsync(userSalesInformation);
                        }

                        generatedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "生成第 {Index} 筆假用戶資料時發生錯誤", i + 1);
                }
            }

            _logger.LogInformation("成功生成 {GeneratedCount} 筆假用戶資料", generatedCount);
            return generatedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成假用戶資料時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取假資料統計
    /// </summary>
    public async Task<FakeDataStatsDto> GetFakeDataStatsAsync()
    {
        try
        {
            var stats = new FakeDataStatsDto
            {
                TotalUsers = await _userRepository.GetCountAsync(),
                TotalUserIntroduces = await _userIntroduceRepository.GetCountAsync(),
                TotalUserRights = await _userRightsRepository.GetCountAsync(),
                TotalUserWallets = await _userWalletRepository.GetCountAsync(),
                TotalMemberSalesProfiles = await _memberSalesProfileRepository.GetCountAsync(),
                TotalUserSalesInformations = await _userSalesInformationRepository.GetCountAsync()
            };

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取假資料統計時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 清理假資料
    /// </summary>
    public async Task<int> CleanupFakeDataAsync()
    {
        try
        {
            _logger.LogInformation("開始清理假資料");

            var deletedCount = 0;
            deletedCount += await _userSalesInformationRepository.DeleteAllAsync();
            deletedCount += await _memberSalesProfileRepository.DeleteAllAsync();
            deletedCount += await _userWalletRepository.DeleteAllAsync();
            deletedCount += await _userRightsRepository.DeleteAllAsync();
            deletedCount += await _userIntroduceRepository.DeleteAllAsync();
            deletedCount += await _userRepository.DeleteAllAsync();

            _logger.LogInformation("成功清理 {DeletedCount} 筆假資料", deletedCount);
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理假資料時發生錯誤");
            throw;
        }
    }

    #region 假資料生成輔助方法

    /// <summary>
    /// 生成假姓名
    /// </summary>
    public string GenerateFakeName()
    {
        var surnames = new[] { "王", "李", "張", "劉", "陳", "楊", "黃", "趙", "吳", "周", "徐", "孫", "馬", "朱", "胡", "林", "何", "郭", "高", "羅" };
        var givenNames = new[] { "志明", "淑芬", "建宏", "雅婷", "俊傑", "美玲", "志偉", "淑惠", "建志", "雅惠", "俊豪", "美惠", "志豪", "淑華", "建良", "雅華", "俊傑", "美華", "志強", "淑貞" };
        
        return surnames[Random.Shared.Next(surnames.Length)] + givenNames[Random.Shared.Next(givenNames.Length)];
    }

    /// <summary>
    /// 生成假帳號
    /// </summary>
    public string GenerateFakeAccount()
    {
        var prefix = new[] { "user", "player", "gamer", "member", "account" };
        var suffix = Random.Shared.Next(1000, 9999);
        return prefix[Random.Shared.Next(prefix.Length)] + suffix;
    }

    /// <summary>
    /// 生成假電子郵件
    /// </summary>
    public string GenerateFakeEmail()
    {
        var domains = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "example.com" };
        var username = GenerateFakeAccount();
        var domain = domains[Random.Shared.Next(domains.Length)];
        return $"{username}@{domain}";
    }

    /// <summary>
    /// 生成假暱稱
    /// </summary>
    public string GenerateFakeNickname()
    {
        var nicknames = new[] { "小王子", "遊戲達人", "快樂玩家", "冒險家", "魔法師", "戰士", "弓箭手", "法師", "盜賊", "牧師", "騎士", "獵人", "薩滿", "德魯伊", "術士", "死靈法師", "聖騎士", "野蠻人", "武僧", "遊俠" };
        return nicknames[Random.Shared.Next(nicknames.Length)];
    }

    /// <summary>
    /// 生成假身分證號
    /// </summary>
    public string GenerateFakeIdNumber()
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var letter = letters[Random.Shared.Next(letters.Length)];
        var gender = Random.Shared.Next(1, 3);
        var numbers = Random.Shared.Next(10000000, 99999999);
        return $"{letter}{gender}{numbers:D8}";
    }

    /// <summary>
    /// 生成假手機號碼
    /// </summary>
    public string GenerateFakeCellphone()
    {
        var prefixes = new[] { "0912", "0913", "0914", "0915", "0916", "0917", "0918", "0919", "0920", "0921", "0922", "0923", "0924", "0925", "0926", "0927", "0928", "0929", "0930", "0931" };
        var prefix = prefixes[Random.Shared.Next(prefixes.Length)];
        var suffix = Random.Shared.Next(100000, 999999);
        return $"{prefix}{suffix}";
    }

    /// <summary>
    /// 生成假地址
    /// </summary>
    public string GenerateFakeAddress()
    {
        var cities = new[] { "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市", "基隆市", "新竹市", "嘉義市" };
        var districts = new[] { "中正區", "大安區", "信義區", "松山區", "中山區", "文山區", "南港區", "內湖區", "士林區", "北投區" };
        var roads = new[] { "中山路", "中正路", "民生路", "民權路", "民族路", "仁愛路", "信義路", "和平路", "復興路", "建國路" };
        
        var city = cities[Random.Shared.Next(cities.Length)];
        var district = districts[Random.Shared.Next(districts.Length)];
        var road = roads[Random.Shared.Next(roads.Length)];
        var number = Random.Shared.Next(1, 999);
        var floor = Random.Shared.Next(1, 20);
        
        return $"{city}{district}{road}{number}號{floor}樓";
    }

    /// <summary>
    /// 生成假用戶介紹
    /// </summary>
    public string GenerateFakeUserIntroduce()
    {
        var introductions = new[]
        {
            "我是一個熱愛遊戲的玩家，喜歡探索各種類型的遊戲世界。",
            "平時喜歡和朋友一起玩遊戲，享受團隊合作的樂趣。",
            "對遊戲設計很感興趣，希望能創造出有趣的遊戲內容。",
            "喜歡收集遊戲中的稀有物品，是個收藏家。",
            "擅長策略類遊戲，喜歡思考和分析遊戲機制。",
            "熱愛角色扮演遊戲，喜歡沉浸在故事情節中。",
            "對競技遊戲很有研究，經常參加各種比賽。",
            "喜歡休閒遊戲，在忙碌的生活中尋找放鬆的時刻。",
            "對遊戲音樂很感興趣，喜歡收集遊戲原聲帶。",
            "熱愛多人遊戲，喜歡結交新朋友。"
        };
        
        return introductions[Random.Shared.Next(introductions.Length)];
    }

    /// <summary>
    /// 生成假優惠券號碼
    /// </summary>
    public string GenerateFakeCouponNumber()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = new char[12];
        for (int i = 0; i < 12; i++)
        {
            result[i] = chars[Random.Shared.Next(chars.Length)];
        }
        return new string(result);
    }

    /// <summary>
    /// 生成假銀行帳號
    /// </summary>
    public string GenerateFakeBankAccountNumber()
    {
        var result = "";
        for (int i = 0; i < 16; i++)
        {
            result += Random.Shared.Next(0, 10);
        }
        return result;
    }

    /// <summary>
    /// 生成假審核備註
    /// </summary>
    public string GenerateFakeReviewNotes()
    {
        var notes = new[]
        {
            "資料齊全，審核通過",
            "銀行帳戶資訊正確，核准申請",
            "申請人資格符合要求，同意授權",
            "審核完成，可以開始使用銷售功能",
            "資料完整，符合平台規範",
            "申請通過，歡迎加入銷售行列",
            "審核結果：通過，可以開始銷售",
            "資料齊備，核准銷售權限",
            "申請人背景良好，同意授權",
            "審核完成，銷售功能已啟用"
        };
        
        return notes[Random.Shared.Next(notes.Length)];
    }

    #endregion
}