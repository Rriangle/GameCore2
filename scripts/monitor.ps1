#!/usr/bin/env pwsh

<#
.SYNOPSIS
    GameCore 監控腳本
.DESCRIPTION
    監控本地和雲端環境的應用程式狀態
.PARAMETER Environment
    監控環境 (local, dev, staging, prod)
.PARAMETER Duration
    監控持續時間（分鐘）
.PARAMETER Interval
    檢查間隔（秒）
.EXAMPLE
    .\monitor.ps1 -Environment local
    .\monitor.ps1 -Environment prod -Duration 60 -Interval 30
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("local", "dev", "staging", "prod")]
    [string]$Environment,
    
    [int]$Duration = 0,
    
    [int]$Interval = 30
)

# 設定錯誤處理
$ErrorActionPreference = "Continue"

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

# 檢查 HTTP 端點
function Test-HttpEndpoint {
    param([string]$Url, [string]$Name)
    
    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec 10 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Success "$Name: 正常 ($($response.StatusCode))"
            return $true
        }
        else {
            Write-Warning "$Name: 異常 ($($response.StatusCode))"
            return $false
        }
    }
    catch {
        Write-Error "$Name: 無法連線 ($($_.Exception.Message))"
        return $false
    }
}

# 檢查 TCP 端點
function Test-TcpEndpoint {
    param([string]$Host, [int]$Port, [string]$Name)
    
    try {
        $tcp = New-Object System.Net.Sockets.TcpClient
        $result = $tcp.BeginConnect($Host, $Port, $null, $null)
        $success = $result.AsyncWaitHandle.WaitOne(5000, $false)
        
        if ($success) {
            $tcp.EndConnect($result)
            Write-Success "$Name: 正常"
            $tcp.Close()
            return $true
        }
        else {
            Write-Error "$Name: 無法連線"
            $tcp.Close()
            return $false
        }
    }
    catch {
        Write-Error "$Name: 連線失敗 ($($_.Exception.Message))"
        return $false
    }
}

# 檢查 Docker 容器狀態
function Test-DockerContainers {
    Write-Info "檢查 Docker 容器狀態..."
    
    $containers = @(
        @{ Name = "SQL Server"; Container = "gamecore-sqlserver" },
        @{ Name = "Redis"; Container = "gamecore-redis" },
        @{ Name = "API"; Container = "gamecore-api" },
        @{ Name = "前端"; Container = "gamecore-frontend" },
        @{ Name = "監控"; Container = "gamecore-monitoring" }
    )
    
    foreach ($container in $containers) {
        try {
            $status = docker inspect --format='{{.State.Status}}' $container.Container 2>$null
            if ($status -eq "running") {
                Write-Success "$($container.Name): 運行中"
            }
            else {
                Write-Warning "$($container.Name): $status"
            }
        }
        catch {
            Write-Error "$($container.Name): 容器不存在"
        }
    }
}

# 檢查本地服務狀態
function Test-LocalServices {
    Write-Info "檢查本地服務狀態..."
    
    $services = @(
        @{ Name = "API"; Url = "http://localhost:5000/health" },
        @{ Name = "前端"; Url = "http://localhost:3000" },
        @{ Name = "監控"; Url = "http://localhost:3001" }
    )
    
    $tcpServices = @(
        @{ Name = "SQL Server"; Host = "localhost"; Port = 1433 },
        @{ Name = "Redis"; Host = "localhost"; Port = 6379 }
    )
    
    foreach ($service in $services) {
        Test-HttpEndpoint -Url $service.Url -Name $service.Name
    }
    
    foreach ($service in $tcpServices) {
        Test-TcpEndpoint -Host $service.Host -Port $service.Port -Name $service.Name
    }
}

# 檢查雲端服務狀態
function Test-CloudServices {
    param([string]$Environment)
    
    Write-Info "檢查 $Environment 環境雲端服務狀態..."
    
    try {
        # 檢查 Azure CLI 登入狀態
        $account = az account show 2>$null | ConvertFrom-Json
        if (-not $account) {
            Write-Error "請先登入 Azure CLI: az login"
            return
        }
        
        $resourceGroup = "gamecore-$Environment-rg"
        
        # 檢查資源群組
        $rg = az group show --name $resourceGroup 2>$null | ConvertFrom-Json
        if ($rg) {
            Write-Success "資源群組: 存在"
            
            # 檢查 Web App
            $webApp = az webapp show --resource-group $resourceGroup --name "gamecore-web-$Environment" 2>$null | ConvertFrom-Json
            if ($webApp) {
                Write-Success "Web App: $($webApp.state)"
                
                # 檢查 Web App 健康狀態
                $health = az webapp show --resource-group $resourceGroup --name "gamecore-web-$Environment" --query "availabilityState" 2>$null
                if ($health -eq "Normal") {
                    Write-Success "Web App 健康狀態: 正常"
                }
                else {
                    Write-Warning "Web App 健康狀態: $health"
                }
            }
            else {
                Write-Error "Web App: 不存在"
            }
            
            # 檢查 SQL Server
            $sqlServer = az sql server show --resource-group $resourceGroup --name "gamecore-sql-$Environment" 2>$null | ConvertFrom-Json
            if ($sqlServer) {
                Write-Success "SQL Server: $($sqlServer.state)"
            }
            else {
                Write-Error "SQL Server: 不存在"
            }
        }
        else {
            Write-Error "資源群組: 不存在"
        }
    }
    catch {
        Write-Error "檢查雲端服務時發生錯誤: $($_.Exception.Message)"
    }
}

