# GameCore UI 組件庫使用指南

## 📚 概述

GameCore UI 組件庫是一個完整的玻璃風設計系統，提供了一致、可重用且無障礙的 UI 組件。所有組件都基於 Vue 3 Composition API 構建，支援 TypeScript 和響應式設計。

## 🎨 設計系統

### 玻璃風設計語言
- **背景系統**：半透明玻璃效果，支援深色模式
- **色彩系統**：18 種漸層色彩，完整的狀態色彩
- **陰影系統**：多層次陰影，營造深度感
- **動畫系統**：流暢的過渡效果和微互動

### 響應式設計
- **移動優先**：480px、768px、1024px 斷點
- **網格系統**：12 列響應式網格
- **組件適配**：所有組件都支援不同螢幕尺寸

### 無障礙性
- **WCAG 2.1 AA 標準**
- **ARIA 屬性支援**
- **鍵盤導航支援**
- **螢幕閱讀器友好**

## 🧩 組件分類

### 1. 基礎組件 (Basic Components)

#### GCButton - 按鈕組件
```vue
<template>
  <GCButton variant="primary" size="lg">主要按鈕</GCButton>
  <GCButton variant="secondary" size="md">次要按鈕</GCButton>
  <GCButton variant="success" size="sm">成功按鈕</GCButton>
</template>

<script setup>
import { GCButton } from '@/components/ui'
</script>
```

**屬性說明：**
- `variant`: 按鈕樣式 (`primary`, `secondary`, `success`, `danger`, `warning`, `info`)
- `size`: 按鈕尺寸 (`sm`, `md`, `lg`, `xl`)
- `loading`: 載入狀態
- `disabled`: 禁用狀態
- `outline`: 外框樣式
- `ghost`: 幽靈樣式

#### GCInput - 輸入框組件
```vue
<template>
  <GCInput 
    v-model="inputValue" 
    placeholder="請輸入文字"
    label="文字輸入"
    variant="default"
    type="text"
  />
</template>

<script setup>
import { GCInput } from '@/components/ui'
import { ref } from 'vue'

const inputValue = ref('')
</script>
```

**屬性說明：**
- `type`: 輸入類型 (`text`, `email`, `password`, `number`, `tel`, `url`)
- `variant`: 輸入框樣式 (`default`, `success`, `warning`, `danger`)
- `size`: 輸入框尺寸 (`sm`, `md`, `lg`)
- `label`: 標籤文字
- `placeholder`: 佔位符文字
- `required`: 必填標記
- `disabled`: 禁用狀態

#### GCCard - 卡片組件
```vue
<template>
  <GCCard variant="default" padding="md">
    <template #header>
      <h5 class="gc-card-title">卡片標題</h5>
    </template>
    <template #body>
      <p class="gc-card-text">卡片內容</p>
    </template>
    <template #footer>
      <div class="gc-card-actions">
        <GCButton variant="primary">確認</GCButton>
        <GCButton variant="secondary">取消</GCButton>
      </div>
    </template>
  </GCCard>
</template>

<script setup>
import { GCCard, GCButton } from '@/components/ui'
</script>
```

**屬性說明：**
- `variant`: 卡片樣式 (`default`, `elevated`, `outlined`)
- `padding`: 內邊距 (`none`, `sm`, `md`, `lg`)

### 2. 數據展示組件 (Data Display Components)

#### GCTable - 表格組件
```vue
<template>
  <GCTable 
    :data="tableData" 
    :columns="tableColumns"
    :sortable="true"
    :pagination="true"
    :items-per-page="10"
    variant="bordered"
  />
</template>

<script setup>
import { GCTable } from '@/components/ui'
import { reactive } from 'vue'

const tableData = reactive([
  { id: 1, name: '張三', email: 'zhang@example.com', status: '活躍' },
  { id: 2, name: '李四', email: 'li@example.com', status: '離線' }
])

const tableColumns = reactive([
  { key: 'id', label: 'ID', sortable: true },
  { key: 'name', label: '姓名', sortable: true },
  { key: 'email', label: '電子郵件', sortable: false },
  { key: 'status', label: '狀態', sortable: true }
])
</script>
```

**屬性說明：**
- `data`: 表格數據陣列
- `columns`: 列定義陣列
- `sortable`: 是否支援排序
- `pagination`: 是否顯示分頁
- `items-per-page`: 每頁項目數
- `variant`: 表格樣式 (`default`, `bordered`, `striped`)

#### GCPagination - 分頁組件
```vue
<template>
  <GCPagination 
    v-model="currentPage"
    :total="100"
    :items-per-page="10"
    variant="default"
    size="lg"
    :show-quick-jump="true"
  />
</template>

<script setup>
import { GCPagination } from '@/components/ui'
import { ref } from 'vue'

const currentPage = ref(1)
</script>
```

