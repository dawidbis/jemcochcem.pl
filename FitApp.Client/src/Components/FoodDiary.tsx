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
    <div className="max-w-6xl mx-auto space-y-6">
      <header className="flex justify-between items-center p-6 bg-white shadow-sm rounded-2xl border border-slate-100">
        <h1 className="text-3xl font-extrabold bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-indigo-600">FitApp</h1>
        <Button variant="outline" className="hover:bg-slate-50" onClick={onLogout}>Wyloguj</Button>
      </header>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
        <div className="lg:col-span-7 space-y-6">
          <ManualFoodForm onAdded={() => setSearch('')} />

          <Card className="shadow-sm border-slate-100">
            <CardHeader className="pb-3">
              <CardTitle className="text-lg font-semibold text-slate-700">Skaner (OFF)</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex gap-3 mb-2">
                <Input className="bg-slate-50 focus-visible:ring-blue-500" value={barcode} onChange={e => setBarcode(e.target.value)} placeholder="Kod kreskowy" /> 
                <Button className="bg-slate-800 hover:bg-slate-700" onClick={fetchExternal}>Szukaj</Button>
              </div>
              {externalFood && (
                <div className="p-4 border border-blue-100 rounded-xl bg-blue-50 flex justify-between items-center mt-4 transition-all">
                  <strong className="text-blue-900">{externalFood.name}</strong>
                  <Button size="sm" className="bg-blue-600 hover:bg-blue-700" onClick={saveExternal}>Zapisz</Button>
                </div>
              )}
            </CardContent>
          </Card>

          <Card className="shadow-sm border-slate-100">
            <CardHeader className="pb-3">
              <CardTitle className="text-lg font-semibold text-slate-700">Wyszukiwarka bazy</CardTitle>
            </CardHeader>
            <CardContent>
              <Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Zacznij pisać..." className="mb-6 bg-slate-50 focus-visible:ring-blue-500 text-lg py-6" />
              <ul className="space-y-3">
                {results.map(f => (
                  <li key={f.id} className="p-4 border border-slate-100 rounded-xl hover:shadow-md hover:border-blue-100 transition-all bg-white group">
                    <div className="flex flex-col sm:flex-row justify-between gap-4">
                      <div>
                        <strong className="text-slate-800 text-lg">{f.name}</strong> <span className="text-sm font-medium text-slate-500 ml-2">{f.caloriesPer100g} kcal/100g</span>
                        <div className="flex gap-2 mt-2 text-xs font-semibold">
                          <span className="bg-blue-50 text-blue-700 px-2 py-1 rounded-md">B: {f.macros?.protein || 0}</span>
                          <span className="bg-amber-50 text-amber-700 px-2 py-1 rounded-md">W: {f.macros?.carbs || 0}</span>
                          <span className="bg-rose-50 text-rose-700 px-2 py-1 rounded-md">T: {f.macros?.fats || 0}</span>
                        </div>
                      </div>
                      <div className="flex items-center gap-3">
                        <div className="flex items-center gap-1">
                          <Input type="number" className="w-20 text-center bg-slate-50" value={grams} onChange={e => setGrams(Number(e.target.value))} />
                          <span className="text-sm font-medium text-slate-500">g</span>
                        </div>
                        <Button size="sm" className="bg-indigo-600 hover:bg-indigo-700" onClick={() => addMeal(f.id)}>Dodaj</Button>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </CardContent>
          </Card>
        </div>

        <div className="lg:col-span-5">
          <Card className="shadow-xl border-0 overflow-hidden sticky top-6">
            <CardHeader className="bg-gradient-to-br from-slate-900 to-slate-800 text-white p-6">
              <CardTitle className="text-3xl font-bold tracking-tight">Suma: {diary?.totalCalories || 0} kcal</CardTitle>
              <div className="flex gap-4 mt-3 text-sm font-medium">
                <span className="bg-white/10 px-3 py-1.5 rounded-lg">Białko: {diary?.totalProtein || 0}g</span>
                <span className="bg-white/10 px-3 py-1.5 rounded-lg">Węgle: {diary?.totalCarbs || 0}g</span>
                <span className="bg-white/10 px-3 py-1.5 rounded-lg">Tłuszcze: {diary?.totalFats || 0}g</span>
              </div>
            </CardHeader>
            <CardContent className="p-6 bg-white">
              <div className="space-y-4">
                {diary?.items.map(item => (
                  <div key={item.id} className="flex justify-between items-center pb-4 border-b border-slate-100 last:border-0 last:pb-0 group">
                    <div>
                      <strong className="text-slate-800">{item.foodName}</strong> <span className="text-sm text-slate-500 bg-slate-100 px-2 py-0.5 rounded-full ml-1">{item.grams}g</span>
                      <div className="text-sm font-semibold text-slate-600 mt-1">{item.calories} kcal</div>
                      <div className="flex gap-2 text-xs font-medium mt-1">
                        <span className="text-blue-600">B: {item.macros?.protein}</span>
                        <span className="text-amber-600">W: {item.macros?.carbs}</span>
                        <span className="text-rose-600">T: {item.macros?.fats}</span>
                      </div>
                    </div>
                    <Button variant="ghost" className="text-slate-300 hover:text-red-500 hover:bg-red-50 transition-colors" size="icon" onClick={() => deleteMeal(item.id)}>✖</Button>
                  </div>
                ))}
                {!diary?.items?.length && (
                  <div className="text-center py-8 text-slate-400">
                    Brak posiłków. Dodaj coś!
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}