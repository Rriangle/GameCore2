# Stage 0 — Crosswalk Plan

## 專案概述

GameCore 是一個整合遊戲熱度觀測、論壇社群、商城、玩家市場、寵物養成的綜合平台。本文件分析現有實作與規格需求的差距，並提供詳細的實作階段規劃。

## Gap Analysis Table（差距分析表）

| 模組 | 規格需求 | 當前狀態 | 缺失項目 | 需要修改的檔案 |
|------|----------|----------|----------|----------------|
| **使用者系統** | Users, User_Introduce, User_Rights, User_wallet, MemberSalesProfile, User_Sales_Information | 僅有基本 Users 和 UserWallet | User_Introduce, User_Rights, MemberSalesProfile, User_Sales_Information | Domain/Entities/, Infrastructure/Data/GameCoreDbContext.cs, API Controllers |
| **管理員系統** | ManagerData, ManagerRolePermission, ManagerRole, Admins, Mutes, Styles | 完全缺失 | 所有管理員相關實體和功能 | 新增所有管理員相關檔案 |
| **熱度排行榜** | games, metric_sources, metrics, game_source_map, game_metric_daily, popularity_index_daily, leaderboard_snapshots | 完全缺失 | 所有熱度相關實體和計算邏輯 | 新增所有熱度相關檔案 |
| **洞察貼文** | posts, post_metric_snapshot, post_sources | 完全缺失 | 所有洞察貼文功能 | 新增貼文相關檔案 |
| **論壇互動** | forums, threads, thread_posts, reactions, bookmarks | 完全缺失 | 所有論壇功能 | 新增論壇相關檔案 |
| **簽到寵物小遊戲** | UserSignInStats, Pet, MiniGame | 完全缺失 | 所有簽到、寵物、小遊戲功能 | 新增遊戲相關檔案 |
| **官方商城** | Supplier, ProductInfo, GameProductDetails, OtherProductDetails, OrderInfo, OrderItems, Official_Store_Ranking, ProductInfoAuditLog | 完全缺失 | 所有商城功能 | 新增商城相關檔案 |
| **自由市場** | PlayerMarketProductInfo, PlayerMarketProductImgs, PlayerMarketOrderInfo, PlayerMarketOrderTradepage, PlayerMarketTradeMsg, PlayerMarketRanking | 完全缺失 | 所有市場功能 | 新增市場相關檔案 |
| **社交通知** | Notifications, Notification_Sources, Notification_Actions, Notification_Recipients, Chat_Message, Groups, Group_Member, Group_Chat, Group_Block | 完全缺失 | 所有社交通知功能 | 新增社交相關檔案 |
| **前端介面** | Vue 3 + TypeScript + Tailwind 完整前端 | 基本框架存在但無實際頁面 | 所有業務頁面和元件 | frontend/src/ 下所有頁面檔案 |
| **測試** | 單元測試、整合測試、端對端測試 | 基本測試框架 | 所有業務邏輯測試 | tests/ 目錄下所有測試檔案 |

## 實作優先順序與階段規劃

### Stage 1 — 基礎使用者系統 (Foundation User System)
**範圍**: 完整的使用者註冊登入、個資管理、權限系統、錢包基礎
- **實體**: Users, User_Introduce, User_Rights, User_wallet, MemberSalesProfile, User_Sales_Information
- **API**: 註冊、登入、個資編輯、權限檢查、錢包查詢
- **前端**: 登入頁、註冊頁、個人資料頁、錢包頁面
- **測試**: 使用者流程測試、權限驗證測試
- **種子資料**: 100+ 使用者資料、各種權限組合

### Stage 2 — 管理員後台系統 (Admin Management System)
**範圍**: 完整的管理員權限體系、後台管理介面
- **實體**: ManagerData, ManagerRolePermission, ManagerRole, Admins, Mutes, Styles
- **API**: 管理員登入、權限管理、使用者管理
- **前端**: 後台登入、使用者管理頁、權限設定頁
- **測試**: 管理員權限測試、後台功能測試
- **種子資料**: 管理員帳號、角色權限設定

### Stage 3 — 遊戲熱度與排行榜 (Game Popularity & Rankings)
**範圍**: 遊戲資料管理、熱度計算、排行榜生成
- **實體**: games, metric_sources, metrics, game_source_map, game_metric_daily, popularity_index_daily, leaderboard_snapshots
- **API**: 遊戲清單、熱度查詢、排行榜API
- **前端**: 遊戲清單頁、排行榜頁面、熱度圖表
- **測試**: 熱度計算邏輯測試、排行榜生成測試
- **種子資料**: 500+ 遊戲資料、歷史熱度數據

### Stage 4 — 洞察貼文系統 (Insights & Posts)
**範圍**: 洞察文章發布、數據快照、來源管理
- **實體**: posts, post_metric_snapshot, post_sources
- **API**: 貼文CRUD、快照管理、來源管理
- **前端**: 洞察文章頁、編輯器、數據展示
- **測試**: 貼文功能測試、快照邏輯測試
- **種子資料**: 洞察文章、數據快照

### Stage 5 — 論壇社群系統 (Forum & Community)
**範圍**: 論壇版面、主題討論、互動功能
- **實體**: forums, threads, thread_posts, reactions, bookmarks
- **API**: 論壇CRUD、主題回覆、讚與收藏
- **前端**: 論壇首頁、版面頁、主題頁、回覆功能
- **測試**: 論壇功能測試、互動邏輯測試
- **種子資料**: 論壇版面、討論主題、回覆內容

