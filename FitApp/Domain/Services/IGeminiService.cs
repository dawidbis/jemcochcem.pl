using System.Threading.Tasks;

namespace FitApp.Domain.Services
{
    public interface IGeminiService
    {
        Task<object> GenerateAiMealAsync(double protein, double carbs, double fat);
    }
}