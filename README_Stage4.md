# GameCore Stage 4 - 社交功能系統

## 概述

Stage 4 實現了 GameCore 平台的完整社交功能系統，包括通知系統、私訊聊天和群組管理功能。這些功能為用戶提供了豐富的社交互動體驗。

## 功能特色

### 1. 通知系統 (Notification System)
- **即時通知**: 支援系統通知、用戶互動通知
- **通知管理**: 標記已讀、刪除通知、通知統計
- **多種來源**: 支援系統、用戶、群組等多種通知來源
- **通知分類**: 根據行為類型分類通知

### 2. 私訊聊天 (Private Messaging)
- **一對一聊天**: 用戶間的私密對話
- **聊天記錄**: 完整的聊天歷史記錄
- **已讀狀態**: 訊息已讀/未讀狀態追蹤
- **聊天對象管理**: 聊天對象列表和搜尋
- **訊息搜尋**: 在聊天記錄中搜尋特定內容

### 3. 群組管理 (Group Management)
- **群組建立**: 用戶可以建立和管理群組
- **成員管理**: 加入/退出群組、成員列表
- **群組聊天**: 群組內的多人聊天功能
- **權限控制**: 群組管理員權限
- **封鎖功能**: 群組內用戶封鎖管理
- **群組搜尋**: 搜尋和發現群組

## 技術架構

### 資料庫設計
- **通知相關表**: `Notifications`, `NotificationSources`, `NotificationActions`, `NotificationRecipients`
- **聊天相關表**: `ChatMessage`
- **群組相關表**: `Groups`, `GroupMembers`, `GroupChats`, `GroupBlocks`

### API 端點

#### 通知系統 API
```
GET    /api/notification                    # 取得通知列表
POST   /api/notification/{id}/read          # 標記通知為已讀
POST   /api/notification/read-all           # 標記所有通知為已讀
GET    /api/notification/stats              # 取得通知統計
DELETE /api/notification/{id}               # 刪除通知
POST   /api/notification                    # 建立通知 (管理員)
POST   /api/notification/system             # 建立系統通知 (管理員)
```

#### 聊天系統 API
```
GET    /api/chat/history/{otherUserId}      # 取得聊天記錄
POST   /api/chat/send                       # 發送訊息
POST   /api/chat/messages/{id}/read         # 標記訊息為已讀
POST   /api/chat/read-all/{otherUserId}     # 標記所有訊息為已讀
GET    /api/chat/unread-count               # 取得未讀數量
GET    /api/chat/partners                   # 取得聊天對象列表
GET    /api/chat/latest/{otherUserId}       # 取得最新訊息
GET    /api/chat/search                     # 搜尋聊天訊息
GET    /api/chat/stats                      # 取得聊天統計
```

#### 群組系統 API
```
POST   /api/group                           # 建立群組
GET    /api/group/{id}                      # 取得群組詳情
GET    /api/group/my-groups                 # 取得我的群組列表
POST   /api/group/{id}/join                 # 加入群組
POST   /api/group/{id}/leave                # 退出群組
GET    /api/group/{id}/members              # 取得群組成員
POST   /api/group/{id}/messages             # 發送群組訊息
GET    /api/group/{id}/messages             # 取得群組聊天記錄
POST   /api/group/{id}/block                # 封鎖用戶
DELETE /api/group/{id}/block/{userId}       # 解除封鎖用戶
GET    /api/group/search                    # 搜尋群組
GET    /api/group/stats                     # 取得群組統計
```

## 使用範例

### 1. 取得通知列表
```bash
curl -X GET "https://localhost:7001/api/notification" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 2. 發送私訊
```bash
curl -X POST "https://localhost:7001/api/chat/send" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "receiverId": 2,
    "content": "你好！"
  }'
```

### 3. 建立群組
```bash
curl -X POST "https://localhost:7001/api/group" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "groupName": "遊戲討論群",
    "initialMemberIds": [2, 3, 4]
  }'
```

### 4. 發送群組訊息
```bash
curl -X POST "https://localhost:7001/api/group/1/messages" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "groupId": 1,
    "content": "大家好！"
  }'
```

## 資料模型

### 通知 DTO
```csharp
public class NotificationDTO
{
    public int NotificationId { get; set; }
    public int SourceId { get; set; }
    public string SourceName { get; set; }
    public int ActionId { get; set; }
    public string ActionName { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
```

### 聊天訊息 DTO
```csharp
public class ChatMessageDTO
{
    public int MessageId { get; set; }
    public int? ManagerId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; }
    public int? ReceiverId { get; set; }
    public string? ReceiverName { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public bool IsSent { get; set; }
}
```

### 群組 DTO
```csharp
public class GroupDTO
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsMember { get; set; }
}
```

## 測試

### 執行測試
```bash
# 執行所有測試
dotnet test

# 執行特定測試專案
dotnet test src/GameCore.Tests/Services/NotificationServiceTests.cs
dotnet test src/GameCore.Tests/Services/ChatServiceTests.cs
dotnet test src/GameCore.Tests/Services/GroupServiceTests.cs
```

### 測試覆蓋範圍
- **通知服務測試**: 8 個測試案例
- **聊天服務測試**: 10 個測試案例
- **群組服務測試**: 12 個測試案例
- **總計**: 30 個測試案例

## 部署

### 環境需求
- .NET 8.0
- SQL Server 2019+
- 至少 2GB RAM

### 部署步驟
1. 建置專案
```bash
dotnet build
```

2. 執行資料庫遷移
```bash
dotnet ef database update
```

3. 啟動應用程式
```bash
dotnet run --project src/GameCore.Api
```

## 安全性考量

### 認證與授權
- 所有 API 端點都需要 JWT 認證
- 管理員功能需要額外的角色權限
- 用戶只能存取自己的資料

### 資料驗證
- 所有輸入資料都經過驗證
- 防止 SQL 注入攻擊
- XSS 防護

### 速率限制
- API 呼叫速率限制
- 防止濫用和 DDoS 攻擊

## 效能優化

### 資料庫優化
- 適當的索引設計
- 查詢優化
- 連線池管理

### 快取策略
- 記憶體快取
- 分散式快取支援

### 非同步處理
- 所有 I/O 操作都是非同步
- 背景工作處理

## 監控與日誌

### 日誌記錄
- 使用 Serilog 進行結構化日誌記錄
- 記錄所有 API 呼叫
- 錯誤追蹤和除錯

### 監控指標
- API 回應時間
- 錯誤率
- 系統資源使用率

## 未來擴展

### 計劃中的功能
- 即時通知推送
- 語音和視訊聊天
- 檔案分享功能
- 表情符號和貼圖支援
- 群組投票功能

### 技術改進
- SignalR 即時通訊
- WebSocket 支援
- 微服務架構
- 容器化部署

## 支援與維護

### 問題回報
如有任何問題或建議，請透過以下方式聯繫：
- GitHub Issues
- 技術支援信箱

### 文件更新
本文檔會根據系統更新持續維護，請定期查看最新版本。

---

**版本**: 1.0.0  
**更新日期**: 2024-01-27  
**作者**: GameCore 開發團隊 