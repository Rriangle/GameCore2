/**
 * GameCore 主應用程式 JavaScript
 * 作者：GameCore 開發團隊
 * 版本：2.3
 * 描述：應用程式初始化、導航、主題切換等核心功能
 */

// 全域變數
let currentUser = null;
let currentTheme = 'light';
let currentDensity = 'normal';
let currentAccent = '#7557ff';

// DOM 元素快取
const elements = {
    // 導航元素
    navProfile: document.getElementById('navProfile'),
    navCheckin: document.getElementById('navCheckin'),
    navForum: document.getElementById('navForum'),
    navSlime: document.getElementById('navSlime'),
    navShop: document.getElementById('navShop'),
    navMarket: document.getElementById('navMarket'),
    navRanks: document.getElementById('navRanks'),
    navMatch: document.getElementById('navMatch'),

    // 主題控制
    darkModeToggle: document.getElementById('darkModeToggle'),
    densityToggle: document.getElementById('densityToggle'),
    colorDots: document.querySelectorAll('.dot'),

    // 搜尋
    searchInput: document.getElementById('searchInput'),

    // 用戶選單
    userMenu: document.getElementById('userMenu'),
    userName: document.getElementById('userName'),
    userAvatar: document.getElementById('userAvatar'),

    // 浮動按鈕
    fab: document.getElementById('fab'),
    btnNewPost: document.getElementById('btnNewPost'),

    // 模態框
    modal: document.getElementById('modal'),
    modalTitle: document.getElementById('modalTitle'),
    modalSubtitle: document.getElementById('modalSubtitle'),
    modalContent: document.getElementById('modalContent'),
    closeModal: document.getElementById('closeModal'),

    // 登入/註冊模態框
    loginModal: document.getElementById('loginModal'),
    registerModal: document.getElementById('registerModal'),
    loginForm: document.getElementById('loginForm'),
    registerForm: document.getElementById('registerForm'),
    closeLoginModal: document.getElementById('closeLoginModal'),
    closeRegisterModal: document.getElementById('closeRegisterModal'),
    showRegister: document.getElementById('showRegister'),
    showLogin: document.getElementById('showLogin')
};

/**
 * 應用程式初始化
 */
function initializeApp() {
    console.log('🚀 GameCore 應用程式初始化中...');

    // 載入用戶設定
    loadUserSettings();

    // 初始化主題
    initializeTheme();

    // 初始化事件監聽器
    initializeEventListeners();

    // 初始化導航
    initializeNavigation();

    // 初始化搜尋
    initializeSearch();

    // 初始化模態框
    initializeModals();

    // 載入用戶資料
    loadUserData();

    // 初始化頁面內容
    initializePageContent();

    console.log('✅ GameCore 應用程式初始化完成');
}

/**
 * 載入用戶設定
 */
function loadUserSettings() {
    try {
        const settings = JSON.parse(localStorage.getItem('gamecore_settings')) || {};
        currentTheme = settings.theme || 'light';
        currentDensity = settings.density || 'normal';
        currentAccent = settings.accent || '#7557ff';

        // 應用設定
        applyTheme(currentTheme);
        applyDensity(currentDensity);
        applyAccent(currentAccent);

        console.log('📋 用戶設定載入完成');
    } catch (error) {
        console.error('❌ 載入用戶設定失敗:', error);
    }
}

/**
 * 儲存用戶設定
 */
function saveUserSettings() {
    try {
        const settings = {
            theme: currentTheme,
            density: currentDensity,
            accent: currentAccent
        };
        localStorage.setItem('gamecore_settings', JSON.stringify(settings));
        console.log('💾 用戶設定已儲存');
    } catch (error) {
        console.error('❌ 儲存用戶設定失敗:', error);
    }
}

/**
 * 初始化主題系統
 */
function initializeTheme() {
    // 設定深色模式切換器狀態
    if (elements.darkModeToggle) {
        elements.darkModeToggle.checked = currentTheme === 'dark';
    }

    // 設定密度切換器狀態
    if (elements.densityToggle) {
        elements.densityToggle.checked = currentDensity === 'compact';
    }

    console.log('🎨 主題系統初始化完成');
}

/**
 * 應用主題
 */
function applyTheme(theme) {
    const body = document.body;

    if (theme === 'dark') {
        body.classList.add('dark');
    } else {
        body.classList.remove('dark');
    }

    currentTheme = theme;
    saveUserSettings();

    console.log(`🌙 主題已切換為: ${theme}`);
}

/**
 * 應用密度設定
 */
