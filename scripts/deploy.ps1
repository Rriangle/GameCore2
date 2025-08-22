#!/usr/bin/env pwsh

<#
.SYNOPSIS
    GameCore 部署腳本
.DESCRIPTION
    自動化部署 GameCore 應用程式到不同環境
.PARAMETER Environment
    部署環境 (local, dev, staging, prod)
.PARAMETER Action
    執行動作 (build, deploy, sync, monitor)
.PARAMETER SkipTests
    跳過測試
.EXAMPLE
    .\deploy.ps1 -Environment dev -Action deploy
    .\deploy.ps1 -Environment local -Action build
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("local", "dev", "staging", "prod")]
    [string]$Environment,
    
    [Parameter(Mandatory = $true)]
    [ValidateSet("build", "deploy", "sync", "monitor", "full")]
    [string]$Action,
    
    [switch]$SkipTests,
    
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

# 檢查必要工具
function Test-Prerequisites {
    Write-Info "檢查必要工具..."
    
    $tools = @{
        "dotnet" = "dotnet --version"
        "node"   = "node --version"
        "pnpm"   = "pnpm --version"
        "docker" = "docker --version"
        "az"     = "az version"
    }
    
    foreach ($tool in $tools.GetEnumerator()) {
        try {
            $version = Invoke-Expression $tool.Value 2>$null
            Write-Success "$($tool.Key): $version"
        }
        catch {
            Write-Error "$($tool.Key) 未安裝或不在 PATH 中"
            return $false
        }
    }
    
    return $true
}

# 建置應用程式
function Build-Application {
    Write-Info "開始建置應用程式..."
    
    # 還原套件
    Write-Info "還原 .NET 套件..."
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error ".NET 套件還原失敗"
        exit 1
    }
    
    # 建置後端
    Write-Info "建置後端..."
    dotnet build --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "後端建置失敗"
        exit 1
    }
    
    # 建置前端
    Write-Info "建置前端..."
    Push-Location frontend
    pnpm install --frozen-lockfile
    pnpm build
    if ($LASTEXITCODE -ne 0) {
        Write-Error "前端建置失敗"
        exit 1
    }
    Pop-Location
    
    Write-Success "應用程式建置完成"
}

# 執行測試
function Run-Tests {
    if ($SkipTests) {
        Write-Warning "跳過測試"
        return
    }
    
    Write-Info "執行測試..."
    
    # 後端測試
    Write-Info "執行後端測試..."
    dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage" --results-directory ./TestResults
    if ($LASTEXITCODE -ne 0) {
        Write-Error "後端測試失敗"
        exit 1
    }
    
    # 前端測試
    Write-Info "執行前端測試..."
    Push-Location frontend
    pnpm test:coverage
    if ($LASTEXITCODE -ne 0) {
        Write-Error "前端測試失敗"
        exit 1
    }
    Pop-Location
    
    Write-Success "所有測試通過"
}

# 部署到本地環境
function Deploy-Local {
    Write-Info "部署到本地環境..."
    
    # 啟動 Docker Compose
    Write-Info "啟動 Docker Compose 服務..."
    docker-compose up -d
    
    # 等待服務啟動
    Write-Info "等待服務啟動..."
    Start-Sleep -Seconds 30
    
    # 執行資料庫遷移
    Write-Info "執行資料庫遷移..."
    dotnet ef database update --project src/GameCore.Infrastructure --startup-project src/GameCore.Api
    
    Write-Success "本地環境部署完成"
    Write-Info "API: http://localhost:5000"
    Write-Info "前端: http://localhost:3000"
    Write-Info "監控: http://localhost:3001"
}

