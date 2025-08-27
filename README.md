# GameCore 專案 - Stage 2 實作

## 概述

GameCore 是一個集結合遊戲熱度觀測、論壇社群、官方商城、玩家自由市場、寵物養成與小遊戲、即時訊息等功能的整合平台。

本文件記錄 Stage 2 的實作內容，擴展了錢包系統與銷售功能，建構在 Stage 1 的身份驗證與用戶管理系統之上。

## Stage 1 - 核心身份驗證與用戶系統 ✅ 已完成

## Stage 2 - 錢包系統與銷售功能

### 已實作功能

#### 1. 錢包系統 (Wallet System)
- ✅ **錢包餘額查詢** - 取得用戶點數和優惠券資訊
- ✅ **點數交易管理** - 增加/扣除點數操作
- ✅ **交易明細查詢** - 分頁式交易記錄 (多來源彙整架構)
- ✅ **餘額檢查** - 驗證用戶點數是否足夠
- ✅ **管理者點數調整** - 後台點數增減功能
- ✅ **銀行代號服務** - 台灣主要銀行資訊查詢

#### 2. 銷售功能 (Sales System)
- ✅ **銷售資料申請** - 用戶申請銷售功能
- ✅ **銀行帳號管理** - 銀行代號與帳號驗證
- ✅ **帳戶封面照片** - Base64 圖片上傳處理
- ✅ **銷售權限控制** - 基於 User_Rights.SalesAuthority
- ✅ **銷售錢包系統** - 獨立的銷售收入管理
- ✅ **申請審核機制** - 管理者核准/拒絕銷售申請
- ✅ **銀行帳號唯一性** - 防止重複銀行帳號註冊

#### 3. 資料存取層 (Data Access)
- ✅ **MemberSalesProfileRepository** - 銷售資料倉儲
- ✅ **UserSalesInformationRepository** - 銷售錢包倉儲
- ✅ **交易安全性** - 餘額不足保護機制
- ✅ **資料一致性** - 原子性操作保證

#### 4. API 端點 (REST Endpoints)
- ✅ **WalletController** - 錢包相關 API
  - `GET /api/wallet/balance` - 查詢錢包餘額
  - `GET /api/wallet/transactions` - 查詢交易明細
  - `GET /api/wallet/check-balance/{amount}` - 檢查餘額
  - `GET /api/wallet/bank-codes` - 取得銀行代號
  - `POST /api/wallet/admin/adjust-points` - 管理者調整點數
- ✅ **SalesController** - 銷售相關 API
  - `POST /api/sales/apply` - 申請銷售功能
  - `GET /api/sales/profile` - 查詢銷售資料
  - `PUT /api/sales/profile` - 更新銷售資料
  - `GET /api/sales/wallet` - 查詢銷售錢包
  - `GET /api/sales/authority` - 檢查銷售權限
  - `GET /api/sales/admin/pending-applications` - 待審核申請
  - `POST /api/sales/admin/review/{userId}` - 審核銷售申請

#### 5. 測試與品質保證
- ✅ **WalletServiceTests** - 錢包服務單元測試 (19 個測試案例)
- ✅ **SalesServiceTests** - 銷售服務單元測試 (19 個測試案例)
- ✅ **Mock 物件架構** - 完整的測試隔離
- ✅ **邊界條件測試** - 餘額不足、重複申請等情境
- ✅ **業務邏輯驗證** - 權限檢查、資料驗證

## 專案結構

```
GameCore/
├── src/
│   ├── GameCore.Domain/          # 領域層
│   │   ├── Entities/             # 實體類別
│   │   └── Interfaces/           # 服務介面
│   ├── GameCore.Infrastructure/  # 基礎設施層
│   │   ├── Data/                 # 資料存取
│   │   ├── Repositories/         # 資料倉庫
│   │   └── Services/             # 服務實作
│   ├── GameCore.Shared/          # 共用層
│   │   └── DTOs/                 # 資料傳輸物件
│   └── GameCore.Mvc/             # 展示層 (API)
├── GameCore.Tests/               # 單元測試
└── README.md                     # 說明文件
```

