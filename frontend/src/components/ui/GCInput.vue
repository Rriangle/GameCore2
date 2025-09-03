<template>
  <div class="gc-input-wrapper">
    <!-- 標籤 -->
    <label
      v-if="label"
      :for="inputId"
      class="gc-form-label"
      :class="{ 'gc-form-label-required': required }"
    >
      {{ label }}
      <span v-if="required" class="gc-required-mark">*</span>
    </label>
    
    <!-- 輸入框容器 -->
    <div class="gc-input-container" :class="containerClasses">
      <!-- 左側圖標 -->
      <div v-if="leftIcon" class="gc-input-icon gc-input-icon-left">
        <component :is="leftIcon" class="gc-icon" />
      </div>
      
      <!-- 輸入框 -->
      <input
        :id="inputId"
        ref="inputRef"
        v-model="inputValue"
        :type="inputType"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        :min="min"
        :max="max"
        :step="step"
        :maxlength="maxlength"
        :pattern="pattern"
        :autocomplete="autocomplete"
        :autofocus="autofocus"
        :name="name"
        class="gc-input"
        :class="inputClasses"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
        @keydown="handleKeydown"
        v-bind="$attrs"
      />
      
      <!-- 右側圖標 -->
      <div v-if="rightIcon" class="gc-input-icon gc-input-icon-right">
        <component :is="rightIcon" class="gc-icon" />
      </div>
      
      <!-- 清除按鈕 -->
      <button
        v-if="clearable && inputValue && !disabled"
        type="button"
        class="gc-input-clear"
        @click="handleClear"
        aria-label="清除輸入內容"
      >
        <svg class="gc-clear-icon" viewBox="0 0 24 24" fill="none">
          <path
            d="M18 6L6 18M6 6L18 18"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </button>
    </div>
    
    <!-- 幫助文字 -->
    <p v-if="helpText" class="gc-help-text">
      {{ helpText }}
    </p>
    
    <!-- 錯誤訊息 -->
    <p v-if="errorMessage" class="gc-form-error">
      {{ errorMessage }}
    </p>
    
    <!-- 字數統計 -->
    <p v-if="showCount && maxlength" class="gc-char-count">
      {{ inputValue.length }}/{{ maxlength }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, onMounted, watch } from 'vue'

// 組件屬性定義
interface Props {
  modelValue?: string | number
  type?: 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'search'
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  readonly?: boolean
  clearable?: boolean
  showCount?: boolean
  leftIcon?: any
  rightIcon?: any
  helpText?: string
  errorMessage?: string
  name?: string
  min?: number | string
  max?: number | string
  step?: number | string
  maxlength?: number
  pattern?: string
  autocomplete?: string
  autofocus?: boolean
  size?: 'sm' | 'md' | 'lg'
}

// 組件事件定義
interface Emits {
  (e: 'update:modelValue', value: string | number): void
  (e: 'input', value: string | number): void
  (e: 'focus', event: FocusEvent): void
  (e: 'blur', event: FocusEvent): void
  (e: 'keydown', event: KeyboardEvent): void
  (e: 'clear'): void
}

// 屬性預設值
const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  required: false,
  disabled: false,
  readonly: false,
  clearable: false,
  showCount: false,
  size: 'md'
})

// 事件發射
const emit = defineEmits<Emits>()

// 響應式數據
const inputRef = ref<HTMLInputElement>()
const inputValue = ref(props.modelValue || '')

// 計算屬性
const inputId = computed(() => `gc-input-${Math.random().toString(36).substr(2, 9)}`)
const inputType = computed(() => props.type)
const containerClasses = computed(() => {
  const classes = ['gc-input-container']
  
  if (props.size !== 'md') {
    classes.push(`gc-input-${props.size}`)
  }
  
  if (props.leftIcon) {
    classes.push('gc-input-has-left-icon')
  }
  
  if (props.rightIcon) {
    classes.push('gc-input-has-right-icon')
  }
  
  if (props.clearable) {
    classes.push('gc-input-clearable')
  }
  
  return classes.join(' ')
})

const inputClasses = computed(() => {
  const classes = ['gc-input']
  
  if (props.size !== 'md') {
    classes.push(`gc-input-${props.size}`)
  }
  
  if (props.errorMessage) {
    classes.push('gc-input-error')
  }
  
  return classes.join(' ')
})

// 監聽 modelValue 變化
watch(() => props.modelValue, (newValue) => {
  inputValue.value = newValue || ''
})

// 事件處理
const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  const value = target.value
  
  inputValue.value = value
  emit('update:modelValue', value)
  emit('input', value)
}

const handleFocus = (event: FocusEvent) => {
  emit('focus', event)
}

const handleBlur = (event: FocusEvent) => {
  emit('blur', event)
}

const handleKeydown = (event: KeyboardEvent) => {
  emit('keydown', event)
}

const handleClear = () => {
  inputValue.value = ''
  emit('update:modelValue', '')
  emit('clear')
  
  // 聚焦到輸入框
  nextTick(() => {
    inputRef.value?.focus()
  })
}

// 生命週期
onMounted(() => {
  if (props.autofocus) {
    inputRef.value?.focus()
  }
})
</script>

<style scoped>
/* 輸入框包裝器 */
.gc-input-wrapper {
  @apply w-full;
}

/* 輸入框容器 */
.gc-input-container {
  @apply relative flex items-center;
}

/* 輸入框尺寸變體 */
.gc-input-sm {
  @apply px-3 py-2 text-sm;
}

.gc-input-lg {
  @apply px-5 py-4 text-lg;
}

/* 圖標樣式 */
.gc-input-icon {
  @apply absolute z-10 flex items-center justify-center text-gray-400;
  width: 40px;
}

.gc-input-icon-left {
  left: 0;
}

.gc-input-icon-right {
  right: 0;
}

.gc-icon {
  @apply w-4 h-4;
}

/* 有左側圖標的輸入框 */
.gc-input-has-left-icon .gc-input {
  padding-left: 48px;
}

/* 有右側圖標的輸入框 */
.gc-input-has-right-icon .gc-input {
  padding-right: 48px;
}

/* 可清除的輸入框 */
.gc-input-clearable .gc-input {
  padding-right: 48px;
}

/* 清除按鈕 */
.gc-input-clear {
  @apply absolute right-0 top-1/2 -translate-y-1/2 w-10 h-10 flex items-center justify-center text-gray-400 hover:text-gray-600 transition-colors duration-200;
  z-index: 10;
}

.gc-clear-icon {
  @apply w-4 h-4;
}

/* 錯誤狀態 */
.gc-input-error {
  border-color: var(--gc-down);
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
}

/* 幫助文字 */
.gc-help-text {
  @apply text-sm mt-1;
  color: var(--gc-muted);
}

/* 字數統計 */
.gc-char-count {
  @apply text-sm mt-1 text-right;
  color: var(--gc-muted);
}

/* 必填標記 */
.gc-required-mark {
  @apply text-red-500 ml-1;
}
</style> 