export interface User { userId: string; userEmail: string; }
export interface MacroNutrients { protein: number; carbs: number; fats: number; }
export interface Food { id: string; name: string; caloriesPer100g: number; macros: MacroNutrients | null; }
export interface MealItem { id: string; foodName: string; grams: number; calories: number; macros: MacroNutrients; }
export interface DiarySummary { date: string; totalCalories: number; totalProtein: number; totalCarbs: number; totalFats: number; items: MealItem[]; }
export interface ExternalFood { name: string; barcode: string; caloriesPer100g: number; macros: MacroNutrients; }