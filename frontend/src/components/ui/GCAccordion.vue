<template>
  <div class="gc-accordion" :class="[`gc-accordion-${variant}`, { 'gc-accordion-compact': compact }]">
    <div
      v-for="(item, index) in items"
      :key="item.key || index"
      class="gc-accordion-item"
      :class="{ 'gc-accordion-item-expanded': isExpanded(item.key || index) }"
    >
      <!-- 手風琴標題 -->
      <button
        :id="`accordion-header-${item.key || index}`"
        :aria-controls="`accordion-panel-${item.key || index}`"
        :aria-expanded="isExpanded(item.key || index)"
        :aria-disabled="item.disabled"
        :disabled="item.disabled"
        class="gc-accordion-header"
        @click="toggleItem(item.key || index)"
        type="button"
      >
        <!-- 標題圖標 -->
        <span v-if="item.icon" class="gc-accordion-icon" :aria-hidden="true">
          {{ item.icon }}
        </span>
        
        <!-- 標題文字 -->
        <span class="gc-accordion-title">{{ item.title }}</span>
        
        <!-- 標題徽章 -->
        <span v-if="item.badge" class="gc-accordion-badge">
          {{ item.badge }}
        </span>
        
        <!-- 展開/收合箭頭 -->
        <span class="gc-accordion-arrow" :aria-hidden="true">
          {{ isExpanded(item.key || index) ? '▼' : '▶' }}
        </span>
      </button>
      
      <!-- 手風琴內容 -->
      <div
        :id="`accordion-panel-${item.key || index}`"
        :aria-labelledby="`accordion-header-${item.key || index}`"
        class="gc-accordion-panel"
        :class="{ 'gc-accordion-panel-expanded': isExpanded(item.key || index) }"
        :hidden="!isExpanded(item.key || index)"
      >
        <div class="gc-accordion-content">
          <slot :name="item.key || index" :item="item" :index="index">
            {{ item.content || item.title }}
          </slot>
        </div>
      </div>
    </div>
    
    <!-- 空狀態 -->
    <div v-if="items.length === 0" class="gc-accordion-empty">
      <div class="gc-accordion-empty-icon">📋</div>
      <p class="gc-accordion-empty-text">暫無手風琴項目</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

// 手風琴項目介面
interface AccordionItem {
  key: string | number
  title: string
  content?: string
  icon?: string
  badge?: string | number
  disabled?: boolean
  defaultExpanded?: boolean
}

// 組件屬性
interface Props {
  // 手風琴項目列表
  items: AccordionItem[]
  // 手風琴樣式變體
  variant?: 'default' | 'minimal' | 'bordered' | 'pills'
  // 是否為緊湊模式
  compact?: boolean
  // 是否允許多個項目同時展開
  multiple?: boolean
  // 預設展開的項目
  defaultExpanded?: (string | number)[]
  // 無障礙標籤
  ariaLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  compact: false,
  multiple: false,
  defaultExpanded: () => [],
  ariaLabel: '手風琴導航'
})

// 組件事件
const emit = defineEmits<{
  'item-toggle': [key: string | number, expanded: boolean]
  'item-expand': [key: string | number]
  'item-collapse': [key: string | number]
}>()

// 響應式狀態
const expandedItems = ref<Set<string | number>>(new Set())

// 初始化展開狀態
const initializeExpandedItems = () => {
  expandedItems.value.clear()
  
  // 添加預設展開的項目
  props.defaultExpanded.forEach(key => {
    expandedItems.value.add(key)
  })
  
  // 如果沒有預設展開的項目，且允許多個展開，則展開第一個項目
  if (expandedItems.value.size === 0 && props.items.length > 0) {
    const firstItem = props.items.find(item => !item.disabled)
    if (firstItem) {
      expandedItems.value.add(firstItem.key)
    }
  }
}

// 方法
const isExpanded = (key: string | number): boolean => {
  return expandedItems.value.has(key)
}

