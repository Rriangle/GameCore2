-- =============================================================================
-- GameCore 假資料 Seeder 腳本
-- =============================================================================
-- 
-- 使用說明：
-- 1. 此腳本用於插入測試用的假資料
-- 2. 執行前請確認已執行 init-db.sql
-- 3. 適用於開發和測試環境
-- 4. 生產環境請勿執行此腳本
--
-- 執行方式：
-- sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -d GameCore -i seed-data.sql
-- 或使用 SQL Server Management Studio 執行
-- =============================================================================

USE GameCore;
GO

PRINT '開始插入假資料...';
GO

-- =============================================================================
-- 插入測試用戶
-- =============================================================================

-- 插入管理員用戶
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, EmailConfirmed)
    VALUES (
        'admin',
        'admin@gamecore.com',
        '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', -- password: password
        '系統',
        '管理員',
        1,
        1
    );
    PRINT '已插入管理員用戶: admin';
END

-- 插入測試用戶
DECLARE @AdminUserId UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Username = 'admin');

-- 插入一般用戶
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, EmailConfirmed, CreatedBy)
SELECT 'gamer001', 'gamer001@example.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', '小明', '王', 1, 1, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'gamer001');

INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, EmailConfirmed, CreatedBy)
SELECT 'gamer002', 'gamer002@example.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', '小華', '李', 1, 1, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'gamer002');

INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, IsActive, EmailConfirmed, CreatedBy)
SELECT 'gamer003', 'gamer003@example.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', '小美', '張', 1, 1, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'gamer003');

-- 插入用戶角色
INSERT INTO UserRoles (UserId, RoleName, CreatedBy)
SELECT u.Id, 'Admin', @AdminUserId
FROM Users u
WHERE u.Username = 'admin' AND NOT EXISTS (
    SELECT 1 FROM UserRoles ur WHERE ur.UserId = u.Id AND ur.RoleName = 'Admin'
);

INSERT INTO UserRoles (UserId, RoleName, CreatedBy)
SELECT u.Id, 'User', @AdminUserId
FROM Users u
WHERE u.Username IN ('gamer001', 'gamer002', 'gamer003') AND NOT EXISTS (
    SELECT 1 FROM UserRoles ur WHERE ur.UserId = u.Id AND ur.RoleName = 'User'
);

-- =============================================================================
-- 插入遊戲資料
-- =============================================================================

-- 插入熱門遊戲
INSERT INTO Games (Name, Description, Genre, Platform, Publisher, Developer, ReleaseDate, CoverImage, SteamAppId, MetacriticScore, UserRating, TotalRatings, CreatedBy)
SELECT 'Cyberpunk 2077', '賽博朋克風格的開放世界 RPG 遊戲', 'RPG', 'PC,PS4,PS5,Xbox', 'CD Projekt', 'CD Projekt Red', '2020-12-10', '/images/games/cyberpunk2077.jpg', 1091500, 7.2, 4.1, 1250, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Games WHERE Name = 'Cyberpunk 2077');

INSERT INTO Games (Name, Description, Genre, Platform, Publisher, Developer, ReleaseDate, CoverImage, SteamAppId, MetacriticScore, UserRating, TotalRatings, CreatedBy)
SELECT 'Elden Ring', 'FromSoftware 開發的開放世界動作 RPG', 'Action RPG', 'PC,PS4,PS5,Xbox', 'Bandai Namco', 'FromSoftware', '2022-02-25', '/images/games/eldenring.jpg', 1245620, 9.6, 4.8, 2100, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Games WHERE Name = 'Elden Ring');

INSERT INTO Games (Name, Description, Genre, Platform, Publisher, Developer, ReleaseDate, CoverImage, SteamAppId, MetacriticScore, UserRating, TotalRatings, CreatedBy)
SELECT 'Valorant', 'Riot Games 開發的戰術射擊遊戲', 'FPS', 'PC', 'Riot Games', 'Riot Games', '2020-06-02', '/images/games/valorant.jpg', 0, 8.1, 4.3, 1800, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Games WHERE Name = 'Valorant');

INSERT INTO Games (Name, Description, Genre, Platform, Publisher, Developer, ReleaseDate, CoverImage, SteamAppId, MetacriticScore, UserRating, TotalRatings, CreatedBy)
SELECT 'League of Legends', '經典的 MOBA 遊戲', 'MOBA', 'PC', 'Riot Games', 'Riot Games', '2009-10-27', '/images/games/lol.jpg', 0, 8.7, 4.2, 3500, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Games WHERE Name = 'League of Legends');

