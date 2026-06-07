import React, { useState, useEffect, useMemo } from 'react';
import { api } from '../api';
import type { Exercise, LoggedSet } from '../types';
import { Button } from "#components/ui/button";

interface WorkoutLoggerProps {
  userId: string;
}

export const WorkoutLogger: React.FC<WorkoutLoggerProps> = ({ userId }) => {
  const [exercises, setExercises] = useState<Exercise[]>([]);
  const [selectedExerciseId, setSelectedExerciseId] = useState<string>('');
  const [weight, setWeight] = useState<string>('');
  const [reps, setReps] = useState<string>('');
  const [notes, setNotes] = useState<string>('');
  const [duration, setDuration] = useState<string>('');
  const [loggedSets, setLoggedSets] = useState<LoggedSet[]>([]);
  const [saving, setSaving] = useState(false);
  const [savedMsg, setSavedMsg] = useState<string | null>(null);

  useEffect(() => {
    api.getExercises(userId).then(setExercises);
  }, [userId]);

  // mapa nazw ćwiczeń dla podglądu serii
  const exerciseMap = useMemo(() => {
    const m = new Map<string, Exercise>();
    exercises.forEach((e) => m.set(e.id, e));
    return m;
  }, [exercises]);

  const handleAddSet = () => {
    const w = parseFloat(weight.replace(',', '.'));
    const r = parseInt(reps, 10);
    if (!selectedExerciseId || isNaN(w) || isNaN(r) || w < 0 || r <= 0) return;

    // numer serii w ramach danego ćwiczenia
    const setNumber = loggedSets.filter((s) => s.exerciseId === selectedExerciseId).length + 1;

    setLoggedSets((prev) => [
      ...prev,
      { exerciseId: selectedExerciseId, setNumber, weight: w, reps: r },
    ]);
    setWeight('');
    setReps('');
  };

  const handleRemoveSet = (index: number) => {
    setLoggedSets((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSaveSession = async () => {
    if (loggedSets.length === 0) return;
    setSaving(true);
    setSavedMsg(null);

    const today = new Date().toISOString().split('T')[0];
    const res = await api.logWorkout({
      userId,
      date: today,
      notes: notes || null,
      durationMinutes: duration ? parseInt(duration, 10) : null,
      sets: loggedSets,
    });

    setSaving(false);
    if (res.ok) {
      setLoggedSets([]);
      setNotes('');
      setDuration('');
      setSavedMsg('Trening zapisany! 💪');
      setTimeout(() => setSavedMsg(null), 3000);
    }
  };

  const totalVolume = loggedSets.reduce((sum, s) => sum + s.weight * s.reps, 0);

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Zaloguj trening</h1>
        <p className="text-sm text-slate-500">Dodawaj serie ćwiczenie po ćwiczeniu, na koniec zapisz sesję.</p>
      </div>

      {/* Formularz dodawania serii */}
      <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm space-y-4">
        <div>
          <label className="text-xs text-slate-500 font-medium uppercase tracking-wider">Ćwiczenie</label>
          <select
            value={selectedExerciseId}
            onChange={(e) => setSelectedExerciseId(e.target.value)}
            className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm focus:border-blue-400 focus:outline-none"
          >
            <option value="">— wybierz ćwiczenie —</option>
            {exercises.map((ex) => (
              <option key={ex.id} value={ex.id}>
                {ex.muscleGroup} · {ex.name}
              </option>
            ))}
          </select>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-xs text-slate-500 font-medium uppercase tracking-wider">Ciężar (kg)</label>
            <input
              type="number"
              inputMode="decimal"
              value={weight}
              onChange={(e) => setWeight(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm focus:border-blue-400 focus:outline-none"
              placeholder="np. 80"
            />
          </div>
          <div>
            <label className="text-xs text-slate-500 font-medium uppercase tracking-wider">Powtórzenia</label>
            <input
              type="number"
              inputMode="numeric"
              value={reps}
              onChange={(e) => setReps(e.target.value)}
              className="mt-1 w-full rounded-lg border border-slate-200 px-3 py-2 text-sm focus:border-blue-400 focus:outline-none"
              placeholder="np. 8"
            />
          </div>
        </div>

        <Button
          onClick={handleAddSet}
          className="w-full bg-blue-50 text-blue-600 hover:bg-blue-100 border border-blue-100 font-semibold rounded-lg py-2 text-sm transition-colors"
        >
          + Dodaj serię
        </Button>
      </div>

      {/* Podgląd serii bieżącej sesji */}
      {loggedSets.length > 0 && (
        <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm space-y-3">
          <div className="flex justify-between items-center">
            <span className="text-xs text-slate-500 font-medium uppercase tracking-wider">Serie w tej sesji</span>
            <span className="text-xs text-slate-400">Tonaż: {totalVolume.toLocaleString('pl-PL')} kg</span>
          </div>
          <ul className="divide-y divide-slate-100">
            {loggedSets.map((s, i) => (
              <li key={i} className="flex items-center justify-between py-2 text-sm">
                <span className="text-slate-700">
                  <span className="font-medium">{exerciseMap.get(s.exerciseId)?.name ?? '?'}</span>
                  <span className="text-slate-400"> · seria {s.setNumber}</span>
                </span>
                <span className="flex items-center gap-3">
                  <span className="text-slate-900 font-semibold">{s.weight} kg × {s.reps}</span>
                  <button
                    onClick={() => handleRemoveSet(i)}
                    className="text-[11px] text-slate-400 hover:text-red-500 transition-colors"
                  >
                    usuń
                  </button>
                </span>
              </li>
            ))}
          </ul>

          <div className="grid grid-cols-2 gap-3 pt-2">
            <input
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              className="rounded-lg border border-slate-200 px-3 py-2 text-sm focus:border-blue-400 focus:outline-none"
              placeholder="Notatka (opcjonalnie)"
            />
            <input
              type="number"
              inputMode="numeric"
              value={duration}
              onChange={(e) => setDuration(e.target.value)}
              className="rounded-lg border border-slate-200 px-3 py-2 text-sm focus:border-blue-400 focus:outline-none"
              placeholder="Czas (min)"
            />
          </div>

          <Button
            onClick={handleSaveSession}
            disabled={saving}
            className="w-full bg-blue-600 text-white hover:bg-blue-700 font-semibold rounded-lg py-2.5 text-sm transition-colors disabled:opacity-50"
          >
            {saving ? 'Zapisywanie...' : 'Zapisz trening'}
          </Button>
        </div>
      )}

      {savedMsg && (
        <div className="text-center text-sm font-medium text-green-600">{savedMsg}</div>
      )}
    </div>
  );
};
