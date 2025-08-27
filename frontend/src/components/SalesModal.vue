<template>
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-1/2 lg:w-1/3 shadow-lg rounded-md bg-white">
      <div class="mt-3">
        <!-- 標題 -->
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-lg font-medium text-gray-900">申請銷售功能</h3>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <!-- 說明文字 -->
        <div class="mb-6 p-4 bg-blue-50 rounded-md">
          <p class="text-sm text-blue-800">
            申請銷售功能需要提供銀行帳戶資訊，用於接收銷售收入。請確保填寫的資訊正確無誤。
          </p>
        </div>

        <!-- 表單 -->
        <form @submit.prevent="handleSubmit" class="space-y-4">
          <!-- 銀行代號 -->
          <div>
            <label for="bankCode" class="block text-sm font-medium text-gray-700">銀行代號 *</label>
            <input
              id="bankCode"
              v-model.number="form.bankCode"
              type="number"
              required
              min="1"
              max="9999"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              placeholder="例：812 (台新銀行)"
            />
            <p class="mt-1 text-xs text-gray-500">請輸入3-4位數的銀行代號</p>
          </div>

          <!-- 銀行帳號 -->
          <div>
            <label for="bankAccountNumber" class="block text-sm font-medium text-gray-700">銀行帳號 *</label>
            <input
              id="bankAccountNumber"
              v-model="form.bankAccountNumber"
              type="text"
              required
              pattern="[0-9]{10,16}"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
              placeholder="請輸入完整銀行帳號"
              maxlength="16"
            />
            <p class="mt-1 text-xs text-gray-500">請輸入10-16位數的銀行帳號</p>
          </div>

          <!-- 帳戶封面照片 -->
          <div>
            <label for="accountCoverPhoto" class="block text-sm font-medium text-gray-700">帳戶封面照片</label>
            <input
              id="accountCoverPhoto"
              type="file"
              accept="image/*"
              @change="handleFileChange"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
            <p class="mt-1 text-xs text-gray-500">可選：上傳存摺封面或銀行卡照片（JPG, PNG格式，最大2MB）</p>
            
            <!-- 照片預覽 -->
            <div v-if="imagePreview" class="mt-2">
              <img :src="imagePreview" alt="照片預覽" class="h-32 w-auto border rounded-md" />
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
              :disabled="userStore.isLoading || !isFormValid"
              class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ userStore.isLoading ? '申請中...' : '提交申請' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, computed, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import type { ApplySalesRequest } from '@/types/user'

const emit = defineEmits<{
  close: []
  success: []
}>()

const userStore = useUserStore()

// 表單資料
const form = reactive<ApplySalesRequest>({
  bankCode: 0,
  bankAccountNumber: '',
  accountCoverPhotoBase64: undefined
})

// 圖片預覽
const imagePreview = ref<string>('')

// 表單驗證
const isFormValid = computed(() => {
  return form.bankCode > 0 && 
         form.bankAccountNumber.length >= 10 && 
         /^[0-9]+$/.test(form.bankAccountNumber)
})

// 處理檔案變更
function handleFileChange(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (file) {
    // 檢查檔案大小（2MB限制）
    if (file.size > 2 * 1024 * 1024) {
      alert('檔案大小不能超過2MB')
      target.value = ''
      return
    }
    
    // 檢查檔案類型
    if (!file.type.startsWith('image/')) {
      alert('請選擇圖片檔案')
      target.value = ''
      return
    }
    
    // 讀取檔案並轉換為Base64
    const reader = new FileReader()
    reader.onload = (e) => {
      const result = e.target?.result as string
      imagePreview.value = result
      form.accountCoverPhotoBase64 = result.split(',')[1] // 移除data:image/...;base64,前綴
    }
    reader.readAsDataURL(file)
  } else {
    imagePreview.value = ''
    form.accountCoverPhotoBase64 = undefined
  }
}

// 處理提交
async function handleSubmit() {
  if (!isFormValid.value) {
    return
  }

  try {
    userStore.clearError()
    await userStore.applySales(form)
    emit('success')
  } catch (error) {
    console.error('申請銷售功能失敗:', error)
  }
}

// 組件掛載時清除錯誤
onMounted(() => {
  userStore.clearError()
})
</script>