**屬性說明：**
- `total`: 總項目數
- `items-per-page`: 每頁項目數
- `variant`: 分頁樣式 (`default`, `minimal`, `bordered`)
- `size`: 分頁尺寸 (`sm`, `md`, `lg`)
- `show-quick-jump`: 是否顯示快速跳轉
- `compact`: 是否使用緊湊模式

#### GCTabs - 標籤頁組件
```vue
<template>
  <GCTabs v-model="activeTab" variant="default">
    <GCTabs.Tab name="tab1" title="標籤一">
      <div class="gc-tab-content">
        <h5>標籤一內容</h5>
        <p>這是第一個標籤頁的內容。</p>
      </div>
    </GCTabs.Tab>
    <GCTabs.Tab name="tab2" title="標籤二">
      <div class="gc-tab-content">
        <h5>標籤二內容</h5>
        <p>這是第二個標籤頁的內容。</p>
      </div>
    </GCTabs.Tab>
  </GCTabs>
</template>

<script setup>
import { GCTabs } from '@/components/ui'
import { ref } from 'vue'

const activeTab = ref('tab1')
</script>
```

**屬性說明：**
- `variant`: 標籤頁樣式 (`default`, `minimal`, `bordered`, `pills`)
- `orientation`: 標籤頁方向 (`horizontal`, `vertical`)
- `closable`: 是否可關閉
- `addable`: 是否可新增

### 3. 反饋組件 (Feedback Components)

#### GCModal - 模態框組件
```vue
<template>
  <div>
    <GCButton variant="primary" @click="showModal = true">
      開啟模態框
    </GCButton>
    
    <GCModal 
      v-model="showModal" 
      title="確認操作"
      size="md"
      variant="default"
    >
      <template #body>
        <p>您確定要執行此操作嗎？</p>
      </template>
      <template #footer>
        <div class="gc-modal-actions">
          <GCButton variant="secondary" @click="showModal = false">
            取消
          </GCButton>
          <GCButton variant="danger" @click="confirmAction">
            確認
          </GCButton>
        </div>
      </template>
    </GCModal>
  </div>
</template>

<script setup>
import { GCModal, GCButton } from '@/components/ui'
import { ref } from 'vue'

const showModal = ref(false)

const confirmAction = () => {
  showModal.value = false
  // 執行確認邏輯
}
</script>
```

**屬性說明：**
- `title`: 模態框標題
- `size`: 模態框尺寸 (`sm`, `md`, `lg`, `xl`, `full`)
- `variant`: 模態框樣式 (`default`, `centered`, `scrollable`)
- `closable`: 是否可關閉
- `mask-closable`: 點擊遮罩是否可關閉

#### GCToast - 通知組件
```vue
<template>
  <div>
    <GCButton variant="success" @click="showSuccessToast">
      成功通知
    </GCButton>
    <GCButton variant="warning" @click="showWarningToast">
      警告通知
    </GCButton>
  </div>
</template>

<script setup>
import { GCButton } from '@/components/ui'

const showSuccessToast = () => {
  // 顯示成功通知
  // 需要配置 Toast 插件
}

const showWarningToast = () => {
  // 顯示警告通知
}
</script>
```

**屬性說明：**
- `type`: 通知類型 (`success`, `warning`, `error`, `info`)
- `title`: 通知標題
- `message`: 通知內容
- `duration`: 顯示時長（毫秒）
- `closable`: 是否可關閉
- `show-progress`: 是否顯示進度條

#### GCAccordion - 手風琴組件
```vue
<template>
  <GCAccordion 
    v-model="activeItems"
    variant="default"
    :multiple="true"
  >
    <GCAccordion.Item 
      v-for="item in accordionData" 
      :key="item.id"
      :name="item.id"
      :title="item.title"
    >
      <div class="gc-accordion-content">
        <p>{{ item.content }}</p>
      </div>
    </GCAccordion.Item>
  </GCAccordion>
</template>

<script setup>
import { GCAccordion } from '@/components/ui'
import { reactive, ref } from 'vue'

const activeItems = ref(['item1'])

const accordionData = reactive([
  { id: 'item1', title: '項目一', content: '項目一的內容' },
  { id: 'item2', title: '項目二', content: '項目二的內容' }
])
</script>
```

**屬性說明：**
- `variant`: 手風琴樣式 (`default`, `minimal`, `bordered`, `pills`)
- `multiple`: 是否允許多個項目同時展開
- `collapsible`: 是否允許所有項目都收起

