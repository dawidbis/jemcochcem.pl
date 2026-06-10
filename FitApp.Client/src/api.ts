import type { DiarySummary, Food, ExternalFood, Measurement, MeasurementStats, CreateMeasurementPayload, AiMealPlan, WaterStatusDto, Exercise, WorkoutSession, LogWorkoutPayload, ExerciseProgression } from './types';

let accessToken: string | null = localStorage.getItem('accessToken');

export function setTokens(access: string, refresh: string, userId: string) {
  accessToken = access;
  localStorage.setItem('accessToken', access);
  localStorage.setItem('refreshToken', refresh);
  localStorage.setItem('userId', userId);
}

export function clearTokens() {
  accessToken = null;
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('userId');
}

async function tryRefresh(): Promise<boolean> {
  const refreshToken = localStorage.getItem('refreshToken');
  const userId = localStorage.getItem('userId');
  if (!refreshToken || !userId) return false;

  const res = await fetch('/api/users/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ userId, refreshToken }),
  });
  if (!res.ok) { clearTokens(); return false; }

  const data = await res.json();
  accessToken = data.accessToken;
  localStorage.setItem('accessToken', data.accessToken);
  localStorage.setItem('refreshToken', data.refreshToken);
  return true;
}

async function authFetch(url: string, options: RequestInit = {}): Promise<Response> {
  const withAuth = (): RequestInit => ({
    ...options,
    headers: { ...(options.headers || {}), Authorization: `Bearer ${accessToken}` },
  });

  let res = await fetch(url, withAuth());
  if (res.status === 401 && await tryRefresh()) {
    res = await fetch(url, withAuth());
  }
  return res;
}

export const api = {
  async loadDiary(date: string): Promise<DiarySummary | null> {
    const res = await authFetch(`/api/diary/${date}`);
    return res.ok ? res.json() : null;
  },
  async getMonthlyCalendar(year: number, month: number): Promise<Record<string, { totalCalories: number; totalProtein: number; totalCarbs: number; totalFats: number; waterMl: number }>> {
    const res = await authFetch(`/api/diary/monthly-calendar?year=${year}&month=${month}`);
    return res.ok ? res.json() : {};
  },
  async searchFoods(query: string): Promise<Food[]> {
    const res = await authFetch(`/api/foods/search?query=${query}`);
    return res.ok ? res.json() : [];
  },
  async fetchExternalFood(barcode: string): Promise<ExternalFood | null> {
    const res = await authFetch(`/api/foods/external/${barcode}`);
    return res.ok ? res.json() : null;
  },
  async saveFood(payload: any) {
    return authFetch('/api/foods', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async addMealItem(payload: any) {
    return authFetch('/api/diary/items', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async deleteMealItem(date: string, itemId: string) {
    return authFetch(`/api/diary/${date}/items/${itemId}`, { method: 'DELETE' });
  },

  async login(payload: any) {
    return fetch('/api/users/login', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async register(payload: any) {
    return fetch('/api/users/register', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },

  // User profile
  async getUserProfile(userId: string) {
    const res = await authFetch(`/api/users/${userId}`);
    return res.ok ? res.json() : null;
  },
  async updateUserProfile(userId: string, payload: any) {
    return authFetch(`/api/users/${userId}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
  },
  async calculateMacros() {
    const res = await authFetch('/api/users/me/macros', { method: 'POST', headers: { 'Content-Type': 'application/json' } });
    return res.ok ? res.json() : null;
  },
  async calculateMicros() {
    const res = await authFetch('/api/users/me/micros', { method: 'POST', headers: { 'Content-Type': 'application/json' } });
    return res.ok ? res.json() : null;
  },

  // Measurements
  async getMeasurements(): Promise<Measurement[]> {
    const res = await authFetch(`/api/measurements/me`);
    return res.ok ? res.json() : [];
  },
  async getMeasurementStats(): Promise<MeasurementStats | null> {
    const res = await authFetch(`/api/measurements/me/stats`);
    return res.ok ? res.json() : null;
  },
  async createMeasurement(payload: CreateMeasurementPayload): Promise<string | null> {
    const res = await authFetch('/api/measurements', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    if (!res.ok) return null;
    const data = await res.json();
    return data.id;
  },
  async deleteMeasurement(id: string): Promise<boolean> {
    const res = await authFetch(`/api/measurements/${id}`, { method: 'DELETE' });
    return res.ok;
  },
  async setTargetWeight(targetWeight: number | null): Promise<boolean> {
    const res = await authFetch('/api/users/me/target-weight', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ targetWeight }),
    });
    return res.ok;
  },

  // --- AI Meal Plans ---
  async generateAiMealPlan(payload: { prompt: string }): Promise<{ mealPlanId: string } | null> {
    const res = await authFetch('/api/MealPlans/generate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return res.ok ? res.json() : null;
  },
  async addAiMealToDiary(payload: { mealPlanItemId: string; date: string }) {
    return authFetch('/api/Diary/items/from-ai-plan', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
  },
  async getUserMealPlans(): Promise<AiMealPlan[]> {
    const res = await authFetch(`/api/MealPlans`);
    return res.ok ? res.json() : [];
  },

  // --- Water ---
  async getWaterStatus(date: string): Promise<WaterStatusDto | null> {
    const res = await authFetch(`/api/Water/status?date=${date}`);
    return res.ok ? res.json() : null;
  },
  async logWater(payload: { date: string; amountMl: number }) {
    return authFetch('/api/Water/log', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
  },

  // --- Treningi ---
  async getExercises(): Promise<Exercise[]> {
    const res = await authFetch('/api/Workouts/exercises');
    return res.ok ? res.json() : [];
  },
  async addCustomExercise(payload: { name: string; muscleGroup: string }): Promise<{ id: string } | null> {
    const res = await authFetch('/api/Workouts/exercises', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return res.ok ? res.json() : null;
  },
  async deleteCustomExercise(id: string): Promise<boolean> {
    const res = await authFetch(`/api/Workouts/exercises/${id}`, { method: 'DELETE' });
    return res.ok;
  },
  async getExerciseProgression(exerciseId: string): Promise<ExerciseProgression | null> {
    const res = await authFetch(`/api/Workouts/exercises/${exerciseId}/progression`);
    return res.ok ? res.json() : null;
  },
  async logWorkout(payload: LogWorkoutPayload) {
    return authFetch('/api/Workouts/sessions', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
  },
  async deleteWorkoutSession(id: string): Promise<boolean> {
    const res = await authFetch(`/api/Workouts/sessions/${id}`, { method: 'DELETE' });
    return res.ok;
  },
  async getWorkoutHistory(): Promise<WorkoutSession[]> {
    const res = await authFetch('/api/Workouts/sessions');
    return res.ok ? res.json() : [];
  },
  async getWorkoutHistoryRange(from: string, to: string): Promise<WorkoutSession[]> {
    const res = await authFetch(`/api/Workouts/sessions?from=${from}&to=${to}`);
    return res.ok ? res.json() : [];
  },
};