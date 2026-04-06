using AviationFleet.Api.Dtos;
using AviationFleet.Api.Models;
using AviationFleet.Api.Repositories;

namespace AviationFleet.Api.Services;

public sealed class PredictiveMaintenanceService(IAircraftRepository aircraftRepository) : IPredictiveMaintenanceService
{
    public async Task<IReadOnlyList<AircraftWarningDto>> GetWarningRosterAsync(CancellationToken cancellationToken = default)
    {
        var all = await aircraftRepository.GetAllAsync(cancellationToken);
        var warnings = new List<AircraftWarningDto>();

        foreach (var a in all)
        {
            if (a.OperationalStatus == AircraftOperationalStatus.Unavailable)
                continue;

            if (!PredictiveMaintenanceRules.IsOnWarningRoster(
                    a.TotalFlightHours,
                    a.HoursAtLastMaintenance,
                    a.MaintenanceThresholdHours))
                continue;

            var remaining = PredictiveMaintenanceRules.HoursRemainingUntilMaintenanceDue(
                a.TotalFlightHours,
                a.HoursAtLastMaintenance,
                a.MaintenanceThresholdHours);

            var message = remaining <= 0
                ? "Airframe maintenance interval reached or exceeded."
                : "Within predictive window (36 flight hours before due).";

            warnings.Add(new AircraftWarningDto(a.Id, a.TailNumber, remaining, message));
        }

        return warnings;
    }
}
