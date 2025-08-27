<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow-sm border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <h1 class="text-3xl font-bold text-gray-900">Player Market</h1>
          <div class="flex items-center space-x-4">
            <button
              @click="showCreateListing = true"
              class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
            >
              Create Listing
            </button>
            <button
              @click="showMyListings = true"
              class="text-gray-600 hover:text-gray-900 px-4 py-2 rounded-lg hover:bg-gray-100 transition-colors"
            >
              My Listings
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white border-b">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center space-x-6 py-4">
          <div class="flex items-center space-x-4">
            <label class="text-sm font-medium text-gray-700">Price Range:</label>
            <input
              v-model.number="filters.minPrice"
              type="number"
              placeholder="Min"
              class="w-20 px-3 py-1 border border-gray-300 rounded-md text-sm"
            />
            <span class="text-gray-500">-</span>
            <input
              v-model.number="filters.maxPrice"
              type="number"
              placeholder="Max"
              class="w-20 px-3 py-1 border border-gray-300 rounded-md text-sm"
            />
          </div>
          <div class="flex items-center space-x-4">
            <label class="text-sm font-medium text-gray-700">Sort by:</label>
            <select
              v-model="filters.sortBy"
              class="px-3 py-1 border border-gray-300 rounded-md text-sm"
            >
              <option value="newest">Newest First</option>
              <option value="oldest">Oldest First</option>
              <option value="price-low">Price: Low to High</option>
              <option value="price-high">Price: High to Low</option>
            </select>
          </div>
          <div class="flex items-center space-x-2">
            <input
              v-model="filters.negotiableOnly"
              type="checkbox"
              id="negotiable"
              class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <label for="negotiable" class="text-sm text-gray-700">Negotiable Only</label>
          </div>
        </div>
      </div>
    </div>

    <!-- Listings Grid -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>

      <div v-else-if="filteredListings.length === 0" class="text-center py-12">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">No listings found</h3>
        <p class="mt-1 text-sm text-gray-500">Try adjusting your filters or create a new listing.</p>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <div
          v-for="listing in filteredListings"
          :key="listing.id"
          class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow"
        >
          <div class="aspect-w-1 aspect-h-1 w-full">
            <img
              :src="listing.imageUrl || '/images/placeholder.jpg'"
              :alt="listing.title"
              class="w-full h-48 object-cover"
            />
          </div>
          <div class="p-4">
            <div class="flex items-center justify-between mb-2">
              <span class="text-xs text-gray-500">{{ listing.sellerName }}</span>
              <span
                v-if="listing.isNegotiable"
                class="text-xs bg-green-100 text-green-800 px-2 py-1 rounded-full"
              >
                Negotiable
              </span>
            </div>
            <h3 class="text-lg font-medium text-gray-900 mb-2">{{ listing.title }}</h3>
            <p class="text-sm text-gray-600 mb-3 line-clamp-2">{{ listing.description }}</p>
            <div class="flex items-center justify-between mb-3">
              <span class="text-2xl font-bold text-blue-600">${{ listing.price }}</span>
              <span class="text-sm text-gray-500">{{ listing.availableQuantity }} available</span>
            </div>
            <div class="flex space-x-2">
              <button
                @click="viewListing(listing)"
                class="flex-1 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors text-sm font-medium"
              >
                View Details
              </button>
              <button
                @click="buyNow(listing)"
                :disabled="listing.availableQuantity === 0"
                :class="[
                  'flex-1 px-4 py-2 rounded-lg font-medium text-sm transition-colors',
                  listing.availableQuantity > 0
                    ? 'bg-blue-600 text-white hover:bg-blue-700'
                    : 'bg-gray-300 text-gray-500 cursor-not-allowed'
                ]"
              >
                Buy Now
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Listing Modal -->
    <div
      v-if="showCreateListing"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click="showCreateListing = false"
    >
      <div
        class="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto"
        @click.stop
      >
        <div class="p-6">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-xl font-bold text-gray-900">Create New Listing</h2>
            <button
              @click="showCreateListing = false"
              class="text-gray-400 hover:text-gray-600"
            >
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <form @submit.prevent="createListing" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
              <input
                v-model="newListing.title"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Enter listing title"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
              <textarea
                v-model="newListing.description"
                required
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Describe your item"
              ></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Price ($)</label>
                <input
                  v-model.number="newListing.price"
                  type="number"
                  required
                  min="0"
                  step="0.01"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="0.00"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
                <input
                  v-model.number="newListing.quantity"
                  type="number"
                  required
                  min="1"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="1"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Image URL</label>
              <input
                v-model="newListing.imageUrl"
                type="url"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="https://example.com/image.jpg"
              />
            </div>

            <div class="flex items-center space-x-2">
              <input
                v-model="newListing.isNegotiable"
                type="checkbox"
                id="negotiable-new"
                class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
              />
              <label for="negotiable-new" class="text-sm text-gray-700">Price is negotiable</label>
            </div>

            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="showCreateListing = false"
                class="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                Create Listing
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- My Listings Modal -->
    <div
      v-if="showMyListings"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      @click="showMyListings = false"
    >
      <div
        class="bg-white rounded-lg shadow-xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto"
        @click.stop
      >
        <div class="p-6">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-xl font-bold text-gray-900">My Listings</h2>
            <button
              @click="showMyListings = false"
              class="text-gray-400 hover:text-gray-600"
            >
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <div v-if="myListings.length === 0" class="text-center py-8">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No listings yet</h3>
            <p class="mt-1 text-sm text-gray-500">Create your first listing to start selling.</p>
          </div>

          <div v-else class="space-y-4">
            <div
              v-for="listing in myListings"
              :key="listing.id"
              class="border border-gray-200 rounded-lg p-4"
            >
              <div class="flex items-center justify-between">
                <div class="flex items-center space-x-4">
                  <img
                    :src="listing.imageUrl || '/images/placeholder.jpg'"
                    :alt="listing.title"
                    class="w-16 h-16 object-cover rounded"
                  />
                  <div>
                    <h4 class="text-lg font-medium text-gray-900">{{ listing.title }}</h4>
                    <p class="text-sm text-gray-500">{{ listing.description }}</p>
                    <div class="flex items-center space-x-4 mt-1">
                      <span class="text-lg font-bold text-blue-600">${{ listing.price }}</span>
                      <span class="text-sm text-gray-500">{{ listing.availableQuantity }} available</span>
                      <span
                        :class="[
                          'text-xs px-2 py-1 rounded-full',
                          listing.status === 'Active' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                        ]"
                      >
                        {{ listing.status }}
                      </span>
                    </div>
                  </div>
                </div>
                <div class="flex space-x-2">
                  <button
                    @click="editListing(listing)"
                    class="px-3 py-1 text-sm bg-blue-100 text-blue-700 rounded hover:bg-blue-200 transition-colors"
                  >
                    Edit
                  </button>
                  <button
                    @click="deleteListing(listing.id)"
                    class="px-3 py-1 text-sm bg-red-100 text-red-700 rounded hover:bg-red-200 transition-colors"
                  >
                    Delete
                  </button>
                </div>
              </div>
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

