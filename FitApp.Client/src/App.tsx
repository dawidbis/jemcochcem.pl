import { useState } from 'react';
import type { User } from './types';
import { LoginForm } from './Components/LoginForm';
import { FoodDiary } from './Components/FoodDiary';
import { DietCalendar } from './components/DietCalendar';
import { BodyMeasurements } from './Components/BodyMeasurements';
import { UserProfile } from './Components/UserProfile';
import { AiMealMatcher } from './Components/AiMealMatcher';
import './App.css';
import { AppLayout } from './Components/AppLayout';

type Page = 'diary' | 'calendar' | 'measurements' | 'profile' | 'ai-matcher';

export default function App() {
  const [user, setUser] = useState<User | null>(null);
  const [page, setPage] = useState<Page>('diary');

  const handleLogout = () => {
    setUser(null);
    setPage('diary');
  };

  if (!user) return <LoginForm onLogin={setUser} />;

  return (
    <AppLayout onLogout={handleLogout} activePage={page} onNavigate={setPage}>
      {page === 'diary' && <FoodDiary user={user} />}
      {page === 'calendar' && <DietCalendar user={user} />}
      {page === 'measurements' && <BodyMeasurements user={user} />}
      {page === 'profile' && <UserProfile userId={user.userId} />}
      {page === 'ai-matcher' && <AiMealMatcher />}
    </AppLayout>
  );
}