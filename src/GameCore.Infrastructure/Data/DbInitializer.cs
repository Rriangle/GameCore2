using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// 資料庫初始化和種子資料類別
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// 初始化資料庫並填入種子資料
    /// </summary>
    public static async Task InitializeAsync(GameCoreDbContext context, ILogger logger)
    {
        try
        {
            // 確保資料庫已建立
            await context.Database.EnsureCreatedAsync();

            // 如果已有資料，就不再新增種子資料
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("資料庫已有資料，跳過種子資料建立");
                return;
            }

            logger.LogInformation("開始建立種子資料...");

            // 建立測試用戶
            var users = new List<User>
            {
                new User
                {
                    User_name = "管理員",
                    User_Account = "admin@gamecore.com",
                    User_Password = HashPassword("admin123")
                },
                new User
                {
                    User_name = "測試用戶1",
                    User_Account = "user1@gamecore.com",
                    User_Password = HashPassword("user123")
                },
                new User
                {
                    User_name = "測試用戶2",
                    User_Account = "user2@gamecore.com",
                    User_Password = HashPassword("user123")
                },
                new User
                {
                    User_name = "Demo User",
                    User_Account = "demo@gamecore.com",
                    User_Password = HashPassword("demo123")
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {users.Count} 個測試用戶");

            // 建立用戶錢包
            var wallets = new List<UserWallet>();
            for (int i = 1; i <= users.Count; i++)
            {
                wallets.Add(new UserWallet
                {
                    User_Id = i,
                    User_Point = i == 1 ? 10000 : 1000 + (i * 100), // 管理員有更多點數
                    Coupon_Number = i == 2 ? "WELCOME2024" : null // 給第二個用戶一張優惠券
                });
            }

            await context.UserWallets.AddRangeAsync(wallets);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {wallets.Count} 個用戶錢包");

            // 建立用戶介紹資料
            var userIntroduces = new List<UserIntroduce>
            {
                new UserIntroduce
                {
                    User_ID = 1,
                    User_NickName = "SuperAdmin",
                    Gender = "M",
                    IdNumber = "A123456789",
                    Cellphone = "0912345678",
                    Email = "admin@gamecore.com",
                    Address = "台北市信義區",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Create_Account = DateTime.UtcNow,
                    User_Introduce = "系統管理員帳號"
                },
                new UserIntroduce
                {
                    User_ID = 2,
                    User_NickName = "TestUser1",
                    Gender = "F",
                    IdNumber = "B234567890",
                    Cellphone = "0923456789",
                    Email = "user1@gamecore.com",
                    Address = "台中市西區",
                    DateOfBirth = new DateTime(1995, 6, 15),
                    Create_Account = DateTime.UtcNow,
                    User_Introduce = "我是測試用戶1，喜歡玩遊戲！"
                },
                new UserIntroduce
                {
                    User_ID = 3,
                    User_NickName = "TestUser2",
                    Gender = "M",
                    IdNumber = "C345678901",
                    Cellphone = "0934567890",
                    Email = "user2@gamecore.com",
                    Address = "高雄市前金區",
                    DateOfBirth = new DateTime(1992, 12, 3),
                    Create_Account = DateTime.UtcNow,
                    User_Introduce = "熱愛電競的玩家"
                },
                new UserIntroduce
                {
                    User_ID = 4,
                    User_NickName = "DemoPlayer",
                    Gender = "F",
                    IdNumber = "D456789012",
                    Cellphone = "0945678901",
                    Email = "demo@gamecore.com",
                    Address = "新北市板橋區",
                    DateOfBirth = new DateTime(1998, 8, 20),
                    Create_Account = DateTime.UtcNow,
                    User_Introduce = "Demo 展示帳號"
                }
            };

            await context.UserIntroduces.AddRangeAsync(userIntroduces);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {userIntroduces.Count} 個用戶介紹資料");

            // 建立用戶權限
            var userRights = new List<UserRights>();
            for (int i = 1; i <= users.Count; i++)
            {
                userRights.Add(new UserRights
                {
                    User_Id = i,
                    User_Status = true,
                    ShoppingPermission = true,
                    MessagePermission = true,
                    SalesAuthority = i <= 2 // 只有前兩個用戶有銷售權限
                });
            }

            await context.UserRights.AddRangeAsync(userRights);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {userRights.Count} 個用戶權限設定");

            // 建立銷售資料（針對有銷售權限的用戶）
            var salesProfiles = new List<MemberSalesProfile>
            {
                new MemberSalesProfile
                {
                    User_Id = 1,
                    BankCode = 822,
                    BankAccountNumber = "1234567890123",
                    AccountCoverPhoto = null
                },
                new MemberSalesProfile
                {
                    User_Id = 2,
                    BankCode = 700,
                    BankAccountNumber = "9876543210987",
                    AccountCoverPhoto = null
                }
            };

            await context.MemberSalesProfiles.AddRangeAsync(salesProfiles);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {salesProfiles.Count} 個銷售檔案");

            // 建立銷售錢包
            var salesInformation = new List<UserSalesInformation>
            {
                new UserSalesInformation
                {
                    User_Id = 1,
                    UserSales_Wallet = 5000
                },
                new UserSalesInformation
                {
                    User_Id = 2,
                    UserSales_Wallet = 2500
                }
            };

            await context.UserSalesInformation.AddRangeAsync(salesInformation);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {salesInformation.Count} 個銷售錢包");

            // 建立一些寵物資料
            var pets = new List<Pet>
            {
                new Pet
                {
                    UserID = 1,
                    PetName = "管理員的史萊姆"
                },
                new Pet
                {
                    UserID = 2,
                    PetName = "小藍"
                },
                new Pet
                {
                    UserID = 3,
                    PetName = "戰鬥史萊姆"
                },
                new Pet
                {
                    UserID = 4,
                    PetName = "Demo寵物"
                }
            };

            await context.Pets.AddRangeAsync(pets);
            await context.SaveChangesAsync();

            logger.LogInformation($"已建立 {pets.Count} 個寵物");

            logger.LogInformation("種子資料建立完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "建立種子資料時發生錯誤: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 密碼雜湊處理（與 AuthService 一致）
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "GameCore_Salt_2024"));
        return Convert.ToBase64String(hashedBytes);
    }
}