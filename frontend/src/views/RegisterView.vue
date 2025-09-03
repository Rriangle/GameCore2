<template>
  <div class="gc-register-page">
    <div class="gc-register-container">
      <!-- 註冊表單卡片 -->
      <div class="gc-register-card">
        <!-- 標題區域 -->
        <div class="gc-register-header">
          <div class="gc-logo-badge">🎮</div>
          <h1 class="gc-register-title">註冊 GameCore</h1>
          <p class="gc-register-subtitle">
            或
            <router-link to="/login" class="gc-link-accent">
              登入現有帳號
            </router-link>
          </p>
        </div>
        
        <!-- 註冊表單 -->
        <form class="gc-register-form" @submit.prevent="handleSubmit" novalidate>
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
                minlength="3"
                maxlength="50"
                class="gc-input"
                :class="{ 'gc-input-error': errors.username }"
                placeholder="請輸入用戶名（3-50字元）"
                :disabled="loading"
                @blur="validateField('username')"
                @input="clearError('username')"
              />
            </div>
            <p v-if="errors.username" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.username }}
            </p>
            <p v-else class="gc-help-text">用戶名長度需在 3-50 字元之間</p>
          </div>
          
          <!-- 電子郵件輸入框 -->
          <div class="gc-form-group">
            <label for="email" class="gc-form-label">
              電子郵件
              <span class="gc-required">*</span>
            </label>
            <div class="gc-input-wrapper">
              <i class="bi bi-envelope gc-input-icon"></i>
              <input
                id="email"
                v-model="form.email"
                name="email"
                type="email"
                required
                class="gc-input"
                :class="{ 'gc-input-error': errors.email }"
                placeholder="請輸入電子郵件"
                :disabled="loading"
                @blur="validateField('email')"
                @input="clearError('email')"
              />
            </div>
            <p v-if="errors.email" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.email }}
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
                minlength="6"
                class="gc-input"
                :class="{ 'gc-input-error': errors.password }"
                placeholder="請輸入密碼（至少6字元）"
                :disabled="loading"
                @blur="validateField('password')"
                @input="clearError('password')"
              />
              <button 
                type="button" 
                class="gc-input-toggle"
                @click="togglePasswordVisibility('password')"
                :disabled="loading"
              >
                <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
              </button>
            </div>
            <p v-if="errors.password" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.password }}
            </p>
            <p v-else class="gc-help-text">密碼長度至少 6 字元</p>
          </div>
          
          <!-- 確認密碼輸入框 -->
          <div class="gc-form-group">
            <label for="confirmPassword" class="gc-form-label">
              確認密碼
              <span class="gc-required">*</span>
            </label>
            <div class="gc-input-wrapper">
              <i class="bi bi-lock-fill gc-input-icon"></i>
              <input
                id="confirmPassword"
                v-model="form.confirmPassword"
                name="confirmPassword"
                type="password"
                required
                class="gc-input"
                :class="{ 'gc-input-error': errors.confirmPassword }"
                placeholder="請再次輸入密碼"
                :disabled="loading"
                @blur="validateField('confirmPassword')"
                @input="clearError('confirmPassword')"
              />
              <button 
                type="button" 
                class="gc-input-toggle"
                @click="togglePasswordVisibility('confirmPassword')"
                :disabled="loading"
              >
                <i :class="showConfirmPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
              </button>
            </div>
            <p v-if="errors.confirmPassword" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.confirmPassword }}
            </p>
          </div>
          
          <!-- 同意條款 -->
          <div class="gc-form-group">
            <label class="gc-checkbox">
              <input 
                type="checkbox" 
                v-model="agreeTerms" 
                required
                :disabled="loading"
                @change="clearError('terms')"
              />
              <span class="gc-checkbox-label">
                我同意
                <a href="#" class="gc-link-accent">服務條款</a>
                和
                <a href="#" class="gc-link-accent">隱私權政策</a>
                <span class="gc-required">*</span>
              </span>
            </label>
            <p v-if="errors.terms" class="gc-form-error">
              <i class="bi bi-exclamation-circle"></i>
              {{ errors.terms }}
            </p>
          </div>
          
          <!-- 錯誤訊息 -->
          <div v-if="generalError" class="gc-alert gc-alert-error">
            <i class="bi bi-exclamation-triangle"></i>
            <span>{{ generalError }}</span>
          </div>
          
          <!-- 註冊按鈕 -->
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
            {{ loading ? '註冊中...' : '註冊' }}
          </button>
        </form>
        
        <!-- 額外選項 -->
        <div class="gc-register-options">
          <p class="gc-text-center">
            已有帳號？
            <router-link to="/login" class="gc-link-accent">
              立即登入
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import type { RegisterRequest } from '@/types/auth';

const router = useRouter();
const authStore = useAuthStore();

const loading = ref(false);
const generalError = ref<string | null>(null);
const agreeTerms = ref(false);
const showPassword = ref(false);
const showConfirmPassword = ref(false);

const form = reactive<RegisterRequest & { confirmPassword: string }>({
  username: '',
  email: '',
  password: '',
  confirmPassword: ''
});

const errors = reactive({
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
  terms: ''
});

