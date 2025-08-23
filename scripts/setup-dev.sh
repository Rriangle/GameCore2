#!/bin/bash

# GameCore 開發環境設定腳本
# 適用於 Linux/macOS

set -e

echo "🚀 開始設定 GameCore 開發環境..."

# 檢查必要工具
check_requirements() {
    echo "📋 檢查必要工具..."
    
    # 檢查 .NET
    if ! command -v dotnet &> /dev/null; then
        echo "❌ .NET 8.0 SDK 未安裝"
        echo "請前往 https://dotnet.microsoft.com/download/dotnet/8.0 下載安裝"
        exit 1
    fi
    
    # 檢查 Node.js
    if ! command -v node &> /dev/null; then
        echo "❌ Node.js 未安裝"
        echo "請前往 https://nodejs.org/ 下載安裝"
        exit 1
    fi
    
    # 檢查 Docker
    if ! command -v docker &> /dev/null; then
        echo "❌ Docker 未安裝"
        echo "請前往 https://www.docker.com/ 下載安裝"
        exit 1
    fi
    
    # 檢查 Docker Compose
    if ! command -v docker-compose &> /dev/null; then
        echo "❌ Docker Compose 未安裝"
        echo "請前往 https://docs.docker.com/compose/install/ 下載安裝"
        exit 1
    fi
    
    echo "✅ 所有必要工具已安裝"
}

# 設定環境變數
setup_environment() {
    echo "🔧 設定環境變數..."
    
    if [ ! -f .env ]; then
        cp .env.sample .env
        echo "✅ 已建立 .env 檔案"
        echo "⚠️  請編輯 .env 檔案，填入必要的配置"
    else
        echo "✅ .env 檔案已存在"
    fi
}

# 啟動資料庫
start_database() {
    echo "🗄️  啟動 SQL Server 資料庫..."
    
    docker-compose -f docker-compose.dev.yml up -d sqlserver
    
    echo "⏳ 等待資料庫啟動..."
    sleep 30
    
    # 檢查資料庫狀態
    if docker-compose -f docker-compose.dev.yml ps sqlserver | grep -q "Up"; then
        echo "✅ SQL Server 已啟動"
    else
        echo "❌ SQL Server 啟動失敗"
        exit 1
    fi
}

# 初始化資料庫
init_database() {
    echo "📊 初始化資料庫..."
    
    # 執行資料庫初始化腳本
    docker exec -i gamecore-sqlserver /opt/mssql-tools/bin/sqlcmd \
        -S localhost -U sa -P YourStrong@Passw0rd \
        -i scripts/init-database.sql
    
    echo "✅ 資料庫結構已建立"
    
    # 執行假資料 Seeder
    docker exec -i gamecore-sqlserver /opt/mssql-tools/bin/sqlcmd \
        -S localhost -U sa -P YourStrong@Passw0rd \
        -i scripts/seed-data.sql
    
    echo "✅ 假資料已填充"
}

# 安裝前端依賴
install_frontend_deps() {
    echo "📦 安裝前端依賴..."
    
    cd frontend
    
    # 檢查 pnpm
    if ! command -v pnpm &> /dev/null; then
        echo "📦 安裝 pnpm..."
        npm install -g pnpm
    fi
    
    # 安裝依賴
    pnpm install
    
    echo "✅ 前端依賴已安裝"
    cd ..
}

# 建置專案
build_project() {
    echo "🔨 建置專案..."
    
    # 建置後端
    echo "建置後端..."
    dotnet restore GameCore.sln
    dotnet build GameCore.sln --configuration Release
    
    # 建置前端
    echo "建置前端..."
    cd frontend
    pnpm build
    cd ..
    
    echo "✅ 專案建置完成"
}

# 執行測試
run_tests() {
    echo "🧪 執行測試..."
    
    # 後端測試
    echo "執行後端測試..."
    dotnet test tests/GameCore.Tests/GameCore.Tests.csproj
    
    # 前端測試
    echo "執行前端測試..."
    cd frontend
    pnpm test:run
    cd ..
    
    echo "✅ 所有測試通過"
}

# 啟動開發環境
start_dev_environment() {
    echo "🚀 啟動開發環境..."
    
    # 啟動所有服務
    docker-compose -f docker-compose.dev.yml up -d
    
    echo "✅ 開發環境已啟動"
    echo ""
    echo "🌐 服務地址："
    echo "  前端應用：http://localhost:3000"
    echo "  後端 API：http://localhost:5000"
    echo "  API 文件：http://localhost:5000/swagger"
    echo "  資料庫管理：http://localhost:8080"
    echo ""
    echo "📝 開發指令："
    echo "  查看服務狀態：docker-compose -f docker-compose.dev.yml ps"
    echo "  查看日誌：docker-compose -f docker-compose.dev.yml logs -f"
    echo "  停止服務：docker-compose -f docker-compose.dev.yml down"
}

# 主函數
main() {
    echo "🎮 GameCore 開發環境設定"
    echo "=========================="
    
    check_requirements
    setup_environment
    start_database
    init_database
    install_frontend_deps
    build_project
    run_tests
    start_dev_environment
    
    echo ""
    echo "🎉 開發環境設定完成！"
    echo "開始享受 GameCore 的開發之旅吧！"
}

# 執行主函數
main "$@"