<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    @click="handleClick"
    v-bind="$attrs"
  >
    <!-- 載入狀態 -->
    <svg
      v-if="loading"
      class="gc-spinner"
      viewBox="0 0 24 24"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <circle
        class="gc-spinner-circle"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        stroke-width="4"
      />
    </svg>
    
    <!-- 圖標 -->
    <component
      v-else-if="icon"
      :is="icon"
      class="gc-icon"
      :class="{ 'gc-icon-left': iconPosition === 'left', 'gc-icon-right': iconPosition === 'right' }"
    />
    
    <!-- 內容 -->
    <span v-if="$slots.default" class="gc-content">
      <slot />
    </span>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

// 組件屬性定義
interface Props {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger'
  size?: 'sm' | 'md' | 'lg'
  disabled?: boolean
  loading?: boolean
  icon?: any
  iconPosition?: 'left' | 'right'
  fullWidth?: boolean
}

// 組件事件定義
interface Emits {
  (e: 'click', event: MouseEvent): void
}

// 屬性預設值
const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  disabled: false,
  loading: false,
  iconPosition: 'left',
  fullWidth: false
})

// 事件發射
const emit = defineEmits<Emits>()

// 按鈕樣式類別計算
const buttonClasses = computed(() => {
  const baseClasses = ['gc-btn']
  
  // 變體樣式
  switch (props.variant) {
    case 'primary':
      baseClasses.push('gc-btn-primary')
      break
    case 'secondary':
      baseClasses.push('gc-btn-secondary')
      break
    case 'ghost':
      baseClasses.push('gc-btn-ghost')
      break
    case 'danger':
      baseClasses.push('gc-btn-danger')
      break
  }
  
  // 尺寸樣式
  switch (props.size) {
    case 'sm':
      baseClasses.push('gc-btn-sm')
      break
    case 'lg':
      baseClasses.push('gc-btn-lg')
      break
  }
  
  // 全寬度
  if (props.fullWidth) {
    baseClasses.push('gc-btn-full')
  }
  
  // 載入狀態
  if (props.loading) {
    baseClasses.push('gc-btn-loading')
  }
  
  // 禁用狀態
  if (props.disabled) {
    baseClasses.push('gc-btn-disabled')
  }
  
  return baseClasses.join(' ')
})

// 點擊事件處理
const handleClick = (event: MouseEvent) => {
  if (!props.disabled && !props.loading) {
    emit('click', event)
  }
}
</script>

<style scoped>
/* 按鈕尺寸變體 */
.gc-btn-sm {
  @apply px-3 py-1.5 text-sm;
}

.gc-btn-lg {
  @apply px-6 py-3 text-lg;
}

.gc-btn-full {
  @apply w-full justify-center;
}

/* 載入狀態 */
.gc-btn-loading {
  @apply cursor-not-allowed;
}

.gc-btn-disabled {
  @apply cursor-not-allowed opacity-50;
}

/* 載入動畫 */
.gc-spinner {
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

/* 圖標樣式 */
.gc-icon {
  @apply w-4 h-4;
}

.gc-icon-left {
  @apply mr-2;
}

.gc-icon-right {
  @apply ml-2;
}

/* 危險按鈕樣式 */
.gc-btn-danger {
  @apply gc-btn;
  border: 0;
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: #fff;
  box-shadow: var(--gc-shadow);
}

.gc-btn-danger:hover {
  background: linear-gradient(135deg, #dc2626, #b91c1c);
}
</style> 