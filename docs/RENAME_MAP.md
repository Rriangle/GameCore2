# GameCore 重命名對照表

## 概述
本文檔記錄了 GameCore 系統架構重設過程中所有重命名的對應關係，包括類別、介面、方法、屬性等的舊名稱和新名稱。

## 命名空間重命名

### 舊命名空間 → 新命名空間
```
GameCore.Shared.Interfaces → GameCore.Application.Interfaces
GameCore.Shared.DTOs → GameCore.Application.DTOs
GameCore.Domain.Interfaces → GameCore.Application.Interfaces
GameCore.Api.Services → GameCore.Application.Services
GameCore.Api.Controllers → GameCore.Web.Controllers
```

## 介面重命名

### 服務介面
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `IUserService` | `IUserService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IAuthService` | `IAuthService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IWalletService` | `IWalletService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IChatService` | `IChatService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IPetService` | `IPetService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IMiniGameService` | `IMiniGameService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IMarketService` | `IPlayerMarketService` | `GameCore.Application.Interfaces` | 重命名為更明確的名稱 |
| `IStoreService` | `IStoreService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `ISalesService` | `ISalesService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `INotificationService` | `INotificationService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IGroupService` | `IGroupService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IForumService` | `IForumService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `ILeaderboardService` | `ILeaderboardService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `ISignInService` | `ISignInService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |
| `IValidationService` | `IValidationService` | `GameCore.Application.Interfaces` | 保持不變，移至 Application 層 |

### 儲存庫介面
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `IUserRepository` | `IUserRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IWalletRepository` | `IWalletRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IChatRepository` | `IChatRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IPetRepository` | `IPetRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IMiniGameRepository` | `IMiniGameRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IMarketRepository` | `IPlayerMarketRepository` | `GameCore.Domain.Interfaces` | 重命名為更明確的名稱 |
| `IStoreRepository` | `IStoreRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `ISalesRepository` | `ISalesRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `INotificationRepository` | `INotificationRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IGroupRepository` | `IGroupRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |
| `IForumRepository` | `IForumRepository` | `GameCore.Domain.Interfaces` | 保持不變，仍在 Domain 層 |

## 類別重命名

### 服務實作類別
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `UserService` | `UserService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `AuthService` | `AuthService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `WalletService` | `WalletService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `ChatService` | `ChatService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `PetService` | `PetService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `MiniGameService` | `MiniGameService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `MarketService` | `PlayerMarketService` | `GameCore.Application.Services` | 重命名為更明確的名稱 |
| `StoreService` | `StoreService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `SalesService` | `SalesService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `NotificationService` | `NotificationService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `GroupService` | `GroupService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `ForumService` | `ForumService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `LeaderboardService` | `LeaderboardService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `SignInService` | `SignInService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |
| `ValidationService` | `ValidationService` | `GameCore.Application.Services` | 保持不變，移至 Application 層 |

### 儲存庫實作類別
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `UserRepository` | `UserRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `WalletRepository` | `WalletRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `ChatRepository` | `ChatRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `PetRepository` | `PetRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `MiniGameRepository` | `MiniGameRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `MarketRepository` | `PlayerMarketRepository` | `GameCore.Infrastructure.Repositories` | 重命名為更明確的名稱 |
| `StoreRepository` | `StoreRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `SalesRepository` | `SalesRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `NotificationRepository` | `NotificationRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `GroupRepository` | `GroupRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |
| `ForumRepository` | `ForumRepository` | `GameCore.Infrastructure.Repositories` | 保持不變，仍在 Infrastructure 層 |

### 控制器類別
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `UserController` | `UserController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `AuthController` | `AuthController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `WalletController` | `WalletController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `ChatController` | `ChatController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `PetController` | `PetController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `MiniGameController` | `MiniGameController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `MarketController` | `PlayerMarketController` | `GameCore.Web.Controllers` | 重命名為更明確的名稱 |
| `StoreController` | `StoreController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `SalesController` | `SalesController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `NotificationController` | `NotificationController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `GroupController` | `GroupController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `ForumController` | `ForumController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `LeaderboardController` | `LeaderboardController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |
| `SignInController` | `SignInController` | `GameCore.Web.Controllers` | 保持不變，移至 Web 層 |

