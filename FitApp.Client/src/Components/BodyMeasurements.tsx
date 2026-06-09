import { useState, useEffect, useCallback } from 'react';
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip,
  ResponsiveContainer, ReferenceLine,
} from 'recharts';
import { TrendingDown, Target, Scale, Activity, Plus, Trash2 } from 'lucide-react';
import type { User, Measurement, MeasurementStats, CreateMeasurementPayload } from '../types';
import { api } from '../api';

interface Props { user: User; }

function StatCard({ label, value, sub, icon, trend }: {
  label: string; value: string; sub?: string;
  icon: React.ReactNode; trend?: 'up' | 'down' | 'neutral';
}) {
  const trendColor = trend === 'down' ? 'text-green-600' : trend === 'up' ? 'text-red-500' : 'text-slate-500';
  return (
    <div className="bg-white rounded-2xl border border-slate-200 p-5 flex flex-col gap-3 shadow-sm">
      <div className="flex items-center justify-between">
        <span className="text-sm text-slate-500 font-medium">{label}</span>
        <div className="p-2 bg-blue-50 rounded-lg text-blue-600">{icon}</div>
      </div>
      <div>
        <p className="text-2xl font-bold text-slate-900">{value}</p>
        {sub && <p className={`text-sm mt-0.5 ${trendColor}`}>{sub}</p>}
      </div>
    </div>
  );
}

const BMI_ZONES = [
  { min: 0, max: 18.5, color: '#93c5fd', label: 'Niedowaga' },
  { min: 18.5, max: 25, color: '#86efac', label: 'Prawidłowa' },
  { min: 25, max: 30, color: '#fde68a', label: 'Nadwaga' },
  { min: 30, max: 45, color: '#fca5a5', label: 'Otyłość' },
];

function BmiGauge({ bmi }: { bmi: number }) {
  const pct = Math.min(100, Math.max(0, ((bmi - 10) / 30) * 100));
  const zone = BMI_ZONES.find(z => bmi >= z.min && bmi < z.max) ?? BMI_ZONES[3];
  return (
    <div className="w-full">
      <div className="flex justify-between text-xs text-slate-400 mb-1">
        <span>10</span><span>18.5</span><span>25</span><span>30</span><span>40</span>
      </div>
      <div className="relative h-3 w-full rounded-full overflow-hidden flex">
        <div className="flex-none w-[21.25%] bg-blue-300 rounded-l-full" />
        <div className="flex-none w-[21.25%] bg-green-300" />
        <div className="flex-none w-[16.25%] bg-yellow-200" />
        <div className="flex-1 bg-red-300 rounded-r-full" />
        <div
          className="absolute top-0 h-3 w-1 bg-slate-800 rounded-full transform -translate-x-1/2 transition-all duration-500"
          style={{ left: `${pct}%` }}
        />
      </div>
      <p className="text-xs text-center mt-1.5 font-medium" style={{ color: zone.color === '#86efac' ? '#16a34a' : '#64748b' }}>
        {zone.label}
      </p>
    </div>
  );
}

const toChartDate = (iso: string) =>
  new Date(iso).toLocaleDateString('pl-PL', { day: '2-digit', month: '2-digit' });

