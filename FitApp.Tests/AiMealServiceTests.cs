using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
// UWAGA: Upewnij się, że ten namespace pasuje do lokalizacji DTOs (AiGeneratedItemDto) w Twoim projekcie
using FitApp.Domain.Services; 

namespace FitApp.Tests.Services
{
    // KLASA TESTOWA DLA LOGIKI BIZNESOWEJ POSIŁKÓW (PUNKT 4 SPECYFIKACJI)
    public class AiMealServiceTests
    {
        private readonly AiMealService _service;

        public AiMealServiceTests()
        {
            _service = new AiMealService();
        }
        
        [Fact]
        public async Task GenerateMealAsync_WithValidMacros_ShouldReturnDenselyCalculatedMeal()
        {
            // 1. Arrange
            double proteinGoal = 30;
            double carbsGoal = 50;
            double fatGoal = 10;

            // 2. Act
            // Wywołujemy prawdziwą, asynchroniczną metodę z AiMealService.cs
            List<AiGeneratedItemDto> result = await _service.GenerateMealAsync(proteinGoal, carbsGoal, fatGoal);

            // 3. Assert
            
            // A. Podstawowe sprawdzenie integralności wyniku
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Ponieważ losujemy zestawy, lista może mieć od 1 do 3 składników (np. tylko oliwa, jeśli tylko fat > 0)
            // Dla 30/50/10 zawsze powinny być co najmniej 2 składniki.
            Assert.True(result.Count >= 2, "Posiłek powinien składać się z co najmniej dwóch składników");

            // B. Sprawdzenie poprawności matematycznej (logiki biznesowej)
            // Ponieważ kod używa Random i losuje 1 z 4 zestawów, nie sprawdzamy konkretnych nazw produktów.
            // Sprawdzamy sumę makroskładników w całym wygenerowanym posiłku.

            double totalProtein = result.Sum(item => item.Protein);
            double totalCarbs = result.Sum(item => item.Carbs);
            double totalFat = result.Sum(item => item.Fat);

            // Kod używa Math.Round(w, 1), więc sprawdzamy z dokładnością do 0.1g
            Assert.Equal(proteinGoal, totalProtein, 1);
            Assert.Equal(carbsGoal, totalCarbs, 1);
            Assert.Equal(fatGoal, totalFat, 1);

            // C. Sprawdzenie zaokrągleń gramatury
            foreach (var item in result.Select(x => x.WeightInGrams))
            {
                Assert.True(item > 0, $"Gramatura składnika {item} nie może być zerowa ani ujemna");
                
                // Sprawdzamy, czy waga jest zaokrąglona do maks 1 miejsca po przecinku
                double roundedWeight = Math.Round(item, 1);
                Assert.Equal(roundedWeight, item);
            }
        }

        [Theory]
        [InlineData(0, 50, 10)]  // Brak białka
        [InlineData(30, 0, 10)]  // Brak węgli
        [InlineData(30, 50, 0)]  // Brak tłuszczu
        public async Task GenerateMealAsync_WithZeroMacro_ShouldNotIncludeIngredientsForThatMacro(double p, double c, double f)
        {
            // 1. Act
            List<AiGeneratedItemDto> result = await _service.GenerateMealAsync(p, c, f);

            // 2. Assert
            // Logika biznesowa w AiMealService.cs (if (protein > 0)) gwarantuje, 
            // że składniki są dodawane tylko dla makr > 0.
            
            double totalProtein = result.Sum(item => item.Protein);
            double totalCarbs = result.Sum(item => item.Carbs);
            double totalFat = result.Sum(item => item.Fat);

            Assert.Equal(p, totalProtein, 1);
            Assert.Equal(c, totalCarbs, 1);
            Assert.Equal(f, totalFat, 1);
        }

        [Theory]
        [InlineData(-10, 50, 10)]
        [InlineData(30, -5, 10)]
        public async Task GenerateMealAsync_WithNegativeValues_ShouldHandleThemAsZero(double p, double c, double f)
        {
            // 1. Act
            List<AiGeneratedItemDto> result = await _service.GenerateMealAsync(p, c, f);

            // 2. Assert
            // Twoja logika biznesowa (if (protein > 0)) nie rzuca wyjątku dla ujemnych wartości,
            // ale po prostu je ignoruje. Test udowadnia, że system jest stabilny i
            // traktuje ujemne makro jak 0, nie generując dla nich składników.

            double totalProtein = result.Sum(item => item.Protein);
            double totalCarbs = result.Sum(item => item.Carbs);
            double totalFat = result.Sum(item => item.Fat);

            // Jeśli p = -10, suma białka w posiłku powinna być 0.
            double expectedP = p > 0 ? p : 0;
            double expectedC = c > 0 ? c : 0;
            double expectedF = f > 0 ? f : 0;

            Assert.Equal(expectedP, totalProtein, 1);
            Assert.Equal(expectedC, totalCarbs, 1);
            Assert.Equal(expectedF, totalFat, 1);
        }
    }
}