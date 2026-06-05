using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Application.Features.Diet;
using FitApp.Domain.Services;

namespace FitApp.Tests.Diet
{
    public class GetGeminiMealQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Call_GeminiService_With_Correct_Macros()
        {
            // Arrange (Przygotowanie)
            // 1. Tworzymy "fałszywy" (mock) serwis Gemini, żeby nie uderzać do prawdziwego API Google
            var mockGeminiService = new Mock<IGeminiService>();
            
            // 2. Przygotowujemy sztuczną odpowiedź, którą ma zwrócić nasz mock
            var fakeAiResponse = new 
            { 
                mealName = "Testowa Micha Mocy", 
                ingredients = new[] { new { name = "Kurczak", weightInGrams = 200 } },
                preparationSteps = new[] { "Krok 1", "Krok 2" }
            };

            // 3. Konfigurujemy mocka: kiedy ktoś zawoła GenerateAiMealAsync, zwróć naszą sztuczną odpowiedź
            mockGeminiService
                .Setup(s => s.GenerateAiMealAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(fakeAiResponse);

            // 4. Tworzymy nasz prawdziwy Handler, wstrzykując mu "fałszywy" serwis
            var handler = new GetGeminiMealQueryHandler(mockGeminiService.Object);
            
            // 5. Przygotowujemy zapytanie (Query) z przykładowymi makrosami
            var query = new GetGeminiMealQuery { Protein = 40, Carbs = 60, Fat = 15 };

            // Act (Działanie)
            // Uruchamiamy główną metodę Handle
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert (Sprawdzenie wyników)
            // 1. Sprawdzamy, czy wynik w ogóle istnieje
            Assert.NotNull(result);
            
            // 2. Sprawdzamy, czy wynik z Handlera to dokładnie ta sama odpowiedź, którą przygotowaliśmy
            Assert.Equal(fakeAiResponse, result);
            
            // 3. Sprawdzamy, czy Handler poprawnie przekazał makrosy do serwisu (czy wywołał metodę z wartościami 40, 60, 15)
            mockGeminiService.Verify(s => s.GenerateAiMealAsync(40, 60, 15), Times.Once);
        }
    }
}