# Stage 4: Social/Notifications/DM/Groups/Blocks 實現文檔

## 概述

Stage 4 實現了完整的社交功能模組，包括通知系統、私訊系統、群組系統、封鎖系統和管理者後台。這個模組為用戶提供了豐富的社交互動體驗，同時為管理者提供了強大的管理工具。

## 架構設計

### 核心模組

1. **通知系統 (Notifications)**
   - 支持多種通知來源和行為類型
   - 支持群組通知和個人通知
   - 提供已讀/未讀狀態管理

2. **私訊系統 (Chat)**
   - 支持用戶間私訊
   - 支持管理員發送系統訊息
   - 提供訊息狀態追蹤

3. **群組系統 (Groups)**
   - 支持創建、加入、離開群組
   - 支持群組管理員權限管理
   - 支持群組聊天功能

4. **封鎖系統 (Blocks)**
   - 支持群組內用戶封鎖
   - 提供封鎖管理功能

5. **管理者後台 (Manager)**
   - 支持管理者帳號管理
   - 支持角色權限管理
   - 支持禁言和樣式管理

## 實體設計

### 通知相關實體

- **NotificationSource**: 通知來源類型
- **NotificationAction**: 通知行為類型
- **Notification**: 通知主表
- **NotificationRecipient**: 通知接收者

### 聊天相關實體

- **ChatMessage**: 聊天訊息

### 群組相關實體

- **Group**: 群組主表
- **GroupMember**: 群組成員
- **GroupChat**: 群組聊天訊息
- **GroupBlock**: 群組封鎖

### 管理者相關實體

- **ManagerData**: 管理者資料
- **ManagerRolePermission**: 管理者角色權限
- **ManagerRole**: 管理者角色指派
- **Admin**: 管理員登入追蹤
- **Mute**: 禁言選項
- **Style**: 樣式池

## 服務層

### INotificationService

提供通知系統的核心業務邏輯：

```csharp
public interface INotificationService
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool? isRead = null, int page = 1, int pageSize = 20);
    Task<Notification?> GetNotificationAsync(int notificationId);
    Task<Notification> CreateNotificationAsync(Notification notification, List<int> recipientUserIds);
    Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
    Task<bool> MarkAllNotificationsAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
    Task<IEnumerable<NotificationSource>> GetNotificationSourcesAsync();
    Task<IEnumerable<NotificationAction>> GetNotificationActionsAsync();
    Task<Notification> CreateSystemNotificationAsync(string title, string message, List<int> recipientUserIds, int? senderManagerId = null);
}
```

### IChatService

提供聊天系統的核心業務邏輯：

```csharp
public interface IChatService
{
    Task<IEnumerable<ChatMessage>> GetDirectMessagesAsync(int userId, int peerId, int page = 1, int pageSize = 50);
    Task<ChatMessage> SendDirectMessageAsync(int senderId, int receiverId, string content);
    Task<ChatMessage> SendSystemMessageAsync(int receiverId, string content, int? managerId = null);
    Task<bool> MarkMessageAsReadAsync(int messageId, int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task<IEnumerable<ChatMessage>> GetRecentConversationsAsync(int userId, int limit = 10);
    Task<bool> DeleteMessageAsync(int messageId, int userId);
}
```

### IGroupService

提供群組系統的核心業務邏輯：

```csharp
public interface IGroupService
{
    Task<IEnumerable<Group>> GetUserGroupsAsync(int userId);
    Task<Group?> GetGroupAsync(int groupId);
    Task<Group> CreateGroupAsync(string groupName, int createdBy);
    Task<bool> JoinGroupAsync(int groupId, int userId);
    Task<bool> LeaveGroupAsync(int groupId, int userId);
    Task<bool> RemoveMemberAsync(int groupId, int userId, int adminUserId);
    Task<bool> SetMemberAsAdminAsync(int groupId, int userId, int adminUserId);
    Task<bool> RemoveAdminAsync(int groupId, int userId, int adminUserId);
    Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId);
    Task<IEnumerable<GroupChat>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 50);
    Task<GroupChat> SendGroupMessageAsync(int groupId, int senderId, string content);
    Task<bool> BlockUserAsync(int groupId, int userId, int blockedBy);
    Task<bool> UnblockUserAsync(int groupId, int userId, int adminUserId);
    Task<IEnumerable<GroupBlock>> GetGroupBlocksAsync(int groupId);
    Task<bool> IsUserBlockedAsync(int groupId, int userId);
    Task<bool> IsUserAdminAsync(int groupId, int userId);
    Task<bool> DeleteGroupAsync(int groupId, int adminUserId);
}
```

