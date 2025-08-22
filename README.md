# 🎮 GameCore 遊戲平台

一個整合遊戲熱度觀測、論壇社群、商城、玩家市場、寵物養成的綜合平台。

## 📋 專案概述

GameCore 是一個功能豐富的遊戲社群平台，提供：

- 🎯 **遊戲熱度觀測**：即時追蹤遊戲熱度與排行榜
- 💬 **論壇社群**：遊戲討論與社群互動
- 🛒 **商城市集**：官方商城與玩家自由市場
- 🐾 **寵物養成**：可愛史萊姆寵物系統
- 🎮 **小遊戲**：冒險模式與每日簽到
- 👥 **用戶系統**：完整的註冊登入與權限管理
- ☁️ **雲端同步**：本地與雲端環境無縫整合

## 🛠️ 技術棧

### 後端
- **.NET 8.0** - 核心框架
- **ASP.NET Core** - Web API 與 MVC
- **Entity Framework Core** - 資料存取
- **SQL Server** - 資料庫
- **Serilog** - 日誌記錄
- **xUnit** - 單元測試

### 前端
- **Vue 3** - 前端框架
- **TypeScript** - 型別安全
- **Tailwind CSS** - 樣式框架
- **Vite** - 建置工具
- **Pinia** - 狀態管理
- **Vue Router** - 路由管理

### 雲端與部署
- **Azure** - 雲端平台
- **Docker** - 容器化
- **GitHub Actions** - CI/CD
- **Application Insights** - 監控

## 🚀 快速開始

