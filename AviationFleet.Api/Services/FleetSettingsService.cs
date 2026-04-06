using AviationFleet.Api.Dtos;
using AviationFleet.Api.Repositories;

namespace AviationFleet.Api.Services;

public sealed class FleetSettingsService(IFleetSettingsRepository fleetSettingsRepository, IUnitOfWork unitOfWork)
    : IFleetSettingsService
{
    public async Task<FleetSettingsDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var max = await fleetSettingsRepository.GetMaxMechanicActiveJobsAsync(cancellationToken);
        return new FleetSettingsDto(max);
    }

    public async Task<FleetSettingsDto?> SetMaxMechanicJobsAsync(int maxMechanicActiveJobs, CancellationToken cancellationToken = default)
    {
        if (maxMechanicActiveJobs < 1 || maxMechanicActiveJobs > 100)
            return null;

        var row = await fleetSettingsRepository.GetTrackedAsync(cancellationToken);
        row.MaxMechanicActiveJobs = maxMechanicActiveJobs;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new FleetSettingsDto(row.MaxMechanicActiveJobs);
    }
}
