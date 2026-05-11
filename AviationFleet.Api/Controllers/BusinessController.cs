using AviationFleet.Api.Dtos;
using AviationFleet.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AviationFleet.Api.Controllers;

[ApiController]
[Route("api/business")]
public sealed class BusinessController(IBusinessAnalyticsService businessAnalyticsService) : ControllerBase
{
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(BusinessDashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await businessAnalyticsService.GetDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }
}
