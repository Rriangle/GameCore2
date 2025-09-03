<template>
  <Teleport to="body">
    <TransitionGroup 
      name="gc-toast" 
      tag="div" 
      class="gc-toast-container"
    >
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="[
          'gc-toast',
          `gc-toast-${toast.type}`,
          { 'gc-toast-closable': toast.closable }
        ]"
        role="alert"
        :aria-live="toast.type === 'error' ? 'assertive' : 'polite'"
      >
        <!-- Toast 圖標 -->
        <div class="gc-toast-icon">
          <component :is="getToastIcon(toast.type)" />
        </div>

        <!-- Toast 內容 -->
        <div class="gc-toast-content">
          <div v-if="toast.title" class="gc-toast-title">
            {{ toast.title }}
          </div>
          <div class="gc-toast-message">
            {{ toast.message }}
          </div>
        </div>

        <!-- 關閉按鈕 -->
        <button
          v-if="toast.closable"
          class="gc-toast-close"
          @click="removeToast(toast.id)"
          aria-label="關閉通知"
        >
          <svg class="gc-toast-close-icon" viewBox="0 0 24 24" fill="none">
            <path d="M18 6L6 18M6 6l12 12" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </button>

        <!-- 進度條 -->
        <div 
          v-if="toast.duration && toast.duration > 0"
          class="gc-toast-progress"
          :style="{ animationDuration: `${toast.duration}ms` }"
        />
      </div>
    </TransitionGroup>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

// Toast 類型定義
export interface Toast {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title?: string
  message: string
  duration?: number
  closable?: boolean
  onClose?: () => void
}

// Toast 配置選項
export interface ToastOptions {
  type?: 'success' | 'error' | 'warning' | 'info'
  title?: string
  duration?: number
  closable?: boolean
  onClose?: () => void
}

// 組件屬性
interface Props {
  // 最大顯示數量
  maxToasts?: number
  // 預設持續時間
  defaultDuration?: number
  // 預設位置
  position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left' | 'top-center' | 'bottom-center'
}

const props = withDefaults(defineProps<Props>(), {
  maxToasts: 5,
  defaultDuration: 5000,
  position: 'top-right'
})

// 內部狀態
const toasts = ref<Toast[]>([])
let toastCounter = 0

// Toast 圖標組件
const getToastIcon = (type: string) => {
  const icons = {
    success: () => (
      <svg class="gc-toast-icon-svg" viewBox="0 0 24 24" fill="none">
        <path d="M20 6L9 17l-5-5" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    ),
    error: () => (
      <svg class="gc-toast-icon-svg" viewBox="0 0 24 24" fill="none">
        <path d="M18 6L6 18M6 6l12 12" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    ),
    warning: () => (
      <svg class="gc-toast-icon-svg" viewBox="0 0 24 24" fill="none">
        <path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M12 9v4M12 17h.01" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    ),
    info: () => (
      <svg class="gc-toast-icon-svg" viewBox="0 0 24 24" fill="none">
        <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
        <path d="M12 16v-4M12 8h.01" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      </svg>
    )
  }
  
  return icons[type as keyof typeof icons] || icons.info
}

// 方法
const generateId = (): string => {
  return `toast-${++toastCounter}-${Date.now()}`
}

const addToast = (message: string, options: ToastOptions = {}): string => {
  const toast: Toast = {
    id: generateId(),
    type: options.type || 'info',
    title: options.title,
    message,
    duration: options.duration ?? props.defaultDuration,
    closable: options.closable ?? true,
    onClose: options.onClose
  }

  // 限制最大顯示數量
  if (toasts.value.length >= props.maxToasts) {
    removeToast(toasts.value[0].id)
  }

  toasts.value.push(toast)

  // 自動移除
  if (toast.duration && toast.duration > 0) {
    setTimeout(() => {
      removeToast(toast.id)
    }, toast.duration)
  }

  return toast.id
}

const removeToast = (id: string) => {
  const index = toasts.value.findIndex(toast => toast.id === id)
  if (index > -1) {
    const toast = toasts.value[index]
    toasts.value.splice(index, 1)
    
    // 執行關閉回調
    if (toast.onClose) {
      toast.onClose()
    }
  }
}

const clearAll = () => {
  toasts.value.forEach(toast => {
    if (toast.onClose) {
      toast.onClose()
    }
  })
  toasts.value = []
}

// 便捷方法
const success = (message: string, options?: ToastOptions) => addToast(message, { ...options, type: 'success' })
const error = (message: string, options?: ToastOptions) => addToast(message, { ...options, type: 'error' })
const warning = (message: string, options?: ToastOptions) => addToast(message, { ...options, type: 'warning' })
const info = (message: string, options?: ToastOptions) => addToast(message, { ...options, type: 'info' })

// 暴露方法給父組件
defineExpose({
  addToast,
  removeToast,
  clearAll,
  success,
  error,
  warning,
  info
})

// 生命週期
onMounted(() => {
  // 可以添加全局事件監聽等
})

onUnmounted(() => {
  clearAll()
})
</script>

<style scoped>
/* 玻璃風 Toast 組件樣式 */

/* Toast 容器 */
.gc-toast-container {
  position: fixed;
  top: var(--gc-space-4);
  right: var(--gc-space-4);
  z-index: var(--gc-z-toast);
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-3);
  max-width: 400px;
  pointer-events: none;
}