### 必要條件

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (雲端部署)
- [PowerShell 5.1+](https://docs.microsoft.com/powershell/scripting/install/installing-powershell)

### 本地開發環境

1. **複製專案**
   ```bash
   git clone <repository-url>
   cd GameCore
   ```

2. **使用 Docker Compose 啟動完整環境**
   ```bash
   docker-compose up -d
   ```

3. **或使用一鍵設定腳本**
   ```bash
   .\scripts\setup.ps1
   ```

4. **啟動開發伺服器**
   ```bash
   .\scripts\dev.ps1
   ```

### 驗證安裝

啟動完成後，您應該能夠訪問：

- 🌐 **前端應用**：http://localhost:3000
- 📊 **後端 API**：http://localhost:5000
- 📚 **API 文件**：http://localhost:5000/api-docs
- 💚 **健康檢查**：http://localhost:5000/health
- 📈 **監控儀表板**：http://localhost:3001

## ☁️ 雲端與本地同步

### 環境管理

專案支援多環境部署：
- **local**: 本地開發環境
- **dev**: 開發環境
- **staging**: 測試環境
- **prod**: 生產環境

### 部署指令

```powershell
# 本地環境部署
.\scripts\deploy.ps1 -Environment local -Action full

# 開發環境部署
.\scripts\deploy.ps1 -Environment dev -Action deploy

# 生產環境部署
.\scripts\deploy.ps1 -Environment prod -Action deploy

# 資料庫同步
.\scripts\db-sync.ps1 -Environment local -Action migrate
.\scripts\db-sync.ps1 -Environment dev -Action backup

# 監控應用程式
.\scripts\monitor.ps1 -Environment local
.\scripts\monitor.ps1 -Environment prod -Duration 60
```

### Azure 資源部署

```bash
# 部署 Azure 資源
az deployment group create \
  --resource-group gamecore-dev-rg \
  --template-file azure-deploy.yml \
  --parameters environment=dev
```

## 📁 專案結構

```
GameCore/
├── 📁 src/                    # 後端原始碼
│   ├── 📁 GameCore.Api/      # Web API 專案
│   ├── 📁 GameCore.Mvc/      # MVC 專案
│   ├── 📁 GameCore.Domain/   # 領域層
│   ├── 📁 GameCore.Infrastructure/ # 基礎設施層
│   └── 📁 GameCore.Shared/   # 共用元件
├── 📁 frontend/              # 前端專案
├── 📁 tests/                 # 測試專案
├── 📁 scripts/               # 建置腳本
├── 📁 .github/workflows/     # CI/CD 管道
├── 📄 azure-deploy.yml       # Azure 部署模板
├── 📄 docker-compose.yml     # 本地容器化環境
├── 📄 env.development        # 開發環境配置
├── 📄 env.production         # 生產環境配置
└── 📄 README.md              # 專案說明
```

## 🔧 開發指令

### 建置專案
```powershell
# 完整建置
.\scripts\build.ps1 -All

# 僅後端
.\scripts\build.ps1 -Build

# 包含前端
.\scripts\build.ps1 -Build -Frontend
```

### 執行測試
```powershell
# 執行所有測試
.\scripts\build.ps1 -Test

# 或直接執行
dotnet test

# 一鍵測試
.\scripts\test.ps1
```

### 開發環境
```powershell
# 啟動完整開發環境
.\scripts\dev.ps1

# 僅啟動後端
.\scripts\dev.ps1 -Api

# 僅啟動前端
.\scripts\dev.ps1 -Frontend
```

## 📊 資料庫

### 本地開發
專案使用 SQL Server LocalDB 作為開發環境資料庫：

- **連線字串**：`Server=(localdb)\mssqllocaldb;Database=GameCore;Trusted_Connection=true;MultipleActiveResultSets=true`
- **自動建立**：應用程式啟動時會自動建立資料庫
- **假資料**：後續會提供假資料腳本

### 資料庫同步
```powershell
# 執行遷移
.\scripts\db-sync.ps1 -Environment local -Action migrate

# 建立種子資料
.\scripts\db-sync.ps1 -Environment local -Action seed

# 備份資料庫
.\scripts\db-sync.ps1 -Environment dev -Action backup

# 還原資料庫
.\scripts\db-sync.ps1 -Environment prod -Action restore -Source dev
```

## 🔐 環境變數

### 本地環境 (env.development)
```bash
DATABASE_CONNECTION_STRING="Server=localhost,1433;Database=GameCore;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
JWT_SECRET_KEY="dev-super-secret-key-with-at-least-32-characters"
REDIS_CONNECTION_STRING="localhost:6379"
```

### 生產環境 (env.production)
```bash
DATABASE_CONNECTION_STRING="Server=tcp:gamecore-sql-prod.database.windows.net,1433;Initial Catalog=gamecore-db-prod;..."
JWT_SECRET_KEY="@Microsoft.KeyVault(SecretUri=https://gamecore-kv-prod.vault.azure.net/secrets/JwtSecretKey/)"
REDIS_CONNECTION_STRING="gamecore-redis-prod.redis.cache.windows.net:6380,password=${REDIS_PASSWORD},ssl=True"
```

## 🧪 測試

專案包含完整的測試架構：

- **單元測試**：使用 xUnit
- **整合測試**：資料庫與 API 測試
- **端對端測試**：完整流程測試

執行測試：
```powershell
dotnet test --verbosity normal
```

## 📈 監控與日誌

### 本地監控
```powershell
# 即時監控
.\scripts\monitor.ps1 -Environment local

# 持續監控
.\scripts\monitor.ps1 -Environment local -Duration 60 -Interval 30
```

### 雲端監控
- **Application Insights**: 應用程式效能監控
- **Azure Monitor**: 資源監控
- **Grafana**: 自訂儀表板

## 🚀 部署

### 本地部署
```powershell
# 使用 Docker Compose
docker-compose up -d

# 或使用部署腳本
.\scripts\deploy.ps1 -Environment local -Action deploy
```

### 雲端部署
```powershell
# 部署到開發環境
.\scripts\deploy.ps1 -Environment dev -Action deploy

# 部署到生產環境
.\scripts\deploy.ps1 -Environment prod -Action deploy
```

### CI/CD 管道
- **GitHub Actions**: 自動化測試和建置
- **Azure DevOps**: 雲端部署管道
- **Docker**: 容器化部署

## 📝 開發規範

- **程式碼風格**：遵循 .editorconfig 設定
- **命名規範**：C# 使用 PascalCase，JavaScript 使用 camelCase
- **註解**：重要邏輯必須加上中文註解
- **測試**：新功能必須包含對應測試

## 🤝 貢獻指南

1. Fork 專案
2. 建立功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

## 📄 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

## 📞 支援

如有問題或建議，請：

1. 查看 [Issues](../../issues)
2. 建立新的 Issue
3. 聯繫開發團隊

---

**🎮 享受 GameCore 的遊戲世界！**
