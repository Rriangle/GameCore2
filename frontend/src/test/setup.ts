import { config } from '@vue/test-utils';
import { vi } from 'vitest';

// 全域測試設定
config.global.stubs = {
    // 移除不需要的組件
    RouterLink: true,
    RouterView: true
};

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

// Mock fetch
global.fetch = vi.fn();

// Mock console methods to reduce noise in tests
global.console = {
    ...console,
    warn: vi.fn(),
    error: vi.fn()
};
