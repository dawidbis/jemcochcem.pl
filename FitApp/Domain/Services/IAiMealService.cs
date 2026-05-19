using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitApp.Domain.Services
{
    public interface IAiMealService
    {
        Task<List<AiGeneratedItemDto>> GenerateMealAsync(double protein, double carbs, double fat);
    }

    public class AiGeneratedItemDto
    {
        public string Name { get; set; } = string.Empty;
        public double WeightInGrams { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }
}