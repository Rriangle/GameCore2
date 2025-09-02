import { describe, it, expect, beforeEach, vi } from 'vitest';
import { AuthService } from '@/services/authService';
import { createMockRegisterRequest, createMockLoginRequest, createMockAuthResponse, mockApiError, mockNetworkError } from '../helpers/testData';

// Mock axios
const mockAxios = {
    post: vi.fn(),
    get: vi.fn()
};

vi.mock('@/services/api', () => ({
    apiService: mockAxios
}));

// Mock localStorage
const localStorageMock = {
    getItem: vi.fn(),
    setItem: vi.fn(),
    removeItem: vi.fn(),
    clear: vi.fn()
};

Object.defineProperty(window, 'localStorage', {
    value: localStorageMock
});

describe('AuthService', () => {
    let authService: AuthService;

    beforeEach(() => {
        authService = new AuthService();
        vi.clearAllMocks();
    });

    describe('register', () => {
        it('應該成功註冊用戶', async () => {
            const registerData = createMockRegisterRequest();
            const expectedResponse = createMockAuthResponse({ message: '註冊成功' });

            mockAxios.post.mockResolvedValue({ data: expectedResponse });

            const result = await authService.register(registerData);

            expect(mockAxios.post).toHaveBeenCalledWith('/auth/register', registerData);
            expect(result).toEqual(expectedResponse);
        });

        it('應該處理註冊失敗', async () => {
            const registerData = createMockRegisterRequest();
            const errorResponse = { success: false, message: '用戶名已存在' };

            mockAxios.post.mockResolvedValue({ data: errorResponse });

            const result = await authService.register(registerData);

            expect(result).toEqual(errorResponse);
        });

        it('應該處理 API 錯誤', async () => {
            const registerData = createMockRegisterRequest();

            mockAxios.post.mockRejectedValue(mockApiError);

            await expect(authService.register(registerData)).rejects.toThrow();
        });

        it('應該處理網路錯誤', async () => {
            const registerData = createMockRegisterRequest();

            mockAxios.post.mockRejectedValue(mockNetworkError);

            await expect(authService.register(registerData)).rejects.toThrow();
        });
    });

    describe('login', () => {
        it('應該成功登入用戶', async () => {
            const loginData = createMockLoginRequest();
            const expectedResponse = createMockAuthResponse({ message: '登入成功' });

            mockAxios.post.mockResolvedValue({ data: expectedResponse });

            const result = await authService.login(loginData);

            expect(mockAxios.post).toHaveBeenCalledWith('/auth/login', loginData);
            expect(result).toEqual(expectedResponse);
        });

        it('應該處理登入失敗', async () => {
            const loginData = createMockLoginRequest();
            const errorResponse = { success: false, message: '用戶名或密碼錯誤' };

            mockAxios.post.mockResolvedValue({ data: errorResponse });

            const result = await authService.login(loginData);

            expect(result).toEqual(errorResponse);
        });

        it('應該處理 API 錯誤', async () => {
            const loginData = createMockLoginRequest();

            mockAxios.post.mockRejectedValue(mockApiError);

            await expect(authService.login(loginData)).rejects.toThrow();
        });
    });

    describe('getProfile', () => {
        it('應該成功獲取用戶資料', async () => {
            const expectedResponse = { success: true, data: createMockAuthResponse().user };

            mockAxios.get.mockResolvedValue({ data: expectedResponse });

            const result = await authService.getProfile();

            expect(mockAxios.get).toHaveBeenCalledWith('/auth/profile');
            expect(result).toEqual(expectedResponse);
        });

        it('應該處理獲取資料失敗', async () => {
            const errorResponse = { success: false, message: '用戶不存在' };

            mockAxios.get.mockResolvedValue({ data: errorResponse });

            const result = await authService.getProfile();

            expect(result).toEqual(errorResponse);
        });
    });

    describe('localStorage 操作', () => {
        it('應該正確設定認證資料', () => {
            const token = 'test-token';
            const user = createMockAuthResponse().user!;

            authService.setAuthData(token, user);

            expect(localStorageMock.setItem).toHaveBeenCalledWith('token', token);
            expect(localStorageMock.setItem).toHaveBeenCalledWith('user', JSON.stringify(user));
        });

        it('應該正確獲取 token', () => {
            const token = 'test-token';
            localStorageMock.getItem.mockReturnValue(token);

            const result = authService.getToken();

            expect(localStorageMock.getItem).toHaveBeenCalledWith('token');
            expect(result).toBe(token);
        });

        it('應該正確獲取用戶資料', () => {
            const user = createMockAuthResponse().user!;
            localStorageMock.getItem.mockReturnValue(JSON.stringify(user));

            const result = authService.getUser();

            expect(localStorageMock.getItem).toHaveBeenCalledWith('user');
            expect(result).toEqual(user);
        });

        it('應該正確處理無效的用戶資料', () => {
            localStorageMock.getItem.mockReturnValue('invalid-json');

            const result = authService.getUser();

            expect(result).toBeNull();
        });

        it('應該正確登出', () => {
            authService.logout();

            expect(localStorageMock.removeItem).toHaveBeenCalledWith('token');
            expect(localStorageMock.removeItem).toHaveBeenCalledWith('user');
        });

        it('應該正確檢查認證狀態', () => {
            localStorageMock.getItem.mockReturnValue('test-token');

            const result = authService.isAuthenticated();

            expect(localStorageMock.getItem).toHaveBeenCalledWith('token');
            expect(result).toBe(true);
        });

        it('應該正確處理未認證狀態', () => {
            localStorageMock.getItem.mockReturnValue(null);

            const result = authService.isAuthenticated();

            expect(result).toBe(false);
        });
    });

    describe('錯誤處理', () => {
        it('應該處理伺服器錯誤', async () => {
            const loginData = createMockLoginRequest();
            const serverError = {
                response: {
                    status: 500,
                    data: { success: false, message: '伺服器發生錯誤' }
                }
            };

            mockAxios.post.mockRejectedValue(serverError);

            await expect(authService.login(loginData)).rejects.toThrow();
        });

        it('應該處理 401 未授權錯誤', async () => {
            const loginData = createMockLoginRequest();
            const unauthorizedError = {
                response: {
                    status: 401,
                    data: { success: false, message: '認證失敗' }
                }
            };

            mockAxios.post.mockRejectedValue(unauthorizedError);

            await expect(authService.login(loginData)).rejects.toThrow();
        });

        it('應該處理 403 禁止訪問錯誤', async () => {
            const loginData = createMockLoginRequest();
            const forbiddenError = {
                response: {
                    status: 403,
                    data: { success: false, message: '禁止訪問' }
                }
            };

            mockAxios.post.mockRejectedValue(forbiddenError);

            await expect(authService.login(loginData)).rejects.toThrow();
        });
    });
});
