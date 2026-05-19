using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitApp.Domain.Services
{
    public class AiMealService : IAiMealService
    {
        private static readonly Random _random = new();

        public async Task<List<AiGeneratedItemDto>> GenerateMealAsync(double protein, double carbs, double fat)
        {
            // Udajemy pracę sztucznej inteligencji w tle
            await Task.Delay(800);

            var generatedItems = new List<AiGeneratedItemDto>();
            
            // Losujemy jeden z 4 zestawów posiłków (liczba od 1 do 4)
            int mealChoice = _random.Next(1, 5);

            switch (mealChoice)
            {
                case 1: // OKAZJA 1: KLASYCZNY RYŻ I KURCZAK
                    if (protein > 0) {
                        double w = (protein / 21.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Grillowana pierś z kurczaka", WeightInGrams = Math.Round(w, 1), Protein = Math.Round(protein, 1), Carbs = 0, Fat = Math.Round((w * 1.3) / 100.0, 1), Calories = Math.Round(w, 1) });
                    }
                    if (carbs > 0) {
                        double w = (carbs / 77.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Ryż basmati (sucha masa)", WeightInGrams = Math.Round(w, 1), Protein = Math.Round((w * 3.0) / 100.0, 1), Carbs = Math.Round(carbs, 1), Fat = Math.Round((w * 1.0) / 100.0, 1), Calories = Math.Round((w * 350.0) / 100.0, 1) });
                    }
                    if (fat > 0) {
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Oliwa z oliwek (Extra Virgin)", WeightInGrams = Math.Round(fat, 1), Protein = 0, Carbs = 0, Fat = Math.Round(fat, 1), Calories = Math.Round(fat * 9.0, 1) });
                    }
                    break;

                case 2: // OKAZJA 2: ŚNIADANIE MISTRZÓW (JAJA + AWOKADO + CHLEB)
                    if (protein > 0) {
                        // Jajo ma ok 12g białka na 100g
                        double w = (protein / 12.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Jaja kurze (całe, gotowane)", WeightInGrams = Math.Round(w, 1), Protein = Math.Round(protein, 1), Carbs = Math.Round((w * 1.0) / 100.0, 1), Fat = Math.Round((w * 10.0) / 100.0, 1), Calories = Math.Round((w * 140.0) / 100.0, 1) });
                    }
                    if (carbs > 0) {
                        // Chleb żytni ok 48g węgli w 100g
                        double w = (carbs / 48.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Chleb żytni razowy", WeightInGrams = Math.Round(w, 1), Protein = Math.Round((w * 6.0) / 100.0, 1), Carbs = Math.Round(carbs, 1), Fat = Math.Round((w * 1.5) / 100.0, 1), Calories = Math.Round((w * 240.0) / 100.0, 1) });
                    }
                    if (fat > 0) {
                        // Awokado ok 15g tłuszczu w 100g
                        double w = (fat / 15.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Świeże awokado", WeightInGrams = Math.Round(w, 1), Protein = Math.Round((w * 2.0) / 100.0, 1), Carbs = Math.Round((w * 8.0) / 100.0, 1), Fat = Math.Round(fat, 1), Calories = Math.Round((w * 160.0) / 100.0, 1) });
                    }
                    break;

                case 3: // OKAZJA 3: SZYBKI SZEJK ANABOLICZNY (WPC + BANAN + MASŁO ORZECHOWE)
                    if (protein > 0) {
                        // Białko WPC ok 75g białka w 100g
                        double w = (protein / 75.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Odżywka białkowa (WPC 80)", WeightInGrams = Math.Round(w, 1), Protein = Math.Round(protein, 1), Carbs = Math.Round((w * 8.0) / 100.0, 1), Fat = Math.Round((w * 6.0) / 100.0, 1), Calories = Math.Round((w * 390.0) / 100.0, 1) });
                    }
                    if (carbs > 0) {
                        // Banan ok 22g węgli w 100g
                        double w = (carbs / 22.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Banan (do szejka)", WeightInGrams = Math.Round(w, 1), Protein = Math.Round((w * 1.0) / 100.0, 1), Carbs = Math.Round(carbs, 1), Fat = 0, Calories = Math.Round((w * 89.0) / 100.0, 1) });
                    }
                    if (fat > 0) {
                        // Masło orzechowe ok 50g tłuszczu w 100g
                        double w = (fat / 50.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Masło orzechowe 100%", WeightInGrams = Math.Round(w, 1), Protein = Math.Round((w * 25.0) / 100.0, 1), Carbs = Math.Round((w * 20.0) / 100.0, 1), Fat = Math.Round(fat, 1), Calories = Math.Round((w * 600.0) / 100.0, 1) });
                    }
                    break;

                case 4: // OKAZJA 4: FITNESS OBIAD (ŁOSOŚ + ZIEMNIAKI)
                    if (protein > 0) {
                        // Łosoś ma ok 20g białka na 100g
                        double w = (protein / 20.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Pieczony filet z łososia", WeightInGrams = Math.Round(w, 1), Protein = Math.Round(protein, 1), Carbs = 0, Fat = Math.Round((w * 13.0) / 100.0, 1), Calories = Math.Round((w * 200.0) / 100.0, 1) });
                    }
                    if (carbs > 0) {
                        // Ziemniaki ok 17g węgli w 100g
                        double w = (carbs / 17.0) * 100.0;
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Młode ziemniaki (gotowane)", WeightInGrams = Math.Round(w, 1), Protein = Math.Round((w * 2.0) / 100.0, 1), Carbs = Math.Round(carbs, 1), Fat = 0, Calories = Math.Round((w * 87.0) / 100.0, 1) });
                    }
                    if (fat > 0) {
                        // Jeśli zostanie nam czysty tłuszcz, damy lekki dressing z oleju lnianego
                        generatedItems.Add(new AiGeneratedItemDto { Name = "Olej lniany tłoczony na zimno", WeightInGrams = Math.Round(fat, 1), Protein = 0, Carbs = 0, Fat = Math.Round(fat, 1), Calories = Math.Round(fat * 9.0, 1) });
                    }
                    break;
            }

            return generatedItems;
        }
    }
}