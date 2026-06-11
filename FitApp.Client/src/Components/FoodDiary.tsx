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
  const [isSearchOpen, setIsSearchOpen] = useState(false);
  const [isManualOpen, setIsManualOpen] = useState(false);
  const [isScannerOpen, setIsScannerOpen] = useState(false);

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
  <div className="space-y-6 w-full max-w-7xl mx-auto px-4 xs:px-6">
    
    {/* Header z datą */}
    <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between w-full">
      <div className="text-center md:text-left">
        <h1 className="text-2xl font-bold text-slate-900">Dziennik posiłków</h1>
        <p className="text-slate-500 text-sm mt-0.5">Loguj posiłki i śledź dzienne cele</p>
      </div>
      
      <div className="flex items-center justify-between md:justify-end gap-3 bg-white border border-slate-200 rounded-xl px-4 py-2.5 shadow-sm w-full md:w-auto overflow-hidden">
        <div className="flex items-center gap-2 shrink-0">
          <DietStreakWidget userId={user.userId} />
        </div>
        
        <div className="flex items-center gap-1.5 w-full justify-end md:w-auto">
          <button onClick={() => changeDate(-1)} className="p-2 rounded-md hover:bg-slate-100 transition-colors text-slate-600 font-bold text-lg shrink-0">←</button>
          <div className="flex flex-col items-center justify-center min-w-[130px] xs:min-w-[160px] px-1 min-h-[40px]">
            <div className="text-xs xs:text-sm font-semibold text-slate-700 capitalize text-center block leading-tight">{formatDate(selectedDate)}</div>
            {isToday && <span className="text-[9px] uppercase tracking-wider text-emerald-600 font-bold leading-none mt-1 block">Dzisiaj</span>}
          </div>
          <button onClick={() => changeDate(1)} disabled={isToday} className={`p-2 rounded-md transition-colors font-bold text-lg shrink-0 ${isToday ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100'}`}>→</button>
        </div>
      </div>
    </div>

    {/* Kafelki makroskładników */}
    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
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

      <WaterTracker key={`${selectedDate}-${diary?.totalProtein}`} userId={user.userId} date={selectedDate} />
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

    {/* SEKCJA POSIŁKÓW Z NOWYM RESPONSIVNYM PASKIEM NARZĘDZIOWYM */}
    <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
      <div className="px-5 py-4 border-b border-slate-100 flex flex-col sm:flex-row sm:items-center justify-between gap-4 bg-slate-50/50">
        <div>
          <h2 className="font-semibold text-slate-800">Posiłki dnia</h2>
          <p className="text-xs text-slate-400 mt-0.5">{diary?.items?.length || 0} zalogowanych pozycji</p>
        </div>
        
        {/* Przyciski akcji: Szukaj, Skanuj, Dodaj ręcznie */}
        <div className="flex flex-wrap items-center gap-2 w-full sm:w-auto">
          <Button onClick={() => setIsSearchOpen(true)} className="bg-blue-600 hover:bg-blue-700 text-xs py-1.5 px-3 flex-1 sm:flex-none gap-1.5 h-9">
            🔍 Szukaj produktu
          </Button>
          <Button onClick={() => setIsScannerOpen(true)} variant="outline" className="border-slate-200 hover:bg-slate-100 text-xs py-1.5 px-3 flex-1 sm:flex-none gap-1.5 h-9 text-slate-700">
            📷 <span>Skaner kodów</span>
          </Button>
          <Button onClick={() => setIsManualOpen(true)} variant="outline" className="border-slate-200 hover:bg-slate-100 text-xs py-1.5 px-3 flex-1 sm:flex-none gap-1.5 h-9 text-slate-700">
            ➕ <span>Wpisz ręcznie</span>
          </Button>
        </div>
      </div>

      {/* Lista posiłków */}
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
            <p className="font-medium">Twój dziennik jest czysty</p>
            <p className="text-sm mt-1">Użyj przycisków powyżej, aby dodać dzisiejsze jedzenie</p>
          </div>
        )}
      </div>
    </div>

    {/* Generator planów dietetycznych AI */}
    <div className="pt-8 mt-8 border-t border-slate-200">
      <AiMealPlanGenerator date={selectedDate} onAdded={loadDiary} />
    </div>

    {/* ==================== OKNO MODALNE 1: WYSZUKIWARKA PRODUKTÓW ==================== */}
    {isSearchOpen && (
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm animate-fade-in">
        <div className="bg-white rounded-2xl border border-slate-200 shadow-2xl w-full max-w-2xl overflow-hidden max-h-[90vh] flex flex-col">
          <div className="p-5 border-b border-slate-100 flex items-center justify-between bg-slate-50">
            <div>
              <h3 className="font-bold text-slate-800 text-lg">Wyszukaj produkt w bazie</h3>
              <p className="text-xs text-slate-400 mt-0.5">Wpisz nazwę, aby przefiltrować posiłki</p>
            </div>
            <button onClick={() => { setIsSearchOpen(false); setSearch(''); setResults([]); }} className="text-slate-400 hover:text-slate-600 text-xl font-bold p-1">✕</button>
          </div>
          <div className="p-5 border-b border-slate-100">
            <Input value={search} onChange={(e: any) => setSearch(e.target.value)} placeholder="Wpisz nazwę produktu..." className="bg-slate-50 focus-visible:ring-blue-500 text-base py-5" />
          </div>
          <div className="overflow-y-auto p-4 space-y-3 flex-1 min-h-[200px] bg-slate-50/20">
            {results.map(f => (
              <div key={f.id} className="p-4 border border-slate-200 rounded-xl hover:border-blue-300 hover:shadow-md transition-all bg-white flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div className="flex-1 min-w-0">
                  <strong className="text-slate-800">{f.name}</strong>
                  <span className="text-sm text-slate-400 ml-2">{f.caloriesPer100g} kcal/100g</span>
                  <div className="flex gap-2 mt-1.5 text-xs font-semibold">
                    <span className="text-blue-600">B: {f.macros?.protein || 0}</span>
                    <span className="text-amber-600">W: {f.macros?.carbs || 0}</span>
                    <span className="text-rose-600">T: {f.macros?.fats || 0}</span>
                  </div>
                </div>
                <div className="flex items-center gap-2 shrink-0 justify-end border-t sm:border-t-0 pt-2 sm:pt-0">
                  <Input type="number" className="w-16 text-center bg-slate-50 h-9 text-sm" value={grams} onChange={(e: any) => setGrams(Number(e.target.value))} />
                  <span className="text-xs text-slate-400mr-1">g</span>
                  <Button size="sm" className="bg-blue-600 hover:bg-blue-700" onClick={() => { addMeal(f.id); setIsSearchOpen(false); }}>Dodaj</Button>
                </div>
              </div>
            ))}
            {results.length === 0 && search.length > 0 && (
              <div className="text-center py-12 text-slate-400 text-sm">Brak wyników dla "{search}"</div>
            )}
            {search.length === 0 && (
              <div className="text-center py-12 text-slate-400 text-sm">Zacznij pisać, aby wyszukać żywność...</div>
            )}
          </div>
        </div>
      </div>
    )}

    {/* ==================== OKNO MODALNE 2: SKANER KODÓW KRESKOWYCH ==================== */}
    {isScannerOpen && (
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm">
        <div className="bg-white rounded-2xl border border-slate-200 shadow-2xl w-full max-w-md overflow-hidden">
          <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between bg-slate-50">
            <div>
              <h3 className="font-bold text-slate-800">Skaner kodów EAN</h3>
              <p className="text-xs text-slate-400 mt-0.5">Wyszukaj produkt po kodzie kreskowym</p>
            </div>
            <button onClick={() => { setIsScannerOpen(false); setExternalFood(null); setBarcode(''); }} className="text-slate-400 hover:text-slate-600 text-xl font-bold p-1">✕</button>
          </div>
          <div className="p-5 space-y-4">
            <div className="flex gap-2">
              <Input className="bg-slate-50 focus-visible:ring-blue-500 flex-1" value={barcode} onChange={(e: any) => setBarcode(e.target.value)} placeholder="Wpisz kod kreskowy produktu..." /> 
              <Button className="bg-blue-600 hover:bg-blue-700" onClick={fetchExternal}>Szukaj</Button>
            </div>
            {externalFood && (
              <div className="p-4 border border-blue-100 rounded-xl bg-blue-50 flex flex-col gap-3 animate-fade-in">
                <strong className="text-blue-900">{externalFood.name}</strong>
                <div className="flex gap-3 text-xs font-semibold">
                  <span className="text-blue-600">B: {fmt(externalFood.macros?.protein)}g</span>
                  <span className="text-amber-600">W: {fmt(externalFood.macros?.carbs)}g</span>
                  <span className="text-rose-600">T: {fmt(externalFood.macros?.fats)}g</span>
                </div>
                <div className="flex justify-between items-center text-sm font-medium pt-2 border-t border-blue-100">
                  <span>{externalFood.caloriesPer100g} kcal/100g</span>
                  <Button size="sm" className="bg-blue-600 hover:bg-blue-700" onClick={() => { saveExternal(); setIsScannerOpen(false); }}>Zapisz w bazie</Button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    )}

    {/* ==================== OKNO MODALNE 3: DODAWANIE RĘCZNE ==================== */}
    {isManualOpen && (
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm">
        <div className="bg-white rounded-2xl border border-slate-200 shadow-2xl w-full max-w-lg overflow-hidden max-h-[95vh] flex flex-col">
          <div className="px-5 py-4 border-b border-slate-100 flex items-center justify-between bg-slate-50">
            <div>
              <h3 className="font-bold text-slate-800">Nowy produkt własny</h3>
              <p className="text-xs text-slate-400 mt-0.5">Wprowadź wartości odżywcze makro i mikro</p>
            </div>
            <button onClick={() => setIsManualOpen(false)} className="text-slate-400 hover:text-slate-600 text-xl font-bold p-1">✕</button>
          </div>
          <div className="p-5 overflow-y-auto flex-1">
            {/* Przekazujemy zamknięcie modala po udanym dodaniu */}
            <ManualFoodForm onAdded={() => { setSearch(''); setIsManualOpen(false); loadDiary(); }} />
          </div>
        </div>
      </div>
    )}

  </div>
);
}