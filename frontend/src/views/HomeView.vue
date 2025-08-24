<template>
  <div class="home">
    <div class="text-center">
      <h1 class="text-4xl font-bold text-gray-900 mb-4">
        🎮 歡迎來到 GameCore
      </h1>
      <p class="text-xl text-gray-600 mb-8">
        整合遊戲熱度觀測、論壇社群、商城、玩家市場、寵物養成的綜合平台
      </p>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-3xl mb-2">📊</div>
          <h3 class="text-lg font-semibold mb-2">遊戲熱度</h3>
          <p class="text-gray-600">即時觀測遊戲熱度與排行榜</p>
        </div>
        
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-3xl mb-2">💬</div>
          <h3 class="text-lg font-semibold mb-2">論壇社群</h3>
          <p class="text-gray-600">遊戲討論與社群互動</p>
        </div>
        
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-3xl mb-2">🛒</div>
          <h3 class="text-lg font-semibold mb-2">商城市集</h3>
          <p class="text-gray-600">官方商城與玩家交易</p>
        </div>
        
        <div class="bg-white p-6 rounded-lg shadow-md">
          <div class="text-3xl mb-2">🐾</div>
          <h3 class="text-lg font-semibold mb-2">寵物養成</h3>
          <p class="text-gray-600">可愛史萊姆寵物系統</p>
        </div>
      </div>
      
      <div v-if="!userStore.isAuthenticated" class="space-x-4">
        <router-link 
          to="/register" 
          class="bg-indigo-600 text-white hover:bg-indigo-700 px-8 py-3 rounded-lg text-lg font-semibold inline-block transition-colors"
        >
          立即註冊
        </router-link>
        <router-link 
          to="/login" 
          class="border-2 border-indigo-600 text-indigo-600 hover:bg-indigo-600 hover:text-white px-8 py-3 rounded-lg text-lg font-semibold inline-block transition-colors"
        >
          會員登入
        </router-link>
      </div>
      
      <div v-else class="space-x-4">
        <router-link 
          to="/profile" 
          class="bg-indigo-600 text-white hover:bg-indigo-700 px-8 py-3 rounded-lg text-lg font-semibold inline-block transition-colors"
        >
          查看個人資料
        </router-link>
        <div class="mt-4 text-gray-600">
          歡迎回來，{{ userStore.userName }}！您有 {{ userStore.userPoints }} 點數
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()
</script>
