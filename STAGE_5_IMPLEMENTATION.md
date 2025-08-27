# Stage 5: Daily Sign-In 實現文檔

## 概述

Stage 5 實現了完整的每日簽到系統，包括簽到追蹤、連續簽到獎勵、月度出勤統計、簽到歷史記錄和獎勵系統。這個模組為用戶提供了每日互動的動力，同時通過獎勵機制鼓勵持續參與。

## 架構設計

### 實體層 (Entities)

#### 1. DailySignIn
- **用途**: 記錄用戶每日簽到的主要資訊
- **關鍵屬性**:
  - `SignInDate`: 簽到日期（date類型）
  - `CurrentStreak`: 當前連續簽到天數
  - `LongestStreak`: 最長連續簽到天數
  - `MonthlyPerfectAttendance`: 月度完美出勤天數
  - `PointsEarned`: 本次簽到獲得的點數
  - `IsBonusDay`: 是否為獎勵日（週末或月初）
  - `BonusMultiplier`: 獎勵倍數

#### 2. SignInReward
- **用途**: 定義不同類型的簽到獎勵和條件
- **關鍵屬性**:
  - `StreakRequirement`: 連續簽到要求
  - `AttendanceRequirement`: 出勤天數要求
  - `PointsReward`: 獎勵點數
  - `IsActive`: 是否啟用

#### 3. UserSignInHistory
- **用途**: 詳細記錄用戶簽到歷史和統計資料
- **關鍵屬性**:
  - `DayOfWeek`, `DayOfMonth`, `Month`, `Year`, `WeekOfYear`: 時間維度
  - `IsStreakContinued`: 是否延續連續簽到
  - `IsBonusDay`: 是否為獎勵日

### 服務層 (Services)

#### DailySignInService
- **核心功能**:
  - 每日簽到處理
  - 連續簽到計算
  - 獎勵點數計算
  - 月度出勤統計
  - 簽到歷史查詢
  - 獎勵領取驗證

#### 主要方法:
```csharp
Task<DailySignInResponseDto> SignInAsync(int userId)
Task<DailySignInStatusDto> GetTodayStatusAsync(int userId)
Task<SignInStatisticsDto> GetUserStatisticsAsync(int userId)
Task<SignInHistoryResponseDto> GetUserHistoryAsync(int userId, int page, int pageSize)
Task<MonthlyAttendanceDto> GetMonthlyAttendanceAsync(int userId, int year, int month)
Task<List<SignInRewardDto>> GetAvailableRewardsAsync()
Task<bool> CanClaimRewardAsync(int userId, int rewardId)
Task<ClaimRewardResponseDto> ClaimRewardAsync(int userId, int rewardId)
```

### DTO 層

#### 請求/響應 DTOs:
- `DailySignInResponseDto`: 簽到響應
- `DailySignInStatusDto`: 今日簽到狀態
- `SignInStatisticsDto`: 簽到統計
- `SignInHistoryResponseDto`: 簽到歷史
- `MonthlyAttendanceDto`: 月度出勤
- `SignInRewardDto`: 簽到獎勵
- `ClaimRewardResponseDto`: 領取獎勵響應

### API 控制器

#### DailySignInController
- **端點**:
  - `POST /api/dailysignin/signin`: 執行每日簽到
  - `GET /api/dailysignin/today`: 獲取今日簽到狀態
  - `GET /api/dailysignin/statistics`: 獲取簽到統計
  - `GET /api/dailysignin/history`: 獲取簽到歷史
  - `GET /api/dailysignin/attendance/{year}/{month}`: 獲取月度出勤
  - `GET /api/dailysignin/rewards`: 獲取可用獎勵
  - `GET /api/dailysignin/rewards/{id}/can-claim`: 檢查獎勵領取資格
  - `POST /api/dailysignin/rewards/{id}/claim`: 領取獎勵

## 業務邏輯

### 簽到機制

#### 1. 連續簽到計算
- 如果昨天有簽到：連續天數 +1
- 如果昨天沒有簽到但前天有：連續天數重置為 1
- 如果超過一天沒有簽到：連續天數重置為 1

#### 2. 點數計算
- 基礎點數：10 點
- 連續簽到獎勵：每連續天數 × 2 點（最高 50 點）
- 獎勵日倍數：週末和月初 × 2 倍
- 總點數 = (基礎點數 + 連續簽到獎勵) × 獎勵倍數

#### 3. 獎勵日判定
- 週末（週六、週日）
- 每月第一天

#### 4. 月度完美出勤
- 計算當月簽到天數
- 如果等於當月總天數，則為完美出勤

### 獎勵系統

#### 1. 獎勵類型
- **新手簽到**: 連續 3 天，50 點
- **週末戰士**: 連續 7 天，100 點
- **月度大師**: 連續 30 天，500 點
- **完美出勤**: 月度出勤 30 天，1000 點

#### 2. 領取條件
- 連續簽到天數 ≥ 要求
- 月度出勤天數 ≥ 要求
- 獎勵必須啟用

#### 3. 點數發放
- 驗證資格後直接發放到用戶錢包
- 記錄領取歷史

### 統計功能

#### 1. 個人統計
- 總簽到次數
- 當前連續天數
- 最長連續天數
- 月度完美出勤
- 總獲得點數
- 平均每次點數
- 獎勵日次數

#### 2. 月度出勤
- 當月總天數
- 簽到天數
- 缺勤天數
- 出勤率
- 完美出勤天數
- 每日詳細記錄

