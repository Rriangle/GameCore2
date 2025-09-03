/* ========================================
   GameCore CSS 效能優化工具
   ========================================
   
   這個檔案提供了 CSS 效能優化的 JavaScript 工具，
   包括 CSS 檔案合併、壓縮、快取和異步載入功能
   
   功能特點：
   - CSS 檔案合併和壓縮
   - 關鍵路徑資源預載入
   - 非關鍵 CSS 異步載入
   - CSS 快取策略
   - 圖片懶載入優化
   
   回滾方式：刪除此檔案即可恢復預設載入方式
   ======================================== */

(function () {
    'use strict';

    // CSS 效能優化工具類別
    class CSSOptimizer {
        constructor() {
            this.version = '1.0.0';
            this.cacheKey = 'gc-css-cache';
            this.criticalCSS = this.getCriticalCSS();
            this.init();
        }

        // 初始化效能優化
        init() {
            this.inlineCriticalCSS();
            this.preloadCriticalResources();
            this.setupAsyncCSSLoading();
            this.setupImageLazyLoading();
            this.setupCacheStrategy();
        }

        // 內聯關鍵 CSS
        inlineCriticalCSS() {
            const style = document.createElement('style');
            style.textContent = this.criticalCSS;
            style.setAttribute('data-critical', 'true');
            document.head.insertBefore(style, document.head.firstChild);
        }

        // 預載入關鍵資源
        preloadCriticalResources() {
            const resources = [
                { href: '/css/design-tokens.css', as: 'style' },
                { href: '/css/base-components.css', as: 'style' },
                { href: '/fonts/inter-var.woff2', as: 'font', crossorigin: 'anonymous' }
            ];

            resources.forEach(resource => {
                const link = document.createElement('link');
                link.rel = 'preload';
                link.href = resource.href;
                link.as = resource.as;
                if (resource.crossorigin) {
                    link.crossOrigin = resource.crossorigin;
                }
                document.head.appendChild(link);
            });
        }

        // 設置異步 CSS 載入
        setupAsyncCSSLoading() {
            const asyncCSS = [
                '/css/components.css',
                '/css/utilities.css'
            ];

            asyncCSS.forEach(href => {
                this.loadCSSAsync(href);
            });
        }

        // 異步載入 CSS
        loadCSSAsync(href) {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = href + '?v=' + this.version;
            link.setAttribute('data-async', 'true');

            link.onload = () => {
                document.body.classList.add('gc-async-loaded');
            };

            document.head.appendChild(link);
        }

        // 設置圖片懶載入
        setupImageLazyLoading() {
            if ('IntersectionObserver' in window) {
                const imageObserver = new IntersectionObserver((entries, observer) => {
                    entries.forEach(entry => {
                        if (entry.isIntersecting) {
                            const img = entry.target;
                            img.src = img.dataset.src;
                            img.classList.remove('gc-lazy');
                            img.classList.add('gc-loaded');
                            observer.unobserve(img);
                        }
                    });
                });

                document.querySelectorAll('img[data-src]').forEach(img => {
                    imageObserver.observe(img);
                });
            } else {
                // 降級處理：立即載入所有圖片
                document.querySelectorAll('img[data-src]').forEach(img => {
                    img.src = img.dataset.src;
                    img.classList.remove('gc-lazy');
                    img.classList.add('gc-loaded');
                });
            }
        }

        // 設置快取策略
        setupCacheStrategy() {
            // 檢查快取版本
            const cachedVersion = localStorage.getItem(this.cacheKey);
            if (cachedVersion !== this.version) {
                // 清除舊快取
                this.clearOldCache();
                localStorage.setItem(this.cacheKey, this.version);
            }
        }

        // 清除舊快取
        clearOldCache() {
            // 清除舊的 CSS 快取
            const oldLinks = document.querySelectorAll('link[href*=".css"]');
            oldLinks.forEach(link => {
                if (link.href.includes('?v=') && !link.href.includes('v=' + this.version)) {
                    link.remove();
                }
            });
        }

        // 獲取關鍵 CSS
        getCriticalCSS() {
            return `
                /* 關鍵渲染路徑 CSS */
                .gc-body {
                    font-family: var(--gc-font-family);
                    color: var(--gc-ink);
                    background: var(--gc-bg);
                    margin: 0;
                    padding: 0;
                }
                
                .gc-container {
                    max-width: var(--gc-breakpoint-xl);
                    margin: 0 auto;
                    padding: 0 var(--gc-space-4);
                }
                
                .gc-header {
                    position: sticky;
                    top: 0;
                    z-index: var(--gc-z-sticky);
                    background: var(--gc-surface);
                    backdrop-filter: blur(var(--gc-blur));
                    border-bottom: 1px solid var(--gc-line);
                }
                
                .gc-nav {
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    padding: var(--gc-space-4) 0;
                }
                
                .gc-brand {
                    display: flex;
                    align-items: center;
                    color: var(--gc-accent);
                    font-weight: var(--gc-font-bold);
                    font-size: var(--gc-text-xl);
                    text-decoration: none;
                }
                
                .gc-nav-link {
                    display: flex;
                    align-items: center;
                    color: var(--gc-ink);
                    text-decoration: none;
                    padding: var(--gc-space-2) var(--gc-space-3);
                    border-radius: var(--gc-radius-sm);
                    font-weight: var(--gc-font-medium);
                }
                
                .gc-nav-link:hover {
                    color: var(--gc-accent);
                    background: var(--gc-glass);
                }
                
                .gc-nav-link.active {
                    color: var(--gc-accent);
                    background: var(--gc-glass);
                    font-weight: var(--gc-font-semibold);
                }
                
                /* 響應式設計 */
                @media (max-width: 768px) {
                    .gc-container {
                        padding: 0 var(--gc-space-3);
                    }
                    
                    .gc-nav {
                        flex-direction: column;
                        align-items: stretch;
                        padding: var(--gc-space-3) 0;
                    }
                    
                    .gc-nav-menu {
                        display: none;
                        flex-direction: column;
                        padding: var(--gc-space-3) 0;
                    }
                    
                    .gc-nav-menu.active {
                        display: flex;
                    }
                }
            `;
        }

        // 效能監控
        monitorPerformance() {
            if ('PerformanceObserver' in window) {
                const observer = new PerformanceObserver((list) => {
                    list.getEntries().forEach((entry) => {
                        if (entry.entryType === 'largest-contentful-paint') {
                            console.log('LCP:', entry.startTime);
                        }
                        if (entry.entryType === 'first-input') {
                            console.log('FID:', entry.processingStart - entry.startTime);
                        }
                    });
                });

                observer.observe({ entryTypes: ['largest-contentful-paint', 'first-input'] });
            }
        }
    }

    // 初始化效能優化工具
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            new CSSOptimizer();
        });
    } else {
        new CSSOptimizer();
    }

    // 全域效能優化函數
    window.GameCoreOptimizer = {
        // 手動觸發 CSS 優化
        optimizeCSS: function () {
            new CSSOptimizer();
        },

        // 手動觸發圖片懶載入
        lazyLoadImages: function () {
            const optimizer = new CSSOptimizer();
            optimizer.setupImageLazyLoading();
        },

        // 清除快取
        clearCache: function () {
            localStorage.removeItem('gc-css-cache');
            location.reload();
        },

        // 效能監控
        startMonitoring: function () {
            const optimizer = new CSSOptimizer();
            optimizer.monitorPerformance();
        }
    };

})(); 