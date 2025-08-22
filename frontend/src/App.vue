<template>
  <div id="app">
    <nav class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <router-link to="/" class="text-xl font-bold text-primary-600">
              🎮 GameCore
            </router-link>
          </div>
          
          <div class="flex items-center space-x-4">
            <router-link to="/" class="text-gray-700 hover:text-primary-600">首頁</router-link>
            <router-link to="/about" class="text-gray-700 hover:text-primary-600">關於</router-link>
            
            <!-- 點數餘額顯示 -->
            <WalletBalance v-if="authStore.isAuthenticated" />
            
            <!-- 用戶選單 -->
            <div v-if="authStore.isAuthenticated" class="relative">
              <div class="flex items-center space-x-2">
                <span class="text-sm text-gray-700">歡迎，{{ authStore.user?.username }}</span>
                <button
                  @click="handleLogout"
                  class="text-sm text-gray-500 hover:text-red-600 transition-colors"
                >
                  登出
                </button>
              </div>
            </div>
            
            <!-- 登入/註冊按鈕 -->
            <div v-else class="flex items-center space-x-2">
              <router-link
                to="/login"
                class="text-sm text-gray-700 hover:text-primary-600 transition-colors"
              >
                登入
              </router-link>
              <span class="text-gray-400">|</span>
              <router-link
                to="/register"
                class="text-sm text-primary-600 hover:text-primary-700 transition-colors"
              >
                註冊
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import WalletBalance from '@/components/WalletBalance.vue';

const router = useRouter();
const authStore = useAuthStore();

onMounted(() => {
  // 初始化認證狀態
  authStore.initializeAuth();
});

const handleLogout = () => {
  authStore.logout();
  router.push('/login');
};
</script>
