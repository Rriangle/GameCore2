<template>
  <div class="gc-tabs" :class="[`gc-tabs-${variant}`, { 'gc-tabs-vertical': vertical }]">
    <!-- 標籤導航 -->
    <div 
      class="gc-tabs-nav" 
      role="tablist"
      :aria-label="ariaLabel"
      :aria-orientation="vertical ? 'vertical' : 'horizontal'"
    >
      <button
        v-for="(tab, index) in tabs"
        :key="tab.key || index"
        :id="`tab-${tab.key || index}`"
        :aria-controls="`panel-${tab.key || index}`"
        :aria-selected="activeTab === (tab.key || index)"
        :aria-disabled="tab.disabled"
        :disabled="tab.disabled"
        :class="[
          'gc-tabs-tab',
          {
            'gc-tabs-tab-active': activeTab === (tab.key || index),
            'gc-tabs-tab-disabled': tab.disabled
          }
        ]"
        @click="selectTab(tab.key || index)"
        role="tab"
        type="button"
      >
        <!-- 標籤圖標 -->
        <span v-if="tab.icon" class="gc-tabs-icon" :aria-hidden="true">
          {{ tab.icon }}
        </span>
        
        <!-- 標籤文字 -->
        <span class="gc-tabs-text">{{ tab.label }}</span>
        
        <!-- 標籤徽章 -->
        <span v-if="tab.badge" class="gc-tabs-badge">
          {{ tab.badge }}
        </span>
        
        <!-- 標籤關閉按鈕 -->
        <button
          v-if="closable && tab.closable !== false"
          class="gc-tabs-close"
          @click.stop="closeTab(tab.key || index)"
          :aria-label="`關閉標籤 ${tab.label}`"
          type="button"
        >
          ×
        </button>
      </button>
      
      <!-- 新增標籤按鈕 -->
      <button
        v-if="addable"
        class="gc-tabs-add"
        @click="addTab"
        :aria-label="addButtonLabel"
        type="button"
      >
        <span class="gc-tabs-add-icon">+</span>
        <span v-if="!compact" class="gc-tabs-add-text">{{ addButtonLabel }}</span>
      </button>
    </div>
    
    <!-- 標籤內容 -->
    <div 
      v-for="(tab, index) in tabs"
      :key="tab.key || index"
      :id="`panel-${tab.key || index}`"
      :class="[
        'gc-tabs-panel',
        {
          'gc-tabs-panel-active': activeTab === (tab.key || index)
        }
      ]"
      role="tabpanel"
      :aria-labelledby="`tab-${tab.key || index}`"
      :hidden="activeTab !== (tab.key || index)"
    >
      <div class="gc-tabs-content">
        <slot :name="tab.key || index" :tab="tab" :index="index">
          {{ tab.content || tab.label }}
        </slot>
      </div>
    </div>
    
    <!-- 空狀態 -->
    <div v-if="tabs.length === 0" class="gc-tabs-empty">
      <div class="gc-tabs-empty-icon">📋</div>
      <p class="gc-tabs-empty-text">暫無標籤頁</p>
      <button v-if="addable" @click="addTab" class="gc-btn gc-btn-primary">
        新增標籤頁
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'

// 標籤頁介面
interface Tab {
  key: string | number
  label: string
  content?: string
  icon?: string
  badge?: string | number
  disabled?: boolean
  closable?: boolean
}

// 組件屬性
interface Props {
  // 標籤頁數據
  modelValue?: string | number
  // 標籤頁列表
  tabs: Tab[]
  // 標籤頁樣式變體
  variant?: 'default' | 'minimal' | 'bordered' | 'pills'
  // 是否為垂直布局
  vertical?: boolean
  // 是否為緊湊模式
  compact?: boolean
  // 是否可關閉標籤頁
  closable?: boolean
  // 是否可新增標籤頁
  addable?: boolean
  // 新增按鈕標籤
  addButtonLabel?: string
  // 無障礙標籤
  ariaLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  vertical: false,
  compact: false,
  closable: false,
  addable: false,
  addButtonLabel: '新增標籤頁',
  ariaLabel: '標籤頁導航'
})

