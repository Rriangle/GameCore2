# Stage 3: Popularity/Leaderboards/Insights 實現文檔

## 概述

Stage 3 實現了遊戲熱度觀察系統、排行榜功能和數據分析與洞察模組。這個階段建立了完整的遊戲熱度追蹤體系，包括多來源數據收集、熱度指數計算、排行榜生成、洞察貼文管理和社群互動功能。

## 架構設計

### 核心模組

1. **熱度追蹤系統** - 收集和分析遊戲相關指標
2. **排行榜系統** - 生成和展示遊戲熱度排名
3. **洞察系統** - 創建和管理數據洞察貼文
4. **論壇系統** - 遊戲相關討論和社群互動
5. **互動系統** - 讚、收藏等用戶互動功能

### 數據流

```
外部數據源 → 指標收集 → 熱度計算 → 排行榜生成 → 洞察分析 → 用戶展示
```

## 實體設計

### 核心實體

#### 1. 遊戲相關實體
- **`Game`** - 遊戲主檔，包含遊戲基本信息和類型
- **`MetricSource`** - 指標來源，定義數據收集的外部平台
- **`Metric`** - 指標定義，描述可收集的數據類型
- **`GameSourceMap`** - 遊戲與外部平台的對應關係

#### 2. 數據收集實體
- **`GameMetricDaily`** - 每日遊戲指標數據
- **`PopularityIndexDaily`** - 每日熱度指數
- **`LeaderboardSnapshot`** - 排行榜快照

#### 3. 內容管理實體
- **`Post`** - 洞察貼文
- **`PostMetricSnapshot`** - 貼文發布時的指標快照
- **`PostSource`** - 貼文引用來源

#### 4. 社群互動實體
- **`Forum`** - 遊戲論壇
- **`Thread`** - 討論主題
- **`ThreadPost`** - 主題回覆
- **`Reaction`** - 用戶反應（讚、表情等）
- **`Bookmark`** - 用戶收藏

### 實體關係

```
Game (1) ←→ (1) Forum
Game (1) ←→ (N) Post
Game (1) ←→ (N) GameMetricDaily
Game (1) ←→ (N) PopularityIndexDaily
Game (1) ←→ (N) LeaderboardSnapshot

MetricSource (1) ←→ (N) Metric
Metric (1) ←→ (N) GameMetricDaily

Post (1) ←→ (1) PostMetricSnapshot
Post (1) ←→ (N) PostSource
Post (1) ←→ (N) Reaction
Post (1) ←→ (N) Bookmark

Forum (1) ←→ (N) Thread
Thread (1) ←→ (N) ThreadPost
Thread (1) ←→ (N) Reaction
Thread (1) ←→ (N) Bookmark
```

## 服務層設計

### 1. IPopularityService
遊戲熱度相關業務邏輯接口，包含：
- 遊戲熱度指數計算
- 排行榜快照生成
- 指標數據查詢
- 熱度分析

### 2. IInsightService
洞察貼文管理接口，包含：
- 洞察貼文的 CRUD 操作
- 貼文發布和狀態管理
- 置頂功能
- 熱門洞察查詢

### 3. IForumService
論壇功能接口，包含：
- 論壇和主題管理
- 回覆系統
- 搜索功能
- 熱門主題

### 4. IInteractionService
用戶互動接口，包含：
- 反應（讚、表情）管理
- 收藏功能
- 互動統計
- 用戶行為追蹤

## 數據傳輸對象 (DTOs)

### 熱度相關 DTOs
- **`PopularityIndexResponseDto`** - 遊戲熱度指數響應
- **`LeaderboardResponseDto`** - 排行榜響應
- **`GameMetricResponseDto`** - 遊戲指標響應
- **`PopularityAnalysisResponseDto`** - 熱度分析響應

### 洞察相關 DTOs
- **`CreateInsightRequestDto`** - 創建洞察請求
- **`InsightResponseDto`** - 洞察響應
- **`InsightListResponseDto`** - 洞察列表響應
- **`InsightSearchRequestDto`** - 洞察搜索請求

### 論壇相關 DTOs
- **`CreateThreadRequestDto`** - 創建主題請求
- **`ThreadResponseDto`** - 主題響應
- **`CreatePostRequestDto`** - 創建回覆請求
- **`PostResponseDto`** - 回覆響應

### 互動相關 DTOs
- **`AddReactionRequestDto`** - 添加反應請求
- **`ReactionResponseDto`** - 反應響應
- **`AddBookmarkRequestDto`** - 添加收藏請求
- **`BookmarkResponseDto`** - 收藏響應

