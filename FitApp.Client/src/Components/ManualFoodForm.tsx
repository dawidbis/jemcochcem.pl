import { useState } from 'react';
import { api } from '../api';

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
    <section style={{ background: '#eef', padding: '1rem', borderRadius: '8px', marginBottom: '1rem' }}>
      <h3>Dodaj produkt ręcznie</h3>
      <form onSubmit={submit} style={{ display: 'grid', gap: '5px' }}>
        <input placeholder="Nazwa" value={form.name} onChange={e => setForm({...form, name: e.target.value})} required />
        <input type="number" placeholder="Kcal/100g" value={form.caloriesPer100g || ''} onChange={e => setForm({...form, caloriesPer100g: Number(e.target.value)})} required />
        <input type="number" step="0.1" placeholder="Białko/100g" value={form.proteinPer100g || ''} onChange={e => setForm({...form, proteinPer100g: Number(e.target.value)})} required />
        <input type="number" step="0.1" placeholder="Węgle/100g" value={form.carbsPer100g || ''} onChange={e => setForm({...form, carbsPer100g: Number(e.target.value)})} required />
        <input type="number" step="0.1" placeholder="Tłuszcz/100g" value={form.fatPer100g || ''} onChange={e => setForm({...form, fatPer100g: Number(e.target.value)})} required />
        <button type="submit">Dodaj do bazy</button>
      </form>
      <p><small>{status}</small></p>
    </section>
  );
}