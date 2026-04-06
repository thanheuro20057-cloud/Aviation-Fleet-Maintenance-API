using AviationFleet.Api.Dtos;

namespace AviationFleet.Api.Services;

public interface IFleetSettingsService
{
    Task<FleetSettingsDto> GetAsync(CancellationToken cancellationToken = default);

    Task<FleetSettingsDto?> SetMaxMechanicJobsAsync(int maxMechanicActiveJobs, CancellationToken cancellationToken = default);
}