interface PlayerMarketListing {
  id: number
  sellerId: number
  sellerName: string
  title: string
  description: string
  price: number
  quantity: number
  availableQuantity: number
  status: string
  imageUrl: string
  isNegotiable: boolean
  createdAt: string
  updatedAt?: string
  expiresAt?: string
}

interface Filters {
  minPrice: number | null
  maxPrice: number | null
  sortBy: string
  negotiableOnly: boolean
}

interface NewListing {
  title: string
  description: string
  price: number
  quantity: number
  imageUrl: string
  isNegotiable: boolean
}

const router = useRouter()

// Reactive data
const listings = ref<PlayerMarketListing[]>([])
const myListings = ref<PlayerMarketListing[]>([])
const loading = ref(true)
const showCreateListing = ref(false)
const showMyListings = ref(false)
const filters = ref<Filters>({
  minPrice: null,
  maxPrice: null,
  sortBy: 'newest',
  negotiableOnly: false
})
const newListing = ref<NewListing>({
  title: '',
  description: '',
  price: 0,
  quantity: 1,
  imageUrl: '',
  isNegotiable: false
})

// Computed properties
const filteredListings = computed(() => {
  let filtered = listings.value

  if (filters.value.minPrice !== null) {
    filtered = filtered.filter(l => l.price >= filters.value.minPrice!)
  }

  if (filters.value.maxPrice !== null) {
    filtered = filtered.filter(l => l.price <= filters.value.maxPrice!)
  }

  if (filters.value.negotiableOnly) {
    filtered = filtered.filter(l => l.isNegotiable)
  }

  // Sort listings
  switch (filters.value.sortBy) {
    case 'newest':
      filtered = [...filtered].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      break
    case 'oldest':
      filtered = [...filtered].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime())
      break
    case 'price-low':
      filtered = [...filtered].sort((a, b) => a.price - b.price)
      break
    case 'price-high':
      filtered = [...filtered].sort((a, b) => b.price - a.price)
      break
  }

  return filtered
})

