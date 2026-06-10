import { useState, useEffect } from 'react';
import { CalendarDays, Dumbbell, ChevronLeft, ChevronRight } from 'lucide-react';
import { api } from '../api';
import type { WorkoutSession } from '../types';

const MONTH_NAMES = ['Styczeń','Luty','Marzec','Kwiecień','Maj','Czerwiec','Lipiec','Sierpień','Wrzesień','Październik','Listopad','Grudzień'];
const DAY_NAMES = ['Pn','Wt','Śr','Cz','Pt','Sb','Nd'];

export function WorkoutCalendar() {
  const today = new Date();
  const todayStr = today.toISOString().split('T')[0];

  const [viewMonth, setViewMonth] = useState(today.getMonth());
  const [viewYear, setViewYear] = useState(today.getFullYear());
  const [sessions, setSessions] = useState<WorkoutSession[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedDate, setSelectedDate] = useState<string | null>(null);

  const daysInMonth = new Date(viewYear, viewMonth + 1, 0).getDate();
  const firstDayOfWeek = (new Date(viewYear, viewMonth, 1).getDay() + 6) % 7;

  useEffect(() => {
    const from = `${viewYear}-${String(viewMonth + 1).padStart(2, '0')}-01`;
    const to = `${viewYear}-${String(viewMonth + 1).padStart(2, '0')}-${String(daysInMonth).padStart(2, '0')}`;
    setLoading(true);
    api.getWorkoutHistoryRange(from, to).then(data => {
      setSessions(data);
      setLoading(false);
    });
  }, [viewMonth, viewYear]);

  const sessionsByDate = sessions.reduce<Record<string, WorkoutSession[]>>((acc, s) => {
    const d = s.date.split('T')[0];
    (acc[d] ??= []).push(s);
    return acc;
  }, {});

  const prevMonth = () => {
    if (viewMonth === 0) { setViewMonth(11); setViewYear(y => y - 1); }
    else setViewMonth(m => m - 1);
    setSelectedDate(null);
  };
  const nextMonth = () => {
    if (viewYear > today.getFullYear() || (viewYear === today.getFullYear() && viewMonth >= today.getMonth())) return;
    if (viewMonth === 11) { setViewMonth(0); setViewYear(y => y + 1); }
    else setViewMonth(m => m + 1);
    setSelectedDate(null);
  };

  const isNextDisabled = viewYear > today.getFullYear() ||
    (viewYear === today.getFullYear() && viewMonth >= today.getMonth());

  const totalSessions = sessions.length;
  const totalVolume = sessions.reduce((s, x) => s + x.totalVolume, 0);
  const trainingDays = Object.keys(sessionsByDate).length;

  const selectedSessions = selectedDate ? (sessionsByDate[selectedDate] ?? []) : [];

  return (
    <div className="space-y-6 max-w-5xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-slate-900 flex items-center gap-2">
          <CalendarDays className="h-6 w-6 text-blue-600" />
          Kalendarz treningów
        </h1>
        <p className="text-slate-500 text-sm mt-0.5">Przegląd aktywności i historii sesji</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-3 gap-4">
        <div className="bg-white rounded-2xl border border-slate-200 p-4 shadow-sm text-center">
          <p className="text-xs text-slate-500 font-medium uppercase tracking-wider">Sesje</p>
          <p className="text-2xl font-bold text-slate-900 mt-1">{totalSessions}</p>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-4 shadow-sm text-center">
          <p className="text-xs text-slate-500 font-medium uppercase tracking-wider">Dni aktywnych</p>
          <p className="text-2xl font-bold text-slate-900 mt-1">{trainingDays} / {daysInMonth}</p>
        </div>
        <div className="bg-white rounded-2xl border border-slate-200 p-4 shadow-sm text-center">
          <p className="text-xs text-slate-500 font-medium uppercase tracking-wider">Łączny tonaż</p>
          <p className="text-2xl font-bold text-slate-900 mt-1">{totalVolume.toLocaleString('pl-PL')} <span className="text-sm font-normal text-slate-400">kg</span></p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-[340px_1fr] gap-6">
        {/* Calendar grid */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5">
          <div className="flex items-center justify-between mb-4">
            <button onClick={prevMonth} className="p-2 rounded-lg hover:bg-slate-100 transition-colors text-slate-600">
              <ChevronLeft className="h-4 w-4" />
            </button>
            <span className="text-base font-semibold text-slate-700">{MONTH_NAMES[viewMonth]} {viewYear}</span>
            <button onClick={nextMonth} disabled={isNextDisabled} className={`p-2 rounded-lg transition-colors ${isNextDisabled ? 'text-slate-300 cursor-not-allowed' : 'text-slate-600 hover:bg-slate-100'}`}>
              <ChevronRight className="h-4 w-4" />
            </button>
          </div>

          <div className="grid grid-cols-7 gap-1 mb-2">
            {DAY_NAMES.map(d => (
              <div key={d} className="text-center text-[11px] font-semibold text-slate-400 uppercase py-1">{d}</div>
            ))}
          </div>

          <div className="grid grid-cols-7 gap-1">
            {Array.from({ length: firstDayOfWeek }).map((_, i) => <div key={`e-${i}`} />)}
            {Array.from({ length: daysInMonth }, (_, i) => {
              const day = i + 1;
              const dateStr = `${viewYear}-${String(viewMonth + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
              const isFuture = dateStr > todayStr;
              const isToday = dateStr === todayStr;
              const isSelected = dateStr === selectedDate;
              const daySessions = sessionsByDate[dateStr] ?? [];
              const hasWorkout = daySessions.length > 0;

              return (
                <button
                  key={day}
                  onClick={() => !isFuture && setSelectedDate(dateStr === selectedDate ? null : dateStr)}
                  disabled={isFuture}
                  className={`
                    relative aspect-square rounded-lg text-sm font-medium transition-all flex flex-col items-center justify-center gap-0.5
                    ${isFuture ? 'text-slate-200 cursor-not-allowed' : 'hover:ring-2 hover:ring-blue-300 cursor-pointer'}
                    ${isSelected ? 'ring-2 ring-blue-500 bg-blue-50' : ''}
                    ${isToday && !isSelected ? 'ring-2 ring-slate-300' : ''}
                    ${hasWorkout && !isSelected ? 'bg-green-50 text-green-800' : ''}
                    ${!hasWorkout && !isSelected && !isFuture ? 'text-slate-600 hover:bg-slate-50' : ''}
                  `}
                >
                  <span className="text-xs">{day}</span>
                  {hasWorkout && (
                    <span className="text-[8px] font-bold text-green-600 flex items-center gap-px leading-none">
                      💪{daySessions.length}
                    </span>
                  )}
                </button>
              );
            })}
          </div>
          {loading && <p className="text-center text-xs text-slate-400 mt-3">Ładowanie...</p>}
        </div>

        {/* Right panel */}
        <div className="space-y-4">
          {selectedDate && selectedSessions.length > 0 ? (
            selectedSessions.map(session => (
              <div key={session.id} className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
                <div className="bg-slate-900 text-white px-5 py-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="font-bold text-lg">
                        {new Date(session.date).toLocaleDateString('pl-PL', { weekday: 'long', day: 'numeric', month: 'long' })}
                      </h3>
                      <p className="text-slate-300 text-sm mt-0.5">
                        {session.totalSets} serii · {session.totalVolume.toLocaleString('pl-PL')} kg tonażu
                        {session.durationMinutes ? ` · ${session.durationMinutes} min` : ''}
                      </p>
                    </div>
                    <Dumbbell className="h-6 w-6 text-slate-400" />
                  </div>
                  {session.notes && <p className="text-sm text-slate-300 italic mt-2">„{session.notes}"</p>}
                </div>
                <ul className="divide-y divide-slate-100 max-h-60 overflow-y-auto">
                  {session.sets.map((set, i) => (
                    <li key={i} className="flex justify-between items-center px-5 py-3 text-sm">
                      <span className="text-slate-700">
                        <span className="font-medium">{set.exerciseName}</span>
                        <span className="text-slate-400"> · seria {set.setNumber}</span>
                      </span>
                      <span className="font-semibold text-slate-900">{set.weight} kg × {set.reps}</span>
                    </li>
                  ))}
                </ul>
              </div>
            ))
          ) : selectedDate ? (
            <div className="bg-white rounded-2xl border border-dashed border-slate-300 p-12 text-center text-slate-400">
              <CalendarDays className="h-8 w-8 mx-auto mb-2 opacity-30" />
              <p className="font-medium">Brak treningu tego dnia</p>
            </div>
          ) : (
            <div className="bg-white rounded-2xl border border-dashed border-slate-300 p-12 text-center text-slate-400">
              <CalendarDays className="h-8 w-8 mx-auto mb-2 opacity-30" />
              <p className="font-medium">Wybierz dzień na kalendarzu</p>
              <p className="text-sm mt-1">Zielone dni mają zalogowane treningi.</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
