<template>
  <div class="wallet-balance">
    <div class="flex items-center space-x-2 bg-gradient-to-r from-yellow-400 to-orange-500 text-white px-4 py-2 rounded-lg shadow-md">
      <div class="text-2xl">💰</div>
      <div class="flex flex-col">
        <span class="text-sm font-medium">點數餘額</span>
        <span class="text-lg font-bold">{{ formatBalance(balance) }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();

const balance = computed(() => authStore.userBalance);

const formatBalance = (value: number): string => {
  return value.toLocaleString('zh-TW', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2
  });
};
</script>

<style scoped>
.wallet-balance {
  transition: all 0.3s ease;
}

.wallet-balance:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}
</style>
