<template>
  <div class="gc-table-container">
    <!-- 表格標題和操作區域 -->
    <div v-if="title || $slots.header" class="gc-table-header">
      <h3 v-if="title" class="gc-table-title">{{ title }}</h3>
      <div v-if="$slots.header" class="gc-table-actions">
        <slot name="header" />
      </div>
    </div>

    <!-- 表格內容 -->
    <div class="gc-table-wrapper">
      <table class="gc-table" :class="{ 'gc-table-striped': striped, 'gc-table-hover': hover }">
        <thead class="gc-table-head">
          <tr>
            <th 
              v-for="column in columns" 
              :key="column.key"
              :class="[
                'gc-table-th',
                { 'gc-table-th-sortable': column.sortable },
                { 'gc-table-th-sorted': sortKey === column.key }
              ]"
              :style="{ width: column.width }"
              @click="column.sortable ? handleSort(column.key) : null"
            >
              <div class="gc-table-th-content">
                <span>{{ column.label }}</span>
                <span v-if="column.sortable" class="gc-table-sort-icon">
                  <svg 
                    v-if="sortKey !== column.key" 
                    class="gc-table-sort-default" 
                    viewBox="0 0 24 24" 
                    fill="none"
                  >
                    <path d="M7 14l5-5 5 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                  <svg 
                    v-else-if="sortOrder === 'asc'" 
                    class="gc-table-sort-asc" 
                    viewBox="0 0 24 24" 
                    fill="none"
                  >
                    <path d="M18 15l-6-6-6 6" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                  <svg 
                    v-else 
                    class="gc-table-sort-desc" 
                    viewBox="0 0 24 24" 
                    fill="none"
                  >
                    <path d="M7 14l5-5 5 5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </span>
              </div>
            </th>
          </tr>
        </thead>
        
        <tbody class="gc-table-body">
          <tr 
            v-for="(row, index) in paginatedData" 
            :key="getRowKey(row, index)"
            class="gc-table-row"
            :class="{ 'gc-table-row-selected': isRowSelected(row) }"
            @click="handleRowClick(row, index)"
          >
            <td 
              v-for="column in columns" 
              :key="column.key"
              class="gc-table-td"
              :class="column.className"
            >
              <slot 
                :name="`cell-${column.key}`" 
                :row="row" 
                :column="column" 
                :value="getCellValue(row, column.key)"
              >
                {{ formatCellValue(getCellValue(row, column.key), column) }}
              </slot>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- 空狀態 -->
      <div v-if="paginatedData.length === 0" class="gc-table-empty">
        <div class="gc-table-empty-icon">📋</div>
        <div class="gc-table-empty-text">{{ emptyText || '暫無資料' }}</div>
        <div v-if="$slots.empty" class="gc-table-empty-actions">
          <slot name="empty" />
        </div>
      </div>
    </div>

    <!-- 分頁控制 -->
    <div v-if="showPagination && totalPages > 1" class="gc-table-pagination">
      <div class="gc-table-pagination-info">
        顯示 {{ startIndex + 1 }}-{{ endIndex }} 筆，共 {{ totalItems }} 筆
      </div>
      
      <div class="gc-table-pagination-controls">
        <button 
          class="gc-btn gc-btn-secondary gc-btn-sm"
          :disabled="currentPage === 1"
          @click="goToPage(currentPage - 1)"
        >
          上一頁
        </button>
        
        <div class="gc-table-pagination-pages">
          <button 
            v-for="page in visiblePages" 
            :key="page"
            :class="[
              'gc-btn gc-btn-sm',
              page === currentPage ? 'gc-btn-primary' : 'gc-btn-secondary'
            ]"
            @click="goToPage(page)"
          >
            {{ page }}
          </button>
        </div>
        
        <button 
          class="gc-btn gc-btn-secondary gc-btn-sm"
          :disabled="currentPage === totalPages"
          @click="goToPage(currentPage + 1)"
        >
          下一頁
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'

// 表格列定義介面
export interface TableColumn {
  key: string
  label: string
  width?: string
  sortable?: boolean
  className?: string
  formatter?: (value: any) => string
}

