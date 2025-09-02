# GameCore 資料庫假資料生成腳本

## 概述
本腳本用於在 GameCore 專案的 SQL Server 資料庫中生成大量假資料，用於展示和測試目的。所有資料均為模擬資料，不會影響現有的資料庫結構。

## 注意事項
- 本腳本僅用於生成測試資料
- 不會修改任何現有的資料表結構
- 所有資料均為假資料，僅供展示使用
- 執行前請確保已備份重要資料

---

## 1. 用戶資料生成 (1000+ 筆)

```sql
-- =============================================
-- 生成用戶基本資料 (1000+ 筆)
-- 包含用戶註冊、個人資料等資訊
-- =============================================

-- 清空現有用戶資料 (保留管理員帳號)
DELETE FROM Users WHERE UserID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('Users', RESEED, 100);

-- 生成用戶資料
DECLARE @i INT = 1;
DECLARE @maxUsers INT = 1000;

WHILE @i <= @maxUsers
BEGIN
    INSERT INTO Users (
        User_Name,           -- 用戶名稱
        Email,              -- 電子郵件
        Password_Hash,      -- 密碼雜湊 (BCrypt)
        Registration_Date,  -- 註冊日期
        Last_Login_Date,    -- 最後登入日期
        Is_Active,          -- 是否啟用
        Role,               -- 用戶角色
        Avatar_URL,         -- 頭像URL
        Bio,                -- 個人簡介
        Location,           -- 所在地
        Website,            -- 個人網站
        Social_Media,       -- 社群媒體連結
        Preferences,        -- 用戶偏好設定
        Created_At,         -- 建立時間
        Updated_At          -- 更新時間
    )
    VALUES (
        -- 用戶名稱 (中文 + 英文組合)
        CASE 
            WHEN @i % 4 = 0 THEN '玩家' + CAST(@i AS VARCHAR(10))
            WHEN @i % 4 = 1 THEN 'Gamer' + CAST(@i AS VARCHAR(10))
            WHEN @i % 4 = 2 THEN '遊戲' + CAST(@i AS VARCHAR(10))
            ELSE 'Player' + CAST(@i AS VARCHAR(10))
        END,
        
        -- 電子郵件
        'user' + CAST(@i AS VARCHAR(10)) + '@gamecore.com',
        
        -- 密碼雜湊 (所有用戶密碼都是 'password123')
        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J/HS.iK8i',
        
        -- 註冊日期 (隨機分布在過去一年內)
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 365), GETDATE()),
        
        -- 最後登入日期 (隨機分布在過去30天內)
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()),
        
        -- 是否啟用 (90% 用戶啟用)
        CASE WHEN @i % 10 = 0 THEN 0 ELSE 1 END,
        
        -- 用戶角色 (99% 一般用戶，1% 管理員)
        CASE WHEN @i % 100 = 0 THEN 'Admin' ELSE 'User' END,
        
        -- 頭像URL (使用假圖片服務)
        'https://picsum.photos/200/200?random=' + CAST(@i AS VARCHAR(10)),
        
        -- 個人簡介 (隨機生成)
        CASE 
            WHEN @i % 5 = 0 THEN '熱愛遊戲的玩家，喜歡分享遊戲心得'
            WHEN @i % 5 = 1 THEN '專業遊戲評論員，專注於獨立遊戲'
            WHEN @i % 5 = 2 THEN '電競愛好者，擅長MOBA類遊戲'
            WHEN @i % 5 = 3 THEN '休閒玩家，享受遊戲的樂趣'
            ELSE '遊戲收藏家，擁有豐富的遊戲庫'
        END,
        
        -- 所在地 (台灣主要城市)
        CASE 
            WHEN @i % 6 = 0 THEN '台北市'
            WHEN @i % 6 = 1 THEN '新北市'
            WHEN @i % 6 = 2 THEN '桃園市'
            WHEN @i % 6 = 3 THEN '台中市'
            WHEN @i % 6 = 4 THEN '台南市'
            ELSE '高雄市'
        END,
        
        -- 個人網站
        CASE WHEN @i % 3 = 0 THEN 'https://blog' + CAST(@i AS VARCHAR(10)) + '.com' ELSE NULL END,
        
        -- 社群媒體連結 (JSON格式)
        '{"twitter": "user' + CAST(@i AS VARCHAR(10)) + '", "discord": "user' + CAST(@i AS VARCHAR(10)) + '#1234"}',
        
        -- 用戶偏好設定 (JSON格式)
        '{"theme": "dark", "language": "zh-TW", "notifications": true}',
        
        -- 建立時間
        GETDATE(),
        
        -- 更新時間
        GETDATE()
    );
    
    SET @i = @i + 1;
END;

PRINT '已生成 ' + CAST(@maxUsers AS VARCHAR(10)) + ' 筆用戶資料';
```

---

## 2. 用戶錢包資料生成

