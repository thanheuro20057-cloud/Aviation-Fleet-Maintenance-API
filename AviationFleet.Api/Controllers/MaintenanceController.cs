using AviationFleet.Api.Dtos;
using AviationFleet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AviationFleet.Api.Controllers;

[ApiController]
[Route("api/maintenance")]
public sealed class MaintenanceController(
    IMaintenanceService maintenanceService,
    IPredictiveMaintenanceService predictiveMaintenanceService) : ControllerBase
{
    [HttpPost("ticket")]
    [ProducesResponseType(typeof(MaintenanceTicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateMaintenanceTicketDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.IssueDescription))
            return BadRequest(new { error = "IssueDescription is required." });

        var created = await maintenanceService.CreateTicketAsync(dto, cancellationToken);
        if (created is null)
            return NotFound(new { error = "Aircraft not found." });

        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet("tickets")]
    [ProducesResponseType(typeof(IReadOnlyList<MaintenanceTicketDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTickets(CancellationToken cancellationToken)
    {
        var rows = await maintenanceService.GetAllTicketsDetailedAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpGet("warnings")]
    [ProducesResponseType(typeof(IReadOnlyList<AircraftWarningDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWarnings(CancellationToken cancellationToken)
    {
        var roster = await predictiveMaintenanceService.GetWarningRosterAsync(cancellationToken);
        return Ok(roster);
    }
}