function applyDensity(density) {
    const body = document.body;

    if (density === 'compact') {
        body.classList.add('compact');
    } else {
        body.classList.remove('compact');
    }

    currentDensity = density;
    saveUserSettings();

    console.log(`📏 密度設定已切換為: ${density}`);
}

/**
 * 應用主色調
 */
function applyAccent(accent) {
    document.documentElement.style.setProperty('--accent', accent);
    currentAccent = accent;
    saveUserSettings();

    console.log(`🎨 主色調已切換為: ${accent}`);
}

/**
 * 初始化事件監聽器
 */
function initializeEventListeners() {
    // 深色模式切換
    if (elements.darkModeToggle) {
        elements.darkModeToggle.addEventListener('change', (e) => {
            const theme = e.target.checked ? 'dark' : 'light';
            applyTheme(theme);
        });
    }

    // 密度切換
    if (elements.densityToggle) {
        elements.densityToggle.addEventListener('change', (e) => {
            const density = e.target.checked ? 'compact' : 'normal';
            applyDensity(density);
        });
    }

    // 顏色選擇器
    elements.colorDots.forEach(dot => {
        dot.addEventListener('click', () => {
            const accent = dot.dataset.accent;
            applyAccent(accent);
        });
    });

    // 用戶選單
    if (elements.userMenu) {
        elements.userMenu.addEventListener('click', () => {
            if (currentUser) {
                showUserProfile();
            } else {
                showLoginModal();
            }
        });
    }

    // 浮動按鈕
    if (elements.fab) {
        elements.fab.addEventListener('click', () => {
            if (currentUser) {
                showNewPostModal();
            } else {
                showLoginModal();
            }
        });
    }

    if (elements.btnNewPost) {
        elements.btnNewPost.addEventListener('click', () => {
            if (currentUser) {
                showNewPostModal();
            } else {
                showLoginModal();
            }
        });
    }

    console.log('🎧 事件監聽器初始化完成');
}

/**
 * 初始化導航
 */
function initializeNavigation() {
    // 導航項目點擊事件
    const navItems = [
        elements.navProfile,
        elements.navCheckin,
        elements.navForum,
        elements.navSlime,
        elements.navShop,
        elements.navMarket,
        elements.navRanks,
        elements.navMatch
    ];

    navItems.forEach(item => {
        if (item) {
            item.addEventListener('click', (e) => {
                e.preventDefault();
                handleNavigation(item.getAttribute('href').substring(1));
            });
        }
    });

    console.log('🧭 導航系統初始化完成');
}

/**
 * 處理導航
 */
function handleNavigation(section) {
    // 移除所有導航項目的活動狀態
    document.querySelectorAll('.pill').forEach(pill => {
        pill.classList.remove('on');
    });

    // 設定當前導航項目為活動狀態
    const currentNav = document.querySelector(`[href="#${section}"]`);
    if (currentNav) {
        currentNav.classList.add('on');
    }

    // 根據區段執行相應操作
    switch (section) {
        case 'profile':
            showUserProfile();
            break;
        case 'checkin':
            showCheckinPage();
            break;
        case 'forum':
            showForumPage();
            break;
        case 'slime':
            showSlimePage();
            break;
        case 'shop':
            showShopPage();
            break;
        case 'market':
            showMarketPage();
            break;
        case 'ranks':
            showRanksPage();
            break;
        case 'match':
            showMatchPage();
            break;
        default:
            console.log(`📍 導航到: ${section}`);
    }
}

/**
 * 初始化搜尋
 */
function initializeSearch() {
    if (elements.searchInput) {
        // 搜尋輸入事件
        elements.searchInput.addEventListener('input', debounce(handleSearch, 300));

        // 搜尋表單提交事件
        const searchForm = elements.searchInput.closest('form');
        if (searchForm) {
            searchForm.addEventListener('submit', (e) => {
                e.preventDefault();
                handleSearch();
            });
        }
    }

    console.log('🔍 搜尋系統初始化完成');
}

/**
 * 處理搜尋
 */
function handleSearch() {
    const query = elements.searchInput.value.trim();

    if (query.length === 0) {
        // 清空搜尋結果，顯示預設內容
        showDefaultContent();
        return;
    }

    // 執行搜尋
    performSearch(query);
}

/**
 * 執行搜尋
 */
function performSearch(query) {
    console.log(`🔍 搜尋: ${query}`);

    // 這裡可以整合後端 API 搜尋
    // 目前使用前端搜尋
    const results = searchContent(query);
    displaySearchResults(results, query);
}

