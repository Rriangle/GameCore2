<template>
  <Teleport to="body">
    <Transition name="gc-modal">
      <div v-if="modelValue" class="gc-modal-overlay" @click="handleOverlayClick">
        <div class="gc-modal-container" :class="size" @click.stop>
          <!-- 模態框標題 -->
          <div v-if="title || $slots.header" class="gc-modal-header">
            <h3 v-if="title" class="gc-modal-title">{{ title }}</h3>
            <div v-if="$slots.header" class="gc-modal-header-content">
              <slot name="header" />
            </div>
            <button 
              v-if="showClose" 
              class="gc-modal-close" 
              @click="handleClose"
              aria-label="關閉"
            >
              <svg class="gc-modal-close-icon" viewBox="0 0 24 24" fill="none">
                <path d="M18 6L6 18M6 6l12 12" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </button>
          </div>

          <!-- 模態框內容 -->
          <div class="gc-modal-body">
            <slot />
          </div>

          <!-- 模態框操作按鈕 -->
          <div v-if="$slots.footer || showDefaultFooter" class="gc-modal-footer">
            <slot name="footer">
              <div v-if="showDefaultFooter" class="gc-modal-default-footer">
                <button 
                  v-if="cancelText" 
                  class="gc-btn gc-btn-secondary" 
                  @click="handleCancel"
                >
                  {{ cancelText }}
                </button>
                <button 
                  v-if="confirmText" 
                  class="gc-btn gc-btn-primary" 
                  @click="handleConfirm"
                  :disabled="loading"
                >
                  <span v-if="loading" class="gc-spinner">
                    <svg class="gc-spinner-icon" viewBox="0 0 24 24" fill="none">
                      <circle
                        class="gc-spinner-circle"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        stroke-width="4"
                      />
                    </svg>
                  </span>
                  {{ confirmText }}
                </button>
              </div>
            </slot>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, watch } from 'vue'

// 組件屬性
interface Props {
  // 控制顯示/隱藏
  modelValue: boolean
  // 模態框標題
  title?: string
  // 模態框尺寸
  size?: 'sm' | 'md' | 'lg' | 'xl' | 'full'
  // 是否顯示關閉按鈕
  showClose?: boolean
  // 是否顯示預設操作按鈕
  showDefaultFooter?: boolean
  // 取消按鈕文字
  cancelText?: string
  // 確認按鈕文字
  confirmText?: string
  // 是否可點擊遮罩關閉
  closeOnOverlay?: boolean
  // 是否可按 ESC 關閉
  closeOnEsc?: boolean
  // 載入狀態
  loading?: boolean
  // 是否阻止背景滾動
  preventScroll?: boolean
}

// 組件事件
interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'close'): void
  (e: 'cancel'): void
  (e: 'confirm'): void
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  showClose: true,
  showDefaultFooter: false,
  cancelText: '取消',
  confirmText: '確認',
  closeOnOverlay: true,
  closeOnEsc: true,
  preventScroll: true
})

const emit = defineEmits<Emits>()

// 計算屬性
const modalSize = computed(() => `gc-modal-${props.size}`)

// 方法
const handleClose = () => {
  emit('update:modelValue', false)
  emit('close')
}

const handleCancel = () => {
  emit('cancel')
  handleClose()
}

const handleConfirm = () => {
  emit('confirm')
}

const handleOverlayClick = () => {
  if (props.closeOnOverlay) {
    handleClose()
  }
}

const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && props.closeOnEsc) {
    handleClose()
  }
}

const preventBodyScroll = () => {
  if (props.preventScroll) {
    document.body.style.overflow = 'hidden'
  }
}

const restoreBodyScroll = () => {
  if (props.preventScroll) {
    document.body.style.overflow = ''
  }
}

// 生命週期
onMounted(() => {
  if (props.modelValue) {
    preventBodyScroll()
  }
  
  if (props.closeOnEsc) {
    document.addEventListener('keydown', handleKeydown)
  }
})

onUnmounted(() => {
  restoreBodyScroll()
  document.removeEventListener('keydown', handleKeydown)
})

