/* ========================================
   GameCore 效能監控工具
   ========================================
   
   這個檔案提供了效能監控和測試功能，
   幫助開發者優化網站效能
   
   功能特點：
   - 核心網頁指標監控 (LCP, FID, CLS)
   - 資源載入效能分析
   - 效能瓶頸檢測
   - 效能報告生成
   - 自動效能優化建議
   
   回滾方式：刪除此檔案即可恢復預設效能監控
   ======================================== */

(function () {
    'use strict';

    // 效能監控工具類別
    class PerformanceMonitor {
        constructor() {
            this.metrics = {};
            this.observers = [];
            this.init();
        }

        // 初始化效能監控
        init() {
            this.setupCoreWebVitals();
            this.setupResourceTiming();
            this.setupPerformanceObserver();
            this.setupMemoryMonitoring();
            this.setupNetworkMonitoring();
        }

        // 設置核心網頁指標
        setupCoreWebVitals() {
            // LCP (Largest Contentful Paint)
            if ('PerformanceObserver' in window) {
                const lcpObserver = new PerformanceObserver((list) => {
                    const entries = list.getEntries();
                    const lastEntry = entries[entries.length - 1];
                    this.metrics.lcp = lastEntry.startTime;
                    this.logMetric('LCP', lastEntry.startTime);
                });

                try {
                    lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] });
                } catch (e) {
                    console.warn('LCP 監控不可用:', e);
                }
            }

            // FID (First Input Delay)
            if ('PerformanceObserver' in window) {
                const fidObserver = new PerformanceObserver((list) => {
                    const entries = list.getEntries();
                    entries.forEach((entry) => {
                        const fid = entry.processingStart - entry.startTime;
                        this.metrics.fid = fid;
                        this.logMetric('FID', fid);
                    });
                });

                try {
                    fidObserver.observe({ entryTypes: ['first-input'] });
                } catch (e) {
                    console.warn('FID 監控不可用:', e);
                }
            }

            // CLS (Cumulative Layout Shift)
            if ('PerformanceObserver' in window) {
                let clsValue = 0;
                const clsObserver = new PerformanceObserver((list) => {
                    const entries = list.getEntries();
                    entries.forEach((entry) => {
                        if (!entry.hadRecentInput) {
                            clsValue += entry.value;
                        }
                    });
                    this.metrics.cls = clsValue;
                    this.logMetric('CLS', clsValue);
                });

                try {
                    clsObserver.observe({ entryTypes: ['layout-shift'] });
                } catch (e) {
                    console.warn('CLS 監控不可用:', e);
                }
            }
        }

        // 設置資源計時
        setupResourceTiming() {
            if ('PerformanceObserver' in window) {
                const resourceObserver = new PerformanceObserver((list) => {
                    const entries = list.getEntries();
                    entries.forEach((entry) => {
                        this.analyzeResource(entry);
                    });
                });

                try {
                    resourceObserver.observe({ entryTypes: ['resource'] });
                } catch (e) {
                    console.warn('資源計時監控不可用:', e);
                }
            }
        }

        // 分析資源載入
        analyzeResource(entry) {
            const resource = {
                name: entry.name,
                type: entry.initiatorType,
                duration: entry.duration,
                size: entry.transferSize,
                startTime: entry.startTime
            };

            // 檢測慢速資源
            if (entry.duration > 1000) {
                this.logSlowResource(resource);
            }

            // 檢測大檔案
            if (entry.transferSize > 500000) {
                this.logLargeResource(resource);
            }
        }

        // 設置效能觀察器
        setupPerformanceObserver() {
            // 監控長任務
            if ('PerformanceObserver' in window) {
                const longTaskObserver = new PerformanceObserver((list) => {
                    const entries = list.getEntries();
                    entries.forEach((entry) => {
                        if (entry.duration > 50) {
                            this.logLongTask(entry);
                        }
                    });
                });

                try {
                    longTaskObserver.observe({ entryTypes: ['longtask'] });
                } catch (e) {
                    console.warn('長任務監控不可用:', e);
                }
            }
        }

        // 設置記憶體監控
        setupMemoryMonitoring() {
            if ('memory' in performance) {
                setInterval(() => {
                    const memory = performance.memory;
                    this.metrics.memory = {
                        used: memory.usedJSHeapSize,
                        total: memory.totalJSHeapSize,
                        limit: memory.jsHeapSizeLimit
                    };

                    // 檢測記憶體洩漏
                    if (memory.usedJSHeapSize > memory.jsHeapSizeLimit * 0.8) {
                        this.logMemoryWarning();
                    }
                }, 5000);
            }
        }

        // 設置網路監控
        setupNetworkMonitoring() {
            if ('connection' in navigator) {
                const connection = navigator.connection;
                this.metrics.network = {
                    effectiveType: connection.effectiveType,
                    downlink: connection.downlink,
                    rtt: connection.rtt
                };

                // 根據網路狀況調整載入策略
                this.adjustLoadingStrategy(connection);
            }
        }

        // 調整載入策略
        adjustLoadingStrategy(connection) {
            if (connection.effectiveType === 'slow-2g' || connection.effectiveType === '2g') {
                // 慢速網路：減少圖片品質，延遲載入非關鍵資源
                this.enableLowBandwidthMode();
            } else if (connection.effectiveType === '4g') {
                // 快速網路：預載入更多資源
                this.enableHighBandwidthMode();
            }
        }

        // 啟用低頻寬模式
        enableLowBandwidthMode() {
            // 降低圖片品質
            const images = document.querySelectorAll('img');
            images.forEach(img => {
                if (img.dataset.lowRes) {
                    img.src = img.dataset.lowRes;
                }
            });

            // 延遲載入非關鍵 CSS
            const nonCriticalCSS = document.querySelectorAll('link[data-async="true"]');
            nonCriticalCSS.forEach(link => {
                link.setAttribute('media', 'print');
                link.onload = () => {
                    link.setAttribute('media', 'all');
                };
            });
        }

        // 啟用高頻寬模式
        enableHighBandwidthMode() {
            // 預載入更多資源
            this.preloadAdditionalResources();
        }

        // 預載入額外資源
        preloadAdditionalResources() {
            const resources = [
                '/css/utilities.css',
                '/js/components.js'
            ];

            resources.forEach(href => {
                const link = document.createElement('link');
                link.rel = 'preload';
                link.href = href;
                link.as = href.endsWith('.css') ? 'style' : 'script';
                document.head.appendChild(link);
            });
        }

        // 記錄指標
        logMetric(name, value) {
            console.log(`效能指標 ${name}:`, value);

            // 發送到分析服務（如果配置了）
            if (window.gtag) {
                gtag('event', 'performance_metric', {
                    metric_name: name,
                    metric_value: value
                });
            }
        }

        // 記錄慢速資源
        logSlowResource(resource) {
            console.warn('慢速資源載入:', resource);

            // 提供優化建議
            this.suggestOptimization('resource', resource);
        }

        // 記錄大檔案
        logLargeResource(resource) {
            console.warn('大檔案載入:', resource);

            // 提供壓縮建議
            this.suggestOptimization('size', resource);
        }

        // 記錄長任務
        logLongTask(task) {
            console.warn('長任務執行:', task);

            // 提供優化建議
            this.suggestOptimization('task', task);
        }

        // 記錄記憶體警告
        logMemoryWarning() {
            console.warn('記憶體使用量過高');

            // 提供記憶體優化建議
            this.suggestOptimization('memory');
        }

        // 提供優化建議
        suggestOptimization(type, data) {
            const suggestions = {
                resource: [
                    '考慮使用 CDN 加速資源載入',
                    '啟用 Gzip 壓縮',
                    '使用 WebP 格式圖片',
                    '實施資源預載入'
                ],
                size: [
                    '壓縮圖片檔案',
                    '使用現代圖片格式 (WebP, AVIF)',
                    '實施圖片懶載入',
                    '考慮使用圖片 CDN'
                ],
                task: [
                    '將長任務分解為小任務',
                    '使用 Web Workers 處理複雜計算',
                    '優化 JavaScript 執行效率',
                    '避免在主線程執行重負載任務'
                ],
                memory: [
                    '檢查記憶體洩漏',
                    '及時清理事件監聽器',
                    '使用物件池模式',
                    '避免閉包導致的記憶體洩漏'
                ]
            };

            const typeSuggestions = suggestions[type] || [];
            console.log('優化建議:', typeSuggestions);
        }

        // 生成效能報告
        generateReport() {
            const report = {
                timestamp: new Date().toISOString(),
                url: window.location.href,
                metrics: this.metrics,
                recommendations: this.generateRecommendations()
            };

            return report;
        }

        // 生成建議
        generateRecommendations() {
            const recommendations = [];

            // 基於 LCP 的建議
            if (this.metrics.lcp > 2500) {
                recommendations.push('LCP 過高，建議優化關鍵資源載入');
            }

            // 基於 FID 的建議
            if (this.metrics.fid > 100) {
                recommendations.push('FID 過高，建議優化 JavaScript 執行');
            }

            // 基於 CLS 的建議
            if (this.metrics.cls > 0.1) {
                recommendations.push('CLS 過高，建議避免佈局偏移');
            }

            return recommendations;
        }

        // 自動優化
        autoOptimize() {
            // 自動圖片優化
            this.optimizeImages();

            // 自動 CSS 優化
            this.optimizeCSS();

            // 自動 JavaScript 優化
            this.optimizeJavaScript();
        }

        // 優化圖片
        optimizeImages() {
            const images = document.querySelectorAll('img');
            images.forEach(img => {
                // 添加懶載入
                if (!img.loading) {
                    img.loading = 'lazy';
                }

                // 添加尺寸屬性
                if (!img.width || !img.height) {
                    img.setAttribute('width', 'auto');
                    img.setAttribute('height', 'auto');
                }
            });
        }

        // 優化 CSS
        optimizeCSS() {
            // 移除未使用的 CSS
            const unusedCSS = this.detectUnusedCSS();
            if (unusedCSS.length > 0) {
                console.log('未使用的 CSS 規則:', unusedCSS);
            }
        }

        // 檢測未使用的 CSS
        detectUnusedCSS() {
            // 簡化的未使用 CSS 檢測
            // 實際實現需要更複雜的分析
            return [];
        }

        // 優化 JavaScript
        optimizeJavaScript() {
            // 檢查 JavaScript 錯誤
            window.addEventListener('error', (e) => {
                console.error('JavaScript 錯誤:', e.error);
            });

            // 檢查未處理的 Promise 拒絕
            window.addEventListener('unhandledrejection', (e) => {
                console.error('未處理的 Promise 拒絕:', e.reason);
            });
        }
    }

    // 初始化效能監控
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            new PerformanceMonitor();
        });
    } else {
        new PerformanceMonitor();
    }

    // 全域效能監控工具
    window.GameCorePerformance = {
        // 手動觸發效能監控
        monitor: function () {
            return new PerformanceMonitor();
        },

        // 生成效能報告
        report: function () {
            const monitor = new PerformanceMonitor();
            return monitor.generateReport();
        },

        // 自動優化
        optimize: function () {
            const monitor = new PerformanceMonitor();
            monitor.autoOptimize();
        },

        // 獲取效能指標
        getMetrics: function () {
            const monitor = new PerformanceMonitor();
            return monitor.metrics;
        },

        // 測試效能
        test: function () {
            const monitor = new PerformanceMonitor();
            const report = monitor.generateReport();
            console.log('效能測試報告:', report);
            return report;
        }
    };

})(); 