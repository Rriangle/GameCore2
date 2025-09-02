# GameCore 開發環境啟動腳本
# 同時啟動後端 API 和前端開發伺服器

param(
    [switch]$Api,
    [switch]$Frontend,
    [switch]$All
)

Write-Host "🎮 GameCore 開發環境啟動腳本" -ForegroundColor Green

# 檢查必要工具
function Test-Prerequisites {
    Write-Host "檢查開發環境..." -ForegroundColor Yellow
    
    # 檢查 .NET
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -notlike "8.*") {
        Write-Error ".NET 8.0 未安裝"
        exit 1
    }
    
    # 檢查 Node.js
    $nodeVersion = node --version
    if (-not $nodeVersion) {
        Write-Error "Node.js 未安裝"
        exit 1
    }
    
    # 檢查 pnpm
    $pnpmVersion = pnpm --version
    if (-not $pnpmVersion) {
        Write-Host "安裝 pnpm..." -ForegroundColor Yellow
        npm install -g pnpm
    }
    
    Write-Host "✅ 開發環境檢查完成" -ForegroundColor Green
}

# 啟動後端 API
function Start-Api {
    Write-Host "🚀 啟動後端 API..." -ForegroundColor Yellow
    Set-Location "src\GameCore.Api"
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --urls `"http://localhost:5000;https://localhost:5001`""
    Set-Location ..\..
}

# 啟動前端開發伺服器
function Start-Frontend {
    Write-Host "🎨 啟動前端開發伺服器..." -ForegroundColor Yellow
    Set-Location "frontend"
    
    # 檢查是否已安裝套件
    if (-not (Test-Path "node_modules")) {
        Write-Host "安裝前端套件..." -ForegroundColor Yellow
        pnpm install
    }
    
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "pnpm dev"
    Set-Location ..
}

# 主執行流程
try {
    Test-Prerequisites
    
    if ($Api) {
        Start-Api
    }
    elseif ($Frontend) {
        Start-Frontend
    }
    elseif ($All -or (-not $Api -and -not $Frontend)) {
        Write-Host "同時啟動後端和前端..." -ForegroundColor Cyan
        Start-Api
        Start-Sleep -Seconds 3
        Start-Frontend
        
        Write-Host "`n🎉 開發環境啟動完成！" -ForegroundColor Green
        Write-Host "📊 後端 API: http://localhost:5000" -ForegroundColor Cyan
        Write-Host "🎨 前端應用: http://localhost:3000" -ForegroundColor Cyan
        Write-Host "📚 API 文件: http://localhost:5000/api-docs" -ForegroundColor Cyan
        Write-Host "💚 健康檢查: http://localhost:5000/health" -ForegroundColor Cyan
    }
}
catch {
    Write-Error "啟動過程中發生錯誤: $($_.Exception.Message)"
    exit 1
}
