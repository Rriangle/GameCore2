/**
 * GameCore 視覺微調工具控制器
 * ========================================
 * 
 * 這個檔案提供了視覺微調工具的 JavaScript 控制功能，
 * 包括跨設備測試、動畫效果控制、無障礙性驗證等
 * 
 * 功能特點：
 * - 視覺一致性檢查
 * - 跨設備響應式測試
 * - 動畫效果優化
 * - 無障礙性驗證
 * - 效能監控
 * 
 * 使用方法：
 * 1. 在開發環境中引入此檔案
 * 2. 使用瀏覽器開發者工具檢查視覺效果
 * 3. 在不同設備和瀏覽器中測試
 * 4. 使用無障礙性工具驗證
 * 
 * 回滾方式：刪除此檔案即可移除視覺微調工具控制
 * ========================================
 */

(function () {
    'use strict';

    // 工具狀態管理
    const VisualPolishTools = {
        // 工具狀態
        isEnabled: false,
        isDebugMode: false,
        currentDevice: 'desktop',
        currentAnimationSpeed: 'normal',
        currentContrastMode: 'normal',

        // 工具元素
        elements: {},

        // 初始化
        init() {
            this.createDebugPanel();
            this.createBreakpointIndicator();
            this.createDeviceFrame();
            this.bindEvents();
            this.detectEnvironment();
            console.log('🎨 GameCore 視覺微調工具已初始化');
        },

        // 創建調試面板
        createDebugPanel() {
            const panel = document.createElement('div');
            panel.className = 'gc-debug-panel';
            panel.innerHTML = `
                <h3>🎨 視覺微調工具</h3>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-grid-debug"> 網格系統
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-spacing-debug"> 間距系統
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>設備模擬：</label>
                    <select id="gc-device-selector">
                        <option value="desktop">桌面版</option>
                        <option value="tablet">平板版</option>
                        <option value="mobile">手機版</option>
                    </select>
                </div>
                
                <div class="gc-debug-control">
                    <label>動畫速度：</label>
                    <select id="gc-animation-selector">
                        <option value="normal">正常</option>
                        <option value="fast">快速</option>
                        <option value="slow">慢速</option>
                        <option value="none">無動畫</option>
                    </select>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-focus-debug"> 焦點可視化
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-contrast-debug"> 對比度檢查
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-repaint-debug"> 重繪區域
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-z-index-debug"> Z-Index 可視化
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-hover-debug"> 懸停狀態
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-loading-debug"> 載入狀態
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-error-debug"> 錯誤狀態
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-success-debug"> 成功狀態
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-warning-debug"> 警告狀態
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-font-debug"> 字體可視化
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-color-debug"> 色彩可視化
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-high-contrast-debug"> 高對比度
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <label>
                        <input type="checkbox" id="gc-reduced-motion-debug"> 減少動畫
                    </label>
                </div>
                
                <div class="gc-debug-control">
                    <button id="gc-reset-all" class="gc-btn gc-btn-secondary">重置所有</button>
                </div>
            `;

            document.body.appendChild(panel);
            this.elements.panel = panel;
        },

        // 創建斷點指示器
        createBreakpointIndicator() {
            const indicator = document.createElement('div');
            indicator.className = 'gc-breakpoint-indicator';
            document.body.appendChild(indicator);
            this.elements.indicator = indicator;
        },

        // 創建設備框架
        createDeviceFrame() {
            const frame = document.createElement('div');
            frame.className = 'gc-device-frame';
            frame.innerHTML = `
                <div class="gc-device-content">
                    <iframe src="${window.location.href}" frameborder="0" width="100%" height="100%"></iframe>
                </div>
            `;
            document.body.appendChild(frame);
            this.elements.frame = frame;
        },

        // 創建切換按鈕
        createToggleButton() {
            const toggle = document.createElement('button');
            toggle.className = 'gc-debug-toggle';
            toggle.innerHTML = '🎨';
            toggle.title = '視覺微調工具';
            document.body.appendChild(toggle);
            this.elements.toggle = toggle;
        },

        // 綁定事件
        bindEvents() {
            // 網格系統切換
            document.getElementById('gc-grid-debug')?.addEventListener('change', (e) => {
                this.toggleGridDebug(e.target.checked);
            });

            // 間距系統切換
            document.getElementById('gc-spacing-debug')?.addEventListener('change', (e) => {
                this.toggleSpacingDebug(e.target.checked);
            });

            // 設備選擇器
            document.getElementById('gc-device-selector')?.addEventListener('change', (e) => {
                this.switchDevice(e.target.value);
            });

            // 動畫速度選擇器
            document.getElementById('gc-animation-selector')?.addEventListener('change', (e) => {
                this.setAnimationSpeed(e.target.value);
            });

            // 焦點可視化
            document.getElementById('gc-focus-debug')?.addEventListener('change', (e) => {
                this.toggleFocusDebug(e.target.checked);
            });

            // 對比度檢查
            document.getElementById('gc-contrast-debug')?.addEventListener('change', (e) => {
                this.toggleContrastDebug(e.target.checked);
            });

            // 重繪區域
            document.getElementById('gc-repaint-debug')?.addEventListener('change', (e) => {
                this.toggleRepaintDebug(e.target.checked);
            });

            // Z-Index 可視化
            document.getElementById('gc-z-index-debug')?.addEventListener('change', (e) => {
                this.toggleZIndexDebug(e.target.checked);
            });

            // 懸停狀態
            document.getElementById('gc-hover-debug')?.addEventListener('change', (e) => {
                this.toggleHoverDebug(e.target.checked);
            });

            // 載入狀態
            document.getElementById('gc-loading-debug')?.addEventListener('change', (e) => {
                this.toggleLoadingDebug(e.target.checked);
            });

            // 錯誤狀態
            document.getElementById('gc-error-debug')?.addEventListener('change', (e) => {
                this.toggleErrorDebug(e.target.checked);
            });

            // 成功狀態
            document.getElementById('gc-success-debug')?.addEventListener('change', (e) => {
                this.toggleSuccessDebug(e.target.checked);
            });

            // 警告狀態
            document.getElementById('gc-warning-debug')?.addEventListener('change', (e) => {
                this.toggleWarningDebug(e.target.checked);
            });

            // 字體可視化
            document.getElementById('gc-font-debug')?.addEventListener('change', (e) => {
                this.toggleFontDebug(e.target.checked);
            });

            // 色彩可視化
            document.getElementById('gc-color-debug')?.addEventListener('change', (e) => {
                this.toggleColorDebug(e.target.checked);
            });

            // 高對比度
            document.getElementById('gc-high-contrast-debug')?.addEventListener('change', (e) => {
                this.toggleHighContrastDebug(e.target.checked);
            });

            // 減少動畫
            document.getElementById('gc-reduced-motion-debug')?.addEventListener('change', (e) => {
                this.toggleReducedMotionDebug(e.target.checked);
            });

            // 重置所有
            document.getElementById('gc-reset-all')?.addEventListener('click', () => {
                this.resetAll();
            });

            // 鍵盤快捷鍵
            document.addEventListener('keydown', (e) => {
                this.handleKeyboardShortcuts(e);
            });
        },

        // 切換網格調試
        toggleGridDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-grid-debug');
            } else {
                document.body.classList.remove('gc-grid-debug');
            }
        },

        // 切換間距調試
        toggleSpacingDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-spacing-debug');
            } else {
                document.body.classList.remove('gc-spacing-debug');
            }
        },

        // 切換設備
        switchDevice(device) {
            this.currentDevice = device;
            const frame = this.elements.frame;

            // 移除所有設備類別
            frame.classList.remove('mobile', 'tablet', 'desktop');

            // 添加新設備類別
            frame.classList.add(device);

            // 顯示設備框架
            frame.classList.add('show');

            // 更新 iframe 源
            const iframe = frame.querySelector('iframe');
            if (iframe) {
                iframe.src = window.location.href;
            }
        },

        // 設置動畫速度
        setAnimationSpeed(speed) {
            this.currentAnimationSpeed = speed;

            // 移除所有動畫類別
            document.body.classList.remove('gc-animation-fast', 'gc-animation-slow', 'gc-animation-none');

            // 添加新動畫類別
            if (speed !== 'normal') {
                document.body.classList.add(`gc-animation-${speed}`);
            }
        },

        // 切換焦點調試
        toggleFocusDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-focus-debug');
            } else {
                document.body.classList.remove('gc-focus-debug');
            }
        },

        // 切換對比度調試
        toggleContrastDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-contrast-debug');
            } else {
                document.body.classList.remove('gc-contrast-debug');
            }
        },

        // 切換重繪調試
        toggleRepaintDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-repaint-debug');
            } else {
                document.body.classList.remove('gc-repaint-debug');
            }
        },

        // 切換 Z-Index 調試
        toggleZIndexDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-z-index-debug');
            } else {
                document.body.classList.remove('gc-z-index-debug');
            }
        },

        // 切換懸停調試
        toggleHoverDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-hover-debug');
            } else {
                document.body.classList.remove('gc-hover-debug');
            }
        },

        // 切換載入調試
        toggleLoadingDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-loading-debug');
            } else {
                document.body.classList.remove('gc-loading-debug');
            }
        },

        // 切換錯誤調試
        toggleErrorDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-error-debug');
            } else {
                document.body.classList.remove('gc-error-debug');
            }
        },

        // 切換成功調試
        toggleSuccessDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-success-debug');
            } else {
                document.body.classList.remove('gc-success-debug');
            }
        },

        // 切換警告調試
        toggleWarningDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-warning-debug');
            } else {
                document.body.classList.remove('gc-warning-debug');
            }
        },

        // 切換字體調試
        toggleFontDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-font-debug');
            } else {
                document.body.classList.remove('gc-font-debug');
            }
        },

        // 切換色彩調試
        toggleColorDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-color-debug');
            } else {
                document.body.classList.remove('gc-color-debug');
            }
        },

        // 切換高對比度調試
        toggleHighContrastDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-high-contrast-debug');
            } else {
                document.body.classList.remove('gc-high-contrast-debug');
            }
        },

        // 切換減少動畫調試
        toggleReducedMotionDebug(enabled) {
            if (enabled) {
                document.body.classList.add('gc-reduced-motion-debug');
            } else {
                document.body.classList.remove('gc-reduced-motion-debug');
            }
        },

        // 重置所有
        resetAll() {
            // 重置所有調試狀態
            document.body.classList.remove(
                'gc-grid-debug',
                'gc-spacing-debug',
                'gc-focus-debug',
                'gc-contrast-debug',
                'gc-repaint-debug',
                'gc-z-index-debug',
                'gc-hover-debug',
                'gc-loading-debug',
                'gc-error-debug',
                'gc-success-debug',
                'gc-warning-debug',
                'gc-font-debug',
                'gc-color-debug',
                'gc-high-contrast-debug',
                'gc-reduced-motion-debug',
                'gc-animation-fast',
                'gc-animation-slow',
                'gc-animation-none'
            );

            // 隱藏設備框架
            this.elements.frame?.classList.remove('show');

            // 重置選擇器
            document.getElementById('gc-device-selector').value = 'desktop';
            document.getElementById('gc-animation-selector').value = 'normal';

            // 重置所有複選框
            const checkboxes = document.querySelectorAll('.gc-debug-panel input[type="checkbox"]');
            checkboxes.forEach(checkbox => {
                checkbox.checked = false;
            });

            console.log('🔄 所有視覺微調工具已重置');
        },

        // 處理鍵盤快捷鍵
        handleKeyboardShortcuts(e) {
            // Ctrl + Shift + D: 切換調試面板
            if (e.ctrlKey && e.shiftKey && e.key === 'D') {
                e.preventDefault();
                this.toggleDebugPanel();
            }

            // Ctrl + Shift + G: 切換網格
            if (e.ctrlKey && e.shiftKey && e.key === 'G') {
                e.preventDefault();
                const checkbox = document.getElementById('gc-grid-debug');
                checkbox.checked = !checkbox.checked;
                this.toggleGridDebug(checkbox.checked);
            }

            // Ctrl + Shift + S: 切換間距
            if (e.ctrlKey && e.shiftKey && e.key === 'S') {
                e.preventDefault();
                const checkbox = document.getElementById('gc-spacing-debug');
                checkbox.checked = !checkbox.checked;
                this.toggleSpacingDebug(checkbox.checked);
            }

            // Ctrl + Shift + R: 重置所有
            if (e.ctrlKey && e.shiftKey && e.key === 'R') {
                e.preventDefault();
                this.resetAll();
            }
        },

        // 切換調試面板
        toggleDebugPanel() {
            const panel = this.elements.panel;
            if (panel.style.display === 'none') {
                panel.style.display = 'block';
            } else {
                panel.style.display = 'none';
            }
        },

        // 檢測環境
        detectEnvironment() {
            // 檢測是否為開發環境
            const isDevelopment = window.location.hostname === 'localhost' ||
                window.location.hostname === '127.0.0.1' ||
                window.location.hostname.includes('dev') ||
                window.location.hostname.includes('test');

            if (isDevelopment) {
                document.body.classList.add('gc-debug-mode');
                this.isDebugMode = true;
                console.log('🔧 檢測到開發環境，啟用調試模式');
            } else {
                console.log('🚀 檢測到生產環境，禁用調試工具');
            }
        },

        // 生成報告
        generateReport() {
            const report = {
                timestamp: new Date().toISOString(),
                url: window.location.href,
                userAgent: navigator.userAgent,
                viewport: {
                    width: window.innerWidth,
                    height: window.innerHeight
                },
                devicePixelRatio: window.devicePixelRatio,
                colorScheme: window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light',
                reducedMotion: window.matchMedia('(prefers-reduced-motion: reduce)').matches,
                currentSettings: {
                    device: this.currentDevice,
                    animationSpeed: this.currentAnimationSpeed,
                    contrastMode: this.currentContrastMode
                }
            };

            console.log('📊 視覺微調工具報告:', report);
            return report;
        }
    };

    // 等待 DOM 載入完成後初始化
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            VisualPolishTools.init();
        });
    } else {
        VisualPolishTools.init();
    }

    // 暴露到全域
    window.GameCoreVisualPolishTools = VisualPolishTools;

})(); 