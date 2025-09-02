/**
 * GameCore 認證系統 JavaScript
 * 作者：GameCore 開發團隊
 * 版本：2.3
 * 描述：用戶登入、註冊、權限管理等
 */

// 認證系統全域變數
let authState = {
    isAuthenticated: false,
    currentUser: null,
    token: null,
    refreshToken: null,
    tokenExpiry: null
};

// API 端點
const API_ENDPOINTS = {
    LOGIN: '/api/auth/login',
    REGISTER: '/api/auth/register',
    PROFILE: '/api/auth/profile',
    REFRESH: '/api/auth/refresh',
    LOGOUT: '/api/auth/logout'
};

/**
 * 初始化認證系統
 */
function initializeAuthSystem() {
    console.log('🔐 初始化認證系統...');
    
    // 載入儲存的認證狀態
    loadAuthState();
    
    // 檢查 token 是否過期
    checkTokenExpiry();
    
    // 更新 UI 狀態
    updateAuthUI();
    
    // 綁定認證事件
    bindAuthEvents();
    
    console.log('✅ 認證系統初始化完成');
}

/**
 * 載入認證狀態
 */
function loadAuthState() {
    try {
        const savedState = localStorage.getItem('gamecore_auth_state');
        if (savedState) {
            const parsedState = JSON.parse(savedState);
            authState = { ...authState, ...parsedState };
            
            // 檢查 token 是否有效
            if (authState.token && authState.tokenExpiry) {
                const now = new Date().getTime();
                if (now < authState.tokenExpiry) {
                    authState.isAuthenticated = true;
                    console.log('🔐 已載入有效的認證狀態');
                } else {
                    // Token 已過期，清除狀態
                    clearAuthState();
                    console.log('⚠️ Token 已過期，已清除認證狀態');
                }
            }
        }
    } catch (error) {
        console.error('❌ 載入認證狀態失敗:', error);
        clearAuthState();
    }
}

/**
 * 儲存認證狀態
 */
function saveAuthState() {
    try {
        localStorage.setItem('gamecore_auth_state', JSON.stringify(authState));
        console.log('💾 認證狀態已儲存');
    } catch (error) {
        console.error('❌ 儲存認證狀態失敗:', error);
    }
}

/**
 * 清除認證狀態
 */
function clearAuthState() {
    authState = {
        isAuthenticated: false,
        currentUser: null,
        token: null,
        refreshToken: null,
        tokenExpiry: null
    };
    
    localStorage.removeItem('gamecore_auth_state');
    console.log('🧹 認證狀態已清除');
}

/**
 * 檢查 Token 過期
 */
function checkTokenExpiry() {
    if (authState.tokenExpiry) {
        const now = new Date().getTime();
        const timeUntilExpiry = authState.tokenExpiry - now;
        
        if (timeUntilExpiry <= 0) {
            // Token 已過期
            handleTokenExpiry();
        } else if (timeUntilExpiry < 5 * 60 * 1000) { // 5分鐘內過期
            // 自動刷新 token
            refreshAuthToken();
        }
    }
}

/**
 * 處理 Token 過期
 */
function handleTokenExpiry() {
    console.log('⚠️ Token 已過期');
    clearAuthState();
    updateAuthUI();
    showNotification('登入已過期，請重新登入', 'warning');
}

/**
 * 刷新認證 Token
 */