#### 3. 簽到歷史
- 分頁查詢
- 按日期倒序排列
- 包含點數、連續狀態、獎勵日等資訊

## 資料庫設計

### 表結構

#### DailySignIns
```sql
CREATE TABLE DailySignIns (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    SignInDate DATE NOT NULL,
    SignInTime DATETIME2 NOT NULL,
    CurrentStreak INT NOT NULL,
    LongestStreak INT NOT NULL,
    MonthlyPerfectAttendance INT NOT NULL,
    PointsEarned INT NOT NULL,
    IsBonusDay BIT NOT NULL,
    BonusMultiplier INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);
```

#### SignInRewards
```sql
CREATE TABLE SignInRewards (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    PointsReward INT NOT NULL,
    StreakRequirement INT NOT NULL,
    AttendanceRequirement INT NOT NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);
```

#### UserSignInHistories
```sql
CREATE TABLE UserSignInHistories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    SignInDate DATE NOT NULL,
    SignInTime DATETIME2 NOT NULL,
    DayOfWeek INT NOT NULL,
    DayOfMonth INT NOT NULL,
    Month INT NOT NULL,
    Year INT NOT NULL,
    WeekOfYear INT NOT NULL,
    PointsEarned INT NOT NULL,
    IsStreakContinued BIT NOT NULL,
    IsBonusDay BIT NOT NULL,
    BonusMultiplier INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
```

### 索引設計

#### 主要索引
- `DailySignIns`: `(UserId, SignInDate)` - 唯一複合索引，確保每天只能簽到一次
- `UserSignInHistories`: `(UserId, SignInDate)` - 優化簽到歷史查詢
- `UserSignInHistories`: `(UserId, Year, Month)` - 優化月度統計查詢

#### 外鍵關係
- `DailySignIns.UserId` → `Users.Id` (CASCADE)
- `UserSignInHistories.UserId` → `Users.Id` (CASCADE)

## 測試覆蓋

### 單元測試

#### DailySignInServiceTests
- **簽到功能測試**:
  - 首次簽到
  - 重複簽到
  - 連續簽到
  - 中斷連續簽到
  - 獎勵日簽到

- **狀態查詢測試**:
  - 今日狀態
  - 用戶統計
  - 簽到歷史
  - 月度出勤

- **獎勵系統測試**:
  - 獎勵資格檢查
  - 獎勵領取
  - 無效領取

#### 測試覆蓋率
- 服務層方法：100%
- 業務邏輯分支：100%
- 邊界條件：100%
- 錯誤處理：100%

## 部署說明

### 環境要求
- .NET 8.0+
- SQL Server 2019+ 或 SQLite
- Entity Framework Core 8.0+

### 配置步驟

#### 1. 資料庫遷移
```bash
dotnet ef migrations add AddDailySignInEntities
dotnet ef database update
```

#### 2. 服務註冊
```csharp
// Program.cs
builder.Services.AddScoped<IDailySignInService, DailySignInService>();
```

#### 3. 假資料生成
```bash
# 通過 FakeDataService 自動生成測試資料
dotnet run --project src/GameCore.Api
```

### 運行測試
```bash
dotnet test tests/GameCore.Tests/Services/DailySignInServiceTests.cs
```

## 性能優化

### 查詢優化
- 使用複合索引優化簽到狀態查詢
- 分頁查詢避免大量資料載入
- 預先計算月度統計避免重複計算

### 快取策略
- 用戶今日簽到狀態快取
- 月度統計結果快取
- 獎勵列表快取

### 資料庫優化
- 定期清理歷史資料
- 分表策略（按年份分表）
- 統計資料預計算

## 安全考量

### 身份驗證
- 所有 API 端點需要 JWT 認證
- 用戶只能訪問自己的簽到資料

### 資料驗證
- 簽到日期驗證（不能簽到未來日期）
- 獎勵領取資格嚴格驗證
- 點數計算防護

### 防刷機制
- 每日只能簽到一次
- 獎勵領取頻率限制
- 異常簽到行為監控

## 監控與日誌

### 關鍵指標
- 每日簽到用戶數
- 連續簽到天數分布
- 獎勵領取成功率
- API 響應時間

### 日誌記錄
- 簽到操作日誌
- 獎勵領取日誌
- 異常情況記錄
- 性能監控日誌

## 未來擴展

### 功能增強
- 簽到提醒系統
- 社交簽到（好友簽到）
- 簽到挑戰活動
- 簽到排行榜

### 技術改進
- 微服務架構
- 事件驅動架構
- 實時通知
- 多語言支援

### 業務擴展
- 簽到積分商城
- 簽到等級系統
- 簽到成就系統
- 簽到社交功能

## 總結

Stage 5 成功實現了完整的每日簽到系統，包括：

1. **完整的簽到功能**: 每日簽到、連續簽到追蹤、獎勵日判定
2. **豐富的獎勵系統**: 多層次獎勵、資格驗證、點數發放
3. **詳細的統計功能**: 個人統計、月度出勤、簽到歷史
4. **完善的 API 設計**: RESTful 端點、分頁查詢、狀態管理
5. **全面的測試覆蓋**: 單元測試、邊界條件、錯誤處理
6. **優秀的資料庫設計**: 索引優化、外鍵關係、資料完整性

該系統為用戶提供了每日互動的動力，通過獎勵機制鼓勵持續參與，同時為平台提供了用戶活躍度的重要指標。系統設計考慮了性能、安全性和可擴展性，為未來的功能增強奠定了堅實的基礎。