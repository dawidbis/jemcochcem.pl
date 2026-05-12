import { useState } from 'react';
import type  {User}  from '../types';

export function LoginForm({ onLogin }: { onLogin: (u: User) => void }) {
  const [isRegistering, setIsRegistering] = useState(false);
  const [formData, setFormData] = useState({ email: '', password: '', weight: 75, height: 180, age: 30, gender: 'Male' });
  const [msg, setMsg] = useState({ text: '', isError: false });

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    const endpoint = isRegistering ? '/api/users/register' : '/api/users/login';
    const payload = isRegistering ? formData : { email: formData.email, password: formData.password };

    try {
      const res = await fetch(endpoint, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
      if (res.ok) {
        if (isRegistering) { setMsg({ text: 'Utworzono.', isError: false }); setIsRegistering(false); } 
        else { onLogin(await res.json()); }
      } else setMsg({ text: 'Błąd operacji.', isError: true });
    } catch { setMsg({ text: 'Błąd sieci.', isError: true }); }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '2rem auto', padding: '1rem', border: '1px solid #ccc' }}>
      <h2>{isRegistering ? 'Rejestracja' : 'Logowanie'}</h2>
      <form onSubmit={submit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
        <input type="email" placeholder="Email" onChange={e => setFormData({...formData, email: e.target.value})} required />
        <input type="password" placeholder="Hasło" onChange={e => setFormData({...formData, password: e.target.value})} required />
        {isRegistering && (
          <>
            <input type="number" placeholder="Waga" onChange={e => setFormData({...formData, weight: Number(e.target.value)})} />
            <input type="number" placeholder="Wzrost" onChange={e => setFormData({...formData, height: Number(e.target.value)})} />
            <input type="number" placeholder="Wiek" onChange={e => setFormData({...formData, age: Number(e.target.value)})} />
            <select onChange={e => setFormData({...formData, gender: e.target.value})}>
              <option value="Male">M</option><option value="Female">K</option>
            </select>
          </>
        )}
        <button type="submit">{isRegistering ? 'Rejestruj' : 'Loguj'}</button>
      </form>
      <button onClick={() => setIsRegistering(!isRegistering)}>{isRegistering ? 'Zaloguj' : 'Załóż konto'}</button>
      {msg.text && <p style={{ color: msg.isError ? 'red' : 'green' }}>{msg.text}</p>}
    </div>
  );
}