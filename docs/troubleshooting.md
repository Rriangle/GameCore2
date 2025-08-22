# 🔧 GameCore 錯誤排解指南

## 🚨 常見錯誤與解決方案

### 1. 資料庫連線問題

#### 錯誤訊息
```
System.Data.SqlClient.SqlException: A network-related or instance-specific error occurred while establishing a connection to SQL Server
```

#### 解決方案
```powershell
# 檢查 SQL Server 服務狀態
Get-Service -Name "MSSQL*"

# 啟動 SQL Server 服務
Start-Service -Name "MSSQLSERVER"

# 檢查 Docker 容器狀態
docker ps | findstr sqlserver

# 重啟 SQL Server 容器
docker restart gamecore-sqlserver
```

#### 預防措施
- 確認 SQL Server 服務已啟動
- 檢查防火牆設定
- 驗證連線字串格式

### 2. Docker 容器啟動失敗

#### 錯誤訊息
```
Error response from daemon: port is already allocated
```

#### 解決方案
```powershell
# 檢查端口使用情況
netstat -ano | findstr :1433
netstat -ano | findstr :5000
netstat -ano | findstr :3000

# 停止佔用端口的程序
taskkill /PID <PID> /F

# 清理 Docker 資源
docker system prune -f
docker volume prune -f

# 重新啟動容器
docker-compose down
docker-compose up -d
```

#### 預防措施
- 使用 `docker-compose down` 完全停止服務
- 定期清理未使用的 Docker 資源
- 檢查端口衝突

### 3. 前端建置失敗

#### 錯誤訊息
```
Error: Cannot find module 'xxx'
```

#### 解決方案
```powershell
# 清除 node_modules
cd frontend
Remove-Item -Recurse -Force node_modules
Remove-Item package-lock.json

# 重新安裝依賴
npm install

# 或使用 pnpm
pnpm install

# 清除快取
npm cache clean --force
```

#### 預防措施
- 確認 Node.js 版本為 18+
- 使用 `package-lock.json` 鎖定依賴版本
- 定期更新依賴套件

### 4. API 認證失敗

#### 錯誤訊息
```
System.ArgumentException: IDX10634: Unable to create the SignatureProvider
```

#### 解決方案
```powershell
# 檢查 JWT 密鑰設定
Get-Content env.development | Select-String "JWT_SECRET_KEY"

# 重新生成 JWT 密鑰
$jwtKey = [System.Web.Security.Membership]::GeneratePassword(64, 10)
echo "JWT_SECRET_KEY=`"$jwtKey`"" >> env.development

# 重啟 API 服務
docker-compose restart api
```

#### 預防措施
- 使用足夠長度的 JWT 密鑰 (至少 32 字元)
- 不同環境使用不同的密鑰
- 定期輪換密鑰

### 5. 記憶體不足錯誤

#### 錯誤訊息
```
System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown
```

#### 解決方案
```powershell
# 增加 Docker 記憶體限制
# 在 Docker Desktop 設定中調整記憶體分配

# 清理系統記憶體
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"} | Stop-Process -Force

# 重啟開發環境
.\scripts\dev.ps1 -Restart
```

#### 預防措施
- 監控記憶體使用情況
- 定期重啟開發環境
- 使用記憶體分析工具

### 6. 網路連線問題

#### 錯誤訊息
```
System.Net.Http.HttpRequestException: No such host is known
```

#### 解決方案
```powershell
# 檢查網路連線
Test-NetConnection -ComputerName localhost -Port 5000
Test-NetConnection -ComputerName localhost -Port 3000

# 檢查 Docker 網路
docker network ls
docker network inspect gamecore-network

# 重新建立網路
docker network create gamecore-network
docker-compose up -d
```

#### 預防措施
- 確認 Docker 網路設定正確
- 檢查防火牆規則
- 使用 `docker-compose` 管理網路

## 🔍 診斷工具

### 1. 健康檢查
```powershell
# API 健康檢查
Invoke-RestMethod -Uri "http://localhost:5000/health"

# 資料庫連線檢查
.\scripts\db-sync.ps1 -Environment local -Action check

# 前端狀態檢查
Invoke-RestMethod -Uri "http://localhost:3000"
```

### 2. 日誌檢查
```powershell
# 查看 API 日誌
docker logs gamecore-api

# 查看資料庫日誌
docker logs gamecore-sqlserver

# 查看前端日誌
docker logs gamecore-frontend
```

### 3. 效能監控
```powershell
# 監控系統資源
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*" -or $_.ProcessName -like "*node*"}

# 監控 Docker 資源
docker stats
```

## 🛠️ 快速修復腳本

### 完整環境重置
```powershell
# 停止所有服務
docker-compose down

# 清理資源
docker system prune -f
docker volume prune -f

# 重新啟動
docker-compose up -d

# 等待服務啟動
Start-Sleep -Seconds 30

# 檢查服務狀態
docker-compose ps
```

### 資料庫重置
```powershell
# 備份現有資料
.\scripts\db-sync.ps1 -Environment local -Action backup

# 重置資料庫
.\scripts\db-sync.ps1 -Environment local -Action reset

# 重新建立假資料
.\scripts\db-seeder.ps1 -Environment local
```

### 前端重置
```powershell
# 進入前端目錄
cd frontend

# 清理快取
npm cache clean --force
Remove-Item -Recurse -Force node_modules
Remove-Item package-lock.json

# 重新安裝
npm install

# 重新建置
npm run build
```

## 📞 尋求協助

如果以上解決方案無法解決問題，請：

1. **收集錯誤資訊**
   - 完整的錯誤訊息
   - 系統環境資訊
   - 重現步驟

2. **檢查已知問題**
   - 查看 [GitHub Issues](../../issues)
   - 搜尋相關討論

3. **建立 Issue**
   - 使用問題模板
   - 提供詳細資訊
   - 附上相關日誌

## 🔄 預防性維護

### 定期檢查清單
- [ ] 更新依賴套件
- [ ] 清理 Docker 資源
- [ ] 備份資料庫
- [ ] 檢查磁碟空間
- [ ] 更新開發工具

### 監控指標
- 記憶體使用率
- CPU 使用率
- 磁碟空間
- 網路連線
- 服務響應時間

---

*此指南會持續更新，請定期檢查最新版本*