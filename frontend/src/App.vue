<template>
  <div id="app" class="gc-app">
    <!-- 跳過連結 (無障礙功能) -->
    <a href="#main-content" class="gc-skip-link">跳至主要內容</a>
    
    <!-- 玻璃風導航欄 -->
    <header class="gc-header">
      <div class="gc-container">
        <nav class="gc-nav">
          <div class="gc-nav-brand">
            <router-link to="/" class="gc-logo">
              <span class="gc-logo-badge">GC</span>
              <span class="gc-logo-text">GameCore</span>
            </router-link>
          </div>
          
          <div class="gc-nav-menu">
            <router-link to="/" class="gc-nav-item">首頁</router-link>
            <router-link to="/about" class="gc-nav-item">關於</router-link>
            <router-link to="/login" class="gc-nav-item">登入</router-link>
            <router-link to="/register" class="gc-nav-item">註冊</router-link>
          </div>
          
          <div class="gc-nav-actions">
            <button class="gc-btn gc-btn-primary" @click="toggleTheme">
              {{ isDarkMode ? '🌞' : '🌙' }}
            </button>
          </div>
        </nav>
      </div>
    </header>

    <!-- 主要內容區域 -->
    <main id="main-content" class="gc-main">
      <router-view />
    </main>

    <!-- 玻璃風頁腳 -->
    <footer class="gc-footer">
      <div class="gc-container">
        <div class="gc-footer-content">
          <p>&copy; 2025 GameCore — 玻璃風設計系統 v2.3</p>
          <div class="gc-footer-links">
            <a href="#" class="gc-link">隱私政策</a>
            <a href="#" class="gc-link">使用條款</a>
            <a href="#" class="gc-link">聯絡我們</a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// 深色模式狀態
const isDarkMode = ref(false)

// 切換主題
const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value
  document.body.classList.toggle('dark', isDarkMode.value)
  
  // 儲存到 localStorage
  localStorage.setItem('gc-theme', isDarkMode.value ? 'dark' : 'light')
}

// 初始化主題
onMounted(() => {
  const savedTheme = localStorage.getItem('gc-theme')
  if (savedTheme === 'dark' || (!savedTheme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    isDarkMode.value = true
    document.body.classList.add('dark')
  }
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
  font-family: var(--gc-font-family);
  color: var(--gc-ink);
  line-height: var(--gc-line-height-base);
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

.gc-nav-menu {
  display: flex;
  gap: var(--gc-space-2);
  flex-wrap: wrap;
  flex: 1;
  justify-content: center;
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
}

.gc-nav-item:hover {
  color: var(--gc-ink);
  background: rgba(255, 255, 255, 0.1);
  border-color: var(--gc-accent);
}

.gc-nav-item.router-link-active {
  background: linear-gradient(90deg, var(--gc-primary-500), var(--gc-accent));
  color: white;
  border-color: transparent;
}

.gc-nav-actions {
  flex-shrink: 0;
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
  padding: var(--gc-space-6) 0;
}

.gc-footer-content {
  display: flex;
  flex-direction: column;
  gap: var(--gc-space-4);
  align-items: center;
  text-align: center;
}

.gc-footer-content p {
  margin: 0;
  color: var(--gc-muted);
  font-size: var(--gc-text-sm);
}

.gc-footer-links {
  display: flex;
  gap: var(--gc-space-4);
  flex-wrap: wrap;
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
@media (max-width: 768px) {
  .gc-nav {
    flex-direction: column;
    gap: var(--gc-space-3);
    padding: var(--gc-space-4) 0;
  }
  
  .gc-nav-menu {
    justify-content: center;
  }
  
  .gc-footer-content {
    gap: var(--gc-space-3);
  }
  
  .gc-footer-links {
    flex-direction: column;
    gap: var(--gc-space-2);
  }
}

@media (max-width: 480px) {
  .gc-logo-text {
    font-size: var(--gc-text-lg);
  }
  
  .gc-nav-item {
    padding: var(--gc-space-1) var(--gc-space-2);
    font-size: var(--gc-text-sm);
  }
}
</style>
