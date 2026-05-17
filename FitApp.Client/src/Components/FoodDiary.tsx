import { useState, useEffect } from 'react';
import type { User, DiarySummary, Food, ExternalFood } from '../types';
import { ManualFoodForm } from './ManualFoodForm';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "#components/ui/card";

export function FoodDiary({ user, onLogout }: { user: User, onLogout: () => void }) {
  const [diary, setDiary] = useState<DiarySummary | null>(null);
  const [search, setSearch] = useState('');
  const [results, setResults] = useState<Food[]>([]);
  const [barcode, setBarcode] = useState('');
  const [externalFood, setExternalFood] = useState<ExternalFood | null>(null);
  const [grams, setGrams] = useState(100);
  const TODAY = new Date().toISOString().split('T')[0];

  const loadDiary = async () => {
    const data = await api.loadDiary(TODAY, user.userId);
    if (data) setDiary(data);
  };

  useEffect(() => { loadDiary(); }, []);

  useEffect(() => {
    if (!search.trim()) { setResults([]); return; }
    const timer = setTimeout(async () => {
      const data = await api.searchFoods(search);
      setResults(data);
    }, 300);
    return () => clearTimeout(timer);
  }, [search]);

  const fetchExternal = async () => {
    const data = await api.fetchExternalFood(barcode);
    if (data) setExternalFood(data);
  };

  const saveExternal = async () => {
    if (!externalFood) return;
    const payload = { name: externalFood.name, barcode, caloriesPer100g: externalFood.caloriesPer100g, proteinPer100g: externalFood.macros?.protein || 0, carbsPer100g: externalFood.macros?.carbs || 0, fatPer100g: externalFood.macros?.fats || 0 };
    await api.saveFood(payload);
    setExternalFood(null); setBarcode('');
  };

  const addMeal = async (foodId: string) => {
    await api.addMealItem({ userId: user.userId, date: new Date().toISOString(), foodProductId: foodId, grams });
    loadDiary(); setResults([]); setSearch('');
  };

  const deleteMeal = async (itemId: string) => {
    await api.deleteMealItem(TODAY, itemId, user.userId);
    loadDiary();
  };

  return (
    <div className="max-w-6xl mx-auto">
      <header className="flex justify-between items-center pb-4 border-b mb-6">
        <h1 className="text-3xl font-bold">FitApp</h1>
        <Button variant="outline" onClick={onLogout}>Wyloguj</Button>
      </header>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div>
          <ManualFoodForm onAdded={() => setSearch('')} />

          <Card className="mb-6">
            <CardHeader>
              <CardTitle className="text-lg">Skaner OFF</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex gap-2 mb-2">
                <Input value={barcode} onChange={e => setBarcode(e.target.value)} placeholder="Kod kreskowy" /> 
                <Button onClick={fetchExternal}>Szukaj</Button>
              </div>
              {externalFood && (
                <div className="p-3 border rounded-md bg-slate-50 flex justify-between items-center mt-4">
                  <strong>{externalFood.name}</strong>
                  <Button size="sm" onClick={saveExternal}>Zapisz</Button>
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-lg">Wyszukiwarka bazy</CardTitle>
            </CardHeader>
            <CardContent>
              <Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Zacznij pisać..." className="mb-4" />
              <ul className="space-y-4">
                {results.map(f => (
                  <li key={f.id} className="p-3 border rounded-md">
                    <div className="flex justify-between">
                      <div>
                        <strong>{f.name}</strong> <span className="text-sm text-slate-500">({f.caloriesPer100g} kcal)</span>
                        <div className="text-xs text-slate-500 mt-1">B: {f.macros?.protein || 0} W: {f.macros?.carbs || 0} T: {f.macros?.fats || 0}</div>
                      </div>
                      <div className="flex items-center gap-2">
                        <Input type="number" className="w-16 h-8" value={grams} onChange={e => setGrams(Number(e.target.value))} /> <span className="text-sm">g</span>
                        <Button size="sm" onClick={() => addMeal(f.id)}>Dodaj</Button>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </CardContent>
          </Card>
        </div>

        <div>
          <Card>
            <CardHeader className="bg-slate-900 text-white rounded-t-xl">
              <CardTitle className="text-2xl">Suma: {diary?.totalCalories || 0} kcal</CardTitle>
              <p className="text-sm opacity-80">B: {diary?.totalProtein || 0}g | W: {diary?.totalCarbs || 0}g | T: {diary?.totalFats || 0}g</p>
            </CardHeader>
            <CardContent className="pt-6">
              <div className="space-y-4">
                {diary?.items.map(item => (
                  <div key={item.id} className="flex justify-between items-center pb-4 border-b last:border-0">
                    <div>
                      <strong>{item.foodName}</strong> <span className="text-sm text-slate-500">({item.grams}g)</span>
                      <div className="text-xs text-slate-500 mt-1">{item.calories} kcal | B:{item.macros?.protein} W:{item.macros?.carbs} T:{item.macros?.fats}</div>
                    </div>
                    <Button variant="destructive" size="icon" onClick={() => deleteMeal(item.id)}>✖</Button>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}