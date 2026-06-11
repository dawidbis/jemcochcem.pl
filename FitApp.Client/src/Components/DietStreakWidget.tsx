import { useState, useEffect } from 'react';
import { Flame, Trophy } from 'lucide-react';
import { api } from '../api';

export function DietStreakWidget({ userId }: { userId: string }) {
  const [streak, setStreak] = useState<{ current: number; longest: number } | null>(null);

  useEffect(() => {
    api.getUserProfile(userId).then(data => {
      if (data) setStreak({ current: data.currentStreak || 0, longest: data.longestStreak || 0 });
    });
  }, [userId]);

  if (!streak) {
    return (
      <div className="flex items-center gap-2 px-3 py-2 bg-slate-50 rounded-xl animate-pulse">
        <div className="w-4 h-4 bg-slate-200 rounded" />
        <div className="w-12 h-3 bg-slate-200 rounded" />
      </div>
    );
  }

  const isActive = streak.current > 0;

  return (
    <div className={`flex items-center gap-3 rounded-xl px-3 py-2 border transition-colors ${isActive ? 'bg-orange-50 border-orange-200' : 'bg-slate-50 border-slate-200'}`}>
      <div className={`flex items-center justify-center w-8 h-8 rounded-lg ${isActive ? 'bg-orange-100' : 'bg-slate-100'}`}>
        <Flame className={`h-4 w-4 ${isActive ? 'text-orange-500' : 'text-slate-400'}`} />
      </div>
      <div className="flex flex-col leading-none">
        <span className={`text-sm font-bold ${isActive ? 'text-orange-700' : 'text-slate-500'}`}>
          {streak.current} {streak.current === 1 ? 'dzień' : 'dni'}
        </span>
        <span className="flex items-center gap-1 text-[10px] text-slate-400 font-medium mt-0.5">
          <Trophy className="h-2.5 w-2.5" />
          Rekord: {streak.longest}
        </span>
      </div>
    </div>
  );
}
