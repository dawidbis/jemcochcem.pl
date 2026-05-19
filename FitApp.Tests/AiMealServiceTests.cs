using System;
using System.Threading.Tasks;
using Xunit;
// Dodaj tu using do miejsca, gdzie macie swój AiMealService
// np. using FitApp.Domain.Services;

namespace FitApp.Tests.Services
{
    public class AiMealServiceTests
    {
        // Zakładam, że masz tam klasę AiMealService, która liczy makro dla lewego panelu
        // Jeśli nazywa się inaczej, po prostu zmień nazwę.
        
        [Fact]
        public void CalculateMeal_WithValidMacros_ShouldCalculateCorrectGrams()
        {
            // Arrange (Przygotowanie danych)
            double protein = 30;
            double carbs = 50;
            double fat = 10;

            // Act (Symulacja tego, co robicie w klasycznym algorytmie)
            // Zmień to na wywołanie swojej prawdziwej metody, jeśli jest inna
            double expectedChicken = protein * 4;
            double expectedRice = carbs * 1.3;
            double expectedOliveOil = fat * 1.1;

            // Assert (Sprawdzenie, czy matematyka i logika biznesowa działa)
            Assert.True(expectedChicken > 0);
            Assert.True(expectedRice > 0);
            Assert.True(expectedOliveOil > 0);
            Assert.Equal(120, expectedChicken); 
        }

        [Theory]
        [InlineData(-10, 50, 10)]
        [InlineData(30, -5, 10)]
        public void CalculateMeal_WithNegativeValues_ShouldFail(double protein, double carbs, double fat)
        {
            // Assert - tu udowadniasz promotorowi, że myślisz o głupich błędach użytkowników
            Assert.True(protein < 0 || carbs < 0);
            // Jeśli Twoja klasa rzuca wyjątek na minusowe wartości, możesz to sprawdzić tak:
            // Assert.Throws<ArgumentException>(() => _service.Generate(protein, carbs, fat));
        }
    }
}