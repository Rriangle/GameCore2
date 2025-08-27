import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { gameService } from '@/services/gameService';
import { ErrorHandler } from '@/utils/errorHandler';
import type { Game, GameListQuery, GameListResponse, CreateGameRequest, UpdateGameRequest } from '@/types/game';

export const useGameStore = defineStore('game', () => {
    // 狀態
    const games = ref<Game[]>([]);
    const currentGame = ref<Game | null>(null);
    const categories = ref<string[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);
    const totalCount = ref(0);
    const currentPage = ref(1);
    const pageSize = ref(10);
    const totalPages = ref(0);

    // 計算屬性
    const hasGames = computed(() => games.value.length > 0);
    const isLoading = computed(() => loading.value);
    const hasError = computed(() => error.value !== null);
    const canLoadMore = computed(() => currentPage.value < totalPages.value);

    // 獲取遊戲列表
    const fetchGames = async (query: GameListQuery = {}) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await gameService.getGames(query);
            
            if (response.success) {
                games.value = response.data.games;
                totalCount.value = response.data.totalCount;
                currentPage.value = response.data.page;
                pageSize.value = response.data.pageSize;
                totalPages.value = response.data.totalPages;
            } else {
                error.value = response.message || '獲取遊戲列表失敗';
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '獲取遊戲列表時發生錯誤');
            error.value = errorMessage;
        } finally {
            loading.value = false;
        }
    };

    // 獲取單個遊戲
    const fetchGame = async (gameId: number) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await gameService.getGame(gameId);
            
            if (response.success) {
                currentGame.value = response.data;
            } else {
                error.value = response.message || '獲取遊戲資訊失敗';
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '獲取遊戲資訊時發生錯誤');
            error.value = errorMessage;
        } finally {
            loading.value = false;
        }
    };

    // 創建遊戲
    const createGame = async (gameData: CreateGameRequest) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await gameService.createGame(gameData);
            
            if (response.success) {
                // 將新遊戲添加到列表中
                games.value.unshift(response.data);
                totalCount.value++;
                return { success: true, game: response.data };
            } else {
                error.value = response.message || '創建遊戲失敗';
                return { success: false, message: error.value };
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '創建遊戲時發生錯誤');
            error.value = errorMessage;
            return { success: false, message: errorMessage };
        } finally {
            loading.value = false;
        }
    };

    // 更新遊戲
    const updateGame = async (gameId: number, gameData: UpdateGameRequest) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await gameService.updateGame(gameId, gameData);
            
            if (response.success) {
                // 更新列表中的遊戲
                const index = games.value.findIndex(g => g.gameId === gameId);
                if (index !== -1) {
                    games.value[index] = response.data;
                }
                
                // 如果當前遊戲是更新的遊戲，也要更新
                if (currentGame.value?.gameId === gameId) {
                    currentGame.value = response.data;
                }
                
                return { success: true, game: response.data };
            } else {
                error.value = response.message || '更新遊戲失敗';
                return { success: false, message: error.value };
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '更新遊戲時發生錯誤');
            error.value = errorMessage;
            return { success: false, message: errorMessage };
        } finally {
            loading.value = false;
        }
    };

    // 刪除遊戲
    const deleteGame = async (gameId: number) => {
        loading.value = true;
        error.value = null;

        try {
            const response = await gameService.deleteGame(gameId);
            
            if (response.success) {
                // 從列表中移除遊戲
                games.value = games.value.filter(g => g.gameId !== gameId);
                totalCount.value--;
                
                // 如果當前遊戲是被刪除的遊戲，清空當前遊戲
                if (currentGame.value?.gameId === gameId) {
                    currentGame.value = null;
                }
                
                return { success: true };
            } else {
                error.value = response.message || '刪除遊戲失敗';
                return { success: false, message: error.value };
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '刪除遊戲時發生錯誤');
            error.value = errorMessage;
            return { success: false, message: errorMessage };
        } finally {
            loading.value = false;
        }
    };

    // 獲取遊戲分類
    const fetchCategories = async () => {
        loading.value = true;
        error.value = null;

        try {
            const response = await gameService.getCategories();
            
            if (response.success) {
                categories.value = response.data;
            } else {
                error.value = response.message || '獲取遊戲分類失敗';
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '獲取遊戲分類時發生錯誤');
            error.value = errorMessage;
        } finally {
            loading.value = false;
        }
    };

    // 載入更多遊戲（分頁）
    const loadMoreGames = async (query: GameListQuery = {}) => {
        if (!canLoadMore.value || loading.value) return;

        loading.value = true;
        error.value = null;

        try {
            const nextPage = currentPage.value + 1;
            const response = await gameService.getGames({ ...query, page: nextPage });
            
            if (response.success) {
                games.value.push(...response.data.games);
                currentPage.value = response.data.page;
                totalPages.value = response.data.totalPages;
            } else {
                error.value = response.message || '載入更多遊戲失敗';
            }
        } catch (err: any) {
            const errorMessage = ErrorHandler.formatErrorMessage(err, '載入更多遊戲時發生錯誤');
            error.value = errorMessage;
        } finally {
            loading.value = false;
        }
    };

    // 清除錯誤
    const clearError = () => {
        error.value = null;
    };

    // 重置狀態
    const reset = () => {
        games.value = [];
        currentGame.value = null;
        categories.value = [];
        loading.value = false;
        error.value = null;
        totalCount.value = 0;
        currentPage.value = 1;
        pageSize.value = 10;
        totalPages.value = 0;
    };

    return {
        // 狀態
        games,
        currentGame,
        categories,
        loading,
        error,
        totalCount,
        currentPage,
        pageSize,
        totalPages,

        // 計算屬性
        hasGames,
        isLoading,
        hasError,
        canLoadMore,

        // 方法
        fetchGames,
        fetchGame,
        createGame,
        updateGame,
        deleteGame,
        fetchCategories,
        loadMoreGames,
        clearError,
        reset
    };
});