<template>
  <nav 
    class="gc-pagination" 
    :class="[`gc-pagination-${variant}`, { 'gc-pagination-compact': compact }]"
    role="navigation"
    :aria-label="ariaLabel"
  >
    <!-- 分頁資訊 -->
    <div v-if="showInfo" class="gc-pagination-info">
      <span class="gc-pagination-text">
        顯示 {{ startItem }}-{{ endItem }} 項，共 {{ totalItems }} 項
      </span>
    </div>
    
    <!-- 分頁控制項 -->
    <div class="gc-pagination-controls">
      <!-- 上一頁按鈕 -->
      <button
        class="gc-pagination-btn gc-pagination-prev"
        :disabled="currentPage <= 1"
        @click="goToPage(currentPage - 1)"
        :aria-label="`前往第 ${currentPage - 1} 頁`"
      >
        <span class="gc-pagination-icon">‹</span>
        <span v-if="!compact" class="gc-pagination-text">上一頁</span>
      </button>
      
      <!-- 頁碼按鈕 -->
      <div class="gc-pagination-pages">
        <!-- 第一頁 -->
        <button
          v-if="showFirstPage"
          class="gc-pagination-btn gc-pagination-page"
          :class="{ 'gc-pagination-page-active': currentPage === 1 }"
          @click="goToPage(1)"
          :aria-label="`前往第 1 頁`"
          :aria-current="currentPage === 1 ? 'page' : undefined"
        >
          1
        </button>
        
        <!-- 左側省略號 -->
        <span v-if="showLeftEllipsis" class="gc-pagination-ellipsis">...</span>
        
        <!-- 中間頁碼 -->
        <button
          v-for="page in visiblePages"
          :key="page"
          class="gc-pagination-btn gc-pagination-page"
          :class="{ 'gc-pagination-page-active': currentPage === page }"
          @click="goToPage(page)"
          :aria-label="`前往第 ${page} 頁`"
          :aria-current="currentPage === page ? 'page' : undefined"
        >
          {{ page }}
        </button>
        
        <!-- 右側省略號 -->
        <span v-if="showRightEllipsis" class="gc-pagination-ellipsis">...</span>
        
        <!-- 最後一頁 -->
        <button
          v-if="showLastPage"
          class="gc-pagination-btn gc-pagination-page"
          :class="{ 'gc-pagination-page-active': currentPage === totalPages }"
          @click="goToPage(totalPages)"
          :aria-label="`前往第 ${totalPages} 頁`"
          :aria-current="currentPage === totalPages ? 'page' : undefined"
        >
          {{ totalPages }}
        </button>
      </div>
      
      <!-- 下一頁按鈕 -->
      <button
        class="gc-pagination-btn gc-pagination-next"
        :disabled="currentPage >= totalPages"
        @click="goToPage(currentPage + 1)"
        :aria-label="`前往第 ${currentPage + 1} 頁`"
      >
        <span v-if="!compact" class="gc-pagination-text">下一頁</span>
        <span class="gc-pagination-icon">›</span>
      </button>
    </div>
    
    <!-- 快速跳轉 -->
    <div v-if="showQuickJump" class="gc-pagination-quick-jump">
      <span class="gc-pagination-text">跳至</span>
      <input
        type="number"
        :min="1"
        :max="totalPages"
        :value="jumpPage"
        @input="handleJumpInput"
        @keyup.enter="handleJumpEnter"
        class="gc-pagination-jump-input"
        :aria-label="`輸入頁碼，按 Enter 跳轉`"
      />
      <span class="gc-pagination-text">頁</span>
      <button
        @click="handleJumpEnter"
        class="gc-pagination-btn gc-pagination-jump-btn"
        :disabled="!isValidJumpPage"
      >
        跳轉
      </button>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'

