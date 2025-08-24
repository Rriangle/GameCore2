import { createRouter, createWebHistory } from 'vue-router'
import { useUserStore } from '@/stores/user'
import HomeView from '../views/HomeView.vue'
import AboutView from '../views/AboutView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import ProfileView from '../views/ProfileView.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView
        },
        {
            path: '/about',
            name: 'about',
            component: AboutView
        },
        {
            path: '/login',
            name: 'login',
            component: LoginView,
            meta: { requiresGuest: true }
        },
        {
            path: '/register',
            name: 'register',
            component: RegisterView,
            meta: { requiresGuest: true }
        },
        {
            path: '/profile',
            name: 'profile',
            component: ProfileView,
            meta: { requiresAuth: true }
        }
    ]
})

// 路由守衛
router.beforeEach(async (to, from, next) => {
  const userStore = useUserStore()
  
  // 初始化使用者狀態（如果尚未初始化）
  if (!userStore.currentUser && userStore.isAuthenticated) {
    try {
      await userStore.initialize()
    } catch (error) {
      console.error('初始化使用者狀態失敗:', error)
    }
  }
  
  // 檢查需要認證的路由
  if (to.meta.requiresAuth && !userStore.isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
    return
  }
  
  // 檢查只允許訪客的路由（已登入用戶不應該看到登入/註冊頁面）
  if (to.meta.requiresGuest && userStore.isAuthenticated) {
    next({ name: 'profile' })
    return
  }
  
  next()
})

export default router
