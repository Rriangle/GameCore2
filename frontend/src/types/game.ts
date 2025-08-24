/**
 * 遊戲資訊
 */
export interface Game {
    gameId: number;
    name: string;
    description: string;
    category: string;
    price: number;
    isActive: boolean;
    createdAt: string;
    updatedAt?: string;
}

/**
 * 創建遊戲請求
 */
export interface CreateGameRequest {
    name: string;
    description: string;
    category: string;
    price: number;
}

/**
 * 更新遊戲請求
 */
export interface UpdateGameRequest {
    name?: string;
    description?: string;
    category?: string;
    price?: number;
    isActive?: boolean;
}

/**
 * 遊戲列表查詢參數
 */
export interface GameListQuery {
    searchTerm?: string;
    category?: string;
    minPrice?: number;
    maxPrice?: number;
    isActive?: boolean;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortDescending?: boolean;
}

/**
 * 遊戲列表回應
 */
export interface GameListResponse {
    games: Game[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

/**
 * 遊戲統計資訊
 */
export interface GameStats {
    totalGames: number;
    activeGames: number;
    totalCategories: number;
    averagePrice: number;
    mostExpensiveGame?: Game;
    cheapestGame?: Game;
}

/**
 * 遊戲分類統計
 */
export interface CategoryStats {
    category: string;
    count: number;
    averagePrice: number;
    totalValue: number;
}