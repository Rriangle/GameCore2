<template>
  <div class="component-library">
    <!-- 頁面標題 -->
    <div class="page-header">
      <h1 class="page-title">🎨 GameCore UI 組件庫</h1>
      <p class="page-subtitle">完整的玻璃風設計系統組件庫，支援響應式設計和無障礙性</p>
    </div>

    <!-- 組件分類導航 -->
    <nav class="component-nav">
      <button 
        v-for="category in categories" 
        :key="category.id"
        :class="['nav-btn', { active: activeCategory === category.id }]"
        @click="activeCategory = category.id"
      >
        {{ category.name }}
        <span class="component-count">{{ category.components.length }}</span>
      </button>
    </nav>

    <!-- 組件展示區域 -->
    <div class="component-showcase">
      <!-- 基礎組件 -->
      <section v-if="activeCategory === 'basic'" class="component-section">
        <h2 class="section-title">🔧 基礎組件</h2>
        
        <!-- 按鈕組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCButton 按鈕組件</h3>
          <div class="demo-area">
            <GCButton variant="primary" size="lg">主要按鈕</GCButton>
            <GCButton variant="secondary" size="md">次要按鈕</GCButton>
            <GCButton variant="danger" size="sm">危險按鈕</GCButton>
            <GCButton variant="ghost" size="md">幽靈按鈕</GCButton>
            <GCButton variant="link" size="md">連結按鈕</GCButton>
          </div>
          <div class="demo-area">
            <GCButton variant="primary" size="lg" :loading="true">載入中...</GCButton>
            <GCButton variant="secondary" size="md" disabled>已禁用</GCButton>
          </div>
          <div class="code-example">
            <pre><code>&lt;GCButton variant="primary" size="lg"&gt;主要按鈕&lt;/GCButton&gt;
&lt;GCButton variant="secondary" size="md"&gt;次要按鈕&lt;/GCButton&gt;
&lt;GCButton variant="danger" size="sm"&gt;危險按鈕&lt;/GCButton&gt;</code></pre>
          </div>
        </div>

        <!-- 輸入框組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCInput 輸入框組件</h3>
          <div class="demo-area">
            <GCInput 
              v-model="inputValue" 
              placeholder="請輸入文字..." 
              type="text"
              size="lg"
            />
            <GCInput 
              v-model="emailValue" 
              placeholder="請輸入電子郵件" 
              type="email"
              size="md"
            />
            <GCInput 
              v-model="passwordValue" 
              placeholder="請輸入密碼" 
              type="password"
              size="sm"
            />
          </div>
          <div class="demo-area">
            <GCInput 
              v-model="searchValue" 
              placeholder="搜尋..." 
              type="search"
              size="md"
              :error="searchError"
              error-message="搜尋關鍵字不能為空"
            />
          </div>
          <div class="code-example">
            <pre><code>&lt;GCInput 
  v-model="inputValue" 
  placeholder="請輸入文字..." 
  type="text"
  size="lg"
/&gt;</code></pre>
          </div>
        </div>

        <!-- 卡片組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCCard 卡片組件</h3>
          <div class="demo-area cards-grid">
            <GCCard 
              title="基礎卡片" 
              subtitle="這是一個基礎卡片組件"
              size="md"
            >
              <template #content>
                <p>卡片內容區域，可以放置任何內容。</p>
              </template>
              <template #footer>
                <div class="card-actions">
                  <GCButton variant="primary" size="sm">確認</GCButton>
                  <GCButton variant="ghost" size="sm">取消</GCButton>
                </div>
              </template>
            </GCCard>

            <GCCard 
              title="互動卡片" 
              subtitle="支援點擊事件的卡片"
              size="md"
              :clickable="true"
              @click="handleCardClick"
            >
              <template #content>
                <p>點擊此卡片會觸發事件。</p>
              </template>
            </GCCard>
          </div>
          <div class="code-example">
            <pre><code>&lt;GCCard 
  title="基礎卡片" 
  subtitle="這是一個基礎卡片組件"
  size="md"
&gt;
  &lt;template #content&gt;
    &lt;p&gt;卡片內容區域，可以放置任何內容。&lt;/p&gt;
  &lt;/template&gt;
