using AviationFleet.Api.Models;

namespace AviationFleet.Api.Repositories;

public interface IMaintenanceTicketRepository
{
    void Add(MaintenanceTicket ticket);

    Task<IReadOnlyList<MaintenanceTicket>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);

    Task<bool> HasActiveTicketForAircraftAsync(Guid aircraftId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MaintenanceTicket>> GetActiveTicketsForAircraftAsync(
        Guid aircraftId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MaintenanceTicket>> GetTicketsAssignedToMechanicAsync(
        Guid mechanicId,
        CancellationToken cancellationToken = default);
}
