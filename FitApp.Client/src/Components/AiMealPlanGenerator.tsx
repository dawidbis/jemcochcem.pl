import { useState } from 'react';
import { api } from '../api';
import type { AiMealPlan, AiMealPlanItem } from '../types';

export function AiMealPlanGenerator({ date, onAdded }: { date: string, onAdded: () => void }) {
  const [prompt, setPrompt] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [currentPlan, setCurrentPlan] = useState<AiMealPlan | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [successMsg, setSuccessMsg] = useState<string | null>(null);

  // Funkcja wysyłająca zapytanie do Claude/Gemini AI
  const handleGeneratePlan = async () => {
    if (!prompt.trim()) return;
    
    setIsLoading(true);
    setError(null);
    setCurrentPlan(null);

    try {
      // 1. ZMIANA: Przekazujemy userId do API!
const res = await api.generateAiMealPlan({ prompt });
      
      if (res && res.mealPlanId) {
        // 2. Pobieramy plany usera i znajdujemy ten nowo wygenerowany
        const plans = await api.getUserMealPlans();
        const newPlan = plans.find(p => p.id === res.mealPlanId);
        
        if (newPlan) {
          setCurrentPlan(newPlan);
        } else {
            setError("Plan został wygenerowany, ale nie można go pobrać.");
        }
      } else {
        setError("Błąd podczas generowania planu z AI.");
      }
    } catch (err) {
      setError("Wystąpił błąd sieci lub serwera.");
    } finally {
      setIsLoading(false);
    }
  };

  // Funkcja przenosząca posiłek do dziennika usera
  const handleAddToDiary = async (itemId: string) => {
    try {
      const res = await api.addAiMealToDiary({
        mealPlanItemId: itemId,
        date: new Date(date).toISOString(),
        // 2. ZMIANA: Przekazujemy userId do zapisania w dzienniku!
      });

      if (res.ok) {
        setSuccessMsg("Dodano do dziennika!");
        onAdded(); // <-- To odświeża listę posiłków!
        setTimeout(() => setSuccessMsg(null), 3000);
      } else {
        setError("Nie udało się dodać do dziennika.");
      }
    } catch (err) {
      setError("Wystąpił błąd przy zapisie do dziennika.");
    }
  };

  return (
    <div className="p-6 bg-white rounded-xl shadow-sm border border-gray-100 max-w-4xl mx-auto mt-6">
      <div className="mb-6">
        <h2 className="text-2xl font-bold text-gray-800 flex items-center gap-2">
          ✨ Wygeneruj dietę z AI
        </h2>
        <p className="text-gray-500 text-sm mt-1">
          Opisz swoje cele, a nasz dietetyk AI wygeneruje dla Ciebie dedykowany plan posiłków.
        </p>
      </div>

      <div className="flex flex-col gap-3 mb-8">
        <textarea
          className="w-full p-4 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none resize-none"
          rows={3}
          placeholder="Np. Chcę dietę ketogeniczną, około 2000 kcal, lubię jajka i awokado..."
          value={prompt}
          onChange={(e) => setPrompt(e.target.value)}
          disabled={isLoading}
        />
        <button
          onClick={handleGeneratePlan}
          disabled={isLoading || !prompt.trim()}
          className="self-end bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-6 rounded-lg transition-colors disabled:opacity-50 flex items-center justify-center min-w-[150px]"
        >
          {isLoading ? (
            <span className="animate-pulse">Generowanie...</span>
          ) : (
            "Wygeneruj Plan"
          )}
        </button>
      </div>

      {error && <div className="p-3 mb-4 bg-red-50 text-red-600 rounded-lg">{error}</div>}
      {successMsg && <div className="p-3 mb-4 bg-green-50 text-green-700 rounded-lg">{successMsg}</div>}

      {/* Wyświetlanie wygenerowanego planu */}
      {currentPlan && currentPlan.items.length > 0 && (
        <div className="animate-in fade-in slide-in-from-bottom-4 duration-500">
          <h3 className="text-lg font-semibold text-gray-800 mb-4 border-b pb-2">
            Twój wygenerowany plan posiłków
          </h3>
          
          <div className="grid gap-4 md:grid-cols-2">
            {currentPlan.items.map((item: AiMealPlanItem) => (
              <div key={item.id} className="p-4 border border-gray-200 rounded-lg bg-gray-50 hover:bg-white transition-colors shadow-sm flex flex-col justify-between h-full">
                <div>
                    <div className="flex justify-between items-start mb-2">
                        <span className="text-xs font-bold uppercase tracking-wider text-blue-600 bg-blue-100 px-2 py-1 rounded">
                            {item.mealType}
                        </span>
                        <span className="text-sm font-semibold text-gray-700 bg-gray-200 px-2 py-1 rounded">
                            {item.calories} kcal
                        </span>
                    </div>
                    <h4 className="font-semibold text-gray-900 text-lg mb-1">{item.productName}</h4>
                    <p className="text-sm text-gray-500 mb-3">{item.grams} g</p>
                    
                    <div className="flex gap-4 text-xs text-gray-600 mb-4 bg-white p-2 rounded border">
                        <div className="flex flex-col"><span>Białko</span><span className="font-semibold">{item.proteins}g</span></div>
                        {/* 3. ZMIANA: item.carbohydrates -> item.carbs */}
                        <div className="flex flex-col"><span>Węgle</span><span className="font-semibold">{item.carbs}g</span></div>
                        <div className="flex flex-col"><span>Tłuszcz</span><span className="font-semibold">{item.fats}g</span></div>
                    </div>
                </div>

                <button
                  onClick={() => handleAddToDiary(item.id)}
                  className="w-full bg-white hover:bg-gray-100 text-gray-800 font-medium py-2 px-4 border border-gray-300 rounded shadow-sm transition-all"
                >
                  Dodaj do dzisiejszego dziennika
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}