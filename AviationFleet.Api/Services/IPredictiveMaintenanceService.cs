using AviationFleet.Api.Dtos;

namespace AviationFleet.Api.Services;

public interface IPredictiveMaintenanceService
{
    Task<IReadOnlyList<AircraftWarningDto>> GetWarningRosterAsync(CancellationToken cancellationToken = default);
}
