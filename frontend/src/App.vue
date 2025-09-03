<template>
  <div id="app" class="gc-app">
    <!-- 跳過連結 (無障礙功能) -->
    <a href="#main-content" class="gc-skip-link">跳至主要內容</a>
    
    <!-- 玻璃風導航欄 -->
    <header class="gc-header">
      <div class="gc-container">
        <nav class="gc-nav">
          <!-- 品牌區域 -->
          <div class="gc-nav-brand">
            <router-link to="/" class="gc-logo">
              <span class="gc-logo-badge">GC</span>
              <span class="gc-logo-text">GameCore</span>
            </router-link>
          </div>
          
          <!-- 搜尋區域 -->
          <div class="gc-search">
            <input 
              type="text" 
              placeholder="搜尋遊戲、玩家..." 
              v-model="searchQuery"
              @keyup.enter="handleSearch"
              class="gc-search-input"
            />
            <button @click="handleSearch" class="gc-search-btn">
              🔍
            </button>
          </div>
          
          <!-- 主要導航選單 -->
          <div class="gc-nav-menu" :class="{ 'gc-nav-menu-open': isMobileMenuOpen }">
            <router-link to="/" class="gc-nav-item" @click="closeMobileMenu">首頁</router-link>
            <router-link to="/games" class="gc-nav-item" @click="closeMobileMenu">遊戲</router-link>
            <router-link to="/leaderboard" class="gc-nav-item" @click="closeMobileMenu">排行榜</router-link>
            <router-link to="/community" class="gc-nav-item" @click="closeMobileMenu">社群</router-link>
            <router-link to="/about" class="gc-nav-item" @click="closeMobileMenu">關於</router-link>
          </div>
          
          <!-- 用戶操作區域 -->
          <div class="gc-nav-actions">
            <!-- 主題切換 -->
            <button class="gc-btn gc-btn-theme" @click="toggleTheme" :title="isDarkMode ? '切換至淺色模式' : '切換至深色模式'">
              {{ isDarkMode ? '🌞' : '🌙' }}
            </button>
            
            <!-- 用戶選單 -->
            <div class="gc-user-menu" v-if="isAuthenticated">
              <button class="gc-btn gc-btn-user" @click="toggleUserMenu">
                <span class="gc-user-avatar">{{ userInitials }}</span>
                <span class="gc-user-name">{{ userName }}</span>
              </button>
              <div class="gc-user-dropdown" :class="{ 'gc-user-dropdown-open': isUserMenuOpen }">
                <router-link to="/profile" class="gc-dropdown-item">個人資料</router-link>
                <router-link to="/settings" class="gc-dropdown-item">設定</router-link>
                <router-link to="/wallet" class="gc-dropdown-item">錢包</router-link>
                <router-link to="/pets" class="gc-dropdown-item">寵物</router-link>
                <div class="gc-dropdown-divider"></div>
                <button @click="handleLogout" class="gc-dropdown-item gc-dropdown-item-danger">登出</button>
              </div>
            </div>
            
            <!-- 登入/註冊按鈕 -->
            <div v-else class="gc-auth-buttons">
              <router-link to="/login" class="gc-btn gc-btn-link">登入</router-link>
              <router-link to="/register" class="gc-btn gc-btn-primary">註冊</router-link>
            </div>
          </div>
          
          <!-- 移動端漢堡選單按鈕 -->
          <button 
            class="gc-mobile-menu-btn" 
            @click="toggleMobileMenu"
            :class="{ 'gc-mobile-menu-btn-open': isMobileMenuOpen }"
            aria-label="切換導航選單"
          >
            <span></span>
            <span></span>
            <span></span>
          </button>
        </nav>
      </div>
    </header>

    <!-- 主要內容區域 -->
    <main id="main-content" class="gc-main">
      <router-view />
    </main>

    <!-- Toast 通知容器 -->
    <ToastContainer />
    
    <!-- 玻璃風頁腳 -->
    <footer class="gc-footer">
      <div class="gc-container">
        <div class="gc-footer-content">
          <div class="gc-footer-section">
            <h3 class="gc-footer-title">GameCore</h3>
            <p class="gc-footer-description">打造最佳的遊戲體驗平台</p>
          </div>
          
          <div class="gc-footer-section">
            <h4 class="gc-footer-subtitle">快速連結</h4>
            <div class="gc-footer-links">
              <a href="#" class="gc-link">遊戲中心</a>
              <a href="#" class="gc-link">排行榜</a>
              <a href="#" class="gc-link">社群</a>
              <a href="#" class="gc-link">支援</a>
            </div>
          </div>
          
          <div class="gc-footer-section">
            <h4 class="gc-footer-subtitle">法律</h4>
            <div class="gc-footer-links">
              <a href="#" class="gc-link">隱私政策</a>
              <a href="#" class="gc-link">使用條款</a>
              <a href="#" class="gc-link">Cookie 政策</a>
            </div>
          </div>
          
          <div class="gc-footer-section">
            <h4 class="gc-footer-subtitle">聯絡我們</h4>
            <div class="gc-footer-links">
              <a href="#" class="gc-link">客服中心</a>
              <a href="#" class="gc-link">意見回饋</a>
              <a href="#" class="gc-link">合作夥伴</a>
            </div>
          </div>
        </div>
        
        <div class="gc-footer-bottom">
          <p>&copy; 2025 GameCore — 玻璃風設計系統 v2.3</p>
          <div class="gc-footer-social">
            <a href="#" class="gc-social-link" aria-label="Facebook">📘</a>
            <a href="#" class="gc-social-link" aria-label="Twitter">🐦</a>
            <a href="#" class="gc-social-link" aria-label="Discord">💬</a>
            <a href="#" class="gc-social-link" aria-label="YouTube">📺</a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import ToastContainer from './components/ui/ToastContainer.vue'

