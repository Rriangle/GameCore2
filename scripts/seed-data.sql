-- =============================================================================
-- GameCore 假資料腳本
-- =============================================================================
-- 此腳本提供豐富的測試資料，用於開發和測試環境
-- 執行前請確保資料庫已初始化

USE GameCore;
GO

-- =============================================================================
-- 清除現有假資料 (保留管理員和測試用戶)
-- =============================================================================
DELETE FROM [dbo].[Products] WHERE [ShopId] IN (SELECT [ShopId] FROM [dbo].[Shops] WHERE [Type] = 'Official');
DELETE FROM [dbo].[Posts] WHERE [UserId] NOT IN (SELECT [UserId] FROM [dbo].[Users] WHERE [Username] IN ('admin', 'testuser'));
DELETE FROM [dbo].[Pets] WHERE [UserId] NOT IN (SELECT [UserId] FROM [dbo].[Users] WHERE [Username] IN ('admin', 'testuser'));
DELETE FROM [dbo].[UserWallets] WHERE [UserId] NOT IN (SELECT [UserId] FROM [dbo].[Users] WHERE [Username] IN ('admin', 'testuser'));
DELETE FROM [dbo].[Users] WHERE [Username] NOT IN ('admin', 'testuser');
GO

-- =============================================================================
-- 建立測試用戶
-- =============================================================================
DECLARE @TestUsers TABLE (
    Username NVARCHAR(50),
    Email NVARCHAR(100),
    Balance DECIMAL(18,2)
);

INSERT INTO @TestUsers VALUES
('alice', 'alice@gamecore.com', 250.00),
('bob', 'bob@gamecore.com', 180.50),
('charlie', 'charlie@gamecore.com', 320.75),
('diana', 'diana@gamecore.com', 95.25),
('edward', 'edward@gamecore.com', 450.00),
('fiona', 'fiona@gamecore.com', 175.80),
('george', 'george@gamecore.com', 280.90),
('helen', 'helen@gamecore.com', 125.40),
('ivan', 'ivan@gamecore.com', 380.60),
('julia', 'julia@gamecore.com', 210.30);

-- 插入用戶
INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [CreatedAt], [IsActive], [IsEmailVerified])
SELECT 
    t.Username,
    t.Email,
    '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', -- 密碼: password
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 365), GETUTCDATE()), -- 隨機註冊日期
    1,
    1
FROM @TestUsers t
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = t.Username);

-- 插入用戶錢包
INSERT INTO [dbo].[UserWallets] ([UserId], [Balance], [CreatedAt], [UpdatedAt])
SELECT 
    u.[UserId],
    t.Balance,
    u.[CreatedAt],
    GETUTCDATE()
FROM [dbo].[Users] u
INNER JOIN @TestUsers t ON u.[Username] = t.Username
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UserWallets] WHERE [UserId] = u.[UserId]);

GO

-- =============================================================================
-- 建立寵物資料
-- =============================================================================
DECLARE @PetTypes TABLE (Type NVARCHAR(20), Name NVARCHAR(50));
INSERT INTO @PetTypes VALUES
('Slime', '史萊姆'),
('Dragon', '小龍'),
('Cat', '小貓'),
('Dog', '小狗'),
('Bird', '小鳥');

-- 為每個用戶建立寵物
INSERT INTO [dbo].[Pets] ([UserId], [Name], [Type], [Level], [Experience], [Health], [Hunger], [Happiness], [CreatedAt], [LastFedAt], [LastPlayedAt])
SELECT 
    u.[UserId],
    CONCAT(pt.Name, ' #', ROW_NUMBER() OVER (PARTITION BY u.[UserId] ORDER BY NEWID())),
    pt.Type,
    ABS(CHECKSUM(NEWID()) % 10) + 1, -- 1-10 級
    ABS(CHECKSUM(NEWID()) % 1000), -- 0-999 經驗值
    ABS(CHECKSUM(NEWID()) % 40) + 60, -- 60-100 健康度
    ABS(CHECKSUM(NEWID()) % 30) + 70, -- 70-100 飢餓度
    ABS(CHECKSUM(NEWID()) % 30) + 70, -- 70-100 快樂度
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETUTCDATE()), -- 隨機建立日期
    DATEADD(HOUR, -ABS(CHECKSUM(NEWID()) % 24), GETUTCDATE()), -- 隨機餵食時間
    DATEADD(HOUR, -ABS(CHECKSUM(NEWID()) % 12), GETUTCDATE())  -- 隨機玩耍時間
FROM [dbo].[Users] u
CROSS JOIN @PetTypes pt
WHERE u.[Username] NOT IN ('admin', 'testuser')
AND ABS(CHECKSUM(NEWID()) % 3) = 0; -- 每個用戶有 1/3 機率擁有該類型寵物

