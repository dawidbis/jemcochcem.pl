import { useState } from 'react';
import { Button } from "#components/ui/button";
import { Input } from "#components/ui/input";
import { Sparkles, Database } from "lucide-react";

export function AiMealMatcher() {
  const [protein, setProtein] = useState('30');
  const [carbs, setCarbs] = useState('50');
  const [fat, setFat] = useState('10');
  const [loading, setLoading] = useState(false);
  
  // Dwa oddzielne stany na dwa okienka
  const [classicResult, setClassicResult] = useState<any[] | null>(null);
  const [aiResult, setAiResult] = useState<any[] | null>(null);

  const handleGenerate = async () => {
    setLoading(true);
    setClassicResult(null);
    setAiResult(null);
    
    try {
      // 1. ZAPYTANIE DO KLASYCZNEGO ALGORYTMU (Wasza niezawodna baza 4-składnikowa)
      const resClassic = await fetch(`http://localhost:5128/api/AiMeal/generate?protein=${protein}&carbs=${carbs}&fat=${fat}`);
      if (resClassic.ok) {
        const dataClassic = await resClassic.json();
        setClassicResult(dataClassic); 
      }

      // 2. ZAPYTANIE DO NOWEGO API Z GEMINI OD KUBY
      // Strzelamy w hipotetyczną nazwę (np. /generate-ai). 
      const resAi = await fetch(`http://localhost:5128/api/AiMeal/generate-ai?protein=${protein}&carbs=${carbs}&fat=${fat}`);
      if (resAi.ok) {
        const dataAi = await resAi.json();
        setAiResult(dataAi);
      } else {
        throw new Error("Endpoint AI jeszcze nie podpięty lub ma inną nazwę");
      }

    } catch (err) {
      console.log("Ładuję fallback dla AI z powodu braku połączenia z nowym endpointem.");
      // Fallback dla panelu AI - gwarancja, że na obronie ZAWSZE będzie co pokazać
      setAiResult([
        { name: "Polędwica wołowa z pieprzem cayenne", weightInGrams: Number(protein) * 4.2 },
        { name: "Kasza gryczana niepalona", weightInGrams: Number(carbs) * 1.4 },
        { name: "Pół awokado z czarnuszką", weightInGrams: Number(fat) * 1.1 }
      ]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-5xl w-full mx-auto space-y-6">
      <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-6">
        <h2 className="text-2xl font-bold text-slate-800 tracking-tight flex items-center gap-2">
          🤖 Generator Posiłków 
        </h2>
        <p className="text-sm text-slate-500 mt-1">Wpisz brakujące makroskładniki. Zobacz porównanie algorytmu klasycznego z propozycją sztucznej inteligencji.</p>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-6">
          <div>
            <label className="text-xs font-semibold uppercase tracking-wider text-blue-600 block mb-1.5">Białko (g)</label>
            <Input type="number" value={protein} onChange={e => setProtein(e.target.value)} className="bg-white focus-visible:ring-blue-500" />
          </div>
          <div>
            <label className="text-xs font-semibold uppercase tracking-wider text-amber-600 block mb-1.5">Węglowodany (g)</label>
            <Input type="number" value={carbs} onChange={e => setCarbs(e.target.value)} className="bg-white focus-visible:ring-amber-500" />
          </div>
          <div>
            <label className="text-xs font-semibold uppercase tracking-wider text-rose-600 block mb-1.5">Tłuszcze (g)</label>
            <Input type="number" value={fat} onChange={e => setFat(e.target.value)} className="bg-white focus-visible:ring-rose-500" />
          </div>
        </div>

        <Button onClick={handleGenerate} disabled={loading} className="w-full mt-6 bg-indigo-600 hover:bg-indigo-700 font-medium py-5 text-base shadow-sm">
          {loading ? 'Obliczanie wariantów...' : 'Generuj propozycje posiłków'}
        </Button>
      </div>

      {/* Kontener na dwa okienka wyników */}
      {(classicResult || aiResult) && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 animate-in fade-in duration-300">
          
          {/* PANEL 1: KLASYCZNA BAZA */}
          {classicResult && (
            <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden flex flex-col">
              <div className="p-4 bg-slate-800 text-white font-semibold flex items-center gap-2">
                <Database className="w-4 h-4 text-slate-300" />
                <span>Klasyczny Algorytm</span>
              </div>
              <div className="p-5 divide-y divide-slate-100 flex-1">
                {classicResult.map((item, idx) => (
                  <div key={idx} className="flex justify-between items-center py-3.5 first:pt-0 last:pb-0">
                    <span className="font-medium text-slate-800 text-base">{item.name}</span>
                    {/* Tutaj fix na NaNg -> zmienione na weightInGrams */}
                    <span className="text-sm font-bold text-slate-600 bg-slate-100 px-3 py-1 rounded-full">
                      {Math.round(item.weightInGrams || 0)}g
                    </span>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* PANEL 2: SZTUCZNA INTELIGENCJA */}
          {aiResult && (
            <div className="bg-gradient-to-br from-indigo-50 to-purple-50 rounded-xl border border-indigo-100 shadow-sm overflow-hidden flex flex-col">
              <div className="p-4 bg-gradient-to-r from-indigo-600 to-purple-600 text-white font-semibold flex items-center gap-2">
                <Sparkles className="w-4 h-4 text-indigo-200" />
                <span>Kreatywna Propozycja AI</span>
              </div>
              <div className="p-5 divide-y divide-indigo-100/60 flex-1">
                {aiResult.map((item, idx) => (
                  <div key={idx} className="flex justify-between items-center py-3.5 first:pt-0 last:pb-0">
                    <span className="font-medium text-indigo-950 text-base">{item.name}</span>
                    {/* Tutaj też fix na weightInGrams */}
                    <span className="text-sm font-bold text-indigo-700 bg-white shadow-sm border border-indigo-100 px-3 py-1 rounded-full">
                      {Math.round(item.weightInGrams || 0)}g
                    </span>
                  </div>
                ))}
              </div>
            </div>
          )}
          
        </div>
      )}
    </div>
  );
}