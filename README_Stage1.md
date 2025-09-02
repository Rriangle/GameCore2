# GameCore Stage 1 - 認證系統實現

## 概述
Stage 1 實現了 GameCore 平台的核心認證系統，包括使用者註冊、登入、JWT 認證等功能。

## 功能特性
- ✅ 使用者註冊與登入
- ✅ JWT 認證機制
- ✅ 密碼加密（SHA256）
- ✅ 使用者資料管理
- ✅ 基本權限控制
- ✅ 資料庫種子資料

## 技術架構
- **後端**: ASP.NET Core 8.0 Web API
- **資料庫**: SQL Server (LocalDB)
- **ORM**: Entity Framework Core 8.0
- **認證**: JWT Bearer Token
- **架構**: 三層式架構 (API, Domain, Infrastructure)

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

### 4. 測試 API
應用程式啟動後，可以測試以下端點：

#### 健康檢查
```bash
GET http://localhost:5000/health
```

#### 使用者註冊
```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "account": "newuser",
  "password": "password123",
  "name": "新使用者",
  "email": "newuser@example.com"
}
```

#### 使用者登入
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "account": "testuser",
  "password": "password123"
}
```

## 預設測試帳號
- **一般使用者**: testuser / password123
- **管理員**: admin / password123

## 資料庫結構
Stage 1 包含以下核心資料表：
- `Users` - 使用者基本資料
- `User_Introduce` - 使用者詳細資訊
- `User_Rights` - 使用者權限
- `User_Wallet` - 使用者錢包

## 下一步
Stage 2 將實現：
- 錢包系統
- 銷售功能
- 官方商店

## 注意事項
- 所有時間戳記使用 UTC 時間
- 密碼使用 SHA256 加密（生產環境建議使用 BCrypt）
- JWT 金鑰僅用於開發測試 