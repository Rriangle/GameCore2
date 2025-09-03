<template>
  <div
    :class="cardClasses"
    :style="cardStyles"
    @click="handleClick"
  >
    <!-- 卡片頭部 -->
    <div v-if="$slots.header || title" class="gc-card-header">
      <slot name="header">
        <h3 v-if="title" class="gc-card-title">{{ title }}</h3>
        <p v-if="subtitle" class="gc-card-subtitle">{{ subtitle }}</p>
      </slot>
    </div>
    
    <!-- 卡片內容 -->
    <div class="gc-card-content">
      <slot />
    </div>
    
    <!-- 卡片底部 -->
    <div v-if="$slots.footer" class="gc-card-footer">
      <slot name="footer" />
    </div>
    
    <!-- 點擊遮罩 -->
    <div v-if="clickable" class="gc-card-overlay" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

// 組件屬性定義
interface Props {
  title?: string
  subtitle?: string
  variant?: 'default' | 'glass' | 'elevated' | 'outlined'
  size?: 'sm' | 'md' | 'lg'
  clickable?: boolean
  hover?: boolean
  padding?: 'none' | 'sm' | 'md' | 'lg'
  shadow?: 'none' | 'sm' | 'md' | 'lg'
  border?: boolean
  radius?: 'none' | 'sm' | 'md' | 'lg' | 'full'
}

// 組件事件定義
interface Emits {
  (e: 'click', event: MouseEvent): void
}

// 屬性預設值
const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  size: 'md',
  clickable: false,
  hover: true,
  padding: 'md',
  shadow: 'md',
  border: true,
  radius: 'md'
})

// 事件發射
const emit = defineEmits<Emits>()

// 計算屬性
const cardClasses = computed(() => {
  const classes = ['gc-card']
  
  // 變體樣式
  if (props.variant !== 'default') {
    classes.push(`gc-card-${props.variant}`)
  }
  
  // 尺寸樣式
  if (props.size !== 'md') {
    classes.push(`gc-card-${props.size}`)
  }
  
  // 內邊距樣式
  if (props.padding !== 'md') {
    classes.push(`gc-card-padding-${props.padding}`)
  }
  
  // 陰影樣式
  if (props.shadow !== 'md') {
    classes.push(`gc-card-shadow-${props.shadow}`)
  }
  
  // 圓角樣式
  if (props.radius !== 'md') {
    classes.push(`gc-card-radius-${props.radius}`)
  }
  
  // 邊框樣式
  if (!props.border) {
    classes.push('gc-card-no-border')
  }
  
  // 可點擊樣式
  if (props.clickable) {
    classes.push('gc-card-clickable')
  }
  
  // 懸停效果
  if (props.hover) {
    classes.push('gc-card-hover')
  }
  
  return classes.join(' ')
})

const cardStyles = computed(() => {
  const styles: Record<string, string> = {}
  
  // 自定義圓角
  if (props.radius === 'none') {
    styles.borderRadius = '0'
  } else if (props.radius === 'full') {
    styles.borderRadius = '9999px'
  }
  
  return styles
})

// 事件處理
const handleClick = (event: MouseEvent) => {
  if (props.clickable) {
    emit('click', event)
  }
}
</script>

<style scoped>
/* 卡片基礎樣式 */
.gc-card {
  @apply relative overflow-hidden transition-all duration-200;
}

/* 卡片變體 */
.gc-card-glass {
  background: var(--gc-glass);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-card-elevated {
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.gc-card-outlined {
  background: transparent;
  border: 2px solid var(--gc-line);
}

/* 卡片尺寸 */
.gc-card-sm {
  @apply text-sm;
}

.gc-card-lg {
  @apply text-lg;
}

/* 內邊距變體 */
.gc-card-padding-none {
  padding: 0;
}

.gc-card-padding-sm {
  padding: 0.75rem;
}

.gc-card-padding-lg {
  padding: 1.5rem;
}

/* 陰影變體 */
.gc-card-shadow-none {
  box-shadow: none;
}

.gc-card-shadow-sm {
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
}

.gc-card-shadow-lg {
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

/* 圓角變體 */
.gc-card-radius-none {
  border-radius: 0;
}

.gc-card-radius-sm {
  border-radius: 0.5rem;
}

.gc-card-radius-lg {
  border-radius: 1.5rem;
}

/* 無邊框 */
.gc-card-no-border {
  border: none;
}

/* 可點擊樣式 */
.gc-card-clickable {
  cursor: pointer;
}

.gc-card-clickable:hover {
  transform: translateY(-2px);
}

/* 懸停效果 */
.gc-card-hover:hover {
  transform: translateY(-3px) scale(1.01);
}

/* 卡片頭部 */
.gc-card-header {
  @apply border-b pb-4 mb-4;
  border-color: var(--gc-line);
}

.gc-card-title {
  @apply text-lg font-bold mb-1;
  color: var(--gc-ink);
}

.gc-card-subtitle {
  @apply text-sm;
  color: var(--gc-muted);
}

/* 卡片內容 */
.gc-card-content {
  @apply flex-1;
}

/* 卡片底部 */
.gc-card-footer {
  @apply border-t pt-4 mt-4;
  border-color: var(--gc-line);
}

/* 點擊遮罩 */
.gc-card-overlay {
  @apply absolute inset-0 bg-transparent transition-colors duration-200;
}

.gc-card-clickable:hover .gc-card-overlay {
  background: rgba(0, 0, 0, 0.05);
}

/* 深色模式適配 */
.dark .gc-card-clickable:hover .gc-card-overlay {
  background: rgba(255, 255, 255, 0.05);
}
</style> 