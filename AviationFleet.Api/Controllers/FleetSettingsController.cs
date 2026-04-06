using AviationFleet.Api.Dtos;
using AviationFleet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AviationFleet.Api.Controllers;

[ApiController]
[Route("api/settings/fleet")]
public sealed class FleetSettingsController(IFleetSettingsService fleetSettingsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(FleetSettingsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var dto = await fleetSettingsService.GetAsync(cancellationToken);
        return Ok(dto);
    }

    [HttpPut]
    [ProducesResponseType(typeof(FleetSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromBody] FleetSettingsDto dto, CancellationToken cancellationToken)
    {
        var updated = await fleetSettingsService.SetMaxMechanicJobsAsync(dto.MaxMechanicActiveJobs, cancellationToken);
        return updated is null
            ? BadRequest(new { error = "MaxMechanicActiveJobs must be between 1 and 100." })
            : Ok(updated);
    }
}