/**
 * 搜尋內容（前端搜尋）
 */
function searchContent(query) {
    // 這裡可以搜尋論壇文章、用戶、標籤等
    // 目前返回模擬結果
    return [
        {
            type: 'post',
            title: `搜尋結果: ${query}`,
            author: '系統',
            content: `找到與 "${query}" 相關的內容`,
            url: '#'
        }
    ];
}

/**
 * 顯示搜尋結果
 */
function displaySearchResults(results, query) {
    // 更新頁面標題
    document.title = `搜尋: ${query} - GameCore`;

    // 顯示搜尋結果
    const feedElement = document.getElementById('postFeed');
    if (feedElement) {
        feedElement.innerHTML = results.map(result => `
            <article class="row">
                <div class="av">🔍</div>
                <div>
                    <div style="font-weight:900">${result.title}</div>
                    <div class="meta">
                        <span>@${result.author}</span>
                        <span>｜</span>
                        <span>搜尋結果</span>
                    </div>
                </div>
                <div style="display:flex; gap:8px">
                    <span class="ghost">📄</span>
                </div>
            </article>
        `).join('');
    }

    // 更新計數
    const countElement = document.getElementById('feedCount');
    if (countElement) {
        countElement.textContent = `搜尋結果: ${results.length} 項`;
    }
}

/**
 * 顯示預設內容
 */
function showDefaultContent() {
    // 恢復預設頁面標題
    document.title = 'GameCore｜遊戲社群平台 - 論壇、市集、虛擬寵物';

    // 重新載入預設內容
    initializePageContent();
}

/**
 * 初始化模態框
 */
function initializeModals() {
    // 關閉模態框事件
    if (elements.closeModal) {
        elements.closeModal.addEventListener('click', closeModal);
    }

    if (elements.closeLoginModal) {
        elements.closeLoginModal.addEventListener('click', closeLoginModal);
    }

    if (elements.closeRegisterModal) {
        elements.closeRegisterModal.addEventListener('click', closeRegisterModal);
    }

    // 模態框背景點擊關閉
    if (elements.modal) {
        elements.modal.addEventListener('click', (e) => {
            if (e.target === elements.modal) {
                closeModal();
            }
        });
    }

    if (elements.loginModal) {
        elements.loginModal.addEventListener('click', (e) => {
            if (e.target === elements.loginModal) {
                closeLoginModal();
            }
        });
    }

    if (elements.registerModal) {
        elements.registerModal.addEventListener('click', (e) => {
            if (e.target === elements.registerModal) {
                closeRegisterModal();
            }
        });
    }

    // 登入/註冊切換
    if (elements.showRegister) {
        elements.showRegister.addEventListener('click', () => {
            closeLoginModal();
            showRegisterModal();
        });
    }

    if (elements.showLogin) {
        elements.showLogin.addEventListener('click', () => {
            closeRegisterModal();
            showLoginModal();
        });
    }

    // 表單提交事件
    if (elements.loginForm) {
        elements.loginForm.addEventListener('submit', handleLogin);
    }

    if (elements.registerForm) {
        elements.registerForm.addEventListener('submit', handleRegister);
    }

    console.log('🪟 模態框系統初始化完成');
}

/**
 * 顯示模態框
 */
function showModal(title, content, subtitle = '') {
    if (elements.modalTitle) elements.modalTitle.textContent = title;
    if (elements.modalSubtitle) elements.modalSubtitle.textContent = subtitle;
    if (elements.modalContent) elements.modalContent.innerHTML = content;

    elements.modal.classList.add('show');
    document.body.classList.add('noscroll');

    console.log(`🪟 顯示模態框: ${title}`);
}

/**
 * 關閉模態框
 */
function closeModal() {
    elements.modal.classList.remove('show');
    document.body.classList.remove('noscroll');

    console.log('🪟 關閉模態框');
}

/**
 * 顯示登入模態框
 */
function showLoginModal() {
    elements.loginModal.classList.add('show');
    document.body.classList.add('noscroll');

    console.log('🔐 顯示登入模態框');
}

/**
 * 關閉登入模態框
 */
function closeLoginModal() {
    elements.loginModal.classList.remove('show');
    document.body.classList.remove('noscroll');

    console.log('🔐 關閉登入模態框');
}

/**
 * 顯示註冊模態框
 */
function showRegisterModal() {
    elements.registerModal.classList.add('show');
    document.body.classList.remove('noscroll');

    console.log('📝 顯示註冊模態框');
}