// 監聽 modelValue 變化
watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    preventBodyScroll()
  } else {
    restoreBodyScroll()
  }
})
</script>

<style scoped>
/* 玻璃風模態框組件樣式 */

/* 遮罩層 */
.gc-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: var(--gc-z-modal);
  padding: var(--gc-space-4);
}

/* 模態框容器 */
.gc-modal-container {
  @apply gc-card;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
  border: 1px solid var(--gc-line);
  max-width: 90vw;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative;
}

/* 模態框尺寸 */
.gc-modal-sm {
  width: 400px;
}

.gc-modal-md {
  width: 600px;
}

.gc-modal-lg {
  width: 800px;
}

.gc-modal-xl {
  width: 1000px;
}

.gc-modal-full {
  width: 95vw;
  height: 95vh;
}

/* 模態框標題 */
.gc-modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--gc-space-6);
  border-bottom: 1px solid var(--gc-line);
  background: var(--gc-surface-2);
}

.gc-modal-title {
  @apply text-xl font-bold;
  color: var(--gc-ink);
  margin: 0;
}

.gc-modal-header-content {
  flex: 1;
  margin: 0 var(--gc-space-4);
}

.gc-modal-close {
  @apply w-8 h-8 rounded-lg flex items-center justify-center transition-all duration-200;
  background: var(--gc-surface);
  border: 1px solid var(--gc-line);
  color: var(--gc-muted);
  cursor: pointer;
  flex-shrink: 0;
}

.gc-modal-close:hover {
  background: var(--gc-accent);
  color: white;
  border-color: var(--gc-accent);
}

.gc-modal-close-icon {
  @apply w-4 h-4;
}

/* 模態框內容 */
.gc-modal-body {
  flex: 1;
  padding: var(--gc-space-6);
  overflow-y: auto;
  color: var(--gc-ink);
}

/* 模態框操作按鈕 */
.gc-modal-footer {
  padding: var(--gc-space-6);
  border-top: 1px solid var(--gc-line);
  background: var(--gc-surface-2);
}

.gc-modal-default-footer {
  display: flex;
  gap: var(--gc-space-3);
  justify-content: flex-end;
}

/* 載入狀態 */
.gc-spinner {
  @apply inline-flex items-center mr-2;
}

.gc-spinner-icon {
  @apply w-4 h-4 animate-spin;
}

.gc-spinner-circle {
  stroke-dasharray: 80;
  stroke-dashoffset: 60;
  animation: gc-spin 1.2s linear infinite;
}

@keyframes gc-spin {
  0% { stroke-dashoffset: 80; }
  100% { stroke-dashoffset: -80; }
}

/* 動畫效果 */
.gc-modal-enter-active,
.gc-modal-leave-active {
  transition: all 0.3s var(--gc-ease-out);
}

.gc-modal-enter-from {
  opacity: 0;
  transform: scale(0.9) translateY(-20px);
}

.gc-modal-leave-to {
  opacity: 0;
  transform: scale(0.9) translateY(20px);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .gc-modal-overlay {
    padding: var(--gc-space-2);
  }
  
  .gc-modal-container {
    max-width: 95vw;
    max-height: 95vh;
  }
  
  .gc-modal-sm,
  .gc-modal-md,
  .gc-modal-lg,
  .gc-modal-xl {
    width: 95vw;
  }
  
  .gc-modal-header {
    @apply p-4;
  }
  
  .gc-modal-body {
    @apply p-4;
  }
  
  .gc-modal-footer {
    @apply p-4;
  }
  
  .gc-modal-default-footer {
    flex-direction: column;
  }
  
  .gc-modal-default-footer .gc-btn {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .gc-modal-overlay {
    padding: var(--gc-space-1);
  }
  
  .gc-modal-container {
    max-width: 98vw;
    max-height: 98vh;
  }
  
  .gc-modal-header {
    @apply p-3;
  }
  
  .gc-modal-body {
    @apply p-3;
  }
  
  .gc-modal-footer {
    @apply p-3;
  }
}
</style> 