## 🚀 快速開始

### 1. 安裝依賴
```bash
npm install
```

### 2. 導入組件
```vue
<template>
  <div>
    <GCButton variant="primary">按鈕</GCButton>
    <GCInput v-model="value" placeholder="請輸入" />
  </div>
</template>

<script setup>
import { GCButton, GCInput } from '@/components/ui'
import { ref } from 'vue'

const value = ref('')
</script>
```

### 3. 使用組件庫文檔
```vue
<template>
  <ComponentLibrary />
</template>

<script setup>
import ComponentLibrary from '@/components/ui/ComponentLibrary.vue'
</script>
```

## 🎯 最佳實踐

### 1. 組件命名
- 使用 PascalCase 命名組件
- 組件名稱以 `GC` 開頭，表示 GameCore 組件
- 使用描述性的名稱，如 `GCUserProfile`、`GCGameCard`

### 2. 屬性傳遞
- 使用 `v-bind` 傳遞複雜對象
- 使用 `v-model` 進行雙向綁定
- 使用具名插槽組織複雜內容

### 3. 響應式設計
- 優先考慮移動端體驗
- 使用組件的響應式屬性
- 測試不同螢幕尺寸

### 4. 無障礙性
- 為所有互動元素添加適當的 ARIA 屬性
- 確保鍵盤導航的可用性
- 提供足夠的色彩對比度

## 🔧 自定義主題

### 1. CSS 變數
```css
:root {
  --gc-primary-500: #your-color;
  --gc-accent: #your-accent-color;
  --gc-radius-glass: 12px;
  --gc-blur-glass: 20px;
}
```

### 2. 深色模式
```css
@media (prefers-color-scheme: dark) {
  :root {
    --gc-bg: #your-dark-bg;
    --gc-ink: #your-dark-text;
  }
}
```

### 3. 組件變體
```vue
<template>
  <GCButton class="custom-button">自定義按鈕</GCButton>
</template>

<style scoped>
.custom-button {
  background: linear-gradient(45deg, #ff6b6b, #4ecdc4);
  border: none;
  color: white;
}
</style>
```

## 📱 響應式設計

### 1. 斷點系統
- **480px**: 小型手機
- **768px**: 平板設備
- **1024px**: 桌面設備
- **1200px**: 大型桌面

### 2. 組件適配
```vue
<template>
  <GCTable 
    :responsive="true"
    :mobile-view="isMobile"
  />
</template>

<script setup>
import { ref, onMounted } from 'vue'

const isMobile = ref(false)

onMounted(() => {
  const checkMobile = () => {
    isMobile.value = window.innerWidth < 768
  }
  
  checkMobile()
  window.addEventListener('resize', checkMobile)
})
</script>
```

## ♿ 無障礙性

### 1. ARIA 屬性
- `aria-label`: 為元素提供標籤
- `aria-describedby`: 關聯描述元素
- `aria-expanded`: 表示展開狀態
- `aria-selected`: 表示選中狀態

### 2. 鍵盤導航
- Tab 鍵導航
- Enter 鍵激活
- 空格鍵切換
- 方向鍵選擇

### 3. 螢幕閱讀器
- 語義化 HTML
- 適當的標題層級
- 描述性文字
- 狀態變化通知

## 🧪 測試

### 1. 組件測試
```vue
<template>
  <div>
    <GCButton 
      data-testid="test-button"
      variant="primary"
      @click="handleClick"
    >
      測試按鈕
    </GCButton>
  </div>
</template>

<script setup>
import { GCButton } from '@/components/ui'

const handleClick = () => {
  // 測試邏輯
}
</script>
```

### 2. 響應式測試
- 使用瀏覽器開發者工具
- 測試不同螢幕尺寸
- 檢查觸控體驗
- 驗證鍵盤導航

## 📚 參考資源

- [Vue 3 官方文檔](https://vuejs.org/)
- [Tailwind CSS 文檔](https://tailwindcss.com/)
- [WCAG 2.1 指南](https://www.w3.org/WAI/WCAG21/quickref/)
- [玻璃風設計指南](https://glassmorphism.com/)

## 🤝 貢獻指南

### 1. 開發環境
```bash
git clone <repository>
cd frontend
npm install
npm run dev
```

### 2. 代碼規範
- 使用 TypeScript
- 遵循 Vue 3 最佳實踐
- 添加適當的註釋
- 編寫組件文檔

### 3. 測試要求
- 組件功能測試
- 響應式設計測試
- 無障礙性測試
- 跨瀏覽器測試

---

**版本**: 2.0.0  
**更新日期**: 2024-12-19  
**維護者**: GameCore Team 