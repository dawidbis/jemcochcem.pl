using FitApp.Application.Features.Diet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitApp.Infrastructure.Interfaces;

namespace FitApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeasurementsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IBodyMeasurementRepository _measurementRepository;

    public MeasurementsController(IMediator mediator, IBodyMeasurementRepository measurementRepository)
    {
        _mediator = mediator;
        _measurementRepository = measurementRepository;
    }

    // Istniejący POST (przez Mediator)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { Id = id });
    }

    // NOWY: Pobieranie historii dla konkretnego użytkownika
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetHistory(Guid userId)
    {
        var history = await _measurementRepository.GetUserHistoryAsync(userId);
        
        // Mapujemy na DTO, żeby nie zwracać surowych encji z bazy
        var result = history.Select(m => new {
            m.Id,
            m.Weight,
            m.Date
        }).OrderByDescending(m => m.Date);

        return Ok(result);
    }

    // NOWY: Usuwanie błędnego wpisu
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var measurement = await _measurementRepository.GetByIdAsync(id);
        if (measurement == null) return NotFound();

        await _measurementRepository.RemoveAsync(measurement);
        return NoContent();
    }
}