&lt;/GCCard&gt;</code></pre>
          </div>
        </div>
      </section>

      <!-- 數據展示組件 -->
      <section v-if="activeCategory === 'data'" class="component-section">
        <h2 class="section-title">📊 數據展示組件</h2>
        
        <!-- 表格組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCTable 表格組件</h3>
          <div class="demo-area">
            <GCTable 
              :data="tableData" 
              :columns="tableColumns"
              :sortable="true"
              :pagination="true"
              :page-size="5"
              size="md"
            />
          </div>
          <div class="code-example">
            <pre><code>&lt;GCTable 
  :data="tableData" 
  :columns="tableColumns"
  :sortable="true"
  :pagination="true"
  :page-size="5"
  size="md"
/&gt;</code></pre>
          </div>
        </div>

        <!-- 分頁組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCPagination 分頁組件</h3>
          <div class="demo-area">
            <GCPagination 
              v-model:current-page="currentPage"
              :total-pages="10"
              :show-quick-jump="true"
              :show-page-size="true"
              :page-size="20"
              size="md"
            />
          </div>
          <div class="code-example">
            <pre><code>&lt;GCPagination 
  v-model:current-page="currentPage"
  :total-pages="10"
  :show-quick-jump="true"
  :show-page-size="true"
  :page-size="20"
  size="md"
/&gt;</code></pre>
          </div>
        </div>

        <!-- 標籤頁組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCTabs 標籤頁組件</h3>
          <div class="demo-area">
            <GCTabs 
              v-model:active-tab="activeTab"
              :tabs="tabsData"
              variant="pills"
              size="md"
            />
            <div class="tab-content">
              <div v-if="activeTab === 'tab1'" class="tab-panel">
                <h4>標籤頁 1 內容</h4>
                <p>這是第一個標籤頁的內容區域。</p>
              </div>
              <div v-else-if="activeTab === 'tab2'" class="tab-panel">
                <h4>標籤頁 2 內容</h4>
                <p>這是第二個標籤頁的內容區域。</p>
              </div>
              <div v-else-if="activeTab === 'tab3'" class="tab-panel">
                <h4>標籤頁 3 內容</h4>
                <p>這是第三個標籤頁的內容區域。</p>
              </div>
            </div>
          </div>
          <div class="code-example">
            <pre><code>&lt;GCTabs 
  v-model:active-tab="activeTab"
  :tabs="tabsData"
  variant="pills"
  size="md"
/&gt;</code></pre>
          </div>
        </div>

        <!-- 手風琴組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCAccordion 手風琴組件</h3>
          <div class="demo-area">
            <GCAccordion 
              :items="accordionItems"
              variant="bordered"
              size="md"
              :multiple="true"
            />
          </div>
          <div class="code-example">
            <pre><code>&lt;GCAccordion 
  :items="accordionItems"
  variant="bordered"
  size="md"
  :multiple="true"
/&gt;</code></pre>
          </div>
        </div>
      </section>

      <!-- 反饋組件 -->
      <section v-if="activeCategory === 'feedback'" class="component-section">
        <h2 class="section-title">💬 反饋組件</h2>
        
        <!-- 模態框組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCModal 模態框組件</h3>
          <div class="demo-area">
            <GCButton variant="primary" @click="showModal = true">
              開啟模態框
            </GCButton>
            <GCModal 
              v-model:visible="showModal"
              title="確認操作"
              size="md"
              :closable="true"
            >
              <template #content>
                <p>這是一個模態框組件，用於顯示重要的操作確認或信息。</p>
              </template>
              <template #footer>
                <div class="modal-actions">
                  <GCButton variant="ghost" @click="showModal = false">取消</GCButton>
                  <GCButton variant="primary" @click="handleConfirm">確認</GCButton>
                </div>
              </template>
            </GCModal>
          </div>
          <div class="code-example">
            <pre><code>&lt;GCModal 
  v-model:visible="showModal"
  title="確認操作"
  size="md"
  :closable="true"
&gt;
  &lt;template #content&gt;
    &lt;p&gt;模態框內容&lt;/p&gt;
  &lt;/template&gt;
