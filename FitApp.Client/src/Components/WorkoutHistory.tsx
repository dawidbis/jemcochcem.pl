import React, { useState, useEffect } from 'react';
import { api } from '../api';
import type { WorkoutSession } from '../types';
import { Trash2 } from 'lucide-react';

interface WorkoutHistoryProps {
  userId: string;
}

export const WorkoutHistory: React.FC<WorkoutHistoryProps> = ({ userId }) => {
  const [sessions, setSessions] = useState<WorkoutSession[]>([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    api.getWorkoutHistory().then((data) => {
      setSessions(data);
      setLoading(false);
    });
  };

  useEffect(() => { load(); }, [userId]);

  const handleDelete = async (id: string) => {
    await api.deleteWorkoutSession(id);
    load();
  };

  if (loading) {
    return <div className="text-center text-sm text-slate-400 py-10">Ładowanie historii...</div>;
  }

  if (sessions.length === 0) {
    return (
      <div className="max-w-2xl mx-auto text-center py-16">
        <p className="text-slate-500">Brak zapisanych treningów.</p>
        <p className="text-sm text-slate-400 mt-1">Zaloguj pierwszą sesję w zakładce „Zaloguj trening".</p>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900">Historia treningów</h1>
        <p className="text-sm text-slate-500">{sessions.length} zapisanych sesji.</p>
      </div>

      {sessions.map((session) => (
        <div key={session.id} className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm">
          <div className="flex justify-between items-center mb-3">
            <span className="font-semibold text-slate-900">
              {new Date(session.date).toLocaleDateString('pl-PL', { weekday: 'short', day: 'numeric', month: 'long' })}
            </span>
            <div className="flex items-center gap-3">
              <span className="text-xs text-slate-400">
                {session.totalSets} serii · {session.totalVolume.toLocaleString('pl-PL')} kg
                {session.durationMinutes ? ` · ${session.durationMinutes} min` : ''}
              </span>
              <button
                onClick={() => handleDelete(session.id)}
                className="p-1.5 text-slate-300 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                title="Usuń sesję"
              >
                <Trash2 className="h-3.5 w-3.5" />
              </button>
            </div>
          </div>

          {session.notes && (
            <p className="text-xs text-slate-500 italic mb-3">„{session.notes}"</p>
          )}

          <ul className="divide-y divide-slate-100">
            {session.sets.map((set, i) => (
              <li key={i} className="flex items-center justify-between py-1.5 text-sm">
                <span className="text-slate-700">
                  <span className="font-medium">{set.exerciseName}</span>
                  <span className="text-slate-400"> · seria {set.setNumber}</span>
                </span>
                <span className="text-slate-900 font-semibold">{set.weight} kg × {set.reps}</span>
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
};
