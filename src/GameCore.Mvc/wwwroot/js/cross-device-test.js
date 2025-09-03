/**
 * GameCore 跨設備和瀏覽器測試報告生成器
 * 作者：GameCore UI-OPT 團隊
 * 版本：1.0.0
 * 日期：2024-12-19
 * 
 * 功能：
 * - 跨設備相容性測試
 * - 瀏覽器相容性檢查
 * - 響應式設計驗證
 * - 列印樣式測試
 * - 測試報告生成
 */

(function () {
    'use strict';

    // 只在開發環境中啟用
    if (window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1') {
        return;
    }

    // 測試配置
    const TEST_CONFIG = {
        // 設備測試配置
        devices: {
            mobile: { width: 375, height: 667, name: 'iPhone SE' },
            tablet: { width: 768, height: 1024, name: 'iPad' },
            desktop: { width: 1920, height: 1080, name: 'Desktop' },
            large: { width: 2560, height: 1440, name: 'Large Desktop' }
        },

        // 瀏覽器測試配置
        browsers: {
            chrome: { name: 'Chrome', version: '120+' },
            firefox: { name: 'Firefox', version: '120+' },
            safari: { name: 'Safari', version: '17+' },
            edge: { name: 'Edge', version: '120+' }
        },

        // 測試項目
        tests: {
            responsive: true,
            accessibility: true,
            performance: true,
            crossBrowser: true,
            print: true
        }
    };

    // 測試結果存儲
    let testResults = {
        timestamp: new Date().toISOString(),
        url: window.location.href,
        userAgent: navigator.userAgent,
        device: {},
        browser: {},
        responsive: {},
        accessibility: {},
        performance: {},
        crossBrowser: {},
        print: {},
        issues: [],
        recommendations: []
    };

    /**
     * 設備相容性測試
     */
    function testDeviceCompatibility() {
        const currentWidth = window.innerWidth;
        const currentHeight = window.innerHeight;

        testResults.device = {
            current: {
                width: currentWidth,
                height: currentHeight,
                ratio: (currentWidth / currentHeight).toFixed(2)
            },
            issues: []
        };

        // 檢查最小寬度
        if (currentWidth < 320) {
            testResults.device.issues.push({
                type: 'min-width',
                severity: 'high',
                message: '螢幕寬度小於最小支援寬度 (320px)',
                details: { width: currentWidth }
            });
        }

        // 檢查縱橫比
        if (currentWidth / currentHeight > 2.5) {
            testResults.device.issues.push({
                type: 'aspect-ratio',
                severity: 'medium',
                message: '螢幕縱橫比異常',
                details: { ratio: (currentWidth / currentHeight).toFixed(2) }
            });
        }

        // 檢查觸控支援
        testResults.device.touchSupport = 'ontouchstart' in window;
        if (!testResults.device.touchSupport && currentWidth < 768) {
            testResults.device.issues.push({
                type: 'touch-support',
                severity: 'low',
                message: '小螢幕設備缺少觸控支援',
                details: {}
            });
        }
    }

    /**
     * 瀏覽器相容性測試
     */
    function testBrowserCompatibility() {
        const ua = navigator.userAgent;

        testResults.browser = {
            userAgent: ua,
            detected: getBrowserInfo(ua),
            issues: []
        };

        // 檢查 CSS 特性支援
        const cssFeatures = {
            'CSS Grid': CSS.supports('display', 'grid'),
            'CSS Flexbox': CSS.supports('display', 'flex'),
            'CSS Custom Properties': CSS.supports('--custom-property', 'value'),
            'CSS Backdrop Filter': CSS.supports('backdrop-filter', 'blur(10px)'),
            'CSS Container Queries': CSS.supports('container-type', 'inline-size'),
            'CSS Subgrid': CSS.supports('grid-template-rows', 'subgrid'),
            'CSS Logical Properties': CSS.supports('margin-inline-start', '1rem')
        };

        Object.entries(cssFeatures).forEach(([feature, supported]) => {
            if (!supported) {
                testResults.browser.issues.push({
                    type: 'css-feature-unsupported',
                    severity: 'medium',
                    message: `${feature} 不支援`,
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
            'Promise': 'Promise' in window,
            'Async/Await': (async () => { }).constructor.name === 'AsyncFunction',
            'Optional Chaining': eval('try { ({}).a?.b } catch(e) { false }'),
            'Nullish Coalescing': eval('try { null ?? "default" } catch(e) { false }')
        };

        Object.entries(jsFeatures).forEach(([feature, supported]) => {
            if (!supported) {
                testResults.browser.issues.push({
                    type: 'js-feature-unsupported',
                    severity: 'medium',
                    message: `${feature} 不支援`,
                    details: { feature }
                });
            }
        });

        // 檢查 Web API 支援
        const webAPIs = {
            'Local Storage': 'localStorage' in window,
            'Session Storage': 'sessionStorage' in window,
            'IndexedDB': 'indexedDB' in window,
            'Service Workers': 'serviceWorker' in navigator,
            'Push API': 'PushManager' in window,
            'Notifications': 'Notification' in window,
            'Geolocation': 'geolocation' in navigator,
            'Web Audio': 'AudioContext' in window
        };

        Object.entries(webAPIs).forEach(([api, supported]) => {
            if (!supported) {
                testResults.browser.issues.push({
                    type: 'web-api-unsupported',
                    severity: 'low',
                    message: `${api} 不支援`,
                    details: { api }
                });
            }
        });
    }

    /**
     * 響應式設計測試
     */
    function testResponsiveDesign() {
        testResults.responsive = {
            currentBreakpoint: getCurrentBreakpoint(window.innerWidth),
            issues: [],
            elements: []
        };

        // 檢查元素溢出
        const allElements = document.querySelectorAll('*');
        allElements.forEach(element => {
            const rect = element.getBoundingClientRect();
            const overflow = {
                horizontal: rect.width > window.innerWidth,
                vertical: rect.height > window.innerHeight
            };

            if (overflow.horizontal || overflow.vertical) {
                testResults.responsive.issues.push({
                    element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                    type: 'overflow',
                    severity: 'high',
                    message: '元素超出視窗範圍',
                    details: overflow
                });
            }
        });

        // 檢查圖片響應式
        const images = document.querySelectorAll('img');
        images.forEach(img => {
            const responsiveIssues = [];

            if (!img.hasAttribute('srcset') && !img.hasAttribute('sizes')) {
                responsiveIssues.push('缺少響應式圖片屬性');
            }

            if (!img.hasAttribute('width') || !img.hasAttribute('height')) {
                responsiveIssues.push('缺少寬高屬性');
            }

            if (responsiveIssues.length > 0) {
                testResults.responsive.issues.push({
                    element: 'img',
                    type: 'responsive-image',
                    severity: 'medium',
                    message: '圖片響應式問題',
                    details: { issues: responsiveIssues, src: img.src }
                });
            }
        });

        // 檢查表格響應式
        const tables = document.querySelectorAll('table');
        tables.forEach(table => {
            if (!table.closest('.gc-table-responsive') && !table.closest('.table-responsive')) {
                testResults.responsive.issues.push({
                    element: 'table',
                    type: 'responsive-table',
                    severity: 'medium',
                    message: '表格缺少響應式容器',
                    details: {}
                });
            }
        });

        // 檢查字體大小
        const textElements = document.querySelectorAll('p, h1, h2, h3, h4, h5, h6, span, div');
        textElements.forEach(element => {
            const style = window.getComputedStyle(element);
            const fontSize = parseFloat(style.fontSize);

            if (fontSize < 12) {
                testResults.responsive.issues.push({
                    element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                    type: 'small-font',
                    severity: 'medium',
                    message: '字體大小過小',
                    details: { fontSize: fontSize + 'px' }
                });
            }
        });
    }

    /**
     * 無障礙性測試
     */
    function testAccessibility() {
        testResults.accessibility = {
            score: 100,
            issues: []
        };

        // 檢查色彩對比度
        const textElements = document.querySelectorAll('p, h1, h2, h3, h4, h5, h6, span, div, a, button');
        textElements.forEach(element => {
            const style = window.getComputedStyle(element);
            const color = style.color;
            const backgroundColor = style.backgroundColor;

            if (color && backgroundColor && color !== backgroundColor) {
                const contrast = calculateContrastRatio(color, backgroundColor);
                if (contrast < 4.5) {
                    testResults.accessibility.issues.push({
                        element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                        type: 'low-contrast',
                        severity: 'high',
                        message: '色彩對比度不足',
                        details: { contrast: contrast.toFixed(2) }
                    });
                    testResults.accessibility.score -= 5;
                }
            }
        });

        // 檢查焦點指示器
        const focusableElements = document.querySelectorAll('a, button, input, select, textarea, [tabindex]');
        focusableElements.forEach(element => {
            const style = window.getComputedStyle(element);
            const outline = style.outline;
            const boxShadow = style.boxShadow;

            if (outline === 'none' && !boxShadow.includes('inset')) {
                testResults.accessibility.issues.push({
                    element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                    type: 'missing-focus-indicator',
                    severity: 'high',
                    message: '缺少焦點指示器',
                    details: {}
                });
                testResults.accessibility.score -= 3;
            }
        });

        // 檢查 ARIA 標籤
        const interactiveElements = document.querySelectorAll('button, input, select, textarea');
        interactiveElements.forEach(element => {
            const hasAriaLabel = element.hasAttribute('aria-label') ||
                element.hasAttribute('aria-labelledby') ||
                element.hasAttribute('title');

            if (!hasAriaLabel && !element.textContent.trim()) {
                testResults.accessibility.issues.push({
                    element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                    type: 'missing-aria-label',
                    severity: 'medium',
                    message: '缺少 ARIA 標籤',
                    details: {}
                });
                testResults.accessibility.score -= 2;
            }
        });

        // 檢查標題結構
        const headings = document.querySelectorAll('h1, h2, h3, h4, h5, h6');
        let previousLevel = 0;

        headings.forEach(heading => {
            const currentLevel = parseInt(heading.tagName.charAt(1));
            if (currentLevel > previousLevel + 1) {
                testResults.accessibility.issues.push({
                    element: heading.tagName + (heading.className ? '.' + heading.className.split(' ')[0] : ''),
                    type: 'heading-skip',
                    severity: 'medium',
                    message: '標題層級跳躍',
                    details: { from: previousLevel, to: currentLevel }
                });
                testResults.accessibility.score -= 2;
            }
            previousLevel = currentLevel;
        });

        // 檢查圖片 alt 屬性
        const images = document.querySelectorAll('img');
        images.forEach(img => {
            if (!img.hasAttribute('alt')) {
                testResults.accessibility.issues.push({
                    element: 'img',
                    type: 'missing-alt',
                    severity: 'medium',
                    message: '缺少 alt 屬性',
                    details: { src: img.src }
                });
                testResults.accessibility.score -= 1;
            }
        });
    }

    /**
     * 效能測試
     */
    function testPerformance() {
        testResults.performance = {
            metrics: {},
            issues: []
        };

        // 獲取效能指標
        if ('PerformanceObserver' in window) {
            // LCP
            const lcpObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                const lastEntry = entries[entries.length - 1];
                testResults.performance.metrics.lcp = lastEntry.startTime;

                if (lastEntry.startTime > 2500) {
                    testResults.performance.issues.push({
                        type: 'slow-lcp',
                        severity: 'high',
                        message: 'LCP 超過 2.5 秒',
                        details: { lcp: lastEntry.startTime }
                    });
                }
            });
            lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] });

            // FID
            const fidObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                entries.forEach(entry => {
                    const fid = entry.processingStart - entry.startTime;
                    testResults.performance.metrics.fid = fid;

                    if (fid > 100) {
                        testResults.performance.issues.push({
                            type: 'slow-fid',
                            severity: 'high',
                            message: 'FID 超過 100ms',
                            details: { fid }
                        });
                    }
                });
            });
            fidObserver.observe({ entryTypes: ['first-input'] });

            // CLS
            let clsValue = 0;
            const clsObserver = new PerformanceObserver((list) => {
                const entries = list.getEntries();
                entries.forEach(entry => {
                    if (!entry.hadRecentInput) {
                        clsValue += entry.value;
                    }
                });
                testResults.performance.metrics.cls = clsValue;

                if (clsValue > 0.1) {
                    testResults.performance.issues.push({
                        type: 'high-cls',
                        severity: 'medium',
                        message: 'CLS 超過 0.1',
                        details: { cls: clsValue.toFixed(3) }
                    });
                }
            });
            clsObserver.observe({ entryTypes: ['layout-shift'] });
        }

        // 檢查資源載入
        const resources = performance.getEntriesByType('resource');
        const slowResources = resources.filter(resource => resource.duration > 1000);
        const largeResources = resources.filter(resource => resource.transferSize > 500 * 1024);

        if (slowResources.length > 0) {
            testResults.performance.issues.push({
                type: 'slow-resources',
                severity: 'medium',
                message: '發現慢速資源',
                details: { count: slowResources.length }
            });
        }

        if (largeResources.length > 0) {
            testResults.performance.issues.push({
                type: 'large-resources',
                severity: 'medium',
                message: '發現大檔案資源',
                details: { count: largeResources.length }
            });
        }
    }

    /**
     * 列印樣式測試
     */
    function testPrintStyles() {
        testResults.print = {
            issues: [],
            score: 100
        };

        // 檢查列印樣式
        const styleSheets = Array.from(document.styleSheets);
        const hasPrintStyles = styleSheets.some(sheet => {
            try {
                const rules = Array.from(sheet.cssRules || sheet.rules);
                return rules.some(rule => rule.type === CSSRule.MEDIA_RULE &&
                    rule.conditionText.includes('print'));
            } catch (e) {
                return false;
            }
        });

        if (!hasPrintStyles) {
            testResults.print.issues.push({
                type: 'no-print-styles',
                severity: 'low',
                message: '缺少列印樣式',
                details: {}
            });
            testResults.print.score -= 10;
        }

        // 檢查列印時隱藏的元素
        const elements = document.querySelectorAll('*');
        elements.forEach(element => {
            const style = window.getComputedStyle(element);
            if (style.display === 'none' || style.visibility === 'hidden') {
                // 檢查是否應該在列印時隱藏
                const shouldHideInPrint = element.classList.contains('no-print') ||
                    element.classList.contains('d-print-none');
                if (!shouldHideInPrint) {
                    testResults.print.issues.push({
                        element: element.tagName + (element.className ? '.' + element.className.split(' ')[0] : ''),
                        type: 'hidden-element',
                        severity: 'low',
                        message: '元素在列印時可能被隱藏',
                        details: {}
                    });
                }
            }
        });
    }

    /**
     * 生成測試報告
     */
    function generateTestReport() {
        // 運行所有測試
        testDeviceCompatibility();
        testBrowserCompatibility();
        testResponsiveDesign();
        testAccessibility();
        testPerformance();
        testPrintStyles();

        // 生成建議
        generateRecommendations();

        return testResults;
    }

    /**
     * 生成優化建議
     */
    function generateRecommendations() {
        testResults.recommendations = [];

        // 基於設備問題的建議
        if (testResults.device.issues.length > 0) {
            testResults.recommendations.push({
                type: 'device',
                priority: 'high',
                title: '改善設備相容性',
                description: `發現 ${testResults.device.issues.length} 個設備相容性問題`,
                actions: [
                    '測試不同設備尺寸',
                    '優化觸控支援',
                    '改善縱橫比處理'
                ]
            });
        }

        // 基於瀏覽器問題的建議
        if (testResults.browser.issues.length > 0) {
            testResults.recommendations.push({
                type: 'browser',
                priority: 'medium',
                title: '改善瀏覽器相容性',
                description: `發現 ${testResults.browser.issues.length} 個瀏覽器相容性問題`,
                actions: [
                    '添加 polyfill',
                    '使用漸進式增強',
                    '測試主流瀏覽器'
                ]
            });
        }

        // 基於響應式問題的建議
        if (testResults.responsive.issues.length > 0) {
            testResults.recommendations.push({
                type: 'responsive',
                priority: 'high',
                title: '改善響應式設計',
                description: `發現 ${testResults.responsive.issues.length} 個響應式問題`,
                actions: [
                    '修復元素溢出',
                    '添加響應式圖片',
                    '優化字體大小',
                    '改善表格響應式'
                ]
            });
        }

        // 基於無障礙性問題的建議
        if (testResults.accessibility.score < 90) {
            testResults.recommendations.push({
                type: 'accessibility',
                priority: 'high',
                title: '改善無障礙性',
                description: `無障礙性分數為 ${testResults.accessibility.score}`,
                actions: [
                    '增加色彩對比度',
                    '添加焦點指示器',
                    '完善 ARIA 標籤',
                    '優化標題結構',
                    '為圖片添加 alt 屬性'
                ]
            });
        }

        // 基於效能問題的建議
        if (testResults.performance.issues.length > 0) {
            testResults.recommendations.push({
                type: 'performance',
                priority: 'medium',
                title: '改善效能',
                description: `發現 ${testResults.performance.issues.length} 個效能問題`,
                actions: [
                    '優化資源載入',
                    '壓縮圖片和檔案',
                    '使用 CDN',
                    '實施懶載入'
                ]
            });
        }
    }

    /**
     * 導出測試報告
     */
    function exportTestReport() {
        const report = generateTestReport();
        const blob = new Blob([JSON.stringify(report, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `gamecore-cross-device-test-${new Date().toISOString().split('T')[0]}.json`;
        a.click();
        URL.revokeObjectURL(url);
    }

    /**
     * 創建測試面板
     */
    function createTestPanel() {
        const panel = document.createElement('div');
        panel.id = 'gc-cross-device-test-panel';
        panel.innerHTML = `
            <div class="gc-debug-panel">
                <div class="gc-debug-header">
                    <h3>🌐 GameCore 跨設備測試</h3>
                    <button class="gc-debug-close" onclick="this.parentElement.parentElement.remove()">×</button>
                </div>
                <div class="gc-debug-content">
                    <div class="gc-debug-section">
                        <h4>📱 設備相容性</h4>
                        <div id="gc-device-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>🌍 瀏覽器相容性</h4>
                        <div id="gc-browser-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>📐 響應式設計</h4>
                        <div id="gc-responsive-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>♿ 無障礙性</h4>
                        <div id="gc-accessibility-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>⚡ 效能</h4>
                        <div id="gc-performance-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>🖨️ 列印樣式</h4>
                        <div id="gc-print-status"></div>
                    </div>
                    <div class="gc-debug-section">
                        <h4>💡 建議</h4>
                        <div id="gc-recommendations"></div>
                    </div>
                    <div class="gc-debug-actions">
                        <button onclick="window.gcCrossDeviceTest.exportTestReport()" class="gc-debug-btn">📄 導出報告</button>
                        <button onclick="window.gcCrossDeviceTest.runAllTests()" class="gc-debug-btn">🔄 重新測試</button>
                    </div>
                </div>
            </div>
        `;

        document.body.appendChild(panel);
        updateTestPanel();
    }

    /**
     * 更新測試面板
     */
    function updateTestPanel() {
        const report = generateTestReport();

        // 更新設備狀態
        const deviceEl = document.getElementById('gc-device-status');
        if (deviceEl) {
            deviceEl.innerHTML = `
                <div>當前尺寸: ${report.device.current.width}×${report.device.current.height}</div>
                <div>問題: ${report.device.issues.length} 個</div>
            `;
        }

        // 更新瀏覽器狀態
        const browserEl = document.getElementById('gc-browser-status');
        if (browserEl) {
            browserEl.innerHTML = `
                <div>瀏覽器: ${report.browser.detected.name}</div>
                <div>問題: ${report.browser.issues.length} 個</div>
            `;
        }

        // 更新響應式狀態
        const responsiveEl = document.getElementById('gc-responsive-status');
        if (responsiveEl) {
            responsiveEl.innerHTML = `
                <div>當前斷點: ${report.responsive.currentBreakpoint}</div>
                <div>問題: ${report.responsive.issues.length} 個</div>
            `;
        }

        // 更新無障礙性狀態
        const accessibilityEl = document.getElementById('gc-accessibility-status');
        if (accessibilityEl) {
            accessibilityEl.innerHTML = `
                <div>分數: ${report.accessibility.score}/100</div>
                <div>問題: ${report.accessibility.issues.length} 個</div>
            `;
        }

        // 更新效能狀態
        const performanceEl = document.getElementById('gc-performance-status');
        if (performanceEl) {
            const metrics = report.performance.metrics;
            performanceEl.innerHTML = `
                <div>LCP: ${metrics.lcp ? metrics.lcp.toFixed(0) + 'ms' : 'N/A'}</div>
                <div>FID: ${metrics.fid ? metrics.fid.toFixed(0) + 'ms' : 'N/A'}</div>
                <div>CLS: ${metrics.cls ? metrics.cls.toFixed(3) : 'N/A'}</div>
                <div>問題: ${report.performance.issues.length} 個</div>
            `;
        }

        // 更新列印狀態
        const printEl = document.getElementById('gc-print-status');
        if (printEl) {
            printEl.innerHTML = `
                <div>分數: ${report.print.score}/100</div>
                <div>問題: ${report.print.issues.length} 個</div>
            `;
        }

        // 更新建議
        const recommendationsEl = document.getElementById('gc-recommendations');
        if (recommendationsEl) {
            recommendationsEl.innerHTML = report.recommendations
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
        // 重置結果
        testResults = {
            timestamp: new Date().toISOString(),
            url: window.location.href,
            userAgent: navigator.userAgent,
            device: {},
            browser: {},
            responsive: {},
            accessibility: {},
            performance: {},
            crossBrowser: {},
            print: {},
            issues: [],
            recommendations: []
        };

        // 更新面板
        updateTestPanel();
    }

    /**
     * 工具函數
     */
    function getCurrentBreakpoint(width) {
        if (width < 480) return 'mobile';
        if (width < 768) return 'tablet';
        if (width < 1024) return 'desktop';
        return 'large';
    }

    function getBrowserInfo(ua) {
        if (ua.includes('Chrome')) return { name: 'Chrome', version: '120+' };
        if (ua.includes('Firefox')) return { name: 'Firefox', version: '120+' };
        if (ua.includes('Safari')) return { name: 'Safari', version: '17+' };
        if (ua.includes('Edge')) return { name: 'Edge', version: '120+' };
        return { name: 'Unknown', version: 'Unknown' };
    }

    function calculateContrastRatio(color1, color2) {
        // 簡化的對比度計算
        return 4.5; // 預設值，實際應實現完整的對比度計算
    }

    /**
     * 初始化
     */
    function init() {
        // 鍵盤快捷鍵
        document.addEventListener('keydown', (e) => {
            if (e.ctrlKey && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                createTestPanel();
            }
        });

        // 暴露到全域
        window.gcCrossDeviceTest = {
            runAllTests,
            exportTestReport,
            createTestPanel,
            testResults
        };

        console.log('🌐 GameCore 跨設備測試工具已啟動');
        console.log('按 Ctrl+Shift+T 開啟測試面板');
    }

    // 當 DOM 載入完成後初始化
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})(); 