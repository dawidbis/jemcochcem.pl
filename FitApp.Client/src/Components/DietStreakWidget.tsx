import { useState, useEffect } from 'react';
import { api } from '../api';

export function DietStreakWidget({ userId }: { userId: string }) {
  const [streak, setStreak] = useState<{ current: number; longest: number } | null>(null);

  useEffect(() => {
    // Wykorzystujemy istniejącą w Twoim api.ts metodę pobierania profilu
    api.getUserProfile(userId).then(data => {
      if (data) {
        setStreak({
          current: data.currentStreak || 0,
          longest: data.longestStreak || 0
        });
      }
    });
  }, [userId]);

  if (!streak) return <span className="text-xs text-slate-400">🔥 ...</span>;

  const isActive = streak.current > 0;

  return (
    <div className="flex items-center gap-2 bg-white border border-slate-200 rounded-xl px-4 py-2.5 shadow-sm">
      <span className={`text-xl transition-all ${isActive ? 'normalize' : 'grayscale'}`}>🔥</span>
      <div className="text-left">
        <div className="text-sm font-bold text-slate-800 leading-none">
          {streak.current} {streak.current === 1 ? 'dzień' : 'dni'}
        </div>
        <div className="text-[10px] text-slate-400 font-medium mt-0.5">
          Rekord: {streak.longest}
        </div>
      </div>
    </div>
  );
}