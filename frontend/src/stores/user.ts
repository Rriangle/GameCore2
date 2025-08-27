import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { 
  UserCompleteResponse, 
  LoginUserRequest, 
  RegisterUserRequest,
  UpdateUserIntroduceRequest,
  ApplySalesRequest
} from '@/types/user'
import apiService from '@/services/api'

/**
 * 使用者狀態管理 Store
 * 管理使用者登入狀態、個人資料等
 */
export const useUserStore = defineStore('user', () => {
  // 狀態
  const currentUser = ref<UserCompleteResponse | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // 計算屬性
  const isAuthenticated = computed(() => !!currentUser.value && apiService.isAuthenticated())
  const userName = computed(() => currentUser.value?.userName || '')
  const userAccount = computed(() => currentUser.value?.userAccount || '')
  const userPoints = computed(() => currentUser.value?.wallet?.userPoint || 0)
  const salesWallet = computed(() => currentUser.value?.userSalesInformation?.userSalesWallet || 0)
  const hasIntroduce = computed(() => !!currentUser.value?.userIntroduce)
  const hasSalesProfile = computed(() => !!currentUser.value?.memberSalesProfile)
  const canShopping = computed(() => currentUser.value?.userRights?.shoppingPermission || false)
  const canMessage = computed(() => currentUser.value?.userRights?.messagePermission || false)
  const canSales = computed(() => currentUser.value?.userRights?.salesAuthority || false)
  const isActive = computed(() => currentUser.value?.userRights?.userStatus || false)

  // 動作
  
  /**
   * 使用者登入
   */
  async function login(credentials: LoginUserRequest) {
    try {
      isLoading.value = true
      error.value = null
      
      const response = await apiService.login(credentials)
      apiService.setToken(response.token)
      
      // 登入成功後取得完整使用者資料
      await fetchCurrentUser()
      
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || '登入失敗'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 使用者註冊
   */
  async function register(userData: RegisterUserRequest) {
    try {
      isLoading.value = true
      error.value = null
      
      const response = await apiService.register(userData)
      apiService.setToken(response.token)
      
      // 註冊成功後取得完整使用者資料
      await fetchCurrentUser()
      
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || '註冊失敗'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 使用者登出
   */
  function logout() {
    currentUser.value = null
    apiService.clearToken()
    error.value = null
  }

  /**
   * 取得目前使用者資料
   */
  async function fetchCurrentUser() {
    try {
      isLoading.value = true
      error.value = null
      
      const userData = await apiService.getMe()
      currentUser.value = userData
      
      return userData
    } catch (err: any) {
      error.value = err.response?.data?.message || '取得使用者資料失敗'
      
      // 如果是認證錯誤，清除登入狀態
      if (err.response?.status === 401) {
        logout()
      }
      
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 更新使用者介紹資料
   */
  async function updateIntroduce(introduceData: UpdateUserIntroduceRequest) {
    try {
      isLoading.value = true
      error.value = null
      
      const response = await apiService.updateMyIntroduce(introduceData)
      
      // 更新成功後重新取得使用者資料
      await fetchCurrentUser()
      
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || '更新個人資料失敗'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 申請銷售功能
   */
  async function applySales(salesData: ApplySalesRequest) {
    try {
      isLoading.value = true
      error.value = null
      
      const response = await apiService.applySales(salesData)
      
      // 申請成功後重新取得使用者資料
      await fetchCurrentUser()
      
      return response
    } catch (err: any) {
      error.value = err.response?.data?.message || '申請銷售功能失敗'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * 初始化使用者狀態（應用啟動時調用）
   */
  async function initialize() {
    if (apiService.isAuthenticated()) {
      try {
        await fetchCurrentUser()
      } catch (err) {
        // 如果取得使用者資料失敗，清除登入狀態
        logout()
      }
    }
  }

  /**
   * 清除錯誤訊息
   */
  function clearError() {
    error.value = null
  }

  return {
    // 狀態
    currentUser,
    isLoading,
    error,
    
    // 計算屬性
    isAuthenticated,
    userName,
    userAccount,
    userPoints,
    salesWallet,
    hasIntroduce,
    hasSalesProfile,
    canShopping,
    canMessage,
    canSales,
    isActive,
    
    // 動作
    login,
    register,
    logout,
    fetchCurrentUser,
    updateIntroduce,
    applySales,
    initialize,
    clearError
  }
})