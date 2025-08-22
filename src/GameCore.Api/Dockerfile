# 多階段建置 Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 建置階段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製專案檔案
COPY ["src/GameCore.Api/GameCore.Api.csproj", "src/GameCore.Api/"]
COPY ["src/GameCore.Domain/GameCore.Domain.csproj", "src/GameCore.Domain/"]
COPY ["src/GameCore.Infrastructure/GameCore.Infrastructure.csproj", "src/GameCore.Infrastructure/"]
COPY ["src/GameCore.Shared/GameCore.Shared.csproj", "src/GameCore.Shared/"]

# 還原套件
RUN dotnet restore "src/GameCore.Api/GameCore.Api.csproj"

# 複製所有原始碼
COPY . .

# 建置應用程式
WORKDIR "/src/src/GameCore.Api"
RUN dotnet build "GameCore.Api.csproj" -c Release -o /app/build

# 發布階段
FROM build AS publish
RUN dotnet publish "GameCore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 最終階段
FROM base AS final
WORKDIR /app

# 安裝 curl 用於健康檢查
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# 複製發布檔案
COPY --from=publish /app/publish .

# 建立日誌目錄
RUN mkdir -p /app/logs

# 設定環境變數
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_ENVIRONMENT=Production

# 健康檢查
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# 啟動應用程式
ENTRYPOINT ["dotnet", "GameCore.Api.dll"]
