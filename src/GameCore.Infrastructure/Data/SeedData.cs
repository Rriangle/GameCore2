using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// 資料庫種子資料生成器
/// 生成大量假資料用於開發和展示
/// </summary>
public static class SeedData
{
    /// <summary>
    /// 初始化種子資料
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    public static async Task InitializeAsync(GameCoreDbContext context)
    {
        // 確保資料庫已建立
        await context.Database.EnsureCreatedAsync();

        // 如果已有使用者資料，跳過種子資料生成
        if (await context.Users.AnyAsync())
        {
            return;
        }

        Console.WriteLine("開始生成種子資料...");

        // 生成使用者資料
        await SeedUsersAsync(context);

        Console.WriteLine("種子資料生成完成！");
    }

    /// <summary>
    /// 生成使用者相關種子資料
    /// </summary>
    private static async Task SeedUsersAsync(GameCoreDbContext context)
    {
        Console.WriteLine("正在生成使用者資料...");

        var random = new Random(12345); // 固定種子確保可重現性
        var users = new List<User>();
        var userIntroduces = new List<UserIntroduce>();
        var userRights = new List<UserRights>();
        var userWallets = new List<UserWallet>();
        var memberSalesProfiles = new List<MemberSalesProfile>();
        var userSalesInformations = new List<UserSalesInformation>();

        // 預定義的假資料集合
        var firstNames = new[]
        {
            "王", "李", "張", "劉", "陳", "楊", "趙", "黃", "周", "吳",
            "徐", "孫", "胡", "朱", "高", "林", "何", "郭", "馬", "羅",
            "梁", "宋", "鄭", "謝", "韓", "唐", "馮", "于", "董", "蕭"
        };

        var secondNames = new[]
        {
            "志明", "春嬌", "小華", "小明", "美玲", "淑芬", "建國", "雅婷",
            "怡君", "俊宏", "佳蓉", "文華", "淑惠", "志強", "麗華", "家豪",
            "雅雯", "宗翰", "佩君", "承翰", "欣怡", "家維", "思妤", "宜庭",
            "昱成", "品妤", "家銘", "品妍", "昱廷", "思穎", "家瑋", "欣妤"
        };

        var cities = new[]
        {
            "台北市", "新北市", "桃園市", "台中市", "台南市", "高雄市",
            "基隆市", "新竹市", "嘉義市", "宜蘭縣", "新竹縣", "苗栗縣",
            "彰化縣", "南投縣", "雲林縣", "嘉義縣", "屏東縣", "花蓮縣",
            "台東縣", "澎湖縣", "金門縣", "連江縣"
        };

        var districts = new[]
        {
            "中正區", "大同區", "中山區", "松山區", "大安區", "萬華區",
            "信義區", "士林區", "北投區", "內湖區", "南港區", "文山區",
            "板橋區", "三重區", "中和區", "永和區", "新莊區", "新店區",
            "樹林區", "鶯歌區", "三峽區", "淡水區", "汐止區", "瑞芳區"
        };

        var bankCodes = new[] { 700, 808, 822, 812, 803, 700, 011, 012, 013, 017, 021, 050, 052, 053, 081, 103, 104, 108, 147, 803 };

        // 生成 1500 個使用者（超過1000筆的要求）
        for (int i = 1; i <= 1500; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var secondName = secondNames[random.Next(secondNames.Length)];
            var userName = $"{firstName}{secondName}";
            var userAccount = $"user{i:D4}";
            
            // 確保唯一性
            if (users.Any(u => u.UserName == userName))
            {
                userName += i.ToString();
            }

            // 創建使用者基本資料
            var user = new User
            {
                UserId = i,
                UserName = userName,
                UserAccount = userAccount,
                UserPassword = BC.HashPassword("password123") // 統一密碼便於測試
            };
            users.Add(user);

            // 創建使用者介紹資料
            var birthDate = DateTime.Now.AddYears(-random.Next(18, 65))
                                       .AddMonths(-random.Next(1, 12))
                                       .AddDays(-random.Next(1, 28));

            var city = cities[random.Next(cities.Length)];
            var district = districts[random.Next(districts.Length)];

            var userIntroduce = new UserIntroduce
            {
                UserId = i,
                UserNickName = $"{userName}_{i}",
                Gender = random.Next(2) == 0 ? "M" : "F",
                IdNumber = GenerateIdNumber(random),
                Cellphone = $"09{random.Next(10000000, 99999999)}",
                Email = $"{userAccount}@example.com",
                Address = $"{city}{district}{GenerateAddress(random)}",
                DateOfBirth = birthDate,
                CreateAccount = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                UserIntroduceText = GenerateUserIntroduction(random, userName)
            };
            userIntroduces.Add(userIntroduce);

            // 創建使用者權限
            var userRights = new UserRights
            {
                UserId = i,
                UserStatus = random.Next(100) > 5, // 95% 的使用者是啟用狀態
                ShoppingPermission = random.Next(100) > 10, // 90% 有購物權限
                MessagePermission = random.Next(100) > 5, // 95% 有留言權限
                SalesAuthority = random.Next(100) > 70 // 30% 有銷售權限
            };
            userRights.Add(userRights);

            // 創建使用者錢包
            var userWallet = new UserWallet
            {
                UserId = i,
                UserPoint = random.Next(0, 10000), // 0-10000 點數
                CouponNumber = random.Next(100) > 80 ? $"COUPON{random.Next(1000, 9999)}" : null // 20% 有優惠券
            };
            userWallets.Add(userWallet);

            // 30% 的使用者申請銷售功能
            if (userRights.SalesAuthority)
            {
                var memberSalesProfile = new MemberSalesProfile
                {
                    UserId = i,
                    BankCode = bankCodes[random.Next(bankCodes.Length)],
                    BankAccountNumber = GenerateBankAccount(random),
                    AccountCoverPhoto = null // 暫不生成實際圖片資料
                };
                memberSalesProfiles.Add(memberSalesProfile);

                var userSalesInformation = new UserSalesInformation
                {
                    UserId = i,
                    UserSalesWallet = random.Next(0, 50000) // 0-50000 銷售收入
                };
                userSalesInformations.Add(userSalesInformation);
            }

            if (i % 100 == 0)
            {
                Console.WriteLine($"已生成 {i} 個使用者資料...");
            }
        }

        // 批量插入資料
        Console.WriteLine("正在插入使用者基本資料...");
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        Console.WriteLine("正在插入使用者介紹資料...");
        context.UserIntroduces.AddRange(userIntroduces);
        await context.SaveChangesAsync();

        Console.WriteLine("正在插入使用者權限資料...");
        context.UserRights.AddRange(userRights);
        await context.SaveChangesAsync();

        Console.WriteLine("正在插入使用者錢包資料...");
        context.UserWallets.AddRange(userWallets);
        await context.SaveChangesAsync();

        Console.WriteLine("正在插入會員銷售資料...");
        context.MemberSalesProfiles.AddRange(memberSalesProfiles);
        await context.SaveChangesAsync();

        Console.WriteLine("正在插入使用者銷售資訊...");
        context.UserSalesInformations.AddRange(userSalesInformations);
        await context.SaveChangesAsync();

        Console.WriteLine($"使用者資料生成完成！總計：");
        Console.WriteLine($"- 使用者: {users.Count}");
        Console.WriteLine($"- 使用者介紹: {userIntroduces.Count}");
        Console.WriteLine($"- 使用者權限: {userRights.Count}");
        Console.WriteLine($"- 使用者錢包: {userWallets.Count}");
        Console.WriteLine($"- 會員銷售資料: {memberSalesProfiles.Count}");
        Console.WriteLine($"- 使用者銷售資訊: {userSalesInformations.Count}");
    }