## 資料庫架構

### 核心表格 (Stage 1)

| 表格名稱 | 說明 | 狀態 |
|---------|------|------|
| Users | 使用者基本資料 | ✅ 已實作 |
| User_Introduce | 使用者詳細介紹 | ✅ 已實作 |
| User_Rights | 使用者權限 | ✅ 已實作 |
| User_wallet | 使用者錢包 | ✅ 已實作 |
| MemberSalesProfile | 銷售檔案 | ✅ 已實作 |
| User_Sales_Information | 銷售錢包 | ✅ 已實作 |
| Pet | 寵物狀態 (基礎) | ✅ 基礎實作 |

## 快速開始

### 系統需求

- .NET 8.0 SDK
- SQL Server LocalDB (開發用)
- Visual Studio 2022 或 VS Code

### 安裝與執行

1. **複製專案**
```bash
git clone <repository-url>
cd GameCore
```

2. **還原套件**
```bash
dotnet restore
```

3. **建置專案**
```bash
dotnet build
```

4. **執行測試**
```bash
dotnet test
```

5. **啟動 API 服務**
```bash
cd src/GameCore.Mvc
dotnet run
```

6. **瀏覽 Swagger UI**
開啟瀏覽器前往：`https://localhost:7000/swagger`

## API 端點

### 身份驗證 (/api/auth)

| 方法 | 端點 | 說明 |
|------|------|------|
| POST | `/api/auth/register` | 用戶註冊 |
| POST | `/api/auth/login` | 用戶登入 |
| POST | `/api/auth/oauth` | OAuth 登入 |
| POST | `/api/auth/refresh` | 重新整理權杖 |
| POST | `/api/auth/logout` | 登出 |

### 用戶管理 (/api/users)

| 方法 | 端點 | 說明 |
|------|------|------|
| GET | `/api/users/me` | 取得個人資料 |
| PUT | `/api/users/me` | 更新個人資料 |
| POST | `/api/users/change-password` | 修改密碼 |
| POST | `/api/users/upload-avatar` | 上傳頭像 |

### 錢包系統 (/api/wallet)

| 方法 | 端點 | 說明 |
|------|------|------|
| GET | `/api/wallet/balance` | 查詢錢包餘額 |
| GET | `/api/wallet/transactions` | 查詢交易明細 (分頁) |
| GET | `/api/wallet/check-balance/{amount}` | 檢查餘額是否足夠 |
| GET | `/api/wallet/bank-codes` | 取得銀行代號清單 |
| POST | `/api/wallet/admin/adjust-points` | 管理者調整點數 |

### 銷售系統 (/api/sales)

| 方法 | 端點 | 說明 |
|------|------|------|
| POST | `/api/sales/apply` | 申請銷售功能 |
| GET | `/api/sales/profile` | 查詢銷售資料 |
| PUT | `/api/sales/profile` | 更新銷售資料 |
| GET | `/api/sales/wallet` | 查詢銷售錢包 |
| GET | `/api/sales/authority` | 檢查銷售權限 |
| GET | `/api/sales/check-balance/{amount}` | 檢查銷售錢包餘額 |
| GET | `/api/sales/admin/pending-applications` | 取得待審核申請 |
| POST | `/api/sales/admin/review/{userId}` | 審核銷售申請 |

## 測試資料

系統會自動建立以下測試帳號：

| 帳號 | 密碼 | 說明 | 初始點數 | 銷售權限 | 銷售錢包 |
|------|------|------|----------|----------|----------|
| admin@gamecore.com | admin123 | 管理員帳號 | 10,000 | ✅ | 5,000 |
| user1@gamecore.com | user123 | 測試用戶1 | 1,100 | ✅ | 2,500 |
| user2@gamecore.com | user123 | 測試用戶2 | 1,200 | ❌ | - |
| demo@gamecore.com | demo123 | 展示帳號 | 1,300 | ❌ | - |

