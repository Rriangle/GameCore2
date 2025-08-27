# GameCore API 文檔

## 概述

GameCore API 是一個功能豐富的遊戲社群平台 API，提供用戶認證、錢包管理、商城交易、玩家市場等核心功能。

## 基礎資訊

- **Base URL**: `https://api.gamecore.com`
- **版本**: v1.0
- **認證**: JWT Bearer Token
- **內容類型**: `application/json`

## 認證

### 獲取 Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "your_username",
  "password": "your_password"
}
```

**回應**:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "user": {
    "userId": 1,
    "username": "your_username",
    "email": "user@example.com"
  }
}
```

### 使用 Token

在請求標頭中加入：
```
Authorization: Bearer your_jwt_token
```

## 用戶認證 API

### 註冊用戶

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "StrongPass123!"
}
```

**密碼要求**:
- 至少 8 個字符
- 包含大小寫字母、數字和特殊字符
- 不能包含常見弱密碼模式

### 登入

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "username",
  "password": "password"
}
```

### 登出

```http
POST /api/auth/logout
Authorization: Bearer your_token
```

## 錢包 API

### 獲取餘額

```http
GET /api/wallet/balance
Authorization: Bearer your_token
```

**回應**:
```json
{
  "success": true,
  "balance": 1500.00,
  "currency": "TWD"
}
```

### 充值

```http
POST /api/wallet/deposit
Authorization: Bearer your_token
Content-Type: application/json

{
  "amount": 1000.00,
  "description": "銀行轉帳充值"
}
```

### 扣款

```http
POST /api/wallet/withdraw
Authorization: Bearer your_token
Content-Type: application/json

{
  "amount": 500.00,
  "description": "購買商品"
}
```

### 轉帳

```http
POST /api/wallet/transfer
Authorization: Bearer your_token
Content-Type: application/json

{
  "toUserId": 2,
  "amount": 100.00,
  "description": "轉帳給朋友"
}
```

### 交易歷史

```http
GET /api/wallet/transactions?page=1&pageSize=20
Authorization: Bearer your_token
```

## 玩家市場 API

### 上架商品

```http
POST /api/market/list
Authorization: Bearer your_token
Content-Type: application/json

{
  "productId": 1,
  "quantity": 5,
  "price": 100.00,
  "description": "全新遊戲道具"
}
```

### 購買商品

```http
POST /api/market/buy
Authorization: Bearer your_token
Content-Type: application/json

{
  "marketItemId": 1,
  "quantity": 2
}
```

### 搜尋商品

```http
GET /api/market/search?productId=1&minPrice=50&maxPrice=200&page=1&pageSize=20
```

### 確認交易

```http
POST /api/market/transactions/{transactionId}/confirm
Authorization: Bearer your_token
```

### 取消商品

```http
POST /api/market/items/{marketItemId}/cancel
Authorization: Bearer your_token
```

### 更新商品

```http
PUT /api/market/items/{marketItemId}
Authorization: Bearer your_token
Content-Type: application/json

{
  "price": 150.00,
  "description": "更新後的商品描述"
}
```

## 官方商城 API

### 獲取商品列表

```http
GET /api/store/products?category=games&page=1&pageSize=20
```

### 獲取商品詳情

```http
GET /api/store/products/{productId}
```

### 加入購物車

```http
POST /api/store/cart/add
Authorization: Bearer your_token
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

### 結帳

```http
POST /api/store/checkout
Authorization: Bearer your_token
Content-Type: application/json

{
  "shippingAddress": {
    "name": "收件人姓名",
    "phone": "0912345678",
    "address": "收件地址"
  },
  "paymentMethod": "credit_card"
}
```

## 錯誤處理

### 錯誤回應格式

```json
{
  "success": false,
  "message": "錯誤訊息",
  "errorCode": "ERROR_CODE",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 常見錯誤碼

- `UNAUTHORIZED`: 未授權訪問
- `INVALID_ARGUMENT`: 無效參數
- `INVALID_OPERATION`: 無效操作
- `NOT_FOUND`: 資源不存在
- `INSUFFICIENT_BALANCE`: 餘額不足
- `RATE_LIMIT_EXCEEDED`: 請求過於頻繁

## 速率限制

- **認證端點**: 每分鐘 10 次
- **一般 API**: 每分鐘 100 次
- **其他端點**: 每分鐘 1000 次

## 狀態碼

- `200`: 成功
- `201`: 創建成功
- `400`: 請求錯誤
- `401`: 未授權
- `403`: 禁止訪問
- `404`: 資源不存在
- `429`: 請求過於頻繁
- `500`: 伺服器錯誤

## 開發環境

### 本地開發

```bash
# 啟動 API 伺服器
dotnet run --project src/GameCore.Api

# 啟動前端開發伺服器
cd frontend
npm run dev
```

### 測試

```bash
# 執行所有測試
dotnet test

# 執行特定測試
dotnet test --filter "FullyQualifiedName~AuthServiceTests"
```

## 部署

### Docker 部署

```bash
# 建置映像
docker build -t gamecore-api .

# 執行容器
docker run -p 5000:5000 gamecore-api
```

### 環境變數

```bash
# 資料庫連線
DATABASE_CONNECTION_STRING="Server=localhost;Database=GameCore;..."

# JWT 設定
JWT_SECRET_KEY="your_secret_key"
JWT_ISSUER="gamecore"
JWT_AUDIENCE="gamecore_users"

# 日誌設定
LOG_LEVEL="Information"
``` 