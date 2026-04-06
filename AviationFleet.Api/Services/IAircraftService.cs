using AviationFleet.Api.Dtos;

namespace AviationFleet.Api.Services;

public interface IAircraftService
{
    Task<IReadOnlyList<AircraftResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AircraftResponseDto?> CreateAsync(AircraftCreateDto dto, CancellationToken cancellationToken = default);

    Task<UpdateFlightHoursResultDto?> UpdateFlightHoursAsync(
        Guid id,
        UpdateAircraftFlightHoursDto dto,
        CancellationToken cancellationToken = default);

    Task<AircraftResponseDto?> UpdateLocationAsync(
        Guid id,
        UpdateAircraftLocationDto dto,
        CancellationToken cancellationToken = default);

    Task<AircraftResponseDto?> UpdateOperationalStatusAsync(
        Guid id,
        UpdateAircraftOperationalStatusDto dto,
        CancellationToken cancellationToken = default);

    Task<AircraftResponseDto?> ConfirmMaintenanceCompleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
