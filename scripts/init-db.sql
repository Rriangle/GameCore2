-- GameCore 資料庫初始化腳本
-- 此腳本會在 Docker 容器啟動時自動執行

USE master;
GO

-- 建立資料庫（如果不存在）
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GameCore')
BEGIN
    CREATE DATABASE GameCore;
END
GO

USE GameCore;
GO

-- 建立使用者資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [LastLoginAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsEmailVerified] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
    );
END
GO

-- 建立使用者錢包資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserWallets]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserWallets] (
        [WalletId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Balance] DECIMAL(18,2) NOT NULL DEFAULT 100.00,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_UserWallets] PRIMARY KEY CLUSTERED ([WalletId] ASC),
        CONSTRAINT [FK_UserWallets_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );
END
GO

-- 建立唯一索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users] ([Username] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email] ASC);
END
GO

-- 建立預設管理員使用者（如果不存在）
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [CreatedAt], [IsActive], [IsEmailVerified])
    VALUES ('admin', 'admin@gamecore.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', GETUTCDATE(), 1, 1);
    
    DECLARE @AdminUserId INT = SCOPE_IDENTITY();
    
    INSERT INTO [dbo].[UserWallets] ([UserId], [Balance], [CreatedAt], [UpdatedAt])
    VALUES (@AdminUserId, 1000.00, GETUTCDATE(), GETUTCDATE());
END
GO

-- 建立測試使用者（如果不存在）
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Username] = 'testuser')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [CreatedAt], [IsActive], [IsEmailVerified])
    VALUES ('testuser', 'test@gamecore.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', GETUTCDATE(), 1, 1);
    
    DECLARE @TestUserId INT = SCOPE_IDENTITY();
    
    INSERT INTO [dbo].[UserWallets] ([UserId], [Balance], [CreatedAt], [UpdatedAt])
    VALUES (@TestUserId, 500.00, GETUTCDATE(), GETUTCDATE());
END
GO

-- 建立寵物資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pets]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Pets] (
        [PetId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Name] NVARCHAR(50) NOT NULL,
        [Type] NVARCHAR(20) NOT NULL DEFAULT 'Slime',
        [Level] INT NOT NULL DEFAULT 1,
        [Experience] INT NOT NULL DEFAULT 0,
        [Health] INT NOT NULL DEFAULT 100,
        [Hunger] INT NOT NULL DEFAULT 100,
        [Happiness] INT NOT NULL DEFAULT 100,
        [CreatedAt] DATETIME2 NOT NULL,
        [LastFedAt] DATETIME2 NULL,
        [LastPlayedAt] DATETIME2 NULL,
        CONSTRAINT [PK_Pets] PRIMARY KEY CLUSTERED ([PetId] ASC),
        CONSTRAINT [FK_Pets_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );
END
GO

-- 建立遊戲資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Games]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Games] (
        [GameId] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Genre] NVARCHAR(50) NULL,
        [ReleaseDate] DATE NULL,
        [Developer] NVARCHAR(100) NULL,
        [Publisher] NVARCHAR(100) NULL,
        [Platform] NVARCHAR(50) NULL,
        [Rating] DECIMAL(3,1) NULL,
        [PopularityScore] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED ([GameId] ASC)
    );
END
GO

-- 建立論壇資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Forums]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Forums] (
        [ForumId] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Forums] PRIMARY KEY CLUSTERED ([ForumId] ASC)
    );
END
GO

-- 建立文章資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Posts] (
        [PostId] INT IDENTITY(1,1) NOT NULL,
        [ForumId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Content] NVARCHAR(MAX) NOT NULL,
        [ViewCount] INT NOT NULL DEFAULT 0,
        [LikeCount] INT NOT NULL DEFAULT 0,
        [IsPinned] BIT NOT NULL DEFAULT 0,
        [IsLocked] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED ([PostId] ASC),
        CONSTRAINT [FK_Posts_Forums] FOREIGN KEY ([ForumId]) REFERENCES [dbo].[Forums] ([ForumId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Posts_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
    );
END
GO

-- 建立商城資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Shops]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Shops] (
        [ShopId] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Type] NVARCHAR(20) NOT NULL DEFAULT 'Official', -- Official, User
        [OwnerId] INT NULL, -- NULL for official shops
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Shops] PRIMARY KEY CLUSTERED ([ShopId] ASC),
        CONSTRAINT [FK_Shops_Users] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE SET NULL
    );
END
GO

-- 建立商品資料表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products] (
        [ProductId] INT IDENTITY(1,1) NOT NULL,
        [ShopId] INT NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Price] DECIMAL(18,2) NOT NULL,
        [Stock] INT NOT NULL DEFAULT 0,
        [Category] NVARCHAR(50) NULL,
        [ImageUrl] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductId] ASC),
        CONSTRAINT [FK_Products_Shops] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[Shops] ([ShopId]) ON DELETE CASCADE
    );
END
GO

-- 插入預設資料
-- 插入預設論壇
IF NOT EXISTS (SELECT * FROM [dbo].[Forums] WHERE [Name] = '一般討論')
BEGIN
    INSERT INTO [dbo].[Forums] ([Name], [Description], [Category], [CreatedAt], [UpdatedAt])
    VALUES ('一般討論', '遊戲相關的一般討論區', 'General', GETUTCDATE(), GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Forums] WHERE [Name] = '攻略分享')
BEGIN
    INSERT INTO [dbo].[Forums] ([Name], [Description], [Category], [CreatedAt], [UpdatedAt])
    VALUES ('攻略分享', '分享遊戲攻略和心得', 'Strategy', GETUTCDATE(), GETUTCDATE());
END
GO

-- 插入預設商城
IF NOT EXISTS (SELECT * FROM [dbo].[Shops] WHERE [Name] = '官方商城')
BEGIN
    INSERT INTO [dbo].[Shops] ([Name], [Description], [Type], [CreatedAt], [UpdatedAt])
    VALUES ('官方商城', 'GameCore 官方商品', 'Official', GETUTCDATE(), GETUTCDATE());
END
GO

-- 插入預設遊戲
IF NOT EXISTS (SELECT * FROM [dbo].[Games] WHERE [Name] = 'GameCore 冒險')
BEGIN
    INSERT INTO [dbo].[Games] ([Name], [Description], [Genre], [ReleaseDate], [Developer], [Publisher], [Platform], [Rating], [PopularityScore], [CreatedAt], [UpdatedAt])
    VALUES ('GameCore 冒險', 'GameCore 平台內建冒險遊戲', 'Adventure', GETDATE(), 'GameCore Team', 'GameCore', 'Web', 4.5, 100, GETUTCDATE(), GETUTCDATE());
END
GO

PRINT 'GameCore 資料庫初始化完成！';
GO
