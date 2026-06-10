import { useState, useEffect } from 'react';
import {
  LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip,
  ResponsiveContainer, Legend,
} from 'recharts';
import { TrendingUp, Target, Zap, ChevronDown } from 'lucide-react';
import { api } from '../api';
import type { Exercise, ExerciseProgression } from '../types';

export function WorkoutProgressionChart() {
  const [exercises, setExercises] = useState<Exercise[]>([]);
  const [selectedId, setSelectedId] = useState('');
  const [progression, setProgression] = useState<ExerciseProgression | null>(null);
  const [loading, setLoading] = useState(false);
  const [noData, setNoData] = useState(false);

  useEffect(() => {
    api.getExercises().then(setExercises);
  }, []);

  useEffect(() => {
    if (!selectedId) { setProgression(null); setNoData(false); return; }
    setLoading(true);
    setNoData(false);
    api.getExerciseProgression(selectedId).then(data => {
      setProgression(data);
      setNoData(!data);
      setLoading(false);
    });
  }, [selectedId]);

  const chartData = progression?.history.map(p => ({
    date: new Date(p.date).toLocaleDateString('pl-PL', { day: '2-digit', month: '2-digit' }),
    '1RM (kg)': Number(p.estOneRepMax.toFixed(1)),
    'Maks. ciężar': Number(p.maxWeight),
    'Tonaż (kg)': Number(p.totalVolume.toFixed(0)),
  })) ?? [];

  const byGroup = exercises.reduce<Record<string, Exercise[]>>((acc, ex) => {
    (acc[ex.muscleGroup] ??= []).push(ex);
    return acc;
  }, {});

  return (
    <div className="space-y-6 max-w-4xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Progresja</h1>
        <p className="text-sm text-slate-500 mt-0.5">Analizuj postępy i oblicz cel na następny trening</p>
      </div>

      {/* Exercise selector */}
      <div className="relative bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
        <label className="text-xs text-slate-500 font-medium uppercase tracking-wider block mb-2">Wybierz ćwiczenie</label>
        <div className="relative">
          <select
            value={selectedId}
            onChange={e => setSelectedId(e.target.value)}
            className="w-full border border-slate-200 rounded-xl px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white appearance-none pr-10"
          >
            <option value="">— wybierz ćwiczenie —</option>
            {Object.entries(byGroup).map(([group, exs]) => (
              <optgroup key={group} label={group}>
                {exs.map(ex => <option key={ex.id} value={ex.id}>{ex.name}{ex.isCustom ? ' ★' : ''}</option>)}
              </optgroup>
            ))}
          </select>
          <ChevronDown className="absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400 pointer-events-none" />
        </div>
      </div>

      {loading && (
        <div className="text-center py-12 text-slate-400">Ładowanie danych...</div>
      )}

      {noData && (
        <div className="text-center py-12 text-slate-400">
          <TrendingUp className="h-10 w-10 mx-auto mb-3 opacity-30" />
          <p className="font-medium">Brak danych dla tego ćwiczenia</p>
          <p className="text-sm mt-1">Zaloguj przynajmniej jeden trening z tym ćwiczeniem.</p>
        </div>
      )}

      {progression && !loading && (
        <>
          {/* Suggestion card */}
          {progression.suggestion && (
            <div className="bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-2xl p-5 shadow-sm">
              <div className="flex items-start gap-3">
                <div className="p-2 bg-white/20 rounded-xl">
                  <Zap className="h-5 w-5" />
                </div>
                <div className="flex-1">
                  <p className="text-sm font-medium text-blue-100 mb-1">Sugestia na następny trening</p>
                  <p className="text-xl font-bold">{progression.suggestion.message}</p>
                  <div className="flex gap-6 mt-3 text-sm text-blue-100">
                    <span>Ciężar: <strong className="text-white">{progression.suggestion.suggestedWeight} kg</strong></span>
                    <span>Powtórzenia: <strong className="text-white">{progression.suggestion.suggestedReps}</strong></span>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Summary stats */}
          <div className="grid grid-cols-3 gap-4">
            {(() => {
              const last = progression.history.at(-1);
              const first = progression.history[0];
              const orm1st = first ? Number(first.estOneRepMax.toFixed(1)) : 0;
              const ormLast = last ? Number(last.estOneRepMax.toFixed(1)) : 0;
              const ormGain = Number((ormLast - orm1st).toFixed(1));
              return (
                <>
                  <div className="bg-white border border-slate-200 rounded-2xl p-4 shadow-sm text-center">
                    <p className="text-xs text-slate-500 font-medium uppercase tracking-wider">Maks. ciężar</p>
                    <p className="text-2xl font-bold text-slate-900 mt-1">{last?.maxWeight ?? 0} <span className="text-sm font-normal text-slate-400">kg</span></p>
                  </div>
                  <div className="bg-white border border-slate-200 rounded-2xl p-4 shadow-sm text-center">
                    <p className="text-xs text-slate-500 font-medium uppercase tracking-wider">Szac. 1RM</p>
                    <p className="text-2xl font-bold text-slate-900 mt-1">{ormLast} <span className="text-sm font-normal text-slate-400">kg</span></p>
                  </div>
                  <div className="bg-white border border-slate-200 rounded-2xl p-4 shadow-sm text-center">
                    <p className="text-xs text-slate-500 font-medium uppercase tracking-wider">Wzrost 1RM</p>
                    <p className={`text-2xl font-bold mt-1 ${ormGain >= 0 ? 'text-green-600' : 'text-red-500'}`}>
                      {ormGain > 0 ? '+' : ''}{ormGain} <span className="text-sm font-normal text-slate-400">kg</span>
                    </p>
                  </div>
                </>
              );
            })()}
          </div>

          {/* 1RM chart */}
          <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
            <h3 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
              <Target className="h-4 w-4 text-blue-500" />
              Szacowany 1RM — {progression.exerciseName}
            </h3>
            <ResponsiveContainer width="100%" height={220}>
              <LineChart data={chartData} margin={{ top: 5, right: 10, left: -10, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="date" tick={{ fontSize: 11, fill: '#94a3b8' }} />
                <YAxis tick={{ fontSize: 11, fill: '#94a3b8' }} tickFormatter={v => `${v}kg`} />
                <Tooltip
                  contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 13 }}
                  formatter={(v: any, name: string) => [`${v} kg`, name]}
                />
                <Legend wrapperStyle={{ fontSize: 12 }} />
                <Line type="monotone" dataKey="1RM (kg)" stroke="#3b82f6" strokeWidth={2.5} dot={{ r: 4, fill: '#3b82f6', strokeWidth: 0 }} activeDot={{ r: 6 }} />
                <Line type="monotone" dataKey="Maks. ciężar" stroke="#8b5cf6" strokeWidth={2} strokeDasharray="5 5" dot={false} />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {/* Volume chart */}
          <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
            <h3 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
              <TrendingUp className="h-4 w-4 text-blue-500" />
              Tonaż na sesję (kg)
            </h3>
            <ResponsiveContainer width="100%" height={180}>
              <BarChart data={chartData} margin={{ top: 5, right: 10, left: -10, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="date" tick={{ fontSize: 11, fill: '#94a3b8' }} />
                <YAxis tick={{ fontSize: 11, fill: '#94a3b8' }} tickFormatter={v => `${v}kg`} />
                <Tooltip
                  contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 13 }}
                  formatter={(v: any) => [`${v} kg`, 'Tonaż']}
                />
                <Bar dataKey="Tonaż (kg)" fill="#3b82f6" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* History table */}
          <div className="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
            <div className="px-5 py-4 border-b border-slate-100">
              <h3 className="font-semibold text-slate-800">Historia sesji</h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="bg-slate-50 text-slate-500 text-xs uppercase tracking-wide">
                    <th className="px-5 py-3 text-left font-medium">Data</th>
                    <th className="px-5 py-3 text-right font-medium">Maks. ciężar</th>
                    <th className="px-5 py-3 text-right font-medium">Powt.</th>
                    <th className="px-5 py-3 text-right font-medium">Szac. 1RM</th>
                    <th className="px-5 py-3 text-right font-medium">Tonaż</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {[...progression.history].reverse().map((p, i) => (
                    <tr key={i} className={`hover:bg-slate-50 transition-colors ${i === 0 ? 'bg-blue-50/40' : ''}`}>
                      <td className="px-5 py-3 text-slate-700 font-medium">
                        {new Date(p.date).toLocaleDateString('pl-PL')}
                        {i === 0 && <span className="ml-2 text-xs text-blue-600 font-semibold">ostatni</span>}
                      </td>
                      <td className="px-5 py-3 text-right font-semibold text-slate-900">{p.maxWeight} kg</td>
                      <td className="px-5 py-3 text-right text-slate-600">{p.bestReps}</td>
                      <td className="px-5 py-3 text-right text-slate-600">{p.estOneRepMax.toFixed(1)} kg</td>
                      <td className="px-5 py-3 text-right text-slate-600">{p.totalVolume.toFixed(0)} kg</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
