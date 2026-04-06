using AviationFleet.Api.Models;

namespace AviationFleet.Api.Services;

public interface IDispatcherService
{
    /// <summary>
    /// Assigns the lowest-workload available mechanic with a matching certification and increments workload.
    /// Leaves the ticket open when no candidate exists.
    /// </summary>
    Task DispatchAsync(MaintenanceTicket ticket, CancellationToken cancellationToken = default);
}
