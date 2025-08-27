-- =============================================================================
-- GameCore 資料庫初始化腳本
-- =============================================================================
-- 
-- 使用說明：
-- 1. 此腳本用於初始化 GameCore 資料庫
-- 2. 適用於 SQL Server 2019+ 版本
-- 3. 執行前請確認資料庫連線權限
-- 4. 建議在開發環境使用，生產環境請使用 EF Core 遷移
--
-- 執行方式：
-- sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -i init-db.sql
-- 或使用 SQL Server Management Studio 執行
-- =============================================================================

USE master;
GO

-- 檢查資料庫是否存在，如果存在則刪除
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'GameCore')
BEGIN
    ALTER DATABASE GameCore SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GameCore;
    PRINT '已刪除現有的 GameCore 資料庫';
END
GO

-- 建立新的 GameCore 資料庫
CREATE DATABASE GameCore
ON PRIMARY (
    NAME = 'GameCore_Data',
    FILENAME = 'C:\Data\GameCore_Data.mdf',
    SIZE = 100MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10MB
)
LOG ON (
    NAME = 'GameCore_Log',
    FILENAME = 'C:\Data\GameCore_Log.ldf',
    SIZE = 50MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 10MB
);
GO

-- 設定資料庫選項
ALTER DATABASE GameCore SET RECOVERY SIMPLE;
ALTER DATABASE GameCore SET AUTO_CLOSE OFF;
ALTER DATABASE GameCore SET AUTO_SHRINK OFF;
GO

-- 切換到 GameCore 資料庫
USE GameCore;
GO

-- 建立資料庫角色
CREATE ROLE GameCoreUser;
CREATE ROLE GameCoreAdmin;
GO

-- 建立基本表格結構
-- 用戶表格
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Avatar NVARCHAR(255),
    IsActive BIT NOT NULL DEFAULT 1,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PhoneNumber NVARCHAR(20),
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    TwoFactorEnabled BIT NOT NULL DEFAULT 0,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT NOT NULL DEFAULT 1,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastLoginAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER
);
GO

-- 用戶角色表格
CREATE TABLE UserRoles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleName NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- 遊戲表格
CREATE TABLE Games (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000),
    Genre NVARCHAR(50),
    Platform NVARCHAR(50),
    Publisher NVARCHAR(100),
    Developer NVARCHAR(100),
    ReleaseDate DATE,
    CoverImage NVARCHAR(255),
    SteamAppId INT,
    MetacriticScore DECIMAL(3,1),
    UserRating DECIMAL(3,2),
    TotalRatings INT DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER
);
GO

-- 遊戲熱度表格
CREATE TABLE GameHeat (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GameId UNIQUEIDENTIFIER NOT NULL,
    HeatScore DECIMAL(5,2) NOT NULL DEFAULT 0,
    PlayerCount INT DEFAULT 0,
    ViewCount INT DEFAULT 0,
    DiscussionCount INT DEFAULT 0,
    MarketActivity DECIMAL(10,2) DEFAULT 0,
    CalculatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (GameId) REFERENCES Games(Id) ON DELETE CASCADE
);
GO

-- 論壇分類表格
CREATE TABLE ForumCategories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ParentCategoryId UNIQUEIDENTIFIER,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (ParentCategoryId) REFERENCES ForumCategories(Id)
);
GO

-- 論壇文章表格
CREATE TABLE ForumPosts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    AuthorId UNIQUEIDENTIFIER NOT NULL,
    GameId UNIQUEIDENTIFIER,
    ViewCount INT DEFAULT 0,
    LikeCount INT DEFAULT 0,
    DislikeCount INT DEFAULT 0,
    IsSticky BIT NOT NULL DEFAULT 0,
    IsLocked BIT NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Published',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    FOREIGN KEY (CategoryId) REFERENCES ForumCategories(Id),
    FOREIGN KEY (AuthorId) REFERENCES Users(Id),
    FOREIGN KEY (GameId) REFERENCES Games(Id)
);
GO