## API 端點設計

### 熱度 API (`/api/popularity`)
- `GET /games` - 獲取所有遊戲
- `GET /games/{gameId}/popularity` - 獲取遊戲熱度指數
- `GET /leaderboards` - 獲取排行榜
- `GET /games/{gameId}/metrics` - 獲取遊戲指標數據
- `GET /metric-sources` - 獲取指標來源
- `GET /metrics` - 獲取指標定義
- `POST /analysis` - 獲取熱度分析
- `POST /games/{gameId}/calculate-index` - 手動觸發熱度計算
- `POST /leaderboards/generate` - 手動觸發排行榜生成

### 洞察 API (待實現)
- `GET /insights` - 獲取洞察列表
- `GET /insights/{id}` - 獲取洞察詳情
- `POST /insights` - 創建洞察
- `PUT /insights/{id}` - 更新洞察
- `DELETE /insights/{id}` - 刪除洞察
- `POST /insights/{id}/publish` - 發布洞察
- `POST /insights/{id}/pin` - 設置/取消置頂

### 論壇 API (待實現)
- `GET /forums` - 獲取論壇列表
- `GET /forums/{id}/threads` - 獲取論壇主題
- `GET /threads/{id}` - 獲取主題詳情
- `POST /threads` - 創建主題
- `GET /threads/{id}/posts` - 獲取主題回覆
- `POST /threads/{id}/posts` - 創建回覆

### 互動 API (待實現)
- `POST /reactions` - 添加反應
- `DELETE /reactions` - 移除反應
- `GET /reactions/stats` - 獲取反應統計
- `POST /bookmarks` - 添加收藏
- `DELETE /bookmarks` - 移除收藏
- `GET /bookmarks` - 獲取用戶收藏

## 業務邏輯實現

### 1. 熱度指數計算
```csharp
private decimal CalculateWeightedIndex(List<GameMetricDaily> metrics)
{
    // 定義不同指標的權重
    var weights = new Dictionary<string, decimal>
    {
        { "concurrent_users", 0.4m },    // 同時在線用戶數 40%
        { "forum_posts", 0.2m },         // 論壇發文數 20%
        { "social_mentions", 0.15m },    // 社群提及 15%
        { "stream_viewers", 0.15m },     // 直播觀看 15%
        { "news_articles", 0.1m }        // 新聞文章 10%
    };

    // 計算加權平均值
    decimal totalWeightedValue = 0;
    decimal totalWeight = 0;

    foreach (var metric in metrics)
    {
        if (weights.TryGetValue(metric.Metric.code, out var weight))
        {
            totalWeightedValue += metric.value * weight;
            totalWeight += weight;
        }
    }

    return totalWeight > 0 ? totalWeightedValue / totalWeight : 0;
}
```

### 2. 排行榜快照生成
```csharp
public async Task GenerateLeaderboardSnapshotAsync(string period, DateTime timestamp)
{
    // 根據時段獲取相應的熱度指數數據
    var startDate = GetStartDateForPeriod(period, timestamp);
    var endDate = timestamp;

    var gamePopularity = await GetGamePopularityForPeriod(startDate, endDate);

    // 生成排行榜快照
    var rank = 1;
    var snapshots = new List<LeaderboardSnapshot>();

    foreach (var game in gamePopularity)
    {
        var snapshot = new LeaderboardSnapshot
        {
            period = period,
            ts = timestamp,
            rank = rank++,
            game_id = game.GameId,
            index_value = game.AverageIndexValue,
            created_at = DateTime.UtcNow
        };

        snapshots.Add(snapshot);
    }

    _context.LeaderboardSnapshots.AddRange(snapshots);
    await _context.SaveChangesAsync();
}
```

### 3. 洞察貼文發布流程
```csharp
public async Task<Post> PublishInsightAsync(int postId)
{
    var post = await _context.Posts
        .FirstOrDefaultAsync(p => p.post_id == postId && p.type == "insight");

    if (post == null)
        throw new InvalidOperationException($"洞察貼文 {postId} 不存在");

    // 檢查是否關聯遊戲，如果是則創建指標快照
    if (post.game_id.HasValue)
    {
        await CreateMetricSnapshotAsync(post);
    }

    // 更新狀態
    post.status = "published";
    post.published_at = DateTime.UtcNow;
    post.updated_at = DateTime.UtcNow;

    await _context.SaveChangesAsync();
    return post;
}
```

## 假資料生成

