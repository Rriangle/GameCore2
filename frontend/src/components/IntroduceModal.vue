<template>
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white">
      <div class="mt-3">
        <!-- 標題 -->
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-lg font-medium text-gray-900">
            {{ currentData ? '編輯個人介紹' : '新增個人介紹' }}
          </h3>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <!-- 表單 -->
        <form @submit.prevent="handleSubmit" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <!-- 暱稱 -->
            <div>
              <label for="userNickName" class="block text-sm font-medium text-gray-700">暱稱 *</label>
              <input
                id="userNickName"
                v-model="form.userNickName"
                type="text"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="請輸入暱稱"
              />
            </div>

            <!-- 性別 -->
            <div>
              <label for="gender" class="block text-sm font-medium text-gray-700">性別 *</label>
              <select
                id="gender"
                v-model="form.gender"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              >
                <option value="">請選擇性別</option>
                <option value="M">男</option>
                <option value="F">女</option>
              </select>
            </div>

            <!-- 身分證字號 -->
            <div>
              <label for="idNumber" class="block text-sm font-medium text-gray-700">身分證字號 *</label>
              <input
                id="idNumber"
                v-model="form.idNumber"
                type="text"
                required
                pattern="[A-Z][1-2]\d{8}"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="例：A123456789"
                maxlength="10"
              />
            </div>

            <!-- 手機號碼 -->
            <div>
              <label for="cellphone" class="block text-sm font-medium text-gray-700">手機號碼 *</label>
              <input
                id="cellphone"
                v-model="form.cellphone"
                type="tel"
                required
                pattern="09\d{8}"
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="例：0912345678"
                maxlength="10"
              />
            </div>

            <!-- 電子郵件 -->
            <div>
              <label for="email" class="block text-sm font-medium text-gray-700">電子郵件 *</label>
              <input
                id="email"
                v-model="form.email"
                type="email"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="例：user@example.com"
              />
            </div>

            <!-- 出生日期 -->
            <div>
              <label for="dateOfBirth" class="block text-sm font-medium text-gray-700">出生日期 *</label>
              <input
                id="dateOfBirth"
                v-model="form.dateOfBirth"
                type="date"
                required
                class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              />
            </div>
          </div>

          <!-- 地址 -->
          <div>
            <label for="address" class="block text-sm font-medium text-gray-700">地址 *</label>
            <input
              id="address"
              v-model="form.address"
              type="text"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              placeholder="請輸入完整地址"
            />
          </div>

          <!-- 自我介紹 -->
          <div>
            <label for="userIntroduceText" class="block text-sm font-medium text-gray-700">自我介紹</label>
            <textarea
              id="userIntroduceText"
              v-model="form.userIntroduceText"
              rows="4"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              placeholder="請簡單介紹一下自己..."
            ></textarea>
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

          <!-- 按鈕 -->
          <div class="flex justify-end space-x-3 pt-6">
            <button
              type="button"
              @click="$emit('close')"
              class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            >
              取消
            </button>
            <button
              type="submit"
              :disabled="userStore.isLoading"
              class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ userStore.isLoading ? '儲存中...' : '儲存' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import type { UpdateUserIntroduceRequest, UserIntroduceResponse } from '@/types/user'

interface Props {
  currentData?: UserIntroduceResponse
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  success: []
}>()

const userStore = useUserStore()

// 表單資料
const form = reactive<UpdateUserIntroduceRequest>({
  userNickName: '',
  gender: '',
  idNumber: '',
  cellphone: '',
  email: '',
  address: '',
  dateOfBirth: '',
  userIntroduceText: ''
})

// 處理提交
async function handleSubmit() {
  try {
    userStore.clearError()
    await userStore.updateIntroduce(form)
    emit('success')
  } catch (error) {
    console.error('更新個人介紹失敗:', error)
  }
}

// 初始化表單資料
onMounted(() => {
  if (props.currentData) {
    form.userNickName = props.currentData.userNickName
    form.gender = props.currentData.gender
    form.idNumber = props.currentData.idNumber
    form.cellphone = props.currentData.cellphone
    form.email = props.currentData.email
    form.address = props.currentData.address
    form.dateOfBirth = props.currentData.dateOfBirth.split('T')[0] // 轉換為 YYYY-MM-DD 格式
    form.userIntroduceText = props.currentData.userIntroduceText || ''
  }
  
  userStore.clearError()
})
</script>