&lt;/GCModal&gt;</code></pre>
          </div>
        </div>

        <!-- 通知組件 -->
        <div class="component-demo">
          <h3 class="component-name">GCToast 通知組件</h3>
          <div class="demo-area">
            <div class="toast-buttons">
              <GCButton variant="primary" @click="showSuccessToast">
                成功通知
              </GCButton>
              <GCButton variant="secondary" @click="showInfoToast">
                信息通知
              </GCButton>
              <GCButton variant="danger" @click="showErrorToast">
                錯誤通知
              </GCButton>
              <GCButton variant="warning" @click="showWarningToast">
                警告通知
              </GCButton>
            </div>
          </div>
          <div class="code-example">
            <pre><code>// 使用 Toast 服務
import { useToast } from '@/components/ui/useToast'

const { showToast } = useToast()

showToast({
  type: 'success',
  title: '操作成功',
  message: '您的操作已完成'
})</code></pre>
          </div>
        </div>

        <!-- Toast 容器組件 -->
        <div class="component-demo">
          <h3 class="component-name">ToastContainer 通知容器</h3>
          <div class="demo-area">
            <p>ToastContainer 組件會自動顯示所有通知，支援多種位置和動畫效果。</p>
            <div class="toast-buttons">
              <GCButton variant="primary" @click="showCustomToast">
                自定義位置通知
              </GCButton>
              <GCButton variant="secondary" @click="showLongToast">
                長時間通知
              </GCButton>
            </div>
          </div>
          <div class="code-example">
            <pre><code>&lt;!-- 在 App.vue 中添加 --&gt;
&lt;ToastContainer /&gt;

// 在組件中使用
showToast({
  type: 'info',
  title: '自定義通知',
  message: '這是一個自定義位置的通知',
  position: 'bottom-center',
  duration: 8000
})</code></pre>
          </div>
        </div>
      </section>

      <!-- 導航組件 -->
      <section v-if="activeCategory === 'navigation'" class="component-section">
        <h2 class="section-title">🧭 導航組件</h2>
        
        <!-- 麵包屑導航 -->
        <div class="component-demo">
          <h3 class="component-name">麵包屑導航</h3>
          <div class="demo-area">
            <nav class="breadcrumb" aria-label="麵包屑導航">
              <ol class="breadcrumb-list">
                <li class="breadcrumb-item">
                  <a href="#" class="breadcrumb-link">首頁</a>
                </li>
                <li class="breadcrumb-item">
                  <a href="#" class="breadcrumb-link">組件庫</a>
                </li>
                <li class="breadcrumb-item active" aria-current="page">
                  導航組件
                </li>
              </ol>
            </nav>
          </div>
          <div class="code-example">
            <pre><code>&lt;nav class="breadcrumb" aria-label="麵包屑導航"&gt;
  &lt;ol class="breadcrumb-list"&gt;
    &lt;li class="breadcrumb-item"&gt;
      &lt;a href="#" class="breadcrumb-link"&gt;首頁&lt;/a&gt;
    &lt;/li&gt;
    &lt;li class="breadcrumb-item active" aria-current="page"&gt;
      當前頁面
    &lt;/li&gt;
  &lt;/ol&gt;
&lt;/nav&gt;</code></pre>
          </div>
        </div>
      </section>
    </div>

    <!-- 組件統計信息 -->
    <div class="component-stats">
      <div class="stat-card">
        <div class="stat-number">{{ totalComponents }}</div>
        <div class="stat-label">總組件數</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">{{ totalCategories }}</div>
        <div class="stat-label">分類數</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">100%</div>
        <div class="stat-label">響應式支援</div>
      </div>
      <div class="stat-card">
        <div class="stat-number">WCAG 2.1</div>
        <div class="stat-label">無障礙標準</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import GCButton from './GCButton.vue'
import GCInput from './GCInput.vue'
import GCCard from './GCCard.vue'
import GCTable from './GCTable.vue'
import GCPagination from './GCPagination.vue'
import GCTabs from './GCTabs.vue'
import GCAccordion from './GCAccordion.vue'
import GCModal from './GCModal.vue'
import { useToast } from './useToast'

