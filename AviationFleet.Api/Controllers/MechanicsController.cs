using AviationFleet.Api.Dtos;
using AviationFleet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AviationFleet.Api.Controllers;

[ApiController]
[Route("api/mechanics")]
public sealed class MechanicsController(IMechanicService mechanicService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(MechanicResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] MechanicCreateDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { error = "Name is required." });

        var created = await mechanicService.CreateAsync(dto, cancellationToken);
        if (created is null)
            return BadRequest(new { error = "A mechanic with this name already exists." });

        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MechanicResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var rows = await mechanicService.GetAllAsync(cancellationToken);
        return Ok(rows);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var ok = await mechanicService.DeleteAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}