```sql
-- =============================================
-- 生成用戶錢包資料
-- 為每個用戶建立錢包帳戶
-- =============================================

-- 清空現有錢包資料
DELETE FROM UserWallets WHERE UserID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('UserWallets', RESEED, 100);

-- 為每個用戶建立錢包
INSERT INTO UserWallets (
    UserID,             -- 用戶ID
    Balance,            -- 餘額
    Total_Earned,       -- 總收入
    Total_Spent,        -- 總支出
    Last_Transaction_Date, -- 最後交易日期
    Created_At,         -- 建立時間
    Updated_At          -- 更新時間
)
SELECT 
    u.UserID,
    -- 隨機餘額 (0-10000)
    ABS(CHECKSUM(NEWID()) % 10001),
    -- 總收入 (餘額的1-3倍)
    ABS(CHECKSUM(NEWID()) % 30001) + 1000,
    -- 總支出 (總收入的0.5-0.8倍)
    (ABS(CHECKSUM(NEWID()) % 30001) + 1000) * (0.5 + (ABS(CHECKSUM(NEWID()) % 31) / 100.0)),
    -- 最後交易日期 (隨機分布在過去30天內)
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()),
    GETDATE(),
    GETDATE()
FROM Users u
WHERE u.UserID > 100;

PRINT '已生成用戶錢包資料';
```

---

## 3. 遊戲資料生成 (500+ 筆)

