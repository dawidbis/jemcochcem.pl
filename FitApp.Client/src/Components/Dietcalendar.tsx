import { useState, useEffect } from 'react';
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip,
  ResponsiveContainer, LineChart, Line, Legend,
} from 'recharts';
// Dodaliśmy ikonę Droplet dla reprezentacji wody
import { CalendarDays, Flame, TrendingUp, Droplet } from 'lucide-react';
import type { User, DiarySummary, WaterStatusDto } from '../types';
import { api } from '../api';

interface Props { user: User; }

const MONTH_NAMES = ['Styczeń','Luty','Marzec','Kwiecień','Maj','Czerwiec','Lipiec','Sierpień','Wrzesień','Październik','Listopad','Grudzień'];
const DAY_NAMES = ['Pn','Wt','Śr','Cz','Pt','Sb','Nd'];

export function DietCalendar({ user }: Props) {
  const today = new Date();
  const todayStr = today.toISOString().split('T')[0];
  const [viewMonth, setViewMonth] = useState(today.getMonth());
  const [viewYear, setViewYear] = useState(today.getFullYear());
  const [selectedDate, setSelectedDate] = useState<string | null>(null);
  const [selectedDiary, setSelectedDiary] = useState<DiarySummary | null>(null);
  const [selectedWater, setSelectedWater] = useState<WaterStatusDto | null>(null);
  
  type DaySummary = { totalCalories: number; totalProtein: number; totalCarbs: number; totalFats: number; waterMl: number };
  const [monthData, setMonthData] = useState<Record<string, DaySummary>>({});
  const [loading, setLoading] = useState(false);

  const daysInMonth = new Date(viewYear, viewMonth + 1, 0).getDate();
  const firstDayOfWeek = (new Date(viewYear, viewMonth, 1).getDay() + 6) % 7;

  const prevMonth = () => {
    if (viewMonth === 0) { setViewMonth(11); setViewYear(viewYear - 1); }
    else setViewMonth(viewMonth - 1);
  };

  const nextMonth = () => {
    if (viewYear > today.getFullYear() || (viewYear === today.getFullYear() && viewMonth >= today.getMonth())) return;
    if (viewMonth === 11) { setViewMonth(0); setViewYear(viewYear + 1); }
    else setViewMonth(viewMonth + 1);
  };

  const isNextDisabled = viewYear > today.getFullYear() || (viewYear === today.getFullYear() && viewMonth >= today.getMonth());

  useEffect(() => {
    setMonthData({});
    setLoading(true);
    api.getMonthlyCalendar(viewYear, viewMonth + 1)
      .then(data => setMonthData(data))
      .finally(() => setLoading(false));
  }, [viewMonth, viewYear, user.userId]);

  // Ładowanie szczegółów wybranego dnia (Jedzenie + Woda)
  useEffect(() => {
    if (!selectedDate) { 
      setSelectedDiary(null); 
      setSelectedWater(null); 
      return; 
    }
   api.loadDiary(selectedDate).then((d: any) => setSelectedDiary(d));
api.getWaterStatus(selectedDate).then((w: any) => setSelectedWater(w));  }, [selectedDate]);

  const selectDay = (day: number) => {
    const d = `${viewYear}-${String(viewMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
    if (d <= todayStr) setSelectedDate(d === selectedDate ? null : d);
  };

  // Przygotowanie danych do 3 wykresów analitycznych
  const chartData = Array.from({ length: daysInMonth }, (_, i) => {
    const day = i + 1;
    const dateStr = `${viewYear}-${String(viewMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
    const entry = monthData[dateStr];
    return {
      day: String(day),
      kcal: entry?.totalCalories || 0,
      białko: entry?.totalProtein || 0,
      węgle: entry?.totalCarbs || 0,
      tłuszcz: entry?.totalFats || 0,
      woda: entry?.waterMl || 0,
    };
  });

  const totalMonthKcal = Object.values(monthData).reduce((s, d) => s + (d.totalCalories || 0), 0);
  const daysWithData = Object.keys(monthData).length;
  const avgKcal = daysWithData > 0 ? Math.round(totalMonthKcal / daysWithData) : 0;

  const totalMonthWater = Object.values(monthData).reduce((s, d) => s + (d.waterMl || 0), 0);
  const avgWater = daysWithData > 0 ? Math.round(totalMonthWater / daysWithData) : 0;

  const getCalorieColor = (kcal: number) => {
    if (kcal === 0) return '';
    if (kcal < 1200) return 'bg-amber-100 text-amber-800';
    if (kcal < 2000) return 'bg-emerald-100 text-emerald-800';
    if (kcal < 2800) return 'bg-blue-100 text-blue-800';
    return 'bg-rose-100 text-rose-800';
  };

  return (
    <div className="space-y-6 max-w-6xl mx-auto">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-slate-900 flex items-center gap-2">
          <CalendarDays className="h-6 w-6 text-blue-600" />
          Kalendarz diety
        </h1>
        <p className="text-slate-500 text-sm mt-0.5">Przeglądaj historię posiłków i analizuj trendy żywieniowe</p>
      </div>

      {/* Zaktualizowana siatka kart - grid-cols-4 dla pomieszczenia wody */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm text-slate-500 font-medium">Suma kcal w miesiącu</span>
            <div className="p-2 bg-orange-50 rounded-lg text-orange-600"><Flame className="h-4 w-4" /></div>
          </div>
          <p className="text-2xl font-bold text-slate-900">{totalMonthKcal.toLocaleString('pl-PL')}</p>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm text-slate-500 font-medium">Średnia dzienna</span>
            <div className="p-2 bg-blue-50 rounded-lg text-blue-600"><TrendingUp className="h-4 w-4" /></div>
          </div>
          <p className="text-2xl font-bold text-slate-900">{avgKcal} kcal</p>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm text-slate-500 font-medium">Dni z wpisami</span>
            <div className="p-2 bg-emerald-50 rounded-lg text-emerald-600"><CalendarDays className="h-4 w-4" /></div>
          </div>
          <p className="text-2xl font-bold text-slate-900">{daysWithData} / {daysInMonth}</p>
        </div>
        
        {/* NOWY KAFELEK: ŚREDNIE NAWODNIENIE */}
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm text-slate-500 font-medium">Średnie nawodnienie</span>
            <div className="p-2 bg-blue-50 rounded-lg text-blue-500"><Droplet className="h-4 w-4 animate-pulse" /></div>
          </div>
          <p className="text-2xl font-bold text-slate-900">{avgWater} <span className="text-sm font-normal text-slate-400">ml/dzień</span></p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-[340px_1fr] gap-6">
        {/* Calendar */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5">
          <div className="flex items-center justify-between mb-4">
            <button onClick={prevMonth} className="p-2 rounded-lg hover:bg-slate-100 transition-colors text-slate-600 font-bold text-lg">←</button>
            <span className="text-base font-semibold text-slate-700">{MONTH_NAMES[viewMonth]} {viewYear}</span>
            <button onClick={nextMonth} disabled={isNextDisabled} className={`p-2 rounded-lg transition-colors text-lg font-bold ${isNextDisabled ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100'}`}>→</button>
          </div>

          <div className="grid grid-cols-7 gap-1 mb-2">
            {DAY_NAMES.map(d => (
              <div key={d} className="text-center text-[11px] font-semibold text-slate-400 uppercase py-1">{d}</div>
            ))}
          </div>

          <div className="grid grid-cols-7 gap-1">
            {Array.from({ length: firstDayOfWeek }).map((_, i) => (
              <div key={`empty-${i}`} />
            ))}
            {Array.from({ length: daysInMonth }, (_, i) => {
              const day = i + 1;
              const dateStr = `${viewYear}-${String(viewMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
              const isFuture = dateStr > todayStr;
              const isSelected = dateStr === selectedDate;
              const isToday = dateStr === todayStr;
              const entry = monthData[dateStr];
              const kcal = entry?.totalCalories || 0;
              const waterMl = entry?.waterMl || 0;
              const colorClass = getCalorieColor(kcal);

              return (
                <button
                  key={day}
                  onClick={() => !isFuture && selectDay(day)}
                  disabled={isFuture}
                  className={`
                    relative aspect-square rounded-lg text-sm font-medium transition-all flex flex-col items-center justify-center gap-0.5
                    ${isFuture ? 'text-slate-200 cursor-not-allowed' : 'hover:ring-2 hover:ring-blue-300 cursor-pointer'}
                    ${isSelected ? 'ring-2 ring-blue-500 bg-blue-50' : ''}
                    ${isToday && !isSelected ? 'ring-2 ring-slate-300' : ''}
                    ${!isSelected && !isFuture && colorClass ? colorClass : ''}
                    ${!isSelected && !isFuture && !colorClass ? 'text-slate-600 hover:bg-slate-50' : ''}
                  `}
                >
                  <span className="text-xs">{day}</span>
                  {kcal > 0 && <span className="text-[9px] font-semibold leading-none">{kcal}</span>}
                  {/* MAŁA IKONKA KROPELKI JEŚLI ZALOGOWANO WODĘ TEGO DNIA */}
                  {waterMl > 0 && (
                    <span className="text-[8px] font-bold text-blue-600 flex items-center gap-px mt-0.5 leading-none">
                      💧{waterMl}
                    </span>
                  )}
                </button>
              );
            })}
          </div>

          {loading && <p className="text-center text-xs text-slate-400 mt-3">Ładowanie danych...</p>}
        </div>

        {/* Right side: selected day detail or charts */}
        <div className="space-y-6">
          {/* Selected day detail */}
          {selectedDate && (selectedDiary || selectedWater) && (
            <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">

              {/* Nagłówek z datą */}
              <div className="px-5 pt-5 pb-4 border-b border-slate-100">
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-xs font-semibold uppercase tracking-wider text-slate-400 mb-0.5">Wybrany dzień</p>
                    <h3 className="text-lg font-bold text-slate-900 capitalize">
                      {new Date(selectedDate + 'T12:00:00').toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })}
                    </h3>
                  </div>
                  <button
                    onClick={() => setSelectedDate(null)}
                    className="text-slate-400 hover:text-slate-600 text-lg leading-none p-1 rounded-lg hover:bg-slate-100 transition-colors"
                  >✕</button>
                </div>
              </div>

              {/* Kafelki makro */}
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 p-4 border-b border-slate-100">
                <div className="bg-orange-50 rounded-xl p-3 text-center">
                  <p className="text-[10px] font-semibold uppercase tracking-wider text-orange-500 mb-1">Kalorie</p>
                  <p className="text-xl font-bold text-slate-900">{selectedDiary?.totalCalories || 0}</p>
                  <p className="text-[10px] text-slate-400">kcal</p>
                </div>
                <div className="bg-blue-50 rounded-xl p-3 text-center">
                  <p className="text-[10px] font-semibold uppercase tracking-wider text-blue-500 mb-1">Białko</p>
                  <p className="text-xl font-bold text-slate-900">{Math.round(selectedDiary?.totalProtein || 0)}</p>
                  <p className="text-[10px] text-slate-400">g</p>
                </div>
                <div className="bg-amber-50 rounded-xl p-3 text-center">
                  <p className="text-[10px] font-semibold uppercase tracking-wider text-amber-500 mb-1">Węgle</p>
                  <p className="text-xl font-bold text-slate-900">{Math.round(selectedDiary?.totalCarbs || 0)}</p>
                  <p className="text-[10px] text-slate-400">g</p>
                </div>
                <div className="bg-rose-50 rounded-xl p-3 text-center">
                  <p className="text-[10px] font-semibold uppercase tracking-wider text-rose-500 mb-1">Tłuszcz</p>
                  <p className="text-xl font-bold text-slate-900">{Math.round(selectedDiary?.totalFats || 0)}</p>
                  <p className="text-[10px] text-slate-400">g</p>
                </div>
              </div>

              {/* Woda + mikro */}
              <div className="px-4 py-3 border-b border-slate-100 flex flex-wrap gap-x-5 gap-y-1.5">
                <span className="flex items-center gap-1 text-xs text-blue-600 font-medium">
                  💧 <strong>{selectedWater?.currentAmountMl || 0} ml</strong> wody
                </span>
                {[
                  { label: 'Błonnik', val: selectedDiary?.totalFiber, unit: 'g' },
                  { label: 'Cukry', val: selectedDiary?.totalSugars, unit: 'g' },
                  { label: 'Tł. nasyc.', val: selectedDiary?.totalSaturatedFat, unit: 'g' },
                  { label: 'Sód', val: selectedDiary?.totalSodium, unit: 'mg' },
                  { label: 'Wapń', val: selectedDiary?.totalCalcium, unit: 'mg' },
                  { label: 'Żelazo', val: selectedDiary?.totalIron, unit: 'mg' },
                ].map(({ label, val, unit }) => (
                  <span key={label} className="text-xs text-slate-500">
                    {label}: <strong className="text-slate-700">{Math.round((val || 0) * 10) / 10}{unit}</strong>
                  </span>
                ))}
              </div>

              {/* Lista posiłków */}
              <div className="divide-y divide-slate-50 max-h-64 overflow-y-auto">
                {selectedDiary && selectedDiary.items.length > 0 ? selectedDiary.items.map(item => (
                  <div key={item.id} className="px-4 py-2.5 flex justify-between items-center hover:bg-slate-50 transition-colors">
                    <div>
                      <p className="text-sm font-semibold text-slate-800">{item.foodName}</p>
                      <p className="text-xs text-slate-400">{item.grams}g &middot; B:{item.macros?.protein}g W:{item.macros?.carbs}g T:{item.macros?.fats}g</p>
                    </div>
                    <span className="text-sm font-bold text-slate-700 ml-4 shrink-0">{item.calories} kcal</span>
                  </div>
                )) : (
                  <div className="py-8 text-center text-slate-400 text-sm">Brak posiłków tego dnia</div>
                )}
              </div>
            </div>
          )}

          {/* Kcal bar chart */}
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5">
            <h3 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
              <Flame className="h-4 w-4 text-orange-500" />
              Kalorie dziennie — {MONTH_NAMES[viewMonth]} {viewYear}
            </h3>
            <ResponsiveContainer width="100%" height={200}>
              <BarChart data={chartData} margin={{ top: 5, right: 5, left: -15, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="day" tick={{ fontSize: 10, fill: '#94a3b8' }} interval={1} />
                <YAxis tick={{ fontSize: 10, fill: '#94a3b8' }} />
                <Tooltip
                  contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 12 }}
                  formatter={(v: any) => [`${v} kcal`]}
                />
                <Bar dataKey="kcal" fill="#f97316" radius={[3, 3, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* NOWY TRZECI WYKRES: NAWODNIENIE TRANSAKCYJNE (SŁUPKOWY) */}
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5">
            <h3 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
              <Droplet className="h-4 w-4 text-blue-500" />
              Spożycie wody dziennie — {MONTH_NAMES[viewMonth]} {viewYear}
            </h3>
            <ResponsiveContainer width="100%" height={200}>
              <BarChart data={chartData} margin={{ top: 5, right: 5, left: -15, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="day" tick={{ fontSize: 10, fill: '#94a3b8' }} interval={1} />
                <YAxis tick={{ fontSize: 10, fill: '#94a3b8' }} tickFormatter={(v: any) => `${v}ml`} />
                <Tooltip
                  contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 12 }}
                  formatter={(v: any) => [`${v} ml`]}
                />
                <Bar dataKey="woda" fill="#3b82f6" radius={[3, 3, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* Macros line chart */}
          <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5">
            <h3 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
              <TrendingUp className="h-4 w-4 text-blue-500" />
              Makroskładniki — {MONTH_NAMES[viewMonth]} {viewYear}
            </h3>
            <ResponsiveContainer width="100%" height={220}>
              <LineChart data={chartData} margin={{ top: 5, right: 5, left: -15, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="day" tick={{ fontSize: 10, fill: '#94a3b8' }} interval={1} />
                <YAxis tick={{ fontSize: 10, fill: '#94a3b8' }} tickFormatter={(v: any) => `${v}g`} />
                <Tooltip
                  contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 12 }}
                  formatter={(v: any) => [`${v} g`]}
                />
                <Legend wrapperStyle={{ fontSize: 12 }} />
                <Line type="monotone" dataKey="białko" stroke="#3b82f6" strokeWidth={2} dot={false} connectNulls />
                <Line type="monotone" dataKey="węgle" stroke="#f59e0b" strokeWidth={2} dot={false} connectNulls />
                <Line type="monotone" dataKey="tłuszcz" stroke="#ef4444" strokeWidth={2} dot={false} connectNulls />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>
    </div>
  );
}