using AviationFleet.Api.Dtos;
using AviationFleet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Api.Controllers;

[ApiController]
[Route("api/aircraft")]
public sealed class AircraftController(IAircraftService aircraftService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AircraftResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AircraftCreateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.TailNumber))
            return BadRequest(new { error = "TailNumber is required." });

        try
        {
            var created = await aircraftService.CreateAsync(dto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, created);
        }
        catch (DbUpdateException)
        {
            return BadRequest(new { error = "Unable to create aircraft (duplicate tail number?)." });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AircraftResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rows = await aircraftService.GetAllAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpPut("{id:guid}/hours")]
    [ProducesResponseType(typeof(UpdateFlightHoursResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHours(
        Guid id,
        [FromBody] UpdateAircraftFlightHoursDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await aircraftService.UpdateFlightHoursAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:guid}/location")]
    [ProducesResponseType(typeof(AircraftResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(
        Guid id,
        [FromBody] UpdateAircraftLocationDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await aircraftService.UpdateLocationAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:guid}/operational-status")]
    [ProducesResponseType(typeof(AircraftResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOperationalStatus(
        Guid id,
        [FromBody] UpdateAircraftOperationalStatusDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await aircraftService.UpdateOperationalStatusAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id:guid}/confirm-maintenance-complete")]
    [ProducesResponseType(typeof(AircraftResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmMaintenanceComplete(Guid id, CancellationToken cancellationToken)
    {
        var updated = await aircraftService.ConfirmMaintenanceCompleteAsync(id, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await aircraftService.DeleteAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
