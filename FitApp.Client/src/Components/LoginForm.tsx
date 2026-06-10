import { useState } from 'react';
import type { User } from '../types';
import { api, setTokens } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";

export function LoginForm({ onLogin }: { onLogin: (u: User) => void }) {
  const [isRegistering, setIsRegistering] = useState(false);
  const [formData, setFormData] = useState({ email: '', password: '', weight: 75, height: 180, age: 30, gender: 'Male', multiplier: 1.55 });
  const [msg, setMsg] = useState({ text: '', isError: false });

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = isRegistering ? formData : { email: formData.email, password: formData.password };

    try {
      const res = isRegistering ? await api.register(payload) : await api.login(payload);
      if (res.ok) {
        if (isRegistering) { setMsg({ text: 'Utworzono.', isError: false }); setIsRegistering(false); } 
        else {
          const data = await res.json();
          setTokens(data.accessToken, data.refreshToken, data.userId);
          onLogin(data);
        }
      } else setMsg({ text: 'Błąd operacji.', isError: true });
    } catch { setMsg({ text: 'Błąd sieci.', isError: true }); }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-100 p-4">
      <div className="w-full max-w-md bg-white shadow-xl border border-slate-200 rounded-2xl overflow-hidden flex flex-col">
        <div className="pb-6 pt-10 px-8 text-center">
          <h2 className="text-3xl font-extrabold bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-indigo-600">
            {isRegistering ? 'Rejestracja' : 'Logowanie'}
          </h2>
        </div>
        
        <div className="px-8 pb-8">
          <form onSubmit={submit} className="flex flex-col gap-5">
            <Input className="bg-slate-50 py-6" type="email" placeholder="Email" onChange={e => setFormData({...formData, email: e.target.value})} required />
            <Input className="bg-slate-50 py-6" type="password" placeholder="Hasło" onChange={e => setFormData({...formData, password: e.target.value})} required />
            
            {isRegistering && (
              <div className="grid grid-cols-2 gap-4 animate-in fade-in slide-in-from-top-4 duration-300">
                <Input className="bg-slate-50 h-10" type="number" placeholder="Waga (kg)" onChange={e => setFormData({...formData, weight: Number(e.target.value)})} />
                <Input className="bg-slate-50 h-10" type="number" placeholder="Wzrost (cm)" onChange={e => setFormData({...formData, height: Number(e.target.value)})} />
                <Input className="bg-slate-50 h-10" type="number" placeholder="Wiek" onChange={e => setFormData({...formData, age: Number(e.target.value)})} />
                
                <select 
                  className="bg-slate-50 border border-slate-200 rounded-md px-3 h-10 outline-none focus:ring-2 focus:ring-blue-500 w-full text-sm"
                  value={formData.gender}
                  onChange={e => setFormData({...formData, gender: e.target.value})}
                >
                  <option value="Male">Mężczyzna</option>
                  <option value="Female">Kobieta</option>
                </select>

                <div className="col-span-2">
                  <select 
                    className="bg-slate-50 border border-slate-200 rounded-md px-3 h-10 outline-none focus:ring-2 focus:ring-blue-500 w-full text-sm"
                    value={formData.multiplier}
                    onChange={e => setFormData({...formData, multiplier: Number(e.target.value)})}
                  >
                    <option value={1.2}>Brak ćwiczeń (1.2)</option>
                    <option value={1.375}>Lekka (1.375)</option>
                    <option value={1.55}>Średnia (1.55)</option>
                    <option value={1.725}>Wysoka (1.725)</option>
                    <option value={1.9}>Bardzo wysoka (1.9)</option>
                  </select>
                </div>
              </div>
            )}
            
            <Button type="submit" size="lg" className="w-full mt-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold shadow-md">
              {isRegistering ? 'Zarejestruj się' : 'Zaloguj się'}
            </Button>
          </form>
        </div>
        
        <div className="bg-slate-50 border-t border-slate-100 p-6 flex flex-col gap-3">
          <Button variant="ghost" className="w-full text-slate-500 hover:text-slate-800" onClick={() => setIsRegistering(!isRegistering)}>
            {isRegistering ? 'Masz już konto? Zaloguj' : 'Brak konta? Zarejestruj'}
          </Button>
          {msg.text && (
            <p className={`text-sm font-medium text-center px-4 py-2 rounded-lg w-full ${msg.isError ? 'bg-red-50 text-red-600' : 'bg-green-50 text-green-600'}`}>
              {msg.text}
            </p>
          )}
        </div>
      </div>
    </div>
  );
}