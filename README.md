# GameCore 遊戲社群平台

## 📖 專案概述

GameCore 是一個整合遊戲熱度觀測、論壇社群、官方商城、玩家自由市場（C2C）、寵物養成與小遊戲、即時訊息/群組與通知的完整平台。

### 🎯 主要特色

- **🎮 遊戲熱度觀測** - 即時追蹤遊戲熱度與排行榜
- **💬 論壇社群** - 完整的討論區與互動系統
- **🛒 官方商城** - B2C 遊戲商品購買平台
- **🏪 玩家市集** - C2C 自由交易市場
- **🐾 虛擬寵物** - 互動式史萊姆養成系統
- **🎯 小遊戲** - 冒險遊戲與獎勵系統
- **📱 即時通訊** - 群組聊天與私訊功能
- **🔔 通知系統** - 完整的通知與提醒機制

### 🏗️ 技術架構

- **後端**: ASP.NET Core 8.0 + Entity Framework Core
- **前端**: HTML5 + CSS3 + JavaScript (ES6+)
- **資料庫**: SQL Server
- **認證**: JWT Token
- **設計**: Glass-morphism UI 設計

## 🚀 快速開始

### 前置需求

- .NET 8.0 SDK
- SQL Server 2019 或更新版本
- Visual Studio 2022 或 VS Code
- Node.js 16+ (可選，用於前端開發)

### 安裝步驟

1. **克隆專案**
   ```bash
   git clone https://github.com/your-username/gamecore.git
   cd gamecore
   ```

2. **還原 NuGet 套件**
   ```bash
   dotnet restore
   ```

3. **設定資料庫連接**
   - 編輯 `src/GameCore.Api/appsettings.json`
   - 修改 `ConnectionStrings` 中的資料庫連接字串

4. **執行資料庫遷移**
   ```bash
   cd src/GameCore.Api
   dotnet ef database update
   ```

5. **執行專案**
   ```bash
   dotnet run
   ```

6. **開啟瀏覽器**
   - 前往 `https://localhost:7001` (API)
   - 前往 `https://localhost:7002` (前端)

## 📁 專案結構

```
GameCore/
├── src/
│   ├── GameCore.Api/              # API 專案
│   │   ├── Controllers/           # API 控制器
│   │   ├── Services/              # 業務邏輯服務
│   │   ├── Middleware/            # 中間件
│   │   └── Program.cs             # 應用程式入口
│   ├── GameCore.Domain/           # 領域層
│   │   ├── Entities/              # 實體模型
│   │   └── Interfaces/            # 介面定義
│   ├── GameCore.Infrastructure/   # 基礎設施層
│   │   ├── Data/                  # 資料庫上下文
│   │   └── Repositories/          # 資料存取實作
│   └── GameCore.Shared/           # 共用層
│       ├── DTOs/                  # 資料傳輸物件
│       └── Interfaces/            # 共用介面
├── wwwroot/                       # 前端檔案
│   ├── css/                       # 樣式表
│   ├── js/                        # JavaScript 檔案
│   └── images/                    # 圖片資源
├── docs/                          # 文件
└── tests/                         # 測試專案
```

## 🎨 前端架構

### 設計系統

- **Glass-morphism 設計** - 現代化的玻璃擬態效果
- **響應式設計** - 支援桌面、平板、手機
- **深色模式** - 可切換的深色主題
- **自訂主題** - 多種主色調選擇

### 主要組件

- **虛擬寵物系統** - Canvas 繪製的互動式史萊姆
- **論壇系統** - 完整的文章管理與互動
- **市集系統** - 商品展示與交易介面
- **排行榜系統** - 即時更新的遊戲熱度排行

## 🔧 API 端點

### 認證相關
- `POST /api/auth/register` - 用戶註冊
- `POST /api/auth/login` - 用戶登入
- `GET /api/auth/profile` - 取得用戶資料
- `PUT /api/auth/profile` - 更新用戶資料

### 論壇相關
- `GET /api/forums` - 取得論壇列表
- `GET /api/forums/{id}/threads` - 取得主題列表
- `POST /api/forums/{id}/threads` - 建立新主題
- `POST /api/threads/{id}/posts` - 回覆主題

