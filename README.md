# 🎮 GameCore - 遊戲社群平台

GameCore 是一個功能豐富的遊戲社群平台，提供用戶認證、錢包管理、商城交易、玩家市場等核心功能。

## 🚀 功能特色

### 🔐 用戶認證系統
- 安全的用戶註冊和登入
- JWT Token 認證
- 密碼強度驗證
- 登入嘗試限制
- SQL 注入和 XSS 防護

### 💰 錢包管理
- 虛擬貨幣管理
- 充值、扣款、轉帳功能
- 交易歷史記錄
- 餘額快取機制
- 頻率限制保護

### 🛒 官方商城
- 商品展示和搜尋
- 購物車功能
- 折扣計算
- 響應式設計
- 微互動效果

### 🏪 玩家市場
- 玩家間商品交易
- 價格合理性檢查
- 手續費計算
- 交易確認機制
- 上架頻率限制

## 🏗️ 技術架構

### 後端技術棧
- **.NET 8** - 核心框架
- **Entity Framework Core** - ORM
- **SQL Server** - 資料庫
- **JWT** - 認證
- **Serilog** - 日誌記錄
- **Moq** - 單元測試

### 前端技術棧
- **Vue 3** - 前端框架
- **TypeScript** - 類型安全
- **Tailwind CSS** - 樣式框架
- **Vite** - 建置工具
- **Pinia** - 狀態管理

## 📦 專案結構

```
GameCore/
├── src/
│   ├── GameCore.Api/          # Web API 專案
│   ├── GameCore.Domain/       # 領域模型
│   ├── GameCore.Infrastructure/ # 基礎設施
│   └── GameCore.Shared/       # 共享模型
├── frontend/                  # Vue.js 前端
├── tests/                     # 測試專案
├── docs/                      # 文檔
└── scripts/                   # 部署腳本
```

## 🚀 快速開始

### 環境需求
- .NET 8 SDK
- Node.js 18+
- SQL Server 2019+
- Docker (可選)

### 1. 克隆專案
```bash
git clone https://github.com/your-username/GameCore.git
cd GameCore
```

### 2. 後端設置
```bash
# 還原 NuGet 套件
dotnet restore

# 更新資料庫
dotnet ef database update --project src/GameCore.Infrastructure --startup-project src/GameCore.Api

# 啟動 API 伺服器
dotnet run --project src/GameCore.Api
```

### 3. 前端設置
```bash
cd frontend

# 安裝依賴
npm install

# 啟動開發伺服器
npm run dev
```

### 4. 環境變數配置
複製 `env.sample` 到 `env.development` 並修改配置：

```bash
# 資料庫連線
DATABASE_CONNECTION_STRING="Server=localhost;Database=GameCore;..."

# JWT 設定
JWT_SECRET_KEY="your_secret_key_here"
JWT_ISSUER="gamecore"
JWT_AUDIENCE="gamecore_users"

# 日誌設定
LOG_LEVEL="Information"
```

## 🧪 測試

### 執行所有測試
```bash
dotnet test
```

### 執行特定測試
```bash
# 認證測試
dotnet test --filter "FullyQualifiedName~AuthServiceTests"

# 錢包測試
dotnet test --filter "FullyQualifiedName~WalletServiceTests"

# 市場測試
dotnet test --filter "FullyQualifiedName~MarketServiceTests"
```

### 前端測試
```bash
cd frontend
npm run test
```

## 🐳 Docker 部署

### 建置映像
```bash
# 後端映像
docker build -t gamecore-api -f src/GameCore.Api/Dockerfile .

# 前端映像
docker build -t gamecore-frontend -f frontend/Dockerfile.dev frontend/
```

### 使用 Docker Compose
```bash
docker-compose up -d
```

## 📚 API 文檔

完整的 API 文檔請參考 [docs/api-documentation.md](docs/api-documentation.md)

### 主要端點
- `POST /api/auth/register` - 用戶註冊
- `POST /api/auth/login` - 用戶登入
- `GET /api/wallet/balance` - 獲取餘額
- `POST /api/wallet/deposit` - 充值
- `POST /api/market/list` - 上架商品
- `POST /api/market/buy` - 購買商品

## 🔧 開發指南

### 代碼規範
- 使用 C# 命名規範
- 遵循 SOLID 原則
- 添加適當的註釋
- 編寫單元測試

### 提交規範
```
feat: 新功能
fix: 錯誤修復
docs: 文檔更新
style: 代碼格式
refactor: 重構
test: 測試
chore: 構建過程或輔助工具的變動
```

### 分支策略
- `main` - 主分支
- `develop` - 開發分支
- `feature/*` - 功能分支
- `hotfix/*` - 熱修復分支

## 🚀 部署

### Azure 部署
```bash
# 使用 Azure CLI
az webapp up --name gamecore-api --resource-group gamecore-rg --runtime "DOTNETCORE:8.0"

# 使用 Azure DevOps
az pipelines run --name azure-pipelines.yml
```

### 本地部署
```bash
# 發布應用程式
dotnet publish -c Release -o ./publish

# 啟動服務
dotnet ./publish/GameCore.Api.dll
```

## 📊 監控和日誌

### 日誌配置
- 使用 Serilog 進行結構化日誌記錄
- 日誌檔案位於 `logs/` 目錄
- 支援日誌輪轉

### 健康檢查
```bash
# 檢查 API 健康狀態
curl http://localhost:5000/health
```

## 🔒 安全性

### 已實作的安全措施
- JWT Token 認證
- 密碼雜湊加密
- SQL 注入防護
- XSS 攻擊防護
- 速率限制
- 輸入驗證

### 安全最佳實踐
- 定期更新依賴套件
- 使用 HTTPS
- 實作 CORS 政策
- 監控異常活動

## 🤝 貢獻指南

1. Fork 專案
2. 創建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

## 📄 授權

本專案採用 MIT 授權 - 詳見 [LICENSE](LICENSE) 檔案

## 📞 支援

- 📧 Email: support@gamecore.com
- 💬 Discord: [GameCore Community](https://discord.gg/gamecore)
- 📖 文檔: [docs/](docs/)

## 🎯 路線圖

### v1.1.0 (計劃中)
- [ ] 論壇系統
- [ ] 社交功能
- [ ] 虛擬寵物系統
- [ ] 成就系統

### v1.2.0 (計劃中)
- [ ] 即時聊天
- [ ] 語音通話
- [ ] 遊戲整合
- [ ] 移動端應用

---

**GameCore** - 打造最棒的遊戲社群平台！🎮✨
