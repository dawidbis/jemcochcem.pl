import { useState } from 'react';
import type { User } from './types';
import { LoginForm } from './Components/LoginForm';
import { FoodDiary } from './Components/FoodDiary';
import './App.css';

export default function App() {
  const [user, setUser] = useState<User | null>(null);

  if (!user) return <LoginForm onLogin={setUser} />;
  return <FoodDiary user={user} onLogout={() => setUser(null)} />;
}