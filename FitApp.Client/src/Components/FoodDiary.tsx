import { useState, useEffect } from 'react';
import type { User, DiarySummary, Food, ExternalFood } from '../types';
import { ManualFoodForm } from './ManualFoodForm';
import type {TargetMacros} from './TdeeCalculator';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";

export function FoodDiary({ user }: { user: User }) {
  const [diary, setDiary] = useState<DiarySummary | null>(null);
  const [search, setSearch] = useState('');
  const [results, setResults] = useState<Food[]>([]);
  const [barcode, setBarcode] = useState('');
  const [externalFood, setExternalFood] = useState<ExternalFood | null>(null);
  const [grams, setGrams] = useState(100);
  const [targets, setTargets] = useState<TargetMacros | null>(null);
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
  
  useEffect(() => {
  api.calculateMacros(user.userId, 1.55).then(data => { // 1.55 jako fallback
    if (data) setTargets({
      tdee: data.tdee || data.targetCalories,
      protein: data.protein,
      carbs: data.carbs,
      fats: data.fats || data.fat
    });
  });
}, 
[user.userId]);
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
    <div className="grid grid-cols-1 xl:grid-cols-3 gap-6 items-start w-full">
      
      {/* Kolumna 1: Formularze dodawania */}
      <div className="space-y-6 w-full">    
        <ManualFoodForm onAdded={() => setSearch('')} />
        
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="p-4 border-b border-slate-100 bg-slate-50/50">
            <h3 className="text-lg font-semibold text-slate-700">Skaner (OFF)</h3>
          </div>
          <div className="p-4">
            <div className="flex gap-2">
              <Input className="bg-white focus-visible:ring-blue-500 flex-1" value={barcode} onChange={e => setBarcode(e.target.value)} placeholder="Kod kreskowy" /> 
              <Button className="bg-blue-600 hover:bg-blue-700" onClick={fetchExternal}>Szukaj</Button>
            </div>
            {externalFood && (
              <div className="p-4 border border-blue-100 rounded-xl bg-blue-50 flex flex-col gap-3 mt-4">
                <strong className="text-blue-900">{externalFood.name}</strong>
                <div className="flex justify-between items-center text-sm font-medium">
                  <span>{externalFood.caloriesPer100g} kcal/100g</span>
                  <Button size="sm" className="bg-blue-600 hover:bg-blue-700" onClick={saveExternal}>Zapisz</Button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Kolumna 2: Wyniki Wyszukiwania */}
      <div className="space-y-6 w-full">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden flex flex-col xl:max-h-[calc(100vh-8rem)]">
          <div className="p-5 bg-slate-50/50 border-b border-slate-100">
            <h3 className="text-lg font-semibold text-slate-700">Wyszukiwarka bazy</h3>
            <Input value={search} onChange={e => setSearch(e.target.value)} placeholder="Wpisz nazwę produktu..." className="mt-3 bg-white focus-visible:ring-blue-500 text-base py-5 shadow-sm" />
          </div>
          <div className="overflow-y-auto p-4 space-y-3 flex-1 bg-slate-50/30">
            {results.map(f => (
              <div key={f.id} className="p-4 border border-slate-200 rounded-xl hover:border-blue-300 hover:shadow-md transition-all bg-white flex flex-col gap-3">
                <div>
                  <strong className="text-slate-800 text-lg">{f.name}</strong> <span className="text-sm font-medium text-slate-500 ml-2">{f.caloriesPer100g} kcal/100g</span>
                  <div className="flex gap-2 mt-2 text-xs font-semibold">
                    <span className="bg-blue-50 text-blue-700 px-2 py-1 rounded-md">B: {f.macros?.protein || 0}</span>
                    <span className="bg-amber-50 text-amber-700 px-2 py-1 rounded-md">W: {f.macros?.carbs || 0}</span>
                    <span className="bg-rose-50 text-rose-700 px-2 py-1 rounded-md">T: {f.macros?.fats || 0}</span>
                  </div>
                </div>
                <div className="flex items-center justify-end gap-2 pt-3 border-t border-slate-100 mt-1">
                  <Input type="number" className="w-20 text-center bg-slate-50 h-9" value={grams} onChange={e => setGrams(Number(e.target.value))} />
                  <span className="text-sm font-medium text-slate-500 mr-1">g</span>
                  <Button size="sm" className="bg-indigo-600 hover:bg-indigo-700" onClick={() => addMeal(f.id)}>Dodaj</Button>
                </div>
              </div>
            ))}
            {results.length === 0 && search.length > 0 && (
              <div className="text-center py-8 text-slate-500 font-medium">Brak wyników.</div>
            )}
          </div>
        </div>
      </div>

      {/* Kolumna 3: Dziennik Posiłków */}
      <div className="space-y-6 w-full">
        <div className="bg-white rounded-xl border border-slate-200 shadow-xl sticky top-24 overflow-hidden flex flex-col xl:max-h-[calc(100vh-8rem)]">
          <div className="bg-slate-900 text-white p-6 shrink-0">
            <h2 className="text-2xl font-bold tracking-tight">Kcal: {diary?.totalCalories || 0} / {targets?.tdee || '-'}</h2>
            <div className="grid grid-cols-3 gap-3 mt-5 text-center text-sm font-medium">
              <div className="bg-white/10 p-3 rounded-xl border border-white/5">
                <span className="block text-slate-400 text-[10px] uppercase tracking-wider mb-1">Białko</span>
                {diary?.totalProtein || 0} / {targets?.protein || '-'}g
              </div>
              <div className="bg-white/10 p-3 rounded-xl border border-white/5">
                <span className="block text-slate-400 text-[10px] uppercase tracking-wider mb-1">Węgle</span>
                {diary?.totalCarbs || 0} / {targets?.carbs || '-'}g
              </div>
              <div className="bg-white/10 p-3 rounded-xl border border-white/5">
                <span className="block text-slate-400 text-[10px] uppercase tracking-wider mb-1">Tłuszcz</span>
                {diary?.totalFats || 0} / {targets?.fats || '-'}g
              </div>
            </div>
          </div>
          <div className="overflow-y-auto divide-y divide-slate-100 flex-1">
            {diary?.items.map(item => (
              <div key={item.id} className="flex justify-between items-center p-5 hover:bg-slate-50 transition-colors">
                <div>
                  <strong className="text-slate-800">{item.foodName}</strong> 
                  <span className="text-xs font-semibold text-slate-500 bg-slate-200 px-2 py-0.5 rounded-full ml-2">{item.grams}g</span>
                  <div className="text-sm font-bold text-slate-700 mt-1.5">{item.calories} kcal</div>
                  <div className="flex gap-2 text-xs font-medium mt-1.5">
                    <span className="text-blue-600">B: {item.macros?.protein}</span>
                    <span className="text-amber-600">W: {item.macros?.carbs}</span>
                    <span className="text-rose-600">T: {item.macros?.fats}</span>
                  </div>
                </div>
                <Button variant="ghost" className="text-slate-400 hover:text-red-600 hover:bg-red-50 h-8 w-8 p-0 rounded-full" onClick={() => deleteMeal(item.id)}>✖</Button>
              </div>
            ))}
            {!diary?.items?.length && (
              <div className="text-center py-12 text-slate-400 text-sm font-medium">Brak posiłków. Dodaj coś z wyszukiwarki!</div>
            )}
          </div>
        </div>
      </div>

    </div>
  );
}