using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitApp.Domain.Services;

namespace FitApp.Application.Features.Diet
{
    // Definicja zapytania CQRS
    public class GetAiMealQuery : IRequest<List<AiGeneratedItemDto>>
    {
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
    }

    // Handler, który zostanie automatycznie wywołany przez MediatR
    public class GetAiMealQueryHandler : IRequestHandler<GetAiMealQuery, List<AiGeneratedItemDto>>
    {
        private readonly IAiMealService _aiMealService;

        public GetAiMealQueryHandler(IAiMealService aiMealService)
        {
            _aiMealService = aiMealService;
        }

        public async Task<List<AiGeneratedItemDto>> Handle(GetAiMealQuery request, CancellationToken cancellationToken)
        {
            return await _aiMealService.GenerateMealAsync(request.Protein, request.Carbs, request.Fat);
        }
    }
}