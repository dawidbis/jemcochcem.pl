import React, { useState, useEffect } from 'react';
import { api } from '../api';
import type { WaterStatusDto } from '../types';
import { Button } from "#components/ui/button";

interface WaterTrackerProps {
  userId: string;
  date: string;
}

export const WaterTracker: React.FC<WaterTrackerProps> = ({ userId, date }) => {
  const [status, setStatus] = useState<WaterStatusDto | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  const loadWaterData = async () => {
    setLoading(true);
    const data = await api.getWaterStatus(userId, date);
    if (data) setStatus(data);
    setLoading(false);
  };

  useEffect(() => {
    loadWaterData();
  }, [date, userId]);

  const handleLogWater = async (amountMl: number) => {
    await api.logWater({ userId, date, amountMl });
    await loadWaterData();
  };

  if (loading) return <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm h-[134px] flex items-center justify-center text-xs text-slate-400">Ładowanie wody...</div>;
  if (!status) return null;

  const percentage = Math.min(Math.round((status.currentAmountMl / status.targetAmountMl) * 100), 100);

  return (
    <div className="bg-white rounded-2xl border border-slate-200 p-5 shadow-sm flex flex-col justify-between relative overflow-hidden">
      
      {/* Tytuł i ewentualny alert o wysokim białku jako pulsująca kropka */}
      <div className="flex justify-between items-center">
        <span className="text-xs text-blue-600 font-medium uppercase tracking-wider flex items-center gap-1.5">
          💧 Woda {status.isExtraHydrationRequired && <span className="h-2 w-2 rounded-full bg-amber-500 animate-pulse" title={status.alertMessage} />}
        </span>
        
        {/* Mały przycisk resetu/cofania */}
        {status.currentAmountMl > 0 && (
          <Button 
            onClick={() => handleLogWater(-250)}
            className="text-[10px] text-slate-400 hover:text-red-500 font-medium transition-colors"
            title="Odejmij 250ml"
          >
            Cofnij (-250ml)
          </Button>
        )}
      </div>

      {/* Wartości numeryczne */}
      <p className="text-2xl font-bold text-slate-900 mt-1">
        {status.currentAmountMl}
        <span className="text-base font-normal text-slate-400"> / {status.targetAmountMl}ml</span>
      </p>

      {/* Przyciski szybkiego dodawania wewnątrz kafelka */}
      <div className="flex gap-1.5 mt-2 z-10">
        <Button
          onClick={() => handleLogWater(250)}
          className="text-[11px] font-semibold bg-blue-50 text-blue-600 hover:bg-blue-100 px-2 py-1 rounded-md transition-colors border border-blue-100 flex-1"
        >
          +250ml
        </Button>
        <Button                     
          onClick={() => handleLogWater(500)}
          className="text-[11px] font-semibold bg-blue-600 text-white hover:bg-blue-700 px-2 py-1 rounded-md transition-colors flex-1"
        >
          +500ml
        </Button>
      </div>

      {/* Cienki pasek postępu na samym dole kafelka (taki jak przy kaloriach) */}
      <div className="absolute bottom-0 left-0 right-0 h-1.5 bg-slate-100">
        <div 
          className="h-full bg-blue-500 transition-all duration-500" 
          style={{ width: `${percentage}%` }} 
        />
      </div>
    </div>
  );
};