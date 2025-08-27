import { mount } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/LoginView.vue'
import { mockUserProfile, mockAuthResponse, mockLoginCredentials } from '../helpers/testData'

// Mock the auth store
const mockAuthStore = {
    isAuthenticated: false,
    user: null,
    error: null as string | null,
    loading: false,
    login: vi.fn(),
    clearError: vi.fn()
}

// Mock the auth store module
vi.mock('@/stores/auth', () => ({
    useAuthStore: () => mockAuthStore
}))

describe('LoginView', () => {
    let router: any

    beforeEach(() => {
        // Reset mocks
        vi.clearAllMocks()
        
        // Reset store state
        mockAuthStore.isAuthenticated = false
        mockAuthStore.user = null
        mockAuthStore.error = null
        mockAuthStore.loading = false

        // Create router
        router = createRouter({
            history: createWebHistory(),
            routes: [
                {
                    path: '/',
                    name: 'home',
                    component: { template: '<div>Home</div>' }
                },
                {
                    path: '/login',
                    name: 'login',
                    component: LoginView
                }
            ]
        })
    })

    it('should render login form correctly', () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        expect(wrapper.find('h1').text()).toBe('登入')
        expect(wrapper.find('input[name="username"]').exists()).toBe(true)
        expect(wrapper.find('input[name="password"]').exists()).toBe(true)
        expect(wrapper.find('button[type="submit"]').exists()).toBe(true)
    })

    it('should handle successful login', async () => {
        mockAuthStore.login.mockResolvedValue(mockAuthResponse)
        const pushSpy = vi.spyOn(router, 'push')

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        await wrapper.find('input[name="username"]').setValue(mockLoginCredentials.username)
        await wrapper.find('input[name="password"]').setValue(mockLoginCredentials.password)
        await wrapper.find('form').trigger('submit')

        expect(pushSpy).toHaveBeenCalledWith('/')
    })

    it('should display error message when login fails', async () => {
        const errorMessage = '用戶名或密碼錯誤'
        mockAuthStore.login.mockResolvedValue({ success: false, message: errorMessage })

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        await wrapper.find('input[name="username"]').setValue(mockLoginCredentials.username)
        await wrapper.find('input[name="password"]').setValue(mockLoginCredentials.password)
        await wrapper.find('form').trigger('submit')

        await wrapper.vm.$nextTick()
        expect(wrapper.text()).toContain(errorMessage)
    })

    it('should show loading state during login', async () => {
        mockAuthStore.loading = true

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        const submitButton = wrapper.find('button[type="submit"]')
        expect(submitButton.text()).toContain('登入中')
        expect(submitButton.attributes('disabled')).toBeDefined()
    })

    it('should validate required fields', async () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        // Submit without filling any fields
        await wrapper.find('form').trigger('submit')

        expect(mockAuthStore.login).not.toHaveBeenCalled()
    })

    it('should redirect to home if user is already authenticated', () => {
        mockAuthStore.isAuthenticated = true
        const pushSpy = vi.spyOn(router, 'push')

        mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        expect(pushSpy).toHaveBeenCalledWith('/')
    })

    it('should handle form validation errors correctly', async () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        // Only fill username, not password
        await wrapper.find('input[name="username"]').setValue('testuser')
        await wrapper.find('form').trigger('submit')

        expect(mockAuthStore.login).not.toHaveBeenCalled()
    })

    it('should clear error message', async () => {
        mockAuthStore.error = '之前的錯誤'

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        })

        // Error should be cleared when starting to type
        await wrapper.find('input[name="username"]').setValue('test')

        expect(mockAuthStore.error).toBe('')
    })
})
