using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FitApp.Application.Features.Diet; // upewnij się, że ten namespace pasuje do struktur aplikacji

namespace FitApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiMealController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AiMealController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateMeal([FromQuery] double protein, [FromQuery] double carbs, [FromQuery] double fat)
        {
            // Tworzymy zapytanie CQRS, które przekażemy do MediatR
            var query = new GetAiMealQuery 
            { 
                Protein = protein, 
                Carbs = carbs, 
                Fat = fat 
            };
            
            // MediatR znajdzie odpowiedni Handler w warstwie Application
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
    }
}