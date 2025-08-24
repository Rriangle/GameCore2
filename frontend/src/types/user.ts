/**
 * 使用者相關的 TypeScript 型別定義
 * 對應後端 API 的 DTO 結構
 */

// 使用者註冊請求
export interface RegisterUserRequest {
  userName: string
  userAccount: string
  password: string
  confirmPassword: string
}

// 使用者登入請求
export interface LoginUserRequest {
  userAccount: string
  password: string
}

// 使用者基本資訊回應
export interface UserBasicResponse {
  userId: number
  userName: string
  userAccount: string
}

// 登入回應
export interface LoginResponse {
  token: string
  user: UserBasicResponse
  expiresAt: string
}

// 使用者權限回應
export interface UserRightsResponse {
  userStatus: boolean
  shoppingPermission: boolean
  messagePermission: boolean
  salesAuthority: boolean
}

// 使用者錢包回應
export interface UserWalletResponse {
  userPoint: number
  couponNumber?: string
}

// 使用者介紹資訊回應
export interface UserIntroduceResponse {
  userNickName: string
  gender: string
  idNumber: string
  cellphone: string
  email: string
  address: string
  dateOfBirth: string
  createAccount: string
  userIntroduceText?: string
  hasUserPicture: boolean
}

// 會員銷售資料回應
export interface MemberSalesProfileResponse {
  bankCode?: number
  bankAccountNumber?: string
  hasAccountCoverPhoto: boolean
}

// 使用者銷售資訊回應
export interface UserSalesInformationResponse {
  userSalesWallet: number
}

// 使用者完整資訊回應
export interface UserCompleteResponse {
  userId: number
  userName: string
  userAccount: string
  userIntroduce?: UserIntroduceResponse
  userRights?: UserRightsResponse
  wallet?: UserWalletResponse
  memberSalesProfile?: MemberSalesProfileResponse
  userSalesInformation?: UserSalesInformationResponse
}

// 使用者介紹資料更新請求
export interface UpdateUserIntroduceRequest {
  userNickName: string
  gender: string
  idNumber: string
  cellphone: string
  email: string
  address: string
  dateOfBirth: string
  userIntroduceText?: string
}

// 銷售功能申請請求
export interface ApplySalesRequest {
  bankCode: number
  bankAccountNumber: string
  accountCoverPhotoBase64?: string
}

// 使用者列表項目回應
export interface UserListItemResponse {
  userId: number
  userName: string
  userAccount: string
  userNickName?: string
  userStatus: boolean
  shoppingPermission: boolean
  messagePermission: boolean
  salesAuthority: boolean
  userPoint: number
  hasSalesProfile: boolean
}

// 使用者列表查詢回應
export interface UsersListResponse {
  users: UserListItemResponse[]
  totalCount: number
  currentPage: number
  pageSize: number
  totalPages: number
}

// API 錯誤回應
export interface ApiErrorResponse {
  code: string
  message: string
  details?: any
}