// 響應式數據
const activeCategory = ref('basic')
const { showToast } = useToast()
const inputValue = ref('')
const emailValue = ref('')
const passwordValue = ref('')
const searchValue = ref('')
const searchError = ref(false)
const currentPage = ref(1)
const activeTab = ref('tab1')
const showModal = ref(false)

// 表格數據
const tableData = ref([
  { id: 1, name: '張三', email: 'zhang@example.com', role: '管理員', status: '活躍' },
  { id: 2, name: '李四', email: 'li@example.com', role: '用戶', status: '活躍' },
  { id: 3, name: '王五', email: 'wang@example.com', role: '用戶', status: '停用' },
  { id: 4, name: '趙六', email: 'zhao@example.com', role: '版主', status: '活躍' },
  { id: 5, name: '錢七', email: 'qian@example.com', role: '用戶', status: '活躍' }
])

const tableColumns = ref([
  { key: 'id', label: 'ID', sortable: true, width: '80px' },
  { key: 'name', label: '姓名', sortable: true },
  { key: 'email', label: '電子郵件', sortable: false },
  { key: 'role', label: '角色', sortable: true },
  { key: 'status', label: '狀態', sortable: true }
])

// 標籤頁數據
const tabsData = ref([
  { id: 'tab1', label: '標籤頁 1', icon: '📊' },
  { id: 'tab2', label: '標籤頁 2', icon: '📈' },
  { id: 'tab3', label: '標籤頁 3', icon: '📋' }
])

// 手風琴數據
const accordionItems = ref([
  {
    id: 'item1',
    title: '什麼是 GameCore？',
    content: 'GameCore 是一個專為遊戲社群設計的現代化平台，提供論壇、排行榜、寵物系統等功能。'
  },
  {
    id: 'item2',
    title: '如何開始使用？',
    content: '註冊帳號後，您可以瀏覽論壇、參與討論、養成史萊姆寵物，並與其他玩家互動。'
  },
  {
    id: 'item3',
    title: '支援哪些功能？',
    content: '我們支援論壇討論、排行榜系統、寵物養成、積分系統、深色模式等多種功能。'
  }
])

// 組件分類
const categories = ref([
  {
    id: 'basic',
    name: '基礎組件',
    components: ['GCButton', 'GCInput', 'GCCard']
  },
  {
    id: 'data',
    name: '數據展示',
    components: ['GCTable', 'GCPagination', 'GCTabs', 'GCAccordion']
  },
  {
    id: 'feedback',
    name: '反饋組件',
    components: ['GCModal', 'GCToast', 'ToastContainer']
  },
  {
    id: 'navigation',
    name: '導航組件',
    components: ['Breadcrumb', 'Navigation']
  }
])

// 計算屬性
const totalComponents = computed(() => 
  categories.value.reduce((total, category) => total + category.components.length, 0)
)

const totalCategories = computed(() => categories.value.length)

// 事件處理
const handleCardClick = () => {
  console.log('卡片被點擊了！')
}

const handleConfirm = () => {
  console.log('確認操作')
  showModal.value = false
}

// Toast 通知方法
const showSuccessToast = () => {
  showToast({
    type: 'success',
    title: '操作成功',
    message: '您的操作已完成！'
  })
}

const showInfoToast = () => {
  showToast({
    type: 'info',
    title: '信息提示',
    message: '這是一條信息通知。'
  })
}

const showErrorToast = () => {
  showToast({
    type: 'error',
    title: '操作失敗',
    message: '請檢查輸入並重試。'
  })
}

const showWarningToast = () => {
  showToast({
    type: 'warning',
    title: '注意事項',
    message: '請注意相關操作規範。'
  })
}

const showCustomToast = () => {
  showToast({
    type: 'info',
    title: '自定義通知',
    message: '這是一個自定義位置的通知',
    position: 'bottom-center',
    duration: 8000
  })
}

const showLongToast = () => {
  showToast({
    type: 'info',
    title: '長時間通知',
    message: '這個通知會顯示 10 秒鐘',
    duration: 10000,
    position: 'top-center'
  })
}
</script>