// 組件屬性
interface Props {
  // 表格標題
  title?: string
  // 表格列定義
  columns: TableColumn[]
  // 表格資料
  data: any[]
  // 行鍵值函數
  rowKey?: string | ((row: any) => string | number)
  // 是否顯示條紋
  striped?: boolean
  // 是否顯示懸停效果
  hover?: boolean
  // 分頁設定
  pagination?: {
    pageSize: number
    currentPage?: number
  }
  // 排序設定
  sortable?: boolean
  // 空狀態文字
  emptyText?: string
  // 選中的行
  selectedRows?: any[]
}

// 組件事件
interface Emits {
  (e: 'sort', key: string, order: 'asc' | 'desc'): void
  (e: 'row-click', row: any, index: number): void
  (e: 'selection-change', rows: any[]): void
  (e: 'page-change', page: number): void
}

const props = withDefaults(defineProps<Props>(), {
  striped: true,
  hover: true,
  sortable: true,
  emptyText: '暫無資料'
})

const emit = defineEmits<Emits>()

// 內部狀態
const currentPage = ref(props.pagination?.currentPage || 1)
const sortKey = ref<string>('')
const sortOrder = ref<'asc' | 'desc'>('asc')
const selectedRows = ref<any[]>(props.selectedRows || [])

// 計算屬性
const totalItems = computed(() => props.data.length)
const totalPages = computed(() => {
  if (!props.pagination) return 1
  return Math.ceil(totalItems.value / props.pagination.pageSize)
})

const startIndex = computed(() => {
  if (!props.pagination) return 0
  return (currentPage.value - 1) * props.pagination.pageSize
})

const endIndex = computed(() => {
  if (!props.pagination) return totalItems.value
  return Math.min(startIndex.value + props.pagination.pageSize, totalItems.value)
})

const sortedData = computed(() => {
  if (!sortKey.value) return props.data
  
  return [...props.data].sort((a, b) => {
    const aVal = getCellValue(a, sortKey.value)
    const bVal = getCellValue(b, sortKey.value)
    
    if (aVal === bVal) return 0
    
    const comparison = aVal < bVal ? -1 : 1
    return sortOrder.value === 'asc' ? comparison : -comparison
  })
})

const paginatedData = computed(() => {
  if (!props.pagination) return sortedData.value
  
  return sortedData.value.slice(startIndex.value, endIndex.value)
})

