import { apiClient } from './api';
import type { Product, CartItem, Order } from '@/types/store';

export const storeService = {
  // 產品相關
  async getProducts(): Promise<Product[]> {
    const response = await apiClient.get('/api/products');
    return response.data;
  },

  async getProduct(id: string): Promise<Product> {
    const response = await apiClient.get(`/api/products/${id}`);
    return response.data;
  },

  // 購物車相關
  async getCart(): Promise<CartItem[]> {
    const response = await apiClient.get('/api/cart');
    return response.data;
  },

  async addToCart(productId: string, quantity: number): Promise<CartItem> {
    const response = await apiClient.post('/api/cart/items', {
      productId,
      quantity
    });
    return response.data;
  },

  async updateCartItem(itemId: string, quantity: number): Promise<CartItem> {
    const response = await apiClient.put(`/api/cart/items/${itemId}`, {
      quantity
    });
    return response.data;
  },

  async removeFromCart(itemId: string): Promise<void> {
    await apiClient.delete(`/api/cart/items/${itemId}`);
  },

  async clearCart(): Promise<void> {
    await apiClient.delete('/api/cart');
  },

  // 訂單相關
  async createOrder(): Promise<Order> {
    const response = await apiClient.post('/api/orders');
    return response.data;
  },

  async getOrders(): Promise<Order[]> {
    const response = await apiClient.get('/api/orders');
    return response.data;
  },

  async getOrder(id: string): Promise<Order> {
    const response = await apiClient.get(`/api/orders/${id}`);
    return response.data;
  }
}; 