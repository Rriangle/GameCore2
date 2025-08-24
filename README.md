# GameCore 專案 - Stage 1 實作

## 概述

GameCore 是一個集結合遊戲熱度觀測、論壇社群、官方商城、玩家自由市場、寵物養成與小遊戲、即時訊息等功能的整合平台。

本文件記錄 Stage 1 的實作內容，主要專注於核心身份驗證與用戶管理系統。

## Stage 1 - 核心身份驗證與用戶系統

### 已實作功能

#### 1. 用戶身份驗證系統
- ✅ **用戶註冊** - 支援完整的用戶資料註冊
- ✅ **用戶登入** - 密碼雜湊驗證
- ✅ **JWT 權杖系統** - 產生和驗證存取權杖
- ✅ **OAuth 預備架構** - 支援 Google、Facebook、Discord 登入架構
- ✅ **密碼安全** - SHA256 雜湊加鹽處理

#### 2. 用戶資料管理
- ✅ **基本用戶資料** (Users 表)
- ✅ **詳細用戶資料** (User_Introduce 表)
- ✅ **用戶權限管理** (User_Rights 表)
- ✅ **用戶錢包系統** (User_wallet 表)
- ✅ **銷售檔案管理** (MemberSalesProfile 表)
- ✅ **銷售錢包** (User_Sales_Information 表)

#### 3. 系統架構
- ✅ **三層式架構** - Domain、Infrastructure、Presentation
- ✅ **Entity Framework Core** - Code First 方式
- ✅ **依賴注入** - 完整的 DI 容器設定
- ✅ **JWT 認證中介軟體** - ASP.NET Core 整合
- ✅ **Swagger UI** - API 文件與測試介面

#### 4. 測試與品質保證
- ✅ **單元測試** - AuthService 完整測試覆蓋
- ✅ **Mock 物件** - 使用 Moq 框架
- ✅ **測試資料** - 完整的種子資料

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

## 測試資料

系統會自動建立以下測試帳號：

| 帳號 | 密碼 | 說明 |
|------|------|------|
| admin@gamecore.com | admin123 | 管理員帳號 |
| user1@gamecore.com | user123 | 測試用戶1 |
| user2@gamecore.com | user123 | 測試用戶2 |
| demo@gamecore.com | demo123 | 展示帳號 |

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

## 技術特色

### 安全性
- **密碼雜湊**: SHA256 + 固定鹽值
- **JWT 驗證**: HS256 演算法
- **權限控制**: 角色基礎存取控制

### 效能
- **非同步程式設計**: 全面使用 async/await
- **連線管理**: Entity Framework Core 最佳化
- **快取準備**: 架構支援未來快取實作

### 可維護性
- **SOLID 原則**: 依賴反轉、單一職責
- **介面分離**: 清楚的抽象層
- **測試友善**: 高度可測試的設計

## 下一階段規劃

### Stage 2 - 錢包與銷售系統
- 完整的點數交易系統
- 銷售權限申請流程
- 錢包交易記錄

### Stage 3 - 商城系統
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
