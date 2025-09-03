<template>
  <Teleport to="body">
    <div class="toast-container" :class="positionClass">
      <TransitionGroup 
        name="toast" 
        tag="div" 
        class="toast-list"
        appear
      >
        <div
          v-for="toast in toasts"
          :key="toast.id"
          :class="['toast', `toast-${toast.type}`, { 'toast-visible': toast.visible }]"
          role="alert"
          :aria-live="toast.type === 'error' ? 'assertive' : 'polite'"
        >
          <!-- Toast 圖標 -->
          <div class="toast-icon">
            <svg v-if="toast.type === 'success'" class="icon" viewBox="0 0 24 24" fill="none">
              <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <svg v-else-if="toast.type === 'error'" class="icon" viewBox="0 0 24 24" fill="none">
              <path d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <svg v-else-if="toast.type === 'warning'" class="icon" viewBox="0 0 24 24" fill="none">
              <path d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <svg v-else class="icon" viewBox="0 0 24 24" fill="none">
              <path d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>

          <!-- Toast 內容 -->
          <div class="toast-content">
            <div v-if="toast.title" class="toast-title">{{ toast.title }}</div>
            <div class="toast-message">{{ toast.message }}</div>
          </div>

          <!-- 關閉按鈕 -->
          <button
            v-if="toast.closable"
            class="toast-close"
            @click="hideToast(toast.id)"
            aria-label="關閉通知"
          >
            <svg class="close-icon" viewBox="0 0 24 24" fill="none">
              <path d="M6 18L18 6M6 6l12 12" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </button>

          <!-- 進度條 -->
          <div v-if="toast.showProgress && toast.duration > 0" class="toast-progress">
            <div class="progress-bar">
              <div 
                class="progress-fill" 
                :style="{ width: `${toast.progress}%` }"
              ></div>
            </div>
          </div>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useToast } from './useToast'

// 使用 Toast 服務
const { toasts, hideToast } = useToast()

// 計算位置樣式
const positionClass = computed(() => {
  const position = toasts.value[0]?.position || 'top-right'
  return `toast-${position.replace('-', '-')}`
})
</script>

<style scoped>
.toast-container {
  position: fixed;
  z-index: 9999;
  pointer-events: none;
}

/* 位置樣式 */
.toast-top-right {
  top: 1rem;
  right: 1rem;
}

.toast-top-left {
  top: 1rem;
  left: 1rem;
}

.toast-bottom-right {
  bottom: 1rem;
  right: 1rem;
}

.toast-bottom-left {
  bottom: 1rem;
  left: 1rem;
}

.toast-top-center {
  top: 1rem;
  left: 50%;
  transform: translateX(-50%);
}

.toast-bottom-center {
  bottom: 1rem;
  left: 50%;
  transform: translateX(-50%);
}

.toast-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  max-width: 400px;
}

/* Toast 樣式 */
.toast {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1rem;
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(14px);
  pointer-events: auto;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.toast:hover {
  transform: translateY(-2px);
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.15);
}

/* Toast 類型樣式 */
.toast-success {
  border-left: 4px solid #22c55e;
}

.toast-error {
  border-left: 4px solid #ef4444;
}

.toast-warning {
  border-left: 4px solid #f59e0b;
}

.toast-info {
  border-left: 4px solid #3b82f6;
}

/* Toast 圖標 */
.toast-icon {
  flex-shrink: 0;
  width: 1.5rem;
  height: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

.toast-success .icon {
  color: #22c55e;
}

.toast-error .icon {
  color: #ef4444;
}

.toast-warning .icon {
  color: #f59e0b;
}

.toast-info .icon {
  color: #3b82f6;
}

.icon {
  width: 1.25rem;
  height: 1.25rem;
}

/* Toast 內容 */
.toast-content {
  flex: 1;
  min-width: 0;
}

.toast-title {
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.25rem;
  font-size: 0.875rem;
}

.toast-message {
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.4;
}

/* 關閉按鈕 */
.toast-close {
  flex-shrink: 0;
  width: 1.5rem;
  height: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  color: #9ca3af;
  transition: all 0.2s ease;
}

.toast-close:hover {
  background: #f3f4f6;
  color: #6b7280;
}

.close-icon {
  width: 1rem;
  height: 1rem;
}

/* 進度條 */
.toast-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: rgba(0, 0, 0, 0.1);
}

.progress-bar {
  height: 100%;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #7557ff, #34d2ff);
  transition: width 0.1s linear;
}

/* 動畫效果 */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}

.toast-move {
  transition: transform 0.3s ease;
}

/* 深色模式 */
@media (prefers-color-scheme: dark) {
  .toast {
    background: rgba(17, 24, 39, 0.95);
    border-color: #374151;
    color: #f9fafb;
  }
  
  .toast-title {
    color: #f9fafb;
  }
  
  .toast-message {
    color: #d1d5db;
  }
  
  .toast-close:hover {
    background: #374151;
    color: #d1d5db;
  }
}

/* 響應式設計 */
@media (max-width: 640px) {
  .toast-container {
    left: 1rem;
    right: 1rem;
    transform: none;
  }
  
  .toast-top-center,
  .toast-bottom-center {
    left: 1rem;
    right: 1rem;
    transform: none;
  }
  
  .toast-list {
    max-width: none;
  }
  
  .toast {
    padding: 0.75rem;
  }
  
  .toast-title {
    font-size: 0.8rem;
  }
  
  .toast-message {
    font-size: 0.8rem;
  }
}
</style> 