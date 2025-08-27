# GameCore 部署指南

## 🚀 部署前檢查清單

### 1. 環境準備
- [ ] .NET 8.0 SDK 已安裝
- [ ] Node.js 18+ 已安裝
- [ ] Docker 和 Docker Compose 已安裝
- [ ] SQL Server 2019+ 可用
- [ ] 環境變數檔案已配置

### 2. 資料庫設定
- [ ] SQL Server 連線字串已更新
- [ ] 資料庫初始化腳本已執行
- [ ] 假資料 Seeder 已執行
- [ ] 資料庫權限已配置

### 3. 應用程式配置
- [ ] JWT 密鑰已生成（最少32字元）
- [ ] 外部服務 API 金鑰已配置
- [ ] 郵件服務設定已完成
- [ ] 檔案儲存連線已配置

### 4. 安全設定
- [ ] 生產環境密碼已更改
- [ ] HTTPS 憑證已配置
- [ ] 防火牆規則已設定
- [ ] 資料庫存取權限已限制

## 🐳 Docker 部署

### 本地開發環境
```bash
# 啟動所有服務
docker-compose -f docker-compose.dev.yml up -d

# 查看服務狀態
docker-compose -f docker-compose.dev.yml ps

# 查看日誌
docker-compose -f docker-compose.dev.yml logs -f api
```

### 生產環境部署
```bash
# 建立生產環境映像
docker-compose -f docker-compose.yml build

# 啟動生產服務
docker-compose -f docker-compose.yml up -d

# 健康檢查
curl http://localhost:5000/health
```

## 🌐 環境變數配置

### 必要環境變數
```bash
# 資料庫
DefaultConnection="Server=your-server;Database=GameCore;User Id=your-user;Password=your-password;TrustServerCertificate=true"

# JWT
Jwt__SecretKey="your-super-secret-key-here-minimum-32-characters"
Jwt__Issuer="GameCore"
Jwt__Audience="GameCoreUsers"

# 應用程式
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000;https://+:5001
```

### 外部服務配置
```bash
# OAuth 設定
GOOGLE_CLIENT_ID=your-google-client-id
GOOGLE_CLIENT_SECRET=your-google-client-secret
FACEBOOK_APP_ID=your-facebook-app-id
FACEBOOK_APP_SECRET=your-facebook-app-secret
DISCORD_CLIENT_ID=your-discord-client-id
DISCORD_CLIENT_SECRET=your-discord-client-secret

# 郵件服務
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

## 📊 監控與日誌

### 應用程式日誌
- 日誌路徑：`/app/logs/`
- 日誌格式：JSON
- 日誌輪轉：每日
- 日誌保留：30天

### 健康檢查端點
- 基本健康檢查：`GET /health`
- 詳細健康檢查：`GET /health/detailed`
- 資料庫連線檢查：自動包含在健康檢查中

### 效能監控
- 記憶體使用量監控
- 資料庫連線池監控
- API 回應時間監控
- 錯誤率監控

## 🔒 安全最佳實踐

### 1. 資料庫安全
- 使用強密碼
- 限制網路存取
- 啟用 SSL/TLS 連線
- 定期備份資料

### 2. API 安全
- 啟用 JWT 認證
- 實作速率限制
- 啟用 CORS 保護
- 使用 HTTPS

### 3. 應用程式安全
- 定期更新依賴套件
- 掃描安全漏洞
- 實作輸入驗證
- 記錄安全事件

## 🚨 故障排除

### 常見問題

#### 1. 資料庫連線失敗
```bash
# 檢查 SQL Server 狀態
docker-compose ps sqlserver

# 檢查連線字串
echo $DefaultConnection

# 測試連線
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd
```

#### 2. API 無法啟動
```bash
# 檢查日誌
docker-compose logs api

# 檢查環境變數
docker-compose exec api env | grep -E "(ASPNETCORE|Jwt|DefaultConnection)"

# 檢查端口衝突
netstat -tulpn | grep :5000
```

#### 3. 前端無法連接到 API
```bash
# 檢查 API 狀態
curl http://localhost:5000/health

# 檢查 CORS 設定
# 確認 VITE_API_BASE_URL 設定正確

# 檢查網路連線
docker network ls
docker network inspect gamecore_gamecore-network
```

### 日誌分析
```bash
# 即時查看日誌
docker-compose logs -f --tail=100 api

# 搜尋錯誤日誌
docker-compose logs api | grep -i error

# 查看特定時間的日誌
docker-compose logs --since="2024-01-01T00:00:00" api
```

## 📈 效能優化

### 1. 資料庫優化
- 建立適當的索引
- 使用連線池
- 定期維護統計資料
- 監控慢查詢

### 2. 應用程式優化
- 啟用快取機制
- 使用非同步處理
- 實作分頁機制
- 壓縮回應內容

### 3. 前端優化
- 啟用 Gzip 壓縮
- 使用 CDN 加速
- 實作懶載入
- 優化打包大小

## 🔄 更新與維護

### 部署更新
```bash
# 1. 拉取最新程式碼
git pull origin main

# 2. 重建映像
docker-compose build

# 3. 重新部署
docker-compose up -d

# 4. 檢查服務狀態
docker-compose ps
```

### 資料庫遷移
```bash
# 執行 Entity Framework 遷移
docker-compose exec api dotnet ef database update

# 檢查遷移狀態
docker-compose exec api dotnet ef migrations list
```

### 備份與還原
```bash
# 備份資料庫
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "BACKUP DATABASE GameCore TO DISK = '/var/opt/mssql/backup/GameCore.bak'"

# 還原資料庫
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "RESTORE DATABASE GameCore FROM DISK = '/var/opt/mssql/backup/GameCore.bak'"
```

## 📞 支援與聯絡

### 技術支援
- 專案文件：查看 README.md
- 問題回報：建立 GitHub Issue
- 開發討論：使用 GitHub Discussions

### 緊急聯絡
- 系統管理員：[admin@gamecore.com]
- 技術負責人：[tech@gamecore.com]
- 24/7 支援：[support@gamecore.com]

---

**注意**：此文件會定期更新，請確保使用最新版本。