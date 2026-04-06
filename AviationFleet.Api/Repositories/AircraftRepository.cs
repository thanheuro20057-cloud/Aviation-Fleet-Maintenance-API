using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Api.Repositories;

public sealed class AircraftRepository(AviationFleetDbContext db) : IAircraftRepository
{
    public async Task<IReadOnlyList<Aircraft>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Aircraft
            .AsNoTracking()
            .Include(a => a.Parts)
            .OrderBy(a => a.TailNumber)
            .ToListAsync(cancellationToken);

    public async Task<Aircraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Aircraft
            .Include(a => a.Parts)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public void Add(Aircraft aircraft) => db.Aircraft.Add(aircraft);

    public void Remove(Aircraft aircraft) => db.Aircraft.Remove(aircraft);
}