async function refreshAuthToken() {
    if (!authState.refreshToken) {
        handleTokenExpiry();
        return;
    }
    
    try {
        const response = await fetch(API_ENDPOINTS.REFRESH, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authState.refreshToken}`
            }
        });
        
        if (response.ok) {
            const data = await response.json();
            updateAuthTokens(data.token, data.refreshToken, data.expiresIn);
            console.log('🔄 Token 已自動刷新');
        } else {
            handleTokenExpiry();
        }
    } catch (error) {
        console.error('❌ 刷新 Token 失敗:', error);
        handleTokenExpiry();
    }
}

/**
 * 更新認證 Token
 */
function updateAuthTokens(token, refreshToken, expiresIn) {
    authState.token = token;
    authState.refreshToken = refreshToken;
    authState.tokenExpiry = new Date().getTime() + (expiresIn * 1000);
    saveAuthState();
}

/**
 * 更新認證 UI
 */
function updateAuthUI() {
    const userMenu = document.getElementById('userMenu');
    const userName = document.getElementById('userName');
    const userAvatar = document.getElementById('userAvatar');
    
    if (authState.isAuthenticated && authState.currentUser) {
        // 已登入狀態
        if (userName) {
            userName.textContent = authState.currentUser.username;
        }
        
        if (userAvatar) {
            userAvatar.src = authState.currentUser.avatar || 'images/avatar-default.png';
        }
        
        // 啟用需要登入的功能
        enableAuthenticatedFeatures();
        
        console.log('👤 用戶已登入:', authState.currentUser.username);
    } else {
        // 未登入狀態
        if (userName) {
            userName.textContent = '遊客';
        }
        
        if (userAvatar) {
            userAvatar.src = 'images/avatar-default.png';
        }
        
        // 禁用需要登入的功能
        disableAuthenticatedFeatures();
        
        console.log('👤 用戶未登入');
    }
}

/**
 * 啟用已認證功能
 */
function enableAuthenticatedFeatures() {
    // 啟用發文按鈕
    const btnNewPost = document.getElementById('btnNewPost');
    if (btnNewPost) {
        btnNewPost.disabled = false;
        btnNewPost.style.opacity = '1';
    }
    
    // 啟用浮動按鈕
    const fab = document.getElementById('fab');
    if (fab) {
        fab.disabled = false;
        fab.style.opacity = '1';
    }
    
    // 啟用購物車功能
    const cartButtons = document.querySelectorAll('.add-to-cart, .buy-now');
    cartButtons.forEach(button => {
        button.disabled = false;
        button.style.opacity = '1';
    });
    
    console.log('🔓 已啟用認證功能');
}

/**
 * 禁用已認證功能
 */
function disableAuthenticatedFeatures() {
    // 禁用發文按鈕
    const btnNewPost = document.getElementById('btnNewPost');
    if (btnNewPost) {
        btnNewPost.disabled = true;
        btnNewPost.style.opacity = '0.5';
    }
    
    // 禁用浮動按鈕
    const fab = document.getElementById('fab');
    if (fab) {
        fab.disabled = true;
        fab.style.opacity = '0.5';
    }
    
    // 禁用購物車功能
    const cartButtons = document.querySelectorAll('.add-to-cart, .buy-now');
    cartButtons.forEach(button => {
        button.disabled = true;
        button.style.opacity = '0.5';
    });
    
    console.log('🔒 已禁用認證功能');
}

/**
 * 綁定認證事件
 */
function bindAuthEvents() {
    // 用戶選單點擊事件
    const userMenu = document.getElementById('userMenu');
    if (userMenu) {
        userMenu.addEventListener('click', () => {
            if (authState.isAuthenticated) {
                showUserProfile();
            } else {
                showLoginModal();
            }
        });
    }
    
    // 登入表單提交事件
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }
    
    // 註冊表單提交事件
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }
    
    // 登入/註冊切換事件
    const showRegister = document.getElementById('showRegister');
    if (showRegister) {
        showRegister.addEventListener('click', () => {
            closeLoginModal();
            showRegisterModal();
        });
    }
    
    const showLogin = document.getElementById('showLogin');
    if (showLogin) {
        showLogin.addEventListener('click', () => {
            closeRegisterModal();
            showLoginModal();
        });
    }
    
    console.log('🎧 認證事件綁定完成');
}

/**
 * 處理登入
 */
async function handleLogin(e) {
    e.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    if (!email || !password) {
        showNotification('請填寫完整的登入資訊', 'error');
        return;
    }
    
    console.log('🔐 嘗試登入:', email);
    
    try {
        // 顯示載入狀態
        const submitBtn = e.target.querySelector('button[type="submit"]');
        const originalText = submitBtn.textContent;
        submitBtn.textContent = '登入中...';
        submitBtn.disabled = true;
        
        // 調用登入 API
        const success = await performLogin(email, password);
        
        if (success) {
            closeLoginModal();
            showNotification('登入成功！', 'success');
            
            // 載入用戶資料
            await loadUserProfile();
        }
        
        // 恢復按鈕狀態
        submitBtn.textContent = originalText;
        submitBtn.disabled = false;
        
    } catch (error) {
        console.error('❌ 登入失敗:', error);
        showNotification('登入失敗，請檢查帳號密碼', 'error');
        
        // 恢復按鈕狀態
        const submitBtn = e.target.querySelector('button[type="submit"]');
        submitBtn.textContent = '登入';
        submitBtn.disabled = false;
    }
}

/**
 * 執行登入
 */
async function performLogin(email, password) {
    try {
        const response = await fetch(API_ENDPOINTS.LOGIN, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                email: email,
                password: password
            })
        });
        
        if (response.ok) {
            const data = await response.json();
            
            // 更新認證狀態
            authState.isAuthenticated = true;
            authState.currentUser = {
                id: data.user.id,
                username: data.user.username,
                email: data.user.email,
                avatar: data.user.avatar
            };
            
            updateAuthTokens(data.token, data.refreshToken, data.expiresIn);
            updateAuthUI();
            
            return true;
        } else {
            const errorData = await response.json();
            throw new Error(errorData.message || '登入失敗');
        }
    } catch (error) {
        console.error('❌ 登入 API 錯誤:', error);
        throw error;
    }
}

/**
 * 處理註冊
 */
async function handleRegister(e) {
    e.preventDefault();
    
    const username = document.getElementById('registerUsername').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerConfirmPassword').value;
    
    // 驗證輸入
    if (!username || !email || !password || !confirmPassword) {
        showNotification('請填寫完整的註冊資訊', 'error');
        return;
    }
    
    if (password !== confirmPassword) {
        showNotification('密碼確認不匹配', 'error');
        return;
    }
    
    if (password.length < 6) {
        showNotification('密碼長度至少需要 6 個字元', 'error');
        return;
    }
    
    console.log('📝 嘗試註冊:', username, email);
    
    try {
        // 顯示載入狀態
        const submitBtn = e.target.querySelector('button[type="submit"]');
        const originalText = submitBtn.textContent;
        submitBtn.textContent = '註冊中...';
        submitBtn.disabled = true;
        
        // 調用註冊 API
        const success = await performRegister(username, email, password);
        
        if (success) {
            closeRegisterModal();
            showNotification('註冊成功！歡迎加入 GameCore', 'success');
            
            // 自動登入
            await performLogin(email, password);
        }
        
        // 恢復按鈕狀態
        submitBtn.textContent = originalText;
        submitBtn.disabled = false;
        
    } catch (error) {
        console.error('❌ 註冊失敗:', error);
        showNotification('註冊失敗，請稍後再試', 'error');
        
        // 恢復按鈕狀態
        const submitBtn = e.target.querySelector('button[type="submit"]');
        submitBtn.textContent = '註冊';
        submitBtn.disabled = false;
    }
}

/**
 * 執行註冊
 */
async function performRegister(username, email, password) {
    try {
        const response = await fetch(API_ENDPOINTS.REGISTER, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: username,
                email: email,
                password: password
            })
        });
        
        if (response.ok) {
            const data = await response.json();
            console.log('✅ 註冊成功:', data);
            return true;
        } else {
            const errorData = await response.json();
            throw new Error(errorData.message || '註冊失敗');
        }
    } catch (error) {
        console.error('❌ 註冊 API 錯誤:', error);
        throw error;
    }
}

/**
 * 載入用戶資料
 */
async function loadUserProfile() {
    if (!authState.isAuthenticated || !authState.token) {
        return;
    }
    
    try {
        const response = await fetch(API_ENDPOINTS.PROFILE, {
            headers: {
                'Authorization': `Bearer ${authState.token}`
            }
        });
        
        if (response.ok) {
            const data = await response.json();
            
            // 更新用戶資料
            authState.currentUser = {
                ...authState.currentUser,
                ...data.user
            };
            
            updateAuthUI();
            saveAuthState();
            
            console.log('👤 用戶資料已載入');
        }
    } catch (error) {
        console.error('❌ 載入用戶資料失敗:', error);
    }
}

/**
 * 登出
 */
async function logout() {
    try {
        if (authState.token) {
            // 調用登出 API
            await fetch(API_ENDPOINTS.LOGOUT, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${authState.token}`
                }
            });
        }
    } catch (error) {
        console.error('❌ 登出 API 錯誤:', error);
    }
    
    // 清除本地狀態
    clearAuthState();
    updateAuthUI();
    
    showNotification('已成功登出', 'success');
    console.log('👋 用戶已登出');
}