// 組件事件
const emit = defineEmits<{
  'update:modelValue': [value: string | number]
  'tab-change': [value: string | number]
  'tab-close': [value: string | number]
  'tab-add': []
}>()

// 響應式狀態
const activeTab = ref<string | number>('')

// 計算屬性
const currentTab = computed(() => {
  return props.tabs.find(tab => tab.key === activeTab.value)
})

// 方法
const selectTab = (key: string | number) => {
  const tab = props.tabs.find(t => t.key === key)
  if (tab && !tab.disabled) {
    activeTab.value = key
    emit('update:modelValue', key)
    emit('tab-change', key)
  }
}

const closeTab = (key: string | number) => {
  emit('tab-close', key)
}

const addTab = () => {
  emit('tab-add')
}

// 監聽 modelValue 變化
watch(() => props.modelValue, (newValue) => {
  if (newValue && newValue !== activeTab.value) {
    activeTab.value = newValue
  }
}, { immediate: true })

// 監聽 tabs 變化，自動選擇第一個可用標籤頁
watch(() => props.tabs, (newTabs) => {
  if (newTabs.length > 0 && !activeTab.value) {
    const firstEnabledTab = newTabs.find(tab => !tab.disabled)
    if (firstEnabledTab) {
      activeTab.value = firstEnabledTab.key
      emit('update:modelValue', firstEnabledTab.key)
    }
  }
}, { immediate: true })
</script>

<style>
/* 標籤頁組件基礎樣式 */
.gc-tabs {
  display: flex;
  flex-direction: column;
  font-family: inherit;
}

.gc-tabs-vertical {
  flex-direction: row;
  gap: var(--gc-space-4);
}

/* 標籤導航 */
.gc-tabs-nav {
  display: flex;
  gap: var(--gc-space-1);
  border-bottom: 1px solid var(--gc-line);
  overflow-x: auto;
  scrollbar-width: none;
  -ms-overflow-style: none;
}

.gc-tabs-nav::-webkit-scrollbar {
  display: none;
}

.gc-tabs-vertical .gc-tabs-nav {
  flex-direction: column;
  border-bottom: none;
  border-right: 1px solid var(--gc-line);
  min-width: 200px;
  overflow-x: visible;
  overflow-y: auto;
}

/* 標籤按鈕 */
.gc-tabs-tab {
  display: inline-flex;
  align-items: center;
  gap: var(--gc-space-2);
  padding: var(--gc-space-3) var(--gc-space-4);
  border: none;
  background: transparent;
  color: var(--gc-muted);
  font-weight: var(--gc-font-bold);
  text-decoration: none;
  cursor: pointer;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  font-size: var(--gc-text-sm);
  white-space: nowrap;
  position: relative;
  border-radius: var(--gc-radius-glass-sm) var(--gc-radius-glass-sm) 0 0;
}

.gc-tabs-vertical .gc-tabs-tab {
  border-radius: var(--gc-radius-glass-sm) 0 0 var(--gc-radius-glass-sm);
  justify-content: flex-start;
  text-align: left;
}

.gc-tabs-tab:hover:not(.gc-tabs-tab-disabled) {
  color: var(--gc-ink);
  background: rgba(255, 255, 255, 0.05);
}

.gc-tabs-tab:focus {
  outline: 2px solid var(--gc-accent);
  outline-offset: -2px;
}

.gc-tabs-tab-active {
  color: var(--gc-accent);
  background: var(--gc-surface);
  border-bottom: 2px solid var(--gc-accent);
}

.gc-tabs-vertical .gc-tabs-tab-active {
  border-bottom: none;
  border-right: 2px solid var(--gc-accent);
}

