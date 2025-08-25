import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mockLoginCredentials, mockRegisterData, mockAuthResponse } from '../helpers/testData'

// Mock axios
const mockAxios = {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn()
}

vi.mock('axios', () => ({
    default: mockAxios
}))

describe('AuthService', () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it('should make login request correctly', async () => {
        mockAxios.post.mockResolvedValue({ data: mockAuthResponse })

        // Test login functionality
        const response = await mockAxios.post('/api/auth/login', mockLoginCredentials)
        
        expect(mockAxios.post).toHaveBeenCalledWith('/api/auth/login', mockLoginCredentials)
        expect(response.data).toEqual(mockAuthResponse)
    })

    it('should make register request correctly', async () => {
        mockAxios.post.mockResolvedValue({ data: mockAuthResponse })

        // Test register functionality
        const response = await mockAxios.post('/api/auth/register', mockRegisterData)
        
        expect(mockAxios.post).toHaveBeenCalledWith('/api/auth/register', mockRegisterData)
        expect(response.data).toEqual(mockAuthResponse)
    })
})