// 組件屬性
interface Props {
  // 當前頁碼
  currentPage: number
  // 每頁項目數
  pageSize: number
  // 總項目數
  totalItems: number
  // 顯示的頁碼數量（不包括第一頁和最後一頁）
  maxVisiblePages?: number
  // 分頁樣式變體
  variant?: 'default' | 'minimal' | 'bordered'
  // 是否為緊湊模式
  compact?: boolean
  // 是否顯示分頁資訊
  showInfo?: boolean
  // 是否顯示快速跳轉
  showQuickJump?: boolean
  // 無障礙標籤
  ariaLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  maxVisiblePages: 5,
  variant: 'default',
  compact: false,
  showInfo: true,
  showQuickJump: false,
  ariaLabel: '分頁導航'
})

// 組件事件
const emit = defineEmits<{
  'update:currentPage': [page: number]
  'page-change': [page: number]
}>()

// 響應式狀態
const jumpPage = ref(props.currentPage)

// 計算屬性
const totalPages = computed(() => Math.ceil(props.totalItems / props.pageSize))

const startItem = computed(() => (props.currentPage - 1) * props.pageSize + 1)

const endItem = computed(() => Math.min(props.currentPage * props.pageSize, props.totalItems))

const isValidJumpPage = computed(() => {
  const page = Number(jumpPage.value)
  return page >= 1 && page <= totalPages.value && page !== props.currentPage
})

// 計算可見頁碼
const visiblePages = computed(() => {
  if (totalPages.value <= props.maxVisiblePages + 2) {
    // 如果總頁數不多，顯示所有頁碼
    return Array.from({ length: totalPages.value }, (_, i) => i + 1)
  }
  
  const halfVisible = Math.floor(props.maxVisiblePages / 2)
  let start = Math.max(2, props.currentPage - halfVisible)
  let end = Math.min(totalPages.value - 1, props.currentPage + halfVisible)
  
  // 調整邊界
  if (start === 2) {
    end = Math.min(totalPages.value - 1, start + props.maxVisiblePages - 1)
  } else if (end === totalPages.value - 1) {
    start = Math.max(2, end - props.maxVisiblePages + 1)
  }
  
  return Array.from({ length: end - start + 1 }, (_, i) => start + i)
})

const showFirstPage = computed(() => {
  return totalPages.value > 1 && visiblePages.value[0] > 1
})

const showLastPage = computed(() => {
  return totalPages.value > 1 && visiblePages.value[visiblePages.value.length - 1] < totalPages.value
})

const showLeftEllipsis = computed(() => {
  return visiblePages.value[0] > 2
})

const showRightEllipsis = computed(() => {
  return visiblePages.value[visiblePages.value.length - 1] < totalPages.value - 1
})

// 方法
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value && page !== props.currentPage) {
    emit('update:currentPage', page)
    emit('page-change', page)
    jumpPage.value = page
  }
}

const handleJumpInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  jumpPage.value = Number(target.value)
}

const handleJumpEnter = () => {
  if (isValidJumpPage.value) {
    goToPage(Number(jumpPage.value))
  }
}

// 監聽 currentPage 變化
watch(() => props.currentPage, (newPage) => {
  jumpPage.value = newPage
})
</script>

<style>
/* 分頁組件基礎樣式 */
.gc-pagination {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-4);
  align-items: center;
  font-family: inherit;
}

/* 分頁資訊 */
.gc-pagination-info {
  color: var(--gc-muted);
  font-size: var(--gc-text-sm);
}

.gc-pagination-text {
  color: inherit;
}

/* 分頁控制項 */
.gc-pagination-controls {
  display: flex;
  align-items: center;
  gap: var(--gc-space-2);
  flex-wrap: wrap;
  justify-content: center;
}

/* 分頁按鈕基礎樣式 */
.gc-pagination-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--gc-space-2);
  padding: var(--gc-space-2) var(--gc-space-3);
  border: 1px solid var(--gc-line);
  border-radius: var(--gc-radius-glass-sm);
  background: var(--gc-surface);
  color: var(--gc-ink);
  font-weight: var(--gc-font-bold);
  text-decoration: none;
  cursor: pointer;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  backdrop-filter: blur(var(--gc-blur-glass));
  font-size: var(--gc-text-sm);
  min-width: 40px;
  height: 40px;
}

.gc-pagination-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
  border-color: var(--gc-accent);
  transform: translateY(-1px);
  box-shadow: var(--gc-shadow-glass);
}

