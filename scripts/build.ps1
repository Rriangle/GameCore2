# GameCore 建置腳本
# 執行環境：Windows PowerShell 5.1+

param(
    [switch]$Clean,
    [switch]$Restore,
    [switch]$Build,
    [switch]$Test,
    [switch]$Frontend,
    [switch]$All
)

Write-Host "🎮 GameCore 建置腳本開始執行..." -ForegroundColor Green

# 檢查必要工具
function Test-Prerequisites {
    Write-Host "檢查必要工具..." -ForegroundColor Yellow
    
    # 檢查 .NET 8.0
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -notlike "8.*") {
        Write-Error ".NET 8.0 未安裝或版本不正確"
        exit 1
    }
    Write-Host "✅ .NET 版本: $dotnetVersion" -ForegroundColor Green
    
    # 檢查 Node.js
    $nodeVersion = node --version
    if ($nodeVersion -notlike "v18.*" -and $nodeVersion -notlike "v20.*") {
        Write-Error "Node.js 18+ 未安裝或版本不正確"
        exit 1
    }
    Write-Host "✅ Node.js 版本: $nodeVersion" -ForegroundColor Green
    
    # 檢查 pnpm
    $pnpmVersion = pnpm --version
    if (-not $pnpmVersion) {
        Write-Host "⚠️ pnpm 未安裝，正在安裝..." -ForegroundColor Yellow
        npm install -g pnpm
    }
    Write-Host "✅ pnpm 版本: $pnpmVersion" -ForegroundColor Green
}

# 清理建置
function Invoke-Clean {
    Write-Host "🧹 清理建置檔案..." -ForegroundColor Yellow
    dotnet clean
    Remove-Item -Path "bin", "obj" -Recurse -Force -ErrorAction SilentlyContinue
    if (Test-Path "frontend/node_modules") {
        Remove-Item -Path "frontend/node_modules" -Recurse -Force
    }
    Write-Host "✅ 清理完成" -ForegroundColor Green
}

# 還原套件
function Invoke-Restore {
    Write-Host "📦 還原 .NET 套件..." -ForegroundColor Yellow
    dotnet restore
    Write-Host "✅ .NET 套件還原完成" -ForegroundColor Green
    
    if ($Frontend -or $All) {
        Write-Host "📦 還原前端套件..." -ForegroundColor Yellow
        Set-Location frontend
        pnpm install
        Set-Location ..
        Write-Host "✅ 前端套件還原完成" -ForegroundColor Green
    }
}

# 建置專案
function Invoke-Build {
    Write-Host "🔨 建置 .NET 專案..." -ForegroundColor Yellow
    dotnet build --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "建置失敗"
        exit 1
    }
    Write-Host "✅ .NET 專案建置完成" -ForegroundColor Green
    
    if ($Frontend -or $All) {
        Write-Host "🔨 建置前端專案..." -ForegroundColor Yellow
        Set-Location frontend
        pnpm build
        Set-Location ..
        Write-Host "✅ 前端專案建置完成" -ForegroundColor Green
    }
}

# 執行測試
function Invoke-Test {
    Write-Host "🧪 執行測試..." -ForegroundColor Yellow
    dotnet test --configuration Release --no-build --verbosity normal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "測試失敗"
        exit 1
    }
    Write-Host "✅ 測試完成" -ForegroundColor Green
}

# 主執行流程
try {
    Test-Prerequisites
    
    if ($Clean) { Invoke-Clean }
    if ($Restore -or $All) { Invoke-Restore }
    if ($Build -or $All) { Invoke-Build }
    if ($Test -or $All) { Invoke-Test }
    
    if (-not ($Clean -or $Restore -or $Build -or $Test -or $All)) {
        Write-Host "使用方式:" -ForegroundColor Cyan
        Write-Host "  .\build.ps1 -Clean     # 清理建置檔案" -ForegroundColor White
        Write-Host "  .\build.ps1 -Restore   # 還原套件" -ForegroundColor White
        Write-Host "  .\build.ps1 -Build     # 建置專案" -ForegroundColor White
        Write-Host "  .\build.ps1 -Test      # 執行測試" -ForegroundColor White
        Write-Host "  .\build.ps1 -Frontend  # 包含前端建置" -ForegroundColor White
        Write-Host "  .\build.ps1 -All       # 完整建置流程" -ForegroundColor White
    }
    
    Write-Host "🎉 建置腳本執行完成！" -ForegroundColor Green
}
catch {
    Write-Error "建置過程中發生錯誤: $($_.Exception.Message)"
    exit 1
}
