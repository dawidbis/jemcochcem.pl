import { useState, useEffect } from 'react';
import type { User, DiarySummary, Food, ExternalFood } from '../types';
import { ManualFoodForm } from './ManualFoodForm';

export function FoodDiary({ user, onLogout }: { user: User, onLogout: () => void }) {
  const [diary, setDiary] = useState<DiarySummary | null>(null);
  const [search, setSearch] = useState('');
  const [results, setResults] = useState<Food[]>([]);
  const [barcode, setBarcode] = useState('');
  const [externalFood, setExternalFood] = useState<ExternalFood | null>(null);
  const [grams, setGrams] = useState(100);
  const TODAY = new Date().toISOString().split('T')[0];

  const loadDiary = async () => {
    const res = await fetch(`/api/diary/${TODAY}?userId=${user.userId}`);
    if (res.ok) setDiary(await res.json());
  };

  useEffect(() => { loadDiary(); }, []);

  useEffect(() => {
    if (!search.trim()) { setResults([]); return; }
    const timer = setTimeout(async () => {
      const res = await fetch(`/api/foods/search?query=${search}`);
      if (res.ok) setResults(await res.json());
    }, 300);
    return () => clearTimeout(timer);
  }, [search]);

  const fetchExternal = async () => {
    const res = await fetch(`/api/foods/external/${barcode}`);
    if (res.ok) setExternalFood(await res.json());
  };

  const saveExternal = async () => {
    if (!externalFood) return;
    const payload = { name: externalFood.name, barcode, caloriesPer100g: externalFood.caloriesPer100g, proteinPer100g: externalFood.macros?.protein || 0, carbsPer100g: externalFood.macros?.carbs || 0, fatPer100g: externalFood.macros?.fats || 0 };
    await fetch('/api/foods', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
    setExternalFood(null); setBarcode('');
  };

  const addMeal = async (foodId: string) => {
    await fetch('/api/diary/items', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ userId: user.userId, date: new Date().toISOString(), foodProductId: foodId, grams }) });
    loadDiary(); setResults([]); setSearch('');
  };

  const deleteMeal = async (itemId: string) => {
    await fetch(`/api/diary/${TODAY}/items/${itemId}?userId=${user.userId}`, { method: 'DELETE' });
    loadDiary();
  };

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '1rem' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid #ddd' }}>
        <h1>FitApp</h1><button onClick={onLogout}>Wyloguj</button>
      </header>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '2rem', marginTop: '1rem' }}>
        <div>
          <ManualFoodForm onAdded={() => setSearch('')} />

          <section style={{ background: '#f9f9f9', padding: '1rem', borderRadius: '8px', marginBottom: '1rem' }}>
            <h3>Skaner OFF</h3>
            <input value={barcode} onChange={e => setBarcode(e.target.value)} /> <button onClick={fetchExternal}>Szukaj</button>
            {externalFood && (
              <div style={{ padding: '10px', background: '#fff' }}>
                <strong>{externalFood.name}</strong>
                <button onClick={saveExternal}>Zapisz</button>
              </div>
            )}
          </section>

          <section>
            <h3>Wyszukiwarka bazy</h3>
            <input value={search} onChange={e => setSearch(e.target.value)} placeholder="Zacznij pisać..." style={{ width: '100%' }} />
            <ul>
              {results.map(f => (
                <li key={f.id}>
                  <strong>{f.name}</strong> ({f.caloriesPer100g} kcal) <br/>
                  <small>B: {f.macros?.protein || 0} W: {f.macros?.carbs || 0} T: {f.macros?.fats || 0}</small>
                  <div>
                    <input type="number" style={{width: '50px'}} value={grams} onChange={e => setGrams(Number(e.target.value))} /> g
                    <button onClick={() => addMeal(f.id)}>Dodaj</button>
                  </div>
                </li>
              ))}
            </ul>
          </section>
        </div>

        <div>
          <h2>Suma: {diary?.totalCalories || 0} kcal</h2>
          <p>B: {diary?.totalProtein || 0}g | W: {diary?.totalCarbs || 0}g | T: {diary?.totalFats || 0}g</p>
          <hr />
          {diary?.items.map(item => (
            <div key={item.id} style={{ display: 'flex', justifyContent: 'space-between', padding: '5px 0' }}>
              <div>
                <strong>{item.foodName}</strong> ({item.grams}g) <br/>
                <small>{item.calories} kcal | B:{item.macros?.protein} W:{item.macros?.carbs} T:{item.macros?.fats}</small>
              </div>
              <button onClick={() => deleteMeal(item.id)}>✖</button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}