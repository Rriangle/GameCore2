# GameCore 組件庫使用指南 (繁體中文)

## 📋 目錄
- [概述](#概述)
- [設計令牌系統](#設計令牌系統)
- [基礎組件](#基礎組件)
- [高級組件](#高級組件)
- [最佳實踐](#最佳實踵)
- [無障礙性指南](#無障礙性指南)

## 🎯 概述

GameCore 組件庫是一個基於設計令牌系統的現代化 UI 組件庫，提供完整的玻璃風設計體驗。所有組件都支援響應式設計、深色模式、無障礙性和鍵盤導航。

### 核心特色
- **玻璃風設計**：與 `index.txt` 風格 100% 匹配
- **設計令牌系統**：統一的色彩、間距、字體、動畫系統
- **響應式優先**：支援桌面、平板、手機所有斷點
- **無障礙性支援**：完整的 ARIA 標籤和鍵盤導航
- **深色模式**：自動適應用戶主題偏好
- **減少動畫模式**：尊重用戶的動畫偏好設定

## 🎨 設計令牌系統

### 色彩系統
```css
/* 主要色彩 */
--gc-accent: #3b82f6;      /* 主要強調色 */
--gc-accent-2: #8b5cf6;    /* 次要強調色 */
--gc-accent-3: #06b6d4;    /* 第三強調色 */

/* 語義色彩 */
--gc-up: #10b981;          /* 成功/上升 */
--gc-down: #ef4444;        /* 錯誤/下降 */
--gc-warning: #f59e0b;     /* 警告 */

/* 背景系統 */
--gc-bg: #ffffff;          /* 主要背景 */
--gc-surface: #f8fafc;     /* 表面背景 */
--gc-glass: rgba(255, 255, 255, 0.1); /* 玻璃風背景 */
```

### 間距系統
```css
--gc-space-1: 0.25rem;     /* 4px */
--gc-space-2: 0.5rem;      /* 8px */
--gc-space-3: 0.75rem;     /* 12px */
--gc-space-4: 1rem;        /* 16px */
--gc-space-6: 1.5rem;      /* 24px */
--gc-space-8: 2rem;        /* 32px */
--gc-space-12: 3rem;       /* 48px */
--gc-space-16: 4rem;       /* 64px */
```

### 字體系統
```css
--gc-text-xs: 0.75rem;     /* 12px */
--gc-text-sm: 0.875rem;    /* 14px */
--gc-text-base: 1rem;      /* 16px */
--gc-text-lg: 1.125rem;    /* 18px */
--gc-text-xl: 1.25rem;     /* 20px */
--gc-text-2xl: 1.5rem;     /* 24px */
--gc-text-3xl: 1.875rem;   /* 30px */
--gc-text-4xl: 2.25rem;    /* 36px */
--gc-text-5xl: 3rem;       /* 48px */
--gc-text-6xl: 3.75rem;    /* 60px */
```

## 🔧 基礎組件

### 按鈕 (Button)
```html
<!-- 主要按鈕 -->
<button class="gc-btn">主要按鈕</button>

<!-- 次要按鈕 -->
<button class="gc-btn-secondary">次要按鈕</button>

<!-- 危險按鈕 -->
<button class="gc-btn-danger">危險按鈕</button>

<!-- 載入狀態 -->
<button class="gc-btn" disabled>
    <i class="gc-loading-spinner"></i>
    載入中...
</button>
```

### 輸入框 (Input)
```html
<!-- 基本輸入框 -->
<input type="text" class="gc-input" placeholder="請輸入文字">

<!-- 錯誤狀態 -->
<input type="email" class="gc-input gc-input-error" placeholder="請輸入電子郵件">
<div class="gc-input-error-message">請輸入有效的電子郵件地址</div>

<!-- 成功狀態 -->
<input type="text" class="gc-input gc-input-success" value="輸入成功">
<div class="gc-input-success-message">輸入格式正確</div>
```

### 卡片 (Card)
```html
<!-- 基本卡片 -->
<div class="gc-card">
    <h3>卡片標題</h3>
    <p>卡片內容</p>
</div>

<!-- 互動卡片 -->
<div class="gc-card gc-card-interactive">
    <h3>可點擊卡片</h3>
    <p>點擊此卡片會觸發互動效果</p>
</div>

<!-- 彩色卡片 -->
<div class="gc-card gc-card-accent-2">
    <h3>彩色卡片</h3>
    <p>使用強調色背景的卡片</p>
</div>
```

## 🚀 高級組件

### Modal 彈出層
```html
<!-- Modal 結構 -->
<div class="gc-modal" id="myModal">
    <div class="gc-modal-backdrop" onclick="closeModal()"></div>
    <div class="gc-modal-content">
        <div class="gc-modal-header">
            <h3>Modal 標題</h3>
            <button class="gc-modal-close" onclick="closeModal()">
                <i class="bi bi-x"></i>
            </button>
        </div>
        <div class="gc-modal-body">
            <p>Modal 內容</p>
        </div>
        <div class="gc-modal-footer">
            <button class="gc-btn-secondary" onclick="closeModal()">取消</button>
            <button class="gc-btn" onclick="confirmAction()">確認</button>
        </div>
    </div>
</div>

<!-- JavaScript 控制 -->
<script>
function showModal() {
    document.getElementById('myModal').style.display = 'flex';
    document.body.style.overflow = 'hidden';
}

function closeModal() {
    document.getElementById('myModal').style.display = 'none';
    document.body.style.overflow = '';
}
</script>
```

### Toast 通知
```html
<!-- Toast 容器 -->
<div class="gc-toast-container" id="toastContainer"></div>

<!-- JavaScript 使用 -->
<script>
function showToast(type, message) {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `gc-toast gc-toast-${type}`;
    toast.innerHTML = `
        <i class="bi bi-${getToastIcon(type)}"></i>
        <span>${message}</span>
        <button onclick="this.parentElement.remove()">
            <i class="bi bi-x"></i>
        </button>
    `;
    container.appendChild(toast);
    
    // 自動移除
    setTimeout(() => toast.remove(), 5000);
}

// 使用範例
showToast('success', '操作成功完成！');
showToast('error', '發生錯誤，請稍後再試。');
showToast('warning', '請注意，這是一個警告訊息。');
showToast('info', '這是一條資訊訊息。');
</script>
```

### Accordion 手風琴
```html
<div class="gc-accordion">
    <div class="gc-accordion-item">
        <button class="gc-accordion-header" onclick="toggleAccordion(this)">
            <i class="bi bi-chevron-right"></i>
            標題 1
        </button>
        <div class="gc-accordion-content">
            <p>內容 1</p>
        </div>
    </div>
    
    <div class="gc-accordion-item">
        <button class="gc-accordion-header" onclick="toggleAccordion(this)">
            <i class="bi bi-chevron-right"></i>
            標題 2
        </button>
        <div class="gc-accordion-content">
            <p>內容 2</p>
        </div>
    </div>
</div>

<script>
function toggleAccordion(button) {
    const content = button.nextElementSibling;
    const icon = button.querySelector('i');
    
    if (content.style.display === 'none') {
        content.style.display = 'block';
        icon.style.transform = 'rotate(90deg)';
        button.setAttribute('aria-expanded', 'true');
    } else {
        content.style.display = 'none';
        icon.style.transform = 'rotate(0deg)';
        button.setAttribute('aria-expanded', 'false');
    }
}
</script>
```

### Tabs 標籤頁
```html
<div class="gc-tabs">
    <div class="gc-tabs-header">
        <button class="gc-tab-button active" onclick="switchTab(this, 'tab1')">標籤 1</button>
        <button class="gc-tab-button" onclick="switchTab(this, 'tab2')">標籤 2</button>
        <button class="gc-tab-button" onclick="switchTab(this, 'tab3')">標籤 3</button>
    </div>
    <div class="gc-tabs-content">
        <div id="tab1" class="gc-tab-panel active">
            <p>標籤 1 的內容</p>
        </div>
        <div id="tab2" class="gc-tab-panel">
            <p>標籤 2 的內容</p>
        </div>
        <div id="tab3" class="gc-tab-panel">
            <p>標籤 3 的內容</p>
        </div>
    </div>
</div>

<script>
function switchTab(button, tabId) {
    // 移除所有活動狀態
    const tabButtons = button.parentElement.querySelectorAll('.gc-tab-button');
    const tabPanels = button.parentElement.parentElement.querySelectorAll('.gc-tab-panel');
    
    tabButtons.forEach(btn => btn.classList.remove('active'));
    tabPanels.forEach(panel => panel.classList.remove('active'));
    
    // 設定活動狀態
    button.classList.add('active');
    document.getElementById(tabId).classList.add('active');
}
</script>
```

### Drawer 抽屜
```html
<!-- 左側抽屜 -->
<div class="gc-drawer gc-drawer-left" id="leftDrawer">
    <div class="gc-drawer-backdrop" onclick="closeDrawer('leftDrawer')"></div>
    <div class="gc-drawer-content">
        <div class="gc-drawer-header">
            <h3>左側抽屜</h3>
            <button class="gc-drawer-close" onclick="closeDrawer('leftDrawer')">
                <i class="bi bi-x"></i>
            </button>
        </div>
        <div class="gc-drawer-body">
            <p>抽屜內容</p>
        </div>
    </div>
</div>

<!-- 右側抽屜 -->
<div class="gc-drawer gc-drawer-right" id="rightDrawer">
    <div class="gc-drawer-backdrop" onclick="closeDrawer('rightDrawer')"></div>
    <div class="gc-drawer-content">
        <div class="gc-drawer-header">
            <h3>右側抽屜</h3>
            <button class="gc-drawer-close" onclick="closeDrawer('rightDrawer')">
                <i class="bi bi-x"></i>
            </button>
        </div>
        <div class="gc-drawer-body">
            <p>抽屜內容</p>
        </div>
    </div>
</div>

<script>
function openDrawer(drawerId) {
    document.getElementById(drawerId).style.display = 'flex';
    document.body.style.overflow = 'hidden';
}

function closeDrawer(drawerId) {
    document.getElementById(drawerId).style.display = 'none';
    document.body.style.overflow = '';
}
</script>
```

### Tooltip 提示
```html
<!-- 基本提示 -->
<button class="gc-tooltip" data-tooltip="這是一個提示訊息">
    懸停查看提示
</button>

<!-- 方向提示 -->
<button class="gc-tooltip" data-tooltip="上方提示" data-tooltip-position="top">
    上方提示
</button>
<button class="gc-tooltip" data-tooltip="下方提示" data-tooltip-position="bottom">
    下方提示
</button>
<button class="gc-tooltip" data-tooltip="左側提示" data-tooltip-position="left">
    左側提示
</button>
<button class="gc-tooltip" data-tooltip="右側提示" data-tooltip-position="right">
    右側提示
</button>
```

### Pagination 分頁
```html
<div class="gc-pagination">
    <button class="gc-pagination-prev" onclick="changePage(currentPage - 1)">
        <i class="bi bi-chevron-left"></i>
        上一頁
    </button>
    
    <div class="gc-pagination-pages">
        <button class="gc-pagination-page" onclick="changePage(1)">1</button>
        <button class="gc-pagination-page" onclick="changePage(2)">2</button>
        <button class="gc-pagination-page active" onclick="changePage(3)">3</button>
        <button class="gc-pagination-page" onclick="changePage(4)">4</button>
        <button class="gc-pagination-page" onclick="changePage(5)">5</button>
    </div>
    
    <button class="gc-pagination-next" onclick="changePage(currentPage + 1)">
        下一頁
        <i class="bi bi-chevron-right"></i>
    </button>
</div>
```

### Skeleton 骨架屏
```html
<!-- 卡片骨架屏 -->
<div class="gc-skeleton-card">
    <div class="gc-skeleton-header">
        <div class="gc-skeleton-avatar"></div>
        <div class="gc-skeleton-text">
            <div class="gc-skeleton-line" style="width: 60%;"></div>
            <div class="gc-skeleton-line" style="width: 40%;"></div>
        </div>
    </div>
    <div class="gc-skeleton-content">
        <div class="gc-skeleton-line"></div>
        <div class="gc-skeleton-line"></div>
        <div class="gc-skeleton-line" style="width: 80%;"></div>
    </div>
</div>

<!-- 表格骨架屏 -->
<div class="gc-skeleton-table">
    <div class="gc-skeleton-row">
        <div class="gc-skeleton-cell"></div>
        <div class="gc-skeleton-cell"></div>
        <div class="gc-skeleton-cell"></div>
    </div>
    <div class="gc-skeleton-row">
        <div class="gc-skeleton-cell"></div>
        <div class="gc-skeleton-cell"></div>
        <div class="gc-skeleton-cell"></div>
    </div>
</div>
```

### Empty State 空狀態
```html
<!-- 標準空狀態 -->
<div class="gc-empty-state">
    <div class="gc-empty-state-icon">
        <i class="bi bi-inbox"></i>
    </div>
    <h3 class="gc-empty-state-title">沒有資料</h3>
    <p class="gc-empty-state-description">目前沒有任何資料可以顯示</p>
    <button class="gc-btn">新增資料</button>
</div>

<!-- 緊湊空狀態 -->
<div class="gc-empty-state gc-empty-state-compact">
    <i class="bi bi-search"></i>
    <span>沒有找到相關結果</span>
</div>
```

## 📚 最佳實踵

### 1. 響應式設計
```html
<!-- 使用響應式網格 -->
<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: var(--gc-space-6);">
    <div class="gc-card">卡片 1</div>
    <div class="gc-card">卡片 2</div>
    <div class="gc-card">卡片 3</div>
</div>

<!-- 使用響應式文字 -->
<h1 style="font-size: var(--gc-text-3xl);">標題</h1>
<p style="font-size: var(--gc-text-lg);">內容</p>
```

### 2. 無障礙性
```html
<!-- 正確的 ARIA 標籤 -->
<button class="gc-btn" aria-label="關閉對話框">
    <i class="bi bi-x"></i>
</button>

<!-- 鍵盤導航支援 -->
<div class="gc-modal" tabindex="-1">
    <div class="gc-modal-content" tabindex="0">
        <!-- 內容 -->
    </div>
</div>
```

### 3. 深色模式支援
```css
/* 自動適應用戶偏好 */
@media (prefers-color-scheme: dark) {
    :root {
        --gc-bg: #1a1a1a;
        --gc-surface: #2a2a2a;
        --gc-ink: #ffffff;
        --gc-muted: #a0a0a0;
    }
}
```

### 4. 減少動畫模式
```css
/* 尊重用戶偏好 */
@media (prefers-reduced-motion: reduce) {
    * {
        animation-duration: 0.01ms !important;
        animation-iteration-count: 1 !important;
        transition-duration: 0.01ms !important;
    }
}
```

## ♿ 無障礙性指南

### 1. 鍵盤導航
- 所有互動元素都必須可以透過鍵盤存取
- 使用 `Tab` 鍵進行導航
- 使用 `Enter` 或 `Space` 鍵進行操作
- 使用 `Escape` 鍵關閉 Modal 和 Drawer

### 2. 螢幕閱讀器支援
```html
<!-- 正確的 ARIA 標籤 -->
<button class="gc-btn" aria-label="新增項目">
    <i class="bi bi-plus"></i>
</button>

<!-- 狀態指示 -->
<div class="gc-toast" role="alert" aria-live="polite">
    操作成功完成！
</div>
```

### 3. 色彩對比
- 所有文字都必須符合 WCAG 2.1 AA 標準
- 主要文字對比度至少 4.5:1
- 大型文字對比度至少 3:1

### 4. 焦點指示
```css
/* 清晰的焦點指示 */
.gc-btn:focus,
.gc-input:focus {
    outline: 2px solid var(--gc-accent);
    outline-offset: 2px;
}
```

## 🔧 自定義和擴展

### 1. 自定義主題
```css
/* 自定義色彩主題 */
:root {
    --gc-accent: #your-color;
    --gc-accent-2: #your-color-2;
    --gc-accent-3: #your-color-3;
}
```

### 2. 新增組件變體
```css
/* 新增按鈕變體 */
.gc-btn-outline {
    background: transparent;
    border: 2px solid var(--gc-accent);
    color: var(--gc-accent);
}

.gc-btn-outline:hover {
    background: var(--gc-accent);
    color: white;
}
```

### 3. 組件組合
```html
<!-- 組合多個組件 -->
<div class="gc-card">
    <div class="gc-card-header">
        <h3>卡片標題</h3>
        <button class="gc-btn-secondary" onclick="showModal()">
            <i class="bi bi-plus"></i>
            新增
        </button>
    </div>
    <div class="gc-card-body">
        <div class="gc-empty-state">
            <i class="bi bi-inbox"></i>
            <p>沒有資料</p>
        </div>
    </div>
</div>
```

## 📖 完整範例

### 完整的表單頁面
```html
<!DOCTYPE html>
<html lang="zh-TW">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>表單範例</title>
    <link rel="stylesheet" href="/css/design-tokens.css">
    <link rel="stylesheet" href="/css/base-components.css">
    <link rel="stylesheet" href="/css/components.css">
</head>
<body>
    <div style="max-width: var(--gc-container-max); margin: 0 auto; padding: var(--gc-space-4);">
        <h1 style="font-size: var(--gc-text-3xl); margin-bottom: var(--gc-space-8);">表單範例</h1>
        
        <div class="gc-card" style="max-width: 600px; margin: 0 auto;">
            <div class="gc-card-header">
                <h2>用戶資料</h2>
            </div>
            <div class="gc-card-body">
                <form>
                    <div style="margin-bottom: var(--gc-space-4);">
                        <label for="name" style="display: block; margin-bottom: var(--gc-space-2);">姓名</label>
                        <input type="text" id="name" class="gc-input" placeholder="請輸入姓名">
                    </div>
                    
                    <div style="margin-bottom: var(--gc-space-4);">
                        <label for="email" style="display: block; margin-bottom: var(--gc-space-2);">電子郵件</label>
                        <input type="email" id="email" class="gc-input" placeholder="請輸入電子郵件">
                    </div>
                    
                    <div style="margin-bottom: var(--gc-space-6);">
                        <label for="message" style="display: block; margin-bottom: var(--gc-space-2);">訊息</label>
                        <textarea id="message" class="gc-input" rows="4" placeholder="請輸入訊息"></textarea>
                    </div>
                    
                    <div style="display: flex; gap: var(--gc-space-3);">
                        <button type="submit" class="gc-btn">提交</button>
                        <button type="button" class="gc-btn-secondary" onclick="resetForm()">重置</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <script>
        function resetForm() {
            document.querySelector('form').reset();
            showToast('info', '表單已重置');
        }
        
        document.querySelector('form').addEventListener('submit', function(e) {
            e.preventDefault();
            showToast('success', '表單提交成功！');
        });
    </script>
</body>
</html>
```

---

**最後更新**：2024-12-19  
**版本**：1.0.0  
**作者**：GameCore UI Team 