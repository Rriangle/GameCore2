<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <div>
        <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
          註冊新帳戶
        </h2>
        <p class="mt-2 text-center text-sm text-gray-600">
          已經有帳戶了？
          <router-link to="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
            立即登入
          </router-link>
        </p>
      </div>
      
      <form class="mt-8 space-y-6" @submit.prevent="handleRegister">
        <div class="space-y-4">
          <div>
            <label for="userName" class="block text-sm font-medium text-gray-700">使用者姓名</label>
            <input
              id="userName"
              v-model="form.userName"
              name="userName"
              type="text"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="請輸入您的姓名"
            />
          </div>
          
          <div>
            <label for="userAccount" class="block text-sm font-medium text-gray-700">登入帳號</label>
            <input
              id="userAccount"
              v-model="form.userAccount"
              name="userAccount"
              type="text"
              required
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="請輸入登入帳號"
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
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="請輸入密碼（至少6位字元）"
              minlength="6"
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
              class="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
              placeholder="請再次輸入密碼"
              :class="{ 'border-red-500': form.password && form.confirmPassword && form.password !== form.confirmPassword }"
            />
            <p v-if="form.password && form.confirmPassword && form.password !== form.confirmPassword" 
               class="mt-1 text-sm text-red-600">
              密碼與確認密碼不一致
            </p>
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
            :disabled="userStore.isLoading || !isFormValid"
            class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="userStore.isLoading" class="absolute left-0 inset-y-0 flex items-center pl-3">
              <svg class="animate-spin h-5 w-5 text-indigo-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
            </span>
            {{ userStore.isLoading ? '註冊中...' : '註冊' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import type { RegisterUserRequest } from '@/types/user'

const router = useRouter()
const userStore = useUserStore()

// 表單資料
const form = reactive<RegisterUserRequest>({
  userName: '',
  userAccount: '',
  password: '',
  confirmPassword: ''
})

// 表單驗證
const isFormValid = computed(() => {
  return form.userName.trim() && 
         form.userAccount.trim() && 
         form.password.length >= 6 && 
         form.password === form.confirmPassword
})

// 處理註冊
async function handleRegister() {
  if (!isFormValid.value) {
    return
  }

  try {
    userStore.clearError()
    await userStore.register(form)
    
    // 註冊成功，導向到個人資料頁面
    router.push('/profile')
  } catch (error) {
    // 錯誤已經在 store 中處理
    console.error('註冊失敗:', error)
  }
}

// 組件掛載時清除錯誤
onMounted(() => {
  userStore.clearError()
})
</script>