# GameCore UI-OPT 部署準備指南 (繁體中文)

## 📋 部署前檢查清單

### ✅ 建置狀態確認
- [ ] 專案建置成功 (0 警告)
- [ ] 所有 CSS 檔案語法正確
- [ ] JavaScript 檔案無語法錯誤
- [ ] 靜態資源路徑正確

### ✅ 功能測試確認
- [ ] 首頁正常載入
- [ ] 管理後台正常載入
- [ ] 導航功能正常
- [ ] 響應式設計正常
- [ ] 深色模式切換正常
- [ ] 無障礙性功能正常

### ✅ 效能優化確認
- [ ] 核心網頁指標達標 (LCP < 2.5s, FID < 100ms, CLS < 0.1)
- [ ] 圖片已優化 (WebP/AVIF 格式)
- [ ] CSS/JS 檔案已壓縮
- [ ] 關鍵路徑資源已預載入
- [ ] 非關鍵資源已異步載入

### ✅ 跨設備相容性確認
- [ ] 桌面端 (1920×1080) 正常
- [ ] 平板端 (768×1024) 正常
- [ ] 手機端 (375×667) 正常
- [ ] 大螢幕 (2560×1440) 正常

### ✅ 瀏覽器相容性確認
- [ ] Chrome 120+ 正常
- [ ] Firefox 120+ 正常
- [ ] Safari 17+ 正常
- [ ] Edge 120+ 正常

---

## 🚀 部署步驟

### 1. 生產環境建置
```bash
# 清理舊建置
dotnet clean

# 還原套件
dotnet restore

# 生產環境建置
dotnet build -c Release

# 發佈應用程式
dotnet publish -c Release -o ./publish
```

### 2. 靜態資源優化
```bash
# 壓縮 CSS 檔案
# 使用 gzip 或 brotli 壓縮
gzip -9 wwwroot/css/*.css
brotli -9 wwwroot/css/*.css

# 壓縮 JavaScript 檔案
gzip -9 wwwroot/js/*.js
brotli -9 wwwroot/js/*.js

# 優化圖片
# 轉換為 WebP 格式
cwebp -q 85 image.jpg -o image.webp
```

### 3. 環境配置
```json
// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "StaticFiles": {
    "CacheControl": "public,max-age=31536000"
  }
}
```

### 4. Web 伺服器配置

#### IIS 配置
```xml
<!-- web.config -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <staticContent>
      <remove fileExtension=".css" />
      <mimeMap fileExtension=".css" mimeType="text/css" />
      <remove fileExtension=".js" />
      <mimeMap fileExtension=".js" mimeType="application/javascript" />
      <remove fileExtension=".webp" />
      <mimeMap fileExtension=".webp" mimeType="image/webp" />
    </staticContent>
    <httpProtocol>
      <customHeaders>
        <add name="X-Content-Type-Options" value="nosniff" />
        <add name="X-Frame-Options" value="DENY" />
        <add name="X-XSS-Protection" value="1; mode=block" />
        <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
      </customHeaders>
    </httpProtocol>
  </system.webServer>
</configuration>
```

#### Nginx 配置
```nginx
# nginx.conf
server {
    listen 80;
    server_name gamecore.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
    
    location ~* \.(css|js|png|jpg|jpeg|gif|ico|svg|webp)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
        add_header X-Content-Type-Options nosniff;
    }
    
    # 啟用 gzip 壓縮
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/css application/javascript image/svg+xml;
}
```

#### Apache 配置
```apache
# .htaccess
<IfModule mod_expires.c>
    ExpiresActive On
    ExpiresByType text/css "access plus 1 year"
    ExpiresByType application/javascript "access plus 1 year"
    ExpiresByType image/webp "access plus 1 year"
</IfModule>

<IfModule mod_deflate.c>
    AddOutputFilterByType DEFLATE text/css
    AddOutputFilterByType DEFLATE application/javascript
</IfModule>

<IfModule mod_headers.c>
    Header always set X-Content-Type-Options nosniff
    Header always set X-Frame-Options DENY
    Header always set X-XSS-Protection "1; mode=block"
</IfModule>
```

---

## 🔧 監控和維護

### 效能監控
```javascript
// 監控核心網頁指標
const observer = new PerformanceObserver((list) => {
    for (const entry of list.getEntries()) {
        if (entry.entryType === 'largest-contentful-paint') {
            console.log('LCP:', entry.startTime);
        }
        if (entry.entryType === 'first-input') {
            console.log('FID:', entry.processingStart - entry.startTime);
        }
    }
});

observer.observe({ entryTypes: ['largest-contentful-paint', 'first-input'] });
```

