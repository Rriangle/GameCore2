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
            
            <!-- 用戶選單 -->
            <div v-if="userStore.isAuthenticated" class="flex items-center space-x-4">
              <router-link 
                to="/profile" 
                class="text-gray-700 hover:text-indigo-600 px-3 py-2 text-sm font-medium"
                active-class="text-indigo-600"
              >
                個人資料
              </router-link>
              <span class="text-sm text-gray-600">{{ userStore.userPoints }} 點</span>
              <span class="text-sm text-gray-600">歡迎，{{ userStore.userName }}</span>
              <button
                @click="handleLogout"
                class="text-sm text-gray-500 hover:text-red-600 transition-colors"
              >
                登出
              </button>
            </div>
            
            <!-- 登入/註冊按鈕 -->
            <div v-else class="flex items-center space-x-2">
              <router-link
                to="/login"
                class="text-sm text-gray-700 hover:text-indigo-600 transition-colors"
              >
                登入
              </router-link>
              <span class="text-gray-400">|</span>
              <router-link
                to="/register"
                class="text-sm text-indigo-600 hover:text-indigo-700 transition-colors"
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
import { useRouter } from 'vue-router';
import { useUserStore } from '@/stores/user';

const router = useRouter();
const userStore = useUserStore();

const handleLogout = () => {
  userStore.logout();
  router.push('/login');
};
</script>
