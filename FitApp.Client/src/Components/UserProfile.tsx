import { useState, useEffect } from 'react';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { api } from '../api';

export function UserProfile({ userId }: { userId: string }) {
  const [profile, setProfile] = useState({ weight: 75, height: 180, age: 30, gender: 'Male' });
  const [multiplier, setMultiplier] = useState(1.55);
  const [status, setStatus] = useState('');

  useEffect(() => {
    api.getUserProfile(userId).then(data => { if (data) setProfile(data); });
  }, [userId]);

  const handleSave = async () => {
    setStatus('Zapisywanie...');
    const res = await api.updateUserProfile(userId, profile);
    await api.calculateMacros();
    if (res.ok) setStatus('Profil i zapotrzebowanie zaktualizowane!');
  };

  return (
    <div className="max-w-4xl mx-auto">
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
        <div className="p-8 border-b border-slate-100 bg-slate-50/50">
          <h2 className="text-2xl font-bold text-slate-800">Ustawienia Profilu i TDEE</h2>
          <p className="text-slate-500 mt-1">Uzupełnij dane, abyśmy mogli wyliczyć Twoje zapotrzebowanie kaloryczne.</p>
        </div>
        
        <div className="p-8 space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-600 ml-1">Waga (kg)</label>
              <Input type="number" value={profile.weight} onChange={e => setProfile({...profile, weight: Number(e.target.value)})} className="py-6 text-lg" />
            </div>
            
            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-600 ml-1">Wzrost (cm)</label>
              <Input type="number" value={profile.height} onChange={e => setProfile({...profile, height: Number(e.target.value)})} className="py-6 text-lg" />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-600 ml-1">Wiek</label>
              <Input type="number" value={profile.age} onChange={e => setProfile({...profile, age: Number(e.target.value)})} className="py-6 text-lg" />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-slate-600 ml-1">Płeć</label>
              <select 
                className="w-full border border-slate-200 rounded-md px-4 py-3 bg-white text-base focus:ring-2 focus:ring-blue-500 outline-none" 
                value={profile.gender} 
                onChange={e => setProfile({...profile, gender: e.target.value})}
              >
                <option value="Male">Mężczyzna</option>
                <option value="Female">Kobieta</option>
              </select>
            </div>

            <div className="space-y-2 md:col-span-2">
              <label className="text-sm font-semibold text-slate-600 ml-1">Poziom aktywności fizycznej</label>
              <select 
                className="w-full border border-slate-200 rounded-md px-4 py-3 bg-white text-base focus:ring-2 focus:ring-blue-500 outline-none" 
                value={multiplier} 
                onChange={e => setMultiplier(Number(e.target.value))}
              >
                <option value={1.2}>Brak ćwiczeń (Praca siedząca)</option>
                <option value={1.375}>Lekka aktywność (1-2 treningi / tydzień)</option>
                <option value={1.55}>Średnia aktywność (3-4 treningi / tydzień)</option>
                <option value={1.725}>Wysoka aktywność (Codzienne treningi)</option>
                <option value={1.9}>Bardzo wysoka (Sportowiec / Praca fizyczna)</option>
              </select>
            </div>
          </div>

          <div className="pt-4 flex items-center justify-between">
            <p className="text-sm font-medium text-blue-600">{status}</p>
            <Button onClick={handleSave} className="bg-blue-600 hover:bg-blue-700 px-10 py-6 text-lg">Zapisz i oblicz cel</Button>
          </div>
        </div>
      </div>
    </div>
  );
}