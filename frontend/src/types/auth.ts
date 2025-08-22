export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    success: boolean;
    token?: string;
    message?: string;
    user?: UserProfile;
}

export interface UserProfile {
    userId: number;
    username: string;
    email: string;
    balance: number;
    createdAt: string;
    lastLoginAt?: string;
}

export interface WalletBalance {
    userId: number;
    balance: number;
    updatedAt: string;
}

export interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
    errors?: Record<string, string[]>;
}