export function BodyMeasurements({ user }: Props) {
  const [measurements, setMeasurements] = useState<Measurement[]>([]);
  const [stats, setStats] = useState<MeasurementStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [targetInput, setTargetInput] = useState('');
  const [editingTarget, setEditingTarget] = useState(false);

  const [form, setForm] = useState<Partial<CreateMeasurementPayload>>({
    date: new Date().toISOString().split('T')[0],
  });

  const load = useCallback(async () => {
    setLoading(true);
    const [m, s] = await Promise.all([
      api.getMeasurements(),
      api.getMeasurementStats(),
    ]);
    setMeasurements(m);
    setStats(s);
    if (s?.targetWeight) setTargetInput(String(s.targetWeight));
    setLoading(false);
  }, [user.userId]);

  useEffect(() => { load(); }, [load]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.weight || !form.date) return;

    await api.createMeasurement({
      weight: Number(form.weight),
      date: new Date(form.date).toISOString(),
      bodyFatPercentage: form.bodyFatPercentage ? Number(form.bodyFatPercentage) : null,
      waist: form.waist ? Number(form.waist) : null,
      hips: form.hips ? Number(form.hips) : null,
      notes: form.notes || null,
    });
    setForm({ date: new Date().toISOString().split('T')[0] });
    setShowForm(false);
    await load();
  };

  const handleDelete = async (id: string) => {
    await api.deleteMeasurement(id);
    await load();
  };

  const handleSaveTarget = async () => {
    const val = targetInput ? Number(targetInput) : null;
    await api.setTargetWeight(val);
    setEditingTarget(false);
    await load();
  };

  const chartData = [...measurements]
    .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    .map(m => ({
      date: toChartDate(m.date),
      waga: m.weight,
      bmi: m.bmi ?? undefined,
      tłuszcz: m.bodyFatPercentage ?? undefined,
    }));

  const weightTrend = stats?.weightChange != null
    ? stats.weightChange < 0 ? 'down' : stats.weightChange > 0 ? 'up' : 'neutral'
    : 'neutral';

  const weightChangeSub = stats?.weightChange != null
    ? `${stats.weightChange > 0 ? '+' : ''}${stats.weightChange} kg od poprzedniego`
    : undefined;

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64 text-slate-400">
        Ładowanie...
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-slate-900">Pomiary ciała</h1>
          <p className="text-slate-500 text-sm mt-0.5">Śledź swoją wagę, BMI i wymiary</p>
        </div>
        <button
          onClick={() => setShowForm(v => !v)}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-xl font-medium hover:bg-blue-700 transition-colors text-sm"
        >
          <Plus className="h-4 w-4" />
          Dodaj pomiar
        </button>
      </div>

      {/* Add form */}
      {showForm && (
        <form
          onSubmit={handleSubmit}
          className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm space-y-4"
        >
          <h2 className="font-semibold text-slate-800">Nowy pomiar</h2>
          <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
            <label className="flex flex-col gap-1">
              <span className="text-xs text-slate-500 font-medium">Data *</span>
              <input
                type="date"
                required
                value={form.date ?? ''}
                onChange={e => setForm(f => ({ ...f, date: e.target.value }))}
                className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-xs text-slate-500 font-medium">Waga (kg) *</span>
              <input
                type="number"
                step="0.1"
                min="1"
                required
                placeholder="np. 75.5"
                value={form.weight ?? ''}
                onChange={e => setForm(f => ({ ...f, weight: e.target.value as any }))}
                className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-xs text-slate-500 font-medium">Tkanka tłuszczowa (%)</span>
              <input
                type="number"
                step="0.1"
                min="1"
                max="60"
                placeholder="np. 18"
                value={form.bodyFatPercentage ?? ''}
                onChange={e => setForm(f => ({ ...f, bodyFatPercentage: e.target.value as any }))}
                className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-xs text-slate-500 font-medium">Talia (cm)</span>
              <input
                type="number"
                step="0.5"
                min="1"
                placeholder="np. 80"
                value={form.waist ?? ''}
                onChange={e => setForm(f => ({ ...f, waist: e.target.value as any }))}
                className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-xs text-slate-500 font-medium">Biodra (cm)</span>
              <input
                type="number"
                step="0.5"
                min="1"
                placeholder="np. 95"
                value={form.hips ?? ''}
                onChange={e => setForm(f => ({ ...f, hips: e.target.value as any }))}
                className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            </label>
            <label className="flex flex-col gap-1">
              <span className="text-xs text-slate-500 font-medium">Notatka</span>
              <input
                type="text"
                placeholder="np. rano na czczo"
                value={form.notes ?? ''}
                onChange={e => setForm(f => ({ ...f, notes: e.target.value }))}
                className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            </label>
          </div>
          <div className="flex gap-3 pt-1">
            <button type="submit" className="bg-blue-600 text-white px-5 py-2 rounded-xl text-sm font-medium hover:bg-blue-700 transition-colors">
              Zapisz
            </button>
            <button type="button" onClick={() => setShowForm(false)} className="text-slate-500 px-4 py-2 rounded-xl text-sm hover:bg-slate-100 transition-colors">
              Anuluj
            </button>
          </div>
        </form>
      )}

      {/* Stats cards */}
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        <StatCard
          label="Aktualna waga"
          value={stats?.latestWeight != null ? `${stats.latestWeight} kg` : '—'}
          sub={weightChangeSub}
          icon={<Scale className="h-4 w-4" />}
          trend={weightTrend}
        />
        <StatCard
          label="BMI"
          value={stats?.currentBmi != null ? String(stats.currentBmi) : '—'}
          sub={stats?.bmiCategory ?? undefined}
          icon={<Activity className="h-4 w-4" />}
        />
        <StatCard
          label="Cel wagowy"
          value={stats?.targetWeight != null ? `${stats.targetWeight} kg` : 'Brak'}
          sub={stats?.progressPercent != null ? `${stats.progressPercent}% postępu` : undefined}
          icon={<Target className="h-4 w-4" />}
          trend="neutral"
        />
        <StatCard
          label="Pomiarów łącznie"
          value={String(stats?.totalMeasurements ?? 0)}
          icon={<TrendingDown className="h-4 w-4" />}
        />
      </div>

      {/* Target weight editor */}
      <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
        <div className="flex items-center justify-between mb-3">
          <h2 className="font-semibold text-slate-800 flex items-center gap-2">
            <Target className="h-4 w-4 text-blue-500" />
            Cel wagowy
          </h2>
          {!editingTarget && (
            <button onClick={() => setEditingTarget(true)} className="text-xs text-blue-600 hover:underline">
              {stats?.targetWeight ? 'Zmień' : 'Ustaw'}
            </button>
          )}
        </div>
        {editingTarget ? (
          <div className="flex gap-3 items-center">
            <input
              type="number"
              step="0.1"
              min="1"
              placeholder="np. 70"
              value={targetInput}
              onChange={e => setTargetInput(e.target.value)}
              className="border border-slate-200 rounded-lg px-3 py-2 text-sm w-32 focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
            <span className="text-sm text-slate-500">kg</span>
            <button onClick={handleSaveTarget} className="bg-blue-600 text-white px-4 py-2 rounded-xl text-sm font-medium hover:bg-blue-700">
              Zapisz
            </button>
            <button onClick={() => setEditingTarget(false)} className="text-slate-500 text-sm hover:underline">
              Anuluj
            </button>
          </div>
        ) : (
          <div>
            {stats?.targetWeight != null ? (
              <div className="space-y-2">
                <div className="flex items-center justify-between text-sm text-slate-500">
                  <span>Aktualnie: {stats.latestWeight ?? '—'} kg</span>
                  <span>Cel: {stats.targetWeight} kg</span>
                </div>
                <div className="h-2.5 w-full bg-slate-100 rounded-full overflow-hidden">
                  <div
                    className="h-full bg-blue-500 rounded-full transition-all duration-700"
                    style={{ width: `${stats.progressPercent ?? 0}%` }}
                  />
                </div>
                <p className="text-xs text-slate-400">{stats.progressPercent ?? 0}% postępu</p>
              </div>
            ) : (
              <p className="text-sm text-slate-400">Nie ustawiono celu wagowego.</p>
            )}
          </div>
        )}
      </div>

      {/* BMI gauge */}
      {stats?.currentBmi != null && (
        <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
          <h2 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
            <Activity className="h-4 w-4 text-blue-500" />
            Wskaźnik BMI — {stats.currentBmi} ({stats.bmiCategory})
          </h2>
          <BmiGauge bmi={stats.currentBmi} />
        </div>
      )}

      {chartData.length > 1 && (
        <>
          {/* Weight chart */}
          <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
            <h2 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
              <Scale className="h-4 w-4 text-blue-500" />
              Historia wagi
            </h2>
            <ResponsiveContainer width="100%" height={220}>
              <LineChart data={chartData} margin={{ top: 5, right: 10, left: -10, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                <XAxis dataKey="date" tick={{ fontSize: 11, fill: '#94a3b8' }} />
                <YAxis
                  domain={['auto', 'auto']}
                  tick={{ fontSize: 11, fill: '#94a3b8' }}
                  tickFormatter={v => `${v} kg`}
                />
                <Tooltip
                  contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 13 }}
                  formatter={(v: any) => [`${v} kg`, 'Waga']}
                />
                {stats?.targetWeight && (
                  <ReferenceLine
                    y={stats.targetWeight}
                    stroke="#3b82f6"
                    strokeDasharray="5 5"
                    label={{ value: `Cel: ${stats.targetWeight} kg`, position: 'insideTopRight', fontSize: 11, fill: '#3b82f6' }}
                  />
                )}
                <Line
                  type="monotone"
                  dataKey="waga"
                  stroke="#3b82f6"
                  strokeWidth={2.5}
                  dot={{ r: 4, fill: '#3b82f6', strokeWidth: 0 }}
                  activeDot={{ r: 6 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          {/* BMI chart */}
          {chartData.some(d => d.bmi != null) && (
            <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
              <h2 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
                <Activity className="h-4 w-4 text-blue-500" />
                Historia BMI
              </h2>
              <ResponsiveContainer width="100%" height={220}>
                <LineChart data={chartData} margin={{ top: 5, right: 10, left: -10, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                  <XAxis dataKey="date" tick={{ fontSize: 11, fill: '#94a3b8' }} />
                  <YAxis
                    domain={[14, 36]}
                    tick={{ fontSize: 11, fill: '#94a3b8' }}
                    tickFormatter={v => String(v)}
                  />
                  <Tooltip
                    contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 13 }}
                    formatter={(v: any) => [v, 'BMI']}
                  />
                  <ReferenceLine y={18.5} stroke="#93c5fd" strokeDasharray="4 4" />
                  <ReferenceLine y={25} stroke="#86efac" strokeDasharray="4 4" />
                  <ReferenceLine y={30} stroke="#fde68a" strokeDasharray="4 4" />
                  <Line
                    type="monotone"
                    dataKey="bmi"
                    stroke="#8b5cf6"
                    strokeWidth={2.5}
                    dot={{ r: 4, fill: '#8b5cf6', strokeWidth: 0 }}
                    activeDot={{ r: 6 }}
                    connectNulls
                  />
                </LineChart>
              </ResponsiveContainer>
              <div className="flex gap-4 mt-2 justify-end text-xs text-slate-400">
                {BMI_ZONES.map(z => (
                  <span key={z.label} className="flex items-center gap-1">
                    <span className="inline-block w-2 h-2 rounded-full" style={{ background: z.color }} />
                    {z.label}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Body fat chart */}
          {chartData.some(d => d.tłuszcz != null) && (
            <div className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm">
              <h2 className="font-semibold text-slate-800 mb-4 flex items-center gap-2">
                <TrendingDown className="h-4 w-4 text-blue-500" />
                Tkanka tłuszczowa (%)
              </h2>
              <ResponsiveContainer width="100%" height={180}>
                <LineChart data={chartData} margin={{ top: 5, right: 10, left: -10, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                  <XAxis dataKey="date" tick={{ fontSize: 11, fill: '#94a3b8' }} />
                  <YAxis
                    domain={['auto', 'auto']}
                    tick={{ fontSize: 11, fill: '#94a3b8' }}
                    tickFormatter={v => `${v}%`}
                  />
                  <Tooltip
                    contentStyle={{ borderRadius: '12px', border: '1px solid #e2e8f0', fontSize: 13 }}
                    formatter={(v: any) => [`${v}%`, 'Tłuszcz']}
                  />
                  <Line
                    type="monotone"
                    dataKey="tłuszcz"
                    stroke="#f59e0b"
                    strokeWidth={2.5}
                    dot={{ r: 4, fill: '#f59e0b', strokeWidth: 0 }}
                    activeDot={{ r: 6 }}
                    connectNulls
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          )}
        </>
      )}

      {/* History table */}
      {measurements.length > 0 && (
        <div className="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
          <div className="px-5 py-4 border-b border-slate-100">
            <h2 className="font-semibold text-slate-800">Historia pomiarów</h2>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-slate-50 text-slate-500 text-xs uppercase tracking-wide">
                  <th className="px-5 py-3 text-left font-medium">Data</th>
                  <th className="px-5 py-3 text-right font-medium">Waga</th>
                  <th className="px-5 py-3 text-right font-medium">BMI</th>
                  <th className="px-5 py-3 text-right font-medium">Tłuszcz %</th>
                  <th className="px-5 py-3 text-right font-medium">Talia</th>
                  <th className="px-5 py-3 text-right font-medium">Biodra</th>
                  <th className="px-5 py-3 text-left font-medium">Notatka</th>
                  <th className="px-3 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {measurements.map((m, i) => {
                  const prev = measurements[i + 1];
                  const diff = prev ? m.weight - prev.weight : null;
                  return (
                    <tr key={m.id} className="hover:bg-slate-50 transition-colors">
                      <td className="px-5 py-3 text-slate-700 font-medium">
                        {new Date(m.date).toLocaleDateString('pl-PL')}
                      </td>
                      <td className="px-5 py-3 text-right font-semibold text-slate-900">
                        {m.weight} kg
                        {diff != null && (
                          <span className={`ml-1.5 text-xs font-normal ${diff < 0 ? 'text-green-600' : diff > 0 ? 'text-red-500' : 'text-slate-400'}`}>
                            {diff > 0 ? '+' : ''}{diff.toFixed(1)}
                          </span>
                        )}
                      </td>
                      <td className="px-5 py-3 text-right text-slate-600">
                        {m.bmi ?? '—'}
                      </td>
                      <td className="px-5 py-3 text-right text-slate-600">
                        {m.bodyFatPercentage != null ? `${m.bodyFatPercentage}%` : '—'}
                      </td>
                      <td className="px-5 py-3 text-right text-slate-600">
                        {m.waist != null ? `${m.waist} cm` : '—'}
                      </td>
                      <td className="px-5 py-3 text-right text-slate-600">
                        {m.hips != null ? `${m.hips} cm` : '—'}
                      </td>
                      <td className="px-5 py-3 text-slate-500 max-w-[160px] truncate">
                        {m.notes ?? ''}
                      </td>
                      <td className="px-3 py-3">
                        <button
                          onClick={() => handleDelete(m.id)}
                          className="p-1.5 text-slate-300 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                          title="Usuń"
                        >
                          <Trash2 className="h-3.5 w-3.5" />
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {measurements.length === 0 && !showForm && (
        <div className="bg-white border border-dashed border-slate-300 rounded-2xl p-12 text-center">
          <Scale className="h-10 w-10 text-slate-300 mx-auto mb-3" />
          <p className="text-slate-400 font-medium">Brak pomiarów</p>
          <p className="text-slate-400 text-sm mt-1">Kliknij „Dodaj pomiar", żeby zacząć śledzić postępy.</p>
        </div>
      )}
    </div>
  );
}
