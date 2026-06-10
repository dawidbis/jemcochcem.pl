import { useState } from 'react';
import type { User } from './types';
import { clearTokens } from './api';
import { LoginForm } from './Components/LoginForm';
import { FoodDiary } from './Components/FoodDiary';
import { DietCalendar } from './Components/Dietcalendar';
import { BodyMeasurements } from './Components/BodyMeasurements';
import { UserProfile } from './Components/UserProfile';
import { AiMealMatcher } from './Components/AiMealMatcher';
import { WorkoutHub } from './Components/WorkoutHub';
import './App.css';
import { AppLayout } from './Components/AppLayout';

type Page = 'diary' | 'calendar' | 'measurements' | 'profile' | 'ai-matcher' | 'workout';

export default function App() {
  const [user, setUser] = useState<User | null>(null);
  const [page, setPage] = useState<Page>('diary');

  const handleLogout = () => {
    clearTokens();
    setUser(null);
    setPage('diary');
  };

  if (!user) return <LoginForm onLogin={setUser} />;

  return (
    <AppLayout onLogout={handleLogout} activePage={page} onNavigate={setPage}>
      {page === 'diary'       && <FoodDiary user={user} />}
      {page === 'calendar'    && <DietCalendar user={user} />}
      {page === 'measurements'&& <BodyMeasurements user={user} />}
      {page === 'profile'     && <UserProfile userId={user.userId} />}
      {page === 'ai-matcher'  && <AiMealMatcher />}
      {page === 'workout'     && <WorkoutHub user={user} />}
    </AppLayout>
  );
}