```sql
-- =============================================
-- 生成遊戲基本資料 (500+ 筆)
-- 包含各種類型的遊戲資訊
-- =============================================

-- 清空現有遊戲資料
DELETE FROM Games WHERE GameID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('Games', RESEED, 100);

-- 遊戲名稱陣列
DECLARE @gameNames TABLE (
    ID INT IDENTITY(1,1),
    Name NVARCHAR(200),
    Genre NVARCHAR(50),
    Platform NVARCHAR(50)
);

-- 插入遊戲名稱資料
INSERT INTO @gameNames (Name, Genre, Platform) VALUES
-- 動作遊戲
('Elden Ring: Shadow of the Erdtree', 'Action', 'PC,PS5,Xbox'),
('Monster Hunter World: Iceborne', 'Action', 'PC,PS4,Xbox'),
('Devil May Cry 5', 'Action', 'PC,PS4,Xbox'),
('Sekiro: Shadows Die Twice', 'Action', 'PC,PS4,Xbox'),
('God of War Ragnarök', 'Action', 'PS4,PS5'),
('Spider-Man 2', 'Action', 'PS5'),
('Assassin''s Creed Valhalla', 'Action', 'PC,PS4,Xbox'),
('Cyberpunk 2077', 'Action', 'PC,PS4,Xbox'),
('Red Dead Redemption 2', 'Action', 'PC,PS4,Xbox'),
('The Witcher 3: Wild Hunt', 'Action', 'PC,PS4,Xbox'),

-- RPG遊戲
('Final Fantasy XVI', 'RPG', 'PS5'),
('Persona 5 Royal', 'RPG', 'PC,PS4,Switch'),
('Dragon Quest XI', 'RPG', 'PC,PS4,Switch'),
('Octopath Traveler II', 'RPG', 'PC,PS4,Switch'),
('Baldur''s Gate 3', 'RPG', 'PC,PS5'),
('Starfield', 'RPG', 'PC,Xbox'),
('Elder Scrolls VI', 'RPG', 'PC,Xbox'),
('Mass Effect Legendary Edition', 'RPG', 'PC,PS4,Xbox'),
('Divinity: Original Sin 2', 'RPG', 'PC,PS4,Xbox'),
('Pillars of Eternity II', 'RPG', 'PC,PS4,Xbox'),

-- 策略遊戲
('Civilization VI', 'Strategy', 'PC,PS4,Xbox,Switch'),
('Total War: Warhammer III', 'Strategy', 'PC'),
('XCOM 2', 'Strategy', 'PC,PS4,Xbox'),
('Crusader Kings III', 'Strategy', 'PC'),
('Europa Universalis IV', 'Strategy', 'PC'),
('Stellaris', 'Strategy', 'PC,PS4,Xbox'),
('Age of Empires IV', 'Strategy', 'PC'),
('Company of Heroes 3', 'Strategy', 'PC'),
('Wargroove', 'Strategy', 'PC,PS4,Xbox,Switch'),
('Into the Breach', 'Strategy', 'PC,Switch'),

-- 獨立遊戲
('Hades II', 'Indie', 'PC'),
('Stardew Valley', 'Indie', 'PC,PS4,Xbox,Switch'),
('Hollow Knight', 'Indie', 'PC,PS4,Xbox,Switch'),
('Celeste', 'Indie', 'PC,PS4,Xbox,Switch'),
('Dead Cells', 'Indie', 'PC,PS4,Xbox,Switch'),
('Risk of Rain 2', 'Indie', 'PC,PS4,Xbox,Switch'),
('Slay the Spire', 'Indie', 'PC,PS4,Xbox,Switch'),
('Gris', 'Indie', 'PC,PS4,Xbox,Switch'),
('Ori and the Will of the Wisps', 'Indie', 'PC,Xbox'),
('Cuphead', 'Indie', 'PC,PS4,Xbox,Switch'),

-- 競技遊戲
('League of Legends', 'MOBA', 'PC'),
('Dota 2', 'MOBA', 'PC'),
('Valorant', 'FPS', 'PC'),
('Counter-Strike 2', 'FPS', 'PC'),
('Overwatch 2', 'FPS', 'PC,PS4,Xbox,Switch'),
('Apex Legends', 'FPS', 'PC,PS4,Xbox'),
('Fortnite', 'Battle Royale', 'PC,PS4,Xbox,Switch'),
('PUBG: BATTLEGROUNDS', 'Battle Royale', 'PC,PS4,Xbox'),
('Call of Duty: Warzone', 'FPS', 'PC,PS4,Xbox'),
('Rocket League', 'Sports', 'PC,PS4,Xbox,Switch'),

-- 手機遊戲
('Genshin Impact', 'Mobile RPG', 'Mobile,PC,PS4'),
('Honkai: Star Rail', 'Mobile RPG', 'Mobile,PC'),
('Arknights', 'Mobile Strategy', 'Mobile'),
('Fate/Grand Order', 'Mobile RPG', 'Mobile'),
('Azur Lane', 'Mobile Strategy', 'Mobile'),
('Girls'' Frontline', 'Mobile Strategy', 'Mobile'),
('Uma Musume Pretty Derby', 'Mobile Simulation', 'Mobile'),
('Project Sekai', 'Mobile Rhythm', 'Mobile'),
('Puzzle & Dragons', 'Mobile Puzzle', 'Mobile'),
('Monster Strike', 'Mobile Action', 'Mobile');

-- 生成遊戲資料
DECLARE @j INT = 1;
DECLARE @maxGames INT = 500;

WHILE @j <= @maxGames
BEGIN
    DECLARE @gameIndex INT = (@j - 1) % (SELECT COUNT(*) FROM @gameNames) + 1;
    DECLARE @gameName NVARCHAR(200);
    DECLARE @gameGenre NVARCHAR(50);
    DECLARE @gamePlatform NVARCHAR(50);
    
    SELECT @gameName = Name, @gameGenre = Genre, @gamePlatform = Platform 
    FROM @gameNames WHERE ID = @gameIndex;
    
    -- 為重複的遊戲名稱加上編號
    IF @j > (SELECT COUNT(*) FROM @gameNames)
    BEGIN
        SET @gameName = @gameName + ' ' + CAST((@j / (SELECT COUNT(*) FROM @gameNames)) + 1 AS VARCHAR(10));
    END;
    
    INSERT INTO Games (
        Name,               -- 遊戲名稱
        Genre,              -- 遊戲類型
        Platform,           -- 平台
        Release_Date,       -- 發行日期
        Developer,          -- 開發商
        Publisher,          -- 發行商
        Description,        -- 遊戲描述
        Cover_Image_URL,    -- 封面圖片URL
        Rating,             -- 評分
        Price,              -- 價格
        Is_Active,          -- 是否啟用
        Created_At,         -- 建立時間
        Updated_At          -- 更新時間
    )
    VALUES (
        @gameName,
        @gameGenre,
        @gamePlatform,
        -- 發行日期 (隨機分布在過去5年內)
        DATEADD(YEAR, -ABS(CHECKSUM(NEWID()) % 5), GETDATE()),
        -- 開發商 (隨機選擇)
        CASE 
            WHEN @j % 5 = 0 THEN 'FromSoftware'
            WHEN @j % 5 = 1 THEN 'Capcom'
            WHEN @j % 5 = 2 THEN 'Square Enix'
            WHEN @j % 5 = 3 THEN 'Bethesda'
            ELSE 'Indie Studio'
        END,
        -- 發行商
        CASE 
            WHEN @j % 4 = 0 THEN 'Bandai Namco'
            WHEN @j % 4 = 1 THEN 'Capcom'
            WHEN @j % 4 = 2 THEN 'Square Enix'
            ELSE 'Bethesda'
        END,
        -- 遊戲描述
        '這是一款精彩的' + @gameGenre + '遊戲，提供豐富的遊戲體驗和精美的畫面。玩家可以在這個虛擬世界中探索、戰鬥、成長，體驗前所未有的遊戲樂趣。',
        -- 封面圖片URL
        'https://picsum.photos/400/600?random=' + CAST(@j AS VARCHAR(10)),
        -- 評分 (7.0-9.5)
        7.0 + (ABS(CHECKSUM(NEWID()) % 26) / 10.0),
        -- 價格 (0-2000)
        ABS(CHECKSUM(NEWID()) % 2001),
        -- 是否啟用 (95% 啟用)
        CASE WHEN @j % 20 = 0 THEN 0 ELSE 1 END,
        GETDATE(),
        GETDATE()
    );
    
    SET @j = @j + 1;
END;

PRINT '已生成 ' + CAST(@maxGames AS VARCHAR(10)) + ' 筆遊戲資料';
```

