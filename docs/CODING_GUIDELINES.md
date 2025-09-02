# GameCore 編碼規範

## 命名規則

### 專案和資料夾命名
- 使用 PascalCase: `GameCore.Domain`, `GameCore.Application`
- 資料夾使用 PascalCase: `Entities/`, `Services/`, `Controllers/`

### 類別和介面命名
- 類別使用 PascalCase: `UserService`, `WalletController`
- 介面以 `I` 開頭: `IUserService`, `IWalletRepository`
- 抽象類別以 `Base` 或 `Abstract` 結尾: `BaseService`, `AbstractRepository`

### 方法和屬性命名
- 方法使用 PascalCase: `GetUserById()`, `CreateWallet()`
- 屬性使用 PascalCase: `UserId`, `WalletBalance`
- 私有欄位使用 camelCase: `_userService`, `_dbContext`

### 常數和列舉
- 常數使用 PascalCase: `MaxRetryCount`, `DefaultPageSize`
- 列舉使用 PascalCase: `GameStatus`, `TransactionType`
- 列舉值使用 PascalCase: `GameStatus.Active`, `TransactionType.Deposit`

### 變數命名
- 區域變數使用 camelCase: `userId`, `walletBalance`
- 參數使用 camelCase: `CreateUser(string userName, int age)`
- 布林變數使用 `Is`, `Has`, `Can` 前綴: `IsActive`, `HasPermission`

## 專案結構規範

### 檔案組織
```
ProjectName/
├── Common/           # 通用類型和工具
├── Entities/         # 實體類別
├── Interfaces/       # 介面定義
├── Services/         # 服務實作
├── Repositories/     # 儲存庫實作
├── DTOs/            # 資料傳輸物件
├── Validators/      # 驗證器
├── Adapters/        # 適配器
└── Configuration/   # 配置類別
```

### 命名空間規範
- 使用專案名稱作為根命名空間: `GameCore.Domain`
- 按功能模組組織: `GameCore.Domain.Entities`, `GameCore.Application.Services`
- 避免過深的命名空間巢狀結構

## 程式碼風格

### 格式化
- 使用 4 個空格縮排
- 大括號使用 Allman 風格 (新行)
- 行長度限制在 120 字元以內
- 使用 UTF-8 編碼

### 註解規範
- 所有公開 API 必須有 XML 文件註解
- 複雜業務邏輯需要行內註解
- 使用繁體中文撰寫註解
- 註解應該解釋「為什麼」而不是「做什麼」

```csharp
/// <summary>
/// 建立使用者錢包
/// </summary>
/// <param name="userId">使用者識別碼</param>
/// <param name="initialBalance">初始餘額</param>
/// <returns>錢包建立結果</returns>
public async Task<Result<Wallet>> CreateWalletAsync(int userId, decimal initialBalance)
{
    // 檢查使用者是否存在
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
    {
        return Result<Wallet>.Failure("使用者不存在");
    }

    // 建立新錢包
    var wallet = new Wallet(userId, initialBalance);
    await _walletRepository.AddAsync(wallet);
    
    return Result<Wallet>.Success(wallet);
}
```

## 例外處理規範

### 例外類型選擇
- **ArgumentException**: 參數驗證失敗
- **InvalidOperationException**: 操作在當前狀態下無效
- **UnauthorizedAccessException**: 權限不足
- **NotFoundException**: 資源不存在
- **BusinessRuleException**: 業務規則違反

### 例外處理策略
- 在應用層捕獲並轉換為 Result<T>
- 在 Web 層統一處理例外
- 記錄詳細的例外資訊
- 向用戶顯示友善的錯誤訊息

```csharp
try
{
    var result = await _userService.CreateUserAsync(userDto);
    return Ok(result);
}
catch (BusinessRuleException ex)
{
    _logger.LogWarning(ex, "業務規則違反: {Message}", ex.Message);
    return BadRequest(new { error = ex.Message });
}
catch (Exception ex)
{
    _logger.LogError(ex, "建立使用者時發生未預期錯誤");
    return StatusCode(500, new { error = "內部伺服器錯誤" });
}
```

## Result 模式規範

