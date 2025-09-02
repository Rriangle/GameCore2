<template>
  <div class="shopping-cart">
    <!-- 購物車按鈕 -->
    <button
      @click="toggleCart"
      class="cart-button"
      :class="{ 'has-items': cartItemCount > 0, 'pulse': showPulse }"
      aria-label="購物車"
    >
      <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m8 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01"></path>
      </svg>
      <span v-if="cartItemCount > 0" class="cart-badge" :class="{ 'bounce': showBadgeBounce }">{{ cartItemCount }}</span>
    </button>

    <!-- 購物車側邊欄 -->
    <Transition name="slide">
      <div v-if="isOpen" class="cart-sidebar">
        <div class="cart-header">
          <h3 class="cart-title">購物車</h3>
          <button @click="toggleCart" class="close-button" aria-label="關閉">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
        
        <!-- 載入狀態 -->
        <div v-if="loading" class="loading-state">
          <div class="loading-spinner"></div>
          <p class="text-gray-500">載入中...</p>
        </div>

        <div v-else class="cart-content">
          <!-- 空購物車狀態 -->
          <div v-if="cartItems.length === 0" class="empty-cart">
            <svg class="w-16 h-16 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m8 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01"></path>
            </svg>
            <p class="text-gray-500">購物車是空的</p>
            <button @click="goToStore" class="btn-primary mt-4 hover:scale-105 transition-transform">
              去購物
            </button>
          </div>

          <!-- 購物車商品列表 -->
          <div v-else class="cart-items">
            <TransitionGroup name="cart-item" tag="div">
              <div
                v-for="item in cartItems"
                :key="item.productId"
                class="cart-item"
                :class="{ 'item-removing': item.removing }"
              >
                <img :src="item.product.imageUrl" :alt="item.product.name" class="item-image" @error="handleImageError" />
                <div class="item-details">
                  <h4 class="item-name">{{ item.product.name }}</h4>
                  <p class="item-price">NT$ {{ formatPrice(item.price) }}</p>
                  <div class="quantity-controls">
                    <button
                      @click="decreaseQuantity(item.productId)"
                      class="quantity-btn"
                      :disabled="item.quantity <= 1 || item.updating"
                      :class="{ 'disabled': item.quantity <= 1 || item.updating }"
                      aria-label="減少數量"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4"></path>
                      </svg>
                    </button>
                    <span class="quantity" :class="{ 'updating': item.updating }">{{ item.quantity }}</span>
                    <button
                      @click="increaseQuantity(item.productId)"
                      class="quantity-btn"
                      :disabled="item.quantity >= 99 || item.updating"
                      :class="{ 'disabled': item.quantity >= 99 || item.updating }"
                      aria-label="增加數量"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
                      </svg>
                    </button>
                  </div>
                </div>
                <button
                  @click="removeItem(item.productId)"
                  class="remove-btn"
                  :disabled="item.removing"
                  :class="{ 'removing': item.removing }"
                  aria-label="移除商品"
                >
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                  </svg>
                </button>
              </div>
            </TransitionGroup>
          </div>
        </div>

        <!-- 購物車底部 -->
        <div v-if="cartItems.length > 0" class="cart-footer">
          <div class="cart-summary">
            <div class="summary-row">
              <span>小計:</span>
              <span>NT$ {{ formatPrice(subtotal) }}</span>
            </div>
            <div class="summary-row">
              <span>運費:</span>
              <span>{{ shippingFee === 0 ? '免費' : `NT$ ${formatPrice(shippingFee)}` }}</span>
            </div>
            <div v-if="discount > 0" class="summary-row discount">
              <span>折扣:</span>
              <span>-NT$ {{ formatPrice(discount) }}</span>
            </div>
            <div v-if="discount > 0" class="summary-row savings">
              <span>節省:</span>
              <span class="text-green-600 font-semibold">NT$ {{ formatPrice(discount) }}</span>
            </div>
            <div class="summary-row total">
              <span>總計:</span>
              <span class="text-xl font-bold">NT$ {{ formatPrice(total) }}</span>
            </div>
          </div>
          
          <!-- 錯誤訊息 -->
          <div v-if="error" class="error-message">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <span>{{ error }}</span>
          </div>

          <div class="cart-actions">
            <button @click="clearCart" class="btn-secondary" :disabled="clearing">
              {{ clearing ? '清空中...' : '清空購物車' }}
            </button>
            <button @click="checkout" class="btn-primary flex-1" :disabled="isCheckingOut">
              {{ isCheckingOut ? '處理中...' : '結帳' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- 背景遮罩 -->
    <Transition name="fade">
      <div v-if="isOpen" @click="toggleCart" class="cart-backdrop"></div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/cart.store'
import { storeService } from '@/services/store.service'
import type { CartItem } from '@/types/store'

const router = useRouter()
const cartStore = useCartStore()

// 響應式狀態
const isOpen = ref(false)
const isCheckingOut = ref(false)
const loading = ref(false)
const error = ref<string | null>(null)
const clearing = ref(false)

// 計算屬性
const cartItems = computed(() => cartStore.items)
const cartItemCount = computed(() => cartStore.itemCount)
const subtotal = computed(() => cartStore.subtotal)
const shippingFee = computed(() => subtotal.value >= 1000 ? 0 : 100)
const discount = computed(() => {
  // 計算折扣（例如：滿 2000 打 9 折）
  if (subtotal.value >= 2000) {
    return subtotal.value * 0.1
  }
  return 0
})
const total = computed(() => subtotal.value + shippingFee.value - discount.value)
const savings = computed(() => discount.value)

// 微互動狀態
const showPulse = ref(false)
const showBadgeBounce = ref(false)

// 方法
const toggleCart = () => {
  isOpen.value = !isOpen.value
}

const goToStore = () => {
  isOpen.value = false
  router.push('/store')
}

const increaseQuantity = (productId: string) => {
  cartStore.increaseQuantity(productId)
}

const decreaseQuantity = (productId: string) => {
  cartStore.decreaseQuantity(productId)
}

const removeItem = (productId: string) => {
  cartStore.removeItem(productId)
}

const formatPrice = (price: number) => {
  return price.toLocaleString()
}

const checkout = async () => {
  if (cartItems.value.length === 0) return

  isCheckingOut.value = true
  try
  {
    // 創建訂單
    const orderData = {
      items: cartItems.value.map(item => ({
        productId: item.productId,
        quantity: item.quantity,
        price: item.price
      })),
      total: total.value,
      shippingFee: shippingFee.value
    }

    const response = await storeService.createOrder(orderData)

    if (response.success) {
      // 清空購物車
      cartStore.clearCart()
      
      // 關閉購物車
      isOpen.value = false
      
      // 跳轉到訂單詳情頁
      router.push(`/orders/${response.data?.orderId}`)
    } else
    {
      alert('結帳失敗: ' + response.message)
    }
  }
  catch (error) {
    console.error('結帳錯誤:', error)
    alert('結帳失敗，請稍後再試')
  } finally
  {
    isCheckingOut.value = false
  }
}

const clearCart = async () => {
  if (cartItems.value.length === 0) return

  clearing.value = true
  try {
    await cartStore.clearCart()
    isOpen.value = false
    error.value = null
  } catch (err) {
    console.error('清空購物車失敗:', err)
    error.value = '清空購物車失敗，請稍後再試'
  } finally {
    clearing.value = false
  }
}

const handleImageError = (event: Event) => {
  const target = event.target as HTMLImageElement;
  target.src = '/images/placeholder.jpg'; // 替換為預設圖片路徑
};
</script>

<style scoped>
.shopping-cart {
  position: relative;
}

.cart-button {
  position: relative;
  padding: 0.5rem;
  background: none;
  border: none;
  color: #6b7280;
  cursor: pointer;
  transition: color 0.2s;
}

.cart-button:hover {
  color: #374151;
}

.cart-button.has-items {
  color: #3b82f6;
}

.cart-button.pulse {
  animation: pulse 1.5s infinite;
}

.cart-badge {
  position: absolute;
  top: -0.25rem;
  right: -0.25rem;
  background: #ef4444;
  color: white;
  border-radius: 50%;
  width: 1.25rem;
  height: 1.25rem;
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
}

.cart-badge.bounce {
  animation: bounce 0.5s infinite;
}

.cart-sidebar {
  position: fixed;
  top: 0;
  right: 0;
  width: 100%;
  max-width: 24rem;
  height: 100vh;
  background: white;
  box-shadow: -4px 0 6px -1px rgba(0, 0, 0, 0.1);
  z-index: 50;
  display: flex;
  flex-direction: column;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.cart-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.close-button {
  padding: 0.5rem;
  background: none;
  border: none;
  color: #6b7280;
  cursor: pointer;
  transition: color 0.2s;
}

.close-button:hover {
  color: #374151;
}

.cart-content {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  text-align: center;
  color: #6b7280;
}

.loading-spinner {
  width: 3rem;
  height: 3rem;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

.empty-cart {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  text-align: center;
}

.cart-items {
  space-y: 1rem;
}

.cart-item {
  display: flex;
  align-items: center;
  padding: 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  background: #f9fafb;
  transition: all 0.2s;
}

.cart-item:hover {
  border-color: #d1d5db;
  background: white;
}

.item-image {
  width: 3rem;
  height: 3rem;
  object-fit: cover;
  border-radius: 0.375rem;
  margin-right: 1rem;
}

.item-details {
  flex: 1;
}

.item-name {
  font-weight: 500;
  color: #111827;
  margin-bottom: 0.25rem;
}

.item-price {
  color: #6b7280;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.quantity-btn {
  padding: 0.25rem;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s;
}

.quantity-btn:hover:not(:disabled) {
  background: #f3f4f6;
  border-color: #9ca3af;
}

.quantity-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.summary-row.discount {
  color: #059669;
  font-weight: 500;
}

.summary-row.savings {
  color: #dc2626;
  font-weight: 500;
  font-size: 0.875rem;
}

.quantity {
  min-width: 2rem;
  text-align: center;
  font-weight: 500;
}

.remove-btn {
  padding: 0.5rem;
  background: none;
  border: none;
  color: #ef4444;
  cursor: pointer;
  transition: color 0.2s;
}

.remove-btn:hover {
  color: #dc2626;
}

.cart-footer {
  padding: 1rem;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
}

.cart-summary {
  margin-bottom: 1rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.summary-row.total {
  font-weight: 600;
  font-size: 1.125rem;
  color: #111827;
  border-top: 1px solid #e5e7eb;
  padding-top: 0.5rem;
  margin-top: 0.5rem;
}

.checkout-btn {
  width: 100%;
  padding: 0.75rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.checkout-btn:hover:not(:disabled) {
  background: #2563eb;
}

.checkout-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.loading-spinner {
  width: 1rem;
  height: 1rem;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.cart-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  z-index: 40;
}

.cart-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  z-index: 40;
}

.error-message {
  display: flex;
  align-items: center;
  color: #dc2626;
  font-size: 0.875rem;
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #fef3f2;
  border: 1px solid #fcd3c7;
  border-radius: 0.375rem;
}

.error-message svg {
  margin-right: 0.5rem;
}

.cart-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 1rem;
}

.btn-secondary {
  flex: 1;
  padding: 0.75rem;
  background: #e5e7eb;
  color: #374151;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-secondary:hover:not(:disabled) {
  background: #d1d5db;
}

.btn-secondary:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.btn-primary {
  flex: 1;
  padding: 0.75rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn-primary:hover:not(:disabled) {
  background: #2563eb;
}

.btn-primary:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

/* 動畫 */
.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s ease;
}

.slide-enter-from,
.slide-leave-to {
  transform: translateX(100%);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.cart-item-enter-active,
.cart-item-leave-active {
  transition: all 0.2s ease;
}

.cart-item-enter-from,
.cart-item-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

.cart-item-move {
  transition: transform 0.3s ease;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@keyframes pulse {
  0% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.1);
    opacity: 0.7;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-5px);
  }
}

/* 響應式設計 */
@media (max-width: 640px) {
  .cart-sidebar {
    max-width: 100%;
  }
  
  .cart-item {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .item-image {
    margin-right: 0;
    margin-bottom: 0.75rem;
  }
  
  .quantity-controls {
    align-self: flex-end;
  }
}
</style> 