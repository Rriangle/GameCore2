-- GameCore 假資料 Seeder 腳本
-- 執行前請確保已執行 init-database.sql

USE GameCore;
GO

PRINT '開始填充假資料...';

-- 插入遊戲資料
INSERT INTO games (name, description, category, status) VALUES
('英雄聯盟', '多人在線戰術競技遊戲', 'MOBA', 'active'),
('絕地求生', '大逃殺射擊遊戲', 'FPS', 'active'),
('原神', '開放世界冒險RPG', 'RPG', 'active'),
('王者榮耀', '手機MOBA遊戲', 'MOBA', 'active'),
('和平精英', '手機大逃殺遊戲', 'FPS', 'active'),
('崩壞：星穹鐵道', '回合制策略RPG', 'RPG', 'active'),
('爐石戰記', '數位卡牌遊戲', 'CCG', 'active'),
('守望先鋒2', '團隊射擊遊戲', 'FPS', 'active'),
('魔獸世界', '大型多人在線RPG', 'MMORPG', 'active'),
('Dota 2', '多人在線戰術競技遊戲', 'MOBA', 'active');

PRINT '已插入 10 筆遊戲資料';

-- 插入指標資料
INSERT INTO metrics (metric_name, description, metric_type, unit, status) VALUES
('同時在線人數', '遊戲同時在線的玩家數量', 'count', '人', 'active'),
('日活躍用戶', '每日活躍的用戶數量', 'count', '人', 'active'),
('月活躍用戶', '每月活躍的用戶數量', 'count', '人', 'active'),
('評論數量', '遊戲評論的總數量', 'count', '條', 'active'),
('評分', '遊戲的平均評分', 'rating', '分', 'active'),
('下載量', '遊戲的下載次數', 'count', '次', 'active'),
('收入', '遊戲的營收金額', 'currency', '元', 'active'),
('留存率', '用戶留存的比例', 'percentage', '%', 'active');

PRINT '已插入 8 筆指標資料';

-- 插入指標來源資料
INSERT INTO metric_sources (source_name, description, source_type, status) VALUES
('Steam', 'Steam 平台數據', 'api', 'active'),
('巴哈姆特', '巴哈姆特遊戲資訊', 'website', 'active'),
('App Store', 'iOS 應用商店', 'api', 'active'),
('Google Play', 'Android 應用商店', 'api', 'active'),
('Twitch', 'Twitch 直播平台', 'api', 'active'),
('YouTube', 'YouTube 遊戲頻道', 'api', 'active'),
('Discord', 'Discord 社群數據', 'api', 'active'),
('Reddit', 'Reddit 遊戲討論', 'api', 'active');

PRINT '已插入 8 筆指標來源資料';

-- 插入遊戲來源對應資料
INSERT INTO game_source_maps (game_id, source_id, status) VALUES
(1, 1, 'active'), -- 英雄聯盟 - Steam
(1, 5, 'active'), -- 英雄聯盟 - Twitch
(1, 6, 'active'), -- 英雄聯盟 - YouTube
(2, 1, 'active'), -- 絕地求生 - Steam
(2, 5, 'active'), -- 絕地求生 - Twitch
(3, 3, 'active'), -- 原神 - App Store
(3, 4, 'active'), -- 原神 - Google Play
(4, 3, 'active'), -- 王者榮耀 - App Store
(4, 4, 'active'), -- 王者榮耀 - Google Play
(5, 3, 'active'), -- 和平精英 - App Store
(5, 4, 'active'), -- 和平精英 - Google Play
(6, 3, 'active'), -- 崩壞：星穹鐵道 - App Store
(6, 4, 'active'), -- 崩壞：星穹鐵道 - Google Play
(7, 1, 'active'), -- 爐石戰記 - Steam
(7, 5, 'active'), -- 爐石戰記 - Twitch
(8, 1, 'active'), -- 守望先鋒2 - Steam
(8, 5, 'active'), -- 守望先鋒2 - Twitch
(9, 1, 'active'), -- 魔獸世界 - Steam
(9, 5, 'active'), -- 魔獸世界 - Twitch
(10, 1, 'active'); -- Dota 2 - Steam

PRINT '已插入 20 筆遊戲來源對應資料';

-- 插入遊戲每日指標資料（最近30天的數據）
DECLARE @StartDate DATE = DATEADD(DAY, -30, GETDATE());
DECLARE @CurrentDate DATE = @StartDate;
DECLARE @GameId INT = 1;
DECLARE @MetricId INT = 1;