// Methods
const fetchListings = async () => {
  try {
    loading.value = true
    // In a real app, this would be an API call
    // const response = await api.get('/api/player-market/listings')
    // listings.value = response.data
    
    // Mock data for now
    listings.value = [
      {
        id: 1,
        sellerId: 1,
        sellerName: "GamerPro123",
        title: "Used Gaming Mouse",
        description: "Excellent condition gaming mouse, barely used. Perfect for FPS games.",
        price: 45.00,
        quantity: 1,
        availableQuantity: 1,
        status: "Active",
        imageUrl: "/images/used-mouse.jpg",
        isNegotiable: true,
        createdAt: new Date().toISOString()
      },
      {
        id: 2,
        sellerId: 2,
        sellerName: "TechEnthusiast",
        title: "Gaming Keyboard",
        description: "RGB mechanical keyboard with Cherry MX switches. Great condition.",
        price: 120.00,
        quantity: 1,
        availableQuantity: 1,
        status: "Active",
        imageUrl: "/images/used-keyboard.jpg",
        isNegotiable: false,
        createdAt: new Date().toISOString()
      },
      {
        id: 3,
        sellerId: 3,
        sellerName: "StreamerGamer",
        title: "Gaming Headset",
        description: "7.1 surround sound headset with microphone. Perfect for streaming.",
        price: 65.00,
        quantity: 1,
        availableQuantity: 1,
        status: "Active",
        imageUrl: "/images/used-headset.jpg",
        isNegotiable: true,
        createdAt: new Date().toISOString()
      }
    ]
  } catch (error) {
    console.error('Error fetching listings:', error)
  } finally {
    loading.value = false
  }
}

const fetchMyListings = async () => {
  try {
    // In a real app, this would be an API call
    // const response = await api.get('/api/player-market/listings/user/1')
    // myListings.value = response.data
    
    // Mock data for now
    myListings.value = [
      {
        id: 4,
        sellerId: 1,
        sellerName: "You",
        title: "My Gaming Chair",
        description: "Comfortable gaming chair with lumbar support.",
        price: 200.00,
        quantity: 1,
        availableQuantity: 1,
        status: "Active",
        imageUrl: "/images/gaming-chair.jpg",
        isNegotiable: true,
        createdAt: new Date().toISOString()
      }
    ]
  } catch (error) {
    console.error('Error fetching my listings:', error)
  }
}

const createListing = async () => {
  try {
    // In a real app, this would be an API call
    // const response = await api.post('/api/player-market/listings', newListing.value)
    // listings.value.unshift(response.data)
    
    // Mock creation
    const listing: PlayerMarketListing = {
      id: Date.now(),
      sellerId: 1,
      sellerName: "You",
      title: newListing.value.title,
      description: newListing.value.description,
      price: newListing.value.price,
      quantity: newListing.value.quantity,
      availableQuantity: newListing.value.quantity,
      status: "Active",
      imageUrl: newListing.value.imageUrl,
      isNegotiable: newListing.value.isNegotiable,
      createdAt: new Date().toISOString()
    }
    
    listings.value.unshift(listing)
    myListings.value.unshift(listing)
    
    // Reset form
    newListing.value = {
      title: '',
      description: '',
      price: 0,
      quantity: 1,
      imageUrl: '',
      isNegotiable: false
    }
    
    showCreateListing.value = false
  } catch (error) {
    console.error('Error creating listing:', error)
  }
}

const editListing = (listing: PlayerMarketListing) => {
  // In a real app, this would open an edit form
  console.log('Edit listing:', listing)
}

const deleteListing = async (listingId: number) => {
  try {
    // In a real app, this would be an API call
    // await api.delete(`/api/player-market/listings/${listingId}`)
    
    // Mock deletion
    listings.value = listings.value.filter(l => l.id !== listingId)
    myListings.value = myListings.value.filter(l => l.id !== listingId)
  } catch (error) {
    console.error('Error deleting listing:', error)
  }
}

const viewListing = (listing: PlayerMarketListing) => {
  // In a real app, this would navigate to listing detail page
  console.log('View listing:', listing)
}

const buyNow = (listing: PlayerMarketListing) => {
  // In a real app, this would navigate to purchase page
  console.log('Buy listing:', listing)
}

// Lifecycle
onMounted(() => {
  fetchListings()
  fetchMyListings()
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