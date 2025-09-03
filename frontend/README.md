# 🎮 GameCore Frontend

## 📚 概述

GameCore 前端是一個基於 Vue 3 + TypeScript 的現代化 Web 應用，採用玻璃風設計系統，提供完整的遊戲平台功能。

## ✨ 特色功能

- 🎨 **玻璃風設計系統** - 現代化的視覺體驗
- 📱 **完全響應式** - 支援所有設備尺寸
- ♿ **無障礙性** - 符合 WCAG 2.1 AA 標準
- 🚀 **Vue 3 + TypeScript** - 最新的前端技術棧
- 🧩 **組件庫** - 9 個可重用 UI 組件
- 🌙 **深色模式** - 自動適應用戶偏好

## 🏗️ 技術架構

### 核心技術
- **Vue 3** - 使用 Composition API
- **TypeScript** - 完整的類型支援
- **Tailwind CSS** - 實用優先的 CSS 框架
- **Vite** - 快速的建置工具

### 設計系統
- **玻璃風設計** - 半透明效果和漸層色彩
- **CSS 變數** - 可定制的主題系統
- **響應式網格** - 12 列響應式布局
- **動畫系統** - 流暢的過渡效果

## 🧩 組件庫

### 基礎組件
- **GCButton** - 可重用按鈕組件（6 種樣式，4 種尺寸）
- **GCInput** - 可重用輸入框組件（支援多種類型和狀態）
- **GCCard** - 可重用卡片組件（標題、內容、操作區域）

### 數據展示組件
- **GCTable** - 可重用表格組件（排序、分頁、響應式）
- **GCPagination** - 可重用分頁組件（多種樣式、快速跳轉）
- **GCTabs** - 可重用標籤頁組件（多種樣式、垂直布局）

### 反饋組件
- **GCModal** - 可重用模態框組件（多尺寸、動畫、無障礙）
- **GCToast** - 可重用通知組件（多類型、自動消失、進度條）
- **GCAccordion** - 可重用手風琴組件（單個/多個展開）

### 組件文檔
- **ComponentLibrary** - 完整的組件使用示例和文檔

## 🚀 快速開始

### 1. 安裝依賴
```bash
npm install
```

### 2. 開發模式
```bash
npm run dev
```

### 3. 建置生產版本
```bash
npm run build
```

### 4. 預覽生產版本
```bash
npm run preview
```

## 📁 專案結構

```
frontend/
├── src/
│   ├── components/
│   │   ├── ui/                 # UI 組件庫
│   │   │   ├── GCButton.vue    # 按鈕組件
│   │   │   ├── GCInput.vue     # 輸入框組件
│   │   │   ├── GCCard.vue      # 卡片組件
│   │   │   ├── GCTable.vue     # 表格組件
│   │   │   ├── GCPagination.vue # 分頁組件
│   │   │   ├── GCTabs.vue      # 標籤頁組件
│   │   │   ├── GCModal.vue     # 模態框組件
│   │   │   ├── GCToast.vue     # 通知組件
│   │   │   ├── GCAccordion.vue # 手風琴組件
│   │   │   ├── ComponentLibrary.vue # 組件文檔
│   │   │   └── index.ts        # 組件庫索引
│   │   └── ...                 # 其他組件
│   ├── views/                  # 頁面組件
│   │   ├── HomeView.vue        # 首頁
│   │   ├── LoginView.vue       # 登入頁面
│   │   └── RegisterView.vue    # 註冊頁面
│   ├── App.vue                 # 根組件
│   ├── main.ts                 # 應用入口
│   └── style.css               # 全局樣式
├── docs/                       # 文檔
│   └── COMPONENT_GUIDE.md     # 組件使用指南
├── public/                     # 靜態資源
├── tailwind.config.js          # Tailwind 配置
├── vite.config.ts              # Vite 配置
├── tsconfig.json               # TypeScript 配置
└── package.json                # 專案配置
```

## 🎨 設計系統

### 色彩系統
- **主要色彩**：藍色系（#3b82f6）
- **輔助色彩**：紫色（#7557ff）、藍綠（#34d2ff）
- **漸層色彩**：18 種預定義漸層
- **狀態色彩**：成功、警告、錯誤、信息

### 玻璃風效果
- **背景**：半透明玻璃效果
- **陰影**：多層次陰影系統
- **模糊**：背景模糊效果
- **邊框**：半透明邊框

