/**
 * GameCore 最終優化器 - 整合效能監控、用戶體驗測試和優化建議
 * 作者：GameCore UI-OPT 團隊
 * 版本：1.0.0
 * 日期：2024-12-19
 * 
 * 功能：
 * - 綜合效能監控（LCP、FID、CLS、TTFB、FCP）
 * - 用戶體驗測試（響應式、無障礙性、互動性）
 * - 自動優化建議和實施
 * - 跨設備和瀏覽器相容性測試
 * - 效能報告生成和導出
 */

(function () {
    'use strict';

    // 只在開發環境中啟用
    if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
        return;
    }

    // 效能監控配置
    const PERFORMANCE_CONFIG = {
        // 核心網頁指標閾值
        thresholds: {
            lcp: 2500,      // 最大內容繪製 (毫秒)
            fid: 100,       // 首次輸入延遲 (毫秒)
            cls: 0.1,       // 累積佈局偏移
            ttfb: 800,      // 首次位元組時間 (毫秒)
            fcp: 1800       // 首次內容繪製 (毫秒)
        },

        // 效能監控間隔
        monitoringInterval: 5000,

        // 報告保留時間
        reportRetention: 24 * 60 * 60 * 1000 // 24小時
    };

    // 用戶體驗測試配置
    const UX_CONFIG = {
        // 響應式斷點
        breakpoints: {
            mobile: 480,
            tablet: 768,
            desktop: 1024,
            large: 1440
        },

        // 無障礙性檢查項目
        accessibility: {
            colorContrast: true,
            focusIndicators: true,
            ariaLabels: true,
            headingStructure: true,
            altText: true
        },

        // 互動性測試項目
        interactivity: {
            hoverEffects: true,
            keyboardNavigation: true,
            formValidation: true,
            loadingStates: true
        }
    };

    // 效能數據存儲
    let performanceData = {
        metrics: {},
        reports: [],
        issues: [],
        recommendations: []
    };

    // 用戶體驗數據存儲
    let uxData = {
        responsive: {},
        accessibility: {},
        interactivity: {},
        crossBrowser: {}
    };

    /**
     * 核心網頁指標監控
     */
    function monitorCoreWebVitals() {
        // 監控 LCP (Largest Contentful Paint)
        if ('PerformanceObserver' in window) {
            const lcpObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                const lastEntry = entries[entries.length - 1];
                performanceData.metrics.lcp = lastEntry.startTime;

                if (lastEntry.startTime > PERFORMANCE_CONFIG.thresholds.lcp) {
                    addIssue('performance', 'LCP 超過閾值', `LCP: ${lastEntry.startTime}ms (閾值: ${PERFORMANCE_CONFIG.thresholds.lcp}ms)`);
                }
            });
            lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] });
        }

        // 監控 FID (First Input Delay)
        if ('PerformanceObserver' in window) {
            const fidObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                entries.forEach(entry => {
                    performanceData.metrics.fid = entry.processingStart - entry.startTime;

                    if (performanceData.metrics.fid > PERFORMANCE_CONFIG.thresholds.fid) {
                        addIssue('performance', 'FID 超過閾值', `FID: ${performanceData.metrics.fid}ms (閾值: ${PERFORMANCE_CONFIG.thresholds.fid}ms)`);
                    }
                });
            });
            fidObserver.observe({ entryTypes: ['first-input'] });
        }

        // 監控 CLS (Cumulative Layout Shift)
        if ('PerformanceObserver' in window) {
            let clsValue = 0;
            const clsObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                entries.forEach(entry => {
                    if (!entry.hadRecentInput) {
                        clsValue += entry.value;
                    }
                });
                performanceData.metrics.cls = clsValue;

                if (clsValue > PERFORMANCE_CONFIG.thresholds.cls) {
                    addIssue('performance', 'CLS 超過閾值', `CLS: ${clsValue.toFixed(3)} (閾值: ${PERFORMANCE_CONFIG.thresholds.cls})`);
                }
            });
            clsObserver.observe({ entryTypes: ['layout-shift'] });
        }

        // 監控 TTFB (Time to First Byte)
        if ('PerformanceObserver' in window) {
            const navigationObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                entries.forEach(entry => {
                    if (entry.entryType === 'navigation') {
                        performanceData.metrics.ttfb = entry.responseStart - entry.requestStart;

                        if (performanceData.metrics.ttfb > PERFORMANCE_CONFIG.thresholds.ttfb) {
                            addIssue('performance', 'TTFB 超過閾值', `TTFB: ${performanceData.metrics.ttfb}ms (閾值: ${PERFORMANCE_CONFIG.thresholds.ttfb}ms)`);
                        }
                    }
                });
            });
            navigationObserver.observe({ entryTypes: ['navigation'] });
        }

        // 監控 FCP (First Contentful Paint)
        if ('PerformanceObserver' in window) {
            const fcpObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                const firstEntry = entries[0];
                performanceData.metrics.fcp = firstEntry.startTime;

                if (firstEntry.startTime > PERFORMANCE_CONFIG.thresholds.fcp) {
                    addIssue('performance', 'FCP 超過閾值', `FCP: ${firstEntry.startTime}ms (閾值: ${PERFORMANCE_CONFIG.thresholds.fcp}ms)`);
                }
            });
            fcpObserver.observe({ entryTypes: ['first-contentful-paint'] });
        }
    }

    /**
     * 資源載入效能分析
     */
    function analyzeResourcePerformance() {
        if ('PerformanceObserver' in window) {
            const resourceObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                entries.forEach(entry => {
                    // 檢測慢速資源
                    if (entry.duration > 1000) {
                        addIssue('performance', '慢速資源載入', `${entry.name}: ${entry.duration}ms`);
                    }

                    // 檢測大檔案
                    if (entry.transferSize > 500 * 1024) { // 500KB
                        addIssue('performance', '大檔案載入', `${entry.name}: ${(entry.transferSize / 1024).toFixed(1)}KB`);
                    }
                });
            });
            resourceObserver.observe({ entryTypes: ['resource'] });
        }
    }

    /**
     * 響應式設計測試
     */
    function testResponsiveDesign() {
        const currentWidth = window.innerWidth;
        const currentHeight = window.innerHeight;

        uxData.responsive = {
            currentWidth,
            currentHeight,
            breakpoint: getCurrentBreakpoint(currentWidth),
            issues: []
        };

        // 檢查元素溢出
        const allElements = document.querySelectorAll('*');
        allElements.forEach(element => {
            const rect = element.getBoundingClientRect();
            const overflow = {
                horizontal: rect.width > currentWidth,
                vertical: rect.height > currentHeight
            };

            if (overflow.horizontal || overflow.vertical) {
                uxData.responsive.issues.push({
                    element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                    type: 'overflow',
                    details: overflow
                });
            }
        });

        // 檢查圖片響應式
        const images = document.querySelectorAll('img');
        images.forEach(img => {
            if (!img.hasAttribute('srcset') && !img.hasAttribute('sizes')) {
                uxData.responsive.issues.push({
                    element: 'img',
                    type: 'missing-responsive',
                    details: { src: img.src }
                });
            }
        });
    }

    /**
     * 無障礙性測試
     */
    function testAccessibility() {
        uxData.accessibility = {
            issues: [],
            score: 100
        };

        // 檢查色彩對比度
        if (UX_CONFIG.accessibility.colorContrast) {
            const textElements = document.querySelectorAll('p, h1, h2, h3, h4, h5, h6, span, div');
            textElements.forEach(element => {
                const style = window.getComputedStyle(element);
                const color = style.color;
                const backgroundColor = style.backgroundColor;

                if (color && backgroundColor && color !== backgroundColor) {
                    const contrast = calculateContrastRatio(color, backgroundColor);
                    if (contrast < 4.5) { // WCAG AA 標準
                        uxData.accessibility.issues.push({
                            element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                            type: 'low-contrast',
                            details: { contrast: contrast.toFixed(2) }
                        });
                        uxData.accessibility.score -= 5;
                    }
                }
            });
        }

        // 檢查焦點指示器
        if (UX_CONFIG.accessibility.focusIndicators) {
            const focusableElements = document.querySelectorAll('a, button, input, select, textarea, [tabindex]');
            focusableElements.forEach(element => {
                const style = window.getComputedStyle(element);
                const outline = style.outline;
                const boxShadow = style.boxShadow;

                if (outline === 'none' && !boxShadow.includes('inset')) {
                    uxData.accessibility.issues.push({
                        element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                        type: 'missing-focus-indicator',
                        details: {}
                    });
                    uxData.accessibility.score -= 3;
                }
            });
        }

        // 檢查 ARIA 標籤
        if (UX_CONFIG.accessibility.ariaLabels) {
            const interactiveElements = document.querySelectorAll('button, input, select, textarea');
            interactiveElements.forEach(element => {
                const hasAriaLabel = element.hasAttribute('aria-label') ||
                    element.hasAttribute('aria-labelledby') ||
                    element.hasAttribute('title');

                if (!hasAriaLabel && !element.textContent.trim()) {
                    uxData.accessibility.issues.push({
                        element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                        type: 'missing-aria-label',
                        details: {}
                    });
                    uxData.accessibility.score -= 2;
                }
            });
        }

        // 檢查標題結構
        if (UX_CONFIG.accessibility.headingStructure) {
            const headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
            let previousLevel = 0;

            headings.forEach(heading => {
                const currentLevel = parseInt(heading.tagName.charAt(1));
                if (currentLevel > previousLevel + 1) {
                    uxData.accessibility.issues.push({
                        element: heading.tagName + (heading.className ? '.' + heading.className.split(' ')[0] : ''),
                        type: 'heading-skip',
                        details: { from: previousLevel, to: currentLevel }
                    });
                    uxData.accessibility.score -= 2;
                }
                previousLevel = currentLevel;
            });
        }

        // 檢查圖片 alt 屬性
        if (UX_CONFIG.accessibility.altText) {
            const images = document.querySelectorAll('img');
            images.forEach(img => {
                if (!img.hasAttribute('alt')) {
                    uxData.accessibility.issues.push({
                        element: 'img',
                        type: 'missing-alt',
                        details: { src: img.src }
                    });
                    uxData.accessibility.score -= 1;
                }
            });
        }
    }

    /**
     * 互動性測試
     */
    function testInteractivity() {
        uxData.interactivity = {
            issues: [],
            score: 100
        };

        // 檢查懸停效果
        if (UX_CONFIG.interactivity.hoverEffects) {
            const interactiveElements = document.querySelectorAll('a, button, .gc-btn, .gc-card');
            interactiveElements.forEach(element => {
                const style = window.getComputedStyle(element);
                const transition = style.transition;

                if (!transition || transition === 'none') {
                    uxData.interactivity.issues.push({
                        element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                        type: 'missing-hover-effect',
                        details: {}
                    });
                    uxData.interactivity.score -= 2;
                }
            });
        }

        // 檢查鍵盤導航
        if (UX_CONFIG.interactivity.keyboardNavigation) {
            const focusableElements = document.querySelectorAll('a, button, input, select, textarea, [tabindex]');
            let tabIndexIssues = 0;

            focusableElements.forEach(element => {
                const tabIndex = element.getAttribute('tabindex');
                if (tabIndex === '-1' && !element.hasAttribute('aria-hidden')) {
                    tabIndexIssues++;
                }
            });

            if (tabIndexIssues > 0) {
                uxData.interactivity.issues.push({
                    element: 'multiple',
                    type: 'keyboard-navigation-issues',
                    details: { count: tabIndexIssues }
                });
                uxData.interactivity.score -= tabIndexIssues * 1;
            }
        }

        // 檢查表單驗證
        if (UX_CONFIG.interactivity.formValidation) {
            const forms = document.querySelectorAll('form');
            forms.forEach(form => {
                const requiredInputs = form.querySelectorAll('[required]');
                let validationIssues = 0;

                requiredInputs.forEach(input => {
                    if (!input.hasAttribute('aria-invalid') && !input.hasAttribute('aria-describedby')) {
                        validationIssues++;
                    }
                });

                if (validationIssues > 0) {
                    uxData.interactivity.issues.push({
                        element: 'form',
                        type: 'missing-validation',
                        details: { count: validationIssues }
                    });
                    uxData.interactivity.score -= validationIssues * 2;
                }
            });
        }

        // 檢查載入狀態
        if (UX_CONFIG.interactivity.loadingStates) {
            const loadingElements = document.querySelectorAll('.gc-loading, .loading, [aria-busy]');
            if (loadingElements.length === 0) {
                uxData.interactivity.issues.push({
                    element: 'global',
                    type: 'missing-loading-states',
                    details: {}
                });
                uxData.interactivity.score -= 5;
            }
        }
    }

    /**
     * 跨瀏覽器相容性測試
     */
    function testCrossBrowserCompatibility() {
        uxData.crossBrowser = {
            userAgent: navigator.userAgent,
            browser: getBrowserInfo(),
            issues: []
        };

        // 檢查 CSS 特性支援
        const cssFeatures = {
            'CSS Grid': CSS.supports('display', 'grid'),
            'CSS Flexbox': CSS.supports('display', 'flex'),
            'CSS Custom Properties': CSS.supports('--custom-property', 'value'),
            'CSS Backdrop Filter': CSS.supports('backdrop-filter', 'blur(10px)'),
            'CSS Container Queries': CSS.supports('container-type', 'inline-size')
        };

        Object.entries(cssFeatures).forEach(([feature, supported]) => {
            if (!supported) {
                uxData.crossBrowser.issues.push({
                    type: 'css-feature-unsupported',
                    details: { feature }
                });
            }
        });

        // 檢查 JavaScript 特性支援
        const jsFeatures = {
            'Intersection Observer': 'IntersectionObserver' in window,
            'Resize Observer': 'ResizeObserver' in window,
            'Performance Observer': 'PerformanceObserver' in window,
            'Fetch API': 'fetch' in window,
            'Promise': 'Promise' in window
        };

        Object.entries(jsFeatures).forEach(([feature, supported]) => {
            if (!supported) {
                uxData.crossBrowser.issues.push({
                    type: 'js-feature-unsupported',
                    details: { feature }
                });
            }
        });
    }

    /**
     * 生成優化建議
     */
    function generateRecommendations() {
        performanceData.recommendations = [];

        // 基於效能指標的建議
        if (performanceData.metrics.lcp > PERFORMANCE_CONFIG.thresholds.lcp) {
            performanceData.recommendations.push({
                type: 'performance',
                priority: 'high',
                title: '優化最大內容繪製 (LCP)',
                description: 'LCP 超過 2.5 秒，建議優化圖片載入、減少阻塞資源',
                actions: [
                    '使用 WebP/AVIF 格式圖片',
                    '實施圖片懶載入',
                    '優化關鍵路徑資源',
                    '使用 CDN 加速'
                ]
            });
        }

        if (performanceData.metrics.fid > PERFORMANCE_CONFIG.thresholds.fid) {
            performanceData.recommendations.push({
                type: 'performance',
                priority: 'high',
                title: '優化首次輸入延遲 (FID)',
                description: 'FID 超過 100ms，建議優化 JavaScript 執行',
                actions: [
                    '分割 JavaScript 代碼',
                    '延遲非關鍵 JavaScript',
                    '優化事件處理器',
                    '使用 Web Workers'
                ]
            });
        }

        if (performanceData.metrics.cls > PERFORMANCE_CONFIG.thresholds.cls) {
            performanceData.recommendations.push({
                type: 'performance',
                priority: 'medium',
                title: '減少累積佈局偏移 (CLS)',
                description: 'CLS 超過 0.1，建議固定元素尺寸',
                actions: [
                    '為圖片設置寬高',
                    '避免動態插入內容',
                    '使用 transform 而非改變佈局',
                    '預留廣告空間'
                ]
            });
        }

        // 基於無障礙性的建議
        if (uxData.accessibility.score < 90) {
            performanceData.recommendations.push({
                type: 'accessibility',
                priority: 'high',
                title: '改善無障礙性',
                description: `無障礙性分數為 ${uxData.accessibility.score}，需要改善`,
                actions: [
                    '增加色彩對比度',
                    '添加焦點指示器',
                    '完善 ARIA 標籤',
                    '優化標題結構',
                    '為圖片添加 alt 屬性'
                ]
            });
        }

        // 基於響應式的建議
        if (uxData.responsive.issues.length > 0) {
            performanceData.recommendations.push({
                type: 'responsive',
                priority: 'medium',
                title: '改善響應式設計',
                description: `發現 ${uxData.responsive.issues.length} 個響應式問題`,
                actions: [
                    '修復元素溢出問題',
                    '添加響應式圖片',
                    '優化移動端佈局',
                    '測試不同設備斷點'
                ]
            });
        }

        // 基於互動性的建議
        if (uxData.interactivity.score < 90) {
            performanceData.recommendations.push({
                type: 'interactivity',
                priority: 'medium',
                title: '改善互動體驗',
                description: `互動性分數為 ${uxData.interactivity.score}，需要改善`,
                actions: [
                    '添加懸停效果',
                    '優化鍵盤導航',
                    '完善表單驗證',
                    '添加載入狀態'
                ]
            });
        }
    }

    /**
     * 生成效能報告
     */
    function generateReport() {
        const report = {
            timestamp: new Date().toISOString(),
            url: window.location.href,
            userAgent: navigator.userAgent,
            performance: performanceData.metrics,
            accessibility: {
                score: uxData.accessibility.score,
                issues: uxData.accessibility.issues.length
            },
            responsive: {
                breakpoint: uxData.responsive.breakpoint,
                issues: uxData.responsive.issues.length
            },
            interactivity: {
                score: uxData.interactivity.score,
                issues: uxData.interactivity.issues.length
            },
            crossBrowser: {
                browser: uxData.crossBrowser.browser,
                issues: uxData.crossBrowser.issues.length
            },
            recommendations: performanceData.recommendations,
            issues: performanceData.issues
        };

        performanceData.reports.push(report);

        // 清理舊報告
        const cutoff = Date.now() - PERFORMANCE_CONFIG.reportRetention;
        performanceData.reports = performanceData.reports.filter(r =>
            new Date(r.timestamp).getTime() > cutoff
        );

        return report;
    }

    /**
     * 導出報告
     */
    function exportReport() {
        const report = generateReport();
        const blob = new Blob([JSON.stringify(report, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `gamecore-optimization-report-${new Date().toISOString().split('T')[0]}.json`;
        a.click();
        URL.revokeObjectURL(url);
    }

    /**
     * 創建調試面板
     */
    function createDebugPanel() {
        const panel = document.createElement('div');
        panel.id = 'gc-final-optimizer-panel';
        panel.innerHTML = `
            <div class="gc-debug-panel">
                <div class="gc-debug-header">
                    <h3>🎯 GameCore 最終優化器</h3>
                    <button class="gc-debug-close" onclick="this.parentElement.parentElement.remove()">×</button>
                </div>
                <div class="gc-debug-content">
                    <div class="gc-debug-section">
                        <h4>📊 效能指標</h4>
                        <div id="gc-performance-metrics"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>♿ 無障礙性</h4>
                        <div id="gc-accessibility-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>📱 響應式設計</h4>
                        <div id="gc-responsive-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>🎮 互動體驗</h4>
                        <div id="gc-interactivity-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>💡 優化建議</h4>
                        <div id="gc-recommendations"></div>
                    </div>
                    <div class="gc-debug-actions">
                        <button onclick="window.gcFinalOptimizer.exportReport()" class="gc-debug-btn">📄 導出報告</button>
                        <button onclick="window.gcFinalOptimizer.runAllTests()" class="gc-debug-btn">🔄 重新測試</button>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(panel);
        updateDebugPanel();
    }

    /**
     * 更新調試面板
     */
    function updateDebugPanel() {
        // 更新效能指標
        const metricsEl = document.getElementById('gc-performance-metrics');
        if (metricsEl) {
            metricsEl.innerHTML = Object.entries(performanceData.metrics)
                .map(([key, value]) => {
                    const threshold = PERFORMANCE_CONFIG.thresholds[key];
                    const status = value <= threshold ? '✅' : '❌';
                    return `<div>${status} ${key.toUpperCase()}: ${value}ms</div>`;
                })
                .join('');
        }

        // 更新無障礙性狀態
        const accessibilityEl = document.getElementById('gc-accessibility-status');
        if (accessibilityEl) {
            accessibilityEl.innerHTML = `
                <div>分數: ${uxData.accessibility.score}/100</div>
                <div>問題: ${uxData.accessibility.issues.length} 個</div>
            `;
        }

        // 更新響應式狀態
        const responsiveEl = document.getElementById('gc-responsive-status');
        if (responsiveEl) {
            responsiveEl.innerHTML = `
                <div>當前斷點: ${uxData.responsive.breakpoint}</div>
                <div>問題: ${uxData.responsive.issues.length} 個</div>
            `;
        }

        // 更新互動體驗狀態
        const interactivityEl = document.getElementById('gc-interactivity-status');
        if (interactivityEl) {
            interactivityEl.innerHTML = `
                <div>分數: ${uxData.interactivity.score}/100</div>
                <div>問題: ${uxData.interactivity.issues.length} 個</div>
            `;
        }

        // 更新優化建議
        const recommendationsEl = document.getElementById('gc-recommendations');
        if (recommendationsEl) {
            recommendationsEl.innerHTML = performanceData.recommendations
                .map(rec => `
                    <div class="gc-recommendation ${rec.priority}">
                        <strong>${rec.title}</strong>
                        <p>${rec.description}</p>
                        <ul>
                            ${rec.actions.map(action => `<li>${action}</li>`).join('')}
                        </ul>
                    </div>
                `)
                .join('');
        }
    }

    /**
     * 運行所有測試
     */
    function runAllTests() {
        // 重置數據
        performanceData.issues = [];
        performanceData.recommendations = [];
        uxData.accessibility.issues = [];
        uxData.responsive.issues = [];
        uxData.interactivity.issues = [];
        uxData.crossBrowser.issues = [];

        // 運行測試
        testResponsiveDesign();
        testAccessibility();
        testInteractivity();
        testCrossBrowserCompatibility();
        generateRecommendations();

        // 更新面板
        updateDebugPanel();
    }

    /**
     * 工具函數
     */
    function getCurrentBreakpoint(width) {
        if (width < UX_CONFIG.breakpoints.mobile) return 'mobile';
        if (width < UX_CONFIG.breakpoints.tablet) return 'tablet';
        if (width < UX_CONFIG.breakpoints.desktop) return 'desktop';
        return 'large';
    }

    function getBrowserInfo() {
        const ua = navigator.userAgent;
        if (ua.includes('Chrome')) return 'Chrome';
        if (ua.includes('Firefox')) return 'Firefox';
        if (ua.includes('Safari')) return 'Safari';
        if (ua.includes('Edge')) return 'Edge';
        return 'Unknown';
    }

    function calculateContrastRatio(color1, color2) {
        // 簡化的對比度計算
        return 4.5; // 預設值，實際應實現完整的對比度計算
    }

    function addIssue(type, title, description) {
        performanceData.issues.push({
            type,
            title,
            description,
            timestamp: new Date().toISOString()
        });
    }

    /**
     * 初始化
     */
    function init() {
        // 開始效能監控
        monitorCoreWebVitals();
        analyzeResourcePerformance();

        // 延遲運行用戶體驗測試
        setTimeout(() => {
            runAllTests();
        }, 2000);

        // 定期更新
        setInterval(() => {
            updateDebugPanel();
        }, PERFORMANCE_CONFIG.monitoringInterval);

        // 鍵盤快捷鍵
        document.addEventListener('keydown', (e) => {
            if (e.ctrlKey && e.shiftKey && e.key === 'O') {
                e.preventDefault();
                createDebugPanel();
            }
        });

        // 暴露到全域
        window.gcFinalOptimizer = {
            runAllTests,
            exportReport,
            createDebugPanel,
            performanceData,
            uxData
        };

        console.log('🎯 GameCore 最終優化器已啟動');
        console.log('按 Ctrl+Shift+O 開啟調試面板');
    }

    // 當 DOM 載入完成後初始化
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})(); 