### 錯誤監控
```javascript
// 全域錯誤處理
window.addEventListener('error', (event) => {
    console.error('JavaScript Error:', event.error);
    // 發送到錯誤監控服務
});

window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled Promise Rejection:', event.reason);
    // 發送到錯誤監控服務
});
```

### 用戶體驗監控
```javascript
// 監控用戶互動
document.addEventListener('click', (event) => {
    // 記錄用戶點擊行為
    analytics.track('click', {
        element: event.target.tagName,
        className: event.target.className,
        page: window.location.pathname
    });
});

// 監控表單提交
document.addEventListener('submit', (event) => {
    analytics.track('form_submit', {
        form: event.target.id || event.target.className,
        page: window.location.pathname
    });
});
```

---

## 📊 部署後檢查

### 1. 功能驗證
- [ ] 首頁載入正常
- [ ] 所有導航連結正常
- [ ] 表單提交正常
- [ ] 響應式設計正常
- [ ] 深色模式正常

### 2. 效能驗證
- [ ] 頁面載入時間 < 3 秒
- [ ] 首次內容繪製 < 1.8 秒
- [ ] 最大內容繪製 < 2.5 秒
- [ ] 首次輸入延遲 < 100ms
- [ ] 累積佈局偏移 < 0.1

### 3. 無障礙性驗證
- [ ] 鍵盤導航正常
- [ ] 螢幕閱讀器支援正常
- [ ] 色彩對比度達標
- [ ] ARIA 標籤正確

### 4. 跨設備驗證
- [ ] 桌面端正常
- [ ] 平板端正常
- [ ] 手機端正常
- [ ] 不同瀏覽器正常

---

## 🛠️ 故障排除

### 常見問題

#### 1. 樣式不生效
```bash
# 檢查 CSS 檔案路徑
ls -la wwwroot/css/

# 檢查檔案權限
chmod 644 wwwroot/css/*.css

# 清除瀏覽器快取
# 在瀏覽器中按 Ctrl+Shift+R
```

#### 2. JavaScript 錯誤
```bash
# 檢查 JavaScript 檔案語法
node -c wwwroot/js/*.js

# 檢查檔案路徑
ls -la wwwroot/js/
```

#### 3. 效能問題
```bash
# 檢查檔案大小
du -h wwwroot/css/*.css
du -h wwwroot/js/*.js

# 檢查圖片大小
du -h wwwroot/images/*

# 使用 Lighthouse 進行效能分析
# 在 Chrome DevTools 中執行
```

#### 4. 響應式問題
```bash
# 檢查 viewport meta 標籤
grep -r "viewport" Views/

# 檢查媒體查詢
grep -r "@media" wwwroot/css/
```

### 回滾程序
```bash
# 1. 停止應用程式
sudo systemctl stop gamecore

# 2. 恢復備份
cp -r backup/current/* /var/www/gamecore/

# 3. 重新啟動應用程式
sudo systemctl start gamecore

# 4. 檢查狀態
sudo systemctl status gamecore
```

---

## 📈 持續改進

### 定期檢查項目
- [ ] 每週檢查效能指標
- [ ] 每月檢查無障礙性
- [ ] 每季檢查瀏覽器相容性
- [ ] 每年更新依賴套件

### 用戶反饋收集
```javascript
// 用戶體驗調查
function showUXSurvey() {
    const survey = {
        questions: [
            '您對網站載入速度滿意嗎？',
            '您覺得導航容易使用嗎？',
            '您在使用過程中遇到任何問題嗎？'
        ],
        onSubmit: (answers) => {
            // 發送反饋到後端
            fetch('/api/feedback', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(answers)
            });
        }
    };
    
    // 顯示調查問卷
    showModal(survey);
}
```

### 效能優化建議
1. **圖片優化**：使用 WebP 格式，實施懶載入
2. **代碼分割**：按路由分割 JavaScript 代碼
3. **快取策略**：實施適當的快取策略
4. **CDN 使用**：使用 CDN 加速靜態資源
5. **監控工具**：使用 APM 工具監控效能

---

## 📞 支援聯絡

### 技術支援
- **電子郵件**：tech-support@gamecore.com
- **電話**：+886-2-1234-5678
- **線上支援**：https://support.gamecore.com

### 文檔資源
- [API 文件](../api-docs/)
- [開發指南](../dev-guide/)
- [故障排除](../troubleshooting/)

### 緊急聯絡
- **緊急電話**：+886-2-1234-9999
- **緊急郵件**：emergency@gamecore.com

---

**最後更新**：2024-12-19  
**版本**：1.0.0  
**維護者**：GameCore UI-OPT 團隊 