const toggleItem = (key: string | number) => {
  const item = props.items.find(item => item.key === key)
  if (!item || item.disabled) return
  
  const isCurrentlyExpanded = isExpanded(key)
  
  if (props.multiple) {
    // 多個項目可同時展開
    if (isCurrentlyExpanded) {
      expandedItems.value.delete(key)
      emit('item-collapse', key)
    } else {
      expandedItems.value.add(key)
      emit('item-expand', key)
    }
  } else {
    // 單個項目展開（收合其他項目）
    expandedItems.value.clear()
    if (!isCurrentlyExpanded) {
      expandedItems.value.add(key)
      emit('item-expand', key)
    }
  }
  
  emit('item-toggle', key, !isCurrentlyExpanded)
}

const expandItem = (key: string | number) => {
  if (props.multiple) {
    expandedItems.value.add(key)
  } else {
    expandedItems.value.clear()
    expandedItems.value.add(key)
  }
  emit('item-expand', key)
  emit('item-toggle', key, true)
}

const collapseItem = (key: string | number) => {
  expandedItems.value.delete(key)
  emit('item-collapse', key)
  emit('item-toggle', key, false)
}

const expandAll = () => {
  if (props.multiple) {
    props.items.forEach(item => {
      if (!item.disabled) {
        expandedItems.value.add(item.key)
      }
    })
  }
}

const collapseAll = () => {
  expandedItems.value.clear()
}

// 監聽 items 變化，重新初始化展開狀態
watch(() => props.items, () => {
  initializeExpandedItems()
}, { immediate: true })

// 暴露方法給父組件
defineExpose({
  expandItem,
  collapseItem,
  expandAll,
  collapseAll,
  isExpanded
})
</script>

<style>
/* 手風琴組件基礎樣式 */
.gc-accordion {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-2);
  font-family: inherit;
}

/* 手風琴項目 */
.gc-accordion-item {
  border: 1px solid var(--gc-line);
  border-radius: var(--gc-radius-glass-sm);
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur-glass));
  overflow: hidden;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-accordion-item:hover {
  border-color: var(--gc-accent);
  box-shadow: var(--gc-shadow-glass);
}

.gc-accordion-item-expanded {
  border-color: var(--gc-accent);
  box-shadow: var(--gc-shadow-glass);
}

/* 手風琴標題 */
.gc-accordion-header {
  display: flex;
  align-items: center;
  gap: var(--gc-space-3);
  width: 100%;
  padding: var(--gc-space-4);
  border: none;
  background: transparent;
  color: var(--gc-ink);
  font-weight: var(--gc-font-bold);
  text-align: left;
  cursor: pointer;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  font-size: var(--gc-text-base);
}

.gc-accordion-header:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.05);
}

.gc-accordion-header:focus {
  outline: 2px solid var(--gc-accent);
  outline-offset: -2px;
}

.gc-accordion-header:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.gc-accordion-header:disabled:hover {
  background: transparent;
}

/* 標題圖標 */
.gc-accordion-icon {
  font-size: var(--gc-text-lg);
  line-height: 1;
  color: var(--gc-muted);
  flex-shrink: 0;
}

/* 標題文字 */
.gc-accordion-title {
  flex: 1;
  font-weight: inherit;
}

/* 標題徽章 */
.gc-accordion-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 20px;
  height: 20px;
  padding: 0 var(--gc-space-1);
  background: var(--gc-accent);
  color: white;
  border-radius: 999px;
  font-size: var(--gc-text-xs);
  font-weight: var(--gc-font-bold);
  line-height: 1;
  flex-shrink: 0;
}

/* 展開/收合箭頭 */
.gc-accordion-arrow {
  font-size: var(--gc-text-sm);
  color: var(--gc-muted);
  transition: transform var(--gc-transition-glass) var(--gc-ease-in-out);
  flex-shrink: 0;
}

.gc-accordion-item-expanded .gc-accordion-arrow {
  transform: rotate(90deg);
}