-- 商品分類表格
CREATE TABLE ProductCategories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ParentCategoryId UNIQUEIDENTIFIER,
    DisplayOrder INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (ParentCategoryId) REFERENCES ProductCategories(Id)
);
GO

-- 商品表格
CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    GameId UNIQUEIDENTIFIER,
    Price DECIMAL(10,2) NOT NULL,
    OriginalPrice DECIMAL(10,2),
    Currency NVARCHAR(3) NOT NULL DEFAULT 'TWD',
    StockQuantity INT NOT NULL DEFAULT 0,
    IsDigital BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    ImageUrls NVARCHAR(MAX), -- JSON 格式儲存多張圖片
    Tags NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    FOREIGN KEY (CategoryId) REFERENCES ProductCategories(Id),
    FOREIGN KEY (GameId) REFERENCES Games(Id)
);
GO

-- 寵物表格
CREATE TABLE Pets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    Level INT NOT NULL DEFAULT 1,
    Experience INT NOT NULL DEFAULT 0,
    MaxExperience INT NOT NULL DEFAULT 100,
    Hunger INT NOT NULL DEFAULT 100,
    Happiness INT NOT NULL DEFAULT 100,
    Health INT NOT NULL DEFAULT 100,
    LastFedAt DATETIME2,
    LastPlayedAt DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (OwnerId) REFERENCES Users(Id)
);
GO

-- 交易記錄表格
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TransactionType NVARCHAR(20) NOT NULL, -- Purchase, Sale, Transfer
    BuyerId UNIQUEIDENTIFIER,
    SellerId UNIQUEIDENTIFIER,
    ProductId UNIQUEIDENTIFIER,
    Quantity INT NOT NULL DEFAULT 1,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Commission DECIMAL(10,2) DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    PaymentMethod NVARCHAR(50),
    TransactionDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    Notes NVARCHAR(500),
    FOREIGN KEY (BuyerId) REFERENCES Users(Id),
    FOREIGN KEY (SellerId) REFERENCES Users(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO

-- 建立索引
-- 用戶表格索引
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);

-- 遊戲表格索引
CREATE INDEX IX_Games_Name ON Games(Name);
CREATE INDEX IX_Games_Genre ON Games(Genre);
CREATE INDEX IX_Games_Platform ON Games(Platform);
CREATE INDEX IX_Games_ReleaseDate ON Games(ReleaseDate);

-- 遊戲熱度表格索引
CREATE INDEX IX_GameHeat_GameId ON GameHeat(GameId);
CREATE INDEX IX_GameHeat_HeatScore ON GameHeat(HeatScore);
CREATE INDEX IX_GameHeat_CalculatedAt ON GameHeat(CalculatedAt);

-- 論壇文章索引
CREATE INDEX IX_ForumPosts_CategoryId ON ForumPosts(CategoryId);
CREATE INDEX IX_ForumPosts_AuthorId ON ForumPosts(AuthorId);
CREATE INDEX IX_ForumPosts_GameId ON ForumPosts(GameId);
CREATE INDEX IX_ForumPosts_CreatedAt ON ForumPosts(CreatedAt);

-- 商品索引
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_GameId ON Products(GameId);
CREATE INDEX IX_Products_Price ON Products(Price);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);

-- 交易記錄索引
CREATE INDEX IX_Transactions_BuyerId ON Transactions(BuyerId);
CREATE INDEX IX_Transactions_SellerId ON Transactions(SellerId);
CREATE INDEX IX_Transactions_ProductId ON Transactions(ProductId);
CREATE INDEX IX_Transactions_TransactionDate ON Transactions(TransactionDate);
CREATE INDEX IX_Transactions_Status ON Transactions(Status);

-- 建立預設資料
-- 插入預設用戶角色
INSERT INTO UserRoles (UserId, RoleName, CreatedBy) 
SELECT Id, 'Admin', Id FROM Users WHERE Username = 'admin';

