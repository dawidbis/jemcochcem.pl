import type { User, DiarySummary, Food, ExternalFood } from './types';

export const api = {
  async loadDiary(date: string, userId: string): Promise<DiarySummary | null> {
    const res = await fetch(`/api/diary/${date}?userId=${userId}`);
    return res.ok ? res.json() : null;
  },
  async searchFoods(query: string): Promise<Food[]> {
    const res = await fetch(`/api/foods/search?query=${query}`);
    return res.ok ? res.json() : [];
  },
  async fetchExternalFood(barcode: string): Promise<ExternalFood | null> {
    const res = await fetch(`/api/foods/external/${barcode}`);
    return res.ok ? res.json() : null;
  },
  async saveFood(payload: any) {
    return fetch('/api/foods', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async addMealItem(payload: any) {
    return fetch('/api/diary/items', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async deleteMealItem(date: string, itemId: string, userId: string) {
    return fetch(`/api/diary/${date}/items/${itemId}?userId=${userId}`, { method: 'DELETE' });
  },
  async login(payload: any) {
    return fetch('/api/users/login', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async register(payload: any) {
    return fetch('/api/users/register', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  }
};