---

## 4. 論壇資料生成 (1000+ 筆)

```sql
-- =============================================
-- 生成論壇分類資料
-- =============================================

-- 清空現有論壇資料
DELETE FROM Forums WHERE ForumID > 10;

-- 重置身份識別欄位
DBCC CHECKIDENT ('Forums', RESEED, 10);

-- 插入論壇分類
INSERT INTO Forums (Name, Description, Category, Is_Active, Created_At, Updated_At) VALUES
('英雄聯盟', 'League of Legends 討論區，包含攻略、賽事、心得分享', 'MOBA', 1, GETDATE(), GETDATE()),
('原神', 'Genshin Impact 討論區，角色配隊、抽卡心得、世界探索', 'RPG', 1, GETDATE(), GETDATE()),
('Steam 綜合', 'Steam 平台遊戲討論，促銷情報、遊戲心得、實況討論', 'PC', 1, GETDATE(), GETDATE()),
('手機遊戲', 'Android / iOS 手遊討論，包含各種熱門手遊', 'Mobile', 1, GETDATE(), GETDATE()),
('綜合討論', '硬體外設、雜談灌水、求助問答', 'General', 1, GETDATE(), GETDATE()),
('心情板', '日常、告白、碎碎念、抱怨', 'Social', 1, GETDATE(), GETDATE()),
('電競賽事', '各大電競賽事討論，包含 LOL、Dota2、CS2 等', 'Esports', 1, GETDATE(), GETDATE()),
('獨立遊戲', '獨立遊戲推薦、心得分享、開發者交流', 'Indie', 1, GETDATE(), GETDATE()),
('硬體討論', '電腦硬體、主機、外設討論', 'Hardware', 1, GETDATE(), GETDATE()),
('遊戲新聞', '最新遊戲資訊、業界新聞、更新情報', 'News', 1, GETDATE(), GETDATE());

-- =============================================
-- 生成討論串資料 (1000+ 筆)
-- =============================================

-- 清空現有討論串資料
DELETE FROM Threads WHERE ThreadID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('Threads', RESEED, 100);

-- 討論串標題模板
DECLARE @threadTitles TABLE (
    ID INT IDENTITY(1,1),
    Title NVARCHAR(200),
    Category NVARCHAR(50)
);

INSERT INTO @threadTitles (Title, Category) VALUES
-- 攻略類
('新手入門攻略 - 從零開始的遊戲指南', 'Guide'),
('角色配隊推薦 - 最強陣容分析', 'Guide'),
('裝備搭配心得 - 提升戰力的關鍵', 'Guide'),
('關卡攻略分享 - 困難關卡突破技巧', 'Guide'),
('技能連招教學 - 華麗連擊技巧', 'Guide'),

-- 心得類
('遊戲心得分享 - 100小時遊玩體驗', 'Review'),
('劇情解析討論 - 深度劇情分析', 'Review'),
('畫面表現評價 - 視覺效果評測', 'Review'),
('音效配樂心得 - 聽覺享受分享', 'Review'),
('操作手感體驗 - 遊戲性評價', 'Review'),

-- 情報類
('最新更新情報 - 版本更新內容', 'News'),
('活動預告分享 - 即將到來的活動', 'News'),
('新角色預告 - 即將登場的角色', 'News'),
('平衡性調整 - 角色技能調整', 'News'),
('Bug修復報告 - 已知問題修復', 'News'),

-- 閒聊類
('每日閒聊串 - 今天玩什麼遊戲？', 'Chat'),
('遊戲推薦分享 - 最近玩的好遊戲', 'Chat'),
('實況討論串 - 推薦實況主', 'Chat'),
('同人創作分享 - 玩家自製內容', 'Chat'),
('梗圖分享串 - 有趣的遊戲梗圖', 'Chat');

-- 生成討論串資料
DECLARE @k INT = 1;
DECLARE @maxThreads INT = 1000;

WHILE @k <= @maxThreads
BEGIN
    DECLARE @titleIndex INT = (@k - 1) % (SELECT COUNT(*) FROM @threadTitles) + 1;
    DECLARE @threadTitle NVARCHAR(200);
    DECLARE @threadCategory NVARCHAR(50);
    
    SELECT @threadTitle = Title, @threadCategory = Category 
    FROM @threadTitles WHERE ID = @titleIndex;
    
    -- 為重複的標題加上編號
    IF @k > (SELECT COUNT(*) FROM @threadTitles)
    BEGIN
        SET @threadTitle = @threadTitle + ' #' + CAST((@k / (SELECT COUNT(*) FROM @threadTitles)) + 1 AS VARCHAR(10));
    END;
    
    INSERT INTO Threads (
        ForumID,            -- 論壇ID
        UserID,             -- 發文者ID
        Title,              -- 標題
        Content,            -- 內容
        View_Count,         -- 瀏覽次數
        Reply_Count,        -- 回覆次數
        Like_Count,         -- 讚數
        Is_Sticky,          -- 是否置頂
        Is_Locked,          -- 是否鎖定
        Status,             -- 狀態
        Created_At,         -- 建立時間
        Updated_At          -- 更新時間
    )
    VALUES (
        -- 論壇ID (隨機選擇)
        (SELECT TOP 1 ForumID FROM Forums ORDER BY NEWID()),
        -- 用戶ID (隨機選擇)
        (SELECT TOP 1 UserID FROM Users WHERE UserID > 100 ORDER BY NEWID()),
        @threadTitle,
        -- 內容 (根據類別生成不同內容)
        CASE @threadCategory
            WHEN 'Guide' THEN '這是一篇詳細的攻略指南，包含了完整的遊戲流程和技巧分享。希望對新手玩家有所幫助！'
            WHEN 'Review' THEN '經過長時間的遊玩體驗，我想分享一些個人的心得和感受。這款遊戲確實有很多值得稱讚的地方。'
            WHEN 'News' THEN '最新消息！官方剛剛發布了重要的更新情報，大家一起來討論吧！'
            WHEN 'Chat' THEN '大家最近都在玩什麼遊戲呢？有什麼有趣的事情想分享的嗎？'
            ELSE '這是一個有趣的討論話題，歡迎大家一起參與討論！'
        END,
        -- 瀏覽次數 (100-10000)
        ABS(CHECKSUM(NEWID()) % 9901) + 100,
        -- 回覆次數 (0-500)
        ABS(CHECKSUM(NEWID()) % 501),
        -- 讚數 (0-200)
        ABS(CHECKSUM(NEWID()) % 201),
        -- 是否置頂 (5% 機率)
        CASE WHEN @k % 20 = 0 THEN 1 ELSE 0 END,
        -- 是否鎖定 (1% 機率)
        CASE WHEN @k % 100 = 0 THEN 1 ELSE 0 END,
        -- 狀態
        'Active',
        -- 建立時間 (隨機分布在過去一年內)
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 365), GETDATE()),
        GETDATE()
    );
    
    SET @k = @k + 1;
END;

PRINT '已生成 ' + CAST(@maxThreads AS VARCHAR(10)) + ' 筆討論串資料';
```