GO

-- =============================================================================
-- 建立遊戲資料
-- =============================================================================
DECLARE @Games TABLE (
    Name NVARCHAR(100),
    Description NVARCHAR(500),
    Genre NVARCHAR(50),
    Developer NVARCHAR(100),
    Publisher NVARCHAR(100),
    Platform NVARCHAR(50),
    Rating DECIMAL(3,1),
    PopularityScore INT
);

INSERT INTO @Games VALUES
('魔獸世界', '經典的 MMORPG 遊戲', 'MMORPG', 'Blizzard Entertainment', 'Blizzard Entertainment', 'PC', 4.8, 95),
('英雄聯盟', '熱門的 MOBA 遊戲', 'MOBA', 'Riot Games', 'Riot Games', 'PC', 4.6, 92),
('原神', '開放世界冒險遊戲', 'RPG', 'miHoYo', 'miHoYo', 'Multi-platform', 4.5, 88),
('王者榮耀', '手機 MOBA 遊戲', 'MOBA', 'TiMi Studios', 'Tencent', 'Mobile', 4.3, 85),
('和平精英', '戰術射擊遊戲', 'FPS', 'Tencent', 'Tencent', 'Mobile', 4.2, 82),
('賽博朋克 2077', '未來科幻 RPG', 'RPG', 'CD Projekt Red', 'CD Projekt', 'Multi-platform', 4.0, 78),
('艾爾登法環', '動作角色扮演遊戲', 'Action RPG', 'FromSoftware', 'Bandai Namco', 'Multi-platform', 4.9, 96),
('薩爾達傳說：曠野之息', '開放世界冒險', 'Adventure', 'Nintendo', 'Nintendo', 'Switch', 4.9, 97),
('我的世界', '沙盒建造遊戲', 'Sandbox', 'Mojang Studios', 'Microsoft', 'Multi-platform', 4.7, 90),
('GTA V', '開放世界動作遊戲', 'Action', 'Rockstar North', 'Rockstar Games', 'Multi-platform', 4.6, 89);

-- 插入遊戲資料
INSERT INTO [dbo].[Games] ([Name], [Description], [Genre], [ReleaseDate], [Developer], [Publisher], [Platform], [Rating], [PopularityScore], [CreatedAt], [UpdatedAt])
SELECT 
    g.Name,
    g.Description,
    g.Genre,
    DATEADD(YEAR, -ABS(CHECKSUM(NEWID()) % 10), GETDATE()), -- 隨機發行日期
    g.Developer,
    g.Publisher,
    g.Platform,
    g.Rating,
    g.PopularityScore,
    GETUTCDATE(),
    GETUTCDATE()
FROM @Games g
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Games] WHERE [Name] = g.Name);

GO

-- =============================================================================
-- 建立論壇文章
-- =============================================================================
DECLARE @PostTitles TABLE (
    Title NVARCHAR(200),
    Category NVARCHAR(50)
);

INSERT INTO @PostTitles VALUES
('新手入門攻略分享', 'Strategy'),
('遊戲心得與討論', 'General'),
('最新更新內容分析', 'General'),
('角色培養指南', 'Strategy'),
('裝備搭配建議', 'Strategy'),
('遊戲平衡性討論', 'General'),
('活動參與心得', 'General'),
('技術問題求助', 'General'),
('遊戲截圖分享', 'General'),
('建議與反饋', 'General');

-- 插入文章
INSERT INTO [dbo].[Posts] ([ForumId], [UserId], [Title], [Content], [ViewCount], [LikeCount], [IsPinned], [IsLocked], [CreatedAt], [UpdatedAt])
SELECT 
    f.[ForumId],
    u.[UserId],
    pt.Title,
    CONCAT('這是關於「', pt.Title, '」的詳細內容。包含遊戲技巧、心得分享和實用建議。希望對大家有幫助！'),
    ABS(CHECKSUM(NEWID()) % 1000) + 10, -- 10-1009 瀏覽次數
    ABS(CHECKSUM(NEWID()) % 100) + 1,   -- 1-100 讚數
    CASE WHEN ABS(CHECKSUM(NEWID()) % 20) = 0 THEN 1 ELSE 0 END, -- 5% 機率置頂
    0, -- 不鎖定
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETUTCDATE()), -- 隨機發布日期
    GETUTCDATE()
FROM [dbo].[Forums] f
CROSS JOIN [dbo].[Users] u
CROSS JOIN @PostTitles pt
WHERE u.[Username] NOT IN ('admin', 'testuser')
AND ABS(CHECKSUM(NEWID()) % 5) = 0; -- 每個組合有 1/5 機率產生文章

