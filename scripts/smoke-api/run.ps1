#!/usr/bin/env pwsh

# API 冒煙測試腳本
# 測試所有主要端點的功能

$BaseUrl = "http://localhost:5000"
$ApiUrl = "http://localhost:5001"

Write-Host "🚀 開始 API 冒煙測試..." -ForegroundColor Green
Write-Host "API 基礎 URL: $ApiUrl" -ForegroundColor Yellow

# 測試結果追蹤
$TestResults = @()

# 1. 健康檢查
Write-Host "`n📋 測試 1: 健康檢查" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/health" -Method GET -TimeoutSec 10
    $TestResults += @{Test = "健康檢查"; Status = "PASS"; Code = 200; Message = "API 正常運行" }
    Write-Host "✅ 健康檢查通過" -ForegroundColor Green
}
catch {
    $TestResults += @{Test = "健康檢查"; Status = "FAIL"; Code = $_.Exception.Response.StatusCode; Message = $_.Exception.Message }
    Write-Host "❌ 健康檢查失敗: $($_.Exception.Message)" -ForegroundColor Red
}

# 2. 錢包 API
Write-Host "`n💰 測試 2: 錢包 API" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/wallet" -Method GET -TimeoutSec 10
    if ($response -and ($response.points -ne $null -or $response.transactions -ne $null)) {
        $TestResults += @{Test = "錢包 API"; Status = "PASS"; Code = 200; Message = "錢包資料結構正確" }
        Write-Host "✅ 錢包 API 通過" -ForegroundColor Green
    }
    else {
        $TestResults += @{Test = "錢包 API"; Status = "FAIL"; Code = 200; Message = "回應格式不正確" }
        Write-Host "❌ 錢包 API 格式錯誤" -ForegroundColor Red
    }
}
catch {
    $TestResults += @{Test = "錢包 API"; Status = "FAIL"; Code = $_.Exception.Response.StatusCode; Message = $_.Exception.Message }
    Write-Host "❌ 錢包 API 失敗: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. 每日簽到
Write-Host "`n📅 測試 3: 每日簽到" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/signin/daily" -Method POST -TimeoutSec 10
    $TestResults += @{Test = "每日簽到"; Status = "PASS"; Code = 200; Message = "簽到成功" }
    Write-Host "✅ 每日簽到通過" -ForegroundColor Green
}
catch {
    $TestResults += @{Test = "每日簽到"; Status = "FAIL"; Code = $_.Exception.Response.StatusCode; Message = $_.Exception.Message }
    Write-Host "❌ 每日簽到失敗: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. 市場商品
Write-Host "`n🛒 測試 4: 市場商品" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/market/products" -Method GET -TimeoutSec 10
    if ($response -and $response.Count -ge 0) {
        $TestResults += @{Test = "市場商品"; Status = "PASS"; Code = 200; Message = "商品列表正常" }
        Write-Host "✅ 市場商品通過" -ForegroundColor Green
    }
    else {
        $TestResults += @{Test = "市場商品"; Status = "FAIL"; Code = 200; Message = "商品列表格式錯誤" }
        Write-Host "❌ 市場商品格式錯誤" -ForegroundColor Red
    }
}
catch {
    $TestResults += @{Test = "市場商品"; Status = "FAIL"; Code = $_.Exception.Response.StatusCode; Message = $_.Exception.Message }
    Write-Host "❌ 市場商品失敗: $($_.Exception.Message)" -ForegroundColor Red
}

# 5. 寵物 API
Write-Host "`n🐾 測試 5: 寵物 API" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/pet/me" -Method GET -TimeoutSec 10
    if ($response) {
        $TestResults += @{Test = "寵物 API"; Status = "PASS"; Code = 200; Message = "寵物資料正常" }
        Write-Host "✅ 寵物 API 通過" -ForegroundColor Green
    }
    else {
        $TestResults += @{Test = "寵物 API"; Status = "FAIL"; Code = 200; Message = "寵物資料格式錯誤" }
        Write-Host "❌ 寵物 API 格式錯誤" -ForegroundColor Red
    }
}
catch {
    $TestResults += @{Test = "寵物 API"; Status = "FAIL"; Code = $_.Exception.Response.StatusCode; Message = $_.Exception.Message }
    Write-Host "❌ 寵物 API 失敗: $($_.Exception.Message)" -ForegroundColor Red
}

# 輸出測試結果摘要
Write-Host "`n📊 測試結果摘要:" -ForegroundColor Magenta
Write-Host "=" * 60
$TestResults | Format-Table -AutoSize

$PassCount = ($TestResults | Where-Object { $_.Status -eq "PASS" }).Count
$TotalCount = $TestResults.Count

Write-Host "`n🎯 總計: $PassCount/$TotalCount 測試通過" -ForegroundColor $(if ($PassCount -eq $TotalCount) { "Green" } else { "Red" })

if ($PassCount -eq $TotalCount) {
    Write-Host "🎉 所有 API 冒煙測試通過！" -ForegroundColor Green
    exit 0
}
else {
    Write-Host "⚠️  部分測試失敗，請檢查 API 狀態" -ForegroundColor Yellow
    exit 1
} 