---

## 5. 市集商品資料生成 (2000+ 筆)

```sql
-- =============================================
-- 生成市集商品資料 (2000+ 筆)
-- 包含各種類型的虛擬商品
-- =============================================

-- 清空現有商品資料
DELETE FROM MarketProducts WHERE ProductID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('MarketProducts', RESEED, 100);

-- 商品名稱模板
DECLARE @productNames TABLE (
    ID INT IDENTITY(1,1),
    Name NVARCHAR(200),
    Category NVARCHAR(50),
    Description NVARCHAR(500)
);

INSERT INTO @productNames (Name, Category, Description) VALUES
-- 遊戲道具
('傳說級武器 - 龍牙劍', 'Weapon', '稀有度極高的傳說級武器，擁有強大的攻擊力和特殊效果'),
('史詩級防具 - 龍鱗甲', 'Armor', '由龍鱗製成的史詩級防具，提供極高的防禦力'),
('稀有寵物 - 火焰鳳凰', 'Pet', '稀有的火焰鳳凰寵物，擁有獨特的外觀和技能'),
('限定坐騎 - 幽靈馬', 'Mount', '限定的幽靈馬坐騎，移動速度極快且外觀炫酷'),
('特殊技能書 - 終極技', 'Skill', '學習終極技能的技能書，大幅提升角色戰鬥力'),

-- 遊戲貨幣
('遊戲金幣包 - 1000金', 'Currency', '包含1000遊戲金幣的禮包，可用於遊戲內消費'),
('鑽石禮包 - 100鑽', 'Currency', '包含100鑽石的禮包，可用於購買特殊道具'),
('點券包 - 500點', 'Currency', '包含500點券的禮包，可用於商城購物'),
('代幣包 - 200代幣', 'Currency', '包含200代幣的禮包，可用於抽獎活動'),

-- 遊戲帳號
('高級遊戲帳號', 'Account', '等級高、裝備齊全的遊戲帳號，適合新手玩家'),
('稀有角色帳號', 'Account', '擁有稀有角色的遊戲帳號，角色收集度極高'),
('競技帳號', 'Account', '專注於競技模式的帳號，技術水平高'),

-- 遊戲服務
('代練服務 - 等級提升', 'Service', '專業代練服務，快速提升角色等級'),
('代打服務 - 競技模式', 'Service', '專業代打服務，幫助提升競技排名'),
('裝備強化服務', 'Service', '專業裝備強化服務，提升裝備品質'),

-- 實體商品
('遊戲周邊 - 角色手辦', 'Merchandise', '精美的遊戲角色手辦，收藏價值極高'),
('遊戲周邊 - 主題T恤', 'Merchandise', '遊戲主題T恤，舒適透氣且設計精美'),
('遊戲周邊 - 鍵盤滑鼠', 'Merchandise', '遊戲專用鍵盤滑鼠，提升遊戲體驗'),
('遊戲周邊 - 海報', 'Merchandise', '高品質遊戲海報，適合裝飾房間');

-- 生成商品資料
DECLARE @l INT = 1;
DECLARE @maxProducts INT = 2000;

WHILE @l <= @maxProducts
BEGIN
    DECLARE @productIndex INT = (@l - 1) % (SELECT COUNT(*) FROM @productNames) + 1;
    DECLARE @productName NVARCHAR(200);
    DECLARE @productCategory NVARCHAR(50);
    DECLARE @productDescription NVARCHAR(500);
    
    SELECT @productName = Name, @productCategory = Category, @productDescription = Description 
    FROM @productNames WHERE ID = @productIndex;
    
    -- 為重複的商品名稱加上編號
    IF @l > (SELECT COUNT(*) FROM @productNames)
    BEGIN
        SET @productName = @productName + ' #' + CAST((@l / (SELECT COUNT(*) FROM @productNames)) + 1 AS VARCHAR(10));
    END;
    
    INSERT INTO MarketProducts (
        SellerID,           -- 賣家ID
        Product_Name,       -- 商品名稱
        Description,        -- 商品描述
        Price,              -- 價格
        Original_Price,     -- 原價
        Category,           -- 分類
        Condition,          -- 商品狀況
        Quantity,           -- 數量
        PProduct_Img_URL,   -- 商品圖片URL
        Tags,               -- 標籤
        Is_Active,          -- 是否啟用
        View_Count,         -- 瀏覽次數
        Like_Count,         -- 讚數
        Created_At,         -- 建立時間
        Updated_At          -- 更新時間
    )
    VALUES (
        -- 賣家ID (隨機選擇用戶)
        (SELECT TOP 1 UserID FROM Users WHERE UserID > 100 ORDER BY NEWID()),
        @productName,
        @productDescription + ' 這是一個高品質的商品，值得購買！',
        -- 價格 (100-10000)
        ABS(CHECKSUM(NEWID()) % 9901) + 100,
        -- 原價 (價格的1.2-1.5倍)
        (ABS(CHECKSUM(NEWID()) % 9901) + 100) * (1.2 + (ABS(CHECKSUM(NEWID()) % 31) / 100.0)),
        @productCategory,
        -- 商品狀況
        CASE 
            WHEN @l % 4 = 0 THEN 'New'
            WHEN @l % 4 = 1 THEN 'Like New'
            WHEN @l % 4 = 2 THEN 'Good'
            ELSE 'Fair'
        END,
        -- 數量 (1-100)
        ABS(CHECKSUM(NEWID()) % 100) + 1,
        -- 商品圖片URL
        'https://picsum.photos/400/400?random=' + CAST(@l AS VARCHAR(10)),
        -- 標籤 (JSON格式)
        '["遊戲", "' + @productCategory + '", "熱門", "推薦"]',
        -- 是否啟用 (90% 啟用)
        CASE WHEN @l % 10 = 0 THEN 0 ELSE 1 END,
        -- 瀏覽次數 (50-5000)
        ABS(CHECKSUM(NEWID()) % 4951) + 50,
        -- 讚數 (0-200)
        ABS(CHECKSUM(NEWID()) % 201),
        GETDATE(),
        GETDATE()
    );
    
    SET @l = @l + 1;
END;

PRINT '已生成 ' + CAST(@maxProducts AS VARCHAR(10)) + ' 筆商品資料';
```

