/* ========================================
   GameCore 主要 JavaScript 檔案 (Main JavaScript)
   ========================================
   
   這個檔案提供了整個 GameCore 平台的互動功能，
   包含表單驗證、UI 增強、無障礙功能等。
   
   主要功能：
   - 表單驗證和錯誤處理
   - UI 組件互動
   - 無障礙功能增強
   - 效能優化
   
   回滾方式：刪除此檔案即可恢復預設功能
   ======================================== */

(function () {
    'use strict';

    // ========================================
    // 全域變數和配置 (Global Variables & Configuration)
    // ========================================

    const GameCore = {
        // 配置選項
        config: {
            animationDuration: 250,
            debounceDelay: 300,
            tooltipDelay: 500,
            maxRetries: 3
        },

        // 狀態追蹤
        state: {
            isLoading: false,
            hasErrors: false,
            currentForm: null
        },

        // 快取
        cache: new Map()
    };

    // ========================================
    // 工具函數 (Utility Functions)
    // ========================================

    /**
     * 防抖函數 - 避免頻繁調用
     * @param {Function} func - 要執行的函數
     * @param {number} wait - 等待時間
     * @returns {Function} 防抖後的函數
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
     * 節流函數 - 限制函數執行頻率
     * @param {Function} func - 要執行的函數
     * @param {number} limit - 限制時間
     * @returns {Function} 節流後的函數
     */
    function throttle(func, limit) {
        let inThrottle;
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }

    /**
     * 安全的 DOM 查詢
     * @param {string} selector - CSS 選擇器
     * @param {Element} parent - 父元素
     * @returns {Element|null} 找到的元素或 null
     */
    function safeQuerySelector(selector, parent = document) {
        try {
            return parent.querySelector(selector);
        } catch (error) {
            console.warn('無效的選擇器:', selector, error);
            return null;
        }
    }

    /**
     * 安全的 DOM 查詢全部
     * @param {string} selector - CSS 選擇器
     * @param {Element} parent - 父元素
     * @returns {NodeList} 找到的元素列表
     */
    function safeQuerySelectorAll(selector, parent = document) {
        try {
            return parent.querySelectorAll(selector);
        } catch (error) {
            console.warn('無效的選擇器:', selector, error);
            return new NodeList();
        }
    }

    // ========================================
    // 表單驗證系統 (Form Validation System)
    // ========================================

    /**
     * 表單驗證器類別
     */
    class FormValidator {
        constructor(form) {
            this.form = form;
            this.errors = new Map();
            this.init();
        }

        /**
         * 初始化驗證器
         */
        init() {
            this.setupValidation();
            this.setupSubmitHandler();
        }

        /**
         * 設置驗證規則
         */
        setupValidation() {
            const inputs = this.form.querySelectorAll('input, select, textarea');

            inputs.forEach(input => {
                // 即時驗證
                input.addEventListener('blur', () => this.validateField(input));
                input.addEventListener('input', debounce(() => this.validateField(input), GameCore.config.debounceDelay));

                // 設置 ARIA 屬性
                this.setupAriaAttributes(input);
            });
        }

        /**
         * 設置 ARIA 屬性
         */
        setupAriaAttributes(input) {
            const label = this.form.querySelector(`label[for="${input.id}"]`);
            if (label) {
                input.setAttribute('aria-describedby', `${input.id}-error`);
            }
        }

        /**
         * 驗證單個欄位
         */
        validateField(field) {
            const value = field.value.trim();
            const rules = this.getValidationRules(field);
            const errors = [];

            // 必填驗證
            if (rules.required && !value) {
                errors.push('此欄位為必填');
            }

            // 最小長度驗證
            if (rules.minLength && value.length < rules.minLength) {
                errors.push(`最少需要 ${rules.minLength} 個字元`);
            }

            // 最大長度驗證
            if (rules.maxLength && value.length > rules.maxLength) {
                errors.push(`最多只能 ${rules.maxLength} 個字元`);
            }

            // 電子郵件驗證
            if (rules.email && value && !this.isValidEmail(value)) {
                errors.push('請輸入有效的電子郵件地址');
            }

            // 數字驗證
            if (rules.number && value && !this.isValidNumber(value)) {
                errors.push('請輸入有效的數字');
            }

            // 更新錯誤狀態
            this.updateFieldErrors(field, errors);

            return errors.length === 0;
        }

        /**
         * 獲取驗證規則
         */
        getValidationRules(field) {
            const rules = {};

            if (field.hasAttribute('required')) rules.required = true;
            if (field.hasAttribute('minlength')) rules.minLength = parseInt(field.getAttribute('minlength'));
            if (field.hasAttribute('maxlength')) rules.maxLength = parseInt(field.getAttribute('maxlength'));
            if (field.type === 'email') rules.email = true;
            if (field.type === 'number') rules.number = true;

            return rules;
        }

        /**
         * 驗證電子郵件格式
         */
        isValidEmail(email) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        }

        /**
         * 驗證數字格式
         */
        isValidNumber(value) {
            return !isNaN(value) && isFinite(value);
        }

        /**
         * 更新欄位錯誤狀態
         */
        updateFieldErrors(field, errors) {
            const errorContainer = this.form.querySelector(`#${field.id}-error`);

            if (errors.length > 0) {
                // 顯示錯誤
                field.classList.add('error');
                field.setAttribute('aria-invalid', 'true');

                if (errorContainer) {
                    errorContainer.textContent = errors[0];
                    errorContainer.style.display = 'block';
                }
            } else {
                // 清除錯誤
                field.classList.remove('error');
                field.setAttribute('aria-invalid', 'false');

                if (errorContainer) {
                    errorContainer.style.display = 'none';
                }
            }
        }

        /**
         * 設置提交處理器
         */
        setupSubmitHandler() {
            this.form.addEventListener('submit', (e) => {
                if (!this.validateForm()) {
                    e.preventDefault();
                    this.showFormErrors();
                }
            });
        }

        /**
         * 驗證整個表單
         */
        validateForm() {
            const inputs = this.form.querySelectorAll('input, select, textarea');
            let isValid = true;

            inputs.forEach(input => {
                if (!this.validateField(input)) {
                    isValid = false;
                }
            });

            return isValid;
        }

        /**
         * 顯示表單錯誤摘要
         */
        showFormErrors() {
            const errorSummary = this.createErrorSummary();
            const formTop = this.form.querySelector('.form-header') || this.form;

            // 移除現有的錯誤摘要
            const existingSummary = this.form.querySelector('.error-summary');
            if (existingSummary) {
                existingSummary.remove();
            }

            // 插入新的錯誤摘要
            formTop.insertAdjacentElement('afterend', errorSummary);

            // 滾動到錯誤摘要
            errorSummary.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }

        /**
         * 創建錯誤摘要
         */
        createErrorSummary() {
            const summary = document.createElement('div');
            summary.className = 'error-summary alert alert-danger';
            summary.setAttribute('role', 'alert');
            summary.setAttribute('aria-live', 'polite');

            const title = document.createElement('h3');
            title.textContent = '請修正以下錯誤：';
            title.className = 'h5';

            const errorList = document.createElement('ul');
            errorList.className = 'mb-0';

            const errorFields = this.form.querySelectorAll('.error');
            errorFields.forEach(field => {
                const label = this.form.querySelector(`label[for="${field.id}"]`);
                const labelText = label ? label.textContent : field.name || '未知欄位';

                const listItem = document.createElement('li');
                listItem.innerHTML = `<a href="#${field.id}">${labelText}</a>`;
                errorList.appendChild(listItem);
            });

            summary.appendChild(title);
            summary.appendChild(errorList);

            return summary;
        }
    }

    // ========================================
    // UI 組件增強 (UI Component Enhancement)
    // ========================================

    /**
     * 工具提示管理器
     */
    class TooltipManager {
        constructor() {
            this.tooltips = new Map();
            this.init();
        }

        /**
         * 初始化工具提示
         */
        init() {
            const tooltipElements = safeQuerySelectorAll('[data-tooltip]');

            tooltipElements.forEach(element => {
                this.createTooltip(element);
            });
        }

        /**
         * 創建工具提示
         */
        createTooltip(element) {
            const tooltip = document.createElement('div');
            tooltip.className = 'gc-tooltip-content';
            tooltip.textContent = element.getAttribute('data-tooltip');
            tooltip.style.display = 'none';

            document.body.appendChild(tooltip);

            this.tooltips.set(element, tooltip);

            // 事件處理
            element.addEventListener('mouseenter', () => this.showTooltip(element));
            element.addEventListener('mouseleave', () => this.hideTooltip(element));
            element.addEventListener('focus', () => this.showTooltip(element));
            element.addEventListener('blur', () => this.hideTooltip(element));
        }

        /**
         * 顯示工具提示
         */
        showTooltip(element) {
            const tooltip = this.tooltips.get(element);
            if (!tooltip) return;

            const rect = element.getBoundingClientRect();
            const tooltipRect = tooltip.getBoundingClientRect();

            // 計算位置
            let left = rect.left + (rect.width / 2) - (tooltipRect.width / 2);
            let top = rect.top - tooltipRect.height - 8;

            // 確保不超出視窗邊界
            if (left < 0) left = 8;
            if (left + tooltipRect.width > window.innerWidth) {
                left = window.innerWidth - tooltipRect.width - 8;
            }
            if (top < 0) {
                top = rect.bottom + 8;
            }

            tooltip.style.left = `${left}px`;
            tooltip.style.top = `${top}px`;
            tooltip.style.display = 'block';

            // 延遲隱藏
            setTimeout(() => {
                if (tooltip.style.display === 'block') {
                    this.hideTooltip(element);
                }
            }, GameCore.config.tooltipDelay);
        }

        /**
         * 隱藏工具提示
         */
        hideTooltip(element) {
            const tooltip = this.tooltips.get(element);
            if (tooltip) {
                tooltip.style.display = 'none';
            }
        }
    }

    /**
     * 載入狀態管理器
     */
    class LoadingManager {
        constructor() {
            this.loadingStates = new Map();
        }

        /**
         * 設置載入狀態
         */
        setLoading(element, isLoading) {
            if (isLoading) {
                element.classList.add('loading');
                element.setAttribute('aria-busy', 'true');
                element.disabled = true;

                // 添加載入指示器
                const spinner = document.createElement('span');
                spinner.className = 'spinner-border spinner-border-sm me-2';
                spinner.setAttribute('role', 'status');
                spinner.setAttribute('aria-hidden', 'true');

                element.insertBefore(spinner, element.firstChild);
                this.loadingStates.set(element, spinner);
            } else {
                element.classList.remove('loading');
                element.setAttribute('aria-busy', 'false');
                element.disabled = false;

                // 移除載入指示器
                const spinner = this.loadingStates.get(element);
                if (spinner) {
                    spinner.remove();
                    this.loadingStates.delete(element);
                }
            }
        }

        /**
         * 設置全域載入狀態
         */
        setGlobalLoading(isLoading) {
            GameCore.state.isLoading = isLoading;

            if (isLoading) {
                document.body.classList.add('loading');
                this.showGlobalSpinner();
            } else {
                document.body.classList.remove('loading');
                this.hideGlobalSpinner();
            }
        }

        /**
         * 顯示全域載入指示器
         */
        showGlobalSpinner() {
            let spinner = document.getElementById('global-spinner');
            if (!spinner) {
                spinner = document.createElement('div');
                spinner.id = 'global-spinner';
                spinner.className = 'global-spinner';
                spinner.innerHTML = `
          <div class="spinner-overlay">
            <div class="spinner-content">
              <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">載入中...</span>
              </div>
              <div class="mt-2">載入中...</div>
            </div>
          </div>
        `;
                document.body.appendChild(spinner);
            }
            spinner.style.display = 'block';
        }

        /**
         * 隱藏全域載入指示器
         */
        hideGlobalSpinner() {
            const spinner = document.getElementById('global-spinner');
            if (spinner) {
                spinner.style.display = 'none';
            }
        }
    }

    // ========================================
    // 無障礙功能增強 (Accessibility Enhancement)
    // ========================================

    /**
     * 無障礙功能管理器
     */
    class AccessibilityManager {
        constructor() {
            this.init();
        }

        /**
         * 初始化無障礙功能
         */
        init() {
            this.setupSkipLinks();
            this.setupFocusManagement();
            this.setupKeyboardNavigation();
            this.setupScreenReaderSupport();
        }

        /**
         * 設置跳過連結
         */
        setupSkipLinks() {
            const skipLink = document.createElement('a');
            skipLink.href = '#main-content';
            skipLink.className = 'gc-skip-link';
            skipLink.textContent = '跳過導航';

            document.body.insertBefore(skipLink, document.body.firstChild);
        }

        /**
         * 設置焦點管理
         */
        setupFocusManagement() {
            // 追蹤焦點變化
            document.addEventListener('focusin', (e) => {
                e.target.classList.add('focused');
            });

            document.addEventListener('focusout', (e) => {
                e.target.classList.remove('focused');
            });

            // 模態框焦點陷阱
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Tab') {
                    this.handleTabKey(e);
                }
            });
        }

        /**
         * 處理 Tab 鍵導航
         */
        handleTabKey(e) {
            const modal = e.target.closest('.modal');
            if (!modal) return;

            const focusableElements = modal.querySelectorAll(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
            );

            const firstElement = focusableElements[0];
            const lastElement = focusableElements[focusableElements.length - 1];

            if (e.shiftKey) {
                if (e.target === firstElement) {
                    e.preventDefault();
                    lastElement.focus();
                }
            } else {
                if (e.target === lastElement) {
                    e.preventDefault();
                    firstElement.focus();
                }
            }
        }

        /**
         * 設置鍵盤導航
         */
        setupKeyboardNavigation() {
            // Enter 鍵觸發點擊
            document.addEventListener('keydown', (e) => {
                if (e.key === 'Enter' && e.target.getAttribute('role') === 'button') {
                    e.preventDefault();
                    e.target.click();
                }
            });

            // 空格鍵觸發點擊
            document.addEventListener('keydown', (e) => {
                if (e.key === ' ' && e.target.getAttribute('role') === 'button') {
                    e.preventDefault();
                    e.target.click();
                }
            });
        }

        /**
         * 設置螢幕閱讀器支援
         */
        setupScreenReaderSupport() {
            // 動態內容更新通知
            const observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                        this.announceToScreenReader('內容已更新');
                    }
                });
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
        }

        /**
         * 向螢幕閱讀器宣布訊息
         */
        announceToScreenReader(message) {
            const announcement = document.createElement('div');
            announcement.setAttribute('aria-live', 'polite');
            announcement.setAttribute('aria-atomic', 'true');
            announcement.className = 'sr-only';
            announcement.textContent = message;

            document.body.appendChild(announcement);

            setTimeout(() => {
                announcement.remove();
            }, 1000);
        }
    }

    // ========================================
    // 效能優化 (Performance Optimization)
    // ========================================

    /**
     * 效能優化管理器
     */
    class PerformanceManager {
        constructor() {
            this.init();
        }

        /**
         * 初始化效能優化
         */
        init() {
            this.setupIntersectionObserver();
            this.setupResizeObserver();
            this.setupPerformanceMonitoring();
        }

        /**
         * 設置交叉觀察器 (懶載入)
         */
        setupIntersectionObserver() {
            const imageObserver = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src;
                        img.classList.remove('lazy');
                        imageObserver.unobserve(img);
                    }
                });
            });

            const lazyImages = safeQuerySelectorAll('img[data-src]');
            lazyImages.forEach(img => imageObserver.observe(img));
        }

        /**
         * 設置尺寸觀察器
         */
        setupResizeObserver() {
            const resizeObserver = new ResizeObserver(throttle((entries) => {
                entries.forEach(entry => {
                    // 處理響應式佈局調整
                    this.handleResize(entry);
                });
            }, 100));

            resizeObserver.observe(document.body);
        }

        /**
         * 處理尺寸變化
         */
        handleResize(entry) {
            const { width } = entry.contentRect;

            // 根據螢幕寬度調整佈局
            if (width < 768) {
                document.body.classList.add('mobile-layout');
                document.body.classList.remove('desktop-layout');
            } else {
                document.body.classList.add('desktop-layout');
                document.body.classList.remove('mobile-layout');
            }
        }

        /**
         * 設置效能監控
         */
        setupPerformanceMonitoring() {
            // 監控頁面載入效能
            window.addEventListener('load', () => {
                setTimeout(() => {
                    this.logPerformanceMetrics();
                }, 0);
            });
        }

        /**
         * 記錄效能指標
         */
        logPerformanceMetrics() {
            if ('performance' in window) {
                const perfData = performance.getEntriesByType('navigation')[0];
                const loadTime = perfData.loadEventEnd - perfData.loadEventStart;
                const domContentLoaded = perfData.domContentLoadedEventEnd - perfData.domContentLoadedEventStart;

                console.log('頁面載入效能:', {
                    'DOM 內容載入': `${domContentLoaded}ms`,
                    '頁面完全載入': `${loadTime}ms`
                });
            }
        }
    }

    // ========================================
    // 主要初始化 (Main Initialization)
    // ========================================

    /**
     * 主要初始化函數
     */
    function init() {
        // 等待 DOM 載入完成
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initApp);
        } else {
            initApp();
        }
    }

    /**
     * 初始化應用程式
     */
    function initApp() {
        try {
            // 初始化各個管理器
            GameCore.tooltipManager = new TooltipManager();
            GameCore.loadingManager = new LoadingManager();
            GameCore.accessibilityManager = new AccessibilityManager();
            GameCore.performanceManager = new PerformanceManager();

            // 初始化表單驗證
            initializeFormValidation();

            // 初始化事件監聽器
            initializeEventListeners();

            // 標記初始化完成
            document.body.classList.add('js-initialized');

            console.log('GameCore UI 初始化完成');
        } catch (error) {
            console.error('GameCore UI 初始化失敗:', error);
        }
    }

    /**
     * 初始化表單驗證
     */
    function initializeFormValidation() {
        const forms = safeQuerySelectorAll('form[data-validate]');
        forms.forEach(form => {
            new FormValidator(form);
        });
    }

    /**
     * 初始化事件監聽器
     */
    function initializeEventListeners() {
        // 全域點擊事件
        document.addEventListener('click', handleGlobalClick);

        // 全域鍵盤事件
        document.addEventListener('keydown', handleGlobalKeydown);

        // 視窗大小變化事件
        window.addEventListener('resize', throttle(handleWindowResize, 100));

        // 滾動事件
        window.addEventListener('scroll', throttle(handleScroll, 100));
    }

    /**
     * 處理全域點擊事件
     */
    function handleGlobalClick(e) {
        // 處理外部點擊關閉下拉選單
        const dropdowns = safeQuerySelectorAll('.dropdown-menu.show');
        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target) && !e.target.closest('.dropdown-toggle')) {
                dropdown.classList.remove('show');
            }
        });

        // 處理模態框外部點擊關閉
        const modals = safeQuerySelectorAll('.modal.show');
        modals.forEach(modal => {
            if (e.target === modal) {
                const closeBtn = modal.querySelector('[data-bs-dismiss="modal"]');
                if (closeBtn) closeBtn.click();
            }
        });
    }

    /**
     * 處理全域鍵盤事件
     */
    function handleGlobalKeydown(e) {
        // Escape 鍵關閉模態框
        if (e.key === 'Escape') {
            const openModal = safeQuerySelector('.modal.show');
            if (openModal) {
                const closeBtn = openModal.querySelector('[data-bs-dismiss="modal"]');
                if (closeBtn) closeBtn.click();
            }
        }
    }

    /**
     * 處理視窗大小變化
     */
    function handleWindowResize() {
        // 觸發自訂事件
        window.dispatchEvent(new CustomEvent('gamecore:resize'));
    }

    /**
     * 處理滾動事件
     */
    function handleScroll() {
        // 觸發自訂事件
        window.dispatchEvent(new CustomEvent('gamecore:scroll'));
    }

    // ========================================
    // 全域函數 (Global Functions)
    // ========================================

    /**
     * 顯示通知訊息
     */
    window.showNotification = function (message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

        document.body.appendChild(notification);

        // 自動隱藏
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, duration);
    };

    /**
     * 確認對話框
     */
    window.confirmAction = function (message, callback) {
        if (confirm(message)) {
            callback();
        }
    };

    /**
     * 格式化日期
     */
    window.formatDate = function (date, format = 'YYYY-MM-DD') {
        const d = new Date(date);
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');

        return format
            .replace('YYYY', year)
            .replace('MM', month)
            .replace('DD', day);
    };

    // ========================================
    // 啟動應用程式 (Start Application)
    // ========================================

    // 啟動初始化
    init();

    // 將 GameCore 物件暴露到全域
    window.GameCore = GameCore;

})(); 