### Stage 6 — 寵物養成與小遊戲 (Pet System & Mini Games)
**範圍**: 每日簽到、虛擬寵物、冒險小遊戲
- **實體**: UserSignInStats, Pet, MiniGame
- **API**: 簽到API、寵物互動、小遊戲邏輯
- **前端**: 簽到頁面、寵物介面、小遊戲畫面
- **測試**: 簽到邏輯測試、寵物狀態測試、遊戲邏輯測試
- **種子資料**: 簽到記錄、寵物資料、遊戲記錄

### Stage 7 — 官方商城 (Official Store)
**範圍**: 商品管理、訂單處理、排行榜統計
- **實體**: Supplier, ProductInfo, GameProductDetails, OtherProductDetails, OrderInfo, OrderItems, Official_Store_Ranking, ProductInfoAuditLog
- **API**: 商品CRUD、訂單處理、支付流程
- **前端**: 商城首頁、商品詳情、購物車、訂單管理
- **測試**: 商城功能測試、訂單流程測試
- **種子資料**: 供應商、商品資料、訂單記錄

### Stage 8 — 玩家自由市場 (Player Market)
**範圍**: C2C交易、交易頁面、市場排行
- **實體**: PlayerMarketProductInfo, PlayerMarketProductImgs, PlayerMarketOrderInfo, PlayerMarketOrderTradepage, PlayerMarketTradeMsg, PlayerMarketRanking
- **API**: 市場商品、交易流程、訊息系統
- **前端**: 市場首頁、商品上架、交易頁面、訊息介面
- **測試**: 交易流程測試、訊息功能測試
- **種子資料**: 市場商品、交易記錄、訊息資料

### Stage 9 — 社交通知系統 (Social & Notifications)
**範圍**: 通知系統、私訊、群組聊天
- **實體**: Notifications, Notification_Sources, Notification_Actions, Notification_Recipients, Chat_Message, Groups, Group_Member, Group_Chat, Group_Block
- **API**: 通知管理、訊息系統、群組功能
- **前端**: 通知中心、聊天介面、群組管理
- **測試**: 通知邏輯測試、聊天功能測試
- **種子資料**: 通知記錄、聊天訊息、群組資料

### Stage 10 — 系統整合與優化 (System Integration & Optimization)
**範圍**: 效能優化、安全強化、監控完善
- **優化**: 資料庫索引、快取機制、API效能
- **安全**: 權限細化、輸入驗證、防護機制
- **監控**: 日誌完善、錯誤追蹤、效能監控
- **測試**: 端對端測試、負載測試、安全測試
- **文件**: API文件、部署指南、操作手冊

## 技術實作細節

### 資料庫設計
- 嚴格按照規格文件中的資料表結構
- 不修改任何欄位名稱或資料型別
- 使用 Entity Framework Core Code First 方式
- 建立完整的關聯關係和索引

### API 設計
- RESTful API 設計原則
- JWT 認證與授權
- 統一的回應格式
- 完整的錯誤處理

### 前端架構
- Vue 3 + Composition API
- TypeScript 型別安全
- Tailwind CSS 樣式框架
- Pinia 狀態管理
- Vue Router 路由管理

### 測試策略
- 單元測試：業務邏輯測試
- 整合測試：API 與資料庫測試
- 端對端測試：完整流程測試
- 測試覆蓋率目標：80% 以上

### 種子資料規劃
- 每個模組至少 100 筆測試資料
- 涵蓋各種邊界情況
- 真實感的假資料生成
- 支援開發和展示需求

## 預估時程

| 階段 | 預估時間 | 主要交付物 |
|------|----------|------------|
| Stage 1 | 3-4 天 | 完整使用者系統 |
| Stage 2 | 2-3 天 | 管理員後台 |
| Stage 3 | 3-4 天 | 熱度排行榜 |
| Stage 4 | 2-3 天 | 洞察貼文 |
| Stage 5 | 4-5 天 | 論壇社群 |
| Stage 6 | 3-4 天 | 寵物小遊戲 |
| Stage 7 | 4-5 天 | 官方商城 |
| Stage 8 | 4-5 天 | 自由市場 |
| Stage 9 | 3-4 天 | 社交通知 |
| Stage 10 | 2-3 天 | 整合優化 |
| **總計** | **30-40 天** | **完整系統** |

## 風險評估與應對

### 主要風險
1. **資料庫複雜度**：多表關聯複雜，需要仔細設計
2. **效能考量**：大量資料的查詢與計算效能
3. **前端複雜度**：多模組整合的使用者體驗
4. **測試覆蓋**：確保所有功能的穩定性

### 應對策略
1. **階段式開發**：每階段確保品質再進行下一階段
2. **持續測試**：每個功能開發完立即測試
3. **效能監控**：及早發現並解決效能問題
4. **文件完善**：確保後續維護的可讀性

## 交付標準

每個階段必須滿足以下條件才能進入下一階段：

### 程式碼品質
- [ ] 建置成功，無編譯錯誤
- [ ] 所有測試通過
- [ ] 程式碼覆蓋率達標
- [ ] 符合編碼規範

### 功能完整性
- [ ] 所有規格需求實作完成
- [ ] API 文件完整
- [ ] 前端介面可用
- [ ] 種子資料完整

### 品質保證
- [ ] 無已知 Bug
- [ ] 效能符合要求
- [ ] 安全性檢查通過
- [ ] 使用者體驗良好

---

**此計劃將作為後續開發的藍圖，確保專案按照規格要求有序推進。**