### IManagerService

提供管理者後台的核心業務邏輯：

```csharp
public interface IManagerService
{
    Task<ManagerData?> GetManagerAsync(int managerId);
    Task<ManagerData?> GetManagerByAccountAsync(string account);
    Task<ManagerData> CreateManagerAsync(ManagerData manager);
    Task<bool> UpdateManagerAsync(ManagerData manager);
    Task<bool> DeleteManagerAsync(int managerId);
    Task<IEnumerable<ManagerData>> GetAllManagersAsync();
    Task<bool> AssignRoleAsync(int managerId, int roleId);
    Task<bool> RemoveRoleAsync(int managerId, int roleId);
    Task<IEnumerable<ManagerRole>> GetManagerRolesAsync(int managerId);
    Task<bool> HasPermissionAsync(int managerId, string permissionName);
    Task<bool> UpdateLastLoginAsync(int managerId);
    Task<IEnumerable<ManagerRolePermission>> GetAllRolesAsync();
    Task<ManagerRolePermission> CreateRoleAsync(ManagerRolePermission role);
    Task<bool> UpdateRoleAsync(ManagerRolePermission role);
    Task<bool> DeleteRoleAsync(int roleId);
    Task<IEnumerable<Mute>> GetMutesAsync();
    Task<Mute> CreateMuteAsync(Mute mute);
    Task<bool> UpdateMuteAsync(Mute mute);
    Task<bool> DeleteMuteAsync(int muteId);
    Task<IEnumerable<Style>> GetStylesAsync();
    Task<Style> CreateStyleAsync(Style style);
    Task<bool> UpdateStyleAsync(Style style);
    Task<bool> DeleteStyleAsync(int styleId);
}
```

## DTOs

### 通知相關 DTOs

- **CreateNotificationRequestDto**: 創建通知請求
- **NotificationResponseDto**: 通知響應
- **NotificationListResponseDto**: 通知列表響應
- **MarkNotificationReadRequestDto**: 標記通知已讀請求
- **NotificationSourceResponseDto**: 通知來源響應
- **NotificationActionResponseDto**: 通知行為響應
- **SystemNotificationRequestDto**: 系統通知請求

### 聊天相關 DTOs

- **SendMessageRequestDto**: 發送私訊請求
- **ChatMessageResponseDto**: 聊天訊息響應
- **ChatConversationResponseDto**: 聊天對話響應
- **RecentConversationResponseDto**: 最近對話響應
- **SystemMessageRequestDto**: 系統訊息請求
- **MarkMessageReadRequestDto**: 標記訊息已讀請求

### 群組相關 DTOs

- **CreateGroupRequestDto**: 創建群組請求
- **GroupResponseDto**: 群組響應
- **GroupDetailResponseDto**: 群組詳情響應
- **GroupMemberResponseDto**: 群組成員響應
- **GroupChatResponseDto**: 群組聊天訊息響應
- **GroupBlockResponseDto**: 群組封鎖響應
- **JoinGroupRequestDto**: 加入群組請求
- **LeaveGroupRequestDto**: 離開群組請求
- **GroupManagementRequestDto**: 群組管理請求
- **SendGroupMessageRequestDto**: 發送群組訊息請求
- **GroupListResponseDto**: 群組列表響應
- **GroupMessageListResponseDto**: 群組訊息列表響應

### 管理者相關 DTOs

- **ManagerLoginRequestDto**: 管理者登入請求
- **ManagerLoginResponseDto**: 管理者登入響應
- **ManagerResponseDto**: 管理者資料響應
- **ManagerRoleResponseDto**: 管理者角色響應
- **ManagerRolePermissionResponseDto**: 管理者角色權限響應
- **CreateManagerRequestDto**: 創建管理者請求
- **UpdateManagerRequestDto**: 更新管理者請求
- **CreateRoleRequestDto**: 創建角色請求
- **UpdateRoleRequestDto**: 更新角色請求
- **MuteResponseDto**: 禁言選項響應
- **StyleResponseDto**: 樣式響應
- **ManagerListResponseDto**: 管理者列表響應
- **RoleListResponseDto**: 角色列表響應