---

## 6. 虛擬寵物資料生成

```sql
-- =============================================
-- 生成虛擬寵物資料
-- 為每個用戶建立虛擬寵物
-- =============================================

-- 清空現有寵物資料
DELETE FROM VirtualPets WHERE UserID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('VirtualPets', RESEED, 100);

-- 寵物名稱模板
DECLARE @petNames TABLE (
    ID INT IDENTITY(1,1),
    Name NVARCHAR(100)
);

INSERT INTO @petNames (Name) VALUES
('史萊姆王'),
('火焰史萊姆'),
('冰霜史萊姆'),
('雷電史萊姆'),
('暗影史萊姆'),
('彩虹史萊姆'),
('金屬史萊姆'),
('水晶史萊姆'),
('幽靈史萊姆'),
('龍族史萊姆');

-- 為每個用戶建立虛擬寵物
INSERT INTO VirtualPets (
    UserID,             -- 用戶ID
    Name,               -- 寵物名稱
    Type,               -- 寵物類型
    Level,              -- 等級
    Experience,         -- 經驗值
    Max_Experience,     -- 最大經驗值
    Hunger,             -- 飢餓度
    Mood,               -- 心情
    Energy,             -- 體力
    Cleanliness,        -- 清潔度
    Health,             -- 健康度
    Last_Care_Date,     -- 最後照顧日期
    Created_At,         -- 建立時間
    Updated_At          -- 更新時間
)
SELECT 
    u.UserID,
    -- 寵物名稱 (隨機選擇)
    (SELECT TOP 1 Name FROM @petNames ORDER BY NEWID()) + ' ' + CAST(u.UserID AS VARCHAR(10)),
    -- 寵物類型
    'Slime',
    -- 等級 (1-50)
    ABS(CHECKSUM(NEWID()) % 50) + 1,
    -- 經驗值 (0-最大經驗值)
    ABS(CHECKSUM(NEWID()) % 1000),
    -- 最大經驗值 (等級 * 100)
    (ABS(CHECKSUM(NEWID()) % 50) + 1) * 100,
    -- 飢餓度 (0-100)
    ABS(CHECKSUM(NEWID()) % 101),
    -- 心情 (0-100)
    ABS(CHECKSUM(NEWID()) % 101),
    -- 體力 (0-100)
    ABS(CHECKSUM(NEWID()) % 101),
    -- 清潔度 (0-100)
    ABS(CHECKSUM(NEWID()) % 101),
    -- 健康度 (0-100)
    ABS(CHECKSUM(NEWID()) % 101),
    -- 最後照顧日期 (隨機分布在過去7天內)
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 7), GETDATE()),
    GETDATE(),
    GETDATE()
FROM Users u
WHERE u.UserID > 100;

PRINT '已生成虛擬寵物資料';
```

