# GameCore 完整設定腳本
# 執行環境：Windows PowerShell 5.1+

Write-Host "🎮 GameCore 完整設定開始..." -ForegroundColor Green

# 1. 檢查必要工具
Write-Host "`n📋 檢查必要工具..." -ForegroundColor Yellow

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

# 2. 還原 .NET 套件
Write-Host "`n📦 還原 .NET 套件..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error ".NET 套件還原失敗"
    exit 1
}
Write-Host "✅ .NET 套件還原完成" -ForegroundColor Green

# 3. 安裝前端套件
Write-Host "`n📦 安裝前端套件..." -ForegroundColor Yellow
Set-Location frontend
pnpm install
if ($LASTEXITCODE -ne 0) {
    Write-Error "前端套件安裝失敗"
    exit 1
}
Set-Location ..
Write-Host "✅ 前端套件安裝完成" -ForegroundColor Green

# 4. 建置專案
Write-Host "`n🔨 建置專案..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "專案建置失敗"
    exit 1
}
Write-Host "✅ 專案建置完成" -ForegroundColor Green

# 5. 執行測試
Write-Host "`n🧪 執行測試..." -ForegroundColor Yellow
dotnet test --configuration Release --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Error "測試失敗"
    exit 1
}
Write-Host "✅ 測試完成" -ForegroundColor Green

# 6. 建立環境變數檔案
Write-Host "`n⚙️ 設定環境變數..." -ForegroundColor Yellow
if (-not (Test-Path ".env")) {
    Copy-Item "env.sample" ".env"
    Write-Host "✅ 已建立 .env 檔案，請檢查並修改設定" -ForegroundColor Green
}
else {
    Write-Host "ℹ️ .env 檔案已存在" -ForegroundColor Cyan
}

Write-Host "`n🎉 GameCore 設定完成！" -ForegroundColor Green
Write-Host "`n📋 下一步：" -ForegroundColor Cyan
Write-Host "1. 檢查 .env 檔案設定" -ForegroundColor White
Write-Host "2. 執行 .\scripts\dev.ps1 啟動開發環境" -ForegroundColor White
Write-Host "3. 訪問 http://localhost:3000 查看前端" -ForegroundColor White
Write-Host "4. 訪問 http://localhost:5000/api-docs 查看 API 文件" -ForegroundColor White
