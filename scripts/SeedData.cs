using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameCore.Scripts;

public class SeedData
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<SeedData> _logger;

    public SeedData(GameCoreDbContext context, ILogger<SeedData> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAllDataAsync()
    {
        _logger.LogInformation("開始填充測試資料...");

        // 清空現有資料
        await ClearAllDataAsync();

        // 填充用戶資料
        var users = await SeedUsersAsync();

        // 填充供應商資料
        var suppliers = await SeedSuppliersAsync();

        // 填充商店商品
        await SeedStoreProductsAsync(suppliers);

        // 填充寵物資料
        await SeedPetsAsync(users);

        // 填充簽到記錄
        await SeedSignInStatsAsync(users);

        // 填充玩家市場商品
        await SeedMarketItemsAsync(users);

        // 填充論壇資料
        await SeedForumDataAsync(users);

        // 填充通知資料
        await SeedNotificationsAsync(users);

        // 填充群組資料
        await SeedGroupDataAsync(users);

        // 填充小遊戲記錄
        await SeedMiniGameRecordsAsync(users);

        _logger.LogInformation("測試資料填充完成！");
    }

    private async Task ClearAllDataAsync()
    {
        _logger.LogInformation("清空現有資料...");

        _context.MiniGames.RemoveRange(_context.MiniGames);
        _context.GroupConversations.RemoveRange(_context.GroupConversations);
        _context.GroupMembers.RemoveRange(_context.GroupMembers);
        _context.Groups.RemoveRange(_context.Groups);
        _context.BlockedUsers.RemoveRange(_context.BlockedUsers);
        _context.Notifications.RemoveRange(_context.Notifications);
        _context.ForumBookmarks.RemoveRange(_context.ForumBookmarks);
        _context.ForumReactions.RemoveRange(_context.ForumReactions);
        _context.ForumThreadPosts.RemoveRange(_context.ForumThreadPosts);
        _context.ForumThreads.RemoveRange(_context.ForumThreads);
        _context.ForumPosts.RemoveRange(_context.ForumPosts);
        _context.PlayerOwnedToPlayers.RemoveRange(_context.PlayerOwnedToPlayers);
        _context.PlayerOwneds.RemoveRange(_context.PlayerOwneds);
        _context.OrderInfos.RemoveRange(_context.OrderInfos);
        _context.StoreProducts.RemoveRange(_context.StoreProducts);
        _context.Suppliers.RemoveRange(_context.Suppliers);
        _context.UserSignInStats.RemoveRange(_context.UserSignInStats);
        _context.Pets.RemoveRange(_context.Pets);
        _context.UserWallets.RemoveRange(_context.UserWallets);
        _context.Users.RemoveRange(_context.Users);

        await _context.SaveChangesAsync();
    }

    private async Task<List<User>> SeedUsersAsync()
    {
        _logger.LogInformation("填充用戶資料...");

        var users = new List<User>
        {
            new User
            {
                Username = "normaluser",
                Email = "normal@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                PhoneNumber = "0912345678",
                AvatarUrl = "https://example.com/avatar1.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLoginAt = DateTime.UtcNow.AddHours(-2)
            },
            new User
            {
                Username = "banneduser",
                Email = "banned@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                PhoneNumber = "0923456789",
                AvatarUrl = "https://example.com/avatar2.jpg",
                IsActive = false,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                LastLoginAt = DateTime.UtcNow.AddDays(-5)
            },
            new User
            {
                Username = "selleruser",
                Email = "seller@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                PhoneNumber = "0934567890",
                AvatarUrl = "https://example.com/avatar3.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                LastLoginAt = DateTime.UtcNow.AddHours(-1)
            },
            new User
            {
                Username = "buyeruser",
                Email = "buyer@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                PhoneNumber = "0945678901",
                AvatarUrl = "https://example.com/avatar4.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                LastLoginAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new User
            {
                Username = "adminuser",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                PhoneNumber = "0956789012",
                AvatarUrl = "https://example.com/avatar5.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                LastLoginAt = DateTime.UtcNow.AddMinutes(-10)
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // 為每個用戶創建錢包
        var wallets = users.Select(u => new UserWallet
        {
            UserId = u.UserId,
            Balance = u.Username switch
            {
                "normaluser" => 500.00m,
                "banneduser" => 0.00m,
                "selleruser" => 2000.00m,
                "buyeruser" => 1500.00m,
                "adminuser" => 10000.00m,
                _ => 100.00m
            },
            CreatedAt = u.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        _context.UserWallets.AddRange(wallets);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {users.Count} 個用戶");
        return users;
    }

    private async Task<List<Supplier>> SeedSuppliersAsync()
    {
        _logger.LogInformation("填充供應商資料...");

        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Name = "遊戲世界供應商",
                Address = "台北市信義區信義路五段7號",
                Phone = "02-23456789",
                Email = "gameworld@supplier.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow
            },
            new Supplier
            {
                Name = "數位科技供應商",
                Address = "新北市板橋區文化路一段100號",
                Phone = "02-34567890",
                Email = "digitaltech@supplier.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-55),
                UpdatedAt = DateTime.UtcNow
            },
            new Supplier
            {
                Name = "生活用品供應商",
                Address = "台中市西區台灣大道二段200號",
                Phone = "04-45678901",
                Email = "lifestyle@supplier.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-50),
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Suppliers.AddRange(suppliers);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {suppliers.Count} 個供應商");
        return suppliers;
    }

    private async Task SeedStoreProductsAsync(List<Supplier> suppliers)
    {
        _logger.LogInformation("填充商店商品...");

        var products = new List<StoreProduct>
        {
            // 遊戲商品
            new StoreProduct
            {
                SupplierId = suppliers[0].SupplierId,
                Name = "超級瑪利歐遊戲卡帶",
                Description = "經典任天堂遊戲，包含所有關卡和角色",
                Price = 1500.00m,
                Stock = 50,
                Category = "game",
                ImageUrl = "https://example.com/mario.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-40),
                UpdatedAt = DateTime.UtcNow
            },
            new StoreProduct
            {
                SupplierId = suppliers[0].SupplierId,
                Name = "寶可夢劍盾遊戲",
                Description = "最新寶可夢系列遊戲，探索伽勒爾地區",
                Price = 1800.00m,
                Stock = 30,
                Category = "game",
                ImageUrl = "https://example.com/pokemon.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-35),
                UpdatedAt = DateTime.UtcNow
            },
            // 非遊戲商品
            new StoreProduct
            {
                SupplierId = suppliers[1].SupplierId,
                Name = "無線藍牙耳機",
                Description = "高音質無線耳機，支援降噪功能",
                Price = 2500.00m,
                Stock = 25,
                Category = "non-game",
                ImageUrl = "https://example.com/headphones.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            },
            new StoreProduct
            {
                SupplierId = suppliers[2].SupplierId,
                Name = "遊戲主題T恤",
                Description = "舒適透氣的遊戲主題服飾",
                Price = 800.00m,
                Stock = 100,
                Category = "non-game",
                ImageUrl = "https://example.com/tshirt.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.StoreProducts.AddRange(products);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {products.Count} 個商店商品");
    }

    private async Task SeedPetsAsync(List<User> users)
    {
        _logger.LogInformation("填充寵物資料...");

        var pets = new List<Pet>
        {
            // 正常用戶的寵物
            new Pet
            {
                UserId = users[0].UserId,
                Name = "小可愛",
                Level = 5,
                Exp = 150,
                Hunger = 85,
                Mood = 90,
                Stamina = 80,
                Cleanliness = 75,
                Health = 70,
                SkinColor = "default",
                CreatedAt = users[0].CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                LastDecayAt = DateTime.UtcNow.AddDays(-1)
            },
            // 被禁用戶的寵物
            new Pet
            {
                UserId = users[1].UserId,
                Name = "小可憐",
                Level = 1,
                Exp = 0,
                Hunger = 0,
                Mood = 0,
                Stamina = 0,
                Cleanliness = 0,
                Health = 0,
                SkinColor = "default",
                CreatedAt = users[1].CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                LastDecayAt = DateTime.UtcNow.AddDays(-5)
            },
            // 賣家用戶的寵物
            new Pet
            {
                UserId = users[2].UserId,
                Name = "小商人",
                Level = 10,
                Exp = 500,
                Hunger = 100,
                Mood = 100,
                Stamina = 100,
                Cleanliness = 100,
                Health = 100,
                SkinColor = "gold",
                CreatedAt = users[2].CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                LastDecayAt = DateTime.UtcNow
            },
            // 買家用戶的寵物
            new Pet
            {
                UserId = users[3].UserId,
                Name = "小買家",
                Level = 3,
                Exp = 80,
                Hunger = 60,
                Mood = 70,
                Stamina = 65,
                Cleanliness = 55,
                Health = 50,
                SkinColor = "blue",
                CreatedAt = users[3].CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                LastDecayAt = DateTime.UtcNow.AddDays(-2)
            },
            // 管理員的寵物
            new Pet
            {
                UserId = users[4].UserId,
                Name = "小管理員",
                Level = 20,
                Exp = 2000,
                Hunger = 100,
                Mood = 100,
                Stamina = 100,
                Cleanliness = 100,
                Health = 100,
                SkinColor = "rainbow",
                CreatedAt = users[4].CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                LastDecayAt = DateTime.UtcNow
            }
        };

        _context.Pets.AddRange(pets);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {pets.Count} 個寵物");
    }

    private async Task SeedSignInStatsAsync(List<User> users)
    {
        _logger.LogInformation("填充簽到記錄...");

        var signInStats = new List<UserSignInStats>();
        var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei");
        var today = TimeZoneInfo.ConvertTime(DateTime.UtcNow, taipeiTimeZone).Date;

        foreach (var user in users.Where(u => u.IsActive))
        {
            // 為每個活躍用戶創建過去7天的簽到記錄
            for (int i = 6; i >= 0; i--)
            {
                var signInDate = today.AddDays(-i);
                var isWeekend = signInDate.DayOfWeek == DayOfWeek.Saturday || signInDate.DayOfWeek == DayOfWeek.Sunday;
                var streak = 7 - i;
                var isStreakBonus = streak == 7;
                var isMonthBonus = signInDate.Day == DateTime.DaysInMonth(signInDate.Year, signInDate.Month) && streak >= 28;

                var basePoints = isWeekend ? 30 : 20;
                var baseExp = isWeekend ? 200 : 0;
                var streakBonus = isStreakBonus ? 40 : 0;
                var monthBonus = isMonthBonus ? 200 : 0;
                var monthExpBonus = isMonthBonus ? 2000 : 0;

                var totalPoints = basePoints + streakBonus + monthBonus;
                var totalExp = baseExp + monthExpBonus;

                signInStats.Add(new UserSignInStats
                {
                    UserId = user.UserId,
                    SignInDate = signInDate,
                    PointsEarned = totalPoints,
                    ExpEarned = totalExp,
                    StreakCount = streak,
                    CreatedAt = signInDate
                });
            }
        }

        _context.UserSignInStats.AddRange(signInStats);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {signInStats.Count} 條簽到記錄");
    }

    private async Task SeedMarketItemsAsync(List<User> users)
    {
        _logger.LogInformation("填充玩家市場商品...");

        var marketItems = new List<PlayerOwned>
        {
            // 活躍商品
            new PlayerOwned
            {
                UserId = users[2].UserId, // selleruser
                Name = "稀有遊戲卡牌",
                Description = "限量版遊戲卡牌，收藏價值極高",
                Price = 5000.00m,
                Status = "Active",
                ImageUrl = "https://example.com/card1.jpg",
                Category = "collectible",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow
            },
            new PlayerOwned
            {
                UserId = users[0].UserId, // normaluser
                Name = "二手遊戲主機",
                Description = "九成新遊戲主機，功能正常",
                Price = 8000.00m,
                Status = "Active",
                ImageUrl = "https://example.com/console.jpg",
                Category = "electronics",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow
            },
            // 已售出商品
            new PlayerOwned
            {
                UserId = users[2].UserId,
                Name = "遊戲周邊商品",
                Description = "各種遊戲周邊，品質優良",
                Price = 1200.00m,
                Status = "Sold",
                ImageUrl = "https://example.com/accessories.jpg",
                Category = "accessories",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow,
                SoldAt = DateTime.UtcNow.AddDays(-5)
            },
            // 已取消商品
            new PlayerOwned
            {
                UserId = users[3].UserId, // buyeruser
                Name = "遊戲攻略本",
                Description = "詳細的遊戲攻略指南",
                Price = 300.00m,
                Status = "Cancelled",
                ImageUrl = "https://example.com/guide.jpg",
                Category = "books",
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.PlayerOwneds.AddRange(marketItems);
        await _context.SaveChangesAsync();

        // 創建一些交易記錄
        var transactions = new List<PlayerOwnedToPlayer>
        {
            new PlayerOwnedToPlayer
            {
                ItemId = marketItems[2].ItemId, // 已售出的商品
                SellerId = users[2].UserId,
                BuyerId = users[3].UserId,
                Price = 1200.00m,
                PlatformFee = 60.00m,
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                SellerConfirmedAt = DateTime.UtcNow.AddDays(-4),
                BuyerConfirmedAt = DateTime.UtcNow.AddDays(-4),
                CompletedAt = DateTime.UtcNow.AddDays(-4),
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            }
        };

        _context.PlayerOwnedToPlayers.AddRange(transactions);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {marketItems.Count} 個市場商品和 {transactions.Count} 筆交易");
    }

    private async Task SeedForumDataAsync(List<User> users)
    {
        _logger.LogInformation("填充論壇資料...");

        // 創建論壇帖子
        var posts = new List<ForumPost>
        {
            new ForumPost
            {
                UserId = users[0].UserId,
                Title = "新手玩家求助",
                Content = "我是新手玩家，請問如何快速升級寵物？",
                Status = "published",
                ViewCount = 45,
                LikeCount = 8,
                ReplyCount = 3,
                PublishedAt = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new ForumPost
            {
                UserId = users[2].UserId,
                Title = "遊戲攻略分享",
                Content = "分享一些遊戲技巧和攻略心得",
                Status = "published",
                ViewCount = 120,
                LikeCount = 25,
                ReplyCount = 7,
                PublishedAt = DateTime.UtcNow.AddDays(-3),
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _context.ForumPosts.AddRange(posts);
        await _context.SaveChangesAsync();

        // 創建論壇主題
        var threads = new List<ForumThread>
        {
            new ForumThread
            {
                PostId = posts[0].PostId,
                UserId = users[1].UserId,
                Title = "Re: 新手玩家求助",
                Content = "建議先完成每日任務，這樣可以獲得很多經驗值",
                Status = "normal",
                ViewCount = 20,
                LikeCount = 5,
                ReplyCount = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            }
        };

        _context.ForumThreads.AddRange(threads);
        await _context.SaveChangesAsync();

        // 創建主題回覆
        var threadPosts = new List<ForumThreadPost>
        {
            new ForumThreadPost
            {
                ThreadId = threads[0].ThreadId,
                UserId = users[3].UserId,
                Content = "同意樓上的建議，每日任務真的很重要",
                Status = "normal",
                LikeCount = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _context.ForumThreadPosts.AddRange(threadPosts);
        await _context.SaveChangesAsync();

        // 創建反應
        var reactions = new List<ForumReaction>
        {
            new ForumReaction
            {
                UserId = users[3].UserId,
                TargetType = "post",
                TargetId = posts[0].PostId,
                ReactionType = "like",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ForumReaction
            {
                UserId = users[4].UserId,
                TargetType = "thread",
                TargetId = threads[0].ThreadId,
                ReactionType = "like",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _context.ForumReactions.AddRange(reactions);
        await _context.SaveChangesAsync();

        // 創建書籤
        var bookmarks = new List<ForumBookmark>
        {
            new ForumBookmark
            {
                UserId = users[0].UserId,
                TargetType = "post",
                TargetId = posts[1].PostId, // 攻略分享帖子
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        _context.ForumBookmarks.AddRange(bookmarks);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {posts.Count} 個帖子、{threads.Count} 個主題、{threadPosts.Count} 個回覆、{reactions.Count} 個反應、{bookmarks.Count} 個書籤");
    }

    private async Task SeedNotificationsAsync(List<User> users)
    {
        _logger.LogInformation("填充通知資料...");

        var notifications = new List<Notification>
        {
            // 系統公告
            new Notification
            {
                UserId = users[0].UserId,
                Title = "系統維護通知",
                Content = "系統將於今晚23:00-01:00進行維護，期間可能無法正常使用",
                Type = "system",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            // 點數調整
            new Notification
            {
                UserId = users[2].UserId,
                Title = "點數調整通知",
                Content = "由於系統調整，您的點數已增加500點",
                Type = "points",
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            // 寵物顏色變更
            new Notification
            {
                UserId = users[2].UserId,
                Title = "寵物顏色變更",
                Content = "您的寵物顏色已變更為 gold，扣除2000點",
                Type = "pet_color",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {notifications.Count} 個通知");
    }

    private async Task SeedGroupDataAsync(List<User> users)
    {
        _logger.LogInformation("填充群組資料...");

        // 創建群組
        var groups = new List<Group>
        {
            new Group
            {
                OwnerId = users[4].UserId, // adminuser
                Name = "遊戲交流群",
                Description = "歡迎所有遊戲愛好者加入討論",
                AvatarUrl = "https://example.com/group1.jpg",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Groups.AddRange(groups);
        await _context.SaveChangesAsync();

        // 創建群組成員
        var groupMembers = new List<GroupMember>
        {
            new GroupMember
            {
                GroupId = groups[0].GroupId,
                UserId = users[4].UserId,
                Role = "owner",
                IsActive = true,
                JoinedAt = groups[0].CreatedAt
            },
            new GroupMember
            {
                GroupId = groups[0].GroupId,
                UserId = users[0].UserId,
                Role = "member",
                IsActive = true,
                JoinedAt = DateTime.UtcNow.AddDays(-15)
            },
            new GroupMember
            {
                GroupId = groups[0].GroupId,
                UserId = users[2].UserId,
                Role = "member",
                IsActive = true,
                JoinedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        _context.GroupMembers.AddRange(groupMembers);
        await _context.SaveChangesAsync();

        // 創建群組對話
        var conversations = new List<GroupConversation>
        {
            new GroupConversation
            {
                GroupId = groups[0].GroupId,
                UserId = users[4].UserId,
                Message = "歡迎大家加入遊戲交流群！",
                MessageType = "text",
                CreatedAt = groups[0].CreatedAt
            },
            new GroupConversation
            {
                GroupId = groups[0].GroupId,
                UserId = users[0].UserId,
                Message = "大家好，我是新手玩家",
                MessageType = "text",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new GroupConversation
            {
                GroupId = groups[0].GroupId,
                UserId = users[2].UserId,
                Message = "歡迎新手！有什麼問題都可以問",
                MessageType = "text",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        _context.GroupConversations.AddRange(conversations);
        await _context.SaveChangesAsync();

        // 創建封鎖用戶記錄
        var blockedUsers = new List<BlockedUser>
        {
            new BlockedUser
            {
                BlockerId = users[0].UserId,
                BlockedId = users[1].UserId, // 正常用戶封鎖被禁用戶
                Reason = "不當行為",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        _context.BlockedUsers.AddRange(blockedUsers);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {groups.Count} 個群組、{groupMembers.Count} 個成員、{conversations.Count} 個對話、{blockedUsers.Count} 個封鎖記錄");
    }

    private async Task SeedMiniGameRecordsAsync(List<User> users)
    {
        _logger.LogInformation("填充小遊戲記錄...");

        var pets = await _context.Pets.ToListAsync();
        var miniGames = new List<MiniGame>();

        foreach (var user in users.Where(u => u.IsActive))
        {
            var pet = pets.FirstOrDefault(p => p.UserId == user.UserId);
            if (pet == null) continue;

            // 為每個活躍用戶創建一些遊戲記錄
            for (int i = 0; i < 5; i++)
            {
                var gameTypes = new[] { "puzzle", "action", "strategy" };
                var gameType = gameTypes[i % gameTypes.Length];
                var isWin = i % 3 != 0; // 66% 勝率
                var score = isWin ? Random.Shared.Next(70, 100) : Random.Shared.Next(30, 60);
                var pointsEarned = isWin ? Random.Shared.Next(40, 80) : Random.Shared.Next(10, 30);
                var expEarned = isWin ? Random.Shared.Next(80, 150) : Random.Shared.Next(20, 50);

                miniGames.Add(new MiniGame
                {
                    UserId = user.UserId,
                    PetId = pet.PetId,
                    GameType = gameType,
                    IsWin = isWin,
                    Score = score,
                    PointsEarned = pointsEarned,
                    ExpEarned = expEarned,
                    CreatedAt = DateTime.UtcNow.AddDays(-(i + 1))
                });
            }
        }

        _context.MiniGames.AddRange(miniGames);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"已創建 {miniGames.Count} 個小遊戲記錄");
    }
}