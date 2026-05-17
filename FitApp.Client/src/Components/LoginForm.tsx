import { useState } from 'react';
import type { User } from '../types';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "#components/ui/card";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "#components/ui/select";

export function LoginForm({ onLogin }: { onLogin: (u: User) => void }) {
  const [isRegistering, setIsRegistering] = useState(false);
  const [formData, setFormData] = useState({ email: '', password: '', weight: 75, height: 180, age: 30, gender: 'Male' });
  const [msg, setMsg] = useState({ text: '', isError: false });

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = isRegistering ? formData : { email: formData.email, password: formData.password };

    try {
      const res = isRegistering ? await api.register(payload) : await api.login(payload);
      if (res.ok) {
        if (isRegistering) { setMsg({ text: 'Utworzono.', isError: false }); setIsRegistering(false); } 
        else { onLogin(await res.json()); }
      } else setMsg({ text: 'Błąd operacji.', isError: true });
    } catch { setMsg({ text: 'Błąd sieci.', isError: true }); }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gradient-to-br from-slate-100 to-slate-200 p-4">
      <Card className="w-full max-w-md shadow-2xl border-0 rounded-2xl overflow-hidden">
        <CardHeader className="bg-white pb-8 pt-10 text-center">
          <CardTitle className="text-3xl font-extrabold bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-indigo-600">
            {isRegistering ? 'Rejestracja' : 'Logowanie'}
          </CardTitle>
        </CardHeader>
        <CardContent className="bg-white px-8 pb-6">
          <form onSubmit={submit} className="flex flex-col gap-5">
            <Input className="bg-slate-50 py-6" type="email" placeholder="Email" onChange={e => setFormData({...formData, email: e.target.value})} required />
            <Input className="bg-slate-50 py-6" type="password" placeholder="Hasło" onChange={e => setFormData({...formData, password: e.target.value})} required />
            {isRegistering && (
              <div className="grid grid-cols-2 gap-4 animate-in fade-in slide-in-from-top-4 duration-300">
                <Input className="bg-slate-50" type="number" placeholder="Waga (kg)" onChange={e => setFormData({...formData, weight: Number(e.target.value)})} />
                <Input className="bg-slate-50" type="number" placeholder="Wzrost (cm)" onChange={e => setFormData({...formData, height: Number(e.target.value)})} />
                <Input className="bg-slate-50" type="number" placeholder="Wiek" onChange={e => setFormData({...formData, age: Number(e.target.value)})} />
                <Select onValueChange={val => setFormData({...formData, gender: val})} defaultValue={formData.gender}>
                  <SelectTrigger className="bg-slate-50"><SelectValue placeholder="Płeć" /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Male">Mężczyzna</SelectItem>
                    <SelectItem value="Female">Kobieta</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            )}
            <Button type="submit" size="lg" className="w-full mt-2 bg-blue-600 hover:bg-blue-700 text-base font-semibold shadow-md">
              {isRegistering ? 'Zarejestruj się' : 'Zaloguj się'}
            </Button>
          </form>
        </CardContent>
        <CardFooter className="bg-slate-50 border-t border-slate-100 p-6 flex flex-col gap-3">
          <Button variant="ghost" className="w-full text-slate-500 hover:text-slate-800" onClick={() => setIsRegistering(!isRegistering)}>
            {isRegistering ? 'Masz już konto? Zaloguj' : 'Brak konta? Zarejestruj'}
          </Button>
          {msg.text && (
            <p className={`text-sm font-medium text-center px-4 py-2 rounded-lg w-full ${msg.isError ? 'bg-red-50 text-red-600' : 'bg-green-50 text-green-600'}`}>
              {msg.text}
            </p>
          )}
        </CardFooter>
      </Card>
    </div>
  );
}