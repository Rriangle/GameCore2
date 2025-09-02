# GameCore 專案部署指南

## 概述
本指南詳細說明如何將 GameCore 遊戲社群平台部署到 GitHub 和生產環境。包含前端部署、後端部署、資料庫配置等完整流程。

---

## 目錄
1. [GitHub 部署](#github-部署)
2. [本地開發環境](#本地開發環境)
3. [生產環境部署](#生產環境部署)
4. [資料庫部署](#資料庫部署)
5. [監控與維護](#監控與維護)
6. [故障排除](#故障排除)

---

## GitHub 部署

### 1.1 準備 GitHub 倉庫

#### 步驟 1：建立 GitHub 倉庫
```bash
# 在 GitHub 上建立新的倉庫
# 倉庫名稱：GameCore
# 描述：GameCore 遊戲社群平台
# 可見性：Public 或 Private
```

#### 步驟 2：初始化本地 Git 倉庫
```bash
# 進入專案目錄
cd D:\GameCore2

# 初始化 Git 倉庫（如果尚未初始化）
git init

# 添加遠端倉庫
git remote add origin https://github.com/your-username/GameCore.git

# 檢查遠端倉庫
git remote -v
```

#### 步驟 3：推送程式碼到 GitHub
```bash
# 添加所有檔案
git add .

# 提交變更
git commit -m "Initial commit: GameCore project setup"

# 推送到主分支
git push -u origin main
```

### 1.2 GitHub Pages 部署（前端）

#### 步驟 1：設定 GitHub Pages
1. 進入 GitHub 倉庫設定頁面
2. 找到 "Pages" 選項
3. 選擇 "Deploy from a branch"
4. 選擇 "main" 分支和 "/docs" 資料夾
5. 點擊 "Save"

#### 步驟 2：準備前端檔案
```bash
# 建立 docs 資料夾（GitHub Pages 根目錄）
mkdir docs

# 複製前端檔案到 docs 資料夾
cp index.html docs/
cp styles.css docs/
cp components.css docs/
cp animations.css docs/
cp app.js docs/
cp pet.js docs/
cp forum.js docs/
cp market.js docs/
cp auth.js docs/
cp data.js docs/
```

#### 步驟 3：建立 GitHub Pages 設定檔
```yaml
# docs/.github/workflows/deploy.yml
name: Deploy to GitHub Pages

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    
    - name: Deploy to GitHub Pages
      uses: peaceiris/actions-gh-pages@v3
      with:
        github_token: ${{ secrets.GITHUB_TOKEN }}
        publish_dir: ./docs
```

### 1.3 GitHub Actions CI/CD

#### 建立 CI/CD 工作流程
```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Upload artifacts
      uses: actions/upload-artifact@v2
      with:
        name: gamecore-api
        path: ./publish
```

---

## 本地開發環境

### 2.1 環境需求

#### 必要軟體
- **Visual Studio 2022** 或 **Visual Studio Code**
- **.NET 8.0 SDK**
- **SQL Server 2022** 或 **SQL Server Express**
- **Git**
- **Node.js** (用於前端開發工具)

#### 系統需求
- **作業系統：** Windows 10/11, macOS, Linux
- **記憶體：** 最少 8GB RAM
- **硬碟空間：** 最少 10GB 可用空間
- **網路：** 穩定的網際網路連線

### 2.2 環境設定

#### 步驟 1：安裝 .NET 8.0 SDK
```bash
# 下載並安裝 .NET 8.0 SDK
# 網址：https://dotnet.microsoft.com/download/dotnet/8.0

# 驗證安裝
dotnet --version
```

#### 步驟 2：設定 SQL Server
```bash
# 安裝 SQL Server 2022 Express
# 網址：https://www.microsoft.com/sql-server/sql-server-downloads

# 啟動 SQL Server 服務
net start MSSQLSERVER

# 或使用 SQL Server Configuration Manager
```

#### 步驟 3：設定資料庫連線
```json
// src/GameCore.Api/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GameCoreDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 2.3 專案建置與執行

#### 步驟 1：還原套件
```bash
# 進入專案目錄
cd D:\GameCore2

# 還原 NuGet 套件
dotnet restore
```

#### 步驟 2：建置專案
```bash
# 建置整個解決方案
dotnet build

# 或建置特定專案
dotnet build src/GameCore.Api/GameCore.Api.csproj
```

#### 步驟 3：執行專案
```bash
# 執行 API 專案
cd src/GameCore.Api
dotnet run

# 或使用 Visual Studio
# 按 F5 或 Ctrl+F5
```

#### 步驟 4：執行資料庫遷移
```bash
# 安裝 Entity Framework 工具
dotnet tool install --global dotnet-ef

# 建立資料庫
dotnet ef database update

# 或使用 Package Manager Console
Update-Database
```

---

## 生產環境部署

### 3.1 伺服器準備

#### 硬體需求
- **CPU：** 最少 4 核心
- **記憶體：** 最少 16GB RAM
- **硬碟：** 最少 100GB SSD
- **網路：** 穩定的網際網路連線

#### 軟體需求
- **作業系統：** Windows Server 2022
- **Web 伺服器：** IIS 10.0
- **資料庫：** SQL Server 2022
- **.NET 8.0 Runtime**

### 3.2 IIS 部署

#### 步驟 1：安裝 IIS
```powershell
# 安裝 IIS 和必要功能
Install-WindowsFeature -Name Web-Server -IncludeManagementTools
Install-WindowsFeature -Name Web-Asp-Net45
Install-WindowsFeature -Name Web-Net-Ext45
```

#### 步驟 2：安裝 .NET 8.0 Runtime
```bash
# 下載並安裝 .NET 8.0 Runtime
# 網址：https://dotnet.microsoft.com/download/dotnet/8.0
```

#### 步驟 3：建立應用程式集區
1. 開啟 IIS Manager
2. 右鍵點擊 "Application Pools"
3. 選擇 "Add Application Pool"
4. 設定名稱：GameCorePool
5. .NET CLR Version：No Managed Code
6. Managed Pipeline Mode：Integrated

#### 步驟 4：建立網站
1. 右鍵點擊 "Sites"
2. 選擇 "Add Website"
3. 網站名稱：GameCore
4. 應用程式集區：GameCorePool
5. 實體路徑：C:\inetpub\wwwroot\GameCore
6. 綁定：http://localhost:80

#### 步驟 5：部署應用程式
```bash
# 發布應用程式
dotnet publish -c Release -o C:\inetpub\wwwroot\GameCore

# 設定權限
icacls "C:\inetpub\wwwroot\GameCore" /grant "IIS_IUSRS:(OI)(CI)F"
```

### 3.3 Docker 部署

#### 建立 Dockerfile
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/GameCore.Api/GameCore.Api.csproj", "src/GameCore.Api/"]
COPY ["src/GameCore.Shared/GameCore.Shared.csproj", "src/GameCore.Shared/"]
COPY ["src/GameCore.Domain/GameCore.Domain.csproj", "src/GameCore.Domain/"]
COPY ["src/GameCore.Infrastructure/GameCore.Infrastructure.csproj", "src/GameCore.Infrastructure/"]
RUN dotnet restore "src/GameCore.Api/GameCore.Api.csproj"
COPY . .
WORKDIR "/src/src/GameCore.Api"
RUN dotnet build "GameCore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameCore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameCore.Api.dll"]
```

#### 建立 Docker Compose
```yaml
# docker-compose.yml
version: '3.8'

services:
  gamecore-api:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=gamecore-db;Database=GameCoreDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;
    depends_on:
      - gamecore-db

  gamecore-db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"
    volumes:
      - gamecore-data:/var/opt/mssql

volumes:
  gamecore-data:
```

#### 執行 Docker 部署
```bash
# 建置並執行
docker-compose up -d

# 查看日誌
docker-compose logs -f

# 停止服務
docker-compose down
```

---

## 資料庫部署

### 4.1 SQL Server 設定

#### 步驟 1：安裝 SQL Server
```bash
# 下載 SQL Server 2022
# 網址：https://www.microsoft.com/sql-server/sql-server-downloads

# 安裝時選擇：
# - Database Engine Services
# - SQL Server Replication
# - Full-Text and Semantic Extractions for Search
# - Client Tools Connectivity
# - Integration Services
# - Client Tools Backwards Compatibility
# - Client Tools SDK
# - SQL Client Connectivity SDK
# - SQL Server Integration Services
```

#### 步驟 2：設定 SQL Server
```sql
-- 啟用 TCP/IP 協議
-- 1. 開啟 SQL Server Configuration Manager
-- 2. 展開 SQL Server Network Configuration
-- 3. 右鍵點擊 Protocols for MSSQLSERVER
-- 4. 啟用 TCP/IP

-- 設定防火牆規則
-- 新增 TCP 1433 端口的防火牆規則

-- 建立資料庫
CREATE DATABASE GameCoreDB;
GO

-- 建立登入帳號
CREATE LOGIN gamecore_user WITH PASSWORD = 'YourPassword123!';
GO

-- 建立資料庫使用者
USE GameCoreDB;
CREATE USER gamecore_user FOR LOGIN gamecore_user;
GO

-- 授權
GRANT CONTROL SERVER TO gamecore_user;
GO
```

### 4.2 資料庫遷移

#### 步驟 1：建立遷移
```bash
# 建立初始遷移
dotnet ef migrations add InitialCreate

# 建立其他遷移
dotnet ef migrations add AddUserTable
dotnet ef migrations add AddGameTable
dotnet ef migrations add AddForumTable
```

#### 步驟 2：執行遷移
```bash
# 更新資料庫
dotnet ef database update

# 或指定特定遷移
dotnet ef database update MigrationName
```

#### 步驟 3：生成假資料
```bash
# 執行假資料生成腳本
# 參考 docs/SQL_DATA_GENERATION.md
```

### 4.3 資料庫備份與還原

#### 建立備份策略
```sql
-- 建立完整備份
BACKUP DATABASE GameCoreDB 
TO DISK = 'C:\Backups\GameCoreDB_Full.bak'
WITH FORMAT, INIT, NAME = 'GameCoreDB-Full Database Backup';

-- 建立差異備份
BACKUP DATABASE GameCoreDB 
TO DISK = 'C:\Backups\GameCoreDB_Diff.bak'
WITH DIFFERENTIAL, NAME = 'GameCoreDB-Differential Database Backup';

-- 建立交易記錄備份
BACKUP LOG GameCoreDB 
TO DISK = 'C:\Backups\GameCoreDB_Log.trn'
WITH NAME = 'GameCoreDB-Transaction Log Backup';
```

#### 自動化備份腳本
```sql
-- 建立備份作業
USE msdb;
GO

EXEC dbo.sp_add_job
    @job_name = N'GameCoreDB_Backup_Job',
    @enabled = 1,
    @description = N'GameCore 資料庫自動備份作業';

EXEC dbo.sp_add_jobstep
    @job_name = N'GameCoreDB_Backup_Job',
    @step_name = N'Full Backup',
    @subsystem = N'TSQL',
    @command = N'
BACKUP DATABASE GameCoreDB 
TO DISK = ''C:\Backups\GameCoreDB_Full_$(Get-Date -Format "yyyyMMdd_HHmmss").bak''
WITH FORMAT, INIT, NAME = ''GameCoreDB-Full Database Backup'';
',
    @database_name = N'master';

EXEC dbo.sp_add_schedule
    @schedule_name = N'Daily_Backup_Schedule',
    @freq_type = 4,  -- Daily
    @freq_interval = 1,
    @active_start_time = 020000;  -- 2:00 AM

EXEC dbo.sp_attach_schedule
    @job_name = N'GameCoreDB_Backup_Job',
    @schedule_name = N'Daily_Backup_Schedule';
```

---

## 監控與維護

### 5.1 應用程式監控

#### 設定應用程式日誌
```json
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    },
    "Console": {
      "IncludeScopes": true
    },
    "File": {
      "Path": "logs/gamecore-{Date}.txt",
      "LogLevel": {
        "Default": "Information"
      }
    }
  }
}
```

#### 設定健康檢查
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddCheck("api", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health");
```

### 5.2 效能監控

#### 設定效能計數器
```csharp
// 在 Startup.cs 中設定
services.AddMetrics();

// 在控制器中使用
[HttpGet("metrics")]
public IActionResult GetMetrics()
{
    var metrics = new
    {
        RequestCount = _metrics.GetCounter("requests_total"),
        ResponseTime = _metrics.GetHistogram("response_time"),
        ActiveConnections = _metrics.GetGauge("active_connections")
    };
    
    return Ok(metrics);
}
```

### 5.3 錯誤處理

#### 全域錯誤處理
```csharp
// Program.cs
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 自訂錯誤頁面
app.Map("/Error", () => Results.Problem("An error occurred."));
```

---

## 故障排除

### 6.1 常見問題

#### 問題 1：資料庫連線失敗
```bash
# 錯誤訊息：A network-related or instance-specific error occurred

# 解決方案：
# 1. 檢查 SQL Server 服務是否啟動
net start MSSQLSERVER

# 2. 檢查連線字串
# 3. 檢查防火牆設定
# 4. 檢查 SQL Server 設定
```

#### 問題 2：API 無法存取
```bash
# 錯誤訊息：HTTP 500 Internal Server Error

# 解決方案：
# 1. 檢查應用程式日誌
# 2. 檢查資料庫連線
# 3. 檢查設定檔
# 4. 重新啟動應用程式
```

#### 問題 3：前端無法載入
```bash
# 錯誤訊息：Failed to load resource

# 解決方案：
# 1. 檢查 API 端點是否正確
# 2. 檢查 CORS 設定
# 3. 檢查網路連線
# 4. 清除瀏覽器快取
```

### 6.2 日誌分析

#### 查看應用程式日誌
```bash
# Windows 事件檢視器
eventvwr.msc

# 應用程式日誌檔案
tail -f logs/gamecore-2024-12-19.txt

# IIS 日誌
C:\inetpub\logs\LogFiles\W3SVC1\
```

#### 資料庫日誌
```sql
-- 查看 SQL Server 錯誤日誌
EXEC sp_readerrorlog;

-- 查看特定錯誤
EXEC sp_readerrorlog 0, 1, 'error';

-- 查看慢查詢
SELECT 
    qs.execution_count,
    qs.total_elapsed_time / qs.execution_count AS avg_elapsed_time,
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2) + 1) AS statement_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qs.total_elapsed_time / qs.execution_count > 1000
ORDER BY avg_elapsed_time DESC;
```

### 6.3 效能調優

#### 資料庫效能調優
```sql
-- 建立索引
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Threads_ForumID ON Threads(ForumID);
CREATE INDEX IX_Posts_ThreadID ON Posts(ThreadID);

-- 更新統計資訊
UPDATE STATISTICS Users;
UPDATE STATISTICS Threads;
UPDATE STATISTICS Posts;

-- 重建索引
ALTER INDEX ALL ON Users REBUILD;
ALTER INDEX ALL ON Threads REBUILD;
ALTER INDEX ALL ON Posts REBUILD;
```

#### 應用程式效能調優
```csharp
// 啟用快取
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

// 設定快取策略
app.UseResponseCaching();

// 在控制器中使用快取
[ResponseCache(Duration = 300)]
public async Task<IActionResult> GetPopularGames()
{
    // 實作邏輯
}
```

---

## 總結

本部署指南涵蓋了 GameCore 專案的完整部署流程，包括：

1. **GitHub 部署** - 前端和 CI/CD 流程
2. **本地開發環境** - 開發者環境設定
3. **生產環境部署** - IIS 和 Docker 部署
4. **資料庫部署** - SQL Server 設定和遷移
5. **監控與維護** - 應用程式監控和維護
6. **故障排除** - 常見問題解決方案

### 部署檢查清單

- [ ] GitHub 倉庫建立完成
- [ ] 前端檔案部署到 GitHub Pages
- [ ] CI/CD 工作流程設定完成
- [ ] 本地開發環境設定完成
- [ ] 資料庫安裝和設定完成
- [ ] 生產環境部署完成
- [ ] 監控系統設定完成
- [ ] 備份策略實施完成
- [ ] 效能測試通過
- [ ] 安全性檢查通過

### 聯絡資訊

如有任何問題或需要協助，請聯絡：
- **專案負責人：** GameCore 開發團隊
- **技術支援：** AI Assistant
- **文件版本：** v1.0
- **更新日期：** 2024年12月19日

---

**部署指南版本：** v1.0  
**建立日期：** 2024年12月19日  
**適用版本：** GameCore v1.0  
**專案狀態：** ✅ 完成並可部署 