/**
 * 關閉註冊模態框
 */
function closeRegisterModal() {
    elements.registerModal.classList.remove('show');
    document.body.classList.remove('noscroll');

    console.log('📝 關閉註冊模態框');
}

/**
 * 處理登入
 */
function handleLogin(e) {
    e.preventDefault();

    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    console.log('🔐 嘗試登入:', email);

    // 這裡應該調用後端 API
    // 目前使用模擬登入
    simulateLogin(email, password);
}

/**
 * 處理註冊
 */
function handleRegister(e) {
    e.preventDefault();

    const username = document.getElementById('registerUsername').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerConfirmPassword').value;

    if (password !== confirmPassword) {
        showNotification('密碼確認不匹配', 'error');
        return;
    }

    console.log('📝 嘗試註冊:', username, email);

    // 這裡應該調用後端 API
    // 目前使用模擬註冊
    simulateRegister(username, email, password);
}

/**
 * 模擬登入
 */
function simulateLogin(email, password) {
    // 模擬 API 延遲
    setTimeout(() => {
        // 模擬成功登入
        currentUser = {
            id: 1,
            username: email.split('@')[0],
            email: email,
            avatar: 'images/avatar-default.png'
        };

        // 更新 UI
        updateUserInterface();

        // 關閉登入模態框
        closeLoginModal();

        // 顯示成功通知
        showNotification('登入成功！', 'success');

        console.log('✅ 登入成功:', currentUser);
    }, 1000);
}

/**
 * 模擬註冊
 */
function simulateRegister(username, email, password) {
    // 模擬 API 延遲
    setTimeout(() => {
        // 模擬成功註冊
        currentUser = {
            id: 1,
            username: username,
            email: email,
            avatar: 'images/avatar-default.png'
        };

        // 更新 UI
        updateUserInterface();

        // 關閉註冊模態框
        closeRegisterModal();

        // 顯示成功通知
        showNotification('註冊成功！歡迎加入 GameCore', 'success');

        console.log('✅ 註冊成功:', currentUser);
    }, 1000);
}

/**
 * 更新用戶介面
 */
function updateUserInterface() {
    if (currentUser) {
        // 更新用戶選單
        if (elements.userName) {
            elements.userName.textContent = currentUser.username;
        }

        if (elements.userAvatar) {
            elements.userAvatar.src = currentUser.avatar;
        }

        // 啟用需要登入的功能
        enableAuthenticatedFeatures();
    } else {
        // 更新為遊客狀態
        if (elements.userName) {
            elements.userName.textContent = '遊客';
        }

        if (elements.userAvatar) {
            elements.userAvatar.src = 'images/avatar-default.png';
        }

        // 禁用需要登入的功能
        disableAuthenticatedFeatures();
    }
}

/**
 * 啟用已認證功能
 */
function enableAuthenticatedFeatures() {
    // 啟用發文按鈕
    if (elements.btnNewPost) {
        elements.btnNewPost.disabled = false;
        elements.btnNewPost.style.opacity = '1';
    }

    if (elements.fab) {
        elements.fab.disabled = false;
        elements.fab.style.opacity = '1';
    }

    console.log('🔓 已啟用認證功能');
}

/**
 * 禁用已認證功能
 */
function disableAuthenticatedFeatures() {
    // 禁用發文按鈕
    if (elements.btnNewPost) {
        elements.btnNewPost.disabled = true;
        elements.btnNewPost.style.opacity = '0.5';
    }

    if (elements.fab) {
        elements.fab.disabled = true;
        elements.fab.style.opacity = '0.5';
    }

    console.log('🔒 已禁用認證功能');
}

/**
 * 載入用戶資料
 */
function loadUserData() {
    // 檢查是否有儲存的用戶資料
    const savedUser = localStorage.getItem('gamecore_user');
    if (savedUser) {
        try {
            currentUser = JSON.parse(savedUser);
            updateUserInterface();
            console.log('👤 已載入儲存的用戶資料');
        } catch (error) {
            console.error('❌ 載入用戶資料失敗:', error);
        }
    }
}

/**
 * 儲存用戶資料
 */
function saveUserData() {
    if (currentUser) {
        localStorage.setItem('gamecore_user', JSON.stringify(currentUser));
    }
}

/**
 * 顯示通知
 */
