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
                'gc-none': '0',
                'gc-sm': '0.375rem',       // 6px
                'gc-base': '0.5rem',       // 8px
                'gc-md': '0.75rem',        // 12px
                'gc-lg': '1rem',           // 16px
                'gc-xl': '1.5rem',         // 24px
                'gc-2xl': '2rem',          // 32px
                'gc-full': '9999px',

                // 玻璃風特殊圓角
                'gc-glass': '18px',
                'gc-glass-sm': '12px',
            },

            // 陰影系統
            boxShadow: {
                'gc-sm': '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
                'gc-base': '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',
                'gc-md': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
                'gc-lg': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
                'gc-xl': '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',

                // 玻璃風特殊陰影
                'gc-glass': '0 18px 40px rgba(17, 24, 39, 0.12)',
                'gc-glass-dark': '0 18px 42px rgba(0, 0, 0, 0.35)',
            },

            // 模糊效果系統
            blur: {
                'gc-sm': '4px',
                'gc-base': '8px',
                'gc-md': '12px',
                'gc-lg': '16px',
                'gc-xl': '24px',
                'gc-2xl': '40px',

                // 玻璃風特殊模糊
                'gc-glass': '14px',
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

            // 過渡動畫系統
            transitionDuration: {
                'gc-fast': '0.15s',
                'gc-base': '0.2s',
                'gc-slow': '0.3s',
                'gc-slower': '0.5s',

                // 玻璃風特殊過渡
                'gc-glass': '0.16s',
            },

            // 響應式斷點系統
            screens: {
                'gc-sm': '640px',
                'gc-md': '768px',
                'gc-lg': '1024px',
                'gc-xl': '1280px',
                'gc-2xl': '1536px',
            },

            // 容器寬度系統
            maxWidth: {
                'gc-container': '1380px',
            },

            // 背景漸層
            backgroundImage: {
                'gc-gradient-1': 'linear-gradient(135deg, #4f46e5, #22d3ee)',
                'gc-gradient-2': 'linear-gradient(135deg, #f43f5e, #f59e0b)',
                'gc-gradient-3': 'linear-gradient(135deg, #22c55e, #16a34a)',
                'gc-gradient-4': 'linear-gradient(135deg, #8b5cf6, #06b6d4)',
                'gc-gradient-5': 'linear-gradient(135deg, #f97316, #ef4444)',
                'gc-gradient-6': 'linear-gradient(135deg, #06b6d4, #3b82f6)',
                'gc-gradient-primary': 'linear-gradient(135deg, #7557ff, #3b82f6)',
                'gc-gradient-nav': 'linear-gradient(90deg, #3b82f6, #a78bfa)',
            },

            // 動畫系統
            keyframes: {
                'gc-spin': {
                    'to': { transform: 'rotate(360deg)' }
                },
                'gc-pulse-glow': {
                    '0%': { transform: 'translateX(-20%)' },
                    '50%': { opacity: '0.26' },
                    '100%': { transform: 'translateX(20%)' }
                },
                'gc-ticker': {
                    '0%': { transform: 'translateX(0)' },
                    '100%': { transform: 'translateX(-50%)' }
                },
                'gc-fade-in': {
                    '0%': { opacity: '0', transform: 'translateY(10px)' },
                    '100%': { opacity: '1', transform: 'translateY(0)' }
                },
                'gc-slide-up': {
                    '0%': { transform: 'translateY(100%)' },
                    '100%': { transform: 'translateY(0)' }
                }
            },

            animation: {
                'gc-spin': 'gc-spin 1s linear infinite',
                'gc-pulse-glow': 'gc-pulse-glow 2.8s linear infinite',
                'gc-ticker': 'gc-ticker 10s linear infinite',
                'gc-fade-in': 'gc-fade-in 0.3s ease-out',
                'gc-slide-up': 'gc-slide-up 0.3s ease-out',
            },

            // 背景混合模式
            backdropBlur: {
                'gc-glass': '14px',
            },

            // 過濾器
            filter: {
                'gc-glass': 'blur(14px)',
            },
        },
    },
    plugins: [
        // 玻璃風組件插件
        function ({ addComponents, theme }) {
            addComponents({
                // 玻璃風背景
                '.gc-glass-bg': {
                    background: theme('colors.gc-glass'),
                    backdropFilter: 'blur(14px)',
                    border: `1px solid ${theme('colors.gc-line')}`,
                },

                // 玻璃風表面
                '.gc-surface-bg': {
                    background: theme('colors.gc-surface'),
                    backdropFilter: 'blur(14px)',
                    border: `1px solid ${theme('colors.gc-line')}`,
                },

                // 玻璃風按鈕
                '.gc-btn': {
                    display: 'inline-flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    gap: theme('spacing.gc-2'),
                    padding: `${theme('spacing.gc-2)} ${theme('spacing.gc - 3)}`,
                    border: `1px solid ${theme('colors.gc-line')}`,
                    borderRadius: theme('borderRadius.gc-glass-sm'),
                    background: theme('colors.gc-surface'),
                    color: theme('colors.gc-ink'),
                    fontFamily: theme('fontFamily.gc'),
                    fontSize: theme('fontSize.gc-sm'),
                    fontWeight: theme('fontWeight.gc-medium'),
                    lineHeight: '1',
                    textDecoration: 'none',
                    cursor: 'pointer',
                    transition: 'all 0.16s cubic-bezier(0.4, 0, 0.2, 1)',
                    backdropFilter: 'blur(14px)',
                    boxShadow: theme('boxShadow.gc-sm'),
                    userSelect: 'none',
                    whiteSpace: 'nowrap',
                },

                '.gc-btn:hover': {
                    transform: 'translateY(-1px)',
                    boxShadow: theme('boxShadow.gc-glass'),
                    borderColor: theme('colors.gc-accent'),
                },

                '.gc-btn:focus': {
                    outline: 'none',
                    boxShadow: `0 0 0 3px rgba(117, 87, 255, 0.1)`,
                    borderColor: theme('colors.gc-accent'),
                },

                // 主要按鈕
                '.gc-btn-primary': {
                    border: '0',
                    background: 'linear-gradient(135deg, #7557ff, #3b82f6)',
                    color: 'white',
                    boxShadow: theme('boxShadow.gc-glass'),
                },

                '.gc-btn-primary:hover': {
                    background: 'linear-gradient(135deg, #2563eb, #7557ff)',
                    boxShadow: theme('boxShadow.gc-lg'),
                },

                // 玻璃風卡片
                '.gc-card': {
                    background: theme('colors.gc-surface'),
                    border: `1px solid ${theme('colors.gc-line')}`,
                    borderRadius: theme('borderRadius.gc-glass'),
                    boxShadow: theme('boxShadow.gc-glass'),
                    backdropFilter: 'blur(14px)',
                    transition: 'all 0.16s cubic-bezier(0.4, 0, 0.2, 1)',
                    overflow: 'hidden',
                },

                '.gc-card:hover': {
                    transform: 'translateY(-3px) scale(1.01)',
                    boxShadow: theme('boxShadow.gc-lg'),
                },

                // 玻璃風面板
                '.gc-panel': {
                    background: theme('colors.gc-surface'),
                    border: `1px solid ${theme('colors.gc-line')}`,
                    borderRadius: theme('borderRadius.gc-glass'),
                    boxShadow: theme('boxShadow.gc-glass'),
                    backdropFilter: 'blur(14px)',
                    padding: theme('spacing.gc-4'),
                },

                // 彩色看板
                '.gc-tile': {
                    position: 'relative',
                    minHeight: '120px',
                    borderRadius: theme('borderRadius.gc-lg'),
                    padding: theme('spacing.gc-4'),
                    overflow: 'hidden',
                    display: 'flex',
                    flexDirection: 'column',
                    justifyContent: 'space-between',
                    cursor: 'pointer',
                    background: theme('colors.gc-glass'),
                    border: `1px solid ${theme('colors.gc-line')}`,
                    backdropFilter: 'blur(14px)',
                    transition: 'transform 0.16s ease, box-shadow 0.16s ease, filter 0.2s ease',
                    boxShadow: theme('boxShadow.gc-glass'),
                },

                '.gc-tile:hover': {
                    transform: 'translateY(-3px) scale(1.01)',
                },

                // 排行榜行
                '.gc-rank-row': {
                    position: 'relative',
                    display: 'grid',
                    gridTemplateColumns: '40px 1fr auto',
                    gap: theme('spacing.gc-3'),
                    alignItems: 'center',
                    border: `1px solid ${theme('colors.gc-line')}`,
                    background: theme('colors.gc-bg2'),
                    borderRadius: theme('borderRadius.gc-md'),
                    padding: theme('spacing.gc-3'),
                    overflow: 'hidden',
                },

                '.gc-rank-row.top::before': {
                    content: '""',
                    position: 'absolute',
                    inset: '0',
                    pointerEvents: 'none',
                    zIndex: '0',
                    opacity: '0.9',
                    filter: 'blur(14px)',
                },

                '.gc-rank-row.top1::before': {
                    background: 'linear-gradient(90deg, rgba(255, 215, 0, 0.18), transparent 60%)',
                },

                '.gc-rank-row.top2::before': {
                    background: 'linear-gradient(90deg, rgba(192, 192, 192, 0.18), transparent 60%)',
                },

                '.gc-rank-row.top3::before': {
                    background: 'linear-gradient(90deg, rgba(205, 127, 50, 0.18), transparent 60%)',
                },

                // 置頂樣式
                '.gc-pinned': {
                    border: '2px dashed #c4b5fd',
                    background: 'rgba(196, 181, 253, 0.15)',
                    padding: theme('spacing.gc-3'),
                    borderRadius: theme('borderRadius.gc-md'),
                },

                // 跑馬燈
                '.gc-ticker': {
                    position: 'relative',
                    overflow: 'hidden',
                    border: `1px solid ${theme('colors.gc-line')}`,
                    background: theme('colors.gc-surface'),
                    borderRadius: theme('borderRadius.gc-md'),
                },

                '.gc-ticker-track': {
                    display: 'flex',
                    gap: theme('spacing.gc-8'),
                    padding: `${theme('spacing.gc-3)} ${theme('spacing.gc - 4')}`,
          whiteSpace: 'nowrap',
                        animation: 'gc-ticker 10s linear infinite',
        },
      });
    }
  ],
}
