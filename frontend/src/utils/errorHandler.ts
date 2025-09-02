import type { ApiResponse } from '@/types/auth';

/**
 * 統一錯誤處理工具類
 */
export class ErrorHandler {
    /**
     * 處理 API 錯誤回應
     */
    static handleApiError(error: any): string {
        if (error.response?.data?.message) {
            return error.response.data.message;
        }

        if (error.response?.data?.errors && Array.isArray(error.response.data.errors)) {
            return error.response.data.errors.join(', ');
        }

        if (error.message) {
            return error.message;
        }

        return '發生未知錯誤，請稍後再試';
    }

    /**
     * 處理表單驗證錯誤
     */
    static handleValidationError(errors: Record<string, string[]>): string[] {
        const errorMessages: string[] = [];

        Object.values(errors).forEach(errorArray => {
            if (Array.isArray(errorArray)) {
                errorMessages.push(...errorArray);
            }
        });

        return errorMessages;
    }

    /**
     * 檢查是否為網路錯誤
     */
    static isNetworkError(error: any): boolean {
        return !error.response && error.request;
    }

    /**
     * 檢查是否為伺服器錯誤
     */
    static isServerError(error: any): boolean {
        return error.response?.status >= 500;
    }

    /**
     * 檢查是否為認證錯誤
     */
    static isAuthError(error: any): boolean {
        return error.response?.status === 401 || error.response?.status === 403;
    }

    /**
     * 取得錯誤的 HTTP 狀態碼
     */
    static getStatusCode(error: any): number | null {
        return error.response?.status || null;
    }

    /**
     * 格式化錯誤訊息
     */
    static formatErrorMessage(error: any, defaultMessage = '操作失敗'): string {
        if (this.isNetworkError(error)) {
            return '網路連線失敗，請檢查網路設定';
        }

        if (this.isServerError(error)) {
            return '伺服器發生錯誤，請稍後再試';
        }

        return this.handleApiError(error) || defaultMessage;
    }
}

/**
 * 表單驗證工具類
 */
export class ValidationHelper {
    /**
     * 驗證電子郵件格式
     */
    static isValidEmail(email: string): boolean {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    /**
     * 驗證密碼強度
     */
    static validatePassword(password: string): { isValid: boolean; errors: string[] } {
        const errors: string[] = [];

        if (password.length < 8) {
            errors.push('密碼長度至少需要 8 個字元');
        }

        if (!/[A-Z]/.test(password)) {
            errors.push('密碼需要包含至少一個大寫字母');
        }

        if (!/[a-z]/.test(password)) {
            errors.push('密碼需要包含至少一個小寫字母');
        }

        if (!/\d/.test(password)) {
            errors.push('密碼需要包含至少一個數字');
        }

        if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
            errors.push('密碼需要包含至少一個特殊字元');
        }

        return {
            isValid: errors.length === 0,
            errors
        };
    }

    /**
     * 驗證用戶名格式
     */
    static validateUsername(username: string): { isValid: boolean; errors: string[] } {
        const errors: string[] = [];

        if (username.length < 3 || username.length > 20) {
            errors.push('用戶名長度必須在 3-20 個字元之間');
        }

        if (!/^[a-zA-Z0-9_-]+$/.test(username)) {
            errors.push('用戶名只能包含字母、數字、底線和連字號');
        }

        if (username.startsWith('-') || username.endsWith('-') ||
            username.startsWith('_') || username.endsWith('_')) {
            errors.push('用戶名不能以底線或連字號開頭或結尾');
        }

        return {
            isValid: errors.length === 0,
            errors
        };
    }

    /**
     * 驗證確認密碼
     */
    static validateConfirmPassword(password: string, confirmPassword: string): boolean {
        return password === confirmPassword;
    }
}
