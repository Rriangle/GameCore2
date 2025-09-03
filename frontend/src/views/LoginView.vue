<template>
  <div class="gc-login-page">
    <div class="gc-login-container">
      <!-- 登入表單卡片 -->
      <div class="gc-login-card">
        <!-- 標題區域 -->
        <div class="gc-login-header">
          <div class="gc-logo-badge">🎮</div>
          <h1 class="gc-login-title">登入 GameCore</h1>
          <p class="gc-login-subtitle">
            或
            <router-link to="/register" class="gc-link-accent">
              註冊新帳號
            </router-link>
          </p>
        </div>
        
        <!-- 登入表單 -->
        <form class="gc-login-form" @submit.prevent="handleSubmit" novalidate>
          <!-- 用戶名輸入框 -->
          <div class="gc-form-group">
            <label for="username" class="gc-form-label">
              用戶名
              <span class="gc-required">*</span>
            </label>
            <div class="gc-input-wrapper">
              <i class="bi bi-person gc-input-icon"></i>
              <input
                id="username"
                v-model="form.username"
                name="username"
                type="text"
                required
                class="gc-input"
                :class="{ 'gc-input-error': errors.username }"
                placeholder="請輸入用戶名"
                :disabled="loading"
                @blur="validateField('username')"
                @input="clearError('username')"
              />
            </div>
            <p v-if="errors.username" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.username }}
            </p>
          </div>
          
          <!-- 密碼輸入框 -->
          <div class="gc-form-group">
            <label for="password" class="gc-form-label">
              密碼
              <span class="gc-required">*</span>
            </label>
            <div class="gc-input-wrapper">
              <i class="bi bi-lock gc-input-icon"></i>
              <input
                id="password"
                v-model="form.password"
                name="password"
                type="password"
                required
                class="gc-input"
                :class="{ 'gc-input-error': errors.password }"
                placeholder="請輸入密碼"
                :disabled="loading"
                @blur="validateField('password')"
                @input="clearError('password')"
              />
              <button 
                type="button" 
                class="gc-input-toggle"
                @click="togglePasswordVisibility"
                :disabled="loading"
              >
                <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
              </button>
            </div>
            <p v-if="errors.password" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.password }}
            </p>
          </div>
          
          <!-- 錯誤訊息 -->
          <div v-if="generalError" class="gc-alert gc-alert-error">
            <i class="bi bi-exclamation-triangle"></i>
            <span>{{ generalError }}</span>
          </div>
          
          <!-- 登入按鈕 -->
          <button
            type="submit"
            :disabled="loading || !isFormValid"
            class="gc-btn gc-btn-primary gc-btn-full"
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
            {{ loading ? '登入中...' : '登入' }}
          </button>
        </form>
        
        <!-- 額外選項 -->
        <div class="gc-login-options">
          <label class="gc-checkbox">
            <input 
              type="checkbox" 
              v-model="rememberMe" 
              :disabled="loading"
            />
            <span class="gc-checkbox-label">記住我</span>
          </label>
          <a href="#" class="gc-link-accent">忘記密碼？</a>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import type { LoginRequest } from '@/types/auth';

const router = useRouter();
const authStore = useAuthStore();

const loading = ref(false);
const generalError = ref<string | null>(null);
const rememberMe = ref(false);
const showPassword = ref(false);

const form = reactive<LoginRequest>({
  username: '',
  password: ''
});

const errors = reactive({
  username: '',
  password: ''
});

// 表單驗證規則
const validationRules = {
  username: {
    required: '請輸入用戶名',
    minLength: (value: string) => value.length < 3 ? '用戶名至少需要3個字元' : '',
    maxLength: (value: string) => value.length > 50 ? '用戶名不能超過50個字元' : ''
  },
  password: {
    required: '請輸入密碼',
    minLength: (value: string) => value.length < 6 ? '密碼至少需要6個字元' : ''
  }
};

// 驗證單個欄位
const validateField = (field: keyof typeof errors) => {
  const value = form[field as keyof LoginRequest] as string;
  const rules = validationRules[field as keyof typeof validationRules];
  
  if (!value) {
    errors[field] = rules.required;
    return false;
  }
  
  // 檢查最小長度
  if (rules.minLength) {
    const minError = rules.minLength(value);
    if (minError) {
      errors[field] = minError;
      return false;
    }
  }
  
  // 檢查最大長度
  if (rules.maxLength) {
    const maxError = rules.maxLength(value);
    if (maxError) {
      errors[field] = maxError;
      return false;
    }
  }
  
  errors[field] = '';
  return true;
};

// 清除錯誤
const clearError = (field: keyof typeof errors) => {
  errors[field] = '';
  generalError.value = null;
};

// 驗證整個表單
const validateForm = () => {
  const usernameValid = validateField('username');
  const passwordValid = validateField('password');
  return usernameValid && passwordValid;
};

