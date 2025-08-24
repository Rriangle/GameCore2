# GameCore 小步任務列表

## 當前狀態：2024-12-19
- 整體完成度：24%
- 優先級：高優先級任務優先執行

## 🔥 高優先級任務（立即執行）

### 1. 後端核心模組補齊

#### 1.1 Game 模組
- [x] `src/GameCore.Api/Controllers/GameController.cs` - 遊戲控制器
- [x] `src/GameCore.Api/DTOs/GameDTOs.cs` - 遊戲相關 DTO
- [x] `src/GameCore.Api/Services/GameService.cs` - 遊戲服務層
- [ ] `src/GameCore.Infrastructure/Repositories/GameRepository.cs` - 遊戲資料存取層
- [ ] `tests/GameCore.Api.Tests/Controllers/GameControllerTests.cs` - 遊戲控制器測試

#### 1.2 Forum 模組
- [ ] `src/GameCore.Api/Controllers/ForumController.cs` - 論壇控制器
- [ ] `src/GameCore.Api/DTOs/ForumDTOs.cs` - 論壇相關 DTO
- [ ] `src/GameCore.Api/Services/ForumService.cs` - 論壇服務層
- [ ] `src/GameCore.Infrastructure/Repositories/ForumRepository.cs` - 論壇資料存取層
- [ ] `tests/GameCore.Api.Tests/Controllers/ForumControllerTests.cs` - 論壇控制器測試

#### 1.3 Pet 模組
- [ ] `src/GameCore.Api/Controllers/PetController.cs` - 寵物控制器
- [ ] `src/GameCore.Api/DTOs/PetDTOs.cs` - 寵物相關 DTO
- [ ] `src/GameCore.Api/Services/PetService.cs` - 寵物服務層
- [ ] `src/GameCore.Infrastructure/Repositories/PetRepository.cs` - 寵物資料存取層
- [ ] `tests/GameCore.Api.Tests/Controllers/PetControllerTests.cs` - 寵物控制器測試

### 2. 前端 Pinia Stores 補齊

#### 2.1 核心 Stores
- [x] `frontend/src/stores/game.ts` - 遊戲狀態管理
- [ ] `frontend/src/stores/forum.ts` - 論壇狀態管理
- [ ] `frontend/src/stores/pet.ts` - 寵物系統狀態管理
- [ ] `frontend/src/stores/notification.ts` - 通知狀態管理
- [ ] `frontend/src/stores/signin.ts` - 簽到系統狀態管理
- [ ] `frontend/src/stores/user.ts` - 用戶資料管理

#### 2.2 前端服務層
- [x] `frontend/src/services/gameService.ts` - 遊戲 API 服務
- [ ] `frontend/src/services/forumService.ts` - 論壇 API 服務
- [ ] `frontend/src/services/petService.ts` - 寵物 API 服務
- [ ] `frontend/src/services/notificationService.ts` - 通知 API 服務
- [ ] `frontend/src/services/signinService.ts` - 簽到 API 服務

### 3. CI/CD 流程修復

#### 3.1 GitHub Actions
- [ ] `.github/workflows/backend.yml` - .NET 8 build+test workflow
- [ ] `.github/workflows/frontend.yml` - Node.js build+test workflow
- [ ] 修復現有 `ci.yml` 配置問題

#### 3.2 測試覆蓋率
- [ ] 後端測試覆蓋率達到 80%
- [ ] 前端測試配置與執行
- [ ] 整合測試實作

## 🔶 中優先級任務

### 4. 基礎設施完善

#### 4.1 後端基礎設施
- [ ] `src/GameCore.Api/Middleware/JwtMiddleware.cs` - JWT 驗證中間件
- [ ] `src/GameCore.Api/Extensions/ServiceCollectionExtensions.cs` - 依賴注入配置
- [ ] `src/GameCore.Api/Extensions/ApplicationBuilderExtensions.cs` - 應用程式配置

#### 4.2 前端基礎設施
- [ ] `frontend/src/types/` - TypeScript 型別定義
- [ ] `frontend/src/components/` - 可重用 UI 組件
- [ ] `frontend/src/views/` - 頁面組件

### 5. 文件完善

#### 5.1 專案文件
- [ ] `README.md` - 專案說明與安裝指南
- [ ] `CONTRIBUTING.md` - 開發規範
- [ ] `docs/DEPLOYMENT.md` - 部署指南

#### 5.2 API 文件
- [ ] Swagger/OpenAPI 配置
- [ ] API 端點文件

## 🔵 低優先級任務

### 6. 優化與擴展

#### 6.1 效能優化
- [ ] 資料庫查詢優化
- [ ] 前端效能優化
- [ ] 快取機制實作

#### 6.2 額外功能
- [ ] 即時通知系統
- [ ] 檔案上傳功能
- [ ] 搜尋功能

## 📊 進度追蹤

### 當前完成項目
- ✅ Auth 模組（後端）
- ✅ Auth Store（前端）
- ✅ 基礎專案結構
- ✅ Docker 配置（基礎）

### 本輪目標
- [ ] 完成 Game、Forum、Pet 模組
- [ ] 完成對應的 Pinia stores
- [ ] 修復 CI/CD 流程
- [ ] 達到 80% 測試覆蓋率

### 完成度預估
- 後端核心功能：30% → 80%
- 前端狀態管理：20% → 80%
- CI/CD 流程：40% → 90%
- 測試覆蓋率：10% → 80%
- 文件完整性：20% → 70%

**目標整體完成度：80%**