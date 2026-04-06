using AviationFleet.Api.Models;

namespace AviationFleet.Api.Repositories;

public interface IFleetSettingsRepository
{
    Task<int> GetMaxMechanicActiveJobsAsync(CancellationToken cancellationToken = default);

    Task<FleetSettings> GetTrackedAsync(CancellationToken cancellationToken = default);
}
