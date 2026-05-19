import { useState } from 'react';
import type { User } from './types';
import { LoginForm } from './components/LoginForm';
import { FoodDiary } from './components/FoodDiary';
import { UserProfile } from './components/UserProfile';
import { AppLayout } from './components/AppLayout';

export default function App() {
  const [user, setUser] = useState<User | null>(null);
  const [view, setView] = useState<'diary' | 'profile'>('diary');

  const handleLogout = () => setUser(null);

  if (!user) return <LoginForm onLogin={setUser} />;
  
  return(
    <AppLayout onLogout={handleLogout} currentView={view} setView={setView}> 
      {view === 'diary' ? <FoodDiary user={user} /> : <UserProfile userId={user.userId} />}
    </AppLayout>
  );
}