// 響應式狀態
const isDarkMode = ref(false)
const isMobileMenuOpen = ref(false)
const isUserMenuOpen = ref(false)
const searchQuery = ref('')

// 模擬用戶認證狀態（實際應從 store 或 API 獲取）
const isAuthenticated = ref(false)
const userName = ref('遊戲玩家')
const userInitials = ref('遊')

// 切換主題
const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value
  document.body.classList.toggle('dark', isDarkMode.value)
  
  // 儲存到 localStorage
  localStorage.setItem('gc-theme', isDarkMode.value ? 'dark' : 'light')
}

// 切換移動端選單
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
  // 防止背景滾動
  document.body.style.overflow = isMobileMenuOpen.value ? 'hidden' : ''
}

// 關閉移動端選單
const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
  document.body.style.overflow = ''
}

// 切換用戶選單
const toggleUserMenu = () => {
  isUserMenuOpen.value = !isUserMenuOpen.value
}

// 處理搜尋
const handleSearch = () => {
  if (searchQuery.value.trim()) {
    // 實際應導航到搜尋結果頁面
    console.log('搜尋:', searchQuery.value)
    // router.push({ name: 'search', query: { q: searchQuery.value } })
  }
}

// 處理登出
const handleLogout = () => {
  // 實際應清除認證狀態
  isAuthenticated.value = false
  isUserMenuOpen.value = false
  console.log('用戶登出')
}

// 點擊外部關閉選單
const handleClickOutside = (event: Event) => {
  const target = event.target as HTMLElement
  
  // 關閉用戶選單
  if (!target.closest('.gc-user-menu')) {
    isUserMenuOpen.value = false
  }
  
  // 關閉移動端選單
  if (!target.closest('.gc-nav') && !target.closest('.gc-mobile-menu-btn')) {
    closeMobileMenu()
  }
}

