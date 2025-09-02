# GameCore Stage 3 - 玩家市場、排行榜與論壇系統

## 概述
Stage 3 實現了 GameCore 平台的核心社群功能，包括玩家市場（C2C 二手交易）、遊戲排行榜與熱門度分析、以及完整的論壇系統，為使用者提供豐富的互動和交易體驗。

## 功能特性
- ✅ 玩家市場（商品上架、購買、訂單管理、排行榜）
- ✅ 遊戲排行榜（熱門度指數、分類排行、趨勢分析）
- ✅ 論壇系統（主題、回覆、反應、書籤、搜尋）
- ✅ 完整的 API 端點和控制器
- ✅ 詳細的單元測試覆蓋
- ✅ 三層式架構設計

## 技術架構
- **後端**: ASP.NET Core 8.0 Web API
- **資料庫**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 8.0
- **認證**: JWT Bearer Token
- **架構**: 三層式架構 (API, Domain, Infrastructure)
- **測試**: xUnit + Moq

## 快速開始

### 前置需求
- .NET 8.0 SDK
- SQL Server LocalDB
- PowerShell 或命令提示字元

### 1. 建置專案
```bash
cd src/GameCore.Api
dotnet build
```

### 2. 運行測試
```bash
dotnet test
```

### 3. 啟動應用程式
```bash
dotnet run --urls "http://localhost:5000"
```

## API 端點

### 玩家市場 (`/api/market`)
- `GET /api/market/products` - 取得市場商品列表
- `GET /api/market/products/{id}` - 取得商品詳細資訊
- `POST /api/market/orders` - 建立市場訂單
- `GET /api/market/orders` - 取得使用者訂單列表
- `GET /api/market/ranking` - 取得市場排行榜
- `GET /api/market/categories` - 取得商品類別列表
- `GET /api/market/conditions` - 取得商品狀態列表

### 遊戲排行榜 (`/api/leaderboard`)
- `GET /api/leaderboard` - 取得遊戲排行榜
- `GET /api/leaderboard/games/{id}/metrics` - 取得遊戲熱門度指標
- `GET /api/leaderboard/games/{id}/trend` - 取得遊戲熱門度趨勢
- `GET /api/leaderboard/categories/{category}` - 取得分類排行榜
- `GET /api/leaderboard/snapshots` - 取得熱門遊戲快照
- `GET /api/leaderboard/categories` - 取得遊戲分類列表
- `GET /api/leaderboard/metrics` - 取得指標代碼列表
- `POST /api/leaderboard/games/{id}/update-popularity` - 更新遊戲熱門度指數

### 論壇系統 (`/api/forum`)
- `GET /api/forum/categories` - 取得論壇分類列表
- `GET /api/forum/threads` - 取得論壇主題列表
- `POST /api/forum/threads` - 建立新主題
- `GET /api/forum/threads/{id}` - 取得主題詳細資訊
- `POST /api/forum/posts` - 新增回覆
- `GET /api/forum/posts/{id}` - 取得回覆詳細資訊
- `POST /api/forum/reactions` - 新增反應
- `POST /api/forum/bookmarks` - 新增書籤
- `GET /api/forum/bookmarks` - 取得使用者書籤列表
- `GET /api/forum/stats/{id}` - 取得論壇統計資訊
- `GET /api/forum/search` - 搜尋論壇內容
- `GET /api/forum/hot-threads` - 取得熱門主題

## 測試範例

### 玩家市場操作
```bash
# 瀏覽市場商品
GET /api/market/products?category=遊戲&searchTerm=RPG&page=1&pageSize=20

# 建立訂單
POST /api/market/orders
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "productId": 1,
  "quantity": 1
}

# 查看市場排行榜
GET /api/market/ranking?top=10
```

### 遊戲排行榜查詢
```bash
# 取得遊戲排行榜
GET /api/leaderboard?category=動作遊戲&period=daily&top=20&sortBy=popularity&sortOrder=desc

# 查看遊戲趨勢
GET /api/leaderboard/games/1/trend?days=7&metricCode=concurrent_users

# 取得分類排行榜
GET /api/leaderboard/categories/動作遊戲?top=10
```

### 論壇互動
```bash
# 建立新主題
POST /api/forum/threads
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "forumId": 1,
  "title": "新主題標題",
  "content": "這是主題內容",
  "tags": ["標籤1", "標籤2"]
}

# 新增回覆
POST /api/forum/posts
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "threadId": 1,
  "content": "這是回覆內容",
  "contentType": "text"
}

# 新增反應
POST /api/forum/reactions
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "postId": 1,
  "reactionType": "like"
}

# 搜尋論壇內容
GET /api/forum/search?searchTerm=遊戲&searchType=all&page=1&pageSize=20
```

## 資料庫結構
Stage 3 包含以下核心資料表：
- `Player_Market_Product_Info` - 玩家市場商品資訊
- `Player_Market_Product_Img` - 玩家市場商品圖片
- `Player_Market_Order_Info` - 玩家市場訂單資訊
- `Game` - 遊戲資訊
- `Metric` - 指標定義
- `Game_Metric_Daily` - 遊戲每日指標
- `Popularity_Index_Daily` - 遊戲熱門度指數
- `Forum` - 論壇分類
- `Forum_Thread` - 論壇主題
- `Thread_Post` - 論壇回覆
- `Reaction` - 回覆反應
- `Bookmark` - 書籤

## 測試覆蓋
- **玩家市場服務測試**: 商品查詢、訂單建立、排行榜等 8 個測試案例
- **排行榜服務測試**: 排行榜生成、指標查詢、趨勢分析等 10 個測試案例
- **論壇服務測試**: 主題管理、回覆系統、反應書籤等 12 個測試案例

## 核心功能說明

### 玩家市場系統
- **商品管理**: 支援商品上架、下架、編輯、搜尋
- **訂單處理**: 完整的購買流程，包含庫存檢查和點數扣除
- **平台抽成**: 自動計算平台手續費（5%）
- **排行榜系統**: 基於銷售額和使用者評分的賣家排行

### 遊戲排行榜系統
- **熱門度指數**: 綜合同時在線人數、論壇發文、市場上架等指標
- **分類排行**: 支援按遊戲分類的排行榜
- **趨勢分析**: 提供指定天數的指標變化趨勢
- **即時更新**: 支援手動觸發熱門度指數更新

### 論壇系統
- **主題管理**: 支援建立、編輯、置頂、鎖定主題
- **回覆系統**: 多層級回覆，支援分頁顯示
- **反應系統**: 多種反應類型（讚、踩、愛心等）
- **書籤功能**: 個人化的內容收藏
- **搜尋功能**: 全文搜尋主題和回覆內容

## 下一步
Stage 4 將實現：
- 社交功能（通知、私訊、群組）
- 每日簽到系統
- 虛擬寵物（史萊姆）系統

## 注意事項
- 所有時間戳記使用 UTC 時間
- 市場交易需要 JWT 認證
- 論壇發文支援多種內容類型
- 排行榜資料支援多種排序方式
- 熱門度指數可配置計算權重 