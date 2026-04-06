using AviationFleet.Api.Dtos;

namespace AviationFleet.Api.Services;

public interface IMaintenanceService
{
    Task<MaintenanceTicketResponseDto?> CreateTicketAsync(CreateMaintenanceTicketDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MaintenanceTicketDetailDto>> GetAllTicketsDetailedAsync(CancellationToken cancellationToken = default);
}
