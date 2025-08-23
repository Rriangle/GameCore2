-- GameCore 資料庫初始化腳本
-- 執行前請確保已建立 GameCore 資料庫

USE GameCore;
GO

-- 建立資料庫結構
-- 注意：此腳本僅建立基本結構，詳細的表格結構請參考 dbdiagram.io 設計

PRINT '開始初始化 GameCore 資料庫...';

-- 檢查並建立資料庫
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'GameCore')
BEGIN
    CREATE DATABASE GameCore;
    PRINT '已建立 GameCore 資料庫';
END
ELSE
BEGIN
    PRINT 'GameCore 資料庫已存在';
END

USE GameCore;
GO

-- 建立基本表格結構（這裡只建立幾個關鍵表格作為範例）
-- 完整的表格結構請參考專案中的 Entity 類別

-- 建立用戶表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'users')
BEGIN
    CREATE TABLE users (
        user_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(50) NOT NULL UNIQUE,
        email NVARCHAR(100) NOT NULL UNIQUE,
        password_hash NVARCHAR(255) NOT NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'active',
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL
    );
    PRINT '已建立 users 表格';
END

-- 建立遊戲表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'games')
BEGIN
    CREATE TABLE games (
        game_id INT IDENTITY(1,1) PRIMARY KEY,
        name NVARCHAR(100) NOT NULL,
        description NVARCHAR(500) NULL,
        category NVARCHAR(50) NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'active',
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL
    );
    PRINT '已建立 games 表格';
END

-- 建立指標表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'metrics')
BEGIN
    CREATE TABLE metrics (
        metric_id INT IDENTITY(1,1) PRIMARY KEY,
        metric_name NVARCHAR(100) NOT NULL,
        description NVARCHAR(500) NULL,
        metric_type NVARCHAR(50) NOT NULL,
        unit NVARCHAR(20) NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'active',
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL
    );
    PRINT '已建立 metrics 表格';
END

-- 建立指標來源表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'metric_sources')
BEGIN
    CREATE TABLE metric_sources (
        source_id INT IDENTITY(1,1) PRIMARY KEY,
        source_name NVARCHAR(100) NOT NULL,
        description NVARCHAR(500) NULL,
        source_type NVARCHAR(50) NOT NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'active',
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL
    );
    PRINT '已建立 metric_sources 表格';
END

-- 建立遊戲來源對應表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'game_source_maps')
BEGIN
    CREATE TABLE game_source_maps (
        map_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        game_id INT NOT NULL,
        source_id INT NOT NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'active',
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NULL,
        FOREIGN KEY (game_id) REFERENCES games(game_id),
        FOREIGN KEY (source_id) REFERENCES metric_sources(source_id)
    );
    PRINT '已建立 game_source_maps 表格';
END

-- 建立遊戲每日指標表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'game_metric_dailies')
BEGIN
    CREATE TABLE game_metric_dailies (
        record_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        game_id INT NOT NULL,
        metric_id INT NOT NULL,
        record_date DATETIME2 NOT NULL,
        metric_value DECIMAL(18,4) NOT NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (game_id) REFERENCES games(game_id),
        FOREIGN KEY (metric_id) REFERENCES metrics(metric_id)
    );
    PRINT '已建立 game_metric_dailies 表格';
END

-- 建立熱度指數每日表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'popularity_index_dailies')
BEGIN
    CREATE TABLE popularity_index_dailies (
        record_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        game_id INT NOT NULL,
        record_date DATETIME2 NOT NULL,
        popularity_index DECIMAL(18,4) NOT NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (game_id) REFERENCES games(game_id)
    );
    PRINT '已建立 popularity_index_dailies 表格';
END

-- 建立排行榜快照表格
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'leaderboard_snapshots')
BEGIN
    CREATE TABLE leaderboard_snapshots (
        snapshot_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        period NVARCHAR(20) NOT NULL,
        ts DATETIME2 NOT NULL,
        rank INT NOT NULL,
        game_id INT NOT NULL,
        index_value DECIMAL(18,4) NOT NULL,
        change_rate DECIMAL(18,4) NULL,
        change_amount DECIMAL(18,4) NULL,
        index_grade NVARCHAR(10) NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY (game_id) REFERENCES games(game_id)
    );
    PRINT '已建立 leaderboard_snapshots 表格';
END

PRINT '資料庫初始化完成！';
PRINT '請執行假資料 Seeder 腳本來填充測試資料。';
GO