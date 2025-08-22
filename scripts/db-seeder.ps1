# =============================================================================
# GameCore 資料庫 Seeder 腳本
# =============================================================================
# 此腳本用於建立假資料，適用於開發和測試環境
# 使用方式: .\scripts\db-seeder.ps1 -Environment local -Action seed

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("local", "dev", "staging", "prod")]
    [string]$Environment,
    
    [Parameter(Mandatory=$true)]
    [ValidateSet("seed", "reset", "check", "backup")]
    [string]$Action,
    
    [string]$ConnectionString = "",
    [switch]$Force,
    [switch]$Verbose
)

# 設定錯誤處理
$ErrorActionPreference = "Stop"

# 顏色設定
$Colors = @{
    Info = "Cyan"
    Success = "Green"
    Warning = "Yellow"
    Error = "Red"
}

# 日誌函數
function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "Info"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $color = $Colors[$Level]
    Write-Host "[$timestamp] $Message" -ForegroundColor $color
}

# 檢查必要工具
function Test-Prerequisites {
    Write-Log "檢查必要工具..." "Info"
    
    # 檢查 SQL Server 連線
    try {
        $sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
        if (-not $sqlcmd) {
            Write-Log "警告: sqlcmd 未找到，請安裝 SQL Server Command Line Utilities" "Warning"
        }
    }
    catch {
        Write-Log "警告: 無法找到 sqlcmd" "Warning"
    }
    
    # 檢查 Docker
    try {
        $docker = Get-Command docker -ErrorAction SilentlyContinue
        if (-not $docker) {
            Write-Log "錯誤: Docker 未安裝或未在 PATH 中" "Error"
            exit 1
        }
    }
    catch {
        Write-Log "錯誤: Docker 未安裝或未在 PATH 中" "Error"
        exit 1
    }
}

# 取得連線字串
function Get-ConnectionString {
    param([string]$Environment)
    
    switch ($Environment) {
        "local" {
            if ($ConnectionString) {
                return $ConnectionString
            }
            return "Server=(localdb)\mssqllocaldb;Database=GameCore;Trusted_Connection=true;MultipleActiveResultSets=true"
        }
        "dev" {
            if ($ConnectionString) {
                return $ConnectionString
            }
            return "Server=localhost,1433;Database=GameCore;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
        }
        default {
            if (-not $ConnectionString) {
                Write-Log "錯誤: 生產環境需要明確指定連線字串" "Error"
                exit 1
            }
            return $ConnectionString
        }
    }
}

# 檢查資料庫連線
function Test-DatabaseConnection {
    param([string]$ConnectionString)
    
    Write-Log "測試資料庫連線..." "Info"
    
    try {
        # 使用 sqlcmd 測試連線
        $testQuery = "SELECT 1 as TestResult"
        $result = sqlcmd -S $ConnectionString -Q $testQuery -h -1 -W -s "," 2>$null
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "資料庫連線成功" "Success"
            return $true
        }
        else {
            Write-Log "資料庫連線失敗" "Error"
            return $false
        }
    }
    catch {
        Write-Log "資料庫連線測試失敗: $($_.Exception.Message)" "Error"
        return $false
    }
}

# 檢查資料庫是否存在
function Test-DatabaseExists {
    param([string]$ConnectionString)
    
    Write-Log "檢查資料庫是否存在..." "Info"
    
    try {
        $query = "SELECT COUNT(*) FROM sys.databases WHERE name = 'GameCore'"
        $result = sqlcmd -S $ConnectionString -Q $query -h -1 -W -s "," 2>$null
        
        if ($LASTEXITCODE -eq 0 -and $result -match "1") {
            Write-Log "資料庫 GameCore 存在" "Success"
            return $true
        }
        else {
            Write-Log "資料庫 GameCore 不存在" "Warning"
            return $false
        }
    }
    catch {
        Write-Log "檢查資料庫失敗: $($_.Exception.Message)" "Error"
        return $false
    }
}

# 執行 SQL 腳本
function Invoke-SqlScript {
    param(
        [string]$ConnectionString,
        [string]$ScriptPath,
        [string]$Description
    )
    
    Write-Log "執行 $Description..." "Info"
    
    if (-not (Test-Path $ScriptPath)) {
        Write-Log "錯誤: 腳本檔案不存在: $ScriptPath" "Error"
        return $false
    }
    
    try {
        $result = sqlcmd -S $ConnectionString -i $ScriptPath -h -1 -W -s "," 2>$null
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "$Description 執行成功" "Success"
            return $true
        }
        else {
            Write-Log "$Description 執行失敗" "Error"
            return $false
        }
    }
    catch {
        Write-Log "$Description 執行失敗: $($_.Exception.Message)" "Error"
        return $false
    }
}