// 表單驗證規則
const validationRules = {
  username: {
    required: '請輸入用戶名',
    minLength: (value: string) => value.length < 3 ? '用戶名至少需要3個字元' : '',
    maxLength: (value: string) => value.length > 50 ? '用戶名不能超過50個字元' : '',
    pattern: (value: string) => !/^[a-zA-Z0-9_]+$/.test(value) ? '用戶名只能包含字母、數字和下劃線' : ''
  },
  email: {
    required: '請輸入電子郵件',
    pattern: (value: string) => !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) ? '請輸入有效的電子郵件地址' : ''
  },
  password: {
    required: '請輸入密碼',
    minLength: (value: string) => value.length < 6 ? '密碼至少需要6個字元' : '',
    strength: (value: string) => {
      if (value.length < 6) return '';
      const hasLetter = /[a-zA-Z]/.test(value);
      const hasNumber = /\d/.test(value);
      if (!hasLetter || !hasNumber) {
        return '密碼應包含字母和數字';
      }
      return '';
    }
  },
  confirmPassword: {
    required: '請確認密碼',
    match: (value: string) => value !== form.password ? '密碼不一致，請重新輸入' : ''
  },
  terms: {
    required: '請同意服務條款和隱私權政策'
  }
};

// 驗證單個欄位
const validateField = (field: keyof typeof errors) => {
  const value = form[field as keyof typeof form] as string;
  const rules = validationRules[field as keyof typeof validationRules];
  
  if (!value && field !== 'terms') {
    errors[field] = rules.required;
    return false;
  }
  
  // 特殊處理條款同意
  if (field === 'terms') {
    if (!agreeTerms.value) {
      errors.terms = rules.required;
      return false;
    }
    errors.terms = '';
    return true;
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
  
  // 檢查模式
  if (rules.pattern) {
    const patternError = rules.pattern(value);
    if (patternError) {
      errors[field] = patternError;
      return false;
    }
  }
  
  // 檢查密碼強度
  if (rules.strength) {
    const strengthError = rules.strength(value);
    if (strengthError) {
      errors[field] = strengthError;
      return false;
    }
  }
  
  // 檢查密碼匹配
  if (rules.match) {
    const matchError = rules.match(value);
    if (matchError) {
      errors[field] = matchError;
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
  const emailValid = validateField('email');
  const passwordValid = validateField('password');
  const confirmPasswordValid = validateField('confirmPassword');
  const termsValid = validateField('terms');
  
  return usernameValid && emailValid && passwordValid && confirmPasswordValid && termsValid;
};

// 表單是否有效
const isFormValid = computed(() => {
  return form.username && form.email && form.password && form.confirmPassword && 
         agreeTerms.value && !errors.username && !errors.email && !errors.password && 
         !errors.confirmPassword && !errors.terms;
});

// 切換密碼可見性
const togglePasswordVisibility = (field: 'password' | 'confirmPassword') => {
  if (field === 'password') {
    showPassword.value = !showPassword.value;
    const passwordInput = document.getElementById('password') as HTMLInputElement;
    if (passwordInput) {
      passwordInput.type = showPassword.value ? 'text' : 'password';
    }
  } else {
    showConfirmPassword.value = !showConfirmPassword.value;
    const confirmPasswordInput = document.getElementById('confirmPassword') as HTMLInputElement;
    if (confirmPasswordInput) {
      confirmPasswordInput.type = showConfirmPassword.value ? 'text' : 'password';
    }
  }
};

onMounted(() => {
  // 如果已經登入，跳轉到首頁
  if (authStore.isAuthenticated) {
    router.push('/');
  }
});

const handleSubmit = async () => {
  if (!isFormValid.value) {
    error.value = '請填寫所有必要欄位並確認密碼一致';
    return;
  }
  
  if (!agreeTerms.value) {
    error.value = '請同意服務條款和隱私政策';
    return;
  }
  
  loading.value = true;
  error.value = null;
  
  try {
    const { confirmPassword, ...registerData } = form;
    await authStore.register(registerData);
    router.push('/login');
  } catch (err: any) {
    error.value = err.message || '註冊失敗，請稍後再試';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
/* 註冊頁面樣式 */
.gc-register-page {
  @apply min-h-screen flex items-center justify-center py-12 px-4;
  background: 
    radial-gradient(900px 500px at -10% -10%, rgba(117, 87, 255, 0.10), transparent 60%),
    radial-gradient(800px 460px at 110% 10%, rgba(52, 210, 255, 0.10), transparent 50%),
    linear-gradient(180deg, var(--gc-bg), var(--gc-bg2));
}

.gc-register-container {
  @apply w-full max-w-md;
}

.gc-register-card {
  @apply gc-card p-8;
  background: var(--gc-surface);
  backdrop-filter: blur(var(--gc-blur));
}

.gc-register-header {
  @apply text-center mb-8;
}

.gc-logo-badge {
  @apply mx-auto w-16 h-16 flex items-center justify-center text-3xl rounded-2xl mb-4;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: #fff;
  box-shadow: var(--gc-shadow);
}

.gc-register-title {
  @apply text-3xl font-black mb-2;
  color: var(--gc-ink);
}

.gc-register-subtitle {
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

.gc-register-form {
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

.gc-help-text {
  @apply text-sm;
  color: var(--gc-muted);
}

.gc-text-center {
  @apply text-center;
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

.gc-checkbox {
  @apply flex items-start space-x-2 cursor-pointer;
}

.gc-checkbox-input {
  @apply w-4 h-4 rounded border-2 mt-0.5;
  border-color: var(--gc-line);
  background: var(--gc-surface);
}

.gc-checkbox-input:checked {
  border-color: var(--gc-accent);
  background: var(--gc-accent);
}

.gc-checkbox-label {
  @apply text-sm leading-relaxed;
  color: var(--gc-muted);
}

/* 響應式設計 */
@media (max-width: 640px) {
  .gc-register-card {
    @apply p-6;
  }
  
  .gc-register-title {
    @apply text-2xl;
  }
  
  .gc-logo-badge {
    @apply w-12 h-12 text-2xl;
  }
}
</style>