INSERT INTO Games (Name, Description, Genre, Platform, Publisher, Developer, ReleaseDate, CoverImage, SteamAppId, MetacriticScore, UserRating, TotalRatings, CreatedBy)
SELECT 'Minecraft', '沙盒建造遊戲', 'Sandbox', 'PC,PS4,PS5,Xbox,Switch', 'Mojang', 'Mojang', '2011-11-18', '/images/games/minecraft.jpg', 0, 9.3, 4.9, 4200, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Games WHERE Name = 'Minecraft');

-- 插入遊戲熱度資料
DECLARE @CyberpunkId UNIQUEIDENTIFIER = (SELECT Id FROM Games WHERE Name = 'Cyberpunk 2077');
DECLARE @EldenRingId UNIQUEIDENTIFIER = (SELECT Id FROM Games WHERE Name = 'Elden Ring');
DECLARE @ValorantId UNIQUEIDENTIFIER = (SELECT Id FROM Games WHERE Name = 'Valorant');
DECLARE @LolId UNIQUEIDENTIFIER = (SELECT Id FROM Games WHERE Name = 'League of Legends');
DECLARE @MinecraftId UNIQUEIDENTIFIER = (SELECT Id FROM Games WHERE Name = 'Minecraft');

INSERT INTO GameHeat (GameId, HeatScore, PlayerCount, ViewCount, DiscussionCount, MarketActivity)
SELECT @CyberpunkId, 8.5, 45000, 125000, 89, 12500.00
WHERE NOT EXISTS (SELECT 1 FROM GameHeat WHERE GameId = @CyberpunkId);

INSERT INTO GameHeat (GameId, HeatScore, PlayerCount, ViewCount, DiscussionCount, MarketActivity)
SELECT @EldenRingId, 9.2, 78000, 210000, 156, 28900.00
WHERE NOT EXISTS (SELECT 1 FROM GameHeat WHERE GameId = @EldenRingId);

INSERT INTO GameHeat (GameId, HeatScore, PlayerCount, ViewCount, DiscussionCount, MarketActivity)
SELECT @ValorantId, 8.8, 120000, 180000, 234, 15600.00
WHERE NOT EXISTS (SELECT 1 FROM GameHeat WHERE GameId = @ValorantId);

INSERT INTO GameHeat (GameId, HeatScore, PlayerCount, ViewCount, DiscussionCount, MarketActivity)
SELECT @LolId, 9.0, 250000, 320000, 445, 28900.00
WHERE NOT EXISTS (SELECT 1 FROM GameHeat WHERE GameId = @LolId);

INSERT INTO GameHeat (GameId, HeatScore, PlayerCount, ViewCount, DiscussionCount, MarketActivity)
SELECT @MinecraftId, 8.7, 180000, 280000, 312, 19800.00
WHERE NOT EXISTS (SELECT 1 FROM GameHeat WHERE GameId = @MinecraftId);

-- =============================================================================
-- 插入論壇文章
-- =============================================================================

DECLARE @GeneralCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ForumCategories WHERE Name = '一般討論');
DECLARE @StrategyCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ForumCategories WHERE Name = '攻略心得');
DECLARE @HelpCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ForumCategories WHERE Name = '問題求助');
DECLARE @ChatCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ForumCategories WHERE Name = '閒聊灌水');

DECLARE @Gamer001Id UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Username = 'gamer001');
DECLARE @Gamer002Id UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Username = 'gamer002');
DECLARE @Gamer003Id UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Username = 'gamer003');

-- 插入論壇文章
INSERT INTO ForumPosts (Title, Content, CategoryId, AuthorId, GameId, ViewCount, LikeCount, DislikeCount, CreatedBy)
SELECT 'Cyberpunk 2077 2.0 更新心得', '這次更新真的讓遊戲煥然一新！新的技能樹系統和載具戰鬥都很有趣。', @StrategyCategoryId, @Gamer001Id, @CyberpunkId, 156, 23, 2, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM ForumPosts WHERE Title = 'Cyberpunk 2077 2.0 更新心得');

INSERT INTO ForumPosts (Title, Content, CategoryId, AuthorId, GameId, ViewCount, LikeCount, DislikeCount, CreatedBy)
SELECT 'Elden Ring 新手攻略分享', '分享一下我玩 Elden Ring 的心得，新手建議先練級再挑戰 Boss。', @StrategyCategoryId, @Gamer002Id, @EldenRingId, 234, 45, 1, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM ForumPosts WHERE Title = 'Elden Ring 新手攻略分享');

