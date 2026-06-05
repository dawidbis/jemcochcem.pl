using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FitApp.Application.Features.Diet;

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
            var query = new GetAiMealQuery { Protein = protein, Carbs = carbs, Fat = fat };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("generate-ai")]
        public async Task<IActionResult> GenerateMealAi([FromQuery] double protein, [FromQuery] double carbs, [FromQuery] double fat)
        {
            try
            {
                var query = new GetGeminiMealQuery { Protein = protein, Carbs = carbs, Fat = fat };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Błąd serwera: {ex.Message}");
            }
        }
    }
}