using AviationFleet.Api.Models;

namespace AviationFleet.Api.Repositories;

public interface IAircraftRepository
{
    Task<IReadOnlyList<Aircraft>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Aircraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Aircraft aircraft);
    void Remove(Aircraft aircraft);
}