### 銀行資料範例

已建立的銷售用戶銀行帳號：
- **管理員**: 中國信託銀行 (822) - 1234567890123
- **用戶1**: 中華郵政 (700) - 9876543210987

### 支援的銀行代號

| 銀行代號 | 銀行名稱 |
|----------|----------|
| 822 | 中國信託 |
| 700 | 中華郵政 |
| 808 | 玉山銀行 |
| 812 | 台新銀行 |
| 803 | 聯邦銀行 |

## 範例 API 呼叫

### 用戶註冊
```bash
curl -X POST "https://localhost:7000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "user_name": "新用戶",
    "user_Account": "newuser@example.com",
    "user_Password": "password123",
    "user_NickName": "NewUser",
    "gender": "M",
    "idNumber": "A123456789",
    "cellphone": "0912345678",
    "email": "newuser@example.com",
    "address": "台北市",
    "dateOfBirth": "1990-01-01"
  }'
```

### 用戶登入
```bash
curl -X POST "https://localhost:7000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "user_Account": "admin@gamecore.com",
    "user_Password": "admin123"
  }'
```

### 取得個人資料 (需要 JWT)
```bash
curl -X GET "https://localhost:7000/api/users/me" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 查詢錢包餘額 (需要 JWT)
```bash
curl -X GET "https://localhost:7000/api/wallet/balance" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 申請銷售功能 (需要 JWT)
```bash
curl -X POST "https://localhost:7000/api/sales/apply" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "bankCode": 822,
    "bankAccountNumber": "1234567890123",
    "accountCoverPhotoBase64": null
  }'
```

### 管理者調整用戶點數 (需要 JWT)
```bash
curl -X POST "https://localhost:7000/api/wallet/admin/adjust-points" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "userId": 2,
    "delta": 500,
    "reason": "活動獎勵"
  }'
```

## 技術特色

### 安全性
- **密碼雜湊**: SHA256 + 固定鹽值
- **JWT 驗證**: HS256 演算法
- **權限控制**: 角色基礎存取控制
- **交易安全**: 餘額檢查與原子性操作
- **銀行帳號驗證**: 唯一性檢查與格式驗證

### 效能
- **非同步程式設計**: 全面使用 async/await
- **連線管理**: Entity Framework Core 最佳化
- **分頁查詢**: 交易記錄支援分頁避免效能問題
- **快取準備**: 架構支援未來快取實作

### 可維護性
- **SOLID 原則**: 依賴反轉、單一職責
- **介面分離**: 清楚的抽象層
- **測試友善**: 高度可測試的設計
- **錯誤處理**: 完整的例外處理與日誌記錄

### 資料一致性
- **交易保護**: 避免重複扣款或餘額不足
- **狀態管理**: 銷售申請審核流程控制
- **外鍵約束**: 確保資料關聯完整性

## 下一階段規劃

### Stage 3 - 官方商城系統
- 官方商城 B2C 功能
- 玩家市集 C2C 功能
- 訂單管理與支付整合

### Stage 4 - 論壇與社群
- 遊戲論壇系統
- 互動功能 (按讚、收藏)
- 通知系統

## 程式碼品質

### 測試覆蓋率
- AuthService: 100% 方法覆蓋
- 關鍵業務邏輯: 單元測試完整

### 程式碼標準
- 中文註解與文件
- 一致的命名規範
- 完整的錯誤處理

## 疑難排解

### 常見問題

1. **資料庫連線問題**
   - 確認 SQL Server LocalDB 已安裝
   - 檢查 `appsettings.json` 中的連線字串

2. **JWT 驗證失敗**
   - 檢查權杖是否已過期
   - 確認 Bearer 格式正確

3. **測試失敗**
   - 清理 bin 和 obj 資料夾
   - 重新建置專案

### 聯絡資訊
如有問題請查閱專案文件或提交 Issue。

---

**GameCore Stage 1 實作完成日期**: 2024年12月
**技術棧**: .NET 8, Entity Framework Core, JWT, xUnit, Moq
