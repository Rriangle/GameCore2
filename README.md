# 🎮 GameCore 遊戲數據分析平台

## 📖 專案介紹

GameCore 是一個現代化的遊戲數據分析平台，專為遊戲開發者、發行商和數據分析師設計。平台整合多個遊戲數據來源，提供實時監控、趨勢分析和洞察報告，幫助遊戲產業做出數據驅動的決策。

### ✨ 核心功能
- **多平台數據整合**：支援 Steam、App Store、Google Play、Twitch 等平台
- **實時數據監控**：同時在線人數、日活躍用戶、收入等關鍵指標
- **智能分析引擎**：AI 驅動的趨勢預測和異常檢測
- **可視化儀表板**：互動式圖表和自定義報告
- **API 優先架構**：RESTful API 支援第三方整合

### 🏗️ 技術架構
- **後端**：ASP.NET Core 8.0 + C# + Entity Framework Core
- **資料庫**：SQL Server 2019+ + Redis 快取
- **前端**：Vue 3 + TypeScript + Tailwind CSS + Vite
- **部署**：Docker + Docker Compose + GitHub Actions
- **認證**：JWT + OAuth (Google, Facebook, Discord)

## 🚀 快速開始

### 環境需求
- .NET 8.0 SDK
- Node.js 18+
- Docker & Docker Compose
- SQL Server 2019+ (或使用 Docker)

### 1. 克隆專案
```bash
git clone https://github.com/your-username/gamecore.git
cd gamecore
```

### 2. 配置環境變數
```bash
# 複製環境變數範本
cp .env.sample .env

# 編輯 .env 檔案，填入必要的配置
nano .env
```

### 3. 啟動資料庫
```bash
# 使用 Docker 啟動 SQL Server
docker-compose -f docker-compose.dev.yml up -d sqlserver

# 等待資料庫啟動完成（約 30-60 秒）
```

### 4. 初始化資料庫
```bash
# 執行資料庫初始化腳本
docker exec -i gamecore-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -i scripts/init-database.sql

# 執行假資料 Seeder
docker exec -i gamecore-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -i scripts/seed-data.sql
```

### 5. 啟動後端 API
```bash
# 啟動 API 服務
docker-compose -f docker-compose.dev.yml up -d api

# 檢查 API 狀態
curl http://localhost:5000/health
```

### 6. 啟動前端應用
```bash
# 啟動前端服務
docker-compose -f docker-compose.dev.yml up -d frontend

# 或使用本地開發模式
cd frontend
pnpm install
pnpm dev
```

### 7. 訪問應用
- **前端應用**：http://localhost:3000
- **後端 API**：http://localhost:5000
- **API 文件**：http://localhost:5000/swagger
- **資料庫管理**：http://localhost:8080 (Adminer)

## 🏛️ 專案架構

### 目錄結構
```
gamecore/
├── src/                          # 後端原始碼
│   ├── GameCore.Api/            # Web API 專案
│   ├── GameCore.Domain/         # 領域模型和業務邏輯
│   ├── GameCore.Infrastructure/ # 基礎設施層（資料庫、外部服務）
│   ├── GameCore.Mvc/            # MVC 專案（管理後台）
│   └── GameCore.Shared/         # 共用類別和工具
├── frontend/                     # 前端應用
│   ├── src/                     # Vue 組件和邏輯
│   ├── public/                  # 靜態資源
│   └── dist/                    # 建置輸出
├── tests/                       # 測試專案
├── scripts/                     # 資料庫腳本和工具
├── .github/                     # GitHub Actions 配置
└── docs/                        # 專案文件
```

### 架構圖
- **系統架構圖**：[docs/architecture/system-architecture.png]
- **資料庫 ER 圖**：[docs/database/er-diagram.png]
- **API 流程圖**：[docs/api/api-flow.png]
- **部署架構圖**：[docs/deployment/deployment-architecture.png]

## 🧪 開發與測試

### 本地開發
```bash
# 後端開發
cd src/GameCore.Api
dotnet run

# 前端開發
cd frontend
pnpm dev

# 執行測試
dotnet test
pnpm test
```

