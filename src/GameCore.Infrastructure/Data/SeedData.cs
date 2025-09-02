using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Data;

/// <summary>
/// 資料庫種子資料
/// 為 Stage 1 提供基本的測試資料
/// </summary>
public static class SeedData
{
    /// <summary>
    /// 初始化基本資料
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    public static async Task InitializeAsync(GameCoreDbContext context)
    {
        // 確保資料庫已建立
        await context.Database.EnsureCreatedAsync();

        // 檢查是否已有資料
        if (await context.Users.AnyAsync())
        {
            return; // 已有資料，跳過初始化
        }

        // 建立測試使用者
        var testUser = new User
        {
            User_Name = "測試使用者",
            User_Account = "testuser",
            User_Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=" // SHA256 hash of "password123"
        };

        context.Users.Add(testUser);
        await context.SaveChangesAsync();

        // 建立使用者介紹
        var userIntroduce = new UserIntroduce
        {
            User_ID = testUser.User_ID,
            User_NickName = "測試暱稱",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市測試區測試路123號",
            DateOfBirth = new DateTime(1990, 1, 1),
            Create_Account = DateTime.UtcNow,
            User_Introduce = "這是一個測試使用者帳號，用於演示 Stage 1 的認證功能。"
        };

        context.UserIntroduces.Add(userIntroduce);

        // 建立使用者權限
        var userRights = new UserRights
        {
            User_Id = testUser.User_ID,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false
        };

        context.UserRights.Add(userRights);

        // 建立使用者錢包
        var userWallet = new UserWallet
        {
            User_Id = testUser.User_ID,
            User_Point = 1000,
            Coupon_Number = null
        };

        context.UserWallets.Add(userWallet);

        // 建立管理員帳號
        var adminUser = new User
        {
            User_Name = "系統管理員",
            User_Account = "admin",
            User_Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=" // SHA256 hash of "password123"
        };

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        var adminIntroduce = new UserIntroduce
        {
            User_ID = adminUser.User_ID,
            User_NickName = "管理員",
            Gender = "M",
            IdNumber = "B987654321",
            Cellphone = "0987654321",
            Email = "admin@gamecore.com",
            Address = "台北市管理區管理路999號",
            DateOfBirth = new DateTime(1985, 5, 15),
            Create_Account = DateTime.UtcNow,
            User_Introduce = "系統管理員帳號，擁有最高權限。"
        };

        context.UserIntroduces.Add(adminIntroduce);

        var adminRights = new UserRights
        {
            User_Id = adminUser.User_ID,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = true
        };

        context.UserRights.Add(adminRights);

        var adminWallet = new UserWallet
        {
            User_Id = adminUser.User_ID,
            User_Point = 9999,
            Coupon_Number = null
        };

        context.UserWallets.Add(adminWallet);

        // 儲存所有變更
        await context.SaveChangesAsync();

        Console.WriteLine("資料庫種子資料初始化完成！");
        Console.WriteLine($"已建立 {await context.Users.CountAsync()} 個使用者");
        Console.WriteLine("測試帳號：testuser / password123");
        Console.WriteLine("管理員帳號：admin / password123");
    }
} 