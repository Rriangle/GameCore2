#!/usr/bin/env pwsh

<#
.SYNOPSIS
    GameCore 本地開發啟動腳本（不依賴 Docker）
.DESCRIPTION
    啟動本地開發環境，包括後端 API 和前端開發伺服器
#>

# 顏色函數
function Write-Success { Write-Host "✅ $args" -ForegroundColor Green }
function Write-Info { Write-Host "ℹ️  $args" -ForegroundColor Cyan }
function Write-Warning { Write-Host "⚠️  $args" -ForegroundColor Yellow }
function Write-Error { Write-Host "❌ $args" -ForegroundColor Red }

# 檢查必要工具
function Test-Prerequisites {
    Write-Info "檢查必要工具..."
    
    $tools = @{
        "dotnet" = "dotnet --version"
        "node"   = "node --version"
        "pnpm"   = "pnpm --version"
    }
    
    $missingTools = @()
    
    foreach ($tool in $tools.GetEnumerator()) {
        try {
            $version = Invoke-Expression $tool.Value 2>$null
            Write-Success "$($tool.Key): $version"
        }
        catch {
            Write-Warning "$($tool.Key) 未安裝或不在 PATH 中"
            $missingTools += $tool.Key
        }
    }
    
    if ($missingTools.Count -gt 0) {
        Write-Error "缺少必要工具: $($missingTools -join ', ')"
        Write-Info "請安裝以下工具："
        Write-Info "- .NET 8.0 SDK: https://dotnet.microsoft.com/download"
        Write-Info "- Node.js 18+: https://nodejs.org/"
        Write-Info "- pnpm: npm install -g pnpm"
        return $false
    }
    
    return $true
}

# 建置後端
function Build-Backend {
    Write-Info "建置後端..."
    
    # 還原套件
    Write-Info "還原 .NET 套件..."
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error ".NET 套件還原失敗"
        return $false
    }
    
    # 建置專案
    Write-Info "建置專案..."
    dotnet build --configuration Debug --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "專案建置失敗"
        return $false
    }
    
    Write-Success "後端建置完成"
    return $true
}

# 建置前端
function Build-Frontend {
    Write-Info "建置前端..."
    
    Push-Location frontend
    
    # 安裝依賴
    Write-Info "安裝前端依賴..."
    pnpm install --frozen-lockfile
    if ($LASTEXITCODE -ne 0) {
        Write-Error "前端依賴安裝失敗"
        Pop-Location
        return $false
    }
    
    Pop-Location
    
    Write-Success "前端建置完成"
    return $true
}

# 啟動後端 API
function Start-Backend {
    Write-Info "啟動後端 API..."
    
    $backendJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD
        dotnet run --project src/GameCore.Api --environment Development
    }
    
    Write-Success "後端 API 已啟動（Job ID: $($backendJob.Id)）"
    return $backendJob
}

# 啟動前端開發伺服器
function Start-Frontend {
    Write-Info "啟動前端開發伺服器..."
    
    $frontendJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD
        Push-Location frontend
        pnpm dev
        Pop-Location
    }
    
    Write-Success "前端開發伺服器已啟動（Job ID: $($frontendJob.Id)）"
    return $frontendJob
}

# 等待服務啟動
function Wait-ForServices {
    Write-Info "等待服務啟動..."
    
    $maxAttempts = 30
    $attempt = 0
    
    do {
        $attempt++
        Write-Info "檢查服務狀態... (嘗試 $attempt/$maxAttempts)"
        
        # 檢查後端 API
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                Write-Success "後端 API: 正常"
            }
        }
        catch {
            Write-Warning "後端 API: 尚未就緒"
        }
        
        # 檢查前端
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:3000" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                Write-Success "前端: 正常"
                break
            }
        }
        catch {
            Write-Warning "前端: 尚未就緒"
        }
        
        if ($attempt -lt $maxAttempts) {
            Start-Sleep -Seconds 2
        }
    } while ($attempt -lt $maxAttempts)
    
    if ($attempt -ge $maxAttempts) {
        Write-Warning "服務啟動超時，請手動檢查"
    }
}

# 顯示服務資訊
function Show-ServiceInfo {
    Write-Host "`n🎮 GameCore 本地開發環境已啟動" -ForegroundColor Magenta
    Write-Info "後端 API: http://localhost:5000"
    Write-Info "前端應用: http://localhost:3000"
    Write-Info "API 文件: http://localhost:5000/swagger"
    Write-Info "健康檢查: http://localhost:5000/health"
    Write-Host "`n按 Ctrl+C 停止所有服務" -ForegroundColor Yellow
}

# 主程式
function Main {
    Write-Host "🚀 GameCore 本地開發啟動腳本" -ForegroundColor Magenta
    
    # 檢查必要工具
    if (-not (Test-Prerequisites)) {
        exit 1
    }
    
    # 建置專案
    if (-not (Build-Backend)) {
        exit 1
    }
    
    if (-not (Build-Frontend)) {
        exit 1
    }
    
    # 啟動服務
    $backendJob = Start-Backend
    $frontendJob = Start-Frontend
    
    # 等待服務啟動
    Wait-ForServices
    
    # 顯示服務資訊
    Show-ServiceInfo
    
    # 等待使用者中斷
    try {
        while ($true) {
            Start-Sleep -Seconds 1
            
            # 檢查作業狀態
            if ($backendJob.State -eq "Failed") {
                Write-Error "後端服務失敗"
                break
            }
            
            if ($frontendJob.State -eq "Failed") {
                Write-Error "前端服務失敗"
                break
            }
        }
    }
    catch {
        Write-Info "正在停止服務..."
    }
    finally {
        # 停止作業
        if ($backendJob) {
            Stop-Job $backendJob
            Remove-Job $backendJob
        }
        
        if ($frontendJob) {
            Stop-Job $frontendJob
            Remove-Job $frontendJob
        }
        
        Write-Success "所有服務已停止"
    }
}

# 執行主程式
Main
