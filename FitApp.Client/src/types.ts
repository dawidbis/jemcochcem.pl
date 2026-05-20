export interface User { userId: string; userEmail: string; }
export interface MacroNutrients { protein: number; carbs: number; fats: number; }
export interface Food { id: string; name: string; caloriesPer100g: number; macros: MacroNutrients | null; }
export interface MealItem { id: string; foodName: string; grams: number; calories: number; macros: MacroNutrients; }
export interface DiarySummary { date: string; totalCalories: number; totalProtein: number; totalCarbs: number; totalFats: number; items: MealItem[]; }
export interface ExternalFood { name: string; barcode: string; caloriesPer100g: number; macros: MacroNutrients; }

export interface Measurement {
  id: string;
  date: string;
  weight: number;
  bodyFatPercentage?: number | null;
  waist?: number | null;
  hips?: number | null;
  notes?: string | null;
  bmi?: number | null;
}

export interface MeasurementStats {
  latestWeight?: number | null;
  previousWeight?: number | null;
  weightChange?: number | null;
  currentBmi?: number | null;
  bmiCategory?: string | null;
  targetWeight?: number | null;
  progressPercent?: number | null;
  totalMeasurements: number;
}

export interface CreateMeasurementPayload {
  userId: string;
  weight: number;
  date: string;
  bodyFatPercentage?: number | null;
  waist?: number | null;
  hips?: number | null;
  notes?: string | null;
}

export interface AiMealPlanItem {
  id: string;
  mealType: string;
  productName: string;
  grams: number;
  calories: number;
  proteins: number;
  carbs: number; // Zmienione z carbohydrates!
  fats: number;
  foodProductId?: string | null;
}

export interface AiMealPlan {
  id: string;
  prompt: string;
  createdAt: string;
  items: AiMealPlanItem[];
}
