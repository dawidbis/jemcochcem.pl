export interface User { userId: string; userEmail: string; }
export interface MacroNutrients { protein: number; carbs: number; fats: number; }
// Mikroskładniki: błonnik/cukry/tł. nasycone w g, sód/wapń/żelazo w mg
export interface MicroNutrients { fiber: number; sugars: number; saturatedFat: number; sodium: number; calcium: number; iron: number; }
export interface Food { id: string; name: string; caloriesPer100g: number; macros: MacroNutrients | null; micros: MicroNutrients | null; }
export interface MealItem { id: string; foodName: string; grams: number; calories: number; macros: MacroNutrients; micros: MicroNutrients; }
export interface DiarySummary {
  date: string;
  totalCalories: number;
  totalProtein: number;
  totalCarbs: number;
  totalFats: number;
  totalFiber: number;
  totalSugars: number;
  totalSaturatedFat: number;
  totalSodium: number;
  totalCalcium: number;
  totalIron: number;
  items: MealItem[];
}
export interface ExternalFood { name: string; barcode: string; caloriesPer100g: number; macros: MacroNutrients; micros: MicroNutrients | null; }
// Dzienne cele mikroskładników (z backendu)
export interface TargetMicros { fiber: number; sugars: number; saturatedFat: number; sodium: number; calcium: number; iron: number; }

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

export interface WaterStatusDto {
  currentAmountMl: number;
  targetAmountMl: number;
  isExtraHydrationRequired: boolean;
  alertMessage: string;
}
export interface Exercise {
  id: string;
  name: string;
  muscleGroup: string;
  isCustom: boolean;
}

export interface LoggedSet {
  exerciseId: string;
  setNumber: number;
  weight: number;
  reps: number;
}

export interface WorkoutSet {
  exerciseId: string;
  exerciseName: string;
  muscleGroup: string;
  setNumber: number;
  weight: number;
  reps: number;
}

export interface WorkoutSession {
  id: string;
  date: string;
  notes?: string | null;
  durationMinutes?: number | null;
  totalSets: number;
  totalVolume: number;
  sets: WorkoutSet[];
}

export interface LogWorkoutPayload {
  date: string;
  notes?: string | null;
  durationMinutes?: number | null;
  sets: LoggedSet[];
}

export interface ProgressionPoint {
  date: string;
  maxWeight: number;
  bestReps: number;
  estOneRepMax: number;
  totalVolume: number;
}

export interface ProgressionSuggestion {
  suggestedWeight: number;
  suggestedReps: number;
  message: string;
}

export interface ExerciseProgression {
  exerciseId: string;
  exerciseName: string;
  history: ProgressionPoint[];
  suggestion: ProgressionSuggestion | null;
}