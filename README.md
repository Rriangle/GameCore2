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

## 🚀 快速開始

### 必要條件

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server LocalDB](https://docs.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb)
- [PowerShell 5.1+](https://docs.microsoft.com/powershell/scripting/install/installing-powershell)

### 安裝步驟

1. **複製專案**
   ```bash
   git clone <repository-url>
   cd GameCore
   ```

2. **安裝前端套件**
   ```bash
   cd frontend
   pnpm install
   cd ..
   ```

3. **還原 .NET 套件**
   ```bash
   dotnet restore
   ```

4. **啟動開發環境**
   ```bash
   .\scripts\dev.ps1
   ```

### 驗證安裝

啟動完成後，您應該能夠訪問：

- 🌐 **前端應用**：http://localhost:3000
- 📊 **後端 API**：http://localhost:5000
- 📚 **API 文件**：http://localhost:5000/api-docs
- 💚 **健康檢查**：http://localhost:5000/health

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

專案使用 SQL Server LocalDB 作為開發環境資料庫：

- **連線字串**：`Server=(localdb)\mssqllocaldb;Database=GameCore;Trusted_Connection=true;MultipleActiveResultSets=true`
- **自動建立**：應用程式啟動時會自動建立資料庫
- **假資料**：後續會提供假資料腳本

## 🔐 環境變數

重要設定請在 `appsettings.json` 中配置：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "GameCore",
    "Audience": "GameCoreUsers"
  }
}
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