---

## 7. 排行榜資料生成

```sql
-- =============================================
-- 生成遊戲指標資料
-- =============================================

-- 清空現有指標資料
DELETE FROM GameMetrics WHERE MetricID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('GameMetrics', RESEED, 100);

-- 為每個遊戲生成指標資料
INSERT INTO GameMetrics (
    GameID,             -- 遊戲ID
    Metric_Type,        -- 指標類型
    Value,              -- 數值
    Date,               -- 日期
    Created_At,         -- 建立時間
    Updated_At          -- 更新時間
)
SELECT 
    g.GameID,
    -- 指標類型
    CASE 
        WHEN m.MetricID % 4 = 0 THEN 'PlayerCount'
        WHEN m.MetricID % 4 = 1 THEN 'Revenue'
        WHEN m.MetricID % 4 = 2 THEN 'Rating'
        ELSE 'Downloads'
    END,
    -- 數值 (根據類型設定不同範圍)
    CASE 
        WHEN m.MetricID % 4 = 0 THEN ABS(CHECKSUM(NEWID()) % 1000000) + 1000  -- 玩家數
        WHEN m.MetricID % 4 = 1 THEN ABS(CHECKSUM(NEWID()) % 10000000) + 10000 -- 收入
        WHEN m.MetricID % 4 = 2 THEN 7.0 + (ABS(CHECKSUM(NEWID()) % 31) / 10.0) -- 評分
        ELSE ABS(CHECKSUM(NEWID()) % 500000) + 1000 -- 下載量
    END,
    -- 日期 (隨機分布在過去30天內)
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()),
    GETDATE(),
    GETDATE()
FROM Games g
CROSS JOIN (SELECT ROW_NUMBER() OVER (ORDER BY GameID) AS MetricID FROM Games) m
WHERE g.GameID > 100 AND m.MetricID <= 1000;

PRINT '已生成遊戲指標資料';

-- =============================================
-- 生成熱度指數資料
-- =============================================

-- 清空現有熱度資料
DELETE FROM PopularityIndexDailies WHERE IndexID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('PopularityIndexDailies', RESEED, 100);

-- 為每個遊戲生成熱度指數
INSERT INTO PopularityIndexDailies (
    GameID,             -- 遊戲ID
    Popularity_Score,   -- 熱度分數
    Rank,               -- 排名
    Date,               -- 日期
    Created_At,         -- 建立時間
    Updated_At          -- 更新時間
)
SELECT 
    g.GameID,
    -- 熱度分數 (0-100)
    ABS(CHECKSUM(NEWID()) % 101),
    -- 排名 (1-500)
    ROW_NUMBER() OVER (ORDER BY ABS(CHECKSUM(NEWID()) % 101) DESC),
    -- 日期 (隨機分布在過去30天內)
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()),
    GETDATE(),
    GETDATE()
FROM Games g
WHERE g.GameID > 100;

PRINT '已生成熱度指數資料';
```

---

## 8. 通知資料生成