/* 單個 Toast */
.gc-toast {
  @apply gc-card p-4 flex items-start gap-3;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
  border: 1px solid var(--gc-line);
  pointer-events: auto;
  position: relative;
  overflow: hidden;
  min-width: 300px;
}

/* Toast 類型樣式 */
.gc-toast-success {
  border-left: 4px solid #10b981;
}

.gc-toast-error {
  border-left: 4px solid #ef4444;
}

.gc-toast-warning {
  border-left: 4px solid #f59e0b;
}

.gc-toast-info {
  border-left: 4px solid var(--gc-accent);
}

/* Toast 圖標 */
.gc-toast-icon {
  @apply flex-shrink-0 w-5 h-5;
  color: var(--gc-muted);
}

.gc-toast-success .gc-toast-icon {
  color: #10b981;
}

.gc-toast-error .gc-toast-icon {
  color: #ef4444;
}

.gc-toast-warning .gc-toast-icon {
  color: #f59e0b;
}

.gc-toast-info .gc-toast-icon {
  color: var(--gc-accent);
}

.gc-toast-icon-svg {
  @apply w-5 h-5;
}

/* Toast 內容 */
.gc-toast-content {
  flex: 1;
  min-width: 0;
}

.gc-toast-title {
  @apply font-bold mb-1;
  color: var(--gc-ink);
}

.gc-toast-message {
  @apply text-sm;
  color: var(--gc-muted);
  line-height: 1.4;
}

/* 關閉按鈕 */
.gc-toast-close {
  @apply w-5 h-5 rounded flex items-center justify-center transition-all duration-200;
  background: transparent;
  border: none;
  color: var(--gc-muted);
  cursor: pointer;
  flex-shrink: 0;
}

.gc-toast-close:hover {
  background: var(--gc-surface-2);
  color: var(--gc-ink);
}

.gc-toast-close-icon {
  @apply w-4 h-4;
}

/* 進度條 */
.gc-toast-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  height: 2px;
  background: var(--gc-accent);
  animation: gc-toast-progress linear forwards;
}

@keyframes gc-toast-progress {
  from { width: 100%; }
  to { width: 0%; }
}

/* 動畫效果 */
.gc-toast-enter-active {
  transition: all 0.3s var(--gc-ease-out);
}

.gc-toast-leave-active {
  transition: all 0.3s var(--gc-ease-in);
}

.gc-toast-enter-from {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

.gc-toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

.gc-toast-move {
  transition: transform 0.3s var(--gc-ease-out);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .gc-toast-container {
    top: var(--gc-space-2);
    right: var(--gc-space-2);
    left: var(--gc-space-2);
    max-width: none;
  }
  
  .gc-toast {
    min-width: auto;
    width: 100%;
  }
}

@media (max-width: 480px) {
  .gc-toast-container {
    top: var(--gc-space-1);
    right: var(--gc-space-1);
    left: var(--gc-space-1);
  }
  
  .gc-toast {
    @apply p-3;
  }
  
  .gc-toast-icon {
    @apply w-4 h-4;
  }
  
  .gc-toast-close {
    @apply w-4 h-4;
  }
}
</style> 