.gc-pagination-btn:focus {
  outline: 2px solid var(--gc-accent);
  outline-offset: 2px;
}

.gc-pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}

/* 頁碼按鈕 */
.gc-pagination-page {
  min-width: 40px;
}

.gc-pagination-page-active {
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-primary-500));
  color: white;
  border-color: transparent;
  box-shadow: var(--gc-shadow-glass);
}

.gc-pagination-page-active:hover {
  background: linear-gradient(135deg, var(--gc-accent-2), var(--gc-accent));
}

/* 上一頁/下一頁按鈕 */
.gc-pagination-prev,
.gc-pagination-next {
  padding: var(--gc-space-2) var(--gc-space-3);
}

.gc-pagination-icon {
  font-size: var(--gc-text-lg);
  font-weight: var(--gc-font-black);
  line-height: 1;
}

/* 省略號 */
.gc-pagination-ellipsis {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  color: var(--gc-muted);
  font-weight: var(--gc-font-bold);
  user-select: none;
}

/* 頁碼容器 */
.gc-pagination-pages {
  display: flex;
  align-items: center;
  gap: var(--gc-space-1);
}

/* 快速跳轉 */
.gc-pagination-quick-jump {
  display: flex;
  align-items: center;
  gap: var(--gc-space-2);
  color: var(--gc-muted);
  font-size: var(--gc-text-sm);
}

.gc-pagination-jump-input {
  width: 60px;
  padding: var(--gc-space-1) var(--gc-space-2);
  border: 1px solid var(--gc-line);
  border-radius: var(--gc-radius-glass-xs);
  background: var(--gc-surface);
  color: var(--gc-ink);
  text-align: center;
  font-size: var(--gc-text-sm);
  transition: border-color var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-pagination-jump-input:focus {
  outline: none;
  border-color: var(--gc-accent);
}

.gc-pagination-jump-btn {
  padding: var(--gc-space-1) var(--gc-space-2);
  font-size: var(--gc-text-xs);
  min-width: auto;
  height: auto;
}

/* 分頁樣式變體 */
.gc-pagination-minimal .gc-pagination-btn {
  background: transparent;
  border-color: transparent;
}

.gc-pagination-minimal .gc-pagination-btn:hover:not(:disabled) {
  background: rgba(0, 0, 0, 0.05);
  border-color: var(--gc-line);
}

.gc-pagination-bordered .gc-pagination-btn {
  border-width: 2px;
}

.gc-pagination-bordered .gc-pagination-page-active {
  border-width: 2px;
}

/* 緊湊模式 */
.gc-pagination-compact .gc-pagination-btn {
  padding: var(--gc-space-1) var(--gc-space-2);
  min-width: 36px;
  height: 36px;
  font-size: var(--gc-text-xs);
}

.gc-pagination-compact .gc-pagination-ellipsis {
  width: 36px;
  height: 36px;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .gc-pagination-controls {
    gap: var(--gc-space-1);
  }
  
  .gc-pagination-btn {
    padding: var(--gc-space-1) var(--gc-space-2);
    min-width: 36px;
    height: 36px;
    font-size: var(--gc-text-xs);
  }
  
  .gc-pagination-prev .gc-pagination-text,
  .gc-pagination-next .gc-pagination-text {
    display: none;
  }
  
  .gc-pagination-quick-jump {
    flex-direction: column;
    gap: var(--gc-space-1);
    text-align: center;
  }
}

@media (max-width: 480px) {
  .gc-pagination-pages {
    gap: 2px;
  }
  
  .gc-pagination-btn {
    min-width: 32px;
    height: 32px;
    padding: var(--gc-space-1);
  }
  
  .gc-pagination-ellipsis {
    width: 32px;
    height: 32px;
  }
}

/* 深色模式 */
body.dark .gc-pagination-btn {
  background: var(--gc-surface);
  border-color: var(--gc-line);
}

body.dark .gc-pagination-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.05);
}

body.dark .gc-pagination-jump-input {
  background: var(--gc-surface);
  border-color: var(--gc-line);
  color: var(--gc-ink);
}
</style> 