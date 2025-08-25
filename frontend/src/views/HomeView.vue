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
        <div class="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow cursor-pointer" @click="navigateTo('/store')">
          <div class="text-3xl mb-2">🛒</div>
          <h3 class="text-lg font-semibold mb-2">官方商城</h3>
          <p class="text-gray-600">官方遊戲商品與配件</p>
        </div>
        
        <div class="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow cursor-pointer" @click="navigateTo('/player-market')">
          <div class="text-3xl mb-2">🏪</div>
          <h3 class="text-lg font-semibold mb-2">玩家市場</h3>
          <p class="text-gray-600">玩家之間的二手交易</p>
        </div>
        
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
      </div>
      
      <div class="space-x-4">
        <button @click="navigateTo('/store')" class="btn-primary">瀏覽商城</button>
        <button @click="navigateTo('/player-market')" class="btn-secondary">玩家市場</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const navigateTo = (path: string) => {
  router.push(path)
}
</script>

<style scoped>
.btn-primary {
  @apply bg-blue-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-700 transition-colors;
}

.btn-secondary {
  @apply bg-gray-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-gray-700 transition-colors;
}
</style>
