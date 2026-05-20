import { useState, useEffect } from 'react';
import type { User, DiarySummary, Food, ExternalFood } from '../types';
import { ManualFoodForm } from './ManualFoodForm';
import type {TargetMacros} from './TdeeCalculator';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { AiMealPlanGenerator } from './AiMealPlanGenerator';

export function FoodDiary({ user }: { user: User }) {
  const [diary, setDiary] = useState<DiarySummary | null>(null);
  const [search, setSearch] = useState('');
  const [results, setResults] = useState<Food[]>([]);
  const [barcode, setBarcode] = useState('');
  const [externalFood, setExternalFood] = useState<ExternalFood | null>(null);
  const [grams, setGrams] = useState(100);
  const [targets, setTargets] = useState<TargetMacros | null>(null);
  const [selectedDate, setSelectedDate] = useState(() => new Date().toISOString().split('T')[0]);

  const isToday = selectedDate === new Date().toISOString().split('T')[0];

  const changeDate = (days: number) => {
    const d = new Date(selectedDate);
    d.setDate(d.getDate() + days);
    if (d <= new Date()) setSelectedDate(d.toISOString().split('T')[0]);
  };

  const formatDate = (dateStr: string) => {
    const d = new Date(dateStr);
    return d.toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
  };

  const loadDiary = async () => {
    const data = await api.loadDiary(selectedDate, user.userId);
    if (data) setDiary(data);
    else setDiary({ date: selectedDate, items: [], totalCalories: 0, totalProtein: 0, totalCarbs: 0, totalFats: 0 });
  };

  useEffect(() => { loadDiary(); }, [selectedDate]);

  useEffect(() => {
    if (!search.trim()) { setResults([]); return; }
    const timer = setTimeout(async () => {
      const data = await api.searchFoods(search);
      setResults(data);
    }, 300);
    return () => clearTimeout(timer);
  }, [search]);
  
  useEffect(() => {
    api.calculateMacros(user.userId, 1.55).then((data: any) => {
      if (data) setTargets({
        tdee: data.tdee || data.targetCalories,
        protein: data.protein,
        carbs: data.carbs,
        fats: data.fats || data.fat
      });
    });
  }, [user.userId]);

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
    await api.addMealItem({ userId: user.userId, date: new Date(selectedDate).toISOString(), foodProductId: foodId, grams });
    loadDiary(); setResults([]); setSearch('');
  };

  const deleteMeal = async (itemId: string) => {
    await api.deleteMealItem(selectedDate, itemId, user.userId);
    loadDiary();
  };

  return (
    <div className="space-y-6 w-full max-w-7xl mx-auto">
      
      {/* Header z datą */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Dziennik posiłków</h1>
          <p className="text-slate-500 text-sm mt-0.5">Loguj posiłki i śledź dzienne cele</p>
        </div>
        <div className="flex items-center gap-3 bg-white border border-slate-200 rounded-xl px-4 py-2.5 shadow-sm">
          <button onClick={() => changeDate(-1)} className="p-1 rounded-md hover:bg-slate-100 transition-colors text-slate-600 font-bold">←</button>
          <div className="text-center min-w-[180px]">
            <div className="text-sm font-semibold text-slate-700 capitalize">{formatDate(selectedDate)}</div>
            {isToday && <span className="text-[10px] uppercase tracking-wider text-emerald-600 font-semibold">Dzisiaj</span>}
          </div>
          <button onClick={() => changeDate(1)} disabled={isToday} className={`p-1 rounded-md transition-colors font-bold ${isToday ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100'}`}>→</button>
        </div>
      </div>

      {/* Podsumowanie kalorii */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <span className="text-xs text-slate-500 font-medium uppercase tracking-wider">Kalorie</span>
          <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalCalories || 0} <span className="text-base font-normal text-slate-400">/ {targets?.tdee || '-'}</span></p>
          {targets?.tdee && diary ? (
            <div className="mt-3 h-2 bg-slate-100 rounded-full overflow-hidden">
              <div className="h-full bg-blue-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalCalories / targets.tdee) * 100)}%` }} />
            </div>
          ) : null}
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <span className="text-xs text-blue-600 font-medium uppercase tracking-wider">Białko</span>
          <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalProtein || 0}<span className="text-base font-normal text-slate-400">g / {targets?.protein || '-'}g</span></p>
          {targets?.protein && diary ? (
            <div className="mt-3 h-2 bg-blue-50 rounded-full overflow-hidden">
              <div className="h-full bg-blue-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalProtein / targets.protein) * 100)}%` }} />
            </div>
          ) : null}
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <span className="text-xs text-amber-600 font-medium uppercase tracking-wider">Węglowodany</span>
          <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalCarbs || 0}<span className="text-base font-normal text-slate-400">g / {targets?.carbs || '-'}g</span></p>
          {targets?.carbs && diary ? (
            <div className="mt-3 h-2 bg-amber-50 rounded-full overflow-hidden">
              <div className="h-full bg-amber-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalCarbs / targets.carbs) * 100)}%` }} />
            </div>
          ) : null}
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <span className="text-xs text-rose-600 font-medium uppercase tracking-wider">Tłuszcz</span>
          <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalFats || 0}<span className="text-base font-normal text-slate-400">g / {targets?.fats || '-'}g</span></p>
          {targets?.fats && diary ? (
            <div className="mt-3 h-2 bg-rose-50 rounded-full overflow-hidden">
              <div className="h-full bg-rose-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalFats / targets.fats) * 100)}%` }} />
            </div>
          ) : null}
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-[1fr_380px] gap-6 items-start">
        {/* Lewa: Lista posiłków + wyszukiwarka */}
        <div className="space-y-6">
          {/* Posiłki dnia */}
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
            <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between">
              <h2 className="font-semibold text-slate-800">Posiłki</h2>
              <span className="text-sm text-slate-400">{diary?.items?.length || 0} pozycji</span>
            </div>
            <div className="divide-y divide-slate-100">
              {diary?.items.map(item => (
                <div key={item.id} className="flex justify-between items-center px-5 py-4 hover:bg-slate-50 transition-colors">
                  <div className="flex-1">
                    <div className="flex items-center gap-2">
                      <strong className="text-slate-800">{item.foodName}</strong>
                      <span className="text-xs font-semibold text-slate-500 bg-slate-100 px-2 py-0.5 rounded-full">{item.grams}g</span>
                    </div>
                    <div className="flex items-center gap-4 mt-1.5">
                      <span className="text-sm font-bold text-slate-700">{item.calories} kcal</span>
                      <div className="flex gap-3 text-xs font-medium">
                        <span className="text-blue-600">B: {item.macros?.protein}g</span>
                        <span className="text-amber-600">W: {item.macros?.carbs}g</span>
                        <span className="text-rose-600">T: {item.macros?.fats}g</span>
                      </div>
                    </div>
                  </div>
                  <Button variant="ghost" className="text-slate-300 hover:text-red-600 hover:bg-red-50 h-8 w-8 p-0 rounded-full shrink-0" onClick={() => deleteMeal(item.id)}>✖</Button>
                </div>
              ))}
              {!diary?.items?.length && (
                <div className="text-center py-16 text-slate-400">
                  <p className="text-4xl mb-3">🍽️</p>
                  <p className="font-medium">Brak posiłków</p>
                  <p className="text-sm mt-1">Wyszukaj produkt i dodaj go do dziennika</p>
                </div>
              )}
            </div>
          </div>

          {/* Wyszukiwarka */}
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
            <div className="p-5 border-b border-slate-100">
              <h3 className="font-semibold text-slate-800 mb-3">Wyszukaj produkt</h3>
              <Input value={search} onChange={(e: any) => setSearch(e.target.value)} placeholder="Wpisz nazwę produktu..." className="bg-slate-50 focus-visible:ring-blue-500 text-base py-5" />
            </div>
            {(results.length > 0 || search.length > 0) && (
              <div className="max-h-80 overflow-y-auto p-4 space-y-3 bg-slate-50/30">
                {results.map(f => (
                  <div key={f.id} className="p-4 border border-slate-200 rounded-xl hover:border-blue-300 hover:shadow-md transition-all bg-white flex items-center justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <strong className="text-slate-800">{f.name}</strong>
                      <span className="text-sm text-slate-400 ml-2">{f.caloriesPer100g} kcal/100g</span>
                      <div className="flex gap-2 mt-1.5 text-xs font-semibold">
                        <span className="text-blue-600">B: {f.macros?.protein || 0}</span>
                        <span className="text-amber-600">W: {f.macros?.carbs || 0}</span>
                        <span className="text-rose-600">T: {f.macros?.fats || 0}</span>
                      </div>
                    </div>
                    <div className="flex items-center gap-2 shrink-0">
                      <Input type="number" className="w-16 text-center bg-slate-50 h-9 text-sm" value={grams} onChange={(e: any) => setGrams(Number(e.target.value))} />
                      <span className="text-xs text-slate-400">g</span>
                      <Button size="sm" className="bg-blue-600 hover:bg-blue-700" onClick={() => addMeal(f.id)}>Dodaj</Button>
                    </div>
                  </div>
                ))}
                {results.length === 0 && search.length > 0 && (
                  <div className="text-center py-6 text-slate-400 text-sm">Brak wyników dla "{search}"</div>
                )}
              </div>
            )}
          </div>
        </div>

        {/* Prawa: Dodawanie produktów */}
        <div className="space-y-6">
          <ManualFoodForm onAdded={() => setSearch('')} />
          
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
            <div className="px-5 py-4 border-b border-slate-100">
              <h3 className="font-semibold text-slate-800">Skaner kodów</h3>
              <p className="text-xs text-slate-400 mt-0.5">Wyszukaj produkt po kodzie kreskowym</p>
            </div>
            <div className="p-5">
              <div className="flex gap-2">
                <Input className="bg-slate-50 focus-visible:ring-blue-500 flex-1" value={barcode} onChange={(e: any) => setBarcode(e.target.value)} placeholder="Kod kreskowy" /> 
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
      </div>
      <div className="pt-8 mt-8 border-t border-slate-200">
        <AiMealPlanGenerator 
          userId={user.userId} 
          date={selectedDate} // Podajemy wybraną w kalendarzu datę
          onAdded={loadDiary} // Po dodaniu, funkcja loadDiary odświeży widok
        />
      </div>
    </div>
  );
}