<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          登入您的帳戶
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          或者
          <router-link to="/register" class="font-medium text-indigo-600 hover:text-indigo-500">
            註冊新帳戶
          </router-link>
        </p>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleLogin">
        <div class="rounded-md shadow-sm -space-y-px">
          <div>
            <label for="userAccount" class="sr-only">帳號</label>
            <input
              id="userAccount"
              v-model="form.userAccount"
              name="userAccount"
              type="text"
              required
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="請輸入登入帳號"
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
              class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="請輸入密碼"
            />
          </div>
        </div>

        <!-- 錯誤訊息 -->
        <div v-if="userStore.error" class="rounded-md bg-red-50 p-4">
          <div class="flex">
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800">
                {{ userStore.error }}
              </h3>
            </div>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="userStore.isLoading"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="userStore.isLoading" class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="animate-spin h-5 w-5 text-indigo-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ userStore.isLoading ? '登入中...' : '登入' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import type { LoginUserRequest } from '@/types/user'

const router = useRouter()
const userStore = useUserStore()

// 表單資料
const form = reactive<LoginUserRequest>({
  userAccount: '',
  password: ''
})

// 處理登入
async function handleLogin() {
  try {
    userStore.clearError()
    await userStore.login(form)
    
    // 登入成功，導向到首頁或原本要前往的頁面
    const redirect = router.currentRoute.value.query.redirect as string
    router.push(redirect || '/profile')
  } catch (error) {
    // 錯誤已經在 store 中處理
    console.error('登入失敗:', error)
  }
}

// 組件掛載時清除錯誤
onMounted(() => {
  userStore.clearError()
})
</script>