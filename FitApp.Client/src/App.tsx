import { useState } from 'react';
import type { User } from './types';
import { LoginForm } from './components/LoginForm';
import { FoodDiary } from './components/FoodDiary';
import { BodyMeasurements } from './components/BodyMeasurements';
import './App.css';
import { AppLayout } from '#components/AppLayout';

type Page = 'diary' | 'measurements';

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
      {page === 'diary' && <FoodDiary user={user} onLogout={handleLogout} />}
      {page === 'measurements' && <BodyMeasurements user={user} />}
    </AppLayout>
  );
}
