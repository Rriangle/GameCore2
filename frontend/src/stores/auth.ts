import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { authService } from '@/services/authService';
import { ErrorHandler } from '@/utils/errorHandler';
import type { UserProfile, RegisterRequest, LoginRequest } from '@/types/auth';

export const useAuthStore = defineStore('auth', () => {
    const user = ref<UserProfile | null>(null);
    const token = ref<string | null>(null);
    const loading = ref(false);
    const error = ref<string | null>(null);

    // 計算屬性
    const isAuthenticated = computed(() => !!token.value);
    const userBalance = computed(() => user.value?.balance ?? 0);

    // 初始化狀態
    const initializeAuth = () => {
        const storedToken = authService.getToken();
        const storedUser = authService.getUser();

        if (storedToken && storedUser) {
            token.value = storedToken;
            user.value = storedUser;
        }
    };

    // 處理認證回應的共用邏輯
    const handleAuthResponse = (response: any, operation: string) => {
        if (response.success && response.token && response.user) {
            token.value = response.token;
            user.value = response.user;
            authService.setAuthData(response.token, response.user);
            return { success: true };
        } else {
            const errorMessage = response.message || `${operation}失敗`;
            error.value = errorMessage;
            return { success: false, message: errorMessage };
        }
    };

    // 處理認證錯誤的共用邏輯
    const handleAuthError = (err: any, operation: string) => {
        const errorMessage = ErrorHandler.formatErrorMessage(err, `${operation}過程中發生錯誤`);
        error.value = errorMessage;
        return { success: false, message: errorMessage };
    };

    // 註冊
    const register = async (data: RegisterRequest) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await authService.register(data);
            return handleAuthResponse(response, '註冊');
        } catch (err: any) {
            return handleAuthError(err, '註冊');
        } finally {
            loading.value = false;
        }
    };

    // 登入
    const login = async (data: LoginRequest) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await authService.login(data);
            return handleAuthResponse(response, '登入');
        } catch (err: any) {
            return handleAuthError(err, '登入');
        } finally {
            loading.value = false;
        }
    };

    // 登出
    const logout = () => {
        authService.logout();
        user.value = null;
        token.value = null;
        error.value = null;
    };

    // 更新用戶資料
    const updateUser = (updatedUser: UserProfile) => {
        user.value = updatedUser;
        if (user.value) {
            authService.setAuthData(token.value!, user.value);
        }
    };

    // 清除錯誤
    const clearError = () => {
        error.value = null;
    };

    return {
        // 狀態
        user,
        token,
        loading,
        error,

        // 計算屬性
        isAuthenticated,
        userBalance,

        // 方法
        initializeAuth,
        register,
        login,
        logout,
        updateUser,
        clearError
    };
});
