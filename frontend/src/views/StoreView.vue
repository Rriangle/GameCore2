<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <h1 class="text-3xl font-bold text-gray-900">Official Store</h1>
          <div class="flex items-center space-x-4">
            <div class="relative">
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search products..."
                class="w-64 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <div class="absolute inset-y-0 right-0 pr-3 flex items-center">
                <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
            </div>
            <button
              @click="showCart = true"
              class="relative p-2 text-gray-600 hover:text-gray-900"
            >
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 01-2 2H9a2 2 0 01-2-2v-6m6 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01" />
              </svg>
              <span
                v-if="cartItems.length > 0"
                class="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center"
              >
                {{ cartItems.length }}
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Categories -->
    <div class="bg-white border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex space-x-8 py-4">
          <button
            v-for="category in categories"
            :key="category.id"
            @click="selectedCategory = category.id"
            :class="[
              'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
              selectedCategory === category.id
                ? 'bg-blue-100 text-blue-700'
                : 'text-gray-600 hover:text-gray-900 hover:bg-gray-100'
            ]"
          >
            {{ category.name }}
          </button>
        </div>
      </div>
    </div>

    <!-- Products Grid -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>

      <div v-else-if="filteredProducts.length === 0" class="text-center py-12">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">No products found</h3>
        <p class="mt-1 text-sm text-gray-500">Try adjusting your search or filter criteria.</p>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <div
          v-for="product in filteredProducts"
          :key="product.id"
          class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow"
        >
          <div class="aspect-w-1 aspect-h-1 w-full">
            <img
              :src="product.imageUrl || '/images/placeholder.jpg'"
              :alt="product.name"
              class="w-full h-48 object-cover"
            />
          </div>
          <div class="p-4">
            <h3 class="text-lg font-medium text-gray-900 mb-2">{{ product.name }}</h3>
            <p class="text-sm text-gray-600 mb-3 line-clamp-2">{{ product.description }}</p>
            <div class="flex items-center justify-between mb-3">
              <span class="text-2xl font-bold text-blue-600">${{ product.price }}</span>
              <span class="text-sm text-gray-500">{{ product.stockQuantity }} in stock</span>
            </div>
            <button
              @click="addToCart(product)"
              :disabled="product.stockQuantity === 0"
              :class="[
                'w-full px-4 py-2 rounded-lg font-medium transition-colors',
                product.stockQuantity > 0
                  ? 'bg-blue-600 text-white hover:bg-blue-700'
                  : 'bg-gray-300 text-gray-500 cursor-not-allowed'
              ]"
            >
              {{ product.stockQuantity > 0 ? 'Add to Cart' : 'Out of Stock' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Shopping Cart Modal -->
    <div
      v-if="showCart"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click="showCart = false"
    >
      <div
        class="bg-white rounded-lg shadow-xl max-w-md w-full mx-4 max-h-[80vh] overflow-y-auto"
        @click.stop
      >
        <div class="p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-bold text-gray-900">Shopping Cart</h2>
            <button
              @click="showCart = false"
              class="text-gray-400 hover:text-gray-600"
            >
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <div v-if="cartItems.length === 0" class="text-center py-8">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">Your cart is empty</h3>
            <p class="mt-1 text-sm text-gray-500">Start shopping to add items to your cart.</p>
          </div>

          <div v-else>
            <div class="space-y-4 mb-6">
              <div
                v-for="item in cartItems"
                :key="item.id"
                class="flex items-center space-x-4"
              >
                <img
                  :src="item.imageUrl || '/images/placeholder.jpg'"
                  :alt="item.name"
                  class="w-16 h-16 object-cover rounded"
                />
                <div class="flex-1">
                  <h4 class="text-sm font-medium text-gray-900">{{ item.name }}</h4>
                  <p class="text-sm text-gray-500">${{ item.price }} x {{ item.quantity }}</p>
                </div>
                <div class="flex items-center space-x-2">
                  <button
                    @click="updateQuantity(item.id, item.quantity - 1)"
                    :disabled="item.quantity <= 1"
                    class="w-6 h-6 rounded-full border border-gray-300 flex items-center justify-center text-gray-600 hover:bg-gray-100 disabled:opacity-50"
                  >
                    -
                  </button>
                  <span class="text-sm text-gray-900">{{ item.quantity }}</span>
                  <button
                    @click="updateQuantity(item.id, item.quantity + 1)"
                    class="w-6 h-6 rounded-full border border-gray-300 flex items-center justify-center text-gray-600 hover:bg-gray-100"
                  >
                    +
                  </button>
                </div>
                <button
                  @click="removeFromCart(item.id)"
                  class="text-red-500 hover:text-red-700"
                >
                  <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </div>
            </div>

            <div class="border-t pt-4">
              <div class="flex justify-between text-sm text-gray-600 mb-2">
                <span>Subtotal:</span>
                <span>${{ cartSubtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm text-gray-600 mb-2">
                <span>Tax (10%):</span>
                <span>${{ cartTax.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm text-gray-600 mb-2">
                <span>Shipping:</span>
                <span>{{ cartSubtotal > 100 ? 'Free' : '$10.00' }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold text-gray-900 mb-4">
                <span>Total:</span>
                <span>${{ cartTotal.toFixed(2) }}</span>
              </div>
              <button
                @click="checkout"
                class="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-medium hover:bg-blue-700 transition-colors"
              >
                Proceed to Checkout
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'

interface Product {
  id: number
  name: string
  description: string
  price: number
  stockQuantity: number
  categoryId: number
  categoryName: string
  imageUrl: string
  isActive: boolean
  isOfficialStore: boolean
  createdAt: string
  updatedAt?: string
}

interface Category {
  id: number
  name: string
  description: string
  iconUrl: string
  isActive: boolean
  sortOrder: number
  productCount: number
  createdAt: string
}

interface CartItem {
  id: number
  name: string
  price: number
  quantity: number
  imageUrl: string
}

const router = useRouter()

// Reactive data
const products = ref<Product[]>([])
const categories = ref<Category[]>([])
const loading = ref(true)
const searchQuery = ref('')
const selectedCategory = ref<number | null>(null)
const showCart = ref(false)
const cartItems = ref<CartItem[]>([])

// Computed properties
const filteredProducts = computed(() => {
  let filtered = products.value

  if (selectedCategory.value) {
    filtered = filtered.filter(p => p.categoryId === selectedCategory.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(p => 
      p.name.toLowerCase().includes(query) || 
      p.description.toLowerCase().includes(query)
    )
  }

  return filtered
})

const cartSubtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
})

const cartTax = computed(() => {
  return cartSubtotal.value * 0.1
})

const cartTotal = computed(() => {
  const shipping = cartSubtotal.value > 100 ? 0 : 10
  return cartSubtotal.value + cartTax.value + shipping
})

// Methods
const fetchProducts = async () => {
  try {
    loading.value = true
    // In a real app, this would be an API call
    // const response = await api.get('/api/products')
    // products.value = response.data
    
    // Mock data for now
    products.value = [
      {
        id: 1,
        name: "Pro Gaming Mouse",
        description: "High-precision gaming mouse with customizable DPI settings and RGB lighting",
        price: 79.99,
        stockQuantity: 100,
        categoryId: 1,
        categoryName: "Gaming Hardware",
        imageUrl: "/images/mouse.jpg",
        isActive: true,
        isOfficialStore: true,
        createdAt: new Date().toISOString()
      },
      {
        id: 2,
        name: "Mechanical Keyboard",
        description: "RGB mechanical keyboard with Cherry MX switches and customizable backlighting",
        price: 149.99,
        stockQuantity: 75,
        categoryId: 1,
        categoryName: "Gaming Hardware",
        imageUrl: "/images/keyboard.jpg",
        isActive: true,
        isOfficialStore: true,
        createdAt: new Date().toISOString()
      },
      {
        id: 3,
        name: "Gaming Headset",
        description: "7.1 surround sound gaming headset with noise cancellation",
        price: 89.99,
        stockQuantity: 120,
        categoryId: 1,
        categoryName: "Gaming Hardware",
        imageUrl: "/images/headset.jpg",
        isActive: true,
        isOfficialStore: true,
        createdAt: new Date().toISOString()
      }
    ]
  } catch (error) {
    console.error('Error fetching products:', error)
  } finally {
    loading.value = false
  }
}

const fetchCategories = async () => {
  try {
    // In a real app, this would be an API call
    // const response = await api.get('/api/products/categories')
    // categories.value = response.data
    
    // Mock data for now
    categories.value = [
      {
        id: 1,
        name: "Gaming Hardware",
        description: "Gaming peripherals and hardware",
        iconUrl: "/icons/hardware.png",
        isActive: true,
        sortOrder: 1,
        productCount: 3,
        createdAt: new Date().toISOString()
      },
      {
        id: 2,
        name: "Gaming Apparel",
        description: "Gaming-themed clothing and accessories",
        iconUrl: "/icons/apparel.png",
        isActive: true,
        sortOrder: 2,
        productCount: 0,
        createdAt: new Date().toISOString()
      }
    ]
  } catch (error) {
    console.error('Error fetching categories:', error)
  }
}

const addToCart = (product: Product) => {
  const existingItem = cartItems.value.find(item => item.id === product.id)
  
  if (existingItem) {
    existingItem.quantity++
  } else {
    cartItems.value.push({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1,
      imageUrl: product.imageUrl
    })
  }
  
  showCart.value = true
}

const updateQuantity = (itemId: number, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeFromCart(itemId)
    return
  }
  
  const item = cartItems.value.find(item => item.id === itemId)
  if (item) {
    item.quantity = newQuantity
  }
}

const removeFromCart = (itemId: number) => {
  const index = cartItems.value.findIndex(item => item.id === itemId)
  if (index > -1) {
    cartItems.value.splice(index, 1)
  }
}

const checkout = () => {
  // In a real app, this would navigate to checkout page
  router.push('/checkout')
}

// Lifecycle
onMounted(() => {
  fetchProducts()
  fetchCategories()
})
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>