# 部署到雲端環境
function Deploy-Cloud {
    param([string]$Environment)
    
    Write-Info "部署到 $Environment 環境..."
    
    # 檢查 Azure CLI 登入狀態
    $account = az account show 2>$null | ConvertFrom-Json
    if (-not $account) {
        Write-Error "請先登入 Azure CLI: az login"
        exit 1
    }
    
    # 設定資源群組名稱
    $resourceGroup = "gamecore-$Environment-rg"
    $location = "East Asia"
    
    # 建立資源群組
    Write-Info "建立資源群組: $resourceGroup"
    az group create --name $resourceGroup --location $location
    
    # 部署 Azure 資源
    Write-Info "部署 Azure 資源..."
    az deployment group create `
        --resource-group $resourceGroup `
        --template-file azure-deploy.yml `
        --parameters environment=$Environment `
        --parameters dbAdminPassword="$(Read-Host -Prompt "請輸入資料庫密碼" -AsSecureString | ConvertFrom-SecureString -AsPlainText)"
    
    # 部署應用程式
    Write-Info "部署應用程式..."
    az webapp deployment source config-zip `
        --resource-group $resourceGroup `
        --name "gamecore-web-$Environment" `
        --src "src/GameCore.Api/bin/Release/net8.0/publish.zip"
    
    Write-Success "$Environment 環境部署完成"
}

# 同步資料庫
function Sync-Database {
    param([string]$Environment)
    
    Write-Info "同步 $Environment 環境資料庫..."
    
    if ($Environment -eq "local") {
        # 本地資料庫同步
        Write-Info "執行本地資料庫遷移..."
        dotnet ef database update --project src/GameCore.Infrastructure --startup-project src/GameCore.Api
    }
    else {
        # 雲端資料庫同步
        $connectionString = Get-EnvironmentVariable "DATABASE_CONNECTION_STRING" $Environment
        Write-Info "執行雲端資料庫遷移..."
        dotnet ef database update --project src/GameCore.Infrastructure --startup-project src/GameCore.Api --connection "$connectionString"
    }
    
    Write-Success "資料庫同步完成"
}

# 監控應用程式
function Monitor-Application {
    param([string]$Environment)
    
    Write-Info "監控 $Environment 環境..."
    
    if ($Environment -eq "local") {
        # 本地監控
        Write-Info "檢查本地服務狀態..."
        
        $services = @(
            @{ Name = "API"; Url = "http://localhost:5000/health" },
            @{ Name = "前端"; Url = "http://localhost:3000" },
            @{ Name = "資料庫"; Url = "localhost:1433" },
            @{ Name = "Redis"; Url = "localhost:6379" }
        )
        
        foreach ($service in $services) {
            try {
                if ($service.Url -like "http*") {
                    $response = Invoke-WebRequest -Uri $service.Url -TimeoutSec 5
                    Write-Success "$($service.Name): 正常 ($($response.StatusCode))"
                }
                else {
                    $tcp = New-Object System.Net.Sockets.TcpClient
                    $tcp.ConnectAsync($service.Url.Split(":")[0], $service.Url.Split(":")[1]) | Out-Null
                    Write-Success "$($service.Name): 正常"
                }
            }
            catch {
                Write-Error "$($service.Name): 異常"
            }
        }
    }
    else {
        # 雲端監控
        $resourceGroup = "gamecore-$Environment-rg"
        Write-Info "檢查雲端服務狀態..."
        
        # 檢查 Web App 狀態
        $webApp = az webapp show --resource-group $resourceGroup --name "gamecore-web-$Environment" | ConvertFrom-Json
        Write-Info "Web App 狀態: $($webApp.state)"
        
        # 檢查資料庫狀態
        $sqlServer = az sql server show --resource-group $resourceGroup --name "gamecore-sql-$Environment" | ConvertFrom-Json
        Write-Info "SQL Server 狀態: $($sqlServer.state)"
    }
}

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

# 主程式
function Main {
    Write-ColorOutput "🚀 GameCore 部署腳本" "Magenta"
    Write-Info "環境: $Environment"
    Write-Info "動作: $Action"
    
    # 檢查必要工具
    if (-not (Test-Prerequisites)) {
        exit 1
    }
    
    # 根據動作執行相應功能
    switch ($Action) {
        "build" {
            Build-Application
            if (-not $SkipTests) {
                Run-Tests
            }
        }
        "deploy" {
            if ($Environment -eq "local") {
                Deploy-Local
            }
            else {
                Deploy-Cloud $Environment
            }
        }
        "sync" {
            Sync-Database $Environment
        }
        "monitor" {
            Monitor-Application $Environment
        }
        "full" {
            Build-Application
            if (-not $SkipTests) {
                Run-Tests
            }
            if ($Environment -eq "local") {
                Deploy-Local
            }
            else {
                Deploy-Cloud $Environment
            }
            Sync-Database $Environment
            Monitor-Application $Environment
        }
    }
    
    Write-Success "部署腳本執行完成"
}

# 執行主程式
Main