.gc-tabs-tab-disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.gc-tabs-tab-disabled:hover {
  color: var(--gc-muted);
  background: transparent;
}

/* 標籤圖標 */
.gc-tabs-icon {
  font-size: var(--gc-text-lg);
  line-height: 1;
}

/* 標籤文字 */
.gc-tabs-text {
  font-weight: inherit;
}

/* 標籤徽章 */
.gc-tabs-badge {
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
}

/* 標籤關閉按鈕 */
.gc-tabs-close {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  border: none;
  background: transparent;
  color: var(--gc-muted);
  cursor: pointer;
  border-radius: 50%;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  font-size: var(--gc-text-lg);
  line-height: 1;
  margin-left: var(--gc-space-1);
}

.gc-tabs-close:hover {
  background: rgba(0, 0, 0, 0.1);
  color: var(--gc-ink);
}

/* 新增標籤按鈕 */
.gc-tabs-add {
  display: inline-flex;
  align-items: center;
  gap: var(--gc-space-2);
  padding: var(--gc-space-3) var(--gc-space-4);
  border: 1px dashed var(--gc-line);
  background: transparent;
  color: var(--gc-muted);
  font-weight: var(--gc-font-bold);
  cursor: pointer;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  font-size: var(--gc-text-sm);
  white-space: nowrap;
  border-radius: var(--gc-radius-glass-sm) var(--gc-radius-glass-sm) 0 0;
}

.gc-tabs-vertical .gc-tabs-add {
  border-radius: var(--gc-radius-glass-sm) 0 0 var(--gc-radius-glass-sm);
  border-right: 1px dashed var(--gc-line);
  border-bottom: none;
}

.gc-tabs-add:hover {
  color: var(--gc-accent);
  border-color: var(--gc-accent);
  background: rgba(117, 87, 255, 0.05);
}

.gc-tabs-add-icon {
  font-size: var(--gc-text-lg);
  font-weight: var(--gc-font-black);
  line-height: 1;
}

.gc-tabs-add-text {
  font-weight: inherit;
}

/* 標籤內容 */
.gc-tabs-panel {
  display: none;
  padding: var(--gc-space-6);
  background: var(--gc-surface);
  border-radius: 0 var(--gc-radius-glass-sm) var(--gc-radius-glass-sm) var(--gc-radius-glass-sm);
  border: 1px solid var(--gc-line);
  border-top: none;
}

.gc-tabs-vertical .gc-tabs-panel {
  border-radius: 0 var(--gc-radius-glass-sm) var(--gc-radius-glass-sm) 0;
  border-left: none;
  border-top: 1px solid var(--gc-line);
  flex: 1;
}

.gc-tabs-panel-active {
  display: block;
}

.gc-tabs-content {
  min-height: 100px;
}

/* 空狀態 */
.gc-tabs-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--gc-space-12);
  text-align: center;
  color: var(--gc-muted);
}

.gc-tabs-empty-icon {
  font-size: var(--gc-text-4xl);
  margin-bottom: var(--gc-space-4);
  opacity: 0.5;
}

.gc-tabs-empty-text {
  margin: 0 0 var(--gc-space-4) 0;
  font-size: var(--gc-text-lg);
  font-weight: var(--gc-font-bold);
}

/* 標籤頁樣式變體 */
.gc-tabs-minimal .gc-tabs-nav {
  border-bottom: none;
}

.gc-tabs-minimal .gc-tabs-tab {
  border-bottom: 2px solid transparent;
}

.gc-tabs-minimal .gc-tabs-tab-active {
  border-bottom-color: var(--gc-accent);
  background: transparent;
}

.gc-tabs-minimal .gc-tabs-panel {
  border: none;
  background: transparent;
  padding: var(--gc-space-4) 0;
}

.gc-tabs-bordered .gc-tabs-tab {
  border: 1px solid var(--gc-line);
  border-bottom: none;
  margin-right: -1px;
}