GO

-- =============================================================================
-- 建立商城商品
-- =============================================================================
DECLARE @Products TABLE (
    Name NVARCHAR(100),
    Description NVARCHAR(500),
    Price DECIMAL(18,2),
    Category NVARCHAR(50),
    Stock INT
);

INSERT INTO @Products VALUES
('遊戲點數卡 1000 點', '可在遊戲內使用的點數卡', 100.00, 'Digital', 999),
('遊戲點數卡 5000 點', '可在遊戲內使用的點數卡', 450.00, 'Digital', 500),
('遊戲點數卡 10000 點', '可在遊戲內使用的點數卡', 850.00, 'Digital', 200),
('限定角色造型', '稀有角色造型，限時販售', 299.00, 'Cosmetic', 50),
('特效武器外觀', '炫酷武器外觀效果', 199.00, 'Cosmetic', 100),
('經驗值加倍券', '24小時內經驗值加倍', 50.00, 'Boost', 999),
('金幣加倍券', '24小時內金幣加倍', 50.00, 'Boost', 999),
('VIP 會員月卡', '享受 VIP 特權一個月', 299.00, 'Membership', 999),
('VIP 會員季卡', '享受 VIP 特權三個月', 799.00, 'Membership', 999),
('VIP 會員年卡', '享受 VIP 特權一年', 2999.00, 'Membership', 999);

-- 插入商品
INSERT INTO [dbo].[Products] ([ShopId], [Name], [Description], [Price], [Stock], [Category], [ImageUrl], [IsActive], [CreatedAt], [UpdatedAt])
SELECT 
    s.[ShopId],
    p.Name,
    p.Description,
    p.Price,
    p.Stock,
    p.Category,
    CONCAT('/images/products/', LOWER(REPLACE(p.Name, ' ', '-')), '.jpg'),
    1,
    GETUTCDATE(),
    GETUTCDATE()
FROM [dbo].[Shops] s
CROSS JOIN @Products p
WHERE s.[Type] = 'Official';

GO

-- =============================================================================
-- 建立用戶商城 (玩家市場)
-- =============================================================================
-- 為活躍用戶建立個人商城
INSERT INTO [dbo].[Shops] ([Name], [Description], [Type], [OwnerId], [IsActive], [CreatedAt], [UpdatedAt])
SELECT 
    CONCAT(u.[Username], ' 的商店'),
    CONCAT('歡迎來到 ', u.[Username], ' 的個人商店！'),
    'User',
    u.[UserId],
    1,
    GETUTCDATE(),
    GETUTCDATE()
FROM [dbo].[Users] u
WHERE u.[Username] NOT IN ('admin', 'testuser')
AND ABS(CHECKSUM(NEWID()) % 3) = 0; -- 1/3 用戶有個人商店

-- 為用戶商城添加商品
INSERT INTO [dbo].[Products] ([ShopId], [Name], [Description], [Price], [Stock], [Category], [ImageUrl], [IsActive], [CreatedAt], [UpdatedAt])
SELECT 
    s.[ShopId],
    CONCAT('二手 ', g.[Name], ' 遊戲帳號'),
    CONCAT('出售 ', g.[Name], ' 遊戲帳號，等級高，裝備齊全'),
    ABS(CHECKSUM(NEWID()) % 500) + 100, -- 100-600 價格
    ABS(CHECKSUM(NEWID()) % 5) + 1,     -- 1-5 庫存
    'Account',
    '/images/products/account.jpg',
    1,
    GETUTCDATE(),
    GETUTCDATE()
FROM [dbo].[Shops] s
CROSS JOIN [dbo].[Games] g
WHERE s.[Type] = 'User'
AND ABS(CHECKSUM(NEWID()) % 4) = 0; -- 1/4 機率有該遊戲帳號

GO

-- =============================================================================
-- 更新統計資訊
-- =============================================================================
PRINT '假資料建立完成！';
PRINT '統計資訊：';
PRINT CONCAT('- 用戶數量: ', (SELECT COUNT(*) FROM [dbo].[Users]));
PRINT CONCAT('- 寵物數量: ', (SELECT COUNT(*) FROM [dbo].[Pets]));
PRINT CONCAT('- 遊戲數量: ', (SELECT COUNT(*) FROM [dbo].[Games]));
PRINT CONCAT('- 文章數量: ', (SELECT COUNT(*) FROM [dbo].[Posts]));
PRINT CONCAT('- 商城數量: ', (SELECT COUNT(*) FROM [dbo].[Shops]));
PRINT CONCAT('- 商品數量: ', (SELECT COUNT(*) FROM [dbo].[Products]));

GO