# GameCore 部署指南

## 📋 目錄

1. [環境需求](#環境需求)
2. [本地開發環境](#本地開發環境)
3. [Docker 部署](#docker-部署)
4. [Azure 部署](#azure-部署)
5. [AWS 部署](#aws-部署)
6. [GitHub Pages 部署](#github-pages-部署)
7. [Vercel 部署](#vercel-部署)
8. [生產環境配置](#生產環境配置)
9. [監控與維護](#監控與維護)

## 🔧 環境需求

### 最低需求
- **作業系統**: Windows 10/11, macOS 10.15+, Ubuntu 18.04+
- **.NET**: .NET 8.0 SDK
- **資料庫**: SQL Server 2019+ 或 SQL Server Express
- **記憶體**: 4GB RAM
- **儲存空間**: 10GB 可用空間

### 建議需求
- **作業系統**: Windows 11, Ubuntu 20.04+
- **.NET**: .NET 8.0 SDK
- **資料庫**: SQL Server 2022
- **記憶體**: 8GB RAM
- **儲存空間**: 50GB 可用空間
- **CPU**: 4 核心以上

## 🏠 本地開發環境

### 1. 安裝必要工具

#### Windows
```powershell
# 安裝 .NET 8.0 SDK
winget install Microsoft.DotNet.SDK.8

# 安裝 SQL Server Express
winget install Microsoft.SQLServer.2019.Express

# 安裝 Visual Studio 2022 Community
winget install Microsoft.VisualStudio.2022.Community
```

#### macOS
```bash
# 安裝 .NET 8.0 SDK
brew install dotnet

# 安裝 Docker Desktop
brew install --cask docker
```

#### Ubuntu
```bash
# 安裝 .NET 8.0 SDK
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# 安裝 Docker
sudo apt-get install docker.io
sudo systemctl start docker
sudo systemctl enable docker
```

### 2. 專案設定

```bash
# 克隆專案
git clone https://github.com/your-username/gamecore.git
cd gamecore

# 還原套件
dotnet restore

# 設定資料庫連接
cp src/GameCore.Api/appsettings.Development.json src/GameCore.Api/appsettings.json
```

### 3. 資料庫設定

編輯 `src/GameCore.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GameCore;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 4. 執行專案

```bash
# 建立資料庫
cd src/GameCore.Api
dotnet ef database update

# 執行專案
dotnet run

# 或使用 Visual Studio
# 開啟 GameCore.sln 並按 F5
```

## 🐳 Docker 部署

### 1. 建立 Dockerfile

```dockerfile
# 使用官方 .NET 8.0 映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 建立階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製專案檔案
COPY ["src/GameCore.Api/GameCore.Api.csproj", "src/GameCore.Api/"]
COPY ["src/GameCore.Domain/GameCore.Domain.csproj", "src/GameCore.Domain/"]
COPY ["src/GameCore.Infrastructure/GameCore.Infrastructure.csproj", "src/GameCore.Infrastructure/"]
COPY ["src/GameCore.Shared/GameCore.Shared.csproj", "src/GameCore.Shared/"]

# 還原套件
RUN dotnet restore "src/GameCore.Api/GameCore.Api.csproj"

# 複製所有檔案
COPY . .

# 建立應用程式
RUN dotnet build "src/GameCore.Api/GameCore.Api.csproj" -c Release -o /app/build

# 發布階段
FROM build AS publish
RUN dotnet publish "src/GameCore.Api/GameCore.Api.csproj" -c Release -o /app/publish

# 最終階段
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 設定環境變數
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "GameCore.Api.dll"]
```

### 2. 建立 docker-compose.yml

```yaml
version: '3.8'

services:
  app:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=GameCore;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
    depends_on:
      - db
    volumes:
      - ./wwwroot:/app/wwwroot

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

volumes:
  sqlserver_data:
```

### 3. 執行 Docker 部署

```bash
# 建立並啟動容器
docker-compose up -d

# 查看日誌
docker-compose logs -f

# 停止容器
docker-compose down
```

## ☁️ Azure 部署

### 1. 安裝 Azure CLI

```bash
# Windows
winget install Microsoft.AzureCLI

# macOS
brew install azure-cli

# Ubuntu
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### 2. 登入 Azure

```bash
az login
```

### 3. 建立資源群組

```bash
az group create --name gamecore-rg --location eastus
```

### 4. 建立 App Service Plan

```bash
az appservice plan create \
  --name gamecore-plan \
  --resource-group gamecore-rg \
  --sku B1 \
  --is-linux
```

### 5. 建立 Web App

```bash
az webapp create \
  --name gamecore-app \
  --resource-group gamecore-rg \
  --plan gamecore-plan \
  --runtime "DOTNETCORE:8.0"
```

### 6. 設定應用程式設定

```bash
az webapp config appsettings set \
  --name gamecore-app \
  --resource-group gamecore-rg \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Server=your-sql-server.database.windows.net;Database=GameCore;User Id=your-username;Password=your-password;"
```

### 7. 部署應用程式

```bash
# 發布應用程式
dotnet publish -c Release -o ./publish

# 部署到 Azure
az webapp deployment source config-zip \
  --resource-group gamecore-rg \
  --name gamecore-app \
  --src ./publish.zip
```

## ☁️ AWS 部署

### 1. 安裝 AWS CLI

```bash
# Windows
winget install Amazon.AWSCLI

# macOS
brew install awscli

# Ubuntu
sudo apt-get install awscli
```

### 2. 設定 AWS 認證

```bash
aws configure
```

### 3. 建立 Elastic Beanstalk 應用程式

```bash
# 建立應用程式
aws elasticbeanstalk create-application --application-name gamecore

# 建立環境
aws elasticbeanstalk create-environment \
  --application-name gamecore \
  --environment-name gamecore-prod \
  --solution-stack-name "64bit Amazon Linux 2 v2.5.0 running .NET Core"
```

### 4. 建立部署套件

```bash
# 發布應用程式
dotnet publish -c Release -o ./publish

# 建立部署套件
cd publish
zip -r ../gamecore-deploy.zip .
cd ..
```

### 5. 部署應用程式

```bash
aws elasticbeanstalk create-application-version \
  --application-name gamecore \
  --version-label v1.0.0 \
  --source-bundle S3Bucket=your-bucket,S3Key=gamecore-deploy.zip

aws elasticbeanstalk update-environment \
  --environment-name gamecore-prod \
  --version-label v1.0.0
```

## 📄 GitHub Pages 部署

### 1. 建立 GitHub Actions 工作流程

建立 `.github/workflows/deploy.yml`:

```yaml
name: Deploy to GitHub Pages

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Deploy to GitHub Pages
      uses: peaceiris/actions-gh-pages@v3
      with:
        github_token: ${{ secrets.GITHUB_TOKEN }}
        publish_dir: ./wwwroot
```

### 2. 設定 GitHub Pages

1. 前往專案設定
2. 選擇 Pages 選項
3. 設定 Source 為 GitHub Actions

### 3. 推送變更

```bash
git add .
git commit -m "Add GitHub Pages deployment"
git push origin main
```

## ⚡ Vercel 部署

### 1. 安裝 Vercel CLI

```bash
npm install -g vercel
```

### 2. 建立 vercel.json

```json
{
  "version": 2,
  "builds": [
    {
      "src": "wwwroot/**",
      "use": "@vercel/static"
    }
  ],
  "routes": [
    {
      "src": "/(.*)",
      "dest": "/wwwroot/$1"
    }
  ]
}
```

### 3. 部署到 Vercel

```bash
vercel
```

## 🔧 生產環境配置

### 1. 環境變數設定

```bash
# 應用程式設定
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80

# 資料庫連接
ConnectionStrings__DefaultConnection=Server=your-server;Database=GameCore;User Id=your-user;Password=your-password;

# JWT 設定
JWT__SecretKey=your-super-secret-key-here
JWT__Issuer=GameCore
JWT__Audience=GameCoreUsers
JWT__ExpirationHours=24

# 快取設定
Redis__ConnectionString=your-redis-connection-string

# 檔案儲存
Storage__ConnectionString=your-storage-connection-string
```

### 2. 資料庫優化

```sql
-- 建立索引
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Posts_Category ON Posts(Category);
CREATE INDEX IX_Products_SellerId ON Products(SellerId);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);

-- 設定統計資訊
UPDATE STATISTICS Users;
UPDATE STATISTICS Posts;
UPDATE STATISTICS Products;
UPDATE STATISTICS Orders;
```

### 3. 快取配置

```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddMemoryCache();
```

### 4. 日誌配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  },
  "ApplicationInsights": {
    "ConnectionString": "your-application-insights-connection-string"
  }
}
```

## 📊 監控與維護

### 1. 應用程式監控

#### Azure Application Insights
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

#### 健康檢查
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddRedis(builder.Configuration.GetConnectionString("Redis"));
```

### 2. 效能監控

```csharp
// 效能計數器
builder.Services.AddMetrics();

// 分散式追蹤
builder.Services.AddDistributedTracing();
```

### 3. 錯誤處理

```csharp
// 全域錯誤處理
app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error/{0}");
```

### 4. 備份策略

```bash
# 資料庫備份腳本
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
sqlcmd -S localhost -d GameCore -E -Q "BACKUP DATABASE GameCore TO DISK = '/backup/GameCore_$DATE.bak'"
```

### 5. 監控告警

```yaml
# Azure Monitor 告警規則
- name: "High CPU Usage"
  condition: "CPU Percentage > 80"
  action: "Send email notification"

- name: "Database Connection Issues"
  condition: "Database connection failures > 5"
  action: "Send SMS notification"
```

## 🔒 安全性配置

### 1. HTTPS 設定

```csharp
// Program.cs
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 443;
});
```

### 2. CORS 設定

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

### 3. 速率限制

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

## 📈 效能優化

### 1. 資料庫優化

```sql
-- 查詢優化
SELECT * FROM Posts 
WHERE Category = @Category 
ORDER BY CreatedAt DESC 
OFFSET @Skip ROWS 
FETCH NEXT @Take ROWS ONLY;

-- 索引優化
CREATE NONCLUSTERED INDEX IX_Posts_Category_CreatedAt 
ON Posts(Category, CreatedAt DESC);
```

### 2. 快取策略

```csharp
// 快取熱門資料
[ResponseCache(Duration = 300)]
public async Task<IActionResult> GetPopularPosts()
{
    var cacheKey = "popular_posts";
    var posts = await _cache.GetOrSetAsync(cacheKey, 
        () => _postService.GetPopularPostsAsync(), 
        TimeSpan.FromMinutes(5));
    
    return Ok(posts);
}
```

### 3. 非同步處理

```csharp
// 非同步資料庫操作
public async Task<IEnumerable<Post>> GetPostsAsync()
{
    return await _context.Posts
        .Include(p => p.Author)
        .Include(p => p.Comments)
        .AsNoTracking()
        .ToListAsync();
}
```

## 🚀 持續整合/持續部署 (CI/CD)

### 1. GitHub Actions

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Deploy to Azure
      run: |
        az webapp deployment source config-zip \
          --resource-group gamecore-rg \
          --name gamecore-app \
          --src ./publish.zip
```

### 2. Azure DevOps

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'
  buildConfiguration: 'Release'

steps:
- task: DotNetCoreCLI@2
  inputs:
    command: 'restore'
    projects: '$(solution)'

- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    projects: '$(solution)'
    arguments: '--configuration $(buildConfiguration)'

- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    projects: '**/*Tests/*.csproj'
    arguments: '--configuration $(buildConfiguration)'

- task: DotNetCoreCLI@2
  inputs:
    command: 'publish'
    publishWebProjects: true
    arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
    zipAfterPublish: true

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: 'drop'
    publishLocation: 'Container'
```

## 📞 支援與維護

### 1. 常見問題

**Q: 如何重置資料庫？**
```bash
dotnet ef database drop
dotnet ef database update
```

**Q: 如何查看應用程式日誌？**
```bash
# Docker
docker-compose logs -f app

# Azure
az webapp log tail --name gamecore-app --resource-group gamecore-rg
```

**Q: 如何備份資料庫？**
```bash
sqlcmd -S localhost -d GameCore -E -Q "BACKUP DATABASE GameCore TO DISK = 'C:\backup\GameCore.bak'"
```

### 2. 聯絡支援

- **技術支援**: support@gamecore.com
- **文件**: https://docs.gamecore.com
- **GitHub Issues**: https://github.com/gamecore/issues

---

**注意**: 請根據您的實際環境調整配置參數。建議在生產環境部署前先在測試環境進行充分測試。 