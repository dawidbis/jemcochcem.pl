namespace FitApp.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitApp.Application.Features.Diet;
using System.Threading.Tasks;

[ApiController]
[Route("diary")] 
public class DiaryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiaryController(IMediator mediator) => _mediator = mediator;

    [HttpPost] // POST na /diary
    public async Task<IActionResult> AddItem([FromBody] AddMealItemCommand cmd)
    {
        await _mediator.Send(cmd);
        return Ok("Diary OK");
    }
}