# GameCore 介面契約

## 概述
本文檔定義了 GameCore 系統中所有 Repository 和 Service 介面的統一方法命名和簽名規範，確保整個系統的一致性。

## 通用模式

### Result<T> 模式
所有服務方法都返回 `Result<T>` 類型，提供統一的成功/失敗處理：

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string ErrorMessage { get; }
    public List<ValidationError> ValidationErrors { get; }
    
    public static Result<T> Success(T value);
    public static Result<T> Failure(string error);
    public static Result<T> ValidationFailure(List<ValidationError> errors);
}
```

### 分頁結果
分頁查詢返回 `PagedResult<T>` 類型：

```csharp
public class PagedResult<T>
{
    public List<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }
}
```

## Repository 介面契約

### 基礎 Repository 介面
```csharp
public interface IRepository<TEntity, TKey> where TEntity : class
{
    // 基本 CRUD 操作
    Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<List<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
    
    // 分頁查詢
    Task<Result<PagedResult<TEntity>>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    // 條件查詢
    Task<Result<List<TEntity>>> FindAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default);
    
    // 存在性檢查
    Task<Result<bool>> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default);
    
    // 計數
    Task<Result<int>> CountAsync(
        Expression<Func<TEntity, bool>> predicate = null, 
        CancellationToken cancellationToken = default);
}
```

### 特殊化 Repository 介面
```csharp
// 使用者 Repository
public interface IUserRepository : IRepository<User, int>
{
    Task<Result<User>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
}

// 錢包 Repository
public interface IWalletRepository : IRepository<Wallet, int>
{
    Task<Result<Wallet>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<List<Transaction>>> GetTransactionsAsync(
        int walletId, 
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        CancellationToken cancellationToken = default);
}

// 聊天 Repository
public interface IChatRepository : IRepository<ChatMessage, int>
{
    Task<Result<List<ChatMessage>>> GetConversationAsync(
        int user1Id, 
        int user2Id, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    Task<Result<List<ChatMessage>>> GetGroupMessagesAsync(
        int groupId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
}
```

## Service 介面契約

### 認證服務 (IAuthService)
```csharp
public interface IAuthService
{
    // 使用者註冊
    Task<Result<UserDto>> RegisterAsync(
        RegisterRequest request, 
        CancellationToken cancellationToken = default);
    
    // 使用者登入
    Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request, 
        CancellationToken cancellationToken = default);
    
    // 重新整理權杖
    Task<Result<LoginResponse>> RefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default);
    
