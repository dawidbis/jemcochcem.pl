using MediatR;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Services;

namespace FitApp.Application.Features.Diet
{
    // Zapytanie
    public class GetGeminiMealQuery : IRequest<object>
    {
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }

    // Handler
    public class GetGeminiMealQueryHandler : IRequestHandler<GetGeminiMealQuery, object>
    {
        private readonly IGeminiService _geminiService;

        public GetGeminiMealQueryHandler(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        public async Task<object> Handle(GetGeminiMealQuery request, CancellationToken cancellationToken)
        {
            return await _geminiService.GenerateAiMealAsync(request.Protein, request.Carbs, request.Fat);
        }
    }
}