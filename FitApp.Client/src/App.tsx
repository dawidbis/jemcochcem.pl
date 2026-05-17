import { useState } from 'react';
import type { User } from './types';
import { LoginForm } from './components/LoginForm';
import { FoodDiary } from './components/FoodDiary';
import './App.css';
import { AppLayout } from '#components/AppLayout';
export default function App() {
  const [user, setUser] = useState<User | null>(null);
  const handleLogout = () => setUser(null);

  if (!user) return <LoginForm onLogin={setUser} />;
  return(
    <AppLayout onLogout={handleLogout}> 
  <FoodDiary user={user} onLogout={() => setUser(null)} />
    </AppLayout>
)
}