/**
 * 顯示登入模態框
 */
function showLoginModal() {
    const loginModal = document.getElementById('loginModal');
    if (loginModal) {
        loginModal.classList.add('show');
        document.body.classList.add('noscroll');
        
        // 聚焦到第一個輸入框
        const emailInput = document.getElementById('loginEmail');
        if (emailInput) {
            emailInput.focus();
        }
        
        console.log('🔐 顯示登入模態框');
    }
}

/**
 * 關閉登入模態框
 */
function closeLoginModal() {
    const loginModal = document.getElementById('loginModal');
    if (loginModal) {
        loginModal.classList.remove('show');
        document.body.classList.remove('noscroll');
        
        // 清空表單
        const loginForm = document.getElementById('loginForm');
        if (loginForm) {
            loginForm.reset();
        }
        
        console.log('🔐 關閉登入模態框');
    }
}

/**
 * 顯示註冊模態框
 */
function showRegisterModal() {
    const registerModal = document.getElementById('registerModal');
    if (registerModal) {
        registerModal.classList.add('show');
        document.body.classList.add('noscroll');
        
        // 聚焦到第一個輸入框
        const usernameInput = document.getElementById('registerUsername');
        if (usernameInput) {
            usernameInput.focus();
        }
        
        console.log('📝 顯示註冊模態框');
    }
}

