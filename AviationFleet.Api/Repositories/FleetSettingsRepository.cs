using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Api.Repositories;

public sealed class FleetSettingsRepository(AviationFleetDbContext db) : IFleetSettingsRepository
{
    public async Task<int> GetMaxMechanicActiveJobsAsync(CancellationToken cancellationToken = default)
    {
        var row = await db.FleetSettings.AsNoTracking().SingleOrDefaultAsync(cancellationToken);
        return row?.MaxMechanicActiveJobs ?? 3;
    }

    public async Task<FleetSettings> GetTrackedAsync(CancellationToken cancellationToken = default) =>
        await db.FleetSettings.SingleAsync(cancellationToken);
}
