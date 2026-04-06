using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;

namespace AviationFleet.Api.Services;

public sealed class DispatcherService(IMechanicRepository mechanicRepository, IFleetSettingsRepository fleetSettings)
    : IDispatcherService
{
    public async Task DispatchAsync(MaintenanceTicket ticket, CancellationToken cancellationToken = default)
    {
        var max = await fleetSettings.GetMaxMechanicActiveJobsAsync(cancellationToken);
        var mechanic = await mechanicRepository.FindBestDispatchCandidateAsync(
            ticket.RequiredCertification,
            max,
            cancellationToken);

        if (mechanic is null)
            return;

        ticket.MechanicId = mechanic.Id;
        ticket.Status = MaintenanceTicketStatus.InProgress;
        mechanic.CurrentWorkload++;
    }
}
