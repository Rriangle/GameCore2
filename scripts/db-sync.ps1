#!/usr/bin/env pwsh

<#
.SYNOPSIS
    GameCore 資料庫同步腳本
.DESCRIPTION
    同步本地和雲端資料庫結構與資料
.PARAMETER Environment
    目標環境 (local, dev, staging, prod)
.PARAMETER Action
    同步動作 (migrate, seed, backup, restore)
.PARAMETER Source
    來源環境 (用於 restore 動作)
.EXAMPLE
    .\db-sync.ps1 -Environment local -Action migrate
    .\db-sync.ps1 -Environment dev -Action backup
    .\db-sync.ps1 -Environment prod -Action restore -Source dev
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("local", "dev", "staging", "prod")]
    [string]$Environment,
    
    [Parameter(Mandatory = $true)]
    [ValidateSet("migrate", "seed", "backup", "restore", "compare")]
    [string]$Action,
    
    [string]$Source,
    
    [switch]$Force
)

# 設定錯誤處理
$ErrorActionPreference = "Stop"

# 顏色函數
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Write-Success { param([string]$Message) Write-ColorOutput "✅ $Message" "Green" }
function Write-Info { param([string]$Message) Write-ColorOutput "ℹ️  $Message" "Cyan" }
function Write-Warning { param([string]$Message) Write-ColorOutput "⚠️  $Message" "Yellow" }
function Write-Error { param([string]$Message) Write-ColorOutput "❌ $Message" "Red" }

# 取得環境變數
function Get-EnvironmentVariable {
    param([string]$Name, [string]$Environment)
    
    $envFile = "env.$Environment"
    if (Test-Path $envFile) {
        $content = Get-Content $envFile
        $line = $content | Where-Object { $_ -like "$Name=*" }
        if ($line) {
            return $line.Split("=", 2)[1].Trim('"')
        }
    }
    return $null
}

# 執行資料庫遷移
function Invoke-DatabaseMigration {
    param([string]$Environment)
    
    Write-Info "執行資料庫遷移到 $Environment 環境..."
    
    if ($Environment -eq "local") {
        # 本地遷移
        dotnet ef database update --project src/GameCore.Infrastructure --startup-project src/GameCore.Api
    }
    else {
        # 雲端遷移
        $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" $Environment
        if (-not $connectionString) {
            Write-Error "找不到 $Environment 環境的資料庫連線字串"
            return $false
        }
        
        dotnet ef database update --project src/GameCore.Infrastructure --startup-project src/GameCore.Api --connection "$connectionString"
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "資料庫遷移完成"
        return $true
    }
    else {
        Write-Error "資料庫遷移失敗"
        return $false
    }
}

# 執行資料庫種子資料
function Invoke-DatabaseSeeding {
    param([string]$Environment)
    
    Write-Info "執行資料庫種子資料到 $Environment 環境..."
    
    # 建立種子資料腳本
    $seedScript = @"
using GameCore.Infrastructure.Data;
using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GameCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<GameCoreDbContext>();

// 清除現有資料
context.Users.RemoveRange(context.Users);
context.UserWallets.RemoveRange(context.UserWallets);
context.SaveChanges();

// 建立測試使用者
var users = new List<User>
{
    new User { Username = "admin", Email = "admin@gamecore.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), CreatedAt = DateTime.UtcNow, IsActive = true },
    new User { Username = "testuser", Email = "test@gamecore.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123"), CreatedAt = DateTime.UtcNow, IsActive = true },
    new User { Username = "demo", Email = "demo@gamecore.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"), CreatedAt = DateTime.UtcNow, IsActive = true }
};

context.Users.AddRange(users);
context.SaveChanges();

// 建立使用者錢包
var wallets = users.Select(u => new UserWallet
{
    UserId = u.UserId,
    Balance = 1000.00m,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
}).ToList();

context.UserWallets.AddRange(wallets);
context.SaveChanges();

Console.WriteLine("種子資料建立完成");
"@

    $tempFile = "temp-seed.cs"
    $seedScript | Out-File -FilePath $tempFile -Encoding UTF8
    
    try {
        if ($Environment -eq "local") {
            dotnet run --project src/GameCore.Api --environment Development
        }
        else {
            $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" $Environment
            $env:ConnectionStrings__DefaultConnection = $connectionString
            dotnet run --project src/GameCore.Api --environment Production
        }
        
        Write-Success "種子資料建立完成"
    }
    finally {
        if (Test-Path $tempFile) {
            Remove-Item $tempFile
        }
    }
}

# 備份資料庫
function Backup-Database {
    param([string]$Environment)
    
    Write-Info "備份 $Environment 環境資料庫..."
    
    $backupDir = "backups"
    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir
    }
    
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $backupFile = "$backupDir/gamecore-$Environment-$timestamp.bak"
    
    if ($Environment -eq "local") {
        # 本地備份
        $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" "local"
        $server = ($connectionString -split ";") | Where-Object { $_ -like "Server=*" } | ForEach-Object { ($_ -split "=")[1] }
        $database = ($connectionString -split ";") | Where-Object { $_ -like "Database=*" } | ForEach-Object { ($_ -split "=")[1] }
        
        sqlcmd -S $server -d $database -Q "BACKUP DATABASE [$database] TO DISK = '$backupFile' WITH FORMAT, INIT, NAME = 'GameCore Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
    }
    else {
        # 雲端備份
        $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" $Environment
        $server = ($connectionString -split ";") | Where-Object { $_ -like "Server=*" } | ForEach-Object { ($_ -split "=")[1] }
        $database = ($connectionString -split ";") | Where-Object { $_ -like "Initial Catalog=*" } | ForEach-Object { ($_ -split "=")[1] }
        
        # 使用 Azure CLI 建立備份
        az sql db export --resource-group "gamecore-$Environment-rg" --server "gamecore-sql-$Environment" --name $database --storage-uri "https://gamecore${Environment}storage.blob.core.windows.net/backups/$backupFile"
    }
    
    Write-Success "資料庫備份完成: $backupFile"
}