# 備份資料庫
function Backup-Database {
    param([string]$ConnectionString)
    
    Write-Log "備份資料庫..." "Info"
    
    $backupPath = ".\backups\GameCore_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"
    $backupDir = Split-Path $backupPath -Parent
    
    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    }
    
    try {
        $query = "BACKUP DATABASE GameCore TO DISK = '$backupPath' WITH FORMAT, INIT, NAME = 'GameCore-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
        $result = sqlcmd -S $ConnectionString -Q $query -h -1 -W -s "," 2>$null
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "資料庫備份成功: $backupPath" "Success"
            return $true
        }
        else {
            Write-Log "資料庫備份失敗" "Error"
            return $false
        }
    }
    catch {
        Write-Log "資料庫備份失敗: $($_.Exception.Message)" "Error"
        return $false
    }
}

# 重置資料庫
function Reset-Database {
    param([string]$ConnectionString)
    
    Write-Log "重置資料庫..." "Info"
    
    if (-not $Force) {
        $confirmation = Read-Host "確定要重置資料庫嗎？這將刪除所有資料！(y/N)"
        if ($confirmation -ne "y" -and $confirmation -ne "Y") {
            Write-Log "操作已取消" "Warning"
            return $false
        }
    }
    
    try {
        # 關閉所有連線
        $closeConnections = "ALTER DATABASE GameCore SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
        sqlcmd -S $ConnectionString -Q $closeConnections -h -1 -W -s "," 2>$null
        
        # 刪除資料庫
        $dropDatabase = "DROP DATABASE GameCore"
        sqlcmd -S $ConnectionString -Q $dropDatabase -h -1 -W -s "," 2>$null
        
        Write-Log "資料庫重置成功" "Success"
        return $true
    }
    catch {
        Write-Log "資料庫重置失敗: $($_.Exception.Message)" "Error"
        return $false
    }
}

# 建立假資料
function Seed-Database {
    param([string]$ConnectionString)
    
    Write-Log "開始建立假資料..." "Info"
    
    # 檢查資料庫是否存在
    if (-not (Test-DatabaseExists $ConnectionString)) {
        Write-Log "錯誤: 資料庫不存在，請先執行初始化" "Error"
        return $false
    }
    
    # 執行假資料腳本
    $seedScript = ".\scripts\seed-data.sql"
    if (-not (Invoke-SqlScript $ConnectionString $seedScript "假資料建立")) {
        return $false
    }
    
    Write-Log "假資料建立完成" "Success"
    return $true
}

# 顯示統計資訊
function Show-Statistics {
    param([string]$ConnectionString)
    
    Write-Log "顯示資料庫統計資訊..." "Info"
    
    $queries = @{
        "用戶數量" = "SELECT COUNT(*) FROM Users"
        "寵物數量" = "SELECT COUNT(*) FROM Pets"
        "遊戲數量" = "SELECT COUNT(*) FROM Games"
        "文章數量" = "SELECT COUNT(*) FROM Posts"
        "商城數量" = "SELECT COUNT(*) FROM Shops"
        "商品數量" = "SELECT COUNT(*) FROM Products"
    }
    
    foreach ($stat in $queries.GetEnumerator()) {
        try {
            $result = sqlcmd -S $ConnectionString -Q $stat.Value -h -1 -W -s "," 2>$null
            if ($LASTEXITCODE -eq 0) {
                $count = ($result -split ",")[0].Trim()
                Write-Log "$($stat.Key): $count" "Info"
            }
        }
        catch {
            Write-Log "無法取得 $($stat.Key) 統計" "Warning"
        }
    }
}

# 主程式
function Main {
    Write-Log "GameCore 資料庫 Seeder 開始執行" "Info"
    Write-Log "環境: $Environment, 動作: $Action" "Info"
    
    # 檢查必要工具
    Test-Prerequisites
    
    # 取得連線字串
    $connString = Get-ConnectionString $Environment
    if ($Verbose) {
        Write-Log "連線字串: $connString" "Info"
    }
    
    # 根據動作執行相應操作
    switch ($Action) {
        "check" {
            if (Test-DatabaseConnection $connString) {
                Show-Statistics $connString
            }
        }
        "backup" {
            if (Test-DatabaseConnection $connString) {
                Backup-Database $connString
            }
        }
        "reset" {
            if (Test-DatabaseConnection $connString) {
                Reset-Database $connString
            }
        }
        "seed" {
            if (Test-DatabaseConnection $connString) {
                Seed-Database $connString
                Show-Statistics $connString
            }
        }
    }
    
    Write-Log "GameCore 資料庫 Seeder 執行完成" "Success"
}

# 執行主程式
try {
    Main
}
catch {
    Write-Log "執行失敗: $($_.Exception.Message)" "Error"
    exit 1
}