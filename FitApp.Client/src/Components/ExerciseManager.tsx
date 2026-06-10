import { useState, useEffect } from 'react';
import { api } from '../api';
import type { Exercise } from '../types';
import { Plus, Trash2, Dumbbell } from 'lucide-react';

const MUSCLE_GROUPS = ['Klatka', 'Plecy', 'Nogi', 'Barki', 'Ramiona', 'Brzuch', 'Cardio', 'Inne'];

export function ExerciseManager() {
  const [exercises, setExercises] = useState<Exercise[]>([]);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState('');
  const [muscleGroup, setMuscleGroup] = useState(MUSCLE_GROUPS[0]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setExercises(await api.getExercises());
    setLoading(false);
  };

  useEffect(() => { load(); }, []);

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    setSaving(true);
    setError(null);
    const result = await api.addCustomExercise({ name: name.trim(), muscleGroup });
    if (result) {
      setName('');
      await load();
    } else {
      setError('Nie udało się dodać ćwiczenia.');
    }
    setSaving(false);
  };

  const handleDelete = async (id: string) => {
    const ok = await api.deleteCustomExercise(id);
    if (ok) await load();
  };

  const customExercises = exercises.filter(e => e.isCustom);
  const globalExercises = exercises.filter(e => !e.isCustom);
  const byGroup = globalExercises.reduce<Record<string, Exercise[]>>((acc, ex) => {
    (acc[ex.muscleGroup] ??= []).push(ex);
    return acc;
  }, {});

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Zarządzaj ćwiczeniami</h1>
        <p className="text-sm text-slate-500 mt-0.5">Dodaj własne ćwiczenia do swojej bazy</p>
      </div>

      {/* Add form */}
      <form onSubmit={handleAdd} className="bg-white border border-slate-200 rounded-2xl p-5 shadow-sm space-y-4">
        <h2 className="font-semibold text-slate-800 flex items-center gap-2">
          <Plus className="h-4 w-4 text-blue-600" />
          Nowe ćwiczenie
        </h2>
        <div className="flex gap-3 flex-col sm:flex-row">
          <input
            type="text"
            required
            value={name}
            onChange={e => setName(e.target.value)}
            placeholder="Nazwa ćwiczenia (np. Face Pull)"
            className="flex-1 border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
          />
          <select
            value={muscleGroup}
            onChange={e => setMuscleGroup(e.target.value)}
            className="border border-slate-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400 bg-white"
          >
            {MUSCLE_GROUPS.map(g => <option key={g}>{g}</option>)}
          </select>
          <button
            type="submit"
            disabled={saving || !name.trim()}
            className="bg-blue-600 text-white px-5 py-2 rounded-xl text-sm font-medium hover:bg-blue-700 transition-colors disabled:opacity-50 whitespace-nowrap"
          >
            {saving ? 'Dodawanie...' : 'Dodaj'}
          </button>
        </div>
        {error && <p className="text-red-500 text-sm">{error}</p>}
      </form>

      {/* Custom exercises */}
      <div className="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
        <div className="px-5 py-4 border-b border-slate-100 flex items-center gap-2">
          <Dumbbell className="h-4 w-4 text-blue-500" />
          <h2 className="font-semibold text-slate-800">Moje ćwiczenia</h2>
          <span className="ml-auto text-xs text-slate-400">{customExercises.length} własnych</span>
        </div>
        {loading ? (
          <div className="p-8 text-center text-slate-400 text-sm">Ładowanie...</div>
        ) : customExercises.length === 0 ? (
          <div className="p-8 text-center text-slate-400 text-sm">Nie masz jeszcze własnych ćwiczeń.</div>
        ) : (
          <ul className="divide-y divide-slate-100">
            {customExercises.map(ex => (
              <li key={ex.id} className="flex items-center justify-between px-5 py-3">
                <div>
                  <span className="font-medium text-slate-800">{ex.name}</span>
                  <span className="ml-2 text-xs text-slate-500 bg-slate-100 px-2 py-0.5 rounded-full">{ex.muscleGroup}</span>
                </div>
                <button
                  onClick={() => handleDelete(ex.id)}
                  className="p-1.5 text-slate-300 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                  title="Usuń"
                >
                  <Trash2 className="h-3.5 w-3.5" />
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>

      {/* Global exercises reference */}
      <div className="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden">
        <div className="px-5 py-4 border-b border-slate-100">
          <h2 className="font-semibold text-slate-800">Ćwiczenia globalne</h2>
          <p className="text-xs text-slate-400 mt-0.5">Dostępne dla wszystkich użytkowników</p>
        </div>
        <div className="p-5 space-y-4">
          {Object.entries(byGroup).map(([group, exs]) => (
            <div key={group}>
              <p className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">{group}</p>
              <div className="flex flex-wrap gap-2">
                {exs.map(ex => (
                  <span key={ex.id} className="text-xs bg-slate-100 text-slate-700 px-2.5 py-1 rounded-full">
                    {ex.name}
                  </span>
                ))}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
