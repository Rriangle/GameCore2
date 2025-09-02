import type { UserProfile, RegisterRequest, LoginRequest, AuthResponse } from '@/types/auth';

export const createMockUserProfile = (overrides: Partial<UserProfile> = {}): UserProfile => ({
    userId: 1,
    username: 'testuser',
    email: 'test@example.com',
    balance: 100.00,
    createdAt: '2024-01-01T00:00:00Z',
    lastLoginAt: '2024-01-01T12:00:00Z',
    ...overrides
});

export const createMockRegisterRequest = (overrides: Partial<RegisterRequest> = {}): RegisterRequest => ({
    username: 'testuser',
    email: 'test@example.com',
    password: 'ValidPass123!',
    confirmPassword: 'ValidPass123!',
    ...overrides
});

export const createMockLoginRequest = (overrides: Partial<LoginRequest> = {}): LoginRequest => ({
    username: 'testuser',
    password: 'ValidPass123!',
    ...overrides
});

export const createMockAuthResponse = (overrides: Partial<AuthResponse> = {}): AuthResponse => ({
    success: true,
    token: 'mock-jwt-token',
    message: '操作成功',
    user: createMockUserProfile(),
    ...overrides
});

export const createMockErrorResponse = (message: string = '操作失敗'): AuthResponse => ({
    success: false,
    message,
    token: '',
    user: undefined
});

export const mockApiError = {
    response: {
        status: 400,
        data: {
            success: false,
            message: '請求資料無效',
            errors: ['用戶名為必填項目']
        }
    }
};

export const mockNetworkError = {
    message: 'Network Error',
    request: {}
};

export const mockServerError = {
    response: {
        status: 500,
        data: {
            success: false,
            message: '伺服器發生錯誤'
        }
    }
};
