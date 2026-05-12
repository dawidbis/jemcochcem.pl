using FitApp.Application.Features.Diet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FitApp.Application.Features.Foods;

namespace FitApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoodsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Wyszukuje produkty po nazwie lub kodzie kreskowym.
        /// Jeśli nie podasz parametru 'query', zwróci wszystkie produkty z bazy.
        /// Użycie: GET /api/foods/search?query=jabłko
        /// </summary>
        /// <param name="query">Opcjonalna nazwa lub kod kreskowy do wyszukania</param>
        /// <returns>Lista produktów (FoodDto)</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchFoods([FromQuery] string? query)
        {
            // Tworzymy zapytanie i wysyłamy je przez MediatR
            var request = new SearchFoodsQuery 
            { 
                // Jeśli query jest null, przypisujemy pusty string, żeby uniknąć błędów
                SearchTerm = query ?? string.Empty 
            };
            
            var result = await _mediator.Send(request);
            
            return Ok(result); // Zwraca HTTP 200 z listą w formacie JSON
        }

        /// <summary>
        /// Pobiera szczegóły konkretnego produktu na podstawie jego unikalnego ID.
        /// Użycie: GET /api/foods/{id}
        /// </summary>
        /// <param name="id">Identyfikator Guid produktu</param>
        /// <returns>Szczegóły produktu (FoodDto)</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodById([FromRoute] Guid id)
        {
            var request = new GetFoodByIdQuery { Id = id };
            
            var result = await _mediator.Send(request);

            if (result == null)
            {
                // Jeśli MediatR zwrócił null, produkt nie istnieje w bazie
                return NotFound(new { message = $"Nie znaleziono produktu o ID: {id}" });
            }

            return Ok(result);
        }
        /// <summary>
        /// Dodaje nowy produkt do lokalnej bazy danych.
        /// Użycie: POST /api/foods
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateFood([FromBody] CreateFoodCommand command)
        {
            // Wysyłamy komendę do MediatR, dostajemy z powrotem ID nowo utworzonego produktu
            var newFoodId = await _mediator.Send(command);
            
            // Zwracamy status 201 Created wraz z utworzonym ID
            return CreatedAtAction(nameof(GetFoodById), new { id = newFoodId }, new { Id = newFoodId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFoodCommand command)
        {
            command.Id = id; 
    
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("external/{barcode}")]
        public async Task<IActionResult> GetFromOpenFoodFacts([FromRoute] string barcode)
        {
            var result = await _mediator.Send(new FetchExternalFoodQuery(barcode));

            if (result == null)
            {
                return NotFound(new { Message = $"Nie znaleziono produktu o kodzie {barcode} w zewnętrznej bazie." });
            }

            return Ok(result);
        }
    }
}