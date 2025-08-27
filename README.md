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

## 🏗️ 系統架構

### 架構圖
- **整體架構**：`docs/architecture/system-overview.png`
- **資料庫設計**：`docs/architecture/database-schema.png`
- **API 設計**：`docs/architecture/api-design.png`
- **部署架構**：`docs/architecture/deployment.png`

### 技術架構
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend API   │    │   Database      │
│   (Vue 3)       │◄──►│   (.NET 8)      │◄──►│   (SQL Server)  │
│   + Tailwind    │    │   + EF Core     │    │   + Redis       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │              ┌─────────────────┐              │
         └──────────────►│   Monitoring   │◄─────────────┘
                        │   (Grafana)    │
                        └─────────────────┘
```

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

2. **環境設定**
   ```bash
   # 複製環境變數範例
   cp env.sample .env
   
   # 編輯 .env 檔案，填入實際值
   # 詳見 .env.sample 說明
   ```

3. **使用 Docker Compose 啟動完整環境**
   ```bash
   docker-compose up -d
   ```

4. **或使用一鍵設定腳本**
   ```bash
   .\scripts\setup.ps1
   ```

5. **啟動開發伺服器**
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

## 🔧 開發流程

### 分支策略
```
main (生產環境)
├── develop (開發環境)
│   ├── feature/user-management
│   ├── feature/game-system
│   └── feature/marketplace
└── hotfix/critical-bug
```

### 開發工作流程
1. **建立功能分支**：`git checkout -b feature/功能名稱`
2. **開發與測試**：本地測試通過後提交
3. **建立 PR**：推送到 develop 分支
4. **程式碼審查**：通過 CI 檢查與人工審查
5. **合併**：合併到 develop 分支
6. **發布**：定期從 develop 合併到 main

### 提交規範
```
feat: 新增用戶管理功能
fix: 修復登入驗證問題
docs: 更新 API 文件
style: 調整程式碼格式
refactor: 重構資料存取層
test: 新增用戶服務測試
chore: 更新依賴套件
```

## ⚠️ 常見錯誤排解

### 資料庫連線問題
```
錯誤：Login failed for user 'sa'
解決：檢查 SQL Server 容器狀態，確認密碼設定
指令：docker-compose logs sqlserver
```

### 前端建置失敗
```
錯誤：Cannot find module 'vue'
解決：清除 node_modules 重新安裝
指令：cd frontend && rm -rf node_modules && pnpm install
```

### Docker 容器啟動失敗
```
錯誤：Port already in use
解決：檢查端口佔用，停止衝突服務
指令：netstat -ano | findstr :5000
```

### 環境變數問題
```
錯誤：Configuration value 'JWT_SECRET_KEY' not found
解決：確認 .env 檔案存在且格式正確
檢查：env.sample 檔案說明
```

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
├── 📄 docker-compose.local.yml # 本地開發環境
├── 📄 env.development        # 開發環境配置
├── 📄 env.production         # 生產環境配置
├── 📄 .env.sample            # 環境變數範例
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
- **假資料**：使用 `scripts/seed-data.sql` 建立測試資料

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

### 本地環境 (.env)
```bash
# 複製 env.sample 為 .env 並填入實際值
cp env.sample .env

# 編輯 .env 檔案
# 詳見 .env.sample 中的詳細說明
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

### 程式碼風格
- **C#**: 遵循 .editorconfig 設定，使用 PascalCase 命名
- **TypeScript**: 使用 camelCase 命名，遵循 ESLint 規則
- **CSS**: 使用 Tailwind CSS 類別，遵循 BEM 命名原則

### 註解規範
- **中文註解**: 重要邏輯必須加上中文註解
- **英文註解**: API 文件、公開方法使用英文註解
- **格式**: 使用 XML 文件註解格式

### 測試規範
- **覆蓋率**: 新功能必須達到 80% 以上測試覆蓋率
- **測試類型**: 單元測試 + 整合測試
- **命名**: 測試方法使用 `Should_ExpectedBehavior_When_StateUnderTest` 格式

## 🤝 貢獻指南

1. **Fork 專案**
2. **建立功能分支** (`git checkout -b feature/AmazingFeature`)
3. **提交變更** (`git commit -m 'feat: Add some AmazingFeature'`)
4. **推送到分支** (`git push origin feature/AmazingFeature`)
5. **開啟 Pull Request**

### PR 檢查清單
- [ ] 程式碼通過所有測試
- [ ] 新增功能包含對應測試
- [ ] 更新相關文件
- [ ] 遵循命名規範
- [ ] 通過 CI/CD 檢查

## 📄 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

## 📞 支援

如有問題或建議，請：

1. 查看 [Issues](../../issues)
2. 建立新的 Issue
3. 聯繫開發團隊

---

**🎮 享受 GameCore 的遊戲世界！**
