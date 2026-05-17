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
      onAdded(); 
    } else setStatus('Błąd zapisu.');
  };

  return (
    <Card className="mb-6">
      <CardHeader>
        <CardTitle className="text-lg">Dodaj produkt ręcznie</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={submit} className="grid grid-cols-2 gap-3">
          <Input className="col-span-2" placeholder="Nazwa" value={form.name} onChange={e => setForm({...form, name: e.target.value})} required />
          <Input type="number" placeholder="Kcal/100g" value={form.caloriesPer100g || ''} onChange={e => setForm({...form, caloriesPer100g: Number(e.target.value)})} required />
          <Input type="number" step="0.1" placeholder="Białko/100g" value={form.proteinPer100g || ''} onChange={e => setForm({...form, proteinPer100g: Number(e.target.value)})} required />
          <Input type="number" step="0.1" placeholder="Węgle/100g" value={form.carbsPer100g || ''} onChange={e => setForm({...form, carbsPer100g: Number(e.target.value)})} required />
          <Input type="number" step="0.1" placeholder="Tłuszcz/100g" value={form.fatPer100g || ''} onChange={e => setForm({...form, fatPer100g: Number(e.target.value)})} required />
          <Button type="submit" className="col-span-2 mt-2">Dodaj do bazy</Button>
        </form>
        {status && <p className="text-sm mt-2 text-slate-500">{status}</p>}
      </CardContent>
    </Card>
  );
}