### 遊戲數據
- 15個熱門遊戲，涵蓋不同類型（MOBA、RPG、FPS、Action等）
- 每個遊戲包含多個指標來源的對應關係

### 指標數據
- 8個指標來源（Steam、YouTube、Twitch、Reddit等）
- 8個核心指標（同時在線用戶、影片觀看、直播觀看等）
- 最近30天的每日指標數據

### 熱度指數
- 基於加權算法計算的每日熱度指數
- 考慮多個指標的綜合影響

### 排行榜快照
- 週榜快照，包含前20名遊戲
- 基於7天平均熱度指數排序

### 社群內容
- 每個遊戲的專屬論壇
- 豐富的主題和回覆內容
- 洞察貼文和引用來源
- 用戶反應和收藏數據

## 測試覆蓋

### 單元測試
- **`PopularityServiceTests`** - 測試熱度服務核心邏輯
- 包含遊戲查詢、熱度指數計算、排行榜生成等測試
- 使用 In-Memory 數據庫進行測試

### 測試案例
1. **基本功能測試**
   - 獲取遊戲列表
   - 獲取遊戲熱度指數
   - 獲取排行榜快照

2. **業務邏輯測試**
   - 熱度指數計算
   - 排行榜快照生成
   - 指標數據查詢

3. **邊界情況測試**
   - 空數據處理
   - 無效遊戲ID
   - 日期範圍驗證

## 部署和配置

### 數據庫配置
- 使用 Entity Framework Core 進行數據訪問
- 配置了完整的實體關係和索引
- 支持多種數據庫提供者

### 依賴注入
```csharp
// Program.cs 或 Startup.cs
services.AddScoped<IPopularityService, PopularityService>();
services.AddScoped<IInsightService, InsightService>();
services.AddScoped<IForumService, ForumService>();
services.AddScoped<IInteractionService, InteractionService>();
```

### 假資料初始化
```csharp
// 在應用啟動時調用
await fakeDataService.GenerateFakeDataAsync();
```

## 性能優化

### 數據庫優化
- 複合索引優化查詢性能
- 適當的外鍵約束保證數據完整性
- 分頁查詢避免大量數據載入

### 緩存策略
- 排行榜快照避免重複計算
- 熱度指數每日計算一次
- 指標數據按需載入

### 查詢優化
- 使用 Include 進行關聯查詢
- 分頁查詢限制結果集大小
- 索引優化常用查詢路徑

## 安全考慮

### 數據驗證
- 輸入參數驗證和清理
- 日期範圍限制
- 用戶權限檢查

### 訪問控制
- API 端點權限控制
- 用戶身份驗證
- 敏感操作審計

### 數據保護
- 軟刪除避免數據丟失
- 外鍵約束保證數據一致性
- 事務處理確保操作原子性

## 監控和日誌

### 日誌記錄
- 使用 ILogger 記錄關鍵操作
- 錯誤和異常詳細記錄
- 性能指標追蹤

### 監控指標
- API 響應時間
- 數據庫查詢性能
- 錯誤率和成功率

## 未來擴展

### 功能擴展
1. **實時數據更新** - 支持實時指標數據更新
2. **預測分析** - 基於歷史數據的趨勢預測
3. **自定義指標** - 支持用戶自定義指標權重
4. **多語言支持** - 國際化內容支持

### 技術改進
1. **分布式緩存** - Redis 緩存熱門數據
2. **消息隊列** - 異步處理大量數據更新
3. **微服務架構** - 模組化部署和擴展
4. **機器學習** - AI 驅動的熱度預測

### 數據源擴展
1. **更多平台** - 支持更多遊戲平台和社群
2. **API 集成** - 與第三方數據提供者集成
3. **爬蟲系統** - 自動化數據收集
4. **數據清洗** - 更智能的數據質量控制

## 總結

Stage 3 成功實現了完整的遊戲熱度與洞察系統，建立了：

1. **完整的數據模型** - 涵蓋遊戲、指標、熱度、內容、互動等各個方面
2. **強大的業務邏輯** - 熱度計算、排行榜生成、內容管理等核心功能
3. **豐富的假資料** - 15個遊戲、30天數據、完整的社群內容
4. **全面的測試覆蓋** - 單元測試驗證核心邏輯
5. **可擴展的架構** - 支持未來功能擴展和性能優化

這個階段為遊戲平台提供了強大的數據分析能力，能夠幫助開發者和運營者了解遊戲熱度趨勢，優化用戶體驗，並建立活躍的遊戲社群。