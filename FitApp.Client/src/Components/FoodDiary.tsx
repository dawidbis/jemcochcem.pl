import { useState, useEffect } from 'react';
import type { User, DiarySummary, Food, ExternalFood, TargetMicros } from '../types';
import { ManualFoodForm } from './ManualFoodForm';
import type { TargetMacros } from './TdeeCalculator';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { AiMealPlanGenerator } from './AiMealPlanGenerator';
import { WaterTracker } from './WaterTracker';
import { DietStreakWidget } from './DietStreakWidget';

export function FoodDiary({ user }: { user: User }) {
  const [diary, setDiary] = useState<DiarySummary | null>(null);
  const [search, setSearch] = useState('');
  const [results, setResults] = useState<Food[]>([]);
  const [barcode, setBarcode] = useState('');
  const [externalFood, setExternalFood] = useState<ExternalFood | null>(null);
  const [grams, setGrams] = useState(100);
  const [targets, setTargets] = useState<TargetMacros | null>(null);
  const [microTargets, setMicroTargets] = useState<TargetMicros | null>(null);
  const [selectedDate, setSelectedDate] = useState(() => new Date().toISOString().split('T')[0]);

  // Zaokrąglenie do 1 miejsca po przecinku dla czytelności
  const fmt = (n: number | undefined | null) => Math.round((n || 0) * 10) / 10;

  const isToday = selectedDate === new Date().toISOString().split('T')[0];

  const changeDate = (days: number) => {
    const d = new Date(selectedDate);
    d.setDate(d.getDate() + days);
    if (d <= new Date()) setSelectedDate(d.toISOString().split('T')[0]);
  };

  const formatDate = (dateStr: string) => {
  const d = new Date(dateStr);
  
  // Sprawdzamy szerokość okna (przydatne przy SSR/hydracji)
  const isMobile = typeof window !== 'undefined' && window.innerWidth < 640;

  if (isMobile) {
    // Widok mobilny: "Czw, 11 cze"
    return d.toLocaleDateString('pl-PL', { weekday: 'short', day: 'numeric', month: 'short' });
  }

  // Widok desktopowy (Twój stary format): "Czwartek, 11 czerwca 2026"
  return d.toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
};

  const loadDiary = async () => {
    const data = await api.loadDiary(selectedDate);
    if (data) setDiary(data);
    else setDiary({ date: selectedDate, items: [], totalCalories: 0, totalProtein: 0, totalCarbs: 0, totalFats: 0, totalFiber: 0, totalSugars: 0, totalSaturatedFat: 0, totalSodium: 0, totalCalcium: 0, totalIron: 0 });
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
    api.calculateMacros().then((data: any) => {
      if (data) setTargets({
        tdee: data.tdee || data.targetCalories,
        protein: data.protein,
        carbs: data.carbs,
        fats: data.fats || data.fat
      });
    });
    api.calculateMicros().then((data: TargetMicros | null) => {
      if (data) setMicroTargets(data);
    });
  }, [user.userId]);

  const fetchExternal = async () => {
    const data = await api.fetchExternalFood(barcode);
    if (data) setExternalFood(data);
  };

  const saveExternal = async () => {
    if (!externalFood) return;
    const payload = {
      name: externalFood.name,
      barcode,
      caloriesPer100g: externalFood.caloriesPer100g,
      proteinPer100g: externalFood.macros?.protein || 0,
      carbsPer100g: externalFood.macros?.carbs || 0,
      fatPer100g: externalFood.macros?.fats || 0,
      fiberPer100g: externalFood.micros?.fiber || 0,
      sugarsPer100g: externalFood.micros?.sugars || 0,
      saturatedFatPer100g: externalFood.micros?.saturatedFat || 0,
      sodiumPer100g: externalFood.micros?.sodium || 0,
      calciumPer100g: externalFood.micros?.calcium || 0,
      ironPer100g: externalFood.micros?.iron || 0,
    };
    await api.saveFood(payload);
    setExternalFood(null); setBarcode('');
  };

  const addMeal = async (foodId: string) => {
    await api.addMealItem({ date: new Date(selectedDate).toISOString(), foodProductId: foodId, grams });

    loadDiary(); setResults([]); setSearch('');
  };

  const deleteMeal = async (itemId: string) => {
await api.deleteMealItem(selectedDate, itemId);    loadDiary();
  };

  // Konfiguracja kafelków mikroskładników (klasy Tailwind muszą być literałami)
  const microTiles = [
    { label: 'Błonnik', unit: 'g', value: diary?.totalFiber, target: microTargets?.fiber, text: 'text-emerald-600', barBg: 'bg-emerald-50', barFill: 'bg-emerald-500' },
    { label: 'Cukry', unit: 'g', value: diary?.totalSugars, target: microTargets?.sugars, text: 'text-pink-600', barBg: 'bg-pink-50', barFill: 'bg-pink-500' },
    { label: 'Tł. nasycone', unit: 'g', value: diary?.totalSaturatedFat, target: microTargets?.saturatedFat, text: 'text-orange-600', barBg: 'bg-orange-50', barFill: 'bg-orange-500' },
    { label: 'Sód', unit: 'mg', value: diary?.totalSodium, target: microTargets?.sodium, text: 'text-violet-600', barBg: 'bg-violet-50', barFill: 'bg-violet-500' },
    { label: 'Wapń', unit: 'mg', value: diary?.totalCalcium, target: microTargets?.calcium, text: 'text-teal-600', barBg: 'bg-teal-50', barFill: 'bg-teal-500' },
    { label: 'Żelazo', unit: 'mg', value: diary?.totalIron, target: microTargets?.iron, text: 'text-red-700', barBg: 'bg-red-50', barFill: 'bg-red-500' },
  ];

  return (
  <div className="space-y-6 w-full max-w-7xl mx-auto">
    
    {/* Header z datą - w pełni responsywny, wyśrodkowany i bez nakładania tekstu */}
    <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between w-full">
      <div className="text-center md:text-left">
        <h1 className="text-2xl font-bold text-slate-900">Dziennik posiłków</h1>
        <p className="text-slate-500 text-sm mt-0.5">Loguj posiłki i śledź dzienne cele</p>
      </div>
      
      {/* Kontener widżetów rozciąga się na smartfonie, gwarantując widoczność strzałek */}
      <div className="flex items-center justify-between md:justify-end gap-3 bg-white border border-slate-200 rounded-xl px-4 py-2.5 shadow-sm w-full md:w-auto overflow-hidden">
        <div className="flex items-center gap-2 shrink-0">
          <DietStreakWidget userId={user.userId} />
        </div>
        
        <div className="flex items-center gap-1.5 w-full justify-end md:w-auto">
          <button 
            onClick={() => changeDate(-1)} 
            className="p-2 rounded-md hover:bg-slate-100 transition-colors text-slate-600 font-bold text-lg shrink-0"
          >
            ←
          </button>
          
          {/* Centralny punkt z datą - używa flex-col, by "Dzisiaj" wskakiwało POD spód */}
          <div className="flex flex-col items-center justify-center min-w-[130px] xs:min-w-[160px] px-1 min-h-[40px]">
            <div className="text-xs xs:text-sm font-semibold text-slate-700 capitalize text-center block leading-tight">
              {formatDate(selectedDate)}
            </div>
            {isToday && (
              <span className="text-[9px] uppercase tracking-wider text-emerald-600 font-bold leading-none mt-1 block">
                Dzisiaj
              </span>
            )}
          </div>
          
          <button 
            onClick={() => changeDate(1)} 
            disabled={isToday} 
            className={`p-2 rounded-md transition-colors font-bold text-lg shrink-0 ${
              isToday ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100'
            }`}
          >
            →
          </button>
        </div>
      </div>
    </div>

    {/* Podsumowanie kalorii i makroskładników na osi czasu */}
    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
      {/* Kafelek: Kalorie */}
      <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
        <span className="text-xs text-slate-500 font-medium uppercase tracking-wider">Kalorie</span>
        <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalCalories || 0} <span className="text-base font-normal text-slate-400">/ {targets?.tdee || '-'}</span></p>
        {targets?.tdee && diary ? (
          <div className="mt-3 h-2 bg-slate-100 rounded-full overflow-hidden">
            <div className="h-full bg-blue-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalCalories / targets.tdee) * 100)}%` }} />
          </div>
        ) : null}
      </div>

      {/* Kafelek: Białko */}
      <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
        <span className="text-xs text-blue-600 font-medium uppercase tracking-wider">Białko</span>
        <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalProtein || 0}<span className="text-base font-normal text-slate-400">g / {targets?.protein || '-'}g</span></p>
        {targets?.protein && diary ? (
          <div className="mt-3 h-2 bg-blue-50 rounded-full overflow-hidden">
            <div className="h-full bg-blue-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalProtein / targets.protein) * 100)}%` }} />
          </div>
        ) : null}
      </div>

      {/* Kafelek: Węglowodany */}
      <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
        <span className="text-xs text-amber-600 font-medium uppercase tracking-wider">Węglowodany</span>
        <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalCarbs || 0}<span className="text-base font-normal text-slate-400">g / {targets?.carbs || '-'}g</span></p>
        {targets?.carbs && diary ? (
          <div className="mt-3 h-2 bg-amber-50 rounded-full overflow-hidden">
            <div className="h-full bg-amber-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalCarbs / targets.carbs) * 100)}%` }} />
          </div>
        ) : null}
      </div>

      {/* Kafelek: Tłuszcz */}
      <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
        <span className="text-xs text-rose-600 font-medium uppercase tracking-wider">Tłuszcz</span>
        <p className="text-2xl font-bold text-slate-900 mt-1">{diary?.totalFats || 0}<span className="text-base font-normal text-slate-400">g / {targets?.fats || '-'}g</span></p>
        {targets?.fats && diary ? (
          <div className="mt-3 h-2 bg-rose-50 rounded-full overflow-hidden">
            <div className="h-full bg-rose-500 rounded-full transition-all duration-500" style={{ width: `${Math.min(100, (diary.totalFats / targets.fats) * 100)}%` }} />
          </div>
        ) : null}
      </div>

      <WaterTracker
        key={`${selectedDate}-${diary?.totalProtein}`}
        userId={user.userId}
        date={selectedDate}
      />
    </div>

    {/* Podsumowanie mikroskładników */}
    <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
      <div className="px-5 py-4 border-b border-slate-100">
        <h2 className="font-semibold text-slate-800">Mikroskładniki</h2>
        <p className="text-xs text-slate-400 mt-0.5">Najważniejsze składniki odżywcze dla zdrowia</p>
      </div>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4 p-5">
        {microTiles.map(tile => {
          const value = fmt(tile.value);
          const pct = tile.target ? Math.min(100, (value / tile.target) * 100) : 0;
          return (
            <div key={tile.label} className="bg-slate-50/50 rounded-xl border border-slate-100 p-4">
              <span className={`text-xs font-medium uppercase tracking-wider ${tile.text}`}>{tile.label}</span>
              <p className="text-xl font-bold text-slate-900 mt-1">
                {value}<span className="text-sm font-normal text-slate-400">{tile.unit} / {tile.target ? `${fmt(tile.target)}${tile.unit}` : '-'}</span>
              </p>
              {tile.target ? (
                <div className={`mt-3 h-2 rounded-full overflow-hidden ${tile.barBg}`}>
                  <div className={`h-full rounded-full transition-all duration-500 ${tile.barFill}`} style={{ width: `${pct}%` }} />
                </div>
              ) : null}
            </div>
          );
        })}
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
                  {item.micros && (
                    <div className="flex flex-wrap gap-x-3 gap-y-1 mt-1 text-[11px] font-medium text-slate-500">
                      <span className="text-emerald-600">Błonnik: {fmt(item.micros.fiber)}g</span>
                      <span className="text-pink-600">Cukry: {fmt(item.micros.sugars)}g</span>
                      <span className="text-orange-600">Nasyc.: {fmt(item.micros.saturatedFat)}g</span>
                      <span className="text-violet-600">Sód: {fmt(item.micros.sodium)}mg</span>
                      <span className="text-teal-600">Wapń: {fmt(item.micros.calcium)}mg</span>
                      <span className="text-red-700">Żelazo: {fmt(item.micros.iron)}mg</span>
                    </div>
                  )}
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
                    {f.micros && (f.micros.fiber || f.micros.sugars || f.micros.saturatedFat || f.micros.sodium || f.micros.calcium || f.micros.iron) ? (
                      <div className="flex flex-wrap gap-x-2 gap-y-0.5 mt-1 text-[10px] font-medium text-slate-400">
                        <span>Błonnik {fmt(f.micros.fiber)}g</span>
                        <span>Cukry {fmt(f.micros.sugars)}g</span>
                        <span>Nasyc. {fmt(f.micros.saturatedFat)}g</span>
                        <span>Sód {fmt(f.micros.sodium)}mg</span>
                        <span>Wapń {fmt(f.micros.calcium)}mg</span>
                        <span>Żelazo {fmt(f.micros.iron)}mg</span>
                      </div>
                    ) : null}
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
                <div className="flex gap-3 text-xs font-semibold">
                  <span className="text-blue-600">B: {fmt(externalFood.macros?.protein)}g</span>
                  <span className="text-amber-600">W: {fmt(externalFood.macros?.carbs)}g</span>
                  <span className="text-rose-600">T: {fmt(externalFood.macros?.fats)}g</span>
                </div>
                {externalFood.micros && (
                  <div className="flex flex-wrap gap-x-2 gap-y-0.5 text-[10px] font-medium text-slate-500">
                    <span>Błonnik {fmt(externalFood.micros.fiber)}g</span>
                    <span>Cukry {fmt(externalFood.micros.sugars)}g</span>
                    <span>Nasyc. {fmt(externalFood.micros.saturatedFat)}g</span>
                    <span>Sód {fmt(externalFood.micros.sodium)}mg</span>
                    <span>Wapń {fmt(externalFood.micros.calcium)}mg</span>
                    <span>Żelazo {fmt(externalFood.micros.iron)}mg</span>
                  </div>
                )}
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
        date={selectedDate}
        onAdded={loadDiary}
      />
    </div>
  </div>
);
}