/**
 * 關閉註冊模態框
 */
function closeRegisterModal() {
    const registerModal = document.getElementById('registerModal');
    if (registerModal) {
        registerModal.classList.remove('show');
        document.body.classList.remove('noscroll');
        
        // 清空表單
        const registerForm = document.getElementById('registerForm');
        if (registerForm) {
            registerForm.reset();
        }
        
        console.log('📝 關閉註冊模態框');
    }
}

/**
 * 顯示用戶資料
 */
function showUserProfile() {
    if (!authState.isAuthenticated || !authState.currentUser) {
        showLoginModal();
        return;
    }
    
    const user = authState.currentUser;
    const modalContent = `
        <div class="user-profile">
            <div class="profile-header">
                <img src="${user.avatar || 'images/avatar-default.png'}" alt="用戶頭像" class="profile-avatar">
                <div class="profile-info">
                    <h2>${user.username}</h2>
                    <p>${user.email}</p>
                    <p>會員編號: ${user.id}</p>
                </div>
            </div>
            <div class="profile-stats">
                <div class="stat-item">
                    <span class="stat-value">${getUserStats().posts}</span>
                    <span class="stat-label">發文數</span>
                </div>
                <div class="stat-item">
                    <span class="stat-value">${getUserStats().likes}</span>
                    <span class="stat-label">獲讚數</span>
                </div>
                <div class="stat-item">
                    <span class="stat-value">${getUserStats().level}</span>
                    <span class="stat-label">等級</span>
                </div>
            </div>
            <div class="profile-actions">
                <button class="btn btn-primary" onclick="editProfile()">編輯資料</button>
                <button class="btn btn-outline" onclick="logout()">登出</button>
            </div>
        </div>
    `;
    
    showModal('個人資料', modalContent, '用戶資訊與設定');
    console.log('👤 顯示用戶資料');
}

