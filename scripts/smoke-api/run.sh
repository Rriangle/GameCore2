#!/bin/bash

# API 冒煙測試腳本
# 測試所有主要端點的功能

BASE_URL="http://localhost:5000"
API_URL="http://localhost:5001"

echo "🚀 開始 API 冒煙測試..."
echo "API 基礎 URL: $API_URL"

# 測試結果追蹤
declare -a test_results
declare -a test_names=("健康檢查" "錢包 API" "每日簽到" "市場商品" "寵物 API")
declare -a test_urls=("$BASE_URL/health" "$API_URL/api/wallet" "$API_URL/api/signin/daily" "$API_URL/api/market/products" "$API_URL/api/pet/me")
declare -a test_methods=("GET" "GET" "POST" "GET" "GET")

# 顏色定義
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# 測試函數
test_endpoint() {
    local test_name="$1"
    local url="$2"
    local method="$3"
    local test_index="$4"
    
    echo -e "\n${CYAN}📋 測試 $((test_index + 1)): $test_name${NC}"
    
    if curl -s -o /dev/null -w "%{http_code}" "$url" -X "$method" --connect-timeout 10 | grep -q "200"; then
        echo -e "${GREEN}✅ $test_name 通過${NC}"
        test_results[$test_index]=1
    else
        echo -e "${RED}❌ $test_name 失敗${NC}"
        test_results[$test_index]=0
    fi
}

# 執行測試
for i in "${!test_names[@]}"; do
    test_endpoint "${test_names[$i]}" "${test_urls[$i]}" "${test_methods[$i]}" "$i"
done

# 輸出測試結果摘要
echo -e "\n${MAGENTA}📊 測試結果摘要:${NC}"
echo "============================================================"

pass_count=0
for i in "${!test_names[@]}"; do
    if [ "${test_results[$i]}" -eq 1 ]; then
        echo -e "${GREEN}✅ ${test_names[$i]}${NC}"
        ((pass_count++))
    else
        echo -e "${RED}❌ ${test_names[$i]}${NC}"
    fi
done

echo -e "\n🎯 總計: $pass_count/${#test_names[@]} 測試通過"

if [ "$pass_count" -eq "${#test_names[@]}" ]; then
    echo -e "${GREEN}🎉 所有 API 冒煙測試通過！${NC}"
    exit 0
else
    echo -e "${YELLOW}⚠️  部分測試失敗，請檢查 API 狀態${NC}"
    exit 1
fi 