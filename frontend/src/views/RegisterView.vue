<template>
  <div class="min-h-screen bg-gradient-to-br from-green-50 to-emerald-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <div class="mx-auto h-12 w-12 flex items-center justify-center rounded-full bg-primary-100">
          <span class="text-2xl">🎮</span>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          註冊 GameCore
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          或
          <router-link to="/login" class="font-medium text-primary-600 hover:text-primary-500">
            登入現有帳號
          </router-link>
        </p>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleSubmit">
        <div class="space-y-4">
          <div>
            <label for="username" class="block text-sm font-medium text-gray-700">用戶名</label>
            <input
              id="username"
              v-model="form.username"
              name="username"
              type="text"
              required
              minlength="3"
              maxlength="50"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
              placeholder="請輸入用戶名（3-50字元）"
              :disabled="loading"
            />
          </div>
          
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">電子郵件</label>
            <input
              id="email"
              v-model="form.email"
              name="email"
              type="email"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
              placeholder="請輸入電子郵件"
              :disabled="loading"
            />
          </div>
          
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">密碼</label>
            <input
              id="password"
              v-model="form.password"
              name="password"
              type="password"
              required
              minlength="6"
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
              placeholder="請輸入密碼（至少6字元）"
              :disabled="loading"
            />
          </div>
          
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700">確認密碼</label>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              name="confirmPassword"
              type="password"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
              placeholder="請再次輸入密碼"
              :disabled="loading"
            />
          </div>
        </div>

        <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
          {{ error }}
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading || !isFormValid"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="loading" class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ loading ? '註冊中...' : '註冊' }}
          </button>
        </div>
      </form>
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
const error = ref<string | null>(null);

const form = reactive<RegisterRequest>({
  username: '',
  email: '',
  password: '',
  confirmPassword: ''
});

const isFormValid = computed(() => {
  return form.username.length >= 3 &&
         form.email.includes('@') &&
         form.password.length >= 6 &&
         form.password === form.confirmPassword;
});

onMounted(() => {
  // 如果已經登入，跳轉到首頁
  if (authStore.isAuthenticated) {
    router.push('/');
  }
});

const handleSubmit = async () => {
  if (!isFormValid.value) {
    error.value = '請檢查表單資料是否正確';
    return;
  }

  if (form.password !== form.confirmPassword) {
    error.value = '密碼與確認密碼不符';
    return;
  }

  loading.value = true;
  error.value = null;

  try {
    const result = await authStore.register(form);
    
    if (result.success) {
      router.push('/');
    } else {
      error.value = result.message || '註冊失敗';
    }
  } catch (err: any) {
    error.value = err.message || '註冊過程中發生錯誤';
  } finally {
    loading.value = false;
  }
};
</script>
