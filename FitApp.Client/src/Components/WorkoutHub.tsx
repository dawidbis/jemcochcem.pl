import { useState } from 'react';
import { Dumbbell, History, TrendingUp, CalendarDays, ListChecks } from 'lucide-react';
import type { User } from '../types';
import { WorkoutLogger } from './WorkoutLogger';
import { WorkoutHistory } from './WorkoutHistory';
import { WorkoutProgressionChart } from './WorkoutProgressionChart';
import { WorkoutCalendar } from './WorkoutCalendar';
import { ExerciseManager } from './ExerciseManager';

type WorkoutTab = 'logger' | 'history' | 'progression' | 'calendar' | 'exercises';

const TABS: { key: WorkoutTab; label: string; icon: React.ReactNode }[] = [
  { key: 'logger',     label: 'Trening',    icon: <Dumbbell className="h-4 w-4" /> },
  { key: 'history',    label: 'Historia',   icon: <History className="h-4 w-4" /> },
  { key: 'progression',label: 'Progresja',  icon: <TrendingUp className="h-4 w-4" /> },
  { key: 'calendar',   label: 'Kalendarz',  icon: <CalendarDays className="h-4 w-4" /> },
  { key: 'exercises',  label: 'Ćwiczenia',  icon: <ListChecks className="h-4 w-4" /> },
];

interface Props { user: User; }

export function WorkoutHub({ user }: Props) {
  const [activeTab, setActiveTab] = useState<WorkoutTab>('logger');

  return (
    <div className="space-y-6">
      {/* Tab bar */}
      <div className="bg-white border border-slate-200 rounded-2xl p-1.5 shadow-sm flex gap-1 overflow-x-auto">
        {TABS.map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`flex items-center gap-2 px-4 py-2.5 rounded-xl text-sm font-medium transition-all whitespace-nowrap flex-1 justify-center
              ${activeTab === tab.key
                ? 'bg-blue-600 text-white shadow-sm'
                : 'text-slate-600 hover:bg-slate-100'
              }`}
          >
            {tab.icon}
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === 'logger'      && <WorkoutLogger userId={user.userId} />}
      {activeTab === 'history'     && <WorkoutHistory userId={user.userId} />}
      {activeTab === 'progression' && <WorkoutProgressionChart />}
      {activeTab === 'calendar'    && <WorkoutCalendar />}
      {activeTab === 'exercises'   && <ExerciseManager />}
    </div>
  );
}
