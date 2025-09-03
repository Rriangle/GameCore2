/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{vue,js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            // 玻璃風背景系統
            colors: {
                // 主要背景
                'gc-bg': '#eef3f8',
                'gc-bg2': '#ffffff',
                'gc-surface': 'rgba(255, 255, 255, 0.75)',
                'gc-glass': 'rgba(255, 255, 255, 0.45)',

                // 深色模式背景
                'gc-bg-dark': '#0c111b',
                'gc-bg2-dark': '#0a0f18',
                'gc-surface-dark': 'rgba(22, 30, 48, 0.65)',
                'gc-glass-dark': 'rgba(18, 24, 39, 0.50)',

                // 玻璃風主色調
                'gc-accent': '#7557ff',
                'gc-accent-2': '#34d2ff',
                'gc-accent-3': '#22c55e',

                // 彩色看板漸層
                'gc-gradient-1': '#4f46e5',
                'gc-gradient-2': '#22d3ee',
                'gc-gradient-3': '#f43f5e',
                'gc-gradient-4': '#f59e0b',
                'gc-gradient-5': '#22c55e',
                'gc-gradient-6': '#16a34a',
                'gc-gradient-7': '#8b5cf6',
                'gc-gradient-8': '#06b6d4',
                'gc-gradient-9': '#f97316',
                'gc-gradient-10': '#ef4444',
                'gc-gradient-11': '#06b6d4',
                'gc-gradient-12': '#3b82f6',

                // 排行榜配色
                'gc-ok': '#22c55e',
                'gc-down': '#ff6b6b',
                'gc-flat': '#9aa8bf',
                'gc-gold': '#FFD700',
                'gc-silver': '#C0C0C0',
                'gc-bronze': '#CD7F32',
                'gc-goldA': 'rgba(255, 215, 0, 0.18)',
                'gc-silverA': 'rgba(192, 192, 192, 0.18)',
                'gc-bronzeA': 'rgba(205, 127, 50, 0.18)',

                // 文字色彩
                'gc-ink': '#1f2937',
                'gc-muted': '#64748b',
                'gc-line': '#e5e7eb',

                // 狀態色彩
                'gc-success': {
                    50: '#f0fdf4',
                    100: '#dcfce7',
                    500: '#22c55e',
                    600: '#16a34a',
                    700: '#15803d',
                },
                'gc-warning': {
                    50: '#fffbeb',
                    100: '#fef3c7',
                    500: '#f59e0b',
                    600: '#d97706',
                    700: '#b45309',
                },
                'gc-error': {
                    50: '#fef2f2',
                    100: '#fee2e2',
                    500: '#ef4444',
                    600: '#dc2626',
                    700: '#b91c1c',
                },
                'gc-info': {
                    50: '#eff6ff',
                    100: '#dbeafe',
                    500: '#3b82f6',
                    600: '#2563eb',
                    700: '#1d4ed8',
                },
            },

            // 字體系統
            fontFamily: {
                'gc': ['system-ui', '-apple-system', '"Segoe UI"', 'Roboto', '"Noto Sans TC"', '"PingFang TC"', '"Microsoft JhengHei"', 'sans-serif'],
            },

            // 字體大小
            fontSize: {
                'gc-xs': '0.75rem',    // 12px
                'gc-sm': '0.875rem',   // 14px
                'gc-base': '1rem',     // 16px
                'gc-lg': '1.125rem',   // 18px
                'gc-xl': '1.25rem',    // 20px
                'gc-2xl': '1.5rem',    // 24px
                'gc-3xl': '1.875rem',  // 30px
                'gc-4xl': '2.25rem',   // 36px
                'gc-5xl': '3rem',      // 48px
                'gc-6xl': '3.75rem',   // 60px
            },

            // 字體粗細
            fontWeight: {
                'gc-light': '300',
                'gc-normal': '400',
                'gc-medium': '500',
                'gc-semibold': '600',
                'gc-bold': '700',
                'gc-extrabold': '800',
                'gc-black': '900',
            },

            // 間距系統
            spacing: {
                'gc-1': '0.25rem',     // 4px
                'gc-2': '0.5rem',      // 8px
                'gc-3': '0.75rem',     // 12px
                'gc-4': '1rem',        // 16px
                'gc-5': '1.25rem',     // 20px
                'gc-6': '1.5rem',      // 24px
                'gc-8': '2rem',        // 32px
                'gc-10': '2.5rem',     // 40px
                'gc-12': '3rem',       // 48px
                'gc-16': '4rem',       // 64px
                'gc-20': '5rem',       // 80px
                'gc-24': '6rem',       // 96px
                'gc-32': '8rem',       // 128px
            },

            // 圓角系統
            borderRadius: {
                'gc': '18px',
                'gc-sm': '12px',
                'gc-lg': '24px',
                'gc-xl': '32px',
            },

            // 陰影系統
            boxShadow: {
                'gc': '0 18px 40px rgba(17, 24, 39, 0.12)',
                'gc-sm': '0 4px 12px rgba(17, 24, 39, 0.08)',
                'gc-lg': '0 24px 48px rgba(17, 24, 39, 0.16)',
                'gc-strong': '0 24px 48px rgba(17, 24, 39, 0.16)',
                'gc-weak': '0 4px 12px rgba(17, 24, 39, 0.08)',
            },

            // 模糊效果
            backdropBlur: {
                'gc': '14px',
                'gc-strong': '20px',
                'gc-weak': '8px',
            },

            // 動畫系統
            transitionDuration: {
                'gc-fast': '150ms',
                'gc-base': '250ms',
                'gc-slow': '350ms',
                'gc-slower': '500ms',
            },

            // 緩動函數
            transitionTimingFunction: {
                'gc-linear': 'linear',
                'gc-in': 'cubic-bezier(0.4, 0, 1, 1)',
                'gc-out': 'cubic-bezier(0, 0, 0.2, 1)',
                'gc-in-out': 'cubic-bezier(0.4, 0, 0.2, 1)',
            },

            // Z-Index 系統
            zIndex: {
                'gc-dropdown': '1000',
                'gc-sticky': '1020',
                'gc-fixed': '1030',
                'gc-modal-backdrop': '1040',
                'gc-modal': '1050',
                'gc-popover': '1060',
                'gc-tooltip': '1070',
                'gc-toast': '1080',
            },

            // 響應式斷點
            screens: {
                'gc-sm': '640px',
                'gc-md': '768px',
                'gc-lg': '1024px',
                'gc-xl': '1280px',
                'gc-2xl': '1536px',
            },

            // 容器系統
            maxWidth: {
                'gc-container': '1380px',
            },

            // 背景圖片
            backgroundImage: {
                'gc-radial-1': 'radial-gradient(900px 500px at -10% -10%, rgba(117, 87, 255, 0.10), transparent 60%)',
                'gc-radial-2': 'radial-gradient(800px 460px at 110% 10%, rgba(52, 210, 255, 0.10), transparent 50%)',
                'gc-linear': 'linear-gradient(180deg, var(--gc-bg), var(--gc-bg2))',
            },

            // 動畫關鍵幀
            keyframes: {
                'gc-spin': {
                    'to': {
                        transform: 'rotate(360deg)',
                    },
                },
                'gc-ticker': {
                    '0%': {
                        transform: 'translateX(0)',
                    },
                    '100%': {
                        transform: 'translateX(-50%)',
                    },
                },
                'gc-pulse-glow': {
                    '0%': {
                        transform: 'translateX(-20%)',
                    },
                    '50%': {
                        opacity: '0.26',
                    },
                    '100%': {
                        transform: 'translateX(20%)',
                    },
                },
                'gc-fade-in': {
                    '0%': {
                        opacity: '0',
                        transform: 'translateY(10px)',
                    },
                    '100%': {
                        opacity: '1',
                        transform: 'translateY(0)',
                    },
                },
                'gc-fade-out': {
                    '0%': {
                        opacity: '1',
                        transform: 'translateY(0)',
                    },
                    '100%': {
                        opacity: '0',
                        transform: 'translateY(10px)',
                    },
                },
                'gc-slide-in-left': {
                    '0%': {
                        transform: 'translateX(-100%)',
                    },
                    '100%': {
                        transform: 'translateX(0)',
                    },
                },
                'gc-slide-in-right': {
                    '0%': {
                        transform: 'translateX(100%)',
                    },
                    '100%': {
                        transform: 'translateX(0)',
                    },
                },
                'gc-scale-in': {
                    '0%': {
                        transform: 'scale(0.9)',
                        opacity: '0',
                    },
                    '100%': {
                        transform: 'scale(1)',
                        opacity: '1',
                    },
                },
                'gc-bounce-in': {
                    '0%': {
                        transform: 'scale(0.3)',
                        opacity: '0',
                    },
                    '50%': {
                        transform: 'scale(1.05)',
                    },
                    '70%': {
                        transform: 'scale(0.9)',
                    },
                    '100%': {
                        transform: 'scale(1)',
                        opacity: '1',
                    },
                },
            },

            // 動畫類別
            animation: {
                'gc-spin': 'gc-spin 1s linear infinite',
                'gc-ticker': 'gc-ticker 10s linear infinite',
                'gc-pulse-glow': 'gc-pulse-glow 2.8s linear infinite',
                'gc-fade-in': 'gc-fade-in 0.3s ease-out',
                'gc-fade-out': 'gc-fade-out 0.3s ease-in',
                'gc-slide-in-left': 'gc-slide-in-left 0.3s ease-out',
                'gc-slide-in-right': 'gc-slide-in-right 0.3s ease-out',
                'gc-scale-in': 'gc-scale-in 0.2s ease-out',
                'gc-bounce-in': 'gc-bounce-in 0.6s ease-out',
            },

            // 網格系統
            gridTemplateColumns: {
                'gc-tiles': 'repeat(6, minmax(0, 1fr))',
                'gc-tiles-md': 'repeat(4, minmax(0, 1fr))',
                'gc-tiles-sm': 'repeat(2, minmax(0, 1fr))',
                'gc-tiles-xs': 'repeat(1, minmax(0, 1fr))',
                'gc-rank': '40px 1fr auto',
            },

            // 列間距
            gap: {
                'gc-1': '0.25rem',
                'gc-2': '0.5rem',
                'gc-3': '0.75rem',
                'gc-4': '1rem',
                'gc-5': '1.25rem',
                'gc-6': '1.5rem',
                'gc-8': '2rem',
                'gc-10': '2.5rem',
                'gc-12': '3rem',
                'gc-16': '4rem',
                'gc-20': '5rem',
                'gc-24': '6rem',
                'gc-32': '8rem',
            },

            // 最小高度
            minHeight: {
                'gc-tile': '120px',
                'gc-card': '200px',
                'gc-panel': '300px',
            },

            // 最大高度
            maxHeight: {
                'gc-modal': '86vh',
                'gc-dropdown': '400px',
                'gc-tooltip': '200px',
            },

            // 寬度
            width: {
                'gc-logo-badge': '36px',
                'gc-avatar': '40px',
                'gc-avatar-lg': '64px',
                'gc-avatar-xl': '96px',
            },

            // 高度
            height: {
                'gc-logo-badge': '36px',
                'gc-avatar': '40px',
                'gc-avatar-lg': '64px',
                'gc-avatar-xl': '96px',
                'gc-bar': '10px',
                'gc-bar-lg': '16px',
            },

            // 邊框寬度
            borderWidth: {
                'gc-1': '1px',
                'gc-2': '2px',
                'gc-3': '3px',
            },

            // 邊框樣式
            borderStyle: {
                'gc-dashed': 'dashed',
                'gc-dotted': 'dotted',
                'gc-solid': 'solid',
            },

            // 透明度
            opacity: {
                'gc-15': '0.15',
                'gc-18': '0.18',
                'gc-25': '0.25',
                'gc-35': '0.35',
                'gc-45': '0.45',
                'gc-65': '0.65',
                'gc-75': '0.75',
                'gc-85': '0.85',
                'gc-90': '0.90',
            },

            // 變換
            transform: {
                'gc-lift': 'translateY(-3px) scale(1.01)',
                'gc-lift-sm': 'translateY(-1px)',
                'gc-lift-lg': 'translateY(-2px)',
                'gc-scale-sm': 'scale(1.01)',
                'gc-scale-lg': 'scale(1.05)',
            },

            // 過渡屬性
            transitionProperty: {
                'gc-all': 'all',
                'gc-transform': 'transform',
                'gc-opacity': 'opacity',
                'gc-shadow': 'box-shadow',
                'gc-border': 'border-color',
                'gc-background': 'background-color',
            },

            // 過渡延遲
            transitionDelay: {
                'gc-0': '0ms',
                'gc-50': '50ms',
                'gc-100': '100ms',
                'gc-150': '150ms',
                'gc-200': '200ms',
                'gc-300': '300ms',
            },

            // 過渡持續時間
            transitionDuration: {
                'gc-0': '0ms',
                'gc-50': '50ms',
                'gc-100': '100ms',
                'gc-150': '150ms',
                'gc-200': '200ms',
                'gc-300': '300ms',
                'gc-500': '500ms',
            },

            // 過渡計時函數
            transitionTimingFunction: {
                'gc-linear': 'linear',
                'gc-ease': 'ease',
                'gc-ease-in': 'ease-in',
                'gc-ease-out': 'ease-out',
                'gc-ease-in-out': 'ease-in-out',
                'gc-bounce': 'cubic-bezier(0.68, -0.55, 0.265, 1.55)',
                'gc-elastic': 'cubic-bezier(0.175, 0.885, 0.32, 1.275)',
            },

            // 過濾器
            filter: {
                'gc-blur-sm': 'blur(4px)',
                'gc-blur': 'blur(8px)',
                'gc-blur-md': 'blur(12px)',
                'gc-blur-lg': 'blur(16px)',
                'gc-blur-xl': 'blur(24px)',
                'gc-blur-2xl': 'blur(40px)',
            },

            // 背景過濾器
            backdropFilter: {
                'gc-blur-sm': 'blur(4px)',
                'gc-blur': 'blur(8px)',
                'gc-blur-md': 'blur(12px)',
                'gc-blur-lg': 'blur(16px)',
                'gc-blur-xl': 'blur(24px)',
                'gc-blur-2xl': 'blur(40px)',
            },

            // 滾動行為
            scrollBehavior: {
                'gc-smooth': 'smooth',
                'gc-auto': 'auto',
            },

            // 滾動捕捉
            scrollSnapType: {
                'gc-x': 'x mandatory',
                'gc-y': 'y mandatory',
                'gc-both': 'both mandatory',
            },

            // 滾動捕捉對齊
            scrollSnapAlign: {
                'gc-start': 'start',
                'gc-center': 'center',
                'gc-end': 'end',
            },

            // 用戶選擇
            userSelect: {
                'gc-none': 'none',
                'gc-text': 'text',
                'gc-all': 'all',
            },

            // 指針事件
            pointerEvents: {
                'gc-none': 'none',
                'gc-auto': 'auto',
            },

            // 游標
            cursor: {
                'gc-pointer': 'pointer',
                'gc-default': 'default',
                'gc-not-allowed': 'not-allowed',
                'gc-wait': 'wait',
                'gc-text': 'text',
                'gc-move': 'move',
                'gc-help': 'help',
                'gc-zoom-in': 'zoom-in',
                'gc-zoom-out': 'zoom-out',
            },

            // 重設大小
            resize: {
                'gc-none': 'none',
                'gc-y': 'vertical',
                'gc-x': 'horizontal',
                'gc-both': 'both',
            },

            // 溢出
            overflow: {
                'gc-auto': 'auto',
                'gc-hidden': 'hidden',
                'gc-visible': 'visible',
                'gc-scroll': 'scroll',
                'gc-clip': 'clip',
            },

            // 文本溢出
            textOverflow: {
                'gc-clip': 'clip',
                'gc-ellipsis': 'ellipsis',
            },

            // 空白
            whiteSpace: {
                'gc-normal': 'normal',
                'gc-nowrap': 'nowrap',
                'gc-pre': 'pre',
                'gc-pre-line': 'pre-line',
                'gc-pre-wrap': 'pre-wrap',
                'gc-break-spaces': 'break-spaces',
            },

            // 單詞斷行
            wordBreak: {
                'gc-normal': 'normal',
                'gc-words': 'break-words',
                'gc-all': 'break-all',
                'gc-keep': 'keep-all',
            },

            // 單詞間距
            wordSpacing: {
                'gc-tight': '-0.05em',
                'gc-normal': '0em',
                'gc-wide': '0.05em',
                'gc-wider': '0.1em',
                'gc-widest': '0.2em',
            },

            // 字母間距
            letterSpacing: {
                'gc-tighter': '-0.05em',
                'gc-tight': '-0.025em',
                'gc-normal': '0em',
                'gc-wide': '0.025em',
                'gc-wider': '0.05em',
                'gc-widest': '0.1em',
            },

            // 行高
            lineHeight: {
                'gc-tight': '1.25',
                'gc-snug': '1.375',
                'gc-normal': '1.5',
                'gc-relaxed': '1.625',
                'gc-loose': '2',
            },

            // 列表樣式
            listStyleType: {
                'gc-none': 'none',
                'gc-disc': 'disc',
                'gc-decimal': 'decimal',
                'gc-circle': 'circle',
                'gc-square': 'square',
            },

            // 列表樣式位置
            listStylePosition: {
                'gc-inside': 'inside',
                'gc-outside': 'outside',
            },

            // 表格佈局
            tableLayout: {
                'gc-auto': 'auto',
                'gc-fixed': 'fixed',
            },

            // 邊框摺疊
            borderCollapse: {
                'gc-collapse': 'collapse',
                'gc-separate': 'separate',
            },

            // 邊框間距
            borderSpacing: {
                'gc-0': '0px',
                'gc-1': '1px',
                'gc-2': '2px',
                'gc-4': '4px',
                'gc-8': '8px',
            },

            // 內容
            content: {
                'gc-empty': '""',
                'gc-none': 'none',
            },

            // 計數器重置
            counterReset: {
                'gc-none': 'none',
            },

            // 計數器增量
            counterIncrement: {
                'gc-none': 'none',
            },

            // 垂直對齊
            verticalAlign: {
                'gc-baseline': 'baseline',
                'gc-top': 'top',
                'gc-middle': 'middle',
                'gc-bottom': 'bottom',
                'gc-text-top': 'text-top',
                'gc-text-bottom': 'text-bottom',
                'gc-sub': 'sub',
                'gc-super': 'super',
            },

            // 文本對齊
            textAlign: {
                'gc-left': 'left',
                'gc-center': 'center',
                'gc-right': 'right',
                'gc-justify': 'justify',
                'gc-start': 'start',
                'gc-end': 'end',
            },

            // 文本裝飾
            textDecoration: {
                'gc-underline': 'underline',
                'gc-line-through': 'line-through',
                'gc-no-underline': 'none',
            },

            // 文本裝飾顏色
            textDecorationColor: {
                'gc-current': 'currentColor',
                'gc-transparent': 'transparent',
            },

            // 文本裝飾樣式
            textDecorationStyle: {
                'gc-solid': 'solid',
                'gc-double': 'double',
                'gc-dotted': 'dotted',
                'gc-dashed': 'dashed',
                'gc-wavy': 'wavy',
            },

            // 文本裝飾厚度
            textDecorationThickness: {
                'gc-auto': 'auto',
                'gc-from-font': 'from-font',
                'gc-0': '0px',
                'gc-1': '1px',
                'gc-2': '2px',
                'gc-4': '4px',
                'gc-8': '8px',
            },

            // 文本下劃線偏移
            textUnderlineOffset: {
                'gc-auto': 'auto',
                'gc-0': '0px',
                'gc-1': '1px',
                'gc-2': '2px',
                'gc-4': '4px',
                'gc-8': '8px',
            },

            // 文本轉換
            textTransform: {
                'gc-uppercase': 'uppercase',
                'gc-lowercase': 'lowercase',
                'gc-capitalize': 'capitalize',
                'gc-normal-case': 'none',
            },

            // 文本縮進
            textIndent: {
                'gc-0': '0px',
                'gc-1': '0.25rem',
                'gc-2': '0.5rem',
                'gc-4': '1rem',
                'gc-8': '2rem',
                'gc-12': '3rem',
                'gc-16': '4rem',
            },

            // 文本方向
            textOrientation: {
                'gc-mixed': 'mixed',
                'gc-upright': 'upright',
                'gc-sideways': 'sideways',
            },

            // 書寫模式
            writingMode: {
                'gc-horizontal': 'horizontal-tb',
                'gc-vertical': 'vertical-rl',
                'gc-vertical-lr': 'vertical-lr',
            },

            // 方向
            direction: {
                'gc-ltr': 'ltr',
                'gc-rtl': 'rtl',
            },

            // 未設置屬性
            unset: {
                'gc-auto': 'auto',
                'gc-inherit': 'inherit',
                'gc-initial': 'initial',
                'gc-revert': 'revert',
                'gc-revert-layer': 'revert-layer',
            },
        },
    },
    plugins: [
        // 自定義插件
        function ({ addUtilities, theme }) {
            const newUtilities = {
                // 玻璃風背景工具類
                '.gc-glass-bg': {
                    background: theme('colors.gc-glass'),
                    backdropFilter: `blur(${theme('backdropBlur.gc')})`,
                    border: `1px solid ${theme('colors.gc-line')}`,
                },
                '.gc-surface-bg': {
                    background: theme('colors.gc-surface'),
                    backdropFilter: `blur(${theme('backdropBlur.gc')})`,
                    border: `1px solid ${theme('colors.gc-line')}`,
                },
                // 彩色看板漸層工具類
                '.gc-gradient-1': {
                    background: theme('colors.gc-gradient-1'),
                },
                '.gc-gradient-2': {
                    background: theme('colors.gc-gradient-2'),
                },
                '.gc-gradient-3': {
                    background: theme('colors.gc-gradient-3'),
                },
                '.gc-gradient-4': {
                    background: theme('colors.gc-gradient-4'),
                },
                '.gc-gradient-5': {
                    background: theme('colors.gc-gradient-5'),
                },
                '.gc-gradient-6': {
                    background: theme('colors.gc-gradient-6'),
                },
                // 排行榜樣式工具類
                '.gc-rank-gold': {
                    color: theme('colors.gc-gold'),
                },
                '.gc-rank-silver': {
                    color: theme('colors.gc-silver'),
                },
                '.gc-rank-bronze': {
                    color: theme('colors.gc-bronze'),
                },
                // 狀態指示器工具類
                '.gc-status-up': {
                    color: theme('colors.gc-ok'),
                },
                '.gc-status-down': {
                    color: theme('colors.gc-down'),
                },
                '.gc-status-flat': {
                    color: theme('colors.gc-flat'),
                },
            };
            addUtilities(newUtilities);
        },
    ],
};
