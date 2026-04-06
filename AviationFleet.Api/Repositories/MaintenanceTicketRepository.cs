using AviationFleet.Api.Data;
using AviationFleet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AviationFleet.Api.Repositories;

public sealed class MaintenanceTicketRepository(AviationFleetDbContext db) : IMaintenanceTicketRepository
{
    public void Add(MaintenanceTicket ticket) => db.MaintenanceTickets.Add(ticket);

    public async Task<IReadOnlyList<MaintenanceTicket>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await db.MaintenanceTickets
            .AsNoTracking()
            .Include(t => t.Aircraft)
            .Include(t => t.Mechanic)
            .OrderByDescending(t => t.DateCreated)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasActiveTicketForAircraftAsync(Guid aircraftId, CancellationToken cancellationToken = default) =>
        await db.MaintenanceTickets.AnyAsync(
            t => t.AircraftId == aircraftId
                 && t.Status != MaintenanceTicketStatus.Completed,
            cancellationToken);

    public async Task<IReadOnlyList<MaintenanceTicket>> GetActiveTicketsForAircraftAsync(
        Guid aircraftId,
        CancellationToken cancellationToken = default) =>
        await db.MaintenanceTickets
            .Where(t => t.AircraftId == aircraftId && t.Status != MaintenanceTicketStatus.Completed)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MaintenanceTicket>> GetTicketsAssignedToMechanicAsync(
        Guid mechanicId,
        CancellationToken cancellationToken = default) =>
        await db.MaintenanceTickets
            .Where(t => t.MechanicId == mechanicId)
            .ToListAsync(cancellationToken);
}