function showNotification(message, type = 'info') {
    // 創建通知元素
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <span class="notification-message">${message}</span>
            <button class="notification-close">×</button>
        </div>
    `;

    // 添加到頁面
    document.body.appendChild(notification);

    // 顯示動畫
    setTimeout(() => {
        notification.classList.add('show');
    }, 100);

    // 關閉按鈕事件
    const closeBtn = notification.querySelector('.notification-close');
    closeBtn.addEventListener('click', () => {
        hideNotification(notification);
    });

    // 自動關閉
    setTimeout(() => {
        hideNotification(notification);
    }, 5000);

    console.log(`📢 顯示通知: ${message} (${type})`);
}

/**
 * 隱藏通知
 */
function hideNotification(notification) {
    notification.classList.remove('show');
    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 300);
}

/**
 * 防抖函數
 */
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

/**
 * 頁面功能實現（佔位符）
 */
function showUserProfile() {
    showModal('個人資料', '<p>個人資料頁面開發中...</p>', '用戶個人資訊');
}

function showCheckinPage() {
    showModal('每日簽到', '<p>每日簽到功能開發中...</p>', '簽到獎勵系統');
}

function showForumPage() {
    console.log('📝 顯示論壇頁面');
    // 論壇頁面已在主頁面實現
}

function showSlimePage() {
    showModal('我的史萊姆', '<p>史萊姆詳細頁面開發中...</p>', '虛擬寵物系統');
}

function showShopPage() {
    showModal('官方商城', '<p>官方商城開發中...</p>', '遊戲商品購買');
}

function showMarketPage() {
    showModal('玩家市集', '<p>玩家市集開發中...</p>', 'C2C 交易平台');
}

function showRanksPage() {
    showModal('排行榜', '<p>排行榜頁面開發中...</p>', '遊戲熱度排行');
}

function showMatchPage() {
    showModal('約玩配對', '<p>約玩配對功能開發中...</p>', '玩家配對系統');
}

function showNewPostModal() {
    const form = `
        <form id="newPostForm">
            <div class="form-group">
                <label for="postTitle">標題</label>
                <input type="text" id="postTitle" placeholder="請輸入文章標題" required>
            </div>
            <div class="form-group">
                <label for="postCategory">分類</label>
                <select id="postCategory" required>
                    <option value="">請選擇分類</option>
                    <option value="lol">英雄聯盟</option>
                    <option value="genshin">原神</option>
                    <option value="steam">Steam 綜合</option>
                    <option value="mobile">手機遊戲</option>
                    <option value="general">綜合討論</option>
                    <option value="mood">心情板</option>
                </select>
            </div>
            <div class="form-group">
                <label for="postContent">內容</label>
                <textarea id="postContent" rows="6" placeholder="請輸入文章內容" required></textarea>
            </div>
            <div class="form-actions">
                <button type="button" class="btn link" onclick="closeModal()">取消</button>
                <button type="submit" class="btn primary">發布</button>
            </div>
        </form>
    `;

    showModal('發表新主題', form, '分享你的想法');

    // 表單提交事件
    const newPostForm = document.getElementById('newPostForm');
    if (newPostForm) {
        newPostForm.addEventListener('submit', handleNewPost);
    }
}

function handleNewPost(e) {
    e.preventDefault();

    const title = document.getElementById('postTitle').value;
    const category = document.getElementById('postCategory').value;
    const content = document.getElementById('postContent').value;

    console.log('📝 發布新文章:', { title, category, content });

    // 模擬發布
    setTimeout(() => {
        closeModal();
        showNotification('文章發布成功！', 'success');

        // 重新載入文章列表
        initializePageContent();
    }, 1000);
}

/**
 * 初始化頁面內容
 */
function initializePageContent() {
    // 這個函數會在 forum.js 中實現
    if (typeof initializeForumContent === 'function') {
        initializeForumContent();
    }

    // 初始化虛擬寵物
    if (typeof initializePetSystem === 'function') {
        initializePetSystem();
    }

    console.log('📄 頁面內容初始化完成');
}

// 全域搜尋函數（供 HTML 調用）
function doSearch() {
    handleSearch();
}

// 當 DOM 載入完成後初始化應用程式
document.addEventListener('DOMContentLoaded', initializeApp);

// 當頁面完全載入後執行
window.addEventListener('load', () => {
    console.log('🎉 GameCore 應用程式完全載入完成');

    // 顯示歡迎訊息
    if (!currentUser) {
        setTimeout(() => {
            showNotification('歡迎來到 GameCore！登入以獲得完整功能', 'info');
        }, 2000);
    }
});

// 頁面卸載前儲存資料
window.addEventListener('beforeunload', () => {
    saveUserData();
    saveUserSettings();
}); 