/* 手風琴內容 */
.gc-accordion-panel {
  max-height: 0;
  overflow: hidden;
  transition: max-height var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-accordion-panel-expanded {
  max-height: 1000px; /* 足夠大的值以容納內容 */
}

.gc-accordion-content {
  padding: 0 var(--gc-space-4) var(--gc-space-4);
  color: var(--gc-muted);
  line-height: 1.6;
}

/* 空狀態 */
.gc-accordion-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--gc-space-12);
  text-align: center;
  color: var(--gc-muted);
}

.gc-accordion-empty-icon {
  font-size: var(--gc-text-4xl);
  margin-bottom: var(--gc-space-4);
  opacity: 0.5;
}

.gc-accordion-empty-text {
  margin: 0;
  font-size: var(--gc-text-lg);
  font-weight: var(--gc-font-bold);
}

/* 手風琴樣式變體 */
.gc-accordion-minimal .gc-accordion-item {
  border: none;
  background: transparent;
  box-shadow: none;
}

.gc-accordion-minimal .gc-accordion-header {
  padding: var(--gc-space-3) 0;
  border-bottom: 1px solid var(--gc-line);
  border-radius: 0;
}

.gc-accordion-minimal .gc-accordion-content {
  padding: var(--gc-space-3) 0;
}

.gc-accordion-bordered .gc-accordion-item {
  border-width: 2px;
}

.gc-accordion-bordered .gc-accordion-item-expanded {
  border-width: 2px;
}

.gc-accordion-pills .gc-accordion-item {
  border-radius: var(--gc-radius-glass-lg);
  margin-bottom: var(--gc-space-3);
}

.gc-accordion-pills .gc-accordion-header {
  border-radius: var(--gc-radius-glass-lg);
}

.gc-accordion-pills .gc-accordion-content {
  padding: 0 var(--gc-space-4) var(--gc-space-4);
}

/* 緊湊模式 */
.gc-accordion-compact .gc-accordion-header {
  padding: var(--gc-space-3);
  font-size: var(--gc-text-sm);
}

.gc-accordion-compact .gc-accordion-content {
  padding: 0 var(--gc-space-3) var(--gc-space-3);
}

.gc-accordion-compact .gc-accordion-item {
  gap: 0;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .gc-accordion-header {
    padding: var(--gc-space-3);
    font-size: var(--gc-text-sm);
  }
  
  .gc-accordion-content {
    padding: 0 var(--gc-space-3) var(--gc-space-3);
  }
  
  .gc-accordion-icon {
    font-size: var(--gc-text-base);
  }
  
  .gc-accordion-arrow {
    font-size: var(--gc-text-xs);
  }
}

@media (max-width: 480px) {
  .gc-accordion-header {
    padding: var(--gc-space-2);
    font-size: var(--gc-text-xs);
    gap: var(--gc-space-2);
  }
  
  .gc-accordion-content {
    padding: 0 var(--gc-space-2) var(--gc-space-2);
  }
  
  .gc-accordion-badge {
    min-width: 16px;
    height: 16px;
    font-size: var(--gc-text-xs);
  }
  
  .gc-accordion-icon {
    font-size: var(--gc-text-sm);
  }
}

/* 深色模式 */
body.dark .gc-accordion-item {
  background: var(--gc-surface);
  border-color: var(--gc-line);
}

body.dark .gc-accordion-header:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.05);
}

body.dark .gc-accordion-content {
  color: var(--gc-muted);
}

/* 動畫優化 */
.gc-accordion-panel {
  will-change: max-height;
}

.gc-accordion-arrow {
  will-change: transform;
}

/* 無障礙性增強 */
.gc-accordion-header:focus-visible {
  outline: 2px solid var(--gc-accent);
  outline-offset: 2px;
}

/* 高對比度模式支援 */
@media (prefers-contrast: high) {
  .gc-accordion-item {
    border-width: 2px;
  }
  
  .gc-accordion-header {
    font-weight: var(--gc-font-black);
  }
  
  .gc-accordion-arrow {
    font-weight: var(--gc-font-bold);
  }
}

/* 減少動畫模式支援 */
@media (prefers-reduced-motion: reduce) {
  .gc-accordion-panel,
  .gc-accordion-arrow {
    transition: none;
  }
}
</style> 