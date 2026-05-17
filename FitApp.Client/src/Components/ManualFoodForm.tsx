import { useState } from 'react';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "#components/ui/card";

export function ManualFoodForm({ onAdded }: { onAdded: () => void }) {
  const [form, setForm] = useState({ name: '', caloriesPer100g: 0, proteinPer100g: 0, carbsPer100g: 0, fatPer100g: 0 });
  const [status, setStatus] = useState('');

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = { ...form, barcode: '' }; 
    const res = await api.saveFood(payload);
    
    if (res.ok) {
      setStatus('Zapisano pomyślnie!');
      setForm({ name: '', caloriesPer100g: 0, proteinPer100g: 0, carbsPer100g: 0, fatPer100g: 0 });
      setTimeout(() => setStatus(''), 3000);
      onAdded(); 
    } else setStatus('Błąd zapisu.');
  };

  return (
    <Card className="shadow-sm border-slate-100">
      <CardHeader className="pb-4">
        <CardTitle className="text-lg font-semibold text-slate-700">Dodaj produkt ręcznie</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={submit} className="grid grid-cols-2 sm:grid-cols-4 gap-4">
          <Input className="col-span-2 sm:col-span-4 bg-slate-50 focus-visible:ring-blue-500" placeholder="Nazwa produktu" value={form.name} onChange={e => setForm({...form, name: e.target.value})} required />
          
          <div className="space-y-1">
            <span className="text-xs font-semibold text-slate-500 ml-1">Kcal / 100g</span>
            <Input type="number" className="bg-slate-50 focus-visible:ring-slate-500" placeholder="Kcal" value={form.caloriesPer100g || ''} onChange={e => setForm({...form, caloriesPer100g: Number(e.target.value)})} required />
          </div>
          
          <div className="space-y-1">
            <span className="text-xs font-semibold text-blue-600 ml-1">Białko (g)</span>
            <Input type="number" step="0.1" className="bg-blue-50/50 focus-visible:ring-blue-500" placeholder="Białko" value={form.proteinPer100g || ''} onChange={e => setForm({...form, proteinPer100g: Number(e.target.value)})} required />
          </div>
          
          <div className="space-y-1">
            <span className="text-xs font-semibold text-amber-600 ml-1">Węgle (g)</span>
            <Input type="number" step="0.1" className="bg-amber-50/50 focus-visible:ring-amber-500" placeholder="Węgle" value={form.carbsPer100g || ''} onChange={e => setForm({...form, carbsPer100g: Number(e.target.value)})} required />
          </div>
          
          <div className="space-y-1">
            <span className="text-xs font-semibold text-rose-600 ml-1">Tłuszcz (g)</span>
            <Input type="number" step="0.1" className="bg-rose-50/50 focus-visible:ring-rose-500" placeholder="Tłuszcz" value={form.fatPer100g || ''} onChange={e => setForm({...form, fatPer100g: Number(e.target.value)})} required />
          </div>
          
          <Button type="submit" className="col-span-2 sm:col-span-4 mt-2 bg-slate-800 hover:bg-slate-700">Dodaj do bazy</Button>
        </form>
        {status && <p className={`text-sm mt-4 font-medium px-3 py-2 rounded-lg text-center ${status.includes('Błąd') ? 'bg-red-50 text-red-600' : 'bg-green-50 text-green-600'}`}>{status}</p>}
      </CardContent>
    </Card>
  );
}