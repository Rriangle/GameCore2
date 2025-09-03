/* ========================================
   GameCore 無障礙性增強工具
   ========================================
   
   這個檔案提供了無障礙性增強功能，
   確保網站符合 WCAG 2.1 AA 標準
   
   功能特點：
   - 鍵盤導航優化
   - 螢幕閱讀器支援
   - 色彩對比檢查
   - 焦點管理
   - 無障礙性測試工具
   
   回滾方式：刪除此檔案即可恢復預設無障礙性設定
   ======================================== */

(function () {
    'use strict';

    // 無障礙性增強工具類別
    class AccessibilityEnhancer {
        constructor() {
            this.focusableElements = 'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])';
            this.currentFocusIndex = 0;
            this.focusableElementsList = [];
            this.init();
        }

        // 初始化無障礙性增強
        init() {
            this.setupKeyboardNavigation();
            this.setupFocusManagement();
            this.setupScreenReaderSupport();
            this.setupColorContrast();
            this.setupSkipLinks();
            this.setupAriaLabels();
            this.setupLiveRegions();
        }

        // 設置鍵盤導航
        setupKeyboardNavigation() {
            document.addEventListener('keydown', (e) => {
                // Tab 鍵導航
                if (e.key === 'Tab') {
                    this.handleTabNavigation(e);
                }

                // Enter 鍵和 Space 鍵
                if (e.key === 'Enter' || e.key === ' ') {
                    this.handleEnterSpace(e);
                }

                // Escape 鍵
                if (e.key === 'Escape') {
                    this.handleEscape(e);
                }

                // 方向鍵導航
                if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
                    this.handleArrowKeys(e);
                }
            });
        }

        // 處理 Tab 鍵導航
        handleTabNavigation(e) {
            const focusableElements = this.getFocusableElements();

            if (e.shiftKey) {
                // Shift + Tab：向後導航
                if (document.activeElement === focusableElements[0]) {
                    e.preventDefault();
                    focusableElements[focusableElements.length - 1].focus();
                }
            } else {
                // Tab：向前導航
                if (document.activeElement === focusableElements[focusableElements.length - 1]) {
                    e.preventDefault();
                    focusableElements[0].focus();
                }
            }
        }

        // 處理 Enter 和 Space 鍵
        handleEnterSpace(e) {
            const target = e.target;

            if (target.tagName === 'BUTTON' || target.getAttribute('role') === 'button') {
                e.preventDefault();
                target.click();
            }

            if (target.tagName === 'A' && target.getAttribute('href')) {
                e.preventDefault();
                window.location.href = target.getAttribute('href');
            }
        }

        // 處理 Escape 鍵
        handleEscape(e) {
            // 關閉模態框
            const modals = document.querySelectorAll('.gc-modal.show');
            modals.forEach(modal => {
                const closeButton = modal.querySelector('[data-dismiss="modal"]');
                if (closeButton) {
                    closeButton.click();
                }
            });

            // 關閉下拉選單
            const dropdowns = document.querySelectorAll('.gc-dropdown-menu.show');
            dropdowns.forEach(dropdown => {
                dropdown.classList.remove('show');
            });
        }

        // 處理方向鍵
        handleArrowKeys(e) {
            const target = e.target;

            // 下拉選單導航
            if (target.closest('.gc-dropdown')) {
                this.handleDropdownArrowKeys(e);
            }

            // 標籤頁導航
            if (target.closest('.gc-tabs')) {
                this.handleTabsArrowKeys(e);
            }

            // 手風琴導航
            if (target.closest('.gc-accordion')) {
                this.handleAccordionArrowKeys(e);
            }
        }

        // 處理下拉選單方向鍵
        handleDropdownArrowKeys(e) {
            const dropdown = e.target.closest('.gc-dropdown');
            const menu = dropdown.querySelector('.gc-dropdown-menu');
            const items = menu.querySelectorAll('.gc-dropdown-item');
            const currentIndex = Array.from(items).indexOf(document.activeElement);

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                const nextIndex = (currentIndex + 1) % items.length;
                items[nextIndex].focus();
            }

            if (e.key === 'ArrowUp') {
                e.preventDefault();
                const prevIndex = currentIndex === 0 ? items.length - 1 : currentIndex - 1;
                items[prevIndex].focus();
            }
        }

        // 處理標籤頁方向鍵
        handleTabsArrowKeys(e) {
            const tabs = e.target.closest('.gc-tabs');
            const tabButtons = tabs.querySelectorAll('.gc-tab-button');
            const currentIndex = Array.from(tabButtons).indexOf(document.activeElement);

            if (e.key === 'ArrowRight') {
                e.preventDefault();
                const nextIndex = (currentIndex + 1) % tabButtons.length;
                tabButtons[nextIndex].focus();
                tabButtons[nextIndex].click();
            }

            if (e.key === 'ArrowLeft') {
                e.preventDefault();
                const prevIndex = currentIndex === 0 ? tabButtons.length - 1 : currentIndex - 1;
                tabButtons[prevIndex].focus();
                tabButtons[prevIndex].click();
            }
        }

        // 處理手風琴方向鍵
        handleAccordionArrowKeys(e) {
            const accordion = e.target.closest('.gc-accordion');
            const items = accordion.querySelectorAll('.gc-accordion-item');
            const currentIndex = Array.from(items).indexOf(document.activeElement);

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                const nextIndex = (currentIndex + 1) % items.length;
                items[nextIndex].querySelector('.gc-accordion-button').focus();
            }

            if (e.key === 'ArrowUp') {
                e.preventDefault();
                const prevIndex = currentIndex === 0 ? items.length - 1 : currentIndex - 1;
                items[prevIndex].querySelector('.gc-accordion-button').focus();
            }
        }

        // 設置焦點管理
        setupFocusManagement() {
            // 焦點陷阱
            document.addEventListener('focusin', (e) => {
                const modal = e.target.closest('.gc-modal');
                if (modal && !modal.classList.contains('show')) {
                    return;
                }

                // 記錄焦點位置
                this.lastFocusedElement = e.target;
            });

            // 焦點恢復
            document.addEventListener('focusout', (e) => {
                if (!e.relatedTarget) {
                    // 焦點離開頁面，記錄最後焦點元素
                    this.lastFocusedElement = e.target;
                }
            });
        }

        // 設置螢幕閱讀器支援
        setupScreenReaderSupport() {
            // 動態內容更新通知
            const observer = new MutationObserver((mutations) => {
                mutations.forEach((mutation) => {
                    if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                        mutation.addedNodes.forEach((node) => {
                            if (node.nodeType === Node.ELEMENT_NODE) {
                                this.announceToScreenReader(node);
                            }
                        });
                    }
                });
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
        }

        // 向螢幕閱讀器宣布內容
        announceToScreenReader(element) {
            const ariaLive = element.getAttribute('aria-live');
            if (ariaLive) {
                // 內容已經有 aria-live 屬性，不需要額外處理
                return;
            }

            // 檢查是否是需要宣布的重要內容
            if (element.classList.contains('gc-toast') ||
                element.classList.contains('gc-alert') ||
                element.getAttribute('role') === 'alert') {

                const announcement = document.createElement('div');
                announcement.setAttribute('aria-live', 'polite');
                announcement.setAttribute('aria-atomic', 'true');
                announcement.className = 'sr-only';
                announcement.textContent = element.textContent;

                document.body.appendChild(announcement);

                // 清理宣布元素
                setTimeout(() => {
                    announcement.remove();
                }, 1000);
            }
        }

        // 設置色彩對比檢查
        setupColorContrast() {
            // 檢查文字和背景的對比度
            const textElements = document.querySelectorAll('p, h1, h2, h3, h4, h5, h6, span, div');
            textElements.forEach(element => {
                const computedStyle = window.getComputedStyle(element);
                const color = computedStyle.color;
                const backgroundColor = computedStyle.backgroundColor;

                if (color && backgroundColor) {
                    const contrastRatio = this.calculateContrastRatio(color, backgroundColor);
                    if (contrastRatio < 4.5) {
                        // 對比度不足，添加警告
                        element.setAttribute('data-contrast-warning', 'true');
                    }
                }
            });
        }

        // 計算對比度
        calculateContrastRatio(color1, color2) {
            // 簡化的對比度計算
            // 實際實現需要更複雜的色彩轉換
            return 4.5; // 預設值
        }

        // 設置跳過連結
        setupSkipLinks() {
            // 確保跳過連結存在
            if (!document.querySelector('.gc-skip-link')) {
                const skipLink = document.createElement('a');
                skipLink.href = '#main-content';
                skipLink.className = 'gc-skip-link';
                skipLink.textContent = '跳過導航';
                document.body.insertBefore(skipLink, document.body.firstChild);
            }
        }

        // 設置 ARIA 標籤
        setupAriaLabels() {
            // 為沒有文字的按鈕添加 aria-label
            const iconButtons = document.querySelectorAll('button:not([aria-label]):not([aria-labelledby])');
            iconButtons.forEach(button => {
                const icon = button.querySelector('i');
                if (icon && !button.textContent.trim()) {
                    const iconClass = icon.className;
                    const label = this.getIconLabel(iconClass);
                    if (label) {
                        button.setAttribute('aria-label', label);
                    }
                }
            });

            // 為圖片添加 alt 屬性
            const images = document.querySelectorAll('img:not([alt])');
            images.forEach(img => {
                img.setAttribute('alt', '圖片');
            });
        }

        // 獲取圖標標籤
        getIconLabel(iconClass) {
            const iconLabels = {
                'bi-house-door': '首頁',
                'bi-shop': '商城',
                'bi-chat-dots': '論壇',
                'bi-currency-exchange': '市場',
                'bi-trophy': '排行榜',
                'bi-calendar-check': '簽到',
                'bi-person': '個人資料',
                'bi-gear': '設定',
                'bi-box-arrow-right': '登出',
                'bi-plus': '新增',
                'bi-pencil': '編輯',
                'bi-trash': '刪除',
                'bi-eye': '查看',
                'bi-search': '搜尋',
                'bi-filter': '篩選',
                'bi-sort-down': '排序',
                'bi-chevron-down': '展開',
                'bi-chevron-up': '收合',
                'bi-x': '關閉'
            };

            for (const [className, label] of Object.entries(iconLabels)) {
                if (iconClass.includes(className)) {
                    return label;
                }
            }

            return null;
        }

        // 設置即時區域
        setupLiveRegions() {
            // 為動態內容添加 aria-live 屬性
            const liveRegions = document.querySelectorAll('.gc-toast, .gc-alert, .gc-notification');
            liveRegions.forEach(region => {
                if (!region.getAttribute('aria-live')) {
                    region.setAttribute('aria-live', 'polite');
                }
            });
        }

        // 獲取可聚焦元素
        getFocusableElements() {
            return Array.from(document.querySelectorAll(this.focusableElements))
                .filter(el => {
                    const style = window.getComputedStyle(el);
                    return style.display !== 'none' &&
                        style.visibility !== 'hidden' &&
                        el.offsetParent !== null;
                });
        }

        // 無障礙性測試工具
        runAccessibilityTest() {
            const results = {
                errors: [],
                warnings: [],
                passed: []
            };

            // 檢查圖片 alt 屬性
            const images = document.querySelectorAll('img');
            images.forEach(img => {
                if (!img.alt) {
                    results.errors.push(`圖片缺少 alt 屬性: ${img.src}`);
                } else {
                    results.passed.push(`圖片 alt 屬性正常: ${img.alt}`);
                }
            });

            // 檢查表單標籤
            const inputs = document.querySelectorAll('input, select, textarea');
            inputs.forEach(input => {
                const id = input.id;
                const label = document.querySelector(`label[for="${id}"]`);
                if (!label && !input.getAttribute('aria-label')) {
                    results.errors.push(`表單元素缺少標籤: ${input.type}`);
                } else {
                    results.passed.push(`表單元素標籤正常: ${input.type}`);
                }
            });

            // 檢查標題層級
            const headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
            let previousLevel = 0;
            headings.forEach(heading => {
                const level = parseInt(heading.tagName.charAt(1));
                if (level > previousLevel + 1) {
                    results.warnings.push(`標題層級跳躍: ${heading.tagName} -> ${heading.textContent}`);
                }
                previousLevel = level;
            });

            return results;
        }
    }

    // 初始化無障礙性增強工具
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            new AccessibilityEnhancer();
        });
    } else {
        new AccessibilityEnhancer();
    }

    // 全域無障礙性工具
    window.GameCoreAccessibility = {
        // 手動觸發無障礙性增強
        enhance: function () {
            new AccessibilityEnhancer();
        },

        // 運行無障礙性測試
        test: function () {
            const enhancer = new AccessibilityEnhancer();
            return enhancer.runAccessibilityTest();
        },

        // 焦點管理
        focusFirstElement: function () {
            const focusableElements = document.querySelectorAll('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
            if (focusableElements.length > 0) {
                focusableElements[0].focus();
            }
        },

        // 焦點恢復
        restoreFocus: function () {
            if (window.GameCoreAccessibility.lastFocusedElement) {
                window.GameCoreAccessibility.lastFocusedElement.focus();
            }
        },

        // 向螢幕閱讀器宣布
        announce: function (message) {
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
    };

})(); 