# 檢查系統資源
function Test-SystemResources {
    Write-Info "檢查系統資源..."
    
    # CPU 使用率
    $cpu = Get-Counter '\Processor(_Total)\% Processor Time' | Select-Object -ExpandProperty CounterSamples | Select-Object -ExpandProperty CookedValue
    Write-Info "CPU 使用率: $([math]::Round($cpu, 2))%"
    
    # 記憶體使用率
    $memory = Get-Counter '\Memory\Available MBytes' | Select-Object -ExpandProperty CounterSamples | Select-Object -ExpandProperty CookedValue
    $totalMemory = (Get-WmiObject -Class Win32_ComputerSystem).TotalPhysicalMemory / 1MB
    $memoryUsage = (($totalMemory - $memory) / $totalMemory) * 100
    Write-Info "記憶體使用率: $([math]::Round($memoryUsage, 2))%"
    
    # 磁碟空間
    $disk = Get-WmiObject -Class Win32_LogicalDisk -Filter "DeviceID='C:'"
    $diskUsage = (($disk.Size - $disk.FreeSpace) / $disk.Size) * 100
    Write-Info "C 槽使用率: $([math]::Round($diskUsage, 2))%"
}

# 檢查日誌檔案
function Test-LogFiles {
    Write-Info "檢查日誌檔案..."
    
    $logDir = "logs"
    if (Test-Path $logDir) {
        $logFiles = Get-ChildItem -Path $logDir -Filter "*.txt" | Sort-Object LastWriteTime -Descending | Select-Object -First 5
        
        foreach ($logFile in $logFiles) {
            $size = [math]::Round($logFile.Length / 1KB, 2)
            $lastWrite = $logFile.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
            Write-Info "$($logFile.Name): $size KB (最後更新: $lastWrite)"
        }
    }
    else {
        Write-Warning "日誌目錄不存在"
    }
}

# 主監控函數
function Start-Monitoring {
    param([string]$Environment, [int]$Duration, [int]$Interval)
    
    Write-ColorOutput "🔍 GameCore 監控開始" "Magenta"
    Write-Info "環境: $Environment"
    Write-Info "檢查間隔: $Interval 秒"
    
    if ($Duration -gt 0) {
        Write-Info "監控持續時間: $Duration 分鐘"
        $endTime = (Get-Date).AddMinutes($Duration)
    }
    
    $iteration = 1
    
    do {
        Write-ColorOutput "`n=== 第 $iteration 次檢查 ($(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')) ===" "Yellow"
        
        # 檢查系統資源
        Test-SystemResources
        
        # 根據環境檢查服務
        if ($Environment -eq "local") {
            Test-DockerContainers
            Test-LocalServices
        }
        else {
            Test-CloudServices $Environment
        }
        
        # 檢查日誌檔案
        Test-LogFiles
        
        $iteration++
        
        if ($Duration -gt 0 -and (Get-Date) -ge $endTime) {
            break
        }
        
        if ($Duration -gt 0) {
            Write-Info "等待 $Interval 秒後進行下次檢查..."
            Start-Sleep -Seconds $Interval
        }
    } while ($Duration -gt 0)
    
    Write-Success "監控完成"
}

# 主程式
function Main {
    # 檢查必要工具
    if ($Environment -eq "local") {
        try {
            docker --version | Out-Null
        }
        catch {
            Write-Error "Docker 未安裝或不在 PATH 中"
            exit 1
        }
    }
    else {
        try {
            az version | Out-Null
        }
        catch {
            Write-Error "Azure CLI 未安裝或不在 PATH 中"
            exit 1
        }
    }
    
    # 開始監控
    Start-Monitoring -Environment $Environment -Duration $Duration -Interval $Interval
}

# 執行主程式
Main
