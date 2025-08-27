import axios, { AxiosResponse } from 'axios'
import type { 
  RegisterUserRequest, 
  LoginUserRequest, 
  LoginResponse,
  UserCompleteResponse,
  UpdateUserIntroduceRequest,
  UserIntroduceResponse,
  ApplySalesRequest,
  MemberSalesProfileResponse,
  UserWalletResponse,
  UserSalesInformationResponse,
  UsersListResponse
} from '@/types/user'

/**
 * API 服務類別
 * 處理所有與後端 API 的通訊
 */
class ApiService {
  private baseURL: string
  private token: string | null = null

  constructor() {
    this.baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'
    this.setupInterceptors()
    this.loadTokenFromStorage()
  }

  /**
   * 設定 Axios 攔截器
   */
  private setupInterceptors() {
    // 請求攔截器 - 自動添加認證標頭
    axios.interceptors.request.use(
      (config) => {
        if (this.token) {
          config.headers.Authorization = `Bearer ${this.token}`
        }
        config.baseURL = this.baseURL
        return config
      },
      (error) => {
        return Promise.reject(error)
      }
    )

    // 回應攔截器 - 處理認證錯誤
    axios.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          this.clearToken()
          // 可以在這裡觸發登出邏輯或重定向到登入頁面
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  /**
   * 從本地儲存載入 Token
   */
  private loadTokenFromStorage() {
    const storedToken = localStorage.getItem('authToken')
    if (storedToken) {
      this.token = storedToken
    }
  }

  /**
   * 設定認證 Token
   */
  setToken(token: string) {
    this.token = token
    localStorage.setItem('authToken', token)
  }

  /**
   * 清除認證 Token
   */
  clearToken() {
    this.token = null
    localStorage.removeItem('authToken')
  }

  /**
   * 檢查是否已登入
   */
  isAuthenticated(): boolean {
    return !!this.token
  }

  // ===== 認證相關 API =====

  /**
   * 使用者註冊
   */
  async register(data: RegisterUserRequest): Promise<LoginResponse> {
    const response: AxiosResponse<LoginResponse> = await axios.post('/api/auth/register', data)
    return response.data
  }

  /**
   * 使用者登入
   */
  async login(data: LoginUserRequest): Promise<LoginResponse> {
    const response: AxiosResponse<LoginResponse> = await axios.post('/api/auth/login', data)
    return response.data
  }

  // ===== 使用者資料相關 API =====

  /**
   * 取得目前登入使用者的完整資料
   */
  async getMe(): Promise<UserCompleteResponse> {
    const response: AxiosResponse<UserCompleteResponse> = await axios.get('/api/users/me')
    return response.data
  }

  /**
   * 根據 ID 取得使用者資料
   */
  async getUser(id: number): Promise<UserCompleteResponse> {
    const response: AxiosResponse<UserCompleteResponse> = await axios.get(`/api/users/${id}`)
    return response.data
  }

  /**
   * 取得使用者列表
   */
  async getUsers(page: number = 1, pageSize: number = 20, search?: string): Promise<UsersListResponse> {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString()
    })
    
    if (search) {
      params.append('search', search)
    }

    const response: AxiosResponse<UsersListResponse> = await axios.get(`/api/users?${params}`)
    return response.data
  }

  /**
   * 更新使用者介紹資料
   */
  async updateMyIntroduce(data: UpdateUserIntroduceRequest): Promise<UserIntroduceResponse> {
    const response: AxiosResponse<UserIntroduceResponse> = await axios.put('/api/users/me/introduce', data)
    return response.data
  }

  /**
   * 申請銷售功能
   */
  async applySales(data: ApplySalesRequest): Promise<MemberSalesProfileResponse> {
    const response: AxiosResponse<MemberSalesProfileResponse> = await axios.post('/api/users/me/apply-sales', data)
    return response.data
  }

  /**
   * 取得我的錢包資訊
   */
  async getMyWallet(): Promise<UserWalletResponse> {
    const response: AxiosResponse<UserWalletResponse> = await axios.get('/api/users/me/wallet')
    return response.data
  }

  /**
   * 取得我的銷售錢包資訊
   */
  async getMySalesWallet(): Promise<UserSalesInformationResponse> {
    const response: AxiosResponse<UserSalesInformationResponse> = await axios.get('/api/users/me/sales-wallet')
    return response.data
  }
}

// 建立單例實例
export const apiService = new ApiService()
export default apiService