# 還原資料庫
function Restore-Database {
    param([string]$Environment, [string]$Source)
    
    Write-Info "從 $Source 環境還原資料庫到 $Environment 環境..."
    
    if (-not $Source) {
        Write-Error "請指定來源環境"
        return $false
    }
    
    $backupDir = "backups"
    $latestBackup = Get-ChildItem -Path $backupDir -Filter "gamecore-$Source-*.bak" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    
    if (-not $latestBackup) {
        Write-Error "找不到 $Source 環境的備份檔案"
        return $false
    }
    
    Write-Info "使用備份檔案: $($latestBackup.Name)"
    
    if ($Environment -eq "local") {
        # 本地還原
        $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" "local"
        $server = ($connectionString -split ";") | Where-Object { $_ -like "Server=*" } | ForEach-Object { ($_ -split "=")[1] }
        $database = ($connectionString -split ";") | Where-Object { $_ -like "Database=*" } | ForEach-Object { ($_ -split "=")[1] }
        
        sqlcmd -S $server -d master -Q "RESTORE DATABASE [$database] FROM DISK = '$($latestBackup.FullName)' WITH REPLACE, STATS = 10"
    }
    else {
        # 雲端還原
        $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" $Environment
        $database = ($connectionString -split ";") | Where-Object { $_ -like "Initial Catalog=*" } | ForEach-Object { ($_ -split "=")[1] }
        
        # 使用 Azure CLI 還原備份
        az sql db import --resource-group "gamecore-$Environment-rg" --server "gamecore-sql-$Environment" --name $database --storage-uri "https://gamecore${Environment}storage.blob.core.windows.net/backups/$($latestBackup.Name)"
    }
    
    Write-Success "資料庫還原完成"
}

# 比較資料庫結構
function Compare-DatabaseStructure {
    param([string]$Environment1, [string]$Environment2)
    
    Write-Info "比較 $Environment1 和 $Environment2 環境的資料庫結構..."
    
    # 建立比較腳本
    $compareScript = @"
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder1 = WebApplication.CreateBuilder(args);
var builder2 = WebApplication.CreateBuilder(args);

// 設定兩個環境的連線
var connection1 = GetConnectionString("$Environment1");
var connection2 = GetConnectionString("$Environment2");

builder1.Services.AddDbContext<GameCoreDbContext>(options => options.UseSqlServer(connection1));
builder2.Services.AddDbContext<GameCoreDbContext>(options => options.UseSqlServer(connection2));

var app1 = builder1.Build();
var app2 = builder2.Build();

using var scope1 = app1.Services.CreateScope();
using var scope2 = app2.Services.CreateScope();

var context1 = scope1.ServiceProvider.GetRequiredService<GameCoreDbContext>();
var context2 = scope2.ServiceProvider.GetRequiredService<GameCoreDbContext>();

// 比較資料庫結構
var diff = context1.Database.GenerateCreateScript().CompareTo(context2.Database.GenerateCreateScript());
Console.WriteLine("資料庫結構差異: " + (diff == 0 ? "無差異" : "有差異"));

// 比較資料表數量
var tables1 = context1.Database.SqlQueryRaw<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'").ToList();
var tables2 = context2.Database.SqlQueryRaw<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'").ToList();

Console.WriteLine("$Environment1 環境資料表: " + string.Join(", ", tables1));
Console.WriteLine("$Environment2 環境資料表: " + string.Join(", ", tables2));
"@

    $tempFile = "temp-compare.cs"
    $compareScript | Out-File -FilePath $tempFile -Encoding UTF8
    
    try {
        dotnet run --project src/GameCore.Api
    }
    finally {
        if (Test-Path $tempFile) {
            Remove-Item $tempFile
        }
    }
}

# 主程式
function Main {
    Write-ColorOutput "🗄️ GameCore 資料庫同步腳本" "Magenta"
    Write-Info "環境: $Environment"
    Write-Info "動作: $Action"
    
    if ($Source) {
        Write-Info "來源: $Source"
    }
    
    # 根據動作執行相應功能
    switch ($Action) {
        "migrate" {
            Invoke-DatabaseMigration $Environment
        }
        "seed" {
            if ($Force -or (Read-Host "確定要清除現有資料並建立種子資料嗎？(y/N)") -eq "y") {
                Invoke-DatabaseSeeding $Environment
            }
        }
        "backup" {
            Backup-Database $Environment
        }
        "restore" {
            if ($Force -or (Read-Host "確定要還原資料庫嗎？這會覆蓋現有資料！(y/N)") -eq "y") {
                Restore-Database $Environment $Source
            }
        }
        "compare" {
            if (-not $Source) {
                Write-Error "比較動作需要指定來源環境"
                exit 1
            }
            Compare-DatabaseStructure $Environment $Source
        }
    }
    
    Write-Success "資料庫同步腳本執行完成"
}

# 執行主程式
Main