WHILE @CurrentDate <= GETDATE()
BEGIN
    WHILE @GameId <= 10
    BEGIN
        WHILE @MetricId <= 8
        BEGIN
            -- 根據遊戲和指標類型生成合理的假數據
            DECLARE @MetricValue DECIMAL(18,4);
            
            IF @MetricId = 1 -- 同時在線人數
                SET @MetricValue = ABS(CHECKSUM(NEWID())) % 100000 + 10000;
            ELSE IF @MetricId = 2 -- 日活躍用戶
                SET @MetricValue = ABS(CHECKSUM(NEWID())) % 500000 + 50000;
            ELSE IF @MetricId = 3 -- 月活躍用戶
                SET @MetricValue = ABS(CHECKSUM(NEWID())) % 2000000 + 200000;
            ELSE IF @MetricId = 4 -- 評論數量
                SET @MetricValue = ABS(CHECKSUM(NEWID())) % 10000 + 1000;
            ELSE IF @MetricId = 5 -- 評分
                SET @MetricValue = CAST(ABS(CHECKSUM(NEWID())) % 50 + 50 AS DECIMAL(18,4)) / 10;
            ELSE IF @MetricId = 6 -- 下載量
                SET @MetricValue = ABS(CHECKSUM(NEWID())) % 100000 + 10000;
            ELSE IF @MetricId = 7 -- 收入
                SET @MetricValue = ABS(CHECKSUM(NEWID())) % 1000000 + 100000;
            ELSE -- 留存率
                SET @MetricValue = CAST(ABS(CHECKSUM(NEWID())) % 40 + 60 AS DECIMAL(18,4));
            
            INSERT INTO game_metric_dailies (game_id, metric_id, record_date, metric_value)
            VALUES (@GameId, @MetricId, @CurrentDate, @MetricValue);
            
            SET @MetricId = @MetricId + 1;
        END
        
        SET @MetricId = 1;
        SET @GameId = @GameId + 1;
    END
    
    SET @GameId = 1;
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

PRINT '已插入遊戲每日指標資料（30天 x 10遊戲 x 8指標 = 2400筆）';

-- 插入熱度指數每日資料（最近30天的數據）
SET @CurrentDate = @StartDate;
SET @GameId = 1;

WHILE @CurrentDate <= GETDATE()
BEGIN
    WHILE @GameId <= 10
    BEGIN
        DECLARE @PopularityIndex DECIMAL(18,4) = ABS(CHECKSUM(NEWID())) % 1000 + 100;
        
        INSERT INTO popularity_index_dailies (game_id, record_date, popularity_index)
        VALUES (@GameId, @CurrentDate, @PopularityIndex);
        
        SET @GameId = @GameId + 1;
    END
    
    SET @GameId = 1;
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

PRINT '已插入熱度指數每日資料（30天 x 10遊戲 = 300筆）';

-- 插入排行榜快照資料
INSERT INTO leaderboard_snapshots (period, ts, rank, game_id, index_value, change_rate, change_amount, index_grade) VALUES
('daily', DATEADD(DAY, -1, GETDATE()), 1, 1, 950.5, 5.2, 47.1, 'S'),
('daily', DATEADD(DAY, -1, GETDATE()), 2, 3, 920.3, 3.1, 27.7, 'S'),
('daily', DATEADD(DAY, -1, GETDATE()), 3, 2, 890.7, -2.3, -21.0, 'A'),
('daily', DATEADD(DAY, -1, GETDATE()), 4, 4, 870.2, 1.8, 15.4, 'A'),
('daily', DATEADD(DAY, -1, GETDATE()), 5, 5, 850.9, -1.2, -10.3, 'A'),
('weekly', DATEADD(WEEK, -1, GETDATE()), 1, 1, 945.8, 8.5, 74.2, 'S'),
('weekly', DATEADD(WEEK, -1, GETDATE()), 2, 3, 918.6, 6.2, 53.7, 'S'),
('weekly', DATEADD(WEEK, -1, GETDATE()), 3, 2, 885.4, -3.1, -28.3, 'A'),
('monthly', DATEADD(MONTH, -1, GETDATE()), 1, 1, 940.2, 12.3, 103.1, 'S'),
('monthly', DATEADD(MONTH, -1, GETDATE()), 2, 3, 915.7, 9.8, 81.6, 'S');

PRINT '已插入 10 筆排行榜快照資料';

-- 插入用戶資料
INSERT INTO users (username, email, password_hash, status) VALUES
('admin', 'admin@gamecore.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'active'),
('testuser1', 'user1@test.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'active'),
('testuser2', 'user2@test.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'active'),
('gamemaster', 'gm@gamecore.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'active'),
('moderator', 'mod@gamecore.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'active');

PRINT '已插入 5 筆用戶資料';

PRINT '假資料填充完成！';
PRINT '總計插入：';
PRINT '- 遊戲：10筆';
PRINT '- 指標：8筆';
PRINT '- 指標來源：8筆';
PRINT '- 遊戲來源對應：20筆';
PRINT '- 遊戲每日指標：2400筆';
PRINT '- 熱度指數每日：300筆';
PRINT '- 排行榜快照：10筆';
PRINT '- 用戶：5筆';
GO