### 測試覆蓋率
```bash
# 後端測試覆蓋率
dotnet test --collect:"XPlat Code Coverage"

# 前端測試覆蓋率
pnpm test:coverage
```

### 程式碼品質
```bash
# 後端程式碼分析
dotnet build --verbosity normal

# 前端程式碼檢查
pnpm lint
pnpm type-check
```

## 🔧 常見錯誤排解

### 1. 資料庫連線問題
**錯誤**：`A network-related or instance-specific error occurred`
**解決方案**：
```bash
# 檢查 SQL Server 容器狀態
docker-compose ps sqlserver

# 檢查連線字串
echo $DefaultConnection

# 重新啟動資料庫服務
docker-compose restart sqlserver
```

### 2. API 啟動失敗
**錯誤**：`Failed to start application`
**解決方案**：
```bash
# 檢查日誌
docker-compose logs api

# 檢查環境變數
docker-compose exec api env | grep -E "(ASPNETCORE|Jwt|DefaultConnection)"

# 檢查端口衝突
netstat -tulpn | grep :5000
```

### 3. 前端建置失敗
**錯誤**：`Build failed with errors`
**解決方案**：
```bash
# 清除快取
rm -rf frontend/node_modules
rm -rf frontend/.pnpm-store

# 重新安裝依賴
cd frontend
pnpm install --force

# 檢查 TypeScript 配置
pnpm type-check
```

### 4. Docker 映像建置失敗
**錯誤**：`Docker build failed`
**解決方案**：
```bash
# 清除 Docker 快取
docker system prune -a

# 重新建置映像
docker-compose build --no-cache

# 檢查 Dockerfile 語法
docker build --target build .
```

## 📋 開發慣例

### Git 提交規範
```bash
# 提交格式
<type>(<scope>): <description>

# 類型說明
feat:     新功能
fix:      錯誤修復
docs:     文件更新
style:    程式碼格式調整
refactor: 重構
test:     測試相關
chore:    建置或輔助工具變更

# 範例
feat(auth): 新增 Google OAuth 登入功能
fix(api): 修復用戶註冊驗證問題
docs(readme): 更新安裝說明
```

### 分支策略
```bash
main          # 生產環境分支
├── develop   # 開發主分支
├── feature/* # 功能開發分支
├── bugfix/*  # 錯誤修復分支
└── hotfix/*  # 緊急修復分支
```

### 程式碼風格
- **C#**：遵循 Microsoft C# 編碼慣例
- **TypeScript**：使用 ESLint + Prettier
- **SQL**：使用 PascalCase 命名，加入中文註解
- **註解**：關鍵邏輯必須加入中文註解

### API 設計規範
- RESTful 設計原則
- 使用 HTTP 狀態碼
- 統一的回應格式
- 版本控制（/api/v1/）
- 完整的 Swagger 文件

## 🚀 CI/CD 流程

### GitHub Actions 工作流程
1. **程式碼檢查**：Lint、Type Check、Build
2. **測試執行**：單元測試、整合測試
3. **品質檢查**：程式碼覆蓋率、安全掃描
4. **建置部署**：Docker 映像建置、部署

### 部署環境
- **開發環境**：自動部署到 develop 分支
- **測試環境**：手動觸發部署
- **生產環境**：手動審核後部署

## 📚 相關文件

- [部署指南](DEPLOYMENT.md)
- [API 文件](docs/api/README.md)
- [資料庫設計](docs/database/README.md)
- [前端組件庫](docs/frontend/components.md)
- [測試指南](docs/testing/README.md)

## 🤝 貢獻指南

### 如何貢獻
1. Fork 專案
2. 建立功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

### 開發環境設定
```bash
# 安裝開發工具
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-format

# 設定 Git Hooks
cp scripts/git-hooks/* .git/hooks/
chmod +x .git/hooks/*
```

## 📄 授權

此專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

## 📞 聯絡資訊

- **專案維護者**：[your-email@example.com]
- **技術支援**：[support@gamecore.com]
- **專案網站**：[https://gamecore.example.com]

## 🙏 致謝

感謝所有為此專案做出貢獻的開發者和社群成員。

---

**注意**：此專案仍在積極開發中，API 和功能可能會有所變更。請查看 [CHANGELOG](CHANGELOG.md) 了解最新變更。
