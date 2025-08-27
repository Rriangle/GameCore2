import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { CartItem, Product } from '@/types/store'

export const useCartStore = defineStore('cart', () => {
  // 狀態
  const items = ref<CartItem[]>([])
  const isOpen = ref(false)

  // 從 localStorage 初始化
  const initializeCart = () => {
    const savedCart = localStorage.getItem('gamecore-cart')
    if (savedCart) {
      try {
        items.value = JSON.parse(savedCart)
      } catch (error) {
        console.error('Failed to parse cart from localStorage:', error)
        items.value = []
      }
    }
  }

  // 儲存到 localStorage
  const saveToStorage = () => {
    localStorage.setItem('gamecore-cart', JSON.stringify(items.value))
  }

  // 計算屬性
  const itemCount = computed(() => {
    return items.value.reduce((total, item) => total + item.quantity, 0)
  })

  const subtotal = computed(() => {
    return items.value.reduce((total, item) => total + (item.price * item.quantity), 0)
  })

  const isEmpty = computed(() => items.value.length === 0)

  // 方法
  const addItem = (product: Product, quantity: number = 1) => {
    const existingItem = items.value.find(item => item.productId === product.id)
    
    if (existingItem) {
      existingItem.quantity += quantity
      // 限制最大數量
      if (existingItem.quantity > 99) {
        existingItem.quantity = 99
      }
    } else {
      items.value.push({
        id: product.id,
        productId: product.id,
        product: product,
        quantity: Math.min(quantity, 99),
        price: product.price,
        addedAt: new Date().toISOString()
      })
    }
    
    saveToStorage()
  }

  const removeItem = (productId: string) => {
    const index = items.value.findIndex(item => item.productId === productId)
    if (index > -1) {
      items.value.splice(index, 1)
      saveToStorage()
    }
  }

  const updateQuantity = (productId: string, quantity: number) => {
    const item = items.value.find(item => item.productId === productId)
    if (item) {
      if (quantity <= 0) {
        removeItem(productId)
      } else {
        item.quantity = Math.min(quantity, 99)
        saveToStorage()
      }
    }
  }

  const increaseQuantity = (productId: string) => {
    const item = items.value.find(item => item.productId === productId)
    if (item && item.quantity < 99) {
      item.quantity++
      saveToStorage()
    }
  }

  const decreaseQuantity = (productId: string) => {
    const item = items.value.find(item => item.productId === productId)
    if (item && item.quantity > 1) {
      item.quantity--
      saveToStorage()
    }
  }

  const clearCart = () => {
    items.value = []
    saveToStorage()
  }

  const toggleCart = () => {
    isOpen.value = !isOpen.value
  }

  const closeCart = () => {
    isOpen.value = false
  }

  const openCart = () => {
    isOpen.value = true
  }

  // 檢查商品是否在購物車中
  const isInCart = (productId: string) => {
    return items.value.some(item => item.productId === productId)
  }

  // 獲取商品在購物車中的數量
  const getItemQuantity = (productId: string) => {
    const item = items.value.find(item => item.productId === productId)
    return item ? item.quantity : 0
  }

  // 初始化
  initializeCart()

  return {
    // 狀態
    items,
    isOpen,
    
    // 計算屬性
    itemCount,
    subtotal,
    isEmpty,
    
    // 方法
    addItem,
    removeItem,
    updateQuantity,
    increaseQuantity,
    decreaseQuantity,
    clearCart,
    toggleCart,
    closeCart,
    openCart,
    isInCart,
    getItemQuantity
  }
}) 