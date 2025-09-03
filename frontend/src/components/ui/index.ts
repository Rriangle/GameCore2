// GameCore UI 組件庫索引
// 提供完整的玻璃風設計系統組件庫，支援響應式設計和無障礙性

// 基礎組件
export { default as GCButton } from './GCButton.vue'
export { default as GCInput } from './GCInput.vue'
export { default as GCCard } from './GCCard.vue'

// 數據展示組件
export { default as GCTable } from './GCTable.vue'
export { default as GCPagination } from './GCPagination.vue'
export { default as GCTabs } from './GCTabs.vue'
export { default as GCAccordion } from './GCAccordion.vue'

// 反饋組件
export { default as GCModal } from './GCModal.vue'
export { default as GCToast } from './GCToast.vue'
export { default as ToastContainer } from './ToastContainer.vue'

// 組件庫文檔頁面
export { default as ComponentLibrary } from './ComponentLibrary.vue'

// 組件類型定義
export interface ComponentInfo {
    name: string
    description: string
    category: 'basic' | 'data' | 'feedback' | 'navigation'
    props: string[]
    events: string[]
    slots: string[]
}

// 組件庫統計信息
export const componentStats = {
    total: 11,
    categories: 4,
    responsive: true,
    accessibility: 'WCAG 2.1 AA'
}

// 組件分類
export const componentCategories = {
    basic: {
        name: '基礎組件',
        description: '最常用的基礎 UI 組件',
        components: ['GCButton', 'GCInput', 'GCCard']
    },
    data: {
        name: '數據展示',
        description: '用於展示和操作數據的組件',
        components: ['GCTable', 'GCPagination', 'GCTabs', 'GCAccordion']
    },
    feedback: {
        name: '反饋組件',
        description: '提供用戶反饋和交互的組件',
        components: ['GCModal', 'GCToast', 'ToastContainer']
    },
    navigation: {
        name: '導航組件',
        description: '用於頁面導航和布局的組件',
        components: ['Breadcrumb', 'Navigation']
    }
}

// 組件使用指南
export const componentGuide = {
    designSystem: '基於 index.txt 的玻璃風設計系統',
    responsive: '完全響應式設計，支援所有設備尺寸',
    accessibility: '符合 WCAG 2.1 AA 無障礙標準',
    performance: '優化的組件性能，支援懶加載',
    theming: '支援深色模式和自定義主題'
}

// 快速導入
export const quickImport = {
    // 基礎組件
    button: "import { GCButton } from '@/components/ui'",
    input: "import { GCInput } from '@/components/ui'",
    card: "import { GCCard } from '@/components/ui'",

    // 數據展示
    table: "import { GCTable } from '@/components/ui'",
    pagination: "import { GCPagination } from '@/components/ui'",
    tabs: "import { GCTabs } from '@/components/ui'",
    accordion: "import { GCAccordion } from '@/components/ui'",

    // 反饋組件
    modal: "import { GCModal } from '@/components/ui'",
    toast: "import { GCToast } from '@/components/ui'"
}

// 組件庫版本信息
export const libraryVersion = {
    version: '2.0.0',
    lastUpdated: '2025-01-27',
    vueVersion: '^3.4.0',
    typescript: '^5.0.0'
}

// 默認導出
export default {
    // 組件
    GCButton,
    GCInput,
    GCCard,
    GCTable,
    GCPagination,
    GCTabs,
    GCAccordion,
    GCModal,
    GCToast,
    ToastContainer,
    ComponentLibrary,

    // 統計信息
    stats: componentStats,

    // 分類信息
    categories: componentCategories,

    // 使用指南
    guide: componentGuide,

    // 快速導入
    quickImport,

    // 版本信息
    version: libraryVersion
} 