### 市集相關
- `GET /api/market/products` - 取得商品列表
- `GET /api/market/products/{id}` - 取得商品詳情
- `POST /api/market/orders` - 建立訂單
- `GET /api/market/orders` - 取得訂單列表

### 寵物相關
- `GET /api/pet` - 取得寵物資料
- `POST /api/pet/actions/{action}` - 執行寵物動作
- `POST /api/pet/adventure` - 開始冒險
- `GET /api/pet/stats` - 取得寵物統計

### 簽到相關
- `GET /api/signin/status` - 取得簽到狀態
- `POST /api/signin` - 執行每日簽到
- `GET /api/signin/history` - 取得簽到歷史

## 🗄️ 資料庫結構

### 主要資料表

- **Users** - 用戶基本資料
- **User_Introduce** - 用戶詳細資料
- **User_Rights** - 用戶權限
- **User_wallet** - 用戶錢包
- **forums** - 論壇版區
- **threads** - 討論主題
- **thread_posts** - 主題回覆
- **products** - 商品資料
- **orders** - 訂單資料
- **pets** - 寵物資料
- **signin_stats** - 簽到統計

## 🎮 功能模組

### 1. 認證系統
- JWT Token 認證
- 角色權限管理
- 用戶資料管理

### 2. 論壇系統
- 多版區管理
- 主題與回覆
- 讚與收藏功能
- 搜尋與篩選

### 3. 市集系統
- 商品上架管理
- 訂單處理
- 交易記錄
- 評價系統

### 4. 虛擬寵物
- 史萊姆養成
- 互動系統
- 等級與經驗
- 冒險遊戲

### 5. 簽到系統
- 每日簽到
- 連續簽到獎勵
- 簽到統計

### 6. 通知系統
- 即時通知
- 通知分類
- 已讀狀態

## 🧪 測試

### 執行測試
```bash
dotnet test
```

### 測試覆蓋率
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 效能優化

### 資料庫優化
- 索引優化
- 查詢效能調校
- 連線池管理

### 前端優化
- 圖片懶載入
- 程式碼分割
- 快取策略

### API 優化
- 回應快取
- 分頁處理
- 非同步處理

## 🔒 安全性

### 認證安全
- JWT Token 加密
- 密碼雜湊
- 權限驗證

### 資料安全
- SQL 注入防護
- XSS 防護
- CSRF 防護

### API 安全
- 請求限制
- 輸入驗證
- 錯誤處理

## 🚀 部署

### Docker 部署
```bash
# 建立 Docker 映像
docker build -t gamecore .

# 執行容器
docker run -p 8080:80 gamecore
```

### Azure 部署
```bash
# 發布到 Azure
dotnet publish -c Release
az webapp deploy --resource-group myResourceGroup --name myApp --src-path ./bin/Release/net8.0/publish
```

### AWS 部署
```bash
# 發布到 AWS
dotnet publish -c Release
aws deploy create-deployment --application-name gamecore --deployment-group-name production --s3-location bucket=my-bucket,key=gamecore.zip
```

## 📈 監控與日誌

### 應用程式監控
- 效能監控
- 錯誤追蹤
- 使用者行為分析

### 日誌管理
- 結構化日誌
- 日誌等級
- 日誌輪替

## 🤝 貢獻指南

### 開發流程
1. Fork 專案
2. 建立功能分支
3. 提交變更
4. 建立 Pull Request

### 程式碼規範
- 遵循 C# 編碼規範
- 使用有意義的變數名稱
- 添加適當的註解
- 撰寫單元測試

## 📄 授權

本專案採用 MIT 授權條款。詳見 [LICENSE](LICENSE) 檔案。

## 📞 聯絡資訊

- **專案維護者**: GameCore 開發團隊
- **Email**: support@gamecore.com
- **GitHub**: https://github.com/gamecore
- **文件**: https://docs.gamecore.com

## 🙏 致謝

感謝所有為這個專案做出貢獻的開發者與使用者。

---

**GameCore** - 讓遊戲社群更美好 🎮✨
