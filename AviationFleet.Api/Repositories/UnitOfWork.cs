using AviationFleet.Api.Data;

namespace AviationFleet.Api.Repositories;

public sealed class UnitOfWork(AviationFleetDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
