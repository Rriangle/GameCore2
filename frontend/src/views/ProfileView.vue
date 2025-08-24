<template>
  <div class="min-h-screen bg-gray-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- 頁面標題 -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900">個人資料</h1>
        <p class="mt-2 text-gray-600">管理您的個人資訊和帳戶設定</p>
      </div>

      <!-- 載入中狀態 -->
      <div v-if="userStore.isLoading" class="flex justify-center items-center h-64">
        <div class="animate-spin rounded-full h-32 w-32 border-b-2 border-indigo-600"></div>
      </div>

      <!-- 主要內容 -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- 左側：基本資訊卡片 -->
        <div class="lg:col-span-1">
          <div class="bg-white shadow rounded-lg p-6">
            <div class="text-center">
              <!-- 頭像佔位符 -->
              <div class="mx-auto h-24 w-24 bg-gray-300 rounded-full flex items-center justify-center">
                <svg class="h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </div>
              
              <h2 class="mt-4 text-xl font-semibold text-gray-900">
                {{ userStore.userName }}
              </h2>
              <p class="text-gray-600">@{{ userStore.userAccount }}</p>
              
              <!-- 狀態標籤 -->
              <div class="mt-4 flex flex-wrap justify-center gap-2">
                <span v-if="userStore.isActive" 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                  帳戶正常
                </span>
                <span v-if="userStore.canShopping" 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                  可購物
                </span>
                <span v-if="userStore.canMessage" 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-purple-100 text-purple-800">
                  可留言
                </span>
                <span v-if="userStore.canSales" 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
                  可銷售
                </span>
              </div>
            </div>

            <!-- 錢包資訊 -->
            <div class="mt-6 border-t pt-6">
              <h3 class="text-lg font-medium text-gray-900 mb-4">錢包資訊</h3>
              <div class="space-y-2">
                <div class="flex justify-between">
                  <span class="text-gray-600">點數餘額</span>
                  <span class="font-semibold text-indigo-600">{{ userStore.userPoints }} 點</span>
                </div>
                <div v-if="userStore.hasSalesProfile" class="flex justify-between">
                  <span class="text-gray-600">銷售收入</span>
                  <span class="font-semibold text-green-600">{{ userStore.salesWallet }} 元</span>
                </div>
                <div v-if="userStore.currentUser?.wallet?.couponNumber" class="flex justify-between">
                  <span class="text-gray-600">優惠券</span>
                  <span class="font-semibold text-orange-600">{{ userStore.currentUser.wallet.couponNumber }}</span>
                </div>
              </div>
            </div>

            <!-- 快速操作 -->
            <div class="mt-6 border-t pt-6">
              <h3 class="text-lg font-medium text-gray-900 mb-4">快速操作</h3>
              <div class="space-y-2">
                <button 
                  @click="showIntroduceModal = true"
                  class="w-full text-left px-3 py-2 text-sm text-indigo-600 hover:bg-indigo-50 rounded-md transition-colors">
                  {{ userStore.hasIntroduce ? '編輯個人介紹' : '新增個人介紹' }}
                </button>
                <button 
                  v-if="!userStore.hasSalesProfile"
                  @click="showSalesModal = true"
                  class="w-full text-left px-3 py-2 text-sm text-green-600 hover:bg-green-50 rounded-md transition-colors">
                  申請銷售功能
                </button>
                <button 
                  @click="handleLogout"
                  class="w-full text-left px-3 py-2 text-sm text-red-600 hover:bg-red-50 rounded-md transition-colors">
                  登出
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- 右側：詳細資訊 -->
        <div class="lg:col-span-2">
          <div class="space-y-6">
            <!-- 個人介紹卡片 -->
            <div class="bg-white shadow rounded-lg p-6">
              <h3 class="text-lg font-medium text-gray-900 mb-4">個人介紹</h3>
              
              <div v-if="userStore.hasIntroduce && userStore.currentUser?.userIntroduce" 
                   class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700">暱稱</label>
                    <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.userIntroduce.userNickName }}</p>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700">性別</label>
                    <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.userIntroduce.gender === 'M' ? '男' : '女' }}</p>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700">電話</label>
                    <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.userIntroduce.cellphone }}</p>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700">電子郵件</label>
                    <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.userIntroduce.email }}</p>
                  </div>
                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700">地址</label>
                    <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.userIntroduce.address }}</p>
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-gray-700">出生日期</label>
                    <p class="mt-1 text-sm text-gray-900">{{ formatDate(userStore.currentUser.userIntroduce.dateOfBirth) }}</p>
                  </div>
                </div>
                
                <div v-if="userStore.currentUser.userIntroduce.userIntroduceText">
                  <label class="block text-sm font-medium text-gray-700">自我介紹</label>
                  <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.userIntroduce.userIntroduceText }}</p>
                </div>
              </div>
              
              <div v-else class="text-center py-8">
                <p class="text-gray-500 mb-4">尚未填寫個人介紹</p>
                <button 
                  @click="showIntroduceModal = true"
                  class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700">
                  立即填寫
                </button>
              </div>
            </div>

            <!-- 銷售資訊卡片 -->
            <div v-if="userStore.hasSalesProfile && userStore.currentUser?.memberSalesProfile" 
                 class="bg-white shadow rounded-lg p-6">
              <h3 class="text-lg font-medium text-gray-900 mb-4">銷售資訊</h3>
              
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700">銀行代號</label>
                  <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.memberSalesProfile.bankCode }}</p>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700">銀行帳號</label>
                  <p class="mt-1 text-sm text-gray-900">{{ userStore.currentUser.memberSalesProfile.bankAccountNumber }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 個人介紹編輯模態框 -->
    <IntroduceModal 
      v-if="showIntroduceModal" 
      :current-data="userStore.currentUser?.userIntroduce"
      @close="showIntroduceModal = false"
      @success="handleIntroduceSuccess" />

    <!-- 銷售申請模態框 -->
    <SalesModal 
      v-if="showSalesModal" 
      @close="showSalesModal = false"
      @success="handleSalesSuccess" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import IntroduceModal from '@/components/IntroduceModal.vue'
import SalesModal from '@/components/SalesModal.vue'

const router = useRouter()
const userStore = useUserStore()

// 模態框狀態
const showIntroduceModal = ref(false)
const showSalesModal = ref(false)

// 格式化日期
function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('zh-TW')
}

// 處理登出
function handleLogout() {
  userStore.logout()
  router.push('/login')
}

// 處理個人介紹更新成功
function handleIntroduceSuccess() {
  showIntroduceModal.value = false
}

// 處理銷售申請成功
function handleSalesSuccess() {
  showSalesModal.value = false
}

// 組件掛載時取得使用者資料
onMounted(async () => {
  if (userStore.isAuthenticated) {
    try {
      await userStore.fetchCurrentUser()
    } catch (error) {
      console.error('取得使用者資料失敗:', error)
    }
  } else {
    router.push('/login')
  }
})
</script>