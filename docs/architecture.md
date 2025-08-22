# 🏗️ GameCore 系統架構

## 📋 架構概覽

GameCore 採用 Clean Architecture 架構，結合領域驅動設計 (DDD) 原則，確保系統的可維護性、可測試性和可擴展性。

## 🎯 核心架構原則

### Clean Architecture 分層
```
┌─────────────────────────────────────┐
│           Presentation Layer        │ ← 前端 (Vue.js) + API Controllers
├─────────────────────────────────────┤
│           Application Layer         │ ← 應用服務 + DTOs
├─────────────────────────────────────┤
│           Domain Layer              │ ← 實體 + 領域服務 + 介面
├─────────────────────────────────────┤
│        Infrastructure Layer         │ ← 資料庫 + 外部服務
└─────────────────────────────────────┘
```

### 依賴方向
- 外層依賴內層
- 內層不依賴外層
- 透過介面進行依賴反轉

## 🏢 專案結構說明

### 後端專案架構

#### 1. GameCore.Api (表現層)
- **職責**: Web API 端點、請求驗證、回應格式化
- **主要元件**:
  - Controllers: API 控制器
  - Middleware: 自訂中介軟體
  - DTOs: 資料傳輸物件
  - Extensions: 擴充方法

#### 2. GameCore.Mvc (表現層)
- **職責**: MVC 網頁介面、管理後台
- **主要元件**:
  - Controllers: MVC 控制器
  - Views: Razor 視圖
  - ViewModels: 視圖模型

#### 3. GameCore.Domain (領域層)
- **職責**: 核心業務邏輯、領域實體、領域服務
- **主要元件**:
  - Entities: 領域實體
  - Interfaces: 領域介面
  - Value Objects: 值物件
  - Domain Services: 領域服務

#### 4. GameCore.Infrastructure (基礎設施層)
- **職責**: 資料存取、外部服務整合
- **主要元件**:
  - Data: Entity Framework 設定
  - Repositories: 儲存庫實作
  - Services: 外部服務實作

#### 5. GameCore.Shared (共用層)
- **職責**: 跨層級共用元件
- **主要元件**:
  - Constants: 常數定義
  - Extensions: 共用擴充方法
  - Utilities: 工具類別

### 前端架構

#### Vue 3 + TypeScript 架構
```
frontend/
├── src/
│   ├── components/     # 可重用元件
│   ├── views/         # 頁面元件
│   ├── stores/        # Pinia 狀態管理
│   ├── services/      # API 服務
│   ├── types/         # TypeScript 型別定義
│   ├── utils/         # 工具函數
│   └── router/        # Vue Router 設定
```

## 🔄 資料流程

### API 請求流程
```
Client Request → Controller → Application Service → Domain Service → Repository → Database
                ↓
Response ← DTO ← Application Service ← Domain Service ← Repository ← Database
```

### 認證流程
```
Login Request → AuthController → AuthService → JwtService → Token Response
```

## 🗄️ 資料庫設計

### 核心資料表
- **Users**: 用戶基本資料
- **UserWallets**: 用戶錢包
- **Pets**: 寵物系統
- **Games**: 遊戲資料
- **Forums**: 論壇分類
- **Posts**: 論壇文章
- **Shops**: 商城
- **Products**: 商品

### 關聯設計
- 一對一: User ↔ UserWallet
- 一對多: User → Pets, User → Posts
- 多對多: 透過中間表實現

## 🔐 安全性架構

### 認證機制
- **JWT Token**: 無狀態認證
- **Refresh Token**: 自動更新機制
- **Role-based Access Control**: 角色權限控制

### 資料保護
- **Password Hashing**: BCrypt 加密
- **HTTPS**: 傳輸加密
- **CORS**: 跨域資源共享控制
- **Rate Limiting**: 請求頻率限制

## 🚀 部署架構

### 本地開發環境
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Frontend  │    │     API     │    │ SQL Server  │
│  (Vue.js)   │◄──►│ (.NET Core) │◄──►│  Database   │
│  Port:3000  │    │  Port:5000  │    │  Port:1433  │
└─────────────┘    └─────────────┘    └─────────────┘
```

### 生產環境架構
```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Azure     │    │   Azure     │    │   Azure     │
│  Frontend   │◄──►│   API App   │◄──►│ SQL Server  │
│   (CDN)     │    │   Service   │    │  Database   │
└─────────────┘    └─────────────┘    └─────────────┘
```

## 📊 監控與日誌

### 應用程式監控
- **Application Insights**: 效能監控
- **Serilog**: 結構化日誌
- **Health Checks**: 健康狀態檢查

### 基礎設施監控
- **Azure Monitor**: 資源監控
- **Grafana**: 自訂儀表板
- **Alerting**: 自動告警機制

## 🔧 開發工具與流程

### 開發工具
- **IDE**: Visual Studio 2022 / VS Code
- **Database**: SQL Server Management Studio
- **API Testing**: Postman / Swagger
- **Version Control**: Git + GitHub

### CI/CD 流程
```
Code Push → GitHub Actions → Build → Test → Deploy → Monitor
```

## 📈 擴展性考量

### 水平擴展
- **API**: 多實例負載平衡
- **Database**: 讀寫分離
- **Cache**: Redis 叢集

### 垂直擴展
- **微服務化**: 按業務領域拆分
- **事件驅動**: 非同步處理
- **API Gateway**: 統一入口

---

*此文件會隨著專案發展持續更新*