// 表單是否有效
const isFormValid = computed(() => {
  return form.username && form.password && !errors.username && !errors.password;
});

// 切換密碼可見性
const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value;
  const passwordInput = document.getElementById('password') as HTMLInputElement;
  if (passwordInput) {
    passwordInput.type = showPassword.value ? 'text' : 'password';
  }
};

onMounted(() => {
  // 如果已經登入，跳轉到首頁
  if (authStore.isAuthenticated) {
    router.push('/');
  }
});

const handleSubmit = async () => {
  // 清除之前的錯誤
  generalError.value = null;
  
  // 驗證表單
  if (!validateForm()) {
    return;
  }
  
  loading.value = true;
  
  try {
    await authStore.login(form);
    router.push('/');
  } catch (err: any) {
    generalError.value = err.message || '登入失敗，請檢查用戶名和密碼';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
/* 登入頁面樣式 */
.gc-login-page {
  @apply min-h-screen flex items-center justify-center py-12 px-4;
  background: 
    radial-gradient(900px 500px at -10% -10%, rgba(117, 87, 255, 0.10), transparent 60%),
    radial-gradient(800px 460px at 110% 10%, rgba(52, 210, 255, 0.10), transparent 50%),
    linear-gradient(180deg, var(--gc-bg), var(--gc-bg2));
}

.gc-login-container {
  @apply w-full max-w-md;
}

.gc-login-card {
  @apply gc-card p-8;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-login-header {
  @apply text-center mb-8;
}

.gc-logo-badge {
  @apply mx-auto w-16 h-16 flex items-center justify-center text-3xl rounded-2xl mb-4;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: #fff;
  box-shadow: var(--gc-shadow);
}

.gc-login-title {
  @apply text-3xl font-black mb-2;
  color: var(--gc-ink);
}

.gc-login-subtitle {
  @apply text-sm;
  color: var(--gc-muted);
}

.gc-link-accent {
  @apply font-medium transition-colors duration-200;
  color: var(--gc-accent);
}

.gc-link-accent:hover {
  color: var(--gc-accent-2);
}

.gc-login-form {
  @apply space-y-6;
}

.gc-form-group {
  @apply space-y-2;
}

.gc-form-label {
  @apply block text-sm font-medium;
  color: var(--gc-ink);
}

.gc-input {
  @apply w-full px-4 py-3 pl-10 pr-10 rounded-xl border transition-all duration-200;
  background: var(--gc-surface);
  border-color: var(--gc-line);
  color: var(--gc-ink);
}

.gc-input:focus {
  @apply outline-none;
  border-color: var(--gc-accent);
  box-shadow: 0 0 0 3px rgba(117, 87, 255, 0.1);
}

.gc-input:disabled {
  @apply opacity-50 cursor-not-allowed;
}

.gc-input-error {
  @apply border-red-500;
}

.gc-input-error:focus {
  border-color: var(--gc-down);
  box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
}

.gc-form-error {
  @apply flex items-center text-red-500 text-sm mt-1;
}

.gc-form-error .bi {
  @apply mr-1;
}

.gc-alert {
  @apply flex items-center px-4 py-3 rounded-xl text-sm;
}

.gc-alert-error {
  @apply bg-red-50 border border-red-200 text-red-700;
}

.gc-alert-error .bi {
  @apply mr-2;
}

.gc-required {
  @apply text-red-500 ml-1;
}

.gc-input-wrapper {
  @apply relative;
}

.gc-input-icon {
  @apply absolute left-3 top-1/2 -translate-y-1/2 text-lg;
  color: var(--gc-muted);
}

.gc-input-toggle {
  @apply absolute right-3 top-1/2 -translate-y-1/2 text-lg cursor-pointer;
  color: var(--gc-muted);
  z-index: 1;
}

.gc-input-toggle:hover {
  color: var(--gc-ink);
}

.gc-btn-full {
  @apply w-full justify-center;
}

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

.gc-login-options {
  @apply flex items-center justify-between mt-6 pt-6 border-t;
  border-color: var(--gc-line);
}

.gc-checkbox {
  @apply flex items-center space-x-2 cursor-pointer;
}

.gc-checkbox input[type="checkbox"] {
  @apply w-4 h-4 rounded border-2;
  border-color: var(--gc-line);
  background: var(--gc-surface);
}

.gc-checkbox input[type="checkbox"]:checked {
  border-color: var(--gc-accent);
  background: var(--gc-accent);
}

.gc-checkbox-label {
  @apply text-sm;
  color: var(--gc-muted);
}

/* 響應式設計 */
@media (max-width: 640px) {
  .gc-login-card {
    @apply p-6;
  }
  
  .gc-login-title {
    @apply text-2xl;
  }
  
  .gc-logo-badge {
    @apply w-12 h-12 text-2xl;
  }
}
</style>