// 初始化主題
onMounted(() => {
  const savedTheme = localStorage.getItem('gc-theme')
  if (savedTheme === 'dark' || (!savedTheme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    isDarkMode.value = true
    document.body.classList.add('dark')
  }
  
  // 添加點擊外部監聽器
  document.addEventListener('click', handleClickOutside)
  
  // 模擬用戶已登入（測試用）
  setTimeout(() => {
    isAuthenticated.value = true
  }, 1000)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  document.body.style.overflow = ''
})
</script>

<style>
/* 玻璃風應用程式樣式 */
.gc-app {
  min-height: 100vh;
  background: 
    radial-gradient(900px 500px at -10% -10%, rgba(117, 87, 255, 0.10), transparent 60%),
    radial-gradient(800px 460px at 110% 10%, rgba(52, 210, 255, 0.10), transparent 50%),
    linear-gradient(180deg, var(--gc-bg), var(--gc-bg2));
  font-family: system-ui, -apple-system, "Segoe UI", Roboto, "Noto Sans TC", "PingFang TC", "Microsoft JhengHei", sans-serif;
  color: var(--gc-ink);
  line-height: 1.65;
}

/* 跳過連結 */
.gc-skip-link {
  position: absolute;
  top: -40px;
  left: 6px;
  background: var(--gc-accent);
  color: white;
  padding: 8px 12px;
  text-decoration: none;
  border-radius: var(--gc-radius-glass-sm);
  z-index: var(--gc-z-fixed);
  transition: top var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-skip-link:focus {
  top: 6px;
}

/* 玻璃風導航欄 */
.gc-header {
  position: sticky;
  top: 0;
  z-index: var(--gc-z-sticky);
  backdrop-filter: saturate(140%) blur(var(--gc-blur-glass));
  background: var(--gc-glass);
  border-bottom: 1px solid var(--gc-line);
}

.gc-container {
  max-width: var(--gc-container-max);
  margin: 0 auto;
  padding: 0 var(--gc-container-padding);
}

.gc-nav {
  display: flex;
  align-items: center;
  gap: var(--gc-space-4);
  padding: var(--gc-space-3) 0;
  position: relative;
}

.gc-nav-brand {
  flex-shrink: 0;
}

.gc-logo {
  display: flex;
  align-items: center;
  gap: var(--gc-space-3);
  font-weight: var(--gc-font-black);
  text-decoration: none;
  color: inherit;
}

.gc-logo-badge {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: white;
  display: grid;
  place-items: center;
  box-shadow: var(--gc-shadow-glass);
  font-weight: var(--gc-font-black);
}

.gc-logo-text {
  font-size: var(--gc-text-xl);
  color: var(--gc-ink);
}

/* 搜尋區域 */
.gc-search {
  flex: 1;
  display: flex;
  gap: var(--gc-space-2);
  background: var(--gc-surface);
  border: 1px solid var(--gc-line);
  border-radius: var(--gc-radius-glass-sm);
  padding: var(--gc-space-2) var(--gc-space-3);
  max-width: 400px;
  margin: 0 var(--gc-space-4);
}

.gc-search-input {
  flex: 1;
  background: transparent;
  border: 0;
  outline: 0;
  color: inherit;
  font-size: var(--gc-text-sm);
}

.gc-search-input::placeholder {
  color: var(--gc-muted);
}

.gc-search-btn {
  background: transparent;
  border: 0;
  cursor: pointer;
  padding: var(--gc-space-1);
  border-radius: var(--gc-radius-glass-xs);
  transition: background-color var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-search-btn:hover {
  background: rgba(0, 0, 0, 0.05);
}

/* 主要導航選單 */
.gc-nav-menu {
  display: flex;
  gap: var(--gc-space-2);
  flex-wrap: wrap;
  align-items: center;
}

.gc-nav-item {
  display: inline-flex;
  align-items: center;
  gap: var(--gc-space-2);
  padding: var(--gc-space-2) var(--gc-space-3);
  border: 1px solid var(--gc-line);
  border-radius: 999px;
  background: var(--gc-surface);
  color: var(--gc-muted);
  font-weight: var(--gc-font-bold);
  text-decoration: none;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  backdrop-filter: blur(var(--gc-blur-glass));
  font-size: var(--gc-text-sm);
}

.gc-nav-item:hover {
  color: var(--gc-ink);
  background: rgba(255, 255, 255, 0.1);
  border-color: var(--gc-accent);
  transform: translateY(-1px);
}

.gc-nav-item.router-link-active {
  background: linear-gradient(90deg, var(--gc-primary-500), var(--gc-accent));
  color: white;
  border-color: transparent;
}

/* 用戶操作區域 */
.gc-nav-actions {
  display: flex;
  gap: var(--gc-space-3);
  align-items: center;
  flex-shrink: 0;
}

/* 按鈕樣式 */
.gc-btn {
  display: inline-flex;
  align-items: center;
  gap: var(--gc-space-2);
  padding: var(--gc-space-2) var(--gc-space-3);
  border: 1px solid var(--gc-line);
  border-radius: var(--gc-radius-glass-sm);
  background: var(--gc-surface);
  color: var(--gc-ink);
  font-weight: var(--gc-font-bold);
  text-decoration: none;
  cursor: pointer;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
  backdrop-filter: blur(var(--gc-blur-glass));
  font-size: var(--gc-text-sm);
}

.gc-btn:hover {
  transform: translateY(-1px);
  box-shadow: var(--gc-shadow-glass);
}

.gc-btn-primary {
  border: 0;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-primary-500));
  color: white;
  box-shadow: var(--gc-shadow-glass);
}

.gc-btn-primary:hover {
  background: linear-gradient(135deg, var(--gc-accent-2), var(--gc-accent));
}

.gc-btn-link {
  background: transparent;
  border: 1px dashed var(--gc-line);
}

.gc-btn-theme {
  padding: var(--gc-space-2);
  min-width: 40px;
  justify-content: center;
}

/* 用戶選單 */
.gc-user-menu {
  position: relative;
}

.gc-btn-user {
  display: flex;
  align-items: center;
  gap: var(--gc-space-2);
  padding: var(--gc-space-2);
}

.gc-user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--gc-accent), var(--gc-accent-2));
  color: white;
  display: grid;
  place-items: center;
  font-weight: var(--gc-font-bold);
  font-size: var(--gc-text-sm);
}