.gc-tabs-bordered .gc-tabs-tab-active {
  background: var(--gc-surface);
  border-color: var(--gc-accent);
  z-index: 1;
}

.gc-tabs-bordered .gc-tabs-panel {
  border: 1px solid var(--gc-accent);
  border-top: none;
}

.gc-tabs-pills .gc-tabs-nav {
  border-bottom: none;
  gap: var(--gc-space-2);
}

.gc-tabs-pills .gc-tabs-tab {
  border-radius: 999px;
  border: 1px solid var(--gc-line);
  background: var(--gc-surface);
}

.gc-tabs-pills .gc-tabs-tab-active {
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-primary-500));
  color: white;
  border-color: transparent;
}

.gc-tabs-pills .gc-tabs-panel {
  border: none;
  background: transparent;
  padding: var(--gc-space-4) 0;
}

/* 緊湊模式 */
.gc-tabs-compact .gc-tabs-tab {
  padding: var(--gc-space-2) var(--gc-space-3);
  font-size: var(--gc-text-xs);
}

.gc-tabs-compact .gc-tabs-panel {
  padding: var(--gc-space-4);
}

.gc-tabs-compact .gc-tabs-add {
  padding: var(--gc-space-2) var(--gc-space-3);
  font-size: var(--gc-text-xs);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .gc-tabs-nav {
    gap: 0;
  }
  
  .gc-tabs-tab {
    padding: var(--gc-space-2) var(--gc-space-3);
    font-size: var(--gc-text-xs);
  }
  
  .gc-tabs-panel {
    padding: var(--gc-space-4);
  }
  
  .gc-tabs-vertical {
    flex-direction: column;
    gap: var(--gc-space-3);
  }
  
  .gc-tabs-vertical .gc-tabs-nav {
    flex-direction: row;
    border-right: none;
    border-bottom: 1px solid var(--gc-line);
    min-width: auto;
    overflow-x: auto;
  }
  
  .gc-tabs-vertical .gc-tabs-tab {
    border-radius: var(--gc-radius-glass-sm) var(--gc-radius-glass-sm) 0 0;
    border-right: none;
    border-bottom: 2px solid transparent;
    justify-content: center;
    text-align: center;
  }
  
  .gc-tabs-vertical .gc-tabs-tab-active {
    border-right: none;
    border-bottom-color: var(--gc-accent);
  }
  
  .gc-tabs-vertical .gc-tabs-panel {
    border-radius: 0 0 var(--gc-radius-glass-sm) var(--gc-radius-glass-sm);
    border-left: 1px solid var(--gc-line);
    border-top: none;
  }
  
  .gc-tabs-vertical .gc-tabs-add {
    border-radius: var(--gc-radius-glass-sm) var(--gc-radius-glass-sm) 0 0;
    border-right: none;
    border-bottom: 1px dashed var(--gc-line);
  }
}

@media (max-width: 480px) {
  .gc-tabs-tab {
    padding: var(--gc-space-1) var(--gc-space-2);
    font-size: var(--gc-text-xs);
  }
  
  .gc-tabs-panel {
    padding: var(--gc-space-3);
  }
  
  .gc-tabs-add {
    padding: var(--gc-space-1) var(--gc-space-2);
    font-size: var(--gc-text-xs);
  }
  
  .gc-tabs-add-text {
    display: none;
  }
}

/* 深色模式 */
body.dark .gc-tabs-tab:hover:not(.gc-tabs-tab-disabled) {
  background: rgba(255, 255, 255, 0.05);
}

body.dark .gc-tabs-tab-active {
  background: var(--gc-surface);
}

body.dark .gc-tabs-panel {
  background: var(--gc-surface);
  border-color: var(--gc-line);
}

body.dark .gc-tabs-close:hover {
  background: rgba(255, 255, 255, 0.1);
}

body.dark .gc-tabs-add:hover {
  background: rgba(117, 87, 255, 0.1);
}
</style> 