const visiblePages = computed(() => {
  const pages: number[] = []
  const maxVisible = 5
  const halfVisible = Math.floor(maxVisible / 2)
  
  let start = Math.max(1, currentPage.value - halfVisible)
  let end = Math.min(totalPages.value, start + maxVisible - 1)
  
  if (end - start + 1 < maxVisible) {
    start = Math.max(1, end - maxVisible + 1)
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

// 方法
const getRowKey = (row: any, index: number): string | number => {
  if (typeof props.rowKey === 'string') {
    return row[props.rowKey]
  }
  if (typeof props.rowKey === 'function') {
    return props.rowKey(row)
  }
  return index
}

const getCellValue = (row: any, key: string): any => {
  return key.split('.').reduce((obj, k) => obj?.[k], row)
}

const formatCellValue = (value: any, column: TableColumn): string => {
  if (column.formatter) {
    return column.formatter(value)
  }
  
  if (value === null || value === undefined) {
    return '-'
  }
  
  if (typeof value === 'boolean') {
    return value ? '是' : '否'
  }
  
  return String(value)
}

const handleSort = (key: string) => {
  if (sortKey.value === key) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value = key
    sortOrder.value = 'asc'
  }
  
  emit('sort', key, sortOrder.value)
}

const handleRowClick = (row: any, index: number) => {
  emit('row-click', row, index)
}

const goToPage = (page: number) => {
  if (page < 1 || page > totalPages.value) return
  
  currentPage.value = page
  emit('page-change', page)
}

const isRowSelected = (row: any): boolean => {
  return selectedRows.value.some(selected => getRowKey(selected, 0) === getRowKey(row, 0))
}

// 監聽外部變化
watch(() => props.selectedRows, (newRows) => {
  selectedRows.value = newRows || []
}, { deep: true })

watch(() => props.pagination?.currentPage, (newPage) => {
  if (newPage) {
    currentPage.value = newPage
  }
})
</script>

<style scoped>
/* 玻璃風表格組件樣式 */
.gc-table-container {
  @apply gc-card;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

/* 表格標題區域 */
.gc-table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--gc-space-6);
  border-bottom: 1px solid var(--gc-line);
}

.gc-table-title {
  @apply text-xl font-bold;
  color: var(--gc-ink);
}

.gc-table-actions {
  display: flex;
  gap: var(--gc-space-3);
  align-items: center;
}

/* 表格包裝器 */
.gc-table-wrapper {
  overflow-x: auto;
  border-radius: var(--gc-radius-lg);
}

/* 表格基礎樣式 */
.gc-table {
  width: 100%;
  border-collapse: collapse;
  font-size: var(--gc-text-sm);
}

/* 表格標題行 */
.gc-table-head {
  background: var(--gc-surface-2);
  border-bottom: 2px solid var(--gc-line);
}

.gc-table-th {
  @apply px-4 py-3 text-left font-bold;
  color: var(--gc-ink);
  border-bottom: 1px solid var(--gc-line);
  position: relative;
}

.gc-table-th-sortable {
  cursor: pointer;
  user-select: none;
}

.gc-table-th-sortable:hover {
  background: var(--gc-surface);
}

.gc-table-th-sorted {
  background: var(--gc-accent);
  color: white;
}

.gc-table-th-content {
  display: flex;
  align-items: center;
  gap: var(--gc-space-2);
}

.gc-table-sort-icon {
  @apply w-4 h-4;
  color: currentColor;
  opacity: 0.6;
}

.gc-table-th-sorted .gc-table-sort-icon {
  opacity: 1;
}

.gc-table-sort-default,
.gc-table-sort-asc,
.gc-table-sort-desc {
  @apply w-4 h-4;
}

/* 表格主體 */
.gc-table-body {
  background: var(--gc-surface);
}

.gc-table-row {
  border-bottom: 1px solid var(--gc-line);
  transition: all var(--gc-transition);
}

.gc-table-row:hover {
  background: var(--gc-surface-2);
}

.gc-table-row-selected {
  background: rgba(117, 87, 255, 0.1);
  border-color: var(--gc-accent);
}

.gc-table-striped .gc-table-row:nth-child(even) {
  background: var(--gc-surface-2);
}

.gc-table-striped .gc-table-row:nth-child(even):hover {
  background: var(--gc-surface-3);
}

.gc-table-td {
  @apply px-4 py-3;
  color: var(--gc-ink);
  vertical-align: top;
}

/* 空狀態 */
.gc-table-empty {
  @apply py-12 text-center;
  color: var(--gc-muted);
}

.gc-table-empty-icon {
  @apply text-4xl mb-4;
}

.gc-table-empty-text {
  @apply text-lg font-medium mb-4;
}

.gc-table-empty-actions {
  display: flex;
  justify-content: center;
  gap: var(--gc-space-3);
}

/* 分頁控制 */
.gc-table-pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--gc-space-6);
  border-top: 1px solid var(--gc-line);
  background: var(--gc-surface-2);
}

.gc-table-pagination-info {
  @apply text-sm;
  color: var(--gc-muted);
}

.gc-table-pagination-controls {
  display: flex;
  align-items: center;
  gap: var(--gc-space-3);
}

.gc-table-pagination-pages {
  display: flex;
  gap: var(--gc-space-1);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .gc-table-header {
    flex-direction: column;
    align-items: flex-start;
    gap: var(--gc-space-3);
  }
  
  .gc-table-pagination {
    flex-direction: column;
    gap: var(--gc-space-3);
  }
  
  .gc-table-pagination-controls {
    flex-wrap: wrap;
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .gc-table-th,
  .gc-table-td {
    @apply px-2 py-2;
  }
  
  .gc-table-header {
    @apply p-4;
  }
  
  .gc-table-pagination {
    @apply p-4;
  }
}
</style> 