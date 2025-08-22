import { apiService } from './api';
import type {
    RegisterRequest,
    LoginRequest,
    AuthResponse,
    UserProfile,
    ApiResponse
} from '@/types/auth';

export class AuthService {
    async register(data: RegisterRequest): Promise<AuthResponse> {
        const response = await apiService.post<AuthResponse>('/auth/register', data);
        return response;
    }

    async login(data: LoginRequest): Promise<AuthResponse> {
        const response = await apiService.post<AuthResponse>('/auth/login', data);
        return response;
    }

    async getProfile(): Promise<ApiResponse<UserProfile>> {
        const response = await apiService.get<UserProfile>('/auth/profile');
        return response;
    }

    logout(): void {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    }

    isAuthenticated(): boolean {
        return !!localStorage.getItem('token');
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    getUser(): UserProfile | null {
        const userStr = localStorage.getItem('user');
        return userStr ? JSON.parse(userStr) : null;
    }

    setAuthData(token: string, user: UserProfile): void {
        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));
    }
}

export const authService = new AuthService();