    /// <summary>
    /// 生成假的身分證字號
    /// </summary>
    private static string GenerateIdNumber(Random random)
    {
        var firstChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[random.Next(26)];
        var gender = random.Next(2) + 1; // 1 or 2
        var numbers = "";
        for (int i = 0; i < 8; i++)
        {
            numbers += random.Next(10).ToString();
        }
        return $"{firstChar}{gender}{numbers}";
    }

    /// <summary>
    /// 生成假的地址
    /// </summary>
    private static string GenerateAddress(Random random)
    {
        var roads = new[] { "中山路", "中正路", "民生路", "民族路", "建國路", "復興路", "和平路", "忠孝路", "仁愛路", "信義路" };
        var road = roads[random.Next(roads.Length)];
        var number = random.Next(1, 500);
        var floor = random.Next(100) > 70 ? $"{random.Next(1, 15)}樓" : "";
        return $"{road}{number}號{floor}";
    }

    /// <summary>
    /// 生成使用者自我介紹
    /// </summary>
    private static string GenerateUserIntroduction(Random random, string userName)
    {
        var introTemplates = new[]
        {
            $"大家好，我是{userName}！喜歡玩遊戲和看動漫。",
            $"Hello！{userName}在此，熱愛電競和科技。",
            $"嗨～我是{userName}，喜歡交朋友，歡迎大家找我聊天！",
            $"{userName}報到！是個遊戲狂熱者，什麼遊戲都愛玩。",
            $"我是{userName}，平常喜歡研究新遊戲和分享心得。",
            null, null, null // 30% 沒有自我介紹
        };

        return introTemplates[random.Next(introTemplates.Length)];
    }

    /// <summary>
    /// 生成假的銀行帳號
    /// </summary>
    private static string GenerateBankAccount(Random random)
    {
        var accountNumber = "";
        for (int i = 0; i < 12; i++)
        {
            accountNumber += random.Next(10).ToString();
        }
        return accountNumber;
    }
}