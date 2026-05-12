using FitApp.Application.Features.Diet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiaryController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Wstrzykujemy IMediator przez konstruktor
        public DiaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Pobiera podsumowanie dziennika dla danego dnia i użytkownika.
        /// </summary>
        /// <param name="date">Data w formacie YYYY-MM-DD</param>
        /// <param name="userId">Identyfikator użytkownika (Guid)</param>
        /// <returns>Obiekt DiaryDto z listą posiłków i kaloriami</returns>
        [HttpGet("{date}")]
        public async Task<IActionResult> GetDailyDiary([FromRoute] DateTime date, [FromQuery] Guid userId)
        {
            // Budujemy obiekt zapytania
            var query = new GetDailyDiaryQuery 
            { 
                Date = date, 
                UserId = userId 
            };
            
            // Przekazujemy zapytanie do MediatR, który znajdzie GetDailyDiaryHandler
            var result = await _mediator.Send(query);
            
            return Ok(result); // Zwraca HTTP 200 z obiektem JSON (DiaryDto)
        }

        /// <summary>
        /// Dodaje nowy produkt spożywczy (posiłek) do dziennika na dany dzień.
        /// </summary>
        /// <param name="command">Dane z ciałą żądania (body)</param>
        /// <returns>Status HTTP 200 w przypadku sukcesu</returns>
        [HttpPost("items")]
        public async Task<IActionResult> AddMealItem([FromBody] AddMealItemCommand command)
        {
            // Przekazujemy komendę do MediatR, który znajdzie AddMealItemHandler
            await _mediator.Send(command);
            
            // Ponieważ handler zwraca Unit (odpowiednik void), 
            // operacja po prostu się udała, więc zwracamy czyste HTTP 200 Ok
            return Ok();
        }
        /// <summary>
        /// Usuwa konkretny posiłek z dziennika.
        /// </summary>
        [HttpDelete("{date}/items/{itemId}")]
        public async Task<IActionResult> DeleteMealItem(
            [FromRoute] DateTime date, 
            [FromRoute] Guid itemId, 
            [FromQuery] Guid userId)
        {
            var command = new DeleteMealItemCommand(userId, date, itemId);
            
            await _mediator.Send(command);
            
            return Ok(new { Message = "Posiłek został usunięty, a kalorie przeliczone!" });
        }
        [HttpPost("items/barcode")]
        public async Task<IActionResult> AddMealByBarcode([FromBody] AddExternalMealItemCommand command)
        {
            // Handler zajmie się pobraniem z sieci, zapisaniem do bazy i dodaniem do dziennika
            await _mediator.Send(command);
            return Ok(new { Message = "Produkt pobrany z bazy zewnętrznej i dodany do dziennika!" });
        }
    }
}