INSERT INTO ForumPosts (Title, Content, CategoryId, AuthorId, GameId, ViewCount, LikeCount, DislikeCount, CreatedBy)
SELECT 'Valorant 新角色討論', '大家覺得新角色怎麼樣？技能組看起來很有趣。', @GeneralCategoryId, @Gamer003Id, @ValorantId, 89, 12, 0, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM ForumPosts WHERE Title = 'Valorant 新角色討論');

INSERT INTO ForumPosts (Title, Content, CategoryId, AuthorId, GameId, ViewCount, LikeCount, DislikeCount, CreatedBy)
SELECT 'Minecraft 建築分享', '分享一下我蓋的城堡，花了兩個禮拜才完成！', @GeneralCategoryId, @Gamer001Id, @MinecraftId, 312, 67, 3, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM ForumPosts WHERE Title = 'Minecraft 建築分享');

INSERT INTO ForumPosts (Title, Content, CategoryId, AuthorId, GameId, ViewCount, LikeCount, DislikeCount, CreatedBy)
SELECT 'LoL 排位賽心得', '終於打上鑽石了！分享一下我的心得和技巧。', @StrategyCategoryId, @Gamer002Id, @LolId, 445, 89, 5, @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM ForumPosts WHERE Title = 'LoL 排位賽心得');

-- =============================================================================
-- 插入商品資料
-- =============================================================================

DECLARE @GameSoftwareCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ProductCategories WHERE Name = '遊戲軟體');
DECLARE @GamePeripheralCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ProductCategories WHERE Name = '遊戲周邊');
DECLARE @DigitalGoodsCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ProductCategories WHERE Name = '數位商品');
DECLARE @SecondHandCategoryId UNIQUEIDENTIFIER = (SELECT Id FROM ProductCategories WHERE Name = '二手交易');

-- 插入遊戲軟體
INSERT INTO Products (Name, Description, CategoryId, GameId, Price, OriginalPrice, StockQuantity, IsDigital, ImageUrls, Tags, CreatedBy)
SELECT 'Cyberpunk 2077 數位版', '賽博朋克 2077 完整版遊戲，包含所有 DLC', @GameSoftwareCategoryId, @CyberpunkId, 1299.00, 1599.00, 999, 1, '["/images/products/cp2077_digital.jpg"]', 'cyberpunk,rpg,digital', @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Cyberpunk 2077 數位版');

INSERT INTO Products (Name, Description, CategoryId, GameId, Price, OriginalPrice, StockQuantity, IsDigital, ImageUrls, Tags, CreatedBy)
SELECT 'Elden Ring 實體版', '艾爾登法環實體光碟版，附贈精美海報', @GameSoftwareCategoryId, @EldenRingId, 1899.00, 1899.00, 50, 0, '["/images/products/eldenring_physical.jpg"]', 'eldenring,action-rpg,physical', @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Elden Ring 實體版');

-- 插入遊戲周邊
INSERT INTO Products (Name, Description, CategoryId, GameId, Price, OriginalPrice, StockQuantity, IsDigital, ImageUrls, Tags, CreatedBy)
SELECT 'Valorant 官方滑鼠墊', 'Valorant 官方授權滑鼠墊，高品質材質', @GamePeripheralCategoryId, @ValorantId, 299.00, 399.00, 200, 0, '["/images/products/valorant_mousepad.jpg"]', 'valorant,mousepad,peripheral', @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Valorant 官方滑鼠墊');

INSERT INTO Products (Name, Description, CategoryId, GameId, Price, OriginalPrice, StockQuantity, IsDigital, ImageUrls, Tags, CreatedBy)
SELECT 'Minecraft 樂高積木組', 'Minecraft 官方樂高積木，可建造遊戲中的建築', @GamePeripheralCategoryId, @MinecraftId, 899.00, 999.00, 75, 0, '["/images/products/minecraft_lego.jpg"]', 'minecraft,lego,peripheral', @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Minecraft 樂高積木組');

-- 插入數位商品
INSERT INTO Products (Name, Description, CategoryId, GameId, Price, OriginalPrice, StockQuantity, IsDigital, ImageUrls, Tags, CreatedBy)
SELECT 'LoL 英雄造型包', '英雄聯盟精選英雄造型 10 個', @DigitalGoodsCategoryId, @LolId, 450.00, 600.00, 999, 1, '["/images/products/lol_skins.jpg"]', 'lol,skins,digital', @AdminUserId
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'LoL 英雄造型包');

-- =============================================================================
-- 插入寵物資料
-- =============================================================================

