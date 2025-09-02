# GameCore 架構藍圖

## 概述
GameCore 採用 Clean Architecture 架構模式，確保業務邏輯與技術實作分離，提高程式碼的可維護性、可測試性和可擴展性。

## 解決方案佈局

### 專案結構
```
GameCore/
├── GameCore.Domain/           # 領域層 - 核心業務邏輯
├── GameCore.Application/      # 應用層 - 業務用例和服務
├── GameCore.Infrastructure/  # 基礎設施層 - 外部依賴實作
├── GameCore.Web/             # 表現層 - API 和 MVC 控制器
└── GameCore.Tests/           # 測試專案
```

## 分層職責與依賴規則

### 1. GameCore.Domain (領域層)
**職責：**
- 實體 (Entities)
- 值物件 (Value Objects)
- 列舉 (Enums)
- 領域事件 (Domain Events)
- 儲存庫介面 (Repository Interfaces)
- 領域服務介面 (Domain Service Interfaces)

**依賴規則：**
- 不依賴任何其他層
- 不依賴外部框架或資料庫
- 純粹的業務邏輯

**核心模組：**
- `Entities/` - 使用者、錢包、寵物、聊天等核心實體
- `ValueObjects/` - 金額、時間範圍、遊戲分數等值物件
- `Enums/` - 遊戲狀態、交易類型、權限等級等列舉
- `Events/` - 領域事件定義
- `Interfaces/` - 儲存庫和服務介面

### 2. GameCore.Application (應用層)
**職責：**
- DTOs (資料傳輸物件)
- Result<T> 和 OperationResult 模式
- 業務用例 (Use Cases)
- 應用服務介面 (Application Service Interfaces)
- 驗證器 (Validators)
- 適配器 (Adapters)

**依賴規則：**
- 依賴 GameCore.Domain
- 不依賴 GameCore.Infrastructure
- 不依賴 GameCore.Web

**核心模組：**
- `Common/` - Result<T>、OperationResult、驗證結果等通用類型
- `DTOs/` - 請求/回應 DTOs
- `Services/` - 應用服務介面
- `Validators/` - 資料驗證器
- `Adapters/` - 向後相容適配器

### 3. GameCore.Infrastructure (基礎設施層)
**職責：**
- Entity Framework DbContext
- 儲存庫實作 (Repository Implementations)
- 外部服務整合 (External Service Integrations)
- 快取實作 (Caching Implementations)
- 日誌實作 (Logging Implementations)

**依賴規則：**
- 依賴 GameCore.Domain
- 依賴 GameCore.Application
- 不依賴 GameCore.Web

**核心模組：**
- `Data/` - EF Core 配置和 DbContext
- `Repositories/` - 儲存庫實作
- `Services/` - 外部服務整合
- `Caching/` - 快取實作
- `Logging/` - 日誌實作

### 4. GameCore.Web (表現層)
**職責：**
- API 控制器 (API Controllers)
- MVC 控制器 (MVC Controllers)
- 中介軟體 (Middleware)
- 路由配置 (Routing Configuration)
- 依賴注入配置 (DI Configuration)

**依賴規則：**
- 依賴 GameCore.Application
- 不直接依賴 GameCore.Domain
- 不直接依賴 GameCore.Infrastructure

**核心模組：**
- `Controllers/` - API 和 MVC 控制器
- `Middleware/` - 自定義中介軟體
- `Configuration/` - 應用程式配置

## 模組對應表

### 核心業務模組
| 模組 | Domain | Application | Infrastructure | Web |
|------|--------|-------------|----------------|-----|
| **使用者/認證** | User, Auth | IAuthService, IUserService | AuthService, UserService | AuthController, UserController |
| **錢包系統** | Wallet, Transaction | IWalletService | WalletService | WalletController |
| **聊天系統** | Chat, Message | IChatService | ChatService | ChatController |
| **寵物系統** | Pet, PetStats | IPetService | PetService | PetController |
| **玩家市場** | Market, Order | IPlayerMarketService | PlayerMarketService | MarketController |
| **小遊戲** | MiniGame, GameRecord | IMiniGameService | MiniGameService | MiniGameController |
| **商店系統** | Store, Product | IStoreService | StoreService | StoreController |
| **通知系統** | Notification | INotificationService | NotificationService | NotificationController |
| **銷售/管理** | Sales, Admin | ISalesService | SalesService | SalesController, AdminController |

### 橫切關注點 (Cross-Cutting Concerns)
| 關注點 | 實作位置 | 說明 |
|--------|----------|------|
| **Result 模式** | Application.Common | 統一的操作結果封裝 |
| **驗證** | Application.Validators | 資料驗證邏輯 |
| **日誌** | Infrastructure.Logging | 應用程式日誌 |
| **快取** | Infrastructure.Caching | 資料快取機制 |
| **交易** | Infrastructure.Data | 資料庫交易管理 |
| **分頁** | Application.Common | 分頁結果封裝 |
| **錯誤處理** | Web.Middleware | 全域錯誤處理 |

## 依賴注入配置

### 服務註冊順序
1. **Domain Services** - 註冊領域服務
2. **Application Services** - 註冊應用服務
3. **Infrastructure Services** - 註冊基礎設施服務
4. **Web Services** - 註冊 Web 層服務

### 生命週期管理
- **Singleton**: 配置服務、快取服務
- **Scoped**: 業務服務、儲存庫
- **Transient**: 驗證器、適配器

## 資料流

### 請求流程
```
HTTP Request → Controller → Application Service → Domain Service → Repository → Database
```

### 回應流程
```
Database → Repository → Domain Service → Application Service → DTO → Controller → HTTP Response
```

## 遷移策略

### 階段 1: 架構骨架
- 建立分層專案結構
- 實作 Result<T> 模式
- 建立服務介面存根

### 階段 2: 核心模組
- 認證與使用者管理
- 錢包系統
- 聊天系統

### 階段 3: 遊戲模組
- 寵物系統
- 小遊戲系統
- 市場系統

### 階段 4: 管理模組
- 商店系統
- 通知系統
- 銷售管理

## 品質保證

### 測試策略
- **單元測試**: 測試業務邏輯和服務層
- **整合測試**: 測試儲存庫和外部服務
- **冒煙測試**: 測試 API 端點基本功能
- **E2E 測試**: 測試完整用戶流程

### 程式碼品質
- 遵循 SOLID 原則
- 使用依賴注入
- 實作介面隔離
- 保持單一職責

## 注意事項

### 資料庫相容性
- **不修改現有資料庫結構**
- 使用 EF Core 映射適配現有結構
- 需要結構變更時，僅在文件中提出建議

### API 相容性
- 保持現有 API 端點穩定
- 使用適配器模式處理向後相容
- 新功能通過新端點提供

### 效能考量
- 實作適當的快取策略
- 使用非同步操作
- 實作分頁和延遲載入 