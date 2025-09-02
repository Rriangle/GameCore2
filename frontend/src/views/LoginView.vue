<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <div class="mx-auto h-12 w-12 flex items-center justify-center rounded-full bg-primary-100">
          <span class="text-2xl">🎮</span>
        </div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          登入 GameCore
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          或
          <router-link to="/register" class="font-medium text-primary-600 hover:text-primary-500">
            註冊新帳號
          </router-link>
        </p>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleSubmit">
        <div class="rounded-md shadow-sm -space-y-px">
          <div>
            <label for="username" class="sr-only">用戶名</label>
            <input
              id="username"
              v-model="form.username"
              name="username"
              type="text"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 focus:z-10 sm:text-sm"
              placeholder="用戶名"
              :disabled="loading"
            />
          </div>
          <div>
            <label for="password" class="sr-only">密碼</label>
            <input
              id="password"
              v-model="form.password"
              name="password"
              type="password"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 focus:z-10 sm:text-sm"
              placeholder="密碼"
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
            :disabled="loading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="loading" class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ loading ? '登入中...' : '登入' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import type { LoginRequest } from '@/types/auth';

const router = useRouter();
const authStore = useAuthStore();

const loading = ref(false);
const error = ref<string | null>(null);

const form = reactive<LoginRequest>({
  username: '',
  password: ''
});

onMounted(() => {
  // 如果已經登入，跳轉到首頁
  if (authStore.isAuthenticated) {
    router.push('/');
  }
});

const handleSubmit = async () => {
  if (!form.username || !form.password) {
    error.value = '請填寫所有欄位';
    return;
  }

  loading.value = true;
  error.value = null;

  try {
    const result = await authStore.login(form);
    
    if (result.success) {
      router.push('/');
    } else {
      error.value = result.message || '登入失敗';
    }
  } catch (err: any) {
    error.value = err.message || '登入過程中發生錯誤';
  } finally {
    loading.value = false;
  }
};
</script>