.gc-user-name {
  display: none;
}

.gc-user-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: var(--gc-space-2);
  background: var(--gc-glass);
  border: 1px solid var(--gc-line);
  border-radius: var(--gc-radius-glass-sm);
  backdrop-filter: blur(var(--gc-blur-glass));
  box-shadow: var(--gc-shadow-glass);
  min-width: 200px;
  opacity: 0;
  visibility: hidden;
  transform: translateY(-10px);
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-user-dropdown-open {
  opacity: 1;
  visibility: visible;
  transform: translateY(0);
}

.gc-dropdown-item {
  display: block;
  padding: var(--gc-space-3);
  color: var(--gc-ink);
  text-decoration: none;
  transition: background-color var(--gc-transition-glass) var(--gc-ease-in-out);
  border: 0;
  background: transparent;
  width: 100%;
  text-align: left;
  cursor: pointer;
  font-size: var(--gc-text-sm);
}

.gc-dropdown-item:hover {
  background: rgba(0, 0, 0, 0.05);
}

.gc-dropdown-item-danger {
  color: var(--gc-danger);
}

.gc-dropdown-item-danger:hover {
  background: rgba(239, 68, 68, 0.1);
}

.gc-dropdown-divider {
  height: 1px;
  background: var(--gc-line);
  margin: var(--gc-space-2) 0;
}

/* 認證按鈕 */
.gc-auth-buttons {
  display: flex;
  gap: var(--gc-space-2);
}

/* 移動端漢堡選單按鈕 */
.gc-mobile-menu-btn {
  display: none;
  flex-direction: column;
  gap: 4px;
  background: transparent;
  border: 0;
  cursor: pointer;
  padding: var(--gc-space-2);
  border-radius: var(--gc-radius-glass-xs);
}

.gc-mobile-menu-btn span {
  width: 20px;
  height: 2px;
  background: var(--gc-ink);
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-mobile-menu-btn-open span:nth-child(1) {
  transform: rotate(45deg) translate(5px, 5px);
}

.gc-mobile-menu-btn-open span:nth-child(2) {
  opacity: 0;
}

.gc-mobile-menu-btn-open span:nth-child(3) {
  transform: rotate(-45deg) translate(7px, -6px);
}

/* 主要內容區域 */
.gc-main {
  margin: var(--gc-space-6) 0 var(--gc-space-12);
  min-height: calc(100vh - 200px);
}

/* 玻璃風頁腳 */
.gc-footer {
  border-top: 1px solid var(--gc-line);
  background: var(--gc-glass);
  backdrop-filter: blur(var(--gc-blur-glass));
  padding: var(--gc-space-8) 0 var(--gc-space-6);
}

.gc-footer-content {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: var(--gc-space-6);
  margin-bottom: var(--gc-space-6);
}

.gc-footer-section {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-3);
}

