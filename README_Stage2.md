# GameCore Stage 2 - 錢包系統、銷售功能與官方商店

## 概述
Stage 2 實現了 GameCore 平台的核心商業功能，包括錢包系統、銷售功能和官方商店，為使用者提供完整的點數管理、銷售統計和商品購買體驗。

## 功能特性
- ✅ 錢包系統（點數管理、交易記錄、轉帳功能）
- ✅ 銷售功能（銷售資料、統計資訊、排行榜、佣金計算）
- ✅ 官方商店（商品管理、訂單處理、購物車、庫存管理）
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

### 錢包系統 (`/api/wallet`)
- `GET /api/wallet` - 取得使用者錢包資訊
- `POST /api/wallet/add-points` - 增加點數
- `POST /api/wallet/deduct-points` - 扣除點數
- `POST /api/wallet/transfer` - 轉帳給其他使用者
- `GET /api/wallet/transactions` - 取得交易歷史

### 銷售功能 (`/api/sales`)
- `GET /api/sales/profile` - 取得使用者銷售資料
- `PUT /api/sales/profile` - 更新銷售資料
- `GET /api/sales/statistics` - 取得銷售統計資訊
- `GET /api/sales/ranking` - 取得銷售排行榜
- `GET /api/sales/commission` - 計算佣金

### 官方商店 (`/api/store`)
- `GET /api/store/products` - 取得商品列表
- `POST /api/store/products/{id}` - 取得商品詳細資訊
- `POST /api/store/orders` - 建立訂單
- `GET /api/store/orders` - 取得使用者訂單列表
- `GET /api/store/categories` - 取得商品類別列表
- `GET /api/store/popular` - 取得熱門商品

## 測試範例

### 錢包操作
```bash
# 增加點數
POST /api/wallet/add-points
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "points": 100,
  "reason": "每日簽到獎勵"
}

# 轉帳給其他使用者
POST /api/wallet/transfer
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "toUserId": 2,
  "points": 50,
  "reason": "朋友分享"
}
```

### 銷售統計
```bash
# 取得銷售統計
GET /api/sales/statistics
Authorization: Bearer <jwt_token>

# 取得銷售排行榜
GET /api/sales/ranking?top=10
```

### 商店購物
```bash
# 瀏覽商品
GET /api/store/products?category=遊戲&page=1&pageSize=20

# 建立訂單
POST /api/store/orders
Authorization: Bearer <jwt_token>
Content-Type: application/json

{
  "items": [
    {
      "productId": 1,
      "quantity": 2
    }
  ]
}
```

## 資料庫結構
Stage 2 包含以下核心資料表：
- `User_Wallet` - 使用者錢包
- `User_Wallet_Transaction` - 錢包交易記錄
- `Member_Sales_Profile` - 會員銷售資料
- `User_Sales_Information` - 使用者銷售資訊
- `Product_Info` - 商品資訊
- `Order_Info` - 訂單資訊
- `Order_Item` - 訂單項目
- `Supplier` - 供應商

## 測試覆蓋
- **錢包服務測試**: 點數操作、轉帳、交易歷史
- **銷售服務測試**: 銷售資料、統計、排行榜、佣金計算
- **商店服務測試**: 商品管理、訂單處理、庫存管理

## 下一步
Stage 3 將實現：
- 玩家市場（二手交易）
- 遊戲排行榜與熱門度分析
- 論壇系統（主題、回覆、反應）

## 注意事項
- 所有時間戳記使用 UTC 時間
- 點數操作需要 JWT 認證
- 商品庫存會自動更新
- 銷售統計即時計算
- 轉帳操作支援回滾機制 