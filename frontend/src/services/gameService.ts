import { apiService } from './api';
import type { Game, GameListQuery, GameListResponse, CreateGameRequest, UpdateGameRequest } from '@/types/game';

class GameService {
    private readonly baseUrl = '/game';

    /**
     * 獲取遊戲列表
     */
    async getGames(query: GameListQuery = {}): Promise<GameListResponse> {
        const queryParams = new URLSearchParams();
        
        if (query.searchTerm) queryParams.append('searchTerm', query.searchTerm);
        if (query.category) queryParams.append('category', query.category);
        if (query.minPrice !== undefined) queryParams.append('minPrice', query.minPrice.toString());
        if (query.maxPrice !== undefined) queryParams.append('maxPrice', query.maxPrice.toString());
        if (query.isActive !== undefined) queryParams.append('isActive', query.isActive.toString());
        if (query.page) queryParams.append('page', query.page.toString());
        if (query.pageSize) queryParams.append('pageSize', query.pageSize.toString());
        if (query.sortBy) queryParams.append('sortBy', query.sortBy);
        if (query.sortDescending !== undefined) queryParams.append('sortDescending', query.sortDescending.toString());

        const url = `${this.baseUrl}?${queryParams.toString()}`;
        return await apiService.get<GameListResponse>(url);
    }

    /**
     * 根據 ID 獲取遊戲
     */
    async getGame(gameId: number): Promise<{ success: boolean; data?: Game; message?: string }> {
        try {
            const response = await apiService.get<Game>(`${this.baseUrl}/${gameId}`);
            return {
                success: response.success,
                data: response.data,
                message: response.message
            };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || '獲取遊戲資訊失敗'
            };
        }
    }

    /**
     * 創建新遊戲
     */
    async createGame(gameData: CreateGameRequest): Promise<{ success: boolean; data?: Game; message?: string }> {
        try {
            const response = await apiService.post<Game>(this.baseUrl, gameData);
            return {
                success: response.success,
                data: response.data,
                message: response.message
            };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || '創建遊戲失敗'
            };
        }
    }

    /**
     * 更新遊戲
     */
    async updateGame(gameId: number, gameData: UpdateGameRequest): Promise<{ success: boolean; data?: Game; message?: string }> {
        try {
            const response = await apiService.put<Game>(`${this.baseUrl}/${gameId}`, gameData);
            return {
                success: response.success,
                data: response.data,
                message: response.message
            };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || '更新遊戲失敗'
            };
        }
    }

    /**
     * 刪除遊戲
     */
    async deleteGame(gameId: number): Promise<{ success: boolean; message?: string }> {
        try {
            const response = await apiService.delete<object>(`${this.baseUrl}/${gameId}`);
            return {
                success: response.success,
                message: response.message
            };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || '刪除遊戲失敗'
            };
        }
    }

    /**
     * 獲取遊戲分類列表
     */
    async getCategories(): Promise<{ success: boolean; data?: string[]; message?: string }> {
        try {
            const response = await apiService.get<string[]>(`${this.baseUrl}/categories`);
            return {
                success: response.success,
                data: response.data,
                message: response.message
            };
        } catch (error: any) {
            return {
                success: false,
                message: error.response?.data?.message || '獲取遊戲分類失敗'
            };
        }
    }
}

export const gameService = new GameService();