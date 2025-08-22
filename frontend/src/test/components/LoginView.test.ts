import { describe, it, expect, beforeEach, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { createPinia, setActivePinia } from 'pinia';
import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '@/views/LoginView.vue';
import { createMockLoginRequest, createMockAuthResponse, mockApiError } from '../helpers/testData';

// Mock router
const router = createRouter({
    history: createWebHistory(),
    routes: [
        { path: '/', component: { template: '<div>Home</div>' } },
        { path: '/login', component: LoginView }
    ]
});

// Mock auth store
const mockAuthStore = {
    login: vi.fn(),
    loading: false,
    error: null,
    isAuthenticated: false
};

vi.mock('@/stores/auth', () => ({
    useAuthStore: () => mockAuthStore
}));

describe('LoginView', () => {
    beforeEach(() => {
        setActivePinia(createPinia());
        vi.clearAllMocks();
        mockAuthStore.loading = false;
        mockAuthStore.error = null;
    });

    it('應該正確渲染登入表單', () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        expect(wrapper.find('h1').text()).toBe('登入');
        expect(wrapper.find('input[name="username"]').exists()).toBe(true);
        expect(wrapper.find('input[name="password"]').exists()).toBe(true);
        expect(wrapper.find('button[type="submit"]').exists()).toBe(true);
    });

    it('應該在表單提交時呼叫登入方法', async () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        const loginData = createMockLoginRequest();

        // 填寫表單
        await wrapper.find('input[name="username"]').setValue(loginData.username);
        await wrapper.find('input[name="password"]').setValue(loginData.password);

        // 提交表單
        await wrapper.find('form').trigger('submit');

        expect(mockAuthStore.login).toHaveBeenCalledWith(loginData);
    });

    it('應該在登入成功時導向首頁', async () => {
        const pushSpy = vi.spyOn(router, 'push');
        mockAuthStore.login.mockResolvedValue({ success: true });

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        const loginData = createMockLoginRequest();
        await wrapper.find('input[name="username"]').setValue(loginData.username);
        await wrapper.find('input[name="password"]').setValue(loginData.password);
        await wrapper.find('form').trigger('submit');

        expect(pushSpy).toHaveBeenCalledWith('/');
    });

    it('應該在登入失敗時顯示錯誤訊息', async () => {
        const errorMessage = '用戶名或密碼錯誤';
        mockAuthStore.login.mockResolvedValue({ success: false, message: errorMessage });

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        const loginData = createMockLoginRequest();
        await wrapper.find('input[name="username"]').setValue(loginData.username);
        await wrapper.find('input[name="password"]').setValue(loginData.password);
        await wrapper.find('form').trigger('submit');

        await wrapper.vm.$nextTick();
        expect(wrapper.text()).toContain(errorMessage);
    });

    it('應該在載入時顯示載入狀態', async () => {
        mockAuthStore.loading = true;

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        const submitButton = wrapper.find('button[type="submit"]');
        expect(submitButton.text()).toContain('登入中');
        expect(submitButton.attributes('disabled')).toBeDefined();
    });

    it('應該驗證必填欄位', async () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        // 不填寫任何欄位直接提交
        await wrapper.find('form').trigger('submit');

        expect(mockAuthStore.login).not.toHaveBeenCalled();
    });

    it('應該在用戶已登入時導向首頁', () => {
        mockAuthStore.isAuthenticated = true;
        const pushSpy = vi.spyOn(router, 'push');

        mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        expect(pushSpy).toHaveBeenCalledWith('/');
    });

    it('應該正確處理表單驗證錯誤', async () => {
        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        // 只填寫用戶名，不填寫密碼
        await wrapper.find('input[name="username"]').setValue('testuser');
        await wrapper.find('form').trigger('submit');

        expect(mockAuthStore.login).not.toHaveBeenCalled();
    });

    it('應該清除錯誤訊息', async () => {
        mockAuthStore.error = '之前的錯誤';

        const wrapper = mount(LoginView, {
            global: {
                plugins: [router]
            }
        });

        // 開始輸入時應該清除錯誤
        await wrapper.find('input[name="username"]').setValue('test');

        expect(mockAuthStore.error).toBeNull();
    });
});