### Result<T> 結構
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string ErrorMessage { get; }
    public List<ValidationError> ValidationErrors { get; }
    
    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(string error) => new Result<T>(error);
}
```

### 使用規範
- 所有服務方法返回 Result<T>
- 成功時使用 `Result<T>.Success(value)`
- 失敗時使用 `Result<T>.Failure(message)`
- 驗證失敗時使用 `Result<T>.ValidationFailure(errors)`

```csharp
public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
{
    if (userId <= 0)
    {
        return Result<UserDto>.Failure("使用者識別碼必須大於 0");
    }

    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
    {
        return Result<UserDto>.Failure("使用者不存在");
    }

    var userDto = _mapper.Map<UserDto>(user);
    return Result<UserDto>.Success(userDto);
}
```

## 非同步程式設計規範

### async/await 使用
- 所有 I/O 操作使用 async/await
- 方法名稱以 `Async` 結尾
- 避免使用 `.Result` 或 `.Wait()`
- 正確處理取消權杖

```csharp
public async Task<Result<List<UserDto>>> GetUsersAsync(
    int page, 
    int pageSize, 
    CancellationToken cancellationToken = default)
{
    try
    {
        var users = await _userRepository
            .GetPagedAsync(page, pageSize, cancellationToken);
            
        var userDtos = _mapper.Map<List<UserDto>>(users);
        return Result<List<UserDto>>.Success(userDtos);
    }
    catch (OperationCanceledException)
    {
        return Result<List<UserDto>>.Failure("操作已取消");
    }
}
```

### 取消權杖處理
- 公開 API 方法接受 CancellationToken 參數
- 在長時間運行的操作中檢查取消狀態
- 正確傳遞取消權杖到下游方法

## 驗證規範

### 輸入驗證
- 使用 FluentValidation 進行 DTO 驗證
- 在控制器層進行模型驗證
- 在服務層進行業務規則驗證
- 提供清晰的驗證錯誤訊息

```csharp
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("使用者名稱不能為空")
            .Length(3, 50).WithMessage("使用者名稱長度必須在 3-50 字元之間")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("使用者名稱只能包含字母、數字和底線");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件不能為空")
            .EmailAddress().WithMessage("電子郵件格式不正確");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("年齡必須大於 0")
            .LessThanOrEqualTo(120).WithMessage("年齡不能超過 120");
    }
}
```

## 日誌規範

### 日誌級別使用
- **Trace**: 詳細的追蹤資訊
- **Debug**: 除錯資訊
- **Information**: 一般資訊
- **Warning**: 警告資訊
- **Error**: 錯誤資訊
- **Critical**: 嚴重錯誤

### 日誌內容
- 記錄操作開始和結束
- 記錄重要的業務事件
- 記錄效能指標
- 記錄使用者操作

```csharp
public async Task<Result<Wallet>> CreateWalletAsync(int userId, decimal initialBalance)
{
    _logger.LogInformation("開始建立使用者錢包，使用者ID: {UserId}, 初始餘額: {InitialBalance}", 
        userId, initialBalance);

    try
    {
        var result = await _walletService.CreateWalletAsync(userId, initialBalance);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("成功建立使用者錢包，錢包ID: {WalletId}", 
                result.Value.WalletId);
        }
        else
        {
            _logger.LogWarning("建立使用者錢包失敗: {ErrorMessage}", 
                result.ErrorMessage);
        }
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "建立使用者錢包時發生未預期錯誤，使用者ID: {UserId}", userId);
        throw;
    }
}
```

## 測試規範

### 測試命名
- 測試方法命名: `MethodName_Scenario_ExpectedResult`
- 測試類別命名: `ClassNameTests`
- 測試專案命名: `ProjectName.Tests`

### 測試結構
- 使用 AAA 模式 (Arrange, Act, Assert)
- 每個測試只測試一個行為
- 使用描述性的測試名稱
- 測試失敗時提供清晰的錯誤訊息

```csharp
[Fact]
public async Task CreateWalletAsync_ValidUserIdAndBalance_ReturnsSuccessResult()
{
    // Arrange
    var userId = 1;
    var initialBalance = 100.0m;
    var expectedWallet = new Wallet(userId, initialBalance);
    
    _userRepository.Setup(x => x.GetByIdAsync(userId))
        .ReturnsAsync(new User { UserId = userId });
    _walletRepository.Setup(x => x.AddAsync(It.IsAny<Wallet>()))
        .Returns(Task.CompletedTask);

    // Act
    var result = await _walletService.CreateWalletAsync(userId, initialBalance);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(userId, result.Value.UserId);
    Assert.Equal(initialBalance, result.Value.Balance);
}
```

## 效能考量

### 資料庫查詢
- 使用適當的索引
- 避免 N+1 查詢問題
- 使用分頁查詢
- 實作適當的快取策略

### 記憶體管理
- 及時釋放大型物件
- 使用 using 語句管理資源
- 避免記憶體洩漏
- 監控記憶體使用量

## 安全性規範

### 輸入驗證
- 驗證所有使用者輸入
- 防止 SQL 注入攻擊
- 防止 XSS 攻擊
- 使用參數化查詢

### 權限控制
- 實作適當的授權機制
- 驗證使用者權限
- 記錄敏感操作
- 使用安全的認證方式

## 文件化

### API 文件
- 使用 Swagger/OpenAPI 文件
- 提供清晰的 API 說明
- 包含請求和回應範例
- 說明錯誤碼和錯誤訊息

### 程式碼文件
- 複雜的業務邏輯需要詳細註解
- 公開 API 必須有 XML 文件
- 更新文件時同步更新程式碼
- 使用繁體中文撰寫文件 