.gc-footer-title {
  font-size: var(--gc-text-xl);
  font-weight: var(--gc-font-black);
  color: var(--gc-ink);
  margin: 0;
}

.gc-footer-subtitle {
  font-size: var(--gc-text-lg);
  font-weight: var(--gc-font-bold);
  color: var(--gc-ink);
  margin: 0;
}

.gc-footer-description {
  color: var(--gc-muted);
  font-size: var(--gc-text-sm);
  margin: 0;
  line-height: 1.6;
}

.gc-footer-links {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-2);
}

.gc-link {
  color: var(--gc-muted);
  text-decoration: none;
  font-size: var(--gc-text-sm);
  transition: color var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-link:hover {
  color: var(--gc-accent);
}

.gc-footer-bottom {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: var(--gc-space-6);
  border-top: 1px solid var(--gc-line);
}

.gc-footer-bottom p {
  margin: 0;
  color: var(--gc-muted);
  font-size: var(--gc-text-sm);
}

.gc-footer-social {
  display: flex;
  gap: var(--gc-space-3);
}

.gc-social-link {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: var(--gc-surface);
  border: 1px solid var(--gc-line);
  color: var(--gc-muted);
  text-decoration: none;
  transition: all var(--gc-transition-glass) var(--gc-ease-in-out);
}

.gc-social-link:hover {
  background: var(--gc-accent);
  color: white;
  border-color: var(--gc-accent);
  transform: translateY(-2px);
}

/* 深色模式 */
body.dark .gc-app {
  background: 
    radial-gradient(900px 500px at -10% -10%, rgba(117, 87, 255, 0.15), transparent 60%),
    radial-gradient(800px 460px at 110% 10%, rgba(52, 210, 255, 0.15), transparent 50%),
    linear-gradient(180deg, var(--gc-bg-dark), var(--gc-bg2-dark));
}

body.dark .gc-header {
  background: var(--gc-glass-dark);
  border-bottom-color: var(--gc-line);
}

body.dark .gc-footer {
  background: var(--gc-glass-dark);
  border-top-color: var(--gc-line);
}

/* 響應式設計 */
@media (max-width: 1024px) {
  .gc-search {
    max-width: 300px;
    margin: 0 var(--gc-space-2);
  }
}

@media (max-width: 768px) {
  .gc-nav {
    flex-wrap: wrap;
    gap: var(--gc-space-3);
    padding: var(--gc-space-4) 0;
  }
  
  .gc-search {
    order: 3;
    max-width: none;
    margin: var(--gc-space-3) 0 0 0;
    width: 100%;
  }
  
  .gc-nav-menu {
    order: 4;
    width: 100%;
    justify-content: center;
    display: none;
  }
  
  .gc-nav-menu-open {
    display: flex;
    flex-direction: column;
    gap: var(--gc-space-2);
    padding: var(--gc-space-4) 0;
    border-top: 1px solid var(--gc-line);
    margin-top: var(--gc-space-3);
  }
  
  .gc-mobile-menu-btn {
    display: flex;
  }
  
  .gc-footer-content {
    grid-template-columns: 1fr;
    gap: var(--gc-space-4);
  }
  
  .gc-footer-bottom {
    flex-direction: column;
    gap: var(--gc-space-3);
    text-align: center;
  }
}

@media (max-width: 480px) {
  .gc-logo-text {
    font-size: var(--gc-text-lg);
  }
  
  .gc-nav-item {
    padding: var(--gc-space-1) var(--gc-space-2);
    font-size: var(--gc-text-xs);
  }
  
  .gc-btn {
    padding: var(--gc-space-1) var(--gc-space-2);
    font-size: var(--gc-text-xs);
  }
  
  .gc-user-name {
    display: none;
  }
}
</style>