## API 端點

### 通知控制器 (NotificationController)

- `GET /api/notification` - 獲取用戶通知列表
- `GET /api/notification/{notificationId}` - 獲取單個通知
- `POST /api/notification` - 創建通知
- `POST /api/notification/{notificationId}/read` - 標記通知為已讀
- `POST /api/notification/mark-all-read` - 標記所有通知為已讀
- `DELETE /api/notification/{notificationId}` - 刪除通知
- `GET /api/notification/sources` - 獲取通知來源列表
- `GET /api/notification/actions` - 獲取通知行為列表
- `POST /api/notification/system` - 創建系統通知

### 聊天控制器 (ChatController)

- `GET /api/chat/direct` - 獲取私訊對話
- `POST /api/chat/direct` - 發送私訊
- `POST /api/chat/system` - 發送系統訊息
- `POST /api/chat/{messageId}/read` - 標記訊息為已讀
- `GET /api/chat/unread-count` - 獲取未讀訊息數量
- `GET /api/chat/recent` - 獲取最近對話
- `DELETE /api/chat/{messageId}` - 刪除訊息

### 群組控制器 (GroupController)

- `GET /api/group/user/{userId}` - 獲取用戶的群組列表
- `GET /api/group/{groupId}` - 獲取群組詳情
- `POST /api/group` - 創建群組
- `POST /api/group/join` - 加入群組
- `POST /api/group/leave` - 離開群組
- `POST /api/group/remove-member` - 移除群組成員
- `POST /api/group/set-admin` - 設置成員為管理員
- `POST /api/group/remove-admin` - 移除管理員權限
- `GET /api/group/{groupId}/members` - 獲取群組成員列表
- `GET /api/group/{groupId}/messages` - 獲取群組訊息列表
- `POST /api/group/{groupId}/messages` - 發送群組訊息
- `POST /api/group/{groupId}/block` - 封鎖用戶
- `POST /api/group/{groupId}/unblock` - 解除封鎖用戶
- `GET /api/group/{groupId}/blocks` - 獲取群組封鎖列表
- `DELETE /api/group/{groupId}` - 刪除群組

### 管理者控制器 (ManagerController)

- `POST /api/manager/login` - 管理者登入
- `GET /api/manager/{managerId}` - 獲取管理者資料
- `POST /api/manager` - 創建管理者
- `PUT /api/manager/{managerId}` - 更新管理者資料
- `DELETE /api/manager/{managerId}` - 刪除管理者
- `GET /api/manager` - 獲取所有管理者
- `GET /api/manager/roles` - 獲取所有角色
- `POST /api/manager/roles` - 創建角色
- `PUT /api/manager/roles/{roleId}` - 更新角色
- `DELETE /api/manager/roles/{roleId}` - 刪除角色
- `GET /api/manager/mutes` - 獲取禁言選項列表
- `POST /api/manager/mutes` - 創建禁言選項
- `GET /api/manager/styles` - 獲取樣式列表
- `POST /api/manager/styles` - 創建樣式

## 業務邏輯

### 通知系統

1. **通知創建流程**
   - 驗證通知來源和行為
   - 創建通知記錄
   - 為每個接收者創建接收記錄

2. **通知狀態管理**
   - 支持已讀/未讀狀態
   - 支持批量標記已讀
   - 支持通知刪除

3. **系統通知**
   - 支持管理員發送系統通知
   - 自動設置系統來源和行為

### 聊天系統

1. **私訊流程**
   - 驗證發送者和接收者
   - 創建聊天訊息
   - 更新未讀計數

2. **系統訊息**
   - 支持管理員發送系統訊息
   - 自動設置管理員標識

3. **訊息管理**
   - 支持已讀狀態管理
   - 支持訊息刪除
   - 支持最近對話查詢

### 群組系統

1. **群組創建流程**
   - 創建群組記錄
   - 創建者自動成為成員和管理員

2. **成員管理**
   - 支持加入/離開群組
   - 支持管理員權限管理
   - 支持成員移除

