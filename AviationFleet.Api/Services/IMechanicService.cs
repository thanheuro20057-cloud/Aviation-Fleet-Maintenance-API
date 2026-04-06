using AviationFleet.Api.Dtos;

namespace AviationFleet.Api.Services;

public interface IMechanicService
{
    Task<IReadOnlyList<MechanicResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MechanicResponseDto?> CreateAsync(MechanicCreateDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
