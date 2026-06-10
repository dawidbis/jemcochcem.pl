using FitApp.Application.Features.Diary.AddAiMealToDiary;
using FitApp.Application.Features.Diet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitApp.API.Controllers
{
    public class DiaryController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public DiaryController(IMediator mediator) => _mediator = mediator;

        [HttpGet("monthly-calendar")]
        public async Task<IActionResult> GetMonthlyCalendar([FromQuery] int year, [FromQuery] int month)
        {
            var result = await _mediator.Send(new GetMonthlyCalendarQuery(CurrentUserId, year, month));
            return Ok(result);
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetDailyDiary([FromRoute] DateTime date)
        {
            var result = await _mediator.Send(new GetDailyDiaryQuery { Date = date, UserId = CurrentUserId });
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddMealItem([FromBody] AddMealItemCommand command)
        {
            command.UserId = CurrentUserId;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("{date}/items/{itemId}")]
        public async Task<IActionResult> DeleteMealItem([FromRoute] DateTime date, [FromRoute] Guid itemId)
        {
            await _mediator.Send(new DeleteMealItemCommand(CurrentUserId, date, itemId));
            return Ok(new { Message = "Posiłek został usunięty, a kalorie przeliczone!" });
        }

        [HttpPost("items/barcode")]
        public async Task<IActionResult> AddMealByBarcode([FromBody] AddBarcodeRequest request)
        {
            await _mediator.Send(new AddExternalMealItemCommand(CurrentUserId, request.Date, request.Barcode, request.Grams));
            return Ok(new { Message = "Produkt pobrany z bazy zewnętrznej i dodany do dziennika!" });
        }

        [HttpPost("items/from-ai-plan")]
        public async Task<IActionResult> AddAiMealToDiary([FromBody] AddAiMealToDiaryRequest request)
        {
            var command = new AddAiMealToDiaryCommand(CurrentUserId, request.MealPlanItemId, request.Date);
            var newMealLogItemId = await _mediator.Send(command);
            return Ok(new { MealLogItemId = newMealLogItemId });
        }

        public record AddBarcodeRequest(DateTime Date, string Barcode, decimal Grams);

        public class AddAiMealToDiaryRequest
        {
            public Guid MealPlanItemId { get; set; }
            public DateTime Date { get; set; }
        }
    }
}