-- 插入預設論壇分類
INSERT INTO ForumCategories (Name, Description, DisplayOrder) VALUES
('一般討論', '遊戲相關的一般討論', 1),
('攻略心得', '遊戲攻略和心得分享', 2),
('問題求助', '遊戲問題和技術支援', 3),
('閒聊灌水', '輕鬆閒聊和灌水', 4);

-- 插入預設商品分類
INSERT INTO ProductCategories (Name, Description, DisplayOrder) VALUES
('遊戲軟體', '各種遊戲軟體', 1),
('遊戲周邊', '遊戲相關周邊商品', 2),
('數位商品', '遊戲內虛擬商品', 3),
('二手交易', '二手遊戲商品交易', 4);

-- 建立觸發器更新 UpdatedAt 欄位
CREATE TRIGGER TR_Users_UpdateTimestamp ON Users
AFTER UPDATE AS
BEGIN
    UPDATE Users 
    SET UpdatedAt = GETUTCDATE()
    FROM Users u
    INNER JOIN inserted i ON u.Id = i.Id;
END;
GO

-- 建立其他表格的更新觸發器
CREATE TRIGGER TR_Games_UpdateTimestamp ON Games
AFTER UPDATE AS
BEGIN
    UPDATE Games 
    SET UpdatedAt = GETUTCDATE()
    FROM Games g
    INNER JOIN inserted i ON g.Id = i.Id;
END;
GO

-- 建立預存程序
-- 獲取遊戲熱度排行榜
CREATE PROCEDURE GetGameHeatRanking
    @TopCount INT = 10,
    @Genre NVARCHAR(50) = NULL
AS
BEGIN
    SELECT TOP(@TopCount)
        g.Id,
        g.Name,
        g.Genre,
        g.CoverImage,
        gh.HeatScore,
        gh.PlayerCount,
        gh.ViewCount,
        gh.DiscussionCount,
        gh.MarketActivity
    FROM Games g
    INNER JOIN GameHeat gh ON g.Id = gh.GameId
    WHERE g.IsActive = 1
        AND (@Genre IS NULL OR g.Genre = @Genre)
    ORDER BY gh.HeatScore DESC, gh.PlayerCount DESC;
END;
GO

-- 獲取用戶統計資訊
CREATE PROCEDURE GetUserStatistics
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        u.Username,
        u.CreatedAt,
        u.LastLoginAt,
        (SELECT COUNT(*) FROM ForumPosts WHERE AuthorId = @UserId) AS PostCount,
        (SELECT COUNT(*) FROM Pets WHERE OwnerId = @UserId) AS PetCount,
        (SELECT COUNT(*) FROM Transactions WHERE BuyerId = @UserId OR SellerId = @UserId) AS TransactionCount
    FROM Users u
    WHERE u.Id = @UserId;
END;
GO

-- 建立資料庫權限
GRANT SELECT, INSERT, UPDATE, DELETE ON Users TO GameCoreUser;
GRANT SELECT, INSERT, UPDATE, DELETE ON Games TO GameCoreUser;
GRANT SELECT, INSERT, UPDATE, DELETE ON ForumPosts TO GameCoreUser;
GRANT SELECT, INSERT, UPDATE, DELETE ON Products TO GameCoreUser;
GRANT SELECT, INSERT, UPDATE, DELETE ON Pets TO GameCoreUser;
GRANT SELECT, INSERT, UPDATE, DELETE ON Transactions TO GameCoreUser;

GRANT ALL ON ALL TABLES TO GameCoreAdmin;
GRANT EXECUTE ON GetGameHeatRanking TO GameCoreUser;
GRANT EXECUTE ON GetUserStatistics TO GameCoreUser;

-- 設定資料庫擁有者
ALTER AUTHORIZATION ON DATABASE::GameCore TO sa;
GO

PRINT 'GameCore 資料庫初始化完成！';
PRINT '資料庫名稱: GameCore';
PRINT '預設表格: Users, Games, ForumPosts, Products, Pets, Transactions';
PRINT '預設角色: GameCoreUser, GameCoreAdmin';
PRINT '預設程序: GetGameHeatRanking, GetUserStatistics';
GO
