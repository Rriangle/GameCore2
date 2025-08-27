using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// 資料庫種子資料生成器
/// 用於產生大量測試資料供展示使用
/// </summary>
public class DbSeeder
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(GameCoreDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 執行資料庫種子資料生成
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("開始生成種子資料...");

        // 確保資料庫已建立
        await _context.Database.EnsureCreatedAsync();

        // 檢查是否已有資料
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("資料庫已包含資料，跳過種子資料生成");
            return;
        }

        // 生成測試用戶資料
        await SeedUsersAsync();

        _logger.LogInformation("種子資料生成完成");
    }

    /// <summary>
    /// 生成用戶測試資料
    /// </summary>
    private async Task SeedUsersAsync()
    {
        _logger.LogInformation("生成用戶資料...");

        var random = new Random(12345); // 固定種子確保資料一致
        var users = new List<User>();

        // 預定義的用戶資料
        var predefinedUsers = new[]
        {
            new { Username = "admin", Email = "admin@gamecore.com", FullName = "系統管理員", Nickname = "Admin", Gender = "M" },
            new { Username = "testuser", Email = "test@gamecore.com", FullName = "測試用戶", Nickname = "TestUser", Gender = "F" },
            new { Username = "player1", Email = "player1@gamecore.com", FullName = "玩家一號", Nickname = "Player1", Gender = "M" },
            new { Username = "seller", Email = "seller@gamecore.com", FullName = "商家", Nickname = "Seller", Gender = "F" },
            new { Username = "buyer", Email = "buyer@gamecore.com", FullName = "買家", Nickname = "Buyer", Gender = "M" }
        };

        foreach (var predefined in predefinedUsers)
        {
            var user = CreateTestUser(predefined.Username, predefined.Email, predefined.FullName, 
                predefined.Nickname, predefined.Gender, random);
            users.Add(user);
        }

        // 生成額外的隨機用戶 (500個)
        var firstNames = new[] { "王", "李", "張", "劉", "陳", "楊", "黃", "趙", "周", "吳", "徐", "孫", "胡", "朱", "高", "林", "何", "郭", "馬", "羅" };
        var lastNames = new[] { "小明", "小華", "小美", "小強", "小芳", "大明", "大華", "志明", "志偉", "怡君", "淑芬", "美玲", "雅婷", "佳玲", "俊宏", "建國", "文雄", "慧琳", "麗華", "秀美" };
        var genders = new[] { "M", "F" };

        for (int i = 1; i <= 500; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var fullName = firstName + lastName;
            var username = $"user{i:D3}";
            var email = $"user{i:D3}@example.com";
            var nickname = fullName;
            var gender = genders[random.Next(genders.Length)];

            var user = CreateTestUser(username, email, fullName, nickname, gender, random);
            users.Add(user);
        }

        // 批次插入資料
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已生成 {Count} 個用戶資料", users.Count);
    }

    /// <summary>
    /// 建立測試用戶 (含完整關聯資料)
    /// </summary>
    private User CreateTestUser(string username, string email, string fullName, string nickname, string gender, Random random)
    {
        var birthYear = random.Next(1970, 2005);
        var birthMonth = random.Next(1, 13);
        var birthDay = random.Next(1, 28);
        var dateOfBirth = new DateTime(birthYear, birthMonth, birthDay);

        var user = new User
        {
            User_name = fullName,
            User_Account = username,
            User_Password = BC.HashPassword("123456"), // 所有測試用戶密碼都是 123456
            UserIntroduce = new UserIntroduce
            {
                User_NickName = nickname,
                Gender = gender,
                IdNumber = GenerateRandomIdNumber(random),
                Cellphone = GenerateRandomPhone(random),
                Email = email,
                Address = GenerateRandomAddress(random),
                DateOfBirth = dateOfBirth,
                Create_Account = DateTime.UtcNow.AddDays(-random.Next(365)),
                User_Introduce = GenerateRandomIntroduction(random)
            },
            UserRights = new UserRights
            {
                User_Status = true,
                ShoppingPermission = true,
                MessagePermission = random.NextDouble() > 0.1, // 90% 可留言
                SalesAuthority = random.NextDouble() > 0.7 // 30% 可銷售
            },
            Wallet = new UserWallet
            {
                User_Point = random.Next(0, 10000), // 隨機點數 0-10000
                Coupon_Number = random.NextDouble() > 0.8 ? GenerateRandomCoupon(random) : null // 20% 有優惠券
            }
        };

        return user;
    }

    /// <summary>
    /// 生成隨機身分證字號
    /// </summary>
    private string GenerateRandomIdNumber(Random random)
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var firstLetter = letters[random.Next(letters.Length)];
        var genderDigit = random.Next(1, 3); // 1=男, 2=女
        var serialNumber = random.Next(1000000, 9999999);
        return $"{firstLetter}{genderDigit}{serialNumber:D7}";
    }

    /// <summary>
    /// 生成隨機電話號碼
    /// </summary>
    private string GenerateRandomPhone(Random random)
    {
        var prefix = new[] { "0912", "0922", "0932", "0952", "0972", "0982", "0987" };
        var selectedPrefix = prefix[random.Next(prefix.Length)];
        var number = random.Next(100000, 999999);
        return $"{selectedPrefix}{number:D6}";
    }

    /// <summary>
    /// 生成隨機地址
    /// </summary>
    private string GenerateRandomAddress(Random random)
    {
        var cities = new[] { "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市", "新竹市", "基隆市" };
        var districts = new[] { "中正區", "大安區", "信義區", "松山區", "中山區", "萬華區", "士林區", "北投區" };
        var roads = new[] { "中山路", "民生路", "建國路", "仁愛路", "忠孝路", "和平路", "信義路", "復興路" };

        var city = cities[random.Next(cities.Length)];
        var district = districts[random.Next(districts.Length)];
        var road = roads[random.Next(roads.Length)];
        var number = random.Next(1, 999);
        var floor = random.Next(1, 20);

        return $"{city}{district}{road}{number}號{floor}樓";
    }

    /// <summary>
    /// 生成隨機自我介紹
    /// </summary>
    private string GenerateRandomIntroduction(Random random)
    {
        var introductions = new[]
        {
            "喜歡玩遊戲，特別是RPG和策略類遊戲。",
            "遊戲愛好者，喜歡探索新的遊戲世界。",
            "資深玩家，對各種遊戲都有涉獵。",
            "新手玩家，正在學習中。",
            "業餘遊戲開發者，喜歡研究遊戲機制。",
            "競技遊戲愛好者，追求技術提升。",
            "休閒玩家，享受遊戲帶來的放鬆。",
            "收集控，喜歡收集遊戲道具。",
            "社交型玩家，喜歡和朋友一起玩。",
            null // 有些用戶沒有自介
        };

        return introductions[random.Next(introductions.Length)];
    }

    /// <summary>
    /// 生成隨機優惠券編號
    /// </summary>
    private string GenerateRandomCoupon(Random random)
    {
        var prefixes = new[] { "NEW", "VIP", "HOT", "SAVE", "GIFT" };
        var prefix = prefixes[random.Next(prefixes.Length)];
        var number = random.Next(1000, 9999);
        return $"{prefix}{number}";
    }
}