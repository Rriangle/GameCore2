import type { UserProfile, AuthResponse } from '@/types/auth'

export const mockUserProfile: UserProfile = {
    userId: 1,
    username: 'testuser',
    email: 'test@example.com',
    balance: 100.00,
    createdAt: '2024-01-01T00:00:00Z',
    lastLoginAt: '2024-01-01T12:00:00Z'
}

export const mockAuthResponse: AuthResponse = {
    success: true,
    message: 'Login successful',
    token: 'mock-jwt-token',
    user: mockUserProfile
}

export const mockAuthResponseFailure: AuthResponse = {
    success: false,
    message: 'Invalid credentials',
    token: undefined,
    user: undefined
}

export const mockLoginCredentials = {
    username: 'testuser',
    password: 'password123'
}

export const mockRegisterData = {
    username: 'newuser',
    email: 'newuser@example.com',
    password: 'password123',
    confirmPassword: 'password123'
}
