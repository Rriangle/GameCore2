import { ref, reactive } from 'vue'

// Toast 通知類型
export interface ToastOptions {
    id?: string
    type: 'success' | 'error' | 'warning' | 'info'
    title?: string
    message: string
    duration?: number
    closable?: boolean
    showProgress?: boolean
    position?: 'top-right' | 'top-left' | 'bottom-right' | 'bottom-left' | 'top-center' | 'bottom-center'
}

// Toast 實例
export interface ToastInstance extends ToastOptions {
    id: string
    visible: boolean
    progress: number
    timer?: NodeJS.Timeout
    progressTimer?: NodeJS.Timeout
}

// Toast 服務狀態
const toasts = reactive<ToastInstance[]>([])
const defaultPosition = ref<'top-right'>('top-right')

// 生成唯一 ID
const generateId = (): string => {
    return `toast-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
}

// 移除 Toast
const removeToast = (id: string) => {
    const index = toasts.findIndex(toast => toast.id === id)
    if (index > -1) {
        toasts.splice(index, 1)
    }
}

// 顯示 Toast
const showToast = (options: ToastOptions): string => {
    const id = options.id || generateId()
    const duration = options.duration ?? 5000
    const position = options.position || defaultPosition.value

    const toast: ToastInstance = {
        id,
        type: options.type,
        title: options.title,
        message: options.message,
        duration,
        closable: options.closable ?? true,
        showProgress: options.showProgress ?? true,
        position,
        visible: true,
        progress: 100
    }

    // 添加到列表
    toasts.push(toast)

    // 自動關閉
    if (duration > 0) {
        toast.timer = setTimeout(() => {
            hideToast(id)
        }, duration)

        // 進度條
        if (toast.showProgress) {
            const startTime = Date.now()
            toast.progressTimer = setInterval(() => {
                const elapsed = Date.now() - startTime
                const remaining = Math.max(0, 100 - (elapsed / duration) * 100)
                toast.progress = remaining

                if (remaining <= 0) {
                    clearInterval(toast.progressTimer)
                }
            }, 16) // 60fps
        }
    }

    return id
}

// 隱藏 Toast
const hideToast = (id: string) => {
    const toast = toasts.find(t => t.id === id)
    if (toast) {
        toast.visible = false

        // 清理定時器
        if (toast.timer) {
            clearTimeout(toast.timer)
        }
        if (toast.progressTimer) {
            clearInterval(toast.progressTimer)
        }

        // 延遲移除，等待動畫完成
        setTimeout(() => {
            removeToast(id)
        }, 300)
    }
}

// 清空所有 Toast
const clearAll = () => {
    toasts.forEach(toast => {
        if (toast.timer) {
            clearTimeout(toast.timer)
        }
        if (toast.progressTimer) {
            clearInterval(toast.progressTimer)
        }
    })
    toasts.splice(0, toasts.length)
}

// 快捷方法
const success = (message: string, title?: string, options?: Partial<ToastOptions>) => {
    return showToast({
        type: 'success',
        message,
        title,
        ...options
    })
}

const error = (message: string, title?: string, options?: Partial<ToastOptions>) => {
    return showToast({
        type: 'error',
        message,
        title,
        ...options
    })
}

const warning = (message: string, title?: string, options?: Partial<ToastOptions>) => {
    return showToast({
        type: 'warning',
        message,
        title,
        ...options
    })
}

const info = (message: string, title?: string, options?: Partial<ToastOptions>) => {
    return showToast({
        type: 'info',
        message,
        title,
        ...options
    })
}

// 設置默認位置
const setPosition = (position: ToastOptions['position']) => {
    if (position) {
        defaultPosition.value = position
    }
}

// 導出 composable
export function useToast() {
    return {
        // 狀態
        toasts: toasts as readonly ToastInstance[],

        // 核心方法
        showToast,
        hideToast,
        clearAll,

        // 快捷方法
        success,
        error,
        warning,
        info,

        // 配置
        setPosition
    }
}

// 默認導出
export default useToast 