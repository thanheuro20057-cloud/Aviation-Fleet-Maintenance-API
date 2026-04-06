using AviationFleet.Api.Models;

namespace AviationFleet.Api.Repositories;

public interface IMechanicRepository
{
    Task<IReadOnlyList<Mechanic>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Mechanic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Mechanic?> FindBestDispatchCandidateAsync(
        CertificationType requiredCertification,
        int maxActiveJobsExclusive,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameNormalizedAsync(string nameNormalized, CancellationToken cancellationToken = default);

    void Add(Mechanic mechanic);
    void Remove(Mechanic mechanic);
}
