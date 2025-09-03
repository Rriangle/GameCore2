# GameCore UI 指南 (繁體中文)

## 📋 目錄
1. [設計系統](#設計系統)
2. [組件庫](#組件庫)
3. [使用規範](#使用規範)
4. [無障礙性](#無障礙性)
5. [效能優化](#效能優化)
6. [響應式設計](#響應式設計)
7. [開發工具](#開發工具)

---

## 🎨 設計系統

### 設計令牌 (Design Tokens)

#### 色彩系統
```css
/* 主要色彩 */
--gc-primary: #3b82f6;
--gc-primary-hover: #2563eb;
--gc-primary-foreground: #ffffff;

/* 次要色彩 */
--gc-secondary: #64748b;
--gc-secondary-hover: #475569;
--gc-secondary-foreground: #ffffff;

/* 成功/錯誤/警告/資訊 */
--gc-success: #10b981;
--gc-error: #ef4444;
--gc-warning: #f59e0b;
--gc-info: #06b6d4;

/* 背景系統 */
--gc-bg: #ffffff;
--gc-surface: rgba(255, 255, 255, 0.8);
--gc-glass: rgba(255, 255, 255, 0.1);
--gc-muted: #f1f5f9;
```

#### 間距系統
```css
/* 間距工具類別 */
.gc-spacing-xs { padding: var(--gc-spacing-xs); }
.gc-spacing-sm { padding: var(--gc-spacing-sm); }
.gc-spacing-md { padding: var(--gc-spacing-md); }
.gc-spacing-base { padding: var(--gc-spacing-base); }
.gc-spacing-lg { padding: var(--gc-spacing-lg); }
.gc-spacing-xl { padding: var(--gc-spacing-xl); }
.gc-spacing-2xl { padding: var(--gc-spacing-2xl); }
.gc-spacing-3xl { padding: var(--gc-spacing-3xl); }
```

#### 字體系統
```css
/* 字體大小 */
--gc-text-xs: 0.75rem;    /* 12px */
--gc-text-sm: 0.875rem;   /* 14px */
--gc-text-base: 1rem;     /* 16px */
--gc-text-lg: 1.125rem;   /* 18px */
--gc-text-xl: 1.25rem;    /* 20px */
--gc-text-2xl: 1.5rem;    /* 24px */
--gc-text-3xl: 1.875rem;  /* 30px */

/* 字體粗細 */
--gc-font-light: 300;
--gc-font-normal: 400;
--gc-font-medium: 500;
--gc-font-semibold: 600;
--gc-font-bold: 700;
```

#### 動畫系統
```css
/* 動畫時間 */
--gc-transition-fast: 150ms;
--gc-transition-base: 300ms;
--gc-transition-slow: 500ms;

/* 動畫曲線 */
--gc-ease-in: cubic-bezier(0.4, 0, 1, 1);
--gc-ease-out: cubic-bezier(0, 0, 0.2, 1);
--gc-ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);
```

---

## 🧩 組件庫

### 按鈕組件

#### 基本按鈕
```html
<!-- 主要按鈕 -->
<button class="gc-btn gc-btn-primary">主要按鈕</button>

<!-- 次要按鈕 -->
<button class="gc-btn gc-btn-secondary">次要按鈕</button>

<!-- 危險按鈕 -->
<button class="gc-btn gc-btn-danger">危險按鈕</button>

<!-- 成功按鈕 -->
<button class="gc-btn gc-btn-success">成功按鈕</button>
```

#### 按鈕尺寸
```html
<!-- 小尺寸 -->
<button class="gc-btn gc-btn-sm">小按鈕</button>

<!-- 預設尺寸 -->
<button class="gc-btn">預設按鈕</button>

<!-- 大尺寸 -->
<button class="gc-btn gc-btn-lg">大按鈕</button>
```

#### 按鈕狀態
```html
<!-- 載入狀態 -->
<button class="gc-btn gc-btn-loading">
    <span class="gc-loading-spinner"></span>
    載入中...
</button>

<!-- 禁用狀態 -->
<button class="gc-btn" disabled>禁用按鈕</button>
```

### 輸入框組件

#### 基本輸入框
```html
<!-- 文字輸入框 -->
<div class="gc-input-group">
    <label for="username" class="gc-label">用戶名</label>
    <input type="text" id="username" class="gc-input" placeholder="請輸入用戶名">
</div>

<!-- 密碼輸入框 -->
<div class="gc-input-group">
    <label for="password" class="gc-label">密碼</label>
    <input type="password" id="password" class="gc-input" placeholder="請輸入密碼">
</div>
```

#### 輸入框狀態
```html
<!-- 錯誤狀態 -->
<div class="gc-input-group gc-input-error">
    <label for="email" class="gc-label">電子郵件</label>
    <input type="email" id="email" class="gc-input" value="invalid-email">
    <div class="gc-input-error-message">請輸入有效的電子郵件地址</div>
</div>

<!-- 成功狀態 -->
<div class="gc-input-group gc-input-success">
    <label for="phone" class="gc-label">電話號碼</label>
    <input type="tel" id="phone" class="gc-input" value="0912345678">
    <div class="gc-input-success-message">格式正確</div>
</div>
```

### 卡片組件

#### 基本卡片
```html
<div class="gc-card">
    <div class="gc-card-header">
        <h3 class="gc-card-title">卡片標題</h3>
    </div>
    <div class="gc-card-content">
        <p>卡片內容...</p>
    </div>
    <div class="gc-card-footer">
        <button class="gc-btn gc-btn-primary">操作</button>
    </div>
</div>
```

#### 玻璃風卡片
```html
<div class="gc-card gc-card-glass">
    <div class="gc-card-content">
        <h3>玻璃風效果</h3>
        <p>具有毛玻璃背景效果的卡片</p>
    </div>
</div>
```

### Modal 彈出層

#### 基本 Modal
```html
<!-- Modal 觸發按鈕 -->
<button class="gc-btn gc-btn-primary" onclick="openModal('example-modal')">
    開啟 Modal
</button>

<!-- Modal 結構 -->
<div id="example-modal" class="gc-modal">
    <div class="gc-modal-overlay" onclick="closeModal('example-modal')"></div>
    <div class="gc-modal-container">
        <div class="gc-modal-header">
            <h3>Modal 標題</h3>
            <button class="gc-modal-close" onclick="closeModal('example-modal')">×</button>
        </div>
        <div class="gc-modal-content">
            <p>Modal 內容...</p>
        </div>
        <div class="gc-modal-footer">
            <button class="gc-btn gc-btn-secondary" onclick="closeModal('example-modal')">取消</button>
            <button class="gc-btn gc-btn-primary">確認</button>
        </div>
    </div>
</div>
```

### Toast 通知

#### 基本 Toast
```html
<!-- 成功通知 -->
<div class="gc-toast gc-toast-success">
    <div class="gc-toast-icon">✓</div>
    <div class="gc-toast-content">
        <div class="gc-toast-title">成功</div>
        <div class="gc-toast-message">操作已成功完成</div>
    </div>
    <button class="gc-toast-close">×</button>
</div>

<!-- 錯誤通知 -->
<div class="gc-toast gc-toast-error">
    <div class="gc-toast-icon">✕</div>
    <div class="gc-toast-content">
        <div class="gc-toast-title">錯誤</div>
        <div class="gc-toast-message">操作失敗，請重試</div>
    </div>
    <button class="gc-toast-close">×</button>
</div>
```

### 表格組件

#### 基本表格
```html
<div class="gc-table-container">
    <table class="gc-table">
        <thead>
            <tr>
                <th>姓名</th>
                <th>電子郵件</th>
                <th>狀態</th>
                <th>操作</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>張三</td>
                <td>zhang@example.com</td>
                <td><span class="gc-badge gc-badge-success">啟用</span></td>
                <td>
                    <button class="gc-btn gc-btn-sm gc-btn-secondary">編輯</button>
                </td>
            </tr>
        </tbody>
    </table>
</div>
```

#### 響應式表格
```html
<div class="gc-table-responsive">
    <table class="gc-table">
        <!-- 表格內容 -->
    </table>
</div>
```

---

## 📏 使用規範

### 命名規範

#### CSS 類別命名
```css
/* 使用 gc- 前綴 */
.gc-btn { }
.gc-input { }
.gc-card { }

/* 使用 BEM 方法論 */
.gc-card { }
.gc-card__header { }
.gc-card__title { }
.gc-card--glass { }
```

#### JavaScript 函數命名
```javascript
// 使用 camelCase
function openModal(modalId) { }
function closeModal(modalId) { }
function showToast(message, type) { }
```

### 響應式設計

#### 斷點系統
```css
/* 手機 */
@media (max-width: 480px) { }

/* 平板 */
@media (max-width: 768px) { }

/* 桌面 */
@media (max-width: 1024px) { }

/* 大螢幕 */
@media (min-width: 1440px) { }
```

#### 容器系統
```html
<!-- 固定寬度容器 -->
<div class="gc-container">
    <!-- 內容 -->
</div>

<!-- 流體容器 -->
<div class="gc-container gc-container-fluid">
    <!-- 內容 -->
</div>
```

### 動畫規範

#### 動畫時間
- **快速動畫**：150ms（懸停效果）
- **標準動畫**：300ms（過渡效果）
- **慢速動畫**：500ms（載入動畫）

#### 動畫曲線
- **進入**：cubic-bezier(0.4, 0, 1, 1)
- **退出**：cubic-bezier(0, 0, 0.2, 1)
- **進入退出**：cubic-bezier(0.4, 0, 0.2, 1)

---

## ♿ 無障礙性

### ARIA 標籤

#### 基本 ARIA 屬性
```html
<!-- 按鈕 -->
<button aria-label="關閉對話框" aria-pressed="false">
    關閉
</button>

<!-- 輸入框 -->
<input type="text" aria-describedby="username-help" aria-invalid="false">
<div id="username-help">請輸入 3-20 個字符</div>

<!-- 圖片 -->
<img src="avatar.jpg" alt="用戶頭像" aria-describedby="avatar-description">
<div id="avatar-description">張三的個人頭像</div>
```

#### 動態內容
```html
<!-- 即時區域 -->
<div aria-live="polite" aria-atomic="true">
    載入中...
</div>

<!-- 狀態更新 -->
<div role="status" aria-live="assertive">
    表單已成功提交
</div>
```

### 鍵盤導航

#### 焦點管理
```javascript
// 焦點陷阱
function trapFocus(element) {
    const focusableElements = element.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    
    const firstElement = focusableElements[0];
    const lastElement = focusableElements[focusableElements.length - 1];
    
    element.addEventListener('keydown', (e) => {
        if (e.key === 'Tab') {
            if (e.shiftKey) {
                if (document.activeElement === firstElement) {
                    e.preventDefault();
                    lastElement.focus();
                }
            } else {
                if (document.activeElement === lastElement) {
                    e.preventDefault();
                    firstElement.focus();
                }
            }
        }
    });
}
```

#### 跳過連結
```html
<a href="#main-content" class="gc-skip-link">
    跳過導航，前往主要內容
</a>
```

### 色彩對比度

#### 對比度要求
- **正常文字**：至少 4.5:1
- **大字體**：至少 3:1
- **UI 元素**：至少 3:1

#### 檢查工具
```css
/* 使用 CSS 變數確保對比度 */
--gc-text-primary: #1f2937; /* 深色文字 */
--gc-text-secondary: #6b7280; /* 次要文字 */
--gc-bg-primary: #ffffff; /* 淺色背景 */
```

---

## ⚡ 效能優化

### 圖片優化

#### 響應式圖片
```html
<img src="image.jpg" 
     srcset="image-480w.jpg 480w, image-768w.jpg 768w, image-1024w.jpg 1024w"
     sizes="(max-width: 480px) 100vw, (max-width: 768px) 50vw, 33vw"
     alt="描述文字">
```

#### 懶載入
```html
<img src="placeholder.jpg" 
     data-src="actual-image.jpg" 
     class="gc-lazy"
     alt="描述文字">
```

### CSS 優化

#### 關鍵路徑 CSS
```html
<!-- 內聯關鍵 CSS -->
<style>
    .gc-critical { /* 關鍵樣式 */ }
</style>

<!-- 異步載入非關鍵 CSS -->
<link rel="preload" href="non-critical.css" as="style" onload="this.onload=null;this.rel='stylesheet'">
```

#### CSS 壓縮
```css
/* 移除不必要的空格和註釋 */
.gc-btn{padding:var(--gc-spacing-2) var(--gc-spacing-4);border-radius:var(--gc-radius-sm)}
```

### JavaScript 優化

#### 代碼分割
```javascript
// 動態載入組件
const Modal = await import('./components/Modal.js');

// 條件載入
if (featureFlag.enabled) {
    const AdvancedFeature = await import('./features/Advanced.js');
}
```

#### 事件委託
```javascript
// 使用事件委託減少事件監聽器
document.addEventListener('click', (e) => {
    if (e.target.matches('.gc-btn')) {
        handleButtonClick(e.target);
    }
});
```

---

## 📱 響應式設計

### 網格系統

#### 基本網格
```html
<div class="gc-grid">
    <div class="gc-col gc-col-12 gc-col-md-6 gc-col-lg-4">
        <!-- 內容 -->
    </div>
</div>
```

#### 響應式工具類別
```css
/* 顯示/隱藏 */
.gc-hidden-xs { display: none; }
.gc-visible-md { display: block; }

/* 文字對齊 */
.gc-text-left-xs { text-align: left; }
.gc-text-center-md { text-align: center; }
.gc-text-right-lg { text-align: right; }
```

### 容器查詢

#### 組件級響應式
```css
@container (min-width: 400px) {
    .gc-card {
        display: grid;
        grid-template-columns: 1fr 2fr;
    }
}
```

---

## 🛠️ 開發工具

### 調試工具

#### 效能監控
```javascript
// 開啟效能監控面板
// 按 Ctrl+Shift+O

// 查看效能指標
console.log(window.gcFinalOptimizer.performanceData);

// 運行所有測試
window.gcFinalOptimizer.runAllTests();
```

#### 視覺微調工具
```javascript
// 開啟視覺微調工具
// 按 Ctrl+Shift+D

// 查看響應式斷點
// 按 Ctrl+Shift+R
```

### 開發規範

#### 代碼風格
```javascript
// 使用 ESLint 和 Prettier
// 遵循 Airbnb JavaScript Style Guide

// 函數命名
function handleButtonClick() { }
function validateForm() { }
function showNotification() { }

// 變數命名
const isModalOpen = false;
const userData = {};
const config = {};
```

#### 註釋規範
```javascript
/**
 * 開啟 Modal 對話框
 * @param {string} modalId - Modal 的 ID
 * @param {Object} options - 選項配置
 * @returns {Promise<void>}
 */
async function openModal(modalId, options = {}) {
    // 實現邏輯
}
```

### 測試規範

#### 單元測試
```javascript
// 使用 Jest 進行測試
describe('Button Component', () => {
    test('should render correctly', () => {
        // 測試邏輯
    });
    
    test('should handle click events', () => {
        // 測試邏輯
    });
});
```

#### 整合測試
```javascript
// 使用 Playwright 進行 E2E 測試
test('user can submit form', async ({ page }) => {
    await page.goto('/form');
    await page.fill('#username', 'testuser');
    await page.click('button[type="submit"]');
    await expect(page.locator('.gc-toast-success')).toBeVisible();
});
```

---

## 📚 最佳實踐

### ✅ 應該做的

1. **使用設計令牌**：確保視覺一致性
2. **實施無障礙性**：支援所有用戶
3. **優化效能**：提供快速載入體驗
4. **測試響應式**：確保跨設備相容性
5. **遵循命名規範**：保持代碼可維護性

### ❌ 不應該做的

1. **硬編碼樣式**：避免重複和不一致
2. **忽略無障礙性**：不要排除任何用戶
3. **過度動畫**：尊重用戶偏好設定
4. **忽略效能**：不要影響用戶體驗
5. **破壞響應式**：確保所有設備都能正常使用

### 🔧 故障排除

#### 常見問題

1. **樣式不生效**
   - 檢查 CSS 檔案是否正確載入
   - 確認類別名稱是否正確
   - 檢查 CSS 優先級

2. **響應式問題**
   - 確認斷點設置是否正確
   - 檢查容器寬度設置
   - 測試不同設備尺寸

3. **效能問題**
   - 使用效能監控工具檢查
   - 優化圖片和資源載入
   - 檢查 JavaScript 執行時間

4. **無障礙性問題**
   - 使用無障礙性檢查工具
   - 確認 ARIA 標籤設置
   - 測試鍵盤導航

---

## 📞 支援

### 文檔資源
- [設計令牌文檔](./design-tokens.md)
- [組件庫文檔](./components.md)
- [效能優化指南](./performance.md)

### 開發工具
- [效能監控工具](./tools/performance-monitor.md)
- [無障礙性檢查工具](./tools/accessibility-checker.md)
- [視覺微調工具](./tools/visual-polish.md)

### 聯絡方式
- **技術支援**：tech-support@gamecore.com
- **設計諮詢**：design@gamecore.com
- **效能優化**：performance@gamecore.com

---

**最後更新**：2024-12-19  
**版本**：1.0.0  
**維護者**：GameCore UI-OPT 團隊 