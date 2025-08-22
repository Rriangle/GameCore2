#!/usr/bin/env pwsh

# GameCore 測試執行腳本
# 執行所有測試並生成報告

param(
    [switch]$BackendOnly,
    [switch]$FrontendOnly,
    [switch]$Coverage,
    [switch]$Verbose
)

Write-Host "🚀 GameCore 測試執行器" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

$startTime = Get-Date
$testResults = @{
    Backend  = $null
    Frontend = $null
    Success  = $true
}

# 檢查必要工具
function Test-Prerequisites {
    Write-Host "📋 檢查必要工具..." -ForegroundColor Yellow
    
    # 檢查 .NET
    try {
        $dotnetVersion = dotnet --version
        Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ .NET SDK 未安裝" -ForegroundColor Red
        exit 1
    }
    
    # 檢查 Node.js
    try {
        $nodeVersion = node --version
        Write-Host "✅ Node.js: $nodeVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Node.js 未安裝" -ForegroundColor Red
        exit 1
    }
    
    # 檢查 pnpm
    try {
        $pnpmVersion = pnpm --version
        Write-Host "✅ pnpm: $pnpmVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ pnpm 未安裝" -ForegroundColor Red
        exit 1
    }
}

# 執行後端測試
function Test-Backend {
    Write-Host "`n🔧 執行後端測試..." -ForegroundColor Yellow
    
    try {
        # 切換到專案根目錄
        Set-Location $PSScriptRoot/..
        
        # 清理並還原套件
        if ($Verbose) {
            Write-Host "清理專案..." -ForegroundColor Gray
            dotnet clean --verbosity quiet
        }
        
        Write-Host "還原 NuGet 套件..." -ForegroundColor Gray
        dotnet restore --verbosity quiet
        
        # 執行測試
        $testArgs = @("test", "--verbosity", "quiet", "--no-restore")
        
        if ($Coverage) {
            $testArgs += "--collect:""XPlat Code Coverage"""
        }
        
        Write-Host "執行單元測試和整合測試..." -ForegroundColor Gray
        $backendResult = dotnet $testArgs
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ 後端測試通過" -ForegroundColor Green
            $testResults.Backend = $true
        }
        else {
            Write-Host "❌ 後端測試失敗" -ForegroundColor Red
            $testResults.Backend = $false
            $testResults.Success = $false
        }
        
        # 顯示測試摘要
        $testSummary = $backendResult | Select-String -Pattern "Tests run:|Passed:|Failed:|Skipped:"
        if ($testSummary) {
            Write-Host "`n📊 後端測試摘要:" -ForegroundColor Cyan
            $testSummary | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
        }
    }
    catch {
        Write-Host "❌ 後端測試執行錯誤: $($_.Exception.Message)" -ForegroundColor Red
        $testResults.Backend = $false
        $testResults.Success = $false
    }
}

# 執行前端測試
function Test-Frontend {
    Write-Host "`n🎨 執行前端測試..." -ForegroundColor Yellow
    
    try {
        # 切換到前端目錄
        Set-Location $PSScriptRoot/../frontend
        
        # 檢查 node_modules
        if (-not (Test-Path "node_modules")) {
            Write-Host "安裝前端依賴..." -ForegroundColor Gray
            pnpm install
        }
        
        # 執行測試
        $testArgs = @("test:run")
        
        if ($Coverage) {
            $testArgs = @("test:coverage")
        }
        
        Write-Host "執行前端組件測試..." -ForegroundColor Gray
        $frontendResult = pnpm $testArgs
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ 前端測試通過" -ForegroundColor Green
            $testResults.Frontend = $true
        }
        else {
            Write-Host "❌ 前端測試失敗" -ForegroundColor Red
            $testResults.Frontend = $false
            $testResults.Success = $false
        }
        
        # 執行型別檢查
        Write-Host "執行型別檢查..." -ForegroundColor Gray
        $typeCheckResult = pnpm type-check
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ 型別檢查通過" -ForegroundColor Green
        }
        else {
            Write-Host "❌ 型別檢查失敗" -ForegroundColor Red
            $testResults.Success = $false
        }
        
        # 執行 Lint
        Write-Host "執行程式碼檢查..." -ForegroundColor Gray
        $lintResult = pnpm lint
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ 程式碼檢查通過" -ForegroundColor Green
        }
        else {
            Write-Host "❌ 程式碼檢查失敗" -ForegroundColor Red
            $testResults.Success = $false
        }
    }
    catch {
        Write-Host "❌ 前端測試執行錯誤: $($_.Exception.Message)" -ForegroundColor Red
        $testResults.Frontend = $false
        $testResults.Success = $false
    }
}

# 生成測試報告
function Show-TestReport {
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-Host "`n📋 測試報告" -ForegroundColor Cyan
    Write-Host "================================" -ForegroundColor Cyan
    
    Write-Host "執行時間: $($duration.TotalSeconds.ToString('F2')) 秒" -ForegroundColor Gray
    
    if ($testResults.Backend -ne $null) {
        $backendStatus = if ($testResults.Backend) { "✅ 通過" } else { "❌ 失敗" }
        Write-Host "後端測試: $backendStatus" -ForegroundColor $(if ($testResults.Backend) { "Green" } else { "Red" })
    }
    
    if ($testResults.Frontend -ne $null) {
        $frontendStatus = if ($testResults.Frontend) { "✅ 通過" } else { "❌ 失敗" }
        Write-Host "前端測試: $frontendStatus" -ForegroundColor $(if ($testResults.Frontend) { "Green" } else { "Red" })
    }
    
    Write-Host "`n整體結果: $(if ($testResults.Success) { "✅ 所有測試通過" } else { "❌ 部分測試失敗" })" -ForegroundColor $(if ($testResults.Success) { "Green" } else { "Red" })
    
    if ($Coverage) {
        Write-Host "`n📊 覆蓋率報告已生成:" -ForegroundColor Cyan
        Write-Host "  後端: tests/GameCore.Tests/TestResults/" -ForegroundColor Gray
        Write-Host "  前端: frontend/coverage/" -ForegroundColor Gray
    }
}

# 主執行流程
try {
    Test-Prerequisites
    
    if ($BackendOnly) {
        Test-Backend
    }
    elseif ($FrontendOnly) {
        Test-Frontend
    }
    else {
        Test-Backend
        Test-Frontend
    }
    
    Show-TestReport
    
    # 返回適當的退出碼
    if ($testResults.Success) {
        exit 0
    }
    else {
        exit 1
    }
}
catch {
    Write-Host "❌ 測試執行器發生錯誤: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    # 回到原始目錄
    Set-Location $PSScriptRoot/..
}
