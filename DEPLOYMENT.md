# 🚀 GameCore 部署指南

## 📋 目錄

- [本地開發環境部署](#本地開發環境部署)
- [雲端環境部署](#雲端環境部署)
- [部署檢查清單](#部署檢查清單)
- [常見問題與解決方案](#常見問題與解決方案)
- [監控與維護](#監控與維護)
- [安全性考量](#安全性考量)

## 🏠 本地開發環境部署

### 前置需求

- [Docker Desktop](https://www.docker.com/products/docker-desktop) 4.0+
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [PowerShell 5.1+](https://docs.microsoft.com/powershell/scripting/install/installing-powershell) (Windows)
- [Git](https://git-scm.com/)

### 快速啟動

#### 1. 完整環境啟動

```bash
# 啟動所有服務
docker-compose -f docker-compose.local.yml --profile full up -d

# 檢查服務狀態
docker-compose -f docker-compose.local.yml ps

# 查看日誌
docker-compose -f docker-compose.local.yml logs -f
```

#### 2. 分階段啟動

```bash
# 僅啟動基礎服務 (資料庫、快取)
docker-compose -f docker-compose.local.yml up -d sqlserver redis

# 啟動後端 API
docker-compose -f docker-compose.local.yml --profile backend up -d

# 啟動前端開發伺服器
docker-compose -f docker-compose.local.yml --profile frontend up -d

# 啟動監控服務
docker-compose -f docker-compose.local.yml --profile monitoring up -d
```

#### 3. 資料庫初始化

```bash
# 手動執行資料庫初始化 (如果自動執行失敗)
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -i /docker-entrypoint-initdb.d/init-db.sql

# 插入假資料
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -d GameCore -i /docker-entrypoint-initdb.d/seed-data.sql
```

### 本地開發工作流程

#### 後端開發

```bash
# 進入 API 容器
docker exec -it gamecore-api-local /bin/bash

# 執行測試
dotnet test

# 執行遷移
dotnet ef database update

# 重新建置
dotnet build
```

#### 前端開發

```bash
# 進入前端容器
docker exec -it gamecore-frontend-local /bin/bash

# 安裝新依賴
pnpm add package-name

# 執行測試
pnpm run test:unit

# 建置生產版本
pnpm run build
```

### 本地環境驗證

#### 健康檢查

```bash
# API 健康檢查
curl http://localhost:5000/health

# 前端狀態檢查
curl http://localhost:3000

# 資料庫連線測試
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"
```

#### 服務端口檢查

| 服務 | 端口 | 狀態檢查 |
|------|------|----------|
| SQL Server | 1433 | `telnet localhost 1433` |
| Redis | 6379 | `redis-cli -h localhost -p 6379 ping` |
| API | 5000 | `curl http://localhost:5000/health` |
| Frontend | 3000 | `curl http://localhost:3000` |
| Grafana | 3001 | `curl http://localhost:3001` |
| Adminer | 8080 | `curl http://localhost:8080` |

## ☁️ 雲端環境部署

### Azure 部署

#### 1. 資源群組建立

```bash
# 建立開發環境資源群組
az group create \
  --name gamecore-dev-rg \
  --location eastasia

# 建立生產環境資源群組
az group create \
  --name gamecore-prod-rg \
  --location eastasia
```

#### 2. 部署基礎設施

```bash
# 部署開發環境
az deployment group create \
  --resource-group gamecore-dev-rg \
  --template-file azure-deploy.yml \
  --parameters environment=dev

# 部署生產環境
az deployment group create \
  --resource-group gamecore-prod-rg \
  --template-file azure-deploy.yml \
  --parameters environment=prod
```

#### 3. 應用程式部署

```bash
# 部署到開發環境
.\scripts\deploy.ps1 -Environment dev -Action deploy

# 部署到生產環境
.\scripts\deploy.ps1 -Environment prod -Action deploy
```

### GitHub Actions 自動部署

#### 觸發條件

- **開發環境**: 推送到 `develop` 分支
- **生產環境**: 推送到 `main` 分支
- **手動觸發**: 使用 `workflow_dispatch` 事件

#### 部署流程

1. **程式碼品質檢查**
2. **測試執行**
3. **安全性掃描**
4. **Docker 映像建置**
5. **部署到目標環境**
6. **健康檢查驗證**

## ✅ 部署檢查清單

### 部署前檢查

- [ ] 所有測試通過
- [ ] 程式碼審查完成
- [ ] 安全性掃描通過
- [ ] 依賴套件更新檢查
- [ ] 環境變數配置正確
- [ ] 資料庫遷移腳本準備
- [ ] 備份策略確認

### 部署中檢查

- [ ] 基礎設施部署成功
- [ ] 資料庫連線正常
- [ ] 應用程式啟動成功
- [ ] 健康檢查通過
- [ ] 監控儀表板正常
- [ ] 日誌收集正常

### 部署後檢查

- [ ] 功能測試通過
- [ ] 效能測試達標
- [ ] 安全性測試通過
- [ ] 監控告警配置
- [ ] 備份驗證
- [ ] 回滾計畫準備

## 🔧 常見問題與解決方案

### 資料庫連線問題

#### 問題：Login failed for user 'sa'

```bash
# 解決方案 1: 檢查容器狀態
docker-compose -f docker-compose.local.yml logs sqlserver

# 解決方案 2: 重新啟動 SQL Server
docker-compose -f docker-compose.local.yml restart sqlserver

# 解決方案 3: 檢查密碼設定
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT 1"
```

#### 問題：資料庫初始化失敗

```bash
# 解決方案 1: 手動執行初始化腳本
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -i /docker-entrypoint-initdb.d/init-db.sql

# 解決方案 2: 檢查腳本語法
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -i /docker-entrypoint-initdb.d/init-db.sql -v
```

### 前端建置問題

#### 問題：Cannot find module 'vue'

```bash
# 解決方案 1: 清除並重新安裝依賴
cd frontend
rm -rf node_modules pnpm-lock.yaml
pnpm install

# 解決方案 2: 檢查 package.json
cat package.json | grep vue

# 解決方案 3: 重新建置容器
docker-compose -f docker-compose.local.yml build frontend
```

#### 問題：建置失敗

```bash
# 解決方案 1: 檢查 TypeScript 錯誤
cd frontend
pnpm run type-check

# 解決方案 2: 檢查 ESLint 錯誤
pnpm run lint

# 解決方案 3: 清理建置快取
pnpm run clean
pnpm run build
```

### 網路連線問題

#### 問題：Port already in use

```bash
# 解決方案 1: 檢查端口佔用
netstat -ano | findstr :5000

# 解決方案 2: 停止衝突服務
taskkill /PID <PID> /F

# 解決方案 3: 修改端口配置
# 編輯 docker-compose.local.yml 中的 ports 設定
```

#### 問題：容器間無法通訊

```bash
# 解決方案 1: 檢查網路配置
docker network ls
docker network inspect gamecore_local

# 解決方案 2: 重新建立網路
docker-compose -f docker-compose.local.yml down
docker-compose -f docker-compose.local.yml up -d

# 解決方案 3: 檢查服務依賴
docker-compose -f docker-compose.local.yml ps
```

## 📊 監控與維護

### 日誌管理

#### 日誌收集

```bash
# 查看 API 日誌
docker-compose -f docker-compose.local.yml logs -f api

# 查看資料庫日誌
docker-compose -f docker-compose.local.yml logs -f sqlserver

# 查看前端日誌
docker-compose -f docker-compose.local.yml logs -f frontend
```

#### 日誌分析

```bash
# 使用 Grafana 查看日誌
# 訪問 http://localhost:3001
# 帳號: admin / admin

# 使用 Loki 查詢日誌
# 訪問 http://localhost:3100
```

### 效能監控

#### 資源使用監控

```bash
# 查看容器資源使用
docker stats

# 查看磁碟使用
docker system df

# 查看網路使用
docker network ls
```

#### 應用程式效能

```bash
# API 響應時間
curl -w "@curl-format.txt" -o /dev/null -s "http://localhost:5000/health"

# 資料庫查詢效能
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -Q "SELECT * FROM sys.dm_exec_query_stats"
```

### 備份與還原

#### 資料庫備份

```bash
# 建立資料庫備份
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -Q "BACKUP DATABASE GameCore TO DISK = '/var/opt/mssql/backup/GameCore.bak'"

# 複製備份檔案到本地
docker cp gamecore-sqlserver-local:/var/opt/mssql/backup/GameCore.bak ./backups/
```

#### 資料庫還原

```bash
# 還原資料庫
docker exec -it gamecore-sqlserver-local /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd \
  -Q "RESTORE DATABASE GameCore FROM DISK = '/var/opt/mssql/backup/GameCore.bak'"
```

## 🔒 安全性考量

### 環境變數管理

#### 本地開發

```bash
# 複製環境變數範例
cp env.sample .env

# 編輯環境變數
# 注意：不要將 .env 提交到版本控制
echo ".env" >> .gitignore
```

#### 雲端環境

```bash
# 使用 Azure Key Vault
az keyvault secret set \
  --vault-name gamecore-kv \
  --name "JWT-Secret-Key" \
  --value "your-secret-key"

# 在應用程式中引用
az keyvault secret show \
  --vault-name gamecore-kv \
  --name "JWT-Secret-Key"
```

### 網路安全

#### 防火牆設定

```bash
# 本地防火牆規則 (Windows)
netsh advfirewall firewall add rule name="GameCore SQL Server" dir=in action=allow protocol=TCP localport=1433

# 本地防火牆規則 (Linux)
sudo ufw allow 1433/tcp
sudo ufw allow 5000/tcp
sudo ufw allow 3000/tcp
```

#### SSL/TLS 配置

```bash
# 生成開發用憑證
dotnet dev-certs https --clean
dotnet dev-certs https --trust

# 檢查憑證狀態
dotnet dev-certs https --check
```

### 存取控制

#### 資料庫權限

```sql
-- 建立最小權限用戶
CREATE LOGIN gamecore_user WITH PASSWORD = 'StrongPassword123!';
CREATE USER gamecore_user FOR LOGIN gamecore_user;

-- 授予必要權限
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO gamecore_user;
GRANT EXECUTE ON GetGameHeatRanking TO gamecore_user;
```

#### API 權限

```csharp
// 使用 JWT 認證
[Authorize]
[ApiController]
public class GameController : ControllerBase
{
    // 控制器實作
}

// 使用角色基礎授權
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // 管理員功能
}
```

## 📚 參考資源

### 官方文件

- [Docker Compose 文件](https://docs.docker.com/compose/)
- [SQL Server 容器文件](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
- [ASP.NET Core 部署](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [Vue.js 部署](https://vuejs.org/guide/best-practices/production-deployment.html)

### 工具與腳本

- [部署腳本](./scripts/deploy.ps1)
- [資料庫同步腳本](./scripts/db-sync.ps1)
- [監控腳本](./scripts/monitor.ps1)
- [建置腳本](./scripts/build.ps1)

### 監控與日誌

- [Grafana 儀表板](http://localhost:3001)
- [Loki 日誌查詢](http://localhost:3100)
- [Adminer 資料庫管理](http://localhost:8080)
- [Redis Commander](http://localhost:8081)

---

**🎯 記住：成功的部署始於充分的準備和測試！**