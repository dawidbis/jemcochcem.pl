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
    <div className="flex items-center justify-center min-h-screen bg-slate-50">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>{isRegistering ? 'Rejestracja' : 'Logowanie'}</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={submit} className="flex flex-col gap-4">
            <Input type="email" placeholder="Email" onChange={e => setFormData({...formData, email: e.target.value})} required />
            <Input type="password" placeholder="Hasło" onChange={e => setFormData({...formData, password: e.target.value})} required />
            {isRegistering && (
              <>
                <Input type="number" placeholder="Waga" onChange={e => setFormData({...formData, weight: Number(e.target.value)})} />
                <Input type="number" placeholder="Wzrost" onChange={e => setFormData({...formData, height: Number(e.target.value)})} />
                <Input type="number" placeholder="Wiek" onChange={e => setFormData({...formData, age: Number(e.target.value)})} />
                <Select onValueChange={val => setFormData({...formData, gender: val})} defaultValue={formData.gender}>
                  <SelectTrigger><SelectValue placeholder="Płeć" /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Male">Mężczyzna</SelectItem>
                    <SelectItem value="Female">Kobieta</SelectItem>
                  </SelectContent>
                </Select>
              </>
            )}
            <Button type="submit" className="w-full">{isRegistering ? 'Rejestruj' : 'Loguj'}</Button>
          </form>
        </CardContent>
        <CardFooter className="flex flex-col gap-2">
          <Button variant="ghost" className="w-full" onClick={() => setIsRegistering(!isRegistering)}>
            {isRegistering ? 'Zaloguj' : 'Załóż konto'}
          </Button>
          {msg.text && <p className={`text-sm ${msg.isError ? 'text-red-500' : 'text-green-500'}`}>{msg.text}</p>}
        </CardFooter>
      </Card>
    </div>
  );
}