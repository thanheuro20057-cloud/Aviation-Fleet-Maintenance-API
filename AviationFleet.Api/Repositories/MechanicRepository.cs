using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Api.Repositories;

public sealed class MechanicRepository(AviationFleetDbContext db) : IMechanicRepository
{
    public async Task<IReadOnlyList<Mechanic>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Mechanics.AsNoTracking().OrderBy(m => m.Name).ToListAsync(cancellationToken);

    public async Task<Mechanic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Mechanics.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Mechanic?> FindBestDispatchCandidateAsync(
        CertificationType requiredCertification,
        int maxActiveJobsExclusive,
        CancellationToken cancellationToken = default) =>
        await db.Mechanics
            .Where(m => m.IsAvailable
                        && m.Certification == requiredCertification
                        && m.CurrentWorkload < maxActiveJobsExclusive)
            .OrderBy(m => m.CurrentWorkload)
            .ThenBy(m => m.Name)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> ExistsByNameNormalizedAsync(string nameNormalized, CancellationToken cancellationToken = default) =>
        await db.Mechanics.AnyAsync(m => m.NameNormalized == nameNormalized, cancellationToken);

    public void Add(Mechanic mechanic) => db.Mechanics.Add(mechanic);

    public void Remove(Mechanic mechanic) => db.Mechanics.Remove(mechanic);
}