```sql
-- =============================================
-- 生成通知資料
-- =============================================

-- 清空現有通知資料
DELETE FROM Notifications WHERE NotificationID > 100;

-- 重置身份識別欄位
DBCC CHECKIDENT ('Notifications', RESEED, 100);

-- 通知類型模板
DECLARE @notificationTypes TABLE (
    ID INT IDENTITY(1,1),
    Type NVARCHAR(50),
    Title NVARCHAR(200),
    Content NVARCHAR(500)
);

INSERT INTO @notificationTypes (Type, Title, Content) VALUES
('System', '系統維護通知', '系統將於今晚進行維護，預計維護時間2小時'),
('Market', '商品售出通知', '您的商品已成功售出，請查看訂單詳情'),
('Forum', '回覆通知', '有人回覆了您的文章，快來看看吧'),
('Pet', '寵物提醒', '您的寵物需要照顧了，記得餵食哦'),
('Game', '遊戲更新通知', '您關注的遊戲有新版本更新了'),
('Social', '好友邀請', '您收到了新的好友邀請'),
('Event', '活動通知', '新的活動開始了，快來參與吧'),
('Achievement', '成就解鎖', '恭喜您解鎖了新的成就！');

-- 生成通知資料
DECLARE @n INT = 1;
DECLARE @maxNotifications INT = 5000;

WHILE @n <= @maxNotifications
BEGIN
    DECLARE @notifIndex INT = (@n - 1) % (SELECT COUNT(*) FROM @notificationTypes) + 1;
    DECLARE @notifType NVARCHAR(50);
    DECLARE @notifTitle NVARCHAR(200);
    DECLARE @notifContent NVARCHAR(500);
    
    SELECT @notifType = Type, @notifTitle = Title, @notifContent = Content 
    FROM @notificationTypes WHERE ID = @notifIndex;
    
    INSERT INTO Notifications (
        UserID,             -- 用戶ID
        Type,               -- 通知類型
        Title,              -- 標題
        Content,            -- 內容
        Is_Read,            -- 是否已讀
        Created_At,         -- 建立時間
        Updated_At          -- 更新時間
    )
    VALUES (
        -- 用戶ID (隨機選擇)
        (SELECT TOP 1 UserID FROM Users WHERE UserID > 100 ORDER BY NEWID()),
        @notifType,
        @notifTitle + ' #' + CAST(@n AS VARCHAR(10)),
        @notifContent + ' 這是一個重要的通知，請及時處理。',
        -- 是否已讀 (70% 未讀)
        CASE WHEN @n % 10 < 7 THEN 0 ELSE 1 END,
        -- 建立時間 (隨機分布在過去30天內)
        DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 30), GETDATE()),
        GETDATE()
    );
    
    SET @n = @n + 1;
END;

PRINT '已生成 ' + CAST(@maxNotifications AS VARCHAR(10)) + ' 筆通知資料';
```

---

## 9. 執行總結

```sql
-- =============================================
-- 資料生成完成總結
-- =============================================

PRINT '=============================================';
PRINT 'GameCore 資料庫假資料生成完成';
PRINT '=============================================';
PRINT '';

-- 統計各表資料量
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users WHERE UserID > 100
UNION ALL
SELECT 'UserWallets', COUNT(*) FROM UserWallets WHERE UserID > 100
UNION ALL
SELECT 'Games', COUNT(*) FROM Games WHERE GameID > 100
UNION ALL
SELECT 'Forums', COUNT(*) FROM Forums WHERE ForumID > 10
UNION ALL
SELECT 'Threads', COUNT(*) FROM Threads WHERE ThreadID > 100
UNION ALL
SELECT 'MarketProducts', COUNT(*) FROM MarketProducts WHERE ProductID > 100
UNION ALL
SELECT 'VirtualPets', COUNT(*) FROM VirtualPets WHERE UserID > 100
UNION ALL
SELECT 'GameMetrics', COUNT(*) FROM GameMetrics WHERE MetricID > 100
UNION ALL
SELECT 'PopularityIndexDailies', COUNT(*) FROM PopularityIndexDailies WHERE IndexID > 100
UNION ALL
SELECT 'Notifications', COUNT(*) FROM Notifications WHERE NotificationID > 100;

PRINT '';
PRINT '資料生成統計：';
PRINT '- 用戶資料：1000+ 筆';
PRINT '- 遊戲資料：500+ 筆';
PRINT '- 論壇文章：1000+ 筆';
PRINT '- 市集商品：2000+ 筆';
PRINT '- 通知資料：5000+ 筆';
PRINT '- 總計：10000+ 筆假資料';
PRINT '';
PRINT '所有假資料已成功生成並儲存到資料庫中！';
PRINT '=============================================';
```

---

## 注意事項

1. **執行前備份：** 請在執行此腳本前備份重要資料
2. **測試環境：** 建議在測試環境中執行，避免影響生產資料
3. **執行時間：** 腳本執行時間約需 5-10 分鐘
4. **資料清理：** 如需清理假資料，可使用對應的 DELETE 語句
5. **效能考量：** 大量資料插入可能影響資料庫效能，建議分批執行

## 使用說明

1. 將此腳本複製到 SQL Server Management Studio
2. 選擇目標資料庫
3. 執行腳本
4. 檢查執行結果和統計資訊
5. 驗證資料是否正確生成

---

**腳本版本：** v1.0  
**建立日期：** 2024年12月19日  
**適用資料庫：** SQL Server 2022  
**專案：** GameCore 遊戲社群平台 