### 響應式斷點
- **480px**：小型手機
- **768px**：平板設備
- **1024px**：桌面設備
- **1200px**：大型桌面

## ♿ 無障礙性

### WCAG 2.1 AA 標準
- **色彩對比度**：符合 AA 級別要求
- **鍵盤導航**：完整的鍵盤支援
- **螢幕閱讀器**：語義化 HTML 和 ARIA 屬性
- **焦點管理**：清晰的焦點指示器

### ARIA 支援
- `aria-label`：元素標籤
- `aria-describedby`：描述關聯
- `aria-expanded`：展開狀態
- `aria-selected`：選中狀態

## 📱 響應式設計

### 移動優先
- 優先考慮小螢幕體驗
- 觸控友好的互動元素
- 自適應的網格系統
- 優化的載入性能

### 組件適配
- 所有組件都支援響應式
- 自動適配不同螢幕尺寸
- 觸控和滑鼠事件支援
- 移動端專用樣式

## 🔧 自定義主題

### CSS 變數
```css
:root {
  --gc-primary-500: #your-color;
  --gc-accent: #your-accent-color;
  --gc-radius-glass: 12px;
  --gc-blur-glass: 20px;
}
```

### 深色模式
```css
@media (prefers-color-scheme: dark) {
  :root {
    --gc-bg: #your-dark-bg;
    --gc-ink: #your-dark-text;
  }
}
```

## 🧪 測試

### 組件測試
- 功能測試
- 響應式測試
- 無障礙性測試
- 跨瀏覽器測試

### 開發工具
- Vue DevTools
- 瀏覽器開發者工具
- 響應式設計測試
- 無障礙性檢查

## 📚 文檔

### 組件文檔
- [組件使用指南](./docs/COMPONENT_GUIDE.md)
- 組件庫示例頁面
- API 參考文檔
- 最佳實踐指南

### 設計文檔
- 設計系統規範
- 色彩使用指南
- 響應式設計原則
- 無障礙性指南

## 🤝 貢獻

### 開發環境
```bash
git clone <repository>
cd frontend
npm install
npm run dev
```

### 代碼規範
- 使用 TypeScript
- 遵循 Vue 3 最佳實踐
- 添加適當的註釋
- 編寫組件文檔

### 測試要求
- 組件功能測試
- 響應式設計測試
- 無障礙性測試
- 跨瀏覽器測試

## 📦 建置

### 開發環境
```bash
npm run dev          # 開發伺服器
npm run build        # 建置生產版本
npm run preview      # 預覽生產版本
npm run lint         # 程式碼檢查
npm run type-check   # 類型檢查
```

### 環境變數
```bash
VITE_APP_TITLE=GameCore
VITE_APP_API_URL=http://localhost:5000
VITE_APP_ENV=development
```

## 🌟 特色亮點

### 1. 玻璃風設計
- 現代化的視覺效果
- 半透明和模糊效果
- 豐富的漸層色彩
- 多層次陰影系統

### 2. 完全響應式
- 移動優先設計
- 自適應布局
- 觸控友好
- 性能優化

### 3. 無障礙性
- WCAG 2.1 AA 標準
- 鍵盤導航支援
- 螢幕閱讀器友好
- 高對比度支援

### 4. 組件庫
- 9 個可重用組件
- 完整的類型定義
- 豐富的變體選項
- 詳細的使用文檔

## 📈 性能優化

### 建置優化
- Vite 快速建置
- 代碼分割
- 樹搖優化
- 資源壓縮

### 運行時優化
- 組件懶加載
- 虛擬滾動
- 記憶化組件
- 事件節流

## 🔮 未來規劃

### 短期目標
- 更多組件變體
- 主題系統完善
- 動畫效果增強
- 測試覆蓋率提升

### 長期目標
- 組件庫生態
- 設計系統工具
- 無障礙性認證
- 國際化支援

## 📞 聯絡資訊

- **專案維護者**：GameCore Team
- **技術支援**：請提交 Issue 或 Pull Request
- **文檔更新**：歡迎貢獻文檔改進

## 📄 授權

本專案採用 MIT 授權條款，詳見 [LICENSE](../LICENSE) 文件。

---

**版本**: 2.0.0  
**更新日期**: 2024-12-19  
**維護者**: GameCore Team 