3. **群組聊天**
   - 支持群組訊息發送
   - 檢查發送者是否被封鎖
   - 支持訊息分頁查詢

4. **封鎖管理**
   - 支持用戶封鎖/解除封鎖
   - 檢查操作者權限
   - 封鎖用戶無法發送訊息

### 管理者後台

1. **帳號管理**
   - 支持管理者創建/更新/刪除
   - 密碼加密存儲
   - 帳號唯一性驗證

2. **角色權限管理**
   - 支持角色創建/更新/刪除
   - 支持角色指派/移除
   - 支持權限檢查

3. **登入追蹤**
   - 記錄最後登入時間
   - 支持登入狀態查詢

4. **功能管理**
   - 支持禁言選項管理
   - 支持樣式池管理

## 測試

### 單元測試

創建了完整的單元測試覆蓋：

- **NotificationServiceTests**: 測試通知服務的所有方法
- 使用 In-Memory 數據庫進行測試
- 測試各種邊界情況和錯誤處理

### 測試覆蓋

- 通知創建、查詢、更新、刪除
- 分頁查詢功能
- 已讀狀態管理
- 系統通知創建
- 錯誤處理和驗證

## 部署

### 數據庫配置

1. **實體配置**
   - 所有新實體都已配置到 DbContext
   - 設置了適當的外鍵關係和約束
   - 配置了索引和唯一性約束

2. **遷移準備**
   - 實體配置已完成
   - 準備好進行數據庫遷移

### 服務註冊

需要在 `Program.cs` 中註冊新的服務：

```csharp
// 註冊 Stage 4 服務
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
```

## 性能優化

### 查詢優化

1. **分頁查詢**
   - 所有列表查詢都支持分頁
   - 使用 Skip/Take 進行分頁

2. **索引優化**
   - 為常用查詢字段添加索引
   - 複合索引優化

3. **關聯查詢優化**
   - 使用 Include 進行關聯查詢
   - 避免 N+1 查詢問題

### 緩存策略

1. **通知計數緩存**
   - 未讀通知數量可以緩存
   - 定期更新緩存

2. **群組成員緩存**
   - 群組成員列表可以緩存
   - 成員變更時更新緩存

## 安全考慮

### 權限控制

1. **用戶權限**
   - 用戶只能操作自己的數據
   - 群組管理員權限驗證

2. **管理員權限**
   - 角色基礎的權限控制
   - 操作權限驗證

### 數據驗證

1. **輸入驗證**
   - 所有輸入都進行驗證
   - 防止 SQL 注入和 XSS 攻擊

2. **業務邏輯驗證**
   - 檢查操作權限
   - 驗證業務規則

## 監控和日誌

### 日誌記錄

1. **操作日誌**
   - 記錄所有重要操作
   - 包含用戶ID和時間戳

2. **錯誤日誌**
   - 記錄所有錯誤和異常
   - 包含詳細的錯誤信息

### 性能監控

1. **查詢性能**
   - 監控慢查詢
   - 記錄查詢執行時間

2. **系統性能**
   - 監控系統資源使用
   - 記錄響應時間

## 未來擴展

### 功能擴展

1. **即時通訊**
   - 集成 SignalR 實現即時通訊
   - 支持在線狀態顯示

2. **文件共享**
   - 支持圖片和文件上傳
   - 實現文件存儲和管理

3. **通知推送**
   - 支持 Web Push 通知
   - 集成第三方推送服務

### 性能擴展

1. **分佈式部署**
   - 支持多實例部署
   - 實現負載均衡

2. **數據庫優化**
   - 實現讀寫分離
   - 添加緩存層

## 總結

Stage 4 成功實現了完整的社交功能模組，包括：

- ✅ 通知系統：支持多種通知類型和狀態管理
- ✅ 聊天系統：支持私訊和系統訊息
- ✅ 群組系統：支持群組創建、管理和聊天
- ✅ 封鎖系統：支持用戶封鎖管理
- ✅ 管理者後台：支持帳號、角色和功能管理
- ✅ 完整的 API 端點和 DTOs
- ✅ 完整的單元測試覆蓋
- ✅ 數據庫配置和實體關係
- ✅ 安全性和性能優化

這個模組為用戶提供了豐富的社交互動體驗，同時為管理者提供了強大的管理工具，為整個平台奠定了堅實的社交基礎。