    // 登出
    Task<Result<bool>> LogoutAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default);
    
    // 變更密碼
    Task<Result<bool>> ChangePasswordAsync(
        int userId, 
        ChangePasswordRequest request, 
        CancellationToken cancellationToken = default);
    
    // 重設密碼
    Task<Result<bool>> ResetPasswordAsync(
        ResetPasswordRequest request, 
        CancellationToken cancellationToken = default);
}
```

### 使用者服務 (IUserService)
```csharp
public interface IUserService
{
    // 取得使用者資訊
    Task<Result<UserDto>> GetUserByIdAsync(
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者列表
    Task<Result<PagedResult<UserDto>>> GetUsersAsync(
        int pageNumber, 
        int pageSize, 
        string searchTerm = null, 
        CancellationToken cancellationToken = default);
    
    // 更新使用者資訊
    Task<Result<UserDto>> UpdateUserAsync(
        int userId, 
        UpdateUserRequest request, 
        CancellationToken cancellationToken = default);
    
    // 刪除使用者
    Task<Result<bool>> DeleteUserAsync(
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 啟用/停用使用者
    Task<Result<bool>> ToggleUserStatusAsync(
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者權限
    Task<Result<List<string>>> GetUserPermissionsAsync(
        int userId, 
        CancellationToken cancellationToken = default);
}
```

### 錢包服務 (IWalletService)
```csharp
public interface IWalletService
{
    // 建立錢包
    Task<Result<WalletDto>> CreateWalletAsync(
        int userId, 
        decimal initialBalance, 
        CancellationToken cancellationToken = default);
    
    // 取得錢包資訊
    Task<Result<WalletDto>> GetWalletAsync(
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 存款
    Task<Result<TransactionDto>> DepositAsync(
        int userId, 
        decimal amount, 
        string description, 
        CancellationToken cancellationToken = default);
    
    // 提款
    Task<Result<TransactionDto>> WithdrawAsync(
        int userId, 
        decimal amount, 
        string description, 
        CancellationToken cancellationToken = default);
    
    // 轉帳
    Task<Result<TransferResult>> TransferAsync(
        int fromUserId, 
        int toUserId, 
        decimal amount, 
        string description, 
        CancellationToken cancellationToken = default);
    
    // 取得交易記錄
    Task<Result<PagedResult<TransactionDto>>> GetTransactionsAsync(
        int userId, 
        int pageNumber, 
        int pageSize, 
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        CancellationToken cancellationToken = default);
    
    // 取得餘額
    Task<Result<decimal>> GetBalanceAsync(
        int userId, 
        CancellationToken cancellationToken = default);
}
```

### 聊天服務 (IChatService)
```csharp
public interface IChatService
{
    // 發送私訊
    Task<Result<ChatMessageDto>> SendPrivateMessageAsync(
        int senderId, 
        int receiverId, 
        string content, 
        CancellationToken cancellationToken = default);
    
    // 發送群組訊息
    Task<Result<ChatMessageDto>> SendGroupMessageAsync(
        int senderId, 
        int groupId, 
        string content, 
        CancellationToken cancellationToken = default);
    
    // 取得私訊對話
    Task<Result<PagedResult<ChatMessageDto>>> GetPrivateConversationAsync(
        int user1Id, 
        int user2Id, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    // 取得群組訊息
    Task<Result<PagedResult<ChatMessageDto>>> GetGroupMessagesAsync(
        int groupId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    // 標記訊息為已讀
    Task<Result<bool>> MarkAsReadAsync(
        int messageId, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 刪除訊息
    Task<Result<bool>> DeleteMessageAsync(
        int messageId, 
        int userId, 
        CancellationToken cancellationToken = default);
}
```

### 寵物服務 (IPetService)
```csharp
public interface IPetService
{
    // 建立寵物
    Task<Result<PetDto>> CreatePetAsync(
        int userId, 
        CreatePetRequest request, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者寵物
    Task<Result<List<PetDto>>> GetUserPetsAsync(
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 取得寵物詳情
    Task<Result<PetDto>> GetPetByIdAsync(
        int petId, 
        CancellationToken cancellationToken = default);
    
    // 更新寵物資訊
    Task<Result<PetDto>> UpdatePetAsync(
        int petId, 
        UpdatePetRequest request, 
        CancellationToken cancellationToken = default);
    
    // 餵食寵物
    Task<Result<PetDto>> FeedPetAsync(
        int petId, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 訓練寵物
    Task<Result<PetDto>> TrainPetAsync(
        int petId, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 寵物進化
    Task<Result<PetDto>> EvolvePetAsync(
        int petId, 
        int userId, 
        CancellationToken cancellationToken = default);
}
```

### 小遊戲服務 (IMiniGameService)
```csharp
public interface IMiniGameService
{
    // 開始遊戲
    Task<Result<GameSessionDto>> StartGameAsync(
        int userId, 
        int gameTypeId, 
        CancellationToken cancellationToken = default);
    
    // 提交遊戲結果
    Task<Result<GameResultDto>> SubmitGameResultAsync(
        int sessionId, 
        int userId, 
        GameResultRequest request, 
        CancellationToken cancellationToken = default);
    
    // 取得遊戲記錄
    Task<Result<PagedResult<GameRecordDto>>> GetGameRecordsAsync(
        int userId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    // 取得排行榜
    Task<Result<List<LeaderboardEntryDto>>> GetLeaderboardAsync(
        int gameTypeId, 
        int topCount = 10, 
        CancellationToken cancellationToken = default);
    
    // 取得遊戲統計
    Task<Result<GameStatsDto>> GetGameStatsAsync(
        int userId, 
        CancellationToken cancellationToken = default);
}
```

### 市場服務 (IPlayerMarketService)
```csharp
public interface IPlayerMarketService
{
    // 上架商品
    Task<Result<MarketItemDto>> ListItemAsync(
        int userId, 
        ListItemRequest request, 
        CancellationToken cancellationToken = default);
    
    // 下架商品
    Task<Result<bool>> UnlistItemAsync(
        int itemId, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 購買商品
    Task<Result<PurchaseResultDto>> PurchaseItemAsync(
        int itemId, 
        int buyerId, 
        CancellationToken cancellationToken = default);
    
    // 搜尋商品
    Task<Result<PagedResult<MarketItemDto>>> SearchItemsAsync(
        MarketSearchRequest request, 
        CancellationToken cancellationToken = default);
    
    // 取得商品詳情
    Task<Result<MarketItemDto>> GetItemByIdAsync(
        int itemId, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者上架商品
    Task<Result<PagedResult<MarketItemDto>>> GetUserListingsAsync(
        int userId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者購買記錄
    Task<Result<PagedResult<PurchaseRecordDto>>> GetPurchaseHistoryAsync(
        int userId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);
}
```

### 商店服務 (IStoreService)
```csharp
public interface IStoreService
{
    // 取得商品列表
    Task<Result<PagedResult<StoreItemDto>>> GetItemsAsync(
        int pageNumber, 
        int pageSize, 
        string category = null, 
        string searchTerm = null, 
        CancellationToken cancellationToken = default);
    
    // 取得商品詳情
    Task<Result<StoreItemDto>> GetItemByIdAsync(
        int itemId, 
        CancellationToken cancellationToken = default);
    
    // 購買商品
    Task<Result<PurchaseResultDto>> PurchaseItemAsync(
        int itemId, 
        int userId, 
        int quantity, 
        CancellationToken cancellationToken = default);
    
    // 取得商品分類
    Task<Result<List<CategoryDto>>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);
    
    // 取得熱門商品
    Task<Result<List<StoreItemDto>>> GetPopularItemsAsync(
        int count = 10, 
        CancellationToken cancellationToken = default);
    
    // 取得推薦商品
    Task<Result<List<StoreItemDto>>> GetRecommendedItemsAsync(
        int userId, 
        int count = 10, 
        CancellationToken cancellationToken = default);
}
```

### 通知服務 (INotificationService)
```csharp
public interface INotificationService
{
    // 發送通知
    Task<Result<NotificationDto>> SendNotificationAsync(
        int userId, 
        NotificationRequest request, 
        CancellationToken cancellationToken = default);
    
    // 發送群組通知
    Task<Result<List<NotificationDto>>> SendGroupNotificationAsync(
        List<int> userIds, 
        NotificationRequest request, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者通知
    Task<Result<PagedResult<NotificationDto>>> GetUserNotificationsAsync(
        int userId, 
        int pageNumber, 
        int pageSize, 
        bool unreadOnly = false, 
        CancellationToken cancellationToken = default);
    
    // 標記通知為已讀
    Task<Result<bool>> MarkAsReadAsync(
        int notificationId, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 標記所有通知為已讀
    Task<Result<bool>> MarkAllAsReadAsync(
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 刪除通知
    Task<Result<bool>> DeleteNotificationAsync(
        int notificationId, 
        int userId, 
        CancellationToken cancellationToken = default);
    
    // 取得未讀通知數量
    Task<Result<int>> GetUnreadCountAsync(
        int userId, 
        CancellationToken cancellationToken = default);
}
```

### 銷售服務 (ISalesService)
```csharp
public interface ISalesService
{
    // 取得銷售統計
    Task<Result<SalesStatsDto>> GetSalesStatsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);
    
    // 取得銷售記錄
    Task<Result<PagedResult<SalesRecordDto>>> GetSalesRecordsAsync(
        DateTime? fromDate = null, 
        DateTime? toDate = null, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);
    
    // 取得商品銷售排行
    Task<Result<List<ItemSalesRankDto>>> GetItemSalesRankAsync(
        DateTime fromDate, 
        DateTime toDate, 
        int topCount = 10, 
        CancellationToken cancellationToken = default);
    
    // 取得使用者消費排行
    Task<Result<List<UserSpendingRankDto>>> GetUserSpendingRankAsync(
        DateTime fromDate, 
        DateTime toDate, 
        int topCount = 10, 
        CancellationToken cancellationToken = default);
    
    // 匯出銷售報表
    Task<Result<byte[]>> ExportSalesReportAsync(
        DateTime fromDate, 
        DateTime toDate, 
        string format = "xlsx", 
        CancellationToken cancellationToken = default);
}
```

## 方法命名規範

### 查詢方法
- `Get` - 取得單一實體
- `GetAll` - 取得所有實體
- `GetPaged` - 取得分頁結果
- `Find` - 條件查詢
- `Search` - 搜尋查詢
- `Exists` - 存在性檢查
- `Count` - 計數查詢

### 操作方法
- `Create` - 建立新實體
- `Add` - 新增實體
- `Update` - 更新實體
- `Delete` - 刪除實體
- `Remove` - 移除實體
- `Save` - 儲存變更

### 業務方法
- `Process` - 處理業務邏輯
- `Execute` - 執行操作
- `Validate` - 驗證資料
- `Calculate` - 計算結果
- `Generate` - 產生資料

## 參數規範

### 必要參數
- 識別碼參數放在第一位
- 使用明確的類型名稱
- 避免使用 object 類型

### 可選參數
- 使用預設值
- 使用 CancellationToken 作為最後一個參數
- 使用可空類型表示可選值

### 分頁參數
- `pageNumber` 從 1 開始
- `pageSize` 有合理的上下限
- 提供預設值

## 返回值規範

### 成功情況
- 返回 `Result<T>.Success(value)`
- 包含完整的資料物件
- 提供適當的狀態碼

### 失敗情況
- 返回 `Result<T>.Failure(message)`
- 提供清晰的錯誤訊息
- 包含適當的錯誤代碼

### 驗證失敗
- 返回 `Result<T>.ValidationFailure(errors)`
- 包含詳細的驗證錯誤
- 提供欄位級別的錯誤資訊

## 例外處理

### 業務例外
- 使用自定義的業務例外類型
- 提供清晰的錯誤訊息
- 記錄適當的日誌資訊

### 系統例外
- 捕獲並記錄系統例外
- 轉換為適當的錯誤回應
- 不向用戶暴露系統詳細資訊

## 效能考量

### 非同步操作
- 所有 I/O 操作使用 async/await
- 正確處理取消權杖
- 避免阻塞操作

### 分頁查詢
- 實作高效的分頁查詢
- 使用適當的索引
- 避免 N+1 查詢問題

### 快取策略
- 實作適當的快取機制
- 使用快取失效策略
- 監控快取效能 