-- 為每個用戶創建寵物
INSERT INTO Pets (Name, Type, OwnerId, Level, Experience, MaxExperience, Hunger, Happiness, Health, LastFedAt, LastPlayedAt)
SELECT '史萊姆王', 'Slime', @Gamer001Id, 15, 1250, 2000, 85, 90, 95, DATEADD(HOUR, -2, GETUTCDATE()), DATEADD(HOUR, -1, GETUTCDATE())
WHERE NOT EXISTS (SELECT 1 FROM Pets WHERE OwnerId = @Gamer001Id);

INSERT INTO Pets (Name, Type, OwnerId, Level, Experience, MaxExperience, Hunger, Happiness, Health, LastFedAt, LastPlayedAt)
SELECT '小龍寶寶', 'Dragon', @Gamer002Id, 8, 450, 800, 70, 85, 90, DATEADD(HOUR, -4, GETUTCDATE()), DATEADD(HOUR, -3, GETUTCDATE())
WHERE NOT EXISTS (SELECT 1 FROM Pets WHERE OwnerId = @Gamer002Id);

INSERT INTO Pets (Name, Type, OwnerId, Level, Experience, MaxExperience, Hunger, Happiness, Health, LastFedAt, LastPlayedAt)
SELECT '魔法貓咪', 'Cat', @Gamer003Id, 12, 980, 1500, 95, 88, 92, DATEADD(HOUR, -1, GETUTCDATE()), DATEADD(MINUTE, -30, GETUTCDATE())
WHERE NOT EXISTS (SELECT 1 FROM Pets WHERE OwnerId = @Gamer003Id);

-- =============================================================================
-- 插入交易記錄
-- =============================================================================

-- 插入一些交易記錄
INSERT INTO Transactions (TransactionType, BuyerId, SellerId, ProductId, Quantity, UnitPrice, TotalAmount, Commission, Status, PaymentMethod, CompletedAt)
SELECT 'Purchase', @Gamer001Id, NULL, (SELECT Id FROM Products WHERE Name = 'Cyberpunk 2077 數位版'), 1, 1299.00, 1299.00, 0.00, 'Completed', 'CreditCard', DATEADD(DAY, -5, GETUTCDATE())
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE BuyerId = @Gamer001Id AND ProductId = (SELECT Id FROM Products WHERE Name = 'Cyberpunk 2077 數位版'));

INSERT INTO Transactions (TransactionType, BuyerId, SellerId, ProductId, Quantity, UnitPrice, TotalAmount, Commission, Status, PaymentMethod, CompletedAt)
SELECT 'Purchase', @Gamer002Id, NULL, (SELECT Id FROM Products WHERE Name = 'Elden Ring 實體版'), 1, 1899.00, 1899.00, 0.00, 'Completed', 'BankTransfer', DATEADD(DAY, -3, GETUTCDATE())
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE BuyerId = @Gamer002Id AND ProductId = (SELECT Id FROM Products WHERE Name = 'Elden Ring 實體版'));

INSERT INTO Transactions (TransactionType, BuyerId, SellerId, ProductId, Quantity, UnitPrice, TotalAmount, Commission, Status, PaymentMethod, CompletedAt)
SELECT 'Purchase', @Gamer003Id, NULL, (SELECT Id FROM Products WHERE Name = 'Valorant 官方滑鼠墊'), 1, 299.00, 299.00, 0.00, 'Completed', 'DigitalWallet', DATEADD(DAY, -1, GETUTCDATE())
WHERE NOT EXISTS (SELECT 1 FROM Transactions WHERE BuyerId = @Gamer003Id AND ProductId = (SELECT Id FROM Products WHERE Name = 'Valorant 官方滑鼠墊'));

-- =============================================================================
-- 更新統計資料
-- =============================================================================

-- 更新遊戲的總評分數
UPDATE Games 
SET TotalRatings = (
    SELECT COUNT(*) 
    FROM ForumPosts fp 
    WHERE fp.GameId = Games.Id
)
WHERE Id IN (@CyberpunkId, @EldenRingId, @ValorantId, @LolId, @MinecraftId);

-- 更新用戶的最後登入時間
UPDATE Users 
SET LastLoginAt = DATEADD(HOUR, -RAND(CHECKSUM(NEWID())) * 24, GETUTCDATE())
WHERE Username IN ('gamer001', 'gamer002', 'gamer003');

PRINT '假資料插入完成！';
PRINT '已插入：';
PRINT '- 4 個測試用戶 (包含 1 個管理員)';
PRINT '- 5 個熱門遊戲';
PRINT '- 5 篇論壇文章';
PRINT '- 6 個商品';
PRINT '- 3 個寵物';
PRINT '- 3 筆交易記錄';
GO