/**
 * 獲取用戶統計資料
 */
function getUserStats() {
    // 這裡應該從後端 API 獲取真實資料
    // 目前返回模擬資料
    return {
        posts: Math.floor(Math.random() * 50) + 5,
        likes: Math.floor(Math.random() * 1000) + 100,
        level: Math.floor(Math.random() * 20) + 1
    };
}

/**
 * 編輯用戶資料
 */
function editProfile() {
    const user = authState.currentUser;
    const modalContent = `
        <form id="editProfileForm">
            <div class="form-group">
                <label for="editUsername">使用者名稱</label>
                <input type="text" id="editUsername" value="${user.username}" required>
            </div>
            <div class="form-group">
                <label for="editEmail">電子郵件</label>
                <input type="email" id="editEmail" value="${user.email}" required>
            </div>
            <div class="form-group">
                <label for="editAvatar">頭像 URL</label>
                <input type="url" id="editAvatar" value="${user.avatar || ''}" placeholder="https://example.com/avatar.jpg">
            </div>
            <div class="form-actions">
                <button type="button" class="btn link" onclick="closeModal()">取消</button>
                <button type="submit" class="btn primary">儲存</button>
            </div>
        </form>
    `;
    
    showModal('編輯個人資料', modalContent, '修改用戶資訊');
    
    // 綁定表單提交事件
    const form = document.getElementById('editProfileForm');
    if (form) {
        form.addEventListener('submit', handleEditProfile);
    }
    
    console.log('✏️ 編輯用戶資料');
}

/**
 * 處理編輯資料
 */
async function handleEditProfile(e) {
    e.preventDefault();
    
    const username = document.getElementById('editUsername').value;
    const email = document.getElementById('editEmail').value;
    const avatar = document.getElementById('editAvatar').value;
    
    try {
        const response = await fetch(API_ENDPOINTS.PROFILE, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authState.token}`
            },
            body: JSON.stringify({
                username: username,
                email: email,
                avatar: avatar
            })
        });
        
        if (response.ok) {
            // 更新本地用戶資料
            authState.currentUser = {
                ...authState.currentUser,
                username: username,
                email: email,
                avatar: avatar
            };
            
            updateAuthUI();
            saveAuthState();
            
            closeModal();
            showNotification('資料更新成功', 'success');
            
            console.log('✅ 用戶資料更新成功');
        } else {
            throw new Error('更新失敗');
        }
    } catch (error) {
        console.error('❌ 更新用戶資料失敗:', error);
        showNotification('更新失敗，請稍後再試', 'error');
    }
}

/**
 * 檢查用戶權限
 */
function checkPermission(permission) {
    if (!authState.isAuthenticated) {
        return false;
    }
    
    // 這裡應該檢查用戶的具體權限
    // 目前返回基本權限
    const permissions = {
        'post': true,
        'comment': true,
        'like': true,
        'purchase': true,
        'sell': false, // 需要額外申請
        'admin': false
    };
    
    return permissions[permission] || false;
}

/**
 * 獲取認證標頭
 */
function getAuthHeaders() {
    const headers = {
        'Content-Type': 'application/json'
    };
    
    if (authState.token) {
        headers['Authorization'] = `Bearer ${authState.token}`;
    }
    
    return headers;
}

// 全域函數（供其他模組調用）
window.initializeAuthSystem = initializeAuthSystem;
window.showLoginModal = showLoginModal;
window.showRegisterModal = showRegisterModal;
window.logout = logout;
window.checkPermission = checkPermission;
window.getAuthHeaders = getAuthHeaders;
window.authState = authState; 