<style scoped>
.component-library {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

/* 頁面標題 */
.page-header {
  text-align: center;
  margin-bottom: 3rem;
}

.page-title {
  font-size: 3rem;
  font-weight: 900;
  background: linear-gradient(135deg, #7557ff, #34d2ff);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin-bottom: 1rem;
}

.page-subtitle {
  font-size: 1.25rem;
  color: #64748b;
  max-width: 600px;
  margin: 0 auto;
}

/* 組件導航 */
.component-nav {
  display: flex;
  gap: 1rem;
  margin-bottom: 3rem;
  flex-wrap: wrap;
  justify-content: center;
}

.nav-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  border: 1px solid #e5e7eb;
  background: rgba(255, 255, 255, 0.75);
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s ease;
  font-weight: 600;
}

.nav-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.nav-btn.active {
  background: linear-gradient(135deg, #7557ff, #3b82f6);
  color: white;
  border-color: transparent;
}

.component-count {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.25rem 0.5rem;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
}

/* 組件展示區域 */
.component-section {
  margin-bottom: 4rem;
}

.section-title {
  font-size: 2rem;
  font-weight: 800;
  margin-bottom: 2rem;
  color: #1f2937;
  border-bottom: 2px solid #e5e7eb;
  padding-bottom: 1rem;
}

.component-demo {
  background: rgba(255, 255, 255, 0.75);
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 2rem;
  margin-bottom: 2rem;
  backdrop-filter: blur(14px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.08);
}

.component-name {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  color: #1f2937;
}

.demo-area {
  margin-bottom: 1.5rem;
  padding: 1.5rem;
  background: #f8fafc;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
}

.demo-area .gc-button,
.demo-area .gc-input,
.demo-area .gc-card {
  margin-right: 1rem;
  margin-bottom: 1rem;
}

.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1rem;
}

.toast-buttons {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.modal-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.tab-content {
  margin-top: 1rem;
  padding: 1rem;
  background: #f8fafc;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.tab-panel h4 {
  margin-bottom: 0.5rem;
  color: #1f2937;
}

/* 代碼示例 */
.code-example {
  background: #1e293b;
  border-radius: 8px;
  overflow: hidden;
}

.code-example pre {
  margin: 0;
  padding: 1rem;
  color: #e2e8f0;
  font-family: 'Fira Code', 'Monaco', 'Consolas', monospace;
  font-size: 0.875rem;
  line-height: 1.5;
  overflow-x: auto;
}

.code-example code {
  color: inherit;
}

/* 麵包屑導航 */
.breadcrumb {
  margin-bottom: 1rem;
}

.breadcrumb-list {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  list-style: none;
  margin: 0;
  padding: 0;
}

.breadcrumb-item {
  display: flex;
  align-items: center;
}

.breadcrumb-item:not(:last-child)::after {
  content: '/';
  margin-left: 0.5rem;
  color: #9ca3af;
}

.breadcrumb-link {
  color: #3b82f6;
  text-decoration: none;
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  transition: background-color 0.2s ease;
}

.breadcrumb-link:hover {
  background-color: #f3f4f6;
}

.breadcrumb-item.active .breadcrumb-link {
  color: #6b7280;
  font-weight: 600;
}

/* 組件統計 */
.component-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-top: 4rem;
}

.stat-card {
  background: rgba(255, 255, 255, 0.75);
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 2rem;
  text-align: center;
  backdrop-filter: blur(14px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.08);
  transition: transform 0.2s ease;
}

.stat-card:hover {
  transform: translateY(-4px);
}

.stat-number {
  font-size: 2.5rem;
  font-weight: 900;
  color: #7557ff;
  margin-bottom: 0.5rem;
}

.stat-label {
  color: #64748b;
  font-weight: 600;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .component-library {
    padding: 1rem;
  }
  
  .page-title {
    font-size: 2rem;
  }
  
  .component-nav {
    flex-direction: column;
    align-items: center;
  }
  
  .nav-btn {
    width: 100%;
    max-width: 300px;
    justify-content: center;
  }
  
  .cards-grid {
    grid-template-columns: 1fr;
  }
  
  .component-stats {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 480px) {
  .component-stats {
    grid-template-columns: 1fr;
  }
  
  .demo-area {
    padding: 1rem;
  }
  
  .toast-buttons {
    flex-direction: column;
  }
}
</style> 