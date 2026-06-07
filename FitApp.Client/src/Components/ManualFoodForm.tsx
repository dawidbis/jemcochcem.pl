import { useState } from 'react';
import { api } from '../api';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";

const EMPTY_FORM = {
  name: '',
  caloriesPer100g: 0,
  proteinPer100g: 0,
  carbsPer100g: 0,
  fatPer100g: 0,
  // Mikroskładniki (na 100g)
  fiberPer100g: 0,
  sugarsPer100g: 0,
  saturatedFatPer100g: 0,
  sodiumPer100g: 0,
  calciumPer100g: 0,
  ironPer100g: 0,
};

export function ManualFoodForm({ onAdded }: { onAdded: () => void }) {
  const [form, setForm] = useState({ ...EMPTY_FORM });
  const [status, setStatus] = useState('');

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    const payload = { ...form, barcode: '' };
    const res = await api.saveFood(payload);

    if (res.ok) {
      setStatus('Zapisano pomyślnie!');
      setForm({ ...EMPTY_FORM });
      setTimeout(() => setStatus(''), 3000);
      onAdded();
    } else setStatus('Błąd zapisu.');
  };

  return (
    <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
      <div className="p-4 border-b border-slate-100 bg-slate-50/50">
        <h3 className="text-lg font-semibold text-slate-700">Dodaj produkt ręcznie</h3>
      </div>
      <div className="p-4">
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

          <div className="col-span-2 sm:col-span-4 pt-2 mt-1 border-t border-slate-100">
            <span className="text-xs font-semibold text-emerald-700 uppercase tracking-wider">Mikroskładniki (na 100g) — opcjonalne</span>
          </div>

          <div className="space-y-1">
            <span className="text-xs font-semibold text-emerald-600 ml-1">Błonnik (g)</span>
            <Input type="number" step="0.1" className="bg-emerald-50/50 focus-visible:ring-emerald-500" placeholder="Błonnik" value={form.fiberPer100g || ''} onChange={e => setForm({...form, fiberPer100g: Number(e.target.value)})} />
          </div>

          <div className="space-y-1">
            <span className="text-xs font-semibold text-pink-600 ml-1">Cukry (g)</span>
            <Input type="number" step="0.1" className="bg-pink-50/50 focus-visible:ring-pink-500" placeholder="Cukry" value={form.sugarsPer100g || ''} onChange={e => setForm({...form, sugarsPer100g: Number(e.target.value)})} />
          </div>

          <div className="space-y-1">
            <span className="text-xs font-semibold text-orange-600 ml-1">Tł. nasyc. (g)</span>
            <Input type="number" step="0.1" className="bg-orange-50/50 focus-visible:ring-orange-500" placeholder="Nasycone" value={form.saturatedFatPer100g || ''} onChange={e => setForm({...form, saturatedFatPer100g: Number(e.target.value)})} />
          </div>

          <div className="space-y-1">
            <span className="text-xs font-semibold text-violet-600 ml-1">Sód (mg)</span>
            <Input type="number" step="0.1" className="bg-violet-50/50 focus-visible:ring-violet-500" placeholder="Sód" value={form.sodiumPer100g || ''} onChange={e => setForm({...form, sodiumPer100g: Number(e.target.value)})} />
          </div>

          <div className="space-y-1">
            <span className="text-xs font-semibold text-teal-600 ml-1">Wapń (mg)</span>
            <Input type="number" step="0.1" className="bg-teal-50/50 focus-visible:ring-teal-500" placeholder="Wapń" value={form.calciumPer100g || ''} onChange={e => setForm({...form, calciumPer100g: Number(e.target.value)})} />
          </div>

          <div className="space-y-1">
            <span className="text-xs font-semibold text-red-700 ml-1">Żelazo (mg)</span>
            <Input type="number" step="0.1" className="bg-red-50/50 focus-visible:ring-red-500" placeholder="Żelazo" value={form.ironPer100g || ''} onChange={e => setForm({...form, ironPer100g: Number(e.target.value)})} />
          </div>

          <Button type="submit" className="col-span-2 sm:col-span-4 mt-2 bg-blue-600 hover:bg-blue-700 text-white shadow-sm">Dodaj do bazy</Button>
        </form>
        {status && <p className={`text-sm mt-4 font-medium px-3 py-2 rounded-lg text-center ${status.includes('Błąd') ? 'bg-red-50 text-red-600' : 'bg-green-50 text-green-600'}`}>{status}</p>}
      </div>
    </div>
  );
}