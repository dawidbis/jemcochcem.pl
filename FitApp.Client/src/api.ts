import type { DiarySummary, Food, ExternalFood, Measurement, MeasurementStats, CreateMeasurementPayload } from './types';

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
  },

  // Measurements
  async getMeasurements(userId: string): Promise<Measurement[]> {
    const res = await fetch(`/api/measurements/user/${userId}`);
    return res.ok ? res.json() : [];
  },
  async getMeasurementStats(userId: string): Promise<MeasurementStats | null> {
    const res = await fetch(`/api/measurements/user/${userId}/stats`);
    return res.ok ? res.json() : null;
  },
  async createMeasurement(payload: CreateMeasurementPayload): Promise<string | null> {
    const res = await fetch('/api/measurements', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    if (!res.ok) return null;
    const data = await res.json();
    return data.id;
  },
  async deleteMeasurement(id: string): Promise<boolean> {
    const res = await fetch(`/api/measurements/${id}`, { method: 'DELETE' });
    return res.ok;
  },
  async setTargetWeight(userId: string, targetWeight: number | null): Promise<boolean> {
    const res = await fetch(`/api/users/${userId}/target-weight`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ targetWeight }),
    });
    return res.ok;
  },
};