## DTO 重命名

### 請求 DTOs
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `CreateUserRequest` | `CreateUserRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `UpdateUserRequest` | `UpdateUserRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `LoginRequest` | `LoginRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `RegisterRequest` | `RegisterRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `CreateWalletRequest` | `CreateWalletRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `DepositRequest` | `DepositRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `WithdrawRequest` | `WithdrawRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `TransferRequest` | `TransferRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `SendMessageRequest` | `SendMessageRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `CreatePetRequest` | `CreatePetRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `UpdatePetRequest` | `UpdatePetRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `StartGameRequest` | `StartGameRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `GameResultRequest` | `GameResultRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `ListItemRequest` | `ListItemRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `PurchaseRequest` | `PurchaseRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |
| `NotificationRequest` | `NotificationRequest` | `GameCore.Application.DTOs.Requests` | 保持不變，移至 Application 層 |

### 回應 DTOs
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `UserDto` | `UserDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `WalletDto` | `WalletDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `TransactionDto` | `TransactionDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `ChatMessageDto` | `ChatMessageDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `PetDto` | `PetDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `GameSessionDto` | `GameSessionDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `GameResultDto` | `GameResultDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `MarketItemDto` | `MarketItemDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `StoreItemDto` | `StoreItemDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `NotificationDto` | `NotificationDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |
| `SalesStatsDto` | `SalesStatsDto` | `GameCore.Application.DTOs.Responses` | 保持不變，移至 Application 層 |

## 結果類型重命名

### 舊結果類型 → 新結果類型
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `ServiceResult<T>` | `Result<T>` | `GameCore.Application.Common` | 統一為 Result<T> 模式 |
| `ApiResult<T>` | `Result<T>` | `GameCore.Application.Common` | 統一為 Result<T> 模式 |
| `OperationResult` | `OperationResult` | `GameCore.Application.Common` | 保持不變，移至 Common |

## 列舉重命名

### 狀態列舉
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `UserStatus` | `UserStatus` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `GameStatus` | `GameStatus` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `TransactionStatus` | `TransactionStatus` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `MessageStatus` | `MessageStatus` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `PetStatus` | `PetStatus` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |

### 類型列舉
| 舊名稱 | 新名稱 | 位置 | 說明 |
|--------|--------|------|------|
| `GameType` | `GameType` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `PetType` | `PetType` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `MessageType` | `MessageType` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `TransactionType` | `TransactionType` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |
| `NotificationType` | `NotificationType` | `GameCore.Domain.Enums` | 保持不變，仍在 Domain 層 |

## 方法重命名

### 服務方法
| 舊名稱 | 新名稱 | 說明 |
|--------|--------|------|
| `GetUser` | `GetUserByIdAsync` | 統一非同步命名規範 |
| `CreateUser` | `CreateUserAsync` | 統一非同步命名規範 |
| `UpdateUser` | `UpdateUserAsync` | 統一非同步命名規範 |
| `DeleteUser` | `DeleteUserAsync` | 統一非同步命名規範 |
| `GetUsers` | `GetUsersAsync` | 統一非同步命名規範 |
| `GetWallet` | `GetWalletAsync` | 統一非同步命名規範 |
| `CreateWallet` | `CreateWalletAsync` | 統一非同步命名規範 |
| `Deposit` | `DepositAsync` | 統一非同步命名規範 |
| `Withdraw` | `WithdrawAsync` | 統一非同步命名規範 |
| `Transfer` | `TransferAsync` | 統一非同步命名規範 |
| `SendMessage` | `SendMessageAsync` | 統一非同步命名規範 |
| `GetMessages` | `GetMessagesAsync` | 統一非同步命名規範 |
| `CreatePet` | `CreatePetAsync` | 統一非同步命名規範 |
| `GetPet` | `GetPetByIdAsync` | 統一非同步命名規範 |
| `UpdatePet` | `UpdatePetAsync` | 統一非同步命名規範 |
| `StartGame` | `StartGameAsync` | 統一非同步命名規範 |
| `SubmitResult` | `SubmitGameResultAsync` | 更明確的方法名稱 |
| `ListItem` | `ListItemAsync` | 統一非同步命名規範 |
| `PurchaseItem` | `PurchaseItemAsync` | 統一非同步命名規範 |

### 儲存庫方法
| 舊名稱 | 新名稱 | 說明 |
|--------|--------|------|
| `GetById` | `GetByIdAsync` | 統一非同步命名規範 |
| `GetAll` | `GetAllAsync` | 統一非同步命名規範 |
| `Add` | `AddAsync` | 統一非同步命名規範 |
| `Update` | `UpdateAsync` | 統一非同步命名規範 |
| `Delete` | `DeleteAsync` | 統一非同步命名規範 |
| `Find` | `FindAsync` | 統一非同步命名規範 |
| `Exists` | `ExistsAsync` | 統一非同步命名規範 |
| `Count` | `CountAsync` | 統一非同步命名規範 |

## 屬性重命名

### 實體屬性
| 舊名稱 | 新名稱 | 說明 |
|--------|--------|------|
| `UserID` | `UserId` | 統一命名規範 |
| `UserName` | `Username` | 統一命名規範 |
| `UserEmail` | `Email` | 簡化命名 |
| `UserPassword` | `Password` | 簡化命名 |
| `WalletID` | `WalletId` | 統一命名規範 |
| `WalletBalance` | `Balance` | 簡化命名 |
| `PetID` | `PetId` | 統一命名規範 |
| `PetName` | `Name` | 簡化命名 |
| `GameID` | `GameId` | 統一命名規範 |
| `GameName` | `Name` | 簡化命名 |
| `MessageID` | `MessageId` | 統一命名規範 |
| `MessageContent` | `Content` | 簡化命名 |

## 適配器模式

### 向後相容適配器
為了保持向後相容性，我們將建立適配器來處理舊的 API 呼叫：

```csharp
// 舊的 API 呼叫
public class LegacyUserController : ControllerBase
{
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        // 使用適配器轉換到新的服務
        var result = await _userServiceAdapter.GetUserAsync(id);
        return Ok(result);
    }
}

// 適配器實作
public class UserServiceAdapter : IUserService
{
    private readonly IUserService _userService;
    
    public async Task<Result<UserDto>> GetUserAsync(int userId)
    {
        // 轉換舊的呼叫到新的服務
        return await _userService.GetUserByIdAsync(userId);
    }
}
```

## 遷移檢查清單

### 階段 1: 準備工作
- [ ] 建立新的專案結構
- [ ] 建立命名空間對應表
- [ ] 準備適配器類別

### 階段 2: 逐步遷移
- [ ] 遷移 Domain 層
- [ ] 遷移 Application 層
- [ ] 遷移 Infrastructure 層
- [ ] 遷移 Web 層

### 階段 3: 測試和驗證
- [ ] 執行單元測試
- [ ] 執行整合測試
- [ ] 驗證 API 端點
- [ ] 檢查向後相容性

### 階段 4: 清理工作
- [ ] 移除舊的程式碼
- [ ] 移除適配器類別
- [ ] 更新文件
- [ ] 更新測試

## 注意事項

### 重命名原則
1. **保持語義清晰**: 新名稱應該更清楚地表達其用途
2. **統一命名規範**: 使用一致的命名模式和慣例
3. **向後相容**: 通過適配器保持舊 API 的相容性
4. **漸進式遷移**: 分階段進行，避免一次性大規模變更

### 風險控制
1. **備份原始程式碼**: 在重命名前建立備份
2. **逐步測試**: 每個階段都要進行充分的測試
3. **回滾計劃**: 準備回滾到前一個版本的計劃
4. **團隊溝通**: 確保團隊成員了解重命名的影響

### 文件更新
1. **API 文件**: 更新 Swagger/OpenAPI 文件
2. **開發文件**: 更新開發者指南和 API 參考
3. **測試文件**: 更新測試案例和測試文件
4. **部署文件**: 更新部署和配置文件 