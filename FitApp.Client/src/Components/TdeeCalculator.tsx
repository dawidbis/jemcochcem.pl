import { useState, useEffect } from 'react';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "#components/ui/card";
import { api } from '../api';

export interface TargetMacros { tdee: number; protein: number; carbs: number; fats: number; }

export function TdeeCalculator({ userId, onCalculated }: { userId: string, onCalculated: (targets: TargetMacros) => void }) {
  const [profile, setProfile] = useState({ weight: 75, height: 180, age: 30, gender: 'Male' });
  const [multiplier, setMultiplier] = useState(1.55);

  useEffect(() => {
    api.getUserProfile(userId).then(data => { if (data) setProfile(data); });
  }, [userId]);

  const calculate = async () => {
    await api.updateUserProfile(userId, profile);
    const data = await api.calculateMacros(userId, multiplier);
    
    if (data) {
      onCalculated({
        tdee: data.tdee || data.targetCalories || 0,
        protein: data.protein || 0,
        carbs: data.carbs || 0,
        fats: data.fats || data.fat || 0
      });
    }
  };

  return (
    <Card className="shadow-sm border-slate-100 mb-6">
      <CardHeader className="pb-3">
        <CardTitle className="text-lg font-semibold text-slate-700">Cel TDEE</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid grid-cols-2 md:grid-cols-5 gap-2">
          <Input type="number" placeholder="Waga" value={profile.weight} onChange={e => setProfile({...profile, weight: Number(e.target.value)})} />
          <Input type="number" placeholder="Wzrost" value={profile.height} onChange={e => setProfile({...profile, height: Number(e.target.value)})} />
          <Input type="number" placeholder="Wiek" value={profile.age} onChange={e => setProfile({...profile, age: Number(e.target.value)})} />
          <select className="border border-slate-200 rounded-md px-2 py-2 bg-slate-50 text-sm focus-visible:ring-blue-500" value={multiplier} onChange={e => setMultiplier(Number(e.target.value))}>
            <option value={1.2}>Brak (1.2)</option>
            <option value={1.375}>Lekka (1.375)</option>
            <option value={1.55}>Średnia (1.55)</option>
            <option value={1.725}>Wysoka (1.725)</option>
          </select>
          <Button className="bg-emerald-600 hover:bg-emerald-700 w-full" onClick={calculate}>Oblicz</Button>
        </div>
      </CardContent>
    </Card>
  );
}