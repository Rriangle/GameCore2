#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("local", "dev", "staging", "prod")]
    [string]$Environment
)

# 顏色函數
function Write-Success { Write-Host "✅ $args" -ForegroundColor Green }
function Write-Info { Write-Host "ℹ️  $args" -ForegroundColor Cyan }
function Write-Warning { Write-Host "⚠️  $args" -ForegroundColor Yellow }
function Write-Error { Write-Host "❌ $args" -ForegroundColor Red }

# 檢查 HTTP 端點
function Test-HttpEndpoint {
    param([string]$Url, [string]$Name)
    
    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec 10 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Success "$Name`: 正常 ($($response.StatusCode))"
            return $true
        }
        else {
            Write-Warning "$Name`: 異常 ($($response.StatusCode))"
            return $false
        }
    }
    catch {
        Write-Error "$Name`: 無法連線 ($($_.Exception.Message))"
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
            Write-Success "$Name`: 正常"
            $tcp.Close()
            return $true
        }
        else {
            Write-Error "$Name`: 無法連線"
            $tcp.Close()
            return $false
        }
    }
    catch {
        Write-Error "$Name`: 連線失敗 ($($_.Exception.Message))"
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
                Write-Success "$($container.Name)`: 運行中"
            }
            else {
                Write-Warning "$($container.Name)`: $status"
            }
        }
        catch {
            Write-Error "$($container.Name)`: 容器不存在"
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

# 主程式
Write-Host "🔍 GameCore 簡化監控開始" -ForegroundColor Magenta
Write-Info "環境: $Environment"

if ($Environment -eq "local") {
    Test-DockerContainers
    Test-LocalServices
}
else {
    